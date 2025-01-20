
using System.ComponentModel;
using System.Runtime.InteropServices;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  [ComVisible(false)]
  public sealed class DocumentMapNavigationEventArgs : CancelEventArgs
  {
    private string m_docMapID;

    public DocumentMapNavigationEventArgs(string docMapID) => this.m_docMapID = docMapID;

    public string DocumentMapId => this.m_docMapID;
  }
}
