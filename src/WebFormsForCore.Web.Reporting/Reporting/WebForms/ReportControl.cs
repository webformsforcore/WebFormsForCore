
using Microsoft.ReportingServices.Rendering.HtmlRenderer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal sealed class ReportControl : CompositeControl, IPostBackEventHandler, IScriptControl
  {
    public const string AutoRefreshParam = "auto";
    private ReportControlSession m_session;
    private Stream m_reportStream;
    private string m_styleBytesString;
    private string m_viewerInstanceIdentifier;
    private int m_pageNumber;
    private SearchState m_searchState;
    private int m_autoRefreshInterval;
    private string m_alertMessage;
    private ScrollTarget m_scrollTarget;
    private InteractivityPostBackMode m_interactivityMode;
    private string m_visibleContainerId;
    private HiddenField m_hiddenActionType;
    private HiddenField m_hiddenActionParam;
    private HiddenField m_hiddenZoomLevel;
    private Timer m_autoRefreshTimer;
    private PostBackTarget m_syncPostBackTarget;
    private string m_scrollScript;
    private string m_scrollContainerId;
    private string m_pageStyles;
    private static DeviceInfoNameBlackList m_blackListDeviceInfoNames;

    public ReportControl()
    {
      this.Style.Add(HtmlTextWriterStyle.Display, "none");
      this.ID = nameof (ReportControl);
    }

    protected override HtmlTextWriterTag TagKey => HtmlTextWriterTag.Div;

    public string ScrollContainerId
    {
      get => this.m_scrollContainerId;
      set => this.m_scrollContainerId = value;
    }

    public override void Dispose()
    {
      this.ClearReport();
      base.Dispose();
    }

    public event EventHandler<ReportActionEventArgs> ReportAction;

    public event ZoomChangedEventHandler ZoomChanged;

    public string VisibleContainerId
    {
      get => this.m_visibleContainerId;
      set => this.m_visibleContainerId = value;
    }

    protected override void CreateChildControls()
    {
      this.Controls.Clear();
      base.CreateChildControls();
      this.m_syncPostBackTarget = new PostBackTarget();
      this.m_syncPostBackTarget.PostBackAsTarget += new EventHandler(this.OnReportAction);
      ScriptManager.GetCurrent(this.Page).RegisterPostBackControl((Control) this.m_syncPostBackTarget);
      this.Controls.Add((Control) this.m_syncPostBackTarget);
      this.m_autoRefreshTimer = new Timer();
      this.m_autoRefreshTimer.Tick += new EventHandler<EventArgs>(this.OnAutoRefresh);
      this.Controls.Add((Control) this.m_autoRefreshTimer);
      this.m_hiddenActionType = new HiddenField();
      this.Controls.Add((Control) this.m_hiddenActionType);
      this.m_hiddenActionParam = new HiddenField();
      this.Controls.Add((Control) this.m_hiddenActionParam);
      this.m_hiddenZoomLevel = new HiddenField();
      this.m_hiddenZoomLevel.ValueChanged += new EventHandler(this.OnZoomChanged);
      this.Controls.Add((Control) this.m_hiddenZoomLevel);
    }

    protected override void OnPreRender(EventArgs e)
    {
      this.EnsureChildControls();
      base.OnPreRender(e);
      ScriptManager.GetCurrent(this.Page)?.RegisterScriptControl<ReportControl>(this);
      this.m_hiddenActionType.Value = (string) null;
      this.m_hiddenActionParam.Value = (string) null;
      this.m_autoRefreshTimer.Enabled = this.m_reportStream != null && this.m_autoRefreshInterval > 0;
      if (!this.m_autoRefreshTimer.Enabled)
        return;
      this.m_autoRefreshTimer.Interval = this.m_autoRefreshInterval * 1000;
    }

    private void RemoveNulls(char[] chars, int count)
    {
      if (chars == null)
        return;
      for (int index = 0; index < chars.Length && index < count; ++index)
      {
        if (chars[index] == char.MinValue)
          chars[index] = ' ';
      }
    }

    protected override void RenderChildren(HtmlTextWriter writer)
    {
      ScriptManager current = ScriptManager.GetCurrent(this.Page);
      current?.RegisterScriptDescriptors((IScriptControl) this);
      this.m_syncPostBackTarget.RenderControl(writer);
      this.m_hiddenActionType.RenderControl(writer);
      this.m_hiddenActionParam.RenderControl(writer);
      this.m_hiddenZoomLevel.RenderControl(writer);
      if (this.m_autoRefreshTimer.Enabled)
        this.m_autoRefreshTimer.RenderControl(writer);
      if (this.m_reportStream == null)
        return;
      writer.AddStyleAttribute(HtmlTextWriterStyle.Display, "none");
      writer.RenderBeginTag(HtmlTextWriterTag.Div);
      int count = 40960;
      byte[] numArray = new byte[count];
      char[] chArray = new char[count];
      Decoder decoder = Encoding.UTF8.GetDecoder();
      int num = 0;
      bool flag = current != null && current.IsInAsyncPostBack;
      int byteCount;
      while ((byteCount = this.m_reportStream.Read(numArray, 0, count)) > 0)
      {
        int chars = decoder.GetChars(numArray, 0, byteCount, chArray, 0, false);
        if (flag)
          this.RemoveNulls(chArray, chars);
        writer.Write(chArray, 0, chars);
        num += byteCount;
        if (num >= count)
        {
          writer.Flush();
          num = 0;
        }
      }
      writer.RenderEndTag();
    }

    public void ClearReport()
    {
      if (this.m_reportStream == null)
        return;
      this.m_scrollScript = (string) null;
      this.m_reportStream.Dispose();
      this.m_reportStream = (Stream) null;
      this.m_pageStyles = (string) null;
    }

    public void SetZoom(ZoomMode zoomMode, int zoomPercent)
    {
      this.EnsureChildControls();
      this.m_hiddenZoomLevel.Value = Global.ZoomString(zoomMode, zoomPercent);
    }

    private void OnZoomChanged(object sender, EventArgs e)
    {
      if (this.ZoomChanged == null)
        return;
      ZoomChangeEventArgs e1 = (ZoomChangeEventArgs) null;
      string str = this.m_hiddenZoomLevel.Value;
      if (string.Equals(str, ZoomMode.FullPage.ToString(), StringComparison.Ordinal))
        e1 = new ZoomChangeEventArgs(ZoomMode.FullPage, 100);
      else if (string.Equals(str, ZoomMode.PageWidth.ToString(), StringComparison.Ordinal))
      {
        e1 = new ZoomChangeEventArgs(ZoomMode.PageWidth, 100);
      }
      else
      {
        int result;
        if (int.TryParse(str, out result) && result > 0)
          e1 = new ZoomChangeEventArgs(ZoomMode.Percent, result);
      }
      if (e1 == null)
        return;
      this.ZoomChanged((object) this, e1);
    }

    public int RenderReport(
      ReportControlSession session,
      string viewerInstanceIdentifier,
      PageCountMode pageCountMode,
      int pageNumber,
      InteractivityPostBackMode interactivityMode,
      SearchState searchState,
      string replacementRoot,
      string hyperlinkTarget,
      ScrollTarget scrollTarget,
      string alertMessage,
      DeviceInfoCollection initialDeviceInfos,
      string browserMode,
      bool sizeToContent)
    {
      if (this.m_reportStream != null)
        throw new InvalidOperationException();
      this.m_session = session;
      ++this.ViewIteration;
      this.m_pageNumber = pageNumber;
      this.m_searchState = searchState;
      this.m_viewerInstanceIdentifier = viewerInstanceIdentifier;
      this.m_alertMessage = alertMessage;
      this.m_scrollTarget = scrollTarget;
      this.m_interactivityMode = interactivityMode;
      bool useImageConsolidation = !sizeToContent;
      NameValueCollection deviceInfo = this.CreateDeviceInfo(initialDeviceInfos, session.Report, pageNumber, searchState, replacementRoot, hyperlinkTarget, browserMode, useImageConsolidation);
      this.m_reportStream = session.RenderReportHTML4(deviceInfo, pageCountMode, out this.m_scrollScript, out this.m_pageStyles);
      this.m_autoRefreshInterval = session.Report.AutoRefreshInterval;
      int totalPages = session.Report.GetTotalPages();
      if (this.m_pageNumber > totalPages)
        this.m_pageNumber = totalPages;
      this.m_autoRefreshInterval = session.Report.AutoRefreshInterval;
      if (sizeToContent)
        this.m_scrollScript = (string) null;
      string styleStreamName = LocalHtmlRenderer.GetStyleStreamName(pageNumber);
      byte[] streamImage = session.GetStreamImage(styleStreamName, (string) null, out string _);
      this.m_styleBytesString = (string) null;
      if (streamImage != null && streamImage.Length > 0)
        this.m_styleBytesString = new UTF8Encoding(false).GetString(streamImage);
      return this.m_pageNumber;
    }

    internal static DeviceInfoNameBlackList GetDeviceInfoBlackList()
    {
      if (ReportControl.m_blackListDeviceInfoNames == null)
      {
        ReportControl.m_blackListDeviceInfoNames = new DeviceInfoNameBlackList();
        ReportControl.m_blackListDeviceInfoNames.Add("HTMLFragment");
        ReportControl.m_blackListDeviceInfoNames.Add("Section", Errors.InvalidDeviceInfoSection);
        ReportControl.m_blackListDeviceInfoNames.Add("StreamRoot");
        ReportControl.m_blackListDeviceInfoNames.Add("ResourceStreamRoot");
        ReportControl.m_blackListDeviceInfoNames.Add("ActionScript");
        ReportControl.m_blackListDeviceInfoNames.Add("JavaScript");
        ReportControl.m_blackListDeviceInfoNames.Add("FindString", Errors.InvalidDeviceInfoFind);
        ReportControl.m_blackListDeviceInfoNames.Add("ReplacementRoot");
        ReportControl.m_blackListDeviceInfoNames.Add("PrefixId");
        ReportControl.m_blackListDeviceInfoNames.Add("StyleStream");
        ReportControl.m_blackListDeviceInfoNames.Add("LinkTarget", Errors.InvalidDeviceInfoLinkTarget);
        ReportControl.m_blackListDeviceInfoNames.Add("ExpandContent");
      }
      return ReportControl.m_blackListDeviceInfoNames;
    }

    private NameValueCollection CreateDeviceInfo(
      DeviceInfoCollection initialDeviceInfos,
      Report report,
      int pageNumber,
      SearchState searchState,
      string replacementRoot,
      string linkTarget,
      string browserMode,
      bool useImageConsolidation)
    {
      NameValueCollection deviceInfo = new NameValueCollection();
      foreach (DeviceInfo initialDeviceInfo in (Collection<DeviceInfo>) initialDeviceInfos)
        deviceInfo.Add(initialDeviceInfo.Name, initialDeviceInfo.Value);
      deviceInfo.Set("HTMLFragment", "true");
      deviceInfo.Set("Section", pageNumber.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      string url1 = ReportImageOperation.CreateUrl(report, this.m_viewerInstanceIdentifier, false);
      deviceInfo.Set("StreamRoot", url1);
      string url2 = ReportImageOperation.CreateUrl(report, this.m_viewerInstanceIdentifier, true);
      deviceInfo.Set("ResourceStreamRoot", url2);
      deviceInfo.Set("ActionScript", this.ActionScriptMethod);
      if (searchState != null)
        deviceInfo.Set("FindString", searchState.Text);
      if (!string.IsNullOrEmpty(replacementRoot))
        deviceInfo.Set("ReplacementRoot", replacementRoot);
      deviceInfo.Set("PrefixId", this.UniqueRenderingId);
      deviceInfo.Set("StyleStream", "true");
      if (!string.IsNullOrEmpty(linkTarget))
        deviceInfo.Set("LinkTarget", linkTarget);
      if (HttpContext.Current != null && HttpContext.Current.Request != null)
      {
        string userAgent = HttpContext.Current.Request.UserAgent;
        if (userAgent != null)
          deviceInfo.Set("UserAgent", userAgent);
      }
      if (!string.IsNullOrEmpty(browserMode) && deviceInfo["BrowserMode"] == null)
        deviceInfo.Set("BrowserMode", browserMode);
      if (!useImageConsolidation)
        deviceInfo.Set("ImageConsolidation", "false");
      return deviceInfo;
    }

    public void RaisePostBackEvent(string eventArgument)
    {
      this.OnReportAction((object) this, EventArgs.Empty);
    }

    private void OnReportAction(object sender, EventArgs e)
    {
      this.EnsureChildControls();
      this.OnReportAction(this.m_hiddenActionType.Value, this.m_hiddenActionParam.Value);
    }

    private void OnAutoRefresh(object sender, EventArgs e)
    {
      this.OnReportAction("Refresh", "auto");
    }

    private void OnReportAction(string actionType, string actionParam)
    {
      if (this.ReportAction == null)
        return;
      this.ReportAction((object) this, new ReportActionEventArgs(actionType, actionParam));
    }

    private string ActionScriptMethod
    {
      get
      {
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "var rp=$get('{0}');if(rp&&rp.control)rp.control.InvokeReportAction", (object) JavaScriptHelper.StringEscapeSingleQuote(this.ClientID));
      }
    }

    private int ViewIteration
    {
      get
      {
        object obj = this.ViewState[nameof (ViewIteration)];
        return obj == null ? 0 : (int) obj;
      }
      set => this.ViewState[nameof (ViewIteration)] = (object) value;
    }

    private string UniqueRenderingId
    {
      get
      {
        return ReportControl.MakeUniqueRenderingId(this.m_viewerInstanceIdentifier, this.ViewIteration);
      }
    }

    private static string MakeUniqueRenderingId(string instanceId, int viewiteration)
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "P{0}_{1}_", (object) instanceId, (object) viewiteration);
    }

    public IEnumerable<ScriptDescriptor> GetScriptDescriptors()
    {
      ScriptControlDescriptor controlDescriptor = new ScriptControlDescriptor("Microsoft.Reporting.WebFormsClient._ReportPage", this.ClientID);
      if (this.m_reportStream != null)
      {
        string uniqueRenderingId = this.UniqueRenderingId;
        controlDescriptor.AddProperty("SearchHitPrefix", (object) (uniqueRenderingId + HTML4Renderer.m_searchHitIdPrefix));
        controlDescriptor.AddProperty("ReportCellId", (object) (uniqueRenderingId + "oReportCell"));
        controlDescriptor.AddProperty("ReportDivId", (object) (uniqueRenderingId + "oReportDiv"));
        controlDescriptor.AddProperty("ScrollableContainerId", (object) this.ScrollContainerId);
        if (this.m_alertMessage != null)
          controlDescriptor.AddProperty("LoadMessage", (object) this.m_alertMessage);
        if (this.m_scrollTarget != null)
        {
          string navigationId = this.m_scrollTarget.NavigationId;
          if (!string.IsNullOrEmpty(navigationId))
            controlDescriptor.AddProperty("NavigationId", (object) (uniqueRenderingId + navigationId));
          if (this.m_scrollTarget.ScrollStyle == ActionScrollStyle.MaintainPosition)
          {
            string str = ReportControl.MakeUniqueRenderingId(this.m_viewerInstanceIdentifier, this.ViewIteration - 1);
            controlDescriptor.AddProperty("PreviousViewNavigationAlignmentId", (object) (str + navigationId));
          }
          controlDescriptor.AddProperty("AvoidScrollChange", (object) (this.m_scrollTarget.ScrollStyle == ActionScrollStyle.AvoidScrolling));
          controlDescriptor.AddProperty("AvoidScrollFromOrigin", (object) (this.m_scrollTarget.ScrollStyle == ActionScrollStyle.AvoidScrollingFromOrigin));
          string pixelPosition = this.m_scrollTarget.PixelPosition;
          if (!string.IsNullOrEmpty(pixelPosition))
            controlDescriptor.AddProperty("SpecificScrollPosition", (object) pixelPosition);
        }
        controlDescriptor.AddProperty("ReportStyles", (object) this.m_styleBytesString);
        controlDescriptor.AddProperty("PrefixId", (object) uniqueRenderingId);
        if (this.m_scrollScript != null)
          controlDescriptor.AddScriptProperty("ScrollScript", this.m_scrollScript);
        if (this.m_pageStyles != null)
          controlDescriptor.AddProperty("ReportPageStyles", (object) this.m_pageStyles);
        controlDescriptor.AddProperty("InteractivityMode", (object) this.m_interactivityMode.ToString());
        controlDescriptor.AddProperty("ActionTypeId", (object) this.m_hiddenActionType.ClientID);
        controlDescriptor.AddProperty("ActionParamId", (object) this.m_hiddenActionParam.ClientID);
        string backEventReference1 = this.Page.ClientScript.GetPostBackEventReference((Control) this.m_syncPostBackTarget, (string) null);
        controlDescriptor.AddScriptProperty("TriggerSyncInteractivity", JavaScriptHelper.FormatAsFunction(backEventReference1 + ";"));
        string backEventReference2 = this.Page.ClientScript.GetPostBackEventReference((Control) this, (string) null);
        controlDescriptor.AddScriptProperty("TriggerAsyncInteractivity", JavaScriptHelper.FormatAsFunction(backEventReference2 + ";"));
        string updateProperties = ToolbarControl.GenerateUpdateProperties(this.m_session, this.m_pageNumber, this.m_searchState);
        controlDescriptor.AddScriptProperty("ToolBarUpdate", updateProperties);
      }
      controlDescriptor.AddProperty("HiddenZoomLevelId", (object) this.m_hiddenZoomLevel.ClientID);
      controlDescriptor.AddProperty("StyleElementId", (object) (this.ClientID + "_styles"));
      return (IEnumerable<ScriptDescriptor>) new ScriptDescriptor[1]
      {
        (ScriptDescriptor) controlDescriptor
      };
    }

    public IEnumerable<ScriptReference> GetScriptReferences()
    {
      ScriptReference scriptReference = new ScriptReference();
      scriptReference.Path = EmbeddedResourceOperation.CreateUrlForScriptFile();
      return (IEnumerable<ScriptReference>) new ScriptReference[1]
      {
        scriptReference
      };
    }
  }
}
