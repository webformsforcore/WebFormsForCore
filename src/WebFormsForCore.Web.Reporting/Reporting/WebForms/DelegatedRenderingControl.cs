
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
