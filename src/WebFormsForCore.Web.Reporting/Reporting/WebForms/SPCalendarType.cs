// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.SPCalendarType
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal enum SPCalendarType
  {
    None = 0,
    Gregorian = 1,
    Japan = 3,
    Taiwan = 4,
    Korea = 5,
    Hijri = 6,
    Thai = 7,
    Hebrew = 8,
    GregorianMEFrench = 9,
    GregorianArabic = 10, // 0x0000000A
    GregorianXLITEnglish = 11, // 0x0000000B
    GregorianXLITFrench = 12, // 0x0000000C
    KoreaJapanLunar = 14, // 0x0000000E
    ChineseLunar = 15, // 0x0000000F
    SakaEra = 16, // 0x00000010
  }
}
