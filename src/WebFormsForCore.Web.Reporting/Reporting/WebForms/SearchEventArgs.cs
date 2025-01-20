
using System.ComponentModel;
using System.Runtime.InteropServices;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  [ComVisible(false)]
  public sealed class SearchEventArgs : CancelEventArgs
  {
    private string m_searchString;
    private int m_startPage;
    private bool m_isFindNext;

    public SearchEventArgs(string searchString, int startPage, bool isFindNext)
    {
      this.m_searchString = searchString;
      this.m_startPage = startPage;
      this.m_isFindNext = isFindNext;
    }

    public string SearchString => this.m_searchString;

    public int StartPage => this.m_startPage;

    public bool IsFindNext => this.m_isFindNext;
  }
}
