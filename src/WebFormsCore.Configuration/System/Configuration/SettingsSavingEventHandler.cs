// Decompiled with JetBrains decompiler
// Type: System.Configuration.SettingsSavingEventHandler
// Assembly: System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 84F5A18A-F2B1-435C-B86E-09CE162E61E4
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System\v4.0_4.0.0.0__b77a5c561934e089\System.dll
// XML documentation location: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1\System.xml

using System.ComponentModel;

#nullable disable
namespace System.Configuration
{
  /// <summary>Represents the method that will handle the <see cref="E:System.Configuration.ApplicationSettingsBase.SettingsSaving" /> event.</summary>
  /// <param name="sender">The source of the event, typically a data container or data-bound collection.</param>
  /// <param name="e">A <see cref="T:System.ComponentModel.CancelEventArgs" /> that contains the event data.</param>
  public delegate void SettingsSavingEventHandler(object sender, CancelEventArgs e);
}
