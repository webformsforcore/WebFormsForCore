
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
  [DesignerCategory("code")]
  [XmlType(Namespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices")]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public class ParameterValue : ParameterValueOrFieldReference
  {
    private string nameField;
    private string valueField;
    private string labelField;

    public string Name
    {
      get => this.nameField;
      set => this.nameField = value;
    }

    public string Value
    {
      get => this.valueField;
      set => this.valueField = value;
    }

    public string Label
    {
      get => this.labelField;
      set => this.labelField = value;
    }
  }
}
