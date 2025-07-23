using System;
using System.Collections.Specialized;
using System.IO;
using System.Security;
using System.Text;
using System.Web.UI;
using Microsoft.ReportingServices.Interfaces;
using Microsoft.ReportingServices.Rendering.HtmlRenderer;
using Microsoft.ReportingServices.Rendering.SPBProcessing;

namespace Microsoft.Reporting.WebForms;

internal static class LocalHtmlRenderer
{
	private const string m_htmlMimeType = "text/html";

	public static byte[] GetResource(string name, out string mimeType)
	{
		try
		{
			return HTMLRendererResources.GetBytes(name, out mimeType);
		}
		catch (Exception renderingException)
		{
			throw new ClientRenderingException(renderingException);
		}
	}

	internal static HtmlTextWriter CreateWriter(string streamName, string mimeType, CreateAndRegisterStream createStreamCallback, StreamOper streamOper)
	{
		Stream stream = CreateHTMLStream(streamName, mimeType, createStreamCallback, streamOper);
		HtmlTextWriter htmlTextWriter = new HtmlTextWriter(new StreamWriter(stream, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false)), string.Empty);
		htmlTextWriter.Indent = 0;
		htmlTextWriter.NewLine = null;
		return htmlTextWriter;
	}

	[SecurityCritical]
	[SecurityTreatAsSafe]
	private static Stream CreateHTMLStream(string streamName, string mimeType, CreateAndRegisterStream createStreamCallback, StreamOper streamOper)
	{
		return createStreamCallback(streamName, "html", Encoding.UTF8, mimeType, willSeek: false, streamOper);
	}

	public static void Render(NameValueCollection deviceInfo, PageCountMode pageCountMode, ReportControlSession reportControlSession, CreateAndRegisterStream streamCallback, out string scrollScript, out string pageStyle)
	{
		HtmlTextWriter htmlTextWriter = null;
		try
		{
			deviceInfo.Add("OnlyVisibleStyles", "True");
			htmlTextWriter = CreateWriter(reportControlSession.Report.DisplayNameForUse, "text/html", streamCallback, StreamOper.CreateAndRegister);
			NameValueCollection browserCaps = new NameValueCollection();
			ViewerRendererDeviceInfo viewerRendererDeviceInfo = new ViewerRendererDeviceInfo();
			viewerRendererDeviceInfo.ParseDeviceInfo(deviceInfo, new NameValueCollection());
			ViewerRenderer viewerRenderer = new ViewerRenderer(reportControlSession, streamCallback, viewerRendererDeviceInfo, browserCaps, (SecondaryStreams)1, pageCountMode);
			viewerRenderer.Render(htmlTextWriter);
			scrollScript = viewerRenderer.FixedHeaderScript;
			pageStyle = viewerRenderer.PageStyle;
		}
		catch (ReportServerException)
		{
			throw;
		}
		catch (LocalProcessingException)
		{
			throw;
		}
		catch (Exception renderingException)
		{
			throw new ClientRenderingException(renderingException);
		}
		finally
		{
			htmlTextWriter?.Flush();
		}
	}

	public static string GetStyleStreamName(int pageNumber)
	{
		try
		{
			return ViewerRenderer.GetStyleStreamName(pageNumber);
		}
		catch (Exception renderingException)
		{
			throw new ClientRenderingException(renderingException);
		}
	}
}
