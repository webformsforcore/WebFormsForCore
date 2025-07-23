using System;
using System.Collections;
using System.Security.Principal;
using System.Threading;
using Microsoft.ReportingServices;
using Microsoft.ReportingServices.DataExtensions;
using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.Interfaces;
using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.Reporting.WebForms;

internal class ProcessingContextForDataSets : ProcessingContext
{
	private IEnumerable m_dataSources;

	private LocalDataRetrievalFromDataSet.GetSubReportDataSetCallback m_subReportInfoCallback;

	internal override bool EnableDataBackedParameters => false;

	internal override IProcessingDataExtensionConnection CreateAndSetupDataExtensionFunction => (IProcessingDataExtensionConnection)(object)new DataSetExtensionConnection(m_subReportInfoCallback, m_dataSources);

	internal override RuntimeDataSourceInfoCollection DataSources => null;

	internal override RuntimeDataSetInfoCollection SharedDataSetReferences => null;

	internal override bool CanShareDataSets => false;

	public ProcessingContextForDataSets(PreviewItemContext reportContext, ParameterInfoCollection parameters, IEnumerable dataSources, OnDemandSubReportCallback subReportCallback, LocalDataRetrievalFromDataSet.GetSubReportDataSetCallback subReportInfoCallback, IGetResource getResourceFunction, IChunkFactory chunkFactory, ReportRuntimeSetup reportRuntimeSetup, CreateAndRegisterStream createStreamCallback)
		: base((ICatalogItemContext)(object)reportContext, WindowsIdentity.GetCurrent().Name, parameters, subReportCallback, getResourceFunction, chunkFactory, (ExecutionType)0, Thread.CurrentThread.CurrentCulture, (UserProfileState)3, (UserProfileState)0, reportRuntimeSetup, createStreamCallback, false, (IJobContext)new ViewerJobContextImpl(), (IExtensionFactory)new ViewerExtensionFactory(), DataProtectionLocal.Instance)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Expected O, but got Unknown
		//IL_0039: Expected O, but got Unknown
		m_dataSources = dataSources;
		m_subReportInfoCallback = subReportInfoCallback;
	}

	internal override ProcessingContext CreateInternalProcessingContext(string chartName, Report report, ErrorContext errorContext, DateTime executionTime, UserProfileState allowUserProfileState, bool isHistorySnapshot, bool snapshotProcessing, bool processWithCachedData, GetReportChunk getChunkCallback, CreateReportChunk cacheDataCallback)
	{
		Global.Tracer.Assert(false, "CreateInternalProcessingContext is not used for ODP Engine Controls");
		return null;
	}

	internal override ProcessingContext ParametersInternalProcessingContext(ErrorContext errorContext, DateTime executionTimeStamp, bool isSnapshot)
	{
		Global.Tracer.Assert(false, "ParametersInternalProcessingContext is not used for ODP Engine Controls");
		return null;
	}
}
