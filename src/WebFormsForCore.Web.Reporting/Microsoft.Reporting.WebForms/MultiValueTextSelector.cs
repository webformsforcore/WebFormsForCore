using System;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Microsoft.Reporting.WebForms;

internal sealed class MultiValueTextSelector : MultiValueSelector
{
	private PostBackTextBox m_textBox;

	private HiddenField m_originalTextBoxValue;

	private bool m_allowBlank;

	private string m_textBoxCssClass;

	public string TextBoxCssClass
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

	public string AltText
	{
		set
		{
			EnsureChildControls();
			m_textBox.ToolTip = value;
		}
	}

	public override bool HasValue => Value.Length > 0;

	public override string[] Value
	{
		get
		{
			EnsureChildControls();
			StringSplitOptions options = ((!m_allowBlank) ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None);
			return m_textBox.Text.Split(new string[2] { "\r\n", "\n" }, options);
		}
		set
		{
			EnsureChildControls();
			StringBuilder stringBuilder = new StringBuilder();
			foreach (string value2 in value)
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append("\n");
				}
				stringBuilder.Append(value2);
			}
			m_textBox.Text = stringBuilder.ToString();
			m_originalTextBoxValue.Value = m_textBox.Text;
		}
	}

	public MultiValueTextSelector(IBrowserDetection browserDetection, bool allowBlank)
	{
		m_allowBlank = allowBlank;
	}

	private void OnTextAreaChanged(object sender, EventArgs e)
	{
		if (!string.Equals(m_textBox.Text, m_originalTextBoxValue.Value, StringComparison.Ordinal))
		{
			OnChange(sender, e);
		}
	}

	protected override void CreateChildControls()
	{
		Controls.Clear();
		base.CreateChildControls();
		m_textBox = new PostBackTextBox();
		m_textBox.TextMode = TextBoxMode.MultiLine;
		m_textBox.TextChanged += OnTextAreaChanged;
		m_textBox.Wrap = false;
		m_textBox.Rows = 4;
		m_textBox.Columns = 28;
		m_textBox.Width = new Unit(100.0, UnitType.Percentage);
		m_textBox.BorderWidth = 0;
		m_textBox.Style["resize"] = "none";
		m_textBox.Style["outline"] = "none";
		m_textBox.Style[HtmlTextWriterStyle.Margin] = "0px";
		m_textBox.Style[HtmlTextWriterStyle.Padding] = "2px";
		m_textBox.Style[HtmlTextWriterStyle.Overflow] = "auto";
		Controls.Add(m_textBox);
		m_originalTextBoxValue = new HiddenField();
		Controls.Add(m_originalTextBoxValue);
	}

	protected override void Render(HtmlTextWriter writer)
	{
		EnsureChildControls();
		m_textBox.Font.CopyFrom(Font);
		m_textBox.CssClass = TextBoxCssClass;
		SetSelectorBorder();
		base.Render(writer);
	}
}
