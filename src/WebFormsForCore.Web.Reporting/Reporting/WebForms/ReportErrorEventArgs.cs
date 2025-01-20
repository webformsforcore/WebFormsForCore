
using System;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  public sealed class ReportErrorEventArgs : EventArgs
  {
    private Exception m_exception;
    private bool m_isHandled;

    internal ReportErrorEventArgs(Exception e) => this.m_exception = e;

    public Exception Exception => this.m_exception;

    public bool Handled
    {
      get => this.m_isHandled;
      set => this.m_isHandled = value;
    }
  }
}
