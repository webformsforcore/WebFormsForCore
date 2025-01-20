
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

#nullable disable
namespace Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution
{
  [DesignerCategory("code")]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [GeneratedCode("wsdl", "2.0.50727.42")]
  [DebuggerStepThrough]
  [XmlType(Namespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices")]
  [Serializable]
  public class DocumentMapNode
  {
    private string labelField;
    private string uniqueNameField;
    private DocumentMapNode[] childrenField;

    public string Label
    {
      get => this.labelField;
      set => this.labelField = value;
    }

    public string UniqueName
    {
      get => this.uniqueNameField;
      set => this.uniqueNameField = value;
    }

    public DocumentMapNode[] Children
    {
      get => this.childrenField;
      set => this.childrenField = value;
    }
  }
}
