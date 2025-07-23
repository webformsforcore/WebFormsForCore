using System;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.UI;

namespace Microsoft.Reporting.WebForms;

[DefaultProperty("SelectedDate")]
internal class SPDatePickerControl : Control
{
	protected DateOptions _dateOptions;

	internal DatePicker _datePicker;

	private int _lcid = 1033;

	private TimeSpan _timezone = TimeSpan.MinValue;

	private SPCalendarType _calendar = SPCalendarType.Gregorian;

	private string _ww = "0111110";

	private int _fdow;

	private int _hj;

	public string _urlCssClass;

	private short _firstWeekOfYear;

	private int _tdym = -1;

	private string _imageDirName;

	private int _langid = Thread.CurrentThread.CurrentUICulture.LCID;

	public string StartMonth
	{
		get
		{
			object obj = ViewState["StartMonth"];
			if (obj != null)
			{
				return (string)obj;
			}
			return string.Empty;
		}
		set
		{
			ViewState["StartMonth"] = value;
		}
	}

	[Description(" Use short format of DateTime")]
	[Category("Data")]
	[Bindable(true)]
	[DefaultValue("")]
	public string SelectedDate
	{
		get
		{
			object obj = ViewState["SelectedDate"];
			if (obj != null)
			{
				return (string)obj;
			}
			return string.Empty;
		}
		set
		{
			ViewState["SelectedDate"] = value;
		}
	}

	[Category("Data")]
	[Bindable(true)]
	[DefaultValue(1033)]
	public int LocaleId
	{
		get
		{
			return _lcid;
		}
		set
		{
			_lcid = value;
		}
	}

	public int LangId
	{
		get
		{
			return _langid;
		}
		set
		{
			_langid = value;
		}
	}

	[Description("Difference between UTC and local time")]
	[Bindable(true)]
	[Category("Data")]
	public TimeSpan TimeZone
	{
		get
		{
			return _timezone;
		}
		set
		{
			_timezone = value;
		}
	}

	[DefaultValue("1")]
	[Bindable(true)]
	[Category("Data")]
	public SPCalendarType Calendar
	{
		get
		{
			return _calendar;
		}
		set
		{
			_calendar = value;
		}
	}

	[Bindable(true)]
	[Category("Data")]
	[DefaultValue("0111110")]
	public string WorkWeek
	{
		get
		{
			return _ww;
		}
		set
		{
			if (SPUtility.IsValidStringInput("[01]{7}", value))
			{
				_ww = value;
			}
		}
	}

	[Description("Valid values: from 0 to 6.")]
	[Category("Data")]
	[Bindable(true)]
	[DefaultValue("0")]
	public int FirstDayOfWeek
	{
		get
		{
			return _fdow;
		}
		set
		{
			if (value >= 0 && value < 7)
			{
				_fdow = value;
			}
		}
	}

	[Bindable(true)]
	[Category("Data")]
	[Description("Valid values: from -3 to 3.")]
	[DefaultValue("0")]
	public int HijriAdjustment
	{
		get
		{
			return _hj;
		}
		set
		{
			if (value > -3 && value < 3)
			{
				_hj = value;
			}
		}
	}

	[Description("Valid values: from 0 to 2.")]
	[Category("Data")]
	[Bindable(true)]
	[DefaultValue("0")]
	public short FirstWeekOfYear
	{
		get
		{
			return _firstWeekOfYear;
		}
		set
		{
			if (value > -1 && value < 3)
			{
				_firstWeekOfYear = value;
			}
		}
	}

	[DefaultValue(true)]
	[Bindable(true)]
	[Category("Visibility")]
	public bool ShowNotThisMonthDays
	{
		get
		{
			InitDatePicker();
			return _datePicker.ShowNotThisMonthDays;
		}
		set
		{
			InitDatePicker();
			_datePicker.ShowNotThisMonthDays = value;
		}
	}

	[Category("Visibility")]
	[DefaultValue(true)]
	[Bindable(true)]
	public bool ShowFooter
	{
		get
		{
			InitDatePicker();
			return _datePicker.ShowFooter;
		}
		set
		{
			InitDatePicker();
			_datePicker.ShowFooter = value;
		}
	}

	[DefaultValue(false)]
	[Category("Visibility")]
	[Bindable(true)]
	public bool ShowWeekNumber
	{
		get
		{
			InitDatePicker();
			return _datePicker.ShowWeekNumber;
		}
		set
		{
			InitDatePicker();
			_datePicker.ShowWeekNumber = value;
		}
	}

