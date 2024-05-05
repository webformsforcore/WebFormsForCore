// Decompiled with JetBrains decompiler
// Type: System.Configuration.SpecialSettingAttribute
// Assembly: System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 84F5A18A-F2B1-435C-B86E-09CE162E61E4
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System\v4.0_4.0.0.0__b77a5c561934e089\System.dll
// XML documentation location: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1\System.xml

#nullable disable
namespace System.Configuration
{
  /// <summary>Indicates that an application settings property has a special significance. This class cannot be inherited.</summary>
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
  public sealed class SpecialSettingAttribute : Attribute
  {
    private readonly SpecialSetting _specialSetting;

    /// <summary>Initializes a new instance of the <see cref="T:System.Configuration.SpecialSettingAttribute" /> class.</summary>
    /// <param name="specialSetting">A <see cref="T:System.Configuration.SpecialSetting" /> enumeration value defining the category of the application settings property.</param>
    public SpecialSettingAttribute(SpecialSetting specialSetting)
    {
      this._specialSetting = specialSetting;
    }

    /// <summary>Gets the value describing the special setting category of the application settings property.</summary>
    /// <returns>A <see cref="T:System.Configuration.SpecialSetting" /> enumeration value defining the category of the application settings property.</returns>
    public SpecialSetting SpecialSetting => this._specialSetting;
  }
}
