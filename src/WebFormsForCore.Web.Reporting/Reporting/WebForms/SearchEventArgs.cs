// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.SearchEventArgs
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

using System.ComponentModel;
using System.Runtime.InteropServices;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  [ComVisible(false)]
  public sealed class SearchEventArgs : CancelEventArgs
  {
    private string m_searchString;
    private int m_startPage;
    private bool m_isFindNext;

    public SearchEventArgs(string searchString, int startPage, bool isFindNext)
    {
      this.m_searchString = searchString;
      this.m_startPage = startPage;
      this.m_isFindNext = isFindNext;
    }

    public string SearchString => this.m_searchString;

    public int StartPage => this.m_startPage;

    public bool IsFindNext => this.m_isFindNext;
  }
}
