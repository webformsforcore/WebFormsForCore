
using System;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal class MultiValueValidValuesInputControl : MultiValueInputControl
  {
    protected MultiValidValuesSelector m_floatingEditor;

    public MultiValueValidValuesInputControl(
      ReportParameterInfo reportParam,
      IBrowserDetection browserDetection,
      bool useAbsoluteScreenPositioning)
      : base(reportParam, browserDetection, useAbsoluteScreenPositioning)
    {
    }

    public override string CheckBoxCssClass
    {
      set
      {
        this.EnsureChildControls();
        base.CheckBoxCssClass = value;
        this.m_floatingEditor.CheckBoxCssClass = value;
      }
    }

    public string DropDownCssClass
    {
      set
      {
        this.EnsureChildControls();
        this.m_floatingEditor.CssClass = value;
      }
    }

    protected override MultiValueSelector CreateFloatingEditor()
    {
      this.m_floatingEditor = new MultiValidValuesSelector(this.ReportParameter.ValidValues, (BaseParameterInputControl) this);
      this.m_floatingEditor.Change += new EventHandler(((BaseParameterInputControl) this).OnValueChanged);
      return (MultiValueSelector) this.m_floatingEditor;
    }
  }
}
