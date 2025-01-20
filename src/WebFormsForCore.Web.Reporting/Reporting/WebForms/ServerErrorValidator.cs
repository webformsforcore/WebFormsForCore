
using System;
using System.Web.UI.WebControls;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal class ServerErrorValidator : CustomValidator
  {
    public ServerErrorValidator() => this.ClientValidationFunction = "MS_ValidateAlwaysTrue";

    protected override void OnPreRender(EventArgs e)
    {
      this.Page.ClientScript.RegisterClientScriptBlock(typeof (ServerErrorValidator), "ValidatorScript", "\r\nfunction MS_ValidateAlwaysTrue(source, args)\r\n{\r\n    args.IsValid = true;\r\n}\r\n", true);
      this.IsValid = false;
      base.OnPreRender(e);
    }

    protected override bool OnServerValidate(string value) => true;
  }
}
