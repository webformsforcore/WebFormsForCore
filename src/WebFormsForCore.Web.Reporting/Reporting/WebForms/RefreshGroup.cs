
using System.Web.UI;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal sealed class RefreshGroup : ToolbarGroup
  {
    private ScriptSwitchImage m_refreshButton;

    public RefreshGroup(ReportViewer viewer)
      : base(viewer)
    {
    }

    public override string GroupCssClassName => this.m_viewer.ViewerStyle.ToolbarRefresh;

    protected override void CreateChildControls()
    {
      this.Controls.Clear();
      this.m_refreshButton = new ScriptSwitchImage(new ToolbarImageInfo("Microsoft.Reporting.WebForms.Icons.Refresh.gif"), new ToolbarImageInfo("Microsoft.Reporting.WebForms.Icons.RefreshDisabled.gif"), true, LocalizationHelper.Current.RefreshButtonToolTip, this.m_viewer);
      this.Controls.Add((Control) this.m_refreshButton);
    }

    public override void AddScriptDescriptorProperties(ScriptControlDescriptor toolbarDesc)
    {
      this.EnsureChildControls();
      toolbarDesc.AddElementProperty("RefreshButton", this.m_refreshButton.ClientID);
    }

    public override string LeadingSpace => ToolbarGroup.ReducedLeadingSpace;
  }
}
