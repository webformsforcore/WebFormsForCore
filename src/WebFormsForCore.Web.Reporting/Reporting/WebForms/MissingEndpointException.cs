
using System;
using System.Runtime.Serialization;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  [Serializable]
  public sealed class MissingEndpointException : ReportServerException
  {
    internal MissingEndpointException(string message, Exception innerException)
      : base(message, (string) null, innerException)
    {
    }

    private MissingEndpointException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
