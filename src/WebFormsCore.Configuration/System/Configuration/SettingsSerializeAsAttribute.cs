// Decompiled with JetBrains decompiler
// Type: System.Configuration.SettingsSerializeAsAttribute
// Assembly: System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 84F5A18A-F2B1-435C-B86E-09CE162E61E4
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System\v4.0_4.0.0.0__b77a5c561934e089\System.dll
// XML documentation location: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1\System.xml

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
