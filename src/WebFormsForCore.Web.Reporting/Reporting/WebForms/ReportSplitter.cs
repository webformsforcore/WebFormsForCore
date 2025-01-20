
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal class ReportSplitter : CompositeScriptControl
  {
    private IReportViewerStyles m_viewerStyle;
    private string m_tooltip;
    private bool m_isVertical;
    private bool m_isResizable = true;
    private bool m_isCollapsable = true;
    private HiddenField m_position;
    private HiddenField m_collapseState;
    private ImageButton m_image;

    public ReportSplitter(IReportViewerStyles viewerStyle, bool isVertical, string tooltip)
    {
      this.m_isVertical = isVertical;
      this.m_viewerStyle = viewerStyle;
      this.m_tooltip = tooltip;
    }

    protected override void CreateChildControls()
    {
      base.CreateChildControls();
      this.m_image = new ImageButton();
      this.m_image.ID = "img";
      this.m_image.AlternateText = this.m_tooltip;
      this.m_image.ToolTip = this.m_tooltip;
      if (this.m_isVertical)
      {
        this.m_image.ImageUrl = EmbeddedResourceOperation.CreateUrl("Microsoft.Reporting.WebForms.Icons.SplitterVertCollapse.png");
        this.m_image.ImageAlign = ImageAlign.Top;
      }
      else
      {
        this.m_image.ImageUrl = EmbeddedResourceOperation.CreateUrl("Microsoft.Reporting.WebForms.Icons.SplitterHorizCollapse.png");
        this.m_image.ImageAlign = ImageAlign.Middle;
      }
      this.m_image.OnClientClick = "void(0)";
      this.m_image.Style[HtmlTextWriterStyle.Cursor] = "pointer";
      this.Controls.Add((Control) this.m_image);
      this.m_position = new HiddenField();
      this.m_position.ID = "store";
      this.Controls.Add((Control) this.m_position);
      this.m_collapseState = new HiddenField();
      this.m_collapseState.ID = "collapse";
      this.m_collapseState.ValueChanged += new EventHandler(this.CollapseState_ValueChanged);
      this.Controls.Add((Control) this.m_collapseState);
    }

    private void CollapseState_ValueChanged(object sender, EventArgs e)
    {
      if (this.CollapsedChanged == null)
        return;
      this.CollapsedChanged((object) this, EventArgs.Empty);
    }

    public event EventHandler CollapsedChanged;

    public bool IsCollapsed
    {
      get
      {
        this.EnsureChildControls();
        return string.Equals(this.m_collapseState.Value, "true", StringComparison.OrdinalIgnoreCase);
      }
      set
      {
        this.EnsureChildControls();
        this.m_collapseState.Value = value ? "true" : "false";
      }
    }

    public bool IsCollapsable
    {
      get => this.m_isCollapsable;
      set => this.m_isCollapsable = value;
    }

    public bool IsResizable
    {
      get => this.m_isResizable;
      set => this.m_isResizable = value;
    }

    public void WriteTableCellCenteringStyles(HtmlTextWriter writer)
    {
      if (this.m_isVertical)
        writer.AddStyleAttribute(HtmlTextWriterStyle.VerticalAlign, "middle");
      else
        writer.AddStyleAttribute(HtmlTextWriterStyle.TextAlign, "center");
      if (this.m_viewerStyle.SplitterNormal != null)
        writer.AddAttribute(HtmlTextWriterAttribute.Class, this.m_viewerStyle.SplitterNormal);
      else
        writer.AddStyleAttribute(HtmlTextWriterStyle.BackgroundColor, ColorTranslator.ToHtml(this.m_viewerStyle.SplitterBackColor));
    }

    protected override void OnPreRender(EventArgs e)
    {
      this.EnsureID();
      base.OnPreRender(e);
    }

    protected override void Render(HtmlTextWriter writer)
    {
      this.EnsureChildControls();
      if (this.IsCollapsed)
        this.m_image.ImageUrl = EmbeddedResourceOperation.CreateUrl(this.m_isVertical ? "Microsoft.Reporting.WebForms.Icons.SplitterVertExpand.png" : "Microsoft.Reporting.WebForms.Icons.SplitterHorizExpand.png");
      if (!this.m_isCollapsable)
        this.m_image.Style[HtmlTextWriterStyle.Display] = "none";
      base.Render(writer);
    }

    public override IEnumerable<ScriptDescriptor> GetScriptDescriptors()
    {
      ScriptControlDescriptor controlDescriptor = new ScriptControlDescriptor("Microsoft.Reporting.WebFormsClient._Splitter", this.ClientID);
      if (this.m_viewerStyle.SplitterNormal != null)
      {
        controlDescriptor.AddProperty("NormalStyle", (object) this.m_viewerStyle.SplitterNormal);
        controlDescriptor.AddProperty("HoverStyle", (object) this.m_viewerStyle.SplitterHover);
      }
      else
      {
        controlDescriptor.AddProperty("NormalColor", (object) ColorTranslator.ToHtml(this.m_viewerStyle.SplitterBackColor));
        controlDescriptor.AddProperty("HoverColor", (object) ColorTranslator.ToHtml(this.m_viewerStyle.HoverBackColor));
      }
      controlDescriptor.AddProperty("Vertical", (object) this.m_isVertical);
      controlDescriptor.AddProperty("Resizable", (object) this.m_isResizable);
      controlDescriptor.AddProperty("StorePositionField", (object) this.m_position.ClientID);
      controlDescriptor.AddProperty("StoreCollapseField", (object) this.m_collapseState.ClientID);
      controlDescriptor.AddProperty("IsCollapsable", (object) this.IsCollapsable);
      controlDescriptor.AddProperty("ImageId", (object) this.m_image.ClientID);
      if (this.m_isVertical)
      {
        controlDescriptor.AddProperty("ImageCollapse", (object) EmbeddedResourceOperation.CreateUrl("Microsoft.Reporting.WebForms.Icons.SplitterVertCollapse.png"));
        controlDescriptor.AddProperty("ImageCollapseHover", (object) EmbeddedResourceOperation.CreateUrl("Microsoft.Reporting.WebForms.Icons.SplitterVertCollapseHover.png"));
        controlDescriptor.AddProperty("ImageExpand", (object) EmbeddedResourceOperation.CreateUrl("Microsoft.Reporting.WebForms.Icons.SplitterVertExpand.png"));
        controlDescriptor.AddProperty("ImageExpandHover", (object) EmbeddedResourceOperation.CreateUrl("Microsoft.Reporting.WebForms.Icons.SplitterVertExpandHover.png"));
      }
      else
      {
        controlDescriptor.AddProperty("ImageCollapse", (object) EmbeddedResourceOperation.CreateUrl("Microsoft.Reporting.WebForms.Icons.SplitterHorizCollapse.png"));
        controlDescriptor.AddProperty("ImageCollapseHover", (object) EmbeddedResourceOperation.CreateUrl("Microsoft.Reporting.WebForms.Icons.SplitterHorizCollapseHover.png"));
        controlDescriptor.AddProperty("ImageExpand", (object) EmbeddedResourceOperation.CreateUrl("Microsoft.Reporting.WebForms.Icons.SplitterHorizExpand.png"));
        controlDescriptor.AddProperty("ImageExpandHover", (object) EmbeddedResourceOperation.CreateUrl("Microsoft.Reporting.WebForms.Icons.SplitterHorizExpandHover.png"));
      }
      return (IEnumerable<ScriptDescriptor>) new ScriptDescriptor[1]
      {
        (ScriptDescriptor) controlDescriptor
      };
    }
  }
}
