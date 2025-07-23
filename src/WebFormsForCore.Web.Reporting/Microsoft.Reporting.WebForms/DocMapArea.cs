using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Microsoft.Reporting.WebForms;

internal sealed class DocMapArea : CompositeScriptControl, IPostBackEventHandler
{
	private HiddenField m_selectedNodeHiddenField;

	private ReportViewer m_viewer;

	private IReportViewerStyles m_styles;

	private CssStyleCollection m_fontStyles;

	private DocumentMapNode m_rootNode;

	private string m_expandImageUrl = EmbeddedResourceOperation.CreateUrl("Microsoft.Reporting.WebForms.Icons.plus.gif");

	private string m_collapseImageUrl = EmbeddedResourceOperation.CreateUrl("Microsoft.Reporting.WebForms.Icons.minus.gif");

	private string m_spacerImageUrl = EmbeddedResourceOperation.CreateUrl("Microsoft.Reporting.WebForms.Icons.white.gif");

	private string m_onKeyDownScript;

	private string m_onClickTextScript;

	private string m_onMouseOverTextScript;

	private string m_onMouseOutTextScript;

	private string m_onClickExpandCollapseScript;

	public DocumentMapNode RootNode
	{
		get
		{
			return m_rootNode;
		}
		set
		{
			m_rootNode = value;
		}
	}

	public string DocMapHeaderOverflowDivId => ClientID + "DocMapHeaderOverflowDiv";

	private string RootNodeId => ClientID + "RootNode";

	public event DocumentMapNavigationEventHandler NodeClick;

	public DocMapArea(ReportViewer viewer)
	{
		m_viewer = viewer;
		m_styles = viewer.ViewerStyle;
		base.Style.Add(HtmlTextWriterStyle.Display, "none");
	}

	protected override void OnInit(EventArgs e)
	{
		base.OnInit(e);
		EnsureID();
	}

	protected override void CreateChildControls()
	{
		Controls.Clear();
		base.CreateChildControls();
		m_selectedNodeHiddenField = new HiddenField();
		m_selectedNodeHiddenField.ID = "ClientClickedId";
		Controls.Add(m_selectedNodeHiddenField);
	}

	protected override void RenderChildren(HtmlTextWriter writer)
	{
		EnsureChildControls();
		m_selectedNodeHiddenField.RenderControl(writer);
		if (m_rootNode != null)
		{
			writer.AddAttribute(HtmlTextWriterAttribute.Cellpadding, "0", fEncode: false);
			writer.AddAttribute(HtmlTextWriterAttribute.Cellspacing, "0", fEncode: false);
			writer.AddAttribute(HtmlTextWriterAttribute.Height, "100%", fEncode: false);
			writer.AddAttribute(HtmlTextWriterAttribute.Width, "100%", fEncode: false);
			writer.RenderBeginTag(HtmlTextWriterTag.Table);
			RenderHeader(writer);
			RenderTree(writer);
			writer.RenderEndTag();
		}
	}

	private void RenderHeader(HtmlTextWriter writer)
	{
		writer.RenderBeginTag(HtmlTextWriterTag.Tr);
		writer.AddAttribute(HtmlTextWriterAttribute.Nowrap, null, fEncode: false);
		writer.AddAttribute(HtmlTextWriterAttribute.Width, "100%");
		writer.RenderBeginTag(HtmlTextWriterTag.Td);
		writer.AddStyleAttribute(HtmlTextWriterStyle.Overflow, "hidden");
		writer.AddStyleAttribute(HtmlTextWriterStyle.WhiteSpace, "nowrap");
		writer.AddAttribute(HtmlTextWriterAttribute.Id, DocMapHeaderOverflowDivId);
		writer.RenderBeginTag(HtmlTextWriterTag.Div);
		if (m_styles.DocMapHeader != null)
		{
			writer.AddAttribute(HtmlTextWriterAttribute.Class, m_styles.DocMapHeader);
		}
		else
		{
			WriteFontStyles(writer);
			writer.AddStyleAttribute(HtmlTextWriterStyle.BackgroundColor, ColorTranslator.ToHtml(m_styles.BackColor));
			writer.AddStyleAttribute(HtmlTextWriterStyle.PaddingLeft, "10px");
			writer.AddStyleAttribute(HtmlTextWriterStyle.PaddingRight, "10px");
			writer.AddStyleAttribute(HtmlTextWriterStyle.PaddingTop, "7px");
			writer.AddStyleAttribute(HtmlTextWriterStyle.PaddingBottom, "7px");
			writer.AddStyleAttribute(HtmlTextWriterStyle.BackgroundImage, BackgroundImageOperation.CreateUrl("Microsoft.Reporting.WebForms.Icons.docmap_bgt.png", m_styles.BackColor));
		}
		writer.RenderBeginTag(HtmlTextWriterTag.Div);
		writer.WriteEncodedText(LocalizationHelper.Current.DocumentMap);
		writer.RenderEndTag();
		writer.RenderEndTag();
		writer.RenderEndTag();
		writer.RenderEndTag();
	}

