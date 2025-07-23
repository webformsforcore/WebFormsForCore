using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.Reporting.WebForms;

[CompilerGenerated]
internal class Strings
{
	[CompilerGenerated]
	public class Keys
	{
		public const string Of = "Of";

		public const string FindFirst = "FindFirst";

		public const string FindNext = "FindNext";

		public const string PageWidth = "PageWidth";

		public const string WholePage = "WholePage";

		public const string SelectFormat = "SelectFormat";

		public const string ExportButton = "ExportButton";

		public const string ViewReport = "ViewReport";

		public const string ChangeCredentials = "ChangeCredentials";

		public const string UserName = "UserName";

		public const string Password = "Password";

		public const string DocumentMap = "DocumentMap";

		public const string Report = "Report";

		public const string ChangeCredentialsTooltip = "ChangeCredentialsTooltip";

		public const string ShowHideDocMapTooltip = "ShowHideDocMapTooltip";

		public const string FirstPageTooltip = "FirstPageTooltip";

		public const string PreviousPageTooltip = "PreviousPageTooltip";

		public const string CurrentPageTooltip = "CurrentPageTooltip";

		public const string NextPageTooltip = "NextPageTooltip";

		public const string LastPageTooltip = "LastPageTooltip";

		public const string ZoomTooltip = "ZoomTooltip";

		public const string SearchTextBoxToolTip = "SearchTextBoxToolTip";

		public const string FindTooltip = "FindTooltip";

		public const string FindNextTooltip = "FindNextTooltip";

		public const string ExportFormatsTooltip = "ExportFormatsTooltip";

		public const string ExportButtonTooltip = "ExportButtonTooltip";

		public const string RefreshTooltip = "RefreshTooltip";

		public const string PrintTooltip = "PrintTooltip";

		public const string AtomDataFeedTooltip = "AtomDataFeedTooltip";

		public const string BackTooltip = "BackTooltip";

		public const string HelpTooltip = "HelpTooltip";

		public const string ShowHideParametersTooltip = "ShowHideParametersTooltip";

		public const string DocMapCollapseTooltip = "DocMapCollapseTooltip";

		public const string DocMapExpandTooltip = "DocMapExpandTooltip";

		public const string DocMapActionTooltip = "DocMapActionTooltip";

		public const string CantLoadPrintControl = "CantLoadPrintControl";

		public const string NoScript = "NoScript";

		public const string NoScriptPrefix = "NoScriptPrefix";

		public const string Here = "Here";

		public const string Loading = "Loading";

		public const string PlaceHolderFrameAccessibleName = "PlaceHolderFrameAccessibleName";

		public const string CalendarFrameAccessibleName = "CalendarFrameAccessibleName";

		private static ResourceManager resourceManager = new ResourceManager(typeof(Strings).FullName, typeof(Strings).Module.Assembly);

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

		public static string GetString(string key, object arg0)
		{
			return string.Format(CultureInfo.CurrentCulture, resourceManager.GetString(key, _culture), arg0);
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

	public static string Of => Keys.GetString("Of");

	public static string FindFirst => Keys.GetString("FindFirst");

	public static string FindNext => Keys.GetString("FindNext");

	public static string PageWidth => Keys.GetString("PageWidth");

	public static string WholePage => Keys.GetString("WholePage");

	public static string SelectFormat => Keys.GetString("SelectFormat");

	public static string ExportButton => Keys.GetString("ExportButton");

	public static string ViewReport => Keys.GetString("ViewReport");

	public static string ChangeCredentials => Keys.GetString("ChangeCredentials");

	public static string UserName => Keys.GetString("UserName");

	public static string Password => Keys.GetString("Password");

	public static string DocumentMap => Keys.GetString("DocumentMap");

	public static string Report => Keys.GetString("Report");

	public static string ChangeCredentialsTooltip => Keys.GetString("ChangeCredentialsTooltip");

	public static string ShowHideDocMapTooltip => Keys.GetString("ShowHideDocMapTooltip");

	public static string FirstPageTooltip => Keys.GetString("FirstPageTooltip");

	public static string PreviousPageTooltip => Keys.GetString("PreviousPageTooltip");

	public static string CurrentPageTooltip => Keys.GetString("CurrentPageTooltip");

	public static string NextPageTooltip => Keys.GetString("NextPageTooltip");

	public static string LastPageTooltip => Keys.GetString("LastPageTooltip");

	public static string ZoomTooltip => Keys.GetString("ZoomTooltip");

	public static string SearchTextBoxToolTip => Keys.GetString("SearchTextBoxToolTip");

	public static string FindTooltip => Keys.GetString("FindTooltip");

	public static string FindNextTooltip => Keys.GetString("FindNextTooltip");

	public static string ExportFormatsTooltip => Keys.GetString("ExportFormatsTooltip");

	public static string ExportButtonTooltip => Keys.GetString("ExportButtonTooltip");

	public static string RefreshTooltip => Keys.GetString("RefreshTooltip");

	public static string PrintTooltip => Keys.GetString("PrintTooltip");

	public static string AtomDataFeedTooltip => Keys.GetString("AtomDataFeedTooltip");

	public static string BackTooltip => Keys.GetString("BackTooltip");

	public static string HelpTooltip => Keys.GetString("HelpTooltip");

	public static string ShowHideParametersTooltip => Keys.GetString("ShowHideParametersTooltip");

	public static string CantLoadPrintControl => Keys.GetString("CantLoadPrintControl");

	public static string NoScript => Keys.GetString("NoScript");

	public static string NoScriptPrefix => Keys.GetString("NoScriptPrefix");

	public static string Here => Keys.GetString("Here");

	public static string Loading => Keys.GetString("Loading");

	protected Strings()
	{
	}

	public static string DocMapCollapseTooltip(string docMapLabel)
	{
		return Keys.GetString("DocMapCollapseTooltip", docMapLabel);
	}

	public static string DocMapExpandTooltip(string docMapLabel)
	{
		return Keys.GetString("DocMapExpandTooltip", docMapLabel);
	}

	public static string DocMapActionTooltip(string docMapLabel)
	{
		return Keys.GetString("DocMapActionTooltip", docMapLabel);
	}

	public static string PlaceHolderFrameAccessibleName(string parameterPrompt)
	{
		return Keys.GetString("PlaceHolderFrameAccessibleName", parameterPrompt);
	}

	public static string CalendarFrameAccessibleName(string parameterPrompt)
	{
		return Keys.GetString("CalendarFrameAccessibleName", parameterPrompt);
	}
}
