// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.LocalDataRetrievalFromDataSet
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

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
