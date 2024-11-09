// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.AbortState
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

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
