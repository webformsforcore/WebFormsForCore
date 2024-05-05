// Decompiled with JetBrains decompiler
// Type: System.Configuration.SettingsPropertyNotFoundException
// Assembly: System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 84F5A18A-F2B1-435C-B86E-09CE162E61E4
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System\v4.0_4.0.0.0__b77a5c561934e089\System.dll
// XML documentation location: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1\System.xml

using System.Runtime.Serialization;

#nullable disable
namespace System.Configuration
{
  /// <summary>Provides an exception for <see cref="T:System.Configuration.SettingsProperty" /> objects that are not found.</summary>
  [Serializable]
  public class SettingsPropertyNotFoundException : Exception
  {
    /// <summary>Initializes a new instance of the <see cref="T:System.Configuration.SettingsPropertyNotFoundException" /> class, based on a supplied parameter.</summary>
    /// <param name="message">A string containing an exception message.</param>
    public SettingsPropertyNotFoundException(string message)
      : base(message)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.Configuration.SettingsPropertyNotFoundException" /> class, based on supplied parameters.</summary>
    /// <param name="message">A string containing an exception message.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public SettingsPropertyNotFoundException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.Configuration.SettingsPropertyNotFoundException" /> class, based on supplied parameters.</summary>
    /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> object that holds the serialized object data about the exception being thrown.</param>
    /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> object that contains contextual information about the source or destination of the serialized stream.</param>
    protected SettingsPropertyNotFoundException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.Configuration.SettingsPropertyNotFoundException" /> class.</summary>
    public SettingsPropertyNotFoundException()
    {
    }
  }
}
