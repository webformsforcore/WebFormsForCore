
using System.Collections;
using System.Collections.ObjectModel;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal sealed class DataSourceCollectionWrapper : IEnumerable
  {
    private readonly ReportDataSourceCollection m_dsCollection;

    internal DataSourceCollectionWrapper(ReportDataSourceCollection dsCollection)
    {
      this.m_dsCollection = dsCollection;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      foreach (ReportDataSource ds in (Collection<ReportDataSource>) this.m_dsCollection)
        yield return (object) new DataSourceWrapper(ds);
    }
  }
}
