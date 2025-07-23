using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution;

[Serializable]
[DebuggerStepThrough]
[GeneratedCode("wsdl", "2.0.50727.42")]
[EditorBrowsable(EditorBrowsableState.Never)]
[DesignerCategory("code")]
[XmlType(Namespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices")]
public class DataSourcePrompt
{
	private string nameField;

	private string dataSourceIDField;

	private string promptField;

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

	public string DataSourceID
	{
		get
		{
			return dataSourceIDField;
		}
		set
		{
			dataSourceIDField = value;
		}
	}

	public string Prompt
	{
		get
		{
			return promptField;
		}
		set
		{
			promptField = value;
		}
	}
}
