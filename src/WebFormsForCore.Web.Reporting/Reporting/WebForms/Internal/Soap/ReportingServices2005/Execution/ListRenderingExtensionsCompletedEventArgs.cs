
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

#nullable disable
namespace Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution
{
  [DebuggerStepThrough]
  [GeneratedCode("wsdl", "2.0.50727.42")]
  [DesignerCategory("code")]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class ListRenderingExtensionsCompletedEventArgs : AsyncCompletedEventArgs
  {
    private object[] results;

    internal ListRenderingExtensionsCompletedEventArgs(
      object[] results,
      Exception exception,
      bool cancelled,
      object userState)
      : base(exception, cancelled, userState)
    {
      this.results = results;
    }

    public Extension[] Result
    {
      get
      {
        this.RaiseExceptionIfNecessary();
        return (Extension[]) this.results[0];
      }
    }
  }
}