	[Bindable(true)]
	[DefaultValue(true)]
	[Category("Visibility")]
	public bool ShowNextPrevNavigation
	{
		get
		{
			InitDatePicker();
			return _datePicker.ShowNextPrevNavigation;
		}
		set
		{
			InitDatePicker();
			_datePicker.ShowNextPrevNavigation = value;
		}
	}

	[DefaultValue(-1)]
	[Bindable(true)]
	[Category("Picker")]
	[Description("Value betweeen -12 and 0")]
	public int StartOffset
	{
		get
		{
			InitDatePicker();
			return _datePicker.StartMonthOffset;
		}
		set
		{
			InitDatePicker();
			if (value > -13 && value <= 0)
			{
				_datePicker.StartMonthOffset = value;
			}
		}
	}

	[Bindable(true)]
	[DefaultValue(3)]
	[Category("Picker")]
	[Description("Value betweeen 0 and 12")]
	public int EndOffset
	{
		get
		{
			InitDatePicker();
			return _datePicker.EndMonthOffset;
		}
		set
		{
			InitDatePicker();
			if (value >= 0 && value < 13)
			{
				_datePicker.EndMonthOffset = value;
			}
		}
	}

	[DefaultValue(-1)]
	[Bindable(true)]
	[Category("Picker")]
	[Description("")]
	public int TwoDigitYearMax
	{
		get
		{
			return _tdym;
		}
		set
		{
			if (value >= 0)
			{
				_tdym = value;
			}
		}
	}

	[DefaultValue(0)]
	[Category("Picker")]
	[Description("Full or web relative path to images location. ")]
	[Bindable(true)]
	public string ImageUrl
	{
		set
		{
			_imageDirName = value;
		}
	}

	[DefaultValue("")]
	[Bindable(true)]
	[Description("Name of javascipt function used as onClick event handler. ")]
	[Category("Picker")]
	public string OnClickScriptHandler
	{
		get
		{
			return _datePicker.OnClickScriptHandler;
		}
		set
		{
			_datePicker.OnClickScriptHandler = value;
		}
	}

	public int MinJDay
	{
		get
		{
			InitDatePicker();
			return _datePicker.MinJDay;
		}
		set
		{
			InitDatePicker();
			_datePicker.MinJDay = value;
		}
	}

	public int MaxJDay
	{
		get
		{
			InitDatePicker();
			return _datePicker.MaxJDay;
		}
		set
		{
			InitDatePicker();
			_datePicker.MaxJDay = value;
		}
	}

	internal string RemoveLoadingScript => "HideUnhide('LoadingDiv', 'DatePickerDiv', g_currentID, null);PositionFrame('DatePickerDiv');";

	protected virtual void InitDatePicker()
	{
		if (_datePicker == null)
		{
			_datePicker = new DatePicker();
		}
	}

	protected override void Render(HtmlTextWriter output)
	{
		_dateOptions = new DateOptions(_lcid.ToString(CultureInfo.InvariantCulture), _calendar, _ww, _fdow.ToString(CultureInfo.InvariantCulture), _hj.ToString(CultureInfo.InvariantCulture), (_timezone != TimeSpan.MinValue) ? _timezone.ToString() : null, (_tdym == -1) ? null : _tdym.ToString(CultureInfo.InvariantCulture), SelectedDate, StartMonth);
		InitDatePicker();
		_datePicker.DateTimeOptions = _dateOptions;
		_datePicker.LangId = LangId;
		output.Write("<div class='ms-rs-calendar-loading' id='LoadingDiv' style='width:100%;height:100%'>");
		output.Write(LocalizationHelper.Current.CalendarLoading);
		output.Write("</div>");
		if (_urlCssClass != null)
		{
			string value = "<LINK REL=\"stylesheet\" TYPE=\"text/css\" HREF=\"" + SPHttpUtility.HtmlUrlAttributeEncode(_urlCssClass.ToString()) + "\">";
			output.Write(value);
		}
		_ = HttpContext.Current.Request.Url;
		_datePicker.ImageDirName = _imageDirName;
		_datePicker.FirstWeekOfYear = _firstWeekOfYear;
		StringBuilder stringBuilder = new StringBuilder();
		_datePicker.RenderAsHtml(stringBuilder);
		output.Write(stringBuilder.ToString());
	}
}
