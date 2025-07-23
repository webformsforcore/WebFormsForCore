using System.Collections.Generic;
using System.Net;

namespace Microsoft.Reporting.WebForms;

public interface IReportServerConnection2 : IReportServerConnection, IReportServerCredentials
{
	IEnumerable<Cookie> Cookies { get; }

	IEnumerable<string> Headers { get; }
}
