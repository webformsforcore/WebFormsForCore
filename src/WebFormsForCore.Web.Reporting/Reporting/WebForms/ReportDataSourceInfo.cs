
using System;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  [Serializable]
  public sealed class ReportDataSourceInfo
  {
    private string m_name;
    private string m_prompt;

    internal ReportDataSourceInfo(string name, string prompt)
    {
      this.m_name = name;
      this.m_prompt = prompt;
    }

    public string Name => this.m_name;

    public string Prompt => this.m_prompt;
  }
}
