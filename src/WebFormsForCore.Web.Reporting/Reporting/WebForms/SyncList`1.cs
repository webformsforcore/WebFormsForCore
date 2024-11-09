// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.SyncList`1
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

using System;
using System.Collections.ObjectModel;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  [Serializable]
  public class SyncList<TListType> : Collection<TListType>
  {
    [NonSerialized]
    private object m_syncObject;

    private SyncList()
    {
    }

    internal SyncList(object syncObject)
      : this()
    {
      this.SetSyncObject(syncObject);
    }

    internal void SetSyncObject(object syncObject) => this.m_syncObject = syncObject;

    protected override void ClearItems()
    {
      lock (this.m_syncObject)
        base.ClearItems();
    }

    protected override void InsertItem(int index, TListType item)
    {
      lock (this.m_syncObject)
        base.InsertItem(index, item);
    }

    protected override void RemoveItem(int index)
    {
      lock (this.m_syncObject)
        base.RemoveItem(index);
    }

    protected override void SetItem(int index, TListType item)
    {
      lock (this.m_syncObject)
        base.SetItem(index, item);
    }
  }
}
