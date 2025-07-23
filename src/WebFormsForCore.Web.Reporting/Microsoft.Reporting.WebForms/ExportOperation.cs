using System;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web;
using Microsoft.ReportingServices.Common;

namespace Microsoft.Reporting.WebForms;

internal sealed class ExportOperation : ReportDataOperation
{
	private const string UrlParamFormat = "Format";

	private const string UrlParamContentDisposition = "ContentDisposition";

	private const string UrlParamFileName = "FileName";

	public static string CreateUrl(Report report, string instanceID, ContentDisposition contentDisposition)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(ReportDataOperation.BaseQuery(report, instanceID));
		stringBuilder.Append('&');
		stringBuilder.AppendFormat("{0}={1}&", "OpType", "Export");
		stringBuilder.AppendFormat("{0}={1}&", "FileName", HttpUtility.UrlEncode(report.DisplayNameForUse));
		stringBuilder.AppendFormat("{0}={1}&", "ContentDisposition", contentDisposition);
		stringBuilder.Append("Format");
		stringBuilder.Append('=');
		UriBuilder handlerUri = ReportViewerFactory.HttpHandler.HandlerUri;
		handlerUri.Query = stringBuilder.ToString();
		return handlerUri.Uri.PathAndQuery;
	}

	public override void PerformOperation(NameValueCollection urlQuery, HttpResponse response)
	{
		string andEnsureParam = HandlerOperation.GetAndEnsureParam(urlQuery, "Format");
		ContentDisposition contentDisposition = (ContentDisposition)HandlerOperation.ParseRequiredEnum(urlQuery, "ContentDisposition", typeof(ContentDisposition));
		string andEnsureParam2 = HandlerOperation.GetAndEnsureParam(urlQuery, "FileName");
		Stream stream = null;
		try
		{
			stream = m_reportControlSession.RenderReport(andEnsureParam, allowInternalRenderers: false, null, null, cacheSecondaryStreamsForHtml: false, out var mimeType, out var fileExtension);
			string text = ((contentDisposition switch
			{
				ContentDisposition.AlwaysAttachment => false, 
				ContentDisposition.AlwaysInline => true, 
				_ => string.Compare(mimeType, "text/html", StringComparison.OrdinalIgnoreCase) == 0, 
			}) ? "" : "attachment");
			string text2 = HttpResponseUtils.EncodeFileNameForMimeHeader(andEnsureParam2 + "." + fileExtension);
			string value = string.Format(CultureInfo.InvariantCulture, "{0}; filename=\"{1}\"", text, text2);
			response.AddHeader("Content-Disposition", value);
			ReportDataOperation.StreamToResponse(stream, mimeType, response);
		}
		finally
		{
			stream?.Close();
		}
	}
}
