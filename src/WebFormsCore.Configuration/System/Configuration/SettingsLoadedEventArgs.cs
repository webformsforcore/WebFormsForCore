// Decompiled with JetBrains decompiler
// Type: System.Configuration.SettingsLoadedEventArgs
// Assembly: System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 84F5A18A-F2B1-435C-B86E-09CE162E61E4
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System\v4.0_4.0.0.0__b77a5c561934e089\System.dll
// XML documentation location: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1\System.xml

#nullable disable
namespace System.Configuration
{
  /// <summary>Provides data for the <see cref="E:System.Configuration.ApplicationSettingsBase.SettingsLoaded" /> event.</summary>
  public class SettingsLoadedEventArgs : EventArgs
  {
    private SettingsProvider _provider;

    /// <summary>Initializes a new instance of the <see cref="T:System.Configuration.SettingsLoadedEventArgs" /> class.</summary>
    /// <param name="provider">A <see cref="T:System.Configuration.SettingsProvider" /> object from which settings are loaded.</param>
    public SettingsLoadedEventArgs(SettingsProvider provider) => this._provider = provider;

    /// <summary>Gets the settings provider used to store configuration settings.</summary>
    /// <returns>A settings provider.</returns>
    public SettingsProvider Provider => this._provider;
  }
}
