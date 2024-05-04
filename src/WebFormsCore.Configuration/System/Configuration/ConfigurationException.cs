#if WebFormsCore && NETCOREAPP

using System.Configuration.Internal;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;
using System.Xml;

namespace System.Configuration;

//
// Summary:
//     The exception that is thrown when a configuration system error has occurred.
[Serializable]
public class ConfigurationException : SystemException
{
	private const string HTTP_PREFIX = "http:";

	private string _filename;

	private int _line;

	//
	// Summary:
	//     Gets an extended description of why this configuration exception was thrown.
	//
	//
	// Returns:
	//     An extended description of why this System.Configuration.ConfigurationException
	//     exception was thrown.
	public override string Message
	{
		get
		{
			string filename = Filename;
			if (!string.IsNullOrEmpty(filename))
			{
				if (Line != 0)
				{
					return BareMessage + " (" + filename + " line " + Line.ToString(CultureInfo.InvariantCulture) + ")";
				}

				return BareMessage + " (" + filename + ")";
			}

			if (Line != 0)
			{
				return BareMessage + " (line " + Line.ToString("G", CultureInfo.InvariantCulture) + ")";
			}

			return BareMessage;
		}
	}

	//
	// Summary:
	//     Gets a description of why this configuration exception was thrown.
	//
	// Returns:
	//     A description of why this System.Configuration.ConfigurationException exception
	//     was thrown.
	public virtual string BareMessage => base.Message;

	//
	// Summary:
	//     Gets the path to the configuration file that caused this configuration exception
	//     to be thrown.
	//
	// Returns:
	//     The path to the configuration file that caused this System.Configuration.ConfigurationException
	//     exception to be thrown.
	public virtual string Filename => SafeFilename(_filename);

	//
	// Summary:
	//     Gets the line number within the configuration file at which this configuration
	//     exception was thrown.
	//
	// Returns:
	//     The line number within the configuration file at which this System.Configuration.ConfigurationException
	//     exception was thrown.
	public virtual int Line => _line;

	private void Init(string filename, int line)
	{
		base.HResult = -2146232062;
		_filename = filename;
		_line = line;
	}

