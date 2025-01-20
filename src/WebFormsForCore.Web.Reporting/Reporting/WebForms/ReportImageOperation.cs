
using System;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Web;
using System.Xml;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal sealed class ReportImageOperation : ReportDataOperation
  {
    private const string UrlParamStreamID = "StreamID";
    private const string UrlParamResourceStreamID = "ResourceStreamID";
    private const string UrlParamIterationId = "IterationId";

    public ReportImageOperation()
      : base(false)
    {
    }

    public static string CreateUrl(Report report, string instanceID, bool isResourceStreamRoot)
    {
      return ReportImageOperation.CreateUrl(report, instanceID, isResourceStreamRoot, (string) null);
    }

    private static string CreateUrl(
      Report report,
      string instanceID,
      bool isResourceStreamRoot,
      string iterationId)
    {
      string str1 = isResourceStreamRoot ? "ResourceStreamID" : "StreamID";
      UriBuilder handlerUri = ReportViewerFactory.HttpHandler.HandlerUri;
      string str2 = ReportDataOperation.BaseQuery(report, instanceID) + "&OpType=ReportImage&";
      if (!isResourceStreamRoot)
      {
        if (iterationId == null)
          iterationId = Guid.NewGuid().ToString("N");
        str2 = str2 + "IterationId=" + HttpUtility.UrlEncode(iterationId) + "&";
      }
      string str3 = str2 + str1 + "=";
      handlerUri.Query = str3;
      return handlerUri.Uri.PathAndQuery;
    }

    public override void PerformOperation(NameValueCollection urlQuery, HttpResponse response)
    {
      string streamID = urlQuery["StreamID"];
      string resourceID = urlQuery["ResourceStreamID"];
      if (streamID != null && streamID.Length > 0)
      {
        string andEnsureParam = HandlerOperation.GetAndEnsureParam(urlQuery, "IterationId");
        this.GetStreamImage(streamID, response, andEnsureParam);
      }
      else
      {
        if (resourceID == null || resourceID.Length <= 0)
        {
          // ISSUE: reference to a compiler-generated method
          throw new HttpHandlerInputException(Errors.MissingUrlParameter("StreamID"));
        }
        this.GetRendererImage(resourceID, response);
      }
    }

    private void GetStreamImage(string streamID, HttpResponse response, string iterationId)
    {
      StringWriter w = new StringWriter((IFormatProvider) CultureInfo.InvariantCulture);
      XmlTextWriter xmlTextWriter = new XmlTextWriter((TextWriter) w);
      xmlTextWriter.WriteStartElement("DeviceInfo");
      string url = ReportImageOperation.CreateUrl(this.m_reportControlSession.Report, this.InstanceID, false, iterationId);
      xmlTextWriter.WriteElementString("StreamRoot", url);
      xmlTextWriter.WriteEndElement();
      string mimeType;
      this.WriteBytesToResponse(this.m_reportControlSession.GetStreamImage(streamID, w.ToString(), out mimeType), mimeType, response);
    }

    private void GetRendererImage(string resourceID, HttpResponse response)
    {
      string mimeType;
      this.WriteBytesToResponse(this.m_reportControlSession.GetRendererImage(resourceID, out mimeType), mimeType, response);
    }

    private void WriteBytesToResponse(byte[] bytes, string mimeType, HttpResponse response)
    {
      if (bytes != null && bytes.Length > 0)
      {
        response.ContentType = mimeType;
        response.BinaryWrite(bytes);
      }
      else
        response.StatusCode = 404;
    }
  }
}
