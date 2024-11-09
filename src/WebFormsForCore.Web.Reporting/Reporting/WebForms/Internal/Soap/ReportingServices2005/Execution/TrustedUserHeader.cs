// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution.TrustedUserHeader
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
  [XmlType(Namespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices")]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [GeneratedCode("wsdl", "2.0.50727.42")]
  [DebuggerStepThrough]
  [DesignerCategory("code")]
  [XmlRoot(Namespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", IsNullable = false)]
  [Serializable]
  public class TrustedUserHeader : SoapHeader
  {
    private string userNameField;
    private byte[] userTokenField;
    private XmlAttribute[] anyAttrField;

    public string UserName
    {
      get => this.userNameField;
      set => this.userNameField = value;
    }

    [XmlElement(DataType = "base64Binary")]
    public byte[] UserToken
    {
      get => this.userTokenField;
      set => this.userTokenField = value;
    }

    [XmlAnyAttribute]
    public XmlAttribute[] AnyAttr
    {
      get => this.anyAttrField;
      set => this.anyAttrField = value;
    }
  }
}
