
using System;
using System.Globalization;
using System.Web.UI;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal sealed class PageNavigationGroup : ToolbarGroup
  {
    private ScriptSwitchImage m_firstPage;
    private ScriptSwitchImage m_prevPage;
    private ScriptSwitchImage m_nextPage;
    private ScriptSwitchImage m_lastPage;
    private EventableTextBox m_currentPage;
    private SafeLiteralControl m_totalPages;
    private SafeLiteralControl m_currentPageSep;

    public PageNavigationGroup(ReportViewer viewer)
      : base(viewer)
    {
    }

    public override string GroupCssClassName => this.m_viewer.ViewerStyle.ToolbarPageNav;

    protected override void CreateChildControls()
    {
      this.Controls.Clear();
      this.m_firstPage = this.CreatePageNavButton("First", new ToolbarImageInfo("Microsoft.Reporting.WebForms.Icons.FirstPage.gif", "Microsoft.Reporting.WebForms.Icons.LastPage.gif"), new ToolbarImageInfo("Microsoft.Reporting.WebForms.Icons.FirstPageDisabled.gif", "Microsoft.Reporting.WebForms.Icons.LastPageDisabled.gif"), LocalizationHelper.Current.FirstPageButtonToolTip, (EventHandler) ((param0, param1) => this.OnPageNavButtonClick(1)));
      this.m_prevPage = this.CreatePageNavButton("Previous", new ToolbarImageInfo("Microsoft.Reporting.WebForms.Icons.PrevPage.gif", "Microsoft.Reporting.WebForms.Icons.NextPage.gif"), new ToolbarImageInfo("Microsoft.Reporting.WebForms.Icons.PrevPageDisabled.gif", "Microsoft.Reporting.WebForms.Icons.NextPageDisabled.gif"), LocalizationHelper.Current.PreviousPageButtonToolTip, (EventHandler) ((param0, param1) => this.OnPageNavButtonClick(this.m_viewer.CurrentPage - 1)));
      this.m_currentPage = new EventableTextBox();
      this.m_currentPage.Enabled = false;
      this.m_currentPage.ID = "CurrentPage";
      this.m_currentPage.Columns = 3;
      this.m_currentPage.MaxLength = 8;
      this.m_currentPage.Text = "1";
      this.m_currentPage.ToolTip = LocalizationHelper.Current.CurrentPageTextBoxToolTip;
      this.m_currentPage.AddKeyPressHandler = false;
      this.m_currentPage.EnterPressed += new EventHandler(this.OnCurrentPageTextBoxEnter);
      if (this.m_viewer.ViewerStyle.ToolbarCurrentPage != null)
        this.m_currentPage.CssClass = this.m_viewer.ViewerStyle.ToolbarCurrentPage;
      this.Controls.Add((Control) this.m_currentPage);
      this.m_currentPageSep = new SafeLiteralControl(LocalizationHelper.Current.PageOf);
      this.m_currentPageSep.CssClass = this.m_viewer.ViewerStyle.ToolbarText;
      this.Controls.Add((Control) this.m_currentPageSep);
      this.m_totalPages = new SafeLiteralControl();
      this.m_totalPages.ID = "TotalPages";
      this.m_totalPages.CssClass = this.m_viewer.ViewerStyle.ToolbarText;
      this.m_totalPages.Text = "1";
      this.m_totalPages.Style.Add(HtmlTextWriterStyle.WhiteSpace, "nowrap");
      this.Controls.Add((Control) this.m_totalPages);
      this.m_nextPage = this.CreatePageNavButton("Next", new ToolbarImageInfo("Microsoft.Reporting.WebForms.Icons.NextPage.gif", "Microsoft.Reporting.WebForms.Icons.PrevPage.gif"), new ToolbarImageInfo("Microsoft.Reporting.WebForms.Icons.NextPageDisabled.gif", "Microsoft.Reporting.WebForms.Icons.PrevPageDisabled.gif"), LocalizationHelper.Current.NextPageButtonToolTip, (EventHandler) ((param0, param1) => this.OnPageNavButtonClick(this.m_viewer.CurrentPage + 1)));
      this.m_lastPage = this.CreatePageNavButton("Last", new ToolbarImageInfo("Microsoft.Reporting.WebForms.Icons.LastPage.gif", "Microsoft.Reporting.WebForms.Icons.FirstPage.gif"), new ToolbarImageInfo("Microsoft.Reporting.WebForms.Icons.LastPageDisabled.gif", "Microsoft.Reporting.WebForms.Icons.FirstPageDisabled.gif"), LocalizationHelper.Current.LastPageButtonToolTip, new EventHandler(this.OnLastPageButtonClick));
    }

    protected override void OnPreRender(EventArgs e)
    {
      this.EnsureChildControls();
      base.OnPreRender(e);
      int currentPage = this.m_viewer.CurrentPage;
      PageCountMode pageCountMode;
      int totalPages = this.m_viewer.Report.GetTotalPages(out pageCountMode);
      this.m_currentPage.Text = currentPage <= 0 || totalPages <= 0 ? "" : currentPage.ToString((IFormatProvider) CultureInfo.CurrentCulture);
      this.m_totalPages.Text = LocalizationHelper.Current.TotalPages(totalPages, pageCountMode);
    }

    protected override void Render(HtmlTextWriter writer)
    {
      this.EnsureChildControls();
      this.m_currentPage.Font.CopyFrom(this.Font);
      this.m_currentPageSep.Font.CopyFrom(this.Font);
      this.m_totalPages.Font.CopyFrom(this.Font);
      base.Render(writer);
    }

    private ScriptSwitchImage CreatePageNavButton(
      string id,
      ToolbarImageInfo image,
      ToolbarImageInfo disabledImage,
      string tooltip,
      EventHandler performPageNav)
    {
      ScriptSwitchImage child = new ScriptSwitchImage(image, disabledImage, true, tooltip, this.m_viewer);
      child.ID = id;
      child.ClickImage1 += performPageNav;
      this.Controls.Add((Control) child);
      return child;
    }

    public override void AddScriptDescriptorProperties(ScriptControlDescriptor toolbarDesc)
    {
      this.EnsureChildControls();
      toolbarDesc.AddElementProperty("CurrentPageTextBox", this.m_currentPage.ClientID);
      toolbarDesc.AddElementProperty("TotalPagesLabel", this.m_totalPages.ClientID);
      toolbarDesc.AddElementProperty("FirstPageNavButton", this.m_firstPage.ClientID);
      toolbarDesc.AddElementProperty("PrevPageNavButton", this.m_prevPage.ClientID);
      toolbarDesc.AddElementProperty("NextPageNavButton", this.m_nextPage.ClientID);
      toolbarDesc.AddElementProperty("LastPageNavButton", this.m_lastPage.ClientID);
      toolbarDesc.AddProperty("InvalidPageNumberMessage", (object) LocalizationHelper.Current.InvalidPageNumber);
      toolbarDesc.AddScriptProperty("OnCurrentPageClick", JavaScriptHelper.FormatAsFunction(this.Page.ClientScript.GetPostBackEventReference((Control) this.m_currentPage, (string) null) + ";"));
      toolbarDesc.AddProperty("CurrentPage", (object) this.m_viewer.CurrentPage);
      PageCountMode pageCountMode;
      toolbarDesc.AddProperty("TotalPages", (object) this.m_viewer.Report.GetTotalPages(out pageCountMode));
      toolbarDesc.AddProperty("IsEstimatePageCount", (object) (pageCountMode != PageCountMode.Actual));
    }

    private void OnPageNavButtonClick(int targetPage)
    {
      this.OnReportAction(new ReportActionEventArgs("PageNav", targetPage.ToString((IFormatProvider) CultureInfo.InvariantCulture)));
    }

    private void OnLastPageButtonClick(object sender, EventArgs e)
    {
      if (this.m_viewer.ReportHasChanged)
        return;
      PageCountMode pageCountMode;
      int totalPages = this.m_viewer.Report.GetTotalPages(out pageCountMode);
      if (pageCountMode == PageCountMode.Actual)
        this.OnPageNavButtonClick(totalPages);
      else
        this.OnPageNavButtonClick(int.MaxValue);
    }

    private void OnCurrentPageTextBoxEnter(object sender, EventArgs e)
    {
      int result;
      if (!int.TryParse(this.m_currentPage.Text, out result))
        return;
      this.OnPageNavButtonClick(result);
    }
  }
}
