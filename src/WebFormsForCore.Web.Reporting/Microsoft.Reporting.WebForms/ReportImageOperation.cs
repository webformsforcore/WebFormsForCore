using System;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Web;
using System.Xml;

namespace Microsoft.Reporting.WebForms;

internal sealed class ReportImageOperation : ReportDataOperation
{
	private const string UrlParamStreamID = "StreamID";

	private const string UrlParamResourceStreamID = "ResourceStreamID";

	private const string UrlParamIterationId = "IterationId";

	public ReportImageOperation()
		: base(requiresFullReportLoad: false)
	{
	}

	public static string CreateUrl(Report report, string instanceID, bool isResourceStreamRoot)
	{
		return CreateUrl(report, instanceID, isResourceStreamRoot, null);
	}

	private static string CreateUrl(Report report, string instanceID, bool isResourceStreamRoot, string iterationId)
	{
		string text = (isResourceStreamRoot ? "ResourceStreamID" : "StreamID");
		UriBuilder handlerUri = ReportViewerFactory.HttpHandler.HandlerUri;
		string text2 = ReportDataOperation.BaseQuery(report, instanceID) + "&OpType=ReportImage&";
		if (!isResourceStreamRoot)
		{
			if (iterationId == null)
			{
				iterationId = Guid.NewGuid().ToString("N");
			}
			text2 = text2 + "IterationId=" + HttpUtility.UrlEncode(iterationId) + "&";
		}
		text2 = text2 + text + "=";
		handlerUri.Query = text2;
		return handlerUri.Uri.PathAndQuery;
	}

	public override void PerformOperation(NameValueCollection urlQuery, HttpResponse response)
	{
		string text = urlQuery["StreamID"];
		string text2 = urlQuery["ResourceStreamID"];
		if (text != null && text.Length > 0)
		{
			string andEnsureParam = HandlerOperation.GetAndEnsureParam(urlQuery, "IterationId");
			GetStreamImage(text, response, andEnsureParam);
			return;
		}
		if (text2 != null && text2.Length > 0)
		{
			GetRendererImage(text2, response);
			return;
		}
		throw new HttpHandlerInputException(Errors.MissingUrlParameter("StreamID"));
	}

	private void GetStreamImage(string streamID, HttpResponse response, string iterationId)
	{
		StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);
		XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
		xmlTextWriter.WriteStartElement("DeviceInfo");
		string value = CreateUrl(m_reportControlSession.Report, base.InstanceID, isResourceStreamRoot: false, iterationId);
		xmlTextWriter.WriteElementString("StreamRoot", value);
		xmlTextWriter.WriteEndElement();
		string mimeType;
		byte[] streamImage = m_reportControlSession.GetStreamImage(streamID, stringWriter.ToString(), out mimeType);
		WriteBytesToResponse(streamImage, mimeType, response);
	}

	private void GetRendererImage(string resourceID, HttpResponse response)
	{
		string mimeType;
		byte[] rendererImage = m_reportControlSession.GetRendererImage(resourceID, out mimeType);
		WriteBytesToResponse(rendererImage, mimeType, response);
	}

	private void WriteBytesToResponse(byte[] bytes, string mimeType, HttpResponse response)
	{
		if (bytes != null && bytes.Length > 0)
		{
			response.ContentType = mimeType;
			response.BinaryWrite(bytes);
		}
		else
		{
			response.StatusCode = 404;
		}
	}
}
