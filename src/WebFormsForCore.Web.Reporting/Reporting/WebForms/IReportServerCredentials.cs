
using System.Net;
using System.Security.Principal;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  public interface IReportServerCredentials
  {
    WindowsIdentity ImpersonationUser { get; }

    ICredentials NetworkCredentials { get; }

    bool GetFormsCredentials(
      out Cookie authCookie,
      out string userName,
      out string password,
      out string authority);
  }
}
