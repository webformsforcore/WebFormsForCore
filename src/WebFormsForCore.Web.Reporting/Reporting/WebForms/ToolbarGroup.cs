// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.ToolbarGroup
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal abstract class ToolbarGroup : CompositeControl
  {
    internal static readonly string NormalLeadingSpace = "20px";
    internal static readonly string ReducedLeadingSpace = "6px";
    public Unit ContainedControlHeight = Unit.Pixel(28);
    protected ReportViewer m_viewer;

    internal ToolbarGroup(ReportViewer viewer)
    {
      this.m_viewer = viewer;
      this.CssClass = this.GroupCssClassName + " " + viewer.ViewerStyle.ToolbarGroup;
    }

    public event EventHandler<ReportActionEventArgs> ReportAction;

    protected void OnReportAction(ReportActionEventArgs args)
    {
      if (this.ReportAction == null)
        return;
      this.ReportAction((object) this, args);
    }

    public abstract void AddScriptDescriptorProperties(ScriptControlDescriptor toolbarDesc);

    public abstract string GroupCssClassName { get; }

    protected override void Render(HtmlTextWriter writer)
    {
      this.EnsureChildControls();
      this.AddAttributesToRender(writer);
      writer.AddStyleAttribute(HtmlTextWriterStyle.VerticalAlign, "top");
      writer.RenderBeginTag(HtmlTextWriterTag.Div);
      writer.AddAttribute(HtmlTextWriterAttribute.Cellpadding, "0");
      writer.AddAttribute(HtmlTextWriterAttribute.Cellspacing, "0");
      writer.AddStyleAttribute(HtmlTextWriterStyle.Display, "inline");
      writer.RenderBeginTag(HtmlTextWriterTag.Table);
      writer.RenderBeginTag(HtmlTextWriterTag.Tr);
      this.RenderChildren(writer);
      writer.RenderEndTag();
      writer.RenderEndTag();
      writer.RenderEndTag();
    }

    protected override void RenderChildren(HtmlTextWriter writer)
    {
      bool flag = true;
      foreach (Control control in this.Controls)
      {
        if (!flag)
          this.RenderItemSpacer(writer);
        else
          flag = false;
        if (!this.ContainedControlHeight.IsEmpty)
          writer.AddAttribute(HtmlTextWriterAttribute.Height, this.ContainedControlHeight.ToString());
        writer.RenderBeginTag(HtmlTextWriterTag.Td);
        control.RenderControl(writer);
        writer.RenderEndTag();
      }
    }

    protected void RenderItemSpacer(HtmlTextWriter writer)
    {
      if (this.m_viewer.ViewerStyle.ToolbarInterGroupSpacing != null)
        writer.AddAttribute(HtmlTextWriterAttribute.Class, this.m_viewer.ViewerStyle.ToolbarInterGroupSpacing);
      else
        writer.AddAttribute(HtmlTextWriterAttribute.Width, "4px");
      writer.RenderBeginTag(HtmlTextWriterTag.Td);
      writer.RenderEndTag();
    }

    public virtual string LeadingSpace => ToolbarGroup.NormalLeadingSpace;

    protected IEnumerable<RenderingExtension> GetClientSupportedRenderingExtensions()
    {
      return ReportViewer.FilterOutClientUnsupportedRenderingExtensions((IEnumerable<RenderingExtension>) this.GetRenderingExtensions());
    }

    private RenderingExtension[] GetRenderingExtensions()
    {
      try
      {
        if (!Global.IsDesignTime)
        {
          if (this.m_viewer.Report.IsReadyForConnection)
            return this.m_viewer.Report.ListRenderingExtensions();
        }
      }
      catch
      {
      }
      return new RenderingExtension[0];
    }
  }
}
