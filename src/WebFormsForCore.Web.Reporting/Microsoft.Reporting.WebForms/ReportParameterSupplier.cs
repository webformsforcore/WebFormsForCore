using System.Collections.Generic;

namespace Microsoft.Reporting.WebForms;

internal class ReportParameterSupplier : IParameterSupplier
{
	private Report m_report;

	public bool IsReadyForConnection => m_report.IsReadyForConnection;

	public bool IsQueryExecutionAllowed
	{
		get
		{
			if (m_report is ServerReport serverReport)
			{
				return serverReport.IsQueryExecutionAllowed();
			}
			return true;
		}
	}

	public ReportParameterSupplier(Report report)
	{
		m_report = report;
	}

	public ReportParameterInfoCollection GetParameters()
	{
		return m_report.GetParameters();
	}

	public void SetParameters(IEnumerable<ReportParameter> parameters)
	{
		m_report.SetParameters(parameters);
	}

	public ReportDataSourceInfoCollection GetDataSources(out bool allCredentialsSatisfied)
	{
		if (m_report is ServerReport serverReport)
		{
			return serverReport.GetDataSources(out allCredentialsSatisfied);
		}
		LocalReport localReport = (LocalReport)m_report;
		if (localReport.SupportsQueries)
		{
			return localReport.GetDataSources(out allCredentialsSatisfied);
		}
		allCredentialsSatisfied = true;
		return null;
	}

	public void SetDataSourceCredentials(DataSourceCredentialsCollection credentials)
	{
		if (m_report is ServerReport serverReport)
		{
			serverReport.SetDataSourceCredentials(credentials);
			return;
		}
		LocalReport localReport = (LocalReport)m_report;
		localReport.SetDataSourceCredentials(credentials);
	}
}
