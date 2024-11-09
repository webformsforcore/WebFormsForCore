// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.PageNavigationEventArgs
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

using System.ComponentModel;
using System.Runtime.InteropServices;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  [ComVisible(false)]
  public sealed class PageNavigationEventArgs : CancelEventArgs
  {
    private int m_newPage;

    public PageNavigationEventArgs(int newPage) => this.m_newPage = newPage;

    public int NewPage => this.m_newPage;
  }
}
