using System;
using System.Globalization;
using System.Web.UI;

namespace Microsoft.Reporting.WebForms;

internal sealed class ExportGroup : ToolbarGroup
{
	private DropDownMenu m_exportButton;

	public override string GroupCssClassName => m_viewer.ViewerStyle.ToolbarExport;

	public ExportGroup(ReportViewer viewer)
		: base(viewer)
	{
		m_viewer = viewer;
	}

	protected override void CreateChildControls()
	{
		Controls.Clear();
		ButtonImageInfo buttonImageInfo = new ButtonImageInfo();
		buttonImageInfo.EnabledUrl = EmbeddedResourceOperation.CreateUrl("Microsoft.Reporting.WebForms.Icons.Export.gif");
		buttonImageInfo.DisabledUrl = EmbeddedResourceOperation.CreateUrl("Microsoft.Reporting.WebForms.Icons.ExportDisabled.gif");
		m_exportButton = new DropDownMenu(buttonImageInfo, LocalizationHelper.Current.ExportButtonToolTip, m_viewer.ViewerStyle);
		m_exportButton.ShowEnabled = false;
		Controls.Add(m_exportButton);
	}

	protected override void OnPreRender(EventArgs e)
	{
		EnsureChildControls();
		PopulateDropDown();
		base.OnPreRender(e);
	}

	protected override void Render(HtmlTextWriter writer)
	{
		EnsureChildControls();
		m_exportButton.Font.CopyFrom(Font);
		base.Render(writer);
	}

	private void PopulateDropDown()
	{
		m_exportButton.Items.Clear();
		m_exportButton.Font.CopyFrom(Font);
		LocalizationHelper localizationHelper = (LocalizationHelper)LocalizationHelper.Current;
		foreach (RenderingExtension clientSupportedRenderingExtension in GetClientSupportedRenderingExtensions())
		{
			if (clientSupportedRenderingExtension.Visible)
			{
				MenuInfo menuInfo = new MenuInfo();
				menuInfo.Text = localizationHelper.GetLocalizedNameForRenderingExtension(clientSupportedRenderingExtension);
				menuInfo.ClientScript = string.Format(CultureInfo.InvariantCulture, "$find('{0}').exportReport('{1}');", JavaScriptHelper.JavaScriptStringEscape(m_viewer.ClientID, '\''), JavaScriptHelper.JavaScriptStringEscape(clientSupportedRenderingExtension.Name, '\''));
				m_exportButton.Items.Add(menuInfo);
			}
		}
	}

	public override void AddScriptDescriptorProperties(ScriptControlDescriptor toolbarDesc)
	{
		EnsureChildControls();
		toolbarDesc.AddElementProperty("ExportButton", m_exportButton.ClientID);
	}
}
