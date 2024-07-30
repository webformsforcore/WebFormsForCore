
using System.Runtime.Serialization;

#nullable disable
namespace System.Configuration
{
  /// <summary>Provides an exception for read-only <see cref="T:System.Configuration.SettingsProperty" /> objects.</summary>
  [Serializable]
  public class SettingsPropertyIsReadOnlyException : Exception
  {
    /// <summary>Initializes a new instance of the <see cref="T:System.Configuration.SettingsPropertyIsReadOnlyException" /> class based on a supplied parameter.</summary>
    /// <param name="message">A string containing an exception message.</param>
    public SettingsPropertyIsReadOnlyException(string message)
      : base(message)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.Configuration.SettingsPropertyIsReadOnlyException" /> class based on supplied parameters.</summary>
    /// <param name="message">A string containing an exception message.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public SettingsPropertyIsReadOnlyException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.Configuration.SettingsPropertyIsReadOnlyException" /> class based on the supplied parameters.</summary>
    /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> object that holds the serialized object data about the exception being thrown.</param>
    /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> object that contains contextual information about the source or destination of the serialized stream.</param>
    protected SettingsPropertyIsReadOnlyException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.Configuration.SettingsPropertyIsReadOnlyException" /> class.</summary>
    public SettingsPropertyIsReadOnlyException()
    {
    }
  }
}
