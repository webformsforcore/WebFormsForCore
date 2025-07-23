namespace Microsoft.Reporting.WebForms;

internal struct SimpleDate
{
	private int m_Year;

	private int m_Month;

	private int m_Day;

	private int m_Era;

	private int m_hashValue;

	public int Year
	{
		get
		{
			return m_Year;
		}
		set
		{
			m_Year = value;
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
			m_Month = value;
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
			m_Day = value;
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
			m_Era = value;
		}
	}

	public SimpleDate(int year, int month, int day, int era)
	{
		m_Year = year;
		m_Month = month;
		m_Day = day;
		m_Era = era;
		m_hashValue = year + month + day + era;
	}

	public SimpleDate(int year, int month, int day)
	{
		m_Year = year;
		m_Month = month;
		m_Day = day;
		m_Era = 1;
		m_hashValue = year + month + day;
	}

	public static bool operator >(SimpleDate di0, SimpleDate di)
	{
		if (di0.Era <= di.Era)
		{
			if (di0.Era == di.Era)
			{
				if (di0.Year <= di.Year)
				{
					if (di0.Year == di.Year)
					{
						if (di0.Month <= di.Month)
						{
							if (di0.Month == di.Month)
							{
								return di0.Day > di.Day;
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
		return true;
	}

	public static bool operator <(SimpleDate di0, SimpleDate di)
	{
		if (di0.Era >= di.Era)
		{
			if (di0.Era == di.Era)
			{
				if (di0.Year >= di.Year)
				{
					if (di0.Year == di.Year)
					{
						if (di0.Month >= di.Month)
						{
							if (di0.Month == di.Month)
							{
								return di0.Day < di.Day;
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
		return true;
	}

	public static bool operator >=(SimpleDate di0, SimpleDate di)
	{
		return !(di0 < di);
	}

	public static bool operator <=(SimpleDate di0, SimpleDate di)
	{
		return !(di0 > di);
	}

	public static bool operator ==(SimpleDate di0, SimpleDate di)
	{
		if (di0.Year == di.Year && di0.Month == di.Month)
		{
			return di0.Day == di.Day;
		}
		return false;
	}

	public static bool operator !=(SimpleDate di0, SimpleDate di)
	{
		return !(di0 == di);
	}

	public override bool Equals(object o)
	{
		return this == (SimpleDate)o;
	}

	public override int GetHashCode()
	{
		return m_hashValue;
	}
}
