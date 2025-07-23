using System.Globalization;
using System.IO;
using System.Text;
using System.Web.UI;

namespace Microsoft.Reporting.WebForms;

internal static class SPHttpUtility
{
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

	private const int bufferSize = 255;

	private static readonly ushort[] HTMLCharMap1 = new ushort[64]
	{
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 1, 0, 0, 0, 2, 3,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		4, 0, 5, 0
	};

	internal static readonly string[] HTMLData = new string[16]
	{
		"", "&quot;", "&amp;", "&#39;", "&lt;", "&gt;", " ", "<br>", "&nbsp;", "<b>",
		"<i>", "<u>", "</b>", "</i>", "</u>", "<wbr>"
	};

	private static readonly ushort[] ScriptCharMap = new ushort[96]
	{
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		1, 0, 0, 2, 0, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 3, 0, 0, 4, 5, 6,
		7, 8, 0, 9, 0, 0, 0, 10, 0, 0,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		11, 0, 12, 0, 0, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 0, 13, 0, 0, 0
	};

	private static readonly string[] ScriptEncodedChars = new string[14]
	{
		"", "\\n", "\\r", "\\u0022", "\\u0025", "\\u0026", "\\u0027", "\\u0028", "\\u0029", "\\u002b",
		"\\u002f", "\\u003c", "\\u003e", "\\\\"
	};

	private static readonly string[] m_crgstrUrlHexValue = new string[256]
	{
		"%00", "%01", "%02", "%03", "%04", "%05", "%06", "%07", "%08", "%09",
		"%0A", "%0B", "%0C", "%0D", "%0E", "%0F", "%10", "%11", "%12", "%13",
		"%14", "%15", "%16", "%17", "%18", "%19", "%1A", "%1B", "%1C", "%1D",
		"%1E", "%1F", "%20", "%21", "%22", "%23", "%24", "%25", "%26", "%27",
		"%28", "%29", "%2A", "%2B", "%2C", "%2D", "%2E", "%2F", "%30", "%31",
		"%32", "%33", "%34", "%35", "%36", "%37", "%38", "%39", "%3A", "%3B",
		"%3C", "%3D", "%3E", "%3F", "%40", "%41", "%42", "%43", "%44", "%45",
		"%46", "%47", "%48", "%49", "%4A", "%4B", "%4C", "%4D", "%4E", "%4F",
		"%50", "%51", "%52", "%53", "%54", "%55", "%56", "%57", "%58", "%59",
		"%5A", "%5B", "%5C", "%5D", "%5E", "%5F", "%60", "%61", "%62", "%63",
		"%64", "%65", "%66", "%67", "%68", "%69", "%6A", "%6B", "%6C", "%6D",
		"%6E", "%6F", "%70", "%71", "%72", "%73", "%74", "%75", "%76", "%77",
		"%78", "%79", "%7A", "%7B", "%7C", "%7D", "%7E", "%7F", "%80", "%81",
		"%82", "%83", "%84", "%85", "%86", "%87", "%88", "%89", "%8A", "%8B",
		"%8C", "%8D", "%8E", "%8F", "%90", "%91", "%92", "%93", "%94", "%95",
		"%96", "%97", "%98", "%99", "%9A", "%9B", "%9C", "%9D", "%9E", "%9F",
		"%A0", "%A1", "%A2", "%A3", "%A4", "%A5", "%A6", "%A7", "%A8", "%A9",
		"%AA", "%AB", "%AC", "%AD", "%AE", "%AF", "%B0", "%B1", "%B2", "%B3",
		"%B4", "%B5", "%B6", "%B7", "%B8", "%B9", "%BA", "%BB", "%BC", "%BD",
		"%BE", "%BF", "%C0", "%C1", "%C2", "%C3", "%C4", "%C5", "%C6", "%C7",
		"%C8", "%C9", "%CA", "%CB", "%CC", "%CD", "%CE", "%CF", "%D0", "%D1",
		"%D2", "%D3", "%D4", "%D5", "%D6", "%D7", "%D8", "%D9", "%DA", "%DB",
		"%DC", "%DD", "%DE", "%DF", "%E0", "%E1", "%E2", "%E3", "%E4", "%E5",
		"%E6", "%E7", "%E8", "%E9", "%EA", "%EB", "%EC", "%ED", "%EE", "%EF",
		"%F0", "%F1", "%F2", "%F3", "%F4", "%F5", "%F6", "%F7", "%F8", "%F9",
		"%FA", "%FB", "%FC", "%FD", "%FE", "%FF"
	};

