using System.Configuration.Internal;
using System.Xml;

#nullable disable
namespace System.Configuration
{
  internal sealed class ConfigXmlComment : XmlComment, IConfigErrorInfo
  {
    private int _line;
    private string _filename;

    public ConfigXmlComment(string filename, int line, string comment, XmlDocument doc)
      : base(comment, doc)
    {
      this._line = line;
      this._filename = filename;
    }

    int IConfigErrorInfo.LineNumber => this._line;

    string IConfigErrorInfo.Filename => this._filename;

    public override XmlNode CloneNode(bool deep)
    {
      XmlNode xmlNode = base.CloneNode(deep);
      if (xmlNode is ConfigXmlComment configXmlComment)
      {
        configXmlComment._line = this._line;
        configXmlComment._filename = this._filename;
      }
      return xmlNode;
    }
  }
}
