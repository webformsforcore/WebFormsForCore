// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.DataSourceViewSelectCallbackEvent
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

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
