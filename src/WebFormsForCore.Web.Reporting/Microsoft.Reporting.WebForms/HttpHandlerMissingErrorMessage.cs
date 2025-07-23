using System;
using System.Drawing;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Microsoft.Reporting.WebForms;

internal class HttpHandlerMissingErrorMessage : WebControl
{
	protected override HtmlTextWriterTag TagKey => HtmlTextWriterTag.Div;

	public HttpHandlerMissingErrorMessage()
	{
		BorderColor = Color.Red;
		BorderWidth = Unit.Pixel(2);
		BorderStyle = BorderStyle.Solid;
		base.Style.Add(HtmlTextWriterStyle.Padding, "10px");
		base.Style.Add(HtmlTextWriterStyle.Display, "none");
		base.Style.Add(HtmlTextWriterStyle.Overflow, "auto");
		base.Style.Add(HtmlTextWriterStyle.FontSize, ".85em");
	}

	protected override void OnPreRender(EventArgs e)
	{
		base.OnPreRender(e);
	}

	protected override void RenderContents(HtmlTextWriter writer)
	{
		writer.RenderBeginTag(HtmlTextWriterTag.H2);
		writer.Write(Errors.HandlerNotRegisteredTitle);
		writer.RenderEndTag();
		HttpHandler httpHandler = ReportViewerFactory.HttpHandler;
		string s = Errors.HandlerNotRegisteredDetails(httpHandler.LegacyHttpHandlerEntry, "system.web/httpHandlers", httpHandler.IIS7HttpHandlerEntry, "system.webServer/handlers");
		writer.RenderBeginTag(HtmlTextWriterTag.P);
		writer.Write(HttpUtility.HtmlEncode(s));
		writer.RenderEndTag();
	}
}
