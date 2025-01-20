
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

#nullable disable
namespace Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution
{
  [GeneratedCode("wsdl", "2.0.50727.42")]
  [DebuggerStepThrough]
  [DesignerCategory("code")]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class GetDocumentMapCompletedEventArgs : AsyncCompletedEventArgs
  {
    private object[] results;

    internal GetDocumentMapCompletedEventArgs(
      object[] results,
      Exception exception,
      bool cancelled,
      object userState)
      : base(exception, cancelled, userState)
    {
      this.results = results;
    }

    public DocumentMapNode Result
    {
      get
      {
        this.RaiseExceptionIfNecessary();
        return (DocumentMapNode) this.results[0];
      }
    }
  }
}
