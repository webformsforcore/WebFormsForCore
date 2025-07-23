using System;
using System.Collections.Specialized;
using System.IO;
using System.Web;
using Microsoft.ReportingServices.Diagnostics.Utilities;

namespace Microsoft.Reporting.WebForms;

[Serializable]
internal sealed class ServerModeSession : ReportControlSession
{
	private ServerReport m_serverReport;

	public override Report Report => m_serverReport;

	public override string PrintCabVersion => m_serverReport.GetServerVersion();

	public ServerModeSession(ServerReport serverReport)
	{
		m_serverReport = serverReport;
	}

	public override void DisposeNonSessionResources()
	{
		ReportInfo.DisposeNonSessionResources(null, this);
	}

	public override Stream RenderReport(string format, bool allowInternalRenderers, string deviceInfo, NameValueCollection additionalParams, bool cacheSecondaryStreamsForHtml, out string mimeType, out string fileExtension)
	{
		Stream stream = null;
		ITemporaryStorage tempStorage = WebConfigReader.Current.TempStorage;
		if (tempStorage != null)
		{
			stream = new MemoryThenTempStorageStream(tempStorage);
			m_serverReport.Render(format, deviceInfo, additionalParams, stream, out mimeType, out fileExtension);
			stream.Position = 0L;
		}
		else
		{
			stream = m_serverReport.Render(format, deviceInfo, additionalParams, out mimeType, out fileExtension);
		}
		return stream;
	}

	public override void RenderReportForPrint(string deviceInfo, NameValueCollection additonalParams, HttpResponse response)
	{
		string mimeType;
		string fileExtension;
		using Stream data = RenderReport("IMAGE", allowInternalRenderers: false, deviceInfo, additonalParams, cacheSecondaryStreamsForHtml: false, out mimeType, out fileExtension);
		ReportDataOperation.StreamToResponse(data, mimeType, response);
	}

	public override bool IsPrintCabSupported(ClientArchitecture arch)
	{
		return m_serverReport.IsPrintCabSupported(arch);
	}

	public override void WritePrintCab(ClientArchitecture arch, Stream stream)
	{
		m_serverReport.WritePrintCab(arch, stream);
	}

	public override string PrintCabCLSID(ClientArchitecture arch)
	{
		return m_serverReport.GetPrintControlClsid(arch);
	}

	public static bool IsPrintCabSupportedByLatestVersion(ClientArchitecture arch)
	{
		if (arch != ClientArchitecture.X86)
		{
			return arch == ClientArchitecture.X64;
		}
		return true;
	}

	public static string GetPrintCabFileName(ClientArchitecture arch)
	{
		if (arch != ClientArchitecture.X64)
		{
			return ServerReport.ClientPrintCabX86Name;
		}
		return "rsclientprint-x64.cab";
	}

	public static string GetPrintControlClsid(ClientArchitecture arch)
	{
		if (arch != ClientArchitecture.X86)
		{
			return "60677965-AB8B-464f-9B04-4BA871A2F17F";
		}
		return "5554DCB0-700B-498D-9B58-4E40E5814405";
	}
}
