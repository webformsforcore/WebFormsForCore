using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution;

[GeneratedCode("wsdl", "2.0.50727.42")]
[EditorBrowsable(EditorBrowsableState.Never)]
[DebuggerStepThrough]
[DesignerCategory("code")]
public class LoadReportDefinition2CompletedEventArgs : AsyncCompletedEventArgs
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

	public Warning[] warnings
	{
		get
		{
			RaiseExceptionIfNecessary();
			return (Warning[])results[1];
		}
	}

	internal LoadReportDefinition2CompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState)
		: base(exception, cancelled, userState)
	{
		this.results = results;
	}
}
