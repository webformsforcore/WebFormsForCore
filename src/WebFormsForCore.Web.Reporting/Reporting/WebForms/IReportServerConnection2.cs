
using System.Collections.Generic;
using System.Net;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  public interface IReportServerConnection2 : IReportServerConnection, IReportServerCredentials
  {
    IEnumerable<Cookie> Cookies { get; }

    IEnumerable<string> Headers { get; }
  }
}
