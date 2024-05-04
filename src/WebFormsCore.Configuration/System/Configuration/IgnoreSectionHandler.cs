#if NETCOREAPP

using System.Xml;

namespace System.Configuration;

//
// Summary:
//     Provides a legacy section-handler definition for configuration sections that
//     are not handled by the System.Configuration types.
public class IgnoreSectionHandler : IConfigurationSectionHandler
{
	//
	// Summary:
	//     Creates a new configuration handler and adds the specified configuration object
	//     to the section-handler collection.
	//
	// Parameters:
	//   parent:
	//     The configuration settings in a corresponding parent configuration section.
	//
	//   configContext:
	//     The virtual path for which the configuration section handler computes configuration
	//     values. Normally this parameter is reserved and is null.
	//
	//   section:
	//     An System.Xml.XmlNode that contains the configuration information to be handled.
	//     Provides direct access to the XML contents of the configuration section.
	//
	// Returns:
	//     The created configuration handler object.
	public virtual object Create(object parent, object configContext, XmlNode section)
	{
		return null;
	}

	//
	// Summary:
	//     Initializes a new instance of the System.Configuration.IgnoreSectionHandler class.
	public IgnoreSectionHandler()
	{
	}
}
#endif