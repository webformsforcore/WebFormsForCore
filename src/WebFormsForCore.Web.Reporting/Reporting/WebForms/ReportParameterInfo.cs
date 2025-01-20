
using System.Collections.Generic;
using System.Collections.ObjectModel;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  public sealed class ReportParameterInfo
  {
    private string m_name;
    private ParameterDataType m_dataType;
    private bool m_isNullable;
    private bool m_allowBlank;
    private bool m_isMultiValue;
    private bool m_isQueryParameter;
    private string m_prompt;
    private bool m_promptUser;
    private bool m_areDefaultValuesQueryBased;
    private bool m_areValidValuesQueryBased;
    private string m_errorMessage;
    private IList<ValidValue> m_validValues;
    private IList<string> m_currentValues;
    private ParameterState m_state;
    private ReportParameterInfoCollection m_dependencyCollection;
    private ReportParameterInfoCollection m_dependentsCollection;
    private string[] m_dependencies;
    private List<ReportParameterInfo> m_dependentsCollectionConstruction = new List<ReportParameterInfo>();
    private bool m_visible;

    internal ReportParameterInfo(
      string name,
      ParameterDataType dataType,
      bool isNullable,
      bool allowBlank,
      bool isMultiValue,
      bool isQueryParameter,
      string prompt,
      bool promptUser,
      bool areDefaultValuesQueryBased,
      bool areValidValuesQueryBased,
      string errorMessage,
      string[] currentValues,
      IList<ValidValue> validValues,
      string[] dependencies,
      ParameterState state)
    {
      this.m_name = name;
      this.m_dataType = dataType;
      this.m_isNullable = isNullable;
      this.m_allowBlank = allowBlank;
      this.m_isMultiValue = isMultiValue;
      this.m_isQueryParameter = isQueryParameter;
      this.m_prompt = prompt;
      this.m_promptUser = promptUser;
      this.m_areDefaultValuesQueryBased = areDefaultValuesQueryBased;
      this.m_areValidValuesQueryBased = areValidValuesQueryBased;
      this.m_errorMessage = errorMessage;
      this.m_currentValues = (IList<string>) new ReadOnlyCollection<string>((IList<string>) (currentValues ?? new string[0]));
      this.m_validValues = validValues;
      this.m_dependencies = dependencies;
      this.m_state = state;
      this.m_visible = true;
    }

    internal void SetDependencies(ReportParameterInfoCollection coll)
    {
      if (this.m_dependencyCollection != null)
        return;
      if (this.m_dependencies == null)
      {
        this.m_dependencyCollection = new ReportParameterInfoCollection();
      }
      else
      {
        List<ReportParameterInfo> parameterInfos = new List<ReportParameterInfo>();
        foreach (string dependency in this.m_dependencies)
        {
          ReportParameterInfo reportParameterInfo = coll[dependency];
          if (reportParameterInfo != null)
          {
            parameterInfos.Add(reportParameterInfo);
            reportParameterInfo.m_dependentsCollectionConstruction.Add(this);
          }
        }
        this.m_dependencyCollection = new ReportParameterInfoCollection((IList<ReportParameterInfo>) parameterInfos);
      }
    }

    internal bool HasUnsatisfiedDownstreamParametersWithDefaults
    {
      get
      {
        foreach (ReportParameterInfo dependent in (ReadOnlyCollection<ReportParameterInfo>) this.Dependents)
        {
          if (dependent.AreDefaultValuesQueryBased && dependent.State != ParameterState.HasValidValue)
            return true;
        }
        return false;
      }
    }

    public string Name => this.m_name;

    public ParameterDataType DataType => this.m_dataType;

    public bool Nullable => this.m_isNullable;

    public bool AllowBlank => this.m_allowBlank;

    public bool MultiValue => this.m_isMultiValue;

    public bool IsQueryParameter => this.m_isQueryParameter;

    public string Prompt => this.m_prompt;

    public bool PromptUser => this.m_promptUser;

    public ReportParameterInfoCollection Dependencies => this.m_dependencyCollection;

    public ReportParameterInfoCollection Dependents
    {
      get
      {
        if (this.m_dependentsCollection == null)
          this.m_dependentsCollection = new ReportParameterInfoCollection((IList<ReportParameterInfo>) this.m_dependentsCollectionConstruction);
        return this.m_dependentsCollection;
      }
    }

    public bool AreValidValuesQueryBased => this.m_areValidValuesQueryBased;

    public IList<ValidValue> ValidValues => this.m_validValues;

    public bool AreDefaultValuesQueryBased => this.m_areDefaultValuesQueryBased;

    public IList<string> Values => this.m_currentValues;

    public ParameterState State => this.m_state;

    public string ErrorMessage => this.m_errorMessage;

    public bool Visible
    {
      get => this.m_visible;
      internal set => this.m_visible = value;
    }
  }
}
