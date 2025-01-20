using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.Interfaces;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using System.Web;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  [Serializable]
  internal sealed class LocalModeSession : ReportControlSession
  {
    private LocalReport m_localReport;

    public LocalModeSession()
      : this(new LocalReport())
    {
    }

    public LocalModeSession(LocalReport report) => this.m_localReport = report;

    public override void DisposeNonSessionResources()
    {
      ReportInfo.DisposeNonSessionResources(this, (ServerModeSession) null);
    }

    public override Stream RenderReport(
      string format,
      bool allowInternalRenderers,
      string deviceInfo,
      NameValueCollection additionalParams,
      bool cacheSecondaryStreamsForHtml,
      out string mimeType,
      out string fileNameExtension)
    {
      using (StreamCache streamCache = new StreamCache())
      {
        PageCountMode pageCountMode = PageCountMode.Estimate;
        if (additionalParams != null && string.Compare(additionalParams["rs:PageCountMode"], PageCountMode.Actual.ToString(), StringComparison.OrdinalIgnoreCase) == 0)
          pageCountMode = PageCountMode.Actual;
        this.m_localReport.InternalRender(format, allowInternalRenderers, deviceInfo, pageCountMode, new CreateAndRegisterStream(streamCache.StreamCallback), out Warning[] _);
        if (cacheSecondaryStreamsForHtml)
          streamCache.MoveSecondaryStreamsTo(this.m_htmlStreamCache);
        return streamCache.GetMainStream(true, out string _, out mimeType, out fileNameExtension);
      }
    }

    public override void RenderReportForPrint(
      string deviceInfo,
      NameValueCollection additonalParams,
      HttpResponse response)
    {
      MemoryStream lastMemoryStream = (MemoryStream) null;
      ReportDataOperation.SetStreamingHeaders((string) null, response);
      this.m_localReport.Render("IMAGE", deviceInfo, (CreateStreamCallback) ((name, extension, encoding, mimeType, willSeek) =>
      {
        if (!HttpContext.Current.Response.IsClientConnected)
          throw new HttpException("Client disconnected");
        if (lastMemoryStream != null)
        {
          this.SendPrintStream((Stream) lastMemoryStream, response);
          lastMemoryStream.Dispose();
          lastMemoryStream = (MemoryStream) null;
        }
        lastMemoryStream = new MemoryStream();
        return (Stream) lastMemoryStream;
      }), out Warning[] _);
      this.SendPrintStream((Stream) lastMemoryStream, response);
      lastMemoryStream.Dispose();
      this.SendPrintStream((Stream) null, response);
    }

    public override Report Report => (Report) this.m_localReport;

    public override bool IsPrintCabSupported(ClientArchitecture arch)
    {
      return arch == ClientArchitecture.X86 || arch == ClientArchitecture.X64;
    }

    public override void WritePrintCab(ClientArchitecture arch, Stream stream)
    {
      string name = "Microsoft.Reporting.WebForms." + (arch == ClientArchitecture.X64 ? "rsclientprint-x64.cab" : "rsclientprint-x86.cab");
      using (Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(name))
      {
        byte[] buffer = new byte[16384];
        int count = -1;
        while (count != 0)
        {
          count = manifestResourceStream.Read(buffer, 0, buffer.Length);
          stream.Write(buffer, 0, count);
        }
      }
    }

    public override string PrintCabVersion => LocalPrintCabInfo.Version;

    public override string PrintCabCLSID(ClientArchitecture arch)
    {
      return arch != ClientArchitecture.X64 ? "5554DCB0-700B-498D-9B58-4E40E5814405" : "60677965-AB8B-464f-9B04-4BA871A2F17F";
    }

    private void SendPrintStream(Stream stream, HttpResponse response)
    {
      int num1 = 0;
      if (stream != null)
        num1 = (int) stream.Length;
      foreach (byte num2 in BitConverter.GetBytes(num1))
        response.OutputStream.WriteByte(num2);
      if (stream == null)
        return;
      stream.Position = 0L;
      ReportDataOperation.StreamToResponse(stream, response);
      response.Flush();
    }
  }
}
