using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Microsoft.Reporting.WebForms;

internal sealed class MultiValidValuesSelector : MultiValueSelector
{
	private class NonPostBackCheckBox : CheckBox
	{
		protected override bool LoadPostData(string postDataKey, NameValueCollection postCollection)
		{
			return false;
		}
	}

	private class SelectedIndicies : CompositeControl
	{
		private const string ValidValueListHash = "ValidValueHash";

		private HiddenField m_hiddenIndices;

		private uint? m_validValuesHash;

		public string HiddenFieldForClientUpdates
		{
			get
			{
				EnsureChildControls();
				return m_hiddenIndices.ClientID;
			}
		}

		public IList<int> Indicies
		{
			get
			{
				EnsureChildControls();
				uint? num = (uint?)ViewState["ValidValueHash"];
				if (num.HasValue && num != m_validValuesHash)
				{
					return null;
				}
				string[] array = m_hiddenIndices.Value.Split(',');
				List<int> list = new List<int>(array.Length);
				string[] array2 = array;
				foreach (string s in array2)
				{
					if (!int.TryParse(s, out var result))
					{
						return null;
					}
					list.Add(result);
				}
				return list;
			}
			set
			{
				EnsureChildControls();
				StringBuilder stringBuilder = new StringBuilder();
				if (value != null)
				{
					foreach (int item in value)
					{
						if (stringBuilder.Length > 0)
						{
							stringBuilder.Append(',');
						}
						stringBuilder.Append(item.ToString(CultureInfo.InvariantCulture));
					}
				}
				m_hiddenIndices.Value = stringBuilder.ToString();
				ViewState["ValidValueHash"] = m_validValuesHash;
			}
		}

		public event EventHandler ValueChanged;

		public SelectedIndicies(IEnumerable<ValidValue> validValues)
		{
			m_validValuesHash = ComputeValidValuesHash(validValues);
		}

		protected override void CreateChildControls()
		{
			base.CreateChildControls();
			Controls.Clear();
			m_hiddenIndices = new HiddenField();
			m_hiddenIndices.ID = "HiddenIndices";
			m_hiddenIndices.ValueChanged += OnChange;
			Controls.Add(m_hiddenIndices);
		}

		private void OnChange(object sender, EventArgs e)
		{
			if (this.ValueChanged != null)
			{
				this.ValueChanged(this, EventArgs.Empty);
			}
		}

		private static uint ComputeValidValuesHash(IEnumerable<ValidValue> validValues)
		{
			uint num = 0u;
			if (validValues != null)
			{
				foreach (ValidValue validValue in validValues)
				{
					ulong num2 = (ulong)(num + validValue.Label.GetHashCode() + validValue.Value.GetHashCode());
					num = (uint)((num2 > uint.MaxValue) ? (num2 % uint.MaxValue) : num2);
				}
			}
			return num;
		}
	}

	private Dictionary<string, CheckBox> m_valueToCheckBox = new Dictionary<string, CheckBox>();

	private List<CheckBox> m_allCheckBoxes = new List<CheckBox>();

	private IList<ValidValue> m_validValues;

	private BaseParameterInputControl m_containingControl;

	private CheckBox m_selectAll;

	private string m_checkBoxCssClass;

	private SelectedIndicies m_selectedIndicies;

	public string CheckBoxCssClass
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

	public override bool HasValue
	{
		get
		{
			EnsureChildControls();
			IList<int> indicies = m_selectedIndicies.Indicies;
			if (indicies != null)
			{
				return indicies.Count > 0;
			}
			return false;
		}
	}

	public override string[] Value
	{
		get
		{
			EnsureChildControls();
			IList<int> indicies = m_selectedIndicies.Indicies;
			List<string> list = new List<string>(indicies.Count);
			foreach (int item in indicies)
			{
				list.Add(m_validValues[item].Value);
			}
			return list.ToArray();
		}
		set
		{
			EnsureChildControls();
			int count = m_allCheckBoxes.Count;
			for (int i = 0; i < count; i++)
			{
				m_allCheckBoxes[i].Checked = false;
			}
			foreach (string key in value)
			{
				if (m_valueToCheckBox.TryGetValue(key, out var value2))
				{
					value2.Checked = true;
				}
			}
			List<int> list = new List<int>();
			bool flag = false;
			for (int k = 0; k < count; k++)
			{
				if (!m_allCheckBoxes[k].Checked)
				{
					flag = true;
				}
				else
				{
					list.Add(k);
				}
			}
			if (m_selectAll != null)
			{
				m_selectAll.Checked = !flag;
			}
			m_selectedIndicies.Indicies = list;
		}
	}

