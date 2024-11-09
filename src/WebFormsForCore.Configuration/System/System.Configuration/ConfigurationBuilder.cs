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
