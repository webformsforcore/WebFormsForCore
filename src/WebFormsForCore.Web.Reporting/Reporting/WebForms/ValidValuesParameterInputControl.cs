// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.ValidValuesParameterInputControl
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
  internal class ValidValuesParameterInputControl : BaseParameterInputControl
  {
    public string SelectAValueString = ParameterInputControlStrings.SelectValidValue;
    protected SpaceAwareDropDownList m_dropDown;
    private string m_emptyDropDownCssClass;

    public ValidValuesParameterInputControl(
      ReportParameterInfo reportParam,
      IBrowserDetection browserDetection)
      : base(reportParam, browserDetection)
    {
    }

    protected override void CreateChildControls()
    {
      this.Controls.Clear();
      base.CreateChildControls();
      this.m_dropDown = new SpaceAwareDropDownList();
      this.m_dropDown.ID = "ddValue";
      this.m_dropDown.SelectedIndexChanged += new EventHandler(((BaseParameterInputControl) this).OnCustomControlChanged);
      if (this.ReportParameter.ValidValues != null)
      {
        int num = 1;
        foreach (ValidValue validValue in (IEnumerable<ValidValue>) this.ReportParameter.ValidValues)
        {
          this.m_dropDown.Items.Add(new ListItem(this.GetLabelForValidValue(validValue), num.ToString((IFormatProvider) CultureInfo.InvariantCulture)));
          ++num;
        }
      }
      if (this.m_dropDown.Items.Count > 0)
      {
        this.m_dropDown.Items.Insert(0, new ListItem(this.SelectAValueString, "0"));
        this.m_dropDown.SelectedIndex = 0;
      }
      this.Controls.Add((Control) this.m_dropDown);
    }

    protected override BaseValidator CreateParameterRequiresValueValidator()
    {
      return (BaseValidator) new DropDownValidator();
    }

    protected override string[] CustomControlValue
    {
      get
      {
        this.EnsureChildControls();
        return new string[1]
        {
          this.ReportParameter.ValidValues[this.m_dropDown.SelectedIndex - 1].Value
        };
      }
      set
      {
        this.EnsureChildControls();
        if (value == null || value.Length != 1)
        {
          this.m_dropDown.SelectedIndex = 0;
        }
        else
        {
          for (int index = 0; index < this.ReportParameter.ValidValues.Count; ++index)
          {
            if (string.Compare(this.ReportParameter.ValidValues[index].Value, value[0], StringComparison.Ordinal) == 0)
            {
              this.m_dropDown.SelectedIndex = index + 1;
              break;
            }
          }
        }
      }
    }

    protected override void OnPreRender(EventArgs e)
    {
      this.EnsureChildControls();
      if (this.ParameterRequiresValueValidator != null)
        this.ParameterRequiresValueValidator.ControlToValidate = this.m_dropDown.ID;
      base.OnPreRender(e);
    }

    protected override bool CustomControlHasValue
    {
      get
      {
        this.EnsureChildControls();
        return this.m_dropDown.SelectedIndex > 0;
      }
    }

    protected override string[] CustomControlIds
    {
      get
      {
        this.EnsureChildControls();
        return new string[1]{ this.m_dropDown.ClientID };
      }
    }

    public override string PrimaryFormElementId
    {
      get
      {
        this.EnsureChildControls();
        return this.m_dropDown.ClientID;
      }
    }

    public override bool AutoPostBack
    {
      set
      {
        this.EnsureChildControls();
        this.m_dropDown.AutoPostBack = value;
        base.AutoPostBack = value;
      }
    }

    public string EmptyDropDownCssClass
    {
      get => this.m_emptyDropDownCssClass;
      set => this.m_emptyDropDownCssClass = value;
    }

    public override string AltText
    {
      set
      {
        this.EnsureChildControls();
        this.m_dropDown.ToolTip = value;
      }
    }

    protected override void RenderContents(HtmlTextWriter writer)
    {
      this.m_dropDown.Font.CopyFrom(this.Font);
      if (this.m_dropDown.Items.Count == 0)
      {
        if (!string.IsNullOrEmpty(this.m_emptyDropDownCssClass))
          this.m_dropDown.CssClass = this.EmptyDropDownCssClass;
        else
          this.m_dropDown.Style.Add(HtmlTextWriterStyle.Width, "15ex");
      }
      this.m_dropDown.RenderControl(writer);
      if (!this.Validators.HasValidatorsToRender)
        return;
      writer.Write("<br>");
      this.Validators.RenderControl(writer);
    }

    protected override void SetCustomControlEnableState(bool enabled)
    {
      this.m_dropDown.Enabled = enabled;
    }

    public override IEnumerable<ScriptDescriptor> GetScriptDescriptors()
    {
      this.EnsureChildControls();
      ScriptControlDescriptor desc = new ScriptControlDescriptor("Microsoft.Reporting.WebFormsClient._ValidValueParameterInputControl", this.ClientID);
      this.AddBaseDescriptorProperties(desc);
      desc.AddProperty("DropDownId", (object) this.m_dropDown.ClientID);
      BaseValidator requiresValueValidator = this.ParameterRequiresValueValidator;
      if (requiresValueValidator != null)
        desc.AddProperty("DropDownValidatorId", (object) requiresValueValidator.ClientID);
      return (IEnumerable<ScriptDescriptor>) new ScriptDescriptor[1]
      {
        (ScriptDescriptor) desc
      };
    }
  }
}
