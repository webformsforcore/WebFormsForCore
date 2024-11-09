// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.ISPCalendar
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal interface ISPCalendar
  {
    bool IsSupportedYear(int year);

    bool IsSupportedMonth(int year, int month);

    bool IsSupportedDate(ref SimpleDate di);

    bool IsDateValid(ref SimpleDate di, int iAdvance, int jDayCurrent);

    bool IsSupportedJulianDay(int JDay);

    int DateToJulianDay(ref SimpleDate di, int iAdvance, int jDayCurrent);

    void JulianDayToDate(int jDay, ref SimpleDate di, int iAdvance, int jDayCurrent);

    bool IsYearLeap(int year);

    bool IsYearLeap(int year, int era);

    int MonthsInYear(ref SimpleDate di);

    int DaysInMonth(ref SimpleDate di);

    int DaysInMonth(ref SimpleDate di, int iAdvance);

    int GetEraOffset(int era);

    int GetEraJulianDay(int era);
  }
}
