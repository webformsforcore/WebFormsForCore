extern alias Common;

#if !NETFRAMEWORK

using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;

#nullable disable
namespace System.Resources
{
	/// <summary>Represents a link to an external resource.</summary>
	[TypeConverter(typeof(ResXFileRef.Converter))]
	[Serializable]
	[PermissionSet(SecurityAction.InheritanceDemand, Name = "FullTrust")]
	[PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
	public class ResXFileRef
	{
		private string fileName;
		private string typeName;
		[OptionalField(VersionAdded = 2)]
		private Encoding textFileEncoding;

		/// <summary>Creates a new instance of the <see cref="T:System.Resources.ResXFileRef" /> class that references the specified file.</summary>
		/// <param name="fileName">The file to reference.</param>
		/// <param name="typeName">The type of the resource that is referenced.</param>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="fileName" /> or <paramref name="typeName" /> is <see langword="null" />.</exception>
		public ResXFileRef(string fileName, string typeName)
		{
			if (fileName == null)
				throw new ArgumentNullException(nameof(fileName));
			if (typeName == null)
				throw new ArgumentNullException(nameof(typeName));
			this.fileName = fileName;
			this.typeName = typeName;
		}

		[System.Runtime.Serialization.OnDeserializing]
		private void OnDeserializing(StreamingContext ctx) => this.textFileEncoding = (Encoding)null;

		[System.Runtime.Serialization.OnDeserialized]
		private void OnDeserialized(StreamingContext ctx)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Resources.ResXFileRef" /> class that references the specified file.</summary>
		/// <param name="fileName">The file to reference.</param>
		/// <param name="typeName">The type name of the resource that is referenced.</param>
		/// <param name="textFileEncoding">The encoding used in the referenced file.</param>
		public ResXFileRef(string fileName, string typeName, Encoding textFileEncoding)
		  : this(fileName, typeName)
		{
			this.textFileEncoding = textFileEncoding;
		}

		internal ResXFileRef Clone()
		{
			return new ResXFileRef(this.fileName, this.typeName, this.textFileEncoding);
		}

		/// <summary>Gets the file name specified in the current <see cref="Overload:System.Resources.ResXFileRef.#ctor" /> constructor.</summary>
		/// <returns>The name of the referenced file.</returns>
		public string FileName => this.fileName;

		/// <summary>Gets the type name specified in the current <see cref="Overload:System.Resources.ResXFileRef.#ctor" /> constructor.</summary>
		/// <returns>The type name of the resource that is referenced.</returns>
		public string TypeName => this.typeName;

		/// <summary>Gets the encoding specified in the current <see cref="Overload:System.Resources.ResXFileRef.#ctor" /> constructor.</summary>
		/// <returns>The encoding used in the referenced file.</returns>
		public Encoding TextFileEncoding => this.textFileEncoding;

		private static string PathDifference(string path1, string path2, bool compareCase)
		{
			int num = -1;
			int index;
			for (index = 0; index < path1.Length && index < path2.Length && ((int)path1[index] == (int)path2[index] || !compareCase && (int)char.ToLower(path1[index], CultureInfo.InvariantCulture) == (int)char.ToLower(path2[index], CultureInfo.InvariantCulture)); ++index)
			{
				if ((int)path1[index] == (int)Path.DirectorySeparatorChar)
					num = index;
			}
			if (index == 0)
				return path2;
			if (index == path1.Length && index == path2.Length)
				return string.Empty;
			StringBuilder stringBuilder = new StringBuilder();
			for (; index < path1.Length; ++index)
			{
				if ((int)path1[index] == (int)Path.DirectorySeparatorChar)
					stringBuilder.Append(".." + Path.DirectorySeparatorChar.ToString());
			}
			return stringBuilder.ToString() + path2.Substring(num + 1);
		}

		internal void MakeFilePathRelative(string basePath)
		{
			switch (basePath)
			{
				case null:
					break;
				case "":
					break;
				default:
					this.fileName = ResXFileRef.PathDifference(basePath, this.fileName, false);
					break;
			}
		}

		/// <summary>Gets the text representation of the current <see cref="T:System.Resources.ResXFileRef" /> object.</summary>
		/// <returns>A string that consists of the concatenated text representations of the parameters specified in the current <see cref="Overload:System.Resources.ResXFileRef.#ctor" /> constructor.</returns>
		public override string ToString()
		{
			string str1 = "";
			string str2 = (this.fileName.IndexOf(";") != -1 || this.fileName.IndexOf("\"") != -1 ? str1 + "\"" + this.fileName + "\";" : str1 + this.fileName + ";") + this.typeName;
			if (this.textFileEncoding != null)
				str2 = str2 + ";" + this.textFileEncoding.WebName;
			return str2;
		}

		/// <summary>Provides a type converter to convert data for a <see cref="T:System.Resources.ResXFileRef" /> to and from a string.</summary>
		[PermissionSet(SecurityAction.InheritanceDemand, Name = "FullTrust")]
		[PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
		public class Converter : TypeConverter
		{
			/// <summary>Returns whether this converter can convert an object of the given type to the type of this converter, using the specified context.</summary>
			/// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.</param>
			/// <param name="sourceType">A <see cref="T:System.Type" /> that represents the type you want to convert from.</param>
			/// <returns>
			/// <see langword="true" /> if this converter can perform the conversion; otherwise, <see langword="false" />.</returns>
			public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
			{
				return sourceType == typeof(string);
			}

			/// <summary>Returns whether this converter can convert the object to the specified type, using the specified context.</summary>
			/// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.</param>
			/// <param name="destinationType">A <see cref="T:System.Type" /> that represents the type you want to convert to.</param>
			/// <returns>
			/// <see langword="true" /> if this converter can perform the conversion; otherwise, <see langword="false" />.</returns>
			public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
			{
				return destinationType == typeof(string);
			}

			/// <summary>Converts the given value object to the specified type, using the specified context and culture information.</summary>
			/// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.</param>
			/// <param name="culture">A <see cref="T:System.Globalization.CultureInfo" />. If null is passed, the current culture is assumed.</param>
			/// <param name="value">The <see cref="T:System.Object" /> to convert.</param>
			/// <param name="destinationType">The <see cref="T:System.Type" /> to convert the value parameter to.</param>
			/// <returns>An <see cref="T:System.Object" /> that represents the converted value.</returns>
			public override object ConvertTo(
			  ITypeDescriptorContext context,
			  CultureInfo culture,
			  object value,
			  Type destinationType)
			{
				object obj = (object)null;
				if (destinationType == typeof(string))
					obj = (object)((ResXFileRef)value).ToString();
				return obj;
			}

			internal static string[] ParseResxFileRefString(string stringValue)
			{
				string[] resxFileRefString = (string[])null;
				if (stringValue != null)
				{
					stringValue = stringValue.Trim();
					string str1;
					string str2;
					if (stringValue.StartsWith("\""))
					{
						int num = stringValue.LastIndexOf("\"");
						str1 = num - 1 >= 0 ? stringValue.Substring(1, num - 1) : throw new ArgumentException("value");
						if (num + 2 > stringValue.Length)
							throw new ArgumentException("value");
						str2 = stringValue.Substring(num + 2);
					}
					else
					{
						int length = stringValue.IndexOf(";");
						str1 = length != -1 ? stringValue.Substring(0, length) : throw new ArgumentException("value");
						if (length + 1 > stringValue.Length)
							throw new ArgumentException("value");
						str2 = stringValue.Substring(length + 1);
					}
					string[] strArray = str2.Split(';');
					if (strArray.Length > 1)
						resxFileRefString = new string[3]
						{
			  str1,
			  strArray[0],
			  strArray[1]
						};
					else if (strArray.Length != 0)
						resxFileRefString = new string[2]
						{
			  str1,
			  strArray[0]
						};
					else
						resxFileRefString = new string[1] { str1 };
				}
				return resxFileRefString;
			}


			private object GetIconSkia(Stream stream)
			{
				throw new NotImplementedException();
			}


			private object GetIconWindows(Stream stream)
			{
				return (object)new Common.System.Drawing.Icon(stream).ToBitmap();
			}


			private object GetIcon(Stream stream) => OSInfo.IsWindows ? GetIconWindows(stream) : GetIconSkia(stream);
			/// <summary>Converts the given object to the type of this converter, using the specified context and culture information.</summary>
			/// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.</param>
			/// <param name="culture">The <see cref="T:System.Globalization.CultureInfo" /> to use as the current culture.</param>
			/// <param name="value">The <see cref="T:System.Object" /> to convert.</param>
			/// <returns>An <see cref="T:System.Object" /> that represents the converted value.</returns>
			public override object ConvertFrom(
				ITypeDescriptorContext context,
				CultureInfo culture,
				object value)
			{
				object obj = (object)null;
				if (value is string stringValue)
				{
					string[] resxFileRefString = ResXFileRef.Converter.ParseResxFileRefString(stringValue);
					string path = resxFileRefString[0];
					Type type = Type.GetType(resxFileRefString[1], true);
					if (type.Equals(typeof(string)))
					{
						Encoding encoding = Encoding.Default;
						if (resxFileRefString.Length > 2)
							encoding = Encoding.GetEncoding(resxFileRefString[2]);
						using (StreamReader streamReader = new StreamReader(path, encoding))
							obj = (object)streamReader.ReadToEnd();
					}
					else
					{
						byte[] buffer = (byte[])null;
						using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
						{
							buffer = new byte[fileStream.Length];
							fileStream.Read(buffer, 0, (int)fileStream.Length);
						}
						if (type.Equals(typeof(byte[])))
						{
							obj = (object)buffer;
						}
						else
						{
							MemoryStream memoryStream = new MemoryStream(buffer);
							if (type.Equals(typeof(MemoryStream)))
								return (object)memoryStream;
							if (type.FullName == "System.Drawing.Bitmap" && path.EndsWith(".ico"))
								obj = GetIcon((Stream)memoryStream);
							else
								obj = Activator.CreateInstance(type, BindingFlags.Instance | BindingFlags.Public | BindingFlags.CreateInstance, (Binder)null, new object[1]
								{
				  (object) memoryStream
								}, (CultureInfo)null);
						}
					}
				}
				return obj;
			}
		}
	}
}
#endif