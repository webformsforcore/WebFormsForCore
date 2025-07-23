using System;
using System.Web.UI;

namespace Microsoft.Reporting.WebForms;

internal sealed class BackGroup : ToolbarGroup
{
	private ScriptSwitchImage m_backButton;

	public override string GroupCssClassName => null;

	public BackGroup(ReportViewer viewer)
		: base(viewer)
	{
	}

	protected override void CreateChildControls()
	{
		Controls.Clear();
		m_backButton = new ScriptSwitchImage(new ToolbarImageInfo("Microsoft.Reporting.WebForms.Icons.BackEnabled.gif", "Microsoft.Reporting.WebForms.Icons.BackRTLEnabled.gif"), new ToolbarImageInfo("Microsoft.Reporting.WebForms.Icons.BackDisabled.gif", "Microsoft.Reporting.WebForms.Icons.BackRTLDisabled.gif"), image2Disabled: true, LocalizationHelper.Current.BackButtonToolTip, m_viewer);
		m_backButton.ClickImage1 += BackButton_Click;
		Controls.Add(m_backButton);
	}

	protected override void OnPreRender(EventArgs e)
	{
		base.OnPreRender(e);
		if (m_viewer.InteractivityPostBackMode != InteractivityPostBackMode.AlwaysAsynchronous)
		{
			ScriptManager.GetCurrent(Page).RegisterPostBackControl(m_backButton);
		}
	}

	private void BackButton_Click(object sender, EventArgs e)
	{
		OnReportAction(new ReportActionEventArgs("Back", null));
	}

	public override void AddScriptDescriptorProperties(ScriptControlDescriptor toolbarDesc)
	{
		EnsureChildControls();
		toolbarDesc.AddProperty("IsDrillthroughReport", m_viewer.Report.IsDrillthroughReport);
		toolbarDesc.AddElementProperty("DrillBackButton", m_backButton.ClientID);
	}
}
