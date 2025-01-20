
using System.ComponentModel;
using System.Runtime.InteropServices;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  [ComVisible(false)]
  public sealed class DrillthroughEventArgs : CancelEventArgs
  {
    private string m_reportPath;
    private Report m_report;

    public DrillthroughEventArgs(string reportPath, Report targetReport)
    {
      this.m_reportPath = reportPath;
      this.m_report = targetReport;
    }

    public string ReportPath => this.m_reportPath;

    public Report Report => this.m_report;
  }
}
