using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution;

[GeneratedCode("wsdl", "2.0.50727.42")]
[EditorBrowsable(EditorBrowsableState.Never)]
[DesignerCategory("code")]
[DebuggerStepThrough]
public class ToggleItemCompletedEventArgs : AsyncCompletedEventArgs
{
	private object[] results;

	public bool Result
	{
		get
		{
			RaiseExceptionIfNecessary();
			return (bool)results[0];
		}
	}

	internal ToggleItemCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState)
		: base(exception, cancelled, userState)
	{
		this.results = results;
	}
}
