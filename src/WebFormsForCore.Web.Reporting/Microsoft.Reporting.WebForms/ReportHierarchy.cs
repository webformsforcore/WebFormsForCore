using System;
using System.Collections.Generic;

namespace Microsoft.Reporting.WebForms;

[Serializable]
internal sealed class ReportHierarchy : Stack<ReportInfo>, IDisposable
{
	public ReportInfo MainReport
	{
		get
		{
			ReportInfo[] array = ToArray();
			return array[array.Length - 1];
		}
	}

	public ServerReport LastStaticServerReport
	{
		get
		{
			ReportInfo[] array = ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				if (!string.IsNullOrEmpty(array[i].ServerReport.ReportPath))
				{
					return array[i].ServerReport;
				}
			}
			return null;
		}
	}

	public ReportHierarchy(ServerReport serverReport)
	{
		ServerModeSession serverSession = new ServerModeSession(serverReport);
		LocalModeSession localSession = new LocalModeSession();
		ReportInfo item = new ReportInfo(localSession, serverSession);
		Push(item);
	}

	public void Dispose()
	{
		using Enumerator enumerator = GetEnumerator();
		while (enumerator.MoveNext())
		{
			ReportInfo current = enumerator.Current;
			current.Dispose();
		}
	}

	public void DisposeNonSessionResources()
	{
		using Enumerator enumerator = GetEnumerator();
		while (enumerator.MoveNext())
		{
			ReportInfo current = enumerator.Current;
			current.DisposeNonSessionResources();
		}
	}

	public void ConnectChangeEvents(EventHandler<ReportChangedEventArgs> changeHandler, InitializeDataSourcesEventHandler dataInitializationHandler)
	{
		using Enumerator enumerator = GetEnumerator();
		while (enumerator.MoveNext())
		{
			ReportInfo current = enumerator.Current;
			current.ConnectChangeEvent(changeHandler, dataInitializationHandler);
		}
	}

	public void DisconnectChangeEvents(EventHandler<ReportChangedEventArgs> changeHandler, InitializeDataSourcesEventHandler dataInitializationHandler)
	{
		using Enumerator enumerator = GetEnumerator();
		while (enumerator.MoveNext())
		{
			ReportInfo current = enumerator.Current;
			current.DisconnectChangeEvent(changeHandler, dataInitializationHandler, disconnectUserEvents: true);
		}
	}

	public void LoadViewState(object viewStateObj)
	{
		object[] array = (object[])viewStateObj;
		SyncToClientPage(array.Length);
		int num = 0;
		using Enumerator enumerator = GetEnumerator();
		while (enumerator.MoveNext())
		{
			ReportInfo current = enumerator.Current;
			current.LoadViewState(array[num++]);
		}
	}

	public object SaveViewState(bool includeReport)
	{
		object[] array = new object[base.Count];
		int num = 0;
		using Enumerator enumerator = GetEnumerator();
		while (enumerator.MoveNext())
		{
			ReportInfo current = enumerator.Current;
			array[num++] = current.SaveViewState(includeReport);
		}
		return array;
	}

	public void SyncToClientPage(int clientStackSize)
	{
		if (clientStackSize < 1)
		{
			throw new ArgumentOutOfRangeException("clientStackSize");
		}
		for (int num = base.Count; num > clientStackSize; num--)
		{
			Pop();
		}
	}
}
