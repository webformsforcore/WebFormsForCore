using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Microsoft.Reporting.WebForms;

internal class ParameterInputRequiredValidator : RequiredFieldValidator
{
	protected override bool EvaluateIsValid()
	{
		Control control = NamingContainer.FindControl(base.ControlToValidate);
		BaseParameterInputControl baseParameterInputControl = ((control == null) ? null : (control.Parent as BaseParameterInputControl));
		if (baseParameterInputControl == null)
		{
			throw new Exception("ParameterInputRequiredValidator must validate a control who has a parent of BaseParaemterInputControl");
		}
		if (baseParameterInputControl.CurrentValue != null)
		{
			return baseParameterInputControl.CurrentValue.Length != 0;
		}
		return false;
	}
}
