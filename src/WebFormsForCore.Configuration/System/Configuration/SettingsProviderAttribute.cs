
#nullable disable
namespace System.Configuration
{
  /// <summary>Specifies the settings provider used to provide storage for the current application settings class or property. This class cannot be inherited.</summary>
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
  public sealed class SettingsProviderAttribute : Attribute
  {
    private readonly string _providerTypeName;

    /// <summary>Initializes an instance of the <see cref="T:System.Configuration.SettingsProviderAttribute" /> class.</summary>
    /// <param name="providerTypeName">A <see cref="T:System.String" /> containing the name of the settings provider.</param>
    public SettingsProviderAttribute(string providerTypeName)
    {
      this._providerTypeName = providerTypeName;
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.Configuration.SettingsProviderAttribute" /> class.</summary>
    /// <param name="providerType">A <see cref="T:System.Type" /> containing the settings provider type.</param>
    public SettingsProviderAttribute(Type providerType)
    {
      if (!(providerType != (Type) null))
        return;
      this._providerTypeName = providerType.AssemblyQualifiedName;
    }

    /// <summary>Gets the type name of the settings provider.</summary>
    /// <returns>A <see cref="T:System.String" /> containing the name of the settings provider.</returns>
    public string ProviderTypeName => this._providerTypeName;
  }
}
