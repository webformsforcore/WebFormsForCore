using System;
using System.Collections.Specialized;
using System.Text;
using System.Web;
using System.Xml;

namespace Microsoft.Reporting.WebForms;

internal sealed class PrintRequestOperation : ReportDataOperation
{
	public static string CreateQuery(Report report, string instanceID)
	{
		return ReportDataOperation.BaseQuery(report, instanceID) + "&OpType=PrintRequest";
	}

	public override void PerformOperation(NameValueCollection urlQuery, HttpResponse response)
	{
		StringBuilder stringBuilder = new StringBuilder("<DeviceInfo>");
		NameValueCollection requestParameters = HttpHandler.RequestParameters;
		NameValueCollection nameValueCollection = new NameValueCollection(1);
		for (int i = 0; i < requestParameters.Count; i++)
		{
			if (requestParameters.Keys[i] != null)
			{
				if (requestParameters.Keys[i].StartsWith("rc:", StringComparison.OrdinalIgnoreCase))
				{
					stringBuilder.AppendFormat("<{0}>{1}</{0}>", XmlConvert.EncodeName(requestParameters.Keys[i].Substring(3)), HttpUtility.HtmlEncode(requestParameters[i]));
				}
				else if (requestParameters.Keys[i].StartsWith("rs:", StringComparison.OrdinalIgnoreCase) && string.Compare(requestParameters.Keys[i], "rs:Command", StringComparison.OrdinalIgnoreCase) != 0 && string.Compare(requestParameters.Keys[i], "rs:format", StringComparison.OrdinalIgnoreCase) != 0)
				{
					nameValueCollection.Add(requestParameters.Keys[i], requestParameters[i]);
				}
			}
		}
		stringBuilder.Append("</DeviceInfo>");
		m_reportControlSession.RenderReportForPrint(stringBuilder.ToString(), nameValueCollection, response);
	}
}
