// Decompiled with JetBrains decompiler
// Type: System.Configuration.ConfigXmlSignificantWhitespace
// Assembly: System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 84F5A18A-F2B1-435C-B86E-09CE162E61E4
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System\v4.0_4.0.0.0__b77a5c561934e089\System.dll
// XML documentation location: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1\System.xml

using System.Configuration.Internal;
using System.Xml;

#nullable disable
namespace System.Configuration
{
  internal sealed class ConfigXmlSignificantWhitespace : XmlSignificantWhitespace, IConfigErrorInfo
  {
    private int _line;
    private string _filename;

    public ConfigXmlSignificantWhitespace(
      string filename,
      int line,
      string strData,
      XmlDocument doc)
      : base(strData, doc)
    {
      this._line = line;
      this._filename = filename;
    }

    int IConfigErrorInfo.LineNumber => this._line;

    string IConfigErrorInfo.Filename => this._filename;

    public override XmlNode CloneNode(bool deep)
    {
      XmlNode xmlNode = base.CloneNode(deep);
      if (xmlNode is ConfigXmlSignificantWhitespace significantWhitespace)
      {
        significantWhitespace._line = this._line;
        significantWhitespace._filename = this._filename;
      }
      return xmlNode;
    }
  }
}
