using System;
using System.Globalization;
using System.Web.UI;

namespace Microsoft.Reporting.WebForms;

internal sealed class PageNavigationGroup : ToolbarGroup
{
	private ScriptSwitchImage m_firstPage;

	private ScriptSwitchImage m_prevPage;

	private ScriptSwitchImage m_nextPage;

	private ScriptSwitchImage m_lastPage;

	private EventableTextBox m_currentPage;

	private SafeLiteralControl m_totalPages;

	private SafeLiteralControl m_currentPageSep;

	public override string GroupCssClassName => m_viewer.ViewerStyle.ToolbarPageNav;

	public PageNavigationGroup(ReportViewer viewer)
		: base(viewer)
	{
	}

	protected override void CreateChildControls()
	{
		Controls.Clear();
		m_firstPage = CreatePageNavButton("First", new ToolbarImageInfo("Microsoft.Reporting.WebForms.Icons.FirstPage.gif", "Microsoft.Reporting.WebForms.Icons.LastPage.gif"), new ToolbarImageInfo("Microsoft.Reporting.WebForms.Icons.FirstPageDisabled.gif", "Microsoft.Reporting.WebForms.Icons.LastPageDisabled.gif"), LocalizationHelper.Current.FirstPageButtonToolTip, delegate
		{
			OnPageNavButtonClick(1);
		});
		m_prevPage = CreatePageNavButton("Previous", new ToolbarImageInfo("Microsoft.Reporting.WebForms.Icons.PrevPage.gif", "Microsoft.Reporting.WebForms.Icons.NextPage.gif"), new ToolbarImageInfo("Microsoft.Reporting.WebForms.Icons.PrevPageDisabled.gif", "Microsoft.Reporting.WebForms.Icons.NextPageDisabled.gif"), LocalizationHelper.Current.PreviousPageButtonToolTip, delegate
		{
			OnPageNavButtonClick(m_viewer.CurrentPage - 1);
		});
		m_currentPage = new EventableTextBox();
		m_currentPage.Enabled = false;
		m_currentPage.ID = "CurrentPage";
		m_currentPage.Columns = 3;
		m_currentPage.MaxLength = 8;
		m_currentPage.Text = "1";
		m_currentPage.ToolTip = LocalizationHelper.Current.CurrentPageTextBoxToolTip;
		m_currentPage.AddKeyPressHandler = false;
		m_currentPage.EnterPressed += OnCurrentPageTextBoxEnter;
		if (m_viewer.ViewerStyle.ToolbarCurrentPage != null)
		{
			m_currentPage.CssClass = m_viewer.ViewerStyle.ToolbarCurrentPage;
		}
		Controls.Add(m_currentPage);
		m_currentPageSep = new SafeLiteralControl(LocalizationHelper.Current.PageOf);
		m_currentPageSep.CssClass = m_viewer.ViewerStyle.ToolbarText;
		Controls.Add(m_currentPageSep);
		m_totalPages = new SafeLiteralControl();
		m_totalPages.ID = "TotalPages";
		m_totalPages.CssClass = m_viewer.ViewerStyle.ToolbarText;
		m_totalPages.Text = "1";
		m_totalPages.Style.Add(HtmlTextWriterStyle.WhiteSpace, "nowrap");
		Controls.Add(m_totalPages);
		m_nextPage = CreatePageNavButton("Next", new ToolbarImageInfo("Microsoft.Reporting.WebForms.Icons.NextPage.gif", "Microsoft.Reporting.WebForms.Icons.PrevPage.gif"), new ToolbarImageInfo("Microsoft.Reporting.WebForms.Icons.NextPageDisabled.gif", "Microsoft.Reporting.WebForms.Icons.PrevPageDisabled.gif"), LocalizationHelper.Current.NextPageButtonToolTip, delegate
		{
			OnPageNavButtonClick(m_viewer.CurrentPage + 1);
		});
		m_lastPage = CreatePageNavButton("Last", new ToolbarImageInfo("Microsoft.Reporting.WebForms.Icons.LastPage.gif", "Microsoft.Reporting.WebForms.Icons.FirstPage.gif"), new ToolbarImageInfo("Microsoft.Reporting.WebForms.Icons.LastPageDisabled.gif", "Microsoft.Reporting.WebForms.Icons.FirstPageDisabled.gif"), LocalizationHelper.Current.LastPageButtonToolTip, OnLastPageButtonClick);
	}

