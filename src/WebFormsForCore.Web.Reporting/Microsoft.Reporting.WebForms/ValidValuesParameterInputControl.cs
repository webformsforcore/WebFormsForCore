using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Microsoft.Reporting.WebForms;

internal class ValidValuesParameterInputControl : BaseParameterInputControl
{
	public string SelectAValueString = ParameterInputControlStrings.SelectValidValue;

	protected SpaceAwareDropDownList m_dropDown;

	private string m_emptyDropDownCssClass;

	protected override string[] CustomControlValue
	{
		get
		{
			EnsureChildControls();
			return new string[1] { base.ReportParameter.ValidValues[m_dropDown.SelectedIndex - 1].Value };
		}
		set
		{
			EnsureChildControls();
			if (value == null || value.Length != 1)
			{
				m_dropDown.SelectedIndex = 0;
				return;
			}
			for (int i = 0; i < base.ReportParameter.ValidValues.Count; i++)
			{
				if (string.Compare(base.ReportParameter.ValidValues[i].Value, value[0], StringComparison.Ordinal) == 0)
				{
					m_dropDown.SelectedIndex = i + 1;
					break;
				}
			}
		}
	}

	protected override bool CustomControlHasValue
	{
		get
		{
			EnsureChildControls();
			return m_dropDown.SelectedIndex > 0;
		}
	}

	protected override string[] CustomControlIds
	{
		get
		{
			EnsureChildControls();
			return new string[1] { m_dropDown.ClientID };
		}
	}

	public override string PrimaryFormElementId
	{
		get
		{
			EnsureChildControls();
			return m_dropDown.ClientID;
		}
	}

	public override bool AutoPostBack
	{
		set
		{
			EnsureChildControls();
			m_dropDown.AutoPostBack = value;
			base.AutoPostBack = value;
		}
	}

	public string EmptyDropDownCssClass
	{
		get
		{
			return m_emptyDropDownCssClass;
		}
		set
		{
			m_emptyDropDownCssClass = value;
		}
	}

	public override string AltText
	{
		set
		{
			EnsureChildControls();
			m_dropDown.ToolTip = value;
		}
	}

	public ValidValuesParameterInputControl(ReportParameterInfo reportParam, IBrowserDetection browserDetection)
		: base(reportParam, browserDetection)
	{
	}

	protected override void CreateChildControls()
	{
		Controls.Clear();
		base.CreateChildControls();
		m_dropDown = new SpaceAwareDropDownList();
		m_dropDown.ID = "ddValue";
		m_dropDown.SelectedIndexChanged += base.OnCustomControlChanged;
		if (base.ReportParameter.ValidValues != null)
		{
			int num = 1;
			foreach (ValidValue validValue in base.ReportParameter.ValidValues)
			{
				string labelForValidValue = GetLabelForValidValue(validValue);
				m_dropDown.Items.Add(new ListItem(labelForValidValue, num.ToString(CultureInfo.InvariantCulture)));
				num++;
			}
		}
		if (m_dropDown.Items.Count > 0)
		{
			m_dropDown.Items.Insert(0, new ListItem(SelectAValueString, "0"));
			m_dropDown.SelectedIndex = 0;
		}
		Controls.Add(m_dropDown);
	}

	protected override BaseValidator CreateParameterRequiresValueValidator()
	{
		return new DropDownValidator();
	}

	protected override void OnPreRender(EventArgs e)
	{
		EnsureChildControls();
		if (base.ParameterRequiresValueValidator != null)
		{
			base.ParameterRequiresValueValidator.ControlToValidate = m_dropDown.ID;
		}
		base.OnPreRender(e);
	}

	protected override void RenderContents(HtmlTextWriter writer)
	{
		m_dropDown.Font.CopyFrom(Font);
		if (m_dropDown.Items.Count == 0)
		{
			if (!string.IsNullOrEmpty(m_emptyDropDownCssClass))
			{
				m_dropDown.CssClass = EmptyDropDownCssClass;
			}
			else
			{
				m_dropDown.Style.Add(HtmlTextWriterStyle.Width, "15ex");
			}
		}
		m_dropDown.RenderControl(writer);
		if (base.Validators.HasValidatorsToRender)
		{
			writer.Write("<br>");
			base.Validators.RenderControl(writer);
		}
	}

	protected override void SetCustomControlEnableState(bool enabled)
	{
		m_dropDown.Enabled = enabled;
	}

	public override IEnumerable<ScriptDescriptor> GetScriptDescriptors()
	{
		EnsureChildControls();
		ScriptControlDescriptor scriptControlDescriptor = new ScriptControlDescriptor("Microsoft.Reporting.WebFormsClient._ValidValueParameterInputControl", ClientID);
		AddBaseDescriptorProperties(scriptControlDescriptor);
		scriptControlDescriptor.AddProperty("DropDownId", m_dropDown.ClientID);
		BaseValidator parameterRequiresValueValidator = base.ParameterRequiresValueValidator;
		if (parameterRequiresValueValidator != null)
		{
			scriptControlDescriptor.AddProperty("DropDownValidatorId", parameterRequiresValueValidator.ClientID);
		}
		return new ScriptDescriptor[1] { scriptControlDescriptor };
	}
}
