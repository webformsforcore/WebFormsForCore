using System;

namespace Microsoft.Reporting.WebForms;

public interface IReportServerConnection : IReportServerCredentials
{
	Uri ReportServerUrl { get; }

	int Timeout { get; }
}
