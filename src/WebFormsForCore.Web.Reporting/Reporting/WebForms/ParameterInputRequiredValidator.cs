// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.ParameterInputRequiredValidator
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

using System;
using System.Web.UI;
using System.Web.UI.WebControls;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal class ParameterInputRequiredValidator : RequiredFieldValidator
  {
    protected override bool EvaluateIsValid()
    {
      Control control = this.NamingContainer.FindControl(this.ControlToValidate);
      BaseParameterInputControl parent = control == null ? (BaseParameterInputControl) null : control.Parent as BaseParameterInputControl;
      if (parent == null)
        throw new Exception("ParameterInputRequiredValidator must validate a control who has a parent of BaseParaemterInputControl");
      return parent.CurrentValue != null && parent.CurrentValue.Length != 0;
    }
  }
}
