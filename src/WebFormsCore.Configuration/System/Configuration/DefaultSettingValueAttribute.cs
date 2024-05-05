// Decompiled with JetBrains decompiler
// Type: System.Configuration.DefaultSettingValueAttribute
// Assembly: System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 84F5A18A-F2B1-435C-B86E-09CE162E61E4
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System\v4.0_4.0.0.0__b77a5c561934e089\System.dll
// XML documentation location: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1\System.xml

#nullable disable
namespace System.Configuration
{
  /// <summary>Specifies the default value for an application settings property.</summary>
  [AttributeUsage(AttributeTargets.Property)]
  public sealed class DefaultSettingValueAttribute : Attribute
  {
    private readonly string _value;

    /// <summary>Initializes an instance of the <see cref="T:System.Configuration.DefaultSettingValueAttribute" /> class.</summary>
    /// <param name="value">A <see cref="T:System.String" /> that represents the default value for the property.</param>
    public DefaultSettingValueAttribute(string value) => this._value = value;

    /// <summary>Gets the default value for the application settings property.</summary>
    /// <returns>A <see cref="T:System.String" /> that represents the default value for the property.</returns>
    public string Value => this._value;
  }
}
