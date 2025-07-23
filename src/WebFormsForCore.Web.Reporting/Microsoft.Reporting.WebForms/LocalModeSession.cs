using System;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using System.Web;
using Microsoft.ReportingServices.Diagnostics.Utilities;

namespace Microsoft.Reporting.WebForms;

[Serializable]
internal sealed class LocalModeSession : ReportControlSession
{
	private LocalReport m_localReport;

	public override Report Report => m_localReport;

	public override string PrintCabVersion => LocalPrintCabInfo.Version;

	public LocalModeSession()
		: this(new LocalReport())
	{
	}

	public LocalModeSession(LocalReport report)
	{
		m_localReport = report;
	}

	public override void DisposeNonSessionResources()
	{
		ReportInfo.DisposeNonSessionResources(this, null);
	}

	public override Stream RenderReport(string format, bool allowInternalRenderers, string deviceInfo, NameValueCollection additionalParams, bool cacheSecondaryStreamsForHtml, out string mimeType, out string fileNameExtension)
	{
		using StreamCache streamCache = new StreamCache();
		PageCountMode pageCountMode = PageCountMode.Estimate;
		if (additionalParams != null && string.Compare(additionalParams["rs:PageCountMode"], PageCountMode.Actual.ToString(), StringComparison.OrdinalIgnoreCase) == 0)
		{
			pageCountMode = PageCountMode.Actual;
		}
		m_localReport.InternalRender(format, allowInternalRenderers, deviceInfo, pageCountMode, streamCache.StreamCallback, out var _);
		if (cacheSecondaryStreamsForHtml)
		{
			streamCache.MoveSecondaryStreamsTo(m_htmlStreamCache);
		}
		string encoding;
		return streamCache.GetMainStream(detach: true, out encoding, out mimeType, out fileNameExtension);
	}

	public override void RenderReportForPrint(string deviceInfo, NameValueCollection additonalParams, HttpResponse response)
	{
		MemoryStream lastMemoryStream = null;
		ReportDataOperation.SetStreamingHeaders(null, response);
		m_localReport.Render("IMAGE", deviceInfo, delegate
		{
			if (!HttpContext.Current.Response.IsClientConnected)
			{
				throw new HttpException("Client disconnected");
			}
			if (lastMemoryStream != null)
			{
				SendPrintStream(lastMemoryStream, response);
				lastMemoryStream.Dispose();
				lastMemoryStream = null;
			}
			lastMemoryStream = new MemoryStream();
			return lastMemoryStream;
		}, out var _);
		SendPrintStream(lastMemoryStream, response);
		lastMemoryStream.Dispose();
		SendPrintStream(null, response);
	}

	public override bool IsPrintCabSupported(ClientArchitecture arch)
	{
		if (arch != ClientArchitecture.X86)
		{
			return arch == ClientArchitecture.X64;
		}
		return true;
	}

	public override void WritePrintCab(ClientArchitecture arch, Stream stream)
	{
		string name = "Microsoft.Reporting.WebForms." + ((arch == ClientArchitecture.X64) ? "rsclientprint-x64.cab" : "rsclientprint-x86.cab");
		Assembly executingAssembly = Assembly.GetExecutingAssembly();
		using Stream stream2 = executingAssembly.GetManifestResourceStream(name);
		byte[] array = new byte[16384];
		int num = -1;
		while (num != 0)
		{
			num = stream2.Read(array, 0, array.Length);
			stream.Write(array, 0, num);
		}
	}

	public override string PrintCabCLSID(ClientArchitecture arch)
	{
		if (arch != ClientArchitecture.X64)
		{
			return "5554DCB0-700B-498D-9B58-4E40E5814405";
		}
		return "60677965-AB8B-464f-9B04-4BA871A2F17F";
	}

	private void SendPrintStream(Stream stream, HttpResponse response)
	{
		int value = 0;
		if (stream != null)
		{
			value = (int)stream.Length;
		}
		byte[] bytes = BitConverter.GetBytes(value);
		foreach (byte value2 in bytes)
		{
			response.OutputStream.WriteByte(value2);
		}
		if (stream != null)
		{
			stream.Position = 0L;
			ReportDataOperation.StreamToResponse(stream, response);
			response.Flush();
		}
	}
}
