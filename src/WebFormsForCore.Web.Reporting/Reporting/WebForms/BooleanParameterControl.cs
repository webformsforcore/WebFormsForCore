
using System;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal sealed class BooleanParameterControl : BooleanParameterInputControl
  {
    public BooleanParameterControl(
      ReportParameterInfo reportParam,
      IBrowserDetection browserDetection)
      : base(reportParam, browserDetection)
    {
    }

    protected override string[] CustomControlValue
    {
      set
      {
        try
        {
          base.CustomControlValue = value;
        }
        catch (ArgumentException ex)
        {
          throw new ArgumentException(Errors.ParamValueTypeMismatch(this.ReportParameter.Name));
        }
      }
    }
  }
}
