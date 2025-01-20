
using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.Diagnostics.Utilities;
using System;
using System.Net.Sockets;
using System.Web.UI;
using System.Web.UI.WebControls;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal sealed class ErrorControl : WebControl
  {
    private Exception m_exception;
    private bool m_hasException;
    private bool m_sanitizeExceptionMessages;
    public bool UseInternalPadding = true;
    public bool InheritFont;
    public bool ShowNonRSMessages = true;

    public ErrorControl() => this.EnableViewState = false;

    public ErrorControl(bool useInternalPadding)
      : this()
    {
      this.UseInternalPadding = useInternalPadding;
    }

    public ErrorControl(bool useInternalPadding, bool sanitizeExceptionMessages)
      : this(useInternalPadding)
    {
      this.m_sanitizeExceptionMessages = sanitizeExceptionMessages;
    }

    public void SetHandledException() => this.m_hasException = true;

    public void SetException(Exception e)
    {
      if (this.m_exception == null)
        this.m_exception = e;
      this.m_hasException = true;
    }

    public void ClearException()
    {
      this.m_exception = (Exception) null;
      this.m_hasException = false;
    }

    public bool HasException => this.m_hasException;

    protected override HtmlTextWriterTag TagKey => HtmlTextWriterTag.Div;

    protected override void Render(HtmlTextWriter writer)
    {
      if (this.m_exception == null)
        return;
      base.Render(writer);
    }

    protected override void RenderContents(HtmlTextWriter writer)
    {
      this.WriteStackTrace(writer);
      Exception exception = this.m_exception;
      if (!(exception is ReportViewerException))
        exception = (Exception) ReportServerException.FromException(this.m_exception);
      if (exception != null && exception.InnerException != null && exception is ReportServerException && string.Equals(exception.Message, exception.InnerException.Message, StringComparison.Ordinal))
        exception = exception.InnerException;
      int num = 0;
      Exception e = exception;
      while (e != null)
      {
        if (!this.UseInternalPadding && num == 0)
          writer.AddStyleAttribute(HtmlTextWriterStyle.MarginLeft, "0px");
        writer.RenderBeginTag(HtmlTextWriterTag.Ul);
        if (!this.InheritFont)
        {
          writer.AddStyleAttribute(HtmlTextWriterStyle.FontFamily, "Verdana");
          writer.AddStyleAttribute(HtmlTextWriterStyle.FontWeight, "normal");
          writer.AddStyleAttribute(HtmlTextWriterStyle.FontSize, "8pt");
        }
        writer.AddStyleAttribute(HtmlTextWriterStyle.Display, "inline");
        writer.RenderBeginTag(HtmlTextWriterTag.Li);
        writer.WriteEncodedText(this.GetExceptionMessage(e));
        writer.RenderEndTag();
        if (this.ShouldDisplayException(e))
        {
          e = e.InnerException;
          ++num;
        }
        else
          break;
      }
      for (int index = 0; index < num; ++index)
        writer.RenderEndTag();
    }

    private string GetExceptionMessage(Exception e)
    {
      if (!this.ShouldDisplayException(e))
        return ErrorStrings.rsErrorNotVisibleToRemoteBrowsers;
      return this.m_sanitizeExceptionMessages ? ErrorControl.SanitizeExceptionMessage(e) : e.Message;
    }

    private bool ShouldDisplayException(Exception e)
    {
      return this.ShowNonRSMessages || e is RSException || e is ReportViewerException;
    }

    public static string SanitizeExceptionMessage(Exception e)
    {
      return e is SocketException ? new SocketException((e as SocketException).ErrorCode).Message : e.Message;
    }

    private void WriteStackTrace(HtmlTextWriter writer)
    {
      if (!WebRequestUtil.IsClientLocal() || this.m_exception.StackTrace == null)
        return;
      writer.WriteLine("<!--");
      writer.WriteEncodedText(this.m_exception.StackTrace);
      writer.WriteLine("-->");
    }
  }
}
