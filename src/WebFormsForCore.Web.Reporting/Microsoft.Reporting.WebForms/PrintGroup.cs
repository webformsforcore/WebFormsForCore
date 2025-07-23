using System.Web.UI;

namespace Microsoft.Reporting.WebForms;

internal sealed class PrintGroup : ToolbarGroup
{
	private ScriptSwitchImage m_printButton;

	public override string GroupCssClassName => m_viewer.ViewerStyle.ToolbarPrint;

	public override string LeadingSpace => ToolbarGroup.ReducedLeadingSpace;

	public PrintGroup(ReportViewer viewer)
		: base(viewer)
	{
	}

	protected override void CreateChildControls()
	{
		Controls.Clear();
		m_printButton = new ScriptSwitchImage(new ToolbarImageInfo("Microsoft.Reporting.WebForms.Icons.Print.gif"), new ToolbarImageInfo("Microsoft.Reporting.WebForms.Icons.PrintDisabled.gif"), image2Disabled: true, LocalizationHelper.Current.PrintButtonToolTip, m_viewer);
		m_printButton.ShowImage2 = true;
		Controls.Add(m_printButton);
	}

	public override void AddScriptDescriptorProperties(ScriptControlDescriptor toolbarDesc)
	{
		EnsureChildControls();
		toolbarDesc.AddElementProperty("PrintButton", m_printButton.ClientID);
	}
}
