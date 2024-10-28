#if !NETFRAMEWORK

using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel.Design;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Xml;
using System.Web;

#nullable disable
namespace System.Resources
{
	/// <summary>Enumerates XML resource (.resx) files and streams, and reads the sequential resource name and value pairs.</summary>
	[PermissionSet(SecurityAction.InheritanceDemand, Name = "FullTrust")]
	[PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
	public class ResXResourceReader : IResourceReader, IEnumerable, IDisposable
	{
		private string fileName;
		private TextReader reader;
		private Stream stream;
		private string fileContents;
		private AssemblyName[] assemblyNames;
		private string basePath;
		private bool isReaderDirty;
		private ITypeResolutionService typeResolver;
		private IAliasResolver aliasResolver;
		private ListDictionary resData;
		private ListDictionary resMetadata;
		private string resHeaderVersion;
		private string resHeaderMimeType;
		private string resHeaderReaderType;
		private string resHeaderWriterType;
		private bool useResXDataNodes;

		private ResXResourceReader(ITypeResolutionService typeResolver)
		{
			this.typeResolver = typeResolver;
			this.aliasResolver = (IAliasResolver)new ResXResourceReader.ReaderAliasResolver();
		}

		private ResXResourceReader(AssemblyName[] assemblyNames)
		{
			this.assemblyNames = assemblyNames;
			this.aliasResolver = (IAliasResolver)new ResXResourceReader.ReaderAliasResolver();
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Resources.ResXResourceReader" /> class for the specified resource file.</summary>
		/// <param name="fileName">The path of the resource file to read.</param>
		public ResXResourceReader(string fileName)
		  : this(fileName, (ITypeResolutionService)null, (IAliasResolver)null)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Resources.ResXResourceReader" /> class using a file name and a type resolution service.</summary>
		/// <param name="fileName">The name of an XML resource file that contains resources.</param>
		/// <param name="typeResolver">An object that resolves type names specified in a resource.</param>
		public ResXResourceReader(string fileName, ITypeResolutionService typeResolver)
		  : this(fileName, typeResolver, (IAliasResolver)null)
		{
		}

		internal ResXResourceReader(
		  string fileName,
		  ITypeResolutionService typeResolver,
		  IAliasResolver aliasResolver)
		{
			this.fileName = fileName;
			this.typeResolver = typeResolver;
			this.aliasResolver = aliasResolver;
			if (this.aliasResolver != null)
				return;
			this.aliasResolver = (IAliasResolver)new ResXResourceReader.ReaderAliasResolver();
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Resources.ResXResourceReader" /> class for the specified <see cref="T:System.IO.TextReader" />.</summary>
		/// <param name="reader">A text input stream that contains resources.</param>
		public ResXResourceReader(TextReader reader)
		  : this(reader, (ITypeResolutionService)null, (IAliasResolver)null)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Resources.ResXResourceReader" /> class using a text stream reader and a type resolution service.</summary>
		/// <param name="reader">A text stream reader that contains resources.</param>
		/// <param name="typeResolver">An object that resolves type names specified in a resource.</param>
		public ResXResourceReader(TextReader reader, ITypeResolutionService typeResolver)
		  : this(reader, typeResolver, (IAliasResolver)null)
		{
		}

		internal ResXResourceReader(
		  TextReader reader,
		  ITypeResolutionService typeResolver,
		  IAliasResolver aliasResolver)
		{
			this.reader = reader;
			this.typeResolver = typeResolver;
			this.aliasResolver = aliasResolver;
			if (this.aliasResolver != null)
				return;
			this.aliasResolver = (IAliasResolver)new ResXResourceReader.ReaderAliasResolver();
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Resources.ResXResourceReader" /> class for the specified stream.</summary>
		/// <param name="stream">An input stream that contains resources.</param>
		public ResXResourceReader(Stream stream)
		  : this(stream, (ITypeResolutionService)null, (IAliasResolver)null)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Resources.ResXResourceReader" /> class using an input stream and a type resolution service.</summary>
		/// <param name="stream">An input stream that contains resources.</param>
		/// <param name="typeResolver">An object that resolves type names specified in a resource.</param>
		public ResXResourceReader(Stream stream, ITypeResolutionService typeResolver)
		  : this(stream, typeResolver, (IAliasResolver)null)
		{
		}

		internal ResXResourceReader(
		  Stream stream,
		  ITypeResolutionService typeResolver,
		  IAliasResolver aliasResolver)
		{
			this.stream = stream;
			this.typeResolver = typeResolver;
			this.aliasResolver = aliasResolver;
			if (this.aliasResolver != null)
				return;
			this.aliasResolver = (IAliasResolver)new ResXResourceReader.ReaderAliasResolver();
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Resources.ResXResourceReader" /> class using a stream and an array of assembly names.</summary>
		/// <param name="stream">An input stream that contains resources.</param>
		/// <param name="assemblyNames">An array of <see cref="T:System.Reflection.AssemblyName" /> objects that specifies one or more assemblies. The assemblies are used to resolve a type name in the resource to an actual type.</param>
		public ResXResourceReader(Stream stream, AssemblyName[] assemblyNames)
		  : this(stream, assemblyNames, (IAliasResolver)null)
		{
		}

		internal ResXResourceReader(
		  Stream stream,
		  AssemblyName[] assemblyNames,
		  IAliasResolver aliasResolver)
		{
			this.stream = stream;
			this.assemblyNames = assemblyNames;
			this.aliasResolver = aliasResolver;
			if (this.aliasResolver != null)
				return;
			this.aliasResolver = (IAliasResolver)new ResXResourceReader.ReaderAliasResolver();
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Resources.ResXResourceReader" /> class using a <see cref="T:System.IO.TextReader" /> object and an array of assembly names.</summary>
		/// <param name="reader">An object used to read resources from a stream of text.</param>
		/// <param name="assemblyNames">An array of <see cref="T:System.Reflection.AssemblyName" /> objects that specifies one or more assemblies. The assemblies are used to resolve a type name in the resource to an actual type.</param>
		public ResXResourceReader(TextReader reader, AssemblyName[] assemblyNames)
		  : this(reader, assemblyNames, (IAliasResolver)null)
		{
		}

		internal ResXResourceReader(
		  TextReader reader,
		  AssemblyName[] assemblyNames,
		  IAliasResolver aliasResolver)
		{
			this.reader = reader;
			this.assemblyNames = assemblyNames;
			this.aliasResolver = aliasResolver;
			if (this.aliasResolver != null)
				return;
			this.aliasResolver = (IAliasResolver)new ResXResourceReader.ReaderAliasResolver();
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Resources.ResXResourceReader" /> class using an XML resource file name and an array of assembly names.</summary>
		/// <param name="fileName">The name of an XML resource file that contains resources.</param>
		/// <param name="assemblyNames">An array of <see cref="T:System.Reflection.AssemblyName" /> objects that specifies one or more assemblies. The assemblies are used to resolve a type name in the resource to an actual type.</param>
		public ResXResourceReader(string fileName, AssemblyName[] assemblyNames)
		  : this(fileName, assemblyNames, (IAliasResolver)null)
		{
		}

		internal ResXResourceReader(
		  string fileName,
		  AssemblyName[] assemblyNames,
		  IAliasResolver aliasResolver)
		{
			this.fileName = fileName;
			this.assemblyNames = assemblyNames;
			this.aliasResolver = aliasResolver;
			if (this.aliasResolver != null)
				return;
			this.aliasResolver = (IAliasResolver)new ResXResourceReader.ReaderAliasResolver();
		}

		/// <summary>This member overrides the <see cref="M:System.Object.Finalize" /> method.</summary>
		~ResXResourceReader() => this.Dispose(false);

		/// <summary>Gets or sets the base path for the relative file path specified in a <see cref="T:System.Resources.ResXFileRef" /> object.</summary>
		/// <returns>A path that, if prepended to the relative file path specified in a <see cref="T:System.Resources.ResXFileRef" /> object, yields an absolute path to a resource file.</returns>
		/// <exception cref="T:System.InvalidOperationException">In a set operation, a value cannot be specified because the XML resource file has already been accessed and is in use.</exception>
		public string BasePath
		{
			get => this.basePath;
			set
			{
				if (this.isReaderDirty)
					throw new InvalidOperationException(SR.GetString(SR.Invalid_ResX_BasePath_Operation));
				this.basePath = value;
			}
		}

		/// <summary>Gets or sets a value that indicates whether <see cref="T:System.Resources.ResXDataNode" /> objects are returned when reading the current XML resource file or stream.</summary>
		/// <returns>
		/// <see langword="true" /> if resource data nodes are retrieved; <see langword="false" /> if resource data nodes are ignored.</returns>
		/// <exception cref="T:System.InvalidOperationException">In a set operation, the enumerator for the resource file or stream is already open.</exception>
		public bool UseResXDataNodes
		{
			get => this.useResXDataNodes;
			set
			{
				if (this.isReaderDirty)
					throw new InvalidOperationException(SR.GetString(SR.Invalid_ResX_BasePath_Operation));
				this.useResXDataNodes = value;
			}
		}

		/// <summary>Releases all resources used by the <see cref="T:System.Resources.ResXResourceReader" />.</summary>
		public void Close() => ((IDisposable)this).Dispose();

		/// <summary>Releases the unmanaged resources used by the <see cref="T:System.Resources.ResXResourceReader" /> and optionally releases the managed resources. For a description of this member, see the <see cref="M:System.IDisposable.Dispose" /> method.</summary>
		void IDisposable.Dispose()
		{
			GC.SuppressFinalize((object)this);
			this.Dispose(true);
		}

		/// <summary>Releases the unmanaged resources used by the <see cref="T:System.Resources.ResXResourceReader" /> and optionally releases the managed resources.</summary>
		/// <param name="disposing">
		/// <see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to release only unmanaged resources.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (!disposing)
				return;
			if (this.fileName != null && this.stream != null)
			{
				this.stream.Close();
				this.stream = (Stream)null;
			}
			if (this.reader == null)
				return;
			this.reader.Close();
			this.reader = (TextReader)null;
		}

		private void SetupNameTable(XmlReader reader)
		{
			reader.NameTable.Add("type");
			reader.NameTable.Add("name");
			reader.NameTable.Add("data");
			reader.NameTable.Add("metadata");
			reader.NameTable.Add("mimetype");
			reader.NameTable.Add("value");
			reader.NameTable.Add("resheader");
			reader.NameTable.Add("version");
			reader.NameTable.Add("resmimetype");
			reader.NameTable.Add(nameof(reader));
			reader.NameTable.Add("writer");
			reader.NameTable.Add(ResXResourceWriter.BinSerializedObjectMimeType);
			reader.NameTable.Add(ResXResourceWriter.SoapSerializedObjectMimeType);
			reader.NameTable.Add("assembly");
			reader.NameTable.Add("alias");
		}

		private void EnsureResData()
		{
			if (this.resData != null)
				return;
			this.resData = new ListDictionary();
			this.resMetadata = new ListDictionary();
			XmlTextReader reader = (XmlTextReader)null;
			try
			{
				if (this.fileContents != null)
					reader = new XmlTextReader((TextReader)new StringReader(this.fileContents));
				else if (this.reader != null)
					reader = new XmlTextReader(this.reader);
				else if (this.fileName != null || this.stream != null)
				{
					if (this.stream == null)
						this.stream = (Stream)new FileStream(this.fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
					reader = new XmlTextReader(this.stream);
				}
				this.SetupNameTable((XmlReader)reader);
				reader.WhitespaceHandling = WhitespaceHandling.None;
				this.ParseXml(reader);
			}
			finally
			{
				if (this.fileName != null && this.stream != null)
				{
					this.stream.Close();
					this.stream = (Stream)null;
				}
			}
		}

		/// <summary>Creates a new <see cref="T:System.Resources.ResXResourceReader" /> object and initializes it to read a string whose contents are in the form of an XML resource file.</summary>
		/// <param name="fileContents">A string containing XML resource-formatted information.</param>
		/// <returns>An object that reads resources from the <paramref name="fileContents" /> string.</returns>
		public static ResXResourceReader FromFileContents(string fileContents)
		{
			return ResXResourceReader.FromFileContents(fileContents, (ITypeResolutionService)null);
		}

		/// <summary>Creates a new <see cref="T:System.Resources.ResXResourceReader" /> object and initializes it to read a string whose contents are in the form of an XML resource file, and to use an <see cref="T:System.ComponentModel.Design.ITypeResolutionService" /> object to resolve type names specified in a resource.</summary>
		/// <param name="fileContents">A string containing XML resource-formatted information.</param>
		/// <param name="typeResolver">An object that resolves type names specified in a resource.</param>
		/// <returns>An object that reads resources from the <paramref name="fileContents" /> string.</returns>
		public static ResXResourceReader FromFileContents(
		  string fileContents,
		  ITypeResolutionService typeResolver)
		{
			return new ResXResourceReader(typeResolver)
			{
				fileContents = fileContents
			};
		}

		/// <summary>Creates a new <see cref="T:System.Resources.ResXResourceReader" /> object and initializes it to read a string whose contents are in the form of an XML resource file, and to use an array of <see cref="T:System.Reflection.AssemblyName" /> objects to resolve type names specified in a resource.</summary>
		/// <param name="fileContents">A string whose contents are in the form of an XML resource file.</param>
		/// <param name="assemblyNames">An array of <see cref="T:System.Reflection.AssemblyName" /> objects that specifies one or more assemblies. The assemblies are used to resolve a type name in the resource to an actual type.</param>
		/// <returns>An object that reads resources from the <paramref name="fileContents" /> string.</returns>
		public static ResXResourceReader FromFileContents(
		  string fileContents,
		  AssemblyName[] assemblyNames)
		{
			return new ResXResourceReader(assemblyNames)
			{
				fileContents = fileContents
			};
		}

		/// <summary>Returns an enumerator for the current <see cref="T:System.Resources.ResXResourceReader" /> object. For a description of this member, see the <see cref="M:System.Collections.IEnumerable.GetEnumerator" /> method.</summary>
		/// <returns>An enumerator that can iterate through the name/value pairs in the XML resource (.resx) stream or string associated with the current <see cref="T:System.Resources.ResXResourceReader" /> object.</returns>
		IEnumerator IEnumerable.GetEnumerator() => (IEnumerator)this.GetEnumerator();

		/// <summary>Returns an enumerator for the current <see cref="T:System.Resources.ResXResourceReader" /> object.</summary>
		/// <returns>An enumerator for the current <see cref="T:System.Resources.ResourceReader" /> object.</returns>
		public IDictionaryEnumerator GetEnumerator()
		{
			this.isReaderDirty = true;
			this.EnsureResData();
			return this.resData.GetEnumerator();
		}

		/// <summary>Provides a dictionary enumerator that can retrieve the design-time properties from the current XML resource file or stream.</summary>
		/// <returns>An enumerator for the metadata in a resource.</returns>
		public IDictionaryEnumerator GetMetadataEnumerator()
		{
			this.EnsureResData();
			return this.resMetadata.GetEnumerator();
		}

		private Point GetPosition(XmlReader reader)
		{
			Point position = new Point(0, 0);
			if (reader is IXmlLineInfo xmlLineInfo)
			{
				position.Y = xmlLineInfo.LineNumber;
				position.X = xmlLineInfo.LinePosition;
			}
			return position;
		}

		private void ParseXml(XmlTextReader reader)
		{
			bool flag1 = false;
			try
			{
				while (reader.Read())
				{
					if (reader.NodeType == XmlNodeType.Element)
					{
						string localName = reader.LocalName;
						if (reader.LocalName.Equals("assembly"))
							this.ParseAssemblyNode((XmlReader)reader, false);
						else if (reader.LocalName.Equals("data"))
							this.ParseDataNode(reader, false);
						else if (reader.LocalName.Equals("resheader"))
							this.ParseResHeaderNode((XmlReader)reader);
						else if (reader.LocalName.Equals("metadata"))
							this.ParseDataNode(reader, true);
					}
				}
				flag1 = true;
			}
			catch (SerializationException ex)
			{
				Point position = this.GetPosition((XmlReader)reader);
				string message = SR.GetString(SR.SerializationException, (object)reader["type"], (object)position.Y, (object)position.X, (object)ex.Message);
				XmlException innerException = new XmlException(message, (Exception)ex, position.Y, position.X);
				throw new SerializationException(message, (Exception)innerException);
			}
			catch (TargetInvocationException ex)
			{
				Point position = this.GetPosition((XmlReader)reader);
				string message = SR.GetString(SR.InvocationException, (object)reader["type"], (object)position.Y, (object)position.X, (object)ex.InnerException.Message);
				XmlException inner = new XmlException(message, ex.InnerException, position.Y, position.X);
				throw new TargetInvocationException(message, (Exception)inner);
			}
			catch (XmlException ex)
			{
				throw new ArgumentException(SR.GetString(SR.InvalidResXFile_Resource, (object)ex.Message), (Exception)ex);
			}
			catch (Exception ex)
			{
				if (System.Windows.Forms.ClientUtils.IsSecurityOrCriticalException(ex))
				{
					throw;
				}
				else
				{
					Point position = this.GetPosition((XmlReader)reader);
					XmlException innerException = new XmlException(ex.Message, ex, position.Y, position.X);
					throw new ArgumentException(SR.GetString(SR.InvalidResXFile_Resource, (object)innerException.Message), (Exception)innerException);
				}
			}
			finally
			{
				if (!flag1)
				{
					this.resData = (ListDictionary)null;
					this.resMetadata = (ListDictionary)null;
				}
			}
			bool flag2 = false;
			if (object.Equals((object)this.resHeaderMimeType, (object)ResXResourceWriter.ResMimeType))
			{
				System.Type type1 = typeof(ResXResourceReader);
				System.Type type2 = typeof(ResXResourceWriter);
				string str1 = this.resHeaderReaderType;
				string str2 = this.resHeaderWriterType;
				if (str1 != null && str1.IndexOf(',') != -1)
					str1 = str1.Split(',')[0].Trim();
				if (str2 != null && str2.IndexOf(',') != -1)
					str2 = str2.Split(',')[0].Trim();
				if (str1 != null && str2 != null && str1.Equals(type1.FullName) && str2.Equals(type2.FullName))
					flag2 = true;
			}
			if (!flag2)
			{
				this.resData = (ListDictionary)null;
				this.resMetadata = (ListDictionary)null;
				throw new ArgumentException(SR.GetString(SR.InvalidResXFileReaderWriterTypes));
			}
		}

		private void ParseResHeaderNode(XmlReader reader)
		{
			string objA = reader["name"];
			if (objA == null)
				return;
			reader.ReadStartElement();
			if (object.Equals((object)objA, (object)"version"))
			{
				if (reader.NodeType == XmlNodeType.Element)
					this.resHeaderVersion = reader.ReadElementString();
				else
					this.resHeaderVersion = reader.Value.Trim();
			}
			else if (object.Equals((object)objA, (object)"resmimetype"))
			{
				if (reader.NodeType == XmlNodeType.Element)
					this.resHeaderMimeType = reader.ReadElementString();
				else
					this.resHeaderMimeType = reader.Value.Trim();
			}
			else if (object.Equals((object)objA, (object)nameof(reader)))
			{
				if (reader.NodeType == XmlNodeType.Element)
					this.resHeaderReaderType = reader.ReadElementString();
				else
					this.resHeaderReaderType = reader.Value.Trim();
			}
			else if (object.Equals((object)objA, (object)"writer"))
			{
				if (reader.NodeType == XmlNodeType.Element)
					this.resHeaderWriterType = reader.ReadElementString();
				else
					this.resHeaderWriterType = reader.Value.Trim();
			}
			else
			{
				switch (objA.ToLower(CultureInfo.InvariantCulture))
				{
					case "version":
						if (reader.NodeType == XmlNodeType.Element)
						{
							this.resHeaderVersion = reader.ReadElementString();
							break;
						}
						this.resHeaderVersion = reader.Value.Trim();
						break;
					case "resmimetype":
						if (reader.NodeType == XmlNodeType.Element)
						{
							this.resHeaderMimeType = reader.ReadElementString();
							break;
						}
						this.resHeaderMimeType = reader.Value.Trim();
						break;
					case nameof(reader):
						if (reader.NodeType == XmlNodeType.Element)
						{
							this.resHeaderReaderType = reader.ReadElementString();
							break;
						}
						this.resHeaderReaderType = reader.Value.Trim();
						break;
					case "writer":
						if (reader.NodeType == XmlNodeType.Element)
						{
							this.resHeaderWriterType = reader.ReadElementString();
							break;
						}
						this.resHeaderWriterType = reader.Value.Trim();
						break;
				}
			}
		}

		private void ParseAssemblyNode(XmlReader reader, bool isMetaData)
		{
			string name1 = reader["alias"];
			AssemblyName name2 = new AssemblyName(reader["name"]);
			if (string.IsNullOrEmpty(name1))
				name1 = name2.Name;
			this.aliasResolver.PushAlias(name1, name2);
		}

		private void ParseDataNode(XmlTextReader reader, bool isMetaData)
		{
			DataNodeInfo nodeInfo = new DataNodeInfo();
			nodeInfo.Name = reader["name"];
			string typeName = reader["type"];
			string alias = (string)null;
			AssemblyName assemblyName = (AssemblyName)null;
			if (!string.IsNullOrEmpty(typeName))
				alias = this.GetAliasFromTypeName(typeName);
			if (!string.IsNullOrEmpty(alias))
				assemblyName = this.aliasResolver.ResolveAlias(alias);
			nodeInfo.TypeName = assemblyName == null ? reader["type"] : this.GetTypeFromTypeName(typeName) + ", " + assemblyName.FullName;
			nodeInfo.MimeType = reader["mimetype"];
			bool flag = false;
			nodeInfo.ReaderPosition = this.GetPosition((XmlReader)reader);
			while (!flag && reader.Read())
			{
				if (reader.NodeType == XmlNodeType.EndElement && (reader.LocalName.Equals("data") || reader.LocalName.Equals("metadata")))
					flag = true;
				else if (reader.NodeType == XmlNodeType.Element)
				{
					if (reader.Name.Equals("value"))
					{
						WhitespaceHandling whitespaceHandling = reader.WhitespaceHandling;
						try
						{
							reader.WhitespaceHandling = WhitespaceHandling.Significant;
							nodeInfo.ValueData = reader.ReadString();
						}
						finally
						{
							reader.WhitespaceHandling = whitespaceHandling;
						}
					}
					else if (reader.Name.Equals("comment"))
						nodeInfo.Comment = reader.ReadString();
				}
				else
					nodeInfo.ValueData = reader.Value.Trim();
			}
			ResXDataNode resXdataNode = nodeInfo.Name != null ? new ResXDataNode(nodeInfo, this.BasePath) :
				throw new ArgumentException(SR.GetString(SR.InvalidResXResourceNoName_Resources, (object)nodeInfo.ValueData));
			if (this.UseResXDataNodes)
			{
				this.resData[(object)nodeInfo.Name] = (object)resXdataNode;
			}
			else
			{
				IDictionary dictionary = isMetaData ? (IDictionary)this.resMetadata : (IDictionary)this.resData;
				if (this.assemblyNames == null)
					dictionary[(object)nodeInfo.Name] = resXdataNode.GetValue(this.typeResolver);
				else
					dictionary[(object)nodeInfo.Name] = resXdataNode.GetValue(this.assemblyNames);
			}
		}

		private string GetAliasFromTypeName(string typeName)
		{
			int num = typeName.IndexOf(",");
			return typeName.Substring(num + 2);
		}

		private string GetTypeFromTypeName(string typeName)
		{
			int length = typeName.IndexOf(",");
			return typeName.Substring(0, length);
		}

		private sealed class ReaderAliasResolver : IAliasResolver
		{
			private Hashtable cachedAliases;

			internal ReaderAliasResolver() => this.cachedAliases = new Hashtable();

			public AssemblyName ResolveAlias(string alias)
			{
				AssemblyName assemblyName = (AssemblyName)null;
				if (this.cachedAliases != null)
					assemblyName = (AssemblyName)this.cachedAliases[(object)alias];
				return assemblyName;
			}

			public void PushAlias(string alias, AssemblyName name)
			{
				if (this.cachedAliases == null || string.IsNullOrEmpty(alias))
					return;
				this.cachedAliases[(object)alias] = (object)name;
			}
		}
	}
}
#endif