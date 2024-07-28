
using System.Runtime.Serialization;

#nullable disable
namespace System.Configuration
{
  /// <summary>Provides an exception that is thrown when an invalid type is used with a <see cref="T:System.Configuration.SettingsProperty" /> object.</summary>
  [Serializable]
  public class SettingsPropertyWrongTypeException : Exception
  {
    /// <summary>Initializes a new instance of the <see cref="T:System.Configuration.SettingsPropertyWrongTypeException" /> class based on the supplied parameter.</summary>
    /// <param name="message">A string containing an exception message.</param>
    public SettingsPropertyWrongTypeException(string message)
      : base(message)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.Configuration.SettingsPropertyWrongTypeException" /> class based on the supplied parameters.</summary>
    /// <param name="message">A string containing an exception message.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public SettingsPropertyWrongTypeException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.Configuration.SettingsPropertyWrongTypeException" /> class based on the supplied parameters.</summary>
    /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
    /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination of the serialized stream.</param>
    protected SettingsPropertyWrongTypeException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.Configuration.SettingsPropertyWrongTypeException" /> class.</summary>
    public SettingsPropertyWrongTypeException()
    {
    }
  }
}
