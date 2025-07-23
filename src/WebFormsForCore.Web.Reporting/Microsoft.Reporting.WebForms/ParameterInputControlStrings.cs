using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.Reporting.WebForms;

[CompilerGenerated]
internal class ParameterInputControlStrings
{
	[CompilerGenerated]
	public class Keys
	{
		public const string True = "True";

		public const string False = "False";

		public const string NullCheckBox = "NullCheckBox";

		public const string NullValue = "NullValue";

		public const string SelectValidValue = "SelectValidValue";

		public const string TodayIs = "TodayIs";

		public const string NextMonthToolTip = "NextMonthToolTip";

		public const string PreviousMonthToolTip = "PreviousMonthToolTip";

		public const string SelectAll = "SelectAll";

		public const string DropDownTooltip = "DropDownTooltip";

		private static ResourceManager resourceManager = new ResourceManager(typeof(ParameterInputControlStrings).FullName, typeof(ParameterInputControlStrings).Module.Assembly);

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

	public static string True => Keys.GetString("True");

	public static string False => Keys.GetString("False");

	public static string NullCheckBox => Keys.GetString("NullCheckBox");

	public static string NullValue => Keys.GetString("NullValue");

	public static string SelectValidValue => Keys.GetString("SelectValidValue");

	public static string TodayIs => Keys.GetString("TodayIs");

	public static string NextMonthToolTip => Keys.GetString("NextMonthToolTip");

	public static string PreviousMonthToolTip => Keys.GetString("PreviousMonthToolTip");

	public static string SelectAll => Keys.GetString("SelectAll");

	public static string DropDownTooltip => Keys.GetString("DropDownTooltip");

	protected ParameterInputControlStrings()
	{
	}
}
