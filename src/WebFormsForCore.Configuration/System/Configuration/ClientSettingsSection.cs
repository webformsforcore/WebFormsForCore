#nullable disable
namespace System.Configuration
{
  /// <summary>Represents a group of user-scoped application settings in a configuration file.</summary>
  public sealed class ClientSettingsSection : ConfigurationSection
  {
    private static ConfigurationPropertyCollection _properties;
    private static readonly ConfigurationProperty _propSettings = new ConfigurationProperty((string) null, typeof (SettingElementCollection), (object) null, ConfigurationPropertyOptions.IsDefaultCollection);

    static ClientSettingsSection()
    {
      ClientSettingsSection._properties = new ConfigurationPropertyCollection();
      ClientSettingsSection._properties.Add(ClientSettingsSection._propSettings);
    }

    protected internal override ConfigurationPropertyCollection Properties
    {
      get => ClientSettingsSection._properties;
    }

    /// <summary>Gets the collection of client settings for the section.</summary>
    /// <returns>A <see cref="T:System.Configuration.SettingElementCollection" /> containing all the client settings found in the current configuration section.</returns>
    [ConfigurationProperty("", IsDefaultCollection = true)]
    public SettingElementCollection Settings
    {
      get => (SettingElementCollection) this[ClientSettingsSection._propSettings];
    }
  }
}
