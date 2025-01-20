
using System.Globalization;
using System.Runtime.CompilerServices;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  [CompilerGenerated]
  internal class Errors
  {
    protected Errors()
    {
    }

    public static CultureInfo Culture
    {
      get => Errors.Keys.Culture;
      set => Errors.Keys.Culture = value;
    }

    public static string InvalidPageNav => Errors.Keys.GetString(nameof (InvalidPageNav));

    public static string MissingCredentials => Errors.Keys.GetString(nameof (MissingCredentials));

    public static string InvalidScriptIdentifier
    {
      get => Errors.Keys.GetString(nameof (InvalidScriptIdentifier));
    }

    public static string SessionDisabled => Errors.Keys.GetString(nameof (SessionDisabled));

    public static string ViewStateDisabled => Errors.Keys.GetString(nameof (ViewStateDisabled));

    public static string SessionOrConfig => Errors.Keys.GetString(nameof (SessionOrConfig));

    public static string ASPNetSessionExpired
    {
      get => Errors.Keys.GetString(nameof (ASPNetSessionExpired));
    }

    public static string HandlerNotRegisteredTitle
    {
      get => Errors.Keys.GetString(nameof (HandlerNotRegisteredTitle));
    }

    public static string ScriptManagerNotFound
    {
      get => Errors.Keys.GetString(nameof (ScriptManagerNotFound));
    }

    public static string ReadOnlyViewer => Errors.Keys.GetString(nameof (ReadOnlyViewer));

    public static string SearchNotFound => Errors.Keys.GetString(nameof (SearchNotFound));

    public static string SearchNextNotFound => Errors.Keys.GetString(nameof (SearchNextNotFound));

    public static string PageNumberInvalid => Errors.Keys.GetString(nameof (PageNumberInvalid));

    public static string TempStorageNeedsSeekReadWrite
    {
      get => Errors.Keys.GetString(nameof (TempStorageNeedsSeekReadWrite));
    }

    public static string BadReportDataSourceType
    {
      get => Errors.Keys.GetString(nameof (BadReportDataSourceType));
    }

    public static string InvalidDeviceInfoLinkTarget
    {
      get => Errors.Keys.GetString(nameof (InvalidDeviceInfoLinkTarget));
    }

    public static string InvalidDeviceInfoFind
    {
      get => Errors.Keys.GetString(nameof (InvalidDeviceInfoFind));
    }

    public static string InvalidDeviceInfoSection
    {
      get => Errors.Keys.GetString(nameof (InvalidDeviceInfoSection));
    }

    public static string MissingValueClientError(string prompt)
    {
      // ISSUE: reference to a compiler-generated method
      return Errors.Keys.GetString(nameof (MissingValueClientError), (object) prompt);
    }

    public static string MissingSelectionClientError(string prompt)
    {
      // ISSUE: reference to a compiler-generated method
      return Errors.Keys.GetString(nameof (MissingSelectionClientError), (object) prompt);
    }

    public static string MissingUrlParameter(string paramName)
    {
      // ISSUE: reference to a compiler-generated method
      return Errors.Keys.GetString(nameof (MissingUrlParameter), (object) paramName);
    }

    public static string ParamValueTypeMismatch(string paramName)
    {
      // ISSUE: reference to a compiler-generated method
      return Errors.Keys.GetString(nameof (ParamValueTypeMismatch), (object) paramName);
    }

    public static string FailedToModifyWebConfig(string reason)
    {
      // ISSUE: reference to a compiler-generated method
      return Errors.Keys.GetString(nameof (FailedToModifyWebConfig), (object) reason);
    }

    public static string HandlerNotRegisteredDetails(
      string legacyHandlerLine,
      string legacyHandlerSection,
      string iis7HandlerLine,
      string iis7HandlerSection)
    {
      // ISSUE: reference to a compiler-generated method
      return Errors.Keys.GetString(nameof (HandlerNotRegisteredDetails), (object) legacyHandlerLine, (object) legacyHandlerSection, (object) iis7HandlerLine, (object) iis7HandlerSection);
    }

    public static string NoNamingContainer(string type, string controlID)
    {
      // ISSUE: reference to a compiler-generated method
      return Errors.Keys.GetString(nameof (NoNamingContainer), (object) type, (object) controlID);
    }

    public static string DataControl_DataSourceDoesntExist(
      string reportDataSourceName,
      string reportViewerName,
      string dataSourceID)
    {
      // ISSUE: reference to a compiler-generated method
      return Errors.Keys.GetString(nameof (DataControl_DataSourceDoesntExist), (object) reportDataSourceName, (object) reportViewerName, (object) dataSourceID);
    }

    public static string DataControl_ViewNotFound(
      string reportViewerName,
      string reportDataSourceName)
    {
      // ISSUE: reference to a compiler-generated method
      return Errors.Keys.GetString(nameof (DataControl_ViewNotFound), (object) reportViewerName, (object) reportDataSourceName);
    }

    public static string DataControl_DataSourceIDMustBeDataControl(
      string reportDataSourceName,
      string reportViewerName,
      string dataSourceID)
    {
      // ISSUE: reference to a compiler-generated method
      return Errors.Keys.GetString(nameof (DataControl_DataSourceIDMustBeDataControl), (object) reportDataSourceName, (object) reportViewerName, (object) dataSourceID);
    }

    public static string CantFindConfigFileType(string configFileTypeName)
    {
      // ISSUE: reference to a compiler-generated method
      return Errors.Keys.GetString(nameof (CantFindConfigFileType), (object) configFileTypeName);
    }

    public static string MissingInterfaceOnConfigFileType(
      string configFileTypeName,
      string expectedTypeName)
    {
      // ISSUE: reference to a compiler-generated method
      return Errors.Keys.GetString(nameof (MissingInterfaceOnConfigFileType), (object) configFileTypeName, (object) expectedTypeName);
    }
  }
}
