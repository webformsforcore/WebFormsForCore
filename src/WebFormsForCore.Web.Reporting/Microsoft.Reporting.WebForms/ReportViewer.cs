using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.Net;
using System.Reflection;
using System.Security.Permissions;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.Diagnostics.Utilities;

namespace Microsoft.Reporting.WebForms;

[SRDescription("ReportViewerDescription")]
[Designer("Microsoft.Reporting.WebForms.ReportViewerDesigner, Microsoft.ReportViewer.WebDesign, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91")]
[ParseChildren(true)]
[PersistChildren(false)]
public class ReportViewer : CompositeControl, IScriptControl
{
	public const int MaximumPageCount = int.MaxValue;

	private static readonly HashSet<string> IOSRenderingExtensionBlackList;

	private AsyncWaitControl m_asyncWaitControl;

	private bool m_spinnyShouldSkipTimer;

	private UpdatePanel m_topLevelUpdatePanel;

	private UpdatePanel m_docMapUpdatePanel;

	private PanelUpdater m_panelUpdater = new PanelUpdater();

	private ParametersArea m_parametersArea;

	private ReportSplitter m_parametersAreaSplitter;

	private ReportArea m_reportArea;

	private DocMapArea m_docMapArea;

	private ReportSplitter m_docMapAreaSplitter;

	private ToolbarControl m_toolbarArea;

	private NoScriptControl m_noScriptControl;

	private ReportViewerClientScript m_clientScript;

	private HiddenField m_direction;

	private HiddenField m_browserMode;

	private HttpHandlerMissingErrorMessage m_httpHandlerMissingError;

	private EventHandler<ReportChangedEventArgs> m_changeHandler;

	private InitializeDataSourcesEventHandler m_dataInitializationHandler;

	private bool m_viewStateSaved;

	private ReportChangeType m_reportHasChanged;

	private bool m_userParamsChanged;

	private bool m_viewReportClicked;

	private bool m_lockChanges;

	private bool m_hasErrorsOnThisPostBack;

	private bool m_aspSessionFailedToLoad;

	private ReportAreaContent m_contentType = ReportAreaContent.Unknown;

	private string m_originalWaitMessageFont;

	private string m_alertMessage;

	private ReportHierarchy m_reportHierarchy;

	private DeviceInfoCollection m_interactiveDeviceInfos;

	private Guid m_instanceIdentifier = Guid.NewGuid();

	protected override HtmlTextWriterTag TagKey => HtmlTextWriterTag.Div;

	[WebBrowsable(true)]
	[DefaultValue(false)]
	[SRDescription("DocMapCollapsedDesc")]
	[Category("Appearance")]
	public bool DocumentMapCollapsed
	{
		get
		{
			return GetViewStateProperty("DocumentMapCollapsed", defaultValue: false);
		}
		set
		{
			SetDocumentMapCollapsedInternal(value, redrawClient: true);
		}
	}

	[Category("Appearance")]
	[DefaultValue(true)]
	[SRDescription("ShowToolBarDesc")]
	[WebBrowsable(true)]
	public bool ShowToolBar
	{
		get
		{
			return GetViewStateProperty("ShowToolbar", defaultValue: true);
		}
		set
		{
			SetUnlockedViewStateProperty("ShowToolbar", ShowToolBar, value, UpdateGroup.ExecutionSession);
		}
	}

	[WebBrowsable(true)]
	[Category("Appearance")]
	[DefaultValue(true)]
	[SRDescription("ShowParameterPromptsDesc")]
	public bool ShowParameterPrompts
	{
		get
		{
			return GetViewStateProperty("ShowParameters", defaultValue: true);
		}
		set
		{
			SetUnlockedViewStateProperty("ShowParameters", ShowParameterPrompts, value, UpdateGroup.ExecutionSession);
		}
	}

	[WebBrowsable(true)]
	[Category("Appearance")]
	[DefaultValue(true)]
	[SRDescription("ShowCredentialPromptsDesc")]
	public bool ShowCredentialPrompts
	{
		get
		{
			return GetViewStateProperty("ShowCredentialsArea", defaultValue: true);
		}
		set
		{
			SetUnlockedViewStateProperty("ShowCredentialsArea", ShowCredentialPrompts, value, UpdateGroup.ExecutionSession);
		}
	}

	[SRDescription("PromptAreaCollapsedDesc")]
	[Category("Appearance")]
	[DefaultValue(false)]
	[WebBrowsable(true)]
	public bool PromptAreaCollapsed
	{
		get
		{
			return GetViewStateProperty("PromptAreaCollapsed", defaultValue: false);
		}
		set
		{
			SetPromptAreaCollapsedInternal(value, redrawClient: true);
		}
	}

	[Category("Appearance")]
	[DefaultValue(true)]
	[WebBrowsable(true)]
	[SRDescription("ShowReportBodyDesc")]
	public bool ShowReportBody
	{
		get
		{
			return GetViewStateProperty("ShowReportBody", defaultValue: true);
		}
		set
		{
			if (SetUnlockedViewStateProperty("ShowReportBody", ShowReportBody, value, UpdateGroup.ExecutionSession) && value && RenderingState == ReportRenderingState.NotReady)
			{
				RenderingState = ReportRenderingState.Preparing;
			}
		}
	}

	[WebBrowsable(true)]
	[SRDescription("DocMapWidthDesc")]
	[Category("Appearance")]
	[DefaultValue(typeof(Unit), "25%")]
	public Unit DocumentMapWidth
	{
		get
		{
			return GetViewStateProperty("DocumentMapWidth", Unit.Percentage(25.0));
		}
		set
		{
			SetUnlockedViewStateProperty("DocumentMapWidth", DocumentMapWidth, value, UpdateGroup.ExecutionSession);
		}
	}

	[SRDescription("ShowDocumentMapButtonDesc")]
	[WebBrowsable(true)]
	[DefaultValue(true)]
	public bool ShowDocumentMapButton
	{
		get
		{
			return GetViewStateProperty("ShowDocumentMapButton", defaultValue: true);
		}
		set
		{
			SetUnlockedViewStateProperty("ShowDocumentMapButton", ShowDocumentMapButton, value, UpdateGroup.ExecutionSession);
		}
	}

	[DefaultValue(true)]
	[SRDescription("ShowPromptAreaButtonDesc")]
	[WebBrowsable(true)]
	public bool ShowPromptAreaButton
	{
		get
		{
			return GetViewStateProperty("ShowPromptAreaButton", defaultValue: true);
		}
		set
		{
			SetUnlockedViewStateProperty("ShowPromptAreaButton", ShowPromptAreaButton, value, UpdateGroup.ExecutionSession);
		}
	}

	[WebBrowsable(true)]
	[DefaultValue(true)]
	[SRDescription("ShowPageNavigationDesc")]
	[SRCategory("ToolBarCategoryDesc")]
	public bool ShowPageNavigationControls
	{
		get
		{
			return GetViewStateProperty("ShowPageNavigationControls", defaultValue: true);
		}
		set
		{
			SetUnlockedViewStateProperty("ShowPageNavigationControls", ShowPageNavigationControls, value, UpdateGroup.ExecutionSession);
		}
	}

	[DefaultValue(true)]
	[SRDescription("ShowBackButtonDesc")]
	[WebBrowsable(true)]
	[SRCategory("ToolBarCategoryDesc")]
	public bool ShowBackButton
	{
		get
		{
			return GetViewStateProperty("ShowBackButton", defaultValue: true);
		}
		set
		{
			SetUnlockedViewStateProperty("ShowBackButton", ShowBackButton, value, UpdateGroup.ExecutionSession);
		}
	}

	[SRDescription("ShowRefreshButtonDesc")]
	[SRCategory("ToolBarCategoryDesc")]
	[DefaultValue(true)]
	[WebBrowsable(true)]
	public bool ShowRefreshButton
	{
		get
		{
			return GetViewStateProperty("ShowRefreshButton", defaultValue: true);
		}
		set
		{
			SetUnlockedViewStateProperty("ShowRefreshButton", ShowRefreshButton, value, UpdateGroup.ExecutionSession);
		}
	}

	[SRDescription("ShowPrintButtonDesc")]
	[DefaultValue(true)]
	[SRCategory("ToolBarCategoryDesc")]
	[WebBrowsable(true)]
	public bool ShowPrintButton
	{
		get
		{
			return GetViewStateProperty("ShowPrintButton", defaultValue: true);
		}
		set
		{
			SetUnlockedViewStateProperty("ShowPrintButton", ShowPrintButton, value, UpdateGroup.ExecutionSession);
		}
	}

	internal bool ShowAtomDataFeedButton
	{
		get
		{
			object obj = ViewState["ShowAtomDataFeedButton"];
			if (obj == null)
			{
				return false;
			}
			return (bool)obj;
		}
		set
		{
			ViewState["ShowAtomDataFeedButton"] = value;
		}
	}

	[DefaultValue(true)]
	[SRDescription("ShowExportButtonDesc")]
	[WebBrowsable(true)]
	[SRCategory("ToolBarCategoryDesc")]
	public bool ShowExportControls
	{
		get
		{
			return GetViewStateProperty("ShowExportControls", defaultValue: true);
		}
		set
		{
			SetUnlockedViewStateProperty("ShowExportControls", ShowExportControls, value, UpdateGroup.ExecutionSession);
		}
	}

	[DefaultValue(true)]
	[SRDescription("ShowZoomButtonDesc")]
	[WebBrowsable(true)]
	[SRCategory("ToolBarCategoryDesc")]
	public bool ShowZoomControl
	{
		get
		{
			return GetViewStateProperty("ShowZoomControls", defaultValue: true);
		}
		set
		{
			SetUnlockedViewStateProperty("ShowZoomControls", ShowZoomControl, value, UpdateGroup.ExecutionSession);
		}
	}

	[SRCategory("ToolBarCategoryDesc")]
	[DefaultValue(true)]
	[WebBrowsable(true)]
	[SRDescription("ShowFindButtonDesc")]
	public bool ShowFindControls
	{
		get
		{
			return GetViewStateProperty("ShowFindControls", defaultValue: true);
		}
		set
		{
			SetUnlockedViewStateProperty("ShowFindControls", ShowFindControls, value, UpdateGroup.ExecutionSession);
		}
	}

	[DefaultValue(typeof(Color), "#ECE9D8")]
	public override Color BackColor
	{
		get
		{
			return base.BackColor;
		}
		set
		{
			base.BackColor = value;
		}
	}

