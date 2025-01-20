
using System;
using System.Globalization;
using System.Text;
using System.Threading;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal class DatePicker
  {
    public const string _PickBackImage = "Microsoft.Reporting.WebForms.pickback.gif";
    public const string _PickForwardImage = "Microsoft.Reporting.WebForms.pickforward.gif";
    private const string m_sRTL = "RTL";
    protected const string m_divopen = "\r<div id={0} style=\"display:{1} {2} \" >\r";
    protected const string m_dirrtl = "; direction:rtl ";
    protected const string m_dirltr = "; direction:ltr ";
    protected const string m_divclose = "</div>\r";
    protected const string m_tableopen_begin = "<table cellpadding=0 cellspacing=0 border=0 class=\"{0}\">\r";
    protected const string m_tableclose = "</table>\r";
    protected const string m_emptyformatTD = "<td>&nbsp;</td>";
    protected const string m_cellformatTD = "<td class={0} onmouseover=\"this.className='{1}';\" onmouseout=\"this.className='{0}';\"  onclick=\"javascript:{5}('{2}')\" {6} ><a onclick=\"event.cancelBubble=true;\" href=\"javascript:{5}('{2}')\" id=\"{3}\" >{4}</a></td>\r";
    protected const string m_cellformatTDDisabled = "<td class={0}>{4}</td>\r";
    protected const string m_nextprevformat = " <td align=center><a id=\"{3}\" href=\"javascript:{0}\" accesskey=\"{2}\">{1}</a></td>\r";
    protected const string m_opendivs = "<div class='ms-datepickerouter'> <div class='ms-quickLaunch' style='width:100%'>";
    protected const string m_closedivs = "</div></div>";
    protected const string m_firstrowformat = "<div class='ms-picker-header'><table cellpadding=0 cellspacing=0 border=0 class=\"{4}\" ><tr>{0}{1}{2}</tr></table></div><div>";
    protected const string m_hideunhideformat = "HideUnhide('{0}','{1}','{2}', '{3}');";
    protected const string m_movedateformat = "ShowLoading();MoveToDate('{0}', {1});";
    protected const string m_dayabbr = " \"{0}\" ";
    private const string m_weeknumberformatTD = "<th scope=\"row\" class={0} onmouseover=\"this.className='{1}';\" onmouseout=\"this.className='{0}';\">{2}</td>\r";
    private const string m_weeknumberselectedLink = "<div class={0}><a href=\"javascript:MoveToDate('{2}', false)\">{1}</a></div>";
    private const string m_weeknumberacronym = "<ACRONYM title=\"{0}\" >{1}</ACRONYM>";
    private const string m_monthformat = " <td align=center class={0}  nowrap >{1}</td>\r";
    private const string m_weekdayheader = " <th scope=\"col\" class={0} nowrap><ABBR title={3}>&nbsp;{1}{2}</ABBR></th>\r";
    private const string m_weekempty = "<td rowspan={0} width=2></td>";
    protected int m_StartMonthOffset = -1;
    protected int m_EndMonthOffset = 3;
    protected DateOptions m_dopt;
    protected SimpleDate m_startMonth;
    protected SimpleDate m_SelectedDate;
    protected SimpleDate m_Today;
    protected SimpleDate[] m_MonthInfo;
    protected bool m_bNeedDirections;
    protected string m_backImage = "Microsoft.Reporting.WebForms.pickback.gif";
    protected string m_forwardImage = "Microsoft.Reporting.WebForms.pickforward.gif";
    protected string m_forwardstr;
    protected string m_backstr;
    private string m_acronymstr;
    private string[] m_DayNames;
    private string[] m_DayFullNames;
    private bool m_bShowNextPrevNavigation = true;
    private bool m_bShowFooter = true;
    private bool m_bShowWeekNumber;
    private bool m_bShowNotThisMonthDays = true;
    private short m_FirstWeekOfYear;
    private int m_minJDay;
    private int m_maxJDay = 2666269;
    private string m_defaultPrevNavText = "<img border=0 alt=\"{0}\" src=\"{1}\" >";
    private string m_defaultNextNavText = "<img border=0 alt=\"{0}\" src=\"{1}\" >";
    private string m_prevNavText;
    private string m_nextNavText;
    private string m_cssClassDatePicker = "ms-picker-table";
    private string m_cssClassDayCenter = "ms-picker-daycenter";
    private string m_cssClassDayCenterOn = "ms-picker-daycenterOn";
    private string m_cssClassDayOtherMonth = "ms-picker-dayother";
    private string m_cssClassDayToday = "ms-picker-today";
    private string m_cssClassWeekSelected = "ms-picker-weekselected";
    private string m_cssClassWeek = "ms-picker-week";
    private string m_cssClassSelectedDay = "ms-picker-dayselected";
    private string m_cssClassWeekDayName = "ms-picker-dayheader";
    private string m_cssClassMonthName = "ms-picker-month ";
    private string m_cssClassFooter = "ms-picker-footer";
    private string m_cssClassWeekBox = "ms-picker-weekbox";
    private bool m_bInit;
    private string m_dayScriptName = "ClickDay";
    private int m_langId = Thread.CurrentThread.CurrentUICulture.LCID;
    private CultureInfo m_LanguageCulture;
    protected string m_headerformat = "&nbsp;{0}&nbsp;";

    public int LangId
    {
      get => this.m_langId;
      set => this.m_langId = value;
    }

    internal CultureInfo LanguageCulture
    {
      get
      {
        if (this.m_LanguageCulture == null)
          this.m_LanguageCulture = new CultureInfo(this.LangId, false);
        return this.m_LanguageCulture;
      }
    }

    public string ImageDirName
    {
      set
      {
        this.m_backImage = value + "Microsoft.Reporting.WebForms.pickback.gif";
        this.m_forwardImage = value + "Microsoft.Reporting.WebForms.pickforward.gif";
      }
    }

    public string OnClickScriptHandler
    {
      get => this.m_dayScriptName;
      set
      {
        string str = SPStringUtility.RemoveNonAlphaNumericChars(value);
        if (string.IsNullOrEmpty(str))
          return;
        this.m_dayScriptName = str;
      }
    }

    public string PrevMonthText
    {
      get => this.m_prevNavText;
      set => this.m_prevNavText = value;
    }

    public string NextMonthText
    {
      get => this.m_nextNavText;
      set => this.m_nextNavText = value;
    }

    public int StartMonthOffset
    {
      get => this.m_StartMonthOffset;
      set
      {
        if (value <= -13 || value > 0)
          return;
        this.m_StartMonthOffset = value;
      }
    }

    public int EndMonthOffset
    {
      get => this.m_EndMonthOffset;
      set
      {
        if (value < 0 || value >= 13)
          return;
        this.m_EndMonthOffset = value;
      }
    }

    public int MinJDay
    {
      get => this.m_minJDay;
      set => this.m_minJDay = value;
    }

    public int MaxJDay
    {
      get => this.m_maxJDay;
      set => this.m_maxJDay = value;
    }

    public bool ShowNextPrevNavigation
    {
      get => this.m_bShowNextPrevNavigation;
      set => this.m_bShowNextPrevNavigation = value;
    }

    public bool ShowFooter
    {
      get => this.m_bShowFooter;
      set => this.m_bShowFooter = value;
    }

    public bool ShowWeekNumber
    {
      get => this.m_bShowWeekNumber;
      set => this.m_bShowWeekNumber = value;
    }

    public bool ShowNotThisMonthDays
    {
      get => this.m_bShowNotThisMonthDays;
      set => this.m_bShowNotThisMonthDays = value;
    }

    internal string CssClassDatePicker => this.m_cssClassDatePicker;

    internal string CssClassMonthName => this.m_cssClassMonthName;

    public DateOptions DateTimeOptions
    {
      get => this.m_dopt;
      set
      {
        this.m_dopt = value;
        this.m_bInit = false;
      }
    }

    public short FirstWeekOfYear
    {
      get => this.m_FirstWeekOfYear;
      set => this.m_FirstWeekOfYear = value;
    }

    protected void Init()
    {
      if (this.m_bInit)
        return;
      if (this.m_dopt == null)
        this.m_dopt = new DateOptions((string) null, SPCalendarType.None, (string) null, (string) null, (string) null, (string) null, (string) null, (string) null);
      this.m_Today = this.m_dopt.Today;
      this.m_startMonth = this.m_dopt.StartMonth;
      this.m_SelectedDate = this.m_dopt.SelectedDate;
      this.m_DayNames = this.m_dopt.GetDaysSuperShortAbbreviation();
      this.m_DayFullNames = this.m_dopt.DayNames;
      this.m_bNeedDirections = this.LanguageCulture.TextInfo.IsRightToLeft;
      this.m_forwardstr = ParameterInputControlStrings.NextMonthToolTip;
      this.m_backstr = ParameterInputControlStrings.PreviousMonthToolTip;
      this.m_acronymstr = "";
      this.m_bInit = true;
    }

    public virtual void RenderAsHtml(StringBuilder st)
    {
      this.Init();
      this.m_MonthInfo = new SimpleDate[this.m_EndMonthOffset - this.m_StartMonthOffset + 3];
      SimpleDate startMonth = this.m_startMonth;
      SimpleDate di;
      for (int nMonths = -1; nMonths >= this.m_StartMonthOffset - 1; --nMonths)
      {
        di = this.m_startMonth;
        try
        {
          di = this.m_dopt.AddMonths(di, nMonths);
          if (!SPIntlCal.IsLocalDateValid(this.m_dopt.CalendarType, ref di, this.m_dopt.HijriAdjustment))
            di.Year = -1;
        }
        catch (ArgumentOutOfRangeException ex)
        {
          di.Year = -1;
        }
        this.m_MonthInfo[nMonths - this.m_StartMonthOffset + 1] = di;
      }
      for (int nMonths = 0; nMonths <= this.m_EndMonthOffset + 1; ++nMonths)
      {
        di = this.m_startMonth;
        try
        {
          di = this.m_dopt.AddMonths(di, nMonths);
          if (!SPIntlCal.IsLocalDateValid(this.m_dopt.CalendarType, ref di, this.m_dopt.HijriAdjustment))
            di.Year = -1;
        }
        catch (ArgumentOutOfRangeException ex)
        {
          di.Year = -1;
        }
        this.m_MonthInfo[nMonths - this.m_StartMonthOffset + 1] = di;
      }
      for (int startMonthOffset = this.m_StartMonthOffset; startMonthOffset <= this.m_EndMonthOffset; ++startMonthOffset)
      {
        if (this.m_MonthInfo[startMonthOffset - this.m_StartMonthOffset + 1].Year > 0)
          this.RenderMonthAsHtml(st, startMonthOffset);
      }
    }

    private void RenderMonthAsHtml(StringBuilder st, int offset)
    {
      SimpleDate simpleDate = this.m_MonthInfo[offset - this.m_StartMonthOffset + 1];
      if (simpleDate.Year < 0)
        return;
      int monthDays = SPIntlCal.DaysInLocalMonth(this.m_dopt.CalendarType, ref simpleDate, this.m_dopt.HijriAdjustment);
      if (offset == 0)
      {
        string str = "<SCRIPT language='javascript'> g_currentID=\"" + this.m_startMonth.Year.ToString("0000", (IFormatProvider) CultureInfo.InvariantCulture) + this.m_startMonth.Month.ToString("00", (IFormatProvider) CultureInfo.InvariantCulture) + this.m_startMonth.Day.ToString("00", (IFormatProvider) CultureInfo.InvariantCulture) + "\";</SCRIPT>";
        st.Append(str);
      }
      st.AppendFormat("\r<div id={0} style=\"display:{1} {2} \" >\r", (object) this.GetDivName(offset), (object) "none", this.m_bNeedDirections ? (object) "; direction:rtl " : (object) "; direction:ltr ");
      st.Append("<div class='ms-datepickerouter'> <div class='ms-quickLaunch' style='width:100%'>");
      string headerstring = string.Format((IFormatProvider) CultureInfo.InvariantCulture, " <td align=center class={0}  nowrap >{1}</td>\r", (object) this.m_cssClassMonthName, (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, this.m_headerformat, (object) this.m_dopt.GetMonthYearString(simpleDate)));
      this.RenderPickerHeaderAsHtml(st, offset, simpleDate, headerstring, this.m_bShowWeekNumber ? 8 : 7);
      st.Append(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "<table cellpadding=0 cellspacing=0 border=0 class=\"{0}\">\r", (object) this.m_cssClassDatePicker));
      this.RenderDaysAsHtml(st, ref simpleDate, monthDays);
      this.RenderFooterAsHtml(st, this.m_bShowWeekNumber ? 9 : 8);
      st.Append("</table>\r");
      st.Append("</div></div>");
      st.Append("</div></div>");
    }

    protected string GetDivName(int offset)
    {
      string divName = "DatePickerDiv";
      if (offset < 0)
        divName = divName + "M" + (-offset).ToString((IFormatProvider) CultureInfo.InvariantCulture);
      else if (offset > 0)
        divName = divName + "P" + offset.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      return divName;
    }

    protected string GetDivFocusElement(int offset, bool previous)
    {
      return this.GetDivName(offset) + (previous ? "MovePrevious" : "MoveNext");
    }

    protected void RenderPickerHeaderAsHtml(
      StringBuilder str,
      int offset,
      SimpleDate selectedDate,
      string headerstring,
      int colspan)
    {
      SimpleDate di1 = this.m_MonthInfo[offset - this.m_StartMonthOffset];
      string str1;
      if (di1.Year > -1 && this.m_bShowNextPrevNavigation)
      {
        if (offset == this.m_StartMonthOffset)
        {
          if (this.m_startMonth.Day > di1.Day)
          {
            int day = di1.Day;
          }
          CultureInfo invariantCulture = CultureInfo.InvariantCulture;
          object[] objArray1 = new object[4]
          {
            (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "ShowLoading();MoveToDate('{0}', {1});", (object) SPHttpUtility.EcmaScriptStringLiteralEncode(this.GetDayString(di1)), (object) "true"),
            null,
            null,
            null
          };
          object[] objArray2 = objArray1;
          string str2;
          if (!string.IsNullOrEmpty(this.m_prevNavText))
            str2 = SPHttpUtility.HtmlEncode(this.m_prevNavText);
          else
            str2 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, this.m_defaultPrevNavText, (object) this.m_backstr, this.m_bNeedDirections ? (object) this.m_forwardImage : (object) this.m_backImage);
          objArray2[1] = (object) str2;
          objArray1[2] = (object) "&lt;";
          objArray1[3] = (object) this.GetDivFocusElement(offset, true);
          object[] objArray3 = objArray1;
          str1 = string.Format((IFormatProvider) invariantCulture, " <td align=center><a id=\"{3}\" href=\"javascript:{0}\" accesskey=\"{2}\">{1}</a></td>\r", objArray3);
        }
        else
        {
          string str3 = di1.Year.ToString("0000", (IFormatProvider) CultureInfo.InvariantCulture) + di1.Month.ToString("00", (IFormatProvider) CultureInfo.InvariantCulture) + "01";
          CultureInfo invariantCulture = CultureInfo.InvariantCulture;
          object[] objArray4 = new object[4]
          {
            (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "HideUnhide('{0}','{1}','{2}', '{3}');", (object) this.GetDivName(offset), (object) this.GetDivName(offset - 1), (object) str3, (object) this.GetDivFocusElement(offset - 1, true)),
            null,
            null,
            null
          };
          object[] objArray5 = objArray4;
          string str4;
          if (!string.IsNullOrEmpty(this.m_prevNavText))
            str4 = SPHttpUtility.HtmlEncode(this.m_prevNavText);
          else
            str4 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, this.m_defaultPrevNavText, (object) this.m_backstr, this.m_bNeedDirections ? (object) this.m_forwardImage : (object) this.m_backImage);
          objArray5[1] = (object) str4;
          objArray4[2] = (object) "&lt;";
          objArray4[3] = (object) this.GetDivFocusElement(offset, true);
          object[] objArray6 = objArray4;
          str1 = string.Format((IFormatProvider) invariantCulture, " <td align=center><a id=\"{3}\" href=\"javascript:{0}\" accesskey=\"{2}\">{1}</a></td>\r", objArray6);
        }
      }
      else
        str1 = "<td>&nbsp;</td>";
      SimpleDate di2 = this.m_MonthInfo[offset - this.m_StartMonthOffset + 2];
      string str5;
      if (di2.Year > -1 && this.m_bShowNextPrevNavigation)
      {
        if (offset == this.m_EndMonthOffset)
        {
          if (this.m_startMonth.Day > di2.Day)
          {
            int day = di2.Day;
          }
          CultureInfo invariantCulture = CultureInfo.InvariantCulture;
          object[] objArray7 = new object[4]
          {
            (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "ShowLoading();MoveToDate('{0}', {1});", (object) SPHttpUtility.EcmaScriptStringLiteralEncode(this.GetDayString(di2)), (object) "false"),
            null,
            null,
            null
          };
          object[] objArray8 = objArray7;
          string str6;
          if (!string.IsNullOrEmpty(this.m_nextNavText))
            str6 = SPHttpUtility.HtmlEncode(this.m_nextNavText);
          else
            str6 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, this.m_defaultNextNavText, (object) this.m_forwardstr, this.m_bNeedDirections ? (object) this.m_backImage : (object) this.m_forwardImage);
          objArray8[1] = (object) str6;
          objArray7[2] = (object) "&gt;";
          objArray7[3] = (object) this.GetDivFocusElement(offset, false);
          object[] objArray9 = objArray7;
          str5 = string.Format((IFormatProvider) invariantCulture, " <td align=center><a id=\"{3}\" href=\"javascript:{0}\" accesskey=\"{2}\">{1}</a></td>\r", objArray9);
        }
        else
        {
          string str7 = di2.Year.ToString("0000", (IFormatProvider) CultureInfo.InvariantCulture) + di2.Month.ToString("00", (IFormatProvider) CultureInfo.InvariantCulture) + "01";
          CultureInfo invariantCulture = CultureInfo.InvariantCulture;
          object[] objArray10 = new object[4]
          {
            (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "HideUnhide('{0}','{1}','{2}', '{3}');", (object) this.GetDivName(offset), (object) this.GetDivName(offset + 1), (object) str7, (object) this.GetDivFocusElement(offset + 1, false)),
            null,
            null,
            null
          };
          object[] objArray11 = objArray10;
          string str8;
          if (!string.IsNullOrEmpty(this.m_nextNavText))
            str8 = SPHttpUtility.HtmlEncode(this.m_nextNavText);
          else
            str8 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, this.m_defaultNextNavText, (object) this.m_forwardstr, this.m_bNeedDirections ? (object) this.m_backImage : (object) this.m_forwardImage);
          objArray11[1] = (object) str8;
          objArray10[2] = (object) "&gt;";
          objArray10[3] = (object) this.GetDivFocusElement(offset, false);
          object[] objArray12 = objArray10;
          str5 = string.Format((IFormatProvider) invariantCulture, " <td align=center><a id=\"{3}\" href=\"javascript:{0}\" accesskey=\"{2}\">{1}</a></td>\r", objArray12);
        }
      }
      else
        str5 = "<td>&nbsp;</td>";
      str.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "<div class='ms-picker-header'><table cellpadding=0 cellspacing=0 border=0 class=\"{4}\" ><tr>{0}{1}{2}</tr></table></div><div>", (object) str1, (object) headerstring, (object) str5, (object) colspan, (object) this.m_cssClassDatePicker);
    }

    private bool RenderWeekHeaderAsHtml(StringBuilder st, int weekCount)
    {
      st.Append("<tr>\r");
      if (this.m_bShowWeekNumber)
        st.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, " <th scope=\"col\" class={0} nowrap><ABBR title={3}>&nbsp;{1}{2}</ABBR></th>\r", (object) this.m_cssClassWeekDayName, (object) "", (object) "", (object) "");
      for (int index1 = 0; index1 < 7; ++index1)
      {
        int index2 = (index1 + this.m_dopt.FirstDayOfWeek) % 7;
        st.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, " <th scope=\"col\" class={0} nowrap><ABBR title={3}>&nbsp;{1}{2}</ABBR></th>\r", (object) this.m_cssClassWeekDayName, (object) this.m_DayNames[index2], (object) "&nbsp;", (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, " \"{0}\" ", (object) this.m_DayFullNames[index2]));
      }
      st.Append("</tr>\r");
      return true;
    }

    private bool RenderDaysAsHtml(StringBuilder st, ref SimpleDate selectedDate, int monthDays)
    {
      SimpleDate di1 = selectedDate with { Day = 1 };
      bool flag1 = false;
      int julianDay = SPIntlCal.LocalToJulianDay(this.m_dopt.CalendarType, ref di1, this.m_dopt.HijriAdjustment);
      int num1 = (julianDay + 1) % 7;
      int firstDayOfWeek = this.m_dopt.FirstDayOfWeek;
      int num2 = 0;
      int jDay1 = julianDay;
      if (num1 != firstDayOfWeek)
      {
        num2 = (num1 - firstDayOfWeek + 7) % 7;
        jDay1 -= num2;
        if (!SPIntlCal.IsSupportedLocalJulianDay(this.m_dopt.CalendarType, jDay1))
          di1.Year = -1;
        else
          SPIntlCal.JulianDayToLocal(this.m_dopt.CalendarType, jDay1, ref di1, this.m_dopt.HijriAdjustment);
      }
      int num3 = 7 * ((monthDays + num2 + 7 - 1) / 7);
      int jDay2 = jDay1 + num3 - 1;
      SimpleDate di2 = selectedDate;
      if (!SPIntlCal.IsSupportedLocalJulianDay(this.m_dopt.CalendarType, jDay2))
        di2.Year = -1;
      else
        SPIntlCal.JulianDayToLocal(this.m_dopt.CalendarType, jDay2, ref di2, this.m_dopt.HijriAdjustment);
      SimpleDate simpleDate = di1;
      this.RenderWeekHeaderAsHtml(st, num3 / 7);
      for (int index = 0; index < num3 / 7; ++index)
      {
        st.Append("<tr>\r");
        if (this.m_bShowWeekNumber)
        {
          string dayString;
          int weekNumber;
          if (simpleDate.Year > -1)
          {
            simpleDate = this.m_dopt.fixYear(simpleDate);
            dayString = this.GetDayString(simpleDate);
            SimpleDate di3;
            try
            {
              di3 = this.m_dopt.AddDays(simpleDate, 6);
            }
            catch (ArgumentOutOfRangeException ex)
            {
              di3 = simpleDate;
            }
            weekNumber = SPIntlCal.GetWeekNumber(this.m_dopt.CalendarType, di3, this.m_dopt.FirstDayOfWeek, this.m_FirstWeekOfYear);
            if (weekNumber <= 0)
              weekNumber = SPIntlCal.GetWeekNumber(this.m_dopt.CalendarType, simpleDate, this.m_dopt.FirstDayOfWeek, this.m_FirstWeekOfYear);
          }
          else
          {
            SimpleDate di4 = this.m_dopt.fixYear(selectedDate with
            {
              Day = 1
            });
            weekNumber = SPIntlCal.GetWeekNumber(this.m_dopt.CalendarType, di4, this.m_dopt.FirstDayOfWeek, this.m_FirstWeekOfYear);
            dayString = this.GetDayString(di4);
          }
          st.AppendFormat("<th scope=\"row\" class={0} onmouseover=\"this.className='{1}';\" onmouseout=\"this.className='{0}';\">{2}</td>\r", (object) (flag1 ? (this.m_bNeedDirections ? this.m_cssClassWeekSelected + "RTL" : this.m_cssClassWeekSelected) : (this.m_bNeedDirections ? this.m_cssClassWeek + "RTL" : this.m_cssClassWeek)), (object) (this.m_bNeedDirections ? this.m_cssClassWeekSelected + "RTL" : this.m_cssClassWeekSelected), (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "<div class={0}><a href=\"javascript:MoveToDate('{2}', false)\">{1}</a></div>", this.m_bNeedDirections ? (object) (this.m_cssClassWeekBox + "RTL") : (object) this.m_cssClassWeekBox, (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "<ACRONYM title=\"{0}\" >{1}</ACRONYM>", (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, this.m_acronymstr, (object) weekNumber), (object) weekNumber), (object) SPHttpUtility.EcmaScriptStringLiteralEncode(dayString)));
        }
        for (int iDay = 0; iDay < 7; ++iDay)
        {
          simpleDate = this.m_dopt.fixYear(simpleDate);
          bool flag2 = simpleDate.Month == selectedDate.Month;
          this.m_dopt.IsWorkDay(iDay);
          bool isSelected = simpleDate.Year == this.m_SelectedDate.Year && simpleDate.Month == this.m_SelectedDate.Month && simpleDate.Day == this.m_SelectedDate.Day;
          bool flag3 = jDay1 <= this.m_maxJDay && jDay1 >= this.m_minJDay;
          if (!this.m_bShowNotThisMonthDays & !flag2 || simpleDate.Year <= 0)
          {
            st.Append("<td>&nbsp;</td>");
          }
          else
          {
            string dayString = this.GetDayString(simpleDate);
            string dayChar = this.m_dopt.GetDayChar(simpleDate.Day);
            string valueToEncode = flag2 ? simpleDate.Year.ToString("0000", (IFormatProvider) CultureInfo.InvariantCulture) + simpleDate.Month.ToString("00", (IFormatProvider) CultureInfo.InvariantCulture) + simpleDate.Day.ToString("00", (IFormatProvider) CultureInfo.InvariantCulture) : "";
            st.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, flag3 ? "<td class={0} onmouseover=\"this.className='{1}';\" onmouseout=\"this.className='{0}';\"  onclick=\"javascript:{5}('{2}')\" {6} ><a onclick=\"event.cancelBubble=true;\" href=\"javascript:{5}('{2}')\" id=\"{3}\" >{4}</a></td>\r" : "<td class={0}>{4}</td>\r", (object) this.GetDayStyle(isSelected, flag2 && flag3, this.m_Today == simpleDate), (object) this.m_cssClassDayCenterOn, (object) SPHttpUtility.EcmaScriptStringLiteralEncode(dayString), (object) SPHttpUtility.HtmlEncode(valueToEncode), (object) SPHttpUtility.HtmlEncode(dayChar), (object) this.m_dayScriptName, (object) "");
          }
          ++simpleDate.Day;
          if (7 * index + iDay == num2 - 1)
            simpleDate = selectedDate with { Day = 1 };
          else if (7 * index + iDay == num2 + monthDays - 1)
            simpleDate = di2 with { Day = 1 };
          ++jDay1;
        }
        st.Append("</tr>\r");
      }
      return true;
    }

    private string GetTodayDayString(string todayDate)
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, LocalizationHelper.Current.TodayIs, (object) todayDate);
    }

    private string GetDayStyle(bool isSelected, bool inMonth, bool isToday)
    {
      if (isToday)
        return this.m_cssClassDayToday;
      if (isSelected)
        return this.m_cssClassSelectedDay;
      return !inMonth ? this.m_cssClassDayOtherMonth : this.m_cssClassDayCenter;
    }

    protected string GetDayString(SimpleDate di) => this.m_dopt.GetShortDateString(di);

    protected bool RenderFooterAsHtml(StringBuilder st, int colspan)
    {
      if (!this.m_bShowFooter)
        return true;
      string todayDate = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "<a style=\"text-decoration:none\" href=\"javascript:ClickDay('{0}');\">{1}</a>", (object) SPHttpUtility.EcmaScriptStringLiteralEncode(this.GetDayString(this.m_Today)), (object) SPHttpUtility.HtmlEncode(this.m_dopt.GetDowLongDateString(this.m_Today)));
      st.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "<tr><td colspan={2} class={0} dir={3}><div>{1}</div></td></tr>", (object) this.m_cssClassFooter, (object) this.GetTodayDayString(todayDate), (object) colspan, this.LanguageCulture.TextInfo.IsRightToLeft ? (object) "rtl" : (object) "ltr");
      return true;
    }
  }
}
