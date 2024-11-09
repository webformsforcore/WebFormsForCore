// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.DelegatedRenderingControl
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

using System.Web.UI;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal class DelegatedRenderingControl : Control
  {
    private DelegatedRenderingControl.RenderDelegate m_renderChildrenDelegate;

    public DelegatedRenderingControl(
      DelegatedRenderingControl.RenderDelegate renderChildrenDelegate)
    {
      this.m_renderChildrenDelegate = renderChildrenDelegate;
    }

    protected override void Render(HtmlTextWriter writer)
    {
      if (!this.m_renderChildrenDelegate(writer))
        return;
      base.Render(writer);
    }

    public delegate bool RenderDelegate(HtmlTextWriter writer);
  }
}
