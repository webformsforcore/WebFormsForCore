using System;
using System.Collections;
using System.Globalization;
using System.IO;

namespace Microsoft.Reporting.WebForms;

internal sealed class DateOptions
{
	private string m_Lang;

	private int m_Lcid;

	private CultureInfo m_CultureInfo;

	private CultureInfo m_LocaleCultureInfo;

	private SPCalendarType m_CalendarType;

	private int m_FirstDayOfWeek;

	private int m_HijriAdjustment;

	private bool m_UseMonthGenitiveNames;

	private SimpleDate m_Today;

	private SimpleDate m_SelectedDate;

	private bool[] m_WorkWeek;

	private Hashtable m_HebrewNumberToIntDict;

	private Hashtable m_IntToHebrewNumberDict;

	private string m_DateSeparator;

	private string m_TimeSeparator;

	private string m_AMDesignator;

	private string m_PMDesignator;

	private string m_TimePattern12Hour;

	private string m_TimePattern24Hour;

	private string[] m_TimeFormatPatterns;

	private string[] m_MonthNames;

	private string[] m_MonthGenitiveNames;

	private string[] m_AbbrMonthNames;

	private string[] m_LeapMonthNames;

	private string[] m_LeapAbbrMonthNames;

	private string[] m_DayNames;

	private string[] m_SuperShortAbbrDayNames;

	private string[] m_ShortAbbrDayNames;

	private string[] m_AbbrDayNames;

	private string[] m_ArrEngDays = new string[7] { "S", "M", "T", "W", "T", "F", "S" };

	private string m_YearMonthPattern;

	private string m_ShortDatePattern;

	private string m_DowLongDatePattern;

	private string m_MonthDayPattern;

	private int m_TwoDigitYearMax;

	private string[] m_EraName;

	private string[] m_AbbrEraName;

	private int m_Eras;

	private string[] m_TimeMarkHours = new string[24]
	{
		"12", "1", "2", "3", "4", "5", "6", "7", "8", "9",
		"10", "11", "12", "1", "2", "3", "4", "5", "6", "7",
		"8", "9", "10", "11"
	};

	private string[] m_TimeMarkHoursWithMinutes = new string[24];

	private string[] m_12Hours = new string[24]
	{
		"12", "1", "2", "3", "4", "5", "6", "7", "8", "9",
		"10", "11", "12", "1", "2", "3", "4", "5", "6", "7",
		"8", "9", "10", "11"
	};

	private string[] m_24Hours = new string[24]
	{
		"00", "01", "02", "03", "04", "05", "06", "07", "08", "09",
		"10", "11", "12", "13", "14", "15", "16", "17", "18", "19",
		"20", "21", "22", "23"
	};

	private string[] m_24HoursWithMinutes = new string[24];

	private string[] m_HebrewDayChars = new string[31]
	{
		" ", "א׳", "ב׳", "ג׳", "ד׳", "ה׳", "ו׳", "ז׳", "ח׳", "ט׳",
		"י׳", "י״א", "י״ב", "י״ג", "י״ד", "ט״ו", "ט״ז", "י״ז", "י״ח", "י״ט",
		"כ׳", "כ״א", "כ״ב", "כ״ג", "כ״ד", "כ״ה", "כ״ו", "כ״ז", "כ״ח", "כ״ט",
		"ל׳"
	};

	private string[] m_HebrewNumbers = new string[27]
	{
		"א", "ב", "ג", "ד", "ה", "ו", "ז", "ח", "ט", "י",
		"כ", "ך", "ל", "מ", "ם", "נ", "ן", "ס", "ע", "פ",
		"ף", "צ", "ץ", "ק", "ר", "ש", "ת"
	};

	private int[] m_HebrewNumbersToInt = new int[27]
	{
		1, 2, 3, 4, 5, 6, 7, 8, 9, 10,
		20, 20, 30, 40, 40, 50, 50, 60, 70, 80,
		80, 90, 90, 100, 200, 300, 400
	};

	private string[] m_HindiSakaMonthNames = new string[12]
	{
		"च\u0948त\u094dर", "व\u0948श\u093eख", "ज\u094dय\u0947ष\u094dट", "आष\u093eढ़", "श\u094dर\u093eवण", "भ\u093eद\u094dर", "आश\u094dव\u0940न", "क़\u093eर\u094dत\u0940क", "अग\u094dरह\u093eयन", "प\u094cस\u093e",
		"म\u093eघ", "फल\u094dग\u0941ण"
	};

	private SimpleDate m_startMonth;

	public string Lang => m_Lang;

	public Calendar Calendar => m_CultureInfo.DateTimeFormat.Calendar;

	public SPCalendarType CalendarType => m_CalendarType;

	public string AMDesignator => m_AMDesignator;

	public string PMDesignator => m_PMDesignator;

	public string TimeSeparator => m_TimeSeparator;

	public SimpleDate Today => m_Today;

	public SimpleDate StartMonth => m_startMonth;

	public SimpleDate SelectedDate => m_SelectedDate;

	public string[] DayNames => m_DayNames;

	public int FirstDayOfWeek => m_FirstDayOfWeek;

	public int HijriAdjustment => m_HijriAdjustment;

	public string ShortDatePattern => m_ShortDatePattern;

	public string MonthDayPattern => m_MonthDayPattern;

	public string TimePattern12Hour => m_TimePattern12Hour;

	public string TimePattern24Hour => m_TimePattern24Hour;

	public DateOptions(string localeId, SPCalendarType calendar, string workWeek, string firstDayOfWeek, string hijriAdjustment, string timeZoneSpan, string twoDigitYearMax, string selectedDate, string startMonth)
		: this(localeId, calendar, workWeek, firstDayOfWeek, hijriAdjustment, timeZoneSpan, selectedDate, startMonth)
	{
		if (twoDigitYearMax != null && twoDigitYearMax.Length != 0)
		{
			try
			{
				m_TwoDigitYearMax = int.Parse(twoDigitYearMax, CultureInfo.InvariantCulture);
			}
			catch (ArgumentException)
			{
				m_TwoDigitYearMax = m_CultureInfo.DateTimeFormat.Calendar.TwoDigitYearMax;
			}
		}
		else
		{
			m_TwoDigitYearMax = m_CultureInfo.DateTimeFormat.Calendar.TwoDigitYearMax;
		}
		if (!ParseShortDate(selectedDate, out m_SelectedDate) || !SPIntlCal.IsLocalDateValid(m_CalendarType, ref m_SelectedDate, m_HijriAdjustment))
		{
			m_SelectedDate = m_Today;
		}
	}

