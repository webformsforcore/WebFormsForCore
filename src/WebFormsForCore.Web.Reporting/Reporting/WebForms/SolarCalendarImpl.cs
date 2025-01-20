
#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal class SolarCalendarImpl
  {
    protected static short _MonthsInYear = 12;
    protected static int _DaysIn400Years = 146097;

    internal static bool IsYearLeap(int year)
    {
      return year % 400 == 0 || year % 100 != 0 && year % 4 == 0;
    }

    internal static int MonthsInYear() => (int) SolarCalendarImpl._MonthsInYear;
  }
}
