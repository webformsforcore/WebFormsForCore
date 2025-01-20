
using System.Collections.Generic;
using System.Collections.ObjectModel;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal sealed class DataSourceControlCollection : List<DataSourceControl>
  {
    private DataSourceControlCollection()
    {
    }

    public static DataSourceControlCollection Create(
      ReportDataSourceInfoCollection dsInfos,
      DataSourceControl.CredentialRenderer renderer,
      IReportViewerStyles styles)
    {
      DataSourceControlCollection controlCollection = new DataSourceControlCollection();
      foreach (ReportDataSourceInfo dsInfo in (ReadOnlyCollection<ReportDataSourceInfo>) dsInfos)
        controlCollection.Add(new DataSourceControl(dsInfo, styles, renderer));
      return controlCollection;
    }

    public string[] DataSourceClientIds
    {
      get
      {
        List<string> stringList = new List<string>(this.Count);
        foreach (DataSourceControl dataSourceControl in (List<DataSourceControl>) this)
          stringList.Add(dataSourceControl.ClientID);
        return stringList.ToArray();
      }
    }
  }
}