	public DateOptions(string localeId, SPCalendarType calendar, string workWeek, string firstDayOfWeek, string hijriAdjustment, string timeZoneSpan, string selectedDate, string startMonth)
	{
		if (localeId != null && localeId.Length != 0)
		{
			try
			{
				m_Lcid = int.Parse(localeId, CultureInfo.InvariantCulture);
				m_CultureInfo = new CultureInfo(m_Lcid, useUserOverride: false);
				m_Lang = m_CultureInfo.Name;
			}
			catch (ArgumentException)
			{
			}
		}
		if (m_CultureInfo == null)
		{
			m_CultureInfo = new CultureInfo("en-US");
			m_Lang = m_CultureInfo.Name;
			m_Lcid = m_CultureInfo.LCID;
		}
		m_LocaleCultureInfo = m_CultureInfo;
		m_TimeFormatPatterns = m_CultureInfo.DateTimeFormat.GetAllDateTimePatterns('t');
		string[] timeFormatPatterns = m_TimeFormatPatterns;
		foreach (string text in timeFormatPatterns)
		{
			if (m_TimePattern12Hour == null && text.IndexOf('t') != -1 && text.IndexOf('s') == -1)
			{
				m_TimePattern12Hour = text;
			}
			if (m_TimePattern24Hour == null && text.IndexOf('t') == -1 && text.IndexOf('s') == -1)
			{
				m_TimePattern24Hour = text;
			}
		}
		m_AMDesignator = m_CultureInfo.DateTimeFormat.AMDesignator;
		m_PMDesignator = m_CultureInfo.DateTimeFormat.PMDesignator;
		m_TimeSeparator = m_CultureInfo.DateTimeFormat.TimeSeparator;
		bool flag = m_TimePattern12Hour != null && m_TimePattern12Hour[0] == 't';
		for (int j = 0; j < 24; j++)
		{
			m_24HoursWithMinutes[j] = m_24Hours[j] + m_TimeSeparator + "00";
			m_24Hours[j] += m_TimeSeparator;
			if (j < 12)
			{
				m_TimeMarkHoursWithMinutes[j] = (flag ? (m_AMDesignator + " " + m_TimeMarkHours[j] + m_TimeSeparator + "00") : (m_TimeMarkHours[j] + m_TimeSeparator + "00 " + m_AMDesignator));
				m_TimeMarkHours[j] = (flag ? (m_AMDesignator + " " + m_TimeMarkHours[j]) : (m_TimeMarkHours[j] + " " + m_AMDesignator));
			}
			else
			{
				m_TimeMarkHoursWithMinutes[j] = (flag ? (m_PMDesignator + " " + m_TimeMarkHours[j] + m_TimeSeparator + "00") : (m_TimeMarkHours[j] + m_TimeSeparator + "00 " + m_PMDesignator));
				m_TimeMarkHours[j] = (flag ? (m_PMDesignator + " " + m_TimeMarkHours[j]) : (m_TimeMarkHours[j] + " " + m_PMDesignator));
			}
		}
		if (calendar != SPCalendarType.None)
		{
			SetCalendarTypeFromName(calendar);
		}
		else
		{
			SetCalendarTypeFromCulture();
		}
		m_DateSeparator = m_CultureInfo.DateTimeFormat.DateSeparator;
		if (m_CalendarType == SPCalendarType.Hebrew)
		{
			m_MonthNames = new string[12];
			m_AbbrMonthNames = new string[12];
			m_LeapMonthNames = new string[13];
			m_LeapAbbrMonthNames = new string[13];
			for (int k = 0; k < 12; k++)
			{
				DateTime dateTime = new DateTime(5764, k + 1, 1, m_CultureInfo.DateTimeFormat.Calendar);
				m_MonthNames[k] = dateTime.ToString("MMMM", m_CultureInfo.DateTimeFormat).Trim();
				m_AbbrMonthNames[k] = dateTime.ToString("MMM", m_CultureInfo.DateTimeFormat).Trim();
			}
			for (int l = 0; l < 13; l++)
			{
				DateTime dateTime = new DateTime(5765, l + 1, 1, m_CultureInfo.DateTimeFormat.Calendar);
				m_LeapMonthNames[l] = dateTime.ToString("MMMM", m_CultureInfo.DateTimeFormat).Trim();
				m_LeapAbbrMonthNames[l] = dateTime.ToString("MMM", m_CultureInfo.DateTimeFormat).Trim();
			}
		}
		else if (m_CalendarType == SPCalendarType.SakaEra)
		{
			m_MonthNames = m_HindiSakaMonthNames;
			m_AbbrMonthNames = m_HindiSakaMonthNames;
			m_LeapMonthNames = null;
			m_LeapAbbrMonthNames = null;
		}
		else
		{
			m_MonthNames = m_CultureInfo.DateTimeFormat.MonthNames;
			m_AbbrMonthNames = m_CultureInfo.DateTimeFormat.AbbreviatedMonthNames;
			m_LeapMonthNames = null;
			m_LeapAbbrMonthNames = null;
		}
		m_MonthGenitiveNames = m_CultureInfo.DateTimeFormat.MonthGenitiveNames;
		m_DayNames = m_CultureInfo.DateTimeFormat.DayNames;
		m_WorkWeek = new bool[7] { false, true, true, true, true, true, false };
		if (workWeek != null && workWeek.Length == 7)
		{
			for (int m = 0; m < 7; m++)
			{
				m_WorkWeek[m] = workWeek[m] == '1';
			}
		}
		m_FirstDayOfWeek = (int)m_CultureInfo.DateTimeFormat.FirstDayOfWeek;
		if (firstDayOfWeek != null && firstDayOfWeek.Length != 0)
		{
			try
			{
				m_FirstDayOfWeek = int.Parse(firstDayOfWeek, CultureInfo.InvariantCulture);
				if (m_FirstDayOfWeek < 0 || m_FirstDayOfWeek >= 7)
				{
					throw new ArgumentOutOfRangeException("firstDayOfWeek");
				}
			}
			catch (Exception)
			{
				throw new ArgumentOutOfRangeException("firstDayOfWeek");
			}
		}
		m_HijriAdjustment = 0;
		if (hijriAdjustment != null && hijriAdjustment.Length != 0)
		{
			try
			{
				m_HijriAdjustment = int.Parse(hijriAdjustment, CultureInfo.InvariantCulture);
				if (m_HijriAdjustment <= -3 || m_HijriAdjustment >= 3)
				{
					throw new ArgumentOutOfRangeException("hijriAdjustment");
				}
			}
			catch (Exception)
			{
				throw new ArgumentOutOfRangeException("hijriAdjustment");
			}
		}
		if (m_Lang.Length > 2 && m_Lang.Substring(0, 2) != "en")
		{
			m_AbbrDayNames = m_CultureInfo.DateTimeFormat.AbbreviatedDayNames;
			m_SuperShortAbbrDayNames = m_CultureInfo.DateTimeFormat.ShortestDayNames;
		}
		else
		{
			m_AbbrDayNames = m_ArrEngDays;
			m_SuperShortAbbrDayNames = m_ArrEngDays;
		}
		m_ShortAbbrDayNames = m_CultureInfo.DateTimeFormat.AbbreviatedDayNames;
		m_YearMonthPattern = m_CultureInfo.DateTimeFormat.YearMonthPattern;
		if (m_CalendarType == SPCalendarType.Hijri || m_CalendarType == SPCalendarType.GregorianMEFrench || m_CalendarType == SPCalendarType.GregorianArabic || m_CalendarType == SPCalendarType.GregorianXLITEnglish || m_CalendarType == SPCalendarType.GregorianXLITFrench)
		{
			m_ShortDatePattern = "dd/MM/yyyy";
			m_DateSeparator = "/";
		}
		else if (m_CalendarType == SPCalendarType.Hebrew)
		{
			m_ShortDatePattern = "dd MMMM yyyy";
			m_DateSeparator = " ";
		}
		else
		{
			m_ShortDatePattern = m_CultureInfo.DateTimeFormat.ShortDatePattern;
		}
		m_DowLongDatePattern = m_CultureInfo.DateTimeFormat.LongDatePattern;
		m_MonthDayPattern = m_CultureInfo.DateTimeFormat.MonthDayPattern;
		m_TwoDigitYearMax = m_CultureInfo.DateTimeFormat.Calendar.TwoDigitYearMax;
		m_Eras = m_CultureInfo.DateTimeFormat.Calendar.Eras.Length;
		m_EraName = new string[m_Eras + 1];
		m_AbbrEraName = new string[m_Eras + 1];
		m_EraName[0] = "";
		m_AbbrEraName[0] = "";
		for (int n = 1; n <= m_Eras; n++)
		{
			m_EraName[n] = m_CultureInfo.DateTimeFormat.GetEraName(n);
			m_AbbrEraName[n] = m_CultureInfo.DateTimeFormat.GetAbbreviatedEraName(n);
		}
		DateTime dateTime2 = DateTime.UtcNow;
		if (timeZoneSpan != null && timeZoneSpan.Length != 0)
		{
			try
			{
				TimeSpan timeSpan = new TimeSpan(0L);
				timeSpan = TimeSpan.Parse(timeZoneSpan);
				dateTime2 += timeSpan;
			}
			catch (Exception)
			{
				throw new ArgumentOutOfRangeException("timeZoneSpan");
			}
		}
		else
		{
			dateTime2 = DateTime.Now;
		}
		m_Today = new SimpleDate(dateTime2.Year, dateTime2.Month, dateTime2.Day);
		if (m_CalendarType != SPCalendarType.Gregorian)
		{
			int num = SPIntlCal.LocalToJulianDay(SPCalendarType.Gregorian, ref m_Today);
			SPIntlCal.JulianDayToLocal(m_CalendarType, num, ref m_Today, m_HijriAdjustment, num);
		}
		if (!ParseShortDate(startMonth, out m_startMonth) || !SPIntlCal.IsLocalDateValid(m_CalendarType, ref m_startMonth, m_HijriAdjustment))
		{
			m_startMonth = m_Today;
		}
		if (!ParseShortDate(selectedDate, out m_SelectedDate) || !SPIntlCal.IsLocalDateValid(m_CalendarType, ref m_SelectedDate, m_HijriAdjustment))
		{
			m_SelectedDate = m_Today;
		}
	}

