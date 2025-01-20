
using System;
using System.Web.UI;
using System.Web.UI.WebControls;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal class ReportAreaVisibilityState : CompositeControl
  {
    private HiddenField m_persistedState;
    private ReportAreaContent? m_newState = new ReportAreaContent?();
    private ReportArea m_reportArea;

    public ReportAreaVisibilityState(ReportArea reportArea) => this.m_reportArea = reportArea;

    protected override HtmlTextWriterTag TagKey => HtmlTextWriterTag.Div;

    public ReportAreaContent NewClientState
    {
      get => this.m_newState.HasValue ? this.m_newState.Value : this.CurrentClientState;
      set => this.m_newState = new ReportAreaContent?(value);
    }

    public ReportAreaContent CurrentClientState
    {
      get
      {
        this.EnsureChildControls();
        return (ReportAreaContent) Enum.Parse(typeof (ReportAreaContent), this.m_persistedState.Value);
      }
    }

    protected override void CreateChildControls()
    {
      this.Controls.Clear();
      this.m_persistedState = new HiddenField();
      this.m_persistedState.EnableViewState = false;
      this.m_persistedState.Value = ReportAreaContent.None.ToString();
      this.Controls.Add((Control) this.m_persistedState);
    }

    protected override void OnPreRender(EventArgs e)
    {
      this.Style[HtmlTextWriterStyle.Visibility] = "none";
      base.OnPreRender(e);
    }

    protected override void Render(HtmlTextWriter writer)
    {
      this.EnsureChildControls();
      this.m_persistedState.Value = this.NewClientState.ToString();
      writer.AddAttribute("NewContentType", "Microsoft.Reporting.WebFormsClient.ReportAreaContent." + this.NewClientState.ToString());
      writer.AddAttribute("ForNonReportContentArea", ReportArea.IsDisplayedInNonReportContentPanel(this.NewClientState) ? "true" : "false");
      base.Render(writer);
    }
  }
}
