
using System.ComponentModel;
using System.Runtime.InteropServices;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  [ComVisible(false)]
  public sealed class SortEventArgs : CancelEventArgs
  {
    private string m_sortId;
    private SortOrder m_sortDirection;
    private bool m_clearSort;

    public SortEventArgs(string sortId, SortOrder sortDirection, bool clearSort)
    {
      this.m_sortId = sortId;
      this.m_sortDirection = sortDirection;
      this.m_clearSort = clearSort;
    }

    public string SortId => this.m_sortId;

    public SortOrder SortDirection => this.m_sortDirection;

    public bool ClearSort => this.m_clearSort;
  }
}
