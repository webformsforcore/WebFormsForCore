using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution;

[Serializable]
[DebuggerStepThrough]
[EditorBrowsable(EditorBrowsableState.Never)]
[XmlType(Namespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices")]
[GeneratedCode("wsdl", "2.0.50727.42")]
[DesignerCategory("code")]
public class PageSettings
{
	private ReportPaperSize paperSizeField;

	private ReportMargins marginsField;

	public ReportPaperSize PaperSize
	{
		get
		{
			return paperSizeField;
		}
		set
		{
			paperSizeField = value;
		}
	}

	public ReportMargins Margins
	{
		get
		{
			return marginsField;
		}
		set
		{
			marginsField = value;
		}
	}
}
