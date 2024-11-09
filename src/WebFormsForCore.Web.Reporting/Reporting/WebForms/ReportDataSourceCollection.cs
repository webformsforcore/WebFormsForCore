// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.ReportDataSourceCollection
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  [ComVisible(false)]
  [Serializable]
  public sealed class ReportDataSourceCollection : SyncList<ReportDataSource>, ISerializable
  {
    private EventHandler m_onChangeEventHandler;

    internal ReportDataSourceCollection(object syncObject)
      : base(syncObject)
    {
      this.m_onChangeEventHandler = new EventHandler(this.OnChange);
    }

    internal ReportDataSourceCollection(SerializationInfo info, StreamingContext context)
      : this(new object())
    {
      int int32 = info.GetInt32("Count");
      for (int index = 0; index < int32; ++index)
      {
        string str = index.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        this.Add(new ReportDataSource()
        {
          Name = info.GetString("Name" + str),
          DataSourceId = info.GetString("ID" + str)
        });
      }
    }

    [SecurityTreatAsSafe]
    [SecurityCritical]
    [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
    void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
    {
      info.AddValue("Count", this.Count);
      for (int index = 0; index < this.Count; ++index)
      {
        string str = index.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        ReportDataSource reportDataSource = this[index];
        info.AddValue("Name" + str, (object) reportDataSource.Name);
        info.AddValue("ID" + str, (object) reportDataSource.DataSourceId);
      }
    }

    public ReportDataSource this[string name]
    {
      get
      {
        foreach (ReportDataSource reportDataSource in (Collection<ReportDataSource>) this)
        {
          if (string.Compare(name, reportDataSource.Name, StringComparison.OrdinalIgnoreCase) == 0)
            return reportDataSource;
        }
        return (ReportDataSource) null;
      }
    }

    internal event EventHandler Change;

    protected override void ClearItems()
    {
      foreach (ReportDataSource reportDataSource in (Collection<ReportDataSource>) this)
        this.UnregisterItem(reportDataSource);
      base.ClearItems();
    }

    protected override void InsertItem(int index, ReportDataSource item)
    {
      if (item == null)
        throw new ArgumentNullException(nameof (item));
      base.InsertItem(index, item);
      this.RegisterItem(item);
    }

    protected override void RemoveItem(int index)
    {
      this.UnregisterItem(this[index]);
      base.RemoveItem(index);
    }

    protected override void SetItem(int index, ReportDataSource item)
    {
      if (item == null)
        throw new ArgumentNullException(nameof (item));
      this.UnregisterItem(this[index]);
      base.SetItem(index, item);
      this.RegisterItem(item);
    }

    private void RegisterItem(ReportDataSource item)
    {
      item.Changed += this.m_onChangeEventHandler;
      this.OnChange();
    }

    private void UnregisterItem(ReportDataSource item)
    {
      item.Changed -= this.m_onChangeEventHandler;
      this.OnChange();
    }

    private void OnChange()
    {
      if (this.Change == null)
        return;
      this.Change((object) this, EventArgs.Empty);
    }

    private void OnChange(object sender, EventArgs e) => this.OnChange();
  }
}
