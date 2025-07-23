using Microsoft.ReportingServices.Rendering.RPLProcessing;

namespace Microsoft.ReportingServices.Rendering.HtmlRenderer;

internal class ParagraphStyleWriter : ElementStyleWriter
{
	internal enum Mode
	{
		ListOnly = 1,
		ParagraphOnly,
		All
	}

	private RPLParagraph m_paragraph;

	private RPLTextBox m_textBox;

	private bool m_outputSharedInNonShared;

	private Mode m_mode = Mode.All;

	private int m_currentListLevel;

	internal RPLParagraph Paragraph
	{
		get
		{
			return m_paragraph;
		}
		set
		{
			m_paragraph = value;
		}
	}

	internal Mode ParagraphMode
	{
		get
		{
			return m_mode;
		}
		set
		{
			m_mode = value;
		}
	}

	internal int CurrentListLevel
	{
		get
		{
			return m_currentListLevel;
		}
		set
		{
			m_currentListLevel = value;
		}
	}

	internal bool OutputSharedInNonShared
	{
		get
		{
			return m_outputSharedInNonShared;
		}
		set
		{
			m_outputSharedInNonShared = value;
		}
	}

	internal ParagraphStyleWriter(HTML4Renderer renderer, RPLTextBox textBox)
		: base(renderer)
	{
		m_textBox = textBox;
	}

	internal override bool NeedsToWriteNullStyle(StyleWriterMode mode)
	{
		RPLParagraph paragraph = m_paragraph;
		switch (mode)
		{
		case StyleWriterMode.NonShared:
		{
			RPLElementProps elementProps = ((RPLElement)paragraph).ElementProps;
			RPLParagraphProps val2 = (RPLParagraphProps)(object)((elementProps is RPLParagraphProps) ? elementProps : null);
			if (val2.LeftIndent != null || val2.RightIndent != null || val2.SpaceBefore != null || val2.SpaceAfter != null || val2.HangingIndent != null)
			{
				return true;
			}
			_ = ((RPLElement)m_textBox).ElementProps.NonSharedStyle;
			if (m_outputSharedInNonShared)
			{
				return true;
			}
			break;
		}
		case StyleWriterMode.Shared:
		{
			if (m_outputSharedInNonShared)
			{
				return false;
			}
			RPLElementPropsDef definition = ((RPLElement)paragraph).ElementProps.Definition;
			RPLParagraphPropsDef val = (RPLParagraphPropsDef)(object)((definition is RPLParagraphPropsDef) ? definition : null);
			if (val.LeftIndent != null || val.RightIndent != null || val.SpaceBefore != null || val.SpaceAfter != null || val.HangingIndent != null)
			{
				return true;
			}
			IRPLStyle sharedStyle = (IRPLStyle)(object)((RPLElement)m_textBox).ElementPropsDef.SharedStyle;
			if (sharedStyle != null && HTML4Renderer.IsDirectionRTL(sharedStyle))
			{
				return true;
			}
			break;
		}
		}
		return false;
	}

