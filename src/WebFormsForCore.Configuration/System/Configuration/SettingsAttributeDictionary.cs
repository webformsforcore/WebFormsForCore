
using System.Collections;

#nullable disable
namespace System.Configuration
{
  /// <summary>Represents a collection of key/value pairs used to describe a configuration object as well as a <see cref="T:System.Configuration.SettingsProperty" /> object.</summary>
  [Serializable]
  public class SettingsAttributeDictionary : Hashtable
  {
    /// <summary>Initializes a new instance of the <see cref="T:System.Configuration.SettingsAttributeDictionary" /> class.</summary>
    public SettingsAttributeDictionary()
    {
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.Configuration.SettingsAttributeDictionary" /> class.</summary>
    /// <param name="attributes">A collection of key/value pairs that are related to configuration settings.</param>
    public SettingsAttributeDictionary(SettingsAttributeDictionary attributes)
      : base((IDictionary) attributes)
    {
    }
  }
}
