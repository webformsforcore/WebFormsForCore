// Decompiled with JetBrains decompiler
// Type: System.Configuration.SettingsGroupDescriptionAttribute
// Assembly: System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 84F5A18A-F2B1-435C-B86E-09CE162E61E4
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System\v4.0_4.0.0.0__b77a5c561934e089\System.dll
// XML documentation location: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1\System.xml

#nullable disable
namespace System.Configuration
{
  /// <summary>Provides a string that describes an application settings property group. This class cannot be inherited.</summary>
  [AttributeUsage(AttributeTargets.Class)]
  public sealed class SettingsGroupDescriptionAttribute : Attribute
  {
    private readonly string _desc;

    /// <summary>Initializes a new instance of the <see cref="T:System.Configuration.SettingsGroupDescriptionAttribute" /> class.</summary>
    /// <param name="description">A <see cref="T:System.String" /> containing the descriptive text for the application settings group.</param>
    public SettingsGroupDescriptionAttribute(string description) => this._desc = description;

    /// <summary>The descriptive text for the application settings properties group.</summary>
    /// <returns>A <see cref="T:System.String" /> containing the descriptive text for the application settings group.</returns>
    public string Description => this._desc;
  }
}
