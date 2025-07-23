using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Microsoft.Reporting.WebForms;

internal sealed class HoverImage : CompositeControl, IPostBackEventHandler, IScriptControl
{
	public bool ClientVisible = true;

	private string m_tooltip;

	private ToolbarImageInfo m_image;

	private ImageButton m_ltrImageButton;

	private ImageButton m_rtlImageButton;

	private IReportViewerStyles m_viewerStyle;

	private ReportViewer m_viewer;

	private bool IsRtlImageVisible
	{
		get
		{
			if (m_image.IsBiDirectional)
			{
				return m_viewer.IsClientRightToLeft;
			}
			return false;
		}
	}

	public event EventHandler Click;

	public HoverImage(ToolbarImageInfo image, string tooltip, ReportViewer viewer)
	{
		m_image = image;
		m_tooltip = tooltip;
		m_viewer = viewer;
		m_viewerStyle = viewer.ViewerStyle;
	}

	protected override void CreateChildControls()
	{
		Controls.Clear();
		m_ltrImageButton = CreateImageButton(m_image.LTRImageName);
		if (m_image.IsBiDirectional)
		{
			m_rtlImageButton = CreateImageButton(m_image.RTLImageName);
		}
	}

	private ImageButton CreateImageButton(string imageUrl)
	{
		ImageButton imageButton = new ImageButton();
		imageButton.ImageUrl = EmbeddedResourceOperation.CreateUrl(imageUrl);
		imageButton.CssClass = CssClass;
		imageButton.AlternateText = m_tooltip;
		imageButton.Attributes.Add("title", m_tooltip);
		imageButton.Width = 16;
		imageButton.Height = 16;
		imageButton.BorderStyle = BorderStyle.None;
		Controls.Add(imageButton);
		return imageButton;
	}

	protected override void OnPreRender(EventArgs e)
	{
		ScriptManager.GetCurrent(Page)?.RegisterScriptControl(this);
		base.OnPreRender(e);
	}

	protected override void AddAttributesToRender(HtmlTextWriter writer)
	{
		base.AddAttributesToRender(writer);
		if (!Enabled)
		{
			writer.AddAttribute(HtmlTextWriterAttribute.Disabled, "disabled");
		}
	}

	protected override void Render(HtmlTextWriter writer)
	{
		EnsureChildControls();
		ReportViewerStyle.ApplyButtonStyle(m_viewer.ViewerStyle, this);
		ScriptManager.GetCurrent(Page)?.RegisterScriptDescriptors(this);
		AddAttributesToRender(writer);
		if (!ClientVisible)
		{
			writer.AddStyleAttribute(HtmlTextWriterStyle.Display, "none");
		}
		writer.RenderBeginTag(HtmlTextWriterTag.Div);
		writer.AddAttribute(HtmlTextWriterAttribute.Title, m_tooltip, fEncode: true);
		writer.RenderBeginTag(HtmlTextWriterTag.Table);
		writer.RenderBeginTag(HtmlTextWriterTag.Tr);
		writer.RenderBeginTag(HtmlTextWriterTag.Td);
		if (m_image.IsBiDirectional)
		{
			if (IsRtlImageVisible)
			{
				m_ltrImageButton.Style.Add(HtmlTextWriterStyle.Display, "none");
			}
			else
			{
				m_rtlImageButton.Style.Add(HtmlTextWriterStyle.Display, "none");
			}
			RenderImageButton(writer, m_ltrImageButton);
			RenderImageButton(writer, m_rtlImageButton);
		}
		else
		{
			RenderImageButton(writer, m_ltrImageButton);
		}
		writer.RenderEndTag();
		writer.RenderEndTag();
		writer.RenderEndTag();
		writer.RenderEndTag();
	}

	private void RenderImageButton(HtmlTextWriter writer, ImageButton image)
	{
		if (!Enabled)
		{
			image.Style.Add(HtmlTextWriterStyle.Cursor, "default");
		}
		else
		{
			image.Style.Remove(HtmlTextWriterStyle.Cursor);
		}
		image.RenderControl(writer);
	}

	public void RaisePostBackEvent(string eventArgument)
	{
		if (this.Click != null)
		{
			this.Click(this, EventArgs.Empty);
		}
	}

	IEnumerable<ScriptDescriptor> IScriptControl.GetScriptDescriptors()
	{
		EnsureChildControls();
		ScriptControlDescriptor scriptControlDescriptor = new ScriptControlDescriptor("Microsoft.Reporting.WebFormsClient._HoverImage", ClientID);
		scriptControlDescriptor.AddScriptProperty("NormalStyles", ReportViewerStyle.ToolbarItemStyles(m_viewerStyle, Enabled, normal: true));
		scriptControlDescriptor.AddScriptProperty("HoverStyles", ReportViewerStyle.ToolbarItemStyles(m_viewerStyle, Enabled, normal: false));
		string script = JavaScriptHelper.FormatAsFunction(Page.ClientScript.GetPostBackEventReference(this, null) + ";");
		scriptControlDescriptor.AddScriptProperty("OnClickScript", script);
		if (m_image.IsBiDirectional)
		{
			scriptControlDescriptor.AddComponentProperty("ReportViewer", m_viewer.ClientID);
			scriptControlDescriptor.AddProperty("IsRtlVisible", IsRtlImageVisible);
			scriptControlDescriptor.AddProperty("LTRImageID", m_ltrImageButton.ClientID);
			scriptControlDescriptor.AddProperty("RTLImageID", m_rtlImageButton.ClientID);
		}
		return new ScriptDescriptor[1] { scriptControlDescriptor };
	}

	IEnumerable<ScriptReference> IScriptControl.GetScriptReferences()
	{
		ScriptReference scriptReference = new ScriptReference();
		scriptReference.Path = EmbeddedResourceOperation.CreateUrlForScriptFile();
		return new ScriptReference[1] { scriptReference };
	}
}
