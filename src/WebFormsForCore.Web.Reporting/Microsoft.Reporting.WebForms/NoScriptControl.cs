using System.Web.UI;
using System.Web.UI.WebControls;

namespace Microsoft.Reporting.WebForms;

internal class NoScriptControl : WebControl
{
	private string m_alternateUrl;

	public string AlternateUrl
	{
		get
		{
			return m_alternateUrl;
		}
		set
		{
			m_alternateUrl = value;
		}
	}

	protected override HtmlTextWriterTag TagKey => HtmlTextWriterTag.Noscript;

	protected override void RenderContents(HtmlTextWriter writer)
	{
		writer.Write("&nbsp;");
		if (string.IsNullOrEmpty(m_alternateUrl))
		{
			writer.WriteEncodedText(LocalizationHelper.Current.ClientNoScript);
			return;
		}
		writer.WriteEncodedText(Strings.NoScriptPrefix);
		writer.Write("&nbsp;");
		writer.AddAttribute(HtmlTextWriterAttribute.Href, m_alternateUrl, fEncode: true);
		writer.RenderBeginTag(HtmlTextWriterTag.A);
		writer.WriteEncodedText(Strings.Here);
		writer.RenderEndTag();
	}
}
