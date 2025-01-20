
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

#nullable disable
namespace Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution
{
  [DebuggerStepThrough]
  [GeneratedCode("wsdl", "2.0.50727.42")]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [DesignerCategory("code")]
  [XmlType(Namespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices")]
  [Serializable]
  public class DataSourceCredentials
  {
    private string dataSourceNameField;
    private string userNameField;
    private string passwordField;

    public string DataSourceName
    {
      get => this.dataSourceNameField;
      set => this.dataSourceNameField = value;
    }

    public string UserName
    {
      get => this.userNameField;
      set => this.userNameField = value;
    }

    public string Password
    {
      get => this.passwordField;
      set => this.passwordField = value;
    }
  }
}
