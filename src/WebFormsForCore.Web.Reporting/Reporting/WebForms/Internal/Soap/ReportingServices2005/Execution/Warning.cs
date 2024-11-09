// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution.Warning
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

#nullable disable
namespace Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution
{
  [GeneratedCode("wsdl", "2.0.50727.42")]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [DebuggerStepThrough]
  [DesignerCategory("code")]
  [XmlType(Namespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices")]
  [Serializable]
  public class Warning
  {
    private string codeField;
    private string severityField;
    private string objectNameField;
    private string objectTypeField;
    private string messageField;

    public string Code
    {
      get => this.codeField;
      set => this.codeField = value;
    }

    public string Severity
    {
      get => this.severityField;
      set => this.severityField = value;
    }

    public string ObjectName
    {
      get => this.objectNameField;
      set => this.objectNameField = value;
    }

    public string ObjectType
    {
      get => this.objectTypeField;
      set => this.objectTypeField = value;
    }

    public string Message
    {
      get => this.messageField;
      set => this.messageField = value;
    }
  }
}
