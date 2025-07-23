using System.Drawing;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Microsoft.Reporting.WebForms;

internal sealed class WaitControl : CompositeControl
{
	private System.Web.UI.WebControls.Image m_spinningWheel;

	private SafeLiteralControl m_waitText;

	private string m_message;

	private HyperLink m_cancelLink;

	private bool m_cancelLinkVisible;

	private IReportViewerStyles m_styles;

	private string m_cancelUrl;

	public bool CancelLinkVisible
	{
		get
		{
			return m_cancelLinkVisible;
		}
		set
		{
			m_cancelLinkVisible = value;
		}
	}

	public string CancelUrl
	{
		get
		{
			return m_cancelUrl;
		}
		set
		{
			m_cancelUrl = value;
		}
	}

	public WaitControl(IReportViewerStyles styles, string message)
	{
		m_styles = styles;
		m_message = message;
	}

	protected override void CreateChildControls()
	{
		Controls.Clear();
		m_spinningWheel = new System.Web.UI.WebControls.Image();
		m_spinningWheel.AlternateText = LocalizationHelper.Current.ProgressText;
		m_spinningWheel.Width = Unit.Pixel(32);
		m_spinningWheel.Height = Unit.Pixel(32);
		m_spinningWheel.ImageUrl = EmbeddedResourceOperation.CreateUrl("Microsoft.Reporting.WebForms.Icons.SpinningWheel.gif");
		Controls.Add(m_spinningWheel);
		m_waitText = new SafeLiteralControl(m_message);
		Controls.Add(m_waitText);
		m_cancelLink = new HyperLink();
		m_cancelLink.Text = LocalizationHelper.Current.CancelLinkText;
		m_cancelLink.NavigateUrl = m_cancelUrl;
		Controls.Add(m_cancelLink);
	}

	protected override void Render(HtmlTextWriter writer)
	{
		EnsureChildControls();
		if (!m_styles.GetFontFromCss)
		{
			m_waitText.Font.CopyFrom(m_styles.WaitMessageFont);
			m_cancelLink.Font.CopyFrom(m_styles.WaitMessageCancelFont);
		}
		AddAttributesToRender(writer);
		if (m_styles.WaitControlBackground != null)
		{
			writer.AddAttribute(HtmlTextWriterAttribute.Class, m_styles.WaitControlBackground);
		}
		else
		{
			writer.AddStyleAttribute(HtmlTextWriterStyle.Cursor, "wait");
			writer.AddStyleAttribute(HtmlTextWriterStyle.BackgroundColor, ColorTranslator.ToHtml(m_styles.BackColor));
			writer.AddStyleAttribute(HtmlTextWriterStyle.Padding, "15px");
			writer.AddStyleAttribute("border", "1px solid black");
		}
		writer.RenderBeginTag(HtmlTextWriterTag.Div);
		writer.AddAttribute(HtmlTextWriterAttribute.Height, "100%");
		writer.RenderBeginTag(HtmlTextWriterTag.Table);
		writer.RenderBeginTag(HtmlTextWriterTag.Tr);
		writer.AddAttribute(HtmlTextWriterAttribute.Width, m_spinningWheel.Width.ToString(CultureInfo.InvariantCulture));
		writer.AddAttribute(HtmlTextWriterAttribute.Height, m_spinningWheel.Height.ToString(CultureInfo.InvariantCulture));
		writer.RenderBeginTag(HtmlTextWriterTag.Td);
		m_spinningWheel.RenderControl(writer);
		writer.RenderEndTag();
		if (m_styles.WaitCell != null)
		{
			writer.AddAttribute(HtmlTextWriterAttribute.Class, m_styles.WaitCell);
		}
		else
		{
			writer.AddStyleAttribute("vertical-align", "middle");
			writer.AddStyleAttribute("text-align", "center");
		}
		writer.RenderBeginTag(HtmlTextWriterTag.Td);
		if (m_styles.WaitText != null)
		{
			m_waitText.CssClass = m_styles.WaitText;
		}
		m_waitText.RenderControl(writer);
		if (m_styles.CancelLinkDiv != null)
		{
			writer.AddAttribute(HtmlTextWriterAttribute.Class, m_styles.CancelLinkDiv);
		}
		else
		{
			writer.AddStyleAttribute(HtmlTextWriterStyle.MarginTop, "3px");
		}
		if (m_cancelLinkVisible)
		{
			writer.RenderBeginTag(HtmlTextWriterTag.Div);
			if (m_styles.CancelLinkText != null)
			{
				m_cancelLink.CssClass = m_styles.CancelLinkText;
			}
			else
			{
				m_cancelLink.Style.Add(HtmlTextWriterStyle.Color, ColorTranslator.ToHtml(m_styles.LinkActiveColor));
			}
			m_cancelLink.RenderControl(writer);
			writer.RenderEndTag();
		}
		writer.RenderEndTag();
		writer.RenderEndTag();
		writer.RenderEndTag();
		writer.RenderEndTag();
	}
}
