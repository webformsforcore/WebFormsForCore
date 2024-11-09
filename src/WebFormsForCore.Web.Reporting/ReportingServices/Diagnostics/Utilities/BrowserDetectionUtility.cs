// Decompiled with JetBrains decompiler
// Type: Microsoft.ReportingServices.Diagnostics.Utilities.BrowserDetectionUtility
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Web;

#nullable disable
namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
  internal static class BrowserDetectionUtility
  {
    private static readonly char[] userAgentDelimiter = new char[3]
    {
      ';',
      '(',
      ')'
    };

    public static string UserAgentKey => "UserAgent";

    public static string TypeKey => "Type";

    public static string ActiveXControlsKey => "ActiveXControls";

    public static string EcmaScriptVersionKey => "EcmaScriptVersion";

    public static string JavaScriptKey => "JavaScript";

    public static string TablesKey => "Tables";

    public static string MajorVersionKey => "MajorVersion";

    public static string MinorVersionKey => "MinorVersion";

    public static string Win32Key => "Win32";

    public static string IEUserAgentPrefix => "IE";

    public static string IEModernUserAgentPrefix => "InternetExplorer";

    public static string IELayoutEngineName => "Trident";

    private static string GeckoUserAgent => "GECKO";

    private static string SafariUserAgent => "SAFARI";

    private static string IPadUserAgent => "IPAD";

    private static string IPhoneUserAgent => "IPHONE";

    private static string MacintoshUserAgent => "Macintosh";

    private static string ChromeUserAgent => "CHROME";

    private static string ArmUserAgent => "ARM";

    public static NameValueCollection GetBrowserInfoFromRequest(HttpRequest request)
    {
      NameValueCollection browserInfoFromRequest = new NameValueCollection(9);
      if (request == null)
        return browserInfoFromRequest;
      browserInfoFromRequest.Add(BrowserDetectionUtility.UserAgentKey, request.UserAgent);
      HttpBrowserCapabilities browser = request.Browser;
      try
      {
        if (browser != null)
        {
          browserInfoFromRequest.Add(BrowserDetectionUtility.TypeKey, browser.Type);
          browserInfoFromRequest.Add(BrowserDetectionUtility.ActiveXControlsKey, browser.ActiveXControls.ToString((IFormatProvider) CultureInfo.InvariantCulture));
          browserInfoFromRequest.Add(BrowserDetectionUtility.EcmaScriptVersionKey, browser.EcmaScriptVersion.ToString());
          browserInfoFromRequest.Add(BrowserDetectionUtility.JavaScriptKey, BrowserDetectionUtility.SupportsJavaScript(browser).ToString((IFormatProvider) CultureInfo.InvariantCulture));
          browserInfoFromRequest.Add(BrowserDetectionUtility.TablesKey, browser.Tables.ToString((IFormatProvider) CultureInfo.InvariantCulture));
          browserInfoFromRequest.Add(BrowserDetectionUtility.MajorVersionKey, browser.MajorVersion.ToString((IFormatProvider) CultureInfo.InvariantCulture));
          browserInfoFromRequest.Add(BrowserDetectionUtility.MinorVersionKey, browser.MinorVersion.ToString((IFormatProvider) CultureInfo.InvariantCulture));
          browserInfoFromRequest.Add(BrowserDetectionUtility.Win32Key, browser.Win32.ToString((IFormatProvider) CultureInfo.InvariantCulture));
        }
      }
      catch (HttpUnhandledException ex)
      {
      }
      return browserInfoFromRequest;
    }

    internal static ClientArchitecture GetClientArchitecture()
    {
      HttpContext current = HttpContext.Current;
      if (current != null && current.Request != null)
      {
        string userAgent = current.Request.UserAgent;
        if (!string.IsNullOrEmpty(userAgent))
        {
          foreach (string str1 in userAgent.Split(BrowserDetectionUtility.userAgentDelimiter, StringSplitOptions.RemoveEmptyEntries))
          {
            string str2 = str1.Trim();
            if (str2.Equals("x64", StringComparison.OrdinalIgnoreCase))
              return ClientArchitecture.X64;
            if (str2.Equals("WOW64", StringComparison.OrdinalIgnoreCase))
              return ClientArchitecture.X86;
            if (str2.Equals("IA64", StringComparison.OrdinalIgnoreCase))
              return ClientArchitecture.IA64;
          }
        }
      }
      return ClientArchitecture.X86;
    }

    public static bool IsIE55OrHigher(HttpRequest request)
    {
      if (request == null)
        return false;
      HttpBrowserCapabilities browser = request.Browser;
      if (browser == null)
        return false;
      try
      {
        return BrowserDetectionUtility.IsIE55OrHigher(browser.Type, browser.MajorVersion, browser.MinorVersion);
      }
      catch (HttpUnhandledException ex)
      {
      }
      return false;
    }

    public static bool IsIE55OrHigher(NameValueCollection browserCapabilities)
    {
      if (browserCapabilities == null)
        return false;
      string browserCapability = browserCapabilities[BrowserDetectionUtility.TypeKey];
      int result1;
      double result2;
      return int.TryParse(browserCapabilities[BrowserDetectionUtility.MajorVersionKey], out result1) && double.TryParse(browserCapabilities[BrowserDetectionUtility.MinorVersionKey], out result2) && BrowserDetectionUtility.IsIE55OrHigher(browserCapability, result1, result2);
    }

    public static bool IsIE55OrHigher(string type, int majorVersion, double minorVersion)
    {
      return type != null && type.Length >= 3 && (string.Compare(type, 0, BrowserDetectionUtility.IEUserAgentPrefix, 0, BrowserDetectionUtility.IEUserAgentPrefix.Length, StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(type, 0, BrowserDetectionUtility.IEModernUserAgentPrefix, 0, BrowserDetectionUtility.IEModernUserAgentPrefix.Length, StringComparison.OrdinalIgnoreCase) == 0) && (majorVersion >= 6 || majorVersion == 5 && minorVersion >= 5.0);
    }

    public static bool IsIOSSafari()
    {
      return BrowserDetectionUtility.IsIOSSafari(HttpContext.Current.Request);
    }

    public static bool IsIOSSafari(HttpRequest request)
    {
      if (request == null || request.UserAgent == null)
        return false;
      return request.UserAgent.IndexOf(BrowserDetectionUtility.IPadUserAgent, StringComparison.OrdinalIgnoreCase) >= 0 || request.UserAgent.IndexOf(BrowserDetectionUtility.IPhoneUserAgent, StringComparison.OrdinalIgnoreCase) >= 0;
    }

    public static bool IsSafari(HttpRequest request)
    {
      return request != null && BrowserDetectionUtility.IsSafari(request.UserAgent);
    }

    public static bool IsSafari(string userAgent)
    {
      return userAgent != null && userAgent.IndexOf(BrowserDetectionUtility.ChromeUserAgent, StringComparison.OrdinalIgnoreCase) < 0 && userAgent.IndexOf(BrowserDetectionUtility.SafariUserAgent, StringComparison.OrdinalIgnoreCase) >= 0;
    }

    public static bool IsChrome(string userAgent)
    {
      return userAgent != null && userAgent.IndexOf(BrowserDetectionUtility.ChromeUserAgent, StringComparison.OrdinalIgnoreCase) >= 0;
    }

    public static bool IsMac(HttpRequest request)
    {
      return request != null && request.UserAgent != null && request.UserAgent.IndexOf(BrowserDetectionUtility.MacintoshUserAgent, StringComparison.OrdinalIgnoreCase) >= 0;
    }

    public static bool IsArm(HttpRequest request)
    {
      return request != null && request.UserAgent != null && ((IEnumerable<string>) request.UserAgent.Split(BrowserDetectionUtility.userAgentDelimiter, StringSplitOptions.RemoveEmptyEntries)).Any<string>((Func<string, bool>) (section => section.Trim().Equals(BrowserDetectionUtility.ArmUserAgent, StringComparison.OrdinalIgnoreCase)));
    }

    public static bool IsGeckoBrowserEngine(string userAgent)
    {
      return userAgent != null && userAgent.IndexOf(BrowserDetectionUtility.GeckoUserAgent, StringComparison.OrdinalIgnoreCase) >= 0 && userAgent.IndexOf(BrowserDetectionUtility.SafariUserAgent, StringComparison.OrdinalIgnoreCase) < 0 && userAgent.IndexOf(BrowserDetectionUtility.IELayoutEngineName, StringComparison.OrdinalIgnoreCase) < 0;
    }

    internal static bool IsTransparentBorderSupported(HttpRequest request)
    {
      if (request == null)
        return true;
      HttpBrowserCapabilities browser = request.Browser;
      string type = browser.Type;
      return type == null || type.Length < 3 || string.Compare(type, 0, BrowserDetectionUtility.IEUserAgentPrefix, 0, 2, StringComparison.OrdinalIgnoreCase) != 0 || browser.MajorVersion >= 7;
    }

    private static bool SupportsJavaScript(HttpBrowserCapabilities browser)
    {
      return browser.EcmaScriptVersion.Major >= 1;
    }
  }
}
