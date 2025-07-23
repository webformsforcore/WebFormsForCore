using System;
using System.Collections.Generic;
using System.Drawing;
using System.Security.Permissions;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Microsoft.Reporting.WebForms;

internal class DropDownMenu : CompositeScriptControl
{
	private Table m_buttons;

	private Panel m_menu;

	private ButtonImageInfo m_arrowImageInfo;

	private ButtonImageInfo m_buttonImageInfo;

	private string m_tooltip;

	private IReportViewerStyles m_viewerStyle;

	public bool ShowEnabled;

	private List<MenuInfo> m_items = new List<MenuInfo>();

	public IList<MenuInfo> Items => m_items;

	public DropDownMenu(ButtonImageInfo image, string tooltip, IReportViewerStyles viewerStyle)
	{
		m_tooltip = tooltip;
		m_viewerStyle = viewerStyle;
		m_buttonImageInfo = image;
		EnableViewState = false;
	}

	protected override void CreateChildControls()
	{
		Controls.Clear();
		base.CreateChildControls();
		if (m_arrowImageInfo == null)
		{
			m_arrowImageInfo = new ButtonImageInfo();
			m_arrowImageInfo.EnabledUrl = EmbeddedResourceOperation.CreateUrl("Microsoft.Reporting.WebForms.Icons.ArrowDown.gif");
			m_arrowImageInfo.DisabledUrl = EmbeddedResourceOperation.CreateUrl("Microsoft.Reporting.WebForms.Icons.ArrowDownDisabled.gif");
		}
		m_buttons = GetButtonControl();
		m_menu = GetMenuControl();
		Controls.Add(m_buttons);
		Controls.Add(m_menu);
	}

	private Table GetButtonControl()
	{
		Table table = new Table();
		table.ToolTip = m_tooltip;
		table.ID = "Button";
		TableRow tableRow = new TableRow();
		TableCell tableCell = new TableCell();
		table.Rows.Add(tableRow);
		tableRow.Cells.Add(tableCell);
		HyperLink hyperLink = new HyperLink();
		hyperLink.ID = table.ID + "Link";
		hyperLink.Attributes.Add("title", LocalizationHelper.Current.ExportButtonToolTip);
		hyperLink.Attributes.Add("alt", LocalizationHelper.Current.ExportButtonToolTip);
		hyperLink.NavigateUrl = "javascript:void(0)";
		hyperLink.Style.Add(HtmlTextWriterStyle.TextDecoration, "none");
		tableCell.Controls.Add(hyperLink);
		System.Web.UI.WebControls.Image image = new System.Web.UI.WebControls.Image();
		image.AlternateText = LocalizationHelper.Current.ExportButtonToolTip;
		image.ImageUrl = (ShowEnabled ? m_buttonImageInfo.EnabledUrl : m_buttonImageInfo.DisabledUrl);
		image.ID = table.ID + "Img";
		image.Width = 16;
		image.Height = 16;
		image.BorderStyle = BorderStyle.None;
		hyperLink.Controls.Add(image);
		SafeLiteralControl safeLiteralControl = new SafeLiteralControl(" ");
		safeLiteralControl.Style.Add(HtmlTextWriterStyle.Width, "5px");
		safeLiteralControl.Style.Add(HtmlTextWriterStyle.TextDecoration, "none");
		hyperLink.Controls.Add(safeLiteralControl);
		System.Web.UI.WebControls.Image image2 = new System.Web.UI.WebControls.Image();
		image2.AlternateText = LocalizationHelper.Current.ExportButtonToolTip;
		image2.ID = table.ID + "ImgDown";
		image2.ImageUrl = (ShowEnabled ? m_arrowImageInfo.EnabledUrl : m_arrowImageInfo.DisabledUrl);
		image2.Width = 7;
		image2.Height = 6;
		image2.BorderStyle = BorderStyle.None;
		image2.Style.Add(HtmlTextWriterStyle.MarginBottom, "5px");
		hyperLink.Controls.Add(image2);
		return table;
	}

