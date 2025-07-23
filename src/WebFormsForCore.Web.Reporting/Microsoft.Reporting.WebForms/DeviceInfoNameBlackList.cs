using System;
using System.Collections.Generic;

namespace Microsoft.Reporting.WebForms;

internal sealed class DeviceInfoNameBlackList
{
	private Dictionary<string, string> m_blackList = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

	public void Add(string deviceInfoName)
	{
		Add(deviceInfoName, null);
	}

	public void Add(string deviceInfoName, string deviceInfoExceptionText)
	{
		if (deviceInfoName == null)
		{
			throw new ArgumentNullException("deviceInfoName");
		}
		if (m_blackList.ContainsKey(deviceInfoName))
		{
			throw new ArgumentException("DeviceInfo Name already exists", "deviceInfoName");
		}
		m_blackList.Add(deviceInfoName, deviceInfoExceptionText);
	}

	public bool Contains(string deviceInfoName)
	{
		if (deviceInfoName == null)
		{
			return false;
		}
		return m_blackList.ContainsKey(deviceInfoName);
	}

	public string GetExceptionText(string deviceInfoName)
	{
		string value = null;
		if (m_blackList.TryGetValue(deviceInfoName, out value))
		{
			if (value != null)
			{
				return value;
			}
			return CommonStrings.DeviceInfoInternal(deviceInfoName);
		}
		return null;
	}
}
