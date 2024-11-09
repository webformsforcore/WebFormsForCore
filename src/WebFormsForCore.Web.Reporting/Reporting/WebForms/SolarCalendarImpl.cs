// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.SolarCalendarImpl
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

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
