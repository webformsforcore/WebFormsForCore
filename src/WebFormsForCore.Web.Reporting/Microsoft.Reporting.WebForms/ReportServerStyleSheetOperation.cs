using System;
using System.Collections.Specialized;
using System.Web;

namespace Microsoft.Reporting.WebForms;

internal sealed class ReportServerStyleSheetOperation : HandlerOperation
{
	private const string ParamStyleSheetName = "Name";

	private const string ParamVersion = "Version";

	public static string CreateUrl(string styleSheetName, string version, bool isImage)
	{
		ValidateStyleSheetAllowed();
		UriBuilder handlerUri = ReportViewerFactory.HttpHandler.HandlerUri;
		string text = (isImage ? "StyleSheetImage" : "StyleSheet");
		string text2 = "OpType=" + text + "&Version=" + HttpUtility.UrlEncode(version);
		if (!string.IsNullOrEmpty(styleSheetName))
		{
			text2 = text2 + "&Name=" + HttpUtility.UrlEncode(styleSheetName);
		}
		handlerUri.Query = text2;
		return handlerUri.Uri.PathAndQuery;
	}

	public override void PerformOperation(NameValueCollection urlQuery, HttpResponse response)
	{
		ValidateStyleSheetAllowed();
		bool isImage = urlQuery["OpType"] == "StyleSheetImage";
		ReportViewer reportViewer = ReportViewerFactory.CreateReportViewer();
		ServerReport serverReport = reportViewer.CreateServerReport();
		string mimeType;
		byte[] styleSheet = serverReport.GetStyleSheet(urlQuery["Name"], isImage, out mimeType);
		response.ContentType = mimeType;
		response.OutputStream.Write(styleSheet, 0, styleSheet.Length);
	}

	private static void ValidateStyleSheetAllowed()
	{
		if (ServerReport.RequiresConnection && WebConfigReader.Current.ServerConnection == null)
		{
			throw new HttpHandlerInputException(new InvalidOperationException());
		}
	}
}
