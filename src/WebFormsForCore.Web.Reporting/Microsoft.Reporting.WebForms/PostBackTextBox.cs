using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Microsoft.Reporting.WebForms;

internal sealed class PostBackTextBox : TextBox, IPostBackEventHandler
{
	public void RaisePostBackEvent(string eventArgument)
	{
		RaiseBubbleEvent(this, new AutoPostBackEventArgs());
	}

	protected override void OnLoad(EventArgs e)
	{
		EnsureID();
		base.OnLoad(e);
	}
}
