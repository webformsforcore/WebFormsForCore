
#nullable disable
namespace System.Net.Configuration
{
  /// <summary>Controls how Unicode characters are output by the <see cref="Overload:System.Net.WebUtility.HtmlEncode" /> methods.</summary>
  public enum UnicodeEncodingConformance
  {
    /// <summary>Use automatic behavior. The Unicode encoding behavior is determined by current application's target Framework. For .NET Framework 4.5 and later, the Unicode encoding behavior is strict.</summary>
    Auto,
    /// <summary>Use strict behavior. Specifies that individual UTF-16 surrogate code points are combined into a single code point when one of the <see cref="Overload:System.Net.WebUtility.HtmlEncode" /> methods is called. For example, given the input string "\uD84C\uDFB4" (or "\U000233B4"), the output of the <see cref="Overload:System.Net.WebUtility.HtmlEncode" /> methods is "&amp;#144308;".
    /// If the input is a malformed UTF-16 string (it contains unpaired surrogates, for example), the bad code points will be replaced with U+FFFD (Unicode replacement char) before being HTML-encoded.</summary>
    Strict,
    /// <summary>Use compatible behavior. Specifies that individual UTF-16 surrogate code points are output as-is when one of <see cref="Overload:System.Net.WebUtility.HtmlEncode" /> methods is called. For example, given a string "\uD84C\uDFB4" (or "\U000233B4"), the output of <see cref="Overload:System.Net.WebUtility.HtmlEncode" /> is "\uD84C\uDFB4" (the input is not encoded).</summary>
    Compat,
  }
}
