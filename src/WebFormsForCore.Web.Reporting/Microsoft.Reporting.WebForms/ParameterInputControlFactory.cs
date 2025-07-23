namespace Microsoft.Reporting.WebForms;

internal static class ParameterInputControlFactory
{
	public static BaseParameterInputControl Create(ReportParameterInfo reportParam, bool allowQueryExecution, bool positioningMode)
	{
		bool flag = reportParam.ValidValues != null || reportParam.AreValidValuesQueryBased;
		bool disabled = (reportParam.State != ParameterState.HasValidValue && reportParam.State != ParameterState.MissingValidValue) || (!allowQueryExecution && reportParam.IsQueryParameter);
		BaseParameterInputControl baseParameterInputControl = null;
		IBrowserDetection current = BrowserDetection.Current;
		if (reportParam.MultiValue)
		{
			baseParameterInputControl = ((!flag) ? ((MultiValueInputControl)new MultiValueTextInputControl(reportParam, current, positioningMode)) : ((MultiValueInputControl)new MultiValueValidValuesInputControl(reportParam, current, positioningMode)));
		}
		else if (flag)
		{
			ValidValuesParameterInputControl validValuesParameterInputControl = new ValidValuesParameterInputControl(reportParam, current);
			validValuesParameterInputControl.SelectAValueString = LocalizationHelper.Current.SelectAValue;
			baseParameterInputControl = validValuesParameterInputControl;
		}
		else if (reportParam.DataType != ParameterDataType.Boolean)
		{
			baseParameterInputControl = ((reportParam.DataType != ParameterDataType.DateTime || !CalendarDropDownInputControl.IsSupported(current)) ? ((BaseParameterInputControl)new TextParameterInputControl(reportParam, current)) : ((BaseParameterInputControl)new CalendarDropDownInputControl(reportParam, current, positioningMode)));
		}
		else
		{
			BooleanParameterControl booleanParameterControl = new BooleanParameterControl(reportParam, current);
			booleanParameterControl.TrueValueText = LocalizationHelper.Current.TrueValueText;
			booleanParameterControl.FalseValueText = LocalizationHelper.Current.FalseValueText;
			baseParameterInputControl = booleanParameterControl;
		}
		baseParameterInputControl.NullCheckBoxString = LocalizationHelper.Current.NullCheckBoxText;
		baseParameterInputControl.NullValueText = LocalizationHelper.Current.NullValueText;
		baseParameterInputControl.Disabled = disabled;
		baseParameterInputControl.ShowDefaultValue = reportParam.State == ParameterState.HasValidValue || reportParam.State == ParameterState.MissingValidValue;
		return baseParameterInputControl;
	}
}
