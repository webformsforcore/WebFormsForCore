using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution;

[EditorBrowsable(EditorBrowsableState.Never)]
[DesignerCategory("code")]
[GeneratedCode("wsdl", "2.0.50727.42")]
[DebuggerStepThrough]
public class SetExecutionCredentials2CompletedEventArgs : AsyncCompletedEventArgs
{
	private object[] results;

	public ExecutionInfo2 Result
	{
		get
		{
			RaiseExceptionIfNecessary();
			return (ExecutionInfo2)results[0];
		}
	}

	internal SetExecutionCredentials2CompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState)
		: base(exception, cancelled, userState)
	{
		this.results = results;
	}
}
