using System.Globalization;
using System.Web.UI;

namespace Microsoft.Reporting.WebForms;

internal sealed class ScriptReferenceValidationDescriptor : ScriptDescriptor
{
	private string m_errorMessageDivID;

	public ScriptReferenceValidationDescriptor(string errorMessageDivID)
	{
		m_errorMessageDivID = errorMessageDivID;
	}

	protected override string GetScript()
	{
		return string.Format(CultureInfo.InvariantCulture, "\r\nif (typeof Microsoft == 'undefined' ||\r\n    typeof Microsoft.Reporting == 'undefined' ||\r\n    typeof Microsoft.Reporting.WebFormsClient == 'undefined' ||\r\n    typeof Microsoft.Reporting.WebFormsClient.ReportViewer == 'undefined')\r\n    Sys.UI.DomElement.setVisible($get('{0}'), true);", JavaScriptHelper.StringEscapeSingleQuote(m_errorMessageDivID));
	}
}