	public MultiValidValuesSelector(IList<ValidValue> validValues, BaseParameterInputControl containingControl)
	{
		m_validValues = validValues;
		m_containingControl = containingControl;
	}

	public override void AddScriptDescriptors(ScriptControlDescriptor desc)
	{
		EnsureChildControls();
		base.AddScriptDescriptors(desc);
		desc.AddProperty("HiddenIndicesId", m_selectedIndicies.HiddenFieldForClientUpdates);
	}

	protected override void CreateChildControls()
	{
		Controls.Clear();
		base.CreateChildControls();
		m_valueToCheckBox.Clear();
		m_allCheckBoxes.Clear();
		m_selectAll = new NonPostBackCheckBox();
		m_selectAll.EnableViewState = false;
		m_selectAll.Text = HttpUtility.HtmlEncode(LocalizationHelper.Current.SelectAll);
		Controls.Add(m_selectAll);
		m_selectedIndicies = new SelectedIndicies(m_validValues);
		m_selectedIndicies.ValueChanged += base.OnChange;
		Controls.Add(m_selectedIndicies);
		if (m_validValues == null)
		{
			return;
		}
		foreach (ValidValue validValue in m_validValues)
		{
			CheckBox checkBox = new NonPostBackCheckBox();
			string text = HttpUtility.HtmlEncode(m_containingControl.GetLabelForValidValue(validValue));
			checkBox.Text = text.Replace(" ", "&nbsp;");
			checkBox.EnableViewState = false;
			Controls.Add(checkBox);
			if (!m_valueToCheckBox.ContainsKey(validValue.Value))
			{
				m_valueToCheckBox.Add(validValue.Value, checkBox);
			}
			m_allCheckBoxes.Add(checkBox);
		}
	}

	protected override void Render(HtmlTextWriter writer)
	{
		EnsureChildControls();
		SetSelectorBorder();
		AddAttributesToRender(writer);
		if (m_allCheckBoxes.Count > 6)
		{
			writer.AddStyleAttribute(HtmlTextWriterStyle.Height, "150px");
		}
		else if (m_allCheckBoxes.Count < 3)
		{
			writer.AddStyleAttribute(HtmlTextWriterStyle.Height, "75px");
		}
		writer.AddStyleAttribute(HtmlTextWriterStyle.Overflow, "auto");
		if (string.IsNullOrEmpty(CssClass))
		{
			writer.AddStyleAttribute(HtmlTextWriterStyle.BackgroundColor, "window");
		}
		writer.RenderBeginTag(HtmlTextWriterTag.Div);
		writer.AddAttribute(HtmlTextWriterAttribute.Cellpadding, "0");
		writer.AddAttribute(HtmlTextWriterAttribute.Cellspacing, "0");
		if (string.IsNullOrEmpty(CssClass))
		{
			writer.AddStyleAttribute(HtmlTextWriterStyle.BackgroundColor, "window");
		}
		writer.RenderBeginTag(HtmlTextWriterTag.Table);
		string input = null;
		if (m_allCheckBoxes.Count > 1)
		{
			string format = "{0}.OnSelectAllClick(this);";
			string value = string.Format(CultureInfo.InvariantCulture, format, ClientSideObjectName);
			m_selectAll.Attributes.Add("onclick", value);
			input = m_selectAll.ClientID;
			WriteCheckBox(m_selectAll, writer);
		}
		foreach (CheckBox allCheckBox in m_allCheckBoxes)
		{
			allCheckBox.Attributes["onclick"] = string.Format(CultureInfo.InvariantCulture, "{0}.OnValidValueClick(this, '{1}');", ClientSideObjectName, JavaScriptHelper.StringEscapeSingleQuote(input));
			WriteCheckBox(allCheckBox, writer);
		}
		writer.RenderEndTag();
		m_selectedIndicies.RenderControl(writer);
		writer.RenderEndTag();
	}

	private void WriteCheckBox(CheckBox checkBox, HtmlTextWriter writer)
	{
		writer.RenderBeginTag(HtmlTextWriterTag.Tr);
		writer.AddAttribute(HtmlTextWriterAttribute.Nowrap, "nowrap");
		writer.RenderBeginTag(HtmlTextWriterTag.Td);
		checkBox.Font.CopyFrom(Font);
		checkBox.CssClass = CheckBoxCssClass;
		checkBox.RenderControl(writer);
		writer.RenderEndTag();
		writer.RenderEndTag();
	}
}
