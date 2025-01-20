using Microsoft.ReportingServices;
using Microsoft.ReportingServices.DataExtensions;
using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.Interfaces;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.ReportProcessing;
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

#nullable disable
namespace Microsoft.Reporting.WebForms
{
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

    private LocalReport(ILocalProcessingHost processingHost)
    {
      this.m_processingHost = processingHost;
      this.m_dataSources = new ReportDataSourceCollection(this.m_syncObject);
      this.Construct();
    }

    [SecurityCritical]
    [SecurityTreatAsSafe]
    [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
    internal LocalReport(SerializationInfo info, StreamingContext context)
    {
      this.DisplayName = info.GetString("DisplayName");
      this.m_reportPath = info.GetString(nameof (ReportPath));
      this.m_reportEmbeddedResource = info.GetString(nameof (ReportEmbeddedResource));
      this.m_embeddedResourceAssembly = (Assembly) info.GetValue("EmbeddedResourceAssembly", typeof (Assembly));
      this.m_dataSources = (ReportDataSourceCollection) info.GetValue(nameof (DataSources), typeof (ReportDataSourceCollection));
      this.m_dataSources.SetSyncObject(this.m_syncObject);
      this.m_processingHost = (ILocalProcessingHost) info.GetValue("ControlService", typeof (ILocalProcessingHost));
      this.DrillthroughDepth = info.GetInt32("DrillthroughDepth");
      this.m_enableExternalImages = info.GetBoolean(nameof (EnableExternalImages));
      this.m_enableHyperlinks = info.GetBoolean(nameof (EnableHyperlinks));
      this.m_parentSuppliedParameters = (NameValueCollection) info.GetValue("ParentSuppliedParameters", typeof (NameValueCollection));
      this.Construct();
    }

    [SecurityCritical]
    [SecurityTreatAsSafe]
    [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
    void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
    {
      info.AddValue("DisplayName", (object) this.DisplayName);
      info.AddValue("ReportPath", (object) this.m_reportPath);
      info.AddValue("ReportEmbeddedResource", (object) this.m_reportEmbeddedResource);
      info.AddValue("EmbeddedResourceAssembly", (object) this.m_embeddedResourceAssembly);
      info.AddValue("DataSources", (object) this.m_dataSources);
      info.AddValue("ControlService", (object) this.m_processingHost);
      info.AddValue("DrillthroughDepth", this.DrillthroughDepth);
      info.AddValue("EnableExternalImages", this.m_enableExternalImages);
      info.AddValue("EnableHyperlinks", this.m_enableHyperlinks);
      info.AddValue("ParentSuppliedParameters", (object) this.m_parentSuppliedParameters);
    }

    private void Construct()
    {
      if (this.m_processingHost is LocalService processingHost)
      {
        processingHost.DataRetrieval = this.CreateDataRetrieval();
        processingHost.SecurityValidator = new LocalService.LocalModeSecurityValidatorCallback(this.ValidateReportSecurity);
      }
      this.DataSources.Change += new EventHandler(((Report) this).OnChange);
      this.Change += new EventHandler<ReportChangedEventArgs>(this.OnLocalReportChange);
      if (this.m_processingHost.MapTileServerConfiguration == null)
        return;
      this.m_mapTileServerConfiguration = new MapTileServerConfiguration(this.m_processingHost.MapTileServerConfiguration);
    }

    public void Dispose()
    {
      if (!(this.m_processingHost is IDisposable processingHost))
        return;
      processingHost.Dispose();
    }

    internal override string DisplayNameForUse
    {
      get
      {
        lock (this.m_syncObject)
        {
          if (!string.IsNullOrEmpty(this.DisplayName))
            return this.DisplayName;
          PreviewItemContext itemContext = this.m_processingHost.ItemContext;
          if (itemContext == null)
            return string.Empty;
          string displayNameForUse = ((CatalogItemContextBase<string>) itemContext).ItemName;
          if (string.IsNullOrEmpty(displayNameForUse))
            displayNameForUse = CommonStrings.Report;
          return displayNameForUse;
        }
      }
    }

    internal bool SupportsQueries => this.m_processingHost.SupportsQueries;

    internal override bool CanSelfCancel => this.m_processingHost.CanSelfCancel;

    internal override void SetCancelState(bool shouldCancelRequests)
    {
      this.m_processingHost.SetCancelState(shouldCancelRequests);
    }

    private DefinitionSource DefinitionSource
    {
      get
      {
        if (!string.IsNullOrEmpty(this.ReportPath))
          return (DefinitionSource) 1;
        if (!string.IsNullOrEmpty(this.ReportEmbeddedResource))
          return (DefinitionSource) 2;
        return this.m_processingHost.Catalog.HasDirectReportDefinition("") ? (DefinitionSource) 3 : (DefinitionSource) 0;
      }
    }

    private void DemandFullTrustWithFriendlyMessage()
    {
      try
      {
        new SecurityPermission(PermissionState.Unrestricted).Demand();
      }
      catch (SecurityException ex)
      {
        throw new LocalProcessingException(CommonStrings.LocalModeMissingFullTrustErrors);
      }
    }

    [NotifyParentProperty(true)]
    [DefaultValue(null)]
    [Microsoft.Reporting.SRDescription("LocalReportPathDesc")]
    public string ReportPath
    {
      get => this.m_reportPath;
      set
      {
        this.DemandFullTrustWithFriendlyMessage();
        lock (this.m_syncObject)
        {
          if (string.Compare(value, this.ReportPath, StringComparison.Ordinal) == 0)
            return;
          this.ChangeReportDefinition((DefinitionSource) 1, (Action) (() => this.m_reportPath = value));
        }
      }
    }

    [DefaultValue(null)]
    [Microsoft.Reporting.SRDescription("ReportEmbeddedResourceDesc")]
    [TypeConverter("Microsoft.ReportingServices.ReportSelectionConverter, Microsoft.Reporting.Design, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91")]
    [NotifyParentProperty(true)]
    public string ReportEmbeddedResource
    {
      get => this.m_reportEmbeddedResource;
      set
      {
        this.DemandFullTrustWithFriendlyMessage();
        lock (this.m_syncObject)
        {
          if (string.Compare(value, this.ReportEmbeddedResource, StringComparison.Ordinal) == 0)
            return;
          this.SetEmbeddedResourceAsReportDefinition(value, Assembly.GetCallingAssembly());
        }
      }
    }

    [NotifyParentProperty(true)]
    [DefaultValue(false)]
    [Microsoft.Reporting.SRDescription("EnableExternalImagesDesc")]
    [Category("Security")]
    public bool EnableExternalImages
    {
      get => this.m_enableExternalImages;
      set
      {
        lock (this.m_syncObject)
          this.m_enableExternalImages = value;
      }
    }

    [DefaultValue(true)]
    [NotifyParentProperty(true)]
    [Microsoft.Reporting.SRDescription("ShowDetailedSubreportMessagesDesc")]
    public bool ShowDetailedSubreportMessages
    {
      get => this.m_processingHost.ShowDetailedSubreportMessages;
      set
      {
        lock (this.m_syncObject)
        {
          if (this.m_processingHost.ShowDetailedSubreportMessages == value)
            return;
          this.m_processingHost.ShowDetailedSubreportMessages = value;
          this.OnChange(false);
        }
      }
    }

    [NotifyParentProperty(true)]
    [Category("Security")]
    [DefaultValue(false)]
    [Microsoft.Reporting.SRDescription("EnableHyperlinksDesc")]
    public bool EnableHyperlinks
    {
      get => this.m_enableHyperlinks;
      set
      {
        lock (this.m_syncObject)
          this.m_enableHyperlinks = value;
      }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    [Microsoft.Reporting.SRDescription("MapTileServerConfigurationDesc")]
    [NotifyParentProperty(true)]
    [PersistenceMode(PersistenceMode.InnerProperty)]
    public MapTileServerConfiguration MapTileServerConfiguration
    {
      get => this.m_mapTileServerConfiguration;
    }

    private void SetEmbeddedResourceAsReportDefinition(
      string resourceName,
      Assembly assemblyWithResource)
    {
      this.ChangeReportDefinition((DefinitionSource) 2, (Action) (() =>
      {
        if (string.IsNullOrEmpty(resourceName))
          assemblyWithResource = (Assembly) null;
        this.m_reportEmbeddedResource = resourceName;
        this.m_embeddedResourceAssembly = assemblyWithResource;
      }));
    }

    internal void SetDataSourceCredentials(IEnumerable credentials)
    {
      if (credentials == null)
        throw new ArgumentNullException(nameof (credentials));
      lock (this.m_syncObject)
      {
        this.EnsureExecutionSession();
        List<DatasourceCredentials> datasourceCredentialsList = new List<DatasourceCredentials>();
        foreach (DataSourceCredentials credential in credentials)
          datasourceCredentialsList.Add(new DatasourceCredentials(credential.Name, credential.UserId, credential.Password));
        this.m_processingHost.SetReportDataSourceCredentials(datasourceCredentialsList.ToArray());
        this.OnChange(false);
      }
    }

    internal ReportDataSourceInfoCollection GetDataSources(out bool allCredentialsSatisfied)
    {
      lock (this.m_syncObject)
      {
        this.EnsureExecutionSession();
        DataSourcePromptCollection dataSourcePrompts;
        try
        {
          dataSourcePrompts = this.m_processingHost.GetReportDataSourcePrompts(ref allCredentialsSatisfied);
        }
        catch (Exception ex)
        {
          throw this.WrapProcessingException(ex);
        }
        List<ReportDataSourceInfo> dsInfos = new List<ReportDataSourceInfo>(dataSourcePrompts.Count);
        foreach (DataSourceInfo dataSourceInfo in dataSourcePrompts)
          dsInfos.Add(new ReportDataSourceInfo(dataSourceInfo.PromptIdentifier, dataSourceInfo.Prompt));
        return new ReportDataSourceInfoCollection((IList<ReportDataSourceInfo>) dsInfos);
      }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    [PersistenceMode(PersistenceMode.InnerProperty)]
    [NotifyParentProperty(true)]
    [Microsoft.Reporting.SRDescription("ReportDataSourcesDesc")]
    [WebBrowsable(true)]
    public ReportDataSourceCollection DataSources => this.m_dataSources;

    public IList<string> GetDataSourceNames()
    {
      lock (this.m_syncObject)
      {
        this.EnsureExecutionSession();
        string[] dataSetNames;
        try
        {
          dataSetNames = this.m_processingHost.GetDataSetNames((PreviewItemContext) null);
        }
        catch (Exception ex)
        {
          throw this.WrapProcessingException(ex);
        }
        return (IList<string>) new ReadOnlyCollection<string>((IList<string>) dataSetNames);
      }
    }

    public override int GetTotalPages(out PageCountMode pageCountMode)
    {
      lock (this.m_syncObject)
      {
        LocalExecutionInfo executionInfo = this.m_processingHost.ExecutionInfo;
        pageCountMode = executionInfo.PaginationMode != 1 ? PageCountMode.Estimate : PageCountMode.Actual;
        return executionInfo.TotalPages;
      }
    }

    internal override bool IsReadyForConnection => this.DefinitionSource != 0;

    internal override bool IsPreparedReportReadyForRendering
    {
      get
      {
        foreach (string dataSourceName in (IEnumerable<string>) this.GetDataSourceNames())
        {
          if (this.DataSources[dataSourceName] == null)
            return false;
        }
        bool allCredentialsSatisfied;
        this.GetDataSources(out allCredentialsSatisfied);
        if (!allCredentialsSatisfied)
          return false;
        foreach (ReportParameterInfo parameter in (ReadOnlyCollection<ReportParameterInfo>) this.GetParameters())
        {
          if (parameter.State != ParameterState.HasValidValue)
            return false;
        }
        return true;
      }
    }

    public override void LoadReportDefinition(TextReader report)
    {
      lock (this.m_syncObject)
      {
        if (report == null)
          throw new ArgumentNullException(nameof (report));
        this.SetDirectReportDefinition("", report);
      }
    }

    public void LoadSubreportDefinition(string reportName, TextReader report)
    {
      lock (this.m_syncObject)
      {
        switch (reportName)
        {
          case null:
            throw new ArgumentNullException(nameof (reportName));
          case "":
            throw new ArgumentOutOfRangeException(nameof (reportName));
          default:
            if (report == null)
              throw new ArgumentNullException(nameof (report));
            this.SetDirectReportDefinition(reportName, report);
            break;
        }
      }
    }

    public void LoadSubreportDefinition(string reportName, Stream report)
    {
      if (report == null)
        throw new ArgumentNullException(nameof (report));
      this.LoadSubreportDefinition(reportName, (TextReader) new StreamReader(report));
    }

    private void SetDirectReportDefinition(string reportName, TextReader report)
    {
      this.DemandFullTrustWithFriendlyMessage();
      byte[] reportBytes = Encoding.UTF8.GetBytes(report.ReadToEnd());
      this.ChangeReportDefinition((DefinitionSource) 3, (Action) (() => this.m_processingHost.Catalog.SetReportDefinition(reportName, reportBytes)));
    }

    internal override int PerformSearch(string searchText, int startPage, int endPage)
    {
      try
      {
        lock (this.m_syncObject)
        {
          if (!this.m_processingHost.ExecutionInfo.HasSnapshot)
            throw new InvalidOperationException(CommonStrings.ReportNotReady);
          if (string.IsNullOrEmpty(searchText))
            return -1;
          try
          {
            return this.m_processingHost.PerformSearch(startPage, endPage, searchText);
          }
          catch (Exception ex)
          {
            throw this.WrapProcessingException(ex);
          }
        }
      }
      catch (SecurityException ex)
      {
        throw new LocalProcessingException(CommonStrings.LocalModeMissingFullTrustErrors, (Exception) ex);
      }
    }

    internal override void PerformToggle(string toggleId)
    {
      try
      {
        lock (this.m_syncObject)
        {
          if (!this.m_processingHost.ExecutionInfo.HasSnapshot)
            throw new InvalidOperationException(CommonStrings.ReportNotReady);
          try
          {
            this.m_processingHost.PerformToggle(toggleId);
          }
          catch (Exception ex)
          {
            throw this.WrapProcessingException(ex);
          }
        }
      }
      catch (SecurityException ex)
      {
        throw new LocalProcessingException(CommonStrings.LocalModeMissingFullTrustErrors, (Exception) ex);
      }
    }

    internal override int PerformBookmarkNavigation(string bookmarkId, out string uniqueName)
    {
      try
      {
        lock (this.m_syncObject)
        {
          if (!this.m_processingHost.ExecutionInfo.HasSnapshot)
            throw new InvalidOperationException(CommonStrings.ReportNotReady);
          try
          {
            return this.m_processingHost.PerformBookmarkNavigation(bookmarkId, ref uniqueName);
          }
          catch (Exception ex)
          {
            throw this.WrapProcessingException(ex);
          }
        }
      }
      catch (SecurityException ex)
      {
        throw new LocalProcessingException(CommonStrings.LocalModeMissingFullTrustErrors, (Exception) ex);
      }
    }

    internal override int PerformDocumentMapNavigation(string documentMapId)
    {
      try
      {
        lock (this.m_syncObject)
        {
          if (!this.m_processingHost.ExecutionInfo.HasSnapshot)
            throw new InvalidOperationException(CommonStrings.ReportNotReady);
          try
          {
            return this.m_processingHost.PerformDocumentMapNavigation(documentMapId);
          }
          catch (Exception ex)
          {
            throw this.WrapProcessingException(ex);
          }
        }
      }
      catch (SecurityException ex)
      {
        throw new LocalProcessingException(CommonStrings.LocalModeMissingFullTrustErrors, (Exception) ex);
      }
    }

    internal override Report PerformDrillthrough(string drillthroughId, out string reportPath)
    {
      try
      {
        lock (this.m_syncObject)
        {
          if (!this.m_processingHost.ExecutionInfo.HasSnapshot)
            throw new InvalidOperationException(CommonStrings.ReportNotReady);
          if (drillthroughId == null)
            throw new ArgumentNullException(nameof (drillthroughId));
          NameValueCollection drillParams;
          try
          {
            reportPath = this.m_processingHost.PerformDrillthrough(drillthroughId, ref drillParams);
          }
          catch (Exception ex)
          {
            throw this.WrapProcessingException(ex);
          }
          LocalReport newLocalReport = this.CreateNewLocalReport();
          this.PopulateDrillthroughReport(((CatalogItemContextBase<string>) this.CreateItemContext()).MapUserProvidedPath(reportPath), drillParams, newLocalReport);
          return (Report) newLocalReport;
        }
      }
      catch (SecurityException ex)
      {
        throw new LocalProcessingException(CommonStrings.LocalModeMissingFullTrustErrors, (Exception) ex);
      }
    }

    public override ReportPageSettings GetDefaultPageSettings()
    {
      lock (this.m_syncObject)
      {
        this.EnsureExecutionSession();
        PageProperties pageProperties = this.m_processingHost.ExecutionInfo.PageProperties;
        return new ReportPageSettings(pageProperties.PageHeight, pageProperties.PageWidth, pageProperties.LeftMargin, pageProperties.RightMargin, pageProperties.TopMargin, pageProperties.BottomMargin);
      }
    }

    private void PopulateDrillthroughReport(
      string reportPath,
      NameValueCollection drillParams,
      LocalReport drillReport)
    {
      drillReport.CopySecuritySettings(this);
      if (this.ReportPath != null)
        drillReport.ReportPath = reportPath;
      else if (this.ReportEmbeddedResource != null)
        drillReport.SetEmbeddedResourceAsReportDefinition(reportPath, this.m_embeddedResourceAssembly);
      drillReport.DrillthroughDepth = this.DrillthroughDepth + 1;
      drillReport.m_parentSuppliedParameters = drillParams;
    }

    internal override int PerformSort(
      string sortId,
      SortOrder sortDirection,
      bool clearSort,
      PageCountMode pageCountMode,
      out string uniqueName)
    {
      try
      {
        lock (this.m_syncObject)
        {
          if (!this.m_processingHost.ExecutionInfo.HasSnapshot)
            throw new InvalidOperationException(CommonStrings.ReportNotReady);
          SortOptions sortOptions = sortDirection != SortOrder.Ascending ? (SortOptions) 2 : (SortOptions) 1;
          string processingPaginationMode = LocalReport.PageCountModeToProcessingPaginationMode(pageCountMode);
          try
          {
            return this.m_processingHost.PerformSort(processingPaginationMode, sortId, sortOptions, clearSort, ref uniqueName);
          }
          catch (Exception ex)
          {
            throw this.WrapProcessingException(ex);
          }
        }
      }
      catch (SecurityException ex)
      {
        throw new LocalProcessingException(CommonStrings.LocalModeMissingFullTrustErrors, (Exception) ex);
      }
    }

    [Obsolete("This method requires Code Access Security policy, which is deprecated.  For more information please go to http://go.microsoft.com/fwlink/?LinkId=160787.")]
    public void ExecuteReportInCurrentAppDomain(Evidence reportEvidence)
    {
      try
      {
        lock (this.m_syncObject)
          this.m_processingHost.ExecuteReportInCurrentAppDomain(reportEvidence);
      }
      catch (SecurityException ex)
      {
        throw new LocalProcessingException(CommonStrings.LocalModeMissingFullTrustErrors, (Exception) ex);
      }
    }

    [Obsolete("This method requires Code Access Security policy, which is deprecated.  For more information please go to http://go.microsoft.com/fwlink/?LinkId=160787.")]
    public void AddTrustedCodeModuleInCurrentAppDomain(string assemblyName)
    {
      try
      {
        lock (this.m_syncObject)
          this.m_processingHost.AddTrustedCodeModuleInCurrentAppDomain(assemblyName);
      }
      catch (SecurityException ex)
      {
        throw new LocalProcessingException(CommonStrings.LocalModeMissingFullTrustErrors, (Exception) ex);
      }
    }

    [Obsolete("This method requires Code Access Security policy, which is deprecated.  For more information please go to http://go.microsoft.com/fwlink/?LinkId=160787.")]
    public void ExecuteReportInSandboxAppDomain()
    {
      try
      {
        lock (this.m_syncObject)
          this.m_processingHost.ExecuteReportInSandboxAppDomain();
      }
      catch (SecurityException ex)
      {
        throw new LocalProcessingException(CommonStrings.LocalModeMissingFullTrustErrors, (Exception) ex);
      }
    }

    public void AddFullTrustModuleInSandboxAppDomain(StrongName assemblyName)
    {
      try
      {
        lock (this.m_syncObject)
          this.m_processingHost.AddFullTrustModuleInSandboxAppDomain(assemblyName);
      }
      catch (SecurityException ex)
      {
        throw new LocalProcessingException(CommonStrings.LocalModeMissingFullTrustErrors, (Exception) ex);
      }
    }

    public void SetBasePermissionsForSandboxAppDomain(PermissionSet permissions)
    {
      try
      {
        lock (this.m_syncObject)
          this.m_processingHost.SetBasePermissionsForSandboxAppDomain(permissions);
      }
      catch (SecurityException ex)
      {
        throw new LocalProcessingException(CommonStrings.LocalModeMissingFullTrustErrors, (Exception) ex);
      }
    }

    public void ReleaseSandboxAppDomain()
    {
      lock (this.m_syncObject)
        this.m_processingHost.ReleaseSandboxAppDomain();
    }

    private void CopySecuritySettings(LocalReport parentReport)
    {
      this.m_processingHost.CopySecuritySettingsFrom(parentReport.m_processingHost);
      this.m_enableExternalImages = parentReport.EnableExternalImages;
      this.m_enableHyperlinks = parentReport.EnableHyperlinks;
      this.ShowDetailedSubreportMessages = parentReport.ShowDetailedSubreportMessages;
    }

    internal bool HasExecutionSession => this.m_processingHost.ExecutionInfo.IsCompiled;

    internal override void EnsureExecutionSession()
    {
      if (this.DefinitionSource == null)
        throw new MissingReportSourceException();
      try
      {
        if (this.HasExecutionSession)
          return;
        this.m_processingHost.CompileReport();
        this.m_processingHost.SetReportParameters(this.m_parentSuppliedParameters);
      }
      catch (SecurityException ex)
      {
        throw new LocalProcessingException(CommonStrings.LocalModeMissingFullTrustErrors, (Exception) ex);
      }
      catch (Exception ex)
      {
        throw this.WrapProcessingException(ex);
      }
    }

    private void ValidateReportSecurity(
      PreviewItemContext itemContext,
      PublishingResult publishingResult)
    {
      if (publishingResult.HasExternalImages && !this.EnableExternalImages)
      {
        // ISSUE: reference to a compiler-generated method
        throw new ReportSecurityException(CommonStrings.ExternalImagesError(((CatalogItemContextBase<string>) itemContext).ItemName));
      }
      if (publishingResult.HasHyperlinks && !this.EnableHyperlinks)
      {
        // ISSUE: reference to a compiler-generated method
        throw new ReportSecurityException(CommonStrings.HyperlinkSecurityError(((CatalogItemContextBase<string>) itemContext).ItemName));
      }
    }

    public override void Refresh()
    {
      try
      {
        lock (this.m_syncObject)
        {
          if (!this.m_processingHost.ExecutionInfo.HasSnapshot)
            return;
          this.m_processingHost.ResetExecution();
          this.OnChange(true);
        }
      }
      catch (SecurityException ex)
      {
        throw new LocalProcessingException(CommonStrings.LocalModeMissingFullTrustErrors, (Exception) ex);
      }
    }

    private void ChangeReportDefinition(DefinitionSource updatingSourceType, Action changeAction)
    {
      DefinitionSource definitionSource1 = this.DefinitionSource;
      changeAction();
      DefinitionSource definitionSource2 = this.DefinitionSource;
      if (definitionSource2 != updatingSourceType && definitionSource2 == definitionSource1)
        return;
      this.m_processingHost.ItemContext = this.CreateItemContext();
      this.OnChange(false);
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IList<ReportParameter> OriginalParametersToDrillthrough
    {
      get
      {
        return (IList<ReportParameter>) new ReadOnlyCollection<ReportParameter>((IList<ReportParameter>) ReportParameter.FromNameValueCollection(this.m_parentSuppliedParameters));
      }
    }

    public override ReportParameterInfoCollection GetParameters()
    {
      lock (this.m_syncObject)
      {
        this.EnsureExecutionSession();
        return LocalReport.ParameterInfoCollectionToApi(this.m_processingHost.ExecutionInfo.ReportParameters, this.SupportsQueries);
      }
    }

    public override void SetParameters(IEnumerable<ReportParameter> parameters)
    {
      if (parameters == null)
        throw new ArgumentNullException(nameof (parameters));
      lock (this.m_syncObject)
      {
        this.EnsureExecutionSession();
        NameValueCollection nameValueCollection = ReportParameter.ToNameValueCollection(parameters);
        try
        {
          this.m_processingHost.SetReportParameters(nameValueCollection);
        }
        catch (Exception ex)
        {
          throw this.WrapProcessingException(ex);
        }
        this.OnChange(false);
      }
    }

    internal override byte[] InternalRenderStream(
      string format,
      string streamID,
      string deviceInfo,
      out string mimeType,
      out string encoding)
    {
      try
      {
        encoding = (string) null;
        lock (this.m_syncObject)
          return this.m_processingHost.RenderStream(format, deviceInfo, streamID, ref mimeType);
      }
      catch (SecurityException ex)
      {
        throw new LocalProcessingException(CommonStrings.LocalModeMissingFullTrustErrors, (Exception) ex);
      }
      catch (Exception ex)
      {
        throw this.WrapProcessingException(ex);
      }
    }

    internal override bool HasDocMap
    {
      get
      {
        try
        {
          lock (this.m_syncObject)
            return this.m_processingHost.ExecutionInfo.HasDocMap;
        }
        catch (SecurityException ex)
        {
          throw new LocalProcessingException(CommonStrings.LocalModeMissingFullTrustErrors, (Exception) ex);
        }
      }
    }

    internal override DocumentMapNode GetDocumentMap(string rootLabel)
    {
      try
      {
        lock (this.m_syncObject)
        {
          if (!this.m_processingHost.ExecutionInfo.HasSnapshot)
            throw new InvalidOperationException(CommonStrings.ReportNotReady);
          IDocumentMap documentMap;
          try
          {
            documentMap = this.m_processingHost.GetDocumentMap();
          }
          catch (Exception ex)
          {
            throw this.WrapProcessingException(ex);
          }
          return DocumentMapNode.CreateTree(documentMap, rootLabel);
        }
      }
      catch (SecurityException ex)
      {
        throw new LocalProcessingException(CommonStrings.LocalModeMissingFullTrustErrors, (Exception) ex);
      }
    }

    internal override int AutoRefreshInterval
    {
      get
      {
        lock (this.m_syncObject)
          return this.m_processingHost.ExecutionInfo.AutoRefreshInterval;
      }
    }

    public override byte[] Render(
      string format,
      string deviceInfo,
      PageCountMode pageCountMode,
      out string mimeType,
      out string encoding,
      out string fileNameExtension,
      out string[] streams,
      out Warning[] warnings)
    {
      return this.InternalRender(format, false, deviceInfo, pageCountMode, out mimeType, out encoding, out fileNameExtension, out streams, out warnings);
    }

    internal byte[] InternalRender(
      string format,
      bool allowInternalRenderers,
      string deviceInfo,
      PageCountMode pageCountMode,
      out string mimeType,
      out string encoding,
      out string fileNameExtension,
      out string[] streams,
      out Warning[] warnings)
    {
      lock (this.m_syncObject)
      {
        using (StreamCache streamCache = new StreamCache())
        {
          this.InternalRender(format, allowInternalRenderers, deviceInfo, pageCountMode, new CreateAndRegisterStream(streamCache.StreamCallback), out warnings);
          streams = new string[0];
          return streamCache.GetMainStream(out encoding, out mimeType, out fileNameExtension);
        }
      }
    }

    public void Render(
      string format,
      string deviceInfo,
      CreateStreamCallback createStream,
      out Warning[] warnings)
    {
      this.Render(format, deviceInfo, PageCountMode.Estimate, createStream, out warnings);
    }

    public void Render(
      string format,
      string deviceInfo,
      PageCountMode pageCountMode,
      CreateStreamCallback createStream,
      out Warning[] warnings)
    {
      // ISSUE: object of a compiler-generated type is created
      // ISSUE: variable of a compiler-generated type
      LocalReport.\u003C\u003Ec__DisplayClassb cDisplayClassb = new LocalReport.\u003C\u003Ec__DisplayClassb();
      // ISSUE: reference to a compiler-generated field
      cDisplayClassb.createStream = createStream;
      // ISSUE: reference to a compiler-generated field
      if (cDisplayClassb.createStream == null)
        throw new ArgumentNullException(nameof (createStream));
      // ISSUE: method pointer
      using (ProcessingStreamHandler processingStreamHandler = new ProcessingStreamHandler(new CreateAndRegisterStream((object) cDisplayClassb, __methodptr(\u003CRender\u003Eb__a))))
      {
        // ISSUE: method pointer
        this.InternalRender(format, false, deviceInfo, pageCountMode, new CreateAndRegisterStream((object) processingStreamHandler, __methodptr(StreamCallback)).ToInnerType(), out warnings);
      }
    }

    internal void InternalRender(
      string format,
      bool allowInternalRenderers,
      string deviceInfo,
      PageCountMode pageCountMode,
      CreateAndRegisterStream createStreamCallback,
      out Warning[] warnings)
    {
      lock (this.m_syncObject)
      {
        if (createStreamCallback == null)
          throw new ArgumentNullException(nameof (createStreamCallback));
        if (!this.ValidateRenderingFormat(format))
          throw new ArgumentOutOfRangeException(nameof (format));
        this.EnsureExecutionSession();
        try
        {
          this.m_lastRenderingWarnings = this.m_processingHost.Render(format, deviceInfo, LocalReport.PageCountModeToProcessingPaginationMode(pageCountMode), allowInternalRenderers, (IEnumerable) this.m_dataSources, createStreamCallback.ToOuterType());
        }
        catch (Exception ex)
        {
          throw this.WrapProcessingException(ex);
        }
        warnings = Warning.FromProcessingMessageList(this.m_lastRenderingWarnings);
        this.WriteDebugResults(warnings);
      }
    }

    private void WriteDebugResults(Warning[] warnings)
    {
      foreach (Warning warning in warnings)
        Trace.WriteLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}: {1} ({2})", (object) warning.Severity, (object) warning.Message, (object) warning.Code));
    }

    private bool ValidateRenderingFormat(string format)
    {
      try
      {
        foreach (LocalRenderingExtensionInfo renderingExtension in this.m_processingHost.ListRenderingExtensions())
        {
          if (string.Compare(renderingExtension.Name, format, StringComparison.OrdinalIgnoreCase) == 0)
            return true;
        }
        return false;
      }
      catch (Exception ex)
      {
        throw this.WrapProcessingException(ex);
      }
    }

    internal void TransferEvents(LocalReport targetReport)
    {
      if (targetReport != null)
        targetReport.SubreportProcessing = this.SubreportProcessing;
      this.SubreportProcessing = (SubreportProcessingEventHandler) null;
    }

    internal void CreateSnapshot()
    {
      this.m_processingHost.Render((string) null, (string) null, (string) null, false, (IEnumerable) this.m_dataSources, (CreateAndRegisterStream) null);
    }

    internal event InitializeDataSourcesEventHandler InitializeDataSources;

    [Microsoft.Reporting.SRDescription("SubreportProcessingEventDesc")]
    public event SubreportProcessingEventHandler SubreportProcessing;

    private IEnumerable ControlSubReportInfoCallback(
      PreviewItemContext subReportContext,
      ParameterInfoCollection initialParameters)
    {
      if (this.SubreportProcessing == null)
        return (IEnumerable) null;
      string[] dataSetNames;
      try
      {
        dataSetNames = this.m_processingHost.GetDataSetNames(subReportContext);
      }
      catch (Exception ex)
      {
        throw this.WrapProcessingException(ex);
      }
      SubreportProcessingEventArgs e = new SubreportProcessingEventArgs(((CatalogItemContextBase<string>) subReportContext).OriginalItemPath, LocalReport.ParameterInfoCollectionToApi(initialParameters, this.SupportsQueries), dataSetNames);
      this.SubreportProcessing((object) this, e);
      if (this.InitializeDataSources != null)
        this.InitializeDataSources((object) this, new InitializeDataSourcesEventArgs(e.DataSources));
      return (IEnumerable) e.DataSources;
    }

    public override RenderingExtension[] ListRenderingExtensions()
    {
      if (this.m_externalRenderingExtensions == null)
      {
        List<RenderingExtension> renderingExtensionList = new List<RenderingExtension>();
        try
        {
          foreach (LocalRenderingExtensionInfo renderingExtension in this.m_processingHost.ListRenderingExtensions())
          {
            if (renderingExtension.IsExposedExternally)
              renderingExtensionList.Add(new RenderingExtension(renderingExtension.Name, renderingExtension.LocalizedName, renderingExtension.IsVisible));
          }
        }
        catch (Exception ex)
        {
          throw this.WrapProcessingException(ex);
        }
        this.m_externalRenderingExtensions = renderingExtensionList.ToArray();
      }
      return this.m_externalRenderingExtensions;
    }

    private string GetFullyQualifiedReportPath()
    {
      switch (this.DefinitionSource - 1)
      {
        case 0:
          return LocalReport.GetReportNameForFile(this.ReportPath);
        case 1:
          return this.ReportEmbeddedResource;
        case 2:
          return "";
        default:
          return string.Empty;
      }
    }

    private static string GetReportNameForFile(string path)
    {
      if (Path.IsPathRooted(path))
        return path;
      string path1;
      if (HttpContext.Current != null && HttpContext.Current.Request != null)
      {
        HttpRequest request = HttpContext.Current.Request;
        path1 = request.MapPath(request.ApplicationPath);
      }
      else
        path1 = Environment.CurrentDirectory;
      return Path.Combine(path1, path);
    }

    private PreviewItemContext CreateItemContext()
    {
      return LocalReport.CreateItemContext(this.ReportPath, this.GetFullyQualifiedReportPath(), this.DefinitionSource, this.m_embeddedResourceAssembly);
    }

    internal static PreviewItemContext CreateItemContextForFilePath(string filePath)
    {
      return LocalReport.CreateItemContext(filePath, LocalReport.GetReportNameForFile(filePath), (DefinitionSource) 1, (Assembly) null);
    }

    private static PreviewItemContext CreateItemContext(
      string pathForFileDefinitionSource,
      string fullyQualifiedPath,
      DefinitionSource definitionSource,
      Assembly embeddedResourceAssembly)
    {
      if (definitionSource == null)
        return (PreviewItemContext) null;
      PreviewItemContext itemContext = LocalReport.InstantiatePreviewItemContext();
      itemContext.SetPath(pathForFileDefinitionSource, fullyQualifiedPath, definitionSource, embeddedResourceAssembly);
      return itemContext;
    }

    private LocalProcessingException WrapProcessingException(Exception processingException)
    {
      Exception processingException1 = processingException;
      while (processingException1 != null && processingException1.InnerException != null && (processingException1 is ReportRenderingException || processingException1 is UnhandledReportRenderingException || processingException1 is HandledReportRenderingException))
        processingException1 = processingException1.InnerException;
      return processingException1 is LocalProcessingException processingException2 ? processingException2 : new LocalProcessingException(processingException1);
    }

    private static string PageCountModeToProcessingPaginationMode(PageCountMode pageCountMode)
    {
      return pageCountMode == PageCountMode.Actual ? "Actual" : "Estimate";
    }

    private static ReportParameterInfoCollection ParameterInfoCollectionToApi(
      ParameterInfoCollection processingMetadata,
      bool supportsQueries)
    {
      if (processingMetadata == null)
        return new ReportParameterInfoCollection();
      ReportParameterInfo[] parameterInfos = new ReportParameterInfo[((ArrayList) processingMetadata).Count];
      for (int index = 0; index < ((ArrayList) processingMetadata).Count; ++index)
        parameterInfos[index] = LocalReport.ParameterInfoToApi(processingMetadata[index], supportsQueries);
      return new ReportParameterInfoCollection((IList<ReportParameterInfo>) parameterInfos);
    }

    private static ReportParameterInfo ParameterInfoToApi(
      ParameterInfo paramInfo,
      bool supportsQueries)
    {
      string[] dependencies = (string[]) null;
      if (paramInfo.DependencyList != null)
      {
        dependencies = new string[((ArrayList) paramInfo.DependencyList).Count];
        for (int index = 0; index < ((ArrayList) paramInfo.DependencyList).Count; ++index)
          dependencies[index] = ((ParameterBase) paramInfo.DependencyList[index]).Name;
      }
      string[] currentValues = (string[]) null;
      if (paramInfo.Values != null)
      {
        currentValues = new string[paramInfo.Values.Length];
        for (int index = 0; index < paramInfo.Values.Length; ++index)
          currentValues[index] = paramInfo.CastToString(paramInfo.Values[index], CultureInfo.CurrentCulture);
      }
      List<ValidValue> validValues = (List<ValidValue>) null;
      if (paramInfo.ValidValues != null)
      {
        validValues = new List<ValidValue>(((ArrayList) paramInfo.ValidValues).Count);
        foreach (ValidValue validValue in (ArrayList) paramInfo.ValidValues)
        {
          string str = paramInfo.CastToString(validValue.Value, CultureInfo.CurrentCulture);
          validValues.Add(new ValidValue(validValue.Label, str));
        }
      }
      ParameterState state;
      switch ((int) paramInfo.State)
      {
        case 0:
          state = ParameterState.HasValidValue;
          break;
        case 1:
        case 2:
        case 3:
          state = ParameterState.MissingValidValue;
          break;
        case 4:
          state = ParameterState.HasOutstandingDependencies;
          break;
        case 5:
          state = ParameterState.DynamicValuesUnavailable;
          break;
        default:
          state = ParameterState.MissingValidValue;
          break;
      }
      return new ReportParameterInfo(((ParameterBase) paramInfo).Name, (ParameterDataType) Enum.Parse(typeof (ParameterDataType), ((ParameterBase) paramInfo).DataType.ToString()), ((ParameterBase) paramInfo).Nullable, ((ParameterBase) paramInfo).AllowBlank, ((ParameterBase) paramInfo).MultiValue, supportsQueries && ((ParameterBase) paramInfo).UsedInQuery, ((ParameterBase) paramInfo).Prompt, ((ParameterBase) paramInfo).PromptUser, supportsQueries && paramInfo.DynamicDefaultValue, supportsQueries && paramInfo.DynamicValidValues, (string) null, currentValues, (IList<ValidValue>) validValues, dependencies, state);
    }

    private void OnLocalReportChange(object sender, EventArgs e)
    {
      this.m_processingHost.ResetExecution();
    }

    public LocalReport()
      : this((ILocalProcessingHost) new ControlService((ILocalCatalog) new StandalonePreviewStore()))
    {
    }

    private LocalReport CreateNewLocalReport() => new LocalReport();

    private LocalDataRetrieval CreateDataRetrieval()
    {
      return (LocalDataRetrieval) new LocalDataRetrievalFromDataSet()
      {
        SubReportDataSetCallback = new LocalDataRetrievalFromDataSet.GetSubReportDataSetCallback(this.ControlSubReportInfoCallback)
      };
    }

    private static PreviewItemContext InstantiatePreviewItemContext() => new PreviewItemContext();
  }
}
