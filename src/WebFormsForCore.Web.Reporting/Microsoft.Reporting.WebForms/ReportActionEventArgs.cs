using System;

namespace Microsoft.Reporting.WebForms;

internal class ReportActionEventArgs : EventArgs
{
	private string m_actionType;

	private string m_actionParam;

	public string ActionType => m_actionType;

	public string ActionParam => m_actionParam;

	internal ReportActionEventArgs(string actionType, string actionParam)
	{
		m_actionType = actionType;
		m_actionParam = actionParam;
	}
}
