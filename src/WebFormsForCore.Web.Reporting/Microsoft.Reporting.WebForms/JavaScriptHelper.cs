using System.Globalization;
using System.Text;

namespace Microsoft.Reporting.WebForms;

internal static class JavaScriptHelper
{
	internal static string StringEscape(string input, char quoteChar)
	{
		return JavaScriptStringEscape(input, quoteChar);
	}

	internal static string StringEscapeSingleQuote(string input)
	{
		return JavaScriptStringEscapeSingleQuote(input);
	}

	internal static string JavaScriptStringEscape(string input, char quoteChar)
	{
		if (input == null)
		{
			return null;
		}
		string text = input.Replace("\\", "\\\\");
		text = text.Replace("/", "\\/");
		return text.Replace(new string(quoteChar, 1), "\\" + quoteChar);
	}

	internal static string JavaScriptStringEscapeSingleQuote(string input)
	{
		return JavaScriptStringEscape(input, '\'');
	}

	internal static string FormatAsFunction(StringBuilder functionBody)
	{
		return FormatAsFunction(functionBody.ToString());
	}

	internal static string FormatAsFunction(string functionBody)
	{
		return string.Format(CultureInfo.InvariantCulture, "function(){{{0}}}", functionBody);
	}

	internal static string MakeLiteral(string input)
	{
		if (input == null)
		{
			return "null";
		}
		return "\"" + StringEscape(input, '"') + "\"";
	}
}
