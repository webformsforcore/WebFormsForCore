
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

#nullable disable
namespace Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [XmlInclude(typeof (ExecutionInfo2))]
  [GeneratedCode("wsdl", "2.0.50727.42")]
  [DebuggerStepThrough]
  [DesignerCategory("code")]
  [XmlType(Namespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices")]
  [Serializable]
  public class ExecutionInfo
  {
    private bool hasSnapshotField;
    private bool needsProcessingField;
    private bool allowQueryExecutionField;
    private bool credentialsRequiredField;
    private bool parametersRequiredField;
    private DateTime expirationDateTimeField;
    private DateTime executionDateTimeField;
    private int numPagesField;
    private ReportParameter[] parametersField;
    private DataSourcePrompt[] dataSourcePromptsField;
    private bool hasDocumentMapField;
    private string executionIDField;
    private string reportPathField;
    private string historyIDField;
    private PageSettings reportPageSettingsField;
    private int autoRefreshIntervalField;

    public bool HasSnapshot
    {
      get => this.hasSnapshotField;
      set => this.hasSnapshotField = value;
    }

    public bool NeedsProcessing
    {
      get => this.needsProcessingField;
      set => this.needsProcessingField = value;
    }

    public bool AllowQueryExecution
    {
      get => this.allowQueryExecutionField;
      set => this.allowQueryExecutionField = value;
    }

    public bool CredentialsRequired
    {
      get => this.credentialsRequiredField;
      set => this.credentialsRequiredField = value;
    }

    public bool ParametersRequired
    {
      get => this.parametersRequiredField;
      set => this.parametersRequiredField = value;
    }

    public DateTime ExpirationDateTime
    {
      get => this.expirationDateTimeField;
      set => this.expirationDateTimeField = value;
    }

    public DateTime ExecutionDateTime
    {
      get => this.executionDateTimeField;
      set => this.executionDateTimeField = value;
    }

    public int NumPages
    {
      get => this.numPagesField;
      set => this.numPagesField = value;
    }

    public ReportParameter[] Parameters
    {
      get => this.parametersField;
      set => this.parametersField = value;
    }

    public DataSourcePrompt[] DataSourcePrompts
    {
      get => this.dataSourcePromptsField;
      set => this.dataSourcePromptsField = value;
    }

    public bool HasDocumentMap
    {
      get => this.hasDocumentMapField;
      set => this.hasDocumentMapField = value;
    }

    public string ExecutionID
    {
      get => this.executionIDField;
      set => this.executionIDField = value;
    }

    public string ReportPath
    {
      get => this.reportPathField;
      set => this.reportPathField = value;
    }

    public string HistoryID
    {
      get => this.historyIDField;
      set => this.historyIDField = value;
    }

    public PageSettings ReportPageSettings
    {
      get => this.reportPageSettingsField;
      set => this.reportPageSettingsField = value;
    }

    public int AutoRefreshInterval
    {
      get => this.autoRefreshIntervalField;
      set => this.autoRefreshIntervalField = value;
    }
  }
}
