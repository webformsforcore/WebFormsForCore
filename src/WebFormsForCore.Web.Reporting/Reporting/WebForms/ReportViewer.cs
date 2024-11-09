// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.ReportViewer
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.Diagnostics.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.Net;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  [Microsoft.Reporting.SRDescription("ReportViewerDescription")]
  [Designer("Microsoft.Reporting.WebForms.ReportViewerDesigner, Microsoft.ReportViewer.WebDesign, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91")]
  [ParseChildren(true)]
  [PersistChildren(false)]
  public class ReportViewer : CompositeControl, IScriptControl
  {
    public const int MaximumPageCount = 2147483647;
    private static readonly HashSet<string> IOSRenderingExtensionBlackList = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
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

    static ReportViewer()
    {
      ReportViewer.IOSRenderingExtensionBlackList.Add("ATOM");
      ReportViewer.IOSRenderingExtensionBlackList.Add("CSV");
      ReportViewer.IOSRenderingExtensionBlackList.Add("EXCEL");
      ReportViewer.IOSRenderingExtensionBlackList.Add("EXCELOPENXML");
      ReportViewer.IOSRenderingExtensionBlackList.Add("HTML4.0");
      ReportViewer.IOSRenderingExtensionBlackList.Add("MHTML");
      ReportViewer.IOSRenderingExtensionBlackList.Add("NULL");
      ReportViewer.IOSRenderingExtensionBlackList.Add("RGDI");
      ReportViewer.IOSRenderingExtensionBlackList.Add("RPL");
      ReportViewer.IOSRenderingExtensionBlackList.Add("WORD");
      ReportViewer.IOSRenderingExtensionBlackList.Add("WORDOPENXML");
      ReportViewer.IOSRenderingExtensionBlackList.Add("XML");
    }

    public ReportViewer()
    {
      this.Width = Unit.Pixel(400);
      this.Height = Unit.Pixel(400);
      this.m_changeHandler = new EventHandler<ReportChangedEventArgs>(this.OnReportChanged);
      this.m_dataInitializationHandler = new InitializeDataSourcesEventHandler(this.InitializeDataSources);
      this.CreateReportHierarchy();
    }

    public override void Dispose()
    {
      HttpSessionState session = HttpContext.Current.Session;
      this.DisconnectReportHierarchy(session == null || session[this.InstanceIdentifier] != this.m_reportHierarchy);
      base.Dispose();
      GC.SuppressFinalize((object) this);
    }

    protected override HtmlTextWriterTag TagKey => HtmlTextWriterTag.Div;

    private PropertyType GetViewStateProperty<PropertyType>(
      string viewStateKey,
      PropertyType defaultValue)
    {
      object obj = this.ViewState[viewStateKey];
      return obj != null ? (PropertyType) obj : defaultValue;
    }

    private bool SetUnlockedViewStateProperty(
      string viewStateKey,
      object oldValue,
      object newValue,
      UpdateGroup updateGroup)
    {
      if (!object.Equals(oldValue, newValue) && this.m_lockChanges)
        throw new InvalidOperationException(Errors.ReadOnlyViewer);
      return this.SetViewStatePropertyNoValidate(viewStateKey, oldValue, newValue, updateGroup);
    }

    private bool SetViewStateProperty(
      string viewStateKey,
      object oldValue,
      object newValue,
      UpdateGroup group)
    {
      return this.SetViewStatePropertyNoValidate(viewStateKey, oldValue, newValue, group);
    }

    private bool SetViewStatePropertyNoValidate(
      string viewStateKey,
      object oldValue,
      object newValue,
      UpdateGroup group)
    {
      if (object.Equals(oldValue, newValue))
        return false;
      this.ViewState[viewStateKey] = newValue;
      this.m_panelUpdater.MarkPanelsForUpdate(group);
      return true;
    }

    [WebBrowsable(true)]
    [DefaultValue(false)]
    [Microsoft.Reporting.SRDescription("DocMapCollapsedDesc")]
    [Category("Appearance")]
    public bool DocumentMapCollapsed
    {
      get => this.GetViewStateProperty<bool>(nameof (DocumentMapCollapsed), false);
      set => this.SetDocumentMapCollapsedInternal(value, true);
    }

    private void SetDocumentMapCollapsedInternal(bool setToCollapsed, bool redrawClient)
    {
      this.SetUnlockedViewStateProperty("DocumentMapCollapsed", (object) this.DocumentMapCollapsed, (object) setToCollapsed, redrawClient ? UpdateGroup.ExecutionSession : UpdateGroup.None);
    }

    [Category("Appearance")]
    [DefaultValue(true)]
    [Microsoft.Reporting.SRDescription("ShowToolBarDesc")]
    [WebBrowsable(true)]
    public bool ShowToolBar
    {
      get => this.GetViewStateProperty<bool>("ShowToolbar", true);
      set
      {
        this.SetUnlockedViewStateProperty("ShowToolbar", (object) this.ShowToolBar, (object) value, UpdateGroup.ExecutionSession);
      }
    }

    [WebBrowsable(true)]
    [Category("Appearance")]
    [DefaultValue(true)]
    [Microsoft.Reporting.SRDescription("ShowParameterPromptsDesc")]
    public bool ShowParameterPrompts
    {
      get => this.GetViewStateProperty<bool>("ShowParameters", true);
      set
      {
        this.SetUnlockedViewStateProperty("ShowParameters", (object) this.ShowParameterPrompts, (object) value, UpdateGroup.ExecutionSession);
      }
    }

    [WebBrowsable(true)]
    [Category("Appearance")]
    [DefaultValue(true)]
    [Microsoft.Reporting.SRDescription("ShowCredentialPromptsDesc")]
    public bool ShowCredentialPrompts
    {
      get => this.GetViewStateProperty<bool>("ShowCredentialsArea", true);
      set
      {
        this.SetUnlockedViewStateProperty("ShowCredentialsArea", (object) this.ShowCredentialPrompts, (object) value, UpdateGroup.ExecutionSession);
      }
    }

    [Microsoft.Reporting.SRDescription("PromptAreaCollapsedDesc")]
    [Category("Appearance")]
    [DefaultValue(false)]
    [WebBrowsable(true)]
    public bool PromptAreaCollapsed
    {
      get => this.GetViewStateProperty<bool>(nameof (PromptAreaCollapsed), false);
      set => this.SetPromptAreaCollapsedInternal(value, true);
    }

    private void SetPromptAreaCollapsedInternal(bool setToCollapsed, bool redrawClient)
    {
      this.SetUnlockedViewStateProperty("PromptAreaCollapsed", (object) this.PromptAreaCollapsed, (object) setToCollapsed, redrawClient ? UpdateGroup.ExecutionSession : UpdateGroup.None);
    }

    [Category("Appearance")]
    [DefaultValue(true)]
    [WebBrowsable(true)]
    [Microsoft.Reporting.SRDescription("ShowReportBodyDesc")]
    public bool ShowReportBody
    {
      get => this.GetViewStateProperty<bool>(nameof (ShowReportBody), true);
      set
      {
        if (!this.SetUnlockedViewStateProperty(nameof (ShowReportBody), (object) this.ShowReportBody, (object) value, UpdateGroup.ExecutionSession) || !value || this.RenderingState != ReportRenderingState.NotReady)
          return;
        this.RenderingState = ReportRenderingState.Preparing;
      }
    }

    [WebBrowsable(true)]
    [Microsoft.Reporting.SRDescription("DocMapWidthDesc")]
    [Category("Appearance")]
    [DefaultValue(typeof (Unit), "25%")]
    public Unit DocumentMapWidth
    {
      get => this.GetViewStateProperty<Unit>(nameof (DocumentMapWidth), Unit.Percentage(25.0));
      set
      {
        this.SetUnlockedViewStateProperty(nameof (DocumentMapWidth), (object) this.DocumentMapWidth, (object) value, UpdateGroup.ExecutionSession);
      }
    }

    [Microsoft.Reporting.SRDescription("ShowDocumentMapButtonDesc")]
    [WebBrowsable(true)]
    [DefaultValue(true)]
    public bool ShowDocumentMapButton
    {
      get => this.GetViewStateProperty<bool>(nameof (ShowDocumentMapButton), true);
      set
      {
        this.SetUnlockedViewStateProperty(nameof (ShowDocumentMapButton), (object) this.ShowDocumentMapButton, (object) value, UpdateGroup.ExecutionSession);
      }
    }

    [DefaultValue(true)]
    [Microsoft.Reporting.SRDescription("ShowPromptAreaButtonDesc")]
    [WebBrowsable(true)]
    public bool ShowPromptAreaButton
    {
      get => this.GetViewStateProperty<bool>(nameof (ShowPromptAreaButton), true);
      set
      {
        this.SetUnlockedViewStateProperty(nameof (ShowPromptAreaButton), (object) this.ShowPromptAreaButton, (object) value, UpdateGroup.ExecutionSession);
      }
    }

    [WebBrowsable(true)]
    [DefaultValue(true)]
    [Microsoft.Reporting.SRDescription("ShowPageNavigationDesc")]
    [Microsoft.Reporting.SRCategory("ToolBarCategoryDesc")]
    public bool ShowPageNavigationControls
    {
      get => this.GetViewStateProperty<bool>(nameof (ShowPageNavigationControls), true);
      set
      {
        this.SetUnlockedViewStateProperty(nameof (ShowPageNavigationControls), (object) this.ShowPageNavigationControls, (object) value, UpdateGroup.ExecutionSession);
      }
    }

    [DefaultValue(true)]
    [Microsoft.Reporting.SRDescription("ShowBackButtonDesc")]
    [WebBrowsable(true)]
    [Microsoft.Reporting.SRCategory("ToolBarCategoryDesc")]
    public bool ShowBackButton
    {
      get => this.GetViewStateProperty<bool>(nameof (ShowBackButton), true);
      set
      {
        this.SetUnlockedViewStateProperty(nameof (ShowBackButton), (object) this.ShowBackButton, (object) value, UpdateGroup.ExecutionSession);
      }
    }

    [Microsoft.Reporting.SRDescription("ShowRefreshButtonDesc")]
    [Microsoft.Reporting.SRCategory("ToolBarCategoryDesc")]
    [DefaultValue(true)]
    [WebBrowsable(true)]
    public bool ShowRefreshButton
    {
      get => this.GetViewStateProperty<bool>(nameof (ShowRefreshButton), true);
      set
      {
        this.SetUnlockedViewStateProperty(nameof (ShowRefreshButton), (object) this.ShowRefreshButton, (object) value, UpdateGroup.ExecutionSession);
      }
    }

    [Microsoft.Reporting.SRDescription("ShowPrintButtonDesc")]
    [DefaultValue(true)]
    [Microsoft.Reporting.SRCategory("ToolBarCategoryDesc")]
    [WebBrowsable(true)]
    public bool ShowPrintButton
    {
      get => this.GetViewStateProperty<bool>(nameof (ShowPrintButton), true);
      set
      {
        this.SetUnlockedViewStateProperty(nameof (ShowPrintButton), (object) this.ShowPrintButton, (object) value, UpdateGroup.ExecutionSession);
      }
    }

    internal bool ShowAtomDataFeedButton
    {
      get
      {
        object obj = this.ViewState[nameof (ShowAtomDataFeedButton)];
        return obj != null && (bool) obj;
      }
      set => this.ViewState[nameof (ShowAtomDataFeedButton)] = (object) value;
    }

    [DefaultValue(true)]
    [Microsoft.Reporting.SRDescription("ShowExportButtonDesc")]
    [WebBrowsable(true)]
    [Microsoft.Reporting.SRCategory("ToolBarCategoryDesc")]
    public bool ShowExportControls
    {
      get => this.GetViewStateProperty<bool>(nameof (ShowExportControls), true);
      set
      {
        this.SetUnlockedViewStateProperty(nameof (ShowExportControls), (object) this.ShowExportControls, (object) value, UpdateGroup.ExecutionSession);
      }
    }

    [DefaultValue(true)]
    [Microsoft.Reporting.SRDescription("ShowZoomButtonDesc")]
    [WebBrowsable(true)]
    [Microsoft.Reporting.SRCategory("ToolBarCategoryDesc")]
    public bool ShowZoomControl
    {
      get => this.GetViewStateProperty<bool>("ShowZoomControls", true);
      set
      {
        this.SetUnlockedViewStateProperty("ShowZoomControls", (object) this.ShowZoomControl, (object) value, UpdateGroup.ExecutionSession);
      }
    }

    [Microsoft.Reporting.SRCategory("ToolBarCategoryDesc")]
    [DefaultValue(true)]
    [WebBrowsable(true)]
    [Microsoft.Reporting.SRDescription("ShowFindButtonDesc")]
    public bool ShowFindControls
    {
      get => this.GetViewStateProperty<bool>(nameof (ShowFindControls), true);
      set
      {
        this.SetUnlockedViewStateProperty(nameof (ShowFindControls), (object) this.ShowFindControls, (object) value, UpdateGroup.ExecutionSession);
      }
    }

    [DefaultValue(typeof (Color), "#ECE9D8")]
    public override Color BackColor
    {
      get => base.BackColor;
      set => base.BackColor = value;
    }

    [NotifyParentProperty(true)]
    [Microsoft.Reporting.SRDescription("WaitMessageFontDesc")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    [Category("Appearance")]
    public FontInfo WaitMessageFont => ((ReportViewerStyle) this.ControlStyle).WaitMessageFont;

    [DefaultValue(BorderStyle.Solid)]
    [WebBrowsable(true)]
    [Microsoft.Reporting.SRDescription("InternalBorderStyleDesc")]
    [Category("Appearance")]
    public BorderStyle InternalBorderStyle
    {
      get => ((ReportViewerStyle) this.ControlStyle).InternalBorderStyle;
      set
      {
        if (this.InternalBorderStyle == value)
          return;
        this.EnsureUnlocked();
        ((ReportViewerStyle) this.ControlStyle).InternalBorderStyle = value;
        this.m_panelUpdater.MarkPanelsForUpdate(UpdateGroup.ExecutionSession);
      }
    }

    [Category("Appearance")]
    [WebBrowsable(true)]
    [DefaultValue(typeof (Color), "#CCCCCC")]
    [Microsoft.Reporting.SRDescription("InternalBorderColorDesc")]
    public Color InternalBorderColor
    {
      get => ((ReportViewerStyle) this.ControlStyle).InternalBorderColor;
      set
      {
        if (!(this.InternalBorderColor != value))
          return;
        this.EnsureUnlocked();
        ((ReportViewerStyle) this.ControlStyle).InternalBorderColor = value;
        this.m_panelUpdater.MarkPanelsForUpdate(UpdateGroup.ExecutionSession);
      }
    }

    [DefaultValue(typeof (Unit), "1px")]
    [Microsoft.Reporting.SRDescription("InternalBorderWidthDesc")]
    [Category("Appearance")]
    [WebBrowsable(true)]
    public Unit InternalBorderWidth
    {
      get => ((ReportViewerStyle) this.ControlStyle).InternalBorderWidth;
      set
      {
        if (!(this.InternalBorderWidth != value))
          return;
        this.EnsureUnlocked();
        ((ReportViewerStyle) this.ControlStyle).InternalBorderWidth = value;
        this.m_panelUpdater.MarkPanelsForUpdate(UpdateGroup.ExecutionSession);
      }
    }

    [Microsoft.Reporting.SRDescription("ToolBarItemBorderStyleDesc")]
    [Category("Appearance")]
    [WebBrowsable(true)]
    [DefaultValue(BorderStyle.Solid)]
    public BorderStyle ToolBarItemBorderStyle
    {
      get => ((ReportViewerStyle) this.ControlStyle).ToolbarItemBorderStyle;
      set
      {
        if (this.BorderStyle == value)
          return;
        this.EnsureUnlocked();
        ((ReportViewerStyle) this.ControlStyle).ToolbarItemBorderStyle = value;
        this.m_panelUpdater.MarkPanelsForUpdate(UpdateGroup.ExecutionSession);
      }
    }

    [DefaultValue(typeof (Color), "#336699")]
    [WebBrowsable(true)]
    [Microsoft.Reporting.SRDescription("ToolBarItemBorderColorDesc")]
    [Category("Appearance")]
    public Color ToolBarItemBorderColor
    {
      get => ((ReportViewerStyle) this.ControlStyle).ToolbarItemBorderColor;
      set
      {
        if (!(this.ToolBarItemBorderColor != value))
          return;
        this.EnsureUnlocked();
        ((ReportViewerStyle) this.ControlStyle).ToolbarItemBorderColor = value;
        this.m_panelUpdater.MarkPanelsForUpdate(UpdateGroup.ExecutionSession);
      }
    }

    [Microsoft.Reporting.SRDescription("ToolBarItemBorderWidthDesc")]
    [WebBrowsable(true)]
    [DefaultValue(typeof (Unit), "1px")]
    [Category("Appearance")]
    public Unit ToolBarItemBorderWidth
    {
      get => ((ReportViewerStyle) this.ControlStyle).ToolbarItemBorderWidth;
      set
      {
        if (!(this.ToolBarItemBorderWidth != value))
          return;
        this.EnsureUnlocked();
        ((ReportViewerStyle) this.ControlStyle).ToolbarItemBorderWidth = value;
        this.m_panelUpdater.MarkPanelsForUpdate(UpdateGroup.ExecutionSession);
      }
    }

    [DefaultValue(BorderStyle.Solid)]
    [Obsolete("The report viewer no longer uses pressed buttons.")]
    [Browsable(false)]
    public BorderStyle ToolBarItemPressedBorderStyle
    {
      get
      {
        return this.GetViewStateProperty<BorderStyle>(nameof (ToolBarItemPressedBorderStyle), BorderStyle.Solid);
      }
      set => this.ViewState[nameof (ToolBarItemPressedBorderStyle)] = (object) value;
    }

    [Obsolete("The report viewer no longer uses pressed buttons.")]
    [Browsable(false)]
    [DefaultValue(typeof (Color), "#336699")]
    public Color ToolBarItemPressedBorderColor
    {
      get
      {
        return this.GetViewStateProperty<Color>(nameof (ToolBarItemPressedBorderColor), Color.FromArgb(51, 102, 153));
      }
      set => this.ViewState[nameof (ToolBarItemPressedBorderColor)] = (object) value;
    }

    [DefaultValue(typeof (Unit), "1px")]
    [Obsolete("The report viewer no longer uses pressed buttons.")]
    [Browsable(false)]
    public Unit ToolBarItemPressedBorderWidth
    {
      get => this.GetViewStateProperty<Unit>(nameof (ToolBarItemPressedBorderWidth), Unit.Pixel(1));
      set => this.ViewState[nameof (ToolBarItemPressedBorderWidth)] = (object) value;
    }

    [Category("Appearance")]
    [Microsoft.Reporting.SRDescription("ToolBarItemHoverBackColorDesc")]
    [WebBrowsable(true)]
    [DefaultValue(typeof (Color), "#DDEEF7")]
    public Color ToolBarItemHoverBackColor
    {
      get => ((ReportViewerStyle) this.ControlStyle).HoverBackColor;
      set
      {
        if (!(this.ToolBarItemHoverBackColor != value))
          return;
        this.EnsureUnlocked();
        ((ReportViewerStyle) this.ControlStyle).HoverBackColor = value;
        this.m_panelUpdater.MarkPanelsForUpdate(UpdateGroup.ExecutionSession);
      }
    }

    [Browsable(false)]
    [Obsolete("The report viewer no longer uses pressed buttons.")]
    [DefaultValue(typeof (Color), "#99BBE2")]
    public Color ToolBarItemPressedHoverBackColor
    {
      get
      {
        return this.GetViewStateProperty<Color>(nameof (ToolBarItemPressedHoverBackColor), Color.FromArgb(153, 187, 226));
      }
      set => this.ViewState[nameof (ToolBarItemPressedHoverBackColor)] = (object) value;
    }

    [Category("Appearance")]
    [WebBrowsable(true)]
    [Microsoft.Reporting.SRDescription("ToolBarItemHoverBackColorDesc")]
    [DefaultValue(typeof (Color), "#ECE9D8")]
    public Color SplitterBackColor
    {
      get => ((ReportViewerStyle) this.ControlStyle).SplitterBackColor;
      set
      {
        if (!(this.SplitterBackColor != value))
          return;
        this.EnsureUnlocked();
        ((ReportViewerStyle) this.ControlStyle).SplitterBackColor = value;
        this.m_panelUpdater.MarkPanelsForUpdate(UpdateGroup.ExecutionSession);
      }
    }

    [Microsoft.Reporting.SRDescription("LinkDisabledColorDesc")]
    [DefaultValue(typeof (Color), "Gray")]
    [Category("Appearance")]
    [WebBrowsable(true)]
    public Color LinkDisabledColor
    {
      get => ((ReportViewerStyle) this.ControlStyle).LinkDisabledColor;
      set
      {
        if (!(this.LinkDisabledColor != value))
          return;
        this.EnsureUnlocked();
        ((ReportViewerStyle) this.ControlStyle).LinkDisabledColor = value;
        this.m_panelUpdater.MarkPanelsForUpdate(UpdateGroup.ExecutionSession);
      }
    }

    [DefaultValue(typeof (Color), "#3366CC")]
    [Microsoft.Reporting.SRDescription("LinkActiveColorDesc")]
    [Category("Appearance")]
    [WebBrowsable(true)]
    public Color LinkActiveColor
    {
      get => ((ReportViewerStyle) this.ControlStyle).LinkActiveColor;
      set
      {
        if (!(this.LinkActiveColor != value))
          return;
        this.EnsureUnlocked();
        ((ReportViewerStyle) this.ControlStyle).LinkActiveColor = value;
        this.m_panelUpdater.MarkPanelsForUpdate(UpdateGroup.ExecutionSession);
      }
    }

    [Category("Appearance")]
    [Microsoft.Reporting.SRDescription("LinkActiveHoverColorDesc")]
    [DefaultValue(typeof (Color), "#FF3300")]
    [WebBrowsable(true)]
    public Color LinkActiveHoverColor
    {
      get => ((ReportViewerStyle) this.ControlStyle).LinkActiveHoverColor;
      set
      {
        if (!(this.LinkActiveHoverColor != value))
          return;
        this.EnsureUnlocked();
        ((ReportViewerStyle) this.ControlStyle).LinkActiveHoverColor = value;
        this.m_panelUpdater.MarkPanelsForUpdate(UpdateGroup.ExecutionSession);
      }
    }

    [Microsoft.Reporting.SRDescription("SizeToContentDesc")]
    [DefaultValue(false)]
    [Category("Appearance")]
    [WebBrowsable(true)]
    public bool SizeToReportContent
    {
      get => this.GetViewStateProperty<bool>(nameof (SizeToReportContent), false);
      set
      {
        this.SetUnlockedViewStateProperty(nameof (SizeToReportContent), (object) this.SizeToReportContent, (object) value, UpdateGroup.ExecutionSession);
      }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    [Microsoft.Reporting.SRDescription("ProcessingModeDesc")]
    [WebBrowsable(true)]
    [DefaultValue(ProcessingMode.Local)]
    public ProcessingMode ProcessingMode
    {
      get
      {
        return this.GetViewStateProperty<ProcessingMode>(nameof (ProcessingMode), ProcessingMode.Local);
      }
      set
      {
        if (this.ProcessingMode == value)
          return;
        if (this.IsDrillthrough)
          throw new InvalidOperationException();
        this.EnsureUnlocked();
        this.ViewState[nameof (ProcessingMode)] = (object) value;
        this.OnReportChanged((object) this, new ReportChangedEventArgs());
      }
    }

    [Microsoft.Reporting.SRDescription("ServerReportDesc")]
    [NotifyParentProperty(true)]
    [PersistenceMode(PersistenceMode.InnerProperty)]
    public ServerReport ServerReport => this.m_reportHierarchy.Peek().ServerReport;

    [NotifyParentProperty(true)]
    [Microsoft.Reporting.SRDescription("LocalReportDesc")]
    [PersistenceMode(PersistenceMode.InnerProperty)]
    public LocalReport LocalReport => this.m_reportHierarchy.Peek().LocalReport;

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int CurrentPage
    {
      get => this.m_reportHierarchy.Peek().CurrentPage;
      set => this.PerformPageNavigation(value, false);
    }

    [Microsoft.Reporting.SRDescription("PageCountModeDesc")]
    [DefaultValue(PageCountMode.Estimate)]
    [WebBrowsable(true)]
    public PageCountMode PageCountMode
    {
      get
      {
        return this.GetViewStateProperty<PageCountMode>(nameof (PageCountMode), PageCountMode.Estimate);
      }
      set
      {
        this.SetUnlockedViewStateProperty(nameof (PageCountMode), (object) this.PageCountMode, (object) value, UpdateGroup.Rerendering);
      }
    }

    [WebBrowsable(true)]
    [DefaultValue(ZoomMode.Percent)]
    [Microsoft.Reporting.SRDescription("ZoomModeDesc")]
    [Category("Appearance")]
    public ZoomMode ZoomMode
    {
      get => this.GetViewStateProperty<ZoomMode>(nameof (ZoomMode), ZoomMode.Percent);
      set
      {
        this.SetUnlockedViewStateProperty(nameof (ZoomMode), (object) this.ZoomMode, (object) value, UpdateGroup.Rerendering);
      }
    }

    [Microsoft.Reporting.SRDescription("ZoomPercentDesc")]
    [WebBrowsable(true)]
    [Category("Appearance")]
    [DefaultValue(100)]
    public int ZoomPercent
    {
      get => this.GetViewStateProperty<int>(nameof (ZoomPercent), 100);
      set
      {
        if (value <= 0)
          throw new ArgumentOutOfRangeException(nameof (value));
        this.SetUnlockedViewStateProperty(nameof (ZoomPercent), (object) this.ZoomPercent, (object) value, UpdateGroup.Rerendering);
      }
    }

    [WebBrowsable(true)]
    [Microsoft.Reporting.SRDescription("AsyncRenderingDesc")]
    [DefaultValue(true)]
    public bool AsyncRendering
    {
      get => this.GetViewStateProperty<bool>(nameof (AsyncRendering), true);
      set
      {
        if (!this.SetUnlockedViewStateProperty(nameof (AsyncRendering), (object) this.AsyncRendering, (object) value, UpdateGroup.Reprocessing))
          return;
        if (this.RenderingState == ReportRenderingState.AsyncWait && !value)
        {
          this.RenderingState = ReportRenderingState.Ready;
        }
        else
        {
          if (this.RenderingState != ReportRenderingState.Ready || !value)
            return;
          this.RenderingState = ReportRenderingState.AsyncWait;
        }
      }
    }

    [Microsoft.Reporting.SRDescription("WaitControlDisplayAfterDesc")]
    [WebBrowsable(true)]
    [DefaultValue(1000)]
    public int WaitControlDisplayAfter
    {
      get => this.GetViewStateProperty<int>(nameof (WaitControlDisplayAfter), 1000);
      set
      {
        if (value < 0)
          throw new ArgumentOutOfRangeException(nameof (value));
        this.SetUnlockedViewStateProperty(nameof (WaitControlDisplayAfter), (object) this.WaitControlDisplayAfter, (object) value, UpdateGroup.ExecutionSession);
      }
    }

    [Category("Appearance")]
    [WebBrowsable(true)]
    [Microsoft.Reporting.SRDescription("ShowWaitControlCancelLinkDesc")]
    [DefaultValue(true)]
    public bool ShowWaitControlCancelLink
    {
      get => this.GetViewStateProperty<bool>(nameof (ShowWaitControlCancelLink), true);
      set
      {
        this.SetUnlockedViewStateProperty(nameof (ShowWaitControlCancelLink), (object) this.ShowWaitControlCancelLink, (object) value, UpdateGroup.ExecutionSession);
      }
    }

    [Microsoft.Reporting.SRDescription("HyperlinkTargetDesc")]
    [DefaultValue("_top")]
    [WebBrowsable(true)]
    public string HyperlinkTarget
    {
      get => this.GetViewStateProperty<string>(nameof (HyperlinkTarget), "_top");
      set
      {
        this.SetUnlockedViewStateProperty(nameof (HyperlinkTarget), (object) this.HyperlinkTarget, (object) value, UpdateGroup.Rerendering);
      }
    }

    internal string ReplacementRoot
    {
      get => this.GetViewStateProperty<string>(nameof (ReplacementRoot), "");
      set
      {
        this.SetViewStateProperty(nameof (ReplacementRoot), (object) this.ReplacementRoot, (object) value, UpdateGroup.Rerendering);
      }
    }

    [Microsoft.Reporting.SRDescription("ContentDispositionDesc")]
    [WebBrowsable(true)]
    [DefaultValue(ContentDisposition.OnlyHtmlInline)]
    public ContentDisposition ExportContentDisposition
    {
      get
      {
        return this.GetViewStateProperty<ContentDisposition>(nameof (ExportContentDisposition), ContentDisposition.OnlyHtmlInline);
      }
      set
      {
        this.SetUnlockedViewStateProperty(nameof (ExportContentDisposition), (object) this.ExportContentDisposition, (object) value, UpdateGroup.ExecutionSession);
      }
    }

    [Microsoft.Reporting.SRDescription("InteractivityPostBackModeDesc")]
    [WebBrowsable(true)]
    [DefaultValue(InteractivityPostBackMode.AlwaysAsynchronous)]
    public InteractivityPostBackMode InteractivityPostBackMode
    {
      get
      {
        return this.GetViewStateProperty<InteractivityPostBackMode>(nameof (InteractivityPostBackMode), InteractivityPostBackMode.AlwaysAsynchronous);
      }
      set
      {
        this.SetUnlockedViewStateProperty(nameof (InteractivityPostBackMode), (object) this.InteractivityPostBackMode, (object) value, UpdateGroup.ExecutionSession);
      }
    }

    [DefaultValue(typeof (Unit), "400px")]
    public override Unit Height
    {
      get => base.Height;
      set
      {
        if (!(this.Height != value))
          return;
        this.EnsureUnlocked();
        base.Height = value;
        this.m_panelUpdater.MarkPanelsForUpdate(UpdateGroup.ExecutionSession);
      }
    }

    [DefaultValue(typeof (Unit), "400px")]
    public override Unit Width
    {
      get => base.Width;
      set
      {
        if (!(this.Width != value))
          return;
        this.EnsureUnlocked();
        base.Width = value;
        this.m_panelUpdater.MarkPanelsForUpdate(UpdateGroup.ExecutionSession);
      }
    }

    [Microsoft.Reporting.SRDescription("KeepSessionAliveDesc")]
    [WebBrowsable(true)]
    [DefaultValue(true)]
    public bool KeepSessionAlive
    {
      get => this.GetViewStateProperty<bool>(nameof (KeepSessionAlive), true);
      set
      {
        this.SetUnlockedViewStateProperty(nameof (KeepSessionAlive), (object) this.KeepSessionAlive, (object) value, UpdateGroup.ExecutionSession);
      }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ReportAreaContent ReportAreaContentType => this.m_contentType;

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public SearchState SearchState
    {
      get => (SearchState) this.ViewState[nameof (SearchState)];
      private set
      {
        if (this.SearchState == value)
          return;
        this.ViewState[nameof (SearchState)] = (object) value;
        this.m_alertMessage = (string) null;
        this.m_panelUpdater.MarkPanelsForUpdate(UpdateGroup.Rerendering);
      }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Browsable(false)]
    public DeviceInfoCollection InteractiveDeviceInfos
    {
      get
      {
        if (this.m_interactiveDeviceInfos == null)
          this.InteractiveDeviceInfos = new DeviceInfoCollection();
        return this.m_interactiveDeviceInfos;
      }
      private set
      {
        this.m_interactiveDeviceInfos = value != null ? value : throw new ArgumentNullException(nameof (value));
        this.m_interactiveDeviceInfos.DeviceInfoNameBlackList = ReportControl.GetDeviceInfoBlackList();
        this.m_interactiveDeviceInfos.EnsureUnlocked = new Microsoft.Reporting.WebForms.EnsureUnlocked(this.EnsureUnlocked);
      }
    }

    public void JumpToBookmark(string bookmarkId) => this.JumpToBookmark(bookmarkId, false);

    private void JumpToBookmark(string bookmarkId, bool fireEvents)
    {
      this.EnsureUnlocked();
      int newPage;
      ScrollTarget scrollTarget;
      if (!this.CreateActionHandler(fireEvents).HandleBookmarkNavigation(bookmarkId, out newPage, out scrollTarget))
        return;
      this.InternalSetCurrentPage(newPage, scrollTarget);
    }

    public void JumpToDocumentMapId(string documentMapId)
    {
      this.JumpToDocumentMapId(documentMapId, false);
    }

    private void JumpToDocumentMapId(string documentMapId, bool fireEvents)
    {
      this.EnsureUnlocked();
      int newPage;
      ScrollTarget scrollTarget;
      if (!this.CreateActionHandler(fireEvents).HandleDocMapNavigation(documentMapId, out newPage, out scrollTarget))
        return;
      this.InternalSetCurrentPage(newPage, scrollTarget);
    }

    public void PerformBack()
    {
      this.EnsureUnlocked();
      this.PerformBack(false);
    }

    private void PerformBack(bool fireEvents)
    {
      if (!this.IsDrillthrough)
        throw new InvalidOperationException(CommonStrings.NotInDrillthrough);
      bool flag = false;
      if (this.Back != null && fireEvents)
      {
        ReportInfo reportInfo = this.m_reportHierarchy.ToArray()[1];
        BackEventArgs e = new BackEventArgs(this.ProcessingMode != ProcessingMode.Local ? (Report) reportInfo.ServerReport : (Report) reportInfo.LocalReport);
        this.Back((object) this, e);
        flag = e.Cancel;
      }
      if (flag)
        return;
      this.m_reportHierarchy.Pop().DisconnectChangeEvent(this.m_changeHandler, this.m_dataInitializationHandler, false);
      this.m_reportHasChanged = ReportChangeType.Back;
      this.ScrollTarget = new ScrollTarget(this.m_reportHierarchy.Peek().ScrollPosition);
    }

    private void PerformPageNavigation(int targetPage, bool fireEvents)
    {
      if (targetPage == this.CurrentPage || !this.CreateActionHandler(fireEvents).HandlePageNavigation(targetPage))
        return;
      this.InternalSetCurrentPage(targetPage, (ScrollTarget) null);
    }

    private void OnClientSideZoomChanged(object sender, ZoomChangeEventArgs e)
    {
      this.ZoomMode = e.ZoomMode;
      this.ZoomPercent = e.ZoomPercent;
    }

    internal void OnReportAction(object sender, ReportActionEventArgs e)
    {
      try
      {
        if (this.ReportHasChanged)
          return;
        string actionType = e.ActionType;
        string actionParam = e.ActionParam;
        if (string.Equals(actionType, "Toggle", StringComparison.Ordinal))
          this.PerformToggle(actionParam);
        else if (string.Equals(actionType, "Bookmark", StringComparison.Ordinal))
          this.JumpToBookmark(actionParam, true);
        else if (string.Equals(actionType, "Sort", StringComparison.Ordinal))
          this.PerformSort(actionParam);
        else if (string.Equals(actionType, "Refresh", StringComparison.Ordinal))
          this.PerformRefresh(string.Equals(actionParam, "auto", StringComparison.Ordinal));
        else if (string.Equals(actionType, "Drillthrough", StringComparison.Ordinal))
          this.PerformDrillthrough(actionParam);
        else if (string.Equals(actionType, "Find", StringComparison.Ordinal))
          this.Find(actionParam, this.CurrentPage);
        else if (string.Equals(actionType, "FindNext", StringComparison.Ordinal))
          this.FindNext();
        else if (string.Equals(actionType, "PageNav", StringComparison.Ordinal))
        {
          this.PerformPageNavigation(int.Parse(actionParam, (IFormatProvider) CultureInfo.InvariantCulture), true);
        }
        else
        {
          if (!string.Equals(actionType, "Back", StringComparison.Ordinal))
            return;
          this.PerformBack(true);
        }
      }
      catch (Exception ex)
      {
        this.OnError(ex);
      }
    }

    private void OnDocumentMapNavigation(object sender, DocumentMapNavigationEventArgs e)
    {
      if (this.ReportHasChanged)
        return;
      this.JumpToDocumentMapId(e.DocumentMapId, true);
    }

    private void PerformRefresh(bool isAutoRefresh)
    {
      ReportActionHandler actionHandler = this.CreateActionHandler(true);
      int currentPage = this.CurrentPage;
      if (!actionHandler.HandleRefresh())
        return;
      this.ClientCanceledRendering = false;
      if (isAutoRefresh)
      {
        ScrollTarget scrollTarget = new ScrollTarget(this.m_reportArea.ClientScrollPosition);
        this.InternalSetCurrentPage(currentPage, scrollTarget);
      }
      this.m_panelUpdater.MarkPanelsForUpdate(UpdateGroup.Reprocessing);
    }

    public void Find(string searchText, int startPage)
    {
      this.EnsureUnlocked();
      SearchState searchState = new SearchState(searchText, startPage);
      int newPage;
      ScrollTarget scrollTarget;
      this.HandleSearchResult(this.CreateActionHandler(true).HandleSearch(searchState, out newPage, out scrollTarget), searchState, false, newPage, scrollTarget);
    }

    private void FindNext()
    {
      SearchState searchState = this.SearchState;
      if (searchState == null)
        return;
      int newPage;
      ScrollTarget scrollTarget;
      this.HandleSearchResult(this.CreateActionHandler(true).HandleSearchNext(searchState, out newPage, out scrollTarget), searchState, true, newPage, scrollTarget);
    }

    private void HandleSearchResult(
      SearchResult result,
      SearchState searchState,
      bool searchNext,
      int newPage,
      ScrollTarget scrollTarget)
    {
      if (result == SearchResult.Cancelled)
        return;
      if (newPage == 0)
        newPage = searchState.StartPage;
      this.InternalSetCurrentPage(newPage, scrollTarget);
      if (result == SearchResult.FoundMoreHits)
        this.SearchState = searchState;
      else if (searchNext)
        this.m_alertMessage = LocalizationHelper.Current.NoMoreMatches;
      else
        this.m_alertMessage = LocalizationHelper.Current.TextNotFound;
    }

    private void PerformToggle(string toggleId)
    {
      ScrollTarget scrollTarget;
      if (!this.CreateActionHandler(true).HandleToggle(toggleId, out scrollTarget))
        return;
      this.InternalSetCurrentPage(this.CurrentPage, scrollTarget);
    }

    private void PerformDrillthrough(string drillthroughId)
    {
      Report report = this.CreateActionHandler(true).HandleDrillthrough(drillthroughId);
      if (report == null)
        return;
      LocalModeSession localSession;
      ServerReport serverReport;
      if (this.ProcessingMode == ProcessingMode.Local)
      {
        localSession = new LocalModeSession((LocalReport) report);
        serverReport = new ServerReport(this.ServerReport);
      }
      else
      {
        localSession = new LocalModeSession();
        serverReport = (ServerReport) report;
      }
      this.m_reportHierarchy.Peek().ScrollPosition = this.m_reportArea.ClientScrollPosition;
      ReportInfo reportInfo = new ReportInfo(localSession, new ServerModeSession(serverReport));
      reportInfo.ConnectChangeEvent(this.m_changeHandler, this.m_dataInitializationHandler);
      this.m_reportHierarchy.Push(reportInfo);
      this.OnReportChanged((object) this, new ReportChangedEventArgs());
    }

    private void PerformSort(string clientSortAction)
    {
      int pageNumber;
      ScrollTarget scrollTarget;
      if (!this.CreateActionHandler(true).HandleSort(clientSortAction, out pageNumber, out scrollTarget))
        return;
      if (pageNumber > 0)
        this.InternalSetCurrentPage(pageNumber, scrollTarget);
      else
        this.InternalSetCurrentPage(this.CurrentPage, (ScrollTarget) null);
      this.m_panelUpdater.MarkPanelsForUpdate(UpdateGroup.Reprocessing);
    }

    private ReportActionHandler CreateActionHandler(bool fireEvents)
    {
      this.EnsureUnlocked();
      return new ReportActionHandler(this.Report, (object) this, this.CurrentPage, this.PageCountMode, fireEvents ? this.PageNavigation : (PageNavigationEventHandler) null, fireEvents ? this.Toggle : (CancelEventHandler) null, fireEvents ? this.BookmarkNavigation : (BookmarkNavigationEventHandler) null, fireEvents ? this.DocumentMapNavigation : (DocumentMapNavigationEventHandler) null, fireEvents ? this.Drillthrough : (DrillthroughEventHandler) null, fireEvents ? this.Sort : (SortEventHandler) null, fireEvents ? this.Search : (SearchEventHandler) null, fireEvents ? this.ReportRefresh : (CancelEventHandler) null);
    }

    public void Reset()
    {
      this.OnReportChanged((object) this, new ReportChangedEventArgs());
      this.CreateReportHierarchy();
    }

    [Microsoft.Reporting.SRDescription("PageNavigationEventDesc")]
    public event PageNavigationEventHandler PageNavigation;

    [Microsoft.Reporting.SRDescription("BackEventDesc")]
    public event BackEventHandler Back;

    [Microsoft.Reporting.SRDescription("DocMapEventDesc")]
    public event DocumentMapNavigationEventHandler DocumentMapNavigation;

    [Microsoft.Reporting.SRDescription("BookmarkEventDesc")]
    public event BookmarkNavigationEventHandler BookmarkNavigation;

    [Microsoft.Reporting.SRDescription("ToggleEventDesc")]
    public event CancelEventHandler Toggle;

    [Microsoft.Reporting.SRDescription("DrillthroughEventDesc")]
    public event DrillthroughEventHandler Drillthrough;

    [Microsoft.Reporting.SRDescription("SortEventDesc")]
    public event SortEventHandler Sort;

    [Microsoft.Reporting.SRDescription("SearchEventDesc")]
    public event SearchEventHandler Search;

    [Microsoft.Reporting.SRDescription("RefreshEventDesc")]
    public event CancelEventHandler ReportRefresh;

    [Microsoft.Reporting.SRDescription("ErrorEventDesc")]
    public event ReportErrorEventHandler ReportError;

    internal void OnError(object sender, ReportErrorEventArgs e) => this.OnError(e.Exception);

    internal void OnError(Exception e)
    {
      this.EnsureChildControls();
      ReportErrorEventArgs e1 = new ReportErrorEventArgs(e);
      if (this.ReportError != null)
        this.ReportError((object) this, e1);
      this.m_panelUpdater.MarkPanelsForUpdate(UpdateGroup.Reprocessing);
      this.m_hasErrorsOnThisPostBack = true;
      this.m_reportArea.SetException(e, e1.Handled);
      if (this.m_contentType == ReportAreaContent.Unknown)
        return;
      this.m_contentType = ReportAreaContent.Error;
    }

    [Microsoft.Reporting.SRDescription("SubmittingDataSourceCredentialsEventDesc")]
    public event ReportCredentialsEventHandler SubmittingDataSourceCredentials;

    private void OnSubmittingDataSourceCredentials(
      object sender,
      ReportCredentialsEventArgs credentialArgs)
    {
      if (this.SubmittingDataSourceCredentials == null)
        return;
      this.SubmittingDataSourceCredentials((object) this, credentialArgs);
    }

    [Microsoft.Reporting.SRDescription("SubmittingParameterValuesEventDesc")]
    public event ReportParametersEventHandler SubmittingParameterValues;

    private void OnSubmittingParameterValues(object sender, ReportParametersEventArgs parameterArgs)
    {
      if (this.SubmittingParameterValues == null)
        return;
      this.SubmittingParameterValues((object) this, parameterArgs);
    }

    public override string ID
    {
      get => base.ID;
      set
      {
        this.EnsureUnlocked();
        base.ID = value;
      }
    }

    internal PageSettings PageSettings
    {
      get => this.m_reportHierarchy.Peek().PageSettings ?? this.ResetAndGetPageSettings();
    }

    public PageSettings GetPageSettings()
    {
      return ReportViewerUtils.DeepClonePageSettings(this.PageSettings);
    }

    public void ResetPageSettings() => this.ResetAndGetPageSettings();

    private PageSettings ResetAndGetPageSettings()
    {
      PageSettings pageSettings;
      try
      {
        pageSettings = this.Report.GetDefaultPageSettings().CustomPageSettings;
      }
      catch (MissingReportSourceException ex)
      {
        pageSettings = (PageSettings) null;
      }
      this.m_reportHierarchy.Peek().PageSettings = pageSettings;
      return pageSettings;
    }

    public void SetPageSettings(PageSettings pageSettings)
    {
      this.m_reportHierarchy.Peek().PageSettings = pageSettings != null ? ReportViewerUtils.DeepClonePageSettings(pageSettings) : throw new ArgumentNullException(nameof (pageSettings));
    }

    private void CreateReportHierarchy()
    {
      this.DisconnectReportHierarchy(true);
      this.m_reportHierarchy = new ReportHierarchy(this.CreateServerReport());
      this.m_reportHierarchy.ConnectChangeEvents(this.m_changeHandler, this.m_dataInitializationHandler);
    }

    private void DisconnectReportHierarchy(bool shouldDispose)
    {
      if (this.m_reportHierarchy == null)
        return;
      this.m_reportHierarchy.DisconnectChangeEvents(this.m_changeHandler, this.m_dataInitializationHandler);
      if (shouldDispose)
        this.m_reportHierarchy.Dispose();
      else
        this.m_reportHierarchy.DisposeNonSessionResources();
      this.m_reportHierarchy = (ReportHierarchy) null;
    }

    public void RegisterPostBackControl(Control control)
    {
      if (control == null)
        throw new ArgumentNullException(nameof (control));
      this.EnsureChildControls();
      this.m_asyncWaitControl.Triggers.Add(control);
    }

    private bool IsDrillthrough => this.Report.IsDrillthroughReport;

    protected override System.Web.UI.WebControls.Style CreateControlStyle()
    {
      return (System.Web.UI.WebControls.Style) new ReportViewerStyle(this.ViewState);
    }

    private ReportRenderingState RenderingState
    {
      get
      {
        object obj = this.ViewState[nameof (RenderingState)];
        return obj == null ? ReportRenderingState.NotReady : (ReportRenderingState) obj;
      }
      set
      {
        if (this.RenderingState == value)
          return;
        this.ViewState[nameof (RenderingState)] = (object) value;
        this.m_panelUpdater.MarkPanelsForUpdate(UpdateGroup.Reprocessing);
      }
    }

    private bool ClientCanceledRendering
    {
      get => this.GetViewStateProperty<bool>(nameof (ClientCanceledRendering), false);
      set
      {
        this.SetViewStateProperty(nameof (ClientCanceledRendering), (object) this.ClientCanceledRendering, (object) value, UpdateGroup.Reprocessing);
      }
    }

    protected override void CreateChildControls()
    {
      this.Controls.Clear();
      this.m_panelUpdater.UnregisterAllPanels();
      if (!this.DesignMode && (this.Page == null || ScriptManager.GetCurrent(this.Page) == null))
        throw new ScriptManagerNotFoundException();
      this.m_noScriptControl = new NoScriptControl();
      this.m_noScriptControl.Visible = !this.DesignMode;
      this.Controls.Add((Control) this.m_noScriptControl);
      this.m_topLevelUpdatePanel = new UpdatePanel();
      this.m_topLevelUpdatePanel.UpdateMode = UpdatePanelUpdateMode.Conditional;
      this.m_topLevelUpdatePanel.RenderMode = UpdatePanelRenderMode.Block;
      this.m_topLevelUpdatePanel.ChildrenAsTriggers = false;
      this.m_topLevelUpdatePanel.ID = nameof (ReportViewer);
      this.Controls.Add((Control) this.m_topLevelUpdatePanel);
      this.m_panelUpdater.RegisterPanel(this.m_topLevelUpdatePanel, UpdateGroup.ExecutionSession);
      DelegatedRenderingControl child = new DelegatedRenderingControl(new DelegatedRenderingControl.RenderDelegate(this.RenderTopLevelUpdatePanelContents));
      this.m_topLevelUpdatePanel.ContentTemplateContainer.Controls.Add((Control) child);
      this.m_clientScript = new ReportViewerClientScript();
      this.m_clientScript.ReportAction += new EventHandler<ReportActionEventArgs>(this.OnReportAction);
      child.Controls.Add((Control) this.m_clientScript);
      this.m_parametersArea = new ParametersArea(this);
      this.m_parametersArea.ViewReportClick += new EventHandler(this.OnViewReport);
      this.m_parametersArea.ParameterValuesChanged += new EventHandler(this.OnParameterValuesChanged);
      this.m_parametersArea.SubmittingDataSourceCredentials += new ReportCredentialsEventHandler(this.OnSubmittingDataSourceCredentials);
      this.m_parametersArea.SubmittingParameterValues += new ReportParametersEventHandler(this.OnSubmittingParameterValues);
      child.Controls.Add((Control) this.m_parametersArea);
      this.m_parametersAreaSplitter = new ReportSplitter(this.ViewerStyle, false, LocalizationHelper.Current.ParameterAreaButtonToolTip);
      this.m_parametersAreaSplitter.IsResizable = false;
      this.m_parametersAreaSplitter.ID = "ToggleParam";
      child.Controls.Add((Control) this.m_parametersAreaSplitter);
      this.m_toolbarArea = new ToolbarControl(this);
      this.m_toolbarArea.ReportAction += new EventHandler<ReportActionEventArgs>(this.OnReportAction);
      child.Controls.Add((Control) this.m_toolbarArea);
      this.m_docMapUpdatePanel = new UpdatePanel();
      this.m_docMapUpdatePanel.UpdateMode = UpdatePanelUpdateMode.Conditional;
      this.m_docMapUpdatePanel.RenderMode = UpdatePanelRenderMode.Block;
      this.m_docMapUpdatePanel.ChildrenAsTriggers = false;
      this.m_docMapUpdatePanel.ID = "DocMap";
      child.Controls.Add((Control) this.m_docMapUpdatePanel);
      this.m_panelUpdater.RegisterPanel(this.m_docMapUpdatePanel, UpdateGroup.Reprocessing);
      this.m_docMapAreaSplitter = new ReportSplitter(this.ViewerStyle, true, LocalizationHelper.Current.DocumentMapButtonToolTip);
      child.Controls.Add((Control) this.m_docMapAreaSplitter);
      this.m_docMapArea = new DocMapArea(this);
      this.m_docMapArea.NodeClick += new DocumentMapNavigationEventHandler(this.OnDocumentMapNavigation);
      this.m_docMapUpdatePanel.ContentTemplateContainer.Controls.Add((Control) this.m_docMapArea);
      this.m_reportArea = new ReportArea(this.ViewerStyle);
      this.m_reportArea.ReportAction += new EventHandler<ReportActionEventArgs>(this.OnReportAction);
      this.m_reportArea.ZoomChanged += new ZoomChangedEventHandler(this.OnClientSideZoomChanged);
      this.m_reportArea.AsyncLoadRequested += new EventHandler(this.OnAsyncLoadReport);
      if (ScriptManager.GetCurrent(this.Page) != null)
        child.Controls.Add((Control) this.m_reportArea);
      this.m_httpHandlerMissingError = new HttpHandlerMissingErrorMessage();
      this.m_httpHandlerMissingError.ID = "HttpHandlerMissingErrorMessage";
      child.Controls.Add((Control) this.m_httpHandlerMissingError);
      this.m_docMapAreaSplitter.CollapsedChanged += new EventHandler(this.ClientSideDocMapAreaVisibilityChanged);
      this.m_parametersAreaSplitter.CollapsedChanged += new EventHandler(this.ClientSidePromptAreaVisibilityChanged);
      this.m_direction = new HiddenField();
      child.Controls.Add((Control) this.m_direction);
      this.m_browserMode = new HiddenField();
      child.Controls.Add((Control) this.m_browserMode);
      this.m_asyncWaitControl = new AsyncWaitControl(this.ViewerStyle);
      this.m_asyncWaitControl.ID = "AsyncWait";
      this.m_asyncWaitControl.ClientCanceledStateChanged += new EventHandler<ClientCanceledStateChangeEventArgs>(this.OnClientCanceledStateChanged);
      this.m_asyncWaitControl.Triggers.Add((Control) this);
      child.Controls.Add((Control) this.m_asyncWaitControl);
    }

    private void OnClientCanceledStateChanged(object sender, ClientCanceledStateChangeEventArgs e)
    {
      this.ClientCanceledRendering = e.ClientCanceled;
    }

    private bool IsFullControlRendering
    {
      get
      {
        if (!ScriptManager.GetCurrent(this.Page).IsInAsyncPostBack)
          return true;
        for (Control parent = this.Parent; parent != null; parent = parent.Parent)
        {
          if (parent is UpdatePanel updatePanel && updatePanel.IsInPartialRendering)
            return true;
        }
        return false;
      }
    }

    internal void Update()
    {
      this.EnsureChildControls();
      this.m_panelUpdater.MarkPanelsForUpdate(UpdateGroup.ExecutionSession);
    }

    protected override void Render(HtmlTextWriter writer)
    {
      this.EnsureChildControls();
      using (MonitoredScope.New("ReportViewerControl.Render"))
      {
        if (!this.DesignMode)
        {
          ScriptManager.GetCurrent(this.Page)?.RegisterScriptDescriptors((IScriptControl) this);
          if (!this.m_viewStateSaved)
            this.OnError((Exception) new ViewStateDisabledException());
          ((ReportViewerStyle) this.ControlStyle).ObeySizeProperties = !this.SizeToReportContent;
          this.m_noScriptControl.RenderControl(writer);
          if (this.IsFullControlRendering && this.RenderingState == ReportRenderingState.Completed && !this.m_panelUpdater.IsPanelGroupMarkedForUpdate(UpdateGroup.Rerendering))
          {
            this.m_reportArea.SetFullViewerRendering();
            try
            {
              this.RenderReport(true);
            }
            catch (Exception ex)
            {
              this.OnError(ex);
            }
          }
          this.m_topLevelUpdatePanel.RenderControl(writer);
        }
        else
          this.RenderTopLevelUpdatePanelContents(writer);
      }
    }

    private bool RenderTopLevelUpdatePanelContents(HtmlTextWriter writer)
    {
      this.AddAttributesToRender(writer);
      this.m_parametersArea.RenderCloseDropDownAttributes(writer);
      writer.RenderBeginTag(HtmlTextWriterTag.Div);
      this.m_httpHandlerMissingError.RenderControl(writer);
      this.m_clientScript.RenderControl(writer);
      this.m_direction.RenderControl(writer);
      this.m_browserMode.RenderControl(writer);
      if (!this.DesignMode)
        this.m_asyncWaitControl.RenderControl(writer);
      writer.AddAttribute(HtmlTextWriterAttribute.Cellpadding, "0");
      writer.AddAttribute(HtmlTextWriterAttribute.Cellspacing, "0");
      writer.AddAttribute(HtmlTextWriterAttribute.Id, this.FixedTableID);
      if (!this.SizeToReportContent)
      {
        writer.AddStyleAttribute("table-layout", "fixed");
        if (!this.Width.IsEmpty)
          writer.AddStyleAttribute(HtmlTextWriterStyle.Width, "100%");
        if (!this.Height.IsEmpty && !this.DesignMode)
          writer.AddStyleAttribute(HtmlTextWriterStyle.Height, "100%");
      }
      writer.RenderBeginTag(HtmlTextWriterTag.Table);
      bool flag1 = this.ReportAreaContentType == ReportAreaContent.ReportPage && this.m_docMapArea.RootNode != null && !this.DocumentMapCollapsed;
      bool visible = this.m_toolbarArea.Visible;
      bool flag2 = !this.PromptAreaCollapsed && this.m_parametersArea.Visible;
      if (!this.SizeToReportContent)
      {
        if (visible || flag2)
        {
          if (this.ViewerStyle.ViewerAreaBackground != null)
            writer.AddAttribute(HtmlTextWriterAttribute.Class, this.ViewerStyle.ViewerAreaBackground);
          else
            writer.AddStyleAttribute(HtmlTextWriterStyle.BackgroundColor, ColorTranslator.ToHtml(this.ControlStyle.BackColor));
        }
        writer.RenderBeginTag(HtmlTextWriterTag.Tr);
        if (!flag1)
          writer.AddStyleAttribute(HtmlTextWriterStyle.Display, "none");
        writer.AddStyleAttribute(HtmlTextWriterStyle.Width, this.DocumentMapWidth.ToString(CultureInfo.InvariantCulture));
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
      if (!this.DesignMode)
      {
        if (!flag2)
          writer.AddStyleAttribute(HtmlTextWriterStyle.Display, "none");
        writer.AddAttribute(HtmlTextWriterAttribute.Id, this.ParametersRowID);
        writer.RenderBeginTag(HtmlTextWriterTag.Tr);
        writer.AddAttribute(HtmlTextWriterAttribute.Colspan, "3");
        writer.RenderBeginTag(HtmlTextWriterTag.Td);
        if (!this.SizeToReportContent)
        {
          this.m_parametersArea.Style[HtmlTextWriterStyle.Width] = "100%";
          this.m_parametersArea.Style[HtmlTextWriterStyle.OverflowX] = "auto";
          this.m_parametersArea.Style[HtmlTextWriterStyle.OverflowY] = "hidden";
        }
        this.m_parametersArea.RenderControl(writer);
        writer.RenderEndTag();
        writer.RenderEndTag();
        writer.AddStyleAttribute(HtmlTextWriterStyle.Height, "6px");
        writer.AddStyleAttribute(HtmlTextWriterStyle.FontSize, "2pt");
        if (!this.ShouldRenderPromptAreaSplitter)
          writer.AddStyleAttribute(HtmlTextWriterStyle.Display, "none");
        writer.RenderBeginTag(HtmlTextWriterTag.Tr);
        writer.AddAttribute(HtmlTextWriterAttribute.Colspan, "3");
        writer.AddStyleAttribute(HtmlTextWriterStyle.Padding, "0px");
        writer.AddStyleAttribute(HtmlTextWriterStyle.Margin, "0px");
        this.m_parametersAreaSplitter.WriteTableCellCenteringStyles(writer);
        writer.RenderBeginTag(HtmlTextWriterTag.Td);
        this.m_parametersAreaSplitter.RenderControl(writer);
        writer.RenderEndTag();
        writer.RenderEndTag();
      }
      if (!visible)
        writer.AddStyleAttribute(HtmlTextWriterStyle.Display, "none");
      writer.RenderBeginTag(HtmlTextWriterTag.Tr);
      if (visible)
      {
        writer.AddAttribute(HtmlTextWriterAttribute.Colspan, "3");
        writer.RenderBeginTag(HtmlTextWriterTag.Td);
        this.m_toolbarArea.RenderControl(writer);
        writer.RenderEndTag();
      }
      writer.RenderEndTag();
      if (!this.DesignMode)
      {
        writer.RenderBeginTag(HtmlTextWriterTag.Tr);
        writer.AddStyleAttribute(HtmlTextWriterStyle.VerticalAlign, "top");
        if (!this.SizeToReportContent)
        {
          writer.AddStyleAttribute(HtmlTextWriterStyle.Width, this.DocumentMapWidth.ToString(CultureInfo.InvariantCulture));
          writer.AddStyleAttribute(HtmlTextWriterStyle.Height, "100%");
        }
        if (!flag1)
          writer.AddStyleAttribute(HtmlTextWriterStyle.Display, "none");
        writer.RenderBeginTag(HtmlTextWriterTag.Td);
        if (!this.SizeToReportContent)
        {
          writer.AddStyleAttribute(HtmlTextWriterStyle.Width, "100%");
          writer.AddStyleAttribute(HtmlTextWriterStyle.Height, "100%");
        }
        writer.RenderBeginTag(HtmlTextWriterTag.Div);
        this.m_docMapUpdatePanel.RenderControl(writer);
        writer.RenderEndTag();
        writer.RenderEndTag();
        writer.AddStyleAttribute(HtmlTextWriterStyle.Display, "none");
        writer.AddStyleAttribute(HtmlTextWriterStyle.Width, "4px");
        writer.AddStyleAttribute(HtmlTextWriterStyle.Padding, "0px");
        writer.AddStyleAttribute(HtmlTextWriterStyle.Margin, "0px");
        writer.AddStyleAttribute(HtmlTextWriterStyle.Height, "100%");
        this.m_docMapAreaSplitter.WriteTableCellCenteringStyles(writer);
        writer.RenderBeginTag(HtmlTextWriterTag.Td);
        this.m_docMapAreaSplitter.RenderControl(writer);
        writer.RenderEndTag();
        if (!this.SizeToReportContent)
          writer.AddStyleAttribute(HtmlTextWriterStyle.Height, "100%");
        writer.AddStyleAttribute(HtmlTextWriterStyle.VerticalAlign, "top");
        writer.RenderBeginTag(HtmlTextWriterTag.Td);
        if (!this.SizeToReportContent)
        {
          this.m_reportArea.Width = Unit.Percentage(100.0);
          this.m_reportArea.Height = Unit.Percentage(100.0);
          this.m_reportArea.Style.Add(HtmlTextWriterStyle.Overflow, "auto");
        }
        this.m_reportArea.Style.Add(HtmlTextWriterStyle.Position, "relative");
        this.m_reportArea.RenderControl(writer);
        writer.RenderEndTag();
        writer.RenderEndTag();
      }
      writer.RenderEndTag();
      writer.RenderEndTag();
      return false;
    }

    private void RenderReport(bool updateDocMap)
    {
      string browserMode = (string) null;
      if (this.m_browserMode != null)
        browserMode = this.m_browserMode.Value;
      int pageNumber = this.m_reportArea.RenderReport(this.ReportControlSession, this.InstanceIdentifier, this.PageCountMode, this.CurrentPage, this.InteractivityPostBackMode, this.SearchState, this.ReplacementRoot, this.HyperlinkTarget, this.ScrollTarget, this.m_alertMessage, this.InteractiveDeviceInfos, browserMode, this.SizeToReportContent);
      this.ScrollTarget = (ScrollTarget) null;
      if (updateDocMap && this.Report.HasDocMap)
        this.m_docMapArea.RootNode = this.Report.GetDocumentMap();
      if (pageNumber == this.CurrentPage)
        return;
      this.InternalSetCurrentPage(pageNumber, (ScrollTarget) null);
    }

    protected override void OnInit(EventArgs e)
    {
      if (!this.DesignMode && this.ChildControlsCreated)
        throw new Exception("Internal error: ClientID reference before OnInit");
      base.OnInit(e);
    }

    private void SetToLegacyClientIDMode()
    {
      PropertyInfo clientIDModeProp = (PropertyInfo) null;
      SecurityAssertionHandler.RunWithSecurityAssert((CodeAccessPermission) new ReflectionPermission(ReflectionPermissionFlag.NoFlags), (Action) (() => clientIDModeProp = this.GetType().GetProperty("ClientIDMode")));
      if (clientIDModeProp == null)
        return;
      Type propertyType = clientIDModeProp.PropertyType;
      if (propertyType == null)
        return;
      clientIDModeProp.SetValue((object) this, System.Enum.Parse(propertyType, "AutoID"), (object[]) null);
    }

    protected override void OnPreRender(EventArgs e)
    {
      using (MonitoredScope.New("ReportViewerControl.OnPreRender"))
      {
        this.SetToLegacyClientIDMode();
        this.EnsureChildControls();
        ScriptManager current = ScriptManager.GetCurrent(this.Page);
        current?.RegisterScriptControl<ReportViewer>(this);
        ClientPrintInfo clientPrintInfo = (ClientPrintInfo) null;
        try
        {
          base.OnPreRender(e);
          if (!string.Equals(this.m_originalWaitMessageFont, ReportViewerStyle.GetHtmlStyleForFont(this.WaitMessageFont), StringComparison.Ordinal))
            this.m_panelUpdater.MarkPanelsForUpdate(UpdateGroup.ExecutionSession);
          this.m_noScriptControl.AlternateUrl = this.ServerReport.ReportUrlNoScript;
          if (this.m_hasErrorsOnThisPostBack)
            return;
          this.m_reportHierarchy.Peek().ScrollPosition = (string) null;
          if (this.ReportHasChanged)
          {
            this.SearchState = (SearchState) null;
            this.m_reportArea.Clear();
            if (this.m_reportHasChanged != ReportChangeType.Refresh)
            {
              this.m_parametersArea.RefreshControlsFromReportMetadata();
              this.m_panelUpdater.MarkPanelsForUpdate(UpdateGroup.ExecutionSession);
            }
            if (this.m_reportHasChanged == ReportChangeType.ReportObject || this.m_reportHasChanged == ReportChangeType.Refresh)
              this.RenderingState = ReportRenderingState.Preparing;
            else if (this.m_reportHasChanged == ReportChangeType.Back)
              this.RenderingState = ReportRenderingState.Completed;
          }
          else if (this.m_userParamsChanged || this.m_viewReportClicked)
          {
            this.SearchState = (SearchState) null;
            this.m_reportArea.Clear();
            bool report = this.m_parametersArea.SaveControlValuesToReport();
            if (this.m_viewReportClicked && report)
            {
              this.PerformRefresh(false);
              this.RenderingState = ReportRenderingState.Preparing;
            }
            else
              this.RenderingState = ReportRenderingState.NotReady;
          }
          this.m_lockChanges = true;
          if (this.m_hasErrorsOnThisPostBack)
            return;
          if (this.InteractivityPostBackMode == InteractivityPostBackMode.AlwaysSynchronous)
          {
            current.RegisterPostBackControl((Control) this.m_docMapArea);
            current.RegisterPostBackControl((Control) this.m_clientScript);
            current.RegisterPostBackControl((Control) this.m_parametersArea);
            current.RegisterPostBackControl((Control) this.m_toolbarArea);
          }
          if (this.Report.IsReadyForConnection && this.ShowReportBody)
          {
            if (!this.ShouldRenderPromptAreaSplitter && this.PromptAreaCollapsed)
              this.m_parametersArea.ValidateAllReportInputsSatisfied();
            else
              this.m_parametersArea.ValidateNonVisibleReportInputsSatisfied();
            this.ValidateAllDataSourcesSatisfied();
          }
          bool flag = this.Report.PrepareForRender();
          if (this.RenderingState == ReportRenderingState.Preparing && !this.DesignMode)
          {
            if (flag)
            {
              if (this.ProcessingMode == ProcessingMode.Local)
                this.InitializeDataSources(this.LocalReport.DataSources);
              if (this.CurrentPage == 0)
                this.InternalSetCurrentPage(1, (ScrollTarget) null);
              this.RenderingState = ReportRenderingState.Pending;
            }
            else
              this.RenderingState = ReportRenderingState.NotReady;
          }
          if (this.ShowReportBody && !this.ClientCanceledRendering)
          {
            if (this.RenderingState == ReportRenderingState.Pending)
              this.RenderingState = !this.AsyncRendering || this.m_reportHasChanged == ReportChangeType.Refresh ? ReportRenderingState.Ready : ReportRenderingState.AsyncWait;
          }
          else if (this.RenderingState == ReportRenderingState.AsyncWait || this.RenderingState == ReportRenderingState.Ready)
            this.RenderingState = ReportRenderingState.Pending;
          if (this.WillRenderReport)
          {
            if (this.m_panelUpdater.IsPanelGroupMarkedForUpdate(UpdateGroup.Rerendering))
            {
              this.RenderingState = ReportRenderingState.Completed;
              this.RenderReport(this.m_panelUpdater.IsPanelGroupMarkedForUpdate(UpdateGroup.Reprocessing));
              if (this.Report.HasDocMap && !this.SizeToReportContent && this.m_panelUpdater.IsPanelGroupMarkedForUpdate(UpdateGroup.Reprocessing))
                this.m_docMapArea.RootNode = this.Report.GetDocumentMap();
            }
          }
          else if (this.RenderingState == ReportRenderingState.AsyncWait)
          {
            if (this.ProcessingMode == ProcessingMode.Local && !this.LocalReport.SupportsQueries)
              this.LocalReport.CreateSnapshot();
            this.m_reportArea.SetForAsyncRendering();
          }
          clientPrintInfo = new ClientPrintInfo(this);
        }
        catch (Exception ex)
        {
          this.OnError(ex);
        }
        finally
        {
          if (this.m_reportArea.ReportAreaContent == ReportAreaContent.Error && !this.m_hasErrorsOnThisPostBack)
            this.m_reportArea.Clear();
          this.m_contentType = this.m_reportArea.ReportAreaContent;
          this.m_toolbarArea.Visible = this.ShouldRenderToolbar;
          this.m_parametersArea.Visible = this.ShouldRenderPromptArea;
          this.m_parametersAreaSplitter.IsCollapsed = this.PromptAreaCollapsed;
          this.m_parametersAreaSplitter.IsCollapsable = this.ShowPromptAreaButton;
          this.m_docMapAreaSplitter.IsCollapsed = this.DocumentMapCollapsed;
          this.m_docMapAreaSplitter.IsCollapsable = this.ShowDocumentMapButton;
          this.m_docMapAreaSplitter.IsResizable = !this.SizeToReportContent;
          this.m_panelUpdater.PerformUpdates();
          this.m_clientScript.SetViewerInfo(this, this.m_reportArea.ClientID, this.ParametersRowID, this.m_docMapArea.ClientID, this.FixedTableID, this.m_topLevelUpdatePanel.ClientID, this.m_docMapUpdatePanel.ClientID, this.m_parametersAreaSplitter.ClientID, this.m_docMapAreaSplitter.ClientID, this.m_docMapArea.DocMapHeaderOverflowDivId, this.m_direction.ClientID, this.m_browserMode.ClientID, clientPrintInfo);
          this.m_reportArea.SetReportZoom(this.ZoomMode, this.ZoomPercent);
          this.m_asyncWaitControl.DisplayDelayMillis = this.WaitControlDisplayAfter;
          this.m_asyncWaitControl.CancelLinkVisible = this.ShowWaitControlCancelLink;
          this.m_asyncWaitControl.SetViewerInfo(this.ClientID, this.FixedTableID, this.ClientCanceledRendering, this.m_spinnyShouldSkipTimer);
          if (this.ProcessingMode == ProcessingMode.Remote)
          {
            int num = this.ServerReport.IsReadyForConnection ? 1 : 0;
          }
        }
      }
    }

    private void OnAsyncLoadReport(object sender, EventArgs e)
    {
      if (this.RenderingState == ReportRenderingState.AsyncWait)
      {
        this.RenderingState = ReportRenderingState.Ready;
      }
      else
      {
        if (this.RenderingState != ReportRenderingState.Completed)
          return;
        this.m_panelUpdater.MarkPanelsForUpdate(UpdateGroup.Rerendering);
      }
    }

    IEnumerable<ScriptDescriptor> IScriptControl.GetScriptDescriptors()
    {
      this.EnsureChildControls();
      ScriptReferenceValidationDescriptor validationDescriptor = new ScriptReferenceValidationDescriptor(this.m_httpHandlerMissingError.ClientID);
      ScriptComponentDescriptor componentDescriptor = new ScriptComponentDescriptor("Microsoft.Reporting.WebFormsClient.ReportViewer");
      componentDescriptor.ID = this.ClientID;
      componentDescriptor.AddProperty("_internalViewerId", (object) this.m_clientScript.ClientID);
      return (IEnumerable<ScriptDescriptor>) new ScriptDescriptor[2]
      {
        (ScriptDescriptor) validationDescriptor,
        (ScriptDescriptor) componentDescriptor
      };
    }

    IEnumerable<ScriptReference> IScriptControl.GetScriptReferences()
    {
      ScriptReference scriptReference = new ScriptReference();
      scriptReference.Path = EmbeddedResourceOperation.CreateUrlForScriptFile();
      return (IEnumerable<ScriptReference>) new ScriptReference[1]
      {
        scriptReference
      };
    }

    private void InitializeDataSources(object sender, InitializeDataSourcesEventArgs e)
    {
      this.InitializeDataSources(e.DataSources);
    }

    private void InitializeDataSources(ReportDataSourceCollection dataSources)
    {
      if (this.ProcessingMode != ProcessingMode.Local)
        return;
      foreach (ReportDataSource dataSource in (Collection<ReportDataSource>) dataSources)
      {
        if (dataSource.DataSourceId != null && dataSource.DataSourceId.Length != 0)
        {
          Control control = ReportViewer.FindControl((Control) this, dataSource.DataSourceId);
          if (control == null)
          {
            // ISSUE: reference to a compiler-generated method
            throw new HttpException(Errors.DataControl_DataSourceDoesntExist(dataSource.Name, this.ID, dataSource.DataSourceId));
          }
          if (!(control is IDataSource))
          {
            // ISSUE: reference to a compiler-generated method
            throw new HttpException(Errors.DataControl_DataSourceIDMustBeDataControl(dataSource.Name, this.ID, dataSource.DataSourceId ?? ""));
          }
          dataSource.SetValueWithoutChange((object) control);
        }
      }
    }

    private void ValidateAllDataSourcesSatisfied()
    {
      if (this.ProcessingMode != ProcessingMode.Local)
        return;
      foreach (string dataSourceName in (IEnumerable<string>) this.LocalReport.GetDataSourceNames())
      {
        if (this.LocalReport.DataSources[dataSourceName] == null)
          throw new MissingDataSourceException(dataSourceName);
      }
    }

    private static Control FindControl(Control control, string controlID)
    {
      Control control1 = control;
      Control control2 = (Control) null;
      if (control == control.Page)
        return control.FindControl(controlID);
      for (; control2 == null && control1 != control.Page; control2 = control1.FindControl(controlID))
      {
        control1 = control1.NamingContainer;
        if (control1 == null)
        {
          // ISSUE: reference to a compiler-generated method
          throw new HttpException(Errors.NoNamingContainer(control.GetType().Name, control.ID));
        }
      }
      return control2;
    }

    internal bool ReportHasChanged => this.m_reportHasChanged != ReportChangeType.None;

    internal bool IsClientRightToLeft
    {
      get
      {
        this.EnsureChildControls();
        return "rtl".Equals(this.m_direction.Value, StringComparison.OrdinalIgnoreCase);
      }
    }

    private void ClientSidePromptAreaVisibilityChanged(object sender, EventArgs e)
    {
      this.EnsureChildControls();
      this.SetPromptAreaCollapsedInternal(((ReportSplitter) sender).IsCollapsed, false);
    }

    private void ClientSideDocMapAreaVisibilityChanged(object sender, EventArgs e)
    {
      this.EnsureChildControls();
      this.SetDocumentMapCollapsedInternal(((ReportSplitter) sender).IsCollapsed, false);
    }

    private void EnsureUnlocked()
    {
      if (this.m_lockChanges)
        throw new InvalidOperationException(Errors.ReadOnlyViewer);
    }

    protected override void LoadViewState(object savedState)
    {
      try
      {
        this.m_panelUpdater.CancelAllUpdates();
        object[] objArray = (object[]) savedState;
        base.LoadViewState(objArray[0]);
        this.m_instanceIdentifier = (Guid) objArray[1];
        if (objArray[4] != null)
          this.InteractiveDeviceInfos = (DeviceInfoCollection) objArray[4];
        else
          this.m_interactiveDeviceInfos = (DeviceInfoCollection) null;
        this.m_originalWaitMessageFont = ReportViewerStyle.GetHtmlStyleForFont(this.WaitMessageFont);
        ReportHierarchy newReportHierarchy;
        if (this.EnsureSessionOrConfig())
        {
          newReportHierarchy = this.GetReportHierarchyFromSession();
        }
        else
        {
          newReportHierarchy = new ReportHierarchy(this.CreateServerReport());
          for (int index = 1; index < (int) objArray[2]; ++index)
          {
            ServerReport serverReport = this.CreateServerReport();
            newReportHierarchy.Push(new ReportInfo(new LocalModeSession(), new ServerModeSession(serverReport)));
          }
        }
        newReportHierarchy.LoadViewState(objArray[3]);
        this.ConnectNewReportHierarchy(newReportHierarchy);
      }
      catch (Exception ex)
      {
        this.m_aspSessionFailedToLoad = true;
        this.m_panelUpdater.MarkPanelsForUpdate(UpdateGroup.ExecutionSession);
        this.RenderingState = ReportRenderingState.NotReady;
        this.OnError(ex);
      }
    }

    protected override object SaveViewState()
    {
      bool flag = this.EnsureSessionOrConfig();
      object[] objArray = new object[5]
      {
        base.SaveViewState(),
        (object) this.m_instanceIdentifier,
        (object) this.m_reportHierarchy.Count,
        this.m_reportHierarchy.SaveViewState(!flag),
        null
      };
      if (flag && (!this.m_aspSessionFailedToLoad || this.ReportHasChanged))
        HttpContext.Current.Session[this.InstanceIdentifier] = (object) this.m_reportHierarchy;
      this.m_viewStateSaved = true;
      objArray[4] = (object) this.m_interactiveDeviceInfos;
      return (object) objArray;
    }

    internal virtual bool EnsureSessionOrConfig()
    {
      if (this.DesignMode)
        return false;
      bool flag = HttpContext.Current.Session == null;
      if (this.ProcessingMode == ProcessingMode.Local)
      {
        if (flag)
          throw new SessionDisabledException();
        return true;
      }
      if (!ServerReport.RequiresConnection || WebConfigReader.Current.ServerConnection != null)
        return false;
      if (flag)
        throw new MissingReportServerConnectionInformationException();
      return true;
    }

    internal virtual ServerReport CreateServerReport()
    {
      ServerReport serverReport = new ServerReport();
      IReportServerConnection serverConnection = WebConfigReader.Current.ServerConnection;
      if (serverConnection != null)
        this.ApplyConnectionToServerReport(serverConnection, serverReport);
      return serverReport;
    }

    internal virtual IReportViewerStyles ViewerStyle => (IReportViewerStyles) this.ControlStyle;

    internal void ApplyConnectionToServerReport(
      IReportServerConnection connection,
      ServerReport serverReport)
    {
      serverReport.ReportServerUrl = connection.ReportServerUrl;
      serverReport.Timeout = connection.Timeout;
      serverReport.ReportServerCredentials = (IReportServerCredentials) connection;
      if (!(connection is IReportServerConnection2 serverConnection2))
        return;
      IEnumerable<Cookie> cookies = serverConnection2.Cookies;
      if (cookies != null)
      {
        foreach (Cookie cookie in cookies)
        {
          if (cookie != null)
            serverReport.Cookies.Add(cookie);
        }
      }
      IEnumerable<string> headers = serverConnection2.Headers;
      if (headers == null)
        return;
      foreach (string str in headers)
      {
        if (str != null)
          serverReport.Headers.Add(str);
      }
    }

    internal static IEnumerable<RenderingExtension> FilterOutClientUnsupportedRenderingExtensions(
      IEnumerable<RenderingExtension> source)
    {
      bool clientIsIOS = BrowserDetectionUtility.IsIOSSafari();
      HashSet<string> blackListSet = clientIsIOS ? ReportViewer.IOSRenderingExtensionBlackList : new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (RenderingExtension extension in source)
      {
        if (!blackListSet.Contains(extension.Name))
          yield return extension;
      }
    }

    private ScrollTarget ScrollTarget
    {
      get => (ScrollTarget) this.ViewState[nameof (ScrollTarget)];
      set => this.ViewState[nameof (ScrollTarget)] = (object) value;
    }

    private bool PromptAreaHasVisibleContent
    {
      get
      {
        if (this.m_parametersArea.HasVisibleParameters && this.ShowParameterPrompts)
          return true;
        return this.m_parametersArea.HasCredentials && this.ShowCredentialPrompts;
      }
    }

    private bool ShouldRenderPromptArea
    {
      get
      {
        int num;
        switch (this.RenderingState)
        {
          case ReportRenderingState.AsyncWait:
          case ReportRenderingState.Ready:
          case ReportRenderingState.Completed:
            num = 1;
            break;
          default:
            num = this.m_reportArea.ReportAreaContent != ReportAreaContent.Error ? 1 : 0;
            break;
        }
        if (num == 0 || !this.PromptAreaHasVisibleContent)
          return false;
        return this.ProcessingMode == ProcessingMode.Remote || this.LocalReport.SupportsQueries;
      }
    }

    private bool ShouldRenderToolbar
    {
      get
      {
        if (!this.ShowToolBar)
          return false;
        return this.DesignMode || this.IsOrHasAttemptedToRenderReport;
      }
    }

    private bool ShouldRenderPromptAreaSplitter
    {
      get => !this.DesignMode && this.ShowPromptAreaButton && this.ShouldRenderPromptArea;
    }

    private bool IsOrHasAttemptedToRenderReport
    {
      get
      {
        if (this.DesignMode)
          return false;
        switch (this.RenderingState)
        {
          case ReportRenderingState.AsyncWait:
          case ReportRenderingState.Ready:
            return this.ShowReportBody;
          case ReportRenderingState.Completed:
            return true;
          default:
            return this.ClientCanceledRendering;
        }
      }
    }

    private bool WillRenderReport
    {
      get
      {
        bool flag1 = this.ShowReportBody && !this.DesignMode && this.m_reportArea.ReportAreaContent != ReportAreaContent.Error && this.Report.IsReadyForRendering;
        ReportRenderingState renderingState = this.RenderingState;
        bool flag2 = renderingState == ReportRenderingState.Ready || renderingState == ReportRenderingState.Completed;
        return flag1 && flag2;
      }
    }

    private void OnViewReport(object sender, EventArgs e)
    {
      this.m_viewReportClicked = true;
      this.ClientCanceledRendering = false;
    }

    private void OnParameterValuesChanged(object sender, EventArgs e)
    {
      this.m_userParamsChanged = true;
    }

    private void OnReportChanged(object sender, ReportChangedEventArgs e)
    {
      this.EnsureUnlocked();
      this.InternalSetCurrentPage(0, (ScrollTarget) null);
      if (e.IsRefreshOnly && this.m_reportHasChanged == ReportChangeType.None)
      {
        this.m_panelUpdater.MarkPanelsForUpdate(UpdateGroup.Reprocessing);
        this.m_reportHasChanged = ReportChangeType.Refresh;
      }
      else
      {
        this.m_panelUpdater.MarkPanelsForUpdate(UpdateGroup.ExecutionSession);
        this.m_reportHasChanged = ReportChangeType.ReportObject;
        this.m_spinnyShouldSkipTimer = true;
      }
    }

    private void InternalSetCurrentPage(int pageNumber, ScrollTarget scrollTarget)
    {
      this.SearchState = (SearchState) null;
      this.m_alertMessage = (string) null;
      this.ScrollTarget = scrollTarget;
      this.m_reportHierarchy.Peek().CurrentPage = pageNumber;
      this.m_panelUpdater.MarkPanelsForUpdate(UpdateGroup.Rerendering);
    }

    [Conditional("DEBUG")]
    private static void DebugRoundTripSerializers(object obj)
    {
    }

    internal string InstanceIdentifier => this.m_instanceIdentifier.ToString("N");

    internal void UseExistingLocalSession(string otherViewerInstanceIdentifier)
    {
      this.m_instanceIdentifier = new Guid(otherViewerInstanceIdentifier);
      this.ConnectNewReportHierarchy(this.GetReportHierarchyFromSession());
    }

    private ReportHierarchy GetReportHierarchyFromSession()
    {
      return (ReportHierarchy) (HttpContext.Current.Session[this.InstanceIdentifier] ?? throw new AspNetSessionExpiredException());
    }

    private void ConnectNewReportHierarchy(ReportHierarchy newReportHierarchy)
    {
      if (newReportHierarchy.Count > 0 && this.m_reportHierarchy.Count > 0)
        this.m_reportHierarchy.Peek().LocalReport.TransferEvents(newReportHierarchy.Peek().LocalReport);
      this.DisconnectReportHierarchy(true);
      this.m_reportHierarchy = newReportHierarchy;
      this.m_reportHierarchy.ConnectChangeEvents(this.m_changeHandler, this.m_dataInitializationHandler);
      this.m_reportHasChanged = ReportChangeType.None;
      this.m_alertMessage = (string) null;
    }

    private string ParametersRowID => "ParametersRow" + this.ClientID;

    private string FixedTableID => this.ClientID + "_fixedTable";

    internal Report Report
    {
      get
      {
        return this.ProcessingMode == ProcessingMode.Remote ? (Report) this.ServerReport : (Report) this.LocalReport;
      }
    }

    internal ReportControlSession ReportControlSession
    {
      get
      {
        ReportInfo reportInfo = this.m_reportHierarchy.Peek();
        return this.ProcessingMode == ProcessingMode.Remote ? (ReportControlSession) reportInfo.ServerSession : (ReportControlSession) reportInfo.LocalSession;
      }
    }

    internal ReportHierarchy ReportHierarchy => this.m_reportHierarchy;

    internal ReportSplitter ParametersAreaSplitter => this.m_parametersAreaSplitter;
  }
}
