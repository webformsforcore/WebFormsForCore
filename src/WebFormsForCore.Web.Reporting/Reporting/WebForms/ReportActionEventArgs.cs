// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.ReportActionEventArgs
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

using System;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal class ReportActionEventArgs : EventArgs
  {
    private string m_actionType;
    private string m_actionParam;

    internal ReportActionEventArgs(string actionType, string actionParam)
    {
      this.m_actionType = actionType;
      this.m_actionParam = actionParam;
    }

    public string ActionType => this.m_actionType;

    public string ActionParam => this.m_actionParam;
  }
}
