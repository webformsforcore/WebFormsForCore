// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.CalendarIframeOperation
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

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
