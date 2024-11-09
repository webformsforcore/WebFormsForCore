// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.MultiValueValidValuesInputControl
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

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
