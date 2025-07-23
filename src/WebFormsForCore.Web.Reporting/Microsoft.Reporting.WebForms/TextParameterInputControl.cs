using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Microsoft.Reporting.WebForms;

internal class TextParameterInputControl : BaseParameterInputControl
{
	protected PostBackTextBox m_textBox;

	protected override string[] CustomControlValue
	{
		get
		{
			EnsureChildControls();
			return new string[1] { m_textBox.Text };
		}
		set
		{
			EnsureChildControls();
			if (value == null || value.Length != 1)
			{
				m_textBox.Text = "";
			}
			else
			{
				m_textBox.Text = value[0];
			}
		}
	}

	protected override bool CustomControlHasValue
	{
		get
		{
			EnsureChildControls();
			if (!base.ReportParameter.AllowBlank)
			{
				return m_textBox.Text.Length != 0;
			}
			return true;
		}
	}

	public override bool AutoPostBack
	{
		set
		{
			EnsureChildControls();
			m_textBox.AutoPostBack = value;
			base.AutoPostBack = value;
		}
	}

	protected override string[] CustomControlIds
	{
		get
		{
			EnsureChildControls();
			return new string[1] { m_textBox.ClientID };
		}
	}

	public override string PrimaryFormElementId
	{
		get
		{
			EnsureChildControls();
			return m_textBox.ClientID;
		}
	}

	public override string AltText
	{
		set
		{
			EnsureChildControls();
			m_textBox.ToolTip = value;
		}
	}

	public TextParameterInputControl(ReportParameterInfo reportParam, IBrowserDetection browserDetection)
		: base(reportParam, browserDetection)
	{
	}

	protected override void CreateChildControls()
	{
		Controls.Clear();
		base.CreateChildControls();
		m_textBox = new PostBackTextBox();
		m_textBox.ID = "txtValue";
		m_textBox.Columns = 30;
		m_textBox.TextChanged += base.OnCustomControlChanged;
		Controls.Add(m_textBox);
		if (base.ReportParameter.Nullable)
		{
			CreateNullCheckBox();
		}
	}

	protected override BaseValidator CreateParameterRequiresValueValidator()
	{
		if (base.ReportParameter.DataType != ParameterDataType.String || !base.ReportParameter.AllowBlank)
		{
			return new RequiredFieldValidator();
		}
		return null;
	}

	protected override void RenderContents(HtmlTextWriter writer)
	{
		EnsureChildControls();
		ApplyStylesToTextBox(m_textBox);
		m_textBox.RenderControl(writer);
		if (m_nullCheckBox != null)
		{
			WriteNullCheckBoxSpacer(writer);
			RenderNullCheckBox(writer);
		}
		if (base.Validators.HasValidatorsToRender)
		{
			writer.Write("<br>");
			base.Validators.RenderControl(writer);
		}
	}

	protected override void OnPreRender(EventArgs e)
	{
		EnsureChildControls();
		if (base.ParameterRequiresValueValidator != null)
		{
			base.ParameterRequiresValueValidator.ControlToValidate = m_textBox.ID;
		}
		base.OnPreRender(e);
	}

	protected override void SetCustomControlEnableState(bool enabled)
	{
		m_textBox.Enabled = enabled;
	}

	public override IEnumerable<ScriptDescriptor> GetScriptDescriptors()
	{
		EnsureChildControls();
		ScriptControlDescriptor scriptControlDescriptor = new ScriptControlDescriptor("Microsoft.Reporting.WebFormsClient._TextParameterInputControl", ClientID);
		AddBaseDescriptorProperties(scriptControlDescriptor);
		scriptControlDescriptor.AddProperty("TextBoxId", m_textBox.ClientID);
		scriptControlDescriptor.AddProperty("AllowBlank", base.ReportParameter.AllowBlank);
		return new ScriptDescriptor[1] { scriptControlDescriptor };
	}
}
