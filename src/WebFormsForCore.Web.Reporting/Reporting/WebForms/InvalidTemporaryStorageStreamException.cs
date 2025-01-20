
using System;
using System.Configuration;
using System.Runtime.Serialization;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  [Serializable]
  public sealed class InvalidTemporaryStorageStreamException : ConfigurationErrorsException
  {
    internal InvalidTemporaryStorageStreamException()
      : base(Microsoft.Reporting.WebForms.Errors.TempStorageNeedsSeekReadWrite)
    {
    }

    private InvalidTemporaryStorageStreamException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
