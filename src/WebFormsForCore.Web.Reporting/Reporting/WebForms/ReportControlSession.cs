
using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.Interfaces;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Security;
using System.Web;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  [Serializable]
  internal abstract class ReportControlSession : IDisposable
  {
    protected StreamCache m_htmlStreamCache = new StreamCache();

    public void Dispose()
    {
      this.m_htmlStreamCache.Dispose();
      GC.SuppressFinalize((object) this);
    }

    public abstract void DisposeNonSessionResources();

    public Stream RenderReportHTML4(
      NameValueCollection deviceInfo,
      PageCountMode pageCountMode,
      out string scrollScript,
      out string pageStyle)
    {
      this.m_htmlStreamCache.Clear();
      using (StreamCache streamCache = new StreamCache(this.CreateStreamCallback))
      {
        try
        {
          LocalHtmlRenderer.Render(deviceInfo, pageCountMode, this, this.GetStreamCallback(streamCache), out scrollScript, out pageStyle);
        }
        finally
        {
          streamCache.MoveSecondaryStreamsTo(this.m_htmlStreamCache);
        }
        return streamCache.GetMainStream(true);
      }
    }

    [SecurityTreatAsSafe]
    [SecurityCritical]
    private CreateAndRegisterStream GetStreamCallback(StreamCache streamCache)
    {
      return new CreateAndRegisterStream(streamCache.StreamCallback);
    }

    private CreateStreamDelegate CreateStreamCallback
    {
      get
      {
        ITemporaryStorage tempStorage = WebConfigReader.Current.TempStorage;
        return tempStorage == null ? (CreateStreamDelegate) null : (CreateStreamDelegate) (() => (Stream) new MemoryThenTempStorageStream(tempStorage));
      }
    }

    public byte[] GetRendererImage(string streamID, out string mimeType)
    {
      return LocalHtmlRenderer.GetResource(streamID, out mimeType);
    }

    public byte[] GetStreamImage(string streamID, string deviceInfo, out string mimeType)
    {
      string encoding;
      return this.m_htmlStreamCache.GetSecondaryStream(true, streamID, out encoding, out mimeType, out string _) ?? this.Report.InternalRenderStream("RPL", streamID, deviceInfo, out mimeType, out encoding);
    }

    public abstract Stream RenderReport(
      string format,
      bool allowInternalRenderers,
      string deviceInfo,
      NameValueCollection additionalParams,
      bool cacheSecondaryStreamsForHtml,
      out string mimeType,
      out string fileExtension);

    public abstract void RenderReportForPrint(
      string deviceInfo,
      NameValueCollection additonalParams,
      HttpResponse response);

    public abstract Report Report { get; }

    public abstract bool IsPrintCabSupported(ClientArchitecture arch);

    public abstract void WritePrintCab(ClientArchitecture arch, Stream stream);

    public abstract string PrintCabVersion { get; }

    public abstract string PrintCabCLSID(ClientArchitecture arch);
  }
}
