
using Microsoft.ReportingServices.Common;
using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
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
        this.EnsureChildControls();
        return this.m_calendar.RemoveLoadingScript;
      }
    }

    protected override void CreateChildControls()
    {
      this.Controls.Clear();
      DateTime now = DateTime.Now;
      CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
      this.m_calendar = new SPDatePickerControl();
      this.m_calendar.EndOffset = 6;
      this.m_calendar.StartOffset = -6;
      this.m_calendar.TimeZone = TimeZone.CurrentTimeZone.GetUtcOffset(now);
      this.m_calendar.LocaleId = currentCulture.LCID;
      this.m_calendar.ShowFooter = true;
      this.m_calendar.FirstDayOfWeek = (int) currentCulture.DateTimeFormat.FirstDayOfWeek;
      this.Controls.Add((Control) this.m_calendar);
    }

    public string CalendarScriptUrl
    {
      get => this.m_calendarScriptUrl;
      set => this.m_calendarScriptUrl = value;
    }

    public string CssUrl
    {
      get => this.m_cssUrl;
      set => this.m_cssUrl = value;
    }

    public string ImageUrl
    {
      get => this.m_imageUrl;
      set => this.m_imageUrl = value;
    }

    protected override void OnLoad(EventArgs e)
    {
      this.EnsureChildControls();
      if (this.CalendarScriptUrl == null || this.CssUrl == null || this.ImageUrl == null)
        throw new ArgumentException("Missing one of the url properties");
      this.Page.ClientScript.RegisterClientScriptInclude("CalendarScriptInclude", this.CalendarScriptUrl);
      NameValueCollection queryString = HttpContext.Current.Request.QueryString;
      string str1 = queryString.Get("date");
      string s1 = queryString.Get("selectDate");
      string s2 = queryString.Get("LCID");
      if (!string.IsNullOrEmpty(queryString.Get("Previous")))
        this.m_onPageLoadScript += string.Format((IFormatProvider) CultureInfo.InvariantCulture, "try{{document.getElementById('{0}').focus();}}catch (e){{}}", (object) "DatePickerDivMovePrevious");
      else
        this.m_onPageLoadScript += string.Format((IFormatProvider) CultureInfo.InvariantCulture, "try{{document.getElementById('{0}').focus();}}catch (e){{}}", (object) "DatePickerDivMoveNext");
      int result1;
      if (int.TryParse(s2, out result1))
      {
        this.m_calendar.LocaleId = result1;
        CultureInfo cultureInfo = new CultureInfo(result1, false);
        Thread.CurrentThread.CurrentCulture = cultureInfo;
        Thread.CurrentThread.CurrentUICulture = cultureInfo;
      }
      if (str1 == null || str1 == string.Empty)
        str1 = s1;
      string str2 = "";
      DateTimeOffset dateTimeOffset;
      bool flag;
      if (!string.IsNullOrEmpty(str1) && DateTimeUtil.TryParseDateTime(str1, CultureInfo.CurrentCulture, ref dateTimeOffset, ref flag))
      {
        this.m_calendar.StartMonth = dateTimeOffset.Date.ToShortDateString();
        if (flag || dateTimeOffset.TimeOfDay != CalendarPageControl._Midnight)
          str2 = dateTimeOffset.TimeOfDay.ToString();
        if (flag)
          str2 = str2 + " " + dateTimeOffset.ToString("zzz", (IFormatProvider) CultureInfo.CurrentCulture);
      }
      string str3 = "var timePortion = ";
      this.Page.ClientScript.RegisterClientScriptBlock(typeof (CalendarPageControl), "time", !string.IsNullOrEmpty(str2) ? str3 + "' " + str2 + "';" : str3 + "null;", true);
      DateTime result2;
      if (!string.IsNullOrEmpty(s1) && DateTime.TryParse(s1, out result2))
        this.m_calendar.SelectedDate = result2.ToShortDateString();
      this.m_calendar._urlCssClass = this.CssUrl;
      this.m_calendar.ImageUrl = this.ImageUrl;
      base.OnLoad(e);
    }
  }
}
