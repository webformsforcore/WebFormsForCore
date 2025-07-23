using System;
using System.Web.UI;

namespace Microsoft.Reporting.WebForms;

internal sealed class AtomDataFeedGroup : ToolbarGroup
{
	private ScriptSwitchImage m_atomDataFeedButton;

	public bool IsSupported
	{
		get
		{
			foreach (RenderingExtension clientSupportedRenderingExtension in GetClientSupportedRenderingExtensions())
			{
				if (clientSupportedRenderingExtension.Name.Equals("ATOM", StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
			}
			return false;
		}
	}

	public override string GroupCssClassName => m_viewer.ViewerStyle.ToolbarAtomDataFeed;

	public override string LeadingSpace => ToolbarGroup.ReducedLeadingSpace;

	public AtomDataFeedGroup(ReportViewer viewer)
		: base(viewer)
	{
	}

	protected override void CreateChildControls()
	{
		Controls.Clear();
		m_atomDataFeedButton = new ScriptSwitchImage(new ToolbarImageInfo("Microsoft.Reporting.WebForms.Icons.AtomDataFeed.gif"), new ToolbarImageInfo("Microsoft.Reporting.WebForms.Icons.AtomDataFeedDisabled.gif"), image2Disabled: true, Strings.AtomDataFeedTooltip, m_viewer);
		m_atomDataFeedButton.ShowImage2 = true;
		Controls.Add(m_atomDataFeedButton);
	}

	public override void AddScriptDescriptorProperties(ScriptControlDescriptor toolbarDesc)
	{
		EnsureChildControls();
		toolbarDesc.AddElementProperty("AtomDataFeedButton", m_atomDataFeedButton.ClientID);
	}
}
