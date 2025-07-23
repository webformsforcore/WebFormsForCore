namespace Microsoft.Reporting.WebForms;

internal class SPCalendarUtil
{
	internal static bool IsYearInRange(int year, int yearL, int yearH)
	{
		if (year >= yearL)
		{
			return year <= yearH;
		}
		return false;
	}

	internal static bool IsYearMonthInRange(int year, int month, int yearL, int monthL, int yearH, int monthH)
	{
		if (year > yearL || (year == yearL && month >= monthL))
		{
			if (year >= yearH)
			{
				if (year == yearH)
				{
					return month <= monthH;
				}
				return false;
			}
			return true;
		}
		return false;
	}

	internal static bool IsDateInRange(int year, int month, int day, int yearL, int monthL, int dayL, int yearH, int monthH, int dayH)
	{
		if (year > yearL || (year == yearL && (month > monthL || (month == monthL && day >= dayL))))
		{
			if (year >= yearH)
			{
				if (year == yearH)
				{
					if (month >= monthH)
					{
						if (month == monthH)
						{
							return day <= dayH;
						}
						return false;
					}
					return true;
				}
				return false;
			}
			return true;
		}
		return false;
	}
}
