namespace Microsoft.Reporting.WebForms;

internal sealed class WebConfigReader
{
	private ConfigFilePropertyInterface<IReportServerConnection> m_serverConnection = new ConfigFilePropertyInterface<IReportServerConnection>("ReportViewerServerConnection", "IReportServerConnection");

	private ConfigFilePropertyInterface<ITemporaryStorage> m_tempStorage = new ConfigFilePropertyInterface<ITemporaryStorage>("ReportViewerTemporaryStorage", "ITemporaryStorage");

	private ConfigFilePropertyInterface<IReportViewerMessages> m_viewerMessages = new ConfigFilePropertyInterface<IReportViewerMessages>("ReportViewerMessages", "IReportViewerMessages");

	private static WebConfigReader m_theInstance;

	private static object m_lockObject = new object();

	public static WebConfigReader Current
	{
		get
		{
			lock (m_lockObject)
			{
				if (m_theInstance == null)
				{
					m_theInstance = new WebConfigReader();
				}
				return m_theInstance;
			}
		}
	}

	public IReportServerConnection ServerConnection => m_serverConnection.GetInstance();

	public ITemporaryStorage TempStorage => m_tempStorage.GetInstance();

	public IReportViewerMessages ViewerMessages => m_viewerMessages.GetInstance();

	private WebConfigReader()
	{
	}
}
