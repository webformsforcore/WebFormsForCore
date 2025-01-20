
using Microsoft.ReportingServices.Common;
using System;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal sealed class ExportOperation : ReportDataOperation
  {
    private const string UrlParamFormat = "Format";
    private const string UrlParamContentDisposition = "ContentDisposition";
    private const string UrlParamFileName = "FileName";

    public static string CreateUrl(
      Report report,
      string instanceID,
      ContentDisposition contentDisposition)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(ReportDataOperation.BaseQuery(report, instanceID));
      stringBuilder.Append('&');
      stringBuilder.AppendFormat("{0}={1}&", (object) "OpType", (object) "Export");
      stringBuilder.AppendFormat("{0}={1}&", (object) "FileName", (object) HttpUtility.UrlEncode(report.DisplayNameForUse));
      stringBuilder.AppendFormat("{0}={1}&", (object) "ContentDisposition", (object) contentDisposition);
      stringBuilder.Append("Format");
      stringBuilder.Append('=');
      UriBuilder handlerUri = ReportViewerFactory.HttpHandler.HandlerUri;
      handlerUri.Query = stringBuilder.ToString();
      return handlerUri.Uri.PathAndQuery;
    }

    public override void PerformOperation(NameValueCollection urlQuery, HttpResponse response)
    {
      string andEnsureParam1 = HandlerOperation.GetAndEnsureParam(urlQuery, "Format");
      ContentDisposition requiredEnum = (ContentDisposition) HandlerOperation.ParseRequiredEnum(urlQuery, "ContentDisposition", typeof (ContentDisposition));
      string andEnsureParam2 = HandlerOperation.GetAndEnsureParam(urlQuery, "FileName");
      Stream data = (Stream) null;
      try
      {
        string mimeType;
        string fileExtension;
        data = this.m_reportControlSession.RenderReport(andEnsureParam1, false, (string) null, (NameValueCollection) null, false, out mimeType, out fileExtension);
        bool flag;
        switch (requiredEnum)
        {
          case ContentDisposition.AlwaysInline:
            flag = true;
            break;
          case ContentDisposition.AlwaysAttachment:
            flag = false;
            break;
          default:
            flag = string.Compare(mimeType, "text/html", StringComparison.OrdinalIgnoreCase) == 0;
            break;
        }
        string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}; filename=\"{1}\"", (object) (flag ? "" : "attachment"), (object) HttpResponseUtils.EncodeFileNameForMimeHeader(andEnsureParam2 + "." + fileExtension));
        response.AddHeader("Content-Disposition", str);
        ReportDataOperation.StreamToResponse(data, mimeType, response);
      }
      finally
      {
        data?.Close();
      }
    }
  }
}
