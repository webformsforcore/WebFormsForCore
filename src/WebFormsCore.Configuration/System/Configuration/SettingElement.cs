
using System.Xml;

#nullable disable
namespace System.Configuration
{
  /// <summary>Represents a simplified configuration element used for updating elements in the configuration. This class cannot be inherited.</summary>
  public sealed class SettingElement : ConfigurationElement
  {
    private static ConfigurationPropertyCollection _properties;
    private static readonly ConfigurationProperty _propName = new ConfigurationProperty("name", typeof (string), (object) "", ConfigurationPropertyOptions.IsRequired | ConfigurationPropertyOptions.IsKey);
    private static readonly ConfigurationProperty _propSerializeAs = new ConfigurationProperty("serializeAs", typeof (SettingsSerializeAs), (object) SettingsSerializeAs.String, ConfigurationPropertyOptions.IsRequired);
    private static readonly ConfigurationProperty _propValue = new ConfigurationProperty("value", typeof (SettingValueElement), (object) null, ConfigurationPropertyOptions.IsRequired);
    private static XmlDocument doc = new XmlDocument();

    static SettingElement()
    {
      SettingElement._properties = new ConfigurationPropertyCollection();
      SettingElement._properties.Add(SettingElement._propName);
      SettingElement._properties.Add(SettingElement._propSerializeAs);
      SettingElement._properties.Add(SettingElement._propValue);
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.Configuration.SettingElement" /> class.</summary>
    public SettingElement()
    {
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.Configuration.SettingElement" /> class based on supplied parameters.</summary>
    /// <param name="name">The name of the <see cref="T:System.Configuration.SettingElement" /> object.</param>
    /// <param name="serializeAs">A <see cref="T:System.Configuration.SettingsSerializeAs" /> object. This object is an enumeration used as the serialization scheme to store configuration settings.</param>
    public SettingElement(string name, SettingsSerializeAs serializeAs)
      : this()
    {
      this.Name = name;
      this.SerializeAs = serializeAs;
    }

    internal string Key => this.Name;

    /// <summary>Compares the current <see cref="T:System.Configuration.SettingElement" /> instance to the specified object.</summary>
    /// <param name="settings">The object to compare with.</param>
    /// <returns>
    /// <see langword="true" /> if the <see cref="T:System.Configuration.SettingElement" /> instance is equal to the specified object; otherwise, <see langword="false" />.</returns>
    public override bool Equals(object settings)
    {
      return settings is SettingElement settingElement && base.Equals(settings) && object.Equals((object) settingElement.Value, (object) this.Value);
    }

    /// <summary>Gets a unique value representing the <see cref="T:System.Configuration.SettingElement" /> current instance.</summary>
    /// <returns>A unique value representing the <see cref="T:System.Configuration.SettingElement" /> current instance.</returns>
    public override int GetHashCode() => base.GetHashCode() ^ this.Value.GetHashCode();

    protected internal override ConfigurationPropertyCollection Properties => SettingElement._properties;

    /// <summary>Gets or sets the name of the <see cref="T:System.Configuration.SettingElement" /> object.</summary>
    /// <returns>The name of the <see cref="T:System.Configuration.SettingElement" /> object.</returns>
    [ConfigurationProperty("name", IsRequired = true, IsKey = true, DefaultValue = "")]
    public string Name
    {
      get => (string) this[SettingElement._propName];
      set => this[SettingElement._propName] = (object) value;
    }

    /// <summary>Gets or sets the serialization mechanism used to persist the values of the <see cref="T:System.Configuration.SettingElement" /> object.</summary>
    /// <returns>A <see cref="T:System.Configuration.SettingsSerializeAs" /> object.</returns>
    [ConfigurationProperty("serializeAs", IsRequired = true, DefaultValue = SettingsSerializeAs.String)]
    public SettingsSerializeAs SerializeAs
    {
      get => (SettingsSerializeAs) this[SettingElement._propSerializeAs];
      set => this[SettingElement._propSerializeAs] = (object) value;
    }

    /// <summary>Gets or sets the value of a <see cref="T:System.Configuration.SettingElement" /> object by using a <see cref="T:System.Configuration.SettingValueElement" /> object.</summary>
    /// <returns>A <see cref="T:System.Configuration.SettingValueElement" /> object containing the value of the <see cref="T:System.Configuration.SettingElement" /> object.</returns>
    [ConfigurationProperty("value", IsRequired = true, DefaultValue = null)]
    public SettingValueElement Value
    {
      get => (SettingValueElement) this[SettingElement._propValue];
      set => this[SettingElement._propValue] = (object) value;
    }
  }
}
