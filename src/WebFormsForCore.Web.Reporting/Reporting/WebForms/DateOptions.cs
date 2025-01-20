
using System;
using System.Collections;
using System.Globalization;
using System.IO;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
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
    private string[] m_ArrEngDays = new string[7]
    {
      "S",
      "M",
      "T",
      "W",
      "T",
      "F",
      "S"
    };
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
      "12",
      "1",
      "2",
      "3",
      "4",
      "5",
      "6",
      "7",
      "8",
      "9",
      "10",
      "11",
      "12",
      "1",
      "2",
      "3",
      "4",
      "5",
      "6",
      "7",
      "8",
      "9",
      "10",
      "11"
    };
    private string[] m_TimeMarkHoursWithMinutes = new string[24];
    private string[] m_12Hours = new string[24]
    {
      "12",
      "1",
      "2",
      "3",
      "4",
      "5",
      "6",
      "7",
      "8",
      "9",
      "10",
      "11",
      "12",
      "1",
      "2",
      "3",
      "4",
      "5",
      "6",
      "7",
      "8",
      "9",
      "10",
      "11"
    };
    private string[] m_24Hours = new string[24]
    {
      "00",
      "01",
      "02",
      "03",
      "04",
      "05",
      "06",
      "07",
      "08",
      "09",
      "10",
      "11",
      "12",
      "13",
      "14",
      "15",
      "16",
      "17",
      "18",
      "19",
      "20",
      "21",
      "22",
      "23"
    };
    private string[] m_24HoursWithMinutes = new string[24];
    private string[] m_HebrewDayChars = new string[31]
    {
      " ",
      "א׳",
      "ב׳",
      "ג׳",
      "ד׳",
      "ה׳",
      "ו׳",
      "ז׳",
      "ח׳",
      "ט׳",
      "י׳",
      "י״א",
      "י״ב",
      "י״ג",
      "י״ד",
      "ט״ו",
      "ט״ז",
      "י״ז",
      "י״ח",
      "י״ט",
      "כ׳",
      "כ״א",
      "כ״ב",
      "כ״ג",
      "כ״ד",
      "כ״ה",
      "כ״ו",
      "כ״ז",
      "כ״ח",
      "כ״ט",
      "ל׳"
    };
    private string[] m_HebrewNumbers = new string[27]
    {
      "א",
      "ב",
      "ג",
      "ד",
      "ה",
      "ו",
      "ז",
      "ח",
      "ט",
      "י",
      "כ",
      "ך",
      "ל",
      "מ",
      "ם",
      "נ",
      "ן",
      "ס",
      "ע",
      "פ",
      "ף",
      "צ",
      "ץ",
      "ק",
      "ר",
      "ש",
      "ת"
    };
    private int[] m_HebrewNumbersToInt = new int[27]
    {
      1,
      2,
      3,
      4,
      5,
      6,
      7,
      8,
      9,
      10,
      20,
      20,
      30,
      40,
      40,
      50,
      50,
      60,
      70,
      80,
      80,
      90,
      90,
      100,
      200,
      300,
      400
    };
    private string[] m_HindiSakaMonthNames = new string[12]
    {
      "चैत्र",
      "वैशाख",
      "ज्येष्ट",
      "आषाढ़",
      "श्रावण",
      "भाद्र",
      "आश्वीन",
      "क़ार्तीक",
      "अग्रहायन",
      "पौसा",
      "माघ",
      "फल्गुण"
    };
    private SimpleDate m_startMonth;

    public DateOptions(
      string localeId,
      SPCalendarType calendar,
      string workWeek,
      string firstDayOfWeek,
      string hijriAdjustment,
      string timeZoneSpan,
      string twoDigitYearMax,
      string selectedDate,
      string startMonth)
      : this(localeId, calendar, workWeek, firstDayOfWeek, hijriAdjustment, timeZoneSpan, selectedDate, startMonth)
    {
      if (twoDigitYearMax != null)
      {
        if (twoDigitYearMax.Length != 0)
        {
          try
          {
            this.m_TwoDigitYearMax = int.Parse(twoDigitYearMax, (IFormatProvider) CultureInfo.InvariantCulture);
            goto label_5;
          }
          catch (ArgumentException ex)
          {
            this.m_TwoDigitYearMax = this.m_CultureInfo.DateTimeFormat.Calendar.TwoDigitYearMax;
            goto label_5;
          }
        }
      }
      this.m_TwoDigitYearMax = this.m_CultureInfo.DateTimeFormat.Calendar.TwoDigitYearMax;
label_5:
      if (this.ParseShortDate(selectedDate, out this.m_SelectedDate) && SPIntlCal.IsLocalDateValid(this.m_CalendarType, ref this.m_SelectedDate, this.m_HijriAdjustment))
        return;
      this.m_SelectedDate = this.m_Today;
    }

    public DateOptions(
      string localeId,
      SPCalendarType calendar,
      string workWeek,
      string firstDayOfWeek,
      string hijriAdjustment,
      string timeZoneSpan,
      string selectedDate,
      string startMonth)
    {
      if (localeId != null)
      {
        if (localeId.Length != 0)
        {
          try
          {
            this.m_Lcid = int.Parse(localeId, (IFormatProvider) CultureInfo.InvariantCulture);
            this.m_CultureInfo = new CultureInfo(this.m_Lcid, false);
            this.m_Lang = this.m_CultureInfo.Name;
          }
          catch (ArgumentException ex)
          {
          }
        }
      }
      if (this.m_CultureInfo == null)
      {
        this.m_CultureInfo = new CultureInfo("en-US");
        this.m_Lang = this.m_CultureInfo.Name;
        this.m_Lcid = this.m_CultureInfo.LCID;
      }
      this.m_LocaleCultureInfo = this.m_CultureInfo;
      this.m_TimeFormatPatterns = this.m_CultureInfo.DateTimeFormat.GetAllDateTimePatterns('t');
      foreach (string timeFormatPattern in this.m_TimeFormatPatterns)
      {
        if (this.m_TimePattern12Hour == null && timeFormatPattern.IndexOf('t') != -1 && timeFormatPattern.IndexOf('s') == -1)
          this.m_TimePattern12Hour = timeFormatPattern;
        if (this.m_TimePattern24Hour == null && timeFormatPattern.IndexOf('t') == -1 && timeFormatPattern.IndexOf('s') == -1)
          this.m_TimePattern24Hour = timeFormatPattern;
      }
      this.m_AMDesignator = this.m_CultureInfo.DateTimeFormat.AMDesignator;
      this.m_PMDesignator = this.m_CultureInfo.DateTimeFormat.PMDesignator;
      this.m_TimeSeparator = this.m_CultureInfo.DateTimeFormat.TimeSeparator;
      bool flag = this.m_TimePattern12Hour != null && this.m_TimePattern12Hour[0] == 't';
      for (int index1 = 0; index1 < 24; ++index1)
      {
        this.m_24HoursWithMinutes[index1] = this.m_24Hours[index1] + this.m_TimeSeparator + "00";
        this.m_24Hours[index1] = this.m_24Hours[index1] + this.m_TimeSeparator;
        if (index1 < 12)
        {
          string[] hoursWithMinutes = this.m_TimeMarkHoursWithMinutes;
          int index2 = index1;
          string str;
          if (!flag)
            str = this.m_TimeMarkHours[index1] + this.m_TimeSeparator + "00 " + this.m_AMDesignator;
          else
            str = this.m_AMDesignator + " " + this.m_TimeMarkHours[index1] + this.m_TimeSeparator + "00";
          hoursWithMinutes[index2] = str;
          this.m_TimeMarkHours[index1] = flag ? this.m_AMDesignator + " " + this.m_TimeMarkHours[index1] : this.m_TimeMarkHours[index1] + " " + this.m_AMDesignator;
        }
        else
        {
          string[] hoursWithMinutes = this.m_TimeMarkHoursWithMinutes;
          int index3 = index1;
          string str;
          if (!flag)
            str = this.m_TimeMarkHours[index1] + this.m_TimeSeparator + "00 " + this.m_PMDesignator;
          else
            str = this.m_PMDesignator + " " + this.m_TimeMarkHours[index1] + this.m_TimeSeparator + "00";
          hoursWithMinutes[index3] = str;
          this.m_TimeMarkHours[index1] = flag ? this.m_PMDesignator + " " + this.m_TimeMarkHours[index1] : this.m_TimeMarkHours[index1] + " " + this.m_PMDesignator;
        }
      }
      if (calendar != SPCalendarType.None)
        this.SetCalendarTypeFromName(calendar);
      else
        this.SetCalendarTypeFromCulture();
      this.m_DateSeparator = this.m_CultureInfo.DateTimeFormat.DateSeparator;
      if (this.m_CalendarType == SPCalendarType.Hebrew)
      {
        this.m_MonthNames = new string[12];
        this.m_AbbrMonthNames = new string[12];
        this.m_LeapMonthNames = new string[13];
        this.m_LeapAbbrMonthNames = new string[13];
        DateTime dateTime;
        for (int index = 0; index < 12; ++index)
        {
          dateTime = new DateTime(5764, index + 1, 1, this.m_CultureInfo.DateTimeFormat.Calendar);
          this.m_MonthNames[index] = dateTime.ToString("MMMM", (IFormatProvider) this.m_CultureInfo.DateTimeFormat).Trim();
          this.m_AbbrMonthNames[index] = dateTime.ToString("MMM", (IFormatProvider) this.m_CultureInfo.DateTimeFormat).Trim();
        }
        for (int index = 0; index < 13; ++index)
        {
          dateTime = new DateTime(5765, index + 1, 1, this.m_CultureInfo.DateTimeFormat.Calendar);
          this.m_LeapMonthNames[index] = dateTime.ToString("MMMM", (IFormatProvider) this.m_CultureInfo.DateTimeFormat).Trim();
          this.m_LeapAbbrMonthNames[index] = dateTime.ToString("MMM", (IFormatProvider) this.m_CultureInfo.DateTimeFormat).Trim();
        }
      }
      else if (this.m_CalendarType == SPCalendarType.SakaEra)
      {
        this.m_MonthNames = this.m_HindiSakaMonthNames;
        this.m_AbbrMonthNames = this.m_HindiSakaMonthNames;
        this.m_LeapMonthNames = (string[]) null;
        this.m_LeapAbbrMonthNames = (string[]) null;
      }
      else
      {
        this.m_MonthNames = this.m_CultureInfo.DateTimeFormat.MonthNames;
        this.m_AbbrMonthNames = this.m_CultureInfo.DateTimeFormat.AbbreviatedMonthNames;
        this.m_LeapMonthNames = (string[]) null;
        this.m_LeapAbbrMonthNames = (string[]) null;
      }
      this.m_MonthGenitiveNames = this.m_CultureInfo.DateTimeFormat.MonthGenitiveNames;
      this.m_DayNames = this.m_CultureInfo.DateTimeFormat.DayNames;
      this.m_WorkWeek = new bool[7]
      {
        false,
        true,
        true,
        true,
        true,
        true,
        false
      };
      if (workWeek != null && workWeek.Length == 7)
      {
        for (int index = 0; index < 7; ++index)
          this.m_WorkWeek[index] = workWeek[index] == '1';
      }
      this.m_FirstDayOfWeek = (int) this.m_CultureInfo.DateTimeFormat.FirstDayOfWeek;
      if (firstDayOfWeek != null)
      {
        if (firstDayOfWeek.Length != 0)
        {
          try
          {
            this.m_FirstDayOfWeek = int.Parse(firstDayOfWeek, (IFormatProvider) CultureInfo.InvariantCulture);
            if (this.m_FirstDayOfWeek >= 0)
            {
              if (this.m_FirstDayOfWeek < 7)
                goto label_48;
            }
            throw new ArgumentOutOfRangeException(nameof (firstDayOfWeek));
          }
          catch (Exception ex)
          {
            throw new ArgumentOutOfRangeException(nameof (firstDayOfWeek));
          }
        }
      }
label_48:
      this.m_HijriAdjustment = 0;
      if (hijriAdjustment != null)
      {
        if (hijriAdjustment.Length != 0)
        {
          try
          {
            this.m_HijriAdjustment = int.Parse(hijriAdjustment, (IFormatProvider) CultureInfo.InvariantCulture);
            if (this.m_HijriAdjustment > -3)
            {
              if (this.m_HijriAdjustment < 3)
                goto label_55;
            }
            throw new ArgumentOutOfRangeException(nameof (hijriAdjustment));
          }
          catch (Exception ex)
          {
            throw new ArgumentOutOfRangeException(nameof (hijriAdjustment));
          }
        }
      }
label_55:
      if (this.m_Lang.Length > 2 && this.m_Lang.Substring(0, 2) != "en")
      {
        this.m_AbbrDayNames = this.m_CultureInfo.DateTimeFormat.AbbreviatedDayNames;
        this.m_SuperShortAbbrDayNames = this.m_CultureInfo.DateTimeFormat.ShortestDayNames;
      }
      else
      {
        this.m_AbbrDayNames = this.m_ArrEngDays;
        this.m_SuperShortAbbrDayNames = this.m_ArrEngDays;
      }
      this.m_ShortAbbrDayNames = this.m_CultureInfo.DateTimeFormat.AbbreviatedDayNames;
      this.m_YearMonthPattern = this.m_CultureInfo.DateTimeFormat.YearMonthPattern;
      if (this.m_CalendarType == SPCalendarType.Hijri || this.m_CalendarType == SPCalendarType.GregorianMEFrench || this.m_CalendarType == SPCalendarType.GregorianArabic || this.m_CalendarType == SPCalendarType.GregorianXLITEnglish || this.m_CalendarType == SPCalendarType.GregorianXLITFrench)
      {
        this.m_ShortDatePattern = "dd/MM/yyyy";
        this.m_DateSeparator = "/";
      }
      else if (this.m_CalendarType == SPCalendarType.Hebrew)
      {
        this.m_ShortDatePattern = "dd MMMM yyyy";
        this.m_DateSeparator = " ";
      }
      else
        this.m_ShortDatePattern = this.m_CultureInfo.DateTimeFormat.ShortDatePattern;
      this.m_DowLongDatePattern = this.m_CultureInfo.DateTimeFormat.LongDatePattern;
      this.m_MonthDayPattern = this.m_CultureInfo.DateTimeFormat.MonthDayPattern;
      this.m_TwoDigitYearMax = this.m_CultureInfo.DateTimeFormat.Calendar.TwoDigitYearMax;
      this.m_Eras = this.m_CultureInfo.DateTimeFormat.Calendar.Eras.Length;
      this.m_EraName = new string[this.m_Eras + 1];
      this.m_AbbrEraName = new string[this.m_Eras + 1];
      this.m_EraName[0] = "";
      this.m_AbbrEraName[0] = "";
      for (int era = 1; era <= this.m_Eras; ++era)
      {
        this.m_EraName[era] = this.m_CultureInfo.DateTimeFormat.GetEraName(era);
        this.m_AbbrEraName[era] = this.m_CultureInfo.DateTimeFormat.GetAbbreviatedEraName(era);
      }
      DateTime utcNow = DateTime.UtcNow;
      DateTime dateTime1;
      if (timeZoneSpan != null)
      {
        if (timeZoneSpan.Length != 0)
        {
          try
          {
            TimeSpan timeSpan = new TimeSpan(0L);
            timeSpan = TimeSpan.Parse(timeZoneSpan);
            dateTime1 = utcNow + timeSpan;
            goto label_71;
          }
          catch (Exception ex)
          {
            throw new ArgumentOutOfRangeException(nameof (timeZoneSpan));
          }
        }
      }
      dateTime1 = DateTime.Now;
label_71:
      this.m_Today = new SimpleDate(dateTime1.Year, dateTime1.Month, dateTime1.Day);
      if (this.m_CalendarType != SPCalendarType.Gregorian)
      {
        int julianDay = SPIntlCal.LocalToJulianDay(SPCalendarType.Gregorian, ref this.m_Today);
        SPIntlCal.JulianDayToLocal(this.m_CalendarType, julianDay, ref this.m_Today, this.m_HijriAdjustment, julianDay);
      }
      if (!this.ParseShortDate(startMonth, out this.m_startMonth) || !SPIntlCal.IsLocalDateValid(this.m_CalendarType, ref this.m_startMonth, this.m_HijriAdjustment))
        this.m_startMonth = this.m_Today;
      if (this.ParseShortDate(selectedDate, out this.m_SelectedDate) && SPIntlCal.IsLocalDateValid(this.m_CalendarType, ref this.m_SelectedDate, this.m_HijriAdjustment))
        return;
      this.m_SelectedDate = this.m_Today;
    }

    private void SetCalendarTypeFromName(SPCalendarType calendarType)
    {
      bool flag = true;
      this.m_CalendarType = calendarType;
      Calendar calendar;
      switch (this.m_CalendarType)
      {
        case SPCalendarType.Gregorian:
          calendar = (Calendar) new GregorianCalendar();
          break;
        case SPCalendarType.Japan:
          this.m_CultureInfo = new CultureInfo("ja-JP", false);
          this.m_Lang = this.m_CultureInfo.Name;
          calendar = (Calendar) new JapaneseCalendar();
          break;
        case SPCalendarType.Taiwan:
          this.m_CultureInfo = new CultureInfo("zh-TW", false);
          this.m_Lang = this.m_CultureInfo.Name;
          calendar = (Calendar) new TaiwanCalendar();
          break;
        case SPCalendarType.Korea:
          this.m_CultureInfo = new CultureInfo("ko-KR", false);
          this.m_Lang = this.m_CultureInfo.Name;
          calendar = (Calendar) new KoreanCalendar();
          break;
        case SPCalendarType.Hijri:
          this.m_CultureInfo = new CultureInfo("ar-SA", false);
          this.m_Lang = this.m_CultureInfo.Name;
          calendar = (Calendar) new HijriCalendar();
          break;
        case SPCalendarType.Thai:
          this.m_CultureInfo = new CultureInfo("th-TH", false);
          this.m_Lang = this.m_CultureInfo.Name;
          calendar = (Calendar) new ThaiBuddhistCalendar();
          break;
        case SPCalendarType.Hebrew:
          this.m_CultureInfo = new CultureInfo("he-IL", false);
          this.m_Lang = this.m_CultureInfo.Name;
          this.initializeHebrewNumberDict();
          calendar = (Calendar) new HebrewCalendar();
          break;
        case SPCalendarType.GregorianMEFrench:
          this.m_CultureInfo = new CultureInfo("ar-SA", false);
          this.m_Lang = this.m_CultureInfo.Name;
          calendar = (Calendar) new GregorianCalendar(GregorianCalendarTypes.MiddleEastFrench);
          break;
        case SPCalendarType.GregorianArabic:
          this.m_CultureInfo = new CultureInfo("ar-SA", false);
          this.m_Lang = this.m_CultureInfo.Name;
          calendar = (Calendar) new GregorianCalendar(GregorianCalendarTypes.Arabic);
          break;
        case SPCalendarType.GregorianXLITEnglish:
          this.m_CultureInfo = new CultureInfo("ar-JO", false);
          this.m_Lang = this.m_CultureInfo.Name;
          calendar = (Calendar) new GregorianCalendar(GregorianCalendarTypes.TransliteratedEnglish);
          break;
        case SPCalendarType.GregorianXLITFrench:
          this.m_CultureInfo = new CultureInfo("ar-SA", false);
          this.m_Lang = this.m_CultureInfo.Name;
          calendar = (Calendar) new GregorianCalendar(GregorianCalendarTypes.TransliteratedFrench);
          break;
        case SPCalendarType.KoreaJapanLunar:
          calendar = (Calendar) new GregorianCalendar();
          break;
        case SPCalendarType.ChineseLunar:
          calendar = (Calendar) new GregorianCalendar();
          break;
        case SPCalendarType.SakaEra:
          this.m_CultureInfo = new CultureInfo("hi-IN", false);
          this.m_Lang = this.m_CultureInfo.Name;
          calendar = (Calendar) new GregorianCalendar();
          calendar.TwoDigitYearMax = 1960;
          break;
        default:
          this.m_CalendarType = SPCalendarType.Gregorian;
          calendar = (Calendar) new GregorianCalendar();
          break;
      }
      try
      {
        this.m_CultureInfo.DateTimeFormat.Calendar = calendar;
      }
      catch (Exception ex)
      {
        flag = false;
      }
      if (flag)
        return;
      this.SetCalendarTypeFromCulture();
    }

    private void SetCalendarTypeFromCulture()
    {
      Type type = this.m_CultureInfo.Calendar.GetType();
      this.m_CalendarType = type != typeof (GregorianCalendar) ? (type != typeof (JapaneseCalendar) ? (type != typeof (TaiwanCalendar) ? (type != typeof (KoreanCalendar) ? (type != typeof (HijriCalendar) ? (type != typeof (HebrewCalendar) ? (type != typeof (ThaiBuddhistCalendar) ? SPCalendarType.Gregorian : SPCalendarType.Thai) : SPCalendarType.Hebrew) : SPCalendarType.Hijri) : SPCalendarType.Korea) : SPCalendarType.Taiwan) : SPCalendarType.Japan) : SPCalendarType.Gregorian;
      this.m_CultureInfo.DateTimeFormat.Calendar = this.m_CultureInfo.Calendar;
    }

    public string Lang => this.m_Lang;

    public Calendar Calendar => this.m_CultureInfo.DateTimeFormat.Calendar;

    public SPCalendarType CalendarType => this.m_CalendarType;

    public string AMDesignator => this.m_AMDesignator;

    public string PMDesignator => this.m_PMDesignator;

    public string TimeSeparator => this.m_TimeSeparator;

    public SimpleDate Today => this.m_Today;

    public SimpleDate StartMonth => this.m_startMonth;

    public SimpleDate SelectedDate => this.m_SelectedDate;

    public string[] DayNames => this.m_DayNames;

    public bool IsWorkDay(int iDay) => this.m_WorkWeek[iDay % 7];

    public int FirstDayOfWeek => this.m_FirstDayOfWeek;

    public int HijriAdjustment => this.m_HijriAdjustment;

    public string ShortDatePattern => this.m_ShortDatePattern;

    public string MonthDayPattern => this.m_MonthDayPattern;

    public string TimePattern12Hour => this.m_TimePattern12Hour;

    public string TimePattern24Hour => this.m_TimePattern24Hour;

    public string GetDowLongDateString(SimpleDate dt)
    {
      this.m_UseMonthGenitiveNames = true;
      return this.GetDateString(dt, this.m_DowLongDatePattern);
    }

    public string GetMonthDayDateString(SimpleDate dt)
    {
      return this.GetDateString(dt, this.m_MonthDayPattern);
    }

    public string GetTimeString(
      bool hoursMode24,
      DateTime startDate,
      DateTime endDate,
      string format)
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, format, (object) this.GetTimeStringFromPattern(startDate, hoursMode24), (object) this.GetTimeStringFromPattern(endDate, hoursMode24));
    }

    public string[] GetHoursString(bool hoursMode24, bool hasMinutes)
    {
      return hasMinutes ? (!hoursMode24 ? this.m_TimeMarkHoursWithMinutes : this.m_24HoursWithMinutes) : (!hoursMode24 ? this.m_TimeMarkHours : this.m_24Hours);
    }

    public string[] Get12Hours() => this.m_12Hours;

    public string GetMonthYearString(SimpleDate simpleDate)
    {
      string yearMonthPattern = this.m_YearMonthPattern;
      return this.GetDateString(simpleDate, yearMonthPattern);
    }

    public string GetYearString(SimpleDate simpleDate)
    {
      string format;
      switch (this.m_CalendarType)
      {
        case SPCalendarType.Gregorian:
          format = this.m_Lcid != 1041 ? (this.m_Lcid == 1028 || this.m_Lcid == 2052 ? "yy'年'" : (this.m_Lcid != 1042 ? "yyyy" : "yyyy'년'")) : "y'年'";
          break;
        case SPCalendarType.Japan:
          format = "gg y'年'";
          break;
        case SPCalendarType.Taiwan:
          format = "gg yy'年'";
          break;
        case SPCalendarType.Korea:
          format = "gg yyyy'년'";
          break;
        default:
          format = "yyyy";
          break;
      }
      return this.GetDateString(simpleDate, format);
    }

    public string GetDayChar(int day) => this.GetDayChar(day, this.m_CalendarType);

    public string GetDayChar(int day, SPCalendarType calendartype)
    {
      if (calendartype != SPCalendarType.Hebrew)
        return day.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      return day > 0 && day < 31 ? this.m_HebrewDayChars[day] : " ";
    }

    public string GetShortDateString(SimpleDate simpleDate)
    {
      string shortDatePattern = this.m_ShortDatePattern;
      return this.GetDateString(simpleDate, shortDatePattern);
    }

    public string GetTimeStringFromPattern(DateTime dt, bool hoursMode24)
    {
      string format = hoursMode24 ? this.m_TimePattern24Hour : this.m_TimePattern12Hour;
      return dt.ToString(format, (IFormatProvider) this.m_LocaleCultureInfo);
    }

    public string GetDateString(SimpleDate simpleDate, string format)
    {
      StringWriter stringWriter = new StringWriter((IFormatProvider) CultureInfo.InvariantCulture);
      bool flag1 = this.m_CalendarType == SPCalendarType.Hebrew;
      bool flag2 = this.m_CalendarType == SPCalendarType.Taiwan;
      bool flag3 = SPIntlCal.IsLocalYearLeap(this.m_CalendarType, simpleDate.Year);
      bool flag4 = false;
      simpleDate = this.fixYear(simpleDate);
      for (int index1 = 0; index1 < format.Length; ++index1)
      {
        char ch1 = format[index1];
        char ch2;
        if (ch1 == '\'')
          flag4 = !flag4;
        else if (flag4)
        {
          stringWriter.Write(ch1);
        }
        else
        {
          switch (ch1)
          {
            case '\n':
            case ':':
            case 'H':
            case 'f':
            case 'h':
            case 'm':
            case 's':
            case 't':
            case 'z':
              continue;
            case '/':
              stringWriter.Write(this.m_DateSeparator);
              continue;
            case 'M':
              int num1 = 1;
              while (++index1 < format.Length && (ch2 = format[index1]) == 'M')
                ++num1;
              switch (num1)
              {
                case 1:
                  stringWriter.Write(simpleDate.Month.ToString("##", (IFormatProvider) CultureInfo.InvariantCulture));
                  break;
                case 2:
                  stringWriter.Write(simpleDate.Month.ToString("00", (IFormatProvider) CultureInfo.InvariantCulture));
                  break;
                case 3:
                  stringWriter.Write(!flag3 || this.m_LeapAbbrMonthNames == null ? (!this.m_UseMonthGenitiveNames || flag1 ? this.m_AbbrMonthNames[simpleDate.Month - 1] : this.m_MonthGenitiveNames[simpleDate.Month - 1]) : this.m_LeapAbbrMonthNames[simpleDate.Month - 1]);
                  break;
                case 4:
                  stringWriter.Write(!flag3 || this.m_LeapMonthNames == null ? (!this.m_UseMonthGenitiveNames || flag1 ? this.m_MonthNames[simpleDate.Month - 1] : this.m_MonthGenitiveNames[simpleDate.Month - 1]) : this.m_LeapMonthNames[simpleDate.Month - 1]);
                  break;
              }
              --index1;
              continue;
            case 'd':
              int num2 = 1;
              while (++index1 < format.Length && (ch2 = format[index1]) == 'd')
                ++num2;
              switch (num2)
              {
                case 1:
                  stringWriter.Write(simpleDate.Day.ToString("##", (IFormatProvider) CultureInfo.InvariantCulture));
                  break;
                case 2:
                  stringWriter.Write(flag1 ? this.m_HebrewDayChars[simpleDate.Day] : simpleDate.Day.ToString("00", (IFormatProvider) CultureInfo.InvariantCulture));
                  break;
                case 3:
                  if (!flag1)
                  {
                    int index2 = (SPIntlCal.LocalToJulianDay(this.m_CalendarType, ref simpleDate) + 1) % 7;
                    stringWriter.Write(this.m_AbbrDayNames[index2]);
                    break;
                  }
                  ++index1;
                  break;
                case 4:
                  if (!flag1)
                  {
                    int index3 = (SPIntlCal.LocalToJulianDay(this.m_CalendarType, ref simpleDate) + 1) % 7;
                    stringWriter.Write(this.m_DayNames[index3]);
                    break;
                  }
                  ++index1;
                  break;
              }
              --index1;
              continue;
            case 'g':
              int num3 = 1;
              while (++index1 < format.Length && (ch2 = format[index1]) == 'g')
                ++num3;
              if (num3 == 2)
                stringWriter.Write(this.m_EraName[simpleDate.Era]);
              --index1;
              continue;
            case 'y':
              int num4 = 1;
              while (++index1 < format.Length && (ch2 = format[index1]) == 'y')
                ++num4;
              switch (num4)
              {
                case 1:
                  stringWriter.Write(simpleDate.Year.ToString("##", (IFormatProvider) CultureInfo.InvariantCulture));
                  break;
                case 2:
                  if (flag2)
                  {
                    stringWriter.Write(simpleDate.Year.ToString("##", (IFormatProvider) CultureInfo.InvariantCulture));
                    break;
                  }
                  if (flag1)
                  {
                    stringWriter.Write(this.integerToHebrewYear(simpleDate.Year));
                    break;
                  }
                  stringWriter.Write((simpleDate.Year % 100).ToString("00", (IFormatProvider) CultureInfo.InvariantCulture));
                  break;
                case 4:
                  stringWriter.Write(flag1 ? this.integerToHebrewYear(simpleDate.Year) : simpleDate.Year.ToString("0000", (IFormatProvider) CultureInfo.InvariantCulture));
                  break;
              }
              --index1;
              continue;
            default:
              stringWriter.Write(ch1);
              continue;
          }
        }
      }
      this.m_UseMonthGenitiveNames = false;
      return stringWriter.ToString();
    }

    public bool ParseShortDate(string inputDate, out SimpleDate selectedDate)
    {
      string shortDatePattern = this.m_ShortDatePattern;
      return this.ParseDate(inputDate, shortDatePattern, out selectedDate);
    }

    public bool ParseMonthDayDate(string inputDate, out SimpleDate selectedDate)
    {
      string monthDayPattern = this.m_MonthDayPattern;
      if (this.ParseDate(inputDate, monthDayPattern, out selectedDate))
        return true;
      string shortDatePattern = this.m_ShortDatePattern;
      return this.ParseDate(inputDate, shortDatePattern, out selectedDate);
    }

    public bool ParseDate(string inputDate, string format, out SimpleDate selectedDate)
    {
      selectedDate = new SimpleDate(this.m_Today.Year, this.m_Today.Month, this.m_Today.Day);
      bool flag1 = this.m_CalendarType == SPCalendarType.Hebrew;
      bool flag2 = this.m_CalendarType == SPCalendarType.Taiwan;
      bool flag3 = false;
      int num1 = -1;
      if (inputDate == null)
        return false;
      inputDate = inputDate.Replace("　", " ");
      inputDate = inputDate.Trim();
      bool flag4 = false;
      int num2 = 0;
      for (int index1 = 0; index1 < format.Length; ++index1)
      {
        char ch = format[index1];
        if (ch == '\'')
          flag4 = !flag4;
        else if (flag4)
        {
          if (num2 >= inputDate.Length || (int) inputDate[num2++] != (int) ch)
            return false;
        }
        else
        {
          switch (ch)
          {
            case 'M':
              int num3 = 1;
              while (++index1 < format.Length && format[index1] == 'M')
                ++num3;
              --index1;
              int index2 = -1;
              string[] list1 = (string[]) null;
              string[] list2 = (string[]) null;
              bool flag5 = false;
              switch (num3)
              {
                case 1:
                case 2:
                  int startIndex1 = num2;
                  while (num2 < inputDate.Length && "0123456789".IndexOf(inputDate[num2]) >= 0)
                    ++num2;
                  int length1 = num2 - startIndex1;
                  if (length1 < 1 || length1 > 2)
                    return false;
                  selectedDate.Month = int.Parse(inputDate.Substring(startIndex1, length1), (IFormatProvider) CultureInfo.InvariantCulture);
                  break;
                case 3:
                  flag5 = true;
                  list1 = this.m_AbbrMonthNames;
                  list2 = this.m_LeapAbbrMonthNames;
                  break;
                case 4:
                  flag5 = true;
                  list1 = this.m_MonthNames;
                  list2 = this.m_LeapMonthNames;
                  break;
                default:
                  return false;
              }
              if (flag5)
              {
                int index3 = this.checkStringList(list1, inputDate, num2);
                if (list2 != null)
                  index2 = this.checkStringList(list2, inputDate, num2);
                if (index3 >= 0 && index2 >= 0)
                {
                  if (list2[index2].Length > list1[index3].Length)
                    index3 = -1;
                  else if (list1[index3].Length > list2[index2].Length)
                    index2 = -1;
                }
                else if (index3 < 0 && index2 < 0)
                  return false;
                if (index3 >= 0)
                {
                  selectedDate.Month = index3 + 1;
                  num2 += list1[index3].Length;
                }
                if (index2 >= 0)
                {
                  num1 = index2 + 1;
                  if (index3 < 0)
                  {
                    num2 += list2[index2].Length;
                    flag3 = true;
                    continue;
                  }
                  continue;
                }
                continue;
              }
              continue;
            case 'd':
              int num4 = 1;
              while (++index1 < format.Length && format[index1] == 'd')
                ++num4;
              --index1;
              switch (num4)
              {
                case 1:
                case 2:
                  int num5 = num2;
                  if (flag1)
                  {
                    inputDate = inputDate.Replace(" ", " ");
                    while (num2 < inputDate.Length && inputDate.Substring(num2, this.m_DateSeparator.Length) != this.m_DateSeparator)
                      ++num2;
                    int num6 = num2 - num5;
                    if (num6 < 2 || num6 > 3)
                      return false;
                    inputDate = inputDate.Replace("\"", "״").Replace("'", "׳");
                    selectedDate.Day = this.checkStringList(this.m_HebrewDayChars, inputDate, num5);
                    continue;
                  }
                  while (num2 < inputDate.Length && "0123456789".IndexOf(inputDate[num2]) >= 0)
                    ++num2;
                  int length2 = num2 - num5;
                  if (length2 < 1 || length2 > 2)
                    return false;
                  selectedDate.Day = int.Parse(inputDate.Substring(num5, length2), (IFormatProvider) CultureInfo.InvariantCulture);
                  continue;
                case 3:
                case 4:
                  if (flag1)
                  {
                    inputDate = inputDate.Replace(" ", " ");
                    while (num2 < inputDate.Length && inputDate.Substring(num2, this.m_DateSeparator.Length) != this.m_DateSeparator)
                      ++num2;
                    ++num2;
                    while (num2 < inputDate.Length && inputDate.Substring(num2, this.m_DateSeparator.Length) != this.m_DateSeparator)
                      ++num2;
                    continue;
                  }
                  continue;
                default:
                  return false;
              }
            case 'g':
              int num7 = 1;
              while (++index1 < format.Length && format[index1] == 'g')
                ++num7;
              --index1;
              if (num7 != 2)
                return false;
              int index4;
              for (index4 = 1; index4 <= this.m_Eras; ++index4)
              {
                if (string.Compare(this.m_EraName[index4], 0, inputDate, num2, this.m_EraName[index4].Length, true, this.m_CultureInfo) == 0)
                {
                  num2 += this.m_EraName[index4].Length;
                  selectedDate.Era = index4;
                  break;
                }
                if (string.Compare(this.m_AbbrEraName[index4], 0, inputDate, num2, this.m_AbbrEraName[index4].Length, true, this.m_CultureInfo) == 0)
                {
                  num2 += this.m_AbbrEraName[index4].Length;
                  selectedDate.Era = index4;
                  break;
                }
              }
              if (index4 > this.m_Eras)
                return false;
              continue;
            case 'y':
              int num8 = 1;
              while (++index1 < format.Length && format[index1] == 'y')
                ++num8;
              --index1;
              int startIndex2 = num2;
              int integer;
              if (flag1)
              {
                integer = this.hebrewYearToInteger(inputDate.Substring(num2, inputDate.Length - num2));
                if (integer == -1)
                  return false;
                num2 += inputDate.Length - num2;
              }
              else
              {
                while (num2 < inputDate.Length && "0123456789".IndexOf(inputDate[num2]) >= 0)
                  ++num2;
                int length3 = num2 - startIndex2;
                if (length3 < 1 || length3 > 4 || num8 == 4 && (length3 == 3 || length3 == 1) || num8 == 2 && length3 > 2 && !flag2)
                  return false;
                integer = int.Parse(inputDate.Substring(startIndex2, length3), (IFormatProvider) CultureInfo.InvariantCulture);
                if (length3 <= 2)
                {
                  integer += 100 * (this.m_TwoDigitYearMax / 100);
                  if (integer > this.m_TwoDigitYearMax)
                    integer -= 100;
                }
              }
              selectedDate.Year = integer;
              continue;
            default:
              if (num2 < inputDate.Length && (int) inputDate[num2] != (int) format[index1] || num2 > inputDate.Length)
                return false;
              if (num2 != inputDate.Length)
              {
                ++num2;
                continue;
              }
              goto label_96;
          }
        }
      }
label_96:
      if (num2 < inputDate.Length)
        return false;
      if (this.m_LeapMonthNames != null && num1 > 0)
      {
        if (SPIntlCal.IsLocalYearLeap(this.m_CalendarType, selectedDate.Year))
          selectedDate.Month = num1;
        else if (flag3)
          selectedDate.Month = -1;
      }
      return selectedDate.Month > 0;
    }

    public bool ParseTime(string stTime, string format, ref DateTime dtTime)
    {
      stTime = stTime.Trim();
      int index1 = 0;
      int num = 0;
      int minute = 0;
      int second = 0;
      bool flag1 = true;
      bool flag2 = false;
      for (int index2 = 0; index2 < format.Length; ++index2)
      {
        char ch = format[index2];
        int startIndex = index1;
        switch (ch)
        {
          case 'H':
            do
              ;
            while (++index2 < format.Length && format[index2] == 'H');
            --index2;
            while (index1 < stTime.Length && "0123456789".IndexOf(stTime[index1]) >= 0)
              ++index1;
            int length1 = index1 - startIndex;
            num = int.Parse(stTime.Substring(startIndex, length1), (IFormatProvider) CultureInfo.InvariantCulture);
            if (num > 23 || num < 0)
              return false;
            flag1 = false;
            break;
          case 'h':
            do
              ;
            while (++index2 < format.Length && format[index2] == 'h');
            --index2;
            while (index1 < stTime.Length && "0123456789".IndexOf(stTime[index1]) >= 0)
              ++index1;
            int length2 = index1 - startIndex;
            num = int.Parse(stTime.Substring(startIndex, length2), (IFormatProvider) CultureInfo.InvariantCulture);
            if (num > 12 || num < 1)
              return false;
            flag1 = true;
            break;
          case 'm':
            do
              ;
            while (++index2 < format.Length && format[index2] == 'm');
            --index2;
            while (index1 < stTime.Length && "0123456789".IndexOf(stTime[index1]) >= 0)
              ++index1;
            int length3 = index1 - startIndex;
            minute = int.Parse(stTime.Substring(startIndex, length3), (IFormatProvider) CultureInfo.InvariantCulture);
            if (minute > 59 || minute < 0)
              return false;
            break;
          case 's':
            do
              ;
            while (++index2 < format.Length && format[index2] == 's');
            --index2;
            while (index1 < stTime.Length && "0123456789".IndexOf(stTime[index1]) >= 0)
              ++index1;
            int length4 = index1 - startIndex;
            second = int.Parse(stTime.Substring(startIndex, length4), (IFormatProvider) CultureInfo.InvariantCulture);
            if (second > 59 || second < 0)
              return false;
            break;
          case 't':
            do
              ;
            while (++index2 < format.Length && format[index2] == 't');
            --index2;
            while (index1 < stTime.Length && stTime[index1] != ' ')
              ++index1;
            int length5 = index1 - startIndex;
            if (startIndex <= stTime.Length && this.m_AMDesignator == stTime.Substring(startIndex, length5) || startIndex > stTime.Length && string.IsNullOrEmpty(this.m_AMDesignator))
            {
              flag2 = false;
              break;
            }
            if (startIndex > stTime.Length || !(this.m_PMDesignator == stTime.Substring(startIndex, length5)))
              return false;
            flag2 = true;
            break;
          default:
            ++index1;
            break;
        }
      }
      dtTime = new DateTime(dtTime.Year, dtTime.Month, dtTime.Day, !flag1 || !flag2 ? num : num + 12, minute, second);
      return true;
    }

    private int checkStringList(string[] list, string str, int off)
    {
      int num1 = -1;
      int num2 = 0;
      for (int index = 0; index < list.Length; ++index)
      {
        int length = list[index].Length;
        if (string.Compare(list[index], 0, str, off, length, true, this.m_CultureInfo) == 0 && length > num2)
        {
          num1 = index;
          num2 = length;
        }
      }
      return num1;
    }

    private void initializeHebrewNumberDict()
    {
      if (this.m_HebrewNumberToIntDict != null && this.m_IntToHebrewNumberDict != null)
        return;
      this.m_HebrewNumberToIntDict = new Hashtable();
      this.m_IntToHebrewNumberDict = new Hashtable();
      int index = 0;
      foreach (string hebrewNumber in this.m_HebrewNumbers)
      {
        this.m_HebrewNumberToIntDict[(object) hebrewNumber] = (object) this.m_HebrewNumbersToInt[index];
        this.m_IntToHebrewNumberDict[(object) this.m_HebrewNumbersToInt[index]] = (object) hebrewNumber;
        ++index;
      }
    }

    private int hebrewYearToInteger(string str)
    {
      int integer = 5000;
      for (int startIndex = 0; startIndex < str.Length; ++startIndex)
      {
        string key = str.Substring(startIndex, 1);
        if (!(key == " ") && !(key == "״") && !(key == "׳"))
        {
          object obj = this.m_HebrewNumberToIntDict[(object) key];
          if (obj == null)
            return -1;
          integer += (int) obj;
        }
      }
      return integer;
    }

    private string integerToHebrewYear(int num)
    {
      int[] numArray1 = new int[22]
      {
        400,
        300,
        200,
        100,
        90,
        80,
        70,
        60,
        50,
        40,
        30,
        20,
        10,
        9,
        8,
        7,
        6,
        5,
        4,
        3,
        2,
        1
      };
      string str = "";
      num -= 5000;
      int[] numArray2 = numArray1;
label_8:
      for (int index = 0; index < numArray2.Length; ++index)
      {
        int key = numArray2[index];
        if (num > 0)
        {
          for (; num - key >= 0; num -= key)
          {
            switch (num)
            {
              case 15:
                str = str + this.m_IntToHebrewNumberDict[(object) 9] + this.m_IntToHebrewNumberDict[(object) 6];
                num -= 15;
                goto label_8;
              case 16:
                str = str + this.m_IntToHebrewNumberDict[(object) 9] + this.m_IntToHebrewNumberDict[(object) 7];
                num -= 16;
                goto label_8;
              default:
                str += (string) this.m_IntToHebrewNumberDict[(object) key];
                continue;
            }
          }
        }
        else
          break;
      }
      return str.Insert(str.Length - 1, "\"");
    }

    public string[] GetDaysAbbreviation() => this.m_AbbrDayNames;

    public SimpleDate fixYear(SimpleDate cDate)
    {
      SimpleDate di = cDate;
      if (this.CalendarType == SPCalendarType.Japan)
      {
        if (di.Year == 0 && di.Era > 1)
        {
          di.Year = SPIntlCal.EraOffset(this.CalendarType, di.Era) - SPIntlCal.EraOffset(this.CalendarType, di.Era - 1);
          --di.Era;
        }
        else if (di.Era >= 1 && di.Era <= 3 && di.Year == SPIntlCal.EraOffset(this.CalendarType, di.Era + 1) - SPIntlCal.EraOffset(this.CalendarType, di.Era) + 1)
        {
          if (SPIntlCal.LocalToJulianDay(this.CalendarType, ref di) >= SPIntlCal.GetEraJulianDay(this.CalendarType, di.Era))
          {
            ++di.Era;
            di.Year = 1;
          }
        }
        else if (di.Year == 1)
        {
          if (di.Era >= 2)
          {
            if (di.Era <= 4)
            {
              if (SPIntlCal.LocalToJulianDay(this.CalendarType, ref di) < SPIntlCal.GetEraJulianDay(this.CalendarType, di.Era - 1))
              {
                --di.Era;
                di.Year = SPIntlCal.EraOffset(this.CalendarType, di.Era + 1) - SPIntlCal.EraOffset(this.CalendarType, di.Era) + 1;
              }
            }
          }
        }
        try
        {
          SPIntlCal.MonthsInLocalYear(this.CalendarType, ref di);
        }
        catch (ArgumentOutOfRangeException ex)
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
        int num1 = SPIntlCal.MonthsInLocalYear(this.CalendarType, ref di);
        if (di.Month > num1)
        {
          di.Month -= num1;
          ++di.Year;
        }
        else if (di.Month < 1)
        {
          --di.Year;
          int num2 = SPIntlCal.MonthsInLocalYear(this.CalendarType, ref di);
          di.Month += num2;
        }
        else
          break;
        di = this.fixYear(di);
      }
      return di;
    }

    private SimpleDate fixDays(SimpleDate cDate)
    {
      SimpleDate di = cDate;
      int num = SPIntlCal.DaysInLocalMonth(this.CalendarType, ref di, this.HijriAdjustment);
      if (di.Day > num)
        di.Day = num;
      return di;
    }

    public SimpleDate AddDays(SimpleDate cDate, int nDays)
    {
      SimpleDate di = cDate;
      int num1 = nDays;
      do
      {
        int num2 = SPIntlCal.DaysInLocalMonth(this.CalendarType, ref di, this.HijriAdjustment);
        int day = di.Day;
        di.Day += num1;
        if (di.Day > num2)
        {
          num1 -= num2 - day + 1;
          di.Day = 1;
          ++di.Month;
          di = this.fixMonth(di);
          if (num1 < 1)
            break;
        }
        else if (di.Day < 1)
        {
          num1 += day;
          --di.Month;
          di.Day = 1;
          di = this.fixMonth(di);
          int num3 = SPIntlCal.DaysInLocalMonth(this.CalendarType, ref di, this.HijriAdjustment);
          di.Day = num3;
        }
        else
          break;
      }
      while (num1 < 0);
      return di;
    }

    public SimpleDate AddMonths(SimpleDate cDate, int nMonths)
    {
      SimpleDate di = cDate;
      int num1 = nMonths;
      do
      {
        int num2 = SPIntlCal.MonthsInLocalYear(this.CalendarType, ref di);
        int month = di.Month;
        di.Month += num1;
        if (di.Month > num2)
        {
          num1 -= num2 - month + 1;
          di.Month = 1;
          ++di.Year;
          di = this.fixYear(di);
          if (num1 < 1)
            break;
        }
        else if (di.Month < 1)
        {
          num1 += month;
          di.Month = 1;
          --di.Year;
          di = this.fixYear(di);
          int num3 = SPIntlCal.MonthsInLocalYear(this.CalendarType, ref di);
          di.Month = num3;
        }
        else
          goto label_5;
      }
      while (num1 < 0);
      goto label_6;
label_5:
      di = this.fixYear(di);
label_6:
      return this.fixDays(di);
    }

    public SimpleDate AddYears(SimpleDate cDate, int nYears)
    {
      SimpleDate di = cDate;
      while (nYears != 0)
      {
        if (nYears > 0)
        {
          ++di.Year;
          --nYears;
        }
        else
        {
          --di.Year;
          ++nYears;
        }
        di = this.fixYear(di);
      }
      int num = SPIntlCal.MonthsInLocalYear(this.CalendarType, ref di);
      if (di.Month > num)
        di.Month = num;
      return this.fixDays(di);
    }

    public string[] GetShortDayAbbreviation() => this.m_ShortAbbrDayNames;

    public string[] GetDaysSuperShortAbbreviation() => this.m_SuperShortAbbrDayNames;

    public string[] GetMonthNamesAbbreviation(SimpleDate dt)
    {
      return this.m_CalendarType == SPCalendarType.Hebrew && SPIntlCal.IsLocalYearLeap(this.m_CalendarType, dt.Year) && this.m_LeapMonthNames != null ? this.m_LeapAbbrMonthNames : this.m_AbbrMonthNames;
    }

    public string[] GetMonthNamesAbbreviation() => this.m_AbbrMonthNames;

    internal bool isEastAsiaCalendar()
    {
      if (this.m_CalendarType == SPCalendarType.Japan || this.m_CalendarType == SPCalendarType.Taiwan || this.m_CalendarType == SPCalendarType.Korea)
        return true;
      if (this.m_CalendarType != SPCalendarType.Gregorian)
        return false;
      return this.m_Lcid == 1041 || this.m_Lcid == 1042 || this.m_Lcid == 1028 || this.m_Lcid == 2052;
    }

    public string[] GetMonthNames(SimpleDate dt)
    {
      return this.m_CalendarType == SPCalendarType.Hebrew && SPIntlCal.IsLocalYearLeap(this.m_CalendarType, dt.Year) && this.m_LeapMonthNames != null ? this.m_LeapMonthNames : this.m_MonthNames;
    }
  }
}
