
using System.Xml;

#nullable disable
namespace System.Configuration
{
  internal struct StoredSetting
  {
    internal SettingsSerializeAs SerializeAs;
    internal XmlNode Value;

    internal StoredSetting(SettingsSerializeAs serializeAs, XmlNode value)
    {
      this.SerializeAs = serializeAs;
      this.Value = value;
    }
  }
}
