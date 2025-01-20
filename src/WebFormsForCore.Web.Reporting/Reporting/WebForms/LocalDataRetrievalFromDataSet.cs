
using Microsoft.ReportingServices.DataExtensions;
using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.Interfaces;
using Microsoft.ReportingServices.ReportProcessing;
using System.Collections;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal class LocalDataRetrievalFromDataSet : LocalDataRetrieval
  {
    private LocalDataRetrievalFromDataSet.GetSubReportDataSetCallback m_subreportDataCallback;

    public LocalDataRetrievalFromDataSet.GetSubReportDataSetCallback SubReportDataSetCallback
    {
      set => this.m_subreportDataCallback = value;
    }

    public virtual bool SupportsQueries => false;

    public virtual ProcessingContext CreateProcessingContext(
      PreviewItemContext itemContext,
      ParameterInfoCollection parameters,
      IEnumerable dataSources,
      RuntimeDataSourceInfoCollection dataSourceInfoColl,
      RuntimeDataSetInfoCollection dataSetInfoColl,
      SharedDataSetCompiler sharedDataSetCompiler,
      DatasourceCredentialsCollection credentials,
      Microsoft.ReportingServices.ReportProcessing.ReportProcessing.OnDemandSubReportCallback subReportCallback,
      IGetResource getResourceFunction,
      IChunkFactory chunkFactory,
      ReportRuntimeSetup runtimeSetup,
      CreateAndRegisterStream createStreamCallback)
    {
      return (ProcessingContext) new ProcessingContextForDataSets(itemContext, parameters, (IEnumerable) new DataSourceCollectionWrapper((ReportDataSourceCollection) dataSources), subReportCallback, this.m_subreportDataCallback, getResourceFunction, chunkFactory, runtimeSetup, createStreamCallback);
    }

    public delegate IEnumerable GetSubReportDataSetCallback(
      PreviewItemContext subReportContext,
      ParameterInfoCollection initialParameters);
  }
}
