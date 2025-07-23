using System;
using System.Web.UI.HtmlControls;

namespace Microsoft.Reporting.WebForms;

internal sealed class HtmlInputImageWithClientID : HtmlInputImage
{
	protected override void OnPreRender(EventArgs e)
	{
		EnsureID();
		base.OnPreRender(e);
	}
}
