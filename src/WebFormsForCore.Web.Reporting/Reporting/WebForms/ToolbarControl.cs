// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.ToolbarControl
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

using Microsoft.ReportingServices.Diagnostics.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal sealed class ToolbarControl : CompositeControl, IScriptControl
  {
    private PageNavigationGroup m_pageNavGroup;
    private BackGroup m_backGroup;
    private ZoomGroup m_zoomGroup;
    private FindGroup m_findGroup;
    private ExportGroup m_exportGroup;
    private RefreshGroup m_refreshGroup;
    private PrintGroup m_printGroup;
    private AtomDataFeedGroup m_atomDataFeedGroup;
    private List<ToolbarGroup> m_groups = new List<ToolbarGroup>(9);
    private bool m_hasRenderedGroup;
    private bool m_lastGroupVisible = true;
    private ReportViewer m_viewer;

    public ToolbarControl(ReportViewer viewer) => this.m_viewer = viewer;

    protected override HtmlTextWriterTag TagKey => HtmlTextWriterTag.Div;

    public event EventHandler<ReportActionEventArgs> ReportAction;

    protected override void CreateChildControls()
    {
      this.Controls.Clear();
      this.m_groups.Clear();
      this.m_pageNavGroup = new PageNavigationGroup(this.m_viewer);
      this.m_pageNavGroup.ReportAction += new EventHandler<ReportActionEventArgs>(this.OnReportAction);
      this.AddGroup((ToolbarGroup) this.m_pageNavGroup);
      this.m_backGroup = new BackGroup(this.m_viewer);
      this.m_backGroup.ReportAction += new EventHandler<ReportActionEventArgs>(this.OnReportAction);
      this.AddGroup((ToolbarGroup) this.m_backGroup);
      this.m_zoomGroup = new ZoomGroup(this.m_viewer);
      this.AddGroup((ToolbarGroup) this.m_zoomGroup);
      this.m_findGroup = new FindGroup(this.m_viewer, true);
      this.AddGroup((ToolbarGroup) this.m_findGroup);
      this.m_exportGroup = new ExportGroup(this.m_viewer);
      this.AddGroup((ToolbarGroup) this.m_exportGroup);
      this.m_refreshGroup = new RefreshGroup(this.m_viewer);
      this.AddGroup((ToolbarGroup) this.m_refreshGroup);
      this.m_printGroup = new PrintGroup(this.m_viewer);
      this.AddGroup((ToolbarGroup) this.m_printGroup);
      this.m_atomDataFeedGroup = new AtomDataFeedGroup(this.m_viewer);
      this.AddGroup((ToolbarGroup) this.m_atomDataFeedGroup);
    }

    private void OnReportAction(object sender, ReportActionEventArgs e)
    {
      if (this.ReportAction == null)
        return;
      this.ReportAction((object) this, e);
    }

    private void AddGroup(ToolbarGroup group)
    {
      this.Controls.Add((Control) group);
      this.m_groups.Add(group);
    }

    protected override void OnPreRender(EventArgs e)
    {
      this.EnsureChildControls();
      if (!this.m_viewer.ViewerStyle.GetFontFromCss)
        this.Font.CopyFrom(this.m_viewer.ViewerStyle.Font);
      base.OnPreRender(e);
      ScriptManager.GetCurrent(this.Page)?.RegisterScriptControl<ToolbarControl>(this);
    }

    protected override void Render(HtmlTextWriter writer)
    {
      this.EnsureChildControls();
      ScriptManager.GetCurrent(this.Page)?.RegisterScriptDescriptors((IScriptControl) this);
      this.AddAttributesToRender(writer);
      this.m_hasRenderedGroup = false;
      this.m_lastGroupVisible = true;
      if (this.m_viewer.ViewerStyle.ViewerAreaBackground != null)
      {
        writer.AddAttribute(HtmlTextWriterAttribute.Class, this.m_viewer.ViewerStyle.ViewerAreaBackground + " " + this.m_viewer.ViewerStyle.ToolbarBackground);
      }
      else
      {
        this.m_viewer.ViewerStyle.AddInternalBorderAttributes(writer, "border-bottom");
        writer.AddStyleAttribute(HtmlTextWriterStyle.BackgroundColor, ColorTranslator.ToHtml(this.m_viewer.ViewerStyle.BackColor));
        writer.AddStyleAttribute(HtmlTextWriterStyle.BackgroundImage, BackgroundImageOperation.CreateUrl("Microsoft.Reporting.WebForms.Icons.toolbar_bk.png", this.m_viewer.ViewerStyle.BackColor));
      }
      writer.RenderBeginTag(HtmlTextWriterTag.Div);
      foreach (WebControl group in this.m_groups)
        group.Font.CopyFrom(this.Font);
      this.RenderMainButtonDiv(writer);
      writer.RenderEndTag();
    }

    private void RenderMainButtonDiv(HtmlTextWriter writer)
    {
      if (this.m_viewer.ViewerStyle.ToolbarButtonContainer != null)
        writer.AddAttribute(HtmlTextWriterAttribute.Class, this.m_viewer.ViewerStyle.ToolbarButtonContainer);
      else
        writer.AddStyleAttribute(HtmlTextWriterStyle.PaddingLeft, "6px");
      writer.RenderBeginTag(HtmlTextWriterTag.Div);
      if (this.m_viewer.ShowPageNavigationControls)
        this.RenderSpacedGroup((ToolbarGroup) this.m_pageNavGroup, writer, true);
      if (this.m_viewer.ShowBackButton)
        this.RenderSpacedGroup((ToolbarGroup) this.m_backGroup, writer, true);
      if (this.m_viewer.ShowZoomControl && ReportViewerClientScript.IsZoomSupported)
        this.RenderSpacedGroup((ToolbarGroup) this.m_zoomGroup, writer, true);
      if (this.m_viewer.ShowFindControls)
        this.RenderSpacedGroup((ToolbarGroup) this.m_findGroup, writer, true);
      if (this.m_viewer.ShowExportControls)
        this.RenderSpacedGroup((ToolbarGroup) this.m_exportGroup, writer, true);
      if (this.m_viewer.ShowRefreshButton)
        this.RenderSpacedGroup((ToolbarGroup) this.m_refreshGroup, writer, true);
      ClientArchitecture clientArchitecture = BrowserDetectionUtility.GetClientArchitecture();
      if (this.m_viewer.ShowPrintButton && ReportViewerClientScript.IsPrintingSupported && this.m_viewer.ReportControlSession.IsPrintCabSupported(clientArchitecture))
        this.RenderSpacedGroup((ToolbarGroup) this.m_printGroup, writer, true);
      if (this.m_viewer.ShowAtomDataFeedButton && this.m_atomDataFeedGroup.IsSupported)
        this.RenderSpacedGroup((ToolbarGroup) this.m_atomDataFeedGroup, writer, true);
      writer.RenderEndTag();
    }

    private void RenderSpacedGroup(ToolbarGroup group, HtmlTextWriter writer, bool visibleOnLoad)
    {
      string str1 = ReportViewerClientScript.IsIE55OrHigher ? "inline" : "inline-block";
      if (this.m_hasRenderedGroup)
      {
        writer.AddAttribute(HtmlTextWriterAttribute.Cellpadding, "0");
        writer.AddAttribute(HtmlTextWriterAttribute.Cellspacing, "0");
        writer.AddStyleAttribute(HtmlTextWriterStyle.Display, this.m_lastGroupVisible ? str1 : "none");
        writer.AddAttribute("ToolbarSpacer", "true");
        if (this.m_viewer.ViewerStyle.ToolbarGroupSpacer != null || group.GroupCssClassName != null)
        {
          string str2 = group.LeadingSpace == ToolbarGroup.NormalLeadingSpace ? this.m_viewer.ViewerStyle.ToolbarGroupSpacer : this.m_viewer.ViewerStyle.ToolbarGroupShortSpacer;
          writer.AddAttribute(HtmlTextWriterAttribute.Class, str2 + " " + group.GroupCssClassName);
        }
        else
          writer.AddStyleAttribute(HtmlTextWriterStyle.Width, group.LeadingSpace);
        writer.RenderBeginTag(HtmlTextWriterTag.Table);
        writer.RenderBeginTag(HtmlTextWriterTag.Tr);
        writer.RenderBeginTag(HtmlTextWriterTag.Td);
        writer.RenderEndTag();
        writer.RenderEndTag();
        writer.RenderEndTag();
      }
      if (visibleOnLoad)
      {
        if (group.GroupCssClassName == null)
          writer.AddStyleAttribute(HtmlTextWriterStyle.Display, str1);
      }
      else
        writer.AddStyleAttribute(HtmlTextWriterStyle.Display, "none");
      group.RenderControl(writer);
      this.m_hasRenderedGroup = true;
      this.m_lastGroupVisible = visibleOnLoad;
    }

    public static string GenerateUpdateProperties(
      ReportControlSession session,
      int pageNumber,
      SearchState searchState)
    {
      PageCountMode pageCountMode;
      int totalPages = session.Report.GetTotalPages(out pageCountMode);
      string str = "";
      if (searchState != null && searchState.Text != null)
        str = searchState.Text;
      return string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{{'CurrentPage':{0},'TotalPages':{1},'IsEstimatePageCount':{2},'TotalPagesString':'{3}','SearchText':'{4}','CanFindNext':{5}}}", (object) pageNumber, (object) totalPages, pageCountMode == PageCountMode.Actual ? (object) "false" : (object) "true", (object) JavaScriptHelper.StringEscapeSingleQuote(LocalizationHelper.Current.TotalPages(totalPages, pageCountMode)), (object) JavaScriptHelper.StringEscapeSingleQuote(str ?? ""), searchState != null ? (object) "true" : (object) "false");
    }

    public IEnumerable<ScriptDescriptor> GetScriptDescriptors()
    {
      this.EnsureChildControls();
      ScriptControlDescriptor toolbarDesc = new ScriptControlDescriptor("Microsoft.Reporting.WebFormsClient._Toolbar", this.ClientID);
      toolbarDesc.AddComponentProperty("ReportViewer", this.m_viewer.ClientID);
      foreach (ToolbarGroup group in this.m_groups)
        group.AddScriptDescriptorProperties(toolbarDesc);
      return (IEnumerable<ScriptDescriptor>) new ScriptDescriptor[1]
      {
        (ScriptDescriptor) toolbarDesc
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
