
using System;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal static class SPUrlUtility
  {
    private static readonly string[] m_rgstrAllowedProtocols = new string[12]
    {
      "http://",
      "https://",
      "file://",
      "file:\\\\",
      "ftp://",
      "mailto:",
      "msn:",
      "news:",
      "nntp:",
      "pnm://",
      "mms://",
      "outlook:"
    };

    public static bool IsProtocolAllowed(string fullOrRelativeUrl)
    {
      return SPUrlUtility.IsProtocolAllowed(fullOrRelativeUrl, true);
    }

    public static string[] AllowedProtocols => SPUrlUtility.m_rgstrAllowedProtocols;

    public static bool IsProtocolAllowed(string fullOrRelativeUrl, bool allowRelativeUrl)
    {
      if (fullOrRelativeUrl == null || fullOrRelativeUrl.Length <= 0)
        return allowRelativeUrl;
      fullOrRelativeUrl = fullOrRelativeUrl.Split('?')[0];
      if (fullOrRelativeUrl.IndexOf(':') == -1)
        return allowRelativeUrl;
      if (SPUrlUtility.m_rgstrAllowedProtocols == null)
        return false;
      fullOrRelativeUrl = fullOrRelativeUrl.TrimStart();
      foreach (string rgstrAllowedProtocol in SPUrlUtility.m_rgstrAllowedProtocols)
      {
        if (fullOrRelativeUrl.StartsWith(rgstrAllowedProtocol, StringComparison.OrdinalIgnoreCase))
          return true;
      }
      return false;
    }
  }
}
