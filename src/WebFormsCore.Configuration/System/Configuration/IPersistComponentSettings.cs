// Decompiled with JetBrains decompiler
// Type: System.Configuration.IPersistComponentSettings
// Assembly: System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 84F5A18A-F2B1-435C-B86E-09CE162E61E4
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System\v4.0_4.0.0.0__b77a5c561934e089\System.dll
// XML documentation location: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1\System.xml

#nullable disable
namespace System.Configuration
{
  /// <summary>Defines standard functionality for controls or libraries that store and retrieve application settings.</summary>
  public interface IPersistComponentSettings
  {
    /// <summary>Gets or sets a value indicating whether the control should automatically persist its application settings properties.</summary>
    /// <returns>
    /// <see langword="true" /> if the control should automatically persist its state; otherwise, <see langword="false" />.</returns>
    bool SaveSettings { get; set; }

    /// <summary>Gets or sets the value of the application settings key for the current instance of the control.</summary>
    /// <returns>A <see cref="T:System.String" /> containing the settings key for the current instance of the control.</returns>
    string SettingsKey { get; set; }

    /// <summary>Reads the control's application settings into their corresponding properties and updates the control's state.</summary>
    void LoadComponentSettings();

    /// <summary>Persists the control's application settings properties.</summary>
    void SaveComponentSettings();

    /// <summary>Resets the control's application settings properties to their default values.</summary>
    void ResetComponentSettings();
  }
}