	private void RenderTree(HtmlTextWriter writer)
	{
		writer.RenderBeginTag(HtmlTextWriterTag.Tr);
		writer.AddAttribute(HtmlTextWriterAttribute.Height, "100%");
		writer.AddAttribute(HtmlTextWriterAttribute.Width, "100%");
		writer.AddAttribute(HtmlTextWriterAttribute.Valign, "top");
		if (m_styles.DocMapContent != null)
		{
			writer.AddAttribute(HtmlTextWriterAttribute.Class, m_styles.DocMapContent);
		}
		writer.RenderBeginTag(HtmlTextWriterTag.Td);
		if (!m_viewer.SizeToReportContent)
		{
			writer.AddStyleAttribute(HtmlTextWriterStyle.OverflowX, "auto");
			writer.AddStyleAttribute(HtmlTextWriterStyle.OverflowY, "auto");
		}
		else
		{
			HtmlTextWriterStyle key = ((!m_viewer.IsClientRightToLeft) ? HtmlTextWriterStyle.MarginRight : HtmlTextWriterStyle.MarginLeft);
			writer.AddStyleAttribute(key, "4px");
		}
		writer.AddStyleAttribute(HtmlTextWriterStyle.Height, "100%");
		writer.AddStyleAttribute(HtmlTextWriterStyle.Width, "100%");
		writer.RenderBeginTag(HtmlTextWriterTag.Div);
		writer.AddAttribute(HtmlTextWriterAttribute.Id, RootNodeId);
		writer.AddAttribute(HtmlTextWriterAttribute.Tabindex, "0");
		writer.AddAttribute("onkeydown", m_onKeyDownScript);
		writer.AddAttribute("hidefocus", "true");
		writer.AddStyleAttribute("outline", "none");
		RenderNode(writer, expandNode: true, m_rootNode);
		writer.RenderEndTag();
		writer.RenderEndTag();
		writer.RenderEndTag();
	}

	private void RenderNode(HtmlTextWriter writer, bool expandNode, DocumentMapNode node)
	{
		writer.AddStyleAttribute(HtmlTextWriterStyle.WhiteSpace, "nowrap");
		writer.AddAttribute("DocMapId", node.Id);
		writer.RenderBeginTag(HtmlTextWriterTag.Div);
		if (node.Children.Count > 0)
		{
			RenderExpandCollapseImage(writer, m_collapseImageUrl, Strings.DocMapCollapseTooltip(node.Label), expandNode);
			RenderExpandCollapseImage(writer, m_expandImageUrl, Strings.DocMapExpandTooltip(node.Label), !expandNode);
		}
		else
		{
			RenderDocMapImage(writer, forInteractivity: false, m_spacerImageUrl);
		}
		writer.Write(' ');
		writer.AddAttribute(HtmlTextWriterAttribute.Onclick, m_onClickTextScript);
		writer.AddAttribute(HtmlTextWriterAttribute.Href, "#");
		writer.AddAttribute(HtmlTextWriterAttribute.Tabindex, "-1");
		writer.AddStyleAttribute(HtmlTextWriterStyle.TextDecoration, "none");
		writer.AddAttribute(HtmlTextWriterAttribute.Title, Strings.DocMapActionTooltip(node.Label), fEncode: true);
		writer.RenderBeginTag(HtmlTextWriterTag.A);
		writer.AddAttribute("onmouseover", m_onMouseOverTextScript);
		writer.AddAttribute("onmouseout", m_onMouseOutTextScript);
		if (!ReportViewerClientScript.IsIE55OrHigher)
		{
			writer.AddStyleAttribute(HtmlTextWriterStyle.VerticalAlign, "middle");
		}
		if (m_fontStyles[HtmlTextWriterStyle.Color] == null)
		{
			writer.AddStyleAttribute(HtmlTextWriterStyle.Color, "black");
		}
		WriteFontStyles(writer);
		writer.RenderBeginTag(HtmlTextWriterTag.Span);
		writer.WriteEncodedText(node.Label);
		writer.RenderEndTag();
		writer.RenderEndTag();
		foreach (DocumentMapNode child in node.Children)
		{
			HtmlTextWriterStyle key = ((!m_viewer.IsClientRightToLeft) ? HtmlTextWriterStyle.MarginLeft : HtmlTextWriterStyle.MarginRight);
			writer.AddStyleAttribute(key, "19px");
			if (!expandNode)
			{
				writer.AddStyleAttribute(HtmlTextWriterStyle.Display, "none");
			}
			RenderNode(writer, expandNode: false, child);
		}
		writer.RenderEndTag();
	}

