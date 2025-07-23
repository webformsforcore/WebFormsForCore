using System;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Microsoft.Reporting.WebForms;

internal sealed class EventableTextBox : TextBox, IPostBackEventHandler
{
	private const string CurrentPagePostBackScript = "if (event.keyCode == 10 || event.keyCode == 13) {{{0}; return false;}}";

	public string CustomOnEnterScript;

	public bool AddKeyPressHandler = true;

	public event EventHandler EnterPressed;

	protected override void OnPreRender(EventArgs e)
	{
		base.OnPreRender(e);
		if (base.DesignMode)
		{
			return;
		}
		EnsureID();
		if (AddKeyPressHandler)
		{
			string text = CustomOnEnterScript;
			if (text == null)
			{
				text = Page.ClientScript.GetPostBackEventReference(this, null);
			}
			string value = string.Format(CultureInfo.InvariantCulture, "if (event.keyCode == 10 || event.keyCode == 13) {{{0}; return false;}}", text);
			base.Attributes.Add("onkeypress", value);
		}
	}

	public void RaisePostBackEvent(string eventArgument)
	{
		if (this.EnterPressed != null)
		{
			this.EnterPressed(this, null);
		}
	}
}
