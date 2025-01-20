
using System;
using System.Collections.Specialized;
using System.Web;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal sealed class ReportServerStyleSheetOperation : HandlerOperation
  {
    private const string ParamStyleSheetName = "Name";
    private const string ParamVersion = "Version";

    public static string CreateUrl(string styleSheetName, string version, bool isImage)
    {
      ReportServerStyleSheetOperation.ValidateStyleSheetAllowed();
      UriBuilder handlerUri = ReportViewerFactory.HttpHandler.HandlerUri;
      string str = "OpType=" + (isImage ? "StyleSheetImage" : "StyleSheet") + "&Version=" + HttpUtility.UrlEncode(version);
      if (!string.IsNullOrEmpty(styleSheetName))
        str = str + "&Name=" + HttpUtility.UrlEncode(styleSheetName);
      handlerUri.Query = str;
      return handlerUri.Uri.PathAndQuery;
    }

    public override void PerformOperation(NameValueCollection urlQuery, HttpResponse response)
    {
      ReportServerStyleSheetOperation.ValidateStyleSheetAllowed();
      bool isImage = urlQuery["OpType"] == "StyleSheetImage";
      string mimeType;
      byte[] styleSheet = ReportViewerFactory.CreateReportViewer().CreateServerReport().GetStyleSheet(urlQuery["Name"], isImage, out mimeType);
      response.ContentType = mimeType;
      response.OutputStream.Write(styleSheet, 0, styleSheet.Length);
    }

    private static void ValidateStyleSheetAllowed()
    {
      if (ServerReport.RequiresConnection && WebConfigReader.Current.ServerConnection == null)
        throw new HttpHandlerInputException((Exception) new InvalidOperationException());
    }
  }
}
