// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.ProcessingContextForDataSets
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

using Microsoft.ReportingServices;
using Microsoft.ReportingServices.DataExtensions;
using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.Interfaces;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections;
using System.Security.Principal;
using System.Threading;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal class ProcessingContextForDataSets : ProcessingContext
  {
    private IEnumerable m_dataSources;
    private LocalDataRetrievalFromDataSet.GetSubReportDataSetCallback m_subReportInfoCallback;

    public ProcessingContextForDataSets(
      PreviewItemContext reportContext,
      ParameterInfoCollection parameters,
      IEnumerable dataSources,
      Microsoft.ReportingServices.ReportProcessing.ReportProcessing.OnDemandSubReportCallback subReportCallback,
      LocalDataRetrievalFromDataSet.GetSubReportDataSetCallback subReportInfoCallback,
      IGetResource getResourceFunction,
      IChunkFactory chunkFactory,
      ReportRuntimeSetup reportRuntimeSetup,
      CreateAndRegisterStream createStreamCallback)
      : base((ICatalogItemContext) reportContext, WindowsIdentity.GetCurrent().Name, parameters, subReportCallback, getResourceFunction, chunkFactory, (Microsoft.ReportingServices.ReportProcessing.ReportProcessing.ExecutionType) 0, Thread.CurrentThread.CurrentCulture, (UserProfileState) 3, (UserProfileState) 0, reportRuntimeSetup, createStreamCallback, false, (IJobContext) new ViewerJobContextImpl(), (IExtensionFactory) new ViewerExtensionFactory(), DataProtectionLocal.Instance)
    {
      this.m_dataSources = dataSources;
      this.m_subReportInfoCallback = subReportInfoCallback;
    }

    internal virtual bool EnableDataBackedParameters => false;

    internal virtual IProcessingDataExtensionConnection CreateAndSetupDataExtensionFunction
    {
      get
      {
        return (IProcessingDataExtensionConnection) new DataSetExtensionConnection(this.m_subReportInfoCallback, this.m_dataSources);
      }
    }

    internal virtual RuntimeDataSourceInfoCollection DataSources
    {
      get => (RuntimeDataSourceInfoCollection) null;
    }

    internal virtual RuntimeDataSetInfoCollection SharedDataSetReferences
    {
      get => (RuntimeDataSetInfoCollection) null;
    }

    internal virtual bool CanShareDataSets => false;

    internal virtual Microsoft.ReportingServices.ReportProcessing.ReportProcessing.ProcessingContext CreateInternalProcessingContext(
      string chartName,
      Report report,
      ErrorContext errorContext,
      DateTime executionTime,
      UserProfileState allowUserProfileState,
      bool isHistorySnapshot,
      bool snapshotProcessing,
      bool processWithCachedData,
      Microsoft.ReportingServices.ReportProcessing.ReportProcessing.GetReportChunk getChunkCallback,
      Microsoft.ReportingServices.ReportProcessing.ReportProcessing.CreateReportChunk cacheDataCallback)
    {
      Global.Tracer.Assert(false, "CreateInternalProcessingContext is not used for ODP Engine Controls");
      return (Microsoft.ReportingServices.ReportProcessing.ReportProcessing.ProcessingContext) null;
    }

    internal virtual Microsoft.ReportingServices.ReportProcessing.ReportProcessing.ProcessingContext ParametersInternalProcessingContext(
      ErrorContext errorContext,
      DateTime executionTimeStamp,
      bool isSnapshot)
    {
      Global.Tracer.Assert(false, "ParametersInternalProcessingContext is not used for ODP Engine Controls");
      return (Microsoft.ReportingServices.ReportProcessing.ReportProcessing.ProcessingContext) null;
    }
  }
}
