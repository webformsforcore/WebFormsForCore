#define TRACE
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;
using Microsoft.ReportingServices;
using Microsoft.ReportingServices.DataExtensions;
using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.Interfaces;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.Reporting.WebForms;

[Serializable]
public sealed class LocalReport : Report, ISerializable, IDisposable
{
	private const string TopLevelDirectReportDefinitionPath = "";

	private string m_reportPath;

	private string m_reportEmbeddedResource;

	private Assembly m_embeddedResourceAssembly;

	private bool m_enableHyperlinks;

	private bool m_enableExternalImages;

	private NameValueCollection m_parentSuppliedParameters;

	private ReportDataSourceCollection m_dataSources;

	private ProcessingMessageList m_lastRenderingWarnings;

	private readonly ILocalProcessingHost m_processingHost;

	private RenderingExtension[] m_externalRenderingExtensions;

	[NonSerialized]
	private MapTileServerConfiguration m_mapTileServerConfiguration;

	internal override string DisplayNameForUse
	{
		get
		{
			lock (m_syncObject)
			{
				if (string.IsNullOrEmpty(base.DisplayName))
				{
					PreviewItemContext itemContext = m_processingHost.ItemContext;
					if (itemContext != null)
					{
						string text = ((CatalogItemContextBase<string>)(object)itemContext).ItemName;
						if (string.IsNullOrEmpty(text))
						{
							text = CommonStrings.Report;
						}
						return text;
					}
					return string.Empty;
				}
				return base.DisplayName;
			}
		}
	}

	internal bool SupportsQueries => m_processingHost.SupportsQueries;

	internal override bool CanSelfCancel => m_processingHost.CanSelfCancel;

	private DefinitionSource DefinitionSource
	{
		get
		{
			if (string.IsNullOrEmpty(ReportPath))
			{
				if (string.IsNullOrEmpty(ReportEmbeddedResource))
				{
					if (!m_processingHost.Catalog.HasDirectReportDefinition(""))
					{
						return (DefinitionSource)0;
					}
					return (DefinitionSource)3;
				}
				return (DefinitionSource)2;
			}
			return (DefinitionSource)1;
		}
	}

	[NotifyParentProperty(true)]
	[DefaultValue(null)]
	[SRDescription("LocalReportPathDesc")]
	public string ReportPath
	{
		get
		{
			return m_reportPath;
		}
		set
		{
			DemandFullTrustWithFriendlyMessage();
			lock (m_syncObject)
			{
				if (string.Compare(value, ReportPath, StringComparison.Ordinal) != 0)
				{
					ChangeReportDefinition((DefinitionSource)1, delegate
					{
						m_reportPath = value;
					});
				}
			}
		}
	}

	[DefaultValue(null)]
	[SRDescription("ReportEmbeddedResourceDesc")]
	[TypeConverter("Microsoft.ReportingServices.ReportSelectionConverter, Microsoft.Reporting.Design, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91")]
	[NotifyParentProperty(true)]
	public string ReportEmbeddedResource
	{
		get
		{
			return m_reportEmbeddedResource;
		}
		set
		{
			DemandFullTrustWithFriendlyMessage();
			lock (m_syncObject)
			{
				if (string.Compare(value, ReportEmbeddedResource, StringComparison.Ordinal) != 0)
				{
					SetEmbeddedResourceAsReportDefinition(value, Assembly.GetCallingAssembly());
				}
			}
		}
	}

	[NotifyParentProperty(true)]
	[DefaultValue(false)]
	[SRDescription("EnableExternalImagesDesc")]
	[Category("Security")]
	public bool EnableExternalImages
	{
		get
		{
			return m_enableExternalImages;
		}
		set
		{
			lock (m_syncObject)
			{
				m_enableExternalImages = value;
			}
		}
	}

	[DefaultValue(true)]
	[NotifyParentProperty(true)]
	[SRDescription("ShowDetailedSubreportMessagesDesc")]
	public bool ShowDetailedSubreportMessages
	{
		get
		{
			return m_processingHost.ShowDetailedSubreportMessages;
		}
		set
		{
			lock (m_syncObject)
			{
				if (m_processingHost.ShowDetailedSubreportMessages != value)
				{
					m_processingHost.ShowDetailedSubreportMessages = value;
					OnChange(isRefreshOnly: false);
				}
			}
		}
	}

