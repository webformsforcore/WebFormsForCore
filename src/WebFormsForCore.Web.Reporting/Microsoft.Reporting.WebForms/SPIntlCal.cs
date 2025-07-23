using System;

namespace Microsoft.Reporting.WebForms;

internal sealed class SPIntlCal
{
	public const int DaysInWeek = 7;

	public const int MaxJDay = 2666269;

	private static SPGregorianCalendar _GregorianCalendar = new SPGregorianCalendar();

	internal static SPGregorianCalendar GregorianCalendar => _GregorianCalendar;

	private SPIntlCal()
	{
	}

	public static bool IsCalendarSupported(SPCalendarType calType)
	{
		try
		{
			GetLocalCalendar(calType);
			return true;
		}
		catch (Exception)
		{
		}
		return false;
	}

	public static ISPCalendar GetLocalCalendar(SPCalendarType calType)
	{
		return GregorianCalendar;
	}

	public static bool IsSupportedLocalYear(SPCalendarType calType, int year)
	{
		return GetLocalCalendar(calType).IsSupportedYear(year);
	}

	public static bool IsSupportedLocalMonth(SPCalendarType calType, int year, int month)
	{
		return GetLocalCalendar(calType).IsSupportedMonth(year, month);
	}

	public static bool IsSupportedLocalDate(SPCalendarType calType, ref SimpleDate di)
	{
		return GetLocalCalendar(calType).IsSupportedDate(ref di);
	}

	public static bool IsLocalDateValid(SPCalendarType calType, ref SimpleDate di)
	{
		return GetLocalCalendar(calType).IsDateValid(ref di, 0, 0);
	}

	public static bool IsLocalDateValid(SPCalendarType calType, ref SimpleDate di, int iAdvance)
	{
		return GetLocalCalendar(calType).IsDateValid(ref di, iAdvance, 0);
	}

	public static bool IsLocalDateValid(SPCalendarType calType, ref SimpleDate di, int iAdvance, int jDayCurrent)
	{
		return GetLocalCalendar(calType).IsDateValid(ref di, iAdvance, jDayCurrent);
	}

	public static bool IsSupportedLocalJulianDay(SPCalendarType calType, int jDay)
	{
		return GetLocalCalendar(calType).IsSupportedJulianDay(jDay);
	}

	public static int LocalToJulianDay(SPCalendarType calType, ref SimpleDate di)
	{
		return GetLocalCalendar(calType).DateToJulianDay(ref di, 0, 0);
	}

	public static int LocalToJulianDay(SPCalendarType calType, ref SimpleDate di, int iAdvance)
	{
		return GetLocalCalendar(calType).DateToJulianDay(ref di, iAdvance, 0);
	}

	public static int LocalToJulianDay(SPCalendarType calType, ref SimpleDate di, int iAdvance, int jDayCurrent)
	{
		return GetLocalCalendar(calType).DateToJulianDay(ref di, iAdvance, jDayCurrent);
	}

	public static int LocalToJulianDay(SPCalendarType calType, IntlDate id)
	{
		SimpleDate di = new SimpleDate(id.Year, id.Month, id.Day, id.Era);
		return GetLocalCalendar(calType).DateToJulianDay(ref di, 0, 0);
	}

	public static void JulianDayToLocal(SPCalendarType calType, int jDay, ref SimpleDate di)
	{
		GetLocalCalendar(calType).JulianDayToDate(jDay, ref di, 0, 0);
	}

	public static void JulianDayToLocal(SPCalendarType calType, int jDay, ref SimpleDate di, int iAdvance)
	{
		GetLocalCalendar(calType).JulianDayToDate(jDay, ref di, iAdvance, 0);
	}

	public static void JulianDayToLocal(SPCalendarType calType, int jDay, ref SimpleDate di, int iAdvance, int jDayCurrent)
	{
		GetLocalCalendar(calType).JulianDayToDate(jDay, ref di, iAdvance, jDayCurrent);
	}

	public static void JulianDayToLocal(SPCalendarType calType, int jDay, IntlDate id)
	{
		SimpleDate di = new SimpleDate(0, 0, 0);
		GetLocalCalendar(calType).JulianDayToDate(jDay, ref di, 0, 0);
		id.Init(di.Year, di.Month, di.Day, di.Era, calType);
	}

	public static int EraOffset(SPCalendarType calType, int era)
	{
		return GetLocalCalendar(calType).GetEraOffset(era);
	}

	public static int GetEraJulianDay(SPCalendarType calType, int era)
	{
		return GetLocalCalendar(calType).GetEraJulianDay(era);
	}

	public static bool IsLocalYearLeap(SPCalendarType calType, int year)
	{
		return GetLocalCalendar(calType).IsYearLeap(year);
	}

	public static int MonthsInLocalYear(SPCalendarType calType, ref SimpleDate di)
	{
		return GetLocalCalendar(calType).MonthsInYear(ref di);
	}

	public static int DaysInLocalMonth(SPCalendarType calType, ref SimpleDate di)
	{
		return GetLocalCalendar(calType).DaysInMonth(ref di);
	}

	public static int DaysInLocalMonth(SPCalendarType calType, ref SimpleDate di, int iAdvance)
	{
		return GetLocalCalendar(calType).DaysInMonth(ref di, iAdvance);
	}

	public static int GetWeekNumber(SPCalendarType calType, SimpleDate di, int FirstDayOfWeek, short FirstWeekOfYear)
	{
		int num = di.Day;
		int num2 = di.Month;
		while (--num2 > 0)
		{
			SimpleDate di2 = new SimpleDate(di.Year, num2, 1);
			num += DaysInLocalMonth(calType, ref di2);
		}
		SimpleDate di3 = new SimpleDate(di.Year, 1, 1, di.Era);
		int num3 = LocalToJulianDay(calType, ref di3);
		int num4 = (num3 + 1) % 7;
		int num5 = (num - 1) / 7 + 1;
		if (num4 < FirstDayOfWeek)
		{
			num4 += 7;
		}
		if (FirstDayOfWeek < 7 && FirstDayOfWeek >= 0 && ((FirstWeekOfYear == 2 && num4 > FirstDayOfWeek + 3) || (FirstWeekOfYear == 1 && num4 != FirstDayOfWeek)))
		{
			num5--;
		}
		return num5;
	}
}
