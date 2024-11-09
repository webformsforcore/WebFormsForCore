// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution.Extension
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
  [XmlType(Namespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices")]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [DesignerCategory("code")]
  [DebuggerStepThrough]
  [Serializable]
  public class Extension
  {
    private ExtensionTypeEnum extensionTypeField;
    private string nameField;
    private string localizedNameField;
    private bool visibleField;
    private bool isModelGenerationSupportedField;

    public ExtensionTypeEnum ExtensionType
    {
      get => this.extensionTypeField;
      set => this.extensionTypeField = value;
    }

    public string Name
    {
      get => this.nameField;
      set => this.nameField = value;
    }

    public string LocalizedName
    {
      get => this.localizedNameField;
      set => this.localizedNameField = value;
    }

    public bool Visible
    {
      get => this.visibleField;
      set => this.visibleField = value;
    }

    public bool IsModelGenerationSupported
    {
      get => this.isModelGenerationSupportedField;
      set => this.isModelGenerationSupportedField = value;
    }
  }
}
