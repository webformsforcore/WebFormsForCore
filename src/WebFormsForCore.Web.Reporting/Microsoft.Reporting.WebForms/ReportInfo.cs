using System;
using System.Drawing.Printing;

namespace Microsoft.Reporting.WebForms;

[Serializable]
internal sealed class ReportInfo : IDisposable
{
	[NonSerialized]
	public int CurrentPage;

	[NonSerialized]
	public string ScrollPosition;

	[NonSerialized]
	public PageSettings PageSettings;

	private static int PAGESETTINGS_COUNT = 3;

	private LocalModeSession m_localSession;

	private ServerModeSession m_serverSession;

	public LocalModeSession LocalSession => m_localSession;

	public ServerModeSession ServerSession => m_serverSession;

	public LocalReport LocalReport => (LocalReport)m_localSession.Report;

	public ServerReport ServerReport => (ServerReport)m_serverSession.Report;

	public ReportInfo(LocalModeSession localSession, ServerModeSession serverSession)
	{
		m_localSession = localSession;
		m_serverSession = serverSession;
	}

	public void Dispose()
	{
		m_localSession.Dispose();
		m_serverSession.Dispose();
	}

	public void DisposeNonSessionResources()
	{
		DisposeNonSessionResources(m_localSession, m_serverSession);
	}

	public static void DisposeNonSessionResources(LocalModeSession localSession, ServerModeSession serverSession)
	{
		if (localSession != null)
		{
			((LocalReport)localSession.Report).ReleaseSandboxAppDomain();
		}
	}

	public void LoadViewState(object viewStateObj)
	{
		object[] array = (object[])viewStateObj;
		CurrentPage = (int)array[0];
		if (array[1] != null)
		{
			ServerReport.LoadViewState(array[1]);
		}
		DeserializePageSettings(array[2]);
		ScrollPosition = (string)array[3];
	}

	public object SaveViewState(bool includeReport)
	{
		object[] array = new object[4] { CurrentPage, null, null, null };
		if (includeReport)
		{
			array[1] = ServerReport.SaveViewState();
		}
		array[2] = SerializePageSettings();
		array[3] = ScrollPosition;
		return array;
	}

	public void ConnectChangeEvent(EventHandler<ReportChangedEventArgs> changeHandler, InitializeDataSourcesEventHandler dataInitializationHandler)
	{
		ServerReport.Change += changeHandler;
		LocalReport.Change += changeHandler;
		LocalReport.InitializeDataSources += dataInitializationHandler;
	}

	public void DisconnectChangeEvent(EventHandler<ReportChangedEventArgs> changeHandler, InitializeDataSourcesEventHandler dataInitializationHandler, bool disconnectUserEvents)
	{
		ServerReport.Change -= changeHandler;
		LocalReport.Change -= changeHandler;
		LocalReport.InitializeDataSources -= dataInitializationHandler;
		if (disconnectUserEvents)
		{
			LocalReport.TransferEvents(null);
		}
	}

	private object SerializePageSettings()
	{
		if (PageSettings == null)
		{
			return null;
		}
		object[] array = new object[PAGESETTINGS_COUNT];
		array[0] = PageSettings.Margins;
		array[1] = PageSettings.PaperSize;
		array[2] = PageSettings.Landscape;
		return array;
	}

	private void DeserializePageSettings(object pageSettings)
	{
		if (!(pageSettings is object[] array) || array.Length != PAGESETTINGS_COUNT)
		{
			PageSettings = null;
			return;
		}
		PageSettings = new PageSettings();
		PageSettings.Margins = array[0] as Margins;
		PageSettings.PaperSize = array[1] as PaperSize;
		PageSettings.Landscape = (bool)array[2];
	}
}
