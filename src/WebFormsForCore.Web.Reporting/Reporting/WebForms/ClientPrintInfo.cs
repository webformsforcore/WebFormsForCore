// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.ClientPrintInfo
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

using Microsoft.ReportingServices.Diagnostics.Utilities;
using System.Globalization;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal class ClientPrintInfo
  {
    private ReportViewer m_viewer;
    private ReportControlSession m_reportSession;
    private ClientArchitecture m_clientArch;
    private string m_instanceId;
    private string m_cabUrl;
    private string m_cabClsid;
    private Report m_report;

    public ClientPrintInfo(ReportViewer viewer)
    {
      this.m_viewer = viewer;
      ReportControlSession reportControlSession = this.m_viewer.ReportControlSession;
      this.m_reportSession = reportControlSession;
      this.m_instanceId = this.m_viewer.InstanceIdentifier;
      this.m_clientArch = BrowserDetectionUtility.GetClientArchitecture();
      this.m_report = reportControlSession.Report;
      this.m_cabUrl = PrintCabOperation.CreateUrl(this.m_reportSession, this.m_clientArch, this.m_instanceId);
      this.m_cabClsid = this.m_reportSession.PrintCabCLSID(this.m_clientArch);
    }

    public string CabUrl => this.m_cabUrl;

    public string CabClsid => this.m_cabClsid;

    public double MarginLeft
    {
      get => ClientPrintInfo.ConvertPageDimensionToMM(this.m_viewer.PageSettings.Margins.Left);
    }

    public double MarginRight
    {
      get => ClientPrintInfo.ConvertPageDimensionToMM(this.m_viewer.PageSettings.Margins.Right);
    }

    public double MarginTop
    {
      get => ClientPrintInfo.ConvertPageDimensionToMM(this.m_viewer.PageSettings.Margins.Top);
    }

    public double MarginBottom
    {
      get => ClientPrintInfo.ConvertPageDimensionToMM(this.m_viewer.PageSettings.Margins.Bottom);
    }

    public double PageHeight
    {
      get
      {
        return ClientPrintInfo.ConvertPageDimensionToMM(!this.m_viewer.PageSettings.Landscape ? this.m_viewer.PageSettings.PaperSize.Height : this.m_viewer.PageSettings.PaperSize.Width);
      }
    }

    public double PageWidth
    {
      get
      {
        return ClientPrintInfo.ConvertPageDimensionToMM(!this.m_viewer.PageSettings.Landscape ? this.m_viewer.PageSettings.PaperSize.Width : this.m_viewer.PageSettings.PaperSize.Height);
      }
    }

    public int Culture => CultureInfo.CurrentCulture.LCID;

    public int UICulture => CultureInfo.CurrentUICulture.LCID;

    public bool UseSingleRequest => this.m_report is LocalReport;

    public string PrintRequestPath => this.m_report.PrintRequestPath;

    public string PrintRequestQuery => this.m_report.CreatePrintRequestQuery(this.m_instanceId);

    public string ReportDisplayName => this.m_report.DisplayNameForUse;

    private static double ConvertPageDimensionToMM(int pageDimension)
    {
      return (double) pageDimension * 0.254;
    }
  }
}