	private Panel GetMenuControl()
	{
		Panel panel = new Panel();
		panel.ID = "Menu";
		panel.Style.Add(HtmlTextWriterStyle.Display, "none");
		panel.Style.Add(HtmlTextWriterStyle.Position, "absolute");
		panel.Style.Add(HtmlTextWriterStyle.Padding, "1px");
		panel.Style.Add(HtmlTextWriterStyle.ZIndex, "1");
		if (!string.IsNullOrEmpty(m_viewerStyle.HoverButtonBorderValue))
		{
			panel.Style.Add("border", m_viewerStyle.HoverButtonBorderValue);
		}
		if (m_viewerStyle.ViewerAreaBackground != null)
		{
			panel.CssClass = m_viewerStyle.ViewerAreaBackground;
		}
		else
		{
			panel.BackColor = (m_viewerStyle.BackColor.IsEmpty ? Color.White : m_viewerStyle.BackColor);
		}
		return panel;
	}

	protected override void OnPreRender(EventArgs e)
	{
		EnsureChildControls();
		foreach (MenuInfo item in Items)
		{
			Panel panel = new Panel();
			HyperLink hyperLink = new HyperLink();
			hyperLink.Text = HttpUtility.HtmlEncode(item.Text);
			hyperLink.Attributes.Add("title", item.Text);
			hyperLink.Attributes.Add("alt", item.Text);
			hyperLink.NavigateUrl = "javascript:void(0)";
			hyperLink.Style.Add(HtmlTextWriterStyle.Padding, "3px 8px 3px 8px");
			hyperLink.Style.Add(HtmlTextWriterStyle.Display, "block");
			hyperLink.Style.Add(HtmlTextWriterStyle.WhiteSpace, "nowrap");
			hyperLink.Style.Add(HtmlTextWriterStyle.TextDecoration, "none");
			if (m_viewerStyle.LinkActive != null)
			{
				hyperLink.CssClass = m_viewerStyle.LinkActive;
			}
			else
			{
				hyperLink.ForeColor = m_viewerStyle.LinkActiveColor;
			}
			hyperLink.Attributes.Add("onclick", item.ClientScript);
			panel.Controls.Add(hyperLink);
			m_menu.Controls.Add(panel);
		}
		base.OnPreRender(e);
	}

	protected override void Render(HtmlTextWriter writer)
	{
		EnsureChildControls();
		ReportViewerStyle.ApplyButtonStyle(m_viewerStyle, this);
		m_menu.Visible = false;
		base.Render(writer);
		if (base.DesignMode)
		{
			return;
		}
		m_menu.Visible = true;
		foreach (WebControl control in m_menu.Controls)
		{
			ReportViewerStyle.ApplyButtonStyle(m_viewerStyle, control);
			((HyperLink)control.Controls[0]).Font.CopyFrom(Font);
		}
		m_menu.RenderControl(writer);
	}

	public override IEnumerable<ScriptDescriptor> GetScriptDescriptors()
	{
		EnsureChildControls();
		ScriptControlDescriptor scriptControlDescriptor = new ScriptControlDescriptor("Microsoft.Reporting.WebFormsClient._DropDownMenu", ClientID);
		scriptControlDescriptor.AddScriptProperty("NormalStyles", ReportViewerStyle.ToolbarItemStyles(m_viewerStyle, ShowEnabled, normal: true));
		scriptControlDescriptor.AddScriptProperty("HoverStyles", ReportViewerStyle.ToolbarItemStyles(m_viewerStyle, ShowEnabled, normal: false));
		scriptControlDescriptor.AddProperty("MenuId", m_menu.ClientID);
		scriptControlDescriptor.AddProperty("ButtonId", m_buttons.ClientID);
		scriptControlDescriptor.AddProperty("Enabled", ShowEnabled);
		JavaScriptSerializer serializer = new JavaScriptSerializer();
		scriptControlDescriptor.AddProperty("ButtonImages", SerializeButtonInfo(serializer, m_buttonImageInfo));
		scriptControlDescriptor.AddProperty("ArrowImages", SerializeButtonInfo(serializer, m_arrowImageInfo));
		return new ScriptDescriptor[1] { scriptControlDescriptor };
	}

	private string SerializeButtonInfo(JavaScriptSerializer serializer, ButtonImageInfo buttonInfo)
	{
		return SecurityAssertionHandler.RunWithSecurityAssert(new ReflectionPermission(ReflectionPermissionFlag.MemberAccess), () => serializer.Serialize(buttonInfo));
	}
}
