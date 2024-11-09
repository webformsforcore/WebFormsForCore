// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution.ExecutionInfo2
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

#nullable disable
namespace Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution
{
  [DesignerCategory("code")]
  [XmlType(Namespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices")]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [DebuggerStepThrough]
  [GeneratedCode("wsdl", "2.0.50727.42")]
  [Serializable]
  public class ExecutionInfo2 : ExecutionInfo
  {
    private PageCountMode pageCountModeField;

    public PageCountMode PageCountMode
    {
      get => this.pageCountModeField;
      set => this.pageCountModeField = value;
    }
  }
}
