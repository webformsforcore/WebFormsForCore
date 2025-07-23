using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;

namespace Microsoft.Reporting.WebForms;

[Serializable]
[ComVisible(false)]
public sealed class DeviceInfoCollection : KeyedCollection<string, DeviceInfo>
{
	[NonSerialized]
	private DeviceInfoNameBlackList m_deviceInfoNamesBlackList = new DeviceInfoNameBlackList();

	[NonSerialized]
	private EnsureUnlocked m_ensureUnlocked;

	internal DeviceInfoNameBlackList DeviceInfoNameBlackList
	{
		get
		{
			return m_deviceInfoNamesBlackList;
		}
		set
		{
			m_deviceInfoNamesBlackList = value;
		}
	}

	internal EnsureUnlocked EnsureUnlocked
	{
		get
		{
			return m_ensureUnlocked;
		}
		set
		{
			m_ensureUnlocked = value;
		}
	}

	internal DeviceInfoCollection()
		: base((IEqualityComparer<string>)StringComparer.OrdinalIgnoreCase)
	{
	}

	public void Add(string name, string value)
	{
		Add(new DeviceInfo(name, value));
	}

	protected override void ClearItems()
	{
		ValidateUnlocked();
		base.ClearItems();
	}

	protected override string GetKeyForItem(DeviceInfo item)
	{
		return item.Name;
	}

	protected override void InsertItem(int index, DeviceInfo item)
	{
		ValidateKey(item.Name);
		base.InsertItem(index, item);
	}

	protected override void RemoveItem(int index)
	{
		ValidateUnlocked();
		base.RemoveItem(index);
	}

	protected override void SetItem(int index, DeviceInfo item)
	{
		ValidateKey(item.Name);
		base.SetItem(index, item);
	}

	private void ValidateUnlocked()
	{
		m_ensureUnlocked();
	}

	private bool ValidateKey(string key)
	{
		if (key == null)
		{
			throw new ArgumentNullException("key");
		}
		if (m_deviceInfoNamesBlackList.Contains(key))
		{
			throw new ArgumentException(m_deviceInfoNamesBlackList.GetExceptionText(key));
		}
		ValidateUnlocked();
		return true;
	}
}
