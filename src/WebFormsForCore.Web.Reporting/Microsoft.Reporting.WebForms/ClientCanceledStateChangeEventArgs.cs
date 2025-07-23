using System;

namespace Microsoft.Reporting.WebForms;

internal sealed class ClientCanceledStateChangeEventArgs : EventArgs
{
	private bool m_clientCanceled;

	public bool ClientCanceled => m_clientCanceled;

	public ClientCanceledStateChangeEventArgs(bool clientCanceled)
	{
		m_clientCanceled = clientCanceled;
	}
}
