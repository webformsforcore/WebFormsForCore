using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Microsoft.Reporting.WebForms;

internal abstract class ToolbarGroup : CompositeControl
{
	internal static readonly string NormalLeadingSpace = "20px";

	internal static readonly string ReducedLeadingSpace = "6px";

	public Unit ContainedControlHeight = Unit.Pixel(28);

	protected ReportViewer m_viewer;

	public abstract string GroupCssClassName { get; }

	public virtual string LeadingSpace => NormalLeadingSpace;

	public event EventHandler<ReportActionEventArgs> ReportAction;

	internal ToolbarGroup(ReportViewer viewer)
	{
		m_viewer = viewer;
		CssClass = GroupCssClassName + " " + viewer.ViewerStyle.ToolbarGroup;
	}

	protected void OnReportAction(ReportActionEventArgs args)
	{
		if (this.ReportAction != null)
		{
			this.ReportAction(this, args);
		}
	}

	public abstract void AddScriptDescriptorProperties(ScriptControlDescriptor toolbarDesc);

	protected override void Render(HtmlTextWriter writer)
	{
		EnsureChildControls();
		AddAttributesToRender(writer);
		writer.AddStyleAttribute(HtmlTextWriterStyle.VerticalAlign, "top");
		writer.RenderBeginTag(HtmlTextWriterTag.Div);
		writer.AddAttribute(HtmlTextWriterAttribute.Cellpadding, "0");
		writer.AddAttribute(HtmlTextWriterAttribute.Cellspacing, "0");
		writer.AddStyleAttribute(HtmlTextWriterStyle.Display, "inline");
		writer.RenderBeginTag(HtmlTextWriterTag.Table);
		writer.RenderBeginTag(HtmlTextWriterTag.Tr);
		RenderChildren(writer);
		writer.RenderEndTag();
		writer.RenderEndTag();
		writer.RenderEndTag();
	}

	protected override void RenderChildren(HtmlTextWriter writer)
	{
		bool flag = true;
		foreach (Control control in Controls)
		{
			if (!flag)
			{
				RenderItemSpacer(writer);
			}
			else
			{
				flag = false;
			}
			if (!ContainedControlHeight.IsEmpty)
			{
				writer.AddAttribute(HtmlTextWriterAttribute.Height, ContainedControlHeight.ToString());
			}
			writer.RenderBeginTag(HtmlTextWriterTag.Td);
			control.RenderControl(writer);
			writer.RenderEndTag();
		}
	}

	protected void RenderItemSpacer(HtmlTextWriter writer)
	{
		if (m_viewer.ViewerStyle.ToolbarInterGroupSpacing != null)
		{
			writer.AddAttribute(HtmlTextWriterAttribute.Class, m_viewer.ViewerStyle.ToolbarInterGroupSpacing);
		}
		else
		{
			writer.AddAttribute(HtmlTextWriterAttribute.Width, "4px");
		}
		writer.RenderBeginTag(HtmlTextWriterTag.Td);
		writer.RenderEndTag();
	}

	protected IEnumerable<RenderingExtension> GetClientSupportedRenderingExtensions()
	{
		return ReportViewer.FilterOutClientUnsupportedRenderingExtensions(GetRenderingExtensions());
	}

	private RenderingExtension[] GetRenderingExtensions()
	{
		try
		{
			if (!Global.IsDesignTime && m_viewer.Report.IsReadyForConnection)
			{
				return m_viewer.Report.ListRenderingExtensions();
			}
		}
		catch
		{
		}
		return new RenderingExtension[0];
	}
}
