
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  [PersistChildren(false)]
  [TypeConverter(typeof (TypeNameHidingExpandableObjectConverter))]
  [ParseChildren(true)]
  public abstract class Report
  {
    private string m_displayName = "";
    private int m_drillthroughDepth = 1;
    internal object m_syncObject = new object();

    internal Report()
    {
    }

    [WebBrowsable(true)]
    [NotifyParentProperty(true)]
    [DefaultValue("")]
    [Microsoft.Reporting.SRDescription("DisplayNameDesc")]
    public string DisplayName
    {
      get => this.m_displayName;
      set => this.m_displayName = value;
    }

    internal abstract string DisplayNameForUse { get; }

    public abstract ReportParameterInfoCollection GetParameters();

    public abstract void SetParameters(IEnumerable<ReportParameter> parameters);

    public abstract int GetTotalPages(out PageCountMode pageCountMode);

    public abstract RenderingExtension[] ListRenderingExtensions();

    public abstract void LoadReportDefinition(TextReader report);

    public abstract void Refresh();

    public abstract byte[] Render(
      string format,
      string deviceInfo,
      PageCountMode pageCountMode,
      out string mimeType,
      out string encoding,
      out string fileNameExtension,
      out string[] streams,
      out Warning[] warnings);

    internal abstract byte[] InternalRenderStream(
      string format,
      string streamID,
      string deviceInfo,
      out string mimeType,
      out string encoding);

    internal abstract int PerformSearch(string searchText, int startPage, int endPage);

    internal abstract void PerformToggle(string toggleId);

    internal abstract int PerformBookmarkNavigation(string bookmarkId, out string uniqueName);

    internal abstract int PerformDocumentMapNavigation(string documentMapId);

    public abstract ReportPageSettings GetDefaultPageSettings();

    public int GetTotalPages() => this.GetTotalPages(out PageCountMode _);

    public byte[] Render(string format) => this.Render(format, (string) null);

    public byte[] Render(string format, string deviceInfo)
    {
      return this.Render(format, deviceInfo, out string _, out string _, out string _, out string[] _, out Warning[] _);
    }

    public byte[] Render(
      string format,
      string deviceInfo,
      out string mimeType,
      out string encoding,
      out string fileNameExtension,
      out string[] streams,
      out Warning[] warnings)
    {
      return this.Render(format, deviceInfo, PageCountMode.Estimate, out mimeType, out encoding, out fileNameExtension, out streams, out warnings);
    }

    public DocumentMapNode GetDocumentMap() => this.GetDocumentMap(this.DisplayNameForUse);

    [Browsable(false)]
    public bool IsDrillthroughReport => this.DrillthroughDepth > 1;

    internal int DrillthroughDepth
    {
      get => this.m_drillthroughDepth;
      set => this.m_drillthroughDepth = value;
    }

    internal abstract Report PerformDrillthrough(string drillthroughId, out string reportPath);

    internal abstract int PerformSort(
      string sortId,
      SortOrder sortDirection,
      bool clearSort,
      PageCountMode pageCountMode,
      out string uniqueName);

    internal abstract bool IsReadyForConnection { get; }

    [Browsable(false)]
    public bool IsReadyForRendering
    {
      get
      {
        try
        {
          return this.PrepareForRender();
        }
        catch
        {
          return false;
        }
      }
    }

    internal abstract bool IsPreparedReportReadyForRendering { get; }

    internal bool PrepareForRender()
    {
      lock (this.m_syncObject)
      {
        if (!this.IsReadyForConnection)
          return false;
        this.EnsureExecutionSession();
        return this.IsPreparedReportReadyForRendering;
      }
    }

    internal abstract void EnsureExecutionSession();

    internal abstract bool HasDocMap { get; }

    internal event EventHandler<ReportChangedEventArgs> Change;

    internal abstract DocumentMapNode GetDocumentMap(string rootLabel);

    internal abstract int AutoRefreshInterval { get; }

    internal abstract bool CanSelfCancel { get; }

    internal abstract void SetCancelState(bool shouldCancel);

    public void LoadReportDefinition(Stream report)
    {
      if (report == null)
        throw new ArgumentNullException(nameof (report));
      this.LoadReportDefinition((TextReader) new StreamReader(report));
    }

    internal void OnChange(bool isRefreshOnly)
    {
      if (this.Change == null)
        return;
      this.Change((object) this, new ReportChangedEventArgs(isRefreshOnly));
    }

    internal void OnChange(object sender, EventArgs e) => this.OnChange(false);

    internal virtual string CreatePrintRequestQuery(string InstanceID)
    {
      return PrintRequestOperation.CreateQuery(this, InstanceID);
    }

    internal virtual string PrintRequestPath => ReportViewerFactory.HttpHandler.HandlerUri.Path;

    public void SetParameters(ReportParameter parameter)
    {
      this.SetParameters((IEnumerable<ReportParameter>) new ReportParameter[1]
      {
        parameter
      });
    }
  }
}
