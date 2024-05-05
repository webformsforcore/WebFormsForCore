// Decompiled with JetBrains decompiler
// Type: System.Configuration.SingleTagSectionHandler
// Assembly: System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 84F5A18A-F2B1-435C-B86E-09CE162E61E4
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System\v4.0_4.0.0.0__b77a5c561934e089\System.dll
// XML documentation location: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1\System.xml

using System.Collections;
using System.Xml;

#nullable disable
namespace System.Configuration
{
  /// <summary>Handles configuration sections that are represented by a single XML tag in the .config file.</summary>
  public class SingleTagSectionHandler : IConfigurationSectionHandler
  {
    /// <summary>Used internally to create a new instance of this object.</summary>
    /// <param name="parent">The parent of this object.</param>
    /// <param name="context">The context of this object.</param>
    /// <param name="section">The <see cref="T:System.Xml.XmlNode" /> object in the configuration.</param>
    /// <returns>The created object handler.</returns>
    public virtual object Create(object parent, object context, XmlNode section)
    {
      Hashtable hashtable = parent != null ? new Hashtable((IDictionary) parent) : new Hashtable();
      HandlerBase.CheckForChildNodes(section);
      foreach (XmlAttribute attribute in (XmlNamedNodeMap) section.Attributes)
        hashtable[(object) attribute.Name] = (object) attribute.Value;
      return (object) hashtable;
    }
  }
}
