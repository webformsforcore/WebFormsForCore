// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.ScriptReferenceValidationDescriptor
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

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
