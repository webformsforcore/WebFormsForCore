
using System;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal sealed class MultiValueTextSelector : MultiValueSelector
  {
    private PostBackTextBox m_textBox;
    private HiddenField m_originalTextBoxValue;
    private bool m_allowBlank;
    private string m_textBoxCssClass;

    public MultiValueTextSelector(IBrowserDetection browserDetection, bool allowBlank)
    {
      this.m_allowBlank = allowBlank;
    }

    public string TextBoxCssClass
    {
      get => this.m_textBoxCssClass;
      set => this.m_textBoxCssClass = value;
    }

    public string AltText
    {
      set
      {
        this.EnsureChildControls();
        this.m_textBox.ToolTip = value;
      }
    }

    public override bool HasValue => this.Value.Length > 0;

    public override string[] Value
    {
      get
      {
        this.EnsureChildControls();
        return this.m_textBox.Text.Split(new string[2]
        {
          "\r\n",
          "\n"
        }, this.m_allowBlank ? StringSplitOptions.None : StringSplitOptions.RemoveEmptyEntries);
      }
      set
      {
        this.EnsureChildControls();
        StringBuilder stringBuilder = new StringBuilder();
        foreach (string str in value)
        {
          if (stringBuilder.Length > 0)
            stringBuilder.Append("\n");
          stringBuilder.Append(str);
        }
        this.m_textBox.Text = stringBuilder.ToString();
        this.m_originalTextBoxValue.Value = this.m_textBox.Text;
      }
    }

    private void OnTextAreaChanged(object sender, EventArgs e)
    {
      if (string.Equals(this.m_textBox.Text, this.m_originalTextBoxValue.Value, StringComparison.Ordinal))
        return;
      this.OnChange(sender, e);
    }

    protected override void CreateChildControls()
    {
      this.Controls.Clear();
      base.CreateChildControls();
      this.m_textBox = new PostBackTextBox();
      this.m_textBox.TextMode = TextBoxMode.MultiLine;
      this.m_textBox.TextChanged += new EventHandler(this.OnTextAreaChanged);
      this.m_textBox.Wrap = false;
      this.m_textBox.Rows = 4;
      this.m_textBox.Columns = 28;
      this.m_textBox.Width = new Unit(100.0, UnitType.Percentage);
      this.m_textBox.BorderWidth = (Unit) 0;
      this.m_textBox.Style["resize"] = "none";
      this.m_textBox.Style["outline"] = "none";
      this.m_textBox.Style[HtmlTextWriterStyle.Margin] = "0px";
      this.m_textBox.Style[HtmlTextWriterStyle.Padding] = "2px";
      this.m_textBox.Style[HtmlTextWriterStyle.Overflow] = "auto";
      this.Controls.Add((Control) this.m_textBox);
      this.m_originalTextBoxValue = new HiddenField();
      this.Controls.Add((Control) this.m_originalTextBoxValue);
    }

    protected override void Render(HtmlTextWriter writer)
    {
      this.EnsureChildControls();
      this.m_textBox.Font.CopyFrom(this.Font);
      this.m_textBox.CssClass = this.TextBoxCssClass;
      this.SetSelectorBorder();
      base.Render(writer);
    }
  }
}
