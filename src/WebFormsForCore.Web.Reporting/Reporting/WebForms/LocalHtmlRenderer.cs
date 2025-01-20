using Microsoft.ReportingServices.Interfaces;
using Microsoft.ReportingServices.Rendering.HtmlRenderer;
using Microsoft.ReportingServices.Rendering.SPBProcessing;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Security;
using System.Text;
using System.Web.UI;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal static class LocalHtmlRenderer
  {
    private const string m_htmlMimeType = "text/html";

    public static byte[] GetResource(string name, out string mimeType)
    {
      try
      {
        return HTMLRendererResources.GetBytes(name, out mimeType);
      }
      catch (Exception ex)
      {
        throw new ClientRenderingException(ex);
      }
    }

    internal static HtmlTextWriter CreateWriter(
      string streamName,
      string mimeType,
      CreateAndRegisterStream createStreamCallback,
      StreamOper streamOper)
    {
      HtmlTextWriter writer = new HtmlTextWriter((TextWriter) new StreamWriter(LocalHtmlRenderer.CreateHTMLStream(streamName, mimeType, createStreamCallback, streamOper), (Encoding) new UTF8Encoding(false)), string.Empty);
      writer.Indent = 0;
      writer.NewLine = (string) null;
      return writer;
    }

    [SecurityCritical]
    [SecurityTreatAsSafe]
    private static Stream CreateHTMLStream(
      string streamName,
      string mimeType,
      CreateAndRegisterStream createStreamCallback,
      StreamOper streamOper)
    {
      return createStreamCallback(streamName, "html", Encoding.UTF8, mimeType, false, streamOper);
    }

    public static void Render(
      NameValueCollection deviceInfo,
      PageCountMode pageCountMode,
      ReportControlSession reportControlSession,
      CreateAndRegisterStream streamCallback,
      out string scrollScript,
      out string pageStyle)
    {
      HtmlTextWriter outputWriter = (HtmlTextWriter) null;
      try
      {
        deviceInfo.Add("OnlyVisibleStyles", "True");
        outputWriter = LocalHtmlRenderer.CreateWriter(reportControlSession.Report.DisplayNameForUse, "text/html", streamCallback, StreamOper.CreateAndRegister);
        NameValueCollection browserCaps = new NameValueCollection();
        ViewerRendererDeviceInfo deviceInfo1 = new ViewerRendererDeviceInfo();
        deviceInfo1.ParseDeviceInfo(deviceInfo, new NameValueCollection());
        ViewerRenderer viewerRenderer = new ViewerRenderer(reportControlSession, streamCallback, deviceInfo1, browserCaps, (SecondaryStreams) 1, pageCountMode);
        viewerRenderer.Render(outputWriter);
        scrollScript = viewerRenderer.FixedHeaderScript;
        pageStyle = viewerRenderer.PageStyle;
      }
      catch (ReportServerException ex)
      {
        throw;
      }
      catch (LocalProcessingException ex)
      {
        throw;
      }
      catch (Exception ex)
      {
        throw new ClientRenderingException(ex);
      }
      finally
      {
        outputWriter?.Flush();
      }
    }

    public static string GetStyleStreamName(int pageNumber)
    {
      try
      {
        return ViewerRenderer.GetStyleStreamName(pageNumber);
      }
      catch (Exception ex)
      {
        throw new ClientRenderingException(ex);
      }
    }
  }
}
