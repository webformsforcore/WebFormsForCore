// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.ServerReport
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

using Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution;
using Microsoft.ReportingServices.Diagnostics.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web.UI.WebControls.WebParts;
using System.Xml;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  [Serializable]
  public sealed class ServerReport : Report, ISerializable
  {
    private const string ParamServerSession = "ReportSession";
    private Uri m_serverUrl = new Uri("http://localhost/reportserver");
    private IReportServerCredentials m_serverCredentials;
    private WindowsIdentity m_serverIdentity;
    private ReportViewerHeaderCollection m_headers;
    private ReportViewerCookieCollection m_cookies;
    private string m_reportPath = string.Empty;
    private string m_historyID = string.Empty;
    private string m_executionID;
    private int m_timeOut = 600000;
    private List<int> m_hiddenParameters = new List<int>();
    private IReportExecutionService m_service;
    private ExecutionInfo m_executionInfo;
    private TrustedUserHeader m_trustedUserHeader;
    private RenderingExtension[] m_renderingExtensions;
    private AbortState m_abortState = new AbortState();

    public ServerReport()
    {
      this.m_headers = new ReportViewerHeaderCollection(this.m_syncObject);
      this.m_cookies = new ReportViewerCookieCollection(this.m_syncObject);
    }

    internal ServerReport(ServerReport original)
      : this()
    {
      this.ReportServerUrl = new Uri(original.ReportServerUrl.ToString());
      this.Timeout = original.Timeout;
      foreach (string header in (Collection<string>) original.Headers)
        this.Headers.Add(header);
      foreach (Cookie cookie in (Collection<Cookie>) original.Cookies)
        this.Cookies.Add(cookie);
      this.ReportServerCredentials = original.ReportServerCredentials;
    }

    private ServerReport(ServerReport parentReport, ExecutionInfo executionInfo)
      : this(parentReport)
    {
      this.m_reportPath = executionInfo.ReportPath;
      this.m_executionID = executionInfo.ExecutionID;
      this.m_executionInfo = executionInfo;
      this.m_trustedUserHeader = parentReport.TrustedUserHeaderValue;
      this.DrillthroughDepth = parentReport.DrillthroughDepth + 1;
    }

    internal TrustedUserHeader TrustedUserHeaderValue
    {
      get => this.m_trustedUserHeader;
      set
      {
        this.m_trustedUserHeader = value;
        this.m_service = (IReportExecutionService) null;
      }
    }

    [SecurityTreatAsSafe]
    [SecurityCritical]
    [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
    internal ServerReport(SerializationInfo info, StreamingContext context)
      : this()
    {
      this.m_serverUrl = (Uri) info.GetValue("ServerURL", typeof (Uri));
      this.m_timeOut = info.GetInt32("TimeOut");
      this.m_headers = (ReportViewerHeaderCollection) info.GetValue(nameof (Headers), typeof (ReportViewerHeaderCollection));
      this.m_cookies = (ReportViewerCookieCollection) info.GetValue(nameof (Cookies), typeof (ReportViewerCookieCollection));
      this.m_headers.SetSyncObject(this.m_syncObject);
      this.m_cookies.SetSyncObject(this.m_syncObject);
      this.OnCredentialsChanged((IReportServerCredentials) info.GetValue("Credentials", typeof (IReportServerCredentials)));
      this.LoadViewState(info.GetValue("ViewStateValues", typeof (object[])));
    }

    [SecurityCritical]
    [SecurityTreatAsSafe]
    [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
    void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
    {
      lock (this.m_syncObject)
      {
        info.AddValue("ServerURL", (object) this.m_serverUrl);
        info.AddValue("TimeOut", this.m_timeOut);
        info.AddValue("Credentials", (object) this.m_serverCredentials, typeof (IReportServerCredentials));
        info.AddValue("Headers", (object) this.m_headers);
        info.AddValue("Cookies", (object) this.m_cookies);
        info.AddValue("ViewStateValues", this.SaveViewState());
      }
    }

    internal event EventHandler ExecutionIDChanged;

    private void OnExecutionIDChanged()
    {
      if (this.ExecutionIDChanged == null)
        return;
      this.ExecutionIDChanged((object) this, EventArgs.Empty);
    }

    internal object SaveViewState()
    {
      lock (this.m_syncObject)
        return (object) new object[4]
        {
          (object) this.m_executionID,
          (object) this.m_hiddenParameters.ToArray(),
          (object) this.DisplayName,
          (object) this.DrillthroughDepth
        };
    }

    internal void LoadViewState(object viewStateObj)
    {
      lock (this.m_syncObject)
      {
        object[] objArray = (object[]) viewStateObj;
        this.m_executionID = (string) objArray[0];
        this.m_hiddenParameters.AddRange((IEnumerable<int>) (int[]) objArray[1]);
        this.DisplayName = (string) objArray[2];
        this.DrillthroughDepth = (int) objArray[3];
        if (this.m_executionID != null)
        {
          this.EnsureExecutionSession();
          this.m_historyID = this.m_executionInfo.HistoryID;
          this.m_reportPath = this.m_executionInfo.ReportPath != null ? this.m_executionInfo.ReportPath : "";
        }
        this.OnExecutionIDChanged();
      }
    }

    internal string SerializeToUrlQuery()
    {
      lock (this.m_syncObject)
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}={1}", (object) "ReportSession", (object) this.m_executionID);
    }

    internal void LoadFromUrlQuery(NameValueCollection requestParameters, bool fullReportLoad)
    {
      lock (this.m_syncObject)
      {
        string requestParameter = requestParameters["ReportSession"];
        if (string.IsNullOrEmpty(requestParameter))
          return;
        this.SetExecutionId(requestParameter, fullReportLoad);
      }
    }

    private void OnCredentialsChanged(IReportServerCredentials credentials)
    {
      this.m_serverIdentity = credentials == null ? (WindowsIdentity) null : credentials.ImpersonationUser;
      this.m_serverCredentials = credentials;
      this.m_service = (IReportExecutionService) null;
      this.ClearSession();
    }

    [NotifyParentProperty(true)]
    [WebBrowsable(true)]
    [DefaultValue(typeof (Uri), "http://localhost/reportserver")]
    [Microsoft.Reporting.SRDescription("ReportServerUrlDesc")]
    public Uri ReportServerUrl
    {
      get => this.m_serverUrl;
      set
      {
        lock (this.m_syncObject)
        {
          if (value == (Uri) null)
            throw new ArgumentNullException(nameof (value));
          if (this.IsDrillthroughReport)
            throw new InvalidOperationException();
          if (Uri.Compare(this.m_serverUrl, value, UriComponents.AbsoluteUri, UriFormat.UriEscaped, StringComparison.Ordinal) == 0)
            return;
          this.m_serverUrl = value;
          this.m_service = (IReportExecutionService) null;
          this.ClearServerSpecificInfo();
        }
      }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Browsable(false)]
    public IReportServerCredentials ReportServerCredentials
    {
      get => this.m_serverCredentials;
      set
      {
        lock (this.m_syncObject)
        {
          if (value == this.m_serverCredentials)
            return;
          if (this.IsDrillthroughReport)
            throw new InvalidOperationException();
          this.OnCredentialsChanged(value);
        }
      }
    }

    [DefaultValue(600000)]
    [NotifyParentProperty(true)]
    [Microsoft.Reporting.SRDescription("ServerTimeoutDesc")]
    [WebBrowsable(true)]
    public int Timeout
    {
      get => this.m_timeOut;
      set
      {
        lock (this.m_syncObject)
        {
          if (this.IsDrillthroughReport)
            throw new InvalidOperationException();
          this.m_timeOut = value;
          if (this.m_service == null)
            return;
          this.m_service.Timeout = value;
        }
      }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Browsable(false)]
    public ReportViewerHeaderCollection Headers => this.m_headers;

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ReportViewerCookieCollection Cookies => this.m_cookies;

    [NotifyParentProperty(true)]
    [WebBrowsable(true)]
    [DefaultValue("")]
    [Microsoft.Reporting.SRDescription("ServerReportPathDesc")]
    public string ReportPath
    {
      get => this.m_reportPath;
      set
      {
        lock (this.m_syncObject)
        {
          if (value == null)
            throw new ArgumentNullException(nameof (value));
          if (this.IsDrillthroughReport)
            throw new InvalidOperationException();
          if (string.Compare(this.m_reportPath, value, StringComparison.OrdinalIgnoreCase) == 0)
            return;
          this.ClearSession();
          this.m_reportPath = value;
        }
      }
    }

    [NotifyParentProperty(true)]
    [WebBrowsable(true)]
    [DefaultValue("")]
    [Microsoft.Reporting.SRDescription("HistoryIdDesc")]
    public string HistoryId
    {
      get => this.m_historyID;
      set
      {
        lock (this.m_syncObject)
        {
          if (this.IsDrillthroughReport)
            throw new InvalidOperationException();
          this.m_historyID = value;
          this.Refresh();
        }
      }
    }

    internal override string DisplayNameForUse
    {
      get
      {
        lock (this.m_syncObject)
        {
          if (!string.IsNullOrEmpty(this.DisplayName))
            return this.DisplayName;
          string displayNameForUse = ServerReport.RetrieveReportNameFromPath(this.ReportPath);
          if (string.IsNullOrEmpty(displayNameForUse))
            displayNameForUse = CommonStrings.Report;
          return displayNameForUse;
        }
      }
    }

    private static string RetrieveReportNameFromPath(string reportPath)
    {
      string str = (string) null;
      if (!string.IsNullOrEmpty(reportPath))
      {
        if (reportPath.IndexOfAny(Path.GetInvalidPathChars()) < 0)
        {
          try
          {
            str = Path.GetFileName(reportPath);
          }
          catch (ArgumentException ex)
          {
          }
        }
      }
      return str;
    }

    internal DateTime GetExecutionSessionExpiration()
    {
      lock (this.m_syncObject)
      {
        this.EnsureExecutionSession();
        return this.m_executionInfo.ExpirationDateTime;
      }
    }

    public bool IsQueryExecutionAllowed()
    {
      lock (this.m_syncObject)
      {
        this.EnsureExecutionSession();
        return this.m_executionInfo.AllowQueryExecution;
      }
    }

    public override ReportParameterInfoCollection GetParameters()
    {
      lock (this.m_syncObject)
      {
        this.EnsureExecutionSession();
        ReportParameterInfoCollection parameters = this.m_executionInfo.Parameters;
        if (parameters != null)
        {
          for (int index = 0; index < parameters.Count; ++index)
            parameters[index].Visible = !this.m_hiddenParameters.Contains(index);
        }
        return parameters;
      }
    }

    public override void SetParameters(IEnumerable<ReportParameter> parameters)
    {
      lock (this.m_syncObject)
      {
        if (parameters == null)
          throw new ArgumentNullException(nameof (parameters));
        this.EnsureExecutionSession();
        Dictionary<int, bool> dictionary = new Dictionary<int, bool>();
        foreach (ReportParameter parameter in parameters)
        {
          int key = parameter != null && parameter.Name != null ? this.GetIndexForParameter(parameter.Name) : throw new ArgumentNullException(nameof (parameters));
          if (dictionary.ContainsKey(key))
          {
            // ISSUE: reference to a compiler-generated method
            throw new ArgumentException(CommonStrings.ParameterSpecifiedMultipleTimes(parameter.Name));
          }
          dictionary.Add(key, parameter.Visible);
        }
        this.m_executionInfo = this.Service.SetExecutionParameters(parameters, Thread.CurrentThread.CurrentCulture.Name);
        foreach (int key in dictionary.Keys)
        {
          if (dictionary[key])
            this.m_hiddenParameters.Remove(key);
          else if (!this.m_hiddenParameters.Contains(key))
            this.m_hiddenParameters.Add(key);
        }
        this.OnChange(false);
      }
    }

    public override ReportPageSettings GetDefaultPageSettings()
    {
      lock (this.m_syncObject)
      {
        this.EnsureExecutionSession();
        return this.m_executionInfo.ReportPageSettings;
      }
    }

    public void SetDataSourceCredentials(IEnumerable<DataSourceCredentials> credentials)
    {
      lock (this.m_syncObject)
      {
        if (credentials == null)
          throw new ArgumentNullException(nameof (credentials));
        this.EnsureExecutionSession();
        this.m_executionInfo = this.Service.SetExecutionCredentials(credentials);
        this.OnChange(false);
      }
    }

    public void SetExecutionId(string executionId) => this.SetExecutionId(executionId, true);

    internal void SetExecutionId(string executionId, bool fullReportLoad)
    {
      lock (this.m_syncObject)
      {
        switch (executionId)
        {
          case null:
            throw new ArgumentNullException(nameof (executionId));
          case "":
            throw new ArgumentOutOfRangeException(nameof (executionId));
          default:
            if (this.IsDrillthroughReport)
              throw new InvalidOperationException();
            this.ClearSession(false);
            this.m_executionID = executionId;
            this.ApplyExecutionIdToService(this.m_service);
            this.OnExecutionIDChanged();
            if (fullReportLoad)
            {
              this.EnsureExecutionSession();
              this.m_reportPath = this.m_executionInfo.ReportPath;
              this.m_historyID = this.m_executionInfo.HistoryID;
            }
            this.OnChange(false);
            break;
        }
      }
    }

    internal bool HasExecutionId
    {
      get
      {
        lock (this.m_syncObject)
          return !string.IsNullOrEmpty(this.m_executionID);
      }
    }

    public string GetExecutionId()
    {
      lock (this.m_syncObject)
      {
        this.EnsureExecutionSession();
        return this.m_executionID;
      }
    }

    internal override bool CanSelfCancel => true;

    internal override void SetCancelState(bool shouldCancel)
    {
      if (shouldCancel)
        this.m_abortState.AbortRequest();
      else
        this.m_abortState.ClearPendingAbort();
    }

    public Stream Render(
      string format,
      string deviceInfo,
      NameValueCollection urlAccessParameters,
      out string mimeType,
      out string fileNameExtension)
    {
      lock (this.m_syncObject)
      {
        MemoryStream reportStream = new MemoryStream();
        this.Render(format, deviceInfo, urlAccessParameters, (Stream) reportStream, out mimeType, out fileNameExtension);
        reportStream.Position = 0L;
        return (Stream) reportStream;
      }
    }

    public void Render(
      string format,
      string deviceInfo,
      NameValueCollection urlAccessParameters,
      Stream reportStream,
      out string mimeType,
      out string fileNameExtension)
    {
      this.InternalRender(false, format, deviceInfo, urlAccessParameters, reportStream, out mimeType, out fileNameExtension);
    }

    internal void InternalRender(
      bool isAbortable,
      string format,
      string deviceInfo,
      NameValueCollection urlAccessParameters,
      Stream reportStream,
      out string mimeType,
      out string fileNameExtension)
    {
      lock (this.m_syncObject)
      {
        this.EnsureExecutionSession();
        XmlNodeList deviceInfo1 = (XmlNodeList) null;
        if (!string.IsNullOrEmpty(deviceInfo))
        {
          XmlDocument xmlDocument = new XmlDocument();
          XmlReader reader = XmlReader.Create((TextReader) new StringReader(deviceInfo), new XmlReaderSettings()
          {
            CheckCharacters = false
          });
          xmlDocument.Load(reader);
          if (xmlDocument.DocumentElement != null && xmlDocument.DocumentElement.ChildNodes != null)
            deviceInfo1 = xmlDocument.DocumentElement.ChildNodes;
        }
        this.Service.Render(isAbortable ? this.m_abortState : (AbortState) null, this.ReportPath, this.m_executionID, this.HistoryId, format, deviceInfo1, urlAccessParameters, reportStream, out mimeType, out fileNameExtension);
        this.UpdatedExecutionInfoIfNecessary();
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
      lock (this.m_syncObject)
      {
        this.EnsureExecutionSession();
        byte[] numArray = this.Service.Render(format, deviceInfo, pageCountMode, out fileNameExtension, out mimeType, out encoding, out warnings, out streams);
        this.UpdatedExecutionInfoIfNecessary();
        return numArray;
      }
    }

    private void UpdatedExecutionInfoIfNecessary()
    {
      if (this.m_executionInfo.HasSnapshot && !this.m_executionInfo.NeedsProcessing && this.PageCountMode == PageCountMode.Actual)
        return;
      this.m_executionInfo = this.Service.GetExecutionInfo();
    }

    public byte[] RenderStream(
      string format,
      string streamId,
      string deviceInfo,
      out string mimeType,
      out string encoding)
    {
      lock (this.m_syncObject)
      {
        if (!this.PrepareForRender())
          throw new InvalidOperationException(CommonStrings.ReportNotReady);
        return this.InternalRenderStream(format, streamId, deviceInfo, out mimeType, out encoding);
      }
    }

    internal override byte[] InternalRenderStream(
      string format,
      string streamId,
      string deviceInfo,
      out string mimeType,
      out string encoding)
    {
      lock (this.m_syncObject)
        return this.Service.RenderStream(format, streamId, deviceInfo, out encoding, out mimeType);
    }

    public override void LoadReportDefinition(TextReader report)
    {
      lock (this.m_syncObject)
      {
        if (report == null)
          throw new ArgumentNullException(nameof (report));
        if (this.IsDrillthroughReport)
          throw new InvalidOperationException();
        this.m_executionInfo = this.Service.LoadReportDefinition(Encoding.UTF8.GetBytes(report.ReadToEnd()));
        this.m_executionID = this.m_executionInfo.ExecutionID;
        this.OnExecutionIDChanged();
        this.m_reportPath = "";
        this.Refresh();
      }
    }

    internal override int PerformSearch(string searchText, int startPage, int endPage)
    {
      lock (this.m_syncObject)
      {
        if (!this.IsReadyForProcessingPostTasks)
          throw new InvalidOperationException(CommonStrings.ReportNotReady);
        return this.Service.FindString(startPage, endPage, searchText);
      }
    }

    internal override void PerformToggle(string toggleId)
    {
      lock (this.m_syncObject)
      {
        if (!this.IsReadyForProcessingPostTasks)
          throw new InvalidOperationException(CommonStrings.ReportNotReady);
        this.Service.ToggleItem(toggleId);
      }
    }

    internal override int PerformBookmarkNavigation(string bookmarkId, out string uniqueName)
    {
      lock (this.m_syncObject)
      {
        if (!this.IsReadyForProcessingPostTasks)
          throw new InvalidOperationException(CommonStrings.ReportNotReady);
        return this.Service.NavigateBookmark(bookmarkId, out uniqueName);
      }
    }

    internal override int PerformDocumentMapNavigation(string documentMapId)
    {
      lock (this.m_syncObject)
      {
        if (!this.IsReadyForProcessingPostTasks)
          throw new InvalidOperationException(CommonStrings.ReportNotReady);
        return this.Service.NavigateDocumentMap(documentMapId);
      }
    }

    internal override Report PerformDrillthrough(string drillthroughId, out string reportPath)
    {
      lock (this.m_syncObject)
      {
        if (!this.IsReadyForProcessingPostTasks)
          throw new InvalidOperationException(CommonStrings.ReportNotReady);
        ExecutionInfo executionInfo = this.Service.LoadDrillthroughTarget(drillthroughId);
        this.Service.SetExecutionId(this.m_executionID);
        reportPath = executionInfo.ReportPath;
        return (Report) new ServerReport(this, executionInfo);
      }
    }

    internal override int PerformSort(
      string sortId,
      SortOrder sortDirection,
      bool clearSort,
      PageCountMode pageCountMode,
      out string uniqueName)
    {
      lock (this.m_syncObject)
      {
        if (!this.IsReadyForProcessingPostTasks)
          throw new InvalidOperationException(CommonStrings.ReportNotReady);
        ExecutionInfo executionInfo;
        int numPages;
        int num = this.Service.Sort(sortId, sortDirection, clearSort, pageCountMode, out uniqueName, out executionInfo, out numPages);
        this.Service.SetExecutionId(this.m_executionID);
        if (executionInfo == null)
          this.m_executionInfo.NumPages = numPages;
        else
          this.m_executionInfo = executionInfo;
        return num;
      }
    }

    internal void TouchSession()
    {
      lock (this.m_syncObject)
      {
        if (!this.IsReadyForConnection)
          return;
        this.Service.GetExecutionInfo();
      }
    }

    public override int GetTotalPages(out PageCountMode pageCountMode)
    {
      lock (this.m_syncObject)
      {
        if (this.m_executionInfo == null)
        {
          pageCountMode = PageCountMode.Estimate;
          return 0;
        }
        pageCountMode = this.PageCountMode;
        return this.m_executionInfo.NumPages;
      }
    }

    private PageCountMode PageCountMode
    {
      get
      {
        lock (this.m_syncObject)
          return this.m_executionInfo == null || this.m_executionInfo.NumPages == 0 ? PageCountMode.Estimate : this.m_executionInfo.PageCountMode;
      }
    }

    internal override bool HasDocMap
    {
      get
      {
        lock (this.m_syncObject)
          return this.m_executionInfo != null && this.m_executionInfo.HasDocumentMap;
      }
    }

    internal override DocumentMapNode GetDocumentMap(string rootLabel)
    {
      lock (this.m_syncObject)
      {
        if (!this.IsReadyForProcessingPostTasks)
          throw new InvalidOperationException(CommonStrings.ReportNotReady);
        return !this.m_executionInfo.HasDocumentMap ? (DocumentMapNode) null : this.Service.GetDocumentMap(rootLabel);
      }
    }

    internal override int AutoRefreshInterval
    {
      get
      {
        lock (this.m_syncObject)
        {
          this.EnsureExecutionSession();
          return this.m_executionInfo.AutoRefreshInterval;
        }
      }
    }

    public override RenderingExtension[] ListRenderingExtensions()
    {
      lock (this.m_syncObject)
      {
        if (this.m_renderingExtensions == null)
          this.m_renderingExtensions = this.Service.ListRenderingExtensions();
        return this.m_renderingExtensions;
      }
    }

    public override void Refresh()
    {
      lock (this.m_syncObject)
      {
        if (!this.IsReadyForConnection || this.m_executionID == null)
          return;
        this.m_executionInfo = this.Service.ResetExecution();
        this.OnChange(true);
      }
    }

    internal override bool IsReadyForConnection
    {
      get
      {
        lock (this.m_syncObject)
          return !string.IsNullOrEmpty(this.m_reportPath) || this.m_executionID != null;
      }
    }

    internal override bool IsPreparedReportReadyForRendering
    {
      get => !this.m_executionInfo.CredentialsRequired && !this.m_executionInfo.ParametersRequired;
    }

    private bool IsReadyForProcessingPostTasks
    {
      get
      {
        this.EnsureExecutionSession();
        return this.m_executionInfo.HasSnapshot && !this.m_executionInfo.NeedsProcessing;
      }
    }

    public ReportDataSourceInfoCollection GetDataSources() => this.GetDataSources(out bool _);

    public ReportDataSourceInfoCollection GetDataSources(out bool allCredentialsSet)
    {
      lock (this.m_syncObject)
      {
        this.EnsureExecutionSession();
        allCredentialsSet = !this.m_executionInfo.CredentialsRequired;
        return this.m_executionInfo.DataSourcePrompts;
      }
    }

    public string GetServerVersion()
    {
      lock (this.m_syncObject)
        return this.Service.GetServerVersion();
    }

    internal static bool RequiresConnection => true;

    internal string ReportUrlNoScript => (string) null;

    internal bool IsPrintCabSupported(ClientArchitecture arch)
    {
      lock (this.m_syncObject)
        return this.Service.IsPrintCabSupported(arch);
    }

    internal string GetPrintControlClsid(ClientArchitecture arch)
    {
      lock (this.m_syncObject)
        return this.Service.GetPrintControlClsid(arch);
    }

    internal void WritePrintCab(ClientArchitecture arch, Stream stream)
    {
      lock (this.m_syncObject)
        this.Service.WritePrintCab(arch, stream);
    }

    internal string CreateStyleSheetUrl(string styleSheetName)
    {
      return ReportServerStyleSheetOperation.CreateUrl(styleSheetName, this.GetServerVersion(), false);
    }

    internal string CreateStyleSheetImageUrl(string styleSheetImageName)
    {
      return ReportServerStyleSheetOperation.CreateUrl(styleSheetImageName, this.GetServerVersion(), true);
    }

    internal byte[] GetStyleSheet(string styleSheetName, bool isImage, out string mimeType)
    {
      return this.Service.GetStyleSheet(styleSheetName, isImage, out mimeType);
    }

    internal static string ClientPrintCabX86Name
    {
      [DebuggerStepThrough] get => "rsclientprint.cab";
    }

    private IReportExecutionService Service
    {
      get
      {
        if (this.m_service == null)
        {
          this.m_service = this.CreateExecutionService();
          this.ApplyExecutionIdToService(this.m_service);
        }
        return this.m_service;
      }
    }

    private void ApplyExecutionIdToService(IReportExecutionService service)
    {
      service?.SetExecutionId(this.m_executionID);
    }

    internal override void EnsureExecutionSession()
    {
      if (this.m_executionInfo != null)
        return;
      if (this.m_executionID == null)
      {
        string historyId = this.HistoryId;
        if (historyId != null && historyId.Length == 0)
          historyId = (string) null;
        string reportPath = this.ReportPath;
        if (reportPath == null || reportPath.Length == 0)
          throw new MissingReportSourceException();
        this.m_executionInfo = this.Service.LoadReport(reportPath, historyId);
        this.m_executionID = this.m_executionInfo.ExecutionID;
        this.OnExecutionIDChanged();
      }
      else
      {
        this.Service.SetExecutionId(this.m_executionID);
        this.m_executionInfo = this.Service.GetExecutionInfo();
      }
    }

    private void ClearSession() => this.ClearSession(true);

    private void ClearSession(bool doRefresh)
    {
      this.m_executionID = (string) null;
      this.OnExecutionIDChanged();
      this.m_executionInfo = (ExecutionInfo) null;
      this.m_hiddenParameters.Clear();
      if (doRefresh)
        this.Refresh();
      this.OnChange(false);
    }

    private void ClearServerSpecificInfo()
    {
      this.m_renderingExtensions = (RenderingExtension[]) null;
      this.ClearSession();
    }

    private int GetIndexForParameter(string parameterName)
    {
      int num = -1;
      for (int index = 0; index < this.m_executionInfo.Parameters.Count; ++index)
      {
        if (string.Compare(this.m_executionInfo.Parameters[index].Name, parameterName, StringComparison.OrdinalIgnoreCase) == 0)
        {
          num = index;
          break;
        }
      }
      // ISSUE: reference to a compiler-generated method
      return num != -1 || string.Compare(parameterName, "rs:StoredParametersID", StringComparison.OrdinalIgnoreCase) == 0 ? num : throw new ArgumentException(CommonStrings.ParameterNotFound(parameterName));
    }

    private IReportExecutionService CreateExecutionService()
    {
      return (IReportExecutionService) new SoapReportExecutionService(this.m_serverIdentity, this.m_serverUrl, this.m_serverCredentials, this.m_trustedUserHeader, (IEnumerable<string>) this.Headers, (IEnumerable<Cookie>) this.Cookies, this.m_timeOut);
    }
  }
}
