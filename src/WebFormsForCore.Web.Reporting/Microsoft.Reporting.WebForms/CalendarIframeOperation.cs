using System;
using System.Collections.Specialized;
using System.Threading;
using System.Web;

namespace Microsoft.Reporting.WebForms;

internal sealed class CalendarIframeOperation : HandlerOperation
{
	public static string CreateUrl()
	{
		UriBuilder handlerUri = ReportViewerFactory.HttpHandler.HandlerUri;
		handlerUri.Query = "OpType=Calendar&LCID=" + Thread.CurrentThread.CurrentCulture.LCID;
		return handlerUri.Uri.PathAndQuery;
	}

	public override void PerformOperation(NameValueCollection urlQuery, HttpResponse response)
	{
		CalendarPage handler = new CalendarPage();
		HttpContext.Current.Server.Transfer(handler, preserveForm: true);
	}
}
