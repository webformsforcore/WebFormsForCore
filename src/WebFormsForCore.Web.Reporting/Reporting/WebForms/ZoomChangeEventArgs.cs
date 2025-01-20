
using System.ComponentModel;
using System.Runtime.InteropServices;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  [ComVisible(false)]
  internal sealed class ZoomChangeEventArgs : CancelEventArgs
  {
    private int m_zoomPercent;
    private ZoomMode m_zoomMode;

    public ZoomChangeEventArgs(ZoomMode zoomMode, int zoomPercent)
    {
      this.m_zoomMode = zoomMode;
      this.m_zoomPercent = zoomPercent;
    }

    public int ZoomPercent => this.m_zoomPercent;

    public ZoomMode ZoomMode => this.m_zoomMode;
  }
}
