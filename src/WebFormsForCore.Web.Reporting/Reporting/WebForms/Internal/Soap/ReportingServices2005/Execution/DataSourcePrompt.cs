
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
  public class DataSourcePrompt
  {
    private string nameField;
    private string dataSourceIDField;
    private string promptField;

    public string Name
    {
      get => this.nameField;
      set => this.nameField = value;
    }

    public string DataSourceID
    {
      get => this.dataSourceIDField;
      set => this.dataSourceIDField = value;
    }

    public string Prompt
    {
      get => this.promptField;
      set => this.promptField = value;
    }
  }
}
