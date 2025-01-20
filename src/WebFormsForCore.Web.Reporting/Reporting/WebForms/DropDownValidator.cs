
using System.Web.UI;
using System.Web.UI.WebControls;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal sealed class DropDownValidator : CustomValidator
  {
    public DropDownValidator() => this.ClientValidationFunction = "MS_ValidateDropDownSelection";

    protected override void Render(HtmlTextWriter writer)
    {
      base.Render(writer);
      string str = "\r\nfunction MS_ValidateDropDownSelection(source, args)\r\n{\r\n\tvar obj = document.getElementById(source.controltovalidate);\r\n\r\n\tif (obj != null && obj.options[0].selected && !obj.disabled)\r\n\t\targs.IsValid = false;\r\n\telse\r\n\t\targs.IsValid = true;\r\n}\r\n";
      writer.AddAttribute("language", "javascript");
      writer.RenderBeginTag(HtmlTextWriterTag.Script);
      writer.Write(str);
      writer.RenderEndTag();
    }

    protected override bool OnServerValidate(string value) => true;
  }
}
