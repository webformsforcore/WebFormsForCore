
using System;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal class BooleanParamValidator : CustomValidator
  {
    private RadioButton m_true;
    private RadioButton m_false;
    private string m_functionName;

    public BooleanParamValidator(RadioButton trueButton, RadioButton falseButton)
    {
      this.m_true = trueButton;
      this.m_false = falseButton;
    }

    private string FunctionName
    {
      get
      {
        if (string.IsNullOrEmpty(this.m_functionName))
          this.m_functionName = "MS_ValidateBooleanSelection_" + Guid.NewGuid().ToString("N");
        return this.m_functionName;
      }
    }

    protected override void OnPreRender(EventArgs e)
    {
      this.ClientValidationFunction = this.FunctionName;
      base.OnPreRender(e);
    }

    protected override bool OnServerValidate(string value) => true;

    protected override void Render(HtmlTextWriter writer)
    {
      base.Render(writer);
      string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\r\nfunction {0}(source, args)\r\n{{\r\n\tvar b1 = document.getElementById('{1}');\r\n\tvar b2 = document.getElementById('{2}');\r\n\r\n    if ((b1.disabled && b2.disabled) || (b1.checked || b2.checked))\r\n\t\targs.IsValid = true;\r\n\telse\r\n\t\targs.IsValid = false;\r\n}}\r\n", (object) this.FunctionName, (object) JavaScriptHelper.StringEscapeSingleQuote(this.m_true.ClientID), (object) JavaScriptHelper.StringEscapeSingleQuote(this.m_false.ClientID));
      writer.AddAttribute("language", "javascript");
      writer.RenderBeginTag(HtmlTextWriterTag.Script);
      writer.Write(str);
      writer.RenderEndTag();
    }
  }
}
