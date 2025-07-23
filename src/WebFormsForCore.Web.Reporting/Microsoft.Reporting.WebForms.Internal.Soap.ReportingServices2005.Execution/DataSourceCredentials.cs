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
public class DataSourceCredentials
{
	private string dataSourceNameField;

	private string userNameField;

	private string passwordField;

	public string DataSourceName
	{
		get
		{
			return dataSourceNameField;
		}
		set
		{
			dataSourceNameField = value;
		}
	}

	public string UserName
	{
		get
		{
			return userNameField;
		}
		set
		{
			userNameField = value;
		}
	}

	public string Password
	{
		get
		{
			return passwordField;
		}
		set
		{
			passwordField = value;
		}
	}
}
