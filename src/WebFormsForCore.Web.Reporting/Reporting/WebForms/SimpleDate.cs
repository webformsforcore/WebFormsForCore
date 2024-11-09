// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.SimpleDate
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal struct SimpleDate
  {
    private int m_Year;
    private int m_Month;
    private int m_Day;
    private int m_Era;
    private int m_hashValue;

    public SimpleDate(int year, int month, int day, int era)
    {
      this.m_Year = year;
      this.m_Month = month;
      this.m_Day = day;
      this.m_Era = era;
      this.m_hashValue = year + month + day + era;
    }

    public SimpleDate(int year, int month, int day)
    {
      this.m_Year = year;
      this.m_Month = month;
      this.m_Day = day;
      this.m_Era = 1;
      this.m_hashValue = year + month + day;
    }

    public int Year
    {
      get => this.m_Year;
      set => this.m_Year = value;
    }

    public int Month
    {
      get => this.m_Month;
      set => this.m_Month = value;
    }

    public int Day
    {
      get => this.m_Day;
      set => this.m_Day = value;
    }

    public int Era
    {
      get => this.m_Era;
      set => this.m_Era = value;
    }

    public static bool operator >(SimpleDate di0, SimpleDate di)
    {
      if (di0.Era > di.Era)
        return true;
      if (di0.Era != di.Era)
        return false;
      if (di0.Year > di.Year)
        return true;
      if (di0.Year != di.Year)
        return false;
      if (di0.Month > di.Month)
        return true;
      return di0.Month == di.Month && di0.Day > di.Day;
    }

    public static bool operator <(SimpleDate di0, SimpleDate di)
    {
      if (di0.Era < di.Era)
        return true;
      if (di0.Era != di.Era)
        return false;
      if (di0.Year < di.Year)
        return true;
      if (di0.Year != di.Year)
        return false;
      if (di0.Month < di.Month)
        return true;
      return di0.Month == di.Month && di0.Day < di.Day;
    }

    public static bool operator >=(SimpleDate di0, SimpleDate di) => !(di0 < di);

    public static bool operator <=(SimpleDate di0, SimpleDate di) => !(di0 > di);

    public static bool operator ==(SimpleDate di0, SimpleDate di)
    {
      return di0.Year == di.Year && di0.Month == di.Month && di0.Day == di.Day;
    }

    public static bool operator !=(SimpleDate di0, SimpleDate di) => !(di0 == di);

    public override bool Equals(object o) => this == (SimpleDate) o;

    public override int GetHashCode() => this.m_hashValue;
  }
}
