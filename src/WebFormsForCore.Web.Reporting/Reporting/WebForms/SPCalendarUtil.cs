// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.SPCalendarUtil
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal class SPCalendarUtil
  {
    internal static bool IsYearInRange(int year, int yearL, int yearH)
    {
      return year >= yearL && year <= yearH;
    }

    internal static bool IsYearMonthInRange(
      int year,
      int month,
      int yearL,
      int monthL,
      int yearH,
      int monthH)
    {
      if (year <= yearL && (year != yearL || month < monthL))
        return false;
      if (year < yearH)
        return true;
      return year == yearH && month <= monthH;
    }

    internal static bool IsDateInRange(
      int year,
      int month,
      int day,
      int yearL,
      int monthL,
      int dayL,
      int yearH,
      int monthH,
      int dayH)
    {
      if (year <= yearL && (year != yearL || month <= monthL && (month != monthL || day < dayL)))
        return false;
      if (year < yearH)
        return true;
      if (year != yearH)
        return false;
      if (month < monthH)
        return true;
      return month == monthH && day <= dayH;
    }
  }
}
