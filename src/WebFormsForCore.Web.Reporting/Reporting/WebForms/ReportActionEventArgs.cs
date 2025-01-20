
using System;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal class ReportActionEventArgs : EventArgs
  {
    private string m_actionType;
    private string m_actionParam;

    internal ReportActionEventArgs(string actionType, string actionParam)
    {
      this.m_actionType = actionType;
      this.m_actionParam = actionParam;
    }

    public string ActionType => this.m_actionType;

    public string ActionParam => this.m_actionParam;
  }
}
