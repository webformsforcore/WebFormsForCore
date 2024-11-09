// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.ReportArea
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal class ReportArea : CompositeControl, IScriptControl
  {
    private ReportControl m_reportControl;
    private ErrorControl m_errorControl;
    private Panel m_nonReportContent;
    private ReportArea.ReportAreaUpdatePanel m_asyncPanel;
    private ReportAreaAsyncLoadTarget m_asyncReportLoad;
    private bool m_asyncWaitControlVisible;
    private HiddenField m_scrollPosition;
    private ReportAreaVisibilityState m_visibilityState;
    private bool m_isFullViewerRendering;
    private IReportViewerStyles m_styles;

    public ReportArea(IReportViewerStyles styles) => this.m_styles = styles;

    public void SetFullViewerRendering() => this.m_isFullViewerRendering = true;

    public event EventHandler<ReportActionEventArgs> ReportAction
    {
      add
      {
        this.EnsureChildControls();
        this.m_reportControl.ReportAction += value;
      }
      remove
      {
        this.EnsureChildControls();
        this.m_reportControl.ReportAction -= value;
      }
    }

    public event ZoomChangedEventHandler ZoomChanged
    {
      add
      {
        this.EnsureChildControls();
        this.m_reportControl.ZoomChanged += value;
      }
      remove
      {
        this.EnsureChildControls();
        this.m_reportControl.ZoomChanged -= value;
      }
    }

    public string ClientScrollPosition
    {
      get
      {
        this.EnsureChildControls();
        return this.m_scrollPosition.Value;
      }
    }

    public event EventHandler AsyncLoadRequested;

    public void Clear()
    {
      this.EnsureChildControls();
      this.m_reportControl.ClearReport();
      this.SetVisibleRegion(ReportAreaContent.None);
    }

    public void SetReportZoom(ZoomMode zoomMode, int zoomPercent)
    {
      this.EnsureChildControls();
      this.m_reportControl.SetZoom(zoomMode, zoomPercent);
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
      this.EnsureChildControls();
      int num = this.m_reportControl.RenderReport(session, viewerInstanceIdentifier, pageCountMode, pageNumber, interactivityMode, searchState, replacementRoot, hyperlinkTarget, scrollTarget, alertMessage, initialDeviceInfos, browserMode, sizeToContent);
      this.SetVisibleRegion(ReportAreaContent.ReportPage);
      return num;
    }

    public void SetForAsyncRendering()
    {
      this.SetVisibleRegion(ReportAreaContent.None);
      this.m_asyncWaitControlVisible = true;
    }

    public ReportAreaContent ReportAreaContent
    {
      get
      {
        this.EnsureChildControls();
        return this.m_visibilityState.NewClientState;
      }
    }

    protected override HtmlTextWriterTag TagKey => HtmlTextWriterTag.Div;

    public void SetException(Exception e, bool handled)
    {
      this.EnsureChildControls();
      this.Clear();
      if (handled)
      {
        this.m_errorControl.SetHandledException();
        this.SetVisibleRegion(ReportAreaContent.None);
      }
      else
      {
        this.m_errorControl.SetException(e);
        this.SetVisibleRegion(ReportAreaContent.Error);
      }
    }

    protected override void CreateChildControls()
    {
      this.Controls.Clear();
      this.m_asyncPanel = new ReportArea.ReportAreaUpdatePanel();
      this.m_asyncPanel.UpdateMode = UpdatePanelUpdateMode.Conditional;
      this.m_asyncPanel.ChildrenAsTriggers = false;
      this.m_asyncPanel.ID = nameof (ReportArea);
      this.m_asyncPanel.Rendering += new EventHandler(this.OnUpdatePanelRendering);
      this.Controls.Add((Control) this.m_asyncPanel);
      this.m_visibilityState = new ReportAreaVisibilityState(this);
      this.m_visibilityState.ID = "VisibilityState";
      this.m_asyncPanel.ContentTemplateContainer.Controls.Add((Control) this.m_visibilityState);
      this.m_scrollPosition = new HiddenField();
      this.m_scrollPosition.ID = "ScrollPosition";
      this.m_asyncPanel.ContentTemplateContainer.Controls.Add((Control) this.m_scrollPosition);
      this.m_asyncReportLoad = new ReportAreaAsyncLoadTarget();
      this.m_asyncReportLoad.ID = "Reserved_AsyncLoadTarget";
      this.m_asyncReportLoad.PostBackTarget += new EventHandler(this.OnAsyncReportLoad);
      this.m_asyncPanel.ContentTemplateContainer.Controls.Add((Control) this.m_asyncReportLoad);
      this.m_reportControl = new ReportControl();
      this.m_asyncPanel.ContentTemplateContainer.Controls.Add((Control) this.m_reportControl);
      this.m_nonReportContent = new Panel();
      this.m_nonReportContent.ID = "NonReportContent";
      this.m_nonReportContent.Width = Unit.Percentage(100.0);
      this.m_nonReportContent.Height = Unit.Percentage(100.0);
      this.m_asyncPanel.ContentTemplateContainer.Controls.Add((Control) this.m_nonReportContent);
      this.m_errorControl = new ErrorControl();
      this.m_errorControl.InheritFont = true;
      this.m_nonReportContent.Controls.Add((Control) this.m_errorControl);
    }

    protected override void OnPreRender(EventArgs e)
    {
      this.EnsureChildControls();
      ScriptManager.GetCurrent(this.Page)?.RegisterScriptControl<ReportArea>(this);
      this.m_scrollPosition.Value = "";
      this.m_reportControl.VisibleContainerId = this.VisibleReportContentContainerId;
      this.m_reportControl.ScrollContainerId = this.ClientID;
      if (this.m_asyncWaitControlVisible)
        this.m_asyncReportLoad.TriggerImmediatePostBack();
      base.OnPreRender(e);
    }

    protected override void RenderChildren(HtmlTextWriter writer)
    {
      this.EnsureChildControls();
      ScriptManager.GetCurrent(this.Page)?.RegisterScriptDescriptors((IScriptControl) this);
      writer.AddAttribute(HtmlTextWriterAttribute.Id, this.VisibleReportContentContainerId, true);
      writer.AddStyleAttribute(HtmlTextWriterStyle.Height, "100%");
      writer.AddStyleAttribute(HtmlTextWriterStyle.Display, "none");
      writer.RenderBeginTag(HtmlTextWriterTag.Div);
      writer.RenderEndTag();
      if (!this.m_styles.GetFontFromCss)
        this.m_errorControl.Font.CopyFrom(this.m_styles.Font);
      base.RenderChildren(writer);
    }

    private void OnUpdatePanelRendering(object sender, EventArgs e)
    {
      this.EnsureChildControls();
      ReportAreaContent currentClientState = this.m_visibilityState.CurrentClientState;
      ReportAreaContent newClientState = this.m_visibilityState.NewClientState;
      if (!this.m_isFullViewerRendering ? ReportArea.IsDisplayedInNonReportContentPanel(currentClientState) || currentClientState == ReportAreaContent.None : ReportArea.IsDisplayedInNonReportContentPanel(newClientState))
        return;
      this.m_nonReportContent.Style.Add(HtmlTextWriterStyle.Display, "none");
    }

    private void OnAsyncReportLoad(object sender, EventArgs e)
    {
      if (this.AsyncLoadRequested == null)
        return;
      this.AsyncLoadRequested((object) this, EventArgs.Empty);
    }

    internal static bool IsDisplayedInNonReportContentPanel(ReportAreaContent content)
    {
      return content == ReportAreaContent.Error;
    }

    private void SetVisibleRegion(ReportAreaContent content)
    {
      this.EnsureChildControls();
      this.m_visibilityState.NewClientState = content;
      if (this.m_isFullViewerRendering)
        return;
      this.m_asyncPanel.Update();
    }

    public IEnumerable<ScriptDescriptor> GetScriptDescriptors()
    {
      this.EnsureChildControls();
      ScriptControlDescriptor controlDescriptor = new ScriptControlDescriptor("Microsoft.Reporting.WebFormsClient._ReportArea", this.ClientID);
      controlDescriptor.AddProperty("VisibleReportContentContainerId", (object) this.VisibleReportContentContainerId);
      controlDescriptor.AddProperty("ReportControlId", (object) this.m_reportControl.ClientID);
      controlDescriptor.AddProperty("NonReportContentId", (object) this.m_nonReportContent.ClientID);
      controlDescriptor.AddProperty("ScrollPositionId", (object) this.m_scrollPosition.ClientID);
      controlDescriptor.AddProperty("ReportAreaVisibilityStateId", (object) this.m_visibilityState.ClientID);
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

    private string VisibleReportContentContainerId => "VisibleReportContent" + this.ClientID;

    private class ReportAreaUpdatePanel : UpdatePanel
    {
      public event EventHandler Rendering;

      protected override void Render(HtmlTextWriter writer)
      {
        if (this.Rendering != null)
          this.Rendering((object) this, EventArgs.Empty);
        base.Render(writer);
      }
    }
  }
}
