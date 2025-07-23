using System;

namespace Microsoft.Reporting.WebForms;

internal sealed class IntlDate
{
	private int m_Year;

	private int m_Month;

	private int m_Day;

	private int m_Era;

	private int m_Jday;

	private SPCalendarType m_CalendarType;

	public int Year
	{
		get
		{
			return m_Year;
		}
		set
		{
			Init(value, m_Month, m_Day, m_CalendarType);
		}
	}

	public int Month
	{
		get
		{
			return m_Month;
		}
		set
		{
			Init(m_Year, value, m_Day, m_CalendarType);
		}
	}

	public int Day
	{
		get
		{
			return m_Day;
		}
		set
		{
			Init(m_Year, m_Month, value, m_CalendarType);
		}
	}

	public int Era
	{
		get
		{
			return m_Era;
		}
		set
		{
			Init(m_Year, m_Month, m_Day, value, m_CalendarType);
		}
	}

	public int JDay
	{
		get
		{
			return m_Jday;
		}
		set
		{
			Init(value, m_CalendarType);
		}
	}

	public SPCalendarType CalendarType
	{
		get
		{
			return m_CalendarType;
		}
		set
		{
			Init(m_Jday, value);
		}
	}

	public int DayOfWeek => (m_Jday + 1) % 7;

	public IntlDate(int year, int month, int day)
	{
		Init(year, month, day, SPCalendarType.Gregorian);
	}

	public IntlDate(int year, int month, int day, SPCalendarType calendarType)
	{
		Init(year, month, day, calendarType);
	}

	public IntlDate(int year, int month, int day, int era, SPCalendarType calendarType)
	{
		Init(year, month, day, era, calendarType);
	}

	public IntlDate(int julianDay)
	{
		Init(julianDay, SPCalendarType.Gregorian);
	}

	public IntlDate(int julianDay, SPCalendarType calendarType)
	{
		Init(julianDay, calendarType);
	}

	public bool IsYearLeap()
	{
		return SPIntlCal.IsLocalYearLeap(m_CalendarType, m_Year);
	}

	public int MonthsInYear()
	{
		SimpleDate di = new SimpleDate(m_Year, m_Month, m_Day, m_Era);
		return SPIntlCal.MonthsInLocalYear(m_CalendarType, ref di);
	}

	public int DaysInMonth()
	{
		SimpleDate di = new SimpleDate(m_Year, m_Month, m_Day, m_Era);
		return SPIntlCal.DaysInLocalMonth(m_CalendarType, ref di);
	}

	public void AddDays(int days)
	{
		JDay += days;
	}

	internal void Init(int year, int month, int day, SPCalendarType calendarType)
	{
		Init(year, month, day, 1, calendarType);
	}

	internal void Init(int year, int month, int day, int era, SPCalendarType calendarType)
	{
		m_Year = year;
		m_Month = month;
		m_Day = day;
		m_Era = era;
		m_CalendarType = calendarType;
		SimpleDate di = new SimpleDate(m_Year, m_Month, m_Day, m_Era);
		if (!SPIntlCal.IsLocalDateValid(calendarType, ref di))
		{
			throw new ArgumentOutOfRangeException("calendarType");
		}
		m_Jday = SPIntlCal.LocalToJulianDay(m_CalendarType, ref di);
	}

	internal void Init(int julianDay, SPCalendarType calendarType)
	{
		SimpleDate di = new SimpleDate(0, 0, 0);
		SPIntlCal.JulianDayToLocal(calendarType, julianDay, ref di);
		m_Year = di.Year;
		m_Month = di.Month;
		m_Day = di.Day;
		m_Era = di.Era;
		m_CalendarType = calendarType;
		m_Jday = julianDay;
	}
}
