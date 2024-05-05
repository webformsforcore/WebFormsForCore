// Decompiled with JetBrains decompiler
// Type: System.Configuration.SettingsManageabilityAttribute
// Assembly: System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 84F5A18A-F2B1-435C-B86E-09CE162E61E4
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System\v4.0_4.0.0.0__b77a5c561934e089\System.dll
// XML documentation location: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1\System.xml

#nullable disable
namespace System.Configuration
{
  /// <summary>Specifies special services for application settings properties. This class cannot be inherited.</summary>
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
  public sealed class SettingsManageabilityAttribute : Attribute
  {
    private readonly SettingsManageability _manageability;

    /// <summary>Initializes a new instance of the <see cref="T:System.Configuration.SettingsManageabilityAttribute" /> class.</summary>
    /// <param name="manageability">A <see cref="T:System.Configuration.SettingsManageability" /> value that enumerates the services being requested.</param>
    public SettingsManageabilityAttribute(SettingsManageability manageability)
    {
      this._manageability = manageability;
    }

    /// <summary>Gets the set of special services that have been requested.</summary>
    /// <returns>A value that results from using the logical <see langword="OR" /> operator to combine all the <see cref="T:System.Configuration.SettingsManageability" /> enumeration values corresponding to the requested services.</returns>
    public SettingsManageability Manageability => this._manageability;
  }
}
