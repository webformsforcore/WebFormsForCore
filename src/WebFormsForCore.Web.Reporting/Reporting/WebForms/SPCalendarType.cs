
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
