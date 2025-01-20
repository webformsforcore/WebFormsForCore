
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
