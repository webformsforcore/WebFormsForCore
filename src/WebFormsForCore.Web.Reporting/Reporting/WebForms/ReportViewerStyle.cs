
using Microsoft.ReportingServices.Diagnostics.Utilities;
using System;
using System.Drawing;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal sealed class ReportViewerStyle : Style, IReportViewerStyles
  {
    private const string _ViewStateInternalBorderColor = "InternalBorderColor";
    private const string _ViewStateInternalBorderStyle = "InternalBorderStyle";
    private const string _ViewStateInternalBorderWidth = "InternalBorderWidth";
    private const string _ViewStateToolbarItemBorderColor = "ToolbarItemBorderColor";
    private const string _ViewStateToolbarItemBorderStyle = "ToolbarItemBorderStyle";
    private const string _ViewStateToolbarItemBorderWidth = "ToolbarItemBorderWidth";
    private const string _ViewStateHoverBackColor = "HoverBackColor";
    private const string _ViewStateSplitterBackColor = "SplitterBackColor";
    private const string _ViewStateLinkDisabledColor = "LinkDisabledColor";
    private const string _ViewStateLinkActiveColor = "LinkActiveColor";
    private const string _ViewStateLinkActiveHoverColor = "LinkActiveHoverColor";
    private Label m_waitFontControl = new Label();
    private Label m_waitCancelFontControl = new Label();
    public bool ObeySizeProperties = true;

    public ReportViewerStyle() => this.SetDefaults();

    public ReportViewerStyle(StateBag bag)
      : base(bag)
    {
      this.SetDefaults();
    }

    private void SetDefaults()
    {
      this.BackColor = Color.FromArgb(236, 233, 216);
      this.Font.Name = "Verdana";
      this.Font.Size = FontUnit.Point(8);
      this.m_waitFontControl.Font.Name = "Verdana";
      this.m_waitFontControl.Font.Size = FontUnit.Point(14);
      this.m_waitCancelFontControl.Font.Name = "Verdana";
      this.m_waitCancelFontControl.Font.Size = FontUnit.Point(8);
    }

    public static CssStyleCollection GetStylesForFont(FontInfo font)
    {
      Style style = new Style();
      style.Font.CopyFrom(font);
      return style.GetStyleAttributes((IUrlResolutionService) null);
    }

    public static string GetHtmlStyleForFont(FontInfo font)
    {
      CssStyleCollection stylesForFont = ReportViewerStyle.GetStylesForFont(font);
      return stylesForFont.Count > 0 ? stylesForFont.Value : (string) null;
    }

    public override void AddAttributesToRender(HtmlTextWriter writer, WebControl owner)
    {
      base.AddAttributesToRender(writer, owner);
    }

    protected override void FillStyleAttributes(
      CssStyleCollection attributes,
      IUrlResolutionService urlResolver)
    {
      base.FillStyleAttributes(attributes, urlResolver);
      attributes.Remove(HtmlTextWriterStyle.FontFamily);
      attributes.Remove(HtmlTextWriterStyle.FontSize);
      attributes.Remove(HtmlTextWriterStyle.FontStyle);
      attributes.Remove(HtmlTextWriterStyle.FontVariant);
      attributes.Remove(HtmlTextWriterStyle.FontWeight);
      attributes.Remove(HtmlTextWriterStyle.TextDecoration);
      if (!this.ObeySizeProperties)
      {
        attributes.Remove(HtmlTextWriterStyle.Width);
        attributes.Remove(HtmlTextWriterStyle.Height);
      }
      attributes.Remove(HtmlTextWriterStyle.BackgroundColor);
    }

    public FontInfo WaitMessageFont => this.m_waitFontControl.Font;

    public FontInfo WaitMessageCancelFont => this.m_waitCancelFontControl.Font;

    public BorderStyle InternalBorderStyle
    {
      get
      {
        object obj = this.ViewState[nameof (InternalBorderStyle)];
        return obj == null ? BorderStyle.Solid : (BorderStyle) obj;
      }
      set => this.ViewState[nameof (InternalBorderStyle)] = (object) value;
    }

    public Color InternalBorderColor
    {
      get
      {
        object obj = this.ViewState[nameof (InternalBorderColor)];
        return obj == null ? Color.FromArgb(204, 204, 204) : (Color) obj;
      }
      set => this.ViewState[nameof (InternalBorderColor)] = (object) value;
    }

    public Unit InternalBorderWidth
    {
      get
      {
        object obj = this.ViewState[nameof (InternalBorderWidth)];
        return obj == null ? Unit.Pixel(1) : (Unit) obj;
      }
      set => this.ViewState[nameof (InternalBorderWidth)] = (object) value;
    }

    public BorderStyle ToolbarItemBorderStyle
    {
      get
      {
        object obj = this.ViewState[nameof (ToolbarItemBorderStyle)];
        return obj == null ? BorderStyle.Solid : (BorderStyle) obj;
      }
      set => this.ViewState[nameof (ToolbarItemBorderStyle)] = (object) value;
    }

    public Color ToolbarItemBorderColor
    {
      get
      {
        object obj = this.ViewState[nameof (ToolbarItemBorderColor)];
        return obj == null ? Color.FromArgb(51, 102, 153) : (Color) obj;
      }
      set => this.ViewState[nameof (ToolbarItemBorderColor)] = (object) value;
    }

    public Unit ToolbarItemBorderWidth
    {
      get
      {
        object obj = this.ViewState[nameof (ToolbarItemBorderWidth)];
        return obj == null ? Unit.Pixel(1) : (Unit) obj;
      }
      set => this.ViewState[nameof (ToolbarItemBorderWidth)] = (object) value;
    }

    public Color HoverBackColor
    {
      get
      {
        object obj = this.ViewState[nameof (HoverBackColor)];
        return obj == null ? Color.FromArgb(221, 238, 247) : (Color) obj;
      }
      set => this.ViewState[nameof (HoverBackColor)] = (object) value;
    }

    public Color SplitterBackColor
    {
      get
      {
        object obj = this.ViewState[nameof (SplitterBackColor)];
        return obj == null ? Color.FromArgb(236, 233, 216) : (Color) obj;
      }
      set => this.ViewState[nameof (SplitterBackColor)] = (object) value;
    }

    public Color LinkDisabledColor
    {
      get
      {
        object obj = this.ViewState[nameof (LinkDisabledColor)];
        return obj == null ? Color.Gray : (Color) obj;
      }
      set => this.ViewState[nameof (LinkDisabledColor)] = (object) value;
    }

    public Color LinkActiveColor
    {
      get
      {
        object obj = this.ViewState[nameof (LinkActiveColor)];
        return obj == null ? Color.FromArgb(51, 102, 204) : (Color) obj;
      }
      set => this.ViewState[nameof (LinkActiveColor)] = (object) value;
    }

    public Color LinkActiveHoverColor
    {
      get
      {
        object obj = this.ViewState[nameof (LinkActiveHoverColor)];
        return obj == null ? Color.FromArgb((int) byte.MaxValue, 51, 0) : (Color) obj;
      }
      set => this.ViewState[nameof (LinkActiveHoverColor)] = (object) value;
    }

    private void AddBorderAttributes(
      HtmlTextWriter writer,
      string direction,
      BorderStyle style,
      Color color,
      Unit width)
    {
      string str = ReportViewerStyle.BorderValue(style, color, width);
      if (str.Length <= 0)
        return;
      writer.AddStyleAttribute(direction, str);
    }

    private static string BorderValue(BorderStyle style, Color color, Unit width)
    {
      if (style == BorderStyle.NotSet || style == BorderStyle.None)
        return "";
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} {1} {2}", (object) width.ToString(CultureInfo.InvariantCulture), color == Color.Transparent ? (object) "transparent" : (object) ColorTranslator.ToHtml(color), (object) style.ToString());
    }

    internal static void ApplyButtonStyle(IReportViewerStyles m_viewerStyle, WebControl control)
    {
      if (m_viewerStyle.HoverButtonNormal != null)
      {
        if (control.Enabled)
          control.CssClass = m_viewerStyle.HoverButtonNormal;
        else
          control.CssClass = m_viewerStyle.HoverButtonDisabled;
      }
      else
      {
        if (string.IsNullOrEmpty(m_viewerStyle.NormalButtonBorderValue) || m_viewerStyle.NormalButtonBorderWidth.IsEmpty)
          return;
        if (BrowserDetectionUtility.IsTransparentBorderSupported(HttpContext.Current == null ? (HttpRequest) null : HttpContext.Current.Request))
          control.Style["border"] = m_viewerStyle.NormalButtonBorderValue;
        else
          control.Style["padding"] = m_viewerStyle.NormalButtonBorderWidth.ToString(CultureInfo.InvariantCulture);
      }
    }

    internal static string ToolbarItemStyles(
      IReportViewerStyles m_viewerStyle,
      bool enabled,
      bool normal)
    {
      string input1;
      string input2;
      string buttonBorderValue;
      if (normal)
      {
        input1 = enabled ? m_viewerStyle.HoverButtonNormal : m_viewerStyle.HoverButtonDisabled;
        input2 = "transparent";
        buttonBorderValue = m_viewerStyle.NormalButtonBorderValue;
      }
      else
      {
        input1 = m_viewerStyle.HoverButtonHover;
        input2 = ColorTranslator.ToHtml(m_viewerStyle.HoverBackColor);
        buttonBorderValue = m_viewerStyle.HoverButtonBorderValue;
      }
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{{\"CssClass\":{0},\"Color\":{1},\"Border\":{2}}}", (object) JavaScriptHelper.MakeLiteral(input1), (object) JavaScriptHelper.MakeLiteral(input2), (object) JavaScriptHelper.MakeLiteral(buttonBorderValue));
    }

    bool IReportViewerStyles.GetFontFromCss => false;

    string IReportViewerStyles.NormalButtonBorderValue
    {
      get
      {
        return ReportViewerStyle.BorderValue(this.ToolbarItemBorderStyle, Color.Transparent, ((IReportViewerStyles) this).NormalButtonBorderWidth);
      }
    }

    Unit IReportViewerStyles.NormalButtonBorderWidth => this.ToolbarItemBorderWidth;

    string IReportViewerStyles.HoverButtonBorderValue
    {
      get
      {
        return ReportViewerStyle.BorderValue(this.ToolbarItemBorderStyle, this.ToolbarItemBorderColor, ((IReportViewerStyles) this).NormalButtonBorderWidth);
      }
    }

    string IReportViewerStyles.InternalBorderValue
    {
      get
      {
        return ReportViewerStyle.BorderValue(this.InternalBorderStyle, this.InternalBorderColor, this.InternalBorderWidth);
      }
    }

    void IReportViewerStyles.AddInternalBorderAttributes(HtmlTextWriter writer, string direction)
    {
      this.AddBorderAttributes(writer, direction, this.InternalBorderStyle, this.InternalBorderColor, this.InternalBorderWidth);
    }

    string IReportViewerStyles.HoverButtonNormal => (string) null;

    string IReportViewerStyles.HoverButtonHover => (string) null;

    string IReportViewerStyles.HoverButtonDisabled => (string) null;

    string IReportViewerStyles.Image => (string) null;

    string IReportViewerStyles.ToolbarButtonContainer => (string) null;

    string IReportViewerStyles.ToolbarBackground => (string) null;

    string IReportViewerStyles.ToolbarGroup => (string) null;

    string IReportViewerStyles.ToolbarGroupSpacer => (string) null;

    string IReportViewerStyles.ToolbarGroupShortSpacer => (string) null;

    string IReportViewerStyles.ToolbarInterGroupSpacing => (string) null;

    string IReportViewerStyles.ToolbarText => (string) null;

    string IReportViewerStyles.ToolbarPageNav => (string) null;

    string IReportViewerStyles.ToolbarCurrentPage => (string) null;

    string IReportViewerStyles.ToolbarRefresh => (string) null;

    string IReportViewerStyles.ToolbarZoom => (string) null;

    string IReportViewerStyles.ToolbarFind => (string) null;

    string IReportViewerStyles.ToolbarExport => (string) null;

    string IReportViewerStyles.ToolbarPrint => (string) null;

    string IReportViewerStyles.ToolbarAtomDataFeed => (string) null;

    string IReportViewerStyles.ToolbarParams => (string) null;

    string IReportViewerStyles.LinkActive => (string) null;

    string IReportViewerStyles.LinkDisabled => (string) null;

    string IReportViewerStyles.SplitterNormal => (string) null;

    string IReportViewerStyles.SplitterHover => (string) null;

    string IReportViewerStyles.ViewerAreaBackground => (string) null;

    string IReportViewerStyles.CheckBox => (string) null;

    string IReportViewerStyles.ToolbarTextBox => (string) null;

    string IReportViewerStyles.ParameterTextBox => (string) null;

    string IReportViewerStyles.ParameterDisabledTextBox => (string) null;

    string IReportViewerStyles.ParameterContainer => (string) null;

    string IReportViewerStyles.EmptyDropDown => (string) null;

    string IReportViewerStyles.ViewReportContainer => (string) null;

    string IReportViewerStyles.ParameterLabel => (string) null;

    string IReportViewerStyles.ParameterInput => (string) null;

    string IReportViewerStyles.ParameterColumnSpacer => (string) null;

    string IReportViewerStyles.MultiValueValidValueDropDown => (string) null;

    string IReportViewerStyles.DocMapAndReportFrame => (string) null;

    string IReportViewerStyles.WaitCell => (string) null;

    string IReportViewerStyles.WaitText => (string) null;

    string IReportViewerStyles.CancelLinkDiv => (string) null;

    string IReportViewerStyles.CancelLinkText => (string) null;

    string IReportViewerStyles.WaitControlBackground => (string) null;

    string IReportViewerStyles.DocMapHeader => (string) null;

    string IReportViewerStyles.DocMapContent => (string) null;

    [SpecialName]
    FontInfo IReportViewerStyles.get_Font() => this.Font;

    [SpecialName]
    Color IReportViewerStyles.get_BackColor() => this.BackColor;
  }
}
