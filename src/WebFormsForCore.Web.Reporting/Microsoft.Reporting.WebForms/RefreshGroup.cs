using System.Web.UI;

namespace Microsoft.Reporting.WebForms;

internal sealed class RefreshGroup : ToolbarGroup
{
	private ScriptSwitchImage m_refreshButton;

	public override string GroupCssClassName => m_viewer.ViewerStyle.ToolbarRefresh;

	public override string LeadingSpace => ToolbarGroup.ReducedLeadingSpace;

	public RefreshGroup(ReportViewer viewer)
		: base(viewer)
	{
	}

	protected override void CreateChildControls()
	{
		Controls.Clear();
		m_refreshButton = new ScriptSwitchImage(new ToolbarImageInfo("Microsoft.Reporting.WebForms.Icons.Refresh.gif"), new ToolbarImageInfo("Microsoft.Reporting.WebForms.Icons.RefreshDisabled.gif"), image2Disabled: true, LocalizationHelper.Current.RefreshButtonToolTip, m_viewer);
		Controls.Add(m_refreshButton);
	}

	public override void AddScriptDescriptorProperties(ScriptControlDescriptor toolbarDesc)
	{
		EnsureChildControls();
		toolbarDesc.AddElementProperty("RefreshButton", m_refreshButton.ClientID);
	}
}
