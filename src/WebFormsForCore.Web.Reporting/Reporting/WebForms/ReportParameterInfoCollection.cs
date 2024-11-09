// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.ReportParameterInfoCollection
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

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
