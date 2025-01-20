
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

#nullable disable
namespace Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution
{
  [GeneratedCode("wsdl", "2.0.50727.42")]
  [DesignerCategory("code")]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [DebuggerStepThrough]
  public class Sort2CompletedEventArgs : AsyncCompletedEventArgs
  {
    private object[] results;

    internal Sort2CompletedEventArgs(
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

    public ExecutionInfo2 ExecutionInfo
    {
      get
      {
        this.RaiseExceptionIfNecessary();
        return (ExecutionInfo2) this.results[2];
      }
    }
  }
}
