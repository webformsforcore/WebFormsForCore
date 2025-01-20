
using System;
using System.Runtime.Serialization;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  [Serializable]
  public sealed class MissingDataSourceException : ReportViewerException
  {
    internal MissingDataSourceException(string dataSourceName)
      : base(CommonStrings.MissingDataSource(dataSourceName))
    {
      // ISSUE: reference to a compiler-generated method (out of statement scope)
    }

    private MissingDataSourceException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
