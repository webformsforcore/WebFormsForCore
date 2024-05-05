// Decompiled with JetBrains decompiler
// Type: System.Configuration.SettingsDescriptionAttribute
// Assembly: System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 84F5A18A-F2B1-435C-B86E-09CE162E61E4
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System\v4.0_4.0.0.0__b77a5c561934e089\System.dll
// XML documentation location: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1\System.xml

#nullable disable
namespace System.Configuration
{
  /// <summary>Provides a string that describes an individual configuration property. This class cannot be inherited.</summary>
  [AttributeUsage(AttributeTargets.Property)]
  public sealed class SettingsDescriptionAttribute : Attribute
  {
    private readonly string _desc;

    /// <summary>Initializes an instance of the <see cref="T:System.Configuration.SettingsDescriptionAttribute" /> class.</summary>
    /// <param name="description">The <see cref="T:System.String" /> used as descriptive text.</param>
    public SettingsDescriptionAttribute(string description) => this._desc = description;

    /// <summary>Gets the descriptive text for the associated configuration property.</summary>
    /// <returns>A <see cref="T:System.String" /> containing the descriptive text for the associated configuration property.</returns>
    public string Description => this._desc;
  }
}
