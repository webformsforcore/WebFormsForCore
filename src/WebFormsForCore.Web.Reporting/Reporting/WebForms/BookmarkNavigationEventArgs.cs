
using System.ComponentModel;
using System.Runtime.InteropServices;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  [ComVisible(false)]
  public sealed class BookmarkNavigationEventArgs : CancelEventArgs
  {
    private string m_bookmarkId;

    public BookmarkNavigationEventArgs(string bookmarkId) => this.m_bookmarkId = bookmarkId;

    public string BookmarkId => this.m_bookmarkId;
  }
}
