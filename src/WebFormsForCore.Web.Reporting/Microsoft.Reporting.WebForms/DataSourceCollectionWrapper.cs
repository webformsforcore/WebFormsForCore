using System.Collections;

namespace Microsoft.Reporting.WebForms;

internal sealed class DataSourceCollectionWrapper : IEnumerable
{
	private readonly ReportDataSourceCollection m_dsCollection;

	internal DataSourceCollectionWrapper(ReportDataSourceCollection dsCollection)
	{
		m_dsCollection = dsCollection;
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		foreach (ReportDataSource ds in m_dsCollection)
		{
			yield return new DataSourceWrapper(ds);
		}
	}
}
