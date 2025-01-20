
using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal sealed class ScriptSwitchImage : CompositeControl, IScriptControl
  {
    private HoverImage m_image1Hover;
    private HoverImage m_image2Hover;
    private ToolbarImageInfo m_image1;
    private ToolbarImageInfo m_image2;
    private bool m_image2Disabled;
    private string m_tooltip;
    private ReportViewer m_viewer;

    public ScriptSwitchImage(
      ToolbarImageInfo image1,
      ToolbarImageInfo image2,
      bool image2Disabled,
      string tooltip,
      ReportViewer viewer)
    {
      this.m_image1 = image1;
      this.m_image2 = image2;
      this.m_image2Disabled = image2Disabled;
      this.m_tooltip = tooltip;
      this.m_viewer = viewer;
    }

    public event EventHandler ClickImage1;

    protected override HtmlTextWriterTag TagKey => HtmlTextWriterTag.Div;

    protected override void CreateChildControls()
    {
      this.Controls.Clear();
      this.m_image1Hover = new HoverImage(this.m_image1, this.m_tooltip, this.m_viewer);
      if (this.m_viewer.ViewerStyle.Image != null)
        this.m_image1Hover.CssClass = this.m_viewer.ViewerStyle.Image;
      this.m_image1Hover.Click += new EventHandler(this.Image1_Click);
      this.Controls.Add((Control) this.m_image1Hover);
      this.m_image2Hover = new HoverImage(this.m_image2, this.m_tooltip, this.m_viewer);
      this.m_image2Hover.Enabled = !this.m_image2Disabled;
      this.Controls.Add((Control) this.m_image2Hover);
      this.ShowImage2 = true;
    }

    protected override void OnPreRender(EventArgs e)
    {
      ScriptManager.GetCurrent(this.Page)?.RegisterScriptControl<ScriptSwitchImage>(this);
      base.OnPreRender(e);
    }

    protected override void Render(HtmlTextWriter writer)
    {
      this.EnsureChildControls();
      ScriptManager.GetCurrent(this.Page)?.RegisterScriptDescriptors((IScriptControl) this);
      if (this.DesignMode)
        this.m_image2Hover.RenderControl(writer);
      else
        base.Render(writer);
    }

    private void Image1_Click(object sender, EventArgs e)
    {
      if (this.ClickImage1 == null)
        return;
      this.ClickImage1((object) this, e);
    }

    public bool ShowImage2
    {
      get
      {
        this.EnsureChildControls();
        return this.m_image2Hover.ClientVisible;
      }
      set
      {
        this.EnsureChildControls();
        this.m_image1Hover.ClientVisible = !value;
        this.m_image2Hover.ClientVisible = value;
      }
    }

    IEnumerable<ScriptDescriptor> IScriptControl.GetScriptDescriptors()
    {
      this.EnsureChildControls();
      ScriptControlDescriptor controlDescriptor = new ScriptControlDescriptor("Microsoft.Reporting.WebFormsClient._ScriptSwitchImage", this.ClientID);
      controlDescriptor.AddElementProperty("Image1", this.m_image1Hover.ClientID);
      controlDescriptor.AddElementProperty("Image2", this.m_image2Hover.ClientID);
      return (IEnumerable<ScriptDescriptor>) new ScriptDescriptor[1]
      {
        (ScriptDescriptor) controlDescriptor
      };
    }

    IEnumerable<ScriptReference> IScriptControl.GetScriptReferences()
    {
      ScriptReference scriptReference = new ScriptReference();
      scriptReference.Path = EmbeddedResourceOperation.CreateUrlForScriptFile();
      return (IEnumerable<ScriptReference>) new ScriptReference[1]
      {
        scriptReference
      };
    }
  }
}
