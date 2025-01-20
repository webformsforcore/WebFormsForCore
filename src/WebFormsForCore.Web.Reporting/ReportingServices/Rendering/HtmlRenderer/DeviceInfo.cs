
using Microsoft.ReportingServices.Diagnostics.Utilities;
using System;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

#nullable disable
namespace Microsoft.ReportingServices.Rendering.HtmlRenderer
{
  internal abstract class DeviceInfo
  {
    internal string ActionScript;
    internal bool AllowScript = true;
    internal string BookmarkId;
    internal bool ExpandContent;
    internal bool HasActionScript;
    internal bool HTMLFragment;
    internal bool OnlyVisibleStyles = true;
    internal string FindString;
    internal string HtmlPrefixId = "";
    internal string JavascriptPrefixId = "";
    internal string LinkTarget;
    internal string ReplacementRoot;
    internal string ResourceStreamRoot;
    internal int Section;
    internal string StylePrefixId = "a";
    internal bool StyleStream;
    internal bool OutlookCompat;
    internal int Zoom = 100;
    internal bool AccessibleTablix;
    internal DataVisualizationFitSizing DataVisualizationFitSizing;
    internal bool IsBrowserIE = true;
    internal bool IsBrowserSafari;
    internal bool IsBrowserGeckoEngine;
    internal bool IsBrowserIE6Or7StandardsMode;
    internal bool IsBrowserIE6;
    internal bool IsBrowserIE7;
    internal BrowserMode BrowserMode;
    internal readonly string BrowserMode_Quirks = "quirks";
    internal readonly string BrowserMode_Standards = "standards";
    internal string NavigationId;
    internal bool ImageConsolidation = true;
    private static readonly Regex m_safeForJavascriptRegex = new Regex("^([a-zA-Z0-9_\\.]+)$", RegexOptions.ExplicitCapture | RegexOptions.Compiled);
    private NameValueCollection m_rawDeviceInfo;

    public NameValueCollection RawDeviceInfo => this.m_rawDeviceInfo;

