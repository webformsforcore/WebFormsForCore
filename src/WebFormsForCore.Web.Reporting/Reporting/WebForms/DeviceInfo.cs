
using System;
using System.Runtime.InteropServices;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  [ComVisible(false)]
  [Serializable]
  public sealed class DeviceInfo
  {
    private string m_name;
    private string m_value;

    public DeviceInfo(string name, string value)
    {
      if (name == null)
        throw new ArgumentNullException(nameof (name));
      if (value == null)
        throw new ArgumentNullException(nameof (value));
      this.m_name = name;
      this.m_value = value;
    }

    public string Name => this.m_name;

    public string Value => this.m_value;
  }
}
