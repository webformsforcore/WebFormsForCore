
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal abstract class BaseParameterInputControl : CompositeControl, IScriptControl
  {
    internal const int _TextBoxColumns = 30;
    public string NullCheckBoxString = ParameterInputControlStrings.NullCheckBox;
    protected PostBackCheckBox m_nullCheckBox;
    public string NullValueText = ParameterInputControlStrings.NullValue;
    private IBrowserDetection m_browserDetection;
    private ReportParameterInfo m_reportParam;
    private bool m_showDefaultValue = true;
    private bool m_autoPostBack;
    private bool m_disabled;
    private string m_checkBoxCssClass;
    private string m_textBoxCssClass;
    private string m_textBoxDisabledCssClass;
    private Color m_textBoxDisabledColor;
    private bool m_allowNullCheckBoxToWrap = true;
    private ValidatorPanel m_validatorPanel;
    private BaseValidator m_parameterRequiresValueValidator;

    public BaseParameterInputControl(
      ReportParameterInfo reportParam,
      IBrowserDetection browserDetection)
    {
      this.m_reportParam = reportParam;
      this.m_browserDetection = browserDetection;
    }

    protected override HtmlTextWriterTag TagKey => HtmlTextWriterTag.Div;

    public event EventHandler ValueChanged;

    public event EventHandler AutoPostBackOccurred;

    public bool Disabled
    {
      get => this.m_disabled;
      set => this.m_disabled = value;
    }

    public virtual bool AutoPostBack
    {
      get => this.m_autoPostBack;
      set
      {
        this.EnsureChildControls();
        this.m_autoPostBack = value;
      }
    }

    public virtual string CheckBoxCssClass
    {
      get => this.m_checkBoxCssClass;
      set => this.m_checkBoxCssClass = value;
    }

    public virtual string TextBoxCssClass
    {
      get => this.m_textBoxCssClass;
      set => this.m_textBoxCssClass = value;
    }

    public string TextBoxDisabledCssClass
    {
      get => this.m_textBoxDisabledCssClass;
      set => this.m_textBoxDisabledCssClass = value;
    }

    public Color TextBoxDisabledColor
    {
      get => this.m_textBoxDisabledColor;
      set => this.m_textBoxDisabledColor = value;
    }

    public string Prompt
    {
      get
      {
        return !string.IsNullOrEmpty(this.m_reportParam.Prompt) ? this.m_reportParam.Prompt : this.m_reportParam.Name;
      }
    }

    public abstract string AltText { set; }

    protected void ApplyStylesToTextBox(TextBox textBox)
    {
      textBox.Font.CopyFrom(this.Font);
      if (textBox.Enabled)
      {
        textBox.CssClass = this.TextBoxCssClass;
        textBox.Style.Remove(HtmlTextWriterStyle.BackgroundColor);
      }
      else
      {
        textBox.CssClass = this.TextBoxDisabledCssClass;
        if (this.m_textBoxDisabledColor.IsEmpty)
          return;
        textBox.Style.Add(HtmlTextWriterStyle.BackgroundColor, ColorTranslator.ToHtml(this.m_textBoxDisabledColor));
      }
    }

    public string ClientEnableFunctionName => this.ClientObject + ".SetEnableState";

    public bool AllowNullCheckBoxToWrap
    {
      get => this.m_allowNullCheckBoxToWrap;
      set => this.m_allowNullCheckBoxToWrap = value;
    }

    protected void WriteNullCheckBoxSpacer(HtmlTextWriter writer)
    {
      if (this.AllowNullCheckBoxToWrap)
        writer.Write(' ');
      else
        writer.Write("&nbsp");
    }

    protected override bool OnBubbleEvent(object source, EventArgs args)
    {
      if (!(args is AutoPostBackEventArgs))
        return base.OnBubbleEvent(source, args);
      if (this.AutoPostBackOccurred != null)
        this.AutoPostBackOccurred((object) this, EventArgs.Empty);
      return true;
    }

    public bool ShowDefaultValue
    {
      get => this.m_showDefaultValue;
      set => this.m_showDefaultValue = value;
    }

    protected override void OnInit(EventArgs e)
    {
      this.EnsureChildControls();
      if (!this.ShowDefaultValue || this.ReportParameter.Values.Count <= 0 || this.ReportParameter.State != ParameterState.HasValidValue)
        return;
      IList<string> values = this.ReportParameter.Values;
      string[] val = new string[values.Count];
      for (int index = 0; index < values.Count; ++index)
        val[index] = values[index];
      this.SetValue(val);
    }

    protected override void CreateChildControls()
    {
      this.Controls.Clear();
      base.CreateChildControls();
      this.m_validatorPanel = new ValidatorPanel();
      this.Controls.Add((Control) this.m_validatorPanel);
    }

    protected override void OnPreRender(EventArgs e)
    {
      base.OnPreRender(e);
      ScriptManager.GetCurrent(this.Page)?.RegisterScriptControl<BaseParameterInputControl>(this);
    }

    protected void RenderNullCheckBox(HtmlTextWriter writer)
    {
      if (this.m_nullCheckBox == null)
        return;
      this.m_nullCheckBox.Font.CopyFrom(this.Font);
      this.m_nullCheckBox.CssClass = this.CheckBoxCssClass;
      this.m_nullCheckBox.RenderControl(writer);
    }

    protected override void Render(HtmlTextWriter writer)
    {
      this.EnsureChildControls();
      ScriptManager.GetCurrent(this.Page)?.RegisterScriptDescriptors((IScriptControl) this);
      if (this.Disabled)
      {
        if (this.m_nullCheckBox != null)
          this.m_nullCheckBox.Enabled = false;
        this.SetCustomControlEnableState(false);
      }
      else
      {
        if (this.m_nullCheckBox != null)
          this.m_nullCheckBox.Enabled = true;
        this.SetCustomControlEnableState(this.m_nullCheckBox == null || !this.m_nullCheckBox.Checked);
      }
      base.Render(writer);
    }

    public abstract IEnumerable<ScriptDescriptor> GetScriptDescriptors();

    public IEnumerable<ScriptReference> GetScriptReferences()
    {
      ScriptReference scriptReference = new ScriptReference();
      scriptReference.Path = EmbeddedResourceOperation.CreateUrlForScriptFile();
      return (IEnumerable<ScriptReference>) new ScriptReference[1]
      {
        scriptReference
      };
    }

    protected void AddBaseDescriptorProperties(ScriptControlDescriptor desc)
    {
      this.EnsureChildControls();
      if (this.m_nullCheckBox != null)
        desc.AddProperty("NullCheckBoxId", (object) this.m_nullCheckBox.ClientID);
      desc.AddProperty("NullValueText", (object) ParameterInputControlStrings.NullValue);
      desc.AddProperty("ValidationMessage", (object) LocalizationHelper.Current.ParameterMissingValueError(this.ReportParameter.Prompt));
      desc.AddProperty("PostBackOnChange", (object) this.AutoPostBack);
      desc.AddProperty("ValidatorIdList", (object) this.Validators.ChildControlIds);
      desc.AddProperty("CustomInputControlIdList", (object) this.CustomControlIds);
      desc.AddProperty("TextBoxEnabledClass", (object) this.m_textBoxCssClass);
      desc.AddProperty("TextBoxDisabledClass", (object) this.m_textBoxDisabledCssClass);
      desc.AddProperty("TextBoxDisabledColor", (object) ColorTranslator.ToHtml(this.m_textBoxDisabledColor));
      string script = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "function(){{{0};}}", (object) this.Page.ClientScript.GetPostBackEventReference((Control) this, (string) null));
      desc.AddScriptProperty("TriggerPostBackScript", script);
    }

    protected abstract string[] CustomControlIds { get; }

    public abstract string PrimaryFormElementId { get; }

    protected ValidatorPanel Validators
    {
      get
      {
        this.EnsureChildControls();
        return this.m_validatorPanel;
      }
    }

    protected BaseValidator ParameterRequiresValueValidator
    {
      get => this.m_parameterRequiresValueValidator;
    }

    public void AddValidator(BaseValidator validator)
    {
      validator.Display = ValidatorDisplay.Dynamic;
      this.Validators.Controls.Add((Control) validator);
    }

    public BaseValidator AddParameterRquiresValueValidator()
    {
      this.m_parameterRequiresValueValidator = this.CreateParameterRequiresValueValidator();
      if (this.m_parameterRequiresValueValidator != null)
        this.AddValidator(this.m_parameterRequiresValueValidator);
      return this.m_parameterRequiresValueValidator;
    }

    protected abstract BaseValidator CreateParameterRequiresValueValidator();

    protected abstract void SetCustomControlEnableState(bool enabled);

    protected void OnValueChanged(object sender, EventArgs e)
    {
      if (this.ValueChanged == null || this.CurrentValue == null)
        return;
      this.ValueChanged((object) this, (EventArgs) null);
    }

    protected void OnCustomControlChanged(object sender, EventArgs e)
    {
      if (this.m_nullCheckBox != null && this.m_nullCheckBox.Checked)
        return;
      this.OnValueChanged(sender, e);
    }

    protected void CreateNullCheckBox()
    {
      this.m_nullCheckBox = this.m_nullCheckBox == null ? new PostBackCheckBox() : throw new Exception("Internal Error: Multiple null check boxes instantiated");
      this.m_nullCheckBox.Style.Add(HtmlTextWriterStyle.WhiteSpace, "nowrap");
      this.m_nullCheckBox.ID = "cbNull";
      this.m_nullCheckBox.Text = HttpUtility.HtmlEncode(this.NullCheckBoxString);
      this.m_nullCheckBox.CheckedChanged += new EventHandler(this.OnValueChanged);
      this.Controls.Add((Control) this.m_nullCheckBox);
    }

    public string[] CurrentValue
    {
      get
      {
        if (this.m_nullCheckBox != null && this.m_nullCheckBox.Checked)
          return new string[1];
        return !this.CustomControlHasValue ? (string[]) null : this.CustomControlValue;
      }
    }

    public void SetValue(string[] val)
    {
      if (this.m_nullCheckBox != null)
        this.m_nullCheckBox.Checked = val != null && val.Length == 1 && val[0] == null;
      this.CustomControlValue = val;
    }

    protected abstract string[] CustomControlValue { get; set; }

    protected abstract bool CustomControlHasValue { get; }

    internal string ClientObject
    {
      get
      {
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "$get('{0}').control", (object) JavaScriptHelper.StringEscapeSingleQuote(this.ClientID));
      }
    }

    public string GetClientDisplayValueFunctionCall => this.ClientObject + ".GetDisplayValue()";

    public ReportParameterInfo ReportParameter => this.m_reportParam;

    public string GetLabelForValidValue(ValidValue vv)
    {
      return (vv.Label ?? vv.Value) ?? "(" + this.NullValueText + ")";
    }

    protected IBrowserDetection BrowserDetection => this.m_browserDetection;
  }
}
