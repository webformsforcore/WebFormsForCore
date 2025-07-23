using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution;

[GeneratedCode("wsdl", "2.0.50727.42")]
[EditorBrowsable(EditorBrowsableState.Never)]
[DebuggerStepThrough]
[DesignerCategory("code")]
public class Render2CompletedEventArgs : AsyncCompletedEventArgs
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

	public string Extension
	{
		get
		{
			RaiseExceptionIfNecessary();
			return (string)results[1];
		}
	}

	public string MimeType
	{
		get
		{
			RaiseExceptionIfNecessary();
			return (string)results[2];
		}
	}

	public string Encoding
	{
		get
		{
			RaiseExceptionIfNecessary();
			return (string)results[3];
		}
	}

	public Warning[] Warnings
	{
		get
		{
			RaiseExceptionIfNecessary();
			return (Warning[])results[4];
		}
	}

	public string[] StreamIds
	{
		get
		{
			RaiseExceptionIfNecessary();
			return (string[])results[5];
		}
	}

	internal Render2CompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState)
		: base(exception, cancelled, userState)
	{
		this.results = results;
	}
}
