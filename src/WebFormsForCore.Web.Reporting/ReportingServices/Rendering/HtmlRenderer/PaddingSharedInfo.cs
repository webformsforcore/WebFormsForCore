// Decompiled with JetBrains decompiler
// Type: Microsoft.ReportingServices.Rendering.HtmlRenderer.PaddingSharedInfo
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

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