	[NotifyParentProperty(true)]
	[Category("Security")]
	[DefaultValue(false)]
	[SRDescription("EnableHyperlinksDesc")]
	public bool EnableHyperlinks
	{
		get
		{
			return m_enableHyperlinks;
		}
		set
		{
			lock (m_syncObject)
			{
				m_enableHyperlinks = value;
			}
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	[SRDescription("MapTileServerConfigurationDesc")]
	[NotifyParentProperty(true)]
	[PersistenceMode(PersistenceMode.InnerProperty)]
	public MapTileServerConfiguration MapTileServerConfiguration => m_mapTileServerConfiguration;

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	[PersistenceMode(PersistenceMode.InnerProperty)]
	[NotifyParentProperty(true)]
	[SRDescription("ReportDataSourcesDesc")]
	[WebBrowsable(true)]
	public ReportDataSourceCollection DataSources => m_dataSources;

	internal override bool IsReadyForConnection => (int)DefinitionSource != 0;

	internal override bool IsPreparedReportReadyForRendering
	{
		get
		{
			foreach (string dataSourceName in GetDataSourceNames())
			{
				if (DataSources[dataSourceName] == null)
				{
					return false;
				}
			}
			GetDataSources(out var allCredentialsSatisfied);
			if (!allCredentialsSatisfied)
			{
				return false;
			}
			foreach (ReportParameterInfo parameter in GetParameters())
			{
				if (parameter.State != ParameterState.HasValidValue)
				{
					return false;
				}
			}
			return true;
		}
	}

	internal bool HasExecutionSession => m_processingHost.ExecutionInfo.IsCompiled;

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public IList<ReportParameter> OriginalParametersToDrillthrough
	{
		get
		{
			ReportParameter[] list = ReportParameter.FromNameValueCollection(m_parentSuppliedParameters);
			return new ReadOnlyCollection<ReportParameter>(list);
		}
	}

	internal override bool HasDocMap
	{
		get
		{
			try
			{
				lock (m_syncObject)
				{
					return m_processingHost.ExecutionInfo.HasDocMap;
				}
			}
			catch (SecurityException processingException)
			{
				throw new LocalProcessingException(CommonStrings.LocalModeMissingFullTrustErrors, processingException);
			}
		}
	}

	internal override int AutoRefreshInterval
	{
		get
		{
			lock (m_syncObject)
			{
				return m_processingHost.ExecutionInfo.AutoRefreshInterval;
			}
		}
	}

	internal event InitializeDataSourcesEventHandler InitializeDataSources;

	[SRDescription("SubreportProcessingEventDesc")]
	public event SubreportProcessingEventHandler SubreportProcessing;

	private LocalReport(ILocalProcessingHost processingHost)
	{
		m_processingHost = processingHost;
		m_dataSources = new ReportDataSourceCollection(m_syncObject);
		Construct();
	}

	[SecurityCritical]
	[SecurityTreatAsSafe]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	internal LocalReport(SerializationInfo info, StreamingContext context)
	{
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Expected O, but got Unknown
		base.DisplayName = info.GetString("DisplayName");
		m_reportPath = info.GetString("ReportPath");
		m_reportEmbeddedResource = info.GetString("ReportEmbeddedResource");
		m_embeddedResourceAssembly = (Assembly)info.GetValue("EmbeddedResourceAssembly", typeof(Assembly));
		m_dataSources = (ReportDataSourceCollection)info.GetValue("DataSources", typeof(ReportDataSourceCollection));
		m_dataSources.SetSyncObject(m_syncObject);
		m_processingHost = (ILocalProcessingHost)info.GetValue("ControlService", typeof(ILocalProcessingHost));
		base.DrillthroughDepth = info.GetInt32("DrillthroughDepth");
		m_enableExternalImages = info.GetBoolean("EnableExternalImages");
		m_enableHyperlinks = info.GetBoolean("EnableHyperlinks");
		m_parentSuppliedParameters = (NameValueCollection)info.GetValue("ParentSuppliedParameters", typeof(NameValueCollection));
		Construct();
	}

	[SecurityCritical]
	[SecurityTreatAsSafe]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
	{
		info.AddValue("DisplayName", base.DisplayName);
		info.AddValue("ReportPath", m_reportPath);
		info.AddValue("ReportEmbeddedResource", m_reportEmbeddedResource);
		info.AddValue("EmbeddedResourceAssembly", m_embeddedResourceAssembly);
		info.AddValue("DataSources", m_dataSources);
		info.AddValue("ControlService", m_processingHost);
		info.AddValue("DrillthroughDepth", base.DrillthroughDepth);
		info.AddValue("EnableExternalImages", m_enableExternalImages);
		info.AddValue("EnableHyperlinks", m_enableHyperlinks);
		info.AddValue("ParentSuppliedParameters", m_parentSuppliedParameters);
	}

	private void Construct()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Expected O, but got Unknown
		ILocalProcessingHost processingHost = m_processingHost;
		LocalService val = (LocalService)(object)((processingHost is LocalService) ? processingHost : null);
		if (val != null)
		{
			val.DataRetrieval = CreateDataRetrieval();
			val.SecurityValidator = new LocalModeSecurityValidatorCallback(ValidateReportSecurity);
		}
		DataSources.Change += base.OnChange;
		base.Change += OnLocalReportChange;
		if (m_processingHost.MapTileServerConfiguration != null)
		{
			m_mapTileServerConfiguration = new MapTileServerConfiguration(m_processingHost.MapTileServerConfiguration);
		}
	}

	public void Dispose()
	{
		if (m_processingHost is IDisposable disposable)
		{
			disposable.Dispose();
		}
	}

	internal override void SetCancelState(bool shouldCancelRequests)
	{
		m_processingHost.SetCancelState(shouldCancelRequests);
	}

	private void DemandFullTrustWithFriendlyMessage()
	{
		try
		{
			SecurityPermission securityPermission = new SecurityPermission(PermissionState.Unrestricted);
			securityPermission.Demand();
		}
		catch (SecurityException)
		{
			throw new LocalProcessingException(CommonStrings.LocalModeMissingFullTrustErrors);
		}
	}

	private void SetEmbeddedResourceAsReportDefinition(string resourceName, Assembly assemblyWithResource)
	{
		ChangeReportDefinition((DefinitionSource)2, delegate
		{
			if (string.IsNullOrEmpty(resourceName))
			{
				assemblyWithResource = null;
			}
			m_reportEmbeddedResource = resourceName;
			m_embeddedResourceAssembly = assemblyWithResource;
		});
	}

	internal void SetDataSourceCredentials(IEnumerable credentials)
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Expected O, but got Unknown
		if (credentials == null)
		{
			throw new ArgumentNullException("credentials");
		}
		lock (m_syncObject)
		{
			EnsureExecutionSession();
			List<DatasourceCredentials> list = new List<DatasourceCredentials>();
			foreach (DataSourceCredentials credential in credentials)
			{
				list.Add(new DatasourceCredentials(credential.Name, credential.UserId, credential.Password));
			}
			m_processingHost.SetReportDataSourceCredentials(list.ToArray());
			OnChange(isRefreshOnly: false);
		}
	}

