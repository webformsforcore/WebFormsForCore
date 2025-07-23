using System;
using System.Collections;
using Microsoft.ReportingServices.DataExtensions;
using Microsoft.ReportingServices.DataProcessing;
using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.Reporting.WebForms;

internal class DataSetExtensionConnection : IProcessingDataExtensionConnection
{
	private LocalDataRetrievalFromDataSet.GetSubReportDataSetCallback m_subreportCallback;

	private IEnumerable m_dataSources;

	public bool MustResolveSharedDataSources => false;

	public DataSetExtensionConnection(LocalDataRetrievalFromDataSet.GetSubReportDataSetCallback subreportCallback, IEnumerable dataSources)
	{
		m_subreportCallback = subreportCallback;
		m_dataSources = dataSources;
	}

	public void DataSetRetrieveForReportInstance(ICatalogItemContext itemContext, ParameterInfoCollection reportParameters)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Expected O, but got Unknown
		IEnumerable enumerable = m_subreportCallback((PreviewItemContext)itemContext, reportParameters);
		m_dataSources = new DataSourceCollectionWrapper((ReportDataSourceCollection)enumerable);
	}

	public void HandleImpersonation(IProcessingDataSource dataSource, DataSourceInfo dataSourceInfo, string datasetName, IDbConnection connection, Action afterImpersonationAction)
	{
		afterImpersonationAction?.Invoke();
	}

	public IDbConnection OpenDataSourceExtensionConnection(IProcessingDataSource dataSource, string connectionString, DataSourceInfo dataSourceInfo, string datasetName)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Expected O, but got Unknown
		return (IDbConnection)new DataSetProcessingExtension(m_dataSources, datasetName);
	}

	public void CloseConnection(IDbConnection connection, IProcessingDataSource dataSourceObj, DataSourceInfo dataSourceInfo)
	{
		CloseConnectionWithoutPool(connection);
	}

	public void CloseConnectionWithoutPool(IDbConnection connection)
	{
		connection.Close();
	}
}
