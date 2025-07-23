using System.Collections.Generic;
using System.Web.UI;

namespace Microsoft.Reporting.WebForms;

internal sealed class PanelUpdater
{
	private Dictionary<UpdatePanel, UpdateGroup> m_panelMap = new Dictionary<UpdatePanel, UpdateGroup>();

	private UpdateGroup m_groupsToUpdate;

	public bool IsAnyPanelGroupMarkedForUpdate => m_groupsToUpdate > UpdateGroup.None;

	public void RegisterPanel(UpdatePanel panel, UpdateGroup groupForMembership)
	{
		m_panelMap.Add(panel, groupForMembership);
	}

	public void UnregisterAllPanels()
	{
		m_panelMap.Clear();
	}

	public void MarkPanelsForUpdate(UpdateGroup group)
	{
		m_groupsToUpdate |= group;
	}

	public bool IsPanelGroupMarkedForUpdate(UpdateGroup group)
	{
		return (m_groupsToUpdate & group) > UpdateGroup.None;
	}

	public void CancelAllUpdates()
	{
		m_groupsToUpdate = UpdateGroup.None;
	}

	public void PerformUpdates()
	{
		foreach (KeyValuePair<UpdatePanel, UpdateGroup> item in m_panelMap)
		{
			if ((m_groupsToUpdate & item.Value) == item.Value)
			{
				item.Key.Update();
			}
		}
	}
}
