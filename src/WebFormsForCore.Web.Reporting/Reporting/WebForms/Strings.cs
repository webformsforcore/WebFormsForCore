
using System.Globalization;
using System.Runtime.CompilerServices;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  [CompilerGenerated]
  internal class Strings
  {
    protected Strings()
    {
    }

    public static CultureInfo Culture
    {
      get => Strings.Keys.Culture;
      set => Strings.Keys.Culture = value;
    }

    public static string Of => Strings.Keys.GetString(nameof (Of));

    public static string FindFirst => Strings.Keys.GetString(nameof (FindFirst));

    public static string FindNext => Strings.Keys.GetString(nameof (FindNext));

    public static string PageWidth => Strings.Keys.GetString(nameof (PageWidth));

    public static string WholePage => Strings.Keys.GetString(nameof (WholePage));

    public static string SelectFormat => Strings.Keys.GetString(nameof (SelectFormat));

    public static string ExportButton => Strings.Keys.GetString(nameof (ExportButton));

    public static string ViewReport => Strings.Keys.GetString(nameof (ViewReport));

    public static string ChangeCredentials => Strings.Keys.GetString(nameof (ChangeCredentials));

    public static string UserName => Strings.Keys.GetString(nameof (UserName));

    public static string Password => Strings.Keys.GetString(nameof (Password));

    public static string DocumentMap => Strings.Keys.GetString(nameof (DocumentMap));

    public static string Report => Strings.Keys.GetString(nameof (Report));

    public static string ChangeCredentialsTooltip
    {
      get => Strings.Keys.GetString(nameof (ChangeCredentialsTooltip));
    }

    public static string ShowHideDocMapTooltip
    {
      get => Strings.Keys.GetString(nameof (ShowHideDocMapTooltip));
    }

    public static string FirstPageTooltip => Strings.Keys.GetString(nameof (FirstPageTooltip));

    public static string PreviousPageTooltip
    {
      get => Strings.Keys.GetString(nameof (PreviousPageTooltip));
    }

    public static string CurrentPageTooltip => Strings.Keys.GetString(nameof (CurrentPageTooltip));

    public static string NextPageTooltip => Strings.Keys.GetString(nameof (NextPageTooltip));

    public static string LastPageTooltip => Strings.Keys.GetString(nameof (LastPageTooltip));

    public static string ZoomTooltip => Strings.Keys.GetString(nameof (ZoomTooltip));

    public static string SearchTextBoxToolTip
    {
      get => Strings.Keys.GetString(nameof (SearchTextBoxToolTip));
    }

    public static string FindTooltip => Strings.Keys.GetString(nameof (FindTooltip));

    public static string FindNextTooltip => Strings.Keys.GetString(nameof (FindNextTooltip));

    public static string ExportFormatsTooltip
    {
      get => Strings.Keys.GetString(nameof (ExportFormatsTooltip));
    }

    public static string ExportButtonTooltip
    {
      get => Strings.Keys.GetString(nameof (ExportButtonTooltip));
    }

    public static string RefreshTooltip => Strings.Keys.GetString(nameof (RefreshTooltip));

    public static string PrintTooltip => Strings.Keys.GetString(nameof (PrintTooltip));

    public static string AtomDataFeedTooltip
    {
      get => Strings.Keys.GetString(nameof (AtomDataFeedTooltip));
    }

    public static string BackTooltip => Strings.Keys.GetString(nameof (BackTooltip));

    public static string HelpTooltip => Strings.Keys.GetString(nameof (HelpTooltip));

    public static string ShowHideParametersTooltip
    {
      get => Strings.Keys.GetString(nameof (ShowHideParametersTooltip));
    }

    public static string CantLoadPrintControl
    {
      get => Strings.Keys.GetString(nameof (CantLoadPrintControl));
    }

    public static string NoScript => Strings.Keys.GetString(nameof (NoScript));

    public static string NoScriptPrefix => Strings.Keys.GetString(nameof (NoScriptPrefix));

    public static string Here => Strings.Keys.GetString(nameof (Here));

    public static string Loading => Strings.Keys.GetString(nameof (Loading));

    public static string DocMapCollapseTooltip(string docMapLabel)
    {
      // ISSUE: reference to a compiler-generated method
      return Strings.Keys.GetString(nameof (DocMapCollapseTooltip), (object) docMapLabel);
    }

    public static string DocMapExpandTooltip(string docMapLabel)
    {
      // ISSUE: reference to a compiler-generated method
      return Strings.Keys.GetString(nameof (DocMapExpandTooltip), (object) docMapLabel);
    }

    public static string DocMapActionTooltip(string docMapLabel)
    {
      // ISSUE: reference to a compiler-generated method
      return Strings.Keys.GetString(nameof (DocMapActionTooltip), (object) docMapLabel);
    }

    public static string PlaceHolderFrameAccessibleName(string parameterPrompt)
    {
      // ISSUE: reference to a compiler-generated method
      return Strings.Keys.GetString(nameof (PlaceHolderFrameAccessibleName), (object) parameterPrompt);
    }

    public static string CalendarFrameAccessibleName(string parameterPrompt)
    {
      // ISSUE: reference to a compiler-generated method
      return Strings.Keys.GetString(nameof (CalendarFrameAccessibleName), (object) parameterPrompt);
    }
  }
}
