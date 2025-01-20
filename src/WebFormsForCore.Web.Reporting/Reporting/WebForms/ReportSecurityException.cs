
using System;
using System.Runtime.Serialization;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  [Serializable]
  public sealed class ReportSecurityException : ReportViewerException
  {
    internal ReportSecurityException(string message)
      : base(message)
    {
    }

    private ReportSecurityException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
