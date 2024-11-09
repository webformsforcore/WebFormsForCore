// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.SafeLiteralControl
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

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
