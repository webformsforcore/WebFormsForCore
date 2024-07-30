
using System.Xml;

#nullable disable
namespace System.Configuration
{
  /// <summary>Handles the access to certain configuration sections.</summary>
  public interface IConfigurationSectionHandler
  {
    /// <summary>Creates a configuration section handler.</summary>
    /// <param name="parent">Parent object.</param>
    /// <param name="configContext">Configuration context object.</param>
    /// <param name="section">Section XML node.</param>
    /// <returns>The created section handler object.</returns>
    object Create(object parent, object configContext, XmlNode section);
  }
}
