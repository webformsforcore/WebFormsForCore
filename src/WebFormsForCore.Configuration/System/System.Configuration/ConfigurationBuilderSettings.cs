#nullable disable
namespace System.Configuration
{
  /// <summary>Represents a group of configuration elements that configure the providers for the <see langword="&lt;configBuilders&gt;" /> configuration section.</summary>
  public class ConfigurationBuilderSettings : ConfigurationElement
  {
    private ConfigurationPropertyCollection _properties;
    private readonly ConfigurationProperty _propBuilders = new ConfigurationProperty((string) null, typeof (ProviderSettingsCollection), (object) null, ConfigurationPropertyOptions.IsDefaultCollection);

    /// <summary>Initializes a new instance of the <see cref="T:System.Configuration.ConfigurationBuilderSettings" /> class.</summary>
    public ConfigurationBuilderSettings()
    {
      this._properties = new ConfigurationPropertyCollection();
      this._properties.Add(this._propBuilders);
    }

    /// <summary>Gets the <see cref="T:System.Configuration.ConfigurationPropertyCollection" /> of a <see cref="T:System.Configuration.ConfigurationElement" />.</summary>
    /// <returns>A <see cref="T:System.Configuration.ConfigurationPropertyCollection" /> of a <see cref="T:System.Configuration.ConfigurationElement" />.</returns>
    protected internal override ConfigurationPropertyCollection Properties => this._properties;

    /// <summary>Gets a collection of <see cref="T:System.Configuration.ConfigurationBuilderSettings" /> objects that represent the properties of configuration builders.</summary>
    /// <returns>The <see cref="T:System.Configuration.ConfigurationBuilder" /> objects.</returns>
    [ConfigurationProperty("", IsDefaultCollection = true, Options = ConfigurationPropertyOptions.IsDefaultCollection)]
    public ProviderSettingsCollection Builders
    {
      get => (ProviderSettingsCollection) this[this._propBuilders];
    }
  }
}
