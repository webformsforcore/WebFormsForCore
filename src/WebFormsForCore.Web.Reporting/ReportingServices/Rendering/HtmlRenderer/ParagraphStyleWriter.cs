
using Microsoft.ReportingServices.Rendering.RPLProcessing;

#nullable disable
namespace Microsoft.ReportingServices.Rendering.HtmlRenderer
{
  internal class ParagraphStyleWriter : ElementStyleWriter
  {
    private RPLParagraph m_paragraph;
    private RPLTextBox m_textBox;
    private bool m_outputSharedInNonShared;
    private ParagraphStyleWriter.Mode m_mode = ParagraphStyleWriter.Mode.All;
    private int m_currentListLevel;

    internal ParagraphStyleWriter(HTML4Renderer renderer, RPLTextBox textBox)
      : base(renderer)
    {
      this.m_textBox = textBox;
    }

    internal RPLParagraph Paragraph
    {
      get => this.m_paragraph;
      set => this.m_paragraph = value;
    }

    internal ParagraphStyleWriter.Mode ParagraphMode
    {
      get => this.m_mode;
      set => this.m_mode = value;
    }

    internal int CurrentListLevel
    {
      get => this.m_currentListLevel;
      set => this.m_currentListLevel = value;
    }

    internal bool OutputSharedInNonShared
    {
      get => this.m_outputSharedInNonShared;
      set => this.m_outputSharedInNonShared = value;
    }

    internal override bool NeedsToWriteNullStyle(StyleWriterMode mode)
    {
      RPLParagraph paragraph = this.m_paragraph;
      switch (mode)
      {
        case StyleWriterMode.NonShared:
          RPLParagraphProps elementProps = ((RPLElement) paragraph).ElementProps as RPLParagraphProps;
          if (elementProps.LeftIndent != null || elementProps.RightIndent != null || elementProps.SpaceBefore != null || elementProps.SpaceAfter != null || elementProps.HangingIndent != null)
            return true;
          RPLStyleProps nonSharedStyle = ((RPLElement) this.m_textBox).ElementProps.NonSharedStyle;
          if (this.m_outputSharedInNonShared)
            return true;
          break;
        case StyleWriterMode.Shared:
          if (this.m_outputSharedInNonShared)
            return false;
          RPLParagraphPropsDef definition = ((RPLElement) paragraph).ElementProps.Definition as RPLParagraphPropsDef;
          if (definition.LeftIndent != null || definition.RightIndent != null || definition.SpaceBefore != null || definition.SpaceAfter != null || definition.HangingIndent != null)
            return true;
          IRPLStyle sharedStyle = (IRPLStyle) ((RPLElement) this.m_textBox).ElementPropsDef.SharedStyle;
          if (sharedStyle != null && HTML4Renderer.IsDirectionRTL(sharedStyle))
            return true;
          break;
      }
      return false;
    }

