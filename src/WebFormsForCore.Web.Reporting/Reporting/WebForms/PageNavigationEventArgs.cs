
using System.ComponentModel;
using System.Runtime.InteropServices;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  [ComVisible(false)]
  public sealed class PageNavigationEventArgs : CancelEventArgs
  {
    private int m_newPage;

    public PageNavigationEventArgs(int newPage) => this.m_newPage = newPage;

    public int NewPage => this.m_newPage;
  }
}
