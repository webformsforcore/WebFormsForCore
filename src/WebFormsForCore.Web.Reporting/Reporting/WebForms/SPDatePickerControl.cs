
using System;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.UI;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
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
      get => (string) this.ViewState[nameof (StartMonth)] ?? string.Empty;
      set => this.ViewState[nameof (StartMonth)] = (object) value;
    }

    [Description(" Use short format of DateTime")]
    [Category("Data")]
    [Bindable(true)]
    [DefaultValue("")]
    public string SelectedDate
    {
      get => (string) this.ViewState[nameof (SelectedDate)] ?? string.Empty;
      set => this.ViewState[nameof (SelectedDate)] = (object) value;
    }

    [Category("Data")]
    [Bindable(true)]
    [DefaultValue(1033)]
    public int LocaleId
    {
      get => this._lcid;
      set => this._lcid = value;
    }

    public int LangId
    {
      get => this._langid;
      set => this._langid = value;
    }

    [Description("Difference between UTC and local time")]
    [Bindable(true)]
    [Category("Data")]
    public TimeSpan TimeZone
    {
      get => this._timezone;
      set => this._timezone = value;
    }

    [DefaultValue("1")]
    [Bindable(true)]
    [Category("Data")]
    public SPCalendarType Calendar
    {
      get => this._calendar;
      set => this._calendar = value;
    }

    [Bindable(true)]
    [Category("Data")]
    [DefaultValue("0111110")]
    public string WorkWeek
    {
      get => this._ww;
      set
      {
        if (!SPUtility.IsValidStringInput("[01]{7}", value))
          return;
        this._ww = value;
      }
    }

    [Description("Valid values: from 0 to 6.")]
    [Category("Data")]
    [Bindable(true)]
    [DefaultValue("0")]
    public int FirstDayOfWeek
    {
      get => this._fdow;
      set
      {
        if (value < 0 || value >= 7)
          return;
        this._fdow = value;
      }
    }

    [Bindable(true)]
    [Category("Data")]
    [Description("Valid values: from -3 to 3.")]
    [DefaultValue("0")]
    public int HijriAdjustment
    {
      get => this._hj;
      set
      {
        if (value <= -3 || value >= 3)
          return;
        this._hj = value;
      }
    }

    [Description("Valid values: from 0 to 2.")]
    [Category("Data")]
    [Bindable(true)]
    [DefaultValue("0")]
    public short FirstWeekOfYear
    {
      get => this._firstWeekOfYear;
      set
      {
        if (value <= (short) -1 || value >= (short) 3)
          return;
        this._firstWeekOfYear = value;
      }
    }

    [DefaultValue(true)]
    [Bindable(true)]
    [Category("Visibility")]
    public bool ShowNotThisMonthDays
    {
      get
      {
        this.InitDatePicker();
        return this._datePicker.ShowNotThisMonthDays;
      }
      set
      {
        this.InitDatePicker();
        this._datePicker.ShowNotThisMonthDays = value;
      }
    }

    [Category("Visibility")]
    [DefaultValue(true)]
    [Bindable(true)]
    public bool ShowFooter
    {
      get
      {
        this.InitDatePicker();
        return this._datePicker.ShowFooter;
      }
      set
      {
        this.InitDatePicker();
        this._datePicker.ShowFooter = value;
      }
    }

    [DefaultValue(false)]
    [Category("Visibility")]
    [Bindable(true)]
    public bool ShowWeekNumber
    {
      get
      {
        this.InitDatePicker();
        return this._datePicker.ShowWeekNumber;
      }
      set
      {
        this.InitDatePicker();
        this._datePicker.ShowWeekNumber = value;
      }
    }

    [Bindable(true)]
    [DefaultValue(true)]
    [Category("Visibility")]
    public bool ShowNextPrevNavigation
    {
      get
      {
        this.InitDatePicker();
        return this._datePicker.ShowNextPrevNavigation;
      }
      set
      {
        this.InitDatePicker();
        this._datePicker.ShowNextPrevNavigation = value;
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
        this.InitDatePicker();
        return this._datePicker.StartMonthOffset;
      }
      set
      {
        this.InitDatePicker();
        if (value <= -13 || value > 0)
          return;
        this._datePicker.StartMonthOffset = value;
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
        this.InitDatePicker();
        return this._datePicker.EndMonthOffset;
      }
      set
      {
        this.InitDatePicker();
        if (value < 0 || value >= 13)
          return;
        this._datePicker.EndMonthOffset = value;
      }
    }

    [DefaultValue(-1)]
    [Bindable(true)]
    [Category("Picker")]
    [Description("")]
    public int TwoDigitYearMax
    {
      get => this._tdym;
      set
      {
        if (value < 0)
          return;
        this._tdym = value;
      }
    }

    [DefaultValue(0)]
    [Category("Picker")]
    [Description("Full or web relative path to images location. ")]
    [Bindable(true)]
    public string ImageUrl
    {
      set => this._imageDirName = value;
    }

    [DefaultValue("")]
    [Bindable(true)]
    [Description("Name of javascipt function used as onClick event handler. ")]
    [Category("Picker")]
    public string OnClickScriptHandler
    {
      get => this._datePicker.OnClickScriptHandler;
      set => this._datePicker.OnClickScriptHandler = value;
    }

    public int MinJDay
    {
      get
      {
        this.InitDatePicker();
        return this._datePicker.MinJDay;
      }
      set
      {
        this.InitDatePicker();
        this._datePicker.MinJDay = value;
      }
    }

    public int MaxJDay
    {
      get
      {
        this.InitDatePicker();
        return this._datePicker.MaxJDay;
      }
      set
      {
        this.InitDatePicker();
        this._datePicker.MaxJDay = value;
      }
    }

    protected virtual void InitDatePicker()
    {
      if (this._datePicker != null)
        return;
      this._datePicker = new DatePicker();
    }

    protected override void Render(HtmlTextWriter output)
    {
      this._dateOptions = new DateOptions(this._lcid.ToString((IFormatProvider) CultureInfo.InvariantCulture), this._calendar, this._ww, this._fdow.ToString((IFormatProvider) CultureInfo.InvariantCulture), this._hj.ToString((IFormatProvider) CultureInfo.InvariantCulture), this._timezone != TimeSpan.MinValue ? this._timezone.ToString() : (string) null, this._tdym == -1 ? (string) null : this._tdym.ToString((IFormatProvider) CultureInfo.InvariantCulture), this.SelectedDate, this.StartMonth);
      this.InitDatePicker();
      this._datePicker.DateTimeOptions = this._dateOptions;
      this._datePicker.LangId = this.LangId;
      output.Write("<div class='ms-rs-calendar-loading' id='LoadingDiv' style='width:100%;height:100%'>");
      output.Write(LocalizationHelper.Current.CalendarLoading);
      output.Write("</div>");
      if (this._urlCssClass != null)
      {
        string str = "<LINK REL=\"stylesheet\" TYPE=\"text/css\" HREF=\"" + SPHttpUtility.HtmlUrlAttributeEncode(this._urlCssClass.ToString()) + "\">";
        output.Write(str);
      }
      Uri url = HttpContext.Current.Request.Url;
      this._datePicker.ImageDirName = this._imageDirName;
      this._datePicker.FirstWeekOfYear = this._firstWeekOfYear;
      StringBuilder st = new StringBuilder();
      this._datePicker.RenderAsHtml(st);
      output.Write(st.ToString());
    }

    internal string RemoveLoadingScript
    {
      get
      {
        return "HideUnhide('LoadingDiv', 'DatePickerDiv', g_currentID, null);PositionFrame('DatePickerDiv');";
      }
    }
  }
}
