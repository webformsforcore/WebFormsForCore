// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.ClientCanceledStateChangeEventArgs
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

using System;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal sealed class ClientCanceledStateChangeEventArgs : EventArgs
  {
    private bool m_clientCanceled;

    public ClientCanceledStateChangeEventArgs(bool clientCanceled)
    {
      this.m_clientCanceled = clientCanceled;
    }

    public bool ClientCanceled => this.m_clientCanceled;
  }
}
