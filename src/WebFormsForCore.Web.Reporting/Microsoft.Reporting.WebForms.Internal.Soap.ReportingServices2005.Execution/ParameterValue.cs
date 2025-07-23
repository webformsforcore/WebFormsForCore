using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution;

[Serializable]
[DebuggerStepThrough]
[GeneratedCode("wsdl", "2.0.50727.42")]
[DesignerCategory("code")]
[XmlType(Namespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices")]
[EditorBrowsable(EditorBrowsableState.Never)]
public class ParameterValue : ParameterValueOrFieldReference
{
	private string nameField;

	private string valueField;

	private string labelField;

	public string Name
	{
		get
		{
			return nameField;
		}
		set
		{
			nameField = value;
		}
	}

	public string Value
	{
		get
		{
			return valueField;
		}
		set
		{
			valueField = value;
		}
	}

	public string Label
	{
		get
		{
			return labelField;
		}
		set
		{
			labelField = value;
		}
	}
}