    internal override void WriteStyles(StyleWriterMode mode, IRPLStyle style)
    {
      RPLParagraph paragraph = this.m_paragraph;
      RPLTextBoxProps elementProps1 = ((RPLElement) this.m_textBox).ElementProps as RPLTextBoxProps;
      if (paragraph != null)
      {
        RPLParagraphProps elementProps2 = ((RPLElement) paragraph).ElementProps as RPLParagraphProps;
        RPLParagraphPropsDef definition = ((RPLElementProps) elementProps2).Definition as RPLParagraphPropsDef;
        RPLReportSize hangingIndent = (RPLReportSize) null;
        RPLReportSize leftIndent = (RPLReportSize) null;
        RPLReportSize rightIndent = (RPLReportSize) null;
        RPLReportSize spaceBefore = (RPLReportSize) null;
        RPLReportSize spaceAfter = (RPLReportSize) null;
        switch (mode)
        {
          case StyleWriterMode.NonShared:
            RPLStyleProps nonSharedStyle = ((RPLElement) this.m_textBox).ElementProps.NonSharedStyle;
            hangingIndent = elementProps2.HangingIndent;
            rightIndent = elementProps2.RightIndent;
            leftIndent = elementProps2.LeftIndent;
            spaceAfter = elementProps2.SpaceAfter;
            spaceBefore = elementProps2.SpaceBefore;
            if (this.m_outputSharedInNonShared)
            {
              if (hangingIndent == null)
                hangingIndent = definition.HangingIndent;
              if (rightIndent == null)
                rightIndent = definition.RightIndent;
              if (leftIndent == null)
                leftIndent = definition.LeftIndent;
              if (spaceAfter == null)
                spaceAfter = definition.SpaceAfter;
              if (spaceBefore == null)
              {
                spaceBefore = definition.SpaceBefore;
                break;
              }
              break;
            }
            bool flag1 = HTML4Renderer.IsDirectionRTL((IRPLStyle) ((RPLElement) this.m_textBox).ElementProps.Style);
            if (hangingIndent == null)
            {
              if (flag1)
              {
                if (rightIndent != null)
                {
                  hangingIndent = definition.HangingIndent;
                  break;
                }
                break;
              }
              if (leftIndent != null)
              {
                hangingIndent = definition.HangingIndent;
                break;
              }
              break;
            }
            if (flag1)
            {
              if (rightIndent == null)
              {
                rightIndent = definition.RightIndent;
                break;
              }
              break;
            }
            if (leftIndent == null)
            {
              leftIndent = definition.LeftIndent;
              break;
            }
            break;
          case StyleWriterMode.Shared:
            RPLStyleProps sharedStyle = ((RPLElement) this.m_textBox).ElementPropsDef.SharedStyle;
            hangingIndent = definition.HangingIndent;
            leftIndent = definition.LeftIndent;
            rightIndent = definition.RightIndent;
            spaceBefore = definition.SpaceBefore;
            spaceAfter = definition.SpaceAfter;
            break;
          case StyleWriterMode.All:
            hangingIndent = elementProps2.HangingIndent ?? definition.HangingIndent;
            leftIndent = elementProps2.LeftIndent ?? definition.LeftIndent;
            rightIndent = elementProps2.RightIndent ?? definition.RightIndent;
            spaceBefore = elementProps2.SpaceBefore ?? definition.SpaceBefore;
            spaceAfter = elementProps2.SpaceAfter ?? definition.SpaceAfter;
            break;
        }
        if (this.m_currentListLevel > 0 && hangingIndent != null && hangingIndent.ToMillimeters() < 0.0 && !this.m_renderer.IsBrowserIE)
          hangingIndent = (RPLReportSize) null;
        if (this.m_mode != ParagraphStyleWriter.Mode.ParagraphOnly)
        {
          this.FixIndents(ref leftIndent, ref rightIndent, ref spaceBefore, ref spaceAfter, hangingIndent);
          bool flag2 = HTML4Renderer.IsWritingModeVertical((IRPLStyle) ((RPLElementProps) elementProps1).Style);
          if (flag2 && this.m_renderer.IsBrowserIE)
            this.WriteStyle(HTML4Renderer.m_paddingLeft, (object) leftIndent);
          else
            this.WriteStyle(HTML4Renderer.m_marginLeft, (object) leftIndent);
          this.WriteStyle(HTML4Renderer.m_marginRight, (object) rightIndent);
          this.WriteStyle(HTML4Renderer.m_marginTop, (object) spaceBefore);
          if (flag2 && this.m_renderer.IsBrowserIE)
            this.WriteStyle(HTML4Renderer.m_marginBottom, (object) spaceAfter);
          else
            this.WriteStyle(HTML4Renderer.m_paddingBottom, (object) spaceAfter);
        }
        if (this.m_mode == ParagraphStyleWriter.Mode.ListOnly)
        {
          this.WriteStyle(HTML4Renderer.m_fontFamily, (object) "Arial");
          this.WriteStyle(HTML4Renderer.m_fontSize, (object) "10pt");
        }
        else if (hangingIndent != null && hangingIndent.ToMillimeters() < 0.0)
          this.WriteStyle(HTML4Renderer.m_textIndent, (object) hangingIndent);
      }
      if (style == null || this.m_mode != ParagraphStyleWriter.Mode.All && this.m_mode != ParagraphStyleWriter.Mode.ParagraphOnly)
        return;
      object obj = style[(byte) 25];
      if (obj != null || mode != StyleWriterMode.NonShared)
      {
        RPLFormat.TextAlignments val = (RPLFormat.TextAlignments) 0;
        if (obj != null)
          val = (RPLFormat.TextAlignments) obj;
        if (val == null)
        {
          bool flag = HTML4Renderer.GetTextAlignForType(elementProps1);
          if (HTML4Renderer.IsDirectionRTL((IRPLStyle) ((RPLElementProps) elementProps1).Style))
            flag = !flag;
          this.WriteStream(HTML4Renderer.m_textAlign);
          if (flag)
            this.WriteStream(HTML4Renderer.m_rightValue);
          else
            this.WriteStream(HTML4Renderer.m_leftValue);
          this.WriteStream(HTML4Renderer.m_semiColon);
        }
        else
          this.WriteStyle(HTML4Renderer.m_textAlign, (object) EnumStrings.GetValue(val), (object) null);
      }
      this.WriteStyle(HTML4Renderer.m_lineHeight, style[(byte) 28]);
    }

    internal void FixIndents(
      ref RPLReportSize leftIndent,
      ref RPLReportSize rightIndent,
      ref RPLReportSize spaceBefore,
      ref RPLReportSize spaceAfter,
      RPLReportSize hangingIndent)
    {
      RPLTextBoxProps elementProps = ((RPLElement) this.m_textBox).ElementProps as RPLTextBoxProps;
      if (HTML4Renderer.IsDirectionRTL((IRPLStyle) ((RPLElementProps) elementProps).Style))
        rightIndent = this.FixHangingIndent(rightIndent, hangingIndent);
      else
        leftIndent = this.FixHangingIndent(leftIndent, hangingIndent);
      object writingMode = ((RPLElementProps) elementProps).Style[(byte) 30];
      if (!this.m_renderer.IsBrowserIE || writingMode == null || !HTML4Renderer.IsWritingModeVertical((RPLFormat.WritingModes) writingMode))
        return;
      RPLReportSize rplReportSize = leftIndent;
      leftIndent = spaceAfter;
      spaceAfter = rightIndent;
      rightIndent = spaceBefore;
      spaceBefore = rplReportSize;
    }

    internal RPLReportSize FixHangingIndent(RPLReportSize leftIndent, RPLReportSize hangingIndent)
    {
      if (hangingIndent == null)
        return leftIndent;
      double millimeters = hangingIndent.ToMillimeters();
      if (millimeters < 0.0)
      {
        double num = 0.0;
        if (leftIndent != null)
          num = leftIndent.ToMillimeters();
        leftIndent = new RPLReportSize(num - millimeters);
      }
      return leftIndent;
    }

    internal enum Mode
    {
      ListOnly = 1,
      ParagraphOnly = 2,
      All = 3,
    }
  }
}
