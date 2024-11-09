// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.DataSourceControlCollection
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

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
