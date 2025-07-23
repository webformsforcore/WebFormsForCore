using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution;

[EditorBrowsable(EditorBrowsableState.Never)]
[GeneratedCode("wsdl", "2.0.50727.42")]
[DesignerCategory("code")]
[DebuggerStepThrough]
public class LoadDrillthroughTargetCompletedEventArgs : AsyncCompletedEventArgs
{
	private object[] results;

	public ExecutionInfo Result
	{
		get
		{
			RaiseExceptionIfNecessary();
			return (ExecutionInfo)results[0];
		}
	}

	internal LoadDrillthroughTargetCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState)
		: base(exception, cancelled, userState)
	{
		this.results = results;
	}
}
