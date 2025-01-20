
using Microsoft.ReportingServices.Diagnostics.Utilities;
using System;
using System.Collections.Specialized;
using System.Text;
using System.Web;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal sealed class PrintCabOperation : ReportDataOperation
  {
    private const string UrlParamArchitecture = "Arch";

    public static string CreateUrl(
      ReportControlSession reportControlSession,
      ClientArchitecture clientArch,
      string instanceID)
    {
      UriBuilder handlerUri = ReportViewerFactory.HttpHandler.HandlerUri;
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(ReportDataOperation.BaseQuery(reportControlSession.Report, instanceID));
      stringBuilder.AppendFormat("&{0}={1}", (object) "OpType", (object) "PrintCab");
      stringBuilder.AppendFormat("&{0}={1}", (object) "Arch", (object) clientArch.ToString());
      stringBuilder.AppendFormat("#Version={0}", (object) reportControlSession.PrintCabVersion.Replace(".", ","));
      handlerUri.Query = stringBuilder.ToString();
      return handlerUri.Uri.PathAndQuery + handlerUri.Uri.Fragment;
    }

    public override void PerformOperation(NameValueCollection urlQuery, HttpResponse response)
    {
      ClientArchitecture requiredEnum = (ClientArchitecture) HandlerOperation.ParseRequiredEnum(urlQuery, "Arch", typeof (ClientArchitecture));
      response.ContentType = "application/octet-stream";
      this.m_reportControlSession.WritePrintCab(requiredEnum, response.OutputStream);
    }
  }
}
