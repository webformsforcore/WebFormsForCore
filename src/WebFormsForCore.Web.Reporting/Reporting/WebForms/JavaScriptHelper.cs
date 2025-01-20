
using System;
using System.Globalization;
using System.Text;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal static class JavaScriptHelper
  {
    internal static string StringEscape(string input, char quoteChar)
    {
      return JavaScriptHelper.JavaScriptStringEscape(input, quoteChar);
    }

    internal static string StringEscapeSingleQuote(string input)
    {
      return JavaScriptHelper.JavaScriptStringEscapeSingleQuote(input);
    }

    internal static string JavaScriptStringEscape(string input, char quoteChar)
    {
      return input?.Replace("\\", "\\\\").Replace("/", "\\/").Replace(new string(quoteChar, 1), "\\" + (object) quoteChar);
    }

    internal static string JavaScriptStringEscapeSingleQuote(string input)
    {
      return JavaScriptHelper.JavaScriptStringEscape(input, '\'');
    }

    internal static string FormatAsFunction(StringBuilder functionBody)
    {
      return JavaScriptHelper.FormatAsFunction(functionBody.ToString());
    }

    internal static string FormatAsFunction(string functionBody)
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "function(){{{0}}}", (object) functionBody);
    }

    internal static string MakeLiteral(string input)
    {
      return input == null ? "null" : "\"" + JavaScriptHelper.StringEscape(input, '"') + "\"";
    }
  }
}
