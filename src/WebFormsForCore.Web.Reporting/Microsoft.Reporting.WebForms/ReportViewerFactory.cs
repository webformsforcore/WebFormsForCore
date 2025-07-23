namespace Microsoft.Reporting.WebForms;

internal static class ReportViewerFactory
{
	public const string AppSettingServerCredentials = "ReportViewerServerConnection";

	public const string AppSettingTempStorage = "ReportViewerTemporaryStorage";

	public const string AppSettingViewerMessages = "ReportViewerMessages";

	public static HttpHandler HttpHandler => new HttpHandler();

	public static ReportViewer CreateReportViewer()
	{
		return new ReportViewer();
	}
}