	private void SetCalendarTypeFromName(SPCalendarType calendarType)
	{
		bool flag = true;
		m_CalendarType = calendarType;
		Calendar calendar;
		switch (m_CalendarType)
		{
		case SPCalendarType.Gregorian:
			calendar = new GregorianCalendar();
			break;
		case SPCalendarType.Japan:
			m_CultureInfo = new CultureInfo("ja-JP", useUserOverride: false);
			m_Lang = m_CultureInfo.Name;
			calendar = new JapaneseCalendar();
			break;
		case SPCalendarType.Taiwan:
			m_CultureInfo = new CultureInfo("zh-TW", useUserOverride: false);
			m_Lang = m_CultureInfo.Name;
			calendar = new TaiwanCalendar();
			break;
		case SPCalendarType.Korea:
			m_CultureInfo = new CultureInfo("ko-KR", useUserOverride: false);
			m_Lang = m_CultureInfo.Name;
			calendar = new KoreanCalendar();
			break;
		case SPCalendarType.Hijri:
			m_CultureInfo = new CultureInfo("ar-SA", useUserOverride: false);
			m_Lang = m_CultureInfo.Name;
			calendar = new HijriCalendar();
			break;
		case SPCalendarType.Thai:
			m_CultureInfo = new CultureInfo("th-TH", useUserOverride: false);
			m_Lang = m_CultureInfo.Name;
			calendar = new ThaiBuddhistCalendar();
			break;
		case SPCalendarType.Hebrew:
			m_CultureInfo = new CultureInfo("he-IL", useUserOverride: false);
			m_Lang = m_CultureInfo.Name;
			initializeHebrewNumberDict();
			calendar = new HebrewCalendar();
			break;
		case SPCalendarType.GregorianMEFrench:
			m_CultureInfo = new CultureInfo("ar-SA", useUserOverride: false);
			m_Lang = m_CultureInfo.Name;
			calendar = new GregorianCalendar(GregorianCalendarTypes.MiddleEastFrench);
			break;
		case SPCalendarType.GregorianArabic:
			m_CultureInfo = new CultureInfo("ar-SA", useUserOverride: false);
			m_Lang = m_CultureInfo.Name;
			calendar = new GregorianCalendar(GregorianCalendarTypes.Arabic);
			break;
		case SPCalendarType.GregorianXLITEnglish:
			m_CultureInfo = new CultureInfo("ar-JO", useUserOverride: false);
			m_Lang = m_CultureInfo.Name;
			calendar = new GregorianCalendar(GregorianCalendarTypes.TransliteratedEnglish);
			break;
		case SPCalendarType.GregorianXLITFrench:
			m_CultureInfo = new CultureInfo("ar-SA", useUserOverride: false);
			m_Lang = m_CultureInfo.Name;
			calendar = new GregorianCalendar(GregorianCalendarTypes.TransliteratedFrench);
			break;
		case SPCalendarType.KoreaJapanLunar:
			calendar = new GregorianCalendar();
			break;
		case SPCalendarType.ChineseLunar:
			calendar = new GregorianCalendar();
			break;
		case SPCalendarType.SakaEra:
			m_CultureInfo = new CultureInfo("hi-IN", useUserOverride: false);
			m_Lang = m_CultureInfo.Name;
			calendar = new GregorianCalendar();
			calendar.TwoDigitYearMax = 1960;
			break;
		default:
			m_CalendarType = SPCalendarType.Gregorian;
			calendar = new GregorianCalendar();
			break;
		}
		try
		{
			m_CultureInfo.DateTimeFormat.Calendar = calendar;
		}
		catch (Exception)
		{
			flag = false;
		}
		if (!flag)
		{
			SetCalendarTypeFromCulture();
		}
	}

