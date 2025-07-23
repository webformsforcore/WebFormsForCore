using Microsoft.ReportingServices.Rendering.RPLProcessing;

namespace Microsoft.ReportingServices.Rendering.HtmlRenderer;

internal class EnumStrings
{
	public class FontStyles
	{
		public const string Normal = "normal";

		public const string Italic = "italic";
	}

	public class FontWeights
	{
		public const string Normal = "400";

		public const string Thin = "100";

		public const string ExtraLight = "200";

		public const string Light = "300";

		public const string Medium = "500";

		public const string SemiBold = "600";

		public const string Bold = "700";

		public const string ExtraBold = "800";

		public const string Heavy = "900";
	}

	public class TextDecorations
	{
		public const string None = "none";

		public const string Underline = "underline";

		public const string Overline = "overline";

		public const string LineThrough = "line-through";
	}

	public class TextAlignments
	{
		public const string General = "General";

		public const string Left = "left";

		public const string Center = "center";

		public const string Right = "right";
	}

	public class VerticalAlignments
	{
		public const string Top = "top";

		public const string Middle = "middle";

		public const string Bottom = "bottom";
	}

	public class Directions
	{
		public const string LTR = "ltr";

		public const string RTL = "rtl";
	}

	public class WritingModes
	{
		public const string Horizontal = "lr-tb";

		public const string Vertical = "tb-rl";
	}

	public class UnicodeBiDiTypes
	{
		public const string Normal = "normal";

		public const string Embed = "embed";

		public const string BiDiOverride = "bidi-override";
	}

	public class BorderStyles
	{
		public const string None = "none";

		public const string Dotted = "dotted";

		public const string Dashed = "dashed";

		public const string Solid = "solid";

		public const string Double = "double";
	}

	public class BackgroundRepeatTypes
	{
		public const string Repeat = "repeat";

		public const string NoRepeat = "no-repeat";

		public const string RepeatX = "repeat-x";

		public const string RepeatY = "repeat-y";
	}

	public static string GetValue(FontStyles val)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Expected I4, but got Unknown
		return (int)val switch
		{
			0 => "normal", 
			1 => "italic", 
			_ => null, 
		};
	}

	public static string GetValue(FontWeights val)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Expected I4, but got Unknown
		return (int)val switch
		{
			0 => "400", 
			1 => "100", 
			2 => "200", 
			3 => "300", 
			4 => "500", 
			5 => "600", 
			6 => "700", 
			7 => "800", 
			8 => "900", 
			_ => null, 
		};
	}

	public static string GetValue(TextDecorations val)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Expected I4, but got Unknown
		return (int)val switch
		{
			0 => "none", 
			1 => "underline", 
			2 => "overline", 
			3 => "line-through", 
			_ => null, 
		};
	}

	public static string GetValue(TextAlignments val)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected I4, but got Unknown
		return (val - 1) switch
		{
			0 => "left", 
			1 => "center", 
			2 => "right", 
			_ => null, 
		};
	}

	public static string GetValue(VerticalAlignments val)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Expected I4, but got Unknown
		return (int)val switch
		{
			0 => "top", 
			1 => "middle", 
			2 => "bottom", 
			_ => null, 
		};
	}

	public static string GetValue(Directions val)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Expected I4, but got Unknown
		return (int)val switch
		{
			0 => "ltr", 
			1 => "rtl", 
			_ => null, 
		};
	}

	public static string GetValue(UnicodeBiDiTypes val)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Expected I4, but got Unknown
		return (int)val switch
		{
			0 => "normal", 
			1 => "embed", 
			2 => "bidi-override", 
			_ => null, 
		};
	}

	public static string GetValue(BorderStyles val)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected I4, but got Unknown
		return (int)val switch
		{
			0 => "none", 
			1 => "dotted", 
			2 => "dashed", 
			3 => "solid", 
			4 => "double", 
			_ => null, 
		};
	}

	public static string GetValue(BackgroundRepeatTypes val)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Expected I4, but got Unknown
		return (int)val switch
		{
			0 => "repeat", 
			1 => "no-repeat", 
			2 => "repeat-x", 
			3 => "repeat-y", 
			_ => null, 
		};
	}
}