	public static string HtmlEncode(string valueToEncode)
	{
		if (valueToEncode == null || valueToEncode.Length == 0)
		{
			return valueToEncode;
		}
		StringBuilder stringBuilder = new StringBuilder(255);
		HtmlTextWriter output = new HtmlTextWriter(new StringWriter(stringBuilder, CultureInfo.InvariantCulture));
		HtmlEncode(valueToEncode, output);
		return stringBuilder.ToString();
	}

	public static void HtmlEncode(string valueToEncode, TextWriter output)
	{
		if (valueToEncode == null || valueToEncode.Length == 0 || output == null)
		{
			return;
		}
		int num = 0;
		int num2 = 0;
		int length = valueToEncode.Length;
		for (int i = 0; i < length; i++)
		{
			int num3 = valueToEncode[i];
			ushort num4 = (ushort)((num3 < 63) ? HTMLCharMap1[num3] : 0);
			if (num4 > 0)
			{
				if (num2 > 0)
				{
					output.Write(valueToEncode.Substring(num, num2));
					num2 = 0;
				}
				num = i + 1;
				output.Write(HTMLData[num4]);
			}
			else
			{
				num2++;
			}
		}
		if (num < length)
		{
			output.Write(valueToEncode.Substring(num));
		}
	}

	public static string HtmlUrlAttributeEncode(string urlAttributeToEncode)
	{
		if (urlAttributeToEncode == null || urlAttributeToEncode.Length == 0)
		{
			return urlAttributeToEncode;
		}
		if (!SPUrlUtility.IsProtocolAllowed(urlAttributeToEncode))
		{
			return string.Empty;
		}
		return HtmlEncode(urlAttributeToEncode);
	}

	public static string UrlPathEncode(string urlToEncode, bool allowHashParameter)
	{
		return UrlPathEncode(urlToEncode, allowHashParameter, encodeUnicodeCharacters: false);
	}

	public static string UrlPathEncode(string urlToEncode, bool allowHashParameter, bool encodeUnicodeCharacters)
	{
		bool invalidUnicode = false;
		return UrlPathEncode(urlToEncode, allowHashParameter, encodeUnicodeCharacters, ref invalidUnicode);
	}

	internal static string UrlPathEncode(string urlToEncode, bool allowHashParameter, bool encodeUnicodeCharacters, ref bool invalidUnicode)
	{
		if (urlToEncode == null || urlToEncode.Length == 0)
		{
			return urlToEncode;
		}
		StringBuilder stringBuilder = new StringBuilder(255);
		HtmlTextWriter output = new HtmlTextWriter(new StringWriter(stringBuilder, CultureInfo.InvariantCulture));
		UrlPathEncode(urlToEncode, allowHashParameter, encodeUnicodeCharacters, output);
		return stringBuilder.ToString();
	}

	public static void UrlPathEncode(string urlToEncode, bool allowHashParameter, bool encodeUnicodeCharacters, TextWriter output)
	{
		bool invalidUnicode = false;
		UrlPathEncode(urlToEncode, allowHashParameter, encodeUnicodeCharacters, output, ref invalidUnicode);
	}

	private static void UrlPathEncode(string urlToEncode, bool allowHashParameter, bool encodeUnicodeCharacters, TextWriter output, ref bool invalidUnicode)
	{
		if (urlToEncode == null || urlToEncode.Length == 0 || output == null)
		{
			return;
		}
		bool fUsedNextChar = false;
		int i = 0;
		int length;
		for (length = urlToEncode.Length; i < length && urlToEncode[i] == ' '; i++)
		{
		}
		int num = i;
		int num2 = 0;
		for (; i < length; i++)
		{
			char c = urlToEncode[i];
			if (c == '?' || (allowHashParameter && c == '#'))
			{
				break;
			}
			if ((c & 0xFFE0) == 0 || c == ' ' || c == '"' || c == '#' || c == '%' || c == '<' || c == '>' || c == '\'' || c == '&')
			{
				if (num2 > 0)
				{
					output.Write(urlToEncode.Substring(num, num2));
					num2 = 0;
				}
				num = i + 1;
				int num3 = c & 0xFF;
				if (num3 < 16)
				{
					output.Write("%0");
					output.Write(num3.ToString("X", CultureInfo.InvariantCulture));
				}
				else
				{
					output.Write('%');
					output.Write(num3.ToString("X", CultureInfo.InvariantCulture));
				}
			}
			else if (encodeUnicodeCharacters && c > '\u007f')
			{
				if (num2 > 0)
				{
					output.Write(urlToEncode.Substring(num, num2));
					num2 = 0;
				}
				UrlEncodeUnicodeChar(output, urlToEncode[i], (i < length - 1) ? urlToEncode[i + 1] : '\0', ref invalidUnicode, out fUsedNextChar);
				if (fUsedNextChar)
				{
					i++;
				}
				num = i + 1;
			}
			else
			{
				num2++;
			}
		}
		if (num < length)
		{
			output.Write(urlToEncode.Substring(num));
		}
	}

