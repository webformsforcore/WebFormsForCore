using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution;

[Serializable]
[GeneratedCode("wsdl", "2.0.50727.42")]
[EditorBrowsable(EditorBrowsableState.Never)]
[DebuggerStepThrough]
[DesignerCategory("code")]
[XmlType(Namespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices")]
public class Warning
{
	private string codeField;

	private string severityField;

	private string objectNameField;

	private string objectTypeField;

	private string messageField;

	public string Code
	{
		get
		{
			return codeField;
		}
		set
		{
			codeField = value;
		}
	}

	public string Severity
	{
		get
		{
			return severityField;
		}
		set
		{
			severityField = value;
		}
	}

	public string ObjectName
	{
		get
		{
			return objectNameField;
		}
		set
		{
			objectNameField = value;
		}
	}

	public string ObjectType
	{
		get
		{
			return objectTypeField;
		}
		set
		{
			objectTypeField = value;
		}
	}

	public string Message
	{
		get
		{
			return messageField;
		}
		set
		{
			messageField = value;
		}
	}
}
