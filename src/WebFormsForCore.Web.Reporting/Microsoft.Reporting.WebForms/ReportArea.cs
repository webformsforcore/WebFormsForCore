using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Microsoft.Reporting.WebForms;

internal class ReportArea : CompositeControl, IScriptControl
{
	private class ReportAreaUpdatePanel : UpdatePanel
	{
		public event EventHandler Rendering;

		protected override void Render(HtmlTextWriter writer)
		{
			if (this.Rendering != null)
			{
				this.Rendering(this, EventArgs.Empty);
			}
			base.Render(writer);
		}
	}

	private ReportControl m_reportControl;

	private ErrorControl m_errorControl;

	private Panel m_nonReportContent;

	private ReportAreaUpdatePanel m_asyncPanel;

	private ReportAreaAsyncLoadTarget m_asyncReportLoad;

	private bool m_asyncWaitControlVisible;

	private HiddenField m_scrollPosition;

	private ReportAreaVisibilityState m_visibilityState;

	private bool m_isFullViewerRendering;

	private IReportViewerStyles m_styles;

	public string ClientScrollPosition
	{
		get
		{
			EnsureChildControls();
			return m_scrollPosition.Value;
		}
	}

	public ReportAreaContent ReportAreaContent
	{
		get
		{
			EnsureChildControls();
			return m_visibilityState.NewClientState;
		}
	}

	protected override HtmlTextWriterTag TagKey => HtmlTextWriterTag.Div;

	private string VisibleReportContentContainerId => "VisibleReportContent" + ClientID;

	public event EventHandler<ReportActionEventArgs> ReportAction
	{
		add
		{
			EnsureChildControls();
			m_reportControl.ReportAction += value;
		}
		remove
		{
			EnsureChildControls();
			m_reportControl.ReportAction -= value;
		}
	}

	public event ZoomChangedEventHandler ZoomChanged
	{
		add
		{
			EnsureChildControls();
			m_reportControl.ZoomChanged += value;
		}
		remove
		{
			EnsureChildControls();
			m_reportControl.ZoomChanged -= value;
		}
	}

	public event EventHandler AsyncLoadRequested;

	public ReportArea(IReportViewerStyles styles)
	{
		m_styles = styles;
	}

	public void SetFullViewerRendering()
	{
		m_isFullViewerRendering = true;
	}

	public void Clear()
	{
		EnsureChildControls();
		m_reportControl.ClearReport();
		SetVisibleRegion(ReportAreaContent.None);
	}

	public void SetReportZoom(ZoomMode zoomMode, int zoomPercent)
	{
		EnsureChildControls();
		m_reportControl.SetZoom(zoomMode, zoomPercent);
	}

	public int RenderReport(ReportControlSession session, string viewerInstanceIdentifier, PageCountMode pageCountMode, int pageNumber, InteractivityPostBackMode interactivityMode, SearchState searchState, string replacementRoot, string hyperlinkTarget, ScrollTarget scrollTarget, string alertMessage, DeviceInfoCollection initialDeviceInfos, string browserMode, bool sizeToContent)
	{
		EnsureChildControls();
		int result = m_reportControl.RenderReport(session, viewerInstanceIdentifier, pageCountMode, pageNumber, interactivityMode, searchState, replacementRoot, hyperlinkTarget, scrollTarget, alertMessage, initialDeviceInfos, browserMode, sizeToContent);
		SetVisibleRegion(ReportAreaContent.ReportPage);
		return result;
	}

	public void SetForAsyncRendering()
	{
		SetVisibleRegion(ReportAreaContent.None);
		m_asyncWaitControlVisible = true;
	}

	public void SetException(Exception e, bool handled)
	{
		EnsureChildControls();
		Clear();
		if (handled)
		{
			m_errorControl.SetHandledException();
			SetVisibleRegion(ReportAreaContent.None);
		}
		else
		{
			m_errorControl.SetException(e);
			SetVisibleRegion(ReportAreaContent.Error);
		}
	}

