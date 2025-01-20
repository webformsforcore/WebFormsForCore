
using System;
using System.Runtime.Serialization;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  [Serializable]
  internal sealed class HttpHandlerInputException : ReportViewerException
  {
    public HttpHandlerInputException(Exception e)
      : base(new ArgumentException().Message, e)
    {
    }

    public HttpHandlerInputException(string message)
      : base(message)
    {
    }

    private HttpHandlerInputException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
