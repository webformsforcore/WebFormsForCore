namespace Microsoft.ReportingServices.Rendering.HtmlRenderer;

internal class ViewerRendererDeviceInfo : DeviceInfo
{
	public override void VerifySafeForJavascript(string value)
	{
	}

	public override bool IsSupported(string value, bool isTrue, out bool isRelative)
	{
		isRelative = false;
		return true;
	}
}
