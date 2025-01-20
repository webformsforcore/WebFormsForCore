
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

#nullable disable
namespace Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution
{
  [DesignerCategory("code")]
  [XmlType(Namespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices")]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [DebuggerStepThrough]
  [GeneratedCode("wsdl", "2.0.50727.42")]
  [Serializable]
  public class ExecutionInfo2 : ExecutionInfo
  {
    private PageCountMode pageCountModeField;

    public PageCountMode PageCountMode
    {
      get => this.pageCountModeField;
      set => this.pageCountModeField = value;
    }
  }
}
