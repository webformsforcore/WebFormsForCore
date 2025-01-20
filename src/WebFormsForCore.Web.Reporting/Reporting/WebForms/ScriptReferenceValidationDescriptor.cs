
using System;
using System.Globalization;
using System.Web.UI;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal sealed class ScriptReferenceValidationDescriptor : ScriptDescriptor
  {
    private string m_errorMessageDivID;

    public ScriptReferenceValidationDescriptor(string errorMessageDivID)
    {
      this.m_errorMessageDivID = errorMessageDivID;
    }

    protected override string GetScript()
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\r\nif (typeof Microsoft == 'undefined' ||\r\n    typeof Microsoft.Reporting == 'undefined' ||\r\n    typeof Microsoft.Reporting.WebFormsClient == 'undefined' ||\r\n    typeof Microsoft.Reporting.WebFormsClient.ReportViewer == 'undefined')\r\n    Sys.UI.DomElement.setVisible($get('{0}'), true);", (object) JavaScriptHelper.StringEscapeSingleQuote(this.m_errorMessageDivID));
    }
  }
}
