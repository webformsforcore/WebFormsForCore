// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution.LoadReportDefinition2CompletedEventArgs
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

#nullable disable
namespace Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution
{
  [GeneratedCode("wsdl", "2.0.50727.42")]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [DebuggerStepThrough]
  [DesignerCategory("code")]
  public class LoadReportDefinition2CompletedEventArgs : AsyncCompletedEventArgs
  {
    private object[] results;

    internal LoadReportDefinition2CompletedEventArgs(
      object[] results,
      Exception exception,
      bool cancelled,
      object userState)
      : base(exception, cancelled, userState)
    {
      this.results = results;
    }

    public ExecutionInfo2 Result
    {
      get
      {
        this.RaiseExceptionIfNecessary();
        return (ExecutionInfo2) this.results[0];
      }
    }

    public Warning[] warnings
    {
      get
      {
        this.RaiseExceptionIfNecessary();
        return (Warning[]) this.results[1];
      }
    }
  }
}
