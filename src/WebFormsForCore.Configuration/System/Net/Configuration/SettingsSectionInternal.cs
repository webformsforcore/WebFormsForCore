
using System.Configuration;
using System.Net.Security;
using System.Net.Sockets;
using System.Threading;

#nullable disable
namespace System.Net.Configuration
{
  internal sealed class SettingsSectionInternal
  {
    private static object s_InternalSyncObject;
    private static volatile SettingsSectionInternal s_settings;
    private bool alwaysUseCompletionPortsForAccept;
    private bool alwaysUseCompletionPortsForConnect;
    private bool checkCertificateName;
    private bool checkCertificateRevocationList;
    private int defaultCredentialsHandleCacheSize;
    private int autoConfigUrlRetryInterval;
    private int downloadTimeout;
    private int dnsRefreshTimeout;
    private bool enableDnsRoundRobin;
    private EncryptionPolicy encryptionPolicy;
    private bool expect100Continue;
    private IPProtectionLevel ipProtectionLevel;
    private bool ipv6Enabled;
    private int maximumResponseHeadersLength;
    private int maximumErrorResponseLength;
    private int maximumUnauthorizedUploadLength;
    private bool useUnsafeHeaderParsing;
    private bool useNagleAlgorithm;
    private bool performanceCountersEnabled;
    private bool httpListenerUnescapeRequestUrl;
    private long[] httpListenerTimeouts;

    internal SettingsSectionInternal(SettingsSection section)
    {
      if (section == null)
        section = new SettingsSection();
      this.alwaysUseCompletionPortsForConnect = section.Socket.AlwaysUseCompletionPortsForConnect;
      this.alwaysUseCompletionPortsForAccept = section.Socket.AlwaysUseCompletionPortsForAccept;
      this.checkCertificateName = section.ServicePointManager.CheckCertificateName;
      this.checkCertificateRevocationList = section.ServicePointManager.CheckCertificateRevocationList;
      this.dnsRefreshTimeout = section.ServicePointManager.DnsRefreshTimeout;
      this.ipProtectionLevel = section.Socket.IPProtectionLevel;
      this.ipv6Enabled = section.Ipv6.Enabled;
      this.enableDnsRoundRobin = section.ServicePointManager.EnableDnsRoundRobin;
      this.encryptionPolicy = section.ServicePointManager.EncryptionPolicy;
      this.expect100Continue = section.ServicePointManager.Expect100Continue;
      this.maximumUnauthorizedUploadLength = section.HttpWebRequest.MaximumUnauthorizedUploadLength;
      this.maximumResponseHeadersLength = section.HttpWebRequest.MaximumResponseHeadersLength;
      this.maximumErrorResponseLength = section.HttpWebRequest.MaximumErrorResponseLength;
      this.useUnsafeHeaderParsing = section.HttpWebRequest.UseUnsafeHeaderParsing;
      this.useNagleAlgorithm = section.ServicePointManager.UseNagleAlgorithm;
      this.autoConfigUrlRetryInterval = section.WebProxyScript.AutoConfigUrlRetryInterval;
      TimeSpan downloadTimeout = section.WebProxyScript.DownloadTimeout;
      this.downloadTimeout = downloadTimeout == TimeSpan.MaxValue || downloadTimeout == TimeSpan.Zero ? -1 : (int) downloadTimeout.TotalMilliseconds;
      this.performanceCountersEnabled = section.PerformanceCounters.Enabled;
      this.httpListenerUnescapeRequestUrl = section.HttpListener.UnescapeRequestUrl;
      this.httpListenerTimeouts = section.HttpListener.Timeouts.GetTimeouts();
      this.defaultCredentialsHandleCacheSize = section.WindowsAuthentication.DefaultCredentialsHandleCacheSize;
      WebUtilityElement webUtility = section.WebUtility;
      this.WebUtilityUnicodeDecodingConformance = webUtility.UnicodeDecodingConformance;
      this.WebUtilityUnicodeEncodingConformance = webUtility.UnicodeEncodingConformance;
    }

    internal static SettingsSectionInternal Section
    {
      get
      {
        if (SettingsSectionInternal.s_settings == null)
        {
          lock (SettingsSectionInternal.InternalSyncObject)
          {
            if (SettingsSectionInternal.s_settings == null)
              SettingsSectionInternal.s_settings = new SettingsSectionInternal((SettingsSection) PrivilegedConfigurationManager.GetSection(ConfigurationStrings.SettingsSectionPath));
          }
        }
        return SettingsSectionInternal.s_settings;
      }
    }

    private static object InternalSyncObject
    {
      get
      {
        if (SettingsSectionInternal.s_InternalSyncObject == null)
        {
          object obj = new object();
          Interlocked.CompareExchange(ref SettingsSectionInternal.s_InternalSyncObject, obj, (object) null);
        }
        return SettingsSectionInternal.s_InternalSyncObject;
      }
    }

    internal static SettingsSectionInternal GetSection()
    {
      return new SettingsSectionInternal((SettingsSection) PrivilegedConfigurationManager.GetSection(ConfigurationStrings.SettingsSectionPath));
    }

    internal bool AlwaysUseCompletionPortsForAccept => this.alwaysUseCompletionPortsForAccept;

    internal bool AlwaysUseCompletionPortsForConnect => this.alwaysUseCompletionPortsForConnect;

    internal int AutoConfigUrlRetryInterval => this.autoConfigUrlRetryInterval;

    internal bool CheckCertificateName => this.checkCertificateName;

    internal bool CheckCertificateRevocationList
    {
      get => this.checkCertificateRevocationList;
      set => this.checkCertificateRevocationList = value;
    }

    internal int DefaultCredentialsHandleCacheSize
    {
      get => this.defaultCredentialsHandleCacheSize;
      set => this.defaultCredentialsHandleCacheSize = value;
    }

    internal int DnsRefreshTimeout
    {
      get => this.dnsRefreshTimeout;
      set => this.dnsRefreshTimeout = value;
    }

    internal int DownloadTimeout => this.downloadTimeout;

    internal bool EnableDnsRoundRobin
    {
      get => this.enableDnsRoundRobin;
      set => this.enableDnsRoundRobin = value;
    }

    internal EncryptionPolicy EncryptionPolicy => this.encryptionPolicy;

    internal bool Expect100Continue
    {
      get => this.expect100Continue;
      set => this.expect100Continue = value;
    }

    internal IPProtectionLevel IPProtectionLevel => this.ipProtectionLevel;

    internal bool Ipv6Enabled => this.ipv6Enabled;

    internal int MaximumResponseHeadersLength
    {
      get => this.maximumResponseHeadersLength;
      set => this.maximumResponseHeadersLength = value;
    }

    internal int MaximumUnauthorizedUploadLength => this.maximumUnauthorizedUploadLength;

    internal int MaximumErrorResponseLength
    {
      get => this.maximumErrorResponseLength;
      set => this.maximumErrorResponseLength = value;
    }

    internal bool UseUnsafeHeaderParsing => this.useUnsafeHeaderParsing;

    internal bool UseNagleAlgorithm
    {
      get => this.useNagleAlgorithm;
      set => this.useNagleAlgorithm = value;
    }

    internal bool PerformanceCountersEnabled => this.performanceCountersEnabled;

    internal bool HttpListenerUnescapeRequestUrl => this.httpListenerUnescapeRequestUrl;

    internal long[] HttpListenerTimeouts => this.httpListenerTimeouts;

    internal UnicodeDecodingConformance WebUtilityUnicodeDecodingConformance { get; private set; }

    internal UnicodeEncodingConformance WebUtilityUnicodeEncodingConformance { get; private set; }
  }
}