    public void ParseDeviceInfo(NameValueCollection deviceInfo, NameValueCollection browserCaps)
    {
      this.m_rawDeviceInfo = deviceInfo;
      string boolValue1 = deviceInfo["HTMLFragment"];
      if (string.IsNullOrEmpty(boolValue1))
        boolValue1 = deviceInfo["MHTMLFragment"];
      if (!string.IsNullOrEmpty(boolValue1))
        this.HTMLFragment = DeviceInfo.ParseBool(boolValue1, false);
      object browserCap1 = (object) browserCaps[BrowserDetectionUtility.TypeKey];
      if (browserCap1 != null && ((string) browserCap1).StartsWith("Netscape", StringComparison.OrdinalIgnoreCase))
        this.IsBrowserIE = false;
      if (this.HTMLFragment)
      {
        string str = deviceInfo["PrefixId"];
        if (!string.IsNullOrEmpty(str))
        {
          this.VerifySafeForJavascript(str);
          this.HtmlPrefixId = str;
          this.StylePrefixId = this.JavascriptPrefixId = "A" + Guid.NewGuid().ToString().Replace("-", "");
        }
      }
      string str1 = deviceInfo["BookmarkId"];
      if (!string.IsNullOrEmpty(str1))
        this.BookmarkId = str1;
      string boolValue2 = deviceInfo["JavaScript"];
      if (!string.IsNullOrEmpty(boolValue2))
        this.AllowScript = DeviceInfo.ParseBool(boolValue2, true);
      if (this.AllowScript)
      {
        string browserCap2 = browserCaps[BrowserDetectionUtility.JavaScriptKey];
        if (!string.IsNullOrEmpty(browserCap2))
          this.AllowScript = DeviceInfo.ParseBool(browserCap2, true);
        if (this.AllowScript)
        {
          string str2 = deviceInfo["ActionScript"];
          if (!string.IsNullOrEmpty(str2))
          {
            this.VerifySafeForJavascript(str2);
            this.ActionScript = str2;
            this.HasActionScript = true;
          }
        }
      }
      string browserCap3 = deviceInfo["UserAgent"];
      if (browserCap3 == null && browserCaps != null)
        browserCap3 = browserCaps["UserAgent"];
      if (browserCap3 != null && browserCap3.Contains("MSIE 6.0"))
        this.IsBrowserIE6 = true;
      if (browserCap3 != null && browserCap3.Contains("MSIE 7.0"))
        this.IsBrowserIE7 = true;
      this.IsBrowserGeckoEngine = BrowserDetectionUtility.IsGeckoBrowserEngine(browserCap3);
      if (this.IsBrowserGeckoEngine)
        this.IsBrowserIE = false;
      else if (BrowserDetectionUtility.IsSafari(browserCap3) || BrowserDetectionUtility.IsChrome(browserCap3))
      {
        this.IsBrowserSafari = true;
        this.IsBrowserIE = false;
      }
      string boolValue3 = deviceInfo["ExpandContent"];
      if (!string.IsNullOrEmpty(boolValue3))
        this.ExpandContent = DeviceInfo.ParseBool(boolValue3, false);
      string intValue1 = deviceInfo["Section"];
      if (!string.IsNullOrEmpty(intValue1))
        this.Section = DeviceInfo.ParseInt(intValue1, 0);
      string str3 = deviceInfo["FindString"];
      if (!string.IsNullOrEmpty(str3) && str3.LastIndexOfAny(HTML4Renderer.m_standardLineBreak.ToCharArray()) < 0)
        this.FindString = str3.ToUpperInvariant();
      string str4 = deviceInfo["LinkTarget"];
      if (!string.IsNullOrEmpty(str4))
      {
        this.VerifySafeForJavascript(str4);
        this.LinkTarget = str4;
      }
      string boolValue4 = deviceInfo["OutlookCompat"];
      if (!string.IsNullOrEmpty(boolValue4))
        this.OutlookCompat = DeviceInfo.ParseBool(boolValue4, false);
      string boolValue5 = deviceInfo["AccessibleTablix"];
      if (!string.IsNullOrEmpty(boolValue5))
        this.AccessibleTablix = DeviceInfo.ParseBool(boolValue5, false);
      string boolValue6 = deviceInfo["StyleStream"];
      if (!string.IsNullOrEmpty(boolValue6))
        this.StyleStream = DeviceInfo.ParseBool(boolValue6, false);
      this.OnlyVisibleStyles = !this.StyleStream;
      string boolValue7 = deviceInfo["OnlyVisibleStyles"];
      if (!string.IsNullOrEmpty(boolValue7))
        this.OnlyVisibleStyles = DeviceInfo.ParseBool(boolValue7, this.OnlyVisibleStyles);
      string str5 = deviceInfo["ResourceStreamRoot"];
      if (!string.IsNullOrEmpty(str5))
      {
        this.VerifySafeForRoots(str5);
        this.ResourceStreamRoot = str5;
      }
      string str6 = deviceInfo["StreamRoot"];
      if (!string.IsNullOrEmpty(str6))
        this.VerifySafeForRoots(str6);
      if (this.IsBrowserIE)
      {
        string intValue2 = deviceInfo["Zoom"];
        if (!string.IsNullOrEmpty(intValue2))
          this.Zoom = DeviceInfo.ParseInt(intValue2, 100);
      }
      string str7 = deviceInfo["ReplacementRoot"];
      if (!string.IsNullOrEmpty(str7))
      {
        this.VerifySafeForRoots(str7);
        this.ReplacementRoot = str7;
      }
      string boolValue8 = deviceInfo["ImageConsolidation"];
      if (!string.IsNullOrEmpty(boolValue8))
        this.ImageConsolidation = DeviceInfo.ParseBool(boolValue8, this.ImageConsolidation);
      string strA1 = deviceInfo["BrowserMode"];
      if (!string.IsNullOrEmpty(strA1))
      {
        if (string.Compare(strA1, this.BrowserMode_Quirks, StringComparison.OrdinalIgnoreCase) == 0)
          this.BrowserMode = BrowserMode.Quirks;
        else if (string.Compare(strA1, this.BrowserMode_Standards, StringComparison.OrdinalIgnoreCase) == 0)
        {
          this.BrowserMode = BrowserMode.Standards;
          if (this.IsBrowserIE && browserCap3 != null && (this.IsBrowserIE7 || this.IsBrowserIE6))
            this.IsBrowserIE6Or7StandardsMode = true;
        }
      }
      if (this.IsBrowserIE && this.ImageConsolidation && string.IsNullOrEmpty(deviceInfo["ImageConsolidation"]) && this.IsBrowserIE6)
        this.ImageConsolidation = false;
      if (!this.AllowScript)
        this.ImageConsolidation = false;
      string strA2 = deviceInfo["DataVisualizationFitSizing"];
      if (string.IsNullOrEmpty(strA2) || string.Compare(strA2, "Approximate", StringComparison.OrdinalIgnoreCase) != 0)
        return;
      this.DataVisualizationFitSizing = DataVisualizationFitSizing.Approximate;
    }

    public abstract bool IsSupported(string value, bool isTrue, out bool isRelative);

    public virtual void VerifySafeForJavascript(string value)
    {
      if (value != null && !DeviceInfo.m_safeForJavascriptRegex.Match(value.Trim()).Success)
        throw new ArgumentOutOfRangeException(nameof (value));
    }

    internal void VerifySafeForRoots(string value)
    {
      bool isRelative;
      if (this.IsSupported(value, true, out isRelative) && !isRelative)
        return;
      int num1 = value.IndexOf(':');
      int num2 = value.IndexOf('?');
      int num3 = value.IndexOf('&');
      if (num1 == -1 && num3 == -1)
        return;
      if (num2 == -1 && (num1 != -1 || num3 != -1))
        throw new ArgumentOutOfRangeException(nameof (value));
      if (num1 != -1 && num1 < num2)
        throw new ArgumentOutOfRangeException(nameof (value));
      if (num3 != -1 && num3 < num2)
        throw new ArgumentOutOfRangeException(nameof (value));
    }

    private static bool ParseBool(string boolValue, bool defaultValue)
    {
      bool result;
      return bool.TryParse(boolValue, out result) ? result : defaultValue;
    }

    private static int ParseInt(string intValue, int defaultValue)
    {
      int result;
      return int.TryParse(intValue, out result) ? result : defaultValue;
    }
  }
}
