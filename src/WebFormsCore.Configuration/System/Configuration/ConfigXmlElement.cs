using System.Configuration.Internal;
using System.Xml;

#nullable disable
namespace System.Configuration
{
  internal sealed class ConfigXmlElement : XmlElement, IConfigErrorInfo
  {
    private int _line;
    private string _filename;

    public ConfigXmlElement(
      string filename,
      int line,
      string prefix,
      string localName,
      string namespaceUri,
      XmlDocument doc)
      : base(prefix, localName, namespaceUri, doc)
    {
      this._line = line;
      this._filename = filename;
    }

    int IConfigErrorInfo.LineNumber => this._line;

    string IConfigErrorInfo.Filename => this._filename;

    public override XmlNode CloneNode(bool deep)
    {
      XmlNode xmlNode = base.CloneNode(deep);
      if (xmlNode is ConfigXmlElement configXmlElement)
      {
        configXmlElement._line = this._line;
        configXmlElement._filename = this._filename;
      }
      return xmlNode;
    }
  }
}
