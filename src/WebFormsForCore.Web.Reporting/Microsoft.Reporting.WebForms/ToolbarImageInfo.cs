namespace Microsoft.Reporting.WebForms;

internal sealed class ToolbarImageInfo
{
	private string m_ltrImageName;

	private string m_rtlImageName;

	public bool IsBiDirectional => m_rtlImageName != null;

	public string LTRImageName => m_ltrImageName;

	public string RTLImageName => m_rtlImageName;

	public ToolbarImageInfo(string ltrImage)
	{
		m_ltrImageName = ltrImage;
	}

	public ToolbarImageInfo(string ltrImageName, string rtlImageName)
	{
		m_ltrImageName = ltrImageName;
		m_rtlImageName = rtlImageName;
	}
}