	private void RenderExpandCollapseImage(HtmlTextWriter writer, string imageUrl, string altText, bool makeVisible)
	{
		if (!makeVisible)
		{
			writer.AddStyleAttribute(HtmlTextWriterStyle.Display, "none");
		}
		writer.AddAttribute(HtmlTextWriterAttribute.Alt, altText, fEncode: true);
		writer.AddAttribute(HtmlTextWriterAttribute.Onclick, m_onClickExpandCollapseScript);
		RenderDocMapImage(writer, forInteractivity: true, imageUrl);
	}

	private void RenderDocMapImage(HtmlTextWriter writer, bool forInteractivity, string imageUrl)
	{
		writer.AddAttribute(HtmlTextWriterAttribute.Align, "absmiddle");
		writer.AddAttribute(HtmlTextWriterAttribute.Src, imageUrl);
		if (forInteractivity)
		{
			writer.AddAttribute(HtmlTextWriterAttribute.Tabindex, "-1");
			writer.AddAttribute(HtmlTextWriterAttribute.Name, "");
			writer.AddAttribute(HtmlTextWriterAttribute.Type, "image");
			writer.RenderBeginTag(HtmlTextWriterTag.Input);
		}
		else
		{
			writer.RenderBeginTag(HtmlTextWriterTag.Img);
		}
		writer.RenderEndTag();
	}

	public override IEnumerable<ScriptDescriptor> GetScriptDescriptors()
	{
		if (m_rootNode == null)
		{
			return null;
		}
		ScriptControlDescriptor scriptControlDescriptor = new ScriptControlDescriptor("Microsoft.Reporting.WebFormsClient._DocMapArea", ClientID);
		scriptControlDescriptor.AddProperty("RootNodeId", RootNodeId);
		scriptControlDescriptor.AddProperty("SelectedNodeHiddenFieldId", m_selectedNodeHiddenField.ClientID);
		scriptControlDescriptor.AddProperty("IsLTR", !m_viewer.IsClientRightToLeft);
		scriptControlDescriptor.AddProperty("ReportViewerId", m_viewer.ClientID);
		string postBackEventReference = Page.ClientScript.GetPostBackEventReference(this, null);
		string script = JavaScriptHelper.FormatAsFunction(postBackEventReference + ";");
		scriptControlDescriptor.AddScriptProperty("TriggerPostBack", script);
		return new ScriptDescriptor[1] { scriptControlDescriptor };
	}

	public override IEnumerable<ScriptReference> GetScriptReferences()
	{
		if (m_rootNode == null)
		{
			return null;
		}
		ScriptReference scriptReference = new ScriptReference(EmbeddedResourceOperation.CreateUrlForScriptFile());
		return new ScriptReference[1] { scriptReference };
	}

	protected override void OnPreRender(EventArgs e)
	{
		base.OnPreRender(e);
		m_fontStyles = ReportViewerStyle.GetStylesForFont(m_styles.Font);
		string text = JavaScriptHelper.StringEscapeSingleQuote(ClientID);
		m_onKeyDownScript = string.Format(CultureInfo.InvariantCulture, "if($get('{0}').control != null) $get('{0}').control.OnKeyDown(event);", text);
		m_onClickTextScript = string.Format(CultureInfo.InvariantCulture, "if($get('{0}').control != null) $get('{0}').control.OnAnchorNodeSelected(this);return false;", text);
		m_onMouseOverTextScript = string.Format(CultureInfo.InvariantCulture, "if($get('{0}').control != null) $get('{0}').control.OnTextNodeEnter(this);", text);
		m_onMouseOutTextScript = string.Format(CultureInfo.InvariantCulture, "if($get('{0}').control != null) $get('{0}').control.OnTextNodeLeave(this);", text);
		m_onClickExpandCollapseScript = string.Format(CultureInfo.InvariantCulture, "if($get('{0}').control != null) $get('{0}').control.ExpandCollapseNode(this.parentNode); event.cancelBubble=true;return false;", text);
	}

	private void WriteFontStyles(HtmlTextWriter writer)
	{
		foreach (string key in m_fontStyles.Keys)
		{
			writer.AddStyleAttribute(key, m_fontStyles[key]);
		}
	}

	void IPostBackEventHandler.RaisePostBackEvent(string eventArgument)
	{
		EnsureChildControls();
		if (this.NodeClick != null)
		{
			string value = m_selectedNodeHiddenField.Value;
			this.NodeClick(this, new DocumentMapNavigationEventArgs(value));
		}
	}
}
