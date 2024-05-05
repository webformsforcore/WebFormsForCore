// Decompiled with JetBrains decompiler
// Type: System.Configuration.IgnoreSectionHandler
// Assembly: System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 84F5A18A-F2B1-435C-B86E-09CE162E61E4
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System\v4.0_4.0.0.0__b77a5c561934e089\System.dll
// XML documentation location: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1\System.xml

using System.Xml;

#nullable disable
namespace System.Configuration
{
  /// <summary>Provides a legacy section-handler definition for configuration sections that are not handled by the <see cref="N:System.Configuration" /> types.</summary>
  public class IgnoreSectionHandler : IConfigurationSectionHandler
  {
    /// <summary>Creates a new configuration handler and adds the specified configuration object to the section-handler collection.</summary>
    /// <param name="parent">The configuration settings in a corresponding parent configuration section.</param>
    /// <param name="configContext">The virtual path for which the configuration section handler computes configuration values. Normally this parameter is reserved and is <see langword="null" />.</param>
    /// <param name="section">An <see cref="T:System.Xml.XmlNode" /> that contains the configuration information to be handled. Provides direct access to the XML contents of the configuration section.</param>
    /// <returns>The created configuration handler object.</returns>
    public virtual object Create(object parent, object configContext, XmlNode section)
    {
      return (object) null;
    }
  }
}
