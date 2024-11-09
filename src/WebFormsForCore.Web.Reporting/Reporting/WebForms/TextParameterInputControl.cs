// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.TextParameterInputControl
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal class TextParameterInputControl : BaseParameterInputControl
  {
    protected PostBackTextBox m_textBox;

    public TextParameterInputControl(
      ReportParameterInfo reportParam,
      IBrowserDetection browserDetection)
      : base(reportParam, browserDetection)
    {
    }

    protected override void CreateChildControls()
    {
      this.Controls.Clear();
      base.CreateChildControls();
      this.m_textBox = new PostBackTextBox();
      this.m_textBox.ID = "txtValue";
      this.m_textBox.Columns = 30;
      this.m_textBox.TextChanged += new EventHandler(((BaseParameterInputControl) this).OnCustomControlChanged);
      this.Controls.Add((Control) this.m_textBox);
      if (!this.ReportParameter.Nullable)
        return;
      this.CreateNullCheckBox();
    }

    protected override BaseValidator CreateParameterRequiresValueValidator()
    {
      return this.ReportParameter.DataType != ParameterDataType.String || !this.ReportParameter.AllowBlank ? (BaseValidator) new RequiredFieldValidator() : (BaseValidator) null;
    }

    protected override void RenderContents(HtmlTextWriter writer)
    {
      this.EnsureChildControls();
      this.ApplyStylesToTextBox((TextBox) this.m_textBox);
      this.m_textBox.RenderControl(writer);
      if (this.m_nullCheckBox != null)
      {
        this.WriteNullCheckBoxSpacer(writer);
        this.RenderNullCheckBox(writer);
      }
      if (!this.Validators.HasValidatorsToRender)
        return;
      writer.Write("<br>");
      this.Validators.RenderControl(writer);
    }

    protected override string[] CustomControlValue
    {
      get
      {
        this.EnsureChildControls();
        return new string[1]{ this.m_textBox.Text };
      }
      set
      {
        this.EnsureChildControls();
        if (value == null || value.Length != 1)
          this.m_textBox.Text = "";
        else
          this.m_textBox.Text = value[0];
      }
    }

    protected override bool CustomControlHasValue
    {
      get
      {
        this.EnsureChildControls();
        return this.ReportParameter.AllowBlank || this.m_textBox.Text.Length != 0;
      }
    }

    protected override void OnPreRender(EventArgs e)
    {
      this.EnsureChildControls();
      if (this.ParameterRequiresValueValidator != null)
        this.ParameterRequiresValueValidator.ControlToValidate = this.m_textBox.ID;
      base.OnPreRender(e);
    }

    public override bool AutoPostBack
    {
      set
      {
        this.EnsureChildControls();
        this.m_textBox.AutoPostBack = value;
        base.AutoPostBack = value;
      }
    }

    protected override string[] CustomControlIds
    {
      get
      {
        this.EnsureChildControls();
        return new string[1]{ this.m_textBox.ClientID };
      }
    }

    public override string PrimaryFormElementId
    {
      get
      {
        this.EnsureChildControls();
        return this.m_textBox.ClientID;
      }
    }

    protected override void SetCustomControlEnableState(bool enabled)
    {
      this.m_textBox.Enabled = enabled;
    }

    public override IEnumerable<ScriptDescriptor> GetScriptDescriptors()
    {
      this.EnsureChildControls();
      ScriptControlDescriptor desc = new ScriptControlDescriptor("Microsoft.Reporting.WebFormsClient._TextParameterInputControl", this.ClientID);
      this.AddBaseDescriptorProperties(desc);
      desc.AddProperty("TextBoxId", (object) this.m_textBox.ClientID);
      desc.AddProperty("AllowBlank", (object) this.ReportParameter.AllowBlank);
      return (IEnumerable<ScriptDescriptor>) new ScriptDescriptor[1]
      {
        (ScriptDescriptor) desc
      };
    }

    public override string AltText
    {
      set
      {
        this.EnsureChildControls();
        this.m_textBox.ToolTip = value;
      }
    }
  }
}
