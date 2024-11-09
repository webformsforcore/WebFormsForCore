// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.MultiValueInputControl
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal abstract class MultiValueInputControl : GenericDropDownInputControl
  {
    private MultiValueSelector m_floatingEditor;

    protected MultiValueInputControl(
      ReportParameterInfo reportParam,
      IBrowserDetection browserDetection,
      bool useAbsoluteScreenPositioning)
      : base(reportParam, browserDetection, useAbsoluteScreenPositioning)
    {
    }

    protected override void CreateChildControls()
    {
      this.Controls.Clear();
      base.CreateChildControls();
      this.InputControl.EnableViewState = false;
      this.InputControl.ReadOnly = true;
      this.Image.Src = EmbeddedResourceOperation.CreateUrl("Microsoft.Reporting.WebForms.Icons.MultiValueSelect.gif");
      this.Image.Style.Add(HtmlTextWriterStyle.VerticalAlign, "top");
      if (this.BrowserDetection.IsIE)
        this.Image.Style.Add(HtmlTextWriterStyle.MarginTop, "1px");
      this.m_floatingEditor = this.CreateFloatingEditor();
      this.AddFloatingAttributes((WebControl) this.m_floatingEditor);
      this.m_floatingEditor.ID = "divDropDown";
      this.m_floatingEditor.Style.Add(HtmlTextWriterStyle.ZIndex, 11.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      this.Controls.Add((Control) this.m_floatingEditor);
      this.m_absolutePositionedControls.Add((Control) this.m_floatingEditor);
    }

    protected override void OnPreRender(EventArgs e)
    {
      this.EnsureChildControls();
      base.OnPreRender(e);
      this.m_floatingEditor.ClientSideObjectName = this.ClientObject;
    }

    protected override void Render(HtmlTextWriter writer)
    {
      this.EnsureChildControls();
      this.m_floatingEditor.Font.CopyFrom(this.Font);
      base.Render(writer);
    }

    protected override bool CustomControlHasValue
    {
      get
      {
        this.EnsureChildControls();
        return this.m_floatingEditor.HasValue;
      }
    }

    protected override string[] CustomControlValue
    {
      get
      {
        this.EnsureChildControls();
        return this.m_floatingEditor.Value;
      }
      set
      {
        this.EnsureChildControls();
        this.m_floatingEditor.Value = value;
      }
    }

    public override IEnumerable<ScriptDescriptor> GetScriptDescriptors()
    {
      this.EnsureChildControls();
      ScriptControlDescriptor desc = new ScriptControlDescriptor("Microsoft.Reporting.WebFormsClient._MultiValueParameterInputControl", this.ClientID);
      this.AddDropDownDescriptorProperties(desc);
      this.m_floatingEditor.AddScriptDescriptors(desc);
      desc.AddProperty("HasValidValueList", (object) (this is MultiValueValidValuesInputControl));
      desc.AddProperty("AllowBlank", (object) (bool) (!this.ReportParameter.AllowBlank ? 0 : (this.ReportParameter.DataType == ParameterDataType.String ? 1 : 0)));
      desc.AddProperty("FloatingEditorId", (object) this.m_floatingEditor.ClientID);
      desc.AddProperty("ListSeparator", (object) (CultureInfo.CurrentCulture.TextInfo.ListSeparator + " "));
      desc.AddProperty("GripImage", (object) EmbeddedResourceOperation.CreateUrl("Microsoft.Reporting.WebForms.Icons.HandleGrip.gif"));
      desc.AddProperty("GripImageRTL", (object) EmbeddedResourceOperation.CreateUrl("Microsoft.Reporting.WebForms.Icons.HandleGripRTL.gif"));
      return (IEnumerable<ScriptDescriptor>) new ScriptDescriptor[1]
      {
        (ScriptDescriptor) desc
      };
    }

    protected override string FrameAccessibleName
    {
      get => Strings.PlaceHolderFrameAccessibleName(this.ReportParameter.Prompt);
    }

    protected abstract MultiValueSelector CreateFloatingEditor();
  }
}
