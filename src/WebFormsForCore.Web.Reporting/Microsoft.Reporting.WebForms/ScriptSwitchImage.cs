using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Microsoft.Reporting.WebForms;

internal sealed class ScriptSwitchImage : CompositeControl, IScriptControl
{
	private HoverImage m_image1Hover;

	private HoverImage m_image2Hover;

	private ToolbarImageInfo m_image1;

	private ToolbarImageInfo m_image2;

	private bool m_image2Disabled;

	private string m_tooltip;

	private ReportViewer m_viewer;

	protected override HtmlTextWriterTag TagKey => HtmlTextWriterTag.Div;

	public bool ShowImage2
	{
		get
		{
			EnsureChildControls();
			return m_image2Hover.ClientVisible;
		}
		set
		{
			EnsureChildControls();
			m_image1Hover.ClientVisible = !value;
			m_image2Hover.ClientVisible = value;
		}
	}

	public event EventHandler ClickImage1;

	public ScriptSwitchImage(ToolbarImageInfo image1, ToolbarImageInfo image2, bool image2Disabled, string tooltip, ReportViewer viewer)
	{
		m_image1 = image1;
		m_image2 = image2;
		m_image2Disabled = image2Disabled;
		m_tooltip = tooltip;
		m_viewer = viewer;
	}

	protected override void CreateChildControls()
	{
		Controls.Clear();
		m_image1Hover = new HoverImage(m_image1, m_tooltip, m_viewer);
		if (m_viewer.ViewerStyle.Image != null)
		{
			m_image1Hover.CssClass = m_viewer.ViewerStyle.Image;
		}
		m_image1Hover.Click += Image1_Click;
		Controls.Add(m_image1Hover);
		m_image2Hover = new HoverImage(m_image2, m_tooltip, m_viewer);
		m_image2Hover.Enabled = !m_image2Disabled;
		Controls.Add(m_image2Hover);
		ShowImage2 = true;
	}

	protected override void OnPreRender(EventArgs e)
	{
		ScriptManager.GetCurrent(Page)?.RegisterScriptControl(this);
		base.OnPreRender(e);
	}

	protected override void Render(HtmlTextWriter writer)
	{
		EnsureChildControls();
		ScriptManager.GetCurrent(Page)?.RegisterScriptDescriptors(this);
		if (base.DesignMode)
		{
			m_image2Hover.RenderControl(writer);
		}
		else
		{
			base.Render(writer);
		}
	}

	private void Image1_Click(object sender, EventArgs e)
	{
		if (this.ClickImage1 != null)
		{
			this.ClickImage1(this, e);
		}
	}

	IEnumerable<ScriptDescriptor> IScriptControl.GetScriptDescriptors()
	{
		EnsureChildControls();
		ScriptControlDescriptor scriptControlDescriptor = new ScriptControlDescriptor("Microsoft.Reporting.WebFormsClient._ScriptSwitchImage", ClientID);
		scriptControlDescriptor.AddElementProperty("Image1", m_image1Hover.ClientID);
		scriptControlDescriptor.AddElementProperty("Image2", m_image2Hover.ClientID);
		return new ScriptDescriptor[1] { scriptControlDescriptor };
	}

	IEnumerable<ScriptReference> IScriptControl.GetScriptReferences()
	{
		ScriptReference scriptReference = new ScriptReference();
		scriptReference.Path = EmbeddedResourceOperation.CreateUrlForScriptFile();
		return new ScriptReference[1] { scriptReference };
	}
}
