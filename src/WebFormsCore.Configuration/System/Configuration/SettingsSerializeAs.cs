
#nullable disable
namespace System.Configuration
{
  /// <summary>Determines the serialization scheme used to store application settings.</summary>
  public enum SettingsSerializeAs
  {
    /// <summary>The settings property is serialized as plain text.</summary>
    String,
    /// <summary>The settings property is serialized as XML using XML serialization.</summary>
    Xml,
    /// <summary>The settings property is serialized using binary object serialization.</summary>
    Binary,
    /// <summary>The settings provider has implicit knowledge of the property or its type and picks an appropriate serialization mechanism. Often used for custom serialization.</summary>
    ProviderSpecific,
  }
}
