using System.IO;

namespace Microsoft.Reporting.WebForms;

public interface ITemporaryStorage
{
	Stream CreateTemporaryStream();
}
