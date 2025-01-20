
using System;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  [Serializable]
  public sealed class SearchState
  {
    private string m_text;
    private int m_startPage;

    internal SearchState(string text, int startPage)
    {
      this.m_text = text;
      this.m_startPage = startPage;
    }

    public string Text => this.m_text;

    public int StartPage => this.m_startPage;
  }
}
