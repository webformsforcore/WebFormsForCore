// Decompiled with JetBrains decompiler
// Type: System.Configuration.ClientSettingsSection
// Assembly: System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 84F5A18A-F2B1-435C-B86E-09CE162E61E4
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System\v4.0_4.0.0.0__b77a5c561934e089\System.dll
// XML documentation location: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1\System.xml

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
