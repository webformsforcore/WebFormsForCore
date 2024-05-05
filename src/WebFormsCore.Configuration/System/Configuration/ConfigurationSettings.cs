// Decompiled with JetBrains decompiler
// Type: System.Configuration.ConfigurationSettings
// Assembly: System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 84F5A18A-F2B1-435C-B86E-09CE162E61E4
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System\v4.0_4.0.0.0__b77a5c561934e089\System.dll
// XML documentation location: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1\System.xml

using System.Collections.Specialized;

#nullable disable
namespace System.Configuration
{
  /// <summary>Provides runtime versions 1.0 and 1.1 support for reading configuration sections and common configuration settings.</summary>
  public sealed class ConfigurationSettings
  {
    private ConfigurationSettings()
    {
    }

    /// <summary>Gets a read-only <see cref="T:System.Collections.Specialized.NameValueCollection" /> of the application settings section of the configuration file.</summary>
    /// <returns>A read-only <see cref="T:System.Collections.Specialized.NameValueCollection" /> of the application settings section from the configuration file.</returns>
    [Obsolete("This method is obsolete, it has been replaced by System.Configuration!System.Configuration.ConfigurationManager.AppSettings")]
    public static NameValueCollection AppSettings => ConfigurationManager.AppSettings;

    /// <summary>Returns the <see cref="T:System.Configuration.ConfigurationSection" /> object for the passed configuration section name and path.</summary>
    /// <param name="sectionName">A configuration name and path, such as "system.net/settings".</param>
    /// <returns>The <see cref="T:System.Configuration.ConfigurationSection" /> object for the passed configuration section name and path.
    /// 
    /// The <see cref="T:System.Configuration.ConfigurationSettings" /> class provides backward compatibility only. You should use the <see cref="T:System.Configuration.ConfigurationManager" /> class or <see cref="T:System.Web.Configuration.WebConfigurationManager" /> class instead.</returns>
    /// <exception cref="T:System.Configuration.ConfigurationException">Unable to retrieve the requested section.</exception>
    [Obsolete("This method is obsolete, it has been replaced by System.Configuration!System.Configuration.ConfigurationManager.GetSection")]
    public static object GetConfig(string sectionName)
    {
      return ConfigurationManager.GetSection(sectionName);
    }
  }
}
