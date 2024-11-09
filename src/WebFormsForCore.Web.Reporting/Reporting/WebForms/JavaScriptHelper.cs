// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.JavaScriptHelper
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

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
