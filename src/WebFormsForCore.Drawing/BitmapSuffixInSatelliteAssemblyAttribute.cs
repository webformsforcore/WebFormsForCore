namespace System.Drawing
{
	/// <summary>Specifies that, when interpreting <see cref="T:System.Drawing.ToolboxBitmapAttribute" /> declarations, the assembly should look for the indicated resources in a satellite assembly, but with the <see cref="P:System.Drawing.Configuration.SystemDrawingSection.BitmapSuffix" /> configuration value appended to the declared file name.</summary>
	[AttributeUsage(AttributeTargets.Assembly)]
	public class BitmapSuffixInSatelliteAssemblyAttribute : Attribute
	{
	}
}
