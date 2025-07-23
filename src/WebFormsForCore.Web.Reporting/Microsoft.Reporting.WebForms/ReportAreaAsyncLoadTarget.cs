using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.UI;

namespace Microsoft.Reporting.WebForms;

internal class ReportAreaAsyncLoadTarget : ScriptControl, IPostBackEventHandler
{
	private bool m_causePostBack;

	public event EventHandler PostBackTarget;

	public void TriggerImmediatePostBack()
	{
		m_causePostBack = true;
	}

	void IPostBackEventHandler.RaisePostBackEvent(string eventArgument)
	{
		if (this.PostBackTarget != null)
		{
			this.PostBackTarget(this, EventArgs.Empty);
		}
	}

	protected override IEnumerable<ScriptDescriptor> GetScriptDescriptors()
	{
		if (m_causePostBack)
		{
			ScriptControlDescriptor scriptControlDescriptor = new ScriptControlDescriptor("Microsoft.Reporting.WebFormsClient._ReportAreaAsyncLoadTarget", ClientID);
			string script = string.Format(CultureInfo.InvariantCulture, "function() {{{0};}}", Page.ClientScript.GetPostBackEventReference(this, null));
			scriptControlDescriptor.AddScriptProperty("PostBackForAsyncLoad", script);
			return new ScriptDescriptor[1] { scriptControlDescriptor };
		}
		return null;
	}

	protected override IEnumerable<ScriptReference> GetScriptReferences()
	{
		ScriptReference scriptReference = new ScriptReference();
		scriptReference.Path = EmbeddedResourceOperation.CreateUrlForScriptFile();
		return new ScriptReference[1] { scriptReference };
	}
}
