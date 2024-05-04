using System.Xml;

namespace System.Configuration;

//
// Summary:
//     Handles the access to certain configuration sections.
public interface IConfigurationSectionHandler
{
	//
	// Summary:
	//     Creates a configuration section handler.
	//
	// Parameters:
	//   parent:
	//     Parent object.
	//
	//   configContext:
	//     Configuration context object.
	//
	//   section:
	//     Section XML node.
	//
	// Returns:
	//     The created section handler object.
	object Create(object parent, object configContext, XmlNode section);
}