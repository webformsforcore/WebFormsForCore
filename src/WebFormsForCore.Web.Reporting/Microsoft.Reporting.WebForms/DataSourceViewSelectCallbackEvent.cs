using System.Collections;
using System.Threading;
using System.Web.UI;

namespace Microsoft.Reporting.WebForms;

internal class DataSourceViewSelectCallbackEvent
{
	private DataSourceView m_view;

	private IEnumerable m_data;

	private ManualResetEvent m_selectCompletionEvent;

	internal IEnumerable Data => m_data;

	internal DataSourceViewSelectCallbackEvent(DataSourceView view)
	{
		m_view = view;
	}

	internal void InvokeSelect()
	{
		m_selectCompletionEvent = new ManualResetEvent(initialState: false);
		DataSourceViewSelectCallback callback = OnExecuteSelectCompleted;
		m_view.Select(DataSourceSelectArguments.Empty, callback);
	}

	private void OnExecuteSelectCompleted(IEnumerable data)
	{
		m_data = data;
		m_selectCompletionEvent.Set();
	}

	internal bool WaitForSelectCompleted()
	{
		return m_selectCompletionEvent.WaitOne();
	}
}
