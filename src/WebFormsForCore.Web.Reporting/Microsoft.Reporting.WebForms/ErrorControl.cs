using System;
using System.Net.Sockets;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.Diagnostics.Utilities;

namespace Microsoft.Reporting.WebForms;

internal sealed class ErrorControl : WebControl
{
	private Exception m_exception;

	private bool m_hasException;

	private bool m_sanitizeExceptionMessages;

	public bool UseInternalPadding = true;

	public bool InheritFont;

	public bool ShowNonRSMessages = true;

	public bool HasException => m_hasException;

	protected override HtmlTextWriterTag TagKey => HtmlTextWriterTag.Div;

	public ErrorControl()
	{
		EnableViewState = false;
	}

	public ErrorControl(bool useInternalPadding)
		: this()
	{
		UseInternalPadding = useInternalPadding;
	}

	public ErrorControl(bool useInternalPadding, bool sanitizeExceptionMessages)
		: this(useInternalPadding)
	{
		m_sanitizeExceptionMessages = sanitizeExceptionMessages;
	}

	public void SetHandledException()
	{
		m_hasException = true;
	}

	public void SetException(Exception e)
	{
		if (m_exception == null)
		{
			m_exception = e;
		}
		m_hasException = true;
	}

	public void ClearException()
	{
		m_exception = null;
		m_hasException = false;
	}

	protected override void Render(HtmlTextWriter writer)
	{
		if (m_exception != null)
		{
			base.Render(writer);
		}
	}

	protected override void RenderContents(HtmlTextWriter writer)
	{
		WriteStackTrace(writer);
		Exception ex = m_exception;
		if (!(ex is ReportViewerException))
		{
			ex = ReportServerException.FromException(m_exception);
		}
		if (ex != null && ex.InnerException != null && ex is ReportServerException && string.Equals(ex.Message, ex.InnerException.Message, StringComparison.Ordinal))
		{
			ex = ex.InnerException;
		}
		int num = 0;
		Exception ex2 = ex;
		while (ex2 != null)
		{
			if (!UseInternalPadding && num == 0)
			{
				writer.AddStyleAttribute(HtmlTextWriterStyle.MarginLeft, "0px");
			}
			writer.RenderBeginTag(HtmlTextWriterTag.Ul);
			if (!InheritFont)
			{
				writer.AddStyleAttribute(HtmlTextWriterStyle.FontFamily, "Verdana");
				writer.AddStyleAttribute(HtmlTextWriterStyle.FontWeight, "normal");
				writer.AddStyleAttribute(HtmlTextWriterStyle.FontSize, "8pt");
			}
			writer.AddStyleAttribute(HtmlTextWriterStyle.Display, "inline");
			writer.RenderBeginTag(HtmlTextWriterTag.Li);
			writer.WriteEncodedText(GetExceptionMessage(ex2));
			writer.RenderEndTag();
			if (!ShouldDisplayException(ex2))
			{
				break;
			}
			ex2 = ex2.InnerException;
			num++;
		}
		for (int i = 0; i < num; i++)
		{
			writer.RenderEndTag();
		}
	}

	private string GetExceptionMessage(Exception e)
	{
		if (!ShouldDisplayException(e))
		{
			return ErrorStrings.rsErrorNotVisibleToRemoteBrowsers;
		}
		if (m_sanitizeExceptionMessages)
		{
			return SanitizeExceptionMessage(e);
		}
		return e.Message;
	}

	private bool ShouldDisplayException(Exception e)
	{
		if (ShowNonRSMessages)
		{
			return true;
		}
		if (!(e is RSException))
		{
			return e is ReportViewerException;
		}
		return true;
	}

	public static string SanitizeExceptionMessage(Exception e)
	{
		if (e is SocketException)
		{
			SocketException ex = e as SocketException;
			SocketException ex2 = new SocketException(ex.ErrorCode);
			return ex2.Message;
		}
		return e.Message;
	}

	private void WriteStackTrace(HtmlTextWriter writer)
	{
		if (WebRequestUtil.IsClientLocal() && m_exception.StackTrace != null)
		{
			writer.WriteLine("<!--");
			writer.WriteEncodedText(m_exception.StackTrace);
			writer.WriteLine("-->");
		}
	}
}
