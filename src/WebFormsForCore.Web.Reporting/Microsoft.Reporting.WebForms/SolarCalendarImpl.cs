namespace Microsoft.Reporting.WebForms;

internal class SolarCalendarImpl
{
	protected static short _MonthsInYear = 12;

	protected static int _DaysIn400Years = 146097;

	internal static bool IsYearLeap(int year)
	{
		if (year % 400 != 0)
		{
			if (year % 100 != 0)
			{
				if (year % 4 != 0)
				{
					return false;
				}
				return true;
			}
			return false;
		}
		return true;
	}

	internal static int MonthsInYear()
	{
		return _MonthsInYear;
	}
}
