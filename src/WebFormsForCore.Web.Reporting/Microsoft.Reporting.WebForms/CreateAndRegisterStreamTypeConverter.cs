using System.Text;
using Microsoft.ReportingServices.Interfaces;

namespace Microsoft.Reporting.WebForms;

internal static class CreateAndRegisterStreamTypeConverter
{
	internal static CreateAndRegisterStream ToInnerType(this CreateAndRegisterStream callback)
	{
		return (string name, string extension, Encoding encoding, string mimeType, bool willSeek, StreamOper operation) => callback(name, extension, encoding, mimeType, willSeek, operation);
	}

	internal static CreateAndRegisterStream ToOuterType(this CreateAndRegisterStream callback)
	{
		return (string name, string extension, Encoding encoding, string mimeType, bool willSeek, StreamOper operation) => callback(name, extension, encoding, mimeType, willSeek, operation);
	}
}
