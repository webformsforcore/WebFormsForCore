
#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal class GregorianCalendarImpl : SolarCalendarImpl
  {
    protected static short[,] _DaysAccumInMonths = new short[2, 13]
    {
      {
        (short) 0,
        (short) 31,
        (short) 59,
        (short) 90,
        (short) 120,
        (short) 151,
        (short) 181,
        (short) 212,
        (short) 243,
        (short) 273,
        (short) 304,
        (short) 334,
        (short) 365
      },
      {
        (short) 0,
        (short) 31,
        (short) 60,
        (short) 91,
        (short) 121,
        (short) 152,
        (short) 182,
        (short) 213,
        (short) 244,
        (short) 274,
        (short) 305,
        (short) 335,
        (short) 366
      }
    };

    internal static int DateToJulianDay(int year, int month, int day)
    {
      return GregorianCalendarImpl.YearToJulianDay(year) + (int) GregorianCalendarImpl._DaysAccumInMonths[SolarCalendarImpl.IsYearLeap(year) ? 1 : 0, month - 1] + day - 1;
    }

    internal static int YearToJulianDay(int year)
    {
      int num = year - 1601;
      return 365 * num + num / 4 - num / 100 + num / 400;
    }

    internal static void JulianDayToDate(int jDay, ref SimpleDate di)
    {
      di.Year = 1601 + (jDay * 400 + 600) / SolarCalendarImpl._DaysIn400Years;
      jDay -= GregorianCalendarImpl.YearToJulianDay(di.Year);
      bool flag;
      if (jDay < 0)
      {
        --di.Year;
        flag = SolarCalendarImpl.IsYearLeap(di.Year);
        jDay += flag ? 366 : 365;
      }
      else
        flag = SolarCalendarImpl.IsYearLeap(di.Year);
      di.Month = 1 + (jDay >> 5);
      if (di.Month < (int) SolarCalendarImpl._MonthsInYear && jDay >= (int) GregorianCalendarImpl._DaysAccumInMonths[flag ? 1 : 0, di.Month])
        ++di.Month;
      di.Day = 1 + jDay - (int) GregorianCalendarImpl._DaysAccumInMonths[flag ? 1 : 0, di.Month - 1];
    }

    internal static int DaysInMonth(int year, int month)
    {
      bool flag = SolarCalendarImpl.IsYearLeap(year);
      return (int) GregorianCalendarImpl._DaysAccumInMonths[flag ? 1 : 0, month] - (int) GregorianCalendarImpl._DaysAccumInMonths[flag ? 1 : 0, month - 1];
    }
  }
}
