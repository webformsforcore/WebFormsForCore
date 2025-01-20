
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  [ComVisible(false)]
  public sealed class ReportParameterInfoCollection : ReadOnlyCollection<ReportParameterInfo>
  {
    internal ReportParameterInfoCollection(IList<ReportParameterInfo> parameterInfos)
      : base(parameterInfos)
    {
      foreach (ReportParameterInfo reportParameterInfo in (ReadOnlyCollection<ReportParameterInfo>) this)
        reportParameterInfo.SetDependencies(this);
    }

    internal ReportParameterInfoCollection()
      : base((IList<ReportParameterInfo>) new ReportParameterInfo[0])
    {
    }

    public ReportParameterInfo this[string name]
    {
      get
      {
        foreach (ReportParameterInfo reportParameterInfo in (ReadOnlyCollection<ReportParameterInfo>) this)
        {
          if (string.Compare(reportParameterInfo.Name, name, StringComparison.OrdinalIgnoreCase) == 0)
            return reportParameterInfo;
        }
        return (ReportParameterInfo) null;
      }
    }
  }
}
