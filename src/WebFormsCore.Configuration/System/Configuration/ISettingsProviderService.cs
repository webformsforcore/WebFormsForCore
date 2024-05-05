// Decompiled with JetBrains decompiler
// Type: System.Configuration.ISettingsProviderService
// Assembly: System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 84F5A18A-F2B1-435C-B86E-09CE162E61E4
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System\v4.0_4.0.0.0__b77a5c561934e089\System.dll
// XML documentation location: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1\System.xml

#nullable disable
namespace System.Configuration
{
  /// <summary>Provides an interface for defining an alternate application settings provider.</summary>
  public interface ISettingsProviderService
  {
    /// <summary>Returns the settings provider compatible with the specified settings property.</summary>
    /// <param name="property">The <see cref="T:System.Configuration.SettingsProperty" /> that requires serialization.</param>
    /// <returns>If found, the <see cref="T:System.Configuration.SettingsProvider" /> that can persist the specified settings property; otherwise, <see langword="null" />.</returns>
    SettingsProvider GetSettingsProvider(SettingsProperty property);
  }
}
