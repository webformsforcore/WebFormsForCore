using System.Web.UI;
using System.Web.UI.WebControls;

namespace Microsoft.Reporting.WebForms;

internal class SafeLiteralControl : WebControl
{
	public bool Disabled;

	public string Text;

	internal SafeLiteralControl(string text)
		: this(text, disabled: false)
	{
	}

	internal SafeLiteralControl(string text, bool disabled)
	{
		Disabled = disabled;
		Text = text;
	}

	internal SafeLiteralControl()
		: this(null, disabled: false)
	{
	}

	protected override void Render(HtmlTextWriter writer)
	{
		AddAttributesToRender(writer);
		if (Disabled)
		{
			writer.AddAttribute(HtmlTextWriterAttribute.Disabled, "disabled");
		}
		writer.RenderBeginTag(HtmlTextWriterTag.Span);
		if (Text != null)
		{
			writer.WriteEncodedText(Text);
		}
		writer.RenderEndTag();
	}
}
