
using System;
using System.Runtime.Serialization;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  [Serializable]
  public sealed class SoapVersionMismatchException : ReportServerException
  {
    internal SoapVersionMismatchException(string message, Exception innerException)
      : base(message, (string) null, innerException)
    {
    }

    private SoapVersionMismatchException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
