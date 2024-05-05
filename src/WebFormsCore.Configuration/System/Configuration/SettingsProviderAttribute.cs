// Decompiled with JetBrains decompiler
// Type: System.Configuration.SettingsProviderAttribute
// Assembly: System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 84F5A18A-F2B1-435C-B86E-09CE162E61E4
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System\v4.0_4.0.0.0__b77a5c561934e089\System.dll
// XML documentation location: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1\System.xml

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