	//
	// Summary:
	//     Initializes a new instance of the System.Configuration.ConfigurationException
	//     class.
	//
	// Parameters:
	//   info:
	//     The object that holds the information to deserialize.
	//
	//   context:
	//     Contextual information about the source or destination.
	protected ConfigurationException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
		Init(info.GetString("filename"), info.GetInt32("line"));
	}

	//
	// Summary:
	//     Initializes a new instance of the System.Configuration.ConfigurationException
	//     class.
	[Obsolete("This class is obsolete, to create a new exception create a System.Configuration!System.Configuration.ConfigurationErrorsException")]
	public ConfigurationException()
		: this(null, null, null, 0)
	{
	}

	//
	// Summary:
	//     Initializes a new instance of the System.Configuration.ConfigurationException
	//     class.
	//
	// Parameters:
	//   message:
	//     A message describing why this System.Configuration.ConfigurationException exception
	//     was thrown.
	[Obsolete("This class is obsolete, to create a new exception create a System.Configuration!System.Configuration.ConfigurationErrorsException")]
	public ConfigurationException(string message)
		: this(message, null, null, 0)
	{
	}

	//
	// Summary:
	//     Initializes a new instance of the System.Configuration.ConfigurationException
	//     class.
	//
	// Parameters:
	//   message:
	//     A message describing why this System.Configuration.ConfigurationException exception
	//     was thrown.
	//
	//   inner:
	//     The inner exception that caused this System.Configuration.ConfigurationException
	//     to be thrown, if any.
	[Obsolete("This class is obsolete, to create a new exception create a System.Configuration!System.Configuration.ConfigurationErrorsException")]
	public ConfigurationException(string message, Exception inner)
		: this(message, inner, null, 0)
	{
	}

	//
	// Summary:
	//     Initializes a new instance of the System.Configuration.ConfigurationException
	//     class.
	//
	// Parameters:
	//   message:
	//     A message describing why this System.Configuration.ConfigurationException exception
	//     was thrown.
	//
	//   node:
	//     The System.Xml.XmlNode that caused this System.Configuration.ConfigurationException
	//     to be thrown.
	[Obsolete("This class is obsolete, to create a new exception create a System.Configuration!System.Configuration.ConfigurationErrorsException")]
	public ConfigurationException(string message, XmlNode node)
		: this(message, null, GetUnsafeXmlNodeFilename(node), GetXmlNodeLineNumber(node))
	{
	}

	//
	// Summary:
	//     Initializes a new instance of the System.Configuration.ConfigurationException
	//     class.
	//
	// Parameters:
	//   message:
	//     A message describing why this System.Configuration.ConfigurationException exception
	//     was thrown.
	//
	//   inner:
	//     The inner exception that caused this System.Configuration.ConfigurationException
	//     to be thrown, if any.
	//
	//   node:
	//     The System.Xml.XmlNode that caused this System.Configuration.ConfigurationException
	//     to be thrown.
	[Obsolete("This class is obsolete, to create a new exception create a System.Configuration!System.Configuration.ConfigurationErrorsException")]
	public ConfigurationException(string message, Exception inner, XmlNode node)
		: this(message, inner, GetUnsafeXmlNodeFilename(node), GetXmlNodeLineNumber(node))
	{
	}

	//
	// Summary:
	//     Initializes a new instance of the System.Configuration.ConfigurationException
	//     class.
	//
	// Parameters:
	//   message:
	//     A message describing why this System.Configuration.ConfigurationException exception
	//     was thrown.
	//
	//   filename:
	//     The path to the configuration file that caused this System.Configuration.ConfigurationException
	//     to be thrown.
	//
	//   line:
	//     The line number within the configuration file at which this System.Configuration.ConfigurationException
	//     was thrown.
	[Obsolete("This class is obsolete, to create a new exception create a System.Configuration!System.Configuration.ConfigurationErrorsException")]
	public ConfigurationException(string message, string filename, int line)
		: this(message, null, filename, line)
	{
	}

	//
	// Summary:
	//     Initializes a new instance of the System.Configuration.ConfigurationException
	//     class.
	//
	// Parameters:
	//   message:
	//     A message describing why this System.Configuration.ConfigurationException exception
	//     was thrown.
	//
	//   inner:
	//     The inner exception that caused this System.Configuration.ConfigurationException
	//     to be thrown, if any.
	//
	//   filename:
	//     The path to the configuration file that caused this System.Configuration.ConfigurationException
	//     to be thrown.
	//
	//   line:
	//     The line number within the configuration file at which this System.Configuration.ConfigurationException
	//     was thrown.
	[Obsolete("This class is obsolete, to create a new exception create a System.Configuration!System.Configuration.ConfigurationErrorsException")]
	public ConfigurationException(string message, Exception inner, string filename, int line)
		: base(message, inner)
	{
		Init(filename, line);
	}

	//
	// Summary:
	//     Sets the System.Runtime.Serialization.SerializationInfo object with the file
	//     name and line number at which this configuration exception occurred.
	//
	// Parameters:
	//   info:
	//     The object that holds the information to be serialized.
	//
	//   context:
	//     The contextual information about the source or destination.
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
		info.AddValue("filename", _filename);
		info.AddValue("line", _line);
	}

	//
	// Summary:
	//     Gets the path to the configuration file from which the internal System.Xml.XmlNode
	//     object was loaded when this configuration exception was thrown.
	//
	// Parameters:
	//   node:
	//     The System.Xml.XmlNode that caused this System.Configuration.ConfigurationException
	//     exception to be thrown.
	//
	// Returns:
	//     A string representing the node file name.
	[Obsolete("This class is obsolete, use System.Configuration!System.Configuration.ConfigurationErrorsException.GetFilename instead")]
	public static string GetXmlNodeFilename(XmlNode node)
	{
		return SafeFilename(GetUnsafeXmlNodeFilename(node));
	}

	//
	// Summary:
	//     Gets the line number within the configuration file that the internal System.Xml.XmlNode
	//     object represented when this configuration exception was thrown.
	//
	// Parameters:
	//   node:
	//     The System.Xml.XmlNode that caused this System.Configuration.ConfigurationException
	//     exception to be thrown.
	//
	// Returns:
	//     An int representing the node line number.
	[Obsolete("This class is obsolete, use System.Configuration!System.Configuration.ConfigurationErrorsException.GetLinenumber instead")]
	public static int GetXmlNodeLineNumber(XmlNode node)
	{
		if (node is IConfigErrorInfo configErrorInfo)
		{
			return configErrorInfo.LineNumber;
		}

		return 0;
	}

	[FileIOPermission(SecurityAction.Assert, AllFiles = FileIOPermissionAccess.PathDiscovery)]
	private static string FullPathWithAssert(string filename)
	{
		string result = null;
		try
		{
			result = Path.GetFullPath(filename);
		}
		catch
		{
		}

		return result;
	}

	internal static string SafeFilename(string filename)
	{
		if (string.IsNullOrEmpty(filename))
		{
			return filename;
		}

		if (filename.StartsWith("http:", StringComparison.OrdinalIgnoreCase))
		{
			return filename;
		}

		try
		{
			if (!Path.IsPathRooted(filename))
			{
				return filename;
			}
		}
		catch
		{
			return null;
		}

		try
		{
			string fullPath = Path.GetFullPath(filename);
		}
		catch (SecurityException)
		{
			try
			{
				string path = FullPathWithAssert(filename);
				filename = Path.GetFileName(path);
			}
			catch
			{
				filename = null;
			}
		}
		catch
		{
			filename = null;
		}

		return filename;
	}

	private static string GetUnsafeXmlNodeFilename(XmlNode node)
	{
		if (node is IConfigErrorInfo configErrorInfo)
		{
			return configErrorInfo.Filename;
		}

		return string.Empty;
	}
}

#endif