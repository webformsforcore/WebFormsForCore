using System;
using System.Globalization;
using System.Text;
using System.Web;
using System.Web.Services.Protocols;
using System.Web.UI;
using Microsoft.ReportingServices.Diagnostics;

namespace Microsoft.ReportingServices.Common;

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

	protected Exception ExceptionToDisplay => m_exception;

	protected abstract string ProductName { get; }

	protected abstract string ErrorPageTitle { get; }

	public ErrorResponseWriter(HttpResponse response, Exception exception, bool headersSet, int httpStatusCode, string httpStatusDescription, string reportServerErrorCode, string generator, int productLocaleId, int countryLocaleId, bool errorResponseAsXml)
	{
		m_response = response;
		m_exception = exception;
		m_headersSet = headersSet;
		m_httpStatusCode = httpStatusCode;
		m_httpStatusDescription = httpStatusDescription;
		m_reportServerErrorCode = reportServerErrorCode;
		m_generator = generator;
		m_productLocaleId = productLocaleId;
		m_countryLocaleId = countryLocaleId;
		m_errorResponseAsXml = errorResponseAsXml;
	}

	public void WriteError()
	{
		m_response.Clear();
		if (!m_headersSet)
		{
			try
			{
				m_response.StatusCode = m_httpStatusCode;
				if (m_httpStatusDescription == null)
				{
					m_httpStatusDescription = HttpWorkerRequest.GetStatusDescription(m_httpStatusCode);
				}
				m_response.StatusDescription = m_httpStatusDescription;
				m_response.ContentType = (m_errorResponseAsXml ? "text/xml" : "text/html");
				m_response.ContentEncoding = Encoding.UTF8;
			}
			finally
			{
				HttpContext.Current.Items.Add("HeadersSet", true);
			}
		}
		if ("rsReportServerNotActivated".Equals(m_reportServerErrorCode, StringComparison.OrdinalIgnoreCase))
		{
			m_response.AddHeader("RSNotActivated", "true");
		}
		else if ("rsReportServerDisabled".Equals(m_reportServerErrorCode, StringComparison.OrdinalIgnoreCase))
		{
			m_response.AddHeader("RSDisabled", "true");
		}
		else if ("rsEvaluationCopyExpired".Equals(m_reportServerErrorCode, StringComparison.OrdinalIgnoreCase))
		{
			m_response.AddHeader("RSExpired", "true");
		}
		if (m_errorResponseAsXml)
		{
			if (!(m_exception is SoapException ex))
			{
				throw new InvalidOperationException("SoapException required to write error response as XML");
			}
			m_response.Output.Write(ex.Detail.OuterXml);
		}
		else
		{
			WriteExceptionAsHtml();
		}
	}

	private void WriteExceptionAsHtml()
	{
		HtmlTextWriter htmlTextWriter = new HtmlTextWriter(m_response.Output);
		htmlTextWriter.RenderBeginTag(HtmlTextWriterTag.Html);
		htmlTextWriter.RenderBeginTag(HtmlTextWriterTag.Head);
		htmlTextWriter.RenderBeginTag(HtmlTextWriterTag.Title);
		htmlTextWriter.Write(HttpUtility.HtmlEncode(ProductName));
		htmlTextWriter.RenderEndTag();
		WriteMetaTag(htmlTextWriter, "Generator", m_generator);
		WriteMetaTag(htmlTextWriter, "HTTP Status", m_httpStatusCode.ToString(CultureInfo.InvariantCulture));
		WriteMetaTag(htmlTextWriter, "ProductLocaleID", m_productLocaleId.ToString(CultureInfo.InvariantCulture));
		WriteMetaTag(htmlTextWriter, "CountryLocaleID", m_countryLocaleId.ToString(CultureInfo.InvariantCulture));
		if (m_exception != null && WebRequestUtil.IsClientLocal())
		{
			WriteMetaTag(htmlTextWriter, "StackTrace", m_exception.StackTrace);
		}
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
		htmlTextWriter.AddAttribute(HtmlTextWriterAttribute.Bgcolor, "white", fEncode: false);
		htmlTextWriter.RenderBeginTag(HtmlTextWriterTag.Body);
		htmlTextWriter.RenderBeginTag(HtmlTextWriterTag.H1);
		htmlTextWriter.Write(HttpUtility.HtmlEncode(ErrorPageTitle));
		WriteHorizontalLine(htmlTextWriter);
		htmlTextWriter.RenderEndTag();
		int num = WriteHtmlErrorMessage(htmlTextWriter);
		for (int i = 0; i < num; i++)
		{
			htmlTextWriter.RenderEndTag();
		}
		WriteHorizontalLine(htmlTextWriter);
		htmlTextWriter.AddAttribute(HtmlTextWriterAttribute.Class, "ProductInfo");
		htmlTextWriter.RenderBeginTag(HtmlTextWriterTag.Span);
		htmlTextWriter.Write(HttpUtility.HtmlEncode(ProductName));
		htmlTextWriter.RenderEndTag();
		htmlTextWriter.RenderEndTag();
		htmlTextWriter.RenderEndTag();
		htmlTextWriter.Flush();
	}

	protected abstract int WriteHtmlErrorMessage(HtmlTextWriter writer);

	private static void WriteHorizontalLine(HtmlTextWriter hw)
	{
		hw.AddAttribute(HtmlTextWriterAttribute.Width, "100%", fEncode: false);
		hw.AddAttribute(HtmlTextWriterAttribute.Size, "1", fEncode: false);
		hw.AddAttribute("color", "silver", fEndode: false);
		hw.RenderBeginTag(HtmlTextWriterTag.Hr);
		hw.RenderEndTag();
	}

	private static void WriteMetaTag(HtmlTextWriter hw, string name, string content)
	{
		hw.AddAttribute(HtmlTextWriterAttribute.Name, name, fEncode: true);
		hw.AddAttribute("content", content, fEndode: true);
		hw.RenderBeginTag(HtmlTextWriterTag.Meta);
		hw.RenderEndTag();
		hw.Write("\r\n");
	}

	public static bool ShouldWriteErrorAsXml(string commandValue, string writeErrorAsXmlValue)
	{
		if ("ExecuteQuery".Equals(commandValue, StringComparison.OrdinalIgnoreCase))
		{
			return true;
		}
		return string.Compare(writeErrorAsXmlValue, "true", StringComparison.OrdinalIgnoreCase) == 0;
	}
}
