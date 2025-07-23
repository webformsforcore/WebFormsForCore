using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Threading;
using System.Web;
using System.Web.UI.WebControls;
using Microsoft.ReportingServices.Common;

namespace Microsoft.Reporting.WebForms;

internal class CalendarPageControl : CompositeControl
{
	public const string _CalendarStyleSheet = "Microsoft.Reporting.WebForms.calendar.css";

	public const string _CalendarScript = "Microsoft.Reporting.WebForms.datepicker.js";

	public const string _CalendarImage = "Microsoft.Reporting.WebForms.calendar.gif";

	public const string _CalenderDisabledImage = "Microsoft.Reporting.WebForms.calendar_disabled.gif";

	public const string _calendarStartDateAttribute = "date";

	public const string _calendarSelectedDateAttribute = "selectDate";

	public const string _PreviousMove = "Previous";

	public const string _LCIDParameter = "LCID";

	public static readonly TimeSpan _Midnight = new TimeSpan(0, 0, 0);

	private SPDatePickerControl m_calendar;

	private string m_onPageLoadScript = "PositionFrame('DatePickerDiv');";

	private string m_calendarScriptUrl;

	private string m_cssUrl;

	private string m_imageUrl;

	public string OnPageLoadScript
	{
		get
		{
			EnsureChildControls();
			return m_calendar.RemoveLoadingScript;
		}
	}

	public string CalendarScriptUrl
	{
		get
		{
			return m_calendarScriptUrl;
		}
		set
		{
			m_calendarScriptUrl = value;
		}
	}

	public string CssUrl
	{
		get
		{
			return m_cssUrl;
		}
		set
		{
			m_cssUrl = value;
		}
	}

	public string ImageUrl
	{
		get
		{
			return m_imageUrl;
		}
		set
		{
			m_imageUrl = value;
		}
	}

	protected override void CreateChildControls()
	{
		Controls.Clear();
		DateTime now = DateTime.Now;
		CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
		m_calendar = new SPDatePickerControl();
		m_calendar.EndOffset = 6;
		m_calendar.StartOffset = -6;
		m_calendar.TimeZone = TimeZone.CurrentTimeZone.GetUtcOffset(now);
		m_calendar.LocaleId = currentCulture.LCID;
		m_calendar.ShowFooter = true;
		m_calendar.FirstDayOfWeek = (int)currentCulture.DateTimeFormat.FirstDayOfWeek;
		Controls.Add(m_calendar);
	}

	protected override void OnLoad(EventArgs e)
	{
		EnsureChildControls();
		if (CalendarScriptUrl == null || CssUrl == null || ImageUrl == null)
		{
			throw new ArgumentException("Missing one of the url properties");
		}
		Page.ClientScript.RegisterClientScriptInclude("CalendarScriptInclude", CalendarScriptUrl);
		NameValueCollection queryString = HttpContext.Current.Request.QueryString;
		string text = queryString.Get("date");
		string text2 = queryString.Get("selectDate");
		string s = queryString.Get("LCID");
		string value = queryString.Get("Previous");
		if (!string.IsNullOrEmpty(value))
		{
			m_onPageLoadScript += string.Format(CultureInfo.InvariantCulture, "try{{document.getElementById('{0}').focus();}}catch (e){{}}", "DatePickerDivMovePrevious");
		}
		else
		{
			m_onPageLoadScript += string.Format(CultureInfo.InvariantCulture, "try{{document.getElementById('{0}').focus();}}catch (e){{}}", "DatePickerDivMoveNext");
		}
		if (int.TryParse(s, out var result))
		{
			m_calendar.LocaleId = result;
			CultureInfo cultureInfo = new CultureInfo(result, useUserOverride: false);
			Thread.CurrentThread.CurrentCulture = cultureInfo;
			Thread.CurrentThread.CurrentUICulture = cultureInfo;
		}
		if (text == null || text == string.Empty)
		{
			text = text2;
		}
		string text3 = "";
		DateTimeOffset dateTimeOffset = default(DateTimeOffset);
		bool flag = default(bool);
		if (!string.IsNullOrEmpty(text) && DateTimeUtil.TryParseDateTime(text, CultureInfo.CurrentCulture, ref dateTimeOffset, ref flag))
		{
			m_calendar.StartMonth = dateTimeOffset.Date.ToShortDateString();
			if (flag || dateTimeOffset.TimeOfDay != _Midnight)
			{
				text3 = dateTimeOffset.TimeOfDay.ToString();
			}
			if (flag)
			{
				text3 = text3 + " " + dateTimeOffset.ToString("zzz", CultureInfo.CurrentCulture);
			}
		}
		string text4 = "var timePortion = ";
		text4 = ((!string.IsNullOrEmpty(text3)) ? (text4 + "' " + text3 + "';") : (text4 + "null;"));
		Page.ClientScript.RegisterClientScriptBlock(typeof(CalendarPageControl), "time", text4, addScriptTags: true);
		if (!string.IsNullOrEmpty(text2) && DateTime.TryParse(text2, out var result2))
		{
			m_calendar.SelectedDate = result2.ToShortDateString();
		}
		m_calendar._urlCssClass = CssUrl;
		m_calendar.ImageUrl = ImageUrl;
		base.OnLoad(e);
	}
}
