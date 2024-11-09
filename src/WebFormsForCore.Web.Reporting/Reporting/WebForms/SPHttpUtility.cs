// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.SPHttpUtility
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web.UI;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal static class SPHttpUtility
  {
    private const int bufferSize = 255;
    private static readonly ushort[] HTMLCharMap1 = new ushort[64]
    {
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 1,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 2,
      (ushort) 3,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 4,
      (ushort) 0,
      (ushort) 5,
      (ushort) 0
    };
    internal static readonly string[] HTMLData = new string[16]
    {
      "",
      "&quot;",
      "&amp;",
      "&#39;",
      "&lt;",
      "&gt;",
      " ",
      "<br>",
      "&nbsp;",
      "<b>",
      "<i>",
      "<u>",
      "</b>",
      "</i>",
      "</u>",
      "<wbr>"
    };
    private static readonly ushort[] ScriptCharMap = new ushort[96]
    {
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 1,
      (ushort) 0,
      (ushort) 0,
      (ushort) 2,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 3,
      (ushort) 0,
      (ushort) 0,
      (ushort) 4,
      (ushort) 5,
      (ushort) 6,
      (ushort) 7,
      (ushort) 8,
      (ushort) 0,
      (ushort) 9,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 10,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 11,
      (ushort) 0,
      (ushort) 12,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0,
      (ushort) 13,
      (ushort) 0,
      (ushort) 0,
      (ushort) 0
    };
    private static readonly string[] ScriptEncodedChars = new string[14]
    {
      "",
      "\\n",
      "\\r",
      "\\u0022",
      "\\u0025",
      "\\u0026",
      "\\u0027",
      "\\u0028",
      "\\u0029",
      "\\u002b",
      "\\u002f",
      "\\u003c",
      "\\u003e",
      "\\\\"
    };
    private static readonly string[] m_crgstrUrlHexValue = new string[256]
    {
      "%00",
      "%01",
      "%02",
      "%03",
      "%04",
      "%05",
      "%06",
      "%07",
      "%08",
      "%09",
      "%0A",
      "%0B",
      "%0C",
      "%0D",
      "%0E",
      "%0F",
      "%10",
      "%11",
      "%12",
      "%13",
      "%14",
      "%15",
      "%16",
      "%17",
      "%18",
      "%19",
      "%1A",
      "%1B",
      "%1C",
      "%1D",
      "%1E",
      "%1F",
      "%20",
      "%21",
      "%22",
      "%23",
      "%24",
      "%25",
      "%26",
      "%27",
      "%28",
      "%29",
      "%2A",
      "%2B",
      "%2C",
      "%2D",
      "%2E",
      "%2F",
      "%30",
      "%31",
      "%32",
      "%33",
      "%34",
      "%35",
      "%36",
      "%37",
      "%38",
      "%39",
      "%3A",
      "%3B",
      "%3C",
      "%3D",
      "%3E",
      "%3F",
      "%40",
      "%41",
      "%42",
      "%43",
      "%44",
      "%45",
      "%46",
      "%47",
      "%48",
      "%49",
      "%4A",
      "%4B",
      "%4C",
      "%4D",
      "%4E",
      "%4F",
      "%50",
      "%51",
      "%52",
      "%53",
      "%54",
      "%55",
      "%56",
      "%57",
      "%58",
      "%59",
      "%5A",
      "%5B",
      "%5C",
      "%5D",
      "%5E",
      "%5F",
      "%60",
      "%61",
      "%62",
      "%63",
      "%64",
      "%65",
      "%66",
      "%67",
      "%68",
      "%69",
      "%6A",
      "%6B",
      "%6C",
      "%6D",
      "%6E",
      "%6F",
      "%70",
      "%71",
      "%72",
      "%73",
      "%74",
      "%75",
      "%76",
      "%77",
      "%78",
      "%79",
      "%7A",
      "%7B",
      "%7C",
      "%7D",
      "%7E",
      "%7F",
      "%80",
      "%81",
      "%82",
      "%83",
      "%84",
      "%85",
      "%86",
      "%87",
      "%88",
      "%89",
      "%8A",
      "%8B",
      "%8C",
      "%8D",
      "%8E",
      "%8F",
      "%90",
      "%91",
      "%92",
      "%93",
      "%94",
      "%95",
      "%96",
      "%97",
      "%98",
      "%99",
      "%9A",
      "%9B",
      "%9C",
      "%9D",
      "%9E",
      "%9F",
      "%A0",
      "%A1",
      "%A2",
      "%A3",
      "%A4",
      "%A5",
      "%A6",
      "%A7",
      "%A8",
      "%A9",
      "%AA",
      "%AB",
      "%AC",
      "%AD",
      "%AE",
      "%AF",
      "%B0",
      "%B1",
      "%B2",
      "%B3",
      "%B4",
      "%B5",
      "%B6",
      "%B7",
      "%B8",
      "%B9",
      "%BA",
      "%BB",
      "%BC",
      "%BD",
      "%BE",
      "%BF",
      "%C0",
      "%C1",
      "%C2",
      "%C3",
      "%C4",
      "%C5",
      "%C6",
      "%C7",
      "%C8",
      "%C9",
      "%CA",
      "%CB",
      "%CC",
      "%CD",
      "%CE",
      "%CF",
      "%D0",
      "%D1",
      "%D2",
      "%D3",
      "%D4",
      "%D5",
      "%D6",
      "%D7",
      "%D8",
      "%D9",
      "%DA",
      "%DB",
      "%DC",
      "%DD",
      "%DE",
      "%DF",
      "%E0",
      "%E1",
      "%E2",
      "%E3",
      "%E4",
      "%E5",
      "%E6",
      "%E7",
      "%E8",
      "%E9",
      "%EA",
      "%EB",
      "%EC",
      "%ED",
      "%EE",
      "%EF",
      "%F0",
      "%F1",
      "%F2",
      "%F3",
      "%F4",
      "%F5",
      "%F6",
      "%F7",
      "%F8",
      "%F9",
      "%FA",
      "%FB",
      "%FC",
      "%FD",
      "%FE",
      "%FF"
    };

    public static string HtmlEncode(string valueToEncode)
    {
      if (valueToEncode == null || valueToEncode.Length == 0)
        return valueToEncode;
      StringBuilder sb = new StringBuilder((int) byte.MaxValue);
      HtmlTextWriter output = new HtmlTextWriter((TextWriter) new StringWriter(sb, (IFormatProvider) CultureInfo.InvariantCulture));
      SPHttpUtility.HtmlEncode(valueToEncode, (TextWriter) output);
      return sb.ToString();
    }

    public static void HtmlEncode(string valueToEncode, TextWriter output)
    {
      switch (valueToEncode)
      {
        case null:
          break;
        case "":
          break;
        default:
          if (output == null)
            break;
          int startIndex = 0;
          int length1 = 0;
          int length2 = valueToEncode.Length;
          for (int index1 = 0; index1 < length2; ++index1)
          {
            int index2 = (int) valueToEncode[index1];
            ushort index3 = index2 >= 63 ? (ushort) 0 : SPHttpUtility.HTMLCharMap1[index2];
            if (index3 > (ushort) 0)
            {
              if (length1 > 0)
              {
                output.Write(valueToEncode.Substring(startIndex, length1));
                length1 = 0;
              }
              startIndex = index1 + 1;
              output.Write(SPHttpUtility.HTMLData[(int) index3]);
            }
            else
              ++length1;
          }
          if (startIndex >= length2)
            break;
          output.Write(valueToEncode.Substring(startIndex));
          break;
      }
    }

    public static string HtmlUrlAttributeEncode(string urlAttributeToEncode)
    {
      if (urlAttributeToEncode == null || urlAttributeToEncode.Length == 0)
        return urlAttributeToEncode;
      return !SPUrlUtility.IsProtocolAllowed(urlAttributeToEncode) ? string.Empty : SPHttpUtility.HtmlEncode(urlAttributeToEncode);
    }

    public static string UrlPathEncode(string urlToEncode, bool allowHashParameter)
    {
      return SPHttpUtility.UrlPathEncode(urlToEncode, allowHashParameter, false);
    }

    public static string UrlPathEncode(
      string urlToEncode,
      bool allowHashParameter,
      bool encodeUnicodeCharacters)
    {
      bool invalidUnicode = false;
      return SPHttpUtility.UrlPathEncode(urlToEncode, allowHashParameter, encodeUnicodeCharacters, ref invalidUnicode);
    }

    internal static string UrlPathEncode(
      string urlToEncode,
      bool allowHashParameter,
      bool encodeUnicodeCharacters,
      ref bool invalidUnicode)
    {
      if (urlToEncode == null || urlToEncode.Length == 0)
        return urlToEncode;
      StringBuilder sb = new StringBuilder((int) byte.MaxValue);
      HtmlTextWriter output = new HtmlTextWriter((TextWriter) new StringWriter(sb, (IFormatProvider) CultureInfo.InvariantCulture));
      SPHttpUtility.UrlPathEncode(urlToEncode, allowHashParameter, encodeUnicodeCharacters, (TextWriter) output);
      return sb.ToString();
    }

    public static void UrlPathEncode(
      string urlToEncode,
      bool allowHashParameter,
      bool encodeUnicodeCharacters,
      TextWriter output)
    {
      bool invalidUnicode = false;
      SPHttpUtility.UrlPathEncode(urlToEncode, allowHashParameter, encodeUnicodeCharacters, output, ref invalidUnicode);
    }

    private static void UrlPathEncode(
      string urlToEncode,
      bool allowHashParameter,
      bool encodeUnicodeCharacters,
      TextWriter output,
      ref bool invalidUnicode)
    {
      switch (urlToEncode)
      {
        case null:
          break;
        case "":
          break;
        default:
          if (output == null)
            break;
          bool fUsedNextChar = false;
          int index = 0;
          int length1 = urlToEncode.Length;
          while (index < length1 && urlToEncode[index] == ' ')
            ++index;
          int startIndex = index;
          int length2 = 0;
          for (; index < length1; ++index)
          {
            char ch = urlToEncode[index];
            if (ch != '?' && (!allowHashParameter || ch != '#'))
            {
              if (((int) ch & 65504) == 0 || ch == ' ' || ch == '"' || ch == '#' || ch == '%' || ch == '<' || ch == '>' || ch == '\'' || ch == '&')
              {
                if (length2 > 0)
                {
                  output.Write(urlToEncode.Substring(startIndex, length2));
                  length2 = 0;
                }
                startIndex = index + 1;
                int num = (int) ch & (int) byte.MaxValue;
                if (num < 16)
                {
                  output.Write("%0");
                  output.Write(num.ToString("X", (IFormatProvider) CultureInfo.InvariantCulture));
                }
                else
                {
                  output.Write('%');
                  output.Write(num.ToString("X", (IFormatProvider) CultureInfo.InvariantCulture));
                }
              }
              else if (encodeUnicodeCharacters && ch > '\u007F')
              {
                if (length2 > 0)
                {
                  output.Write(urlToEncode.Substring(startIndex, length2));
                  length2 = 0;
                }
                SPHttpUtility.UrlEncodeUnicodeChar(output, urlToEncode[index], index < length1 - 1 ? urlToEncode[index + 1] : char.MinValue, ref invalidUnicode, out fUsedNextChar);
                if (fUsedNextChar)
                  ++index;
                startIndex = index + 1;
              }
              else
                ++length2;
            }
            else
              break;
          }
          if (startIndex >= length1)
            break;
          output.Write(urlToEncode.Substring(startIndex));
          break;
      }
    }

    public static string EcmaScriptStringLiteralEncode(string scriptLiteralToEncode)
    {
      if (scriptLiteralToEncode == null || scriptLiteralToEncode.Length == 0)
        return scriptLiteralToEncode;
      StringBuilder sb = new StringBuilder((int) byte.MaxValue);
      HtmlTextWriter output = new HtmlTextWriter((TextWriter) new StringWriter(sb, (IFormatProvider) CultureInfo.InvariantCulture));
      SPHttpUtility.EcmaScriptStringLiteralEncode(scriptLiteralToEncode, (TextWriter) output);
      return sb.ToString();
    }

    public static void EcmaScriptStringLiteralEncode(
      string scriptLiteralToEncode,
      TextWriter output)
    {
      switch (scriptLiteralToEncode)
      {
        case null:
          break;
        case "":
          break;
        default:
          if (output == null)
            break;
          int startIndex = 0;
          int length1 = 0;
          int length2 = scriptLiteralToEncode.Length;
          for (int index1 = 0; index1 < length2; ++index1)
          {
            int index2 = (int) scriptLiteralToEncode[index1];
            if (index2 > (int) sbyte.MaxValue)
            {
              if (length1 > 0)
              {
                output.Write(scriptLiteralToEncode.Substring(startIndex, length1));
                length1 = 0;
              }
              startIndex = index1 + 1;
              output.Write("\\u");
              int num = index2 >> 8;
              if (num == 0)
                output.Write("00");
              else if (num < 16)
              {
                output.Write('0');
                output.Write(num.ToString("X", (IFormatProvider) CultureInfo.InvariantCulture));
              }
              else
                output.Write(num.ToString("X", (IFormatProvider) CultureInfo.InvariantCulture));
              num = index2 & (int) byte.MaxValue;
              if (num < 16)
              {
                output.Write('0');
                output.Write(num.ToString("X", (IFormatProvider) CultureInfo.InvariantCulture));
              }
              else
                output.Write(num.ToString("X", (IFormatProvider) CultureInfo.InvariantCulture));
            }
            else
            {
              ushort scriptChar = index2 >= 95 ? (ushort) 0 : SPHttpUtility.ScriptCharMap[index2];
              if (scriptChar > (ushort) 0)
              {
                if (length1 > 0)
                {
                  output.Write(scriptLiteralToEncode.Substring(startIndex, length1));
                  length1 = 0;
                }
                startIndex = index1 + 1;
                output.Write(SPHttpUtility.ScriptEncodedChars[(int) scriptChar]);
              }
              else
                ++length1;
            }
          }
          if (startIndex >= length2)
            break;
          output.Write(scriptLiteralToEncode.Substring(startIndex));
          break;
      }
    }

    private static void UrlEncodeUnicodeChar(
      TextWriter output,
      char ch,
      char chNext,
      ref bool fInvalidUnicode,
      out bool fUsedNextChar)
    {
      int num1 = 192;
      int num2 = 224;
      int num3 = 240;
      int num4 = 128;
      int num5 = 55296;
      int num6 = 64512;
      int num7 = 65536;
      fUsedNextChar = false;
      int index1 = (int) ch;
      if (index1 <= (int) sbyte.MaxValue)
        output.Write(SPHttpUtility.m_crgstrUrlHexValue[index1]);
      else if (index1 <= 2047)
      {
        int index2 = num1 | index1 >> 6;
        output.Write(SPHttpUtility.m_crgstrUrlHexValue[index2]);
        int index3 = num4 | index1 & 63;
        output.Write(SPHttpUtility.m_crgstrUrlHexValue[index3]);
      }
      else if ((index1 & num6) != num5)
      {
        int index4 = num2 | index1 >> 12;
        output.Write(SPHttpUtility.m_crgstrUrlHexValue[index4]);
        int index5 = num4 | (index1 & 4032) >> 6;
        output.Write(SPHttpUtility.m_crgstrUrlHexValue[index5]);
        int index6 = num4 | index1 & 63;
        output.Write(SPHttpUtility.m_crgstrUrlHexValue[index6]);
      }
      else if (chNext != char.MinValue)
      {
        int num8 = (index1 & 1023) << 10;
        fUsedNextChar = true;
        int num9 = (num8 | (int) chNext & 1023) + num7;
        int index7 = num3 | num9 >> 18;
        output.Write(SPHttpUtility.m_crgstrUrlHexValue[index7]);
        int index8 = num4 | (num9 & 258048) >> 12;
        output.Write(SPHttpUtility.m_crgstrUrlHexValue[index8]);
        int index9 = num4 | (num9 & 4032) >> 6;
        output.Write(SPHttpUtility.m_crgstrUrlHexValue[index9]);
        int index10 = num4 | num9 & 63;
        output.Write(SPHttpUtility.m_crgstrUrlHexValue[index10]);
      }
      else
        fInvalidUnicode = true;
    }

    internal static class HtmlStrings
    {
      public const string Empty = "";
      public const string Quot = "&quot;";
      public const string Amp = "&amp;";
      public const string Apostrophe = "&#39;";
      public const string Lt = "&lt;";
      public const string Gt = "&gt;";
      public const string Space = " ";
      public const string Br = "<br>";
      public const string Nbsp = "&nbsp;";
      public const string B = "<b>";
      public const string I = "<i>";
      public const string U = "<u>";
      public const string BClose = "</b>";
      public const string IClose = "</i>";
      public const string UClose = "</u>";
      public const string Wbr = "<wbr>";
      public const string Style = "<style>";
      public const string StyleClose = "</style>";
    }
  }
}
