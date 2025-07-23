using System;
using System.Collections.Generic;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Microsoft.Reporting.WebForms;

internal class ReportSplitter : CompositeScriptControl
{
	private IReportViewerStyles m_viewerStyle;

	private string m_tooltip;

	private bool m_isVertical;

	private bool m_isResizable = true;

	private bool m_isCollapsable = true;

	private HiddenField m_position;

	private HiddenField m_collapseState;

	private ImageButton m_image;

	public bool IsCollapsed
	{
		get
		{
			EnsureChildControls();
			return string.Equals(m_collapseState.Value, "true", StringComparison.OrdinalIgnoreCase);
		}
		set
		{
			EnsureChildControls();
			m_collapseState.Value = (value ? "true" : "false");
		}
	}

	public bool IsCollapsable
	{
		get
		{
			return m_isCollapsable;
		}
		set
		{
			m_isCollapsable = value;
		}
	}

	public bool IsResizable
	{
		get
		{
			return m_isResizable;
		}
		set
		{
			m_isResizable = value;
		}
	}

	public event EventHandler CollapsedChanged;

	public ReportSplitter(IReportViewerStyles viewerStyle, bool isVertical, string tooltip)
	{
		m_isVertical = isVertical;
		m_viewerStyle = viewerStyle;
		m_tooltip = tooltip;
	}

	protected override void CreateChildControls()
	{
		base.CreateChildControls();
		m_image = new ImageButton();
		m_image.ID = "img";
		m_image.AlternateText = m_tooltip;
		m_image.ToolTip = m_tooltip;
		if (m_isVertical)
		{
			m_image.ImageUrl = EmbeddedResourceOperation.CreateUrl("Microsoft.Reporting.WebForms.Icons.SplitterVertCollapse.png");
			m_image.ImageAlign = ImageAlign.Top;
		}
		else
		{
			m_image.ImageUrl = EmbeddedResourceOperation.CreateUrl("Microsoft.Reporting.WebForms.Icons.SplitterHorizCollapse.png");
			m_image.ImageAlign = ImageAlign.Middle;
		}
		m_image.OnClientClick = "void(0)";
		m_image.Style[HtmlTextWriterStyle.Cursor] = "pointer";
		Controls.Add(m_image);
		m_position = new HiddenField();
		m_position.ID = "store";
		Controls.Add(m_position);
		m_collapseState = new HiddenField();
		m_collapseState.ID = "collapse";
		m_collapseState.ValueChanged += CollapseState_ValueChanged;
		Controls.Add(m_collapseState);
	}

	private void CollapseState_ValueChanged(object sender, EventArgs e)
	{
		if (this.CollapsedChanged != null)
		{
			this.CollapsedChanged(this, EventArgs.Empty);
		}
	}

	public void WriteTableCellCenteringStyles(HtmlTextWriter writer)
	{
		if (m_isVertical)
		{
			writer.AddStyleAttribute(HtmlTextWriterStyle.VerticalAlign, "middle");
		}
		else
		{
			writer.AddStyleAttribute(HtmlTextWriterStyle.TextAlign, "center");
		}
		if (m_viewerStyle.SplitterNormal != null)
		{
			writer.AddAttribute(HtmlTextWriterAttribute.Class, m_viewerStyle.SplitterNormal);
		}
		else
		{
			writer.AddStyleAttribute(HtmlTextWriterStyle.BackgroundColor, ColorTranslator.ToHtml(m_viewerStyle.SplitterBackColor));
		}
	}

	protected override void OnPreRender(EventArgs e)
	{
		EnsureID();
		base.OnPreRender(e);
	}

	protected override void Render(HtmlTextWriter writer)
	{
		EnsureChildControls();
		if (IsCollapsed)
		{
			m_image.ImageUrl = EmbeddedResourceOperation.CreateUrl(m_isVertical ? "Microsoft.Reporting.WebForms.Icons.SplitterVertExpand.png" : "Microsoft.Reporting.WebForms.Icons.SplitterHorizExpand.png");
		}
		if (!m_isCollapsable)
		{
			m_image.Style[HtmlTextWriterStyle.Display] = "none";
		}
		base.Render(writer);
	}

	public override IEnumerable<ScriptDescriptor> GetScriptDescriptors()
	{
		ScriptControlDescriptor scriptControlDescriptor = new ScriptControlDescriptor("Microsoft.Reporting.WebFormsClient._Splitter", ClientID);
		if (m_viewerStyle.SplitterNormal != null)
		{
			scriptControlDescriptor.AddProperty("NormalStyle", m_viewerStyle.SplitterNormal);
			scriptControlDescriptor.AddProperty("HoverStyle", m_viewerStyle.SplitterHover);
		}
		else
		{
			scriptControlDescriptor.AddProperty("NormalColor", ColorTranslator.ToHtml(m_viewerStyle.SplitterBackColor));
			scriptControlDescriptor.AddProperty("HoverColor", ColorTranslator.ToHtml(m_viewerStyle.HoverBackColor));
		}
		scriptControlDescriptor.AddProperty("Vertical", m_isVertical);
		scriptControlDescriptor.AddProperty("Resizable", m_isResizable);
		scriptControlDescriptor.AddProperty("StorePositionField", m_position.ClientID);
		scriptControlDescriptor.AddProperty("StoreCollapseField", m_collapseState.ClientID);
		scriptControlDescriptor.AddProperty("IsCollapsable", IsCollapsable);
		scriptControlDescriptor.AddProperty("ImageId", m_image.ClientID);
		if (m_isVertical)
		{
			scriptControlDescriptor.AddProperty("ImageCollapse", EmbeddedResourceOperation.CreateUrl("Microsoft.Reporting.WebForms.Icons.SplitterVertCollapse.png"));
			scriptControlDescriptor.AddProperty("ImageCollapseHover", EmbeddedResourceOperation.CreateUrl("Microsoft.Reporting.WebForms.Icons.SplitterVertCollapseHover.png"));
			scriptControlDescriptor.AddProperty("ImageExpand", EmbeddedResourceOperation.CreateUrl("Microsoft.Reporting.WebForms.Icons.SplitterVertExpand.png"));
			scriptControlDescriptor.AddProperty("ImageExpandHover", EmbeddedResourceOperation.CreateUrl("Microsoft.Reporting.WebForms.Icons.SplitterVertExpandHover.png"));
		}
		else
		{
			scriptControlDescriptor.AddProperty("ImageCollapse", EmbeddedResourceOperation.CreateUrl("Microsoft.Reporting.WebForms.Icons.SplitterHorizCollapse.png"));
			scriptControlDescriptor.AddProperty("ImageCollapseHover", EmbeddedResourceOperation.CreateUrl("Microsoft.Reporting.WebForms.Icons.SplitterHorizCollapseHover.png"));
			scriptControlDescriptor.AddProperty("ImageExpand", EmbeddedResourceOperation.CreateUrl("Microsoft.Reporting.WebForms.Icons.SplitterHorizExpand.png"));
			scriptControlDescriptor.AddProperty("ImageExpandHover", EmbeddedResourceOperation.CreateUrl("Microsoft.Reporting.WebForms.Icons.SplitterHorizExpandHover.png"));
		}
		return new ScriptDescriptor[1] { scriptControlDescriptor };
	}
}
