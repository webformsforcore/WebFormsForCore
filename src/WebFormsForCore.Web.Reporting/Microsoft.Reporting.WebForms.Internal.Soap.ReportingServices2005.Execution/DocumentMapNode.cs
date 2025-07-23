using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution;

[Serializable]
[DesignerCategory("code")]
[EditorBrowsable(EditorBrowsableState.Never)]
[GeneratedCode("wsdl", "2.0.50727.42")]
[DebuggerStepThrough]
[XmlType(Namespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices")]
public class DocumentMapNode
{
	private string labelField;

	private string uniqueNameField;

	private DocumentMapNode[] childrenField;

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

	public string UniqueName
	{
		get
		{
			return uniqueNameField;
		}
		set
		{
			uniqueNameField = value;
		}
	}

	public DocumentMapNode[] Children
	{
		get
		{
			return childrenField;
		}
		set
		{
			childrenField = value;
		}
	}
}
