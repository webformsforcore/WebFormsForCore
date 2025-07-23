using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution;

[DesignerCategory("code")]
[EditorBrowsable(EditorBrowsableState.Never)]
[GeneratedCode("wsdl", "2.0.50727.42")]
[DebuggerStepThrough]
public class ListSecureMethodsCompletedEventArgs : AsyncCompletedEventArgs
{
	private object[] results;

	public string[] Result
	{
		get
		{
			RaiseExceptionIfNecessary();
			return (string[])results[0];
		}
	}

	internal ListSecureMethodsCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState)
		: base(exception, cancelled, userState)
	{
		this.results = results;
	}
}
