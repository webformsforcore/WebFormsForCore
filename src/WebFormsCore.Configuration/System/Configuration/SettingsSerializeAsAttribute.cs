
#nullable disable
namespace System.Configuration
{
  /// <summary>Specifies the serialization mechanism that the settings provider should use. This class cannot be inherited.</summary>
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
  public sealed class SettingsSerializeAsAttribute : Attribute
  {
    private readonly SettingsSerializeAs _serializeAs;

    /// <summary>Initializes an instance of the <see cref="T:System.Configuration.SettingsSerializeAsAttribute" /> class.</summary>
    /// <param name="serializeAs">A <see cref="T:System.Configuration.SettingsSerializeAs" /> enumerated value that specifies the serialization scheme.</param>
    public SettingsSerializeAsAttribute(SettingsSerializeAs serializeAs)
    {
      this._serializeAs = serializeAs;
    }

    /// <summary>Gets the <see cref="T:System.Configuration.SettingsSerializeAs" /> enumeration value that specifies the serialization scheme.</summary>
    /// <returns>A <see cref="T:System.Configuration.SettingsSerializeAs" /> enumerated value that specifies the serialization scheme.</returns>
    public SettingsSerializeAs SerializeAs => this._serializeAs;
  }
}
