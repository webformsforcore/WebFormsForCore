
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
  public class ReportParameter
  {
    private string nameField;
    private ParameterTypeEnum typeField;
    private bool typeFieldSpecified;
    private bool nullableField;
    private bool nullableFieldSpecified;
    private bool allowBlankField;
    private bool allowBlankFieldSpecified;
    private bool multiValueField;
    private bool multiValueFieldSpecified;
    private bool queryParameterField;
    private bool queryParameterFieldSpecified;
    private string promptField;
    private bool promptUserField;
    private bool promptUserFieldSpecified;
    private string[] dependenciesField;
    private bool validValuesQueryBasedField;
    private bool validValuesQueryBasedFieldSpecified;
    private ValidValue[] validValuesField;
    private bool defaultValuesQueryBasedField;
    private bool defaultValuesQueryBasedFieldSpecified;
    private string[] defaultValuesField;
    private ParameterStateEnum stateField;
    private bool stateFieldSpecified;
    private string errorMessageField;

    public string Name
    {
      get => this.nameField;
      set => this.nameField = value;
    }

    public ParameterTypeEnum Type
    {
      get => this.typeField;
      set => this.typeField = value;
    }

    [XmlIgnore]
    public bool TypeSpecified
    {
      get => this.typeFieldSpecified;
      set => this.typeFieldSpecified = value;
    }

    public bool Nullable
    {
      get => this.nullableField;
      set => this.nullableField = value;
    }

    [XmlIgnore]
    public bool NullableSpecified
    {
      get => this.nullableFieldSpecified;
      set => this.nullableFieldSpecified = value;
    }

    public bool AllowBlank
    {
      get => this.allowBlankField;
      set => this.allowBlankField = value;
    }

    [XmlIgnore]
    public bool AllowBlankSpecified
    {
      get => this.allowBlankFieldSpecified;
      set => this.allowBlankFieldSpecified = value;
    }

    public bool MultiValue
    {
      get => this.multiValueField;
      set => this.multiValueField = value;
    }

    [XmlIgnore]
    public bool MultiValueSpecified
    {
      get => this.multiValueFieldSpecified;
      set => this.multiValueFieldSpecified = value;
    }

    public bool QueryParameter
    {
      get => this.queryParameterField;
      set => this.queryParameterField = value;
    }

    [XmlIgnore]
    public bool QueryParameterSpecified
    {
      get => this.queryParameterFieldSpecified;
      set => this.queryParameterFieldSpecified = value;
    }

    public string Prompt
    {
      get => this.promptField;
      set => this.promptField = value;
    }

    public bool PromptUser
    {
      get => this.promptUserField;
      set => this.promptUserField = value;
    }

    [XmlIgnore]
    public bool PromptUserSpecified
    {
      get => this.promptUserFieldSpecified;
      set => this.promptUserFieldSpecified = value;
    }

    [XmlArrayItem("Dependency")]
    public string[] Dependencies
    {
      get => this.dependenciesField;
      set => this.dependenciesField = value;
    }

    public bool ValidValuesQueryBased
    {
      get => this.validValuesQueryBasedField;
      set => this.validValuesQueryBasedField = value;
    }

    [XmlIgnore]
    public bool ValidValuesQueryBasedSpecified
    {
      get => this.validValuesQueryBasedFieldSpecified;
      set => this.validValuesQueryBasedFieldSpecified = value;
    }

    public ValidValue[] ValidValues
    {
      get => this.validValuesField;
      set => this.validValuesField = value;
    }

    public bool DefaultValuesQueryBased
    {
      get => this.defaultValuesQueryBasedField;
      set => this.defaultValuesQueryBasedField = value;
    }

    [XmlIgnore]
    public bool DefaultValuesQueryBasedSpecified
    {
      get => this.defaultValuesQueryBasedFieldSpecified;
      set => this.defaultValuesQueryBasedFieldSpecified = value;
    }

    [XmlArrayItem("Value")]
    public string[] DefaultValues
    {
      get => this.defaultValuesField;
      set => this.defaultValuesField = value;
    }

    public ParameterStateEnum State
    {
      get => this.stateField;
      set => this.stateField = value;
    }

    [XmlIgnore]
    public bool StateSpecified
    {
      get => this.stateFieldSpecified;
      set => this.stateFieldSpecified = value;
    }

    public string ErrorMessage
    {
      get => this.errorMessageField;
      set => this.errorMessageField = value;
    }
  }
}
