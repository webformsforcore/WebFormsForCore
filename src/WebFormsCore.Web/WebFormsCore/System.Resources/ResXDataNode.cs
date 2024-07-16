#if !NETFRAMEWORK

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
#if !WebFormsCore || NETFRAMEWORK
using System.Runtime.Serialization.Formatters.Binary;
#else
using IFormatter = WebFormsCore.Serialization.Formatters.IFormatter;
using WebFormsCore.Serialization.Formatters.Binary;
using System.Web;
#endif
using System.Runtime.Serialization.Formatters.Soap;
using System.Security.Permissions;
using System.Text;
using System.Xml;

#nullable disable
namespace System.Resources
{
	/// <summary>Represents an element in an XML resource (.resx) file.</summary>
	[Serializable]
	[PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
	public sealed class ResXDataNode : ISerializable
	{
		private static readonly char[] SpecialChars = new char[3]
		{
	  ' ',
	  '\r',
	  '\n'
		};
		private DataNodeInfo nodeInfo;
		private string name;
		private string comment;
		private string typeName;
		private string fileRefFullPath;
		private string fileRefType;
		private string fileRefTextEncoding;
		private object value;
		private ResXFileRef fileRef;
		private IFormatter binaryFormatter;
		private static ITypeResolutionService internalTypeResolver = (ITypeResolutionService)new AssemblyNamesTypeResolutionService(new AssemblyName[1]
		{
	  new AssemblyName("System.Windows.Forms")
		});
		private Func<System.Type, string> typeNameConverter;

		private ResXDataNode()
		{
		}

		internal ResXDataNode DeepClone()
		{
			return new ResXDataNode()
			{
				nodeInfo = this.nodeInfo != null ? this.nodeInfo.Clone() : (DataNodeInfo)null,
				name = this.name,
				comment = this.comment,
				typeName = this.typeName,
				fileRefFullPath = this.fileRefFullPath,
				fileRefType = this.fileRefType,
				fileRefTextEncoding = this.fileRefTextEncoding,
				value = this.value,
				fileRef = this.fileRef != null ? this.fileRef.Clone() : (ResXFileRef)null,
				typeNameConverter = this.typeNameConverter
			};
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Resources.ResXDataNode" /> class.</summary>
		/// <param name="name">The name of the resource.</param>
		/// <param name="value">The resource to store.</param>
		/// <exception cref="T:System.InvalidOperationException">The resource named in <paramref name="value" /> does not support serialization.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="name" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="name" /> is a string of zero length.</exception>
		public ResXDataNode(string name, object value)
		  : this(name, value, (Func<System.Type, string>)null)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Resources.ResXDataNode" /> class.</summary>
		/// <param name="name">The name of the resource.</param>
		/// <param name="value">The resource to store.</param>
		/// <param name="typeNameConverter">A reference to a method that takes a <see cref="T:System.Type" /> and returns a string containing the <see cref="T:System.Type" /> name.</param>
		/// <exception cref="T:System.InvalidOperationException">The resource named in <paramref name="value" /> does not support serialization.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="name" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="name" /> is a string of zero length.</exception>
		public ResXDataNode(string name, object value, Func<System.Type, string> typeNameConverter)
		{
			switch (name)
			{
				case null:
					throw new ArgumentNullException(nameof(name));
				case "":
					throw new ArgumentException(nameof(name));
				default:
					this.typeNameConverter = typeNameConverter;
					System.Type type = value == null ? typeof(object) : value.GetType();
					if (value != null && !type.IsSerializable)
						throw new InvalidOperationException(SR.GetString(SR.Not_Serializable_Type, name, type.FullName));
					if (value != null)
						this.typeName = MultitargetUtil.GetAssemblyQualifiedName(type, this.typeNameConverter);
					this.name = name;
					this.value = value;
					break;
			}
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Resources.ResXDataNode" /> class with a reference to a resource file.</summary>
		/// <param name="name">The name of the resource.</param>
		/// <param name="fileRef">The file reference to use as the resource.</param>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="name" /> is <see langword="null" /> or <paramref name="fileRef" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="name" /> is a string of zero length.</exception>
		public ResXDataNode(string name, ResXFileRef fileRef)
		  : this(name, fileRef, (Func<System.Type, string>)null)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Resources.ResXDataNode" /> class with a reference to a resource file.</summary>
		/// <param name="name">The name of the resource.</param>
		/// <param name="fileRef">The file reference to use as the resource.</param>
		/// <param name="typeNameConverter">A reference to a method that takes a <see cref="T:System.Type" /> and returns a string containing the <see cref="T:System.Type" /> name.</param>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="name" /> is <see langword="null" /> or <paramref name="fileRef" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="name" /> is a string of zero length.</exception>
		public ResXDataNode(string name, ResXFileRef fileRef, Func<System.Type, string> typeNameConverter)
		{
			if (name == null)
				throw new ArgumentNullException(nameof(name));
			if (fileRef == null)
				throw new ArgumentNullException(nameof(fileRef));
			this.name = name.Length != 0 ? name : throw new ArgumentException(nameof(name));
			this.fileRef = fileRef;
			this.typeNameConverter = typeNameConverter;
		}

		internal ResXDataNode(DataNodeInfo nodeInfo, string basePath)
		{
			this.nodeInfo = nodeInfo;
			this.InitializeDataNode(basePath);
		}

		private void InitializeDataNode(string basePath)
		{
			System.Type type = (System.Type)null;
			if (!string.IsNullOrEmpty(this.nodeInfo.TypeName))
				type = ResXDataNode.internalTypeResolver.GetType(this.nodeInfo.TypeName, false, true);
			if (!(type != (System.Type)null) || !type.Equals(typeof(ResXFileRef)))
				return;
			string[] resxFileRefString = ResXFileRef.Converter.ParseResxFileRefString(this.nodeInfo.ValueData);
			if (resxFileRefString == null || resxFileRefString.Length <= 1)
				return;
			this.fileRefFullPath = Path.IsPathRooted(resxFileRefString[0]) || basePath == null ? resxFileRefString[0] : Path.Combine(basePath, resxFileRefString[0]);
			this.fileRefType = resxFileRefString[1];
			if (resxFileRefString.Length <= 2)
				return;
			this.fileRefTextEncoding = resxFileRefString[2];
		}

		/// <summary>Gets or sets an arbitrary comment regarding this resource.</summary>
		/// <returns>A string that represents the comment.</returns>
		public string Comment
		{
			get
			{
				string comment = this.comment;
				if (comment == null && this.nodeInfo != null)
					comment = this.nodeInfo.Comment;
				return comment ?? "";
			}
			set => this.comment = value;
		}

		/// <summary>Gets or sets the name of this resource.</summary>
		/// <returns>A string that corresponds to the resource name. If no name is assigned, this property returns a zero-length string.</returns>
		/// <exception cref="T:System.ArgumentNullException">The name property is set to <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">The name property is set to a string of zero length.</exception>
		public string Name
		{
			get
			{
				string name = this.name;
				if (name == null && this.nodeInfo != null)
					name = this.nodeInfo.Name;
				return name;
			}
			set
			{
				switch (value)
				{
					case null:
						throw new ArgumentNullException(nameof(Name));
					case "":
						throw new ArgumentException(nameof(Name));
					default:
						this.name = value;
						break;
				}
			}
		}

		/// <summary>Gets the file reference for this resource.</summary>
		/// <returns>The file reference, if this resource uses one. If this resource stores its value as an <see cref="T:System.Object" />, this property will return <see langword="null" />.</returns>
		public ResXFileRef FileRef
		{
			get
			{
				if (this.FileRefFullPath == null)
					return (ResXFileRef)null;
				if (this.fileRef == null)
					this.fileRef = !string.IsNullOrEmpty(this.fileRefTextEncoding) ? new ResXFileRef(this.FileRefFullPath, this.FileRefType, Encoding.GetEncoding(this.FileRefTextEncoding)) : new ResXFileRef(this.FileRefFullPath, this.FileRefType);
				return this.fileRef;
			}
		}

		private string FileRefFullPath
		{
			get => (this.fileRef == null ? (string)null : this.fileRef.FileName) ?? this.fileRefFullPath;
		}

		private string FileRefType
		{
			get => (this.fileRef == null ? (string)null : this.fileRef.TypeName) ?? this.fileRefType;
		}

		private string FileRefTextEncoding
		{
			get
			{
				return (this.fileRef == null ? (string)null : (this.fileRef.TextFileEncoding == null ? (string)null : this.fileRef.TextFileEncoding.BodyName)) ?? this.fileRefTextEncoding;
			}
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private IFormatter CreateSoapFormatter() => new SoapFormatter();
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

		private void FillDataNodeInfoFromObject(DataNodeInfo nodeInfo, object value)
		{
			System.Type type1;
			switch (value)
			{
				case CultureInfo cultureInfo:
					nodeInfo.ValueData = cultureInfo.Name;
					nodeInfo.TypeName = MultitargetUtil.GetAssemblyQualifiedName(typeof(CultureInfo), this.typeNameConverter);
					return;
				case string _:
					nodeInfo.ValueData = (string)value;
					if (value != null)
						return;
					nodeInfo.TypeName = MultitargetUtil.GetAssemblyQualifiedName(typeof(ResXNullRef), this.typeNameConverter);
					return;
				case byte[] _:
					nodeInfo.ValueData = ResXDataNode.ToBase64WrappedString((byte[])value);
					nodeInfo.TypeName = MultitargetUtil.GetAssemblyQualifiedName(typeof(byte[]), this.typeNameConverter);
					return;
				case null:
					type1 = typeof(object);
					break;
				default:
					type1 = value.GetType();
					break;
			}
			System.Type type2 = type1;
			if (value != null && !type2.IsSerializable)
				throw new InvalidOperationException(SR.GetString(SR.Not_Serializable_Type, (object)this.name, (object)type2.FullName));
			TypeConverter converter = TypeDescriptor.GetConverter(type2);
			bool flag1 = converter.CanConvertTo(typeof(string));
			bool flag2 = converter.CanConvertFrom(typeof(string));
			try
			{
				if (flag1 & flag2)
				{
					nodeInfo.ValueData = converter.ConvertToInvariantString(value);
					nodeInfo.TypeName = MultitargetUtil.GetAssemblyQualifiedName(type2, this.typeNameConverter);
					return;
				}
			}
			catch (Exception ex)
			{
				if (System.Windows.Forms.ClientUtils.IsSecurityOrCriticalException(ex))
					throw;
			}
			if (converter.CanConvertTo(typeof(byte[])) & converter.CanConvertFrom(typeof(byte[])))
			{
				string base64WrappedString = ResXDataNode.ToBase64WrappedString((byte[])converter.ConvertTo(value, typeof(byte[])));
				nodeInfo.ValueData = base64WrappedString;
				nodeInfo.MimeType = ResXResourceWriter.ByteArraySerializedObjectMimeType;
				nodeInfo.TypeName = MultitargetUtil.GetAssemblyQualifiedName(type2, this.typeNameConverter);
			}
			else if (value == null)
			{
				nodeInfo.ValueData = string.Empty;
				nodeInfo.TypeName = MultitargetUtil.GetAssemblyQualifiedName(typeof(ResXNullRef), this.typeNameConverter);
			}
			else
			{
				if (this.binaryFormatter == null)
				{
					this.binaryFormatter = (IFormatter)new BinaryFormatter();
					this.binaryFormatter.Binder = (SerializationBinder)new ResXSerializationBinder(this.typeNameConverter);
				}
				MemoryStream serializationStream = new MemoryStream();
				this.binaryFormatter.Serialize((Stream)serializationStream, value);
				string base64WrappedString = ResXDataNode.ToBase64WrappedString(serializationStream.ToArray());
				nodeInfo.ValueData = base64WrappedString;
				nodeInfo.MimeType = ResXResourceWriter.DefaultSerializedObjectMimeType;
			}
		}

		private object GenerateObjectFromDataNodeInfo(
		  DataNodeInfo dataNodeInfo,
		  ITypeResolutionService typeResolver)
		{
			object fromDataNodeInfo = (object)null;
			string mimeType = dataNodeInfo.MimeType;
			string typeName = dataNodeInfo.TypeName == null || dataNodeInfo.TypeName.Length == 0 ? MultitargetUtil.GetAssemblyQualifiedName(typeof(string), this.typeNameConverter) : dataNodeInfo.TypeName;
			if (mimeType != null && mimeType.Length > 0)
			{
				if (string.Equals(mimeType, ResXResourceWriter.BinSerializedObjectMimeType) || string.Equals(mimeType, ResXResourceWriter.Beta2CompatSerializedObjectMimeType) || string.Equals(mimeType, ResXResourceWriter.CompatBinSerializedObjectMimeType))
				{
					byte[] buffer = ResXDataNode.FromBase64WrappedString(dataNodeInfo.ValueData);
					if (this.binaryFormatter == null)
					{
						this.binaryFormatter = (IFormatter)new BinaryFormatter();
						this.binaryFormatter.Binder = (SerializationBinder)new ResXSerializationBinder(typeResolver);
					}
					IFormatter binaryFormatter = this.binaryFormatter;
					if (buffer != null && buffer.Length != 0)
					{
						fromDataNodeInfo = binaryFormatter.Deserialize((Stream)new MemoryStream(buffer));
						if (fromDataNodeInfo is ResXNullRef)
							fromDataNodeInfo = (object)null;
					}
				}
				else if (string.Equals(mimeType, ResXResourceWriter.SoapSerializedObjectMimeType) || string.Equals(mimeType, ResXResourceWriter.CompatSoapSerializedObjectMimeType))
				{
					byte[] buffer = ResXDataNode.FromBase64WrappedString(dataNodeInfo.ValueData);
					if (buffer != null && buffer.Length != 0)
					{
						fromDataNodeInfo = this.CreateSoapFormatter().Deserialize((Stream)new MemoryStream(buffer));
						if (fromDataNodeInfo is ResXNullRef)
							fromDataNodeInfo = (object)null;
					}
				}
				else if (string.Equals(mimeType, ResXResourceWriter.ByteArraySerializedObjectMimeType) && typeName != null && typeName.Length > 0)
				{
					System.Type type = this.ResolveType(typeName, typeResolver);
					if (type != (System.Type)null)
					{
						TypeConverter converter = TypeDescriptor.GetConverter(type);
						if (converter.CanConvertFrom(typeof(byte[])))
						{
							byte[] numArray = ResXDataNode.FromBase64WrappedString(dataNodeInfo.ValueData);
							if (numArray != null)
								fromDataNodeInfo = converter.ConvertFrom((object)numArray);
						}
					}
					else
					{
						string message = SR.GetString(SR.Type_Load_Exception, (object)typeName, (object)dataNodeInfo.ReaderPosition.Y, (object)dataNodeInfo.ReaderPosition.X);
						XmlException inner = new XmlException(message, (Exception)null, dataNodeInfo.ReaderPosition.Y, dataNodeInfo.ReaderPosition.X);
						throw new TypeLoadException(message, (Exception)inner);
					}
				}
			}
			else if (typeName != null && typeName.Length > 0)
			{
				System.Type type = this.ResolveType(typeName, typeResolver);
				if (type != (System.Type)null)
				{
					if (type == typeof(ResXNullRef))
						fromDataNodeInfo = (object)null;
					else if (typeName.IndexOf("System.Byte[]") != -1 && typeName.IndexOf("mscorlib") != -1)
					{
						fromDataNodeInfo = (object)ResXDataNode.FromBase64WrappedString(dataNodeInfo.ValueData);
					}
					else
					{
						TypeConverter converter = TypeDescriptor.GetConverter(type);
						if (converter.CanConvertFrom(typeof(string)))
						{
							string valueData = dataNodeInfo.ValueData;
							try
							{
								fromDataNodeInfo = converter.ConvertFromInvariantString(valueData);
							}
							catch (NotSupportedException ex)
							{
								string message = SR.GetString(SR.Not_Supported, (object)typeName, (object)dataNodeInfo.ReaderPosition.Y, (object)dataNodeInfo.ReaderPosition.X, (object)ex.Message);
								XmlException innerException = new XmlException(message, (Exception)ex, dataNodeInfo.ReaderPosition.Y, dataNodeInfo.ReaderPosition.X);
								throw new NotSupportedException(message, (Exception)innerException);
							}
						}
					}
				}
				else
				{
					string message = SR.GetString(SR.Type_Load_Exception, (object)typeName, (object)dataNodeInfo.ReaderPosition.Y, (object)dataNodeInfo.ReaderPosition.X);
					XmlException inner = new XmlException(message, (Exception)null, dataNodeInfo.ReaderPosition.Y, dataNodeInfo.ReaderPosition.X);
					throw new TypeLoadException(message, (Exception)inner);
				}
			}
			return fromDataNodeInfo;
		}

		internal DataNodeInfo GetDataNodeInfo()
		{
			bool flag = true;
			if (this.nodeInfo != null)
				flag = false;
			else
				this.nodeInfo = new DataNodeInfo();
			this.nodeInfo.Name = this.Name;
			this.nodeInfo.Comment = this.Comment;
			if (flag || this.FileRefFullPath != null)
			{
				if (this.FileRefFullPath != null)
				{
					this.nodeInfo.ValueData = this.FileRef.ToString();
					this.nodeInfo.MimeType = (string)null;
					this.nodeInfo.TypeName = MultitargetUtil.GetAssemblyQualifiedName(typeof(ResXFileRef), this.typeNameConverter);
				}
				else
					this.FillDataNodeInfoFromObject(this.nodeInfo, this.value);
			}
			return this.nodeInfo;
		}

		/// <summary>Retrieves the position of the resource in the resource file.</summary>
		/// <returns>A structure that specifies the location of this resource in the resource file as a line position (<see cref="P:System.Drawing.Point.X" />) and a column position (<see cref="P:System.Drawing.Point.Y" />). If this resource is not part of a resource file, this method returns a structure that has an <see cref="P:System.Drawing.Point.X" /> of 0 and a <see cref="P:System.Drawing.Point.Y" /> of 0.</returns>
		public Point GetNodePosition()
		{
			return this.nodeInfo == null ? new Point() : this.nodeInfo.ReaderPosition;
		}

		/// <summary>Retrieves the type name for the value by using the specified type resolution service.</summary>
		/// <param name="typeResolver">The type resolution service to use to locate a converter for this type.</param>
		/// <returns>A string that represents the fully qualified name of the type.</returns>
		/// <exception cref="T:System.TypeLoadException">The corresponding type could not be found.</exception>
		public string GetValueTypeName(ITypeResolutionService typeResolver)
		{
			if (this.typeName != null && this.typeName.Length > 0)
				return this.typeName.Equals(MultitargetUtil.GetAssemblyQualifiedName(typeof(ResXNullRef), this.typeNameConverter)) ? MultitargetUtil.GetAssemblyQualifiedName(typeof(object), this.typeNameConverter) : this.typeName;
			string valueTypeName = this.FileRefType;
			System.Type type = (System.Type)null;
			if (valueTypeName != null)
				type = this.ResolveType(this.FileRefType, typeResolver);
			else if (this.nodeInfo != null)
			{
				valueTypeName = this.nodeInfo.TypeName;
				if (valueTypeName == null || valueTypeName.Length == 0)
				{
					if (this.nodeInfo.MimeType != null && this.nodeInfo.MimeType.Length > 0)
					{
						object obj = (object)null;
						try
						{
							obj = this.GenerateObjectFromDataNodeInfo(this.nodeInfo, typeResolver);
						}
						catch (Exception ex)
						{
							if (System.Windows.Forms.ClientUtils.IsCriticalException(ex))
								throw;
							else
								valueTypeName = MultitargetUtil.GetAssemblyQualifiedName(typeof(object), this.typeNameConverter);
						}
						if (obj != null)
							valueTypeName = MultitargetUtil.GetAssemblyQualifiedName(obj.GetType(), this.typeNameConverter);
					}
					else
						valueTypeName = MultitargetUtil.GetAssemblyQualifiedName(typeof(string), this.typeNameConverter);
				}
				else
					type = this.ResolveType(this.nodeInfo.TypeName, typeResolver);
			}
			if (type != (System.Type)null)
				valueTypeName = !(type == typeof(ResXNullRef)) ? MultitargetUtil.GetAssemblyQualifiedName(type, this.typeNameConverter) : MultitargetUtil.GetAssemblyQualifiedName(typeof(object), this.typeNameConverter);
			return valueTypeName;
		}

		/// <summary>Retrieves the type name for the value by examining the specified assemblies.</summary>
		/// <param name="names">The assemblies to examine for the type.</param>
		/// <returns>A string that represents the fully qualified name of the type.</returns>
		/// <exception cref="T:System.TypeLoadException">The corresponding type could not be found.</exception>
		public string GetValueTypeName(AssemblyName[] names)
		{
			return this.GetValueTypeName((ITypeResolutionService)new AssemblyNamesTypeResolutionService(names));
		}

		/// <summary>Retrieves the object that is stored by this node by using the specified type resolution service.</summary>
		/// <param name="typeResolver">The type resolution service to use when looking for a type converter.</param>
		/// <returns>The object that corresponds to the stored value.</returns>
		/// <exception cref="T:System.TypeLoadException">The corresponding type could not be found, or an appropriate type converter is not available.</exception>
		public object GetValue(ITypeResolutionService typeResolver)
		{
			if (this.value != null)
				return this.value;
			object obj = (object)null;
			if (this.FileRefFullPath != null)
			{
				if (this.ResolveType(this.FileRefType, typeResolver) != (System.Type)null)
				{
					this.fileRef = this.FileRefTextEncoding == null ? new ResXFileRef(this.FileRefFullPath, this.FileRefType) : new ResXFileRef(this.FileRefFullPath, this.FileRefType, Encoding.GetEncoding(this.FileRefTextEncoding));
					return TypeDescriptor.GetConverter(typeof(ResXFileRef)).ConvertFrom((object)this.fileRef.ToString());
				}
				throw new TypeLoadException(SR.GetString(SR.Type_Load_Exception_Short, (object)this.FileRefType));
			}
			return obj == null && this.nodeInfo.ValueData != null ? this.GenerateObjectFromDataNodeInfo(this.nodeInfo, typeResolver) : (object)null;
		}

		/// <summary>Retrieves the object that is stored by this node by searching the specified assemblies.</summary>
		/// <param name="names">The list of assemblies to search for the type of the object.</param>
		/// <returns>The object that corresponds to the stored value.</returns>
		/// <exception cref="T:System.TypeLoadException">The corresponding type could not be found, or an appropriate type converter is not available.</exception>
		public object GetValue(AssemblyName[] names)
		{
			return this.GetValue((ITypeResolutionService)new AssemblyNamesTypeResolutionService(names));
		}

		private static byte[] FromBase64WrappedString(string text)
		{
			if (text.IndexOfAny(ResXDataNode.SpecialChars) == -1)
				return Convert.FromBase64String(text);
			StringBuilder stringBuilder = new StringBuilder(text.Length);
			for (int index = 0; index < text.Length; ++index)
			{
				switch (text[index])
				{
					case '\n':
					case '\r':
					case ' ':
						continue;
					default:
						stringBuilder.Append(text[index]);
						continue;
				}
			}
			return Convert.FromBase64String(stringBuilder.ToString());
		}

		private System.Type ResolveType(string typeName, ITypeResolutionService typeResolver)
		{
			System.Type type = (System.Type)null;
			if (typeResolver != null)
			{
				type = typeResolver.GetType(typeName, false);
				if (type == (System.Type)null)
				{
					string[] strArray = typeName.Split(',');
					if (strArray != null && strArray.Length >= 2)
					{
						string name = strArray[0].Trim() + ", " + strArray[1].Trim();
						type = typeResolver.GetType(name, false);
					}
				}
			}
			if (type == (System.Type)null)
				type = System.Type.GetType(typeName, false);
			return type;
		}

		/// <summary>Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo" /> object with the data needed to serialize the target object.</summary>
		/// <param name="si">An  object to populate with data.</param>
		/// <param name="context">The destination for this serialization.</param>
		void ISerializable.GetObjectData(SerializationInfo si, StreamingContext context)
		{
			DataNodeInfo dataNodeInfo = this.GetDataNodeInfo();
			si.AddValue("Name", (object)dataNodeInfo.Name, typeof(string));
			si.AddValue("Comment", (object)dataNodeInfo.Comment, typeof(string));
			si.AddValue("TypeName", (object)dataNodeInfo.TypeName, typeof(string));
			si.AddValue("MimeType", (object)dataNodeInfo.MimeType, typeof(string));
			si.AddValue("ValueData", (object)dataNodeInfo.ValueData, typeof(string));
		}

		private ResXDataNode(SerializationInfo info, StreamingContext context)
		{
			this.nodeInfo = new DataNodeInfo()
			{
				Name = (string)info.GetValue(nameof(Name), typeof(string)),
				Comment = (string)info.GetValue(nameof(Comment), typeof(string)),
				TypeName = (string)info.GetValue("TypeName", typeof(string)),
				MimeType = (string)info.GetValue("MimeType", typeof(string)),
				ValueData = (string)info.GetValue("ValueData", typeof(string))
			};
			this.InitializeDataNode((string)null);
		}
	}
}
#endif