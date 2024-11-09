// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.DataSourceCollectionWrapper
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

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
