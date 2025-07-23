using System;
using System.Web.UI;

namespace Microsoft.Reporting.WebForms;

internal class ServerErrorValidatorForException : ServerErrorValidator, INamingContainer
{
	private ErrorControl m_errorControl;

	public override ControlCollection Controls
	{
		get
		{
			EnsureChildControls();
			return base.Controls;
		}
	}

	public void SetException(Exception e)
	{
		EnsureChildControls();
		base.IsValid = e == null;
		m_errorControl.SetException(e);
	}

	protected override void CreateChildControls()
	{
		Controls.Clear();
		base.CreateChildControls();
		m_errorControl = new ErrorControl(useInternalPadding: false);
		Controls.Add(m_errorControl);
	}

	protected override void Render(HtmlTextWriter writer)
	{
		EnsureChildControls();
		if (m_errorControl.HasException)
		{
			base.Render(writer);
		}
	}
}
