// Decompiled with JetBrains decompiler
// Type: System.Configuration.SettingChangingEventHandler
// Assembly: System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 84F5A18A-F2B1-435C-B86E-09CE162E61E4
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System\v4.0_4.0.0.0__b77a5c561934e089\System.dll
// XML documentation location: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1\System.xml

#nullable disable
namespace System.Configuration
{
  /// <summary>Represents the method that will handle the <see cref="E:System.Configuration.ApplicationSettingsBase.SettingChanging" /> event.</summary>
  /// <param name="sender">The source of the event, typically an application settings wrapper class derived from the <see cref="T:System.Configuration.ApplicationSettingsBase" /> class.</param>
  /// <param name="e">A <see cref="T:System.Configuration.SettingChangingEventArgs" /> containing the data for the event.</param>
  public delegate void SettingChangingEventHandler(object sender, SettingChangingEventArgs e);
}
