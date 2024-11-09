// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.DataSourceWrapper
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

using System;
using System.Collections;
using System.Web.UI;
using System.Web.UI.WebControls;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal sealed class DataSourceWrapper : IDataSource
  {
    private DataSourceViewSelectCallbackEvent m_selectCompletionEvent;
    private object m_cachedValue;
    private readonly ReportDataSource m_ds;

    internal DataSourceWrapper(ReportDataSource ds)
    {
      this.m_ds = ds;
      this.StartAsyncSelect();
    }

    string IDataSource.Name => this.m_ds.Name;

    object IDataSource.Value => this.GetIDataSource();

    private void StartAsyncSelect()
    {
      object obj = this.m_ds.Value;
      switch (obj)
      {
        case IDataSource _:
          DataSourceView view = (DataSourceView) null;
          if (!string.IsNullOrEmpty(this.m_ds.DataMember))
          {
            view = ((IDataSource) obj).GetView(this.m_ds.DataMember);
          }
          else
          {
            IEnumerator enumerator = ((IDataSource) obj).GetViewNames().GetEnumerator();
            if (enumerator != null && enumerator.MoveNext())
              view = ((IDataSource) obj).GetView(enumerator.Current as string);
          }
          // ISSUE: reference to a compiler-generated method
          this.m_selectCompletionEvent = view != null ? new DataSourceViewSelectCallbackEvent(view) : throw new InvalidOperationException(Errors.DataControl_ViewNotFound("", this.m_ds.Name));
          this.m_selectCompletionEvent.InvokeSelect();
          break;
      }
    }

    private object GetIDataSource()
    {
      if (this.m_cachedValue != null)
        return this.m_cachedValue;
      object idataSource = this.m_ds.Value;
      switch (idataSource)
      {
        case ObjectDataSource _:
          idataSource = (object) ((ObjectDataSource) idataSource).Select();
          this.m_cachedValue = idataSource;
          break;
        case IDataSource _:
          if (this.m_selectCompletionEvent != null)
          {
            this.m_selectCompletionEvent.WaitForSelectCompleted();
            idataSource = (object) this.m_selectCompletionEvent.Data;
            this.m_cachedValue = idataSource;
            break;
          }
          idataSource = (object) null;
          break;
      }
      return idataSource;
    }
  }
}
