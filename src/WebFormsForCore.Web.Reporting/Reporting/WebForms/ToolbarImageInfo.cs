
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
