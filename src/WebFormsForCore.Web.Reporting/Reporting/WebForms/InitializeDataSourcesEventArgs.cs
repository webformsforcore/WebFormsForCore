
using System;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal sealed class InitializeDataSourcesEventArgs : EventArgs
  {
    private ReportDataSourceCollection m_dataSources;

    internal InitializeDataSourcesEventArgs(ReportDataSourceCollection dataSources)
    {
      this.m_dataSources = dataSources;
    }

    public ReportDataSourceCollection DataSources => this.m_dataSources;
  }
}
