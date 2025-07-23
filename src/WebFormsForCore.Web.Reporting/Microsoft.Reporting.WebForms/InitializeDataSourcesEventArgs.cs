using System;

namespace Microsoft.Reporting.WebForms;

internal sealed class InitializeDataSourcesEventArgs : EventArgs
{
	private ReportDataSourceCollection m_dataSources;

	public ReportDataSourceCollection DataSources => m_dataSources;

	internal InitializeDataSourcesEventArgs(ReportDataSourceCollection dataSources)
	{
		m_dataSources = dataSources;
	}
}
