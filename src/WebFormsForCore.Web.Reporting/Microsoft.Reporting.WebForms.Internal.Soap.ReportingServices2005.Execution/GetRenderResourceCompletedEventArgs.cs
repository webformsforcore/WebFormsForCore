using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution;

[EditorBrowsable(EditorBrowsableState.Never)]
[GeneratedCode("wsdl", "2.0.50727.42")]
[DebuggerStepThrough]
[DesignerCategory("code")]
public class GetRenderResourceCompletedEventArgs : AsyncCompletedEventArgs
{
	private object[] results;

	public byte[] Result
	{
		get
		{
			RaiseExceptionIfNecessary();
			return (byte[])results[0];
		}
	}

	public string MimeType
	{
		get
		{
			RaiseExceptionIfNecessary();
			return (string)results[1];
		}
	}

	internal GetRenderResourceCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState)
		: base(exception, cancelled, userState)
	{
		this.results = results;
	}
}
