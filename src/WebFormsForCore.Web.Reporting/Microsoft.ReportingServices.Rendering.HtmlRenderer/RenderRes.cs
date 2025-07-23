using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.ReportingServices.Rendering.HtmlRenderer;

[CompilerGenerated]
internal class RenderRes
{
	[CompilerGenerated]
	public class Keys
	{
		public const string HideDocMapTooltip = "HideDocMapTooltip";

		public const string DocumentMap = "DocumentMap";

		public const string DefaultDocMapLabel = "DefaultDocMapLabel";

		public const string HTML40LocalizedName = "HTML40LocalizedName";

		public const string MHTMLLocalizedName = "MHTMLLocalizedName";

		public const string rrInvalidSectionError = "rrInvalidSectionError";

		public const string rrInvalidDeviceInfo = "rrInvalidDeviceInfo";

		public const string ToggleStateCollapse = "ToggleStateCollapse";

		public const string ToggleStateExpand = "ToggleStateExpand";

		private static ResourceManager resourceManager = new ResourceManager(typeof(RenderRes).FullName, typeof(RenderRes).Module.Assembly);

		private static CultureInfo _culture = null;

		public static CultureInfo Culture
		{
			get
			{
				return _culture;
			}
			set
			{
				_culture = value;
			}
		}

		private Keys()
		{
		}

		public static string GetString(string key)
		{
			return resourceManager.GetString(key, _culture);
		}
	}

	public static CultureInfo Culture
	{
		get
		{
			return Keys.Culture;
		}
		set
		{
			Keys.Culture = value;
		}
	}

	public static string HideDocMapTooltip => Keys.GetString("HideDocMapTooltip");

	public static string DocumentMap => Keys.GetString("DocumentMap");

	public static string DefaultDocMapLabel => Keys.GetString("DefaultDocMapLabel");

	public static string HTML40LocalizedName => Keys.GetString("HTML40LocalizedName");

	public static string MHTMLLocalizedName => Keys.GetString("MHTMLLocalizedName");

	public static string rrInvalidSectionError => Keys.GetString("rrInvalidSectionError");

	public static string rrInvalidDeviceInfo => Keys.GetString("rrInvalidDeviceInfo");

	public static string ToggleStateCollapse => Keys.GetString("ToggleStateCollapse");

	public static string ToggleStateExpand => Keys.GetString("ToggleStateExpand");

	protected RenderRes()
	{
	}
}