	internal override void WriteStyles(StyleWriterMode mode, IRPLStyle style)
	{
		//IL_0342: Unknown result type (might be due to invalid IL or missing references)
		//IL_0351: Unknown result type (might be due to invalid IL or missing references)
		//IL_034a: Unknown result type (might be due to invalid IL or missing references)
		//IL_034f: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
		RPLParagraph paragraph = m_paragraph;
		RPLTextBox textBox = m_textBox;
		RPLElementProps elementProps = ((RPLElement)textBox).ElementProps;
		RPLTextBoxProps val = (RPLTextBoxProps)(object)((elementProps is RPLTextBoxProps) ? elementProps : null);
		if (paragraph != null)
		{
			RPLElementProps elementProps2 = ((RPLElement)paragraph).ElementProps;
			RPLParagraphProps val2 = (RPLParagraphProps)(object)((elementProps2 is RPLParagraphProps) ? elementProps2 : null);
			RPLElementPropsDef definition = ((RPLElementProps)val2).Definition;
			RPLParagraphPropsDef val3 = (RPLParagraphPropsDef)(object)((definition is RPLParagraphPropsDef) ? definition : null);
			RPLReportSize val4 = null;
			RPLReportSize leftIndent = null;
			RPLReportSize rightIndent = null;
			RPLReportSize spaceBefore = null;
			RPLReportSize spaceAfter = null;
			switch (mode)
			{
			case StyleWriterMode.All:
				val4 = val2.HangingIndent;
				if (val4 == null)
				{
					val4 = val3.HangingIndent;
				}
				leftIndent = val2.LeftIndent;
				if (leftIndent == null)
				{
					leftIndent = val3.LeftIndent;
				}
				rightIndent = val2.RightIndent;
				if (rightIndent == null)
				{
					rightIndent = val3.RightIndent;
				}
				spaceBefore = val2.SpaceBefore;
				if (spaceBefore == null)
				{
					spaceBefore = val3.SpaceBefore;
				}
				spaceAfter = val2.SpaceAfter;
				if (spaceAfter == null)
				{
					spaceAfter = val3.SpaceAfter;
				}
				break;
			case StyleWriterMode.NonShared:
			{
				_ = ((RPLElement)m_textBox).ElementProps.NonSharedStyle;
				val4 = val2.HangingIndent;
				rightIndent = val2.RightIndent;
				leftIndent = val2.LeftIndent;
				spaceAfter = val2.SpaceAfter;
				spaceBefore = val2.SpaceBefore;
				if (m_outputSharedInNonShared)
				{
					if (val4 == null)
					{
						val4 = val3.HangingIndent;
					}
					if (rightIndent == null)
					{
						rightIndent = val3.RightIndent;
					}
					if (leftIndent == null)
					{
						leftIndent = val3.LeftIndent;
					}
					if (spaceAfter == null)
					{
						spaceAfter = val3.SpaceAfter;
					}
					if (spaceBefore == null)
					{
						spaceBefore = val3.SpaceBefore;
					}
					break;
				}
				bool flag = HTML4Renderer.IsDirectionRTL((IRPLStyle)(object)((RPLElement)m_textBox).ElementProps.Style);
				if (val4 == null)
				{
					if (flag)
					{
						if (rightIndent != null)
						{
							val4 = val3.HangingIndent;
						}
					}
					else if (leftIndent != null)
					{
						val4 = val3.HangingIndent;
					}
				}
				else if (flag)
				{
					if (rightIndent == null)
					{
						rightIndent = val3.RightIndent;
					}
				}
				else if (leftIndent == null)
				{
					leftIndent = val3.LeftIndent;
				}
				break;
			}
			case StyleWriterMode.Shared:
				_ = ((RPLElement)m_textBox).ElementPropsDef.SharedStyle;
				val4 = val3.HangingIndent;
				leftIndent = val3.LeftIndent;
				rightIndent = val3.RightIndent;
				spaceBefore = val3.SpaceBefore;
				spaceAfter = val3.SpaceAfter;
				break;
			}
			if (m_currentListLevel > 0 && val4 != null && val4.ToMillimeters() < 0.0 && !m_renderer.IsBrowserIE)
			{
				val4 = null;
			}
			if (m_mode != Mode.ParagraphOnly)
			{
				FixIndents(ref leftIndent, ref rightIndent, ref spaceBefore, ref spaceAfter, val4);
				bool flag2 = HTML4Renderer.IsWritingModeVertical((IRPLStyle)(object)((RPLElementProps)val).Style);
				if (flag2 && m_renderer.IsBrowserIE)
				{
					WriteStyle(HTML4Renderer.m_paddingLeft, leftIndent);
				}
				else
				{
					WriteStyle(HTML4Renderer.m_marginLeft, leftIndent);
				}
				WriteStyle(HTML4Renderer.m_marginRight, rightIndent);
				WriteStyle(HTML4Renderer.m_marginTop, spaceBefore);
				if (flag2 && m_renderer.IsBrowserIE)
				{
					WriteStyle(HTML4Renderer.m_marginBottom, spaceAfter);
				}
				else
				{
					WriteStyle(HTML4Renderer.m_paddingBottom, spaceAfter);
				}
			}
			if (m_mode == Mode.ListOnly)
			{
				WriteStyle(HTML4Renderer.m_fontFamily, "Arial");
				WriteStyle(HTML4Renderer.m_fontSize, "10pt");
			}
			else if (val4 != null && val4.ToMillimeters() < 0.0)
			{
				WriteStyle(HTML4Renderer.m_textIndent, val4);
			}
		}
		if (style == null || (m_mode != Mode.All && m_mode != Mode.ParagraphOnly))
		{
			return;
		}
		object obj = style[(byte)25];
		if (obj != null || mode != StyleWriterMode.NonShared)
		{
			TextAlignments val5 = (TextAlignments)0;
			if (obj != null)
			{
				val5 = (TextAlignments)obj;
			}
			if ((int)val5 == 0)
			{
				bool flag3 = HTML4Renderer.GetTextAlignForType(val);
				if (HTML4Renderer.IsDirectionRTL((IRPLStyle)(object)((RPLElementProps)val).Style))
				{
					flag3 = !flag3;
				}
				WriteStream(HTML4Renderer.m_textAlign);
				if (flag3)
				{
					WriteStream(HTML4Renderer.m_rightValue);
				}
				else
				{
					WriteStream(HTML4Renderer.m_leftValue);
				}
				WriteStream(HTML4Renderer.m_semiColon);
			}
			else
			{
				WriteStyle(HTML4Renderer.m_textAlign, EnumStrings.GetValue(val5), null);
			}
		}
		WriteStyle(HTML4Renderer.m_lineHeight, style[(byte)28]);
	}

	internal void FixIndents(ref RPLReportSize leftIndent, ref RPLReportSize rightIndent, ref RPLReportSize spaceBefore, ref RPLReportSize spaceAfter, RPLReportSize hangingIndent)
	{
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		RPLElementProps elementProps = ((RPLElement)m_textBox).ElementProps;
		RPLTextBoxProps val = (RPLTextBoxProps)(object)((elementProps is RPLTextBoxProps) ? elementProps : null);
		if (HTML4Renderer.IsDirectionRTL((IRPLStyle)(object)((RPLElementProps)val).Style))
		{
			rightIndent = FixHangingIndent(rightIndent, hangingIndent);
		}
		else
		{
			leftIndent = FixHangingIndent(leftIndent, hangingIndent);
		}
		object obj = ((RPLElementProps)val).Style[(byte)30];
		if (m_renderer.IsBrowserIE && obj != null && HTML4Renderer.IsWritingModeVertical((WritingModes)obj))
		{
			RPLReportSize val2 = leftIndent;
			leftIndent = spaceAfter;
			spaceAfter = rightIndent;
			rightIndent = spaceBefore;
			spaceBefore = val2;
		}
	}

	internal RPLReportSize FixHangingIndent(RPLReportSize leftIndent, RPLReportSize hangingIndent)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected O, but got Unknown
		if (hangingIndent == null)
		{
			return leftIndent;
		}
		double num = hangingIndent.ToMillimeters();
		if (num < 0.0)
		{
			double num2 = 0.0;
			if (leftIndent != null)
			{
				num2 = leftIndent.ToMillimeters();
			}
			num2 -= num;
			leftIndent = new RPLReportSize(num2);
		}
		return leftIndent;
	}
}
