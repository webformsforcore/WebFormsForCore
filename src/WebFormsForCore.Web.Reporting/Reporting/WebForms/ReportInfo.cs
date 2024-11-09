// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.ReportInfo
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

using System;
using System.Drawing.Printing;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  [Serializable]
  internal sealed class ReportInfo : IDisposable
  {
    [NonSerialized]
    public int CurrentPage;
    [NonSerialized]
    public string ScrollPosition;
    [NonSerialized]
    public PageSettings PageSettings;
    private static int PAGESETTINGS_COUNT = 3;
    private LocalModeSession m_localSession;
    private ServerModeSession m_serverSession;

    public ReportInfo(LocalModeSession localSession, ServerModeSession serverSession)
    {
      this.m_localSession = localSession;
      this.m_serverSession = serverSession;
    }

    public void Dispose()
    {
      this.m_localSession.Dispose();
      this.m_serverSession.Dispose();
    }

    public void DisposeNonSessionResources()
    {
      ReportInfo.DisposeNonSessionResources(this.m_localSession, this.m_serverSession);
    }

    public static void DisposeNonSessionResources(
      LocalModeSession localSession,
      ServerModeSession serverSession)
    {
      ((LocalReport) localSession?.Report).ReleaseSandboxAppDomain();
    }

    public void LoadViewState(object viewStateObj)
    {
      object[] objArray = (object[]) viewStateObj;
      this.CurrentPage = (int) objArray[0];
      if (objArray[1] != null)
        this.ServerReport.LoadViewState(objArray[1]);
      this.DeserializePageSettings(objArray[2]);
      this.ScrollPosition = (string) objArray[3];
    }

    public object SaveViewState(bool includeReport)
    {
      object[] objArray = new object[4]
      {
        (object) this.CurrentPage,
        null,
        null,
        null
      };
      if (includeReport)
        objArray[1] = this.ServerReport.SaveViewState();
      objArray[2] = this.SerializePageSettings();
      objArray[3] = (object) this.ScrollPosition;
      return (object) objArray;
    }

    public void ConnectChangeEvent(
      EventHandler<ReportChangedEventArgs> changeHandler,
      InitializeDataSourcesEventHandler dataInitializationHandler)
    {
      this.ServerReport.Change += changeHandler;
      this.LocalReport.Change += changeHandler;
      this.LocalReport.InitializeDataSources += dataInitializationHandler;
    }

    public void DisconnectChangeEvent(
      EventHandler<ReportChangedEventArgs> changeHandler,
      InitializeDataSourcesEventHandler dataInitializationHandler,
      bool disconnectUserEvents)
    {
      this.ServerReport.Change -= changeHandler;
      this.LocalReport.Change -= changeHandler;
      this.LocalReport.InitializeDataSources -= dataInitializationHandler;
      if (!disconnectUserEvents)
        return;
      this.LocalReport.TransferEvents((LocalReport) null);
    }

    public LocalModeSession LocalSession => this.m_localSession;

    public ServerModeSession ServerSession => this.m_serverSession;

    public LocalReport LocalReport => (LocalReport) this.m_localSession.Report;

    public ServerReport ServerReport => (ServerReport) this.m_serverSession.Report;

    private object SerializePageSettings()
    {
      if (this.PageSettings == null)
        return (object) null;
      object[] objArray = new object[ReportInfo.PAGESETTINGS_COUNT];
      objArray[0] = (object) this.PageSettings.Margins;
      objArray[1] = (object) this.PageSettings.PaperSize;
      objArray[2] = (object) this.PageSettings.Landscape;
      return (object) objArray;
    }

    private void DeserializePageSettings(object pageSettings)
    {
      if (!(pageSettings is object[] objArray) || objArray.Length != ReportInfo.PAGESETTINGS_COUNT)
      {
        this.PageSettings = (PageSettings) null;
      }
      else
      {
        this.PageSettings = new PageSettings();
        this.PageSettings.Margins = objArray[0] as Margins;
        this.PageSettings.PaperSize = objArray[1] as PaperSize;
        this.PageSettings.Landscape = (bool) objArray[2];
      }
    }
  }
}
