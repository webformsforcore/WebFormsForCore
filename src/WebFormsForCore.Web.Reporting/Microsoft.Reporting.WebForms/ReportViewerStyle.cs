using System.Drawing;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.ReportingServices.Diagnostics.Utilities;

namespace Microsoft.Reporting.WebForms;

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

	public FontInfo WaitMessageFont => m_waitFontControl.Font;

	public FontInfo WaitMessageCancelFont => m_waitCancelFontControl.Font;

	public BorderStyle InternalBorderStyle
	{
		get
		{
			object obj = base.ViewState["InternalBorderStyle"];
			if (obj == null)
			{
				return BorderStyle.Solid;
			}
			return (BorderStyle)obj;
		}
		set
		{
			base.ViewState["InternalBorderStyle"] = value;
		}
	}

	public Color InternalBorderColor
	{
		get
		{
			object obj = base.ViewState["InternalBorderColor"];
			if (obj == null)
			{
				return Color.FromArgb(204, 204, 204);
			}
			return (Color)obj;
		}
		set
		{
			base.ViewState["InternalBorderColor"] = value;
		}
	}

	public Unit InternalBorderWidth
	{
		get
		{
			object obj = base.ViewState["InternalBorderWidth"];
			if (obj == null)
			{
				return Unit.Pixel(1);
			}
			return (Unit)obj;
		}
		set
		{
			base.ViewState["InternalBorderWidth"] = value;
		}
	}

	public BorderStyle ToolbarItemBorderStyle
	{
		get
		{
			object obj = base.ViewState["ToolbarItemBorderStyle"];
			if (obj == null)
			{
				return BorderStyle.Solid;
			}
			return (BorderStyle)obj;
		}
		set
		{
			base.ViewState["ToolbarItemBorderStyle"] = value;
		}
	}

	public Color ToolbarItemBorderColor
	{
		get
		{
			object obj = base.ViewState["ToolbarItemBorderColor"];
			if (obj == null)
			{
				return Color.FromArgb(51, 102, 153);
			}
			return (Color)obj;
		}
		set
		{
			base.ViewState["ToolbarItemBorderColor"] = value;
		}
	}

	public Unit ToolbarItemBorderWidth
	{
		get
		{
			object obj = base.ViewState["ToolbarItemBorderWidth"];
			if (obj == null)
			{
				return Unit.Pixel(1);
			}
			return (Unit)obj;
		}
		set
		{
			base.ViewState["ToolbarItemBorderWidth"] = value;
		}
	}

	public Color HoverBackColor
	{
		get
		{
			object obj = base.ViewState["HoverBackColor"];
			if (obj == null)
			{
				return Color.FromArgb(221, 238, 247);
			}
			return (Color)obj;
		}
		set
		{
			base.ViewState["HoverBackColor"] = value;
		}
	}

	public Color SplitterBackColor
	{
		get
		{
			object obj = base.ViewState["SplitterBackColor"];
			if (obj == null)
			{
				return Color.FromArgb(236, 233, 216);
			}
			return (Color)obj;
		}
		set
		{
			base.ViewState["SplitterBackColor"] = value;
		}
	}

	public Color LinkDisabledColor
	{
		get
		{
			object obj = base.ViewState["LinkDisabledColor"];
			if (obj == null)
			{
				return Color.Gray;
			}
			return (Color)obj;
		}
		set
		{
			base.ViewState["LinkDisabledColor"] = value;
		}
	}

	public Color LinkActiveColor
	{
		get
		{
			object obj = base.ViewState["LinkActiveColor"];
			if (obj == null)
			{
				return Color.FromArgb(51, 102, 204);
			}
			return (Color)obj;
		}
		set
		{
			base.ViewState["LinkActiveColor"] = value;
		}
	}

	public Color LinkActiveHoverColor
	{
		get
		{
			object obj = base.ViewState["LinkActiveHoverColor"];
			if (obj == null)
			{
				return Color.FromArgb(255, 51, 0);
			}
			return (Color)obj;
		}
		set
		{
			base.ViewState["LinkActiveHoverColor"] = value;
		}
	}

	bool IReportViewerStyles.GetFontFromCss => false;

	string IReportViewerStyles.NormalButtonBorderValue => BorderValue(ToolbarItemBorderStyle, Color.Transparent, ((IReportViewerStyles)this).NormalButtonBorderWidth);

	Unit IReportViewerStyles.NormalButtonBorderWidth => ToolbarItemBorderWidth;

	string IReportViewerStyles.HoverButtonBorderValue => BorderValue(ToolbarItemBorderStyle, ToolbarItemBorderColor, ((IReportViewerStyles)this).NormalButtonBorderWidth);

	string IReportViewerStyles.InternalBorderValue => BorderValue(InternalBorderStyle, InternalBorderColor, InternalBorderWidth);

	string IReportViewerStyles.HoverButtonNormal => null;

	string IReportViewerStyles.HoverButtonHover => null;

	string IReportViewerStyles.HoverButtonDisabled => null;

	string IReportViewerStyles.Image => null;

	string IReportViewerStyles.ToolbarButtonContainer => null;

	string IReportViewerStyles.ToolbarBackground => null;

	string IReportViewerStyles.ToolbarGroup => null;

	string IReportViewerStyles.ToolbarGroupSpacer => null;

	string IReportViewerStyles.ToolbarGroupShortSpacer => null;

	string IReportViewerStyles.ToolbarInterGroupSpacing => null;

	string IReportViewerStyles.ToolbarText => null;

	string IReportViewerStyles.ToolbarPageNav => null;

	string IReportViewerStyles.ToolbarCurrentPage => null;

	string IReportViewerStyles.ToolbarRefresh => null;

	string IReportViewerStyles.ToolbarZoom => null;

	string IReportViewerStyles.ToolbarFind => null;

	string IReportViewerStyles.ToolbarExport => null;

	string IReportViewerStyles.ToolbarPrint => null;

	string IReportViewerStyles.ToolbarAtomDataFeed => null;

	string IReportViewerStyles.ToolbarParams => null;

	string IReportViewerStyles.LinkActive => null;

	string IReportViewerStyles.LinkDisabled => null;

	string IReportViewerStyles.SplitterNormal => null;

	string IReportViewerStyles.SplitterHover => null;

	string IReportViewerStyles.ViewerAreaBackground => null;

	string IReportViewerStyles.CheckBox => null;

	string IReportViewerStyles.ToolbarTextBox => null;

	string IReportViewerStyles.ParameterTextBox => null;

	string IReportViewerStyles.ParameterDisabledTextBox => null;

	string IReportViewerStyles.ParameterContainer => null;

	string IReportViewerStyles.EmptyDropDown => null;

	string IReportViewerStyles.ViewReportContainer => null;

	string IReportViewerStyles.ParameterLabel => null;

	string IReportViewerStyles.ParameterInput => null;

	string IReportViewerStyles.ParameterColumnSpacer => null;

	string IReportViewerStyles.MultiValueValidValueDropDown => null;

	string IReportViewerStyles.DocMapAndReportFrame => null;

	string IReportViewerStyles.WaitCell => null;

	string IReportViewerStyles.WaitText => null;

	string IReportViewerStyles.CancelLinkDiv => null;

	string IReportViewerStyles.CancelLinkText => null;

	string IReportViewerStyles.WaitControlBackground => null;

	string IReportViewerStyles.DocMapHeader => null;

	string IReportViewerStyles.DocMapContent => null;

	public ReportViewerStyle()
	{
		SetDefaults();
	}

	public ReportViewerStyle(StateBag bag)
		: base(bag)
	{
		SetDefaults();
	}

	private void SetDefaults()
	{
		base.BackColor = Color.FromArgb(236, 233, 216);
		base.Font.Name = "Verdana";
		base.Font.Size = FontUnit.Point(8);
		m_waitFontControl.Font.Name = "Verdana";
		m_waitFontControl.Font.Size = FontUnit.Point(14);
		m_waitCancelFontControl.Font.Name = "Verdana";
		m_waitCancelFontControl.Font.Size = FontUnit.Point(8);
	}

	public static CssStyleCollection GetStylesForFont(FontInfo font)
	{
		Style style = new Style();
		style.Font.CopyFrom(font);
		return style.GetStyleAttributes(null);
	}

	public static string GetHtmlStyleForFont(FontInfo font)
	{
		CssStyleCollection stylesForFont = GetStylesForFont(font);
		if (stylesForFont.Count > 0)
		{
			return stylesForFont.Value;
		}
		return null;
	}

	public override void AddAttributesToRender(HtmlTextWriter writer, WebControl owner)
	{
		base.AddAttributesToRender(writer, owner);
	}

	protected override void FillStyleAttributes(CssStyleCollection attributes, IUrlResolutionService urlResolver)
	{
		base.FillStyleAttributes(attributes, urlResolver);
		attributes.Remove(HtmlTextWriterStyle.FontFamily);
		attributes.Remove(HtmlTextWriterStyle.FontSize);
		attributes.Remove(HtmlTextWriterStyle.FontStyle);
		attributes.Remove(HtmlTextWriterStyle.FontVariant);
		attributes.Remove(HtmlTextWriterStyle.FontWeight);
		attributes.Remove(HtmlTextWriterStyle.TextDecoration);
		if (!ObeySizeProperties)
		{
			attributes.Remove(HtmlTextWriterStyle.Width);
			attributes.Remove(HtmlTextWriterStyle.Height);
		}
		attributes.Remove(HtmlTextWriterStyle.BackgroundColor);
	}

	private void AddBorderAttributes(HtmlTextWriter writer, string direction, BorderStyle style, Color color, Unit width)
	{
		string text = BorderValue(style, color, width);
		if (text.Length > 0)
		{
			writer.AddStyleAttribute(direction, text);
		}
	}

	private static string BorderValue(BorderStyle style, Color color, Unit width)
	{
		if (style == BorderStyle.NotSet || style == BorderStyle.None)
		{
			return "";
		}
		string format = "{0} {1} {2}";
		return string.Format(CultureInfo.InvariantCulture, format, width.ToString(CultureInfo.InvariantCulture), (color == Color.Transparent) ? "transparent" : ColorTranslator.ToHtml(color), style.ToString());
	}

	internal static void ApplyButtonStyle(IReportViewerStyles m_viewerStyle, WebControl control)
	{
		if (m_viewerStyle.HoverButtonNormal != null)
		{
			if (control.Enabled)
			{
				control.CssClass = m_viewerStyle.HoverButtonNormal;
			}
			else
			{
				control.CssClass = m_viewerStyle.HoverButtonDisabled;
			}
		}
		else if (!string.IsNullOrEmpty(m_viewerStyle.NormalButtonBorderValue) && !m_viewerStyle.NormalButtonBorderWidth.IsEmpty)
		{
			if (BrowserDetectionUtility.IsTransparentBorderSupported((HttpContext.Current == null) ? null : HttpContext.Current.Request))
			{
				control.Style["border"] = m_viewerStyle.NormalButtonBorderValue;
			}
			else
			{
				control.Style["padding"] = m_viewerStyle.NormalButtonBorderWidth.ToString(CultureInfo.InvariantCulture);
			}
		}
	}

	internal static string ToolbarItemStyles(IReportViewerStyles m_viewerStyle, bool enabled, bool normal)
	{
		string input;
		string input2;
		string input3;
		if (normal)
		{
			input = (enabled ? m_viewerStyle.HoverButtonNormal : m_viewerStyle.HoverButtonDisabled);
			input2 = "transparent";
			input3 = m_viewerStyle.NormalButtonBorderValue;
		}
		else
		{
			input = m_viewerStyle.HoverButtonHover;
			input2 = ColorTranslator.ToHtml(m_viewerStyle.HoverBackColor);
			input3 = m_viewerStyle.HoverButtonBorderValue;
		}
		return string.Format(CultureInfo.InvariantCulture, "{{\"CssClass\":{0},\"Color\":{1},\"Border\":{2}}}", JavaScriptHelper.MakeLiteral(input), JavaScriptHelper.MakeLiteral(input2), JavaScriptHelper.MakeLiteral(input3));
	}

	void IReportViewerStyles.AddInternalBorderAttributes(HtmlTextWriter writer, string direction)
	{
		AddBorderAttributes(writer, direction, InternalBorderStyle, InternalBorderColor, InternalBorderWidth);
	}

	[SpecialName]
	FontInfo IReportViewerStyles.get_Font()
	{
		return base.Font;
	}

	[SpecialName]
	Color IReportViewerStyles.get_BackColor()
	{
		return base.BackColor;
	}
}
