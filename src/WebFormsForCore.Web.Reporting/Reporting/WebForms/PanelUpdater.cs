// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.PanelUpdater
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

using System.Collections.Generic;
using System.Web.UI;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal sealed class PanelUpdater
  {
    private Dictionary<UpdatePanel, UpdateGroup> m_panelMap = new Dictionary<UpdatePanel, UpdateGroup>();
    private UpdateGroup m_groupsToUpdate;

    public void RegisterPanel(UpdatePanel panel, UpdateGroup groupForMembership)
    {
      this.m_panelMap.Add(panel, groupForMembership);
    }

    public void UnregisterAllPanels() => this.m_panelMap.Clear();

    public void MarkPanelsForUpdate(UpdateGroup group) => this.m_groupsToUpdate |= group;

    public bool IsAnyPanelGroupMarkedForUpdate => this.m_groupsToUpdate > UpdateGroup.None;

    public bool IsPanelGroupMarkedForUpdate(UpdateGroup group)
    {
      return (this.m_groupsToUpdate & group) > UpdateGroup.None;
    }

    public void CancelAllUpdates() => this.m_groupsToUpdate = UpdateGroup.None;

    public void PerformUpdates()
    {
      foreach (KeyValuePair<UpdatePanel, UpdateGroup> panel in this.m_panelMap)
      {
        if ((this.m_groupsToUpdate & panel.Value) == panel.Value)
          panel.Key.Update();
      }
    }
  }
}
