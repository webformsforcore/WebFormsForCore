
using System;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal sealed class ExecutionInfo
  {
    public ExecutionInfo(
      string executionId,
      string historyId,
      string reportPath,
      int numPages,
      bool hasDocumentMap,
      int autoRefreshInterval,
      bool credentialsRequired,
      bool parametersRequired,
      bool hasSnapshot,
      bool needsProcessing,
      DateTime expirationDateTime,
      bool allowQueryExecution,
      PageCountMode pageCountMode,
      ReportDataSourceInfoCollection dataSourcePrompts,
      ReportParameterInfoCollection parameters,
      ReportPageSettings pageSettings)
    {
      this.ExecutionID = executionId;
      this.HistoryID = historyId;
      this.ReportPath = reportPath;
      this.NumPages = numPages;
      this.HasDocumentMap = hasDocumentMap;
      this.AutoRefreshInterval = autoRefreshInterval;
      this.CredentialsRequired = credentialsRequired;
      this.ParametersRequired = parametersRequired;
      this.HasSnapshot = hasSnapshot;
      this.NeedsProcessing = needsProcessing;
      this.ExpirationDateTime = expirationDateTime;
      this.AllowQueryExecution = allowQueryExecution;
      this.PageCountMode = pageCountMode;
      this.DataSourcePrompts = dataSourcePrompts;
      this.Parameters = parameters;
      this.ReportPageSettings = pageSettings;
    }

    public string ExecutionID { get; private set; }

    public string HistoryID { get; private set; }

    public string ReportPath { get; private set; }

    public int NumPages { get; set; }

    public bool HasDocumentMap { get; private set; }

    public int AutoRefreshInterval { get; private set; }

    public bool CredentialsRequired { get; private set; }

    public bool ParametersRequired { get; private set; }

    public bool HasSnapshot { get; private set; }

    public bool NeedsProcessing { get; private set; }

    public DateTime ExpirationDateTime { get; private set; }

    public bool AllowQueryExecution { get; private set; }

    public PageCountMode PageCountMode { get; private set; }

    public ReportDataSourceInfoCollection DataSourcePrompts { get; private set; }

    public ReportParameterInfoCollection Parameters { get; private set; }

    public ReportPageSettings ReportPageSettings { get; private set; }
  }
}
