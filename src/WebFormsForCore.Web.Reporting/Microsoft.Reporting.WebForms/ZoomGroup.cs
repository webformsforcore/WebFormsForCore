using System;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Microsoft.Reporting.WebForms;

internal sealed class ZoomGroup : ToolbarGroup
{
	private DropDownList m_dropDown;

	public override string GroupCssClassName => m_viewer.ViewerStyle.ToolbarZoom;

	public ZoomGroup(ReportViewer viewer)
		: base(viewer)
	{
		m_viewer = viewer;
	}

	protected override void CreateChildControls()
	{
		Controls.Clear();
		m_dropDown = new DropDownList();
		m_dropDown.Enabled = false;
		m_dropDown.ToolTip = LocalizationHelper.Current.ZoomControlToolTip;
		Controls.Add(m_dropDown);
	}

	protected override void OnPreRender(EventArgs e)
	{
		EnsureChildControls();
		base.OnPreRender(e);
		m_dropDown.Items.Clear();
		m_dropDown.Items.Add(new ListItem(LocalizationHelper.Current.ZoomToPageWidth, ZoomMode.PageWidth.ToString()));
		m_dropDown.Items.Add(new ListItem(LocalizationHelper.Current.ZoomToWholePage, ZoomMode.FullPage.ToString()));
		m_dropDown.Items.Add(new ListItem(Unit.Percentage(500.0).ToString(), "500"));
		m_dropDown.Items.Add(new ListItem(Unit.Percentage(200.0).ToString(), "200"));
		m_dropDown.Items.Add(new ListItem(Unit.Percentage(150.0).ToString(), "150"));
		m_dropDown.Items.Add(new ListItem(Unit.Percentage(100.0).ToString(), "100"));
		m_dropDown.Items.Add(new ListItem(Unit.Percentage(75.0).ToString(), "75"));
		m_dropDown.Items.Add(new ListItem(Unit.Percentage(50.0).ToString(), "50"));
		m_dropDown.Items.Add(new ListItem(Unit.Percentage(25.0).ToString(), "25"));
		m_dropDown.Items.Add(new ListItem(Unit.Percentage(10.0).ToString(), "10"));
		string text = Global.ZoomString(m_viewer.ZoomMode, m_viewer.ZoomPercent);
		if (m_dropDown.Items.FindByValue(text) == null)
		{
			int num = Convert.ToInt32(text, CultureInfo.InvariantCulture);
			string text2 = Unit.Percentage(num).ToString(CultureInfo.CurrentCulture);
			m_dropDown.Items.Add(new ListItem(text2, text));
		}
		m_dropDown.SelectedValue = text;
	}

	protected override void Render(HtmlTextWriter writer)
	{
		EnsureChildControls();
		m_dropDown.Font.CopyFrom(Font);
		base.Render(writer);
	}

	public override void AddScriptDescriptorProperties(ScriptControlDescriptor toolbarDesc)
	{
		EnsureChildControls();
		toolbarDesc.AddElementProperty("ZoomDropDown", m_dropDown.ClientID);
	}
}
