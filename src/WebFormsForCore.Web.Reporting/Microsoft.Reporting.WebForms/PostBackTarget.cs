using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Microsoft.Reporting.WebForms;

internal class PostBackTarget : WebControl, IPostBackEventHandler
{
	public event EventHandler PostBackAsTarget;

	public void RaisePostBackEvent(string eventArgument)
	{
		if (this.PostBackAsTarget != null)
		{
			this.PostBackAsTarget(this, EventArgs.Empty);
		}
	}
}
