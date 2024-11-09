// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.SubreportProcessingEventArgs
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  public sealed class SubreportProcessingEventArgs : EventArgs
  {
    private string m_subReportName;
    private ReportParameterInfoCollection m_paramMetaData;
    private IList<string> m_dsNames;
    private ReportParameter[] m_userParams = new ReportParameter[0];
    private ReportDataSourceCollection m_dataSources = new ReportDataSourceCollection(new object());

    internal SubreportProcessingEventArgs(
      string subreportName,
      ReportParameterInfoCollection paramMetaData,
      string[] dataSetNames)
    {
      this.m_subReportName = subreportName;
      this.m_paramMetaData = paramMetaData;
      this.m_dsNames = (IList<string>) new ReadOnlyCollection<string>((IList<string>) dataSetNames);
    }

    public string ReportPath => this.m_subReportName;

    public ReportParameterInfoCollection Parameters => this.m_paramMetaData;

    public IList<string> DataSourceNames => this.m_dsNames;

    public ReportDataSourceCollection DataSources => this.m_dataSources;
  }
}
