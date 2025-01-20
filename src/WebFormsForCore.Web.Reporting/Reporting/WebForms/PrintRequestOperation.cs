
using System;
using System.Collections.Specialized;
using System.Text;
using System.Web;
using System.Xml;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
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
      NameValueCollection additonalParams = new NameValueCollection(1);
      for (int index = 0; index < requestParameters.Count; ++index)
      {
        if (requestParameters.Keys[index] != null)
        {
          if (requestParameters.Keys[index].StartsWith("rc:", StringComparison.OrdinalIgnoreCase))
            stringBuilder.AppendFormat("<{0}>{1}</{0}>", (object) XmlConvert.EncodeName(requestParameters.Keys[index].Substring(3)), (object) HttpUtility.HtmlEncode(requestParameters[index]));
          else if (requestParameters.Keys[index].StartsWith("rs:", StringComparison.OrdinalIgnoreCase) && string.Compare(requestParameters.Keys[index], "rs:Command", StringComparison.OrdinalIgnoreCase) != 0 && string.Compare(requestParameters.Keys[index], "rs:format", StringComparison.OrdinalIgnoreCase) != 0)
            additonalParams.Add(requestParameters.Keys[index], requestParameters[index]);
        }
      }
      stringBuilder.Append("</DeviceInfo>");
      this.m_reportControlSession.RenderReportForPrint(stringBuilder.ToString(), additonalParams, response);
    }
  }
}
