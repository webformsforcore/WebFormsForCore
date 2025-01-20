
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

#nullable disable
namespace Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [XmlType(Namespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices")]
  [GeneratedCode("wsdl", "2.0.50727.42")]
  [DebuggerStepThrough]
  [DesignerCategory("code")]
  [Serializable]
  public class ReportPaperSize
  {
    private double heightField;
    private double widthField;

    public double Height
    {
      get => this.heightField;
      set => this.heightField = value;
    }

    public double Width
    {
      get => this.widthField;
      set => this.widthField = value;
    }
  }
}
