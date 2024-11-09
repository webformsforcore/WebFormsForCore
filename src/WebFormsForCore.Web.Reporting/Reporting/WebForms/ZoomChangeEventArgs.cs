// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.ZoomChangeEventArgs
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

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
