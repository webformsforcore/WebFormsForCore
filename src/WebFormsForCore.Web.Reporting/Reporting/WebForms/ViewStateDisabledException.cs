
using System;
using System.Runtime.Serialization;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  [Serializable]
  public sealed class ViewStateDisabledException : ReportViewerException
  {
    internal ViewStateDisabledException()
      : base(Errors.ViewStateDisabled)
    {
    }

    private ViewStateDisabledException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
