using System.Collections.Generic;

namespace Microsoft.Reporting.WebForms;

internal sealed class DataSourceControlCollection : List<DataSourceControl>
{
	public string[] DataSourceClientIds
	{
		get
		{
			List<string> list = new List<string>(base.Count);
			using (Enumerator enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					DataSourceControl current = enumerator.Current;
					list.Add(current.ClientID);
				}
			}
			return list.ToArray();
		}
	}

	private DataSourceControlCollection()
	{
	}

	public static DataSourceControlCollection Create(ReportDataSourceInfoCollection dsInfos, DataSourceControl.CredentialRenderer renderer, IReportViewerStyles styles)
	{
		DataSourceControlCollection dataSourceControlCollection = new DataSourceControlCollection();
		foreach (ReportDataSourceInfo dsInfo in dsInfos)
		{
			dataSourceControlCollection.Add(new DataSourceControl(dsInfo, styles, renderer));
		}
		return dataSourceControlCollection;
	}
}
