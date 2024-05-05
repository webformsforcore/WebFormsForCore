// Decompiled with JetBrains decompiler
// Type: System.Configuration.SettingChangingEventArgs
// Assembly: System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 84F5A18A-F2B1-435C-B86E-09CE162E61E4
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System\v4.0_4.0.0.0__b77a5c561934e089\System.dll
// XML documentation location: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1\System.xml

using System.ComponentModel;

#nullable disable
namespace System.Configuration
{
  /// <summary>Provides data for the <see cref="E:System.Configuration.ApplicationSettingsBase.SettingChanging" /> event.</summary>
  public class SettingChangingEventArgs : CancelEventArgs
  {
    private string _settingClass;
    private string _settingName;
    private string _settingKey;
    private object _newValue;

    /// <summary>Initializes an instance of the <see cref="T:System.Configuration.SettingChangingEventArgs" /> class.</summary>
    /// <param name="settingName">A <see cref="T:System.String" /> containing the name of the application setting.</param>
    /// <param name="settingClass">A <see cref="T:System.String" /> containing a category description of the setting. Often this parameter is set to the application settings group name.</param>
    /// <param name="settingKey">A <see cref="T:System.String" /> containing the application settings key.</param>
    /// <param name="newValue">An <see cref="T:System.Object" /> that contains the new value to be assigned to the application settings property.</param>
    /// <param name="cancel">
    /// <see langword="true" /> to cancel the event; otherwise, <see langword="false" />.</param>
    public SettingChangingEventArgs(
      string settingName,
      string settingClass,
      string settingKey,
      object newValue,
      bool cancel)
      : base(cancel)
    {
      this._settingName = settingName;
      this._settingClass = settingClass;
      this._settingKey = settingKey;
      this._newValue = newValue;
    }

    /// <summary>Gets the new value being assigned to the application settings property.</summary>
    /// <returns>An <see cref="T:System.Object" /> that contains the new value to be assigned to the application settings property.</returns>
    public object NewValue => this._newValue;

    /// <summary>Gets the application settings property category.</summary>
    /// <returns>A <see cref="T:System.String" /> containing a category description of the setting. Typically, this parameter is set to the application settings group name.</returns>
    public string SettingClass => this._settingClass;

    /// <summary>Gets the name of the application setting associated with the application settings property.</summary>
    /// <returns>A <see cref="T:System.String" /> containing the name of the application setting.</returns>
    public string SettingName => this._settingName;

    /// <summary>Gets the application settings key associated with the property.</summary>
    /// <returns>A <see cref="T:System.String" /> containing the application settings key.</returns>
    public string SettingKey => this._settingKey;
  }
}
