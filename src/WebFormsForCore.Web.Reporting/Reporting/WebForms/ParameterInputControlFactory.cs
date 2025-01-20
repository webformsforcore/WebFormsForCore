﻿
#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal static class ParameterInputControlFactory
  {
    public static BaseParameterInputControl Create(
      ReportParameterInfo reportParam,
      bool allowQueryExecution,
      bool positioningMode)
    {
      bool flag1 = reportParam.ValidValues != null || reportParam.AreValidValuesQueryBased;
      bool flag2 = reportParam.State != ParameterState.HasValidValue && reportParam.State != ParameterState.MissingValidValue || !allowQueryExecution && reportParam.IsQueryParameter;
      IBrowserDetection current = (IBrowserDetection) BrowserDetection.Current;
      BaseParameterInputControl parameterInputControl;
      if (reportParam.MultiValue)
        parameterInputControl = !flag1 ? (BaseParameterInputControl) new MultiValueTextInputControl(reportParam, current, positioningMode) : (BaseParameterInputControl) new MultiValueValidValuesInputControl(reportParam, current, positioningMode);
      else if (flag1)
        parameterInputControl = (BaseParameterInputControl) new ValidValuesParameterInputControl(reportParam, current)
        {
          SelectAValueString = LocalizationHelper.Current.SelectAValue
        };
      else if (reportParam.DataType == ParameterDataType.Boolean)
      {
        BooleanParameterControl parameterControl = new BooleanParameterControl(reportParam, current);
        parameterControl.TrueValueText = LocalizationHelper.Current.TrueValueText;
        parameterControl.FalseValueText = LocalizationHelper.Current.FalseValueText;
        parameterInputControl = (BaseParameterInputControl) parameterControl;
      }
      else
        parameterInputControl = reportParam.DataType != ParameterDataType.DateTime || !CalendarDropDownInputControl.IsSupported(current) ? (BaseParameterInputControl) new TextParameterInputControl(reportParam, current) : (BaseParameterInputControl) new CalendarDropDownInputControl(reportParam, current, positioningMode);
      parameterInputControl.NullCheckBoxString = LocalizationHelper.Current.NullCheckBoxText;
      parameterInputControl.NullValueText = LocalizationHelper.Current.NullValueText;
      parameterInputControl.Disabled = flag2;
      parameterInputControl.ShowDefaultValue = reportParam.State == ParameterState.HasValidValue || reportParam.State == ParameterState.MissingValidValue;
      return parameterInputControl;
    }
  }
}
