// Decompiled with JetBrains decompiler
// Type: System.Configuration.SettingsProvider
// Assembly: System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 84F5A18A-F2B1-435C-B86E-09CE162E61E4
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System\v4.0_4.0.0.0__b77a5c561934e089\System.dll
// XML documentation location: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1\System.xml

using System.Configuration.Provider;

#nullable disable
namespace System.Configuration
{
  /// <summary>Acts as a base class for deriving custom settings providers in the application settings architecture.</summary>
  public abstract class SettingsProvider : ProviderBase
  {
    /// <summary>Returns the collection of settings property values for the specified application instance and settings property group.</summary>
    /// <param name="context">A <see cref="T:System.Configuration.SettingsContext" /> describing the current application use.</param>
    /// <param name="collection">A <see cref="T:System.Configuration.SettingsPropertyCollection" /> containing the settings property group whose values are to be retrieved.</param>
    /// <returns>A <see cref="T:System.Configuration.SettingsPropertyValueCollection" /> containing the values for the specified settings property group.</returns>
    public abstract SettingsPropertyValueCollection GetPropertyValues(
      SettingsContext context,
      SettingsPropertyCollection collection);

    /// <summary>Sets the values of the specified group of property settings.</summary>
    /// <param name="context">A <see cref="T:System.Configuration.SettingsContext" /> describing the current application usage.</param>
    /// <param name="collection">A <see cref="T:System.Configuration.SettingsPropertyValueCollection" /> representing the group of property settings to set.</param>
    public abstract void SetPropertyValues(
      SettingsContext context,
      SettingsPropertyValueCollection collection);

    /// <summary>Gets or sets the name of the currently running application.</summary>
    /// <returns>A <see cref="T:System.String" /> that contains the application's shortened name, which does not contain a full path or extension, for example, <c>SimpleAppSettings</c>.</returns>
    public abstract string ApplicationName { get; set; }
  }
}
