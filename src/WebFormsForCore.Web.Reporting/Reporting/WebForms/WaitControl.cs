// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.WaitControl
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

using System.Drawing;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal sealed class WaitControl : CompositeControl
  {
    private System.Web.UI.WebControls.Image m_spinningWheel;
    private SafeLiteralControl m_waitText;
    private string m_message;
    private HyperLink m_cancelLink;
    private bool m_cancelLinkVisible;
    private IReportViewerStyles m_styles;
    private string m_cancelUrl;

    public WaitControl(IReportViewerStyles styles, string message)
    {
      this.m_styles = styles;
      this.m_message = message;
    }

    public bool CancelLinkVisible
    {
      get => this.m_cancelLinkVisible;
      set => this.m_cancelLinkVisible = value;
    }

    public string CancelUrl
    {
      get => this.m_cancelUrl;
      set => this.m_cancelUrl = value;
    }

    protected override void CreateChildControls()
    {
      this.Controls.Clear();
      this.m_spinningWheel = new System.Web.UI.WebControls.Image();
      this.m_spinningWheel.AlternateText = LocalizationHelper.Current.ProgressText;
      this.m_spinningWheel.Width = Unit.Pixel(32);
      this.m_spinningWheel.Height = Unit.Pixel(32);
      this.m_spinningWheel.ImageUrl = EmbeddedResourceOperation.CreateUrl("Microsoft.Reporting.WebForms.Icons.SpinningWheel.gif");
      this.Controls.Add((Control) this.m_spinningWheel);
      this.m_waitText = new SafeLiteralControl(this.m_message);
      this.Controls.Add((Control) this.m_waitText);
      this.m_cancelLink = new HyperLink();
      this.m_cancelLink.Text = LocalizationHelper.Current.CancelLinkText;
      this.m_cancelLink.NavigateUrl = this.m_cancelUrl;
      this.Controls.Add((Control) this.m_cancelLink);
    }

    protected override void Render(HtmlTextWriter writer)
    {
      this.EnsureChildControls();
      if (!this.m_styles.GetFontFromCss)
      {
        this.m_waitText.Font.CopyFrom(this.m_styles.WaitMessageFont);
        this.m_cancelLink.Font.CopyFrom(this.m_styles.WaitMessageCancelFont);
      }
      this.AddAttributesToRender(writer);
      if (this.m_styles.WaitControlBackground != null)
      {
        writer.AddAttribute(HtmlTextWriterAttribute.Class, this.m_styles.WaitControlBackground);
      }
      else
      {
        writer.AddStyleAttribute(HtmlTextWriterStyle.Cursor, "wait");
        writer.AddStyleAttribute(HtmlTextWriterStyle.BackgroundColor, ColorTranslator.ToHtml(this.m_styles.BackColor));
        writer.AddStyleAttribute(HtmlTextWriterStyle.Padding, "15px");
        writer.AddStyleAttribute("border", "1px solid black");
      }
      writer.RenderBeginTag(HtmlTextWriterTag.Div);
      writer.AddAttribute(HtmlTextWriterAttribute.Height, "100%");
      writer.RenderBeginTag(HtmlTextWriterTag.Table);
      writer.RenderBeginTag(HtmlTextWriterTag.Tr);
      writer.AddAttribute(HtmlTextWriterAttribute.Width, this.m_spinningWheel.Width.ToString(CultureInfo.InvariantCulture));
      writer.AddAttribute(HtmlTextWriterAttribute.Height, this.m_spinningWheel.Height.ToString(CultureInfo.InvariantCulture));
      writer.RenderBeginTag(HtmlTextWriterTag.Td);
      this.m_spinningWheel.RenderControl(writer);
      writer.RenderEndTag();
      if (this.m_styles.WaitCell != null)
      {
        writer.AddAttribute(HtmlTextWriterAttribute.Class, this.m_styles.WaitCell);
      }
      else
      {
        writer.AddStyleAttribute("vertical-align", "middle");
        writer.AddStyleAttribute("text-align", "center");
      }
      writer.RenderBeginTag(HtmlTextWriterTag.Td);
      if (this.m_styles.WaitText != null)
        this.m_waitText.CssClass = this.m_styles.WaitText;
      this.m_waitText.RenderControl(writer);
      if (this.m_styles.CancelLinkDiv != null)
        writer.AddAttribute(HtmlTextWriterAttribute.Class, this.m_styles.CancelLinkDiv);
      else
        writer.AddStyleAttribute(HtmlTextWriterStyle.MarginTop, "3px");
      if (this.m_cancelLinkVisible)
      {
        writer.RenderBeginTag(HtmlTextWriterTag.Div);
        if (this.m_styles.CancelLinkText != null)
          this.m_cancelLink.CssClass = this.m_styles.CancelLinkText;
        else
          this.m_cancelLink.Style.Add(HtmlTextWriterStyle.Color, ColorTranslator.ToHtml(this.m_styles.LinkActiveColor));
        this.m_cancelLink.RenderControl(writer);
        writer.RenderEndTag();
      }
      writer.RenderEndTag();
      writer.RenderEndTag();
      writer.RenderEndTag();
      writer.RenderEndTag();
    }
  }
}