	private void SetCalendarTypeFromCulture()
	{
		Type type = m_CultureInfo.Calendar.GetType();
		if (type == typeof(GregorianCalendar))
		{
			m_CalendarType = SPCalendarType.Gregorian;
		}
		else if (type == typeof(JapaneseCalendar))
		{
			m_CalendarType = SPCalendarType.Japan;
		}
		else if (type == typeof(TaiwanCalendar))
		{
			m_CalendarType = SPCalendarType.Taiwan;
		}
		else if (type == typeof(KoreanCalendar))
		{
			m_CalendarType = SPCalendarType.Korea;
		}
		else if (type == typeof(HijriCalendar))
		{
			m_CalendarType = SPCalendarType.Hijri;
		}
		else if (type == typeof(HebrewCalendar))
		{
			m_CalendarType = SPCalendarType.Hebrew;
		}
		else if (type == typeof(ThaiBuddhistCalendar))
		{
			m_CalendarType = SPCalendarType.Thai;
		}
		else
		{
			m_CalendarType = SPCalendarType.Gregorian;
		}
		m_CultureInfo.DateTimeFormat.Calendar = m_CultureInfo.Calendar;
	}

	public bool IsWorkDay(int iDay)
	{
		return m_WorkWeek[iDay % 7];
	}

	public string GetDowLongDateString(SimpleDate dt)
	{
		m_UseMonthGenitiveNames = true;
		return GetDateString(dt, m_DowLongDatePattern);
	}

	public string GetMonthDayDateString(SimpleDate dt)
	{
		return GetDateString(dt, m_MonthDayPattern);
	}

	public string GetTimeString(bool hoursMode24, DateTime startDate, DateTime endDate, string format)
	{
		return string.Format(CultureInfo.InvariantCulture, format, GetTimeStringFromPattern(startDate, hoursMode24), GetTimeStringFromPattern(endDate, hoursMode24));
	}

	public string[] GetHoursString(bool hoursMode24, bool hasMinutes)
	{
		if (hasMinutes)
		{
			if (!hoursMode24)
			{
				return m_TimeMarkHoursWithMinutes;
			}
			return m_24HoursWithMinutes;
		}
		if (!hoursMode24)
		{
			return m_TimeMarkHours;
		}
		return m_24Hours;
	}

	public string[] Get12Hours()
	{
		return m_12Hours;
	}

	public string GetMonthYearString(SimpleDate simpleDate)
	{
		string yearMonthPattern = m_YearMonthPattern;
		return GetDateString(simpleDate, yearMonthPattern);
	}

	public string GetYearString(SimpleDate simpleDate)
	{
		return GetDateString(simpleDate, m_CalendarType switch
		{
			SPCalendarType.Japan => "gg y'年'", 
			SPCalendarType.Taiwan => "gg yy'年'", 
			SPCalendarType.Korea => "gg yyyy'년'", 
			SPCalendarType.Gregorian => (m_Lcid != 1041) ? ((m_Lcid != 1028 && m_Lcid != 2052) ? ((m_Lcid != 1042) ? "yyyy" : "yyyy'년'") : "yy'年'") : "y'年'", 
			_ => "yyyy", 
		});
	}

	public string GetDayChar(int day)
	{
		return GetDayChar(day, m_CalendarType);
	}

	public string GetDayChar(int day, SPCalendarType calendartype)
	{
		if (calendartype == SPCalendarType.Hebrew)
		{
			if (day > 0 && day < 31)
			{
				return m_HebrewDayChars[day];
			}
			return " ";
		}
		return day.ToString(CultureInfo.InvariantCulture);
	}

	public string GetShortDateString(SimpleDate simpleDate)
	{
		string shortDatePattern = m_ShortDatePattern;
		return GetDateString(simpleDate, shortDatePattern);
	}

