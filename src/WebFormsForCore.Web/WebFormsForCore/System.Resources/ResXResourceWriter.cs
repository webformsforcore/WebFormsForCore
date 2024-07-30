#if !NETFRAMEWORK

using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using IFormatter = EstrellasDeEsperanza.WebFormsForCore.Serialization.Formatters.IFormatter;
using EstrellasDeEsperanza.WebFormsForCore.Serialization.Formatters.Binary;
using System.Security.Permissions;
using System.Text;
using System.Xml;
using System.Web;

#nullable disable
namespace System.Resources
{
  /// <summary>Writes resources in an XML resource (.resx) file or an output stream.</summary>
  [PermissionSet(SecurityAction.InheritanceDemand, Name = "FullTrust")]
  [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
  public class ResXResourceWriter : IResourceWriter, IDisposable
  {
    internal const string TypeStr = "type";
    internal const string NameStr = "name";
    internal const string DataStr = "data";
    internal const string MetadataStr = "metadata";
    internal const string MimeTypeStr = "mimetype";
    internal const string ValueStr = "value";
    internal const string ResHeaderStr = "resheader";
    internal const string VersionStr = "version";
    internal const string ResMimeTypeStr = "resmimetype";
    internal const string ReaderStr = "reader";
    internal const string WriterStr = "writer";
    internal const string CommentStr = "comment";
    internal const string AssemblyStr = "assembly";
    internal const string AliasStr = "alias";
    private Hashtable cachedAliases;
    private static TraceSwitch ResValueProviderSwitch = new TraceSwitch("ResX", "Debug the resource value provider");
    internal static readonly string Beta2CompatSerializedObjectMimeType = "text/microsoft-urt/psuedoml-serialized/base64";
    internal static readonly string CompatBinSerializedObjectMimeType = "text/microsoft-urt/binary-serialized/base64";
    internal static readonly string CompatSoapSerializedObjectMimeType = "text/microsoft-urt/soap-serialized/base64";
    /// <summary>Specifies the default content type for a binary object. This field is read-only.</summary>
    public static readonly string BinSerializedObjectMimeType = "application/x-microsoft.net.object.binary.base64";
    /// <summary>Specifies the content type for a SOAP object. This field is read-only.</summary>
    public static readonly string SoapSerializedObjectMimeType = "application/x-microsoft.net.object.soap.base64";
    /// <summary>Specifies the default content type for an object. This field is read-only.</summary>
    public static readonly string DefaultSerializedObjectMimeType = ResXResourceWriter.BinSerializedObjectMimeType;
    /// <summary>Specifies the default content type for a byte array object. This field is read-only.</summary>
    public static readonly string ByteArraySerializedObjectMimeType = "application/x-microsoft.net.object.bytearray.base64";
    /// <summary>Specifies the content type of an XML resource. This field is read-only.</summary>
    public static readonly string ResMimeType = "text/microsoft-resx";
    /// <summary>Specifies the version of the schema that the XML output conforms to. This field is read-only.</summary>
    public static readonly string Version = "2.0";
    /// <summary>Specifies the schema to use in writing the XML file. This field is read-only.</summary>
    public static readonly string ResourceSchema = "\r\n    <!-- \r\n    Microsoft ResX Schema \r\n    \r\n    Version " + ResXResourceWriter.Version + "\r\n    \r\n    The primary goals of this format is to allow a simple XML format \r\n    that is mostly human readable. The generation and parsing of the \r\n    various data types are done through the TypeConverter classes \r\n    associated with the data types.\r\n    \r\n    Example:\r\n    \r\n    ... ado.net/XML headers & schema ...\r\n    <resheader name=\"resmimetype\">text/microsoft-resx</resheader>\r\n    <resheader name=\"version\">" + ResXResourceWriter.Version + "</resheader>\r\n    <resheader name=\"reader\">System.Resources.ResXResourceReader, System.Windows.Forms, ...</resheader>\r\n    <resheader name=\"writer\">System.Resources.ResXResourceWriter, System.Windows.Forms, ...</resheader>\r\n    <data name=\"Name1\"><value>this is my long string</value><comment>this is a comment</comment></data>\r\n    <data name=\"Color1\" type=\"System.Drawing.Color, System.Drawing\">Blue</data>\r\n    <data name=\"Bitmap1\" mimetype=\"" + ResXResourceWriter.BinSerializedObjectMimeType + "\">\r\n        <value>[base64 mime encoded serialized .NET Framework object]</value>\r\n    </data>\r\n    <data name=\"Icon1\" type=\"System.Drawing.Icon, System.Drawing\" mimetype=\"" + ResXResourceWriter.ByteArraySerializedObjectMimeType + "\">\r\n        <value>[base64 mime encoded string representing a byte array form of the .NET Framework object]</value>\r\n        <comment>This is a comment</comment>\r\n    </data>\r\n                \r\n    There are any number of \"resheader\" rows that contain simple \r\n    name/value pairs.\r\n    \r\n    Each data row contains a name, and value. The row also contains a \r\n    type or mimetype. Type corresponds to a .NET class that support \r\n    text/value conversion through the TypeConverter architecture. \r\n    Classes that don't support this are serialized and stored with the \r\n    mimetype set.\r\n    \r\n    The mimetype is used for serialized objects, and tells the \r\n    ResXResourceReader how to depersist the object. This is currently not \r\n    extensible. For a given mimetype the value must be set accordingly:\r\n    \r\n    Note - " + ResXResourceWriter.BinSerializedObjectMimeType + " is the format \r\n    that the ResXResourceWriter will generate, however the reader can \r\n    read any of the formats listed below.\r\n    \r\n    mimetype: " + ResXResourceWriter.BinSerializedObjectMimeType + "\r\n    value   : The object must be serialized with \r\n            : System.Runtime.Serialization.Formatters.Binary.BinaryFormatter\r\n            : and then encoded with base64 encoding.\r\n    \r\n    mimetype: " + ResXResourceWriter.SoapSerializedObjectMimeType + "\r\n    value   : The object must be serialized with \r\n            : System.Runtime.Serialization.Formatters.Soap.SoapFormatter\r\n            : and then encoded with base64 encoding.\r\n\r\n    mimetype: " + ResXResourceWriter.ByteArraySerializedObjectMimeType + "\r\n    value   : The object must be serialized into a byte array \r\n            : using a System.ComponentModel.TypeConverter\r\n            : and then encoded with base64 encoding.\r\n    -->\r\n    <xsd:schema id=\"root\" xmlns=\"\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:msdata=\"urn:schemas-microsoft-com:xml-msdata\">\r\n        <xsd:import namespace=\"http://www.w3.org/XML/1998/namespace\"/>\r\n        <xsd:element name=\"root\" msdata:IsDataSet=\"true\">\r\n            <xsd:complexType>\r\n                <xsd:choice maxOccurs=\"unbounded\">\r\n                    <xsd:element name=\"metadata\">\r\n                        <xsd:complexType>\r\n                            <xsd:sequence>\r\n                            <xsd:element name=\"value\" type=\"xsd:string\" minOccurs=\"0\"/>\r\n                            </xsd:sequence>\r\n                            <xsd:attribute name=\"name\" use=\"required\" type=\"xsd:string\"/>\r\n                            <xsd:attribute name=\"type\" type=\"xsd:string\"/>\r\n                            <xsd:attribute name=\"mimetype\" type=\"xsd:string\"/>\r\n                            <xsd:attribute ref=\"xml:space\"/>                            \r\n                        </xsd:complexType>\r\n                    </xsd:element>\r\n                    <xsd:element name=\"assembly\">\r\n                      <xsd:complexType>\r\n                        <xsd:attribute name=\"alias\" type=\"xsd:string\"/>\r\n                        <xsd:attribute name=\"name\" type=\"xsd:string\"/>\r\n                      </xsd:complexType>\r\n                    </xsd:element>\r\n                    <xsd:element name=\"data\">\r\n                        <xsd:complexType>\r\n                            <xsd:sequence>\r\n                                <xsd:element name=\"value\" type=\"xsd:string\" minOccurs=\"0\" msdata:Ordinal=\"1\" />\r\n                                <xsd:element name=\"comment\" type=\"xsd:string\" minOccurs=\"0\" msdata:Ordinal=\"2\" />\r\n                            </xsd:sequence>\r\n                            <xsd:attribute name=\"name\" type=\"xsd:string\" use=\"required\" msdata:Ordinal=\"1\" />\r\n                            <xsd:attribute name=\"type\" type=\"xsd:string\" msdata:Ordinal=\"3\" />\r\n                            <xsd:attribute name=\"mimetype\" type=\"xsd:string\" msdata:Ordinal=\"4\" />\r\n                            <xsd:attribute ref=\"xml:space\"/>\r\n                        </xsd:complexType>\r\n                    </xsd:element>\r\n                    <xsd:element name=\"resheader\">\r\n                        <xsd:complexType>\r\n                            <xsd:sequence>\r\n                                <xsd:element name=\"value\" type=\"xsd:string\" minOccurs=\"0\" msdata:Ordinal=\"1\" />\r\n                            </xsd:sequence>\r\n                            <xsd:attribute name=\"name\" type=\"xsd:string\" use=\"required\" />\r\n                        </xsd:complexType>\r\n                    </xsd:element>\r\n                </xsd:choice>\r\n            </xsd:complexType>\r\n        </xsd:element>\r\n        </xsd:schema>\r\n        ";
    private IFormatter binaryFormatter = (IFormatter) new BinaryFormatter();
    private string fileName;
    private Stream stream;
    private TextWriter textWriter;
    private XmlTextWriter xmlTextWriter;
    private string basePath;
    private bool hasBeenSaved;
    private bool initialized;
    private Func<System.Type, string> typeNameConverter;

    /// <summary>Gets or sets the base path for the relative file path specified in a <see cref="T:System.Resources.ResXFileRef" /> object.</summary>
    /// <returns>A path that, if prepended to the relative file path specified in a <see cref="T:System.Resources.ResXFileRef" /> object, yields an absolute path to an XML resource file.</returns>
    public string BasePath
    {
      get => this.basePath;
      set => this.basePath = value;
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.Resources.ResXResourceWriter" /> class that writes the resources to the specified file.</summary>
    /// <param name="fileName">The output file name.</param>
    /// <exception cref="T:System.IO.DirectoryNotFoundException">The directory specified in <paramref name="filename" /> does not exist.</exception>
    public ResXResourceWriter(string fileName) => this.fileName = fileName;

    /// <summary>Initializes a new instance of the <see cref="T:System.Resources.ResXResourceWriter" /> class that writes the resources to a specified file and sets a delegate that enables resource assemblies to be written that target versions of the .NET Framework before the .NET Framework 4 by using qualified assembly names.</summary>
    /// <param name="fileName">The file to send output to.</param>
    /// <param name="typeNameConverter">The delegate that is used to target earlier versions of the .NET Framework.</param>
    /// <exception cref="T:System.IO.DirectoryNotFoundException">The directory specified in <paramref name="filename" /> does not exist.</exception>
    public ResXResourceWriter(string fileName, Func<System.Type, string> typeNameConverter)
    {
      this.fileName = fileName;
      this.typeNameConverter = typeNameConverter;
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.Resources.ResXResourceWriter" /> class that writes the resources to the specified stream object.</summary>
    /// <param name="stream">The output stream.</param>
    public ResXResourceWriter(Stream stream) => this.stream = stream;

    /// <summary>Initializes a new instance of the <see cref="T:System.Resources.ResXResourceWriter" /> class that writes the resources to a specified stream object and sets a converter delegate. This delegate enables resource assemblies to be written that target versions of the .NET Framework before the .NET Framework 4 by using qualified assembly names.</summary>
    /// <param name="stream">The stream to send the output to.</param>
    /// <param name="typeNameConverter">The delegate that is used to target earlier versions of the .NET Framework.</param>
    public ResXResourceWriter(Stream stream, Func<System.Type, string> typeNameConverter)
    {
      this.stream = stream;
      this.typeNameConverter = typeNameConverter;
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.Resources.ResXResourceWriter" /> class that writes to the specified <see cref="T:System.IO.TextWriter" /> object.</summary>
    /// <param name="textWriter">The <see cref="T:System.IO.TextWriter" /> object to send the output to.</param>
    public ResXResourceWriter(TextWriter textWriter) => this.textWriter = textWriter;

    /// <summary>Initializes a new instance of the <see cref="T:System.Resources.ResXResourceWriter" /> class that writes the resources to a specified <see cref="T:System.IO.TextWriter" /> object and sets a delegate that enables resource assemblies to be written that target versions of the .NET Framework before the .NET Framework 4 by using qualified assembly names.</summary>
    /// <param name="textWriter">The object to send output to.</param>
    /// <param name="typeNameConverter">The delegate that is used to target earlier versions of the .NET Framework.</param>
    public ResXResourceWriter(TextWriter textWriter, Func<System.Type, string> typeNameConverter)
    {
      this.textWriter = textWriter;
      this.typeNameConverter = typeNameConverter;
    }

    /// <summary>This member overrides the <see cref="M:System.Object.Finalize" /> method.</summary>
    ~ResXResourceWriter() => this.Dispose(false);

    private void InitializeWriter()
    {
      if (this.xmlTextWriter == null)
      {
        bool flag = false;
        if (this.textWriter != null)
        {
          this.textWriter.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
          flag = true;
          this.xmlTextWriter = new XmlTextWriter(this.textWriter);
        }
        else
          this.xmlTextWriter = this.stream == null ? new XmlTextWriter(this.fileName, Encoding.UTF8) : new XmlTextWriter(this.stream, Encoding.UTF8);
        this.xmlTextWriter.Formatting = Formatting.Indented;
        this.xmlTextWriter.Indentation = 2;
        if (!flag)
          this.xmlTextWriter.WriteStartDocument();
      }
      else
        this.xmlTextWriter.WriteStartDocument();
      this.xmlTextWriter.WriteStartElement("root");
      this.xmlTextWriter.WriteNode((XmlReader) new XmlTextReader((TextReader) new StringReader(ResXResourceWriter.ResourceSchema))
      {
        WhitespaceHandling = WhitespaceHandling.None
      }, true);
      this.xmlTextWriter.WriteStartElement("resheader");
      this.xmlTextWriter.WriteAttributeString("name", "resmimetype");
      this.xmlTextWriter.WriteStartElement("value");
      this.xmlTextWriter.WriteString(ResXResourceWriter.ResMimeType);
      this.xmlTextWriter.WriteEndElement();
      this.xmlTextWriter.WriteEndElement();
      this.xmlTextWriter.WriteStartElement("resheader");
      this.xmlTextWriter.WriteAttributeString("name", "version");
      this.xmlTextWriter.WriteStartElement("value");
      this.xmlTextWriter.WriteString(ResXResourceWriter.Version);
      this.xmlTextWriter.WriteEndElement();
      this.xmlTextWriter.WriteEndElement();
      this.xmlTextWriter.WriteStartElement("resheader");
      this.xmlTextWriter.WriteAttributeString("name", "reader");
      this.xmlTextWriter.WriteStartElement("value");
      this.xmlTextWriter.WriteString(MultitargetUtil.GetAssemblyQualifiedName(typeof (ResXResourceReader), this.typeNameConverter));
      this.xmlTextWriter.WriteEndElement();
      this.xmlTextWriter.WriteEndElement();
      this.xmlTextWriter.WriteStartElement("resheader");
      this.xmlTextWriter.WriteAttributeString("name", "writer");
      this.xmlTextWriter.WriteStartElement("value");
      this.xmlTextWriter.WriteString(MultitargetUtil.GetAssemblyQualifiedName(typeof (ResXResourceWriter), this.typeNameConverter));
      this.xmlTextWriter.WriteEndElement();
      this.xmlTextWriter.WriteEndElement();
      this.initialized = true;
    }

    private XmlWriter Writer
    {
      get
      {
        if (!this.initialized)
          this.InitializeWriter();
        return (XmlWriter) this.xmlTextWriter;
      }
    }

    /// <summary>Adds the specified alias to a list of aliases.</summary>
    /// <param name="aliasName">The name of the alias.</param>
    /// <param name="assemblyName">The name of the assembly represented by <paramref name="aliasName" />.</param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="assemblyName" /> is <see langword="null" />.</exception>
    public virtual void AddAlias(string aliasName, AssemblyName assemblyName)
    {
      if (assemblyName == null)
        throw new ArgumentNullException(nameof (assemblyName));
      if (this.cachedAliases == null)
        this.cachedAliases = new Hashtable();
      this.cachedAliases[(object) assemblyName.FullName] = (object) aliasName;
    }

    /// <summary>Adds a design-time property whose value is specifed as a byte array to the list of resources to write.</summary>
    /// <param name="name">The name of a property.</param>
    /// <param name="value">A byte array containing the value of the property to add.</param>
    /// <exception cref="T:System.InvalidOperationException">The resource specified by the <paramref name="name" /> parameter has already been added.</exception>
    public void AddMetadata(string name, byte[] value) => this.AddDataRow("metadata", name, value);

    /// <summary>Adds a design-time property whose value is specified as a string to the list of resources to write.</summary>
    /// <param name="name">The name of a property.</param>
    /// <param name="value">A string that is the value of the property to add.</param>
    /// <exception cref="T:System.InvalidOperationException">The resource specified by the <paramref name="name" /> property has already been added.</exception>
    public void AddMetadata(string name, string value) => this.AddDataRow("metadata", name, value);

    /// <summary>Adds a design-time property whose value is specified as an object to the list of resources to write.</summary>
    /// <param name="name">The name of a property.</param>
    /// <param name="value">An object that is the value of the property to add.</param>
    /// <exception cref="T:System.InvalidOperationException">The resource specified by the <paramref name="name" /> parameter has already been added.</exception>
    public void AddMetadata(string name, object value) => this.AddDataRow("metadata", name, value);

    /// <summary>Adds a named resource specified as a byte array to the list of resources to write.</summary>
    /// <param name="name">The name of the resource.</param>
    /// <param name="value">The value of the resource to add as an 8-bit unsigned integer array.</param>
    public void AddResource(string name, byte[] value) => this.AddDataRow("data", name, value);

    /// <summary>Adds a named resource specified as an object to the list of resources to write.</summary>
    /// <param name="name">The name of the resource.</param>
    /// <param name="value">The value of the resource.</param>
    public void AddResource(string name, object value)
    {
      if (value is ResXDataNode)
        this.AddResource((ResXDataNode) value);
      else
        this.AddDataRow("data", name, value);
    }

    /// <summary>Adds a string resource to the resources.</summary>
    /// <param name="name">The name of the resource.</param>
    /// <param name="value">The value of the resource.</param>
    public void AddResource(string name, string value) => this.AddDataRow("data", name, value);

    /// <summary>Adds a named resource specified in a <see cref="T:System.Resources.ResXDataNode" /> object to the list of resources to write.</summary>
    /// <param name="node">A <see cref="T:System.Resources.ResXDataNode" /> object that contains a resource name/value pair.</param>
    public void AddResource(ResXDataNode node)
    {
      ResXDataNode resXdataNode = node.DeepClone();
      ResXFileRef fileRef = resXdataNode.FileRef;
      string basePath = this.BasePath;
      if (!string.IsNullOrEmpty(basePath))
      {
        if (!basePath.EndsWith("\\"))
          basePath += "\\";
        fileRef?.MakeFilePathRelative(basePath);
      }
      DataNodeInfo dataNodeInfo = resXdataNode.GetDataNodeInfo();
      this.AddDataRow("data", dataNodeInfo.Name, dataNodeInfo.ValueData, dataNodeInfo.TypeName, dataNodeInfo.MimeType, dataNodeInfo.Comment);
    }

    private void AddDataRow(string elementName, string name, byte[] value)
    {
      this.AddDataRow(elementName, name, ResXResourceWriter.ToBase64WrappedString(value), this.TypeNameWithAssembly(typeof (byte[])), (string) null, (string) null);
    }

    private void AddDataRow(string elementName, string name, object value)
    {
      switch (value)
      {
        case string _:
          this.AddDataRow(elementName, name, (string) value);
          break;
        case byte[] _:
          this.AddDataRow(elementName, name, (byte[]) value);
          break;
        case ResXFileRef _:
          ResXFileRef fileRef = (ResXFileRef) value;
          ResXDataNode resXdataNode = new ResXDataNode(name, fileRef, this.typeNameConverter);
          fileRef?.MakeFilePathRelative(this.BasePath);
          DataNodeInfo dataNodeInfo1 = resXdataNode.GetDataNodeInfo();
          this.AddDataRow(elementName, dataNodeInfo1.Name, dataNodeInfo1.ValueData, dataNodeInfo1.TypeName, dataNodeInfo1.MimeType, dataNodeInfo1.Comment);
          break;
        default:
          DataNodeInfo dataNodeInfo2 = new ResXDataNode(name, value, this.typeNameConverter).GetDataNodeInfo();
          this.AddDataRow(elementName, dataNodeInfo2.Name, dataNodeInfo2.ValueData, dataNodeInfo2.TypeName, dataNodeInfo2.MimeType, dataNodeInfo2.Comment);
          break;
      }
    }

    private void AddDataRow(string elementName, string name, string value)
    {
      if (value == null)
        this.AddDataRow(elementName, name, value, MultitargetUtil.GetAssemblyQualifiedName(typeof (ResXNullRef), this.typeNameConverter), (string) null, (string) null);
      else
        this.AddDataRow(elementName, name, value, (string) null, (string) null, (string) null);
    }

    private void AddDataRow(
      string elementName,
      string name,
      string value,
      string type,
      string mimeType,
      string comment)
    {
      if (this.hasBeenSaved)
        throw new InvalidOperationException(SR.GetString(SR.ResXResourceWriterSaved));
      string str = (string) null;
      if (!string.IsNullOrEmpty(type) && elementName == "data")
      {
        if (string.IsNullOrEmpty(this.GetFullName(type)))
        {
          try
          {
            System.Type type1 = System.Type.GetType(type);
            if (type1 == typeof (string))
              type = (string) null;
            else if (type1 != (System.Type) null)
              str = this.GetAliasFromName(new AssemblyName(this.GetFullName(MultitargetUtil.GetAssemblyQualifiedName(type1, this.typeNameConverter))));
          }
          catch
          {
          }
        }
        else
          str = this.GetAliasFromName(new AssemblyName(this.GetFullName(type)));
      }
      this.Writer.WriteStartElement(elementName);
      this.Writer.WriteAttributeString(nameof (name), name);
      if (!string.IsNullOrEmpty(str) && !string.IsNullOrEmpty(type) && elementName == "data")
        this.Writer.WriteAttributeString(nameof (type), this.GetTypeName(type) + ", " + str);
      else if (type != null)
        this.Writer.WriteAttributeString(nameof (type), type);
      if (mimeType != null)
        this.Writer.WriteAttributeString("mimetype", mimeType);
      if (type == null && mimeType == null || type != null && type.StartsWith("System.Char", StringComparison.Ordinal))
        this.Writer.WriteAttributeString("xml", "space", (string) null, "preserve");
      this.Writer.WriteStartElement(nameof (value));
      if (!string.IsNullOrEmpty(value))
        this.Writer.WriteString(value);
      this.Writer.WriteEndElement();
      if (!string.IsNullOrEmpty(comment))
      {
        this.Writer.WriteStartElement(nameof (comment));
        this.Writer.WriteString(comment);
        this.Writer.WriteEndElement();
      }
      this.Writer.WriteEndElement();
    }

    private void AddAssemblyRow(string elementName, string alias, string name)
    {
      this.Writer.WriteStartElement(elementName);
      if (!string.IsNullOrEmpty(alias))
        this.Writer.WriteAttributeString(nameof (alias), alias);
      if (!string.IsNullOrEmpty(name))
        this.Writer.WriteAttributeString(nameof (name), name);
      this.Writer.WriteEndElement();
    }

    private string GetAliasFromName(AssemblyName assemblyName)
    {
      if (this.cachedAliases == null)
        this.cachedAliases = new Hashtable();
      string aliasFromName = (string) this.cachedAliases[(object) assemblyName.FullName];
      if (string.IsNullOrEmpty(aliasFromName))
      {
        aliasFromName = assemblyName.Name;
        this.AddAlias(aliasFromName, assemblyName);
        this.AddAssemblyRow("assembly", aliasFromName, assemblyName.FullName);
      }
      return aliasFromName;
    }

    /// <summary>Releases all resources used by the <see cref="T:System.Resources.ResXResourceWriter" />.</summary>
    public void Close() => this.Dispose();

    /// <summary>Releases all resources used by the <see cref="T:System.Resources.ResXResourceWriter" />.</summary>
    public virtual void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    /// <summary>Releases the unmanaged resources used by the <see cref="T:System.Resources.ResXResourceWriter" /> and optionally releases the managed resources.</summary>
    /// <param name="disposing">
    /// <see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
      if (!disposing)
        return;
      if (!this.hasBeenSaved)
        this.Generate();
      if (this.xmlTextWriter != null)
      {
        this.xmlTextWriter.Close();
        this.xmlTextWriter = (XmlTextWriter) null;
      }
      if (this.stream != null)
      {
        this.stream.Close();
        this.stream = (Stream) null;
      }
      if (this.textWriter == null)
        return;
      this.textWriter.Close();
      this.textWriter = (TextWriter) null;
    }

    private string GetTypeName(string typeName)
    {
      int length = typeName.IndexOf(",");
      return length != -1 ? typeName.Substring(0, length) : typeName;
    }

    private string GetFullName(string typeName)
    {
      int num = typeName.IndexOf(",");
      return num == -1 ? (string) null : typeName.Substring(num + 2);
    }

    private static string ToBase64WrappedString(byte[] data)
    {
      string base64String = Convert.ToBase64String(data);
      if (base64String.Length <= 80)
        return base64String;
      StringBuilder stringBuilder = new StringBuilder(base64String.Length + base64String.Length / 80 * 3);
      int startIndex;
      for (startIndex = 0; startIndex < base64String.Length - 80; startIndex += 80)
      {
        stringBuilder.Append("\r\n");
        stringBuilder.Append("        ");
        stringBuilder.Append(base64String, startIndex, 80);
      }
      stringBuilder.Append("\r\n");
      stringBuilder.Append("        ");
      stringBuilder.Append(base64String, startIndex, base64String.Length - startIndex);
      stringBuilder.Append("\r\n");
      return stringBuilder.ToString();
    }

    private string TypeNameWithAssembly(System.Type type)
    {
      return MultitargetUtil.GetAssemblyQualifiedName(type, this.typeNameConverter);
    }

    /// <summary>Writes all resources added by the <see cref="M:System.Resources.ResXResourceWriter.AddResource(System.String,System.Byte[])" /> method to the output file or stream.</summary>
    /// <exception cref="T:System.InvalidOperationException">The resource has already been saved.</exception>
    public void Generate()
    {
      this.hasBeenSaved = !this.hasBeenSaved ? true : throw new InvalidOperationException(SR.GetString(SR.ResXResourceWriterSaved));
      this.Writer.WriteEndElement();
      this.Writer.Flush();
    }
  }
}
#endif
