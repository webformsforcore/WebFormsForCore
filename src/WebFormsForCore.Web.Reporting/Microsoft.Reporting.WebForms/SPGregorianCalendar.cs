using System;

namespace Microsoft.Reporting.WebForms;

internal class SPGregorianCalendar : ISPCalendar
{
	public virtual bool IsSupportedYear(int year)
	{
		return SPCalendarUtil.IsYearInRange(year, 1601, 8900);
	}

	public virtual bool IsSupportedMonth(int year, int month)
	{
		if (SPCalendarUtil.IsYearMonthInRange(year, month, 1601, 1, 8900, 12) && month >= 1)
		{
			return month <= SolarCalendarImpl.MonthsInYear();
		}
		return false;
	}

	public virtual bool IsSupportedDate(ref SimpleDate di)
	{
		if (SPCalendarUtil.IsDateInRange(di.Year, di.Month, di.Day, 1601, 1, 1, 8900, 12, 31) && di.Month >= 1)
		{
			return di.Month <= SolarCalendarImpl.MonthsInYear();
		}
		return false;
	}

	public virtual bool IsDateValid(ref SimpleDate di, int iAdvance, int jDayCurrent)
	{
		if (IsSupportedDate(ref di) && di.Day > 0)
		{
			if (di.Day >= 29)
			{
				return di.Day <= GregorianCalendarImpl.DaysInMonth(di.Year + GetEraOffset(di.Era), di.Month);
			}
			return true;
		}
		return false;
	}

	public virtual bool IsSupportedJulianDay(int JDay)
	{
		if (JDay >= 0)
		{
			return JDay <= 2666269;
		}
		return false;
	}

	public virtual int DateToJulianDay(ref SimpleDate di, int iAdvance, int jDayCurrent)
	{
		if (!IsSupportedDate(ref di))
		{
			throw new ArgumentOutOfRangeException("di");
		}
		return GregorianCalendarImpl.DateToJulianDay(di.Year + GetEraOffset(di.Era), di.Month, di.Day);
	}

	public virtual void JulianDayToDate(int jDay, ref SimpleDate di, int iAdvance, int jDayCurrent)
	{
		if (!IsSupportedJulianDay(jDay))
		{
			throw new ArgumentOutOfRangeException("jDay");
		}
		GregorianCalendarImpl.JulianDayToDate(jDay, ref di);
		di.Year -= GetEraOffset(di.Era);
		di.Era = 1;
	}

	public virtual bool IsYearLeap(int year)
	{
		return IsYearLeap(year, 1);
	}

	public virtual bool IsYearLeap(int year, int era)
	{
		if (!IsSupportedYear(year))
		{
			throw new ArgumentOutOfRangeException("year");
		}
		return SolarCalendarImpl.IsYearLeap(year + GetEraOffset(era));
	}

	public virtual int MonthsInYear(ref SimpleDate di)
	{
		if (!IsSupportedYear(di.Year))
		{
			throw new ArgumentOutOfRangeException("di");
		}
		return SolarCalendarImpl.MonthsInYear();
	}

	public virtual int DaysInMonth(ref SimpleDate di)
	{
		if (!IsSupportedMonth(di.Year, di.Month))
		{
			throw new ArgumentOutOfRangeException("di");
		}
		return GregorianCalendarImpl.DaysInMonth(di.Year + GetEraOffset(di.Era), di.Month);
	}

	public virtual int DaysInMonth(ref SimpleDate di, int iAdvance)
	{
		return DaysInMonth(ref di);
	}

	public virtual int GetEraOffset(int era)
	{
		return 0;
	}

	public virtual int GetEraJulianDay(int era)
	{
		return 1;
	}
}
