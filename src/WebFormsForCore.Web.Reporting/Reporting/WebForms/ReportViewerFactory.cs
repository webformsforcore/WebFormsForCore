
#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal static class ReportViewerFactory
  {
    public const string AppSettingServerCredentials = "ReportViewerServerConnection";
    public const string AppSettingTempStorage = "ReportViewerTemporaryStorage";
    public const string AppSettingViewerMessages = "ReportViewerMessages";

    public static ReportViewer CreateReportViewer() => new ReportViewer();

    public static HttpHandler HttpHandler => new HttpHandler();
  }
}
