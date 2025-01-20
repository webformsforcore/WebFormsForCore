
using System.ComponentModel;
using System.Runtime.InteropServices;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  [ComVisible(false)]
  public sealed class BackEventArgs : CancelEventArgs
  {
    private Report m_parentReport;

    public BackEventArgs(Report parentReport) => this.m_parentReport = parentReport;

    public Report ParentReport => this.m_parentReport;
  }
}
