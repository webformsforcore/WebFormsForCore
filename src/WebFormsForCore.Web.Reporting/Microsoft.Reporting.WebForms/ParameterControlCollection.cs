using System.Collections.Generic;

namespace Microsoft.Reporting.WebForms;

internal sealed class ParameterControlCollection : Dictionary<string, BaseParameterInputControl>
{
	private string m_hiddenUnsatisfiedParameterName;

	private string m_anyUnsatisfiedParameterName;

	private bool m_visibleParameterNeedsValue;

	public string HiddenUnsatisfiedParameter => m_hiddenUnsatisfiedParameterName;

	public string AnyUnsatisfiedParameter => m_anyUnsatisfiedParameterName;

	public bool VisibleParameterNeedsValue => m_visibleParameterNeedsValue;

	private ParameterControlCollection()
	{
	}

	private static bool IsParamVisible(bool showHiddenParameters, ReportParameterInfo param)
	{
		if (!param.PromptUser)
		{
			return false;
		}
		if (!showHiddenParameters)
		{
			if (param.Prompt != null && param.Prompt.Length > 0)
			{
				return param.Visible;
			}
			return false;
		}
		return true;
	}

	public static ParameterControlCollection Create(ReportParameterInfoCollection reportParams, bool allowQueryExecution, IReportViewerStyles styles, bool showHiddenParameters)
	{
		return Create(reportParams, allowQueryExecution, styles, showHiddenParameters, positioningMode: true);
	}

	public static ParameterControlCollection Create(ReportParameterInfoCollection reportParams, bool allowQueryExecution, IReportViewerStyles styles, bool showHiddenParameters, bool positioningMode)
	{
		ParameterControlCollection parameterControlCollection = new ParameterControlCollection();
		foreach (ReportParameterInfo reportParam in reportParams)
		{
			if (IsParamVisible(showHiddenParameters, reportParam))
			{
				BaseParameterInputControl value = ParameterInputControlFactory.Create(reportParam, allowQueryExecution, positioningMode);
				parameterControlCollection.Add(reportParam.Name, value);
				if (reportParam.State != ParameterState.HasValidValue)
				{
					parameterControlCollection.m_visibleParameterNeedsValue = true;
				}
				if (reportParam.Dependencies != null)
				{
					foreach (ReportParameterInfo dependency in reportParam.Dependencies)
					{
						if (parameterControlCollection.TryGetValue(dependency.Name, out var value2))
						{
							value2.AutoPostBack = true;
						}
					}
				}
			}
			else if (reportParam.State != ParameterState.HasValidValue && parameterControlCollection.m_hiddenUnsatisfiedParameterName == null)
			{
				parameterControlCollection.m_hiddenUnsatisfiedParameterName = reportParam.Name;
			}
			if (reportParam.State != ParameterState.HasValidValue && parameterControlCollection.m_anyUnsatisfiedParameterName == null)
			{
				parameterControlCollection.m_anyUnsatisfiedParameterName = reportParam.Name;
			}
		}
		return parameterControlCollection;
	}
}
