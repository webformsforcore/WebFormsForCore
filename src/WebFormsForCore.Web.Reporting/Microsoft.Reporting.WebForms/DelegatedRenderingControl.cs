using System.Web.UI;

namespace Microsoft.Reporting.WebForms;

internal class DelegatedRenderingControl : Control
{
	public delegate bool RenderDelegate(HtmlTextWriter writer);

	private RenderDelegate m_renderChildrenDelegate;

	public DelegatedRenderingControl(RenderDelegate renderChildrenDelegate)
	{
		m_renderChildrenDelegate = renderChildrenDelegate;
	}

	protected override void Render(HtmlTextWriter writer)
	{
		if (m_renderChildrenDelegate(writer))
		{
			base.Render(writer);
		}
	}
}
