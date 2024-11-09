// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.ParameterControlCollection
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal sealed class ParameterControlCollection : Dictionary<string, BaseParameterInputControl>
  {
    private string m_hiddenUnsatisfiedParameterName;
    private string m_anyUnsatisfiedParameterName;
    private bool m_visibleParameterNeedsValue;

    private ParameterControlCollection()
    {
    }

    private static bool IsParamVisible(bool showHiddenParameters, ReportParameterInfo param)
    {
      if (!param.PromptUser)
        return false;
      if (showHiddenParameters)
        return true;
      return param.Prompt != null && param.Prompt.Length > 0 && param.Visible;
    }

    public static ParameterControlCollection Create(
      ReportParameterInfoCollection reportParams,
      bool allowQueryExecution,
      IReportViewerStyles styles,
      bool showHiddenParameters)
    {
      return ParameterControlCollection.Create(reportParams, allowQueryExecution, styles, showHiddenParameters, true);
    }

    public static ParameterControlCollection Create(
      ReportParameterInfoCollection reportParams,
      bool allowQueryExecution,
      IReportViewerStyles styles,
      bool showHiddenParameters,
      bool positioningMode)
    {
      ParameterControlCollection controlCollection = new ParameterControlCollection();
      foreach (ReportParameterInfo reportParam in (ReadOnlyCollection<ReportParameterInfo>) reportParams)
      {
        if (ParameterControlCollection.IsParamVisible(showHiddenParameters, reportParam))
        {
          BaseParameterInputControl parameterInputControl1 = ParameterInputControlFactory.Create(reportParam, allowQueryExecution, positioningMode);
          controlCollection.Add(reportParam.Name, parameterInputControl1);
          if (reportParam.State != ParameterState.HasValidValue)
            controlCollection.m_visibleParameterNeedsValue = true;
          if (reportParam.Dependencies != null)
          {
            foreach (ReportParameterInfo dependency in (ReadOnlyCollection<ReportParameterInfo>) reportParam.Dependencies)
            {
              BaseParameterInputControl parameterInputControl2;
              if (controlCollection.TryGetValue(dependency.Name, out parameterInputControl2))
                parameterInputControl2.AutoPostBack = true;
            }
          }
        }
        else if (reportParam.State != ParameterState.HasValidValue && controlCollection.m_hiddenUnsatisfiedParameterName == null)
          controlCollection.m_hiddenUnsatisfiedParameterName = reportParam.Name;
        if (reportParam.State != ParameterState.HasValidValue && controlCollection.m_anyUnsatisfiedParameterName == null)
          controlCollection.m_anyUnsatisfiedParameterName = reportParam.Name;
      }
      return controlCollection;
    }

    public string HiddenUnsatisfiedParameter => this.m_hiddenUnsatisfiedParameterName;

    public string AnyUnsatisfiedParameter => this.m_anyUnsatisfiedParameterName;

    public bool VisibleParameterNeedsValue => this.m_visibleParameterNeedsValue;
  }
}