	[NotifyParentProperty(true)]
	[SRDescription("WaitMessageFontDesc")]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	[Category("Appearance")]
	public FontInfo WaitMessageFont => ((ReportViewerStyle)base.ControlStyle).WaitMessageFont;

	[DefaultValue(BorderStyle.Solid)]
	[WebBrowsable(true)]
	[SRDescription("InternalBorderStyleDesc")]
	[Category("Appearance")]
	public BorderStyle InternalBorderStyle
	{
		get
		{
			return ((ReportViewerStyle)base.ControlStyle).InternalBorderStyle;
		}
		set
		{
			if (InternalBorderStyle != value)
			{
				EnsureUnlocked();
				((ReportViewerStyle)base.ControlStyle).InternalBorderStyle = value;
				m_panelUpdater.MarkPanelsForUpdate(UpdateGroup.ExecutionSession);
			}
		}
	}

	[Category("Appearance")]
	[WebBrowsable(true)]
	[DefaultValue(typeof(Color), "#CCCCCC")]
	[SRDescription("InternalBorderColorDesc")]
	public Color InternalBorderColor
	{
		get
		{
			return ((ReportViewerStyle)base.ControlStyle).InternalBorderColor;
		}
		set
		{
			if (InternalBorderColor != value)
			{
				EnsureUnlocked();
				((ReportViewerStyle)base.ControlStyle).InternalBorderColor = value;
				m_panelUpdater.MarkPanelsForUpdate(UpdateGroup.ExecutionSession);
			}
		}
	}

	[DefaultValue(typeof(Unit), "1px")]
	[SRDescription("InternalBorderWidthDesc")]
	[Category("Appearance")]
	[WebBrowsable(true)]
	public Unit InternalBorderWidth
	{
		get
		{
			return ((ReportViewerStyle)base.ControlStyle).InternalBorderWidth;
		}
		set
		{
			if (InternalBorderWidth != value)
			{
				EnsureUnlocked();
				((ReportViewerStyle)base.ControlStyle).InternalBorderWidth = value;
				m_panelUpdater.MarkPanelsForUpdate(UpdateGroup.ExecutionSession);
			}
		}
	}

	[SRDescription("ToolBarItemBorderStyleDesc")]
	[Category("Appearance")]
	[WebBrowsable(true)]
	[DefaultValue(BorderStyle.Solid)]
	public BorderStyle ToolBarItemBorderStyle
	{
		get
		{
			return ((ReportViewerStyle)base.ControlStyle).ToolbarItemBorderStyle;
		}
		set
		{
			if (BorderStyle != value)
			{
				EnsureUnlocked();
				((ReportViewerStyle)base.ControlStyle).ToolbarItemBorderStyle = value;
				m_panelUpdater.MarkPanelsForUpdate(UpdateGroup.ExecutionSession);
			}
		}
	}

	[DefaultValue(typeof(Color), "#336699")]
	[WebBrowsable(true)]
	[SRDescription("ToolBarItemBorderColorDesc")]
	[Category("Appearance")]
	public Color ToolBarItemBorderColor
	{
		get
		{
			return ((ReportViewerStyle)base.ControlStyle).ToolbarItemBorderColor;
		}
		set
		{
			if (ToolBarItemBorderColor != value)
			{
				EnsureUnlocked();
				((ReportViewerStyle)base.ControlStyle).ToolbarItemBorderColor = value;
				m_panelUpdater.MarkPanelsForUpdate(UpdateGroup.ExecutionSession);
			}
		}
	}

	[SRDescription("ToolBarItemBorderWidthDesc")]
	[WebBrowsable(true)]
	[DefaultValue(typeof(Unit), "1px")]
	[Category("Appearance")]
	public Unit ToolBarItemBorderWidth
	{
		get
		{
			return ((ReportViewerStyle)base.ControlStyle).ToolbarItemBorderWidth;
		}
		set
		{
			if (ToolBarItemBorderWidth != value)
			{
				EnsureUnlocked();
				((ReportViewerStyle)base.ControlStyle).ToolbarItemBorderWidth = value;
				m_panelUpdater.MarkPanelsForUpdate(UpdateGroup.ExecutionSession);
			}
		}
	}

	[DefaultValue(BorderStyle.Solid)]
	[Obsolete("The report viewer no longer uses pressed buttons.")]
	[Browsable(false)]
	public BorderStyle ToolBarItemPressedBorderStyle
	{
		get
		{
			return GetViewStateProperty("ToolBarItemPressedBorderStyle", BorderStyle.Solid);
		}
		set
		{
			ViewState["ToolBarItemPressedBorderStyle"] = value;
		}
	}

	[Obsolete("The report viewer no longer uses pressed buttons.")]
	[Browsable(false)]
	[DefaultValue(typeof(Color), "#336699")]
	public Color ToolBarItemPressedBorderColor
	{
		get
		{
			return GetViewStateProperty("ToolBarItemPressedBorderColor", Color.FromArgb(51, 102, 153));
		}
		set
		{
			ViewState["ToolBarItemPressedBorderColor"] = value;
		}
	}

	[DefaultValue(typeof(Unit), "1px")]
	[Obsolete("The report viewer no longer uses pressed buttons.")]
	[Browsable(false)]
	public Unit ToolBarItemPressedBorderWidth
	{
		get
		{
			return GetViewStateProperty("ToolBarItemPressedBorderWidth", Unit.Pixel(1));
		}
		set
		{
			ViewState["ToolBarItemPressedBorderWidth"] = value;
		}
	}

	[Category("Appearance")]
	[SRDescription("ToolBarItemHoverBackColorDesc")]
	[WebBrowsable(true)]
	[DefaultValue(typeof(Color), "#DDEEF7")]
	public Color ToolBarItemHoverBackColor
	{
		get
		{
			return ((ReportViewerStyle)base.ControlStyle).HoverBackColor;
		}
		set
		{
			if (ToolBarItemHoverBackColor != value)
			{
				EnsureUnlocked();
				((ReportViewerStyle)base.ControlStyle).HoverBackColor = value;
				m_panelUpdater.MarkPanelsForUpdate(UpdateGroup.ExecutionSession);
			}
		}
	}

	[Browsable(false)]
	[Obsolete("The report viewer no longer uses pressed buttons.")]
	[DefaultValue(typeof(Color), "#99BBE2")]
	public Color ToolBarItemPressedHoverBackColor
	{
		get
		{
			return GetViewStateProperty("ToolBarItemPressedHoverBackColor", Color.FromArgb(153, 187, 226));
		}
		set
		{
			ViewState["ToolBarItemPressedHoverBackColor"] = value;
		}
	}

	[Category("Appearance")]
	[WebBrowsable(true)]
	[SRDescription("ToolBarItemHoverBackColorDesc")]
	[DefaultValue(typeof(Color), "#ECE9D8")]
	public Color SplitterBackColor
	{
		get
		{
			return ((ReportViewerStyle)base.ControlStyle).SplitterBackColor;
		}
		set
		{
			if (SplitterBackColor != value)
			{
				EnsureUnlocked();
				((ReportViewerStyle)base.ControlStyle).SplitterBackColor = value;
				m_panelUpdater.MarkPanelsForUpdate(UpdateGroup.ExecutionSession);
			}
		}
	}

	[SRDescription("LinkDisabledColorDesc")]
	[DefaultValue(typeof(Color), "Gray")]
	[Category("Appearance")]
	[WebBrowsable(true)]
	public Color LinkDisabledColor
	{
		get
		{
			return ((ReportViewerStyle)base.ControlStyle).LinkDisabledColor;
		}
		set
		{
			if (LinkDisabledColor != value)
			{
				EnsureUnlocked();
				((ReportViewerStyle)base.ControlStyle).LinkDisabledColor = value;
				m_panelUpdater.MarkPanelsForUpdate(UpdateGroup.ExecutionSession);
			}
		}
	}

	[DefaultValue(typeof(Color), "#3366CC")]
	[SRDescription("LinkActiveColorDesc")]
	[Category("Appearance")]
	[WebBrowsable(true)]
	public Color LinkActiveColor
	{
		get
		{
			return ((ReportViewerStyle)base.ControlStyle).LinkActiveColor;
		}
		set
		{
			if (LinkActiveColor != value)
			{
				EnsureUnlocked();
				((ReportViewerStyle)base.ControlStyle).LinkActiveColor = value;
				m_panelUpdater.MarkPanelsForUpdate(UpdateGroup.ExecutionSession);
			}
		}
	}

	[Category("Appearance")]
	[SRDescription("LinkActiveHoverColorDesc")]
	[DefaultValue(typeof(Color), "#FF3300")]
	[WebBrowsable(true)]
	public Color LinkActiveHoverColor
	{
		get
		{
			return ((ReportViewerStyle)base.ControlStyle).LinkActiveHoverColor;
		}
		set
		{
			if (LinkActiveHoverColor != value)
			{
				EnsureUnlocked();
				((ReportViewerStyle)base.ControlStyle).LinkActiveHoverColor = value;
				m_panelUpdater.MarkPanelsForUpdate(UpdateGroup.ExecutionSession);
			}
		}
	}

