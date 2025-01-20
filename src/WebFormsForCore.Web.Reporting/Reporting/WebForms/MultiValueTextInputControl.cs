
using System;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal class MultiValueTextInputControl : MultiValueInputControl
  {
    protected MultiValueTextSelector m_floatingEditor;

    public MultiValueTextInputControl(
      ReportParameterInfo reportParam,
      IBrowserDetection browserDetection,
      bool useAbsoluteScreenPositioning)
      : base(reportParam, browserDetection, useAbsoluteScreenPositioning)
    {
    }

    public override string TextBoxCssClass
    {
      set
      {
        this.EnsureChildControls();
        base.TextBoxCssClass = value;
        this.m_floatingEditor.TextBoxCssClass = value;
      }
    }

    public override string AltText
    {
      set
      {
        this.EnsureChildControls();
        this.m_floatingEditor.AltText = value;
        base.AltText = value;
      }
    }

    protected override MultiValueSelector CreateFloatingEditor()
    {
      this.m_floatingEditor = new MultiValueTextSelector(this.BrowserDetection, this.ReportParameter.AllowBlank && this.ReportParameter.DataType == ParameterDataType.String);
      this.m_floatingEditor.Change += new EventHandler(((BaseParameterInputControl) this).OnValueChanged);
      return (MultiValueSelector) this.m_floatingEditor;
    }
  }
}