	protected override void OnPreRender(EventArgs e)
	{
		EnsureChildControls();
		base.OnPreRender(e);
		int currentPage = m_viewer.CurrentPage;
		PageCountMode pageCountMode;
		int totalPages = m_viewer.Report.GetTotalPages(out pageCountMode);
		m_currentPage.Text = ((currentPage > 0 && totalPages > 0) ? currentPage.ToString(CultureInfo.CurrentCulture) : "");
		m_totalPages.Text = LocalizationHelper.Current.TotalPages(totalPages, pageCountMode);
	}

	protected override void Render(HtmlTextWriter writer)
	{
		EnsureChildControls();
		m_currentPage.Font.CopyFrom(Font);
		m_currentPageSep.Font.CopyFrom(Font);
		m_totalPages.Font.CopyFrom(Font);
		base.Render(writer);
	}

	private ScriptSwitchImage CreatePageNavButton(string id, ToolbarImageInfo image, ToolbarImageInfo disabledImage, string tooltip, EventHandler performPageNav)
	{
		ScriptSwitchImage scriptSwitchImage = new ScriptSwitchImage(image, disabledImage, image2Disabled: true, tooltip, m_viewer);
		scriptSwitchImage.ID = id;
		scriptSwitchImage.ClickImage1 += performPageNav;
		Controls.Add(scriptSwitchImage);
		return scriptSwitchImage;
	}

	public override void AddScriptDescriptorProperties(ScriptControlDescriptor toolbarDesc)
	{
		EnsureChildControls();
		toolbarDesc.AddElementProperty("CurrentPageTextBox", m_currentPage.ClientID);
		toolbarDesc.AddElementProperty("TotalPagesLabel", m_totalPages.ClientID);
		toolbarDesc.AddElementProperty("FirstPageNavButton", m_firstPage.ClientID);
		toolbarDesc.AddElementProperty("PrevPageNavButton", m_prevPage.ClientID);
		toolbarDesc.AddElementProperty("NextPageNavButton", m_nextPage.ClientID);
		toolbarDesc.AddElementProperty("LastPageNavButton", m_lastPage.ClientID);
		toolbarDesc.AddProperty("InvalidPageNumberMessage", LocalizationHelper.Current.InvalidPageNumber);
		toolbarDesc.AddScriptProperty("OnCurrentPageClick", JavaScriptHelper.FormatAsFunction(Page.ClientScript.GetPostBackEventReference(m_currentPage, null) + ";"));
		toolbarDesc.AddProperty("CurrentPage", m_viewer.CurrentPage);
		toolbarDesc.AddProperty("TotalPages", m_viewer.Report.GetTotalPages(out var pageCountMode));
		toolbarDesc.AddProperty("IsEstimatePageCount", pageCountMode != PageCountMode.Actual);
	}

	private void OnPageNavButtonClick(int targetPage)
	{
		OnReportAction(new ReportActionEventArgs("PageNav", targetPage.ToString(CultureInfo.InvariantCulture)));
	}

	private void OnLastPageButtonClick(object sender, EventArgs e)
	{
		if (!m_viewer.ReportHasChanged)
		{
			PageCountMode pageCountMode;
			int totalPages = m_viewer.Report.GetTotalPages(out pageCountMode);
			if (pageCountMode == PageCountMode.Actual)
			{
				OnPageNavButtonClick(totalPages);
			}
			else
			{
				OnPageNavButtonClick(int.MaxValue);
			}
		}
	}

	private void OnCurrentPageTextBoxEnter(object sender, EventArgs e)
	{
		if (int.TryParse(m_currentPage.Text, out var result))
		{
			OnPageNavButtonClick(result);
		}
	}
}
