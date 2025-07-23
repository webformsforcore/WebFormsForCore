using System;

namespace Microsoft.Reporting.WebForms;

internal sealed class BooleanParameterControl : BooleanParameterInputControl
{
	protected override string[] CustomControlValue
	{
		set
		{
			try
			{
				base.CustomControlValue = value;
			}
			catch (ArgumentException)
			{
				throw new ArgumentException(Errors.ParamValueTypeMismatch(base.ReportParameter.Name));
			}
		}
	}

	public BooleanParameterControl(ReportParameterInfo reportParam, IBrowserDetection browserDetection)
		: base(reportParam, browserDetection)
	{
	}
}