	internal ReportDataSourceInfoCollection GetDataSources(out bool allCredentialsSatisfied)
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Expected O, but got Unknown
		lock (m_syncObject)
		{
			EnsureExecutionSession();
			DataSourcePromptCollection reportDataSourcePrompts;
			try
			{
				reportDataSourcePrompts = m_processingHost.GetReportDataSourcePrompts(ref allCredentialsSatisfied);
			}
			catch (Exception processingException)
			{
				throw WrapProcessingException(processingException);
			}
			List<ReportDataSourceInfo> list = new List<ReportDataSourceInfo>(reportDataSourcePrompts.Count);
			foreach (DataSourceInfo item in reportDataSourcePrompts)
			{
				DataSourceInfo val = item;
				list.Add(new ReportDataSourceInfo(val.PromptIdentifier, val.Prompt));
			}
			return new ReportDataSourceInfoCollection(list);
		}
	}

	public IList<string> GetDataSourceNames()
	{
		lock (m_syncObject)
		{
			EnsureExecutionSession();
			string[] dataSetNames;
			try
			{
				dataSetNames = m_processingHost.GetDataSetNames((PreviewItemContext)null);
			}
			catch (Exception processingException)
			{
				throw WrapProcessingException(processingException);
			}
			return new ReadOnlyCollection<string>(dataSetNames);
		}
	}

	public override int GetTotalPages(out PageCountMode pageCountMode)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Invalid comparison between Unknown and I4
		lock (m_syncObject)
		{
			LocalExecutionInfo executionInfo = m_processingHost.ExecutionInfo;
			if ((int)executionInfo.PaginationMode == 1)
			{
				pageCountMode = PageCountMode.Actual;
			}
			else
			{
				pageCountMode = PageCountMode.Estimate;
			}
			return executionInfo.TotalPages;
		}
	}

	public override void LoadReportDefinition(TextReader report)
	{
		lock (m_syncObject)
		{
			if (report == null)
			{
				throw new ArgumentNullException("report");
			}
			SetDirectReportDefinition("", report);
		}
	}

	public void LoadSubreportDefinition(string reportName, TextReader report)
	{
		lock (m_syncObject)
		{
			if (reportName == null)
			{
				throw new ArgumentNullException("reportName");
			}
			if (reportName.Length == 0)
			{
				throw new ArgumentOutOfRangeException("reportName");
			}
			if (report == null)
			{
				throw new ArgumentNullException("report");
			}
			SetDirectReportDefinition(reportName, report);
		}
	}

	public void LoadSubreportDefinition(string reportName, Stream report)
	{
		if (report == null)
		{
			throw new ArgumentNullException("report");
		}
		LoadSubreportDefinition(reportName, new StreamReader(report));
	}

	private void SetDirectReportDefinition(string reportName, TextReader report)
	{
		DemandFullTrustWithFriendlyMessage();
		string s = report.ReadToEnd();
		byte[] reportBytes = Encoding.UTF8.GetBytes(s);
		ChangeReportDefinition((DefinitionSource)3, delegate
		{
			m_processingHost.Catalog.SetReportDefinition(reportName, reportBytes);
		});
	}

	internal override int PerformSearch(string searchText, int startPage, int endPage)
	{
		try
		{
			lock (m_syncObject)
			{
				if (!m_processingHost.ExecutionInfo.HasSnapshot)
				{
					throw new InvalidOperationException(CommonStrings.ReportNotReady);
				}
				if (string.IsNullOrEmpty(searchText))
				{
					return -1;
				}
				try
				{
					return m_processingHost.PerformSearch(startPage, endPage, searchText);
				}
				catch (Exception processingException)
				{
					throw WrapProcessingException(processingException);
				}
			}
		}
		catch (SecurityException processingException2)
		{
			throw new LocalProcessingException(CommonStrings.LocalModeMissingFullTrustErrors, processingException2);
		}
	}

	internal override void PerformToggle(string toggleId)
	{
		try
		{
			lock (m_syncObject)
			{
				if (!m_processingHost.ExecutionInfo.HasSnapshot)
				{
					throw new InvalidOperationException(CommonStrings.ReportNotReady);
				}
				try
				{
					m_processingHost.PerformToggle(toggleId);
				}
				catch (Exception processingException)
				{
					throw WrapProcessingException(processingException);
				}
			}
		}
		catch (SecurityException processingException2)
		{
			throw new LocalProcessingException(CommonStrings.LocalModeMissingFullTrustErrors, processingException2);
		}
	}

	internal override int PerformBookmarkNavigation(string bookmarkId, out string uniqueName)
	{
		try
		{
			lock (m_syncObject)
			{
				if (!m_processingHost.ExecutionInfo.HasSnapshot)
				{
					throw new InvalidOperationException(CommonStrings.ReportNotReady);
				}
				try
				{
					return m_processingHost.PerformBookmarkNavigation(bookmarkId, ref uniqueName);
				}
				catch (Exception processingException)
				{
					throw WrapProcessingException(processingException);
				}
			}
		}
		catch (SecurityException processingException2)
		{
			throw new LocalProcessingException(CommonStrings.LocalModeMissingFullTrustErrors, processingException2);
		}
	}

	internal override int PerformDocumentMapNavigation(string documentMapId)
	{
		try
		{
			lock (m_syncObject)
			{
				if (!m_processingHost.ExecutionInfo.HasSnapshot)
				{
					throw new InvalidOperationException(CommonStrings.ReportNotReady);
				}
				try
				{
					return m_processingHost.PerformDocumentMapNavigation(documentMapId);
				}
				catch (Exception processingException)
				{
					throw WrapProcessingException(processingException);
				}
			}
		}
		catch (SecurityException processingException2)
		{
			throw new LocalProcessingException(CommonStrings.LocalModeMissingFullTrustErrors, processingException2);
		}
	}

	internal override Report PerformDrillthrough(string drillthroughId, out string reportPath)
	{
		try
		{
			lock (m_syncObject)
			{
				if (!m_processingHost.ExecutionInfo.HasSnapshot)
				{
					throw new InvalidOperationException(CommonStrings.ReportNotReady);
				}
				if (drillthroughId == null)
				{
					throw new ArgumentNullException("drillthroughId");
				}
				NameValueCollection drillParams = default(NameValueCollection);
				try
				{
					reportPath = m_processingHost.PerformDrillthrough(drillthroughId, ref drillParams);
				}
				catch (Exception processingException)
				{
					throw WrapProcessingException(processingException);
				}
				LocalReport localReport = CreateNewLocalReport();
				PreviewItemContext val = CreateItemContext();
				string reportPath2 = ((CatalogItemContextBase<string>)(object)val).MapUserProvidedPath(reportPath);
				PopulateDrillthroughReport(reportPath2, drillParams, localReport);
				return localReport;
			}
		}
		catch (SecurityException processingException2)
		{
			throw new LocalProcessingException(CommonStrings.LocalModeMissingFullTrustErrors, processingException2);
		}
	}

	public override ReportPageSettings GetDefaultPageSettings()
	{
		lock (m_syncObject)
		{
			EnsureExecutionSession();
			PageProperties pageProperties = m_processingHost.ExecutionInfo.PageProperties;
			return new ReportPageSettings(pageProperties.PageHeight, pageProperties.PageWidth, pageProperties.LeftMargin, pageProperties.RightMargin, pageProperties.TopMargin, pageProperties.BottomMargin);
		}
	}

	private void PopulateDrillthroughReport(string reportPath, NameValueCollection drillParams, LocalReport drillReport)
	{
		drillReport.CopySecuritySettings(this);
		if (ReportPath != null)
		{
			drillReport.ReportPath = reportPath;
		}
		else if (ReportEmbeddedResource != null)
		{
			drillReport.SetEmbeddedResourceAsReportDefinition(reportPath, m_embeddedResourceAssembly);
		}
		drillReport.DrillthroughDepth = base.DrillthroughDepth + 1;
		drillReport.m_parentSuppliedParameters = drillParams;
	}

	internal override int PerformSort(string sortId, SortOrder sortDirection, bool clearSort, PageCountMode pageCountMode, out string uniqueName)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			lock (m_syncObject)
			{
				if (!m_processingHost.ExecutionInfo.HasSnapshot)
				{
					throw new InvalidOperationException(CommonStrings.ReportNotReady);
				}
				SortOptions val = ((sortDirection != SortOrder.Ascending) ? ((SortOptions)2) : ((SortOptions)1));
				string text = PageCountModeToProcessingPaginationMode(pageCountMode);
				try
				{
					return m_processingHost.PerformSort(text, sortId, val, clearSort, ref uniqueName);
				}
				catch (Exception processingException)
				{
					throw WrapProcessingException(processingException);
				}
			}
		}
		catch (SecurityException processingException2)
		{
			throw new LocalProcessingException(CommonStrings.LocalModeMissingFullTrustErrors, processingException2);
		}
	}

	[Obsolete("This method requires Code Access Security policy, which is deprecated.  For more information please go to http://go.microsoft.com/fwlink/?LinkId=160787.")]
	public void ExecuteReportInCurrentAppDomain(Evidence reportEvidence)
	{
		try
		{
			lock (m_syncObject)
			{
				m_processingHost.ExecuteReportInCurrentAppDomain(reportEvidence);
			}
		}
		catch (SecurityException processingException)
		{
			throw new LocalProcessingException(CommonStrings.LocalModeMissingFullTrustErrors, processingException);
		}
	}

	[Obsolete("This method requires Code Access Security policy, which is deprecated.  For more information please go to http://go.microsoft.com/fwlink/?LinkId=160787.")]
	public void AddTrustedCodeModuleInCurrentAppDomain(string assemblyName)
	{
		try
		{
			lock (m_syncObject)
			{
				m_processingHost.AddTrustedCodeModuleInCurrentAppDomain(assemblyName);
			}
		}
		catch (SecurityException processingException)
		{
			throw new LocalProcessingException(CommonStrings.LocalModeMissingFullTrustErrors, processingException);
		}
	}

	[Obsolete("This method requires Code Access Security policy, which is deprecated.  For more information please go to http://go.microsoft.com/fwlink/?LinkId=160787.")]
	public void ExecuteReportInSandboxAppDomain()
	{
		try
		{
			lock (m_syncObject)
			{
				m_processingHost.ExecuteReportInSandboxAppDomain();
			}
		}
		catch (SecurityException processingException)
		{
			throw new LocalProcessingException(CommonStrings.LocalModeMissingFullTrustErrors, processingException);
		}
	}

	public void AddFullTrustModuleInSandboxAppDomain(StrongName assemblyName)
	{
		try
		{
			lock (m_syncObject)
			{
				m_processingHost.AddFullTrustModuleInSandboxAppDomain(assemblyName);
			}
		}
		catch (SecurityException processingException)
		{
			throw new LocalProcessingException(CommonStrings.LocalModeMissingFullTrustErrors, processingException);
		}
	}

	public void SetBasePermissionsForSandboxAppDomain(PermissionSet permissions)
	{
		try
		{
			lock (m_syncObject)
			{
				m_processingHost.SetBasePermissionsForSandboxAppDomain(permissions);
			}
		}
		catch (SecurityException processingException)
		{
			throw new LocalProcessingException(CommonStrings.LocalModeMissingFullTrustErrors, processingException);
		}
	}

	public void ReleaseSandboxAppDomain()
	{
		lock (m_syncObject)
		{
			m_processingHost.ReleaseSandboxAppDomain();
		}
	}

	private void CopySecuritySettings(LocalReport parentReport)
	{
		m_processingHost.CopySecuritySettingsFrom(parentReport.m_processingHost);
		m_enableExternalImages = parentReport.EnableExternalImages;
		m_enableHyperlinks = parentReport.EnableHyperlinks;
		ShowDetailedSubreportMessages = parentReport.ShowDetailedSubreportMessages;
	}

	internal override void EnsureExecutionSession()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if ((int)DefinitionSource == 0)
		{
			throw new MissingReportSourceException();
		}
		try
		{
			if (!HasExecutionSession)
			{
				m_processingHost.CompileReport();
				m_processingHost.SetReportParameters(m_parentSuppliedParameters);
			}
		}
		catch (SecurityException processingException)
		{
			throw new LocalProcessingException(CommonStrings.LocalModeMissingFullTrustErrors, processingException);
		}
		catch (Exception processingException2)
		{
			throw WrapProcessingException(processingException2);
		}
	}

	private void ValidateReportSecurity(PreviewItemContext itemContext, PublishingResult publishingResult)
	{
		if (publishingResult.HasExternalImages && !EnableExternalImages)
		{
			throw new ReportSecurityException(CommonStrings.ExternalImagesError(((CatalogItemContextBase<string>)(object)itemContext).ItemName));
		}
		if (publishingResult.HasHyperlinks && !EnableHyperlinks)
		{
			throw new ReportSecurityException(CommonStrings.HyperlinkSecurityError(((CatalogItemContextBase<string>)(object)itemContext).ItemName));
		}
	}

	public override void Refresh()
	{
		try
		{
			lock (m_syncObject)
			{
				if (m_processingHost.ExecutionInfo.HasSnapshot)
				{
					m_processingHost.ResetExecution();
					OnChange(isRefreshOnly: true);
				}
			}
		}
		catch (SecurityException processingException)
		{
			throw new LocalProcessingException(CommonStrings.LocalModeMissingFullTrustErrors, processingException);
		}
	}

	private void ChangeReportDefinition(DefinitionSource updatingSourceType, Action changeAction)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		DefinitionSource definitionSource = DefinitionSource;
		changeAction();
		DefinitionSource definitionSource2 = DefinitionSource;
		if (definitionSource2 == updatingSourceType || definitionSource2 != definitionSource)
		{
			m_processingHost.ItemContext = CreateItemContext();
			OnChange(isRefreshOnly: false);
		}
	}

	public override ReportParameterInfoCollection GetParameters()
	{
		lock (m_syncObject)
		{
			EnsureExecutionSession();
			return ParameterInfoCollectionToApi(m_processingHost.ExecutionInfo.ReportParameters, SupportsQueries);
		}
	}

	public override void SetParameters(IEnumerable<ReportParameter> parameters)
	{
		if (parameters == null)
		{
			throw new ArgumentNullException("parameters");
		}
		lock (m_syncObject)
		{
			EnsureExecutionSession();
			NameValueCollection reportParameters = ReportParameter.ToNameValueCollection(parameters);
			try
			{
				m_processingHost.SetReportParameters(reportParameters);
			}
			catch (Exception processingException)
			{
				throw WrapProcessingException(processingException);
			}
			OnChange(isRefreshOnly: false);
		}
	}

	internal override byte[] InternalRenderStream(string format, string streamID, string deviceInfo, out string mimeType, out string encoding)
	{
		try
		{
			encoding = null;
			lock (m_syncObject)
			{
				return m_processingHost.RenderStream(format, deviceInfo, streamID, ref mimeType);
			}
		}
		catch (SecurityException processingException)
		{
			throw new LocalProcessingException(CommonStrings.LocalModeMissingFullTrustErrors, processingException);
		}
		catch (Exception processingException2)
		{
			throw WrapProcessingException(processingException2);
		}
	}

	internal override DocumentMapNode GetDocumentMap(string rootLabel)
	{
		try
		{
			lock (m_syncObject)
			{
				if (!m_processingHost.ExecutionInfo.HasSnapshot)
				{
					throw new InvalidOperationException(CommonStrings.ReportNotReady);
				}
				IDocumentMap documentMap;
				try
				{
					documentMap = m_processingHost.GetDocumentMap();
				}
				catch (Exception processingException)
				{
					throw WrapProcessingException(processingException);
				}
				return DocumentMapNode.CreateTree(documentMap, rootLabel);
			}
		}
		catch (SecurityException processingException2)
		{
			throw new LocalProcessingException(CommonStrings.LocalModeMissingFullTrustErrors, processingException2);
		}
	}

	public override byte[] Render(string format, string deviceInfo, PageCountMode pageCountMode, out string mimeType, out string encoding, out string fileNameExtension, out string[] streams, out Warning[] warnings)
	{
		return InternalRender(format, allowInternalRenderers: false, deviceInfo, pageCountMode, out mimeType, out encoding, out fileNameExtension, out streams, out warnings);
	}

	internal byte[] InternalRender(string format, bool allowInternalRenderers, string deviceInfo, PageCountMode pageCountMode, out string mimeType, out string encoding, out string fileNameExtension, out string[] streams, out Warning[] warnings)
	{
		lock (m_syncObject)
		{
			using StreamCache streamCache = new StreamCache();
			InternalRender(format, allowInternalRenderers, deviceInfo, pageCountMode, streamCache.StreamCallback, out warnings);
			streams = new string[0];
			return streamCache.GetMainStream(out encoding, out mimeType, out fileNameExtension);
		}
	}

	public void Render(string format, string deviceInfo, CreateStreamCallback createStream, out Warning[] warnings)
	{
		Render(format, deviceInfo, PageCountMode.Estimate, createStream, out warnings);
	}

	public void Render(string format, string deviceInfo, PageCountMode pageCountMode, CreateStreamCallback createStream, out Warning[] warnings)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Expected O, but got Unknown
		if (createStream == null)
		{
			throw new ArgumentNullException("createStream");
		}
		CreateAndRegisterStream createAndRegisterStream = (string name, string extension, Encoding encoding, string mimeType, bool willSeek, StreamOper operation) => createStream(name, extension, encoding, mimeType, willSeek);
		ProcessingStreamHandler val = new ProcessingStreamHandler(createAndRegisterStream);
		try
		{
			InternalRender(format, allowInternalRenderers: false, deviceInfo, pageCountMode, ((CreateAndRegisterStream)val.StreamCallback).ToInnerType(), out warnings);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	internal void InternalRender(string format, bool allowInternalRenderers, string deviceInfo, PageCountMode pageCountMode, CreateAndRegisterStream createStreamCallback, out Warning[] warnings)
	{
		lock (m_syncObject)
		{
			if (createStreamCallback == null)
			{
				throw new ArgumentNullException("createStreamCallback");
			}
			if (!ValidateRenderingFormat(format))
			{
				throw new ArgumentOutOfRangeException("format");
			}
			EnsureExecutionSession();
			try
			{
				m_lastRenderingWarnings = m_processingHost.Render(format, deviceInfo, PageCountModeToProcessingPaginationMode(pageCountMode), allowInternalRenderers, (IEnumerable)m_dataSources, createStreamCallback.ToOuterType());
			}
			catch (Exception processingException)
			{
				throw WrapProcessingException(processingException);
			}
			warnings = Warning.FromProcessingMessageList(m_lastRenderingWarnings);
			WriteDebugResults(warnings);
		}
	}

	private void WriteDebugResults(Warning[] warnings)
	{
		foreach (Warning warning in warnings)
		{
			string message = string.Format(CultureInfo.InvariantCulture, "{0}: {1} ({2})", warning.Severity, warning.Message, warning.Code);
			Trace.WriteLine(message);
		}
	}

	private bool ValidateRenderingFormat(string format)
	{
		try
		{
			foreach (LocalRenderingExtensionInfo item in m_processingHost.ListRenderingExtensions())
			{
				if (string.Compare(item.Name, format, StringComparison.OrdinalIgnoreCase) == 0)
				{
					return true;
				}
			}
			return false;
		}
		catch (Exception processingException)
		{
			throw WrapProcessingException(processingException);
		}
	}

	internal void TransferEvents(LocalReport targetReport)
	{
		if (targetReport != null)
		{
			targetReport.SubreportProcessing = this.SubreportProcessing;
		}
		this.SubreportProcessing = null;
	}

	internal void CreateSnapshot()
	{
		m_processingHost.Render((string)null, (string)null, (string)null, false, (IEnumerable)m_dataSources, (CreateAndRegisterStream)null);
	}

	private IEnumerable ControlSubReportInfoCallback(PreviewItemContext subReportContext, ParameterInfoCollection initialParameters)
	{
		if (this.SubreportProcessing == null)
		{
			return null;
		}
		string[] dataSetNames;
		try
		{
			dataSetNames = m_processingHost.GetDataSetNames(subReportContext);
		}
		catch (Exception processingException)
		{
			throw WrapProcessingException(processingException);
		}
		SubreportProcessingEventArgs e = new SubreportProcessingEventArgs(((CatalogItemContextBase<string>)(object)subReportContext).OriginalItemPath, ParameterInfoCollectionToApi(initialParameters, SupportsQueries), dataSetNames);
		this.SubreportProcessing(this, e);
		if (this.InitializeDataSources != null)
		{
			InitializeDataSourcesEventArgs e2 = new InitializeDataSourcesEventArgs(e.DataSources);
			this.InitializeDataSources(this, e2);
		}
		return e.DataSources;
	}

	public override RenderingExtension[] ListRenderingExtensions()
	{
		if (m_externalRenderingExtensions == null)
		{
			List<RenderingExtension> list = new List<RenderingExtension>();
			try
			{
				foreach (LocalRenderingExtensionInfo item in m_processingHost.ListRenderingExtensions())
				{
					if (item.IsExposedExternally)
					{
						list.Add(new RenderingExtension(item.Name, item.LocalizedName, item.IsVisible));
					}
				}
			}
			catch (Exception processingException)
			{
				throw WrapProcessingException(processingException);
			}
			m_externalRenderingExtensions = list.ToArray();
		}
		return m_externalRenderingExtensions;
	}

	private string GetFullyQualifiedReportPath()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Expected I4, but got Unknown
		DefinitionSource definitionSource = DefinitionSource;
		return (definitionSource - 1) switch
		{
			0 => GetReportNameForFile(ReportPath), 
			1 => ReportEmbeddedResource, 
			2 => "", 
			_ => string.Empty, 
		};
	}

	private static string GetReportNameForFile(string path)
	{
		if (Path.IsPathRooted(path))
		{
			return path;
		}
		string path2;
		if (HttpContext.Current != null && HttpContext.Current.Request != null)
		{
			HttpRequest request = HttpContext.Current.Request;
			path2 = request.MapPath(request.ApplicationPath);
		}
		else
		{
			path2 = Environment.CurrentDirectory;
		}
		return Path.Combine(path2, path);
	}

	private PreviewItemContext CreateItemContext()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		return CreateItemContext(ReportPath, GetFullyQualifiedReportPath(), DefinitionSource, m_embeddedResourceAssembly);
	}

	internal static PreviewItemContext CreateItemContextForFilePath(string filePath)
	{
		return CreateItemContext(filePath, GetReportNameForFile(filePath), (DefinitionSource)1, null);
	}

	private static PreviewItemContext CreateItemContext(string pathForFileDefinitionSource, string fullyQualifiedPath, DefinitionSource definitionSource, Assembly embeddedResourceAssembly)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if ((int)definitionSource == 0)
		{
			return null;
		}
		PreviewItemContext val = InstantiatePreviewItemContext();
		val.SetPath(pathForFileDefinitionSource, fullyQualifiedPath, definitionSource, embeddedResourceAssembly);
		return val;
	}

	private LocalProcessingException WrapProcessingException(Exception processingException)
	{
		Exception ex = processingException;
		while (ex != null && ex.InnerException != null && (ex is ReportRenderingException || ex is UnhandledReportRenderingException || ex is HandledReportRenderingException))
		{
			ex = ex.InnerException;
		}
		if (ex is LocalProcessingException result)
		{
			return result;
		}
		return new LocalProcessingException(ex);
	}

	private static string PageCountModeToProcessingPaginationMode(PageCountMode pageCountMode)
	{
		if (pageCountMode == PageCountMode.Actual)
		{
			return "Actual";
		}
		return "Estimate";
	}

	private static ReportParameterInfoCollection ParameterInfoCollectionToApi(ParameterInfoCollection processingMetadata, bool supportsQueries)
	{
		if (processingMetadata == null)
		{
			return new ReportParameterInfoCollection();
		}
		ReportParameterInfo[] array = new ReportParameterInfo[((ArrayList)(object)processingMetadata).Count];
		for (int i = 0; i < ((ArrayList)(object)processingMetadata).Count; i++)
		{
			array[i] = ParameterInfoToApi(processingMetadata[i], supportsQueries);
		}
		return new ReportParameterInfoCollection(array);
	}

	private static ReportParameterInfo ParameterInfoToApi(ParameterInfo paramInfo, bool supportsQueries)
	{
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Expected I4, but got Unknown
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Expected O, but got Unknown
		string[] array = null;
		if (paramInfo.DependencyList != null)
		{
			array = new string[((ArrayList)(object)paramInfo.DependencyList).Count];
			for (int i = 0; i < ((ArrayList)(object)paramInfo.DependencyList).Count; i++)
			{
				array[i] = ((ParameterBase)paramInfo.DependencyList[i]).Name;
			}
		}
		string[] array2 = null;
		if (paramInfo.Values != null)
		{
			array2 = new string[paramInfo.Values.Length];
			for (int j = 0; j < paramInfo.Values.Length; j++)
			{
				array2[j] = paramInfo.CastToString(paramInfo.Values[j], CultureInfo.CurrentCulture);
			}
		}
		List<ValidValue> list = null;
		if (paramInfo.ValidValues != null)
		{
			list = new List<ValidValue>(((ArrayList)(object)paramInfo.ValidValues).Count);
			foreach (ValidValue item in (ArrayList)(object)paramInfo.ValidValues)
			{
				ValidValue val = item;
				string value = paramInfo.CastToString(val.Value, CultureInfo.CurrentCulture);
				list.Add(new ValidValue(val.Label, value));
			}
		}
		ReportParameterState state = paramInfo.State;
		ParameterState state2;
		switch ((int)state)
		{
		case 0:
			state2 = ParameterState.HasValidValue;
			break;
		case 1:
		case 2:
		case 3:
			state2 = ParameterState.MissingValidValue;
			break;
		case 4:
			state2 = ParameterState.HasOutstandingDependencies;
			break;
		case 5:
			state2 = ParameterState.DynamicValuesUnavailable;
			break;
		default:
			state2 = ParameterState.MissingValidValue;
			break;
		}
		return new ReportParameterInfo(((ParameterBase)paramInfo).Name, (ParameterDataType)Enum.Parse(typeof(ParameterDataType), ((object)((ParameterBase)paramInfo).DataType).ToString()), ((ParameterBase)paramInfo).Nullable, ((ParameterBase)paramInfo).AllowBlank, ((ParameterBase)paramInfo).MultiValue, supportsQueries && ((ParameterBase)paramInfo).UsedInQuery, ((ParameterBase)paramInfo).Prompt, ((ParameterBase)paramInfo).PromptUser, supportsQueries && paramInfo.DynamicDefaultValue, supportsQueries && paramInfo.DynamicValidValues, null, array2, list, array, state2);
	}

	private void OnLocalReportChange(object sender, EventArgs e)
	{
		m_processingHost.ResetExecution();
	}

	public LocalReport()
		: this((ILocalProcessingHost)new ControlService((ILocalCatalog)new StandalonePreviewStore()))
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)
	//IL_000b: Expected O, but got Unknown
	//IL_0006: Unknown result type (might be due to invalid IL or missing references)
	//IL_0010: Expected O, but got Unknown


	private LocalReport CreateNewLocalReport()
	{
		return new LocalReport();
	}

	private LocalDataRetrieval CreateDataRetrieval()
	{
		LocalDataRetrievalFromDataSet localDataRetrievalFromDataSet = new LocalDataRetrievalFromDataSet();
		localDataRetrievalFromDataSet.SubReportDataSetCallback = ControlSubReportInfoCallback;
		return (LocalDataRetrieval)(object)localDataRetrievalFromDataSet;
	}

	private static PreviewItemContext InstantiatePreviewItemContext()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		return new PreviewItemContext();
	}
}
