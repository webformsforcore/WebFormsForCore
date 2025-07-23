using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Microsoft.Reporting.WebForms;

internal class BooleanParameterInputControl : BaseParameterInputControl
{
	public string TrueValueText = ParameterInputControlStrings.True;

	public string FalseValueText = ParameterInputControlStrings.False;

	protected PostBackRadioButton m_trueButton;

	protected PostBackRadioButton m_falseButton;

	private string m_altText;

	protected override string[] CustomControlIds
	{
		get
		{
			EnsureChildControls();
			return new string[2] { m_trueButton.ClientID, m_falseButton.ClientID };
		}
	}

	public override string PrimaryFormElementId
	{
		get
		{
			EnsureChildControls();
			return m_trueButton.ClientID;
		}
	}

	public override string AltText
	{
		set
		{
			m_altText = value;
		}
	}

	protected override string[] CustomControlValue
	{
		get
		{
			EnsureChildControls();
			return new string[1] { m_trueButton.Checked.ToString() };
		}
		set
		{
			EnsureChildControls();
			if (value == null || value.Length != 1)
			{
				m_trueButton.Checked = false;
				m_falseButton.Checked = false;
				return;
			}
			if (value[0] == null)
			{
				m_trueButton.Checked = false;
				m_falseButton.Checked = false;
				return;
			}
			if (bool.TryParse(value[0], out var result))
			{
				m_trueButton.Checked = result;
				m_falseButton.Checked = !result;
				return;
			}
			throw new ArgumentOutOfRangeException("value");
		}
	}

	protected override bool CustomControlHasValue
	{
		get
		{
			EnsureChildControls();
			if (!m_trueButton.Checked)
			{
				return m_falseButton.Checked;
			}
			return true;
		}
	}

	public override bool AutoPostBack
	{
		set
		{
			EnsureChildControls();
			m_trueButton.AutoPostBack = value;
			m_falseButton.AutoPostBack = value;
			base.AutoPostBack = value;
		}
	}

	public BooleanParameterInputControl(ReportParameterInfo reportParam, IBrowserDetection browserDetection)
		: base(reportParam, browserDetection)
	{
	}

	protected override void CreateChildControls()
	{
		Controls.Clear();
		base.CreateChildControls();
		m_trueButton = CreateRadioButton(TrueValueText, "rbTrue");
		Controls.Add(m_trueButton);
		m_falseButton = CreateRadioButton(FalseValueText, "rbFalse");
		Controls.Add(m_falseButton);
		m_trueButton.Attributes.Add("forceSpan", "true");
		m_falseButton.Attributes.Add("forceSpan", "true");
		if (base.ReportParameter.Nullable)
		{
			CreateNullCheckBox();
		}
	}

	protected override BaseValidator CreateParameterRequiresValueValidator()
	{
		EnsureChildControls();
		return new BooleanParamValidator(m_trueButton, m_falseButton);
	}

	protected override void OnPreRender(EventArgs e)
	{
		base.OnPreRender(e);
		m_trueButton.GroupName = HttpUtility.HtmlAttributeEncode(ClientID);
		m_falseButton.GroupName = m_trueButton.GroupName;
	}

	protected override void RenderContents(HtmlTextWriter writer)
	{
		EnsureChildControls();
		writer.AddAttribute(HtmlTextWriterAttribute.Cellpadding, "0");
		writer.AddAttribute(HtmlTextWriterAttribute.Cellspacing, "0");
		if (!string.IsNullOrEmpty(m_altText))
		{
			writer.AddAttribute(HtmlTextWriterAttribute.Title, m_altText);
		}
		writer.RenderBeginTag(HtmlTextWriterTag.Table);
		writer.RenderBeginTag(HtmlTextWriterTag.Tr);
		writer.RenderBeginTag(HtmlTextWriterTag.Td);
		m_trueButton.Font.CopyFrom(Font);
		m_trueButton.CssClass = CheckBoxCssClass;
		m_trueButton.RenderControl(writer);
		writer.RenderEndTag();
		writer.RenderBeginTag(HtmlTextWriterTag.Td);
		m_falseButton.Font.CopyFrom(Font);
		m_falseButton.CssClass = CheckBoxCssClass;
		m_falseButton.RenderControl(writer);
		writer.RenderEndTag();
		if (m_nullCheckBox != null)
		{
			writer.RenderBeginTag(HtmlTextWriterTag.Td);
			writer.Write("&nbsp;");
			RenderNullCheckBox(writer);
			writer.RenderEndTag();
		}
		writer.RenderEndTag();
		writer.RenderEndTag();
		if (base.Validators.HasValidatorsToRender)
		{
			base.Validators.RenderControl(writer);
		}
	}

	private PostBackRadioButton CreateRadioButton(string unencodedText, string id)
	{
		PostBackRadioButton postBackRadioButton = new PostBackRadioButton();
		postBackRadioButton.CheckedChanged += base.OnCustomControlChanged;
		postBackRadioButton.Attributes.Add("forceSpan", "true");
		postBackRadioButton.Text = HttpUtility.HtmlEncode(unencodedText);
		postBackRadioButton.ID = id;
		return postBackRadioButton;
	}

	protected override void SetCustomControlEnableState(bool enabled)
	{
		m_falseButton.Enabled = enabled;
		m_trueButton.Enabled = enabled;
	}

	public override IEnumerable<ScriptDescriptor> GetScriptDescriptors()
	{
		EnsureChildControls();
		ScriptControlDescriptor scriptControlDescriptor = new ScriptControlDescriptor("Microsoft.Reporting.WebFormsClient._BoolParameterInputControl", ClientID);
		AddBaseDescriptorProperties(scriptControlDescriptor);
		scriptControlDescriptor.AddProperty("TrueCheckId", m_trueButton.ClientID);
		scriptControlDescriptor.AddProperty("FalseCheckId", m_falseButton.ClientID);
		scriptControlDescriptor.AddProperty("TrueValueText", TrueValueText);
		scriptControlDescriptor.AddProperty("FalseValueText", FalseValueText);
		return new ScriptDescriptor[1] { scriptControlDescriptor };
	}
}
