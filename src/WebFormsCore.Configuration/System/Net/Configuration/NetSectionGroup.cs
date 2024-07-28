
using System.Configuration;

#nullable disable
namespace System.Net.Configuration
{
  /// <summary>Gets the section group information for the networking namespaces. This class cannot be inherited.</summary>
  public sealed class NetSectionGroup : ConfigurationSectionGroup
  {
    /// <summary>Gets the configuration section containing the authentication modules registered for the local computer.</summary>
    /// <returns>A <see cref="T:System.Net.Configuration.AuthenticationModulesSection" /> object.</returns>
    [ConfigurationProperty("authenticationModules")]
    public AuthenticationModulesSection AuthenticationModules
    {
      get => (AuthenticationModulesSection) this.Sections["authenticationModules"];
    }

    /// <summary>Gets the configuration section containing the connection management settings for the local computer.</summary>
    /// <returns>A <see cref="T:System.Net.Configuration.ConnectionManagementSection" /> object.</returns>
    [ConfigurationProperty("connectionManagement")]
    public ConnectionManagementSection ConnectionManagement
    {
      get => (ConnectionManagementSection) this.Sections["connectionManagement"];
    }

    /// <summary>Gets the configuration section containing the default Web proxy server settings for the local computer.</summary>
    /// <returns>A <see cref="T:System.Net.Configuration.DefaultProxySection" /> object.</returns>
    [ConfigurationProperty("defaultProxy")]
    public DefaultProxySection DefaultProxy => (DefaultProxySection) this.Sections["defaultProxy"];

    /// <summary>Gets the configuration section containing the SMTP client email settings for the local computer.</summary>
    /// <returns>A <see cref="T:System.Net.Configuration.MailSettingsSectionGroup" /> object.</returns>
    public MailSettingsSectionGroup MailSettings
    {
      get => (MailSettingsSectionGroup) this.SectionGroups["mailSettings"];
    }

    /// <summary>Gets the <see langword="System.Net" /> configuration section group from the specified configuration file.</summary>
    /// <param name="config">A <see cref="T:System.Configuration.Configuration" /> that represents a configuration file.</param>
    /// <returns>A <see cref="T:System.Net.Configuration.NetSectionGroup" /> that represents the <see langword="System.Net" /> settings in <paramref name="config" />.</returns>
    public static NetSectionGroup GetSectionGroup(System.Configuration.Configuration config)
    {
      return config != null ? config.GetSectionGroup("system.net") as NetSectionGroup : throw new ArgumentNullException(nameof (config));
    }

    /// <summary>Gets the configuration section containing the cache configuration settings for the local computer.</summary>
    /// <returns>A <see cref="T:System.Net.Configuration.RequestCachingSection" /> object.</returns>
    [ConfigurationProperty("requestCaching")]
    public RequestCachingSection RequestCaching
    {
      get => (RequestCachingSection) this.Sections["requestCaching"];
    }

    /// <summary>Gets the configuration section containing the network settings for the local computer.</summary>
    /// <returns>A <see cref="T:System.Net.Configuration.SettingsSection" /> object.</returns>
    [ConfigurationProperty("settings")]
    public SettingsSection Settings => (SettingsSection) this.Sections["settings"];

    /// <summary>Gets the configuration section containing the modules registered for use with the <see cref="T:System.Net.WebRequest" /> class.</summary>
    /// <returns>A <see cref="T:System.Net.Configuration.WebRequestModulesSection" /> object.</returns>
    [ConfigurationProperty("webRequestModules")]
    public WebRequestModulesSection WebRequestModules
    {
      get => (WebRequestModulesSection) this.Sections["webRequestModules"];
    }
  }
}
