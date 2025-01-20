
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
