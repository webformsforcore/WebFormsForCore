
using System;
using System.Collections.Specialized;
using System.Threading;
using System.Web;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal sealed class CalendarIframeOperation : HandlerOperation
  {
    public static string CreateUrl()
    {
      UriBuilder handlerUri = ReportViewerFactory.HttpHandler.HandlerUri;
      handlerUri.Query = "OpType=Calendar&LCID=" + (object) Thread.CurrentThread.CurrentCulture.LCID;
      return handlerUri.Uri.PathAndQuery;
    }

    public override void PerformOperation(NameValueCollection urlQuery, HttpResponse response)
    {
      HttpContext.Current.Server.Transfer((IHttpHandler) new CalendarPage(), true);
    }
  }
}
