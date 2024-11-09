// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.ToolbarImageInfo
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal sealed class ToolbarImageInfo
  {
    private string m_ltrImageName;
    private string m_rtlImageName;

    public ToolbarImageInfo(string ltrImage) => this.m_ltrImageName = ltrImage;

    public ToolbarImageInfo(string ltrImageName, string rtlImageName)
    {
      this.m_ltrImageName = ltrImageName;
      this.m_rtlImageName = rtlImageName;
    }

    public bool IsBiDirectional => this.m_rtlImageName != null;

    public string LTRImageName => this.m_ltrImageName;

    public string RTLImageName => this.m_rtlImageName;
  }
}
