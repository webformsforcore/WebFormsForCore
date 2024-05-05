// Decompiled with JetBrains decompiler
// Type: System.Configuration.NameValueFileSectionHandler
// Assembly: System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 84F5A18A-F2B1-435C-B86E-09CE162E61E4
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System\v4.0_4.0.0.0__b77a5c561934e089\System.dll
// XML documentation location: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1\System.xml

using System.Configuration.Internal;
using System.IO;
using System.Xml;

#nullable disable
namespace System.Configuration
{
  /// <summary>Provides access to a configuration file. This type supports the .NET Framework configuration infrastructure and is not intended to be used directly from your code.</summary>
  public class NameValueFileSectionHandler : IConfigurationSectionHandler
  {
    /// <summary>Creates a new configuration handler and adds it to the section-handler collection based on the specified parameters.</summary>
    /// <param name="parent">The parent object.</param>
    /// <param name="configContext">The configuration context object.</param>
    /// <param name="section">The section XML node.</param>
    /// <returns>A configuration object.</returns>
    /// <exception cref="T:System.Configuration.ConfigurationErrorsException">The file specified in the <see langword="file" /> attribute of <paramref name="section" /> exists but cannot be loaded.
    /// -or-
    ///  The <see langword="name" /> attribute of <paramref name="section" /> does not match the root element of the file specified in the <see langword="file" /> attribute.</exception>
    public object Create(object parent, object configContext, XmlNode section)
    {
      object parent1 = parent;
      XmlNode xmlNode = section.Attributes.RemoveNamedItem("file");
      object parent2 = NameValueSectionHandler.CreateStatic(parent1, section);
      if (xmlNode != null && xmlNode.Value.Length != 0)
      {
        string path2 = xmlNode.Value;
        if (!(xmlNode is IConfigErrorInfo configErrorInfo))
          return (object) null;
        string str = Path.Combine(Path.GetDirectoryName(configErrorInfo.Filename), path2);
        if (File.Exists(str))
        {
          ConfigXmlDocument configXmlDocument = new ConfigXmlDocument();
          try
          {
            configXmlDocument.Load(str);
          }
          catch (XmlException ex)
          {
            throw new ConfigurationErrorsException(ex.Message, (Exception) ex, str, ex.LineNumber);
          }
          if (section.Name != configXmlDocument.DocumentElement.Name)
            throw new ConfigurationErrorsException(SR.GetString("Config_name_value_file_section_file_invalid_root", (object) section.Name), (XmlNode) configXmlDocument.DocumentElement);
          parent2 = NameValueSectionHandler.CreateStatic(parent2, (XmlNode) configXmlDocument.DocumentElement);
        }
      }
      return parent2;
    }
  }
}
