using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.ReportingServices.Diagnostics.Utilities;

namespace Microsoft.Reporting.WebForms;

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

	protected override HtmlTextWriterTag TagKey => HtmlTextWriterTag.Div;

	public event EventHandler<ReportActionEventArgs> ReportAction;

	public ToolbarControl(ReportViewer viewer)
	{
		m_viewer = viewer;
	}

	protected override void CreateChildControls()
	{
		Controls.Clear();
		m_groups.Clear();
		m_pageNavGroup = new PageNavigationGroup(m_viewer);
		m_pageNavGroup.ReportAction += OnReportAction;
		AddGroup(m_pageNavGroup);
		m_backGroup = new BackGroup(m_viewer);
		m_backGroup.ReportAction += OnReportAction;
		AddGroup(m_backGroup);
		m_zoomGroup = new ZoomGroup(m_viewer);
		AddGroup(m_zoomGroup);
		m_findGroup = new FindGroup(m_viewer, showTextSeparator: true);
		AddGroup(m_findGroup);
		m_exportGroup = new ExportGroup(m_viewer);
		AddGroup(m_exportGroup);
		m_refreshGroup = new RefreshGroup(m_viewer);
		AddGroup(m_refreshGroup);
		m_printGroup = new PrintGroup(m_viewer);
		AddGroup(m_printGroup);
		m_atomDataFeedGroup = new AtomDataFeedGroup(m_viewer);
		AddGroup(m_atomDataFeedGroup);
	}

	private void OnReportAction(object sender, ReportActionEventArgs e)
	{
		if (this.ReportAction != null)
		{
			this.ReportAction(this, e);
		}
	}

	private void AddGroup(ToolbarGroup group)
	{
		Controls.Add(group);
		m_groups.Add(group);
	}

	protected override void OnPreRender(EventArgs e)
	{
		EnsureChildControls();
		if (!m_viewer.ViewerStyle.GetFontFromCss)
		{
			Font.CopyFrom(m_viewer.ViewerStyle.Font);
		}
		base.OnPreRender(e);
		ScriptManager.GetCurrent(Page)?.RegisterScriptControl(this);
	}

	protected override void Render(HtmlTextWriter writer)
	{
		EnsureChildControls();
		ScriptManager.GetCurrent(Page)?.RegisterScriptDescriptors(this);
		AddAttributesToRender(writer);
		m_hasRenderedGroup = false;
		m_lastGroupVisible = true;
		if (m_viewer.ViewerStyle.ViewerAreaBackground != null)
		{
			writer.AddAttribute(HtmlTextWriterAttribute.Class, m_viewer.ViewerStyle.ViewerAreaBackground + " " + m_viewer.ViewerStyle.ToolbarBackground);
		}
		else
		{
			m_viewer.ViewerStyle.AddInternalBorderAttributes(writer, "border-bottom");
			writer.AddStyleAttribute(HtmlTextWriterStyle.BackgroundColor, ColorTranslator.ToHtml(m_viewer.ViewerStyle.BackColor));
			writer.AddStyleAttribute(HtmlTextWriterStyle.BackgroundImage, BackgroundImageOperation.CreateUrl("Microsoft.Reporting.WebForms.Icons.toolbar_bk.png", m_viewer.ViewerStyle.BackColor));
		}
		writer.RenderBeginTag(HtmlTextWriterTag.Div);
		foreach (ToolbarGroup group in m_groups)
		{
			group.Font.CopyFrom(Font);
		}
		RenderMainButtonDiv(writer);
		writer.RenderEndTag();
	}

	private void RenderMainButtonDiv(HtmlTextWriter writer)
	{
		if (m_viewer.ViewerStyle.ToolbarButtonContainer != null)
		{
			writer.AddAttribute(HtmlTextWriterAttribute.Class, m_viewer.ViewerStyle.ToolbarButtonContainer);
		}
		else
		{
			writer.AddStyleAttribute(HtmlTextWriterStyle.PaddingLeft, "6px");
		}
		writer.RenderBeginTag(HtmlTextWriterTag.Div);
		if (m_viewer.ShowPageNavigationControls)
		{
			RenderSpacedGroup(m_pageNavGroup, writer, visibleOnLoad: true);
		}
		if (m_viewer.ShowBackButton)
		{
			RenderSpacedGroup(m_backGroup, writer, visibleOnLoad: true);
		}
		if (m_viewer.ShowZoomControl && ReportViewerClientScript.IsZoomSupported)
		{
			RenderSpacedGroup(m_zoomGroup, writer, visibleOnLoad: true);
		}
		if (m_viewer.ShowFindControls)
		{
			RenderSpacedGroup(m_findGroup, writer, visibleOnLoad: true);
		}
		if (m_viewer.ShowExportControls)
		{
			RenderSpacedGroup(m_exportGroup, writer, visibleOnLoad: true);
		}
		if (m_viewer.ShowRefreshButton)
		{
			RenderSpacedGroup(m_refreshGroup, writer, visibleOnLoad: true);
		}
		ClientArchitecture clientArchitecture = BrowserDetectionUtility.GetClientArchitecture();
		if (m_viewer.ShowPrintButton && ReportViewerClientScript.IsPrintingSupported && m_viewer.ReportControlSession.IsPrintCabSupported(clientArchitecture))
		{
			RenderSpacedGroup(m_printGroup, writer, visibleOnLoad: true);
		}
		if (m_viewer.ShowAtomDataFeedButton && m_atomDataFeedGroup.IsSupported)
		{
			RenderSpacedGroup(m_atomDataFeedGroup, writer, visibleOnLoad: true);
		}
		writer.RenderEndTag();
	}

	private void RenderSpacedGroup(ToolbarGroup group, HtmlTextWriter writer, bool visibleOnLoad)
	{
		string text = (ReportViewerClientScript.IsIE55OrHigher ? "inline" : "inline-block");
		if (m_hasRenderedGroup)
		{
			writer.AddAttribute(HtmlTextWriterAttribute.Cellpadding, "0");
			writer.AddAttribute(HtmlTextWriterAttribute.Cellspacing, "0");
			writer.AddStyleAttribute(HtmlTextWriterStyle.Display, m_lastGroupVisible ? text : "none");
			writer.AddAttribute("ToolbarSpacer", "true");
			if (m_viewer.ViewerStyle.ToolbarGroupSpacer != null || group.GroupCssClassName != null)
			{
				string text2 = ((group.LeadingSpace == ToolbarGroup.NormalLeadingSpace) ? m_viewer.ViewerStyle.ToolbarGroupSpacer : m_viewer.ViewerStyle.ToolbarGroupShortSpacer);
				writer.AddAttribute(HtmlTextWriterAttribute.Class, text2 + " " + group.GroupCssClassName);
			}
			else
			{
				writer.AddStyleAttribute(HtmlTextWriterStyle.Width, group.LeadingSpace);
			}
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
			{
				writer.AddStyleAttribute(HtmlTextWriterStyle.Display, text);
			}
		}
		else
		{
			writer.AddStyleAttribute(HtmlTextWriterStyle.Display, "none");
		}
		group.RenderControl(writer);
		m_hasRenderedGroup = true;
		m_lastGroupVisible = visibleOnLoad;
	}

	public static string GenerateUpdateProperties(ReportControlSession session, int pageNumber, SearchState searchState)
	{
		PageCountMode pageCountMode;
		int totalPages = session.Report.GetTotalPages(out pageCountMode);
		string text = "";
		if (searchState != null && searchState.Text != null)
		{
			text = searchState.Text;
		}
		return string.Format(CultureInfo.CurrentCulture, "{{'CurrentPage':{0},'TotalPages':{1},'IsEstimatePageCount':{2},'TotalPagesString':'{3}','SearchText':'{4}','CanFindNext':{5}}}", pageNumber, totalPages, (pageCountMode == PageCountMode.Actual) ? "false" : "true", JavaScriptHelper.StringEscapeSingleQuote(LocalizationHelper.Current.TotalPages(totalPages, pageCountMode)), JavaScriptHelper.StringEscapeSingleQuote(text ?? ""), (searchState != null) ? "true" : "false");
	}

	public IEnumerable<ScriptDescriptor> GetScriptDescriptors()
	{
		EnsureChildControls();
		ScriptControlDescriptor scriptControlDescriptor = new ScriptControlDescriptor("Microsoft.Reporting.WebFormsClient._Toolbar", ClientID);
		scriptControlDescriptor.AddComponentProperty("ReportViewer", m_viewer.ClientID);
		foreach (ToolbarGroup group in m_groups)
		{
			group.AddScriptDescriptorProperties(scriptControlDescriptor);
		}
		return new ScriptDescriptor[1] { scriptControlDescriptor };
	}

	public IEnumerable<ScriptReference> GetScriptReferences()
	{
		ScriptReference scriptReference = new ScriptReference();
		scriptReference.Path = EmbeddedResourceOperation.CreateUrlForScriptFile();
		return new ScriptReference[1] { scriptReference };
	}
}
