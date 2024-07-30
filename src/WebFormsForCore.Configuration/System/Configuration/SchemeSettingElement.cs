
#nullable disable
namespace System.Configuration
{
  /// <summary>Represents an element in a <see cref="T:System.Configuration.SchemeSettingElementCollection" /> class.</summary>
  public sealed class SchemeSettingElement : ConfigurationElement
  {
    private static readonly ConfigurationPropertyCollection properties;
    private static readonly ConfigurationProperty name = new ConfigurationProperty(nameof (name), typeof (string), (object) null, ConfigurationPropertyOptions.IsRequired | ConfigurationPropertyOptions.IsKey);
    private static readonly ConfigurationProperty genericUriParserOptions = new ConfigurationProperty(nameof (genericUriParserOptions), typeof (GenericUriParserOptions), (object) GenericUriParserOptions.Default, ConfigurationPropertyOptions.IsRequired);

    static SchemeSettingElement()
    {
      SchemeSettingElement.properties = new ConfigurationPropertyCollection();
      SchemeSettingElement.properties.Add(SchemeSettingElement.name);
      SchemeSettingElement.properties.Add(SchemeSettingElement.genericUriParserOptions);
    }

    /// <summary>Gets the value of the Name entry from a <see cref="T:System.Configuration.SchemeSettingElement" /> instance.</summary>
    /// <returns>The protocol used by this schema setting.</returns>
    [ConfigurationProperty("name", DefaultValue = null, IsRequired = true, IsKey = true)]
    public string Name => (string) this[SchemeSettingElement.name];

    /// <summary>Gets the value of the GenericUriParserOptions entry from a <see cref="T:System.Configuration.SchemeSettingElement" /> instance.</summary>
    /// <returns>The value of GenericUriParserOptions entry.</returns>
    [ConfigurationProperty("genericUriParserOptions", DefaultValue = ConfigurationPropertyOptions.None, IsRequired = true)]
    public GenericUriParserOptions GenericUriParserOptions
    {
      get => (GenericUriParserOptions) this[SchemeSettingElement.genericUriParserOptions];
    }

    protected internal override ConfigurationPropertyCollection Properties
    {
      get => SchemeSettingElement.properties;
    }
  }
}
