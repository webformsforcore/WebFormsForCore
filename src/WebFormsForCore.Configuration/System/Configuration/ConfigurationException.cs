using System.Configuration.Internal;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;
using System.Xml;

#nullable disable
namespace System.Configuration
{
  /// <summary>The exception that is thrown when a configuration system error has occurred.</summary>
  [Serializable]
  public class ConfigurationException : SystemException
  {
    private const string HTTP_PREFIX = "http:";
    private string _filename;
    private int _line;

    private void Init(string filename, int line)
    {
      this.HResult = -2146232062;
      this._filename = filename;
      this._line = line;
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.Configuration.ConfigurationException" /> class.</summary>
    /// <param name="info">The object that holds the information to deserialize.</param>
    /// <param name="context">Contextual information about the source or destination.</param>
    protected ConfigurationException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
      this.Init(info.GetString("filename"), info.GetInt32("line"));
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.Configuration.ConfigurationException" /> class.</summary>
    [Obsolete("This class is obsolete, to create a new exception create a System.Configuration!System.Configuration.ConfigurationErrorsException")]
    public ConfigurationException()
      : this((string) null, (Exception) null, (string) null, 0)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.Configuration.ConfigurationException" /> class.</summary>
    /// <param name="message">A message describing why this <see cref="T:System.Configuration.ConfigurationException" /> exception was thrown.</param>
    [Obsolete("This class is obsolete, to create a new exception create a System.Configuration!System.Configuration.ConfigurationErrorsException")]
    public ConfigurationException(string message)
      : this(message, (Exception) null, (string) null, 0)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.Configuration.ConfigurationException" /> class.</summary>
    /// <param name="message">A message describing why this <see cref="T:System.Configuration.ConfigurationException" /> exception was thrown.</param>
    /// <param name="inner">The inner exception that caused this <see cref="T:System.Configuration.ConfigurationException" /> to be thrown, if any.</param>
    [Obsolete("This class is obsolete, to create a new exception create a System.Configuration!System.Configuration.ConfigurationErrorsException")]
    public ConfigurationException(string message, Exception inner)
      : this(message, inner, (string) null, 0)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.Configuration.ConfigurationException" /> class.</summary>
    /// <param name="message">A message describing why this <see cref="T:System.Configuration.ConfigurationException" /> exception was thrown.</param>
    /// <param name="node">The <see cref="T:System.Xml.XmlNode" /> that caused this <see cref="T:System.Configuration.ConfigurationException" /> to be thrown.</param>
    [Obsolete("This class is obsolete, to create a new exception create a System.Configuration!System.Configuration.ConfigurationErrorsException")]
    public ConfigurationException(string message, XmlNode node)
      : this(message, (Exception) null, ConfigurationException.GetUnsafeXmlNodeFilename(node), ConfigurationException.GetXmlNodeLineNumber(node))
    {
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.Configuration.ConfigurationException" /> class.</summary>
    /// <param name="message">A message describing why this <see cref="T:System.Configuration.ConfigurationException" /> exception was thrown.</param>
    /// <param name="inner">The inner exception that caused this <see cref="T:System.Configuration.ConfigurationException" /> to be thrown, if any.</param>
    /// <param name="node">The <see cref="T:System.Xml.XmlNode" /> that caused this <see cref="T:System.Configuration.ConfigurationException" /> to be thrown.</param>
    [Obsolete("This class is obsolete, to create a new exception create a System.Configuration!System.Configuration.ConfigurationErrorsException")]
    public ConfigurationException(string message, Exception inner, XmlNode node)
      : this(message, inner, ConfigurationException.GetUnsafeXmlNodeFilename(node), ConfigurationException.GetXmlNodeLineNumber(node))
    {
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.Configuration.ConfigurationException" /> class.</summary>
    /// <param name="message">A message describing why this <see cref="T:System.Configuration.ConfigurationException" /> exception was thrown.</param>
    /// <param name="filename">The path to the configuration file that caused this <see cref="T:System.Configuration.ConfigurationException" /> to be thrown.</param>
    /// <param name="line">The line number within the configuration file at which this <see cref="T:System.Configuration.ConfigurationException" /> was thrown.</param>
    [Obsolete("This class is obsolete, to create a new exception create a System.Configuration!System.Configuration.ConfigurationErrorsException")]
    public ConfigurationException(string message, string filename, int line)
      : this(message, (Exception) null, filename, line)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.Configuration.ConfigurationException" /> class.</summary>
    /// <param name="message">A message describing why this <see cref="T:System.Configuration.ConfigurationException" /> exception was thrown.</param>
    /// <param name="inner">The inner exception that caused this <see cref="T:System.Configuration.ConfigurationException" /> to be thrown, if any.</param>
    /// <param name="filename">The path to the configuration file that caused this <see cref="T:System.Configuration.ConfigurationException" /> to be thrown.</param>
    /// <param name="line">The line number within the configuration file at which this <see cref="T:System.Configuration.ConfigurationException" /> was thrown.</param>
    [Obsolete("This class is obsolete, to create a new exception create a System.Configuration!System.Configuration.ConfigurationErrorsException")]
    public ConfigurationException(string message, Exception inner, string filename, int line)
      : base(message, inner)
    {
      this.Init(filename, line);
    }

    /// <summary>Sets the <see cref="T:System.Runtime.Serialization.SerializationInfo" /> object with the file name and line number at which this configuration exception occurred.</summary>
    /// <param name="info">The object that holds the information to be serialized.</param>
    /// <param name="context">The contextual information about the source or destination.</param>
    [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      base.GetObjectData(info, context);
      info.AddValue("filename", (object) this._filename);
      info.AddValue("line", this._line);
    }

    /// <summary>Gets an extended description of why this configuration exception was thrown.</summary>
    /// <returns>An extended description of why this <see cref="T:System.Configuration.ConfigurationException" /> exception was thrown.</returns>
    public override string Message
    {
      get
      {
        string filename = this.Filename;
        if (!string.IsNullOrEmpty(filename))
        {
          if (this.Line == 0)
            return this.BareMessage + " (" + filename + ")";
          return this.BareMessage + " (" + filename + " line " + this.Line.ToString((IFormatProvider) CultureInfo.InvariantCulture) + ")";
        }
        return this.Line != 0 ? this.BareMessage + " (line " + this.Line.ToString("G", (IFormatProvider) CultureInfo.InvariantCulture) + ")" : this.BareMessage;
      }
    }

    /// <summary>Gets a description of why this configuration exception was thrown.</summary>
    /// <returns>A description of why this <see cref="T:System.Configuration.ConfigurationException" /> exception was thrown.</returns>
    public virtual string BareMessage => base.Message;

    /// <summary>Gets the path to the configuration file that caused this configuration exception to be thrown.</summary>
    /// <returns>The path to the configuration file that caused this <see cref="T:System.Configuration.ConfigurationException" /> exception to be thrown.</returns>
    public virtual string Filename => ConfigurationException.SafeFilename(this._filename);

    /// <summary>Gets the line number within the configuration file at which this configuration exception was thrown.</summary>
    /// <returns>The line number within the configuration file at which this <see cref="T:System.Configuration.ConfigurationException" /> exception was thrown.</returns>
    public virtual int Line => this._line;

    /// <summary>Gets the path to the configuration file from which the internal <see cref="T:System.Xml.XmlNode" /> object was loaded when this configuration exception was thrown.</summary>
    /// <param name="node">The <see cref="T:System.Xml.XmlNode" /> that caused this <see cref="T:System.Configuration.ConfigurationException" /> exception to be thrown.</param>
    /// <returns>A <see langword="string" /> representing the node file name.</returns>
    [Obsolete("This class is obsolete, use System.Configuration!System.Configuration.ConfigurationErrorsException.GetFilename instead")]
    public static string GetXmlNodeFilename(XmlNode node)
    {
      return ConfigurationException.SafeFilename(ConfigurationException.GetUnsafeXmlNodeFilename(node));
    }

    /// <summary>Gets the line number within the configuration file that the internal <see cref="T:System.Xml.XmlNode" /> object represented when this configuration exception was thrown.</summary>
    /// <param name="node">The <see cref="T:System.Xml.XmlNode" /> that caused this <see cref="T:System.Configuration.ConfigurationException" /> exception to be thrown.</param>
    /// <returns>An <see langword="int" /> representing the node line number.</returns>
    [Obsolete("This class is obsolete, use System.Configuration!System.Configuration.ConfigurationErrorsException.GetLinenumber instead")]
    public static int GetXmlNodeLineNumber(XmlNode node)
    {
      return node is IConfigErrorInfo configErrorInfo ? configErrorInfo.LineNumber : 0;
    }

    [FileIOPermission(SecurityAction.Assert, AllFiles = FileIOPermissionAccess.PathDiscovery)]
    private static string FullPathWithAssert(string filename)
    {
      string str = (string) null;
      try
      {
        str = Path.GetFullPath(filename);
      }
      catch
      {
      }
      return str;
    }

    internal static string SafeFilename(string filename)
    {
      if (string.IsNullOrEmpty(filename))
        return filename;
      if (filename.StartsWith("http:", StringComparison.OrdinalIgnoreCase))
        return filename;
      try
      {
        if (!Path.IsPathRooted(filename))
          return filename;
      }
      catch
      {
        return (string) null;
      }
      try
      {
        Path.GetFullPath(filename);
      }
      catch (SecurityException ex)
      {
        try
        {
          filename = Path.GetFileName(ConfigurationException.FullPathWithAssert(filename));
        }
        catch
        {
          filename = (string) null;
        }
      }
      catch
      {
        filename = (string) null;
      }
      return filename;
    }

    private static string GetUnsafeXmlNodeFilename(XmlNode node)
    {
      return node is IConfigErrorInfo configErrorInfo ? configErrorInfo.Filename : string.Empty;
    }
  }
}