	public string GetTimeStringFromPattern(DateTime dt, bool hoursMode24)
	{
		string text = (hoursMode24 ? m_TimePattern24Hour : m_TimePattern12Hour);
		return dt.ToString(text, m_LocaleCultureInfo);
	}

	public string GetDateString(SimpleDate simpleDate, string format)
	{
		StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);
		bool flag = m_CalendarType == SPCalendarType.Hebrew;
		bool flag2 = m_CalendarType == SPCalendarType.Taiwan;
		bool flag3 = SPIntlCal.IsLocalYearLeap(m_CalendarType, simpleDate.Year);
		bool flag4 = false;
		simpleDate = fixYear(simpleDate);
		for (int i = 0; i < format.Length; i++)
		{
			char c = format[i];
			if (c == '\'')
			{
				flag4 = !flag4;
				continue;
			}
			if (flag4)
			{
				stringWriter.Write(c);
				continue;
			}
			switch (c)
			{
			case 'g':
			{
				int num7 = 1;
				while (++i < format.Length && (c = format[i]) == 'g')
				{
					num7++;
				}
				if (num7 == 2)
				{
					stringWriter.Write(m_EraName[simpleDate.Era]);
				}
				i--;
				break;
			}
			case 'y':
			{
				int num8 = 1;
				while (++i < format.Length && (c = format[i]) == 'y')
				{
					num8++;
				}
				switch (num8)
				{
				case 4:
					stringWriter.Write(flag ? integerToHebrewYear(simpleDate.Year) : simpleDate.Year.ToString("0000", CultureInfo.InvariantCulture));
					break;
				case 2:
					if (flag2)
					{
						stringWriter.Write(simpleDate.Year.ToString("##", CultureInfo.InvariantCulture));
					}
					else if (flag)
					{
						stringWriter.Write(integerToHebrewYear(simpleDate.Year));
					}
					else
					{
						stringWriter.Write((simpleDate.Year % 100).ToString("00", CultureInfo.InvariantCulture));
					}
					break;
				case 1:
					stringWriter.Write(simpleDate.Year.ToString("##", CultureInfo.InvariantCulture));
					break;
				}
				i--;
				break;
			}
			case 'M':
			{
				int num6 = 1;
				while (++i < format.Length && (c = format[i]) == 'M')
				{
					num6++;
				}
				switch (num6)
				{
				case 4:
					stringWriter.Write((flag3 && m_LeapMonthNames != null) ? m_LeapMonthNames[simpleDate.Month - 1] : ((m_UseMonthGenitiveNames && !flag) ? m_MonthGenitiveNames[simpleDate.Month - 1] : m_MonthNames[simpleDate.Month - 1]));
					break;
				case 3:
					stringWriter.Write((flag3 && m_LeapAbbrMonthNames != null) ? m_LeapAbbrMonthNames[simpleDate.Month - 1] : ((m_UseMonthGenitiveNames && !flag) ? m_MonthGenitiveNames[simpleDate.Month - 1] : m_AbbrMonthNames[simpleDate.Month - 1]));
					break;
				case 2:
					stringWriter.Write(simpleDate.Month.ToString("00", CultureInfo.InvariantCulture));
					break;
				case 1:
					stringWriter.Write(simpleDate.Month.ToString("##", CultureInfo.InvariantCulture));
					break;
				}
				i--;
				break;
			}
			case 'd':
			{
				int num = 1;
				while (++i < format.Length && (c = format[i]) == 'd')
				{
					num++;
				}
				switch (num)
				{
				case 4:
					if (!flag)
					{
						int num4 = SPIntlCal.LocalToJulianDay(m_CalendarType, ref simpleDate);
						int num5 = (num4 + 1) % 7;
						stringWriter.Write(m_DayNames[num5]);
					}
					else
					{
						i++;
					}
					break;
				case 3:
					if (!flag)
					{
						int num2 = SPIntlCal.LocalToJulianDay(m_CalendarType, ref simpleDate);
						int num3 = (num2 + 1) % 7;
						stringWriter.Write(m_AbbrDayNames[num3]);
					}
					else
					{
						i++;
					}
					break;
				case 2:
					stringWriter.Write(flag ? m_HebrewDayChars[simpleDate.Day] : simpleDate.Day.ToString("00", CultureInfo.InvariantCulture));
					break;
				case 1:
					stringWriter.Write(simpleDate.Day.ToString("##", CultureInfo.InvariantCulture));
					break;
				}
				i--;
				break;
			}
			case '/':
				stringWriter.Write(m_DateSeparator);
				break;
			default:
				stringWriter.Write(c);
				break;
			case '\n':
			case ':':
			case 'H':
			case 'f':
			case 'h':
			case 'm':
			case 's':
			case 't':
			case 'z':
				break;
			}
		}
		m_UseMonthGenitiveNames = false;
		return stringWriter.ToString();
	}

	public bool ParseShortDate(string inputDate, out SimpleDate selectedDate)
	{
		string shortDatePattern = m_ShortDatePattern;
		return ParseDate(inputDate, shortDatePattern, out selectedDate);
	}

	public bool ParseMonthDayDate(string inputDate, out SimpleDate selectedDate)
	{
		string monthDayPattern = m_MonthDayPattern;
		if (!ParseDate(inputDate, monthDayPattern, out selectedDate))
		{
			monthDayPattern = m_ShortDatePattern;
			return ParseDate(inputDate, monthDayPattern, out selectedDate);
		}
		return true;
	}

	public bool ParseDate(string inputDate, string format, out SimpleDate selectedDate)
	{
		selectedDate = new SimpleDate(m_Today.Year, m_Today.Month, m_Today.Day);
		bool flag = m_CalendarType == SPCalendarType.Hebrew;
		bool flag2 = m_CalendarType == SPCalendarType.Taiwan;
		bool flag3 = false;
		int num = -1;
		if (inputDate == null)
		{
			return false;
		}
		inputDate = inputDate.Replace("\u3000", " ");
		inputDate = inputDate.Trim();
		bool flag4 = false;
		int i = 0;
		for (int j = 0; j < format.Length; j++)
		{
			char c = format[j];
			if (c == '\'')
			{
				flag4 = !flag4;
				continue;
			}
			if (flag4)
			{
				if (i < inputDate.Length && inputDate[i++] == c)
				{
					continue;
				}
				return false;
			}
			switch (c)
			{
			case 'g':
			{
				int num10 = 1;
				while (++j < format.Length && format[j] == 'g')
				{
					num10++;
				}
				j--;
				if (num10 == 2)
				{
					int k;
					for (k = 1; k <= m_Eras; k++)
					{
						if (string.Compare(m_EraName[k], 0, inputDate, i, m_EraName[k].Length, ignoreCase: true, m_CultureInfo) == 0)
						{
							i += m_EraName[k].Length;
							selectedDate.Era = k;
							break;
						}
						if (string.Compare(m_AbbrEraName[k], 0, inputDate, i, m_AbbrEraName[k].Length, ignoreCase: true, m_CultureInfo) == 0)
						{
							i += m_AbbrEraName[k].Length;
							selectedDate.Era = k;
							break;
						}
					}
					if (k <= m_Eras)
					{
						continue;
					}
					return false;
				}
				return false;
			}
			case 'y':
			{
				int num6 = 1;
				while (++j < format.Length && format[j] == 'y')
				{
					num6++;
				}
				j--;
				int num7 = i;
				int num8;
				if (flag)
				{
					num8 = hebrewYearToInteger(inputDate.Substring(i, inputDate.Length - i));
					if (num8 == -1)
					{
						return false;
					}
					i += inputDate.Length - i;
				}
				else
				{
					for (; i < inputDate.Length && "0123456789".IndexOf(inputDate[i]) >= 0; i++)
					{
					}
					int num9 = i - num7;
					if (num9 < 1 || num9 > 4)
					{
						return false;
					}
					if (num6 == 4 && (num9 == 3 || num9 == 1))
					{
						return false;
					}
					if (num6 == 2 && num9 > 2 && !flag2)
					{
						return false;
					}
					num8 = int.Parse(inputDate.Substring(num7, num9), CultureInfo.InvariantCulture);
					if (num9 <= 2)
					{
						num8 += 100 * (m_TwoDigitYearMax / 100);
						if (num8 > m_TwoDigitYearMax)
						{
							num8 -= 100;
						}
					}
				}
				selectedDate.Year = num8;
				continue;
			}
			case 'M':
			{
				int num11 = 1;
				while (++j < format.Length && format[j] == 'M')
				{
					num11++;
				}
				j--;
				int num12 = -1;
				int num13 = -1;
				string[] array = null;
				string[] array2 = null;
				bool flag5 = false;
				switch (num11)
				{
				case 4:
					flag5 = true;
					array = m_MonthNames;
					array2 = m_LeapMonthNames;
					break;
				case 3:
					flag5 = true;
					array = m_AbbrMonthNames;
					array2 = m_LeapAbbrMonthNames;
					break;
				case 1:
				case 2:
				{
					int num14 = i;
					for (; i < inputDate.Length && "0123456789".IndexOf(inputDate[i]) >= 0; i++)
					{
					}
					int num15 = i - num14;
					if (num15 < 1 || num15 > 2)
					{
						return false;
					}
					selectedDate.Month = int.Parse(inputDate.Substring(num14, num15), CultureInfo.InvariantCulture);
					break;
				}
				default:
					return false;
				}
				if (!flag5)
				{
					continue;
				}
				num13 = checkStringList(array, inputDate, i);
				if (array2 != null)
				{
					num12 = checkStringList(array2, inputDate, i);
				}
				if (num13 >= 0 && num12 >= 0)
				{
					if (array2[num12].Length > array[num13].Length)
					{
						num13 = -1;
					}
					else if (array[num13].Length > array2[num12].Length)
					{
						num12 = -1;
					}
				}
				else if (num13 < 0 && num12 < 0)
				{
					return false;
				}
				if (num13 >= 0)
				{
					selectedDate.Month = num13 + 1;
					i += array[num13].Length;
				}
				if (num12 >= 0)
				{
					num = num12 + 1;
					if (num13 < 0)
					{
						i += array2[num12].Length;
						flag3 = true;
					}
				}
				continue;
			}
			case 'd':
			{
				int num2 = 1;
				while (++j < format.Length && format[j] == 'd')
				{
					num2++;
				}
				j--;
				switch (num2)
				{
				case 3:
				case 4:
					if (flag)
					{
						for (inputDate = inputDate.Replace("\u00a0", " "); i < inputDate.Length && inputDate.Substring(i, m_DateSeparator.Length) != m_DateSeparator; i++)
						{
						}
						for (i++; i < inputDate.Length && inputDate.Substring(i, m_DateSeparator.Length) != m_DateSeparator; i++)
						{
						}
					}
					break;
				case 1:
				case 2:
				{
					int num3 = i;
					if (flag)
					{
						for (inputDate = inputDate.Replace("\u00a0", " "); i < inputDate.Length && inputDate.Substring(i, m_DateSeparator.Length) != m_DateSeparator; i++)
						{
						}
						int num4 = i - num3;
						if (num4 < 2 || num4 > 3)
						{
							return false;
						}
						inputDate = inputDate.Replace("\"", "״").Replace("'", "׳");
						selectedDate.Day = checkStringList(m_HebrewDayChars, inputDate, num3);
					}
					else
					{
						for (; i < inputDate.Length && "0123456789".IndexOf(inputDate[i]) >= 0; i++)
						{
						}
						int num5 = i - num3;
						if (num5 < 1 || num5 > 2)
						{
							return false;
						}
						selectedDate.Day = int.Parse(inputDate.Substring(num3, num5), CultureInfo.InvariantCulture);
					}
					break;
				}
				default:
					return false;
				}
				continue;
			}
			}
			if ((i < inputDate.Length && inputDate[i] != format[j]) || i > inputDate.Length)
			{
				return false;
			}
			if (i == inputDate.Length)
			{
				break;
			}
			i++;
		}
		if (i < inputDate.Length)
		{
			return false;
		}
		if (m_LeapMonthNames != null && num > 0)
		{
			if (SPIntlCal.IsLocalYearLeap(m_CalendarType, selectedDate.Year))
			{
				selectedDate.Month = num;
			}
			else if (flag3)
			{
				selectedDate.Month = -1;
			}
		}
		if (selectedDate.Month <= 0)
		{
			return false;
		}
		return true;
	}

	public bool ParseTime(string stTime, string format, ref DateTime dtTime)
	{
		stTime = stTime.Trim();
		int i = 0;
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		bool flag = true;
		bool flag2 = false;
		for (int j = 0; j < format.Length; j++)
		{
			char c = format[j];
			int num4 = i;
			if (c == 'h')
			{
				while (++j < format.Length && format[j] == 'h')
				{
				}
				j--;
				for (; i < stTime.Length && "0123456789".IndexOf(stTime[i]) >= 0; i++)
				{
				}
				int length = i - num4;
				num = int.Parse(stTime.Substring(num4, length), CultureInfo.InvariantCulture);
				if (num > 12 || num < 1)
				{
					return false;
				}
				flag = true;
			}
			else if (c == 'H')
			{
				while (++j < format.Length && format[j] == 'H')
				{
				}
				j--;
				for (; i < stTime.Length && "0123456789".IndexOf(stTime[i]) >= 0; i++)
				{
				}
				int length2 = i - num4;
				num = int.Parse(stTime.Substring(num4, length2), CultureInfo.InvariantCulture);
				if (num > 23 || num < 0)
				{
					return false;
				}
				flag = false;
			}
			else if (c == 'm')
			{
				while (++j < format.Length && format[j] == 'm')
				{
				}
				j--;
				for (; i < stTime.Length && "0123456789".IndexOf(stTime[i]) >= 0; i++)
				{
				}
				int length3 = i - num4;
				num2 = int.Parse(stTime.Substring(num4, length3), CultureInfo.InvariantCulture);
				if (num2 > 59 || num2 < 0)
				{
					return false;
				}
			}
			else if (c == 's')
			{
				while (++j < format.Length && format[j] == 's')
				{
				}
				j--;
				for (; i < stTime.Length && "0123456789".IndexOf(stTime[i]) >= 0; i++)
				{
				}
				int length4 = i - num4;
				num3 = int.Parse(stTime.Substring(num4, length4), CultureInfo.InvariantCulture);
				if (num3 > 59 || num3 < 0)
				{
					return false;
				}
			}
			else if (c == 't')
			{
				while (++j < format.Length && format[j] == 't')
				{
				}
				j--;
				for (; i < stTime.Length && stTime[i] != ' '; i++)
				{
				}
				int length5 = i - num4;
				if ((num4 <= stTime.Length && m_AMDesignator == stTime.Substring(num4, length5)) || (num4 > stTime.Length && string.IsNullOrEmpty(m_AMDesignator)))
				{
					flag2 = false;
					continue;
				}
				if (num4 > stTime.Length || !(m_PMDesignator == stTime.Substring(num4, length5)))
				{
					return false;
				}
				flag2 = true;
			}
			else
			{
				i++;
			}
		}
		dtTime = new DateTime(dtTime.Year, dtTime.Month, dtTime.Day, (flag && flag2) ? (num + 12) : num, num2, num3);
		return true;
	}

	private int checkStringList(string[] list, string str, int off)
	{
		int result = -1;
		int num = 0;
		for (int i = 0; i < list.Length; i++)
		{
			int length = list[i].Length;
			if (string.Compare(list[i], 0, str, off, length, ignoreCase: true, m_CultureInfo) == 0 && length > num)
			{
				result = i;
				num = length;
			}
		}
		return result;
	}

	private void initializeHebrewNumberDict()
	{
		if (m_HebrewNumberToIntDict == null || m_IntToHebrewNumberDict == null)
		{
			m_HebrewNumberToIntDict = new Hashtable();
			m_IntToHebrewNumberDict = new Hashtable();
			int num = 0;
			string[] hebrewNumbers = m_HebrewNumbers;
			foreach (string text in hebrewNumbers)
			{
				m_HebrewNumberToIntDict[text] = m_HebrewNumbersToInt[num];
				m_IntToHebrewNumberDict[m_HebrewNumbersToInt[num]] = text;
				num++;
			}
		}
	}

	private int hebrewYearToInteger(string str)
	{
		int num = 5000;
		for (int i = 0; i < str.Length; i++)
		{
			string text = str.Substring(i, 1);
			switch (text)
			{
			case " ":
			case "״":
			case "׳":
				continue;
			}
			object obj = m_HebrewNumberToIntDict[text];
			if (obj == null)
			{
				return -1;
			}
			num += (int)obj;
		}
		return num;
	}

	private string integerToHebrewYear(int num)
	{
		int[] array = new int[22]
		{
			400, 300, 200, 100, 90, 80, 70, 60, 50, 40,
			30, 20, 10, 9, 8, 7, 6, 5, 4, 3,
			2, 1
		};
		string text = "";
		num -= 5000;
		int[] array2 = array;
		foreach (int num2 in array2)
		{
			if (num <= 0)
			{
				break;
			}
			for (; num - num2 >= 0; text += m_IntToHebrewNumberDict[num2], num -= num2)
			{
				switch (num)
				{
				case 15:
					text = string.Concat(text, m_IntToHebrewNumberDict[9], m_IntToHebrewNumberDict[6]);
					num -= 15;
					break;
				case 16:
					text = string.Concat(text, m_IntToHebrewNumberDict[9], m_IntToHebrewNumberDict[7]);
					num -= 16;
					break;
				default:
					continue;
				}
				break;
			}
		}
		return text.Insert(text.Length - 1, "\"");
	}

	public string[] GetDaysAbbreviation()
	{
		return m_AbbrDayNames;
	}

	public SimpleDate fixYear(SimpleDate cDate)
	{
		SimpleDate di = cDate;
		if (CalendarType == SPCalendarType.Japan)
		{
			if (di.Year == 0 && di.Era > 1)
			{
				di.Year = SPIntlCal.EraOffset(CalendarType, di.Era) - SPIntlCal.EraOffset(CalendarType, di.Era - 1);
				di.Era--;
			}
			else if (di.Era >= 1 && di.Era <= 3 && di.Year == SPIntlCal.EraOffset(CalendarType, di.Era + 1) - SPIntlCal.EraOffset(CalendarType, di.Era) + 1)
			{
				int num = SPIntlCal.LocalToJulianDay(CalendarType, ref di);
				int eraJulianDay = SPIntlCal.GetEraJulianDay(CalendarType, di.Era);
				if (num >= eraJulianDay)
				{
					di.Era++;
					di.Year = 1;
				}
			}
			else if (di.Year == 1 && di.Era >= 2 && di.Era <= 4)
			{
				int num2 = SPIntlCal.LocalToJulianDay(CalendarType, ref di);
				int eraJulianDay2 = SPIntlCal.GetEraJulianDay(CalendarType, di.Era - 1);
				if (num2 < eraJulianDay2)
				{
					di.Era--;
					di.Year = SPIntlCal.EraOffset(CalendarType, di.Era + 1) - SPIntlCal.EraOffset(CalendarType, di.Era) + 1;
				}
			}
			try
			{
				SPIntlCal.MonthsInLocalYear(CalendarType, ref di);
			}
			catch (ArgumentOutOfRangeException)
			{
				di.Year = -1;
			}
		}
		return di;
	}

	private SimpleDate fixMonth(SimpleDate cDate)
	{
		SimpleDate di = cDate;
		while (true)
		{
			int num = SPIntlCal.MonthsInLocalYear(CalendarType, ref di);
			if (di.Month > num)
			{
				di.Month -= num;
				di.Year++;
			}
			else
			{
				if (di.Month >= 1)
				{
					break;
				}
				di.Year--;
				num = SPIntlCal.MonthsInLocalYear(CalendarType, ref di);
				di.Month += num;
			}
			di = fixYear(di);
		}
		return di;
	}

	private SimpleDate fixDays(SimpleDate cDate)
	{
		SimpleDate di = cDate;
		int num = SPIntlCal.DaysInLocalMonth(CalendarType, ref di, HijriAdjustment);
		if (di.Day > num)
		{
			di.Day = num;
		}
		return di;
	}

	public SimpleDate AddDays(SimpleDate cDate, int nDays)
	{
		SimpleDate di = cDate;
		int num = nDays;
		while (true)
		{
			int num2 = SPIntlCal.DaysInLocalMonth(CalendarType, ref di, HijriAdjustment);
			int day = di.Day;
			di.Day += num;
			if (di.Day > num2)
			{
				num -= num2 - day + 1;
				di.Day = 1;
				di.Month++;
				di = fixMonth(di);
				if (num < 1)
				{
					break;
				}
				continue;
			}
			if (di.Day >= 1)
			{
				break;
			}
			num += day;
			di.Month--;
			di.Day = 1;
			di = fixMonth(di);
			num2 = SPIntlCal.DaysInLocalMonth(CalendarType, ref di, HijriAdjustment);
			di.Day = num2;
			if (num >= 0)
			{
				break;
			}
		}
		return di;
	}

	public SimpleDate AddMonths(SimpleDate cDate, int nMonths)
	{
		SimpleDate di = cDate;
		int num = nMonths;
		while (true)
		{
			int num2 = SPIntlCal.MonthsInLocalYear(CalendarType, ref di);
			int month = di.Month;
			di.Month += num;
			if (di.Month > num2)
			{
				num -= num2 - month + 1;
				di.Month = 1;
				di.Year++;
				di = fixYear(di);
				if (num < 1)
				{
					break;
				}
				continue;
			}
			if (di.Month < 1)
			{
				num += month;
				di.Month = 1;
				di.Year--;
				di = fixYear(di);
				num2 = SPIntlCal.MonthsInLocalYear(CalendarType, ref di);
				di.Month = num2;
				if (num >= 0)
				{
					break;
				}
				continue;
			}
			di = fixYear(di);
			break;
		}
		return fixDays(di);
	}

	public SimpleDate AddYears(SimpleDate cDate, int nYears)
	{
		SimpleDate di = cDate;
		while (nYears != 0)
		{
			if (nYears > 0)
			{
				di.Year++;
				nYears--;
			}
			else
			{
				di.Year--;
				nYears++;
			}
			di = fixYear(di);
		}
		int num = SPIntlCal.MonthsInLocalYear(CalendarType, ref di);
		if (di.Month > num)
		{
			di.Month = num;
		}
		return fixDays(di);
	}

	public string[] GetShortDayAbbreviation()
	{
		return m_ShortAbbrDayNames;
	}

	public string[] GetDaysSuperShortAbbreviation()
	{
		return m_SuperShortAbbrDayNames;
	}

	public string[] GetMonthNamesAbbreviation(SimpleDate dt)
	{
		if (m_CalendarType == SPCalendarType.Hebrew && SPIntlCal.IsLocalYearLeap(m_CalendarType, dt.Year) && m_LeapMonthNames != null)
		{
			return m_LeapAbbrMonthNames;
		}
		return m_AbbrMonthNames;
	}

	public string[] GetMonthNamesAbbreviation()
	{
		return m_AbbrMonthNames;
	}

	internal bool isEastAsiaCalendar()
	{
		if (m_CalendarType != SPCalendarType.Japan && m_CalendarType != SPCalendarType.Taiwan && m_CalendarType != SPCalendarType.Korea)
		{
			if (m_CalendarType == SPCalendarType.Gregorian)
			{
				if (m_Lcid != 1041 && m_Lcid != 1042 && m_Lcid != 1028)
				{
					return m_Lcid == 2052;
				}
				return true;
			}
			return false;
		}
		return true;
	}

	public string[] GetMonthNames(SimpleDate dt)
	{
		if (m_CalendarType == SPCalendarType.Hebrew && SPIntlCal.IsLocalYearLeap(m_CalendarType, dt.Year) && m_LeapMonthNames != null)
		{
			return m_LeapMonthNames;
		}
		return m_MonthNames;
	}
}
