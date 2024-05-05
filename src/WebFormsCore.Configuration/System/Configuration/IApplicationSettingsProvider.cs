// Decompiled with JetBrains decompiler
// Type: System.Configuration.IApplicationSettingsProvider
// Assembly: System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 84F5A18A-F2B1-435C-B86E-09CE162E61E4
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System\v4.0_4.0.0.0__b77a5c561934e089\System.dll
// XML documentation location: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1\System.xml

using System.Security.Permissions;

#nullable disable
namespace System.Configuration
{
  /// <summary>Defines extended capabilities for client-based application settings providers.</summary>
  public interface IApplicationSettingsProvider
  {
    /// <summary>Returns the value of the specified settings property for the previous version of the same application.</summary>
    /// <param name="context">A <see cref="T:System.Configuration.SettingsContext" /> describing the current application usage.</param>
    /// <param name="property">The <see cref="T:System.Configuration.SettingsProperty" /> whose value is to be returned.</param>
    /// <returns>A <see cref="T:System.Configuration.SettingsPropertyValue" /> containing the value of the specified property setting as it was last set in the previous version of the application; or <see langword="null" /> if the setting cannot be found.</returns>
    [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
    SettingsPropertyValue GetPreviousVersion(SettingsContext context, SettingsProperty property);

    /// <summary>Resets the application settings associated with the specified application to their default values.</summary>
    /// <param name="context">A <see cref="T:System.Configuration.SettingsContext" /> describing the current application usage.</param>
    [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
    void Reset(SettingsContext context);

    /// <summary>Indicates to the provider that the application has been upgraded. This offers the provider an opportunity to upgrade its stored settings as appropriate.</summary>
    /// <param name="context">A <see cref="T:System.Configuration.SettingsContext" /> describing the current application usage.</param>
    /// <param name="properties">A <see cref="T:System.Configuration.SettingsPropertyCollection" /> containing the settings property group whose values are to be retrieved.</param>
    [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
    void Upgrade(SettingsContext context, SettingsPropertyCollection properties);
  }
}
