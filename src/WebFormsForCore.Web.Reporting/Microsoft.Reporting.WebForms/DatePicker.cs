using System;
using System.Globalization;
using System.Text;
using System.Threading;

namespace Microsoft.Reporting.WebForms;

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
		get
		{
			return m_langId;
		}
		set
		{
			m_langId = value;
		}
	}

	internal CultureInfo LanguageCulture
	{
		get
		{
			if (m_LanguageCulture == null)
			{
				m_LanguageCulture = new CultureInfo(LangId, useUserOverride: false);
			}
			return m_LanguageCulture;
		}
	}

	public string ImageDirName
	{
		set
		{
			m_backImage = value + "Microsoft.Reporting.WebForms.pickback.gif";
			m_forwardImage = value + "Microsoft.Reporting.WebForms.pickforward.gif";
		}
	}

	public string OnClickScriptHandler
	{
		get
		{
			return m_dayScriptName;
		}
		set
		{
			string text = SPStringUtility.RemoveNonAlphaNumericChars(value);
			if (!string.IsNullOrEmpty(text))
			{
				m_dayScriptName = text;
			}
		}
	}

	public string PrevMonthText
	{
		get
		{
			return m_prevNavText;
		}
		set
		{
			m_prevNavText = value;
		}
	}

	public string NextMonthText
	{
		get
		{
			return m_nextNavText;
		}
		set
		{
			m_nextNavText = value;
		}
	}

	public int StartMonthOffset
	{
		get
		{
			return m_StartMonthOffset;
		}
		set
		{
			if (value > -13 && value <= 0)
			{
				m_StartMonthOffset = value;
			}
		}
	}

	public int EndMonthOffset
	{
		get
		{
			return m_EndMonthOffset;
		}
		set
		{
			if (value >= 0 && value < 13)
			{
				m_EndMonthOffset = value;
			}
		}
	}

	public int MinJDay
	{
		get
		{
			return m_minJDay;
		}
		set
		{
			m_minJDay = value;
		}
	}

	public int MaxJDay
	{
		get
		{
			return m_maxJDay;
		}
		set
		{
			m_maxJDay = value;
		}
	}

	public bool ShowNextPrevNavigation
	{
		get
		{
			return m_bShowNextPrevNavigation;
		}
		set
		{
			m_bShowNextPrevNavigation = value;
		}
	}

	public bool ShowFooter
	{
		get
		{
			return m_bShowFooter;
		}
		set
		{
			m_bShowFooter = value;
		}
	}

	public bool ShowWeekNumber
	{
		get
		{
			return m_bShowWeekNumber;
		}
		set
		{
			m_bShowWeekNumber = value;
		}
	}

	public bool ShowNotThisMonthDays
	{
		get
		{
			return m_bShowNotThisMonthDays;
		}
		set
		{
			m_bShowNotThisMonthDays = value;
		}
	}

	internal string CssClassDatePicker => m_cssClassDatePicker;

	internal string CssClassMonthName => m_cssClassMonthName;

	public DateOptions DateTimeOptions
	{
		get
		{
			return m_dopt;
		}
		set
		{
			m_dopt = value;
			m_bInit = false;
		}
	}

	public short FirstWeekOfYear
	{
		get
		{
			return m_FirstWeekOfYear;
		}
		set
		{
			m_FirstWeekOfYear = value;
		}
	}

	protected void Init()
	{
		if (!m_bInit)
		{
			if (m_dopt == null)
			{
				m_dopt = new DateOptions(null, SPCalendarType.None, null, null, null, null, null, null);
			}
			m_Today = m_dopt.Today;
			m_startMonth = m_dopt.StartMonth;
			m_SelectedDate = m_dopt.SelectedDate;
			m_DayNames = m_dopt.GetDaysSuperShortAbbreviation();
			m_DayFullNames = m_dopt.DayNames;
			m_bNeedDirections = LanguageCulture.TextInfo.IsRightToLeft;
			m_forwardstr = ParameterInputControlStrings.NextMonthToolTip;
			m_backstr = ParameterInputControlStrings.PreviousMonthToolTip;
			m_acronymstr = "";
			m_bInit = true;
		}
	}

	public virtual void RenderAsHtml(StringBuilder st)
	{
		Init();
		m_MonthInfo = new SimpleDate[m_EndMonthOffset - m_StartMonthOffset + 3];
		SimpleDate startMonth = m_startMonth;
		for (int num = -1; num >= m_StartMonthOffset - 1; num--)
		{
			startMonth = m_startMonth;
			try
			{
				startMonth = m_dopt.AddMonths(startMonth, num);
				if (!SPIntlCal.IsLocalDateValid(m_dopt.CalendarType, ref startMonth, m_dopt.HijriAdjustment))
				{
					startMonth.Year = -1;
				}
			}
			catch (ArgumentOutOfRangeException)
			{
				startMonth.Year = -1;
			}
			m_MonthInfo[num - m_StartMonthOffset + 1] = startMonth;
		}
		for (int num = 0; num <= m_EndMonthOffset + 1; num++)
		{
			startMonth = m_startMonth;
			try
			{
				startMonth = m_dopt.AddMonths(startMonth, num);
				if (!SPIntlCal.IsLocalDateValid(m_dopt.CalendarType, ref startMonth, m_dopt.HijriAdjustment))
				{
					startMonth.Year = -1;
				}
			}
			catch (ArgumentOutOfRangeException)
			{
				startMonth.Year = -1;
			}
			m_MonthInfo[num - m_StartMonthOffset + 1] = startMonth;
		}
		for (int num = m_StartMonthOffset; num <= m_EndMonthOffset; num++)
		{
			if (m_MonthInfo[num - m_StartMonthOffset + 1].Year > 0)
			{
				RenderMonthAsHtml(st, num);
			}
		}
	}

	private void RenderMonthAsHtml(StringBuilder st, int offset)
	{
		SimpleDate di = m_MonthInfo[offset - m_StartMonthOffset + 1];
		if (di.Year >= 0)
		{
			int monthDays = SPIntlCal.DaysInLocalMonth(m_dopt.CalendarType, ref di, m_dopt.HijriAdjustment);
			if (offset == 0)
			{
				string value = "<SCRIPT language='javascript'> g_currentID=\"" + m_startMonth.Year.ToString("0000", CultureInfo.InvariantCulture) + m_startMonth.Month.ToString("00", CultureInfo.InvariantCulture) + m_startMonth.Day.ToString("00", CultureInfo.InvariantCulture) + "\";</SCRIPT>";
				st.Append(value);
			}
			st.AppendFormat("\r<div id={0} style=\"display:{1} {2} \" >\r", GetDivName(offset), "none", m_bNeedDirections ? "; direction:rtl " : "; direction:ltr ");
			st.Append("<div class='ms-datepickerouter'> <div class='ms-quickLaunch' style='width:100%'>");
			string headerstring = string.Format(CultureInfo.InvariantCulture, " <td align=center class={0}  nowrap >{1}</td>\r", m_cssClassMonthName, string.Format(CultureInfo.InvariantCulture, m_headerformat, m_dopt.GetMonthYearString(di)));
			RenderPickerHeaderAsHtml(st, offset, di, headerstring, m_bShowWeekNumber ? 8 : 7);
			st.Append(string.Format(CultureInfo.InvariantCulture, "<table cellpadding=0 cellspacing=0 border=0 class=\"{0}\">\r", m_cssClassDatePicker));
			RenderDaysAsHtml(st, ref di, monthDays);
			RenderFooterAsHtml(st, m_bShowWeekNumber ? 9 : 8);
			st.Append("</table>\r");
			st.Append("</div></div>");
			st.Append("</div></div>");
		}
	}

	protected string GetDivName(int offset)
	{
		string text = "DatePickerDiv";
		if (offset < 0)
		{
			text = text + "M" + (-offset).ToString(CultureInfo.InvariantCulture);
		}
		else if (offset > 0)
		{
			text = text + "P" + offset.ToString(CultureInfo.InvariantCulture);
		}
		return text;
	}

	protected string GetDivFocusElement(int offset, bool previous)
	{
		string divName = GetDivName(offset);
		return divName + (previous ? "MovePrevious" : "MoveNext");
	}

	protected void RenderPickerHeaderAsHtml(StringBuilder str, int offset, SimpleDate selectedDate, string headerstring, int colspan)
	{
		SimpleDate di = m_MonthInfo[offset - m_StartMonthOffset];
		string text;
		if (di.Year > -1 && m_bShowNextPrevNavigation)
		{
			if (offset == m_StartMonthOffset)
			{
				int day = m_startMonth.Day;
				if (day > di.Day)
				{
					day = di.Day;
				}
				text = string.Format(CultureInfo.InvariantCulture, " <td align=center><a id=\"{3}\" href=\"javascript:{0}\" accesskey=\"{2}\">{1}</a></td>\r", string.Format(CultureInfo.InvariantCulture, "ShowLoading();MoveToDate('{0}', {1});", SPHttpUtility.EcmaScriptStringLiteralEncode(GetDayString(di)), "true"), string.IsNullOrEmpty(m_prevNavText) ? string.Format(CultureInfo.InvariantCulture, m_defaultPrevNavText, m_backstr, m_bNeedDirections ? m_forwardImage : m_backImage) : SPHttpUtility.HtmlEncode(m_prevNavText), "&lt;", GetDivFocusElement(offset, previous: true));
			}
			else
			{
				string text2 = di.Year.ToString("0000", CultureInfo.InvariantCulture) + di.Month.ToString("00", CultureInfo.InvariantCulture) + "01";
				text = string.Format(CultureInfo.InvariantCulture, " <td align=center><a id=\"{3}\" href=\"javascript:{0}\" accesskey=\"{2}\">{1}</a></td>\r", string.Format(CultureInfo.InvariantCulture, "HideUnhide('{0}','{1}','{2}', '{3}');", GetDivName(offset), GetDivName(offset - 1), text2, GetDivFocusElement(offset - 1, previous: true)), string.IsNullOrEmpty(m_prevNavText) ? string.Format(CultureInfo.InvariantCulture, m_defaultPrevNavText, m_backstr, m_bNeedDirections ? m_forwardImage : m_backImage) : SPHttpUtility.HtmlEncode(m_prevNavText), "&lt;", GetDivFocusElement(offset, previous: true));
			}
		}
		else
		{
			text = "<td>&nbsp;</td>";
		}
		SimpleDate di2 = m_MonthInfo[offset - m_StartMonthOffset + 2];
		string text3;
		if (di2.Year > -1 && m_bShowNextPrevNavigation)
		{
			if (offset == m_EndMonthOffset)
			{
				int day2 = m_startMonth.Day;
				if (day2 > di2.Day)
				{
					day2 = di2.Day;
				}
				text3 = string.Format(CultureInfo.InvariantCulture, " <td align=center><a id=\"{3}\" href=\"javascript:{0}\" accesskey=\"{2}\">{1}</a></td>\r", string.Format(CultureInfo.InvariantCulture, "ShowLoading();MoveToDate('{0}', {1});", SPHttpUtility.EcmaScriptStringLiteralEncode(GetDayString(di2)), "false"), string.IsNullOrEmpty(m_nextNavText) ? string.Format(CultureInfo.InvariantCulture, m_defaultNextNavText, m_forwardstr, m_bNeedDirections ? m_backImage : m_forwardImage) : SPHttpUtility.HtmlEncode(m_nextNavText), "&gt;", GetDivFocusElement(offset, previous: false));
			}
			else
			{
				string text2 = di2.Year.ToString("0000", CultureInfo.InvariantCulture) + di2.Month.ToString("00", CultureInfo.InvariantCulture) + "01";
				text3 = string.Format(CultureInfo.InvariantCulture, " <td align=center><a id=\"{3}\" href=\"javascript:{0}\" accesskey=\"{2}\">{1}</a></td>\r", string.Format(CultureInfo.InvariantCulture, "HideUnhide('{0}','{1}','{2}', '{3}');", GetDivName(offset), GetDivName(offset + 1), text2, GetDivFocusElement(offset + 1, previous: false)), string.IsNullOrEmpty(m_nextNavText) ? string.Format(CultureInfo.InvariantCulture, m_defaultNextNavText, m_forwardstr, m_bNeedDirections ? m_backImage : m_forwardImage) : SPHttpUtility.HtmlEncode(m_nextNavText), "&gt;", GetDivFocusElement(offset, previous: false));
			}
		}
		else
		{
			text3 = "<td>&nbsp;</td>";
		}
		str.AppendFormat(CultureInfo.InvariantCulture, "<div class='ms-picker-header'><table cellpadding=0 cellspacing=0 border=0 class=\"{4}\" ><tr>{0}{1}{2}</tr></table></div><div>", text, headerstring, text3, colspan, m_cssClassDatePicker);
	}

	private bool RenderWeekHeaderAsHtml(StringBuilder st, int weekCount)
	{
		st.Append("<tr>\r");
		if (m_bShowWeekNumber)
		{
			st.AppendFormat(CultureInfo.InvariantCulture, " <th scope=\"col\" class={0} nowrap><ABBR title={3}>&nbsp;{1}{2}</ABBR></th>\r", m_cssClassWeekDayName, "", "", "");
		}
		int num = 0;
		for (int i = 0; i < 7; i++)
		{
			num = (i + m_dopt.FirstDayOfWeek) % 7;
			st.AppendFormat(CultureInfo.InvariantCulture, " <th scope=\"col\" class={0} nowrap><ABBR title={3}>&nbsp;{1}{2}</ABBR></th>\r", m_cssClassWeekDayName, m_DayNames[num], "&nbsp;", string.Format(CultureInfo.InvariantCulture, " \"{0}\" ", m_DayFullNames[num]));
		}
		st.Append("</tr>\r");
		return true;
	}

	private bool RenderDaysAsHtml(StringBuilder st, ref SimpleDate selectedDate, int monthDays)
	{
		SimpleDate di = selectedDate;
		di.Day = 1;
		bool flag = false;
		int num = SPIntlCal.LocalToJulianDay(m_dopt.CalendarType, ref di, m_dopt.HijriAdjustment);
		int num2 = (num + 1) % 7;
		int firstDayOfWeek = m_dopt.FirstDayOfWeek;
		int num3 = 0;
		int num4 = num;
		if (num2 != firstDayOfWeek)
		{
			num3 = (num2 - firstDayOfWeek + 7) % 7;
			num4 -= num3;
			if (!SPIntlCal.IsSupportedLocalJulianDay(m_dopt.CalendarType, num4))
			{
				di.Year = -1;
			}
			else
			{
				SPIntlCal.JulianDayToLocal(m_dopt.CalendarType, num4, ref di, m_dopt.HijriAdjustment);
			}
		}
		int num5 = 7 * ((monthDays + num3 + 7 - 1) / 7);
		int jDay = num4 + num5 - 1;
		SimpleDate di2 = selectedDate;
		if (!SPIntlCal.IsSupportedLocalJulianDay(m_dopt.CalendarType, jDay))
		{
			di2.Year = -1;
		}
		else
		{
			SPIntlCal.JulianDayToLocal(m_dopt.CalendarType, jDay, ref di2, m_dopt.HijriAdjustment);
		}
		SimpleDate simpleDate = di;
		RenderWeekHeaderAsHtml(st, num5 / 7);
		for (int i = 0; i < num5 / 7; i++)
		{
			st.Append("<tr>\r");
			if (m_bShowWeekNumber)
			{
				string dayString;
				int weekNumber;
				if (simpleDate.Year > -1)
				{
					simpleDate = m_dopt.fixYear(simpleDate);
					dayString = GetDayString(simpleDate);
					SimpleDate di3;
					try
					{
						di3 = m_dopt.AddDays(simpleDate, 6);
					}
					catch (ArgumentOutOfRangeException)
					{
						di3 = simpleDate;
					}
					weekNumber = SPIntlCal.GetWeekNumber(m_dopt.CalendarType, di3, m_dopt.FirstDayOfWeek, m_FirstWeekOfYear);
					if (weekNumber <= 0)
					{
						weekNumber = SPIntlCal.GetWeekNumber(m_dopt.CalendarType, simpleDate, m_dopt.FirstDayOfWeek, m_FirstWeekOfYear);
					}
				}
				else
				{
					SimpleDate di3 = selectedDate;
					di3.Day = 1;
					di3 = m_dopt.fixYear(di3);
					weekNumber = SPIntlCal.GetWeekNumber(m_dopt.CalendarType, di3, m_dopt.FirstDayOfWeek, m_FirstWeekOfYear);
					dayString = GetDayString(di3);
				}
				st.AppendFormat("<th scope=\"row\" class={0} onmouseover=\"this.className='{1}';\" onmouseout=\"this.className='{0}';\">{2}</td>\r", (!flag) ? (m_bNeedDirections ? (m_cssClassWeek + "RTL") : m_cssClassWeek) : (m_bNeedDirections ? (m_cssClassWeekSelected + "RTL") : m_cssClassWeekSelected), m_bNeedDirections ? (m_cssClassWeekSelected + "RTL") : m_cssClassWeekSelected, string.Format(CultureInfo.InvariantCulture, "<div class={0}><a href=\"javascript:MoveToDate('{2}', false)\">{1}</a></div>", m_bNeedDirections ? (m_cssClassWeekBox + "RTL") : m_cssClassWeekBox, string.Format(CultureInfo.InvariantCulture, "<ACRONYM title=\"{0}\" >{1}</ACRONYM>", string.Format(CultureInfo.InvariantCulture, m_acronymstr, weekNumber), weekNumber), SPHttpUtility.EcmaScriptStringLiteralEncode(dayString)));
			}
			for (int j = 0; j < 7; j++)
			{
				simpleDate = m_dopt.fixYear(simpleDate);
				bool flag2 = simpleDate.Month == selectedDate.Month;
				m_dopt.IsWorkDay(j);
				bool isSelected = simpleDate.Year == m_SelectedDate.Year && simpleDate.Month == m_SelectedDate.Month && simpleDate.Day == m_SelectedDate.Day;
				bool flag3 = num4 <= m_maxJDay && num4 >= m_minJDay;
				if ((!m_bShowNotThisMonthDays && !flag2) || simpleDate.Year <= 0)
				{
					st.Append("<td>&nbsp;</td>");
				}
				else
				{
					string dayString = GetDayString(simpleDate);
					string dayChar = m_dopt.GetDayChar(simpleDate.Day);
					string valueToEncode = (flag2 ? (simpleDate.Year.ToString("0000", CultureInfo.InvariantCulture) + simpleDate.Month.ToString("00", CultureInfo.InvariantCulture) + simpleDate.Day.ToString("00", CultureInfo.InvariantCulture)) : "");
					st.AppendFormat(CultureInfo.InvariantCulture, flag3 ? "<td class={0} onmouseover=\"this.className='{1}';\" onmouseout=\"this.className='{0}';\"  onclick=\"javascript:{5}('{2}')\" {6} ><a onclick=\"event.cancelBubble=true;\" href=\"javascript:{5}('{2}')\" id=\"{3}\" >{4}</a></td>\r" : "<td class={0}>{4}</td>\r", GetDayStyle(isSelected, flag2 && flag3, m_Today == simpleDate), m_cssClassDayCenterOn, SPHttpUtility.EcmaScriptStringLiteralEncode(dayString), SPHttpUtility.HtmlEncode(valueToEncode), SPHttpUtility.HtmlEncode(dayChar), m_dayScriptName, "");
				}
				simpleDate.Day++;
				if (7 * i + j == num3 - 1)
				{
					simpleDate = selectedDate;
					simpleDate.Day = 1;
				}
				else if (7 * i + j == num3 + monthDays - 1)
				{
					simpleDate = di2;
					simpleDate.Day = 1;
				}
				num4++;
			}
			st.Append("</tr>\r");
		}
		return true;
	}

	private string GetTodayDayString(string todayDate)
	{
		string todayIs = LocalizationHelper.Current.TodayIs;
		return string.Format(CultureInfo.InvariantCulture, todayIs, todayDate);
	}

	private string GetDayStyle(bool isSelected, bool inMonth, bool isToday)
	{
		if (isToday)
		{
			return m_cssClassDayToday;
		}
		if (isSelected)
		{
			return m_cssClassSelectedDay;
		}
		if (!inMonth)
		{
			return m_cssClassDayOtherMonth;
		}
		return m_cssClassDayCenter;
	}

	protected string GetDayString(SimpleDate di)
	{
		return m_dopt.GetShortDateString(di);
	}

	protected bool RenderFooterAsHtml(StringBuilder st, int colspan)
	{
		if (!m_bShowFooter)
		{
			return true;
		}
		string dayString = GetDayString(m_Today);
		string todayDate = string.Format(CultureInfo.InvariantCulture, "<a style=\"text-decoration:none\" href=\"javascript:ClickDay('{0}');\">{1}</a>", SPHttpUtility.EcmaScriptStringLiteralEncode(dayString), SPHttpUtility.HtmlEncode(m_dopt.GetDowLongDateString(m_Today)));
		st.AppendFormat(CultureInfo.InvariantCulture, "<tr><td colspan={2} class={0} dir={3}><div>{1}</div></td></tr>", m_cssClassFooter, GetTodayDayString(todayDate), colspan, LanguageCulture.TextInfo.IsRightToLeft ? "rtl" : "ltr");
		return true;
	}
}
