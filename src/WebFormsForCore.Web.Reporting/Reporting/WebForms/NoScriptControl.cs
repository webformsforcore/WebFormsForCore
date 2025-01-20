
using System.Web.UI;
using System.Web.UI.WebControls;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal class NoScriptControl : WebControl
  {
    private string m_alternateUrl;

    public string AlternateUrl
    {
      get => this.m_alternateUrl;
      set => this.m_alternateUrl = value;
    }

    protected override HtmlTextWriterTag TagKey => HtmlTextWriterTag.Noscript;

    protected override void RenderContents(HtmlTextWriter writer)
    {
      writer.Write("&nbsp;");
      if (string.IsNullOrEmpty(this.m_alternateUrl))
      {
        writer.WriteEncodedText(LocalizationHelper.Current.ClientNoScript);
      }
      else
      {
        writer.WriteEncodedText(Strings.NoScriptPrefix);
        writer.Write("&nbsp;");
        writer.AddAttribute(HtmlTextWriterAttribute.Href, this.m_alternateUrl, true);
        writer.RenderBeginTag(HtmlTextWriterTag.A);
        writer.WriteEncodedText(Strings.Here);
        writer.RenderEndTag();
      }
    }
  }
}
