using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Web.Services.Protocols;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution;

[Serializable]
[EditorBrowsable(EditorBrowsableState.Never)]
[XmlRoot(Namespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", IsNullable = false)]
[GeneratedCode("wsdl", "2.0.50727.42")]
[DebuggerStepThrough]
[DesignerCategory("code")]
[XmlType(Namespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices")]
public class ExecutionHeader : SoapHeader
{
	private string executionIDField;

	private XmlAttribute[] anyAttrField;

	public string ExecutionID
	{
		get
		{
			return executionIDField;
		}
		set
		{
			executionIDField = value;
		}
	}

	[XmlAnyAttribute]
	public XmlAttribute[] AnyAttr
	{
		get
		{
			return anyAttrField;
		}
		set
		{
			anyAttrField = value;
		}
	}
}
