using System;

namespace Microsoft.Reporting.WebForms;

internal class ReportChangedEventArgs : EventArgs
{
	private bool m_isRefreshOnly;

	public bool IsRefreshOnly => m_isRefreshOnly;

	public ReportChangedEventArgs()
		: this(isRefreshOnly: false)
	{
	}

	public ReportChangedEventArgs(bool isRefreshOnly)
	{
		m_isRefreshOnly = isRefreshOnly;
	}
}
