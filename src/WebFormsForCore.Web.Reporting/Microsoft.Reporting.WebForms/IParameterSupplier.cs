using System.Collections.Generic;

namespace Microsoft.Reporting.WebForms;

internal interface IParameterSupplier
{
	bool IsReadyForConnection { get; }

	bool IsQueryExecutionAllowed { get; }

	ReportParameterInfoCollection GetParameters();

	void SetParameters(IEnumerable<ReportParameter> parameters);

	ReportDataSourceInfoCollection GetDataSources(out bool allCredentialsSatisfied);

	void SetDataSourceCredentials(DataSourceCredentialsCollection credentials);
}
