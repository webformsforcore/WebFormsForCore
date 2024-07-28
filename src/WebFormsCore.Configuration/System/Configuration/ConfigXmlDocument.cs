using System.Configuration.Internal;
using System.IO;
using System.Security.Permissions;
using System.Xml;

#nullable disable
namespace System.Configuration
{
  /// <summary>Wraps the corresponding <see cref="T:System.Xml.XmlDocument" /> type and also carries the necessary information for reporting file-name and line numbers.</summary>
  [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
  public sealed class ConfigXmlDocument : XmlDocument, IConfigErrorInfo
  {
    private XmlTextReader _reader;
    private int _lineOffset;
    private string _filename;

    /// <summary>Gets the configuration line number.</summary>
    /// <returns>The line number.</returns>
    int IConfigErrorInfo.LineNumber
    {
      get
      {
        if (this._reader == null)
          return 0;
        return this._lineOffset > 0 ? this._reader.LineNumber + this._lineOffset - 1 : this._reader.LineNumber;
      }
    }

    /// <summary>Gets the current node line number.</summary>
    /// <returns>The line number for the current node.</returns>
    public int LineNumber => ((IConfigErrorInfo) this).LineNumber;

    /// <summary>Gets the configuration file name.</summary>
    /// <returns>The configuration file name.</returns>
    public string Filename => ConfigurationException.SafeFilename(this._filename);

    /// <summary>Gets the configuration file name.</summary>
    /// <returns>The file name.</returns>
    string IConfigErrorInfo.Filename => this._filename;

    /// <summary>Loads the configuration file.</summary>
    /// <param name="filename">The name of the file.</param>
    public override void Load(string filename)
    {
      this._filename = filename;
      try
      {
        this._reader = new XmlTextReader(filename);
        this._reader.XmlResolver = (XmlResolver) null;
        this.Load((XmlReader) this._reader);
      }
      finally
      {
        if (this._reader != null)
        {
          this._reader.Close();
          this._reader = (XmlTextReader) null;
        }
      }
    }

    /// <summary>Loads a single configuration element.</summary>
    /// <param name="filename">The name of the file.</param>
    /// <param name="sourceReader">The source for the reader.</param>
    public void LoadSingleElement(string filename, XmlTextReader sourceReader)
    {
      this._filename = filename;
      this._lineOffset = sourceReader.LineNumber;
      string s = sourceReader.ReadOuterXml();
      try
      {
        this._reader = new XmlTextReader((TextReader) new StringReader(s), sourceReader.NameTable);
        this.Load((XmlReader) this._reader);
      }
      finally
      {
        if (this._reader != null)
        {
          this._reader.Close();
          this._reader = (XmlTextReader) null;
        }
      }
    }

    /// <summary>Creates a configuration element attribute.</summary>
    /// <param name="prefix">The prefix definition.</param>
    /// <param name="localName">The name that is used locally.</param>
    /// <param name="namespaceUri">The URL that is assigned to the namespace.</param>
    /// <returns>The <see cref="P:System.Xml.Serialization.XmlAttributes.XmlAttribute" /> attribute.</returns>
    public override XmlAttribute CreateAttribute(
      string prefix,
      string localName,
      string namespaceUri)
    {
      return (XmlAttribute) new ConfigXmlAttribute(this._filename, this.LineNumber, prefix, localName, namespaceUri, (XmlDocument) this);
    }

    /// <summary>Creates a configuration element.</summary>
    /// <param name="prefix">The prefix definition.</param>
    /// <param name="localName">The name used locally.</param>
    /// <param name="namespaceUri">The namespace for the URL.</param>
    /// <returns>The <see cref="T:System.Xml.XmlElement" /> value.</returns>
    public override XmlElement CreateElement(string prefix, string localName, string namespaceUri)
    {
      return (XmlElement) new ConfigXmlElement(this._filename, this.LineNumber, prefix, localName, namespaceUri, (XmlDocument) this);
    }

    /// <summary>Create a text node.</summary>
    /// <param name="text">The text to use.</param>
    /// <returns>The <see cref="T:System.Xml.XmlText" /> value.</returns>
    public override XmlText CreateTextNode(string text)
    {
      return (XmlText) new ConfigXmlText(this._filename, this.LineNumber, text, (XmlDocument) this);
    }

    /// <summary>Creates an XML CData section.</summary>
    /// <param name="data">The data to use.</param>
    /// <returns>The <see cref="T:System.Xml.XmlCDataSection" /> value.</returns>
    public override XmlCDataSection CreateCDataSection(string data)
    {
      return (XmlCDataSection) new ConfigXmlCDataSection(this._filename, this.LineNumber, data, (XmlDocument) this);
    }

    /// <summary>Create an XML comment.</summary>
    /// <param name="data">The comment data.</param>
    /// <returns>The <see cref="T:System.Xml.XmlComment" /> value.</returns>
    public override XmlComment CreateComment(string data)
    {
      return (XmlComment) new ConfigXmlComment(this._filename, this.LineNumber, data, (XmlDocument) this);
    }

    /// <summary>Creates white spaces.</summary>
    /// <param name="data">The data to use.</param>
    /// <returns>The <see cref="T:System.Xml.XmlSignificantWhitespace" /> value.</returns>
    public override XmlSignificantWhitespace CreateSignificantWhitespace(string data)
    {
      return (XmlSignificantWhitespace) new ConfigXmlSignificantWhitespace(this._filename, this.LineNumber, data, (XmlDocument) this);
    }

    /// <summary>Creates white space.</summary>
    /// <param name="data">The data to use.</param>
    /// <returns>The <see cref="T:System.Xml.XmlWhitespace" /> value.</returns>
    public override XmlWhitespace CreateWhitespace(string data)
    {
      return (XmlWhitespace) new ConfigXmlWhitespace(this._filename, this.LineNumber, data, (XmlDocument) this);
    }
  }
}
