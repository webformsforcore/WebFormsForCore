// Decompiled with JetBrains decompiler
// Type: System.Configuration.ConfigurationBuilder
// Assembly: System.Configuration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F80D3B8-83DB-4C4E-BE29-E92F4607776E
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System.Configuration\v4.0_4.0.0.0__b03f5f7f11d50a3a\System.Configuration.dll
// XML documentation location: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1\System.Configuration.xml

using System.Configuration.Provider;
using System.Xml;

#nullable disable
namespace System.Configuration
{
  /// <summary>Represents the base class to be extended by custom configuration builder implementations.</summary>
  public abstract class ConfigurationBuilder : ProviderBase
  {
    /// <summary>Accepts an <see cref="T:System.Xml.XmlNode" /> representing the raw configuration section from a config file and returns a modified or new <see cref="T:System.Xml.XmlNode" /> for further use.</summary>
    /// <param name="rawXml">The <see cref="T:System.Xml.XmlNode" /> to process.</param>
    /// <returns>The processed <see cref="T:System.Xml.XmlNode" />.</returns>
    public virtual XmlNode ProcessRawXml(XmlNode rawXml) => rawXml;

    /// <summary>Accepts a <see cref="T:System.Configuration.ConfigurationSection" /> object from the configuration system and returns a modified or new <see cref="T:System.Configuration.ConfigurationSection" /> object for further use.</summary>
    /// <param name="configSection">The <see cref="T:System.Configuration.ConfigurationSection" /> to process.</param>
    /// <returns>The processed <see cref="T:System.Configuration.ConfigurationSection" />.</returns>
    public virtual ConfigurationSection ProcessConfigurationSection(
      ConfigurationSection configSection)
    {
      return configSection;
    }
  }
}
