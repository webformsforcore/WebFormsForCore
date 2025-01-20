
using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal sealed class HoverImage : CompositeControl, IPostBackEventHandler, IScriptControl
  {
    public bool ClientVisible = true;
    private string m_tooltip;
    private ToolbarImageInfo m_image;
    private ImageButton m_ltrImageButton;
    private ImageButton m_rtlImageButton;
    private IReportViewerStyles m_viewerStyle;
    private ReportViewer m_viewer;

    public HoverImage(ToolbarImageInfo image, string tooltip, ReportViewer viewer)
    {
      this.m_image = image;
      this.m_tooltip = tooltip;
      this.m_viewer = viewer;
      this.m_viewerStyle = viewer.ViewerStyle;
    }

    public event EventHandler Click;

    protected override void CreateChildControls()
    {
      this.Controls.Clear();
      this.m_ltrImageButton = this.CreateImageButton(this.m_image.LTRImageName);
      if (!this.m_image.IsBiDirectional)
        return;
      this.m_rtlImageButton = this.CreateImageButton(this.m_image.RTLImageName);
    }

    private ImageButton CreateImageButton(string imageUrl)
    {
      ImageButton child = new ImageButton();
      child.ImageUrl = EmbeddedResourceOperation.CreateUrl(imageUrl);
      child.CssClass = this.CssClass;
      child.AlternateText = this.m_tooltip;
      child.Attributes.Add("title", this.m_tooltip);
      child.Width = (Unit) 16;
      child.Height = (Unit) 16;
      child.BorderStyle = BorderStyle.None;
      this.Controls.Add((Control) child);
      return child;
    }

    protected override void OnPreRender(EventArgs e)
    {
      ScriptManager.GetCurrent(this.Page)?.RegisterScriptControl<HoverImage>(this);
      base.OnPreRender(e);
    }

    protected override void AddAttributesToRender(HtmlTextWriter writer)
    {
      base.AddAttributesToRender(writer);
      if (this.Enabled)
        return;
      writer.AddAttribute(HtmlTextWriterAttribute.Disabled, "disabled");
    }

    protected override void Render(HtmlTextWriter writer)
    {
      this.EnsureChildControls();
      ReportViewerStyle.ApplyButtonStyle(this.m_viewer.ViewerStyle, (WebControl) this);
      ScriptManager.GetCurrent(this.Page)?.RegisterScriptDescriptors((IScriptControl) this);
      this.AddAttributesToRender(writer);
      if (!this.ClientVisible)
        writer.AddStyleAttribute(HtmlTextWriterStyle.Display, "none");
      writer.RenderBeginTag(HtmlTextWriterTag.Div);
      writer.AddAttribute(HtmlTextWriterAttribute.Title, this.m_tooltip, true);
      writer.RenderBeginTag(HtmlTextWriterTag.Table);
      writer.RenderBeginTag(HtmlTextWriterTag.Tr);
      writer.RenderBeginTag(HtmlTextWriterTag.Td);
      if (this.m_image.IsBiDirectional)
      {
        if (this.IsRtlImageVisible)
          this.m_ltrImageButton.Style.Add(HtmlTextWriterStyle.Display, "none");
        else
          this.m_rtlImageButton.Style.Add(HtmlTextWriterStyle.Display, "none");
        this.RenderImageButton(writer, this.m_ltrImageButton);
        this.RenderImageButton(writer, this.m_rtlImageButton);
      }
      else
        this.RenderImageButton(writer, this.m_ltrImageButton);
      writer.RenderEndTag();
      writer.RenderEndTag();
      writer.RenderEndTag();
      writer.RenderEndTag();
    }

    private void RenderImageButton(HtmlTextWriter writer, ImageButton image)
    {
      if (!this.Enabled)
        image.Style.Add(HtmlTextWriterStyle.Cursor, "default");
      else
        image.Style.Remove(HtmlTextWriterStyle.Cursor);
      image.RenderControl(writer);
    }

    public void RaisePostBackEvent(string eventArgument)
    {
      if (this.Click == null)
        return;
      this.Click((object) this, EventArgs.Empty);
    }

    private bool IsRtlImageVisible
    {
      get => this.m_image.IsBiDirectional && this.m_viewer.IsClientRightToLeft;
    }

    IEnumerable<ScriptDescriptor> IScriptControl.GetScriptDescriptors()
    {
      this.EnsureChildControls();
      ScriptControlDescriptor controlDescriptor = new ScriptControlDescriptor("Microsoft.Reporting.WebFormsClient._HoverImage", this.ClientID);
      controlDescriptor.AddScriptProperty("NormalStyles", ReportViewerStyle.ToolbarItemStyles(this.m_viewerStyle, this.Enabled, true));
      controlDescriptor.AddScriptProperty("HoverStyles", ReportViewerStyle.ToolbarItemStyles(this.m_viewerStyle, this.Enabled, false));
      string script = JavaScriptHelper.FormatAsFunction(this.Page.ClientScript.GetPostBackEventReference((Control) this, (string) null) + ";");
      controlDescriptor.AddScriptProperty("OnClickScript", script);
      if (this.m_image.IsBiDirectional)
      {
        controlDescriptor.AddComponentProperty("ReportViewer", this.m_viewer.ClientID);
        controlDescriptor.AddProperty("IsRtlVisible", (object) this.IsRtlImageVisible);
        controlDescriptor.AddProperty("LTRImageID", (object) this.m_ltrImageButton.ClientID);
        controlDescriptor.AddProperty("RTLImageID", (object) this.m_rtlImageButton.ClientID);
      }
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
