
#nullable disable
namespace System.Net.Configuration
{
  /// <summary>Controls how Unicode characters are interpreted by the <see cref="Overload:System.Net.WebUtility.HtmlDecode" /> methods.</summary>
  public enum UnicodeDecodingConformance
  {
    /// <summary>Use automatic behavior. The decoding behavior is determined by current application's target Framework. For .NET Framework 4.5 and later, the Unicode encoding decoding is strict.</summary>
    Auto,
    /// <summary>Use strict behavior. Specifies that the incoming encoded data is checked for validity before being decoded. For example, an input string of "&amp;#144308;" would decode as U+233B4, but an input string of "&amp;#xD84C;&amp;#xDFB4;" would fail to decode properly. Already-decoded data in the string is not checked for validity. For example, an input string of "\ud800" will result in an output string of "\ud800", as the already-decoded surrogate is skipped during decoding, even though it is unpaired.</summary>
    Strict,
    /// <summary>Use compatible behavior. Specifies that incoming data is not checked for validity before being decoded. For example, an input string of "&amp;amp;#xD84C;" would decode as U+D84C, which is an unpaired surrogate. Additionally, the decoder does not understand code points in the SMP unless they're represented as HTML-encoded surrogates, so the inputstring "&amp;#144308;" would result in the output string "&amp;#144308;".</summary>
    Compat,
    /// <summary>Use loose behavior. Similar to <see cref="F:System.Net.Configuration.UnicodeDecodingConformance.Compat" /> in that there are no validity checks, but the decoder also understands code points. The input string "&amp;#144308;" would decode into the character U+233B4 correctly. This switch is meant to provide maximum interoperability when the decoder doesn't know which format the provider is using to generate the encoded string.</summary>
    Loose,
  }
}