	[SRDescription("SizeToContentDesc")]
	[DefaultValue(false)]
	[Category("Appearance")]
	[WebBrowsable(true)]
	public bool SizeToReportContent
	{
		get
		{
			return GetViewStateProperty("SizeToReportContent", defaultValue: false);
		}
		set
		{
			SetUnlockedViewStateProperty("SizeToReportContent", SizeToReportContent, value, UpdateGroup.ExecutionSession);
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
	[SRDescription("ProcessingModeDesc")]
	[WebBrowsable(true)]
	[DefaultValue(ProcessingMode.Local)]
	public ProcessingMode ProcessingMode
	{
		get
		{
			return GetViewStateProperty("ProcessingMode", ProcessingMode.Local);
		}
		set
		{
			if (ProcessingMode != value)
			{
				if (IsDrillthrough)
				{
					throw new InvalidOperationException();
				}
				EnsureUnlocked();
				ViewState["ProcessingMode"] = value;
				OnReportChanged(this, new ReportChangedEventArgs());
			}
		}
	}

	[SRDescription("ServerReportDesc")]
	[NotifyParentProperty(true)]
	[PersistenceMode(PersistenceMode.InnerProperty)]
	public ServerReport ServerReport => m_reportHierarchy.Peek().ServerReport;

	[NotifyParentProperty(true)]
	[SRDescription("LocalReportDesc")]
	[PersistenceMode(PersistenceMode.InnerProperty)]
	public LocalReport LocalReport => m_reportHierarchy.Peek().LocalReport;

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public int CurrentPage
	{
		get
		{
			return m_reportHierarchy.Peek().CurrentPage;
		}
		set
		{
			PerformPageNavigation(value, fireEvents: false);
		}
	}

	[SRDescription("PageCountModeDesc")]
	[DefaultValue(PageCountMode.Estimate)]
	[WebBrowsable(true)]
	public PageCountMode PageCountMode
	{
		get
		{
			return GetViewStateProperty("PageCountMode", PageCountMode.Estimate);
		}
		set
		{
			SetUnlockedViewStateProperty("PageCountMode", PageCountMode, value, UpdateGroup.Rerendering);
		}
	}

	[WebBrowsable(true)]
	[DefaultValue(ZoomMode.Percent)]
	[SRDescription("ZoomModeDesc")]
	[Category("Appearance")]
	public ZoomMode ZoomMode
	{
		get
		{
			return GetViewStateProperty("ZoomMode", ZoomMode.Percent);
		}
		set
		{
			SetUnlockedViewStateProperty("ZoomMode", ZoomMode, value, UpdateGroup.Rerendering);
		}
	}

	[SRDescription("ZoomPercentDesc")]
	[WebBrowsable(true)]
	[Category("Appearance")]
	[DefaultValue(100)]
	public int ZoomPercent
	{
		get
		{
			return GetViewStateProperty("ZoomPercent", 100);
		}
		set
		{
			if (value <= 0)
			{
				throw new ArgumentOutOfRangeException("value");
			}
			SetUnlockedViewStateProperty("ZoomPercent", ZoomPercent, value, UpdateGroup.Rerendering);
		}
	}

	[WebBrowsable(true)]
	[SRDescription("AsyncRenderingDesc")]
	[DefaultValue(true)]
	public bool AsyncRendering
	{
		get
		{
			return GetViewStateProperty("AsyncRendering", defaultValue: true);
		}
		set
		{
			if (SetUnlockedViewStateProperty("AsyncRendering", AsyncRendering, value, UpdateGroup.Reprocessing))
			{
				if (RenderingState == ReportRenderingState.AsyncWait && !value)
				{
					RenderingState = ReportRenderingState.Ready;
				}
				else if (RenderingState == ReportRenderingState.Ready && value)
				{
					RenderingState = ReportRenderingState.AsyncWait;
				}
			}
		}
	}

	[SRDescription("WaitControlDisplayAfterDesc")]
	[WebBrowsable(true)]
	[DefaultValue(1000)]
	public int WaitControlDisplayAfter
	{
		get
		{
			return GetViewStateProperty("WaitControlDisplayAfter", 1000);
		}
		set
		{
			if (value < 0)
			{
				throw new ArgumentOutOfRangeException("value");
			}
			SetUnlockedViewStateProperty("WaitControlDisplayAfter", WaitControlDisplayAfter, value, UpdateGroup.ExecutionSession);
		}
	}

	[Category("Appearance")]
	[WebBrowsable(true)]
	[SRDescription("ShowWaitControlCancelLinkDesc")]
	[DefaultValue(true)]
	public bool ShowWaitControlCancelLink
	{
		get
		{
			return GetViewStateProperty("ShowWaitControlCancelLink", defaultValue: true);
		}
		set
		{
			SetUnlockedViewStateProperty("ShowWaitControlCancelLink", ShowWaitControlCancelLink, value, UpdateGroup.ExecutionSession);
		}
	}

	[SRDescription("HyperlinkTargetDesc")]
	[DefaultValue("_top")]
	[WebBrowsable(true)]
	public string HyperlinkTarget
	{
		get
		{
			return GetViewStateProperty("HyperlinkTarget", "_top");
		}
		set
		{
			SetUnlockedViewStateProperty("HyperlinkTarget", HyperlinkTarget, value, UpdateGroup.Rerendering);
		}
	}

	internal string ReplacementRoot
	{
		get
		{
			return GetViewStateProperty("ReplacementRoot", "");
		}
		set
		{
			SetViewStateProperty("ReplacementRoot", ReplacementRoot, value, UpdateGroup.Rerendering);
		}
	}

	[SRDescription("ContentDispositionDesc")]
	[WebBrowsable(true)]
	[DefaultValue(ContentDisposition.OnlyHtmlInline)]
	public ContentDisposition ExportContentDisposition
	{
		get
		{
			return GetViewStateProperty("ExportContentDisposition", ContentDisposition.OnlyHtmlInline);
		}
		set
		{
			SetUnlockedViewStateProperty("ExportContentDisposition", ExportContentDisposition, value, UpdateGroup.ExecutionSession);
		}
	}

	[SRDescription("InteractivityPostBackModeDesc")]
	[WebBrowsable(true)]
	[DefaultValue(InteractivityPostBackMode.AlwaysAsynchronous)]
	public InteractivityPostBackMode InteractivityPostBackMode
	{
		get
		{
			return GetViewStateProperty("InteractivityPostBackMode", InteractivityPostBackMode.AlwaysAsynchronous);
		}
		set
		{
			SetUnlockedViewStateProperty("InteractivityPostBackMode", InteractivityPostBackMode, value, UpdateGroup.ExecutionSession);
		}
	}

	[DefaultValue(typeof(Unit), "400px")]
	public override Unit Height
	{
		get
		{
			return base.Height;
		}
		set
		{
			if (Height != value)
			{
				EnsureUnlocked();
				base.Height = value;
				m_panelUpdater.MarkPanelsForUpdate(UpdateGroup.ExecutionSession);
			}
		}
	}

	[DefaultValue(typeof(Unit), "400px")]
	public override Unit Width
	{
		get
		{
			return base.Width;
		}
		set
		{
			if (Width != value)
			{
				EnsureUnlocked();
				base.Width = value;
				m_panelUpdater.MarkPanelsForUpdate(UpdateGroup.ExecutionSession);
			}
		}
	}

	[SRDescription("KeepSessionAliveDesc")]
	[WebBrowsable(true)]
	[DefaultValue(true)]
	public bool KeepSessionAlive
	{
		get
		{
			return GetViewStateProperty("KeepSessionAlive", defaultValue: true);
		}
		set
		{
			SetUnlockedViewStateProperty("KeepSessionAlive", KeepSessionAlive, value, UpdateGroup.ExecutionSession);
		}
	}

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public ReportAreaContent ReportAreaContentType => m_contentType;

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public SearchState SearchState
	{
		get
		{
			return (SearchState)ViewState["SearchState"];
		}
		private set
		{
			if (SearchState != value)
			{
				ViewState["SearchState"] = value;
				m_alertMessage = null;
				m_panelUpdater.MarkPanelsForUpdate(UpdateGroup.Rerendering);
			}
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	public DeviceInfoCollection InteractiveDeviceInfos
	{
		get
		{
			if (m_interactiveDeviceInfos == null)
			{
				InteractiveDeviceInfos = new DeviceInfoCollection();
			}
			return m_interactiveDeviceInfos;
		}
		private set
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			m_interactiveDeviceInfos = value;
			m_interactiveDeviceInfos.DeviceInfoNameBlackList = ReportControl.GetDeviceInfoBlackList();
			m_interactiveDeviceInfos.EnsureUnlocked = EnsureUnlocked;
		}
	}

	public override string ID
	{
		get
		{
			return base.ID;
		}
		set
		{
			EnsureUnlocked();
			base.ID = value;
		}
	}

	internal PageSettings PageSettings
	{
		get
		{
			PageSettings pageSettings = m_reportHierarchy.Peek().PageSettings;
			if (pageSettings == null)
			{
				return ResetAndGetPageSettings();
			}
			return pageSettings;
		}
	}

	private bool IsDrillthrough => Report.IsDrillthroughReport;

	private ReportRenderingState RenderingState
	{
		get
		{
			object obj = ViewState["RenderingState"];
			if (obj == null)
			{
				return ReportRenderingState.NotReady;
			}
			return (ReportRenderingState)obj;
		}
		set
		{
			if (RenderingState != value)
			{
				ViewState["RenderingState"] = value;
				m_panelUpdater.MarkPanelsForUpdate(UpdateGroup.Reprocessing);
			}
		}
	}

	private bool ClientCanceledRendering
	{
		get
		{
			return GetViewStateProperty("ClientCanceledRendering", defaultValue: false);
		}
		set
		{
			SetViewStateProperty("ClientCanceledRendering", ClientCanceledRendering, value, UpdateGroup.Reprocessing);
		}
	}

	private bool IsFullControlRendering
	{
		get
		{
			if (!ScriptManager.GetCurrent(Page).IsInAsyncPostBack)
			{
				return true;
			}
			for (Control parent = Parent; parent != null; parent = parent.Parent)
			{
				if (parent is UpdatePanel { IsInPartialRendering: not false })
				{
					return true;
				}
			}
			return false;
		}
	}

	internal bool ReportHasChanged => m_reportHasChanged != ReportChangeType.None;

	internal bool IsClientRightToLeft
	{
		get
		{
			EnsureChildControls();
			return "rtl".Equals(m_direction.Value, StringComparison.OrdinalIgnoreCase);
		}
	}

	internal virtual IReportViewerStyles ViewerStyle => (IReportViewerStyles)base.ControlStyle;

	private ScrollTarget ScrollTarget
	{
		get
		{
			return (ScrollTarget)ViewState["ScrollTarget"];
		}
		set
		{
			ViewState["ScrollTarget"] = value;
		}
	}

	private bool PromptAreaHasVisibleContent
	{
		get
		{
			if (!m_parametersArea.HasVisibleParameters || !ShowParameterPrompts)
			{
				if (m_parametersArea.HasCredentials)
				{
					return ShowCredentialPrompts;
				}
				return false;
			}
			return true;
		}
	}

	private bool ShouldRenderPromptArea
	{
		get
		{
			ReportRenderingState renderingState = RenderingState;
			if ((renderingState == ReportRenderingState.AsyncWait || renderingState == ReportRenderingState.Ready || renderingState == ReportRenderingState.Completed || m_reportArea.ReportAreaContent != ReportAreaContent.Error) && PromptAreaHasVisibleContent)
			{
				if (ProcessingMode != ProcessingMode.Remote)
				{
					return LocalReport.SupportsQueries;
				}
				return true;
			}
			return false;
		}
	}

	private bool ShouldRenderToolbar
	{
		get
		{
			if (!ShowToolBar)
			{
				return false;
			}
			if (base.DesignMode)
			{
				return true;
			}
			return IsOrHasAttemptedToRenderReport;
		}
	}

	private bool ShouldRenderPromptAreaSplitter
	{
		get
		{
			if (!base.DesignMode && ShowPromptAreaButton)
			{
				return ShouldRenderPromptArea;
			}
			return false;
		}
	}

	private bool IsOrHasAttemptedToRenderReport
	{
		get
		{
			if (base.DesignMode)
			{
				return false;
			}
			switch (RenderingState)
			{
			case ReportRenderingState.AsyncWait:
			case ReportRenderingState.Ready:
				return ShowReportBody;
			default:
				return ClientCanceledRendering;
			case ReportRenderingState.Completed:
				return true;
			}
		}
	}

	private bool WillRenderReport
	{
		get
		{
			bool flag = ShowReportBody && !base.DesignMode && m_reportArea.ReportAreaContent != ReportAreaContent.Error && Report.IsReadyForRendering;
			ReportRenderingState renderingState = RenderingState;
			bool result = renderingState == ReportRenderingState.Ready || renderingState == ReportRenderingState.Completed;
			if (flag)
			{
				return result;
			}
			return false;
		}
	}

	internal string InstanceIdentifier => m_instanceIdentifier.ToString("N");

	private string ParametersRowID => "ParametersRow" + ClientID;

	private string FixedTableID => ClientID + "_fixedTable";

	internal Report Report
	{
		get
		{
			if (ProcessingMode == ProcessingMode.Remote)
			{
				return ServerReport;
			}
			return LocalReport;
		}
	}

	internal ReportControlSession ReportControlSession
	{
		get
		{
			ReportInfo reportInfo = m_reportHierarchy.Peek();
			if (ProcessingMode == ProcessingMode.Remote)
			{
				return reportInfo.ServerSession;
			}
			return reportInfo.LocalSession;
		}
	}

	internal ReportHierarchy ReportHierarchy => m_reportHierarchy;

	internal ReportSplitter ParametersAreaSplitter => m_parametersAreaSplitter;

	[SRDescription("PageNavigationEventDesc")]
	public event PageNavigationEventHandler PageNavigation;

	[SRDescription("BackEventDesc")]
	public event BackEventHandler Back;

	[SRDescription("DocMapEventDesc")]
	public event DocumentMapNavigationEventHandler DocumentMapNavigation;

	[SRDescription("BookmarkEventDesc")]
	public event BookmarkNavigationEventHandler BookmarkNavigation;

	[SRDescription("ToggleEventDesc")]
	public event CancelEventHandler Toggle;

	[SRDescription("DrillthroughEventDesc")]
	public event DrillthroughEventHandler Drillthrough;

	[SRDescription("SortEventDesc")]
	public event SortEventHandler Sort;

	[SRDescription("SearchEventDesc")]
	public event SearchEventHandler Search;

	[SRDescription("RefreshEventDesc")]
	public event CancelEventHandler ReportRefresh;

	[SRDescription("ErrorEventDesc")]
	public event ReportErrorEventHandler ReportError;

	[SRDescription("SubmittingDataSourceCredentialsEventDesc")]
	public event ReportCredentialsEventHandler SubmittingDataSourceCredentials;

	[SRDescription("SubmittingParameterValuesEventDesc")]
	public event ReportParametersEventHandler SubmittingParameterValues;

	static ReportViewer()
	{
		IOSRenderingExtensionBlackList = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		IOSRenderingExtensionBlackList.Add("ATOM");
		IOSRenderingExtensionBlackList.Add("CSV");
		IOSRenderingExtensionBlackList.Add("EXCEL");
		IOSRenderingExtensionBlackList.Add("EXCELOPENXML");
		IOSRenderingExtensionBlackList.Add("HTML4.0");
		IOSRenderingExtensionBlackList.Add("MHTML");
		IOSRenderingExtensionBlackList.Add("NULL");
		IOSRenderingExtensionBlackList.Add("RGDI");
		IOSRenderingExtensionBlackList.Add("RPL");
		IOSRenderingExtensionBlackList.Add("WORD");
		IOSRenderingExtensionBlackList.Add("WORDOPENXML");
		IOSRenderingExtensionBlackList.Add("XML");
	}

	public ReportViewer()
	{
		Width = Unit.Pixel(400);
		Height = Unit.Pixel(400);
		m_changeHandler = OnReportChanged;
		m_dataInitializationHandler = InitializeDataSources;
		CreateReportHierarchy();
	}

	public override void Dispose()
	{
		HttpSessionState session = HttpContext.Current.Session;
		bool flag = session != null && session[InstanceIdentifier] == m_reportHierarchy;
		DisconnectReportHierarchy(!flag);
		base.Dispose();
		GC.SuppressFinalize(this);
	}

	private PropertyType GetViewStateProperty<PropertyType>(string viewStateKey, PropertyType defaultValue)
	{
		object obj = ViewState[viewStateKey];
		if (obj != null)
		{
			return (PropertyType)obj;
		}
		return defaultValue;
	}

	private bool SetUnlockedViewStateProperty(string viewStateKey, object oldValue, object newValue, UpdateGroup updateGroup)
	{
		if (!object.Equals(oldValue, newValue) && m_lockChanges)
		{
			throw new InvalidOperationException(Errors.ReadOnlyViewer);
		}
		return SetViewStatePropertyNoValidate(viewStateKey, oldValue, newValue, updateGroup);
	}

	private bool SetViewStateProperty(string viewStateKey, object oldValue, object newValue, UpdateGroup group)
	{
		return SetViewStatePropertyNoValidate(viewStateKey, oldValue, newValue, group);
	}

	private bool SetViewStatePropertyNoValidate(string viewStateKey, object oldValue, object newValue, UpdateGroup group)
	{
		if (!object.Equals(oldValue, newValue))
		{
			ViewState[viewStateKey] = newValue;
			m_panelUpdater.MarkPanelsForUpdate(group);
			return true;
		}
		return false;
	}

	private void SetDocumentMapCollapsedInternal(bool setToCollapsed, bool redrawClient)
	{
		SetUnlockedViewStateProperty("DocumentMapCollapsed", DocumentMapCollapsed, setToCollapsed, redrawClient ? UpdateGroup.ExecutionSession : UpdateGroup.None);
	}

	private void SetPromptAreaCollapsedInternal(bool setToCollapsed, bool redrawClient)
	{
		SetUnlockedViewStateProperty("PromptAreaCollapsed", PromptAreaCollapsed, setToCollapsed, redrawClient ? UpdateGroup.ExecutionSession : UpdateGroup.None);
	}

	public void JumpToBookmark(string bookmarkId)
	{
		JumpToBookmark(bookmarkId, fireEvents: false);
	}

	private void JumpToBookmark(string bookmarkId, bool fireEvents)
	{
		EnsureUnlocked();
		ReportActionHandler reportActionHandler = CreateActionHandler(fireEvents);
		if (reportActionHandler.HandleBookmarkNavigation(bookmarkId, out var newPage, out var scrollTarget))
		{
			InternalSetCurrentPage(newPage, scrollTarget);
		}
	}

	public void JumpToDocumentMapId(string documentMapId)
	{
		JumpToDocumentMapId(documentMapId, fireEvents: false);
	}

	private void JumpToDocumentMapId(string documentMapId, bool fireEvents)
	{
		EnsureUnlocked();
		ReportActionHandler reportActionHandler = CreateActionHandler(fireEvents);
		if (reportActionHandler.HandleDocMapNavigation(documentMapId, out var newPage, out var scrollTarget))
		{
			InternalSetCurrentPage(newPage, scrollTarget);
		}
	}

	public void PerformBack()
	{
		EnsureUnlocked();
		PerformBack(fireEvents: false);
	}

	private void PerformBack(bool fireEvents)
	{
		if (!IsDrillthrough)
		{
			throw new InvalidOperationException(CommonStrings.NotInDrillthrough);
		}
		bool flag = false;
		if (this.Back != null && fireEvents)
		{
			ReportInfo[] array = m_reportHierarchy.ToArray();
			ReportInfo reportInfo = array[1];
			Report parentReport = ((ProcessingMode != ProcessingMode.Local) ? ((Report)reportInfo.ServerReport) : ((Report)reportInfo.LocalReport));
			BackEventArgs e = new BackEventArgs(parentReport);
			this.Back(this, e);
			flag = e.Cancel;
		}
		if (!flag)
		{
			ReportInfo reportInfo2 = m_reportHierarchy.Pop();
			reportInfo2.DisconnectChangeEvent(m_changeHandler, m_dataInitializationHandler, disconnectUserEvents: false);
			m_reportHasChanged = ReportChangeType.Back;
			ScrollTarget = new ScrollTarget(m_reportHierarchy.Peek().ScrollPosition);
		}
	}

	private void PerformPageNavigation(int targetPage, bool fireEvents)
	{
		if (targetPage != CurrentPage)
		{
			ReportActionHandler reportActionHandler = CreateActionHandler(fireEvents);
			if (reportActionHandler.HandlePageNavigation(targetPage))
			{
				InternalSetCurrentPage(targetPage, null);
			}
		}
	}

	private void OnClientSideZoomChanged(object sender, ZoomChangeEventArgs e)
	{
		ZoomMode = e.ZoomMode;
		ZoomPercent = e.ZoomPercent;
	}

	internal void OnReportAction(object sender, ReportActionEventArgs e)
	{
		try
		{
			if (!ReportHasChanged)
			{
				string actionType = e.ActionType;
				string actionParam = e.ActionParam;
				if (string.Equals(actionType, "Toggle", StringComparison.Ordinal))
				{
					PerformToggle(actionParam);
				}
				else if (string.Equals(actionType, "Bookmark", StringComparison.Ordinal))
				{
					JumpToBookmark(actionParam, fireEvents: true);
				}
				else if (string.Equals(actionType, "Sort", StringComparison.Ordinal))
				{
					PerformSort(actionParam);
				}
				else if (string.Equals(actionType, "Refresh", StringComparison.Ordinal))
				{
					PerformRefresh(string.Equals(actionParam, "auto", StringComparison.Ordinal));
				}
				else if (string.Equals(actionType, "Drillthrough", StringComparison.Ordinal))
				{
					PerformDrillthrough(actionParam);
				}
				else if (string.Equals(actionType, "Find", StringComparison.Ordinal))
				{
					Find(actionParam, CurrentPage);
				}
				else if (string.Equals(actionType, "FindNext", StringComparison.Ordinal))
				{
					FindNext();
				}
				else if (string.Equals(actionType, "PageNav", StringComparison.Ordinal))
				{
					int targetPage = int.Parse(actionParam, CultureInfo.InvariantCulture);
					PerformPageNavigation(targetPage, fireEvents: true);
				}
				else if (string.Equals(actionType, "Back", StringComparison.Ordinal))
				{
					PerformBack(fireEvents: true);
				}
			}
		}
		catch (Exception e2)
		{
			OnError(e2);
		}
	}

	private void OnDocumentMapNavigation(object sender, DocumentMapNavigationEventArgs e)
	{
		if (!ReportHasChanged)
		{
			JumpToDocumentMapId(e.DocumentMapId, fireEvents: true);
		}
	}

	private void PerformRefresh(bool isAutoRefresh)
	{
		ReportActionHandler reportActionHandler = CreateActionHandler(fireEvents: true);
		int currentPage = CurrentPage;
		if (reportActionHandler.HandleRefresh())
		{
			ClientCanceledRendering = false;
			if (isAutoRefresh)
			{
				string clientScrollPosition = m_reportArea.ClientScrollPosition;
				ScrollTarget scrollTarget = new ScrollTarget(clientScrollPosition);
				InternalSetCurrentPage(currentPage, scrollTarget);
			}
			m_panelUpdater.MarkPanelsForUpdate(UpdateGroup.Reprocessing);
		}
	}

	public void Find(string searchText, int startPage)
	{
		EnsureUnlocked();
		SearchState searchState = new SearchState(searchText, startPage);
		ReportActionHandler reportActionHandler = CreateActionHandler(fireEvents: true);
		int newPage;
		ScrollTarget scrollTarget;
		SearchResult result = reportActionHandler.HandleSearch(searchState, out newPage, out scrollTarget);
		HandleSearchResult(result, searchState, searchNext: false, newPage, scrollTarget);
	}

	private void FindNext()
	{
		SearchState searchState = SearchState;
		if (searchState != null)
		{
			ReportActionHandler reportActionHandler = CreateActionHandler(fireEvents: true);
			int newPage;
			ScrollTarget scrollTarget;
			SearchResult result = reportActionHandler.HandleSearchNext(searchState, out newPage, out scrollTarget);
			HandleSearchResult(result, searchState, searchNext: true, newPage, scrollTarget);
		}
	}

	private void HandleSearchResult(SearchResult result, SearchState searchState, bool searchNext, int newPage, ScrollTarget scrollTarget)
	{
		if (result != SearchResult.Cancelled)
		{
			if (newPage == 0)
			{
				newPage = searchState.StartPage;
			}
			InternalSetCurrentPage(newPage, scrollTarget);
			if (result == SearchResult.FoundMoreHits)
			{
				SearchState = searchState;
			}
			else if (searchNext)
			{
				m_alertMessage = LocalizationHelper.Current.NoMoreMatches;
			}
			else
			{
				m_alertMessage = LocalizationHelper.Current.TextNotFound;
			}
		}
	}

	private void PerformToggle(string toggleId)
	{
		ReportActionHandler reportActionHandler = CreateActionHandler(fireEvents: true);
		if (reportActionHandler.HandleToggle(toggleId, out var scrollTarget))
		{
			InternalSetCurrentPage(CurrentPage, scrollTarget);
		}
	}

	private void PerformDrillthrough(string drillthroughId)
	{
		ReportActionHandler reportActionHandler = CreateActionHandler(fireEvents: true);
		Report report = reportActionHandler.HandleDrillthrough(drillthroughId);
		if (report != null)
		{
			LocalModeSession localSession;
			ServerReport serverReport;
			if (ProcessingMode == ProcessingMode.Local)
			{
				localSession = new LocalModeSession((LocalReport)report);
				serverReport = new ServerReport(ServerReport);
			}
			else
			{
				localSession = new LocalModeSession();
				serverReport = (ServerReport)report;
			}
			m_reportHierarchy.Peek().ScrollPosition = m_reportArea.ClientScrollPosition;
			ReportInfo reportInfo = new ReportInfo(localSession, new ServerModeSession(serverReport));
			reportInfo.ConnectChangeEvent(m_changeHandler, m_dataInitializationHandler);
			m_reportHierarchy.Push(reportInfo);
			OnReportChanged(this, new ReportChangedEventArgs());
		}
	}

	private void PerformSort(string clientSortAction)
	{
		ReportActionHandler reportActionHandler = CreateActionHandler(fireEvents: true);
		if (reportActionHandler.HandleSort(clientSortAction, out var pageNumber, out var scrollTarget))
		{
			if (pageNumber > 0)
			{
				InternalSetCurrentPage(pageNumber, scrollTarget);
			}
			else
			{
				InternalSetCurrentPage(CurrentPage, null);
			}
			m_panelUpdater.MarkPanelsForUpdate(UpdateGroup.Reprocessing);
		}
	}

	private ReportActionHandler CreateActionHandler(bool fireEvents)
	{
		EnsureUnlocked();
		return new ReportActionHandler(Report, this, CurrentPage, PageCountMode, fireEvents ? this.PageNavigation : null, fireEvents ? this.Toggle : null, fireEvents ? this.BookmarkNavigation : null, fireEvents ? this.DocumentMapNavigation : null, fireEvents ? this.Drillthrough : null, fireEvents ? this.Sort : null, fireEvents ? this.Search : null, fireEvents ? this.ReportRefresh : null);
	}

	public void Reset()
	{
		OnReportChanged(this, new ReportChangedEventArgs());
		CreateReportHierarchy();
	}

	internal void OnError(object sender, ReportErrorEventArgs e)
	{
		OnError(e.Exception);
	}

	internal void OnError(Exception e)
	{
		EnsureChildControls();
		ReportErrorEventArgs e2 = new ReportErrorEventArgs(e);
		if (this.ReportError != null)
		{
			this.ReportError(this, e2);
		}
		m_panelUpdater.MarkPanelsForUpdate(UpdateGroup.Reprocessing);
		m_hasErrorsOnThisPostBack = true;
		m_reportArea.SetException(e, e2.Handled);
		if (m_contentType != ReportAreaContent.Unknown)
		{
			m_contentType = ReportAreaContent.Error;
		}
	}

	private void OnSubmittingDataSourceCredentials(object sender, ReportCredentialsEventArgs credentialArgs)
	{
		if (this.SubmittingDataSourceCredentials != null)
		{
			this.SubmittingDataSourceCredentials(this, credentialArgs);
		}
	}

	private void OnSubmittingParameterValues(object sender, ReportParametersEventArgs parameterArgs)
	{
		if (this.SubmittingParameterValues != null)
		{
			this.SubmittingParameterValues(this, parameterArgs);
		}
	}

	public PageSettings GetPageSettings()
	{
		return ReportViewerUtils.DeepClonePageSettings(PageSettings);
	}

	public void ResetPageSettings()
	{
		ResetAndGetPageSettings();
	}

	private PageSettings ResetAndGetPageSettings()
	{
		PageSettings pageSettings = null;
		try
		{
			pageSettings = Report.GetDefaultPageSettings().CustomPageSettings;
		}
		catch (MissingReportSourceException)
		{
			pageSettings = null;
		}
		m_reportHierarchy.Peek().PageSettings = pageSettings;
		return pageSettings;
	}

	public void SetPageSettings(PageSettings pageSettings)
	{
		if (pageSettings == null)
		{
			throw new ArgumentNullException("pageSettings");
		}
		m_reportHierarchy.Peek().PageSettings = ReportViewerUtils.DeepClonePageSettings(pageSettings);
	}

	private void CreateReportHierarchy()
	{
		DisconnectReportHierarchy(shouldDispose: true);
		m_reportHierarchy = new ReportHierarchy(CreateServerReport());
		m_reportHierarchy.ConnectChangeEvents(m_changeHandler, m_dataInitializationHandler);
	}

	private void DisconnectReportHierarchy(bool shouldDispose)
	{
		if (m_reportHierarchy != null)
		{
			m_reportHierarchy.DisconnectChangeEvents(m_changeHandler, m_dataInitializationHandler);
			if (shouldDispose)
			{
				m_reportHierarchy.Dispose();
			}
			else
			{
				m_reportHierarchy.DisposeNonSessionResources();
			}
			m_reportHierarchy = null;
		}
	}

	public void RegisterPostBackControl(Control control)
	{
		if (control == null)
		{
			throw new ArgumentNullException("control");
		}
		EnsureChildControls();
		m_asyncWaitControl.Triggers.Add(control);
	}

	protected override Style CreateControlStyle()
	{
		return new ReportViewerStyle(ViewState);
	}

	protected override void CreateChildControls()
	{
		Controls.Clear();
		m_panelUpdater.UnregisterAllPanels();
		if (!base.DesignMode && (Page == null || ScriptManager.GetCurrent(Page) == null))
		{
			throw new ScriptManagerNotFoundException();
		}
		m_noScriptControl = new NoScriptControl();
		m_noScriptControl.Visible = !base.DesignMode;
		Controls.Add(m_noScriptControl);
		m_topLevelUpdatePanel = new UpdatePanel();
		m_topLevelUpdatePanel.UpdateMode = UpdatePanelUpdateMode.Conditional;
		m_topLevelUpdatePanel.RenderMode = UpdatePanelRenderMode.Block;
		m_topLevelUpdatePanel.ChildrenAsTriggers = false;
		m_topLevelUpdatePanel.ID = "ReportViewer";
		Controls.Add(m_topLevelUpdatePanel);
		m_panelUpdater.RegisterPanel(m_topLevelUpdatePanel, UpdateGroup.ExecutionSession);
		DelegatedRenderingControl delegatedRenderingControl = new DelegatedRenderingControl(RenderTopLevelUpdatePanelContents);
		m_topLevelUpdatePanel.ContentTemplateContainer.Controls.Add(delegatedRenderingControl);
		m_clientScript = new ReportViewerClientScript();
		m_clientScript.ReportAction += OnReportAction;
		delegatedRenderingControl.Controls.Add(m_clientScript);
		m_parametersArea = new ParametersArea(this);
		m_parametersArea.ViewReportClick += OnViewReport;
		m_parametersArea.ParameterValuesChanged += OnParameterValuesChanged;
		m_parametersArea.SubmittingDataSourceCredentials += OnSubmittingDataSourceCredentials;
		m_parametersArea.SubmittingParameterValues += OnSubmittingParameterValues;
		delegatedRenderingControl.Controls.Add(m_parametersArea);
		m_parametersAreaSplitter = new ReportSplitter(ViewerStyle, isVertical: false, LocalizationHelper.Current.ParameterAreaButtonToolTip);
		m_parametersAreaSplitter.IsResizable = false;
		m_parametersAreaSplitter.ID = "ToggleParam";
		delegatedRenderingControl.Controls.Add(m_parametersAreaSplitter);
		m_toolbarArea = new ToolbarControl(this);
		m_toolbarArea.ReportAction += OnReportAction;
		delegatedRenderingControl.Controls.Add(m_toolbarArea);
		m_docMapUpdatePanel = new UpdatePanel();
		m_docMapUpdatePanel.UpdateMode = UpdatePanelUpdateMode.Conditional;
		m_docMapUpdatePanel.RenderMode = UpdatePanelRenderMode.Block;
		m_docMapUpdatePanel.ChildrenAsTriggers = false;
		m_docMapUpdatePanel.ID = "DocMap";
		delegatedRenderingControl.Controls.Add(m_docMapUpdatePanel);
		m_panelUpdater.RegisterPanel(m_docMapUpdatePanel, UpdateGroup.Reprocessing);
		m_docMapAreaSplitter = new ReportSplitter(ViewerStyle, isVertical: true, LocalizationHelper.Current.DocumentMapButtonToolTip);
		delegatedRenderingControl.Controls.Add(m_docMapAreaSplitter);
		m_docMapArea = new DocMapArea(this);
		m_docMapArea.NodeClick += OnDocumentMapNavigation;
		m_docMapUpdatePanel.ContentTemplateContainer.Controls.Add(m_docMapArea);
		m_reportArea = new ReportArea(ViewerStyle);
		m_reportArea.ReportAction += OnReportAction;
		m_reportArea.ZoomChanged += OnClientSideZoomChanged;
		m_reportArea.AsyncLoadRequested += OnAsyncLoadReport;
		if (ScriptManager.GetCurrent(Page) != null)
		{
			delegatedRenderingControl.Controls.Add(m_reportArea);
		}
		m_httpHandlerMissingError = new HttpHandlerMissingErrorMessage();
		m_httpHandlerMissingError.ID = "HttpHandlerMissingErrorMessage";
		delegatedRenderingControl.Controls.Add(m_httpHandlerMissingError);
		m_docMapAreaSplitter.CollapsedChanged += ClientSideDocMapAreaVisibilityChanged;
		m_parametersAreaSplitter.CollapsedChanged += ClientSidePromptAreaVisibilityChanged;
		m_direction = new HiddenField();
		delegatedRenderingControl.Controls.Add(m_direction);
		m_browserMode = new HiddenField();
		delegatedRenderingControl.Controls.Add(m_browserMode);
		m_asyncWaitControl = new AsyncWaitControl(ViewerStyle);
		m_asyncWaitControl.ID = "AsyncWait";
		m_asyncWaitControl.ClientCanceledStateChanged += OnClientCanceledStateChanged;
		m_asyncWaitControl.Triggers.Add(this);
		delegatedRenderingControl.Controls.Add(m_asyncWaitControl);
	}

	private void OnClientCanceledStateChanged(object sender, ClientCanceledStateChangeEventArgs e)
	{
		ClientCanceledRendering = e.ClientCanceled;
	}

	internal void Update()
	{
		EnsureChildControls();
		m_panelUpdater.MarkPanelsForUpdate(UpdateGroup.ExecutionSession);
	}

	protected override void Render(HtmlTextWriter writer)
	{
		EnsureChildControls();
		MonitoredScope val = MonitoredScope.New("ReportViewerControl.Render");
		try
		{
			if (!base.DesignMode)
			{
				ScriptManager.GetCurrent(Page)?.RegisterScriptDescriptors(this);
				if (!m_viewStateSaved)
				{
					OnError(new ViewStateDisabledException());
				}
				((ReportViewerStyle)base.ControlStyle).ObeySizeProperties = !SizeToReportContent;
				m_noScriptControl.RenderControl(writer);
				if (IsFullControlRendering && RenderingState == ReportRenderingState.Completed && !m_panelUpdater.IsPanelGroupMarkedForUpdate(UpdateGroup.Rerendering))
				{
					m_reportArea.SetFullViewerRendering();
					try
					{
						RenderReport(updateDocMap: true);
					}
					catch (Exception e)
					{
						OnError(e);
					}
				}
				m_topLevelUpdatePanel.RenderControl(writer);
			}
			else
			{
				RenderTopLevelUpdatePanelContents(writer);
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private bool RenderTopLevelUpdatePanelContents(HtmlTextWriter writer)
	{
		AddAttributesToRender(writer);
		m_parametersArea.RenderCloseDropDownAttributes(writer);
		writer.RenderBeginTag(HtmlTextWriterTag.Div);
		m_httpHandlerMissingError.RenderControl(writer);
		m_clientScript.RenderControl(writer);
		m_direction.RenderControl(writer);
		m_browserMode.RenderControl(writer);
		if (!base.DesignMode)
		{
			m_asyncWaitControl.RenderControl(writer);
		}
		writer.AddAttribute(HtmlTextWriterAttribute.Cellpadding, "0");
		writer.AddAttribute(HtmlTextWriterAttribute.Cellspacing, "0");
		writer.AddAttribute(HtmlTextWriterAttribute.Id, FixedTableID);
		if (!SizeToReportContent)
		{
			writer.AddStyleAttribute("table-layout", "fixed");
			if (!Width.IsEmpty)
			{
				writer.AddStyleAttribute(HtmlTextWriterStyle.Width, "100%");
			}
			if (!Height.IsEmpty && !base.DesignMode)
			{
				writer.AddStyleAttribute(HtmlTextWriterStyle.Height, "100%");
			}
		}
		writer.RenderBeginTag(HtmlTextWriterTag.Table);
		bool flag = ReportAreaContentType == ReportAreaContent.ReportPage && m_docMapArea.RootNode != null && !DocumentMapCollapsed;
		bool visible = m_toolbarArea.Visible;
		bool flag2 = !PromptAreaCollapsed && m_parametersArea.Visible;
		if (!SizeToReportContent)
		{
			if (visible || flag2)
			{
				if (ViewerStyle.ViewerAreaBackground != null)
				{
					writer.AddAttribute(HtmlTextWriterAttribute.Class, ViewerStyle.ViewerAreaBackground);
				}
				else
				{
					writer.AddStyleAttribute(HtmlTextWriterStyle.BackgroundColor, ColorTranslator.ToHtml(base.ControlStyle.BackColor));
				}
			}
			writer.RenderBeginTag(HtmlTextWriterTag.Tr);
			if (!flag)
			{
				writer.AddStyleAttribute(HtmlTextWriterStyle.Display, "none");
			}
			writer.AddStyleAttribute(HtmlTextWriterStyle.Width, DocumentMapWidth.ToString(CultureInfo.InvariantCulture));
			writer.RenderBeginTag(HtmlTextWriterTag.Td);
			writer.RenderEndTag();
			writer.AddStyleAttribute(HtmlTextWriterStyle.Display, "none");
			writer.AddStyleAttribute(HtmlTextWriterStyle.Width, "6px");
			writer.RenderBeginTag(HtmlTextWriterTag.Td);
			writer.RenderEndTag();
			writer.AddStyleAttribute(HtmlTextWriterStyle.Width, "100%");
			writer.RenderBeginTag(HtmlTextWriterTag.Td);
			writer.RenderEndTag();
			writer.RenderEndTag();
		}
		if (!base.DesignMode)
		{
			if (!flag2)
			{
				writer.AddStyleAttribute(HtmlTextWriterStyle.Display, "none");
			}
			writer.AddAttribute(HtmlTextWriterAttribute.Id, ParametersRowID);
			writer.RenderBeginTag(HtmlTextWriterTag.Tr);
			writer.AddAttribute(HtmlTextWriterAttribute.Colspan, "3");
			writer.RenderBeginTag(HtmlTextWriterTag.Td);
			if (!SizeToReportContent)
			{
				m_parametersArea.Style[HtmlTextWriterStyle.Width] = "100%";
				m_parametersArea.Style[HtmlTextWriterStyle.OverflowX] = "auto";
				m_parametersArea.Style[HtmlTextWriterStyle.OverflowY] = "hidden";
			}
			m_parametersArea.RenderControl(writer);
			writer.RenderEndTag();
			writer.RenderEndTag();
			writer.AddStyleAttribute(HtmlTextWriterStyle.Height, "6px");
			writer.AddStyleAttribute(HtmlTextWriterStyle.FontSize, "2pt");
			if (!ShouldRenderPromptAreaSplitter)
			{
				writer.AddStyleAttribute(HtmlTextWriterStyle.Display, "none");
			}
			writer.RenderBeginTag(HtmlTextWriterTag.Tr);
			writer.AddAttribute(HtmlTextWriterAttribute.Colspan, "3");
			writer.AddStyleAttribute(HtmlTextWriterStyle.Padding, "0px");
			writer.AddStyleAttribute(HtmlTextWriterStyle.Margin, "0px");
			m_parametersAreaSplitter.WriteTableCellCenteringStyles(writer);
			writer.RenderBeginTag(HtmlTextWriterTag.Td);
			m_parametersAreaSplitter.RenderControl(writer);
			writer.RenderEndTag();
			writer.RenderEndTag();
		}
		if (!visible)
		{
			writer.AddStyleAttribute(HtmlTextWriterStyle.Display, "none");
		}
		writer.RenderBeginTag(HtmlTextWriterTag.Tr);
		if (visible)
		{
			writer.AddAttribute(HtmlTextWriterAttribute.Colspan, "3");
			writer.RenderBeginTag(HtmlTextWriterTag.Td);
			m_toolbarArea.RenderControl(writer);
			writer.RenderEndTag();
		}
		writer.RenderEndTag();
		if (!base.DesignMode)
		{
			writer.RenderBeginTag(HtmlTextWriterTag.Tr);
			writer.AddStyleAttribute(HtmlTextWriterStyle.VerticalAlign, "top");
			if (!SizeToReportContent)
			{
				writer.AddStyleAttribute(HtmlTextWriterStyle.Width, DocumentMapWidth.ToString(CultureInfo.InvariantCulture));
				writer.AddStyleAttribute(HtmlTextWriterStyle.Height, "100%");
			}
			if (!flag)
			{
				writer.AddStyleAttribute(HtmlTextWriterStyle.Display, "none");
			}
			writer.RenderBeginTag(HtmlTextWriterTag.Td);
			if (!SizeToReportContent)
			{
				writer.AddStyleAttribute(HtmlTextWriterStyle.Width, "100%");
				writer.AddStyleAttribute(HtmlTextWriterStyle.Height, "100%");
			}
			writer.RenderBeginTag(HtmlTextWriterTag.Div);
			m_docMapUpdatePanel.RenderControl(writer);
			writer.RenderEndTag();
			writer.RenderEndTag();
			writer.AddStyleAttribute(HtmlTextWriterStyle.Display, "none");
			writer.AddStyleAttribute(HtmlTextWriterStyle.Width, "4px");
			writer.AddStyleAttribute(HtmlTextWriterStyle.Padding, "0px");
			writer.AddStyleAttribute(HtmlTextWriterStyle.Margin, "0px");
			writer.AddStyleAttribute(HtmlTextWriterStyle.Height, "100%");
			m_docMapAreaSplitter.WriteTableCellCenteringStyles(writer);
			writer.RenderBeginTag(HtmlTextWriterTag.Td);
			m_docMapAreaSplitter.RenderControl(writer);
			writer.RenderEndTag();
			if (!SizeToReportContent)
			{
				writer.AddStyleAttribute(HtmlTextWriterStyle.Height, "100%");
			}
			writer.AddStyleAttribute(HtmlTextWriterStyle.VerticalAlign, "top");
			writer.RenderBeginTag(HtmlTextWriterTag.Td);
			if (!SizeToReportContent)
			{
				m_reportArea.Width = Unit.Percentage(100.0);
				m_reportArea.Height = Unit.Percentage(100.0);
				m_reportArea.Style.Add(HtmlTextWriterStyle.Overflow, "auto");
			}
			m_reportArea.Style.Add(HtmlTextWriterStyle.Position, "relative");
			m_reportArea.RenderControl(writer);
			writer.RenderEndTag();
			writer.RenderEndTag();
		}
		writer.RenderEndTag();
		writer.RenderEndTag();
		return false;
	}

	private void RenderReport(bool updateDocMap)
	{
		string browserMode = null;
		if (m_browserMode != null)
		{
			browserMode = m_browserMode.Value;
		}
		int num = m_reportArea.RenderReport(ReportControlSession, InstanceIdentifier, PageCountMode, CurrentPage, InteractivityPostBackMode, SearchState, ReplacementRoot, HyperlinkTarget, ScrollTarget, m_alertMessage, InteractiveDeviceInfos, browserMode, SizeToReportContent);
		ScrollTarget = null;
		if (updateDocMap && Report.HasDocMap)
		{
			m_docMapArea.RootNode = Report.GetDocumentMap();
		}
		if (num != CurrentPage)
		{
			InternalSetCurrentPage(num, null);
		}
	}

	protected override void OnInit(EventArgs e)
	{
		if (!base.DesignMode && base.ChildControlsCreated)
		{
			throw new Exception("Internal error: ClientID reference before OnInit");
		}
		base.OnInit(e);
	}

	private void SetToLegacyClientIDMode()
	{
		PropertyInfo clientIDModeProp = null;
		SecurityAssertionHandler.RunWithSecurityAssert(new ReflectionPermission(ReflectionPermissionFlag.NoFlags), delegate
		{
			clientIDModeProp = GetType().GetProperty("ClientIDMode");
		});
		if (clientIDModeProp != null)
		{
			Type propertyType = clientIDModeProp.PropertyType;
			if (propertyType != null)
			{
				clientIDModeProp.SetValue(this, Enum.Parse(propertyType, "AutoID"), null);
			}
		}
	}

	protected override void OnPreRender(EventArgs e)
	{
		MonitoredScope val = MonitoredScope.New("ReportViewerControl.OnPreRender");
		try
		{
			SetToLegacyClientIDMode();
			EnsureChildControls();
			ScriptManager current = ScriptManager.GetCurrent(Page);
			current?.RegisterScriptControl(this);
			ClientPrintInfo clientPrintInfo = null;
			try
			{
				base.OnPreRender(e);
				string htmlStyleForFont = ReportViewerStyle.GetHtmlStyleForFont(WaitMessageFont);
				if (!string.Equals(m_originalWaitMessageFont, htmlStyleForFont, StringComparison.Ordinal))
				{
					m_panelUpdater.MarkPanelsForUpdate(UpdateGroup.ExecutionSession);
				}
				m_noScriptControl.AlternateUrl = ServerReport.ReportUrlNoScript;
				if (m_hasErrorsOnThisPostBack)
				{
					return;
				}
				m_reportHierarchy.Peek().ScrollPosition = null;
				if (ReportHasChanged)
				{
					SearchState = null;
					m_reportArea.Clear();
					if (m_reportHasChanged != ReportChangeType.Refresh)
					{
						m_parametersArea.RefreshControlsFromReportMetadata();
						m_panelUpdater.MarkPanelsForUpdate(UpdateGroup.ExecutionSession);
					}
					if (m_reportHasChanged == ReportChangeType.ReportObject || m_reportHasChanged == ReportChangeType.Refresh)
					{
						RenderingState = ReportRenderingState.Preparing;
					}
					else if (m_reportHasChanged == ReportChangeType.Back)
					{
						RenderingState = ReportRenderingState.Completed;
					}
				}
				else if (m_userParamsChanged || m_viewReportClicked)
				{
					SearchState = null;
					m_reportArea.Clear();
					bool flag = m_parametersArea.SaveControlValuesToReport();
					if (m_viewReportClicked && flag)
					{
						PerformRefresh(isAutoRefresh: false);
						RenderingState = ReportRenderingState.Preparing;
					}
					else
					{
						RenderingState = ReportRenderingState.NotReady;
					}
				}
				m_lockChanges = true;
				if (m_hasErrorsOnThisPostBack)
				{
					return;
				}
				if (InteractivityPostBackMode == InteractivityPostBackMode.AlwaysSynchronous)
				{
					current.RegisterPostBackControl(m_docMapArea);
					current.RegisterPostBackControl(m_clientScript);
					current.RegisterPostBackControl(m_parametersArea);
					current.RegisterPostBackControl(m_toolbarArea);
				}
				if (Report.IsReadyForConnection && ShowReportBody)
				{
					if (!ShouldRenderPromptAreaSplitter && PromptAreaCollapsed)
					{
						m_parametersArea.ValidateAllReportInputsSatisfied();
					}
					else
					{
						m_parametersArea.ValidateNonVisibleReportInputsSatisfied();
					}
					ValidateAllDataSourcesSatisfied();
				}
				bool flag2 = Report.PrepareForRender();
				if (RenderingState == ReportRenderingState.Preparing && !base.DesignMode)
				{
					if (flag2)
					{
						if (ProcessingMode == ProcessingMode.Local)
						{
							InitializeDataSources(LocalReport.DataSources);
						}
						if (CurrentPage == 0)
						{
							InternalSetCurrentPage(1, null);
						}
						RenderingState = ReportRenderingState.Pending;
					}
					else
					{
						RenderingState = ReportRenderingState.NotReady;
					}
				}
				if (ShowReportBody && !ClientCanceledRendering)
				{
					if (RenderingState == ReportRenderingState.Pending)
					{
						if (AsyncRendering && m_reportHasChanged != ReportChangeType.Refresh)
						{
							RenderingState = ReportRenderingState.AsyncWait;
						}
						else
						{
							RenderingState = ReportRenderingState.Ready;
						}
					}
				}
				else if (RenderingState == ReportRenderingState.AsyncWait || RenderingState == ReportRenderingState.Ready)
				{
					RenderingState = ReportRenderingState.Pending;
				}
				if (WillRenderReport)
				{
					if (m_panelUpdater.IsPanelGroupMarkedForUpdate(UpdateGroup.Rerendering))
					{
						RenderingState = ReportRenderingState.Completed;
						bool updateDocMap = m_panelUpdater.IsPanelGroupMarkedForUpdate(UpdateGroup.Reprocessing);
						RenderReport(updateDocMap);
						if (Report.HasDocMap && !SizeToReportContent && m_panelUpdater.IsPanelGroupMarkedForUpdate(UpdateGroup.Reprocessing))
						{
							m_docMapArea.RootNode = Report.GetDocumentMap();
						}
					}
				}
				else if (RenderingState == ReportRenderingState.AsyncWait)
				{
					if (ProcessingMode == ProcessingMode.Local && !LocalReport.SupportsQueries)
					{
						LocalReport.CreateSnapshot();
					}
					m_reportArea.SetForAsyncRendering();
				}
				clientPrintInfo = new ClientPrintInfo(this);
			}
			catch (Exception e2)
			{
				OnError(e2);
			}
			finally
			{
				if (m_reportArea.ReportAreaContent == ReportAreaContent.Error && !m_hasErrorsOnThisPostBack)
				{
					m_reportArea.Clear();
				}
				m_contentType = m_reportArea.ReportAreaContent;
				m_toolbarArea.Visible = ShouldRenderToolbar;
				m_parametersArea.Visible = ShouldRenderPromptArea;
				m_parametersAreaSplitter.IsCollapsed = PromptAreaCollapsed;
				m_parametersAreaSplitter.IsCollapsable = ShowPromptAreaButton;
				m_docMapAreaSplitter.IsCollapsed = DocumentMapCollapsed;
				m_docMapAreaSplitter.IsCollapsable = ShowDocumentMapButton;
				m_docMapAreaSplitter.IsResizable = !SizeToReportContent;
				m_panelUpdater.PerformUpdates();
				m_clientScript.SetViewerInfo(this, m_reportArea.ClientID, ParametersRowID, m_docMapArea.ClientID, FixedTableID, m_topLevelUpdatePanel.ClientID, m_docMapUpdatePanel.ClientID, m_parametersAreaSplitter.ClientID, m_docMapAreaSplitter.ClientID, m_docMapArea.DocMapHeaderOverflowDivId, m_direction.ClientID, m_browserMode.ClientID, clientPrintInfo);
				m_reportArea.SetReportZoom(ZoomMode, ZoomPercent);
				m_asyncWaitControl.DisplayDelayMillis = WaitControlDisplayAfter;
				m_asyncWaitControl.CancelLinkVisible = ShowWaitControlCancelLink;
				m_asyncWaitControl.SetViewerInfo(ClientID, FixedTableID, ClientCanceledRendering, m_spinnyShouldSkipTimer);
				if (ProcessingMode == ProcessingMode.Remote)
				{
					_ = ServerReport.IsReadyForConnection;
				}
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private void OnAsyncLoadReport(object sender, EventArgs e)
	{
		if (RenderingState == ReportRenderingState.AsyncWait)
		{
			RenderingState = ReportRenderingState.Ready;
		}
		else if (RenderingState == ReportRenderingState.Completed)
		{
			m_panelUpdater.MarkPanelsForUpdate(UpdateGroup.Rerendering);
		}
	}

	IEnumerable<ScriptDescriptor> IScriptControl.GetScriptDescriptors()
	{
		EnsureChildControls();
		ScriptReferenceValidationDescriptor scriptReferenceValidationDescriptor = new ScriptReferenceValidationDescriptor(m_httpHandlerMissingError.ClientID);
		ScriptComponentDescriptor scriptComponentDescriptor = new ScriptComponentDescriptor("Microsoft.Reporting.WebFormsClient.ReportViewer");
		scriptComponentDescriptor.ID = ClientID;
		scriptComponentDescriptor.AddProperty("_internalViewerId", m_clientScript.ClientID);
		return new ScriptDescriptor[2] { scriptReferenceValidationDescriptor, scriptComponentDescriptor };
	}

	IEnumerable<ScriptReference> IScriptControl.GetScriptReferences()
	{
		ScriptReference scriptReference = new ScriptReference();
		scriptReference.Path = EmbeddedResourceOperation.CreateUrlForScriptFile();
		return new ScriptReference[1] { scriptReference };
	}

	private void InitializeDataSources(object sender, InitializeDataSourcesEventArgs e)
	{
		InitializeDataSources(e.DataSources);
	}

	private void InitializeDataSources(ReportDataSourceCollection dataSources)
	{
		if (ProcessingMode != ProcessingMode.Local)
		{
			return;
		}
		foreach (ReportDataSource dataSource in dataSources)
		{
			if (dataSource.DataSourceId != null && dataSource.DataSourceId.Length != 0)
			{
				Control control = FindControl(this, dataSource.DataSourceId);
				if (control == null)
				{
					throw new HttpException(Errors.DataControl_DataSourceDoesntExist(dataSource.Name, ID, dataSource.DataSourceId));
				}
				if (!(control is IDataSource))
				{
					throw new HttpException(Errors.DataControl_DataSourceIDMustBeDataControl(dataSource.Name, ID, dataSource.DataSourceId ?? ""));
				}
				dataSource.SetValueWithoutChange(control);
			}
		}
	}

	private void ValidateAllDataSourcesSatisfied()
	{
		if (ProcessingMode != ProcessingMode.Local)
		{
			return;
		}
		foreach (string dataSourceName in LocalReport.GetDataSourceNames())
		{
			if (LocalReport.DataSources[dataSourceName] == null)
			{
				throw new MissingDataSourceException(dataSourceName);
			}
		}
	}

	private static Control FindControl(Control control, string controlID)
	{
		Control control2 = control;
		Control control3 = null;
		if (control == control.Page)
		{
			return control.FindControl(controlID);
		}
		while (control3 == null && control2 != control.Page)
		{
			control2 = control2.NamingContainer;
			if (control2 == null)
			{
				throw new HttpException(Errors.NoNamingContainer(control.GetType().Name, control.ID));
			}
			control3 = control2.FindControl(controlID);
		}
		return control3;
	}

	private void ClientSidePromptAreaVisibilityChanged(object sender, EventArgs e)
	{
		EnsureChildControls();
		SetPromptAreaCollapsedInternal(((ReportSplitter)sender).IsCollapsed, redrawClient: false);
	}

	private void ClientSideDocMapAreaVisibilityChanged(object sender, EventArgs e)
	{
		EnsureChildControls();
		SetDocumentMapCollapsedInternal(((ReportSplitter)sender).IsCollapsed, redrawClient: false);
	}

	private void EnsureUnlocked()
	{
		if (m_lockChanges)
		{
			throw new InvalidOperationException(Errors.ReadOnlyViewer);
		}
	}

	protected override void LoadViewState(object savedState)
	{
		try
		{
			m_panelUpdater.CancelAllUpdates();
			object[] array = (object[])savedState;
			base.LoadViewState(array[0]);
			m_instanceIdentifier = (Guid)array[1];
			if (array[4] != null)
			{
				InteractiveDeviceInfos = (DeviceInfoCollection)array[4];
			}
			else
			{
				m_interactiveDeviceInfos = null;
			}
			m_originalWaitMessageFont = ReportViewerStyle.GetHtmlStyleForFont(WaitMessageFont);
			ReportHierarchy reportHierarchy;
			if (EnsureSessionOrConfig())
			{
				reportHierarchy = GetReportHierarchyFromSession();
			}
			else
			{
				reportHierarchy = new ReportHierarchy(CreateServerReport());
				for (int i = 1; i < (int)array[2]; i++)
				{
					ServerReport serverReport = CreateServerReport();
					reportHierarchy.Push(new ReportInfo(new LocalModeSession(), new ServerModeSession(serverReport)));
				}
			}
			reportHierarchy.LoadViewState(array[3]);
			ConnectNewReportHierarchy(reportHierarchy);
		}
		catch (Exception e)
		{
			m_aspSessionFailedToLoad = true;
			m_panelUpdater.MarkPanelsForUpdate(UpdateGroup.ExecutionSession);
			RenderingState = ReportRenderingState.NotReady;
			OnError(e);
		}
	}

	protected override object SaveViewState()
	{
		bool flag = EnsureSessionOrConfig();
		object[] array = new object[5]
		{
			base.SaveViewState(),
			m_instanceIdentifier,
			m_reportHierarchy.Count,
			m_reportHierarchy.SaveViewState(!flag),
			null
		};
		if (flag && (!m_aspSessionFailedToLoad || ReportHasChanged))
		{
			HttpContext.Current.Session[InstanceIdentifier] = m_reportHierarchy;
		}
		m_viewStateSaved = true;
		array[4] = m_interactiveDeviceInfos;
		return array;
	}

	internal virtual bool EnsureSessionOrConfig()
	{
		if (base.DesignMode)
		{
			return false;
		}
		bool flag = HttpContext.Current.Session == null;
		if (ProcessingMode == ProcessingMode.Local)
		{
			if (flag)
			{
				throw new SessionDisabledException();
			}
			return true;
		}
		if (ServerReport.RequiresConnection)
		{
			if (WebConfigReader.Current.ServerConnection != null)
			{
				return false;
			}
			if (flag)
			{
				throw new MissingReportServerConnectionInformationException();
			}
			return true;
		}
		return false;
	}

	internal virtual ServerReport CreateServerReport()
	{
		ServerReport serverReport = new ServerReport();
		IReportServerConnection serverConnection = WebConfigReader.Current.ServerConnection;
		if (serverConnection != null)
		{
			ApplyConnectionToServerReport(serverConnection, serverReport);
		}
		return serverReport;
	}

	internal void ApplyConnectionToServerReport(IReportServerConnection connection, ServerReport serverReport)
	{
		serverReport.ReportServerUrl = connection.ReportServerUrl;
		serverReport.Timeout = connection.Timeout;
		serverReport.ReportServerCredentials = connection;
		if (!(connection is IReportServerConnection2 { Cookies: var cookies } reportServerConnection))
		{
			return;
		}
		if (cookies != null)
		{
			foreach (Cookie item in cookies)
			{
				if (item != null)
				{
					serverReport.Cookies.Add(item);
				}
			}
		}
		IEnumerable<string> headers = reportServerConnection.Headers;
		if (headers == null)
		{
			return;
		}
		foreach (string item2 in headers)
		{
			if (item2 != null)
			{
				serverReport.Headers.Add(item2);
			}
		}
	}

	internal static IEnumerable<RenderingExtension> FilterOutClientUnsupportedRenderingExtensions(IEnumerable<RenderingExtension> source)
	{
		HashSet<string> blackListSet = (BrowserDetectionUtility.IsIOSSafari() ? IOSRenderingExtensionBlackList : new HashSet<string>(StringComparer.OrdinalIgnoreCase));
		foreach (RenderingExtension extension in source)
		{
			if (!blackListSet.Contains(extension.Name))
			{
				yield return extension;
			}
		}
	}

	private void OnViewReport(object sender, EventArgs e)
	{
		m_viewReportClicked = true;
		ClientCanceledRendering = false;
	}

	private void OnParameterValuesChanged(object sender, EventArgs e)
	{
		m_userParamsChanged = true;
	}

	private void OnReportChanged(object sender, ReportChangedEventArgs e)
	{
		EnsureUnlocked();
		InternalSetCurrentPage(0, null);
		if (e.IsRefreshOnly && m_reportHasChanged == ReportChangeType.None)
		{
			m_panelUpdater.MarkPanelsForUpdate(UpdateGroup.Reprocessing);
			m_reportHasChanged = ReportChangeType.Refresh;
		}
		else
		{
			m_panelUpdater.MarkPanelsForUpdate(UpdateGroup.ExecutionSession);
			m_reportHasChanged = ReportChangeType.ReportObject;
			m_spinnyShouldSkipTimer = true;
		}
	}

	private void InternalSetCurrentPage(int pageNumber, ScrollTarget scrollTarget)
	{
		SearchState = null;
		m_alertMessage = null;
		ScrollTarget = scrollTarget;
		m_reportHierarchy.Peek().CurrentPage = pageNumber;
		m_panelUpdater.MarkPanelsForUpdate(UpdateGroup.Rerendering);
	}

	[Conditional("DEBUG")]
	private static void DebugRoundTripSerializers(object obj)
	{
	}

	internal void UseExistingLocalSession(string otherViewerInstanceIdentifier)
	{
		m_instanceIdentifier = new Guid(otherViewerInstanceIdentifier);
		ReportHierarchy reportHierarchyFromSession = GetReportHierarchyFromSession();
		ConnectNewReportHierarchy(reportHierarchyFromSession);
	}

	private ReportHierarchy GetReportHierarchyFromSession()
	{
		object obj = HttpContext.Current.Session[InstanceIdentifier];
		if (obj != null)
		{
			return (ReportHierarchy)obj;
		}
		throw new AspNetSessionExpiredException();
	}

	private void ConnectNewReportHierarchy(ReportHierarchy newReportHierarchy)
	{
		if (newReportHierarchy.Count > 0 && m_reportHierarchy.Count > 0)
		{
			LocalReport localReport = m_reportHierarchy.Peek().LocalReport;
			LocalReport localReport2 = newReportHierarchy.Peek().LocalReport;
			localReport.TransferEvents(localReport2);
		}
		DisconnectReportHierarchy(shouldDispose: true);
		m_reportHierarchy = newReportHierarchy;
		m_reportHierarchy.ConnectChangeEvents(m_changeHandler, m_dataInitializationHandler);
		m_reportHasChanged = ReportChangeType.None;
		m_alertMessage = null;
	}
}
