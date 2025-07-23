using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Web.UI;

namespace Microsoft.Reporting.WebForms;

internal sealed class AsyncWaitControlTriggerCollection : Collection<Control>
{
	protected override void InsertItem(int index, Control control)
	{
		if (!Contains(control))
		{
			base.InsertItem(index, control);
		}
	}

	public string[] ToClientIDArray()
	{
		List<string> list = new List<string>();
		using (IEnumerator<Control> enumerator = GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				Control current = enumerator.Current;
				list.Add(current.ClientID);
			}
		}
		return list.ToArray();
	}
}
