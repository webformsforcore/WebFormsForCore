
#nullable disable
namespace Microsoft.ReportingServices.Rendering.HtmlRenderer
{
  internal class StyleContext
  {
    private bool m_inTablix;
    private bool m_styleOnCell;
    private bool m_renderMeasurements = true;
    private bool m_noBorders;
    private bool m_emptyTextBox;
    private bool m_onlyRenderMeasurementsBackgroundBorders;
    private byte m_omitBordersState;
    private bool m_ignoreVerticalAlign;
    private bool m_renderMinMeasurements;
    private bool m_ignorePadding;
    private bool m_rotationApplied;
    private bool m_zeroWidth;

    public void Reset()
    {
      this.m_inTablix = false;
      this.m_styleOnCell = false;
      this.m_renderMeasurements = true;
      this.m_noBorders = false;
      this.m_emptyTextBox = false;
      this.m_omitBordersState = (byte) 0;
      this.m_ignoreVerticalAlign = false;
      this.m_ignorePadding = false;
      this.m_rotationApplied = false;
      this.m_ignoreVerticalAlign = false;
      this.m_zeroWidth = false;
    }

    public bool EmptyTextBox
    {
      get => this.m_emptyTextBox;
      set => this.m_emptyTextBox = value;
    }

    public bool NoBorders
    {
      get => this.m_noBorders;
      set => this.m_noBorders = value;
    }

    public bool InTablix
    {
      get => this.m_inTablix;
      set => this.m_inTablix = value;
    }

    public bool StyleOnCell
    {
      get => this.m_styleOnCell;
      set => this.m_styleOnCell = value;
    }

    public bool RenderMeasurements
    {
      get => this.m_renderMeasurements;
      set => this.m_renderMeasurements = value;
    }

    public bool RenderMinMeasurements
    {
      get => this.m_renderMinMeasurements;
      set => this.m_renderMinMeasurements = value;
    }

    public bool OnlyRenderMeasurementsBackgroundBorders
    {
      get => this.m_onlyRenderMeasurementsBackgroundBorders;
      set => this.m_onlyRenderMeasurementsBackgroundBorders = value;
    }

    public byte OmitBordersState
    {
      get => this.m_omitBordersState;
      set => this.m_omitBordersState = value;
    }

    public bool IgnoreVerticalAlign
    {
      get => this.m_ignoreVerticalAlign;
      set => this.m_ignoreVerticalAlign = value;
    }

    public bool IgnorePadding
    {
      get => this.m_ignorePadding;
      set => this.m_ignorePadding = value;
    }

    public bool RotationApplied
    {
      get => this.m_rotationApplied;
      set => this.m_rotationApplied = value;
    }

    public bool ZeroWidth
    {
      get => this.m_zeroWidth;
      set => this.m_zeroWidth = value;
    }
  }
}
