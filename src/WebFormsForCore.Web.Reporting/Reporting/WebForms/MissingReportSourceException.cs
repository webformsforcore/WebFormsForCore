
using System;
using System.Runtime.Serialization;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  [Serializable]
  public sealed class MissingReportSourceException : ReportViewerException
  {
    public MissingReportSourceException()
      : base(CommonStrings.MissingReportSource)
    {
    }

    private MissingReportSourceException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
