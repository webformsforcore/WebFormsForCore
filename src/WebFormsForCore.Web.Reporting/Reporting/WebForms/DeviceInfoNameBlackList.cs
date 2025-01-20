
using System;
using System.Collections.Generic;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal sealed class DeviceInfoNameBlackList
  {
    private Dictionary<string, string> m_blackList = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    public void Add(string deviceInfoName) => this.Add(deviceInfoName, (string) null);

    public void Add(string deviceInfoName, string deviceInfoExceptionText)
    {
      if (deviceInfoName == null)
        throw new ArgumentNullException(nameof (deviceInfoName));
      if (this.m_blackList.ContainsKey(deviceInfoName))
        throw new ArgumentException("DeviceInfo Name already exists", nameof (deviceInfoName));
      this.m_blackList.Add(deviceInfoName, deviceInfoExceptionText);
    }

    public bool Contains(string deviceInfoName)
    {
      return deviceInfoName != null && this.m_blackList.ContainsKey(deviceInfoName);
    }

    public string GetExceptionText(string deviceInfoName)
    {
      string str = (string) null;
      // ISSUE: reference to a compiler-generated method
      return this.m_blackList.TryGetValue(deviceInfoName, out str) ? str ?? CommonStrings.DeviceInfoInternal(deviceInfoName) : (string) null;
    }
  }
}
