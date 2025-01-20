
using System;
using System.Runtime.Serialization;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  [Serializable]
  public sealed class MissingParameterException : ReportViewerException
  {
    internal MissingParameterException(string parameterName)
      : base(CommonStrings.MissingParameter(parameterName))
    {
      // ISSUE: reference to a compiler-generated method (out of statement scope)
    }

    private MissingParameterException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
