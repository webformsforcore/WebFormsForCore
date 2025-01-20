
#nullable disable
namespace Microsoft.ReportingServices.Rendering.HtmlRenderer
{
  internal class PaddingSharedInfo
  {
    private double m_padH;
    private double m_padV;
    private int m_paddingContext;

    internal PaddingSharedInfo(int paddingContext, double padH, double padV)
    {
      this.m_padH = padH;
      this.m_padV = padV;
      this.m_paddingContext = paddingContext;
    }

    internal double PadH => this.m_padH;

    internal double PadV => this.m_padV;

    internal int PaddingContext => this.m_paddingContext;
  }
}
