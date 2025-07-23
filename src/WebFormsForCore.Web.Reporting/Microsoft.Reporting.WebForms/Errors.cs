using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.Reporting.WebForms;

[CompilerGenerated]
internal class Errors
{
	[CompilerGenerated]
	public class Keys
	{
		public const string InvalidPageNav = "InvalidPageNav";

		public const string MissingValueClientError = "MissingValueClientError";

		public const string MissingSelectionClientError = "MissingSelectionClientError";

		public const string MissingCredentials = "MissingCredentials";

		public const string MissingUrlParameter = "MissingUrlParameter";

		public const string InvalidScriptIdentifier = "InvalidScriptIdentifier";

		public const string ParamValueTypeMismatch = "ParamValueTypeMismatch";

		public const string SessionDisabled = "SessionDisabled";

		public const string ViewStateDisabled = "ViewStateDisabled";

		public const string SessionOrConfig = "SessionOrConfig";

		public const string ASPNetSessionExpired = "ASPNetSessionExpired";

		public const string FailedToModifyWebConfig = "FailedToModifyWebConfig";

		public const string HandlerNotRegisteredTitle = "HandlerNotRegisteredTitle";

		public const string HandlerNotRegisteredDetails = "HandlerNotRegisteredDetails";

		public const string ScriptManagerNotFound = "ScriptManagerNotFound";

		public const string ReadOnlyViewer = "ReadOnlyViewer";

		public const string NoNamingContainer = "NoNamingContainer";

		public const string DataControl_DataSourceDoesntExist = "DataControl_DataSourceDoesntExist";

		public const string DataControl_ViewNotFound = "DataControl_ViewNotFound";

		public const string DataControl_DataSourceIDMustBeDataControl = "DataControl_DataSourceIDMustBeDataControl";

		public const string SearchNotFound = "SearchNotFound";

		public const string SearchNextNotFound = "SearchNextNotFound";

		public const string PageNumberInvalid = "PageNumberInvalid";

		public const string CantFindConfigFileType = "CantFindConfigFileType";

		public const string MissingInterfaceOnConfigFileType = "MissingInterfaceOnConfigFileType";

		public const string TempStorageNeedsSeekReadWrite = "TempStorageNeedsSeekReadWrite";

		public const string BadReportDataSourceType = "BadReportDataSourceType";

		public const string InvalidDeviceInfoLinkTarget = "InvalidDeviceInfoLinkTarget";

		public const string InvalidDeviceInfoFind = "InvalidDeviceInfoFind";

		public const string InvalidDeviceInfoSection = "InvalidDeviceInfoSection";

		private static ResourceManager resourceManager = new ResourceManager(typeof(Errors).FullName, typeof(Errors).Module.Assembly);

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

		public static string GetString(string key, object arg0, object arg1)
		{
			return string.Format(CultureInfo.CurrentCulture, resourceManager.GetString(key, _culture), arg0, arg1);
		}

		public static string GetString(string key, object arg0, object arg1, object arg2)
		{
			return string.Format(CultureInfo.CurrentCulture, resourceManager.GetString(key, _culture), arg0, arg1, arg2);
		}

		public static string GetString(string key, object arg0, object arg1, object arg2, object arg3)
		{
			return string.Format(CultureInfo.CurrentCulture, resourceManager.GetString(key, _culture), arg0, arg1, arg2, arg3);
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

	public static string InvalidPageNav => Keys.GetString("InvalidPageNav");

	public static string MissingCredentials => Keys.GetString("MissingCredentials");

	public static string InvalidScriptIdentifier => Keys.GetString("InvalidScriptIdentifier");

	public static string SessionDisabled => Keys.GetString("SessionDisabled");

	public static string ViewStateDisabled => Keys.GetString("ViewStateDisabled");

	public static string SessionOrConfig => Keys.GetString("SessionOrConfig");

	public static string ASPNetSessionExpired => Keys.GetString("ASPNetSessionExpired");

	public static string HandlerNotRegisteredTitle => Keys.GetString("HandlerNotRegisteredTitle");

	public static string ScriptManagerNotFound => Keys.GetString("ScriptManagerNotFound");

	public static string ReadOnlyViewer => Keys.GetString("ReadOnlyViewer");

	public static string SearchNotFound => Keys.GetString("SearchNotFound");

	public static string SearchNextNotFound => Keys.GetString("SearchNextNotFound");

	public static string PageNumberInvalid => Keys.GetString("PageNumberInvalid");

	public static string TempStorageNeedsSeekReadWrite => Keys.GetString("TempStorageNeedsSeekReadWrite");

	public static string BadReportDataSourceType => Keys.GetString("BadReportDataSourceType");

	public static string InvalidDeviceInfoLinkTarget => Keys.GetString("InvalidDeviceInfoLinkTarget");

	public static string InvalidDeviceInfoFind => Keys.GetString("InvalidDeviceInfoFind");

	public static string InvalidDeviceInfoSection => Keys.GetString("InvalidDeviceInfoSection");

	protected Errors()
	{
	}

	public static string MissingValueClientError(string prompt)
	{
		return Keys.GetString("MissingValueClientError", prompt);
	}

	public static string MissingSelectionClientError(string prompt)
	{
		return Keys.GetString("MissingSelectionClientError", prompt);
	}

	public static string MissingUrlParameter(string paramName)
	{
		return Keys.GetString("MissingUrlParameter", paramName);
	}

	public static string ParamValueTypeMismatch(string paramName)
	{
		return Keys.GetString("ParamValueTypeMismatch", paramName);
	}

	public static string FailedToModifyWebConfig(string reason)
	{
		return Keys.GetString("FailedToModifyWebConfig", reason);
	}

	public static string HandlerNotRegisteredDetails(string legacyHandlerLine, string legacyHandlerSection, string iis7HandlerLine, string iis7HandlerSection)
	{
		return Keys.GetString("HandlerNotRegisteredDetails", legacyHandlerLine, legacyHandlerSection, iis7HandlerLine, iis7HandlerSection);
	}

	public static string NoNamingContainer(string type, string controlID)
	{
		return Keys.GetString("NoNamingContainer", type, controlID);
	}

	public static string DataControl_DataSourceDoesntExist(string reportDataSourceName, string reportViewerName, string dataSourceID)
	{
		return Keys.GetString("DataControl_DataSourceDoesntExist", reportDataSourceName, reportViewerName, dataSourceID);
	}

	public static string DataControl_ViewNotFound(string reportViewerName, string reportDataSourceName)
	{
		return Keys.GetString("DataControl_ViewNotFound", reportViewerName, reportDataSourceName);
	}

	public static string DataControl_DataSourceIDMustBeDataControl(string reportDataSourceName, string reportViewerName, string dataSourceID)
	{
		return Keys.GetString("DataControl_DataSourceIDMustBeDataControl", reportDataSourceName, reportViewerName, dataSourceID);
	}

	public static string CantFindConfigFileType(string configFileTypeName)
	{
		return Keys.GetString("CantFindConfigFileType", configFileTypeName);
	}

	public static string MissingInterfaceOnConfigFileType(string configFileTypeName, string expectedTypeName)
	{
		return Keys.GetString("MissingInterfaceOnConfigFileType", configFileTypeName, expectedTypeName);
	}
}
