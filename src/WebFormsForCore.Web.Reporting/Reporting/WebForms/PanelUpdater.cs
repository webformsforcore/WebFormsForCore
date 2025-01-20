
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
