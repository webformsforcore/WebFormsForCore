
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
  public class NavigateDocumentMapCompletedEventArgs : AsyncCompletedEventArgs
  {
    private object[] results;

    internal NavigateDocumentMapCompletedEventArgs(
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
  }
}
