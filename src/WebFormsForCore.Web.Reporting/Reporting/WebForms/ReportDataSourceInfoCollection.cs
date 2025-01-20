
using Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  [ComVisible(false)]
  public sealed class ReportDataSourceInfoCollection : ReadOnlyCollection<ReportDataSourceInfo>
  {
    internal ReportDataSourceInfoCollection(IList<ReportDataSourceInfo> dsInfos)
      : base(dsInfos)
    {
    }

    internal ReportDataSourceInfoCollection()
      : base((IList<ReportDataSourceInfo>) new ReportDataSourceInfo[0])
    {
    }

    public ReportDataSourceInfo this[string name]
    {
      get
      {
        foreach (ReportDataSourceInfo reportDataSourceInfo in (ReadOnlyCollection<ReportDataSourceInfo>) this)
        {
          if (string.Compare(reportDataSourceInfo.Name, name, StringComparison.OrdinalIgnoreCase) == 0)
            return reportDataSourceInfo;
        }
        return (ReportDataSourceInfo) null;
      }
    }

    internal static ReportDataSourceInfoCollection FromSoapDataSourcePrompts(
      DataSourcePrompt[] soapPrompts)
    {
      if (soapPrompts == null)
        return new ReportDataSourceInfoCollection();
      ReportDataSourceInfo[] dsInfos = new ReportDataSourceInfo[soapPrompts.Length];
      for (int index = 0; index < soapPrompts.Length; ++index)
        dsInfos[index] = new ReportDataSourceInfo(soapPrompts[index].DataSourceID, soapPrompts[index].Prompt);
      return new ReportDataSourceInfoCollection((IList<ReportDataSourceInfo>) dsInfos);
    }
  }
}
