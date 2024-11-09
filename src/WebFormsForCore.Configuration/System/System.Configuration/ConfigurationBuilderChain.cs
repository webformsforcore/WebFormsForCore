using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration.Internal;
using System.Xml;

#nullable disable
namespace System.Configuration
{
  internal class ConfigurationBuilderChain : ConfigurationBuilder
  {
    private List<ConfigurationBuilder> _builders;

    public List<ConfigurationBuilder> Builders => this._builders;

    public override void Initialize(string name, NameValueCollection config)
    {
      this._builders = new List<ConfigurationBuilder>();
      base.Initialize(name, config);
    }

    public override XmlNode ProcessRawXml(XmlNode rawXml)
    {
      XmlNode rawXml1 = rawXml;
      string str = (string) null;
      try
      {
        foreach (ConfigurationBuilder builder in this._builders)
        {
          str = builder.Name;
          rawXml1 = builder.ProcessRawXml(rawXml1);
        }
        return rawXml1;
      }
      catch (Exception ex)
      {
        throw ExceptionUtil.WrapAsConfigException(SR.GetString("ConfigBuilder_processXml_error_short", (object) str), ex, (IConfigErrorInfo) null);
      }
    }

    public override ConfigurationSection ProcessConfigurationSection(
      ConfigurationSection configSection)
    {
      ConfigurationSection configSection1 = configSection;
      string str = (string) null;
      try
      {
        foreach (ConfigurationBuilder builder in this._builders)
        {
          str = builder.Name;
          configSection1 = builder.ProcessConfigurationSection(configSection1);
        }
        return configSection1;
      }
      catch (Exception ex)
      {
        throw ExceptionUtil.WrapAsConfigException(SR.GetString("ConfigBuilder_processSection_error", (object) str, (object) configSection.SectionInformation.Name), ex, (IConfigErrorInfo) null);
      }
    }
  }
}
