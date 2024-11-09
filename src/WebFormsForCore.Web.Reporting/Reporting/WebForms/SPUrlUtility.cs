// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.SPUrlUtility
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

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
