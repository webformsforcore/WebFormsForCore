
using System;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal class ReportChangedEventArgs : EventArgs
  {
    private bool m_isRefreshOnly;

    public ReportChangedEventArgs()
      : this(false)
    {
    }

    public ReportChangedEventArgs(bool isRefreshOnly) => this.m_isRefreshOnly = isRefreshOnly;

    public bool IsRefreshOnly => this.m_isRefreshOnly;
  }
}
