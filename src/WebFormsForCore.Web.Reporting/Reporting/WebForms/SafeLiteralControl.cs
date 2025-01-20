
using System.Web.UI;
using System.Web.UI.WebControls;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal class SafeLiteralControl : WebControl
  {
    public bool Disabled;
    public string Text;

    internal SafeLiteralControl(string text)
      : this(text, false)
    {
    }

    internal SafeLiteralControl(string text, bool disabled)
    {
      this.Disabled = disabled;
      this.Text = text;
    }

    internal SafeLiteralControl()
      : this((string) null, false)
    {
    }

    protected override void Render(HtmlTextWriter writer)
    {
      this.AddAttributesToRender(writer);
      if (this.Disabled)
        writer.AddAttribute(HtmlTextWriterAttribute.Disabled, "disabled");
      writer.RenderBeginTag(HtmlTextWriterTag.Span);
      if (this.Text != null)
        writer.WriteEncodedText(this.Text);
      writer.RenderEndTag();
    }
  }
}
