
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Xml.Serialization;

#nullable disable
namespace Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution
{
  [XmlType(Namespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices")]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [GeneratedCode("wsdl", "2.0.50727.42")]
  [Serializable]
  public enum ParameterTypeEnum
  {
    Boolean,
    DateTime,
    Integer,
    Float,
    String,
  }
}
