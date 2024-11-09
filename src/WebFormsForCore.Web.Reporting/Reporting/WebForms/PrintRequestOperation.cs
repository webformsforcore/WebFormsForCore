// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.PrintRequestOperation
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

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
