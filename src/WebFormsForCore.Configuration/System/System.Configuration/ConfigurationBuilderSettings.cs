// Decompiled with JetBrains decompiler
// Type: System.Configuration.ConfigurationBuilderSettings
// Assembly: System.Configuration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F80D3B8-83DB-4C4E-BE29-E92F4607776E
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System.Configuration\v4.0_4.0.0.0__b03f5f7f11d50a3a\System.Configuration.dll
// XML documentation location: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1\System.Configuration.xml

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
