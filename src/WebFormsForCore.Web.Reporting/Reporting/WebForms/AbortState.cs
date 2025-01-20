
using System.Net;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal sealed class AbortState
  {
    private object m_abortLock = new object();
    private bool m_pendingAbort;
    private HttpWebRequest m_abortableRequest;

    public void AbortRequest()
    {
      lock (this.m_abortLock)
      {
        if (this.m_abortableRequest != null)
          this.m_abortableRequest.Abort();
        this.m_pendingAbort = true;
      }
    }

    public bool RegisterAbortableRequest(HttpWebRequest request)
    {
      lock (this.m_abortLock)
      {
        if (this.m_pendingAbort)
          return false;
        this.m_abortableRequest = request;
        return true;
      }
    }

    public void ClearPendingAbort()
    {
      lock (this.m_abortLock)
      {
        this.m_pendingAbort = false;
        this.m_abortableRequest = (HttpWebRequest) null;
      }
    }
  }
}
