using System;

namespace Microsoft.Reporting.WebForms;

[Serializable]
public sealed class ReportViewerHeaderCollection : SyncList<string>
{
	internal ReportViewerHeaderCollection(object syncObject)
		: base(syncObject)
	{
	}
}