	protected override void CreateChildControls()
	{
		Controls.Clear();
		m_asyncPanel = new ReportAreaUpdatePanel();
		m_asyncPanel.UpdateMode = UpdatePanelUpdateMode.Conditional;
		m_asyncPanel.ChildrenAsTriggers = false;
		m_asyncPanel.ID = "ReportArea";
		m_asyncPanel.Rendering += OnUpdatePanelRendering;
		Controls.Add(m_asyncPanel);
		m_visibilityState = new ReportAreaVisibilityState(this);
		m_visibilityState.ID = "VisibilityState";
		m_asyncPanel.ContentTemplateContainer.Controls.Add(m_visibilityState);
		m_scrollPosition = new HiddenField();
		m_scrollPosition.ID = "ScrollPosition";
		m_asyncPanel.ContentTemplateContainer.Controls.Add(m_scrollPosition);
		m_asyncReportLoad = new ReportAreaAsyncLoadTarget();
		m_asyncReportLoad.ID = "Reserved_AsyncLoadTarget";
		m_asyncReportLoad.PostBackTarget += OnAsyncReportLoad;
		m_asyncPanel.ContentTemplateContainer.Controls.Add(m_asyncReportLoad);
		m_reportControl = new ReportControl();
		m_asyncPanel.ContentTemplateContainer.Controls.Add(m_reportControl);
		m_nonReportContent = new Panel();
		m_nonReportContent.ID = "NonReportContent";
		m_nonReportContent.Width = Unit.Percentage(100.0);
		m_nonReportContent.Height = Unit.Percentage(100.0);
		m_asyncPanel.ContentTemplateContainer.Controls.Add(m_nonReportContent);
		m_errorControl = new ErrorControl();
		m_errorControl.InheritFont = true;
		m_nonReportContent.Controls.Add(m_errorControl);
	}

	protected override void OnPreRender(EventArgs e)
	{
		EnsureChildControls();
		ScriptManager.GetCurrent(Page)?.RegisterScriptControl(this);
		m_scrollPosition.Value = "";
		m_reportControl.VisibleContainerId = VisibleReportContentContainerId;
		m_reportControl.ScrollContainerId = ClientID;
		if (m_asyncWaitControlVisible)
		{
			m_asyncReportLoad.TriggerImmediatePostBack();
		}
		base.OnPreRender(e);
	}

	protected override void RenderChildren(HtmlTextWriter writer)
	{
		EnsureChildControls();
		ScriptManager.GetCurrent(Page)?.RegisterScriptDescriptors(this);
		writer.AddAttribute(HtmlTextWriterAttribute.Id, VisibleReportContentContainerId, fEncode: true);
		writer.AddStyleAttribute(HtmlTextWriterStyle.Height, "100%");
		writer.AddStyleAttribute(HtmlTextWriterStyle.Display, "none");
		writer.RenderBeginTag(HtmlTextWriterTag.Div);
		writer.RenderEndTag();
		if (!m_styles.GetFontFromCss)
		{
			m_errorControl.Font.CopyFrom(m_styles.Font);
		}
		base.RenderChildren(writer);
	}

	private void OnUpdatePanelRendering(object sender, EventArgs e)
	{
		EnsureChildControls();
		ReportAreaContent currentClientState = m_visibilityState.CurrentClientState;
		ReportAreaContent newClientState = m_visibilityState.NewClientState;
		if (!((!m_isFullViewerRendering) ? (IsDisplayedInNonReportContentPanel(currentClientState) || currentClientState == ReportAreaContent.None) : IsDisplayedInNonReportContentPanel(newClientState)))
		{
			m_nonReportContent.Style.Add(HtmlTextWriterStyle.Display, "none");
		}
	}

	private void OnAsyncReportLoad(object sender, EventArgs e)
	{
		if (this.AsyncLoadRequested != null)
		{
			this.AsyncLoadRequested(this, EventArgs.Empty);
		}
	}

	internal static bool IsDisplayedInNonReportContentPanel(ReportAreaContent content)
	{
		return content == ReportAreaContent.Error;
	}

	private void SetVisibleRegion(ReportAreaContent content)
	{
		EnsureChildControls();
		m_visibilityState.NewClientState = content;
		if (!m_isFullViewerRendering)
		{
			m_asyncPanel.Update();
		}
	}

	public IEnumerable<ScriptDescriptor> GetScriptDescriptors()
	{
		EnsureChildControls();
		ScriptControlDescriptor scriptControlDescriptor = new ScriptControlDescriptor("Microsoft.Reporting.WebFormsClient._ReportArea", ClientID);
		scriptControlDescriptor.AddProperty("VisibleReportContentContainerId", VisibleReportContentContainerId);
		scriptControlDescriptor.AddProperty("ReportControlId", m_reportControl.ClientID);
		scriptControlDescriptor.AddProperty("NonReportContentId", m_nonReportContent.ClientID);
		scriptControlDescriptor.AddProperty("ScrollPositionId", m_scrollPosition.ClientID);
		scriptControlDescriptor.AddProperty("ReportAreaVisibilityStateId", m_visibilityState.ClientID);
		return new ScriptDescriptor[1] { scriptControlDescriptor };
	}

	public IEnumerable<ScriptReference> GetScriptReferences()
	{
		ScriptReference scriptReference = new ScriptReference();
		scriptReference.Path = EmbeddedResourceOperation.CreateUrlForScriptFile();
		return new ScriptReference[1] { scriptReference };
	}
}
