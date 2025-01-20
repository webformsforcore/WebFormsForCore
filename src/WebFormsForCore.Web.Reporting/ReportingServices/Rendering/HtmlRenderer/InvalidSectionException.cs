
using System;
using System.Runtime.Serialization;

#nullable disable
namespace Microsoft.ReportingServices.Rendering.HtmlRenderer
{
  [Serializable]
  internal class InvalidSectionException : Exception
  {
    protected InvalidSectionException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }

    public InvalidSectionException()
      : base(RenderRes.rrInvalidSectionError)
    {
    }
  }
}
