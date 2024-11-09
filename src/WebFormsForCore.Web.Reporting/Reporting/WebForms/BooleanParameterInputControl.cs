// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.BooleanParameterInputControl
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal class BooleanParameterInputControl : BaseParameterInputControl
  {
    public string TrueValueText = ParameterInputControlStrings.True;
    public string FalseValueText = ParameterInputControlStrings.False;
    protected PostBackRadioButton m_trueButton;
    protected PostBackRadioButton m_falseButton;
    private string m_altText;

    public BooleanParameterInputControl(
      ReportParameterInfo reportParam,
      IBrowserDetection browserDetection)
      : base(reportParam, browserDetection)
    {
    }

    protected override string[] CustomControlIds
    {
      get
      {
        this.EnsureChildControls();
        return new string[2]
        {
          this.m_trueButton.ClientID,
          this.m_falseButton.ClientID
        };
      }
    }

    public override string PrimaryFormElementId
    {
      get
      {
        this.EnsureChildControls();
        return this.m_trueButton.ClientID;
      }
    }

    public override string AltText
    {
      set => this.m_altText = value;
    }

    protected override void CreateChildControls()
    {
      this.Controls.Clear();
      base.CreateChildControls();
      this.m_trueButton = this.CreateRadioButton(this.TrueValueText, "rbTrue");
      this.Controls.Add((Control) this.m_trueButton);
      this.m_falseButton = this.CreateRadioButton(this.FalseValueText, "rbFalse");
      this.Controls.Add((Control) this.m_falseButton);
      this.m_trueButton.Attributes.Add("forceSpan", "true");
      this.m_falseButton.Attributes.Add("forceSpan", "true");
      if (!this.ReportParameter.Nullable)
        return;
      this.CreateNullCheckBox();
    }

    protected override BaseValidator CreateParameterRequiresValueValidator()
    {
      this.EnsureChildControls();
      return (BaseValidator) new BooleanParamValidator((RadioButton) this.m_trueButton, (RadioButton) this.m_falseButton);
    }

    protected override void OnPreRender(EventArgs e)
    {
      base.OnPreRender(e);
      this.m_trueButton.GroupName = HttpUtility.HtmlAttributeEncode(this.ClientID);
      this.m_falseButton.GroupName = this.m_trueButton.GroupName;
    }

    protected override void RenderContents(HtmlTextWriter writer)
    {
      this.EnsureChildControls();
      writer.AddAttribute(HtmlTextWriterAttribute.Cellpadding, "0");
      writer.AddAttribute(HtmlTextWriterAttribute.Cellspacing, "0");
      if (!string.IsNullOrEmpty(this.m_altText))
        writer.AddAttribute(HtmlTextWriterAttribute.Title, this.m_altText);
      writer.RenderBeginTag(HtmlTextWriterTag.Table);
      writer.RenderBeginTag(HtmlTextWriterTag.Tr);
      writer.RenderBeginTag(HtmlTextWriterTag.Td);
      this.m_trueButton.Font.CopyFrom(this.Font);
      this.m_trueButton.CssClass = this.CheckBoxCssClass;
      this.m_trueButton.RenderControl(writer);
      writer.RenderEndTag();
      writer.RenderBeginTag(HtmlTextWriterTag.Td);
      this.m_falseButton.Font.CopyFrom(this.Font);
      this.m_falseButton.CssClass = this.CheckBoxCssClass;
      this.m_falseButton.RenderControl(writer);
      writer.RenderEndTag();
      if (this.m_nullCheckBox != null)
      {
        writer.RenderBeginTag(HtmlTextWriterTag.Td);
        writer.Write("&nbsp;");
        this.RenderNullCheckBox(writer);
        writer.RenderEndTag();
      }
      writer.RenderEndTag();
      writer.RenderEndTag();
      if (!this.Validators.HasValidatorsToRender)
        return;
      this.Validators.RenderControl(writer);
    }

    private PostBackRadioButton CreateRadioButton(string unencodedText, string id)
    {
      PostBackRadioButton radioButton = new PostBackRadioButton();
      radioButton.CheckedChanged += new EventHandler(((BaseParameterInputControl) this).OnCustomControlChanged);
      radioButton.Attributes.Add("forceSpan", "true");
      radioButton.Text = HttpUtility.HtmlEncode(unencodedText);
      radioButton.ID = id;
      return radioButton;
    }

    protected override string[] CustomControlValue
    {
      get
      {
        this.EnsureChildControls();
        return new string[1]
        {
          this.m_trueButton.Checked.ToString()
        };
      }
      set
      {
        this.EnsureChildControls();
        if (value == null || value.Length != 1)
        {
          this.m_trueButton.Checked = false;
          this.m_falseButton.Checked = false;
        }
        else if (value[0] == null)
        {
          this.m_trueButton.Checked = false;
          this.m_falseButton.Checked = false;
        }
        else
        {
          bool result;
          if (!bool.TryParse(value[0], out result))
            throw new ArgumentOutOfRangeException(nameof (value));
          this.m_trueButton.Checked = result;
          this.m_falseButton.Checked = !result;
        }
      }
    }

    protected override bool CustomControlHasValue
    {
      get
      {
        this.EnsureChildControls();
        return this.m_trueButton.Checked || this.m_falseButton.Checked;
      }
    }

    public override bool AutoPostBack
    {
      set
      {
        this.EnsureChildControls();
        this.m_trueButton.AutoPostBack = value;
        this.m_falseButton.AutoPostBack = value;
        base.AutoPostBack = value;
      }
    }

    protected override void SetCustomControlEnableState(bool enabled)
    {
      this.m_falseButton.Enabled = enabled;
      this.m_trueButton.Enabled = enabled;
    }

    public override IEnumerable<ScriptDescriptor> GetScriptDescriptors()
    {
      this.EnsureChildControls();
      ScriptControlDescriptor desc = new ScriptControlDescriptor("Microsoft.Reporting.WebFormsClient._BoolParameterInputControl", this.ClientID);
      this.AddBaseDescriptorProperties(desc);
      desc.AddProperty("TrueCheckId", (object) this.m_trueButton.ClientID);
      desc.AddProperty("FalseCheckId", (object) this.m_falseButton.ClientID);
      desc.AddProperty("TrueValueText", (object) this.TrueValueText);
      desc.AddProperty("FalseValueText", (object) this.FalseValueText);
      return (IEnumerable<ScriptDescriptor>) new ScriptDescriptor[1]
      {
        (ScriptDescriptor) desc
      };
    }
  }
}
