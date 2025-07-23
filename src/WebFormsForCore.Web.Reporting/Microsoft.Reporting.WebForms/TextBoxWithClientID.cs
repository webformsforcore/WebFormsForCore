using System;
using System.Web.UI.WebControls;

namespace Microsoft.Reporting.WebForms;

internal sealed class TextBoxWithClientID : TextBox
{
	protected override void OnLoad(EventArgs e)
	{
		EnsureID();
		base.OnLoad(e);
	}
}
