// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.SPGregorianCalendar
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

using System;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal class SPGregorianCalendar : ISPCalendar
  {
    public virtual bool IsSupportedYear(int year) => SPCalendarUtil.IsYearInRange(year, 1601, 8900);

    public virtual bool IsSupportedMonth(int year, int month)
    {
      return SPCalendarUtil.IsYearMonthInRange(year, month, 1601, 1, 8900, 12) && month >= 1 && month <= SolarCalendarImpl.MonthsInYear();
    }

    public virtual bool IsSupportedDate(ref SimpleDate di)
    {
      return SPCalendarUtil.IsDateInRange(di.Year, di.Month, di.Day, 1601, 1, 1, 8900, 12, 31) && di.Month >= 1 && di.Month <= SolarCalendarImpl.MonthsInYear();
    }

    public virtual bool IsDateValid(ref SimpleDate di, int iAdvance, int jDayCurrent)
    {
      if (!this.IsSupportedDate(ref di) || di.Day <= 0)
        return false;
      return di.Day < 29 || di.Day <= GregorianCalendarImpl.DaysInMonth(di.Year + this.GetEraOffset(di.Era), di.Month);
    }

    public virtual bool IsSupportedJulianDay(int JDay) => JDay >= 0 && JDay <= 2666269;

    public virtual int DateToJulianDay(ref SimpleDate di, int iAdvance, int jDayCurrent)
    {
      if (!this.IsSupportedDate(ref di))
        throw new ArgumentOutOfRangeException(nameof (di));
      return GregorianCalendarImpl.DateToJulianDay(di.Year + this.GetEraOffset(di.Era), di.Month, di.Day);
    }

    public virtual void JulianDayToDate(
      int jDay,
      ref SimpleDate di,
      int iAdvance,
      int jDayCurrent)
    {
      if (!this.IsSupportedJulianDay(jDay))
        throw new ArgumentOutOfRangeException(nameof (jDay));
      GregorianCalendarImpl.JulianDayToDate(jDay, ref di);
      di.Year -= this.GetEraOffset(di.Era);
      di.Era = 1;
    }

    public virtual bool IsYearLeap(int year) => this.IsYearLeap(year, 1);

    public virtual bool IsYearLeap(int year, int era)
    {
      return this.IsSupportedYear(year) ? SolarCalendarImpl.IsYearLeap(year + this.GetEraOffset(era)) : throw new ArgumentOutOfRangeException(nameof (year));
    }

    public virtual int MonthsInYear(ref SimpleDate di)
    {
      if (!this.IsSupportedYear(di.Year))
        throw new ArgumentOutOfRangeException(nameof (di));
      return SolarCalendarImpl.MonthsInYear();
    }

    public virtual int DaysInMonth(ref SimpleDate di)
    {
      if (!this.IsSupportedMonth(di.Year, di.Month))
        throw new ArgumentOutOfRangeException(nameof (di));
      return GregorianCalendarImpl.DaysInMonth(di.Year + this.GetEraOffset(di.Era), di.Month);
    }

    public virtual int DaysInMonth(ref SimpleDate di, int iAdvance) => this.DaysInMonth(ref di);

    public virtual int GetEraOffset(int era) => 0;

    public virtual int GetEraJulianDay(int era) => 1;
  }
}
