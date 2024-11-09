// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.ReportViewerFactory
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

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
