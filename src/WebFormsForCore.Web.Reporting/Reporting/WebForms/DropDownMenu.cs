
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Security;
using System.Security.Permissions;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
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

    public DropDownMenu(ButtonImageInfo image, string tooltip, IReportViewerStyles viewerStyle)
    {
      this.m_tooltip = tooltip;
      this.m_viewerStyle = viewerStyle;
      this.m_buttonImageInfo = image;
      this.EnableViewState = false;
    }

    protected override void CreateChildControls()
    {
      this.Controls.Clear();
      base.CreateChildControls();
      if (this.m_arrowImageInfo == null)
      {
        this.m_arrowImageInfo = new ButtonImageInfo();
        this.m_arrowImageInfo.EnabledUrl = EmbeddedResourceOperation.CreateUrl("Microsoft.Reporting.WebForms.Icons.ArrowDown.gif");
        this.m_arrowImageInfo.DisabledUrl = EmbeddedResourceOperation.CreateUrl("Microsoft.Reporting.WebForms.Icons.ArrowDownDisabled.gif");
      }
      this.m_buttons = this.GetButtonControl();
      this.m_menu = this.GetMenuControl();
      this.Controls.Add((Control) this.m_buttons);
      this.Controls.Add((Control) this.m_menu);
    }

    private Table GetButtonControl()
    {
      Table buttonControl = new Table();
      buttonControl.ToolTip = this.m_tooltip;
      buttonControl.ID = "Button";
      TableRow row = new TableRow();
      TableCell cell = new TableCell();
      buttonControl.Rows.Add(row);
      row.Cells.Add(cell);
      HyperLink child1 = new HyperLink();
      child1.ID = buttonControl.ID + "Link";
      child1.Attributes.Add("title", LocalizationHelper.Current.ExportButtonToolTip);
      child1.Attributes.Add("alt", LocalizationHelper.Current.ExportButtonToolTip);
      child1.NavigateUrl = "javascript:void(0)";
      child1.Style.Add(HtmlTextWriterStyle.TextDecoration, "none");
      cell.Controls.Add((Control) child1);
      System.Web.UI.WebControls.Image child2 = new System.Web.UI.WebControls.Image();
      child2.AlternateText = LocalizationHelper.Current.ExportButtonToolTip;
      child2.ImageUrl = this.ShowEnabled ? this.m_buttonImageInfo.EnabledUrl : this.m_buttonImageInfo.DisabledUrl;
      child2.ID = buttonControl.ID + "Img";
      child2.Width = (Unit) 16;
      child2.Height = (Unit) 16;
      child2.BorderStyle = BorderStyle.None;
      child1.Controls.Add((Control) child2);
      SafeLiteralControl child3 = new SafeLiteralControl(" ");
      child3.Style.Add(HtmlTextWriterStyle.Width, "5px");
      child3.Style.Add(HtmlTextWriterStyle.TextDecoration, "none");
      child1.Controls.Add((Control) child3);
      System.Web.UI.WebControls.Image child4 = new System.Web.UI.WebControls.Image();
      child4.AlternateText = LocalizationHelper.Current.ExportButtonToolTip;
      child4.ID = buttonControl.ID + "ImgDown";
      child4.ImageUrl = this.ShowEnabled ? this.m_arrowImageInfo.EnabledUrl : this.m_arrowImageInfo.DisabledUrl;
      child4.Width = (Unit) 7;
      child4.Height = (Unit) 6;
      child4.BorderStyle = BorderStyle.None;
      child4.Style.Add(HtmlTextWriterStyle.MarginBottom, "5px");
      child1.Controls.Add((Control) child4);
      return buttonControl;
    }

    private Panel GetMenuControl()
    {
      Panel menuControl = new Panel();
      menuControl.ID = "Menu";
      menuControl.Style.Add(HtmlTextWriterStyle.Display, "none");
      menuControl.Style.Add(HtmlTextWriterStyle.Position, "absolute");
      menuControl.Style.Add(HtmlTextWriterStyle.Padding, "1px");
      menuControl.Style.Add(HtmlTextWriterStyle.ZIndex, "1");
      if (!string.IsNullOrEmpty(this.m_viewerStyle.HoverButtonBorderValue))
        menuControl.Style.Add("border", this.m_viewerStyle.HoverButtonBorderValue);
      if (this.m_viewerStyle.ViewerAreaBackground != null)
        menuControl.CssClass = this.m_viewerStyle.ViewerAreaBackground;
      else
        menuControl.BackColor = this.m_viewerStyle.BackColor.IsEmpty ? Color.White : this.m_viewerStyle.BackColor;
      return menuControl;
    }

    public IList<MenuInfo> Items => (IList<MenuInfo>) this.m_items;

    protected override void OnPreRender(EventArgs e)
    {
      this.EnsureChildControls();
      foreach (MenuInfo menuInfo in (IEnumerable<MenuInfo>) this.Items)
      {
        Panel child1 = new Panel();
        HyperLink child2 = new HyperLink();
        child2.Text = HttpUtility.HtmlEncode(menuInfo.Text);
        child2.Attributes.Add("title", menuInfo.Text);
        child2.Attributes.Add("alt", menuInfo.Text);
        child2.NavigateUrl = "javascript:void(0)";
        child2.Style.Add(HtmlTextWriterStyle.Padding, "3px 8px 3px 8px");
        child2.Style.Add(HtmlTextWriterStyle.Display, "block");
        child2.Style.Add(HtmlTextWriterStyle.WhiteSpace, "nowrap");
        child2.Style.Add(HtmlTextWriterStyle.TextDecoration, "none");
        if (this.m_viewerStyle.LinkActive != null)
          child2.CssClass = this.m_viewerStyle.LinkActive;
        else
          child2.ForeColor = this.m_viewerStyle.LinkActiveColor;
        child2.Attributes.Add("onclick", menuInfo.ClientScript);
        child1.Controls.Add((Control) child2);
        this.m_menu.Controls.Add((Control) child1);
      }
      base.OnPreRender(e);
    }

    protected override void Render(HtmlTextWriter writer)
    {
      this.EnsureChildControls();
      ReportViewerStyle.ApplyButtonStyle(this.m_viewerStyle, (WebControl) this);
      this.m_menu.Visible = false;
      base.Render(writer);
      if (this.DesignMode)
        return;
      this.m_menu.Visible = true;
      foreach (WebControl control in this.m_menu.Controls)
      {
        ReportViewerStyle.ApplyButtonStyle(this.m_viewerStyle, control);
        ((WebControl) control.Controls[0]).Font.CopyFrom(this.Font);
      }
      this.m_menu.RenderControl(writer);
    }

    public override IEnumerable<ScriptDescriptor> GetScriptDescriptors()
    {
      this.EnsureChildControls();
      ScriptControlDescriptor controlDescriptor = new ScriptControlDescriptor("Microsoft.Reporting.WebFormsClient._DropDownMenu", this.ClientID);
      controlDescriptor.AddScriptProperty("NormalStyles", ReportViewerStyle.ToolbarItemStyles(this.m_viewerStyle, this.ShowEnabled, true));
      controlDescriptor.AddScriptProperty("HoverStyles", ReportViewerStyle.ToolbarItemStyles(this.m_viewerStyle, this.ShowEnabled, false));
      controlDescriptor.AddProperty("MenuId", (object) this.m_menu.ClientID);
      controlDescriptor.AddProperty("ButtonId", (object) this.m_buttons.ClientID);
      controlDescriptor.AddProperty("Enabled", (object) this.ShowEnabled);
      JavaScriptSerializer serializer = new JavaScriptSerializer();
      controlDescriptor.AddProperty("ButtonImages", (object) this.SerializeButtonInfo(serializer, this.m_buttonImageInfo));
      controlDescriptor.AddProperty("ArrowImages", (object) this.SerializeButtonInfo(serializer, this.m_arrowImageInfo));
      return (IEnumerable<ScriptDescriptor>) new ScriptDescriptor[1]
      {
        (ScriptDescriptor) controlDescriptor
      };
    }

    private string SerializeButtonInfo(JavaScriptSerializer serializer, ButtonImageInfo buttonInfo)
    {
      return SecurityAssertionHandler.RunWithSecurityAssert<string>((CodeAccessPermission) new ReflectionPermission(ReflectionPermissionFlag.MemberAccess), (Func<string>) (() => serializer.Serialize((object) buttonInfo)));
    }
  }
}
