using System;
using System.Collections.Specialized;
using System.IO;
using System.Security;
using System.Web;
using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.Interfaces;

namespace Microsoft.Reporting.WebForms;

[Serializable]
internal abstract class ReportControlSession : IDisposable
{
	protected StreamCache m_htmlStreamCache = new StreamCache();

	private CreateStreamDelegate CreateStreamCallback
	{
		get
		{
			ITemporaryStorage tempStorage = WebConfigReader.Current.TempStorage;
			if (tempStorage == null)
			{
				return null;
			}
			return () => new MemoryThenTempStorageStream(tempStorage);
		}
	}

	public abstract Report Report { get; }

	public abstract string PrintCabVersion { get; }

	public void Dispose()
	{
		m_htmlStreamCache.Dispose();
		GC.SuppressFinalize(this);
	}

	public abstract void DisposeNonSessionResources();

	public Stream RenderReportHTML4(NameValueCollection deviceInfo, PageCountMode pageCountMode, out string scrollScript, out string pageStyle)
	{
		m_htmlStreamCache.Clear();
		using StreamCache streamCache = new StreamCache(CreateStreamCallback);
		try
		{
			LocalHtmlRenderer.Render(deviceInfo, pageCountMode, this, GetStreamCallback(streamCache), out scrollScript, out pageStyle);
		}
		finally
		{
			streamCache.MoveSecondaryStreamsTo(m_htmlStreamCache);
		}
		return streamCache.GetMainStream(detach: true);
	}

	[SecurityTreatAsSafe]
	[SecurityCritical]
	private CreateAndRegisterStream GetStreamCallback(StreamCache streamCache)
	{
		return streamCache.StreamCallback;
	}

	public byte[] GetRendererImage(string streamID, out string mimeType)
	{
		return LocalHtmlRenderer.GetResource(streamID, out mimeType);
	}

	public byte[] GetStreamImage(string streamID, string deviceInfo, out string mimeType)
	{
		string encoding;
		string fileExtension;
		byte[] array = m_htmlStreamCache.GetSecondaryStream(remove: true, streamID, out encoding, out mimeType, out fileExtension);
		if (array == null)
		{
			array = Report.InternalRenderStream("RPL", streamID, deviceInfo, out mimeType, out encoding);
		}
		return array;
	}

	public abstract Stream RenderReport(string format, bool allowInternalRenderers, string deviceInfo, NameValueCollection additionalParams, bool cacheSecondaryStreamsForHtml, out string mimeType, out string fileExtension);

	public abstract void RenderReportForPrint(string deviceInfo, NameValueCollection additonalParams, HttpResponse response);

	public abstract bool IsPrintCabSupported(ClientArchitecture arch);

	public abstract void WritePrintCab(ClientArchitecture arch, Stream stream);

	public abstract string PrintCabCLSID(ClientArchitecture arch);
}
