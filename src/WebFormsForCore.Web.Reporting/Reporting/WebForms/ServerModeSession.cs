// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.ServerModeSession
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

using Microsoft.ReportingServices.Diagnostics.Utilities;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Web;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  [Serializable]
  internal sealed class ServerModeSession : ReportControlSession
  {
    private ServerReport m_serverReport;

    public ServerModeSession(ServerReport serverReport) => this.m_serverReport = serverReport;

    public override void DisposeNonSessionResources()
    {
      ReportInfo.DisposeNonSessionResources((LocalModeSession) null, this);
    }

    public override Stream RenderReport(
      string format,
      bool allowInternalRenderers,
      string deviceInfo,
      NameValueCollection additionalParams,
      bool cacheSecondaryStreamsForHtml,
      out string mimeType,
      out string fileExtension)
    {
      ITemporaryStorage tempStorage = WebConfigReader.Current.TempStorage;
      Stream reportStream;
      if (tempStorage != null)
      {
        reportStream = (Stream) new MemoryThenTempStorageStream(tempStorage);
        this.m_serverReport.Render(format, deviceInfo, additionalParams, reportStream, out mimeType, out fileExtension);
        reportStream.Position = 0L;
      }
      else
        reportStream = this.m_serverReport.Render(format, deviceInfo, additionalParams, out mimeType, out fileExtension);
      return reportStream;
    }

    public override void RenderReportForPrint(
      string deviceInfo,
      NameValueCollection additonalParams,
      HttpResponse response)
    {
      string mimeType;
      using (Stream data = this.RenderReport("IMAGE", false, deviceInfo, additonalParams, false, out mimeType, out string _))
        ReportDataOperation.StreamToResponse(data, mimeType, response);
    }

    public override Report Report => (Report) this.m_serverReport;

    public override bool IsPrintCabSupported(ClientArchitecture arch)
    {
      return this.m_serverReport.IsPrintCabSupported(arch);
    }

    public override void WritePrintCab(ClientArchitecture arch, Stream stream)
    {
      this.m_serverReport.WritePrintCab(arch, stream);
    }

    public override string PrintCabVersion => this.m_serverReport.GetServerVersion();

    public override string PrintCabCLSID(ClientArchitecture arch)
    {
      return this.m_serverReport.GetPrintControlClsid(arch);
    }

    public static bool IsPrintCabSupportedByLatestVersion(ClientArchitecture arch)
    {
      return arch == ClientArchitecture.X86 || arch == ClientArchitecture.X64;
    }

    public static string GetPrintCabFileName(ClientArchitecture arch)
    {
      return arch != ClientArchitecture.X64 ? ServerReport.ClientPrintCabX86Name : "rsclientprint-x64.cab";
    }

    public static string GetPrintControlClsid(ClientArchitecture arch)
    {
      return arch != ClientArchitecture.X86 ? "60677965-AB8B-464f-9B04-4BA871A2F17F" : "5554DCB0-700B-498D-9B58-4E40E5814405";
    }
  }
}
