using System.Globalization;
using Microsoft.ReportingServices.Diagnostics.Utilities;

namespace Microsoft.Reporting.WebForms;

internal class ClientPrintInfo
{
	private ReportViewer m_viewer;

	private ReportControlSession m_reportSession;

	private ClientArchitecture m_clientArch;

	private string m_instanceId;

	private string m_cabUrl;

	private string m_cabClsid;

	private Report m_report;

	public string CabUrl => m_cabUrl;

	public string CabClsid => m_cabClsid;

	public double MarginLeft => ConvertPageDimensionToMM(m_viewer.PageSettings.Margins.Left);

	public double MarginRight => ConvertPageDimensionToMM(m_viewer.PageSettings.Margins.Right);

	public double MarginTop => ConvertPageDimensionToMM(m_viewer.PageSettings.Margins.Top);

	public double MarginBottom => ConvertPageDimensionToMM(m_viewer.PageSettings.Margins.Bottom);

	public double PageHeight
	{
		get
		{
			int pageDimension = ((!m_viewer.PageSettings.Landscape) ? m_viewer.PageSettings.PaperSize.Height : m_viewer.PageSettings.PaperSize.Width);
			return ConvertPageDimensionToMM(pageDimension);
		}
	}

	public double PageWidth
	{
		get
		{
			int pageDimension = ((!m_viewer.PageSettings.Landscape) ? m_viewer.PageSettings.PaperSize.Width : m_viewer.PageSettings.PaperSize.Height);
			return ConvertPageDimensionToMM(pageDimension);
		}
	}

	public int Culture => CultureInfo.CurrentCulture.LCID;

	public int UICulture => CultureInfo.CurrentUICulture.LCID;

	public bool UseSingleRequest => m_report is LocalReport;

	public string PrintRequestPath => m_report.PrintRequestPath;

	public string PrintRequestQuery => m_report.CreatePrintRequestQuery(m_instanceId);

	public string ReportDisplayName => m_report.DisplayNameForUse;

	public ClientPrintInfo(ReportViewer viewer)
	{
		m_viewer = viewer;
		ReportControlSession reportControlSession = (m_reportSession = m_viewer.ReportControlSession);
		m_instanceId = m_viewer.InstanceIdentifier;
		m_clientArch = BrowserDetectionUtility.GetClientArchitecture();
		m_report = reportControlSession.Report;
		m_cabUrl = PrintCabOperation.CreateUrl(m_reportSession, m_clientArch, m_instanceId);
		m_cabClsid = m_reportSession.PrintCabCLSID(m_clientArch);
	}

	private static double ConvertPageDimensionToMM(int pageDimension)
	{
		return (double)pageDimension * 0.254;
	}
}
