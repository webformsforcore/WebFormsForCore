
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
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

    public DocMapArea(ReportViewer viewer)
    {
      this.m_viewer = viewer;
      this.m_styles = viewer.ViewerStyle;
      this.Style.Add(HtmlTextWriterStyle.Display, "none");
    }

    public event DocumentMapNavigationEventHandler NodeClick;

    protected override void OnInit(EventArgs e)
    {
      base.OnInit(e);
      this.EnsureID();
    }

    protected override void CreateChildControls()
    {
      this.Controls.Clear();
      base.CreateChildControls();
      this.m_selectedNodeHiddenField = new HiddenField();
      this.m_selectedNodeHiddenField.ID = "ClientClickedId";
      this.Controls.Add((Control) this.m_selectedNodeHiddenField);
    }

    public DocumentMapNode RootNode
    {
      get => this.m_rootNode;
      set => this.m_rootNode = value;
    }

    public string DocMapHeaderOverflowDivId => this.ClientID + "DocMapHeaderOverflowDiv";

    protected override void RenderChildren(HtmlTextWriter writer)
    {
      this.EnsureChildControls();
      this.m_selectedNodeHiddenField.RenderControl(writer);
      if (this.m_rootNode == null)
        return;
      writer.AddAttribute(HtmlTextWriterAttribute.Cellpadding, "0", false);
      writer.AddAttribute(HtmlTextWriterAttribute.Cellspacing, "0", false);
      writer.AddAttribute(HtmlTextWriterAttribute.Height, "100%", false);
      writer.AddAttribute(HtmlTextWriterAttribute.Width, "100%", false);
      writer.RenderBeginTag(HtmlTextWriterTag.Table);
      this.RenderHeader(writer);
      this.RenderTree(writer);
      writer.RenderEndTag();
    }

    private void RenderHeader(HtmlTextWriter writer)
    {
      writer.RenderBeginTag(HtmlTextWriterTag.Tr);
      writer.AddAttribute(HtmlTextWriterAttribute.Nowrap, (string) null, false);
      writer.AddAttribute(HtmlTextWriterAttribute.Width, "100%");
      writer.RenderBeginTag(HtmlTextWriterTag.Td);
      writer.AddStyleAttribute(HtmlTextWriterStyle.Overflow, "hidden");
      writer.AddStyleAttribute(HtmlTextWriterStyle.WhiteSpace, "nowrap");
      writer.AddAttribute(HtmlTextWriterAttribute.Id, this.DocMapHeaderOverflowDivId);
      writer.RenderBeginTag(HtmlTextWriterTag.Div);
      if (this.m_styles.DocMapHeader != null)
      {
        writer.AddAttribute(HtmlTextWriterAttribute.Class, this.m_styles.DocMapHeader);
      }
      else
      {
        this.WriteFontStyles(writer);
        writer.AddStyleAttribute(HtmlTextWriterStyle.BackgroundColor, ColorTranslator.ToHtml(this.m_styles.BackColor));
        writer.AddStyleAttribute(HtmlTextWriterStyle.PaddingLeft, "10px");
        writer.AddStyleAttribute(HtmlTextWriterStyle.PaddingRight, "10px");
        writer.AddStyleAttribute(HtmlTextWriterStyle.PaddingTop, "7px");
        writer.AddStyleAttribute(HtmlTextWriterStyle.PaddingBottom, "7px");
        writer.AddStyleAttribute(HtmlTextWriterStyle.BackgroundImage, BackgroundImageOperation.CreateUrl("Microsoft.Reporting.WebForms.Icons.docmap_bgt.png", this.m_styles.BackColor));
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
      if (this.m_styles.DocMapContent != null)
        writer.AddAttribute(HtmlTextWriterAttribute.Class, this.m_styles.DocMapContent);
      writer.RenderBeginTag(HtmlTextWriterTag.Td);
      if (!this.m_viewer.SizeToReportContent)
      {
        writer.AddStyleAttribute(HtmlTextWriterStyle.OverflowX, "auto");
        writer.AddStyleAttribute(HtmlTextWriterStyle.OverflowY, "auto");
      }
      else
      {
        HtmlTextWriterStyle key = !this.m_viewer.IsClientRightToLeft ? HtmlTextWriterStyle.MarginRight : HtmlTextWriterStyle.MarginLeft;
        writer.AddStyleAttribute(key, "4px");
      }
      writer.AddStyleAttribute(HtmlTextWriterStyle.Height, "100%");
      writer.AddStyleAttribute(HtmlTextWriterStyle.Width, "100%");
      writer.RenderBeginTag(HtmlTextWriterTag.Div);
      writer.AddAttribute(HtmlTextWriterAttribute.Id, this.RootNodeId);
      writer.AddAttribute(HtmlTextWriterAttribute.Tabindex, "0");
      writer.AddAttribute("onkeydown", this.m_onKeyDownScript);
      writer.AddAttribute("hidefocus", "true");
      writer.AddStyleAttribute("outline", "none");
      this.RenderNode(writer, true, this.m_rootNode);
      writer.RenderEndTag();
      writer.RenderEndTag();
      writer.RenderEndTag();
    }

    private string RootNodeId => this.ClientID + "RootNode";

    private void RenderNode(HtmlTextWriter writer, bool expandNode, DocumentMapNode node)
    {
      writer.AddStyleAttribute(HtmlTextWriterStyle.WhiteSpace, "nowrap");
      writer.AddAttribute("DocMapId", node.Id);
      writer.RenderBeginTag(HtmlTextWriterTag.Div);
      if (node.Children.Count > 0)
      {
        // ISSUE: reference to a compiler-generated method
        this.RenderExpandCollapseImage(writer, this.m_collapseImageUrl, Strings.DocMapCollapseTooltip(node.Label), expandNode);
        // ISSUE: reference to a compiler-generated method
        this.RenderExpandCollapseImage(writer, this.m_expandImageUrl, Strings.DocMapExpandTooltip(node.Label), !expandNode);
      }
      else
        this.RenderDocMapImage(writer, false, this.m_spacerImageUrl);
      writer.Write(' ');
      writer.AddAttribute(HtmlTextWriterAttribute.Onclick, this.m_onClickTextScript);
      writer.AddAttribute(HtmlTextWriterAttribute.Href, "#");
      writer.AddAttribute(HtmlTextWriterAttribute.Tabindex, "-1");
      writer.AddStyleAttribute(HtmlTextWriterStyle.TextDecoration, "none");
      // ISSUE: reference to a compiler-generated method
      writer.AddAttribute(HtmlTextWriterAttribute.Title, Strings.DocMapActionTooltip(node.Label), true);
      writer.RenderBeginTag(HtmlTextWriterTag.A);
      writer.AddAttribute("onmouseover", this.m_onMouseOverTextScript);
      writer.AddAttribute("onmouseout", this.m_onMouseOutTextScript);
      if (!ReportViewerClientScript.IsIE55OrHigher)
        writer.AddStyleAttribute(HtmlTextWriterStyle.VerticalAlign, "middle");
      if (this.m_fontStyles[HtmlTextWriterStyle.Color] == null)
        writer.AddStyleAttribute(HtmlTextWriterStyle.Color, "black");
      this.WriteFontStyles(writer);
      writer.RenderBeginTag(HtmlTextWriterTag.Span);
      writer.WriteEncodedText(node.Label);
      writer.RenderEndTag();
      writer.RenderEndTag();
      foreach (DocumentMapNode child in (IEnumerable<DocumentMapNode>) node.Children)
      {
        HtmlTextWriterStyle key = !this.m_viewer.IsClientRightToLeft ? HtmlTextWriterStyle.MarginLeft : HtmlTextWriterStyle.MarginRight;
        writer.AddStyleAttribute(key, "19px");
        if (!expandNode)
          writer.AddStyleAttribute(HtmlTextWriterStyle.Display, "none");
        this.RenderNode(writer, false, child);
      }
      writer.RenderEndTag();
    }

    private void RenderExpandCollapseImage(
      HtmlTextWriter writer,
      string imageUrl,
      string altText,
      bool makeVisible)
    {
      if (!makeVisible)
        writer.AddStyleAttribute(HtmlTextWriterStyle.Display, "none");
      writer.AddAttribute(HtmlTextWriterAttribute.Alt, altText, true);
      writer.AddAttribute(HtmlTextWriterAttribute.Onclick, this.m_onClickExpandCollapseScript);
      this.RenderDocMapImage(writer, true, imageUrl);
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
        writer.RenderBeginTag(HtmlTextWriterTag.Img);
      writer.RenderEndTag();
    }

    public override IEnumerable<ScriptDescriptor> GetScriptDescriptors()
    {
      if (this.m_rootNode == null)
        return (IEnumerable<ScriptDescriptor>) null;
      ScriptControlDescriptor controlDescriptor = new ScriptControlDescriptor("Microsoft.Reporting.WebFormsClient._DocMapArea", this.ClientID);
      controlDescriptor.AddProperty("RootNodeId", (object) this.RootNodeId);
      controlDescriptor.AddProperty("SelectedNodeHiddenFieldId", (object) this.m_selectedNodeHiddenField.ClientID);
      controlDescriptor.AddProperty("IsLTR", (object) !this.m_viewer.IsClientRightToLeft);
      controlDescriptor.AddProperty("ReportViewerId", (object) this.m_viewer.ClientID);
      string script = JavaScriptHelper.FormatAsFunction(this.Page.ClientScript.GetPostBackEventReference((Control) this, (string) null) + ";");
      controlDescriptor.AddScriptProperty("TriggerPostBack", script);
      return (IEnumerable<ScriptDescriptor>) new ScriptDescriptor[1]
      {
        (ScriptDescriptor) controlDescriptor
      };
    }

    public override IEnumerable<ScriptReference> GetScriptReferences()
    {
      if (this.m_rootNode == null)
        return (IEnumerable<ScriptReference>) null;
      return (IEnumerable<ScriptReference>) new ScriptReference[1]
      {
        new ScriptReference(EmbeddedResourceOperation.CreateUrlForScriptFile())
      };
    }

    protected override void OnPreRender(EventArgs e)
    {
      base.OnPreRender(e);
      this.m_fontStyles = ReportViewerStyle.GetStylesForFont(this.m_styles.Font);
      string str = JavaScriptHelper.StringEscapeSingleQuote(this.ClientID);
      this.m_onKeyDownScript = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "if($get('{0}').control != null) $get('{0}').control.OnKeyDown(event);", (object) str);
      this.m_onClickTextScript = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "if($get('{0}').control != null) $get('{0}').control.OnAnchorNodeSelected(this);return false;", (object) str);
      this.m_onMouseOverTextScript = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "if($get('{0}').control != null) $get('{0}').control.OnTextNodeEnter(this);", (object) str);
      this.m_onMouseOutTextScript = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "if($get('{0}').control != null) $get('{0}').control.OnTextNodeLeave(this);", (object) str);
      this.m_onClickExpandCollapseScript = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "if($get('{0}').control != null) $get('{0}').control.ExpandCollapseNode(this.parentNode); event.cancelBubble=true;return false;", (object) str);
    }

    private void WriteFontStyles(HtmlTextWriter writer)
    {
      foreach (string key in (IEnumerable) this.m_fontStyles.Keys)
        writer.AddStyleAttribute(key, this.m_fontStyles[key]);
    }

    void IPostBackEventHandler.RaisePostBackEvent(string eventArgument)
    {
      this.EnsureChildControls();
      if (this.NodeClick == null)
        return;
      this.NodeClick((object) this, new DocumentMapNavigationEventArgs(this.m_selectedNodeHiddenField.Value));
    }
  }
}
