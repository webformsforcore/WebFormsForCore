
using Microsoft.ReportingServices.DataExtensions;
using Microsoft.ReportingServices.DataProcessing;
using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal class DataSetExtensionConnection : IProcessingDataExtensionConnection
  {
    private LocalDataRetrievalFromDataSet.GetSubReportDataSetCallback m_subreportCallback;
    private IEnumerable m_dataSources;

    public DataSetExtensionConnection(
      LocalDataRetrievalFromDataSet.GetSubReportDataSetCallback subreportCallback,
      IEnumerable dataSources)
    {
      this.m_subreportCallback = subreportCallback;
      this.m_dataSources = dataSources;
    }

    public void DataSetRetrieveForReportInstance(
      ICatalogItemContext itemContext,
      ParameterInfoCollection reportParameters)
    {
      this.m_dataSources = (IEnumerable) new DataSourceCollectionWrapper((ReportDataSourceCollection) this.m_subreportCallback((PreviewItemContext) itemContext, reportParameters));
    }

    public void HandleImpersonation(
      IProcessingDataSource dataSource,
      DataSourceInfo dataSourceInfo,
      string datasetName,
      IDbConnection connection,
      Action afterImpersonationAction)
    {
      if (afterImpersonationAction == null)
        return;
      afterImpersonationAction();
    }

    public IDbConnection OpenDataSourceExtensionConnection(
      IProcessingDataSource dataSource,
      string connectionString,
      DataSourceInfo dataSourceInfo,
      string datasetName)
    {
      return (IDbConnection) new DataSetProcessingExtension(this.m_dataSources, datasetName);
    }

    public void CloseConnection(
      IDbConnection connection,
      IProcessingDataSource dataSourceObj,
      DataSourceInfo dataSourceInfo)
    {
      this.CloseConnectionWithoutPool(connection);
    }

    public void CloseConnectionWithoutPool(IDbConnection connection) => connection.Close();

    public bool MustResolveSharedDataSources => false;
  }
}
