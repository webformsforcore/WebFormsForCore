using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Microsoft.Reporting.WebForms;

internal class ValidatorPanel : WebControl
{
	public bool HasValidatorsToRender
	{
		get
		{
			EnsureChildControls();
			foreach (Control control in Controls)
			{
				if (control.Visible && control.Controls.Count > 0)
				{
					return true;
				}
			}
			return false;
		}
	}

	public string[] ChildControlIds
	{
		get
		{
			EnsureChildControls();
			List<string> list = new List<string>(Controls.Count);
			if (Controls.Count > 0)
			{
				foreach (Control control in Controls)
				{
					list.Add(control.ClientID);
				}
			}
			return list.ToArray();
		}
	}
}
