// Decompiled with JetBrains decompiler
// Type: Microsoft.ReportingServices.Common.ErrorResponseWriter
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

using Microsoft.ReportingServices.Diagnostics;
using System;
using System.Globalization;
using System.Text;
using System.Web;
using System.Web.Services.Protocols;
using System.Web.UI;

#nullable disable
namespace Microsoft.ReportingServices.Common
{
  internal abstract class ErrorResponseWriter
  {
    private HttpResponse m_response;
    private Exception m_exception;
    private bool m_headersSet;
    private int m_httpStatusCode = 500;
    private string m_httpStatusDescription;
    private string m_reportServerErrorCode;
    private bool m_errorResponseAsXml = true;
    private string m_generator;
    private int m_productLocaleId = 1033;
    private int m_countryLocaleId = 1033;

    public ErrorResponseWriter(
      HttpResponse response,
      Exception exception,
      bool headersSet,
      int httpStatusCode,
      string httpStatusDescription,
      string reportServerErrorCode,
      string generator,
      int productLocaleId,
      int countryLocaleId,
      bool errorResponseAsXml)
    {
      this.m_response = response;
      this.m_exception = exception;
      this.m_headersSet = headersSet;
      this.m_httpStatusCode = httpStatusCode;
      this.m_httpStatusDescription = httpStatusDescription;
      this.m_reportServerErrorCode = reportServerErrorCode;
      this.m_generator = generator;
      this.m_productLocaleId = productLocaleId;
      this.m_countryLocaleId = countryLocaleId;
      this.m_errorResponseAsXml = errorResponseAsXml;
    }

    protected Exception ExceptionToDisplay => this.m_exception;

    public void WriteError()
    {
      this.m_response.Clear();
      if (!this.m_headersSet)
      {
        try
        {
          this.m_response.StatusCode = this.m_httpStatusCode;
          if (this.m_httpStatusDescription == null)
            this.m_httpStatusDescription = HttpWorkerRequest.GetStatusDescription(this.m_httpStatusCode);
          this.m_response.StatusDescription = this.m_httpStatusDescription;
          this.m_response.ContentType = this.m_errorResponseAsXml ? "text/xml" : "text/html";
          this.m_response.ContentEncoding = Encoding.UTF8;
        }
        finally
        {
          HttpContext.Current.Items.Add((object) "HeadersSet", (object) true);
        }
      }
      if ("rsReportServerNotActivated".Equals(this.m_reportServerErrorCode, StringComparison.OrdinalIgnoreCase))
        this.m_response.AddHeader("RSNotActivated", "true");
      else if ("rsReportServerDisabled".Equals(this.m_reportServerErrorCode, StringComparison.OrdinalIgnoreCase))
        this.m_response.AddHeader("RSDisabled", "true");
      else if ("rsEvaluationCopyExpired".Equals(this.m_reportServerErrorCode, StringComparison.OrdinalIgnoreCase))
        this.m_response.AddHeader("RSExpired", "true");
      if (this.m_errorResponseAsXml)
      {
        if (!(this.m_exception is SoapException exception))
          throw new InvalidOperationException("SoapException required to write error response as XML");
        this.m_response.Output.Write(exception.Detail.OuterXml);
      }
      else
        this.WriteExceptionAsHtml();
    }

