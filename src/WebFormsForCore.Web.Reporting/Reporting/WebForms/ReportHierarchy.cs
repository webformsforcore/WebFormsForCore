// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.ReportHierarchy
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  [Serializable]
  internal sealed class ReportHierarchy : Stack<ReportInfo>, IDisposable
  {
    public ReportHierarchy(ServerReport serverReport)
    {
      this.Push(new ReportInfo(new LocalModeSession(), new ServerModeSession(serverReport)));
    }

    public void Dispose()
    {
      foreach (ReportInfo reportInfo in (Stack<ReportInfo>) this)
        reportInfo.Dispose();
    }

    public void DisposeNonSessionResources()
    {
      foreach (ReportInfo reportInfo in (Stack<ReportInfo>) this)
        reportInfo.DisposeNonSessionResources();
    }

    public ReportInfo MainReport
    {
      get
      {
        ReportInfo[] array = this.ToArray();
        return array[array.Length - 1];
      }
    }

    public ServerReport LastStaticServerReport
    {
      get
      {
        ReportInfo[] array = this.ToArray();
        for (int index = 0; index < array.Length; ++index)
        {
          if (!string.IsNullOrEmpty(array[index].ServerReport.ReportPath))
            return array[index].ServerReport;
        }
        return (ServerReport) null;
      }
    }

    public void ConnectChangeEvents(
      EventHandler<ReportChangedEventArgs> changeHandler,
      InitializeDataSourcesEventHandler dataInitializationHandler)
    {
      foreach (ReportInfo reportInfo in (Stack<ReportInfo>) this)
        reportInfo.ConnectChangeEvent(changeHandler, dataInitializationHandler);
    }

    public void DisconnectChangeEvents(
      EventHandler<ReportChangedEventArgs> changeHandler,
      InitializeDataSourcesEventHandler dataInitializationHandler)
    {
      foreach (ReportInfo reportInfo in (Stack<ReportInfo>) this)
        reportInfo.DisconnectChangeEvent(changeHandler, dataInitializationHandler, true);
    }

    public void LoadViewState(object viewStateObj)
    {
      object[] objArray = (object[]) viewStateObj;
      this.SyncToClientPage(objArray.Length);
      int num = 0;
      foreach (ReportInfo reportInfo in (Stack<ReportInfo>) this)
        reportInfo.LoadViewState(objArray[num++]);
    }

    public object SaveViewState(bool includeReport)
    {
      object[] objArray = new object[this.Count];
      int num = 0;
      foreach (ReportInfo reportInfo in (Stack<ReportInfo>) this)
        objArray[num++] = reportInfo.SaveViewState(includeReport);
      return (object) objArray;
    }

    public void SyncToClientPage(int clientStackSize)
    {
      if (clientStackSize < 1)
        throw new ArgumentOutOfRangeException(nameof (clientStackSize));
      for (int count = this.Count; count > clientStackSize; --count)
        this.Pop();
    }
  }
}
