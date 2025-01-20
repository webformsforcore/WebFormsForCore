
using System;
using System.Configuration;
using System.Runtime.Serialization;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  [Serializable]
  public sealed class SessionDisabledException : ConfigurationErrorsException
  {
    internal SessionDisabledException()
      : base(Microsoft.Reporting.WebForms.Errors.SessionDisabled)
    {
    }

    private SessionDisabledException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
