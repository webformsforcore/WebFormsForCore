namespace Microsoft.Reporting.WebForms;

internal class MultiValueValidValuesInputControl : MultiValueInputControl
{
	protected MultiValidValuesSelector m_floatingEditor;

	public override string CheckBoxCssClass
	{
		set
		{
			EnsureChildControls();
			base.CheckBoxCssClass = value;
			m_floatingEditor.CheckBoxCssClass = value;
		}
	}

	public string DropDownCssClass
	{
		set
		{
			EnsureChildControls();
			m_floatingEditor.CssClass = value;
		}
	}

	public MultiValueValidValuesInputControl(ReportParameterInfo reportParam, IBrowserDetection browserDetection, bool useAbsoluteScreenPositioning)
		: base(reportParam, browserDetection, useAbsoluteScreenPositioning)
	{
	}

	protected override MultiValueSelector CreateFloatingEditor()
	{
		m_floatingEditor = new MultiValidValuesSelector(base.ReportParameter.ValidValues, this);
		m_floatingEditor.Change += base.OnValueChanged;
		return m_floatingEditor;
	}
}
