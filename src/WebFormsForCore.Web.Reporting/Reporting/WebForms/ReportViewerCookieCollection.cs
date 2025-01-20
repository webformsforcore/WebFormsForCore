
using System;
using System.Net;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  [Serializable]
  public sealed class ReportViewerCookieCollection : SyncList<Cookie>
  {
    internal ReportViewerCookieCollection(object syncObject)
      : base(syncObject)
    {
    }
  }
}
