
using System;
using System.Runtime.Serialization;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  [Serializable]
  public sealed class AspNetSessionExpiredException : ReportViewerException
  {
    internal AspNetSessionExpiredException()
      : base(Errors.ASPNetSessionExpired)
    {
    }

    private AspNetSessionExpiredException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
