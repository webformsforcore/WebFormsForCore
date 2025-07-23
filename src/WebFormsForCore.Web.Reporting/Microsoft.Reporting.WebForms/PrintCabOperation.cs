using System;
using System.Collections.Specialized;
using System.Text;
using System.Web;
using Microsoft.ReportingServices.Diagnostics.Utilities;

namespace Microsoft.Reporting.WebForms;

internal sealed class PrintCabOperation : ReportDataOperation
{
	private const string UrlParamArchitecture = "Arch";

	public static string CreateUrl(ReportControlSession reportControlSession, ClientArchitecture clientArch, string instanceID)
	{
		UriBuilder handlerUri = ReportViewerFactory.HttpHandler.HandlerUri;
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(ReportDataOperation.BaseQuery(reportControlSession.Report, instanceID));
		stringBuilder.AppendFormat("&{0}={1}", "OpType", "PrintCab");
		stringBuilder.AppendFormat("&{0}={1}", "Arch", clientArch.ToString());
		stringBuilder.AppendFormat("#Version={0}", reportControlSession.PrintCabVersion.Replace(".", ","));
		handlerUri.Query = stringBuilder.ToString();
		return handlerUri.Uri.PathAndQuery + handlerUri.Uri.Fragment;
	}

	public override void PerformOperation(NameValueCollection urlQuery, HttpResponse response)
	{
		ClientArchitecture arch = (ClientArchitecture)HandlerOperation.ParseRequiredEnum(urlQuery, "Arch", typeof(ClientArchitecture));
		response.ContentType = "application/octet-stream";
		m_reportControlSession.WritePrintCab(arch, response.OutputStream);
	}
}
