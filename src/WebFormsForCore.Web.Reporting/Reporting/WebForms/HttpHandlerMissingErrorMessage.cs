
using System;
using System.Drawing;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal class HttpHandlerMissingErrorMessage : WebControl
  {
    public HttpHandlerMissingErrorMessage()
    {
      this.BorderColor = Color.Red;
      this.BorderWidth = Unit.Pixel(2);
      this.BorderStyle = BorderStyle.Solid;
      this.Style.Add(HtmlTextWriterStyle.Padding, "10px");
      this.Style.Add(HtmlTextWriterStyle.Display, "none");
      this.Style.Add(HtmlTextWriterStyle.Overflow, "auto");
      this.Style.Add(HtmlTextWriterStyle.FontSize, ".85em");
    }

    protected override HtmlTextWriterTag TagKey => HtmlTextWriterTag.Div;

    protected override void OnPreRender(EventArgs e) => base.OnPreRender(e);

    protected override void RenderContents(HtmlTextWriter writer)
    {
      writer.RenderBeginTag(HtmlTextWriterTag.H2);
      writer.Write(Errors.HandlerNotRegisteredTitle);
      writer.RenderEndTag();
      HttpHandler httpHandler = ReportViewerFactory.HttpHandler;
      // ISSUE: reference to a compiler-generated method
      string s = Errors.HandlerNotRegisteredDetails(httpHandler.LegacyHttpHandlerEntry, "system.web/httpHandlers", httpHandler.IIS7HttpHandlerEntry, "system.webServer/handlers");
      writer.RenderBeginTag(HtmlTextWriterTag.P);
      writer.Write(HttpUtility.HtmlEncode(s));
      writer.RenderEndTag();
    }
  }
}
