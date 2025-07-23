using System;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Microsoft.Reporting.WebForms;

internal abstract class MultiValueSelector : CompositeControl
{
	public string ClientSideObjectName;

	public abstract string[] Value { get; set; }

	public abstract bool HasValue { get; }

	public event EventHandler Change;

	protected void OnChange(object sender, EventArgs e)
	{
		if (this.Change != null)
		{
			this.Change(this, e);
		}
	}

	public virtual void AddScriptDescriptors(ScriptControlDescriptor desc)
	{
	}

	protected void SetSelectorBorder()
	{
		if (string.IsNullOrEmpty(CssClass))
		{
			BorderStyle = BorderStyle.Solid;
			BorderColor = Color.DarkGray;
			BorderWidth = Unit.Pixel(1);
		}
	}
}
