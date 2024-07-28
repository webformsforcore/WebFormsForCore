using System.Configuration.Internal;
using System.Xml;

#nullable disable
namespace System.Configuration
{
  internal sealed class ConfigXmlCDataSection : XmlCDataSection, IConfigErrorInfo
  {
    private int _line;
    private string _filename;

    public ConfigXmlCDataSection(string filename, int line, string data, XmlDocument doc)
      : base(data, doc)
    {
      this._line = line;
      this._filename = filename;
    }

    int IConfigErrorInfo.LineNumber => this._line;

    string IConfigErrorInfo.Filename => this._filename;

    public override XmlNode CloneNode(bool deep)
    {
      XmlNode xmlNode = base.CloneNode(deep);
      if (xmlNode is ConfigXmlCDataSection configXmlCdataSection)
      {
        configXmlCdataSection._line = this._line;
        configXmlCdataSection._filename = this._filename;
      }
      return xmlNode;
    }
  }
}