	public static string EcmaScriptStringLiteralEncode(string scriptLiteralToEncode)
	{
		if (scriptLiteralToEncode == null || scriptLiteralToEncode.Length == 0)
		{
			return scriptLiteralToEncode;
		}
		StringBuilder stringBuilder = new StringBuilder(255);
		HtmlTextWriter output = new HtmlTextWriter(new StringWriter(stringBuilder, CultureInfo.InvariantCulture));
		EcmaScriptStringLiteralEncode(scriptLiteralToEncode, output);
		return stringBuilder.ToString();
	}

	public static void EcmaScriptStringLiteralEncode(string scriptLiteralToEncode, TextWriter output)
	{
		if (scriptLiteralToEncode == null || scriptLiteralToEncode.Length == 0 || output == null)
		{
			return;
		}
		int num = 0;
		int num2 = 0;
		int length = scriptLiteralToEncode.Length;
		for (int i = 0; i < length; i++)
		{
			int num3 = scriptLiteralToEncode[i];
			if (num3 > 127)
			{
				if (num2 > 0)
				{
					output.Write(scriptLiteralToEncode.Substring(num, num2));
					num2 = 0;
				}
				num = i + 1;
				output.Write("\\u");
				int num4 = num3 >> 8;
				if (num4 == 0)
				{
					output.Write("00");
				}
				else if (num4 < 16)
				{
					output.Write('0');
					output.Write(num4.ToString("X", CultureInfo.InvariantCulture));
				}
				else
				{
					output.Write(num4.ToString("X", CultureInfo.InvariantCulture));
				}
				num4 = num3 & 0xFF;
				if (num4 < 16)
				{
					output.Write('0');
					output.Write(num4.ToString("X", CultureInfo.InvariantCulture));
				}
				else
				{
					output.Write(num4.ToString("X", CultureInfo.InvariantCulture));
				}
				continue;
			}
			ushort num5 = (ushort)((num3 < 95) ? ScriptCharMap[num3] : 0);
			if (num5 > 0)
			{
				if (num2 > 0)
				{
					output.Write(scriptLiteralToEncode.Substring(num, num2));
					num2 = 0;
				}
				num = i + 1;
				output.Write(ScriptEncodedChars[num5]);
			}
			else
			{
				num2++;
			}
		}
		if (num < length)
		{
			output.Write(scriptLiteralToEncode.Substring(num));
		}
	}

	private static void UrlEncodeUnicodeChar(TextWriter output, char ch, char chNext, ref bool fInvalidUnicode, out bool fUsedNextChar)
	{
		int num = 192;
		int num2 = 224;
		int num3 = 240;
		int num4 = 128;
		int num5 = 55296;
		int num6 = 64512;
		int num7 = 65536;
		fUsedNextChar = false;
		int num8 = ch;
		if (num8 <= 127)
		{
			output.Write(m_crgstrUrlHexValue[num8]);
		}
		else if (num8 <= 2047)
		{
			int num9 = num | (num8 >> 6);
			output.Write(m_crgstrUrlHexValue[num9]);
			num9 = num4 | (num8 & 0x3F);
			output.Write(m_crgstrUrlHexValue[num9]);
		}
		else if ((num8 & num6) != num5)
		{
			int num9 = num2 | (num8 >> 12);
			output.Write(m_crgstrUrlHexValue[num9]);
			num9 = num4 | ((num8 & 0xFC0) >> 6);
			output.Write(m_crgstrUrlHexValue[num9]);
			num9 = num4 | (num8 & 0x3F);
			output.Write(m_crgstrUrlHexValue[num9]);
		}
		else if (chNext != 0)
		{
			num8 = (num8 & 0x3FF) << 10;
			fUsedNextChar = true;
			num8 |= chNext & 0x3FF;
			num8 += num7;
			int num9 = num3 | (num8 >> 18);
			output.Write(m_crgstrUrlHexValue[num9]);
			num9 = num4 | ((num8 & 0x3F000) >> 12);
			output.Write(m_crgstrUrlHexValue[num9]);
			num9 = num4 | ((num8 & 0xFC0) >> 6);
			output.Write(m_crgstrUrlHexValue[num9]);
			num9 = num4 | (num8 & 0x3F);
			output.Write(m_crgstrUrlHexValue[num9]);
		}
		else
		{
			fInvalidUnicode = true;
		}
	}
}
