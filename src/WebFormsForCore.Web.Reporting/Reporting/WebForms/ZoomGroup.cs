
using System;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal sealed class ZoomGroup : ToolbarGroup
  {
    private DropDownList m_dropDown;

    public ZoomGroup(ReportViewer viewer)
      : base(viewer)
    {
      this.m_viewer = viewer;
    }

    public override string GroupCssClassName => this.m_viewer.ViewerStyle.ToolbarZoom;

    protected override void CreateChildControls()
    {
      this.Controls.Clear();
      this.m_dropDown = new DropDownList();
      this.m_dropDown.Enabled = false;
      this.m_dropDown.ToolTip = LocalizationHelper.Current.ZoomControlToolTip;
      this.Controls.Add((Control) this.m_dropDown);
    }

    protected override void OnPreRender(EventArgs e)
    {
      this.EnsureChildControls();
      base.OnPreRender(e);
      this.m_dropDown.Items.Clear();
      this.m_dropDown.Items.Add(new ListItem(LocalizationHelper.Current.ZoomToPageWidth, ZoomMode.PageWidth.ToString()));
      this.m_dropDown.Items.Add(new ListItem(LocalizationHelper.Current.ZoomToWholePage, ZoomMode.FullPage.ToString()));
      this.m_dropDown.Items.Add(new ListItem(Unit.Percentage(500.0).ToString(), "500"));
      this.m_dropDown.Items.Add(new ListItem(Unit.Percentage(200.0).ToString(), "200"));
      this.m_dropDown.Items.Add(new ListItem(Unit.Percentage(150.0).ToString(), "150"));
      this.m_dropDown.Items.Add(new ListItem(Unit.Percentage(100.0).ToString(), "100"));
      this.m_dropDown.Items.Add(new ListItem(Unit.Percentage(75.0).ToString(), "75"));
      this.m_dropDown.Items.Add(new ListItem(Unit.Percentage(50.0).ToString(), "50"));
      this.m_dropDown.Items.Add(new ListItem(Unit.Percentage(25.0).ToString(), "25"));
      this.m_dropDown.Items.Add(new ListItem(Unit.Percentage(10.0).ToString(), "10"));
      string str = Global.ZoomString(this.m_viewer.ZoomMode, this.m_viewer.ZoomPercent);
      if (this.m_dropDown.Items.FindByValue(str) == null)
        this.m_dropDown.Items.Add(new ListItem(Unit.Percentage((double) Convert.ToInt32(str, (IFormatProvider) CultureInfo.InvariantCulture)).ToString(CultureInfo.CurrentCulture), str));
      this.m_dropDown.SelectedValue = str;
    }

    protected override void Render(HtmlTextWriter writer)
    {
      this.EnsureChildControls();
      this.m_dropDown.Font.CopyFrom(this.Font);
      base.Render(writer);
    }

    public override void AddScriptDescriptorProperties(ScriptControlDescriptor toolbarDesc)
    {
      this.EnsureChildControls();
      toolbarDesc.AddElementProperty("ZoomDropDown", this.m_dropDown.ClientID);
    }
  }
}
