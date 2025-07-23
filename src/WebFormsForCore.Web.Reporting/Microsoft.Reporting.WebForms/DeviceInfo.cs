using System;
using System.Runtime.InteropServices;

namespace Microsoft.Reporting.WebForms;

[Serializable]
[ComVisible(false)]
public sealed class DeviceInfo
{
	private string m_name;

	private string m_value;

	public string Name => m_name;

	public string Value => m_value;

	public DeviceInfo(string name, string value)
	{
		if (name == null)
		{
			throw new ArgumentNullException("name");
		}
		if (value == null)
		{
			throw new ArgumentNullException("value");
		}
		m_name = name;
		m_value = value;
	}
}
