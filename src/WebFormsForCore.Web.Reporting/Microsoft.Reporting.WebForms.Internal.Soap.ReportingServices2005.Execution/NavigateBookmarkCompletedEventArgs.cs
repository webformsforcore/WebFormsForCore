using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution;

[GeneratedCode("wsdl", "2.0.50727.42")]
[EditorBrowsable(EditorBrowsableState.Never)]
[DebuggerStepThrough]
[DesignerCategory("code")]
public class NavigateBookmarkCompletedEventArgs : AsyncCompletedEventArgs
{
	private object[] results;

	public int Result
	{
		get
		{
			RaiseExceptionIfNecessary();
			return (int)results[0];
		}
	}

	public string UniqueName
	{
		get
		{
			RaiseExceptionIfNecessary();
			return (string)results[1];
		}
	}

	internal NavigateBookmarkCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState)
		: base(exception, cancelled, userState)
	{
		this.results = results;
	}
}
