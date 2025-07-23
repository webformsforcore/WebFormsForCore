using System;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Microsoft.Reporting.WebForms;

internal class BooleanParamValidator : CustomValidator
{
	private RadioButton m_true;

	private RadioButton m_false;

	private string m_functionName;

	private string FunctionName
	{
		get
		{
			if (string.IsNullOrEmpty(m_functionName))
			{
				m_functionName = "MS_ValidateBooleanSelection_" + Guid.NewGuid().ToString("N");
			}
			return m_functionName;
		}
	}

	public BooleanParamValidator(RadioButton trueButton, RadioButton falseButton)
	{
		m_true = trueButton;
		m_false = falseButton;
	}

	protected override void OnPreRender(EventArgs e)
	{
		base.ClientValidationFunction = FunctionName;
		base.OnPreRender(e);
	}

	protected override bool OnServerValidate(string value)
	{
		return true;
	}

	protected override void Render(HtmlTextWriter writer)
	{
		base.Render(writer);
		string format = "\r\nfunction {0}(source, args)\r\n{{\r\n\tvar b1 = document.getElementById('{1}');\r\n\tvar b2 = document.getElementById('{2}');\r\n\r\n    if ((b1.disabled && b2.disabled) || (b1.checked || b2.checked))\r\n\t\targs.IsValid = true;\r\n\telse\r\n\t\targs.IsValid = false;\r\n}}\r\n";
		string value = string.Format(CultureInfo.InvariantCulture, format, FunctionName, JavaScriptHelper.StringEscapeSingleQuote(m_true.ClientID), JavaScriptHelper.StringEscapeSingleQuote(m_false.ClientID));
		writer.AddAttribute("language", "javascript");
		writer.RenderBeginTag(HtmlTextWriterTag.Script);
		writer.Write(value);
		writer.RenderEndTag();
	}
}
