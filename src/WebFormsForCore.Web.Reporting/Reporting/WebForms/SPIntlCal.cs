
using System;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal sealed class SPIntlCal
  {
    public const int DaysInWeek = 7;
    public const int MaxJDay = 2666269;
    private static SPGregorianCalendar _GregorianCalendar = new SPGregorianCalendar();

    internal static SPGregorianCalendar GregorianCalendar => SPIntlCal._GregorianCalendar;

    private SPIntlCal()
    {
    }

    public static bool IsCalendarSupported(SPCalendarType calType)
    {
      try
      {
        SPIntlCal.GetLocalCalendar(calType);
        return true;
      }
      catch (Exception ex)
      {
      }
      return false;
    }

    public static ISPCalendar GetLocalCalendar(SPCalendarType calType)
    {
      return (ISPCalendar) SPIntlCal.GregorianCalendar;
    }

    public static bool IsSupportedLocalYear(SPCalendarType calType, int year)
    {
      return SPIntlCal.GetLocalCalendar(calType).IsSupportedYear(year);
    }

    public static bool IsSupportedLocalMonth(SPCalendarType calType, int year, int month)
    {
      return SPIntlCal.GetLocalCalendar(calType).IsSupportedMonth(year, month);
    }

    public static bool IsSupportedLocalDate(SPCalendarType calType, ref SimpleDate di)
    {
      return SPIntlCal.GetLocalCalendar(calType).IsSupportedDate(ref di);
    }

    public static bool IsLocalDateValid(SPCalendarType calType, ref SimpleDate di)
    {
      return SPIntlCal.GetLocalCalendar(calType).IsDateValid(ref di, 0, 0);
    }

    public static bool IsLocalDateValid(SPCalendarType calType, ref SimpleDate di, int iAdvance)
    {
      return SPIntlCal.GetLocalCalendar(calType).IsDateValid(ref di, iAdvance, 0);
    }

    public static bool IsLocalDateValid(
      SPCalendarType calType,
      ref SimpleDate di,
      int iAdvance,
      int jDayCurrent)
    {
      return SPIntlCal.GetLocalCalendar(calType).IsDateValid(ref di, iAdvance, jDayCurrent);
    }

    public static bool IsSupportedLocalJulianDay(SPCalendarType calType, int jDay)
    {
      return SPIntlCal.GetLocalCalendar(calType).IsSupportedJulianDay(jDay);
    }

    public static int LocalToJulianDay(SPCalendarType calType, ref SimpleDate di)
    {
      return SPIntlCal.GetLocalCalendar(calType).DateToJulianDay(ref di, 0, 0);
    }

    public static int LocalToJulianDay(SPCalendarType calType, ref SimpleDate di, int iAdvance)
    {
      return SPIntlCal.GetLocalCalendar(calType).DateToJulianDay(ref di, iAdvance, 0);
    }

    public static int LocalToJulianDay(
      SPCalendarType calType,
      ref SimpleDate di,
      int iAdvance,
      int jDayCurrent)
    {
      return SPIntlCal.GetLocalCalendar(calType).DateToJulianDay(ref di, iAdvance, jDayCurrent);
    }

    public static int LocalToJulianDay(SPCalendarType calType, IntlDate id)
    {
      SimpleDate di = new SimpleDate(id.Year, id.Month, id.Day, id.Era);
      return SPIntlCal.GetLocalCalendar(calType).DateToJulianDay(ref di, 0, 0);
    }

    public static void JulianDayToLocal(SPCalendarType calType, int jDay, ref SimpleDate di)
    {
      SPIntlCal.GetLocalCalendar(calType).JulianDayToDate(jDay, ref di, 0, 0);
    }

    public static void JulianDayToLocal(
      SPCalendarType calType,
      int jDay,
      ref SimpleDate di,
      int iAdvance)
    {
      SPIntlCal.GetLocalCalendar(calType).JulianDayToDate(jDay, ref di, iAdvance, 0);
    }

    public static void JulianDayToLocal(
      SPCalendarType calType,
      int jDay,
      ref SimpleDate di,
      int iAdvance,
      int jDayCurrent)
    {
      SPIntlCal.GetLocalCalendar(calType).JulianDayToDate(jDay, ref di, iAdvance, jDayCurrent);
    }

    public static void JulianDayToLocal(SPCalendarType calType, int jDay, IntlDate id)
    {
      SimpleDate di = new SimpleDate(0, 0, 0);
      SPIntlCal.GetLocalCalendar(calType).JulianDayToDate(jDay, ref di, 0, 0);
      id.Init(di.Year, di.Month, di.Day, di.Era, calType);
    }

    public static int EraOffset(SPCalendarType calType, int era)
    {
      return SPIntlCal.GetLocalCalendar(calType).GetEraOffset(era);
    }

    public static int GetEraJulianDay(SPCalendarType calType, int era)
    {
      return SPIntlCal.GetLocalCalendar(calType).GetEraJulianDay(era);
    }

    public static bool IsLocalYearLeap(SPCalendarType calType, int year)
    {
      return SPIntlCal.GetLocalCalendar(calType).IsYearLeap(year);
    }

    public static int MonthsInLocalYear(SPCalendarType calType, ref SimpleDate di)
    {
      return SPIntlCal.GetLocalCalendar(calType).MonthsInYear(ref di);
    }

    public static int DaysInLocalMonth(SPCalendarType calType, ref SimpleDate di)
    {
      return SPIntlCal.GetLocalCalendar(calType).DaysInMonth(ref di);
    }

    public static int DaysInLocalMonth(SPCalendarType calType, ref SimpleDate di, int iAdvance)
    {
      return SPIntlCal.GetLocalCalendar(calType).DaysInMonth(ref di, iAdvance);
    }

    public static int GetWeekNumber(
      SPCalendarType calType,
      SimpleDate di,
      int FirstDayOfWeek,
      short FirstWeekOfYear)
    {
      int day = di.Day;
      int month = di.Month;
      while (--month > 0)
      {
        SimpleDate di1 = new SimpleDate(di.Year, month, 1);
        day += SPIntlCal.DaysInLocalMonth(calType, ref di1);
      }
      SimpleDate di2 = new SimpleDate(di.Year, 1, 1, di.Era);
      int num = (SPIntlCal.LocalToJulianDay(calType, ref di2) + 1) % 7;
      int weekNumber = (day - 1) / 7 + 1;
      if (num < FirstDayOfWeek)
        num += 7;
      if (FirstDayOfWeek < 7 && FirstDayOfWeek >= 0 && (FirstWeekOfYear == (short) 2 && num > FirstDayOfWeek + 3 || FirstWeekOfYear == (short) 1 && num != FirstDayOfWeek))
        --weekNumber;
      return weekNumber;
    }
  }
}
