// Decompiled with JetBrains decompiler
// Type: System.Configuration.SettingsSerializeAs
// Assembly: System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 84F5A18A-F2B1-435C-B86E-09CE162E61E4
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System\v4.0_4.0.0.0__b77a5c561934e089\System.dll
// XML documentation location: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1\System.xml

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
