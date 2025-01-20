
using System;
using System.Configuration;
using System.Runtime.Serialization;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  [Serializable]
  public sealed class MissingReportServerConnectionInformationException : 
    ConfigurationErrorsException
  {
    internal MissingReportServerConnectionInformationException()
      : base(Microsoft.Reporting.WebForms.Errors.SessionOrConfig)
    {
    }

    private MissingReportServerConnectionInformationException(
      SerializationInfo info,
      StreamingContext context)
      : base(info, context)
    {
    }
  }
}
