
using System.ComponentModel;
using System.Runtime.InteropServices;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  [ComVisible(false)]
  public class ReportCredentialsEventArgs : CancelEventArgs
  {
    private DataSourceCredentialsCollection m_credentials;

    internal ReportCredentialsEventArgs(DataSourceCredentialsCollection credentials)
    {
      this.m_credentials = credentials;
    }

    public DataSourceCredentialsCollection Credentials => this.m_credentials;
  }
}