    private void WriteExceptionAsHtml()
    {
      HtmlTextWriter htmlTextWriter = new HtmlTextWriter(this.m_response.Output);
      htmlTextWriter.RenderBeginTag(HtmlTextWriterTag.Html);
      htmlTextWriter.RenderBeginTag(HtmlTextWriterTag.Head);
      htmlTextWriter.RenderBeginTag(HtmlTextWriterTag.Title);
      htmlTextWriter.Write(System.Web.HttpUtility.HtmlEncode(this.ProductName));
      htmlTextWriter.RenderEndTag();
      ErrorResponseWriter.WriteMetaTag(htmlTextWriter, "Generator", this.m_generator);
      ErrorResponseWriter.WriteMetaTag(htmlTextWriter, "HTTP Status", this.m_httpStatusCode.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      ErrorResponseWriter.WriteMetaTag(htmlTextWriter, "ProductLocaleID", this.m_productLocaleId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      ErrorResponseWriter.WriteMetaTag(htmlTextWriter, "CountryLocaleID", this.m_countryLocaleId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (this.m_exception != null && WebRequestUtil.IsClientLocal())
        ErrorResponseWriter.WriteMetaTag(htmlTextWriter, "StackTrace", this.m_exception.StackTrace);
      htmlTextWriter.RenderBeginTag(HtmlTextWriterTag.Style);
      htmlTextWriter.WriteLine("BODY {FONT-FAMILY:Verdana; FONT-WEIGHT:normal; FONT-SIZE: 8pt; COLOR:black}");
      htmlTextWriter.WriteLine("H1 {FONT-FAMILY:Verdana; FONT-WEIGHT:700; FONT-SIZE:15pt}");
      htmlTextWriter.WriteLine("LI {FONT-FAMILY:Verdana; FONT-WEIGHT:normal; FONT-SIZE:8pt; DISPLAY:inline}");
      htmlTextWriter.WriteLine(".ProductInfo {FONT-FAMILY:Verdana; FONT-WEIGHT:bold; FONT-SIZE: 8pt; COLOR:gray}");
      htmlTextWriter.WriteLine("A:link {FONT-SIZE: 8pt; FONT-FAMILY:Verdana; COLOR:#3366CC; TEXT-DECORATION:none}");
      htmlTextWriter.WriteLine("A:hover {FONT-SIZE: 8pt; FONT-FAMILY:Verdana; COLOR:#FF3300; TEXT-DECORATION:underline}");
      htmlTextWriter.WriteLine("A:visited {FONT-SIZE: 8pt; FONT-FAMILY:Verdana; COLOR:#3366CC; TEXT-DECORATION:none}");
      htmlTextWriter.WriteLine("A:visited:hover {FONT-SIZE: 8pt; FONT-FAMILY:Verdana; color:#FF3300; TEXT-DECORATION:underline}");
      htmlTextWriter.RenderEndTag();
      htmlTextWriter.RenderEndTag();
      htmlTextWriter.AddAttribute(HtmlTextWriterAttribute.Bgcolor, "white", false);
      htmlTextWriter.RenderBeginTag(HtmlTextWriterTag.Body);
      htmlTextWriter.RenderBeginTag(HtmlTextWriterTag.H1);
      htmlTextWriter.Write(System.Web.HttpUtility.HtmlEncode(this.ErrorPageTitle));
      ErrorResponseWriter.WriteHorizontalLine(htmlTextWriter);
      htmlTextWriter.RenderEndTag();
      int num = this.WriteHtmlErrorMessage(htmlTextWriter);
      for (int index = 0; index < num; ++index)
        htmlTextWriter.RenderEndTag();
      ErrorResponseWriter.WriteHorizontalLine(htmlTextWriter);
      htmlTextWriter.AddAttribute(HtmlTextWriterAttribute.Class, "ProductInfo");
      htmlTextWriter.RenderBeginTag(HtmlTextWriterTag.Span);
      htmlTextWriter.Write(System.Web.HttpUtility.HtmlEncode(this.ProductName));
      htmlTextWriter.RenderEndTag();
      htmlTextWriter.RenderEndTag();
      htmlTextWriter.RenderEndTag();
      htmlTextWriter.Flush();
    }

    protected abstract string ProductName { get; }

    protected abstract string ErrorPageTitle { get; }

    protected abstract int WriteHtmlErrorMessage(HtmlTextWriter writer);

    private static void WriteHorizontalLine(HtmlTextWriter hw)
    {
      hw.AddAttribute(HtmlTextWriterAttribute.Width, "100%", false);
      hw.AddAttribute(HtmlTextWriterAttribute.Size, "1", false);
      hw.AddAttribute("color", "silver", false);
      hw.RenderBeginTag(HtmlTextWriterTag.Hr);
      hw.RenderEndTag();
    }

    private static void WriteMetaTag(HtmlTextWriter hw, string name, string content)
    {
      hw.AddAttribute(HtmlTextWriterAttribute.Name, name, true);
      hw.AddAttribute(nameof (content), content, true);
      hw.RenderBeginTag(HtmlTextWriterTag.Meta);
      hw.RenderEndTag();
      hw.Write("\r\n");
    }

    public static bool ShouldWriteErrorAsXml(string commandValue, string writeErrorAsXmlValue)
    {
      return "ExecuteQuery".Equals(commandValue, StringComparison.OrdinalIgnoreCase) || string.Compare(writeErrorAsXmlValue, "true", StringComparison.OrdinalIgnoreCase) == 0;
    }
  }
}
