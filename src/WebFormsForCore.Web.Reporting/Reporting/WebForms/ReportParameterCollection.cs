
using System;
using System.Collections.ObjectModel;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  public sealed class ReportParameterCollection : Collection<ReportParameter>
  {
    public ReportParameter this[string name]
    {
      get
      {
        foreach (ReportParameter reportParameter in (Collection<ReportParameter>) this)
        {
          string name1 = reportParameter.Name;
          if (string.Compare(name, name1, StringComparison.OrdinalIgnoreCase) == 0)
            return reportParameter;
        }
        return (ReportParameter) null;
      }
    }
  }
}
