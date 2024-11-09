// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.SortEventArgs
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

using System.ComponentModel;
using System.Runtime.InteropServices;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  [ComVisible(false)]
  public sealed class SortEventArgs : CancelEventArgs
  {
    private string m_sortId;
    private SortOrder m_sortDirection;
    private bool m_clearSort;

    public SortEventArgs(string sortId, SortOrder sortDirection, bool clearSort)
    {
      this.m_sortId = sortId;
      this.m_sortDirection = sortDirection;
      this.m_clearSort = clearSort;
    }

    public string SortId => this.m_sortId;

    public SortOrder SortDirection => this.m_sortDirection;

    public bool ClearSort => this.m_clearSort;
  }
}
