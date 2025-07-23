namespace Microsoft.Reporting.WebForms;

internal class MultiValueTextInputControl : MultiValueInputControl
{
	protected MultiValueTextSelector m_floatingEditor;

	public override string TextBoxCssClass
	{
		set
		{
			EnsureChildControls();
			base.TextBoxCssClass = value;
			m_floatingEditor.TextBoxCssClass = value;
		}
	}

	public override string AltText
	{
		set
		{
			EnsureChildControls();
			m_floatingEditor.AltText = value;
			base.AltText = value;
		}
	}

	public MultiValueTextInputControl(ReportParameterInfo reportParam, IBrowserDetection browserDetection, bool useAbsoluteScreenPositioning)
		: base(reportParam, browserDetection, useAbsoluteScreenPositioning)
	{
	}

	protected override MultiValueSelector CreateFloatingEditor()
	{
		bool allowBlank = base.ReportParameter.AllowBlank && base.ReportParameter.DataType == ParameterDataType.String;
		m_floatingEditor = new MultiValueTextSelector(base.BrowserDetection, allowBlank);
		m_floatingEditor.Change += base.OnValueChanged;
		return m_floatingEditor;
	}
}
