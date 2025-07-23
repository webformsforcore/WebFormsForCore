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
public class ValidValue
{
	private string labelField;

	private string valueField;

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
}
