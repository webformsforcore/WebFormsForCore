using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Microsoft.Reporting.WebForms;

internal abstract class CompositeScriptControl : CompositeControl, IScriptControl
{
	protected override HtmlTextWriterTag TagKey => HtmlTextWriterTag.Div;

	protected ScriptManager ScriptManager => ScriptManager.GetCurrent(Page);

	protected override void OnPreRender(EventArgs e)
	{
		base.OnPreRender(e);
		ScriptManager.RegisterScriptControl(this);
	}

	protected override void Render(HtmlTextWriter writer)
	{
		base.Render(writer);
		if (!base.DesignMode)
		{
			ScriptManager.RegisterScriptDescriptors(this);
		}
	}

	public abstract IEnumerable<ScriptDescriptor> GetScriptDescriptors();

	public virtual IEnumerable<ScriptReference> GetScriptReferences()
	{
		ScriptReference scriptReference = new ScriptReference(EmbeddedResourceOperation.CreateUrlForScriptFile());
		return new ScriptReference[1] { scriptReference };
	}
}
