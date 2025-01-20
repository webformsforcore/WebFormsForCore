
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  [ComVisible(false)]
  [Serializable]
  public sealed class DeviceInfoCollection : KeyedCollection<string, DeviceInfo>
  {
    [NonSerialized]
    private DeviceInfoNameBlackList m_deviceInfoNamesBlackList = new DeviceInfoNameBlackList();
    [NonSerialized]
    private EnsureUnlocked m_ensureUnlocked;

    internal DeviceInfoCollection()
      : base((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
    }

    internal DeviceInfoNameBlackList DeviceInfoNameBlackList
    {
      get => this.m_deviceInfoNamesBlackList;
      set => this.m_deviceInfoNamesBlackList = value;
    }

    internal EnsureUnlocked EnsureUnlocked
    {
      get => this.m_ensureUnlocked;
      set => this.m_ensureUnlocked = value;
    }

    public void Add(string name, string value) => this.Add(new DeviceInfo(name, value));

    protected override void ClearItems()
    {
      this.ValidateUnlocked();
      base.ClearItems();
    }

    protected override string GetKeyForItem(DeviceInfo item) => item.Name;

    protected override void InsertItem(int index, DeviceInfo item)
    {
      this.ValidateKey(item.Name);
      base.InsertItem(index, item);
    }

    protected override void RemoveItem(int index)
    {
      this.ValidateUnlocked();
      base.RemoveItem(index);
    }

    protected override void SetItem(int index, DeviceInfo item)
    {
      this.ValidateKey(item.Name);
      base.SetItem(index, item);
    }

    private void ValidateUnlocked() => this.m_ensureUnlocked();

    private bool ValidateKey(string key)
    {
      if (key == null)
        throw new ArgumentNullException(nameof (key));
      if (this.m_deviceInfoNamesBlackList.Contains(key))
        throw new ArgumentException(this.m_deviceInfoNamesBlackList.GetExceptionText(key));
      this.ValidateUnlocked();
      return true;
    }
  }
}
