
using System.ComponentModel;
using System.Runtime.InteropServices;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  [ComVisible(false)]
  public class ReportParametersEventArgs : CancelEventArgs
  {
    private ReportParameterCollection m_parameters;
    private bool m_autoSubmit;

    internal ReportParametersEventArgs(ReportParameterCollection parameters, bool autoSubmit)
    {
      this.m_parameters = parameters;
      this.m_autoSubmit = autoSubmit;
    }

    public ReportParameterCollection Parameters => this.m_parameters;

    public bool AutoSubmit => this.m_autoSubmit;
  }
}
