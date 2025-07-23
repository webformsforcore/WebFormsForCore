using System.Globalization;
using System.Text;
using System.Web;

namespace Microsoft.Reporting.WebForms;

internal abstract class ViewerDataOperation : HandlerOperation
{
	private const string ParamIsLocalMode = "Mode";

	private const string ParamControlID = "ControlID";

	private ReportHierarchy m_reportHierarchy;

	private string m_instanceID;

	private bool m_isUsingSession;

	private ProcessingMode m_processingMode = ProcessingMode.Remote;

	protected ProcessingMode ProcessingMode => m_processingMode;

	protected string InstanceID => m_instanceID;

	protected ReportHierarchy ReportHierarchy => m_reportHierarchy;

	protected bool IsUsingSession => m_isUsingSession;

	public ViewerDataOperation()
	{
		m_instanceID = HandlerOperation.GetAndEnsureParam(HttpHandler.RequestParameters, "ControlID");
		if (HttpHandler.RequestParameters["Mode"] != null)
		{
			m_processingMode = ProcessingMode.Local;
		}
		ReportViewer reportViewer = CreateTempReportViewer();
		m_isUsingSession = reportViewer.EnsureSessionOrConfig();
		if (m_isUsingSession)
		{
			m_reportHierarchy = (ReportHierarchy)HttpContext.Current.Session[m_instanceID];
			if (m_reportHierarchy == null)
			{
				throw new AspNetSessionExpiredException();
			}
		}
	}

	private ReportViewer CreateTempReportViewer()
	{
		ReportViewer reportViewer = ReportViewerFactory.CreateReportViewer();
		reportViewer.ProcessingMode = ProcessingMode;
		return reportViewer;
	}

	protected ServerReport CreateTempServerReport()
	{
		ReportViewer reportViewer = CreateTempReportViewer();
		return reportViewer.CreateServerReport();
	}

	protected static string ViewerDataOperationQuery(bool isLocalMode, string instanceID)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "&{0}={1}", "ControlID", HttpUtility.UrlEncode(instanceID));
		if (isLocalMode)
		{
			stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "&{0}={1}", "Mode", "true");
		}
		return stringBuilder.ToString();
	}
}
