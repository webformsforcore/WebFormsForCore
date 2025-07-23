using System;
using System.Configuration;
using System.Runtime.Serialization;

namespace Microsoft.Reporting.WebForms;

[Serializable]
public sealed class InvalidConfigFileTypeException : ConfigurationErrorsException
{
	internal InvalidConfigFileTypeException(string typeName)
		: base(Microsoft.Reporting.WebForms.Errors.CantFindConfigFileType(typeName))
	{
	}

	internal InvalidConfigFileTypeException(string typeName, string expectedTypeName)
		: base(Microsoft.Reporting.WebForms.Errors.MissingInterfaceOnConfigFileType(typeName, expectedTypeName))
	{
	}

	private InvalidConfigFileTypeException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
