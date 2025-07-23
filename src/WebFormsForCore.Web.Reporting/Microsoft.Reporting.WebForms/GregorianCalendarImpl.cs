namespace Microsoft.Reporting.WebForms;

internal class GregorianCalendarImpl : SolarCalendarImpl
{
	protected static short[,] _DaysAccumInMonths = new short[2, 13]
	{
		{
			0, 31, 59, 90, 120, 151, 181, 212, 243, 273,
			304, 334, 365
		},
		{
			0, 31, 60, 91, 121, 152, 182, 213, 244, 274,
			305, 335, 366
		}
	};

	internal static int DateToJulianDay(int year, int month, int day)
	{
		return YearToJulianDay(year) + _DaysAccumInMonths[SolarCalendarImpl.IsYearLeap(year) ? 1 : 0, month - 1] + day - 1;
	}

	internal static int YearToJulianDay(int year)
	{
		int num = year - 1601;
		return 365 * num + num / 4 - num / 100 + num / 400;
	}

	internal static void JulianDayToDate(int jDay, ref SimpleDate di)
	{
		di.Year = 1601 + (jDay * 400 + 600) / SolarCalendarImpl._DaysIn400Years;
		jDay -= YearToJulianDay(di.Year);
		bool flag;
		if (jDay < 0)
		{
			di.Year--;
			flag = SolarCalendarImpl.IsYearLeap(di.Year);
			jDay += (flag ? 366 : 365);
		}
		else
		{
			flag = SolarCalendarImpl.IsYearLeap(di.Year);
		}
		di.Month = 1 + (jDay >> 5);
		if (di.Month < SolarCalendarImpl._MonthsInYear && jDay >= _DaysAccumInMonths[flag ? 1 : 0, di.Month])
		{
			di.Month++;
		}
		di.Day = 1 + jDay - _DaysAccumInMonths[flag ? 1 : 0, di.Month - 1];
	}

	internal static int DaysInMonth(int year, int month)
	{
		bool flag = SolarCalendarImpl.IsYearLeap(year);
		return _DaysAccumInMonths[flag ? 1 : 0, month] - _DaysAccumInMonths[flag ? 1 : 0, month - 1];
	}
}
