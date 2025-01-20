
using System;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal sealed class ClientCanceledStateChangeEventArgs : EventArgs
  {
    private bool m_clientCanceled;

    public ClientCanceledStateChangeEventArgs(bool clientCanceled)
    {
      this.m_clientCanceled = clientCanceled;
    }

    public bool ClientCanceled => this.m_clientCanceled;
  }
}
