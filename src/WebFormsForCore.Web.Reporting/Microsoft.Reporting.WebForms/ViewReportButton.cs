using System;
using System.Web.UI.WebControls;

namespace Microsoft.Reporting.WebForms;

internal sealed class ViewReportButton : Button
{
	protected override void OnPreRender(EventArgs e)
	{
		EnsureID();
		base.OnPreRender(e);
	}
}
