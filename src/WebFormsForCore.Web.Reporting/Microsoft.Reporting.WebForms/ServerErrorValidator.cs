using System;
using System.Web.UI.WebControls;

namespace Microsoft.Reporting.WebForms;

internal class ServerErrorValidator : CustomValidator
{
	public ServerErrorValidator()
	{
		base.ClientValidationFunction = "MS_ValidateAlwaysTrue";
	}

	protected override void OnPreRender(EventArgs e)
	{
		string script = "\r\nfunction MS_ValidateAlwaysTrue(source, args)\r\n{\r\n    args.IsValid = true;\r\n}\r\n";
		Page.ClientScript.RegisterClientScriptBlock(typeof(ServerErrorValidator), "ValidatorScript", script, addScriptTags: true);
		base.IsValid = false;
		base.OnPreRender(e);
	}

	protected override bool OnServerValidate(string value)
	{
		return true;
	}
}
