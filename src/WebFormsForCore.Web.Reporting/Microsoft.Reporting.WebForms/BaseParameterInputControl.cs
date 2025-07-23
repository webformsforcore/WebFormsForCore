using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Microsoft.Reporting.WebForms;

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

	protected override HtmlTextWriterTag TagKey => HtmlTextWriterTag.Div;

	public bool Disabled
	{
		get
		{
			return m_disabled;
		}
		set
		{
			m_disabled = value;
		}
	}

	public virtual bool AutoPostBack
	{
		get
		{
			return m_autoPostBack;
		}
		set
		{
			EnsureChildControls();
			m_autoPostBack = value;
		}
	}

	public virtual string CheckBoxCssClass
	{
		get
		{
			return m_checkBoxCssClass;
		}
		set
		{
			m_checkBoxCssClass = value;
		}
	}

	public virtual string TextBoxCssClass
	{
		get
		{
			return m_textBoxCssClass;
		}
		set
		{
			m_textBoxCssClass = value;
		}
	}

	public string TextBoxDisabledCssClass
	{
		get
		{
			return m_textBoxDisabledCssClass;
		}
		set
		{
			m_textBoxDisabledCssClass = value;
		}
	}

	public Color TextBoxDisabledColor
	{
		get
		{
			return m_textBoxDisabledColor;
		}
		set
		{
			m_textBoxDisabledColor = value;
		}
	}

	public string Prompt
	{
		get
		{
			if (!string.IsNullOrEmpty(m_reportParam.Prompt))
			{
				return m_reportParam.Prompt;
			}
			return m_reportParam.Name;
		}
	}

	public abstract string AltText { set; }

	public string ClientEnableFunctionName => ClientObject + ".SetEnableState";

	public bool AllowNullCheckBoxToWrap
	{
		get
		{
			return m_allowNullCheckBoxToWrap;
		}
		set
		{
			m_allowNullCheckBoxToWrap = value;
		}
	}

	public bool ShowDefaultValue
	{
		get
		{
			return m_showDefaultValue;
		}
		set
		{
			m_showDefaultValue = value;
		}
	}

	protected abstract string[] CustomControlIds { get; }

	public abstract string PrimaryFormElementId { get; }

	protected ValidatorPanel Validators
	{
		get
		{
			EnsureChildControls();
			return m_validatorPanel;
		}
	}

	protected BaseValidator ParameterRequiresValueValidator => m_parameterRequiresValueValidator;

	public string[] CurrentValue
	{
		get
		{
			if (m_nullCheckBox != null && m_nullCheckBox.Checked)
			{
				return new string[1];
			}
			if (!CustomControlHasValue)
			{
				return null;
			}
			return CustomControlValue;
		}
	}

	protected abstract string[] CustomControlValue { get; set; }

	protected abstract bool CustomControlHasValue { get; }

	internal string ClientObject => string.Format(CultureInfo.InvariantCulture, "$get('{0}').control", JavaScriptHelper.StringEscapeSingleQuote(ClientID));

	public string GetClientDisplayValueFunctionCall => ClientObject + ".GetDisplayValue()";

	public ReportParameterInfo ReportParameter => m_reportParam;

	protected IBrowserDetection BrowserDetection => m_browserDetection;

	public event EventHandler ValueChanged;

	public event EventHandler AutoPostBackOccurred;

	public BaseParameterInputControl(ReportParameterInfo reportParam, IBrowserDetection browserDetection)
	{
		m_reportParam = reportParam;
		m_browserDetection = browserDetection;
	}

	protected void ApplyStylesToTextBox(TextBox textBox)
	{
		textBox.Font.CopyFrom(Font);
		if (textBox.Enabled)
		{
			textBox.CssClass = TextBoxCssClass;
			textBox.Style.Remove(HtmlTextWriterStyle.BackgroundColor);
			return;
		}
		textBox.CssClass = TextBoxDisabledCssClass;
		if (!m_textBoxDisabledColor.IsEmpty)
		{
			textBox.Style.Add(HtmlTextWriterStyle.BackgroundColor, ColorTranslator.ToHtml(m_textBoxDisabledColor));
		}
	}

	protected void WriteNullCheckBoxSpacer(HtmlTextWriter writer)
	{
		if (AllowNullCheckBoxToWrap)
		{
			writer.Write(' ');
		}
		else
		{
			writer.Write("&nbsp");
		}
	}

	protected override bool OnBubbleEvent(object source, EventArgs args)
	{
		if (args is AutoPostBackEventArgs)
		{
			if (this.AutoPostBackOccurred != null)
			{
				this.AutoPostBackOccurred(this, EventArgs.Empty);
			}
			return true;
		}
		return base.OnBubbleEvent(source, args);
	}

	protected override void OnInit(EventArgs e)
	{
		EnsureChildControls();
		if (ShowDefaultValue && ReportParameter.Values.Count > 0 && ReportParameter.State == ParameterState.HasValidValue)
		{
			IList<string> values = ReportParameter.Values;
			string[] array = new string[values.Count];
			for (int i = 0; i < values.Count; i++)
			{
				array[i] = values[i];
			}
			SetValue(array);
		}
	}

	protected override void CreateChildControls()
	{
		Controls.Clear();
		base.CreateChildControls();
		m_validatorPanel = new ValidatorPanel();
		Controls.Add(m_validatorPanel);
	}

	protected override void OnPreRender(EventArgs e)
	{
		base.OnPreRender(e);
		ScriptManager.GetCurrent(Page)?.RegisterScriptControl(this);
	}

	protected void RenderNullCheckBox(HtmlTextWriter writer)
	{
		if (m_nullCheckBox != null)
		{
			m_nullCheckBox.Font.CopyFrom(Font);
			m_nullCheckBox.CssClass = CheckBoxCssClass;
			m_nullCheckBox.RenderControl(writer);
		}
	}

	protected override void Render(HtmlTextWriter writer)
	{
		EnsureChildControls();
		ScriptManager.GetCurrent(Page)?.RegisterScriptDescriptors(this);
		if (Disabled)
		{
			if (m_nullCheckBox != null)
			{
				m_nullCheckBox.Enabled = false;
			}
			SetCustomControlEnableState(enabled: false);
		}
		else
		{
			if (m_nullCheckBox != null)
			{
				m_nullCheckBox.Enabled = true;
			}
			bool customControlEnableState = m_nullCheckBox == null || !m_nullCheckBox.Checked;
			SetCustomControlEnableState(customControlEnableState);
		}
		base.Render(writer);
	}

	public abstract IEnumerable<ScriptDescriptor> GetScriptDescriptors();

	public IEnumerable<ScriptReference> GetScriptReferences()
	{
		ScriptReference scriptReference = new ScriptReference();
		scriptReference.Path = EmbeddedResourceOperation.CreateUrlForScriptFile();
		return new ScriptReference[1] { scriptReference };
	}

	protected void AddBaseDescriptorProperties(ScriptControlDescriptor desc)
	{
		EnsureChildControls();
		if (m_nullCheckBox != null)
		{
			desc.AddProperty("NullCheckBoxId", m_nullCheckBox.ClientID);
		}
		desc.AddProperty("NullValueText", ParameterInputControlStrings.NullValue);
		desc.AddProperty("ValidationMessage", LocalizationHelper.Current.ParameterMissingValueError(ReportParameter.Prompt));
		desc.AddProperty("PostBackOnChange", AutoPostBack);
		desc.AddProperty("ValidatorIdList", Validators.ChildControlIds);
		desc.AddProperty("CustomInputControlIdList", CustomControlIds);
		desc.AddProperty("TextBoxEnabledClass", m_textBoxCssClass);
		desc.AddProperty("TextBoxDisabledClass", m_textBoxDisabledCssClass);
		desc.AddProperty("TextBoxDisabledColor", ColorTranslator.ToHtml(m_textBoxDisabledColor));
		string script = string.Format(CultureInfo.InvariantCulture, "function(){{{0};}}", Page.ClientScript.GetPostBackEventReference(this, null));
		desc.AddScriptProperty("TriggerPostBackScript", script);
	}

	public void AddValidator(BaseValidator validator)
	{
		validator.Display = ValidatorDisplay.Dynamic;
		Validators.Controls.Add(validator);
	}

	public BaseValidator AddParameterRquiresValueValidator()
	{
		m_parameterRequiresValueValidator = CreateParameterRequiresValueValidator();
		if (m_parameterRequiresValueValidator != null)
		{
			AddValidator(m_parameterRequiresValueValidator);
		}
		return m_parameterRequiresValueValidator;
	}

	protected abstract BaseValidator CreateParameterRequiresValueValidator();

	protected abstract void SetCustomControlEnableState(bool enabled);

	protected void OnValueChanged(object sender, EventArgs e)
	{
		if (this.ValueChanged != null && CurrentValue != null)
		{
			this.ValueChanged(this, null);
		}
	}

	protected void OnCustomControlChanged(object sender, EventArgs e)
	{
		if (m_nullCheckBox == null || !m_nullCheckBox.Checked)
		{
			OnValueChanged(sender, e);
		}
	}

	protected void CreateNullCheckBox()
	{
		if (m_nullCheckBox != null)
		{
			throw new Exception("Internal Error: Multiple null check boxes instantiated");
		}
		m_nullCheckBox = new PostBackCheckBox();
		m_nullCheckBox.Style.Add(HtmlTextWriterStyle.WhiteSpace, "nowrap");
		m_nullCheckBox.ID = "cbNull";
		m_nullCheckBox.Text = HttpUtility.HtmlEncode(NullCheckBoxString);
		m_nullCheckBox.CheckedChanged += OnValueChanged;
		Controls.Add(m_nullCheckBox);
	}

	public void SetValue(string[] val)
	{
		if (m_nullCheckBox != null)
		{
			m_nullCheckBox.Checked = val != null && val.Length == 1 && val[0] == null;
		}
		CustomControlValue = val;
	}

	public string GetLabelForValidValue(ValidValue vv)
	{
		string text = vv.Label;
		if (text == null)
		{
			text = vv.Value;
		}
		if (text == null)
		{
			text = "(" + NullValueText + ")";
		}
		return text;
	}
}
