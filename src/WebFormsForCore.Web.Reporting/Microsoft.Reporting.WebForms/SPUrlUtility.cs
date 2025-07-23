using System;

namespace Microsoft.Reporting.WebForms;

internal static class SPUrlUtility
{
	private static readonly string[] m_rgstrAllowedProtocols = new string[12]
	{
		"http://", "https://", "file://", "file:\\\\", "ftp://", "mailto:", "msn:", "news:", "nntp:", "pnm://",
		"mms://", "outlook:"
	};

	public static string[] AllowedProtocols => m_rgstrAllowedProtocols;

	public static bool IsProtocolAllowed(string fullOrRelativeUrl)
	{
		return IsProtocolAllowed(fullOrRelativeUrl, allowRelativeUrl: true);
	}

	public static bool IsProtocolAllowed(string fullOrRelativeUrl, bool allowRelativeUrl)
	{
		if (fullOrRelativeUrl == null || fullOrRelativeUrl.Length <= 0)
		{
			if (allowRelativeUrl)
			{
				return true;
			}
			return false;
		}
		fullOrRelativeUrl = fullOrRelativeUrl.Split('?')[0];
		int num = fullOrRelativeUrl.IndexOf(':');
		if (num == -1)
		{
			if (allowRelativeUrl)
			{
				return true;
			}
			return false;
		}
		if (m_rgstrAllowedProtocols == null)
		{
			return false;
		}
		fullOrRelativeUrl = fullOrRelativeUrl.TrimStart();
		string[] rgstrAllowedProtocols = m_rgstrAllowedProtocols;
		foreach (string value in rgstrAllowedProtocols)
		{
			if (fullOrRelativeUrl.StartsWith(value, StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
		}
		return false;
	}
}
