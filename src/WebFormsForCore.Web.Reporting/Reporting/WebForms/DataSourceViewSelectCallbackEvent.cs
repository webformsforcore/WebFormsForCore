
using System.Collections;
using System.Threading;
using System.Web.UI;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal class DataSourceViewSelectCallbackEvent
  {
    private DataSourceView m_view;
    private IEnumerable m_data;
    private ManualResetEvent m_selectCompletionEvent;

    internal DataSourceViewSelectCallbackEvent(DataSourceView view) => this.m_view = view;

    internal IEnumerable Data => this.m_data;

    internal void InvokeSelect()
    {
      this.m_selectCompletionEvent = new ManualResetEvent(false);
      this.m_view.Select(DataSourceSelectArguments.Empty, new DataSourceViewSelectCallback(this.OnExecuteSelectCompleted));
    }

    private void OnExecuteSelectCompleted(IEnumerable data)
    {
      this.m_data = data;
      this.m_selectCompletionEvent.Set();
    }

    internal bool WaitForSelectCompleted() => this.m_selectCompletionEvent.WaitOne();
  }
}
