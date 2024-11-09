// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.ReportErrorEventArgs
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

using System;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  public sealed class ReportErrorEventArgs : EventArgs
  {
    private Exception m_exception;
    private bool m_isHandled;

    internal ReportErrorEventArgs(Exception e) => this.m_exception = e;

    public Exception Exception => this.m_exception;

    public bool Handled
    {
      get => this.m_isHandled;
      set => this.m_isHandled = value;
    }
  }
}
