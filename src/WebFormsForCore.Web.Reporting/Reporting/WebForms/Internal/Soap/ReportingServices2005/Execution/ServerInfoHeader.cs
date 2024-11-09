// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution.ServerInfoHeader
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Web.Services.Protocols;
using System.Xml;
using System.Xml.Serialization;

#nullable disable
namespace Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution
{
  [GeneratedCode("wsdl", "2.0.50727.42")]
  [DebuggerStepThrough]
  [DesignerCategory("code")]
  [XmlType(Namespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices")]
  [XmlRoot(Namespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", IsNullable = false)]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public class ServerInfoHeader : SoapHeader
  {
    private string reportServerVersionNumberField;
    private string reportServerEditionField;
    private string reportServerVersionField;
    private string reportServerDateTimeField;
    private XmlAttribute[] anyAttrField;

    public string ReportServerVersionNumber
    {
      get => this.reportServerVersionNumberField;
      set => this.reportServerVersionNumberField = value;
    }

    public string ReportServerEdition
    {
      get => this.reportServerEditionField;
      set => this.reportServerEditionField = value;
    }

    public string ReportServerVersion
    {
      get => this.reportServerVersionField;
      set => this.reportServerVersionField = value;
    }

    public string ReportServerDateTime
    {
      get => this.reportServerDateTimeField;
      set => this.reportServerDateTimeField = value;
    }

    [XmlAnyAttribute]
    public XmlAttribute[] AnyAttr
    {
      get => this.anyAttrField;
      set => this.anyAttrField = value;
    }
  }
}
