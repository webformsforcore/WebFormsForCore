// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.IntlDate
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

using System;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal sealed class IntlDate
  {
    private int m_Year;
    private int m_Month;
    private int m_Day;
    private int m_Era;
    private int m_Jday;
    private SPCalendarType m_CalendarType;

    public IntlDate(int year, int month, int day)
    {
      this.Init(year, month, day, SPCalendarType.Gregorian);
    }

    public IntlDate(int year, int month, int day, SPCalendarType calendarType)
    {
      this.Init(year, month, day, calendarType);
    }

    public IntlDate(int year, int month, int day, int era, SPCalendarType calendarType)
    {
      this.Init(year, month, day, era, calendarType);
    }

    public IntlDate(int julianDay) => this.Init(julianDay, SPCalendarType.Gregorian);

    public IntlDate(int julianDay, SPCalendarType calendarType)
    {
      this.Init(julianDay, calendarType);
    }

    public int Year
    {
      get => this.m_Year;
      set => this.Init(value, this.m_Month, this.m_Day, this.m_CalendarType);
    }

    public int Month
    {
      get => this.m_Month;
      set => this.Init(this.m_Year, value, this.m_Day, this.m_CalendarType);
    }

    public int Day
    {
      get => this.m_Day;
      set => this.Init(this.m_Year, this.m_Month, value, this.m_CalendarType);
    }

    public int Era
    {
      get => this.m_Era;
      set => this.Init(this.m_Year, this.m_Month, this.m_Day, value, this.m_CalendarType);
    }

    public int JDay
    {
      get => this.m_Jday;
      set => this.Init(value, this.m_CalendarType);
    }

    public SPCalendarType CalendarType
    {
      get => this.m_CalendarType;
      set => this.Init(this.m_Jday, value);
    }

    public bool IsYearLeap() => SPIntlCal.IsLocalYearLeap(this.m_CalendarType, this.m_Year);

    public int MonthsInYear()
    {
      SimpleDate di = new SimpleDate(this.m_Year, this.m_Month, this.m_Day, this.m_Era);
      return SPIntlCal.MonthsInLocalYear(this.m_CalendarType, ref di);
    }

    public int DaysInMonth()
    {
      SimpleDate di = new SimpleDate(this.m_Year, this.m_Month, this.m_Day, this.m_Era);
      return SPIntlCal.DaysInLocalMonth(this.m_CalendarType, ref di);
    }

    public int DayOfWeek => (this.m_Jday + 1) % 7;

    public void AddDays(int days) => this.JDay += days;

    internal void Init(int year, int month, int day, SPCalendarType calendarType)
    {
      this.Init(year, month, day, 1, calendarType);
    }

    internal void Init(int year, int month, int day, int era, SPCalendarType calendarType)
    {
      this.m_Year = year;
      this.m_Month = month;
      this.m_Day = day;
      this.m_Era = era;
      this.m_CalendarType = calendarType;
      SimpleDate di = new SimpleDate(this.m_Year, this.m_Month, this.m_Day, this.m_Era);
      this.m_Jday = SPIntlCal.IsLocalDateValid(calendarType, ref di) ? SPIntlCal.LocalToJulianDay(this.m_CalendarType, ref di) : throw new ArgumentOutOfRangeException(nameof (calendarType));
    }

    internal void Init(int julianDay, SPCalendarType calendarType)
    {
      SimpleDate di = new SimpleDate(0, 0, 0);
      SPIntlCal.JulianDayToLocal(calendarType, julianDay, ref di);
      this.m_Year = di.Year;
      this.m_Month = di.Month;
      this.m_Day = di.Day;
      this.m_Era = di.Era;
      this.m_CalendarType = calendarType;
      this.m_Jday = julianDay;
    }
  }
}
