// Decompiled with JetBrains decompiler
// Type: System.Configuration.SettingsGroupNameAttribute
// Assembly: System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 84F5A18A-F2B1-435C-B86E-09CE162E61E4
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System\v4.0_4.0.0.0__b77a5c561934e089\System.dll
// XML documentation location: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1\System.xml

#nullable disable
namespace System.Configuration
{
  /// <summary>Specifies a name for application settings property group. This class cannot be inherited.</summary>
  [AttributeUsage(AttributeTargets.Class)]
  public sealed class SettingsGroupNameAttribute : Attribute
  {
    private readonly string _groupName;

    /// <summary>Initializes a new instance of the <see cref="T:System.Configuration.SettingsGroupNameAttribute" /> class.</summary>
    /// <param name="groupName">A <see cref="T:System.String" /> containing the name of the application settings property group.</param>
    public SettingsGroupNameAttribute(string groupName) => this._groupName = groupName;

    /// <summary>Gets the name of the application settings property group.</summary>
    /// <returns>A <see cref="T:System.String" /> containing the name of the application settings property group.</returns>
    public string GroupName => this._groupName;
  }
}
