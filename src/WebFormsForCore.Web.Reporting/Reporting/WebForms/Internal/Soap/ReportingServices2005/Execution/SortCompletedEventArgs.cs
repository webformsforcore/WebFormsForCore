// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution.SortCompletedEventArgs
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
  [DebuggerStepThrough]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [GeneratedCode("wsdl", "2.0.50727.42")]
  [DesignerCategory("code")]
  public class SortCompletedEventArgs : AsyncCompletedEventArgs
  {
    private object[] results;

    internal SortCompletedEventArgs(
      object[] results,
      Exception exception,
      bool cancelled,
      object userState)
      : base(exception, cancelled, userState)
    {
      this.results = results;
    }

    public int Result
    {
      get
      {
        this.RaiseExceptionIfNecessary();
        return (int) this.results[0];
      }
    }

    public string ReportItem
    {
      get
      {
        this.RaiseExceptionIfNecessary();
        return (string) this.results[1];
      }
    }

    public int NumPages
    {
      get
      {
        this.RaiseExceptionIfNecessary();
        return (int) this.results[2];
      }
    }
  }
}
