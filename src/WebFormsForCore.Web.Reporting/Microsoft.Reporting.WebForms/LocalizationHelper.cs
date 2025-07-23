using System;
using System.Globalization;

namespace Microsoft.Reporting.WebForms;

internal sealed class LocalizationHelper : IReportViewerMessages3, IReportViewerMessages2, IReportViewerMessages
{
	private static LocalizationHelper m_theInstance;

	private static object m_lockObject = new object();

	public static IReportViewerMessages3 Current
	{
		get
		{
			lock (m_lockObject)
			{
				if (m_theInstance == null)
				{
					m_theInstance = new LocalizationHelper();
				}
				return m_theInstance;
			}
		}
	}

	private IReportViewerMessages2 ReportViewerMessages2 => WebConfigReader.Current.ViewerMessages as IReportViewerMessages2;

	private IReportViewerMessages3 ReportViewerMessages3 => WebConfigReader.Current.ViewerMessages as IReportViewerMessages3;

	string IReportViewerMessages.DocumentMapButtonToolTip => GetLocalizedString(Strings.ShowHideDocMapTooltip, (WebConfigReader.Current.ViewerMessages != null) ? WebConfigReader.Current.ViewerMessages.DocumentMapButtonToolTip : null);

	string IReportViewerMessages.ParameterAreaButtonToolTip => GetLocalizedString(Strings.ShowHideParametersTooltip, (WebConfigReader.Current.ViewerMessages != null) ? WebConfigReader.Current.ViewerMessages.ParameterAreaButtonToolTip : null);

	string IReportViewerMessages.FirstPageButtonToolTip => GetLocalizedString(Strings.FirstPageTooltip, (WebConfigReader.Current.ViewerMessages != null) ? WebConfigReader.Current.ViewerMessages.FirstPageButtonToolTip : null);

	string IReportViewerMessages.PreviousPageButtonToolTip => GetLocalizedString(Strings.PreviousPageTooltip, (WebConfigReader.Current.ViewerMessages != null) ? WebConfigReader.Current.ViewerMessages.PreviousPageButtonToolTip : null);

	string IReportViewerMessages.CurrentPageTextBoxToolTip => GetLocalizedString(Strings.CurrentPageTooltip, (WebConfigReader.Current.ViewerMessages != null) ? WebConfigReader.Current.ViewerMessages.CurrentPageTextBoxToolTip : null);

	string IReportViewerMessages.PageOf => GetLocalizedString(Strings.Of, (WebConfigReader.Current.ViewerMessages != null) ? WebConfigReader.Current.ViewerMessages.PageOf : null);

	string IReportViewerMessages.NextPageButtonToolTip => GetLocalizedString(Strings.NextPageTooltip, (WebConfigReader.Current.ViewerMessages != null) ? WebConfigReader.Current.ViewerMessages.NextPageButtonToolTip : null);

	string IReportViewerMessages.LastPageButtonToolTip => GetLocalizedString(Strings.LastPageTooltip, (WebConfigReader.Current.ViewerMessages != null) ? WebConfigReader.Current.ViewerMessages.LastPageButtonToolTip : null);

	string IReportViewerMessages.BackButtonToolTip => GetLocalizedString(Strings.BackTooltip, (WebConfigReader.Current.ViewerMessages != null) ? WebConfigReader.Current.ViewerMessages.BackButtonToolTip : null);

	string IReportViewerMessages.RefreshButtonToolTip => GetLocalizedString(Strings.RefreshTooltip, (WebConfigReader.Current.ViewerMessages != null) ? WebConfigReader.Current.ViewerMessages.RefreshButtonToolTip : null);

	string IReportViewerMessages.PrintButtonToolTip => GetLocalizedString(Strings.PrintTooltip, (WebConfigReader.Current.ViewerMessages != null) ? WebConfigReader.Current.ViewerMessages.PrintButtonToolTip : null);

	string IReportViewerMessages.ExportButtonToolTip => GetLocalizedString(Strings.ExportButtonTooltip, (WebConfigReader.Current.ViewerMessages != null) ? WebConfigReader.Current.ViewerMessages.ExportButtonToolTip : null);

	string IReportViewerMessages.ZoomControlToolTip => GetLocalizedString(Strings.ZoomTooltip, (WebConfigReader.Current.ViewerMessages != null) ? WebConfigReader.Current.ViewerMessages.ZoomControlToolTip : null);

	string IReportViewerMessages.SearchTextBoxToolTip => GetLocalizedString(Strings.SearchTextBoxToolTip, (WebConfigReader.Current.ViewerMessages != null) ? WebConfigReader.Current.ViewerMessages.SearchTextBoxToolTip : null);

	string IReportViewerMessages.FindButtonToolTip => GetLocalizedString(Strings.FindTooltip, (WebConfigReader.Current.ViewerMessages != null) ? WebConfigReader.Current.ViewerMessages.FindButtonToolTip : null);

	string IReportViewerMessages.FindNextButtonToolTip => GetLocalizedString(Strings.FindNextTooltip, (WebConfigReader.Current.ViewerMessages != null) ? WebConfigReader.Current.ViewerMessages.FindNextButtonToolTip : null);

	string IReportViewerMessages.ZoomToPageWidth => GetLocalizedString(Strings.PageWidth, (WebConfigReader.Current.ViewerMessages != null) ? WebConfigReader.Current.ViewerMessages.ZoomToPageWidth : null);

	string IReportViewerMessages.ZoomToWholePage => GetLocalizedString(Strings.WholePage, (WebConfigReader.Current.ViewerMessages != null) ? WebConfigReader.Current.ViewerMessages.ZoomToWholePage : null);

	string IReportViewerMessages.FindButtonText => GetLocalizedString(Strings.FindFirst, (WebConfigReader.Current.ViewerMessages != null) ? WebConfigReader.Current.ViewerMessages.FindButtonText : null);

	string IReportViewerMessages.FindNextButtonText => GetLocalizedString(Strings.FindNext, (WebConfigReader.Current.ViewerMessages != null) ? WebConfigReader.Current.ViewerMessages.FindNextButtonText : null);

	string IReportViewerMessages.ViewReportButtonText => GetLocalizedString(Strings.ViewReport, (WebConfigReader.Current.ViewerMessages != null) ? WebConfigReader.Current.ViewerMessages.ViewReportButtonText : null);

	string IReportViewerMessages.ProgressText => GetLocalizedString(CommonStrings.AsyncProgressText, (WebConfigReader.Current.ViewerMessages != null) ? WebConfigReader.Current.ViewerMessages.ProgressText : null);

	string IReportViewerMessages.TextNotFound => GetLocalizedString(Errors.SearchNotFound, (WebConfigReader.Current.ViewerMessages != null) ? WebConfigReader.Current.ViewerMessages.TextNotFound : null);

	string IReportViewerMessages.NoMoreMatches => GetLocalizedString(Errors.SearchNextNotFound, (WebConfigReader.Current.ViewerMessages != null) ? WebConfigReader.Current.ViewerMessages.NoMoreMatches : null);

	string IReportViewerMessages.ChangeCredentialsText => GetLocalizedString(Strings.ChangeCredentials, (WebConfigReader.Current.ViewerMessages != null) ? WebConfigReader.Current.ViewerMessages.ChangeCredentialsText : null);

	string IReportViewerMessages.NullCheckBoxText => GetLocalizedString(ParameterInputControlStrings.NullCheckBox, (WebConfigReader.Current.ViewerMessages != null) ? WebConfigReader.Current.ViewerMessages.NullCheckBoxText : null);

	string IReportViewerMessages.NullValueText => GetLocalizedString(ParameterInputControlStrings.NullValue, (WebConfigReader.Current.ViewerMessages != null) ? WebConfigReader.Current.ViewerMessages.NullValueText : null);

	string IReportViewerMessages.TrueValueText => GetLocalizedString(ParameterInputControlStrings.True, (WebConfigReader.Current.ViewerMessages != null) ? WebConfigReader.Current.ViewerMessages.TrueValueText : null);

	string IReportViewerMessages.FalseValueText => GetLocalizedString(ParameterInputControlStrings.False, (WebConfigReader.Current.ViewerMessages != null) ? WebConfigReader.Current.ViewerMessages.FalseValueText : null);

	string IReportViewerMessages.SelectAValue => GetLocalizedString(ParameterInputControlStrings.SelectValidValue, (WebConfigReader.Current.ViewerMessages != null) ? WebConfigReader.Current.ViewerMessages.SelectAValue : null);

	string IReportViewerMessages.UserNamePrompt => GetLocalizedString(Strings.UserName, (WebConfigReader.Current.ViewerMessages != null) ? WebConfigReader.Current.ViewerMessages.UserNamePrompt : null);

	string IReportViewerMessages.PasswordPrompt => GetLocalizedString(Strings.Password, (WebConfigReader.Current.ViewerMessages != null) ? WebConfigReader.Current.ViewerMessages.PasswordPrompt : null);

	string IReportViewerMessages.SelectAll => GetLocalizedString(ParameterInputControlStrings.SelectAll, (WebConfigReader.Current.ViewerMessages != null) ? WebConfigReader.Current.ViewerMessages.SelectAll : null);

	string IReportViewerMessages.TodayIs => GetLocalizedString(ParameterInputControlStrings.TodayIs, (WebConfigReader.Current.ViewerMessages != null) ? WebConfigReader.Current.ViewerMessages.TodayIs : null);

	string IReportViewerMessages.ExportFormatsToolTip => GetLocalizedString(Strings.ExportFormatsTooltip, (WebConfigReader.Current.ViewerMessages != null) ? WebConfigReader.Current.ViewerMessages.ExportFormatsToolTip : null);

	string IReportViewerMessages.ExportButtonText => GetLocalizedString(Strings.ExportButton, (WebConfigReader.Current.ViewerMessages != null) ? WebConfigReader.Current.ViewerMessages.ExportButtonText : null);

	string IReportViewerMessages.SelectFormat => GetLocalizedString(Strings.SelectFormat, (WebConfigReader.Current.ViewerMessages != null) ? WebConfigReader.Current.ViewerMessages.SelectFormat : null);

	string IReportViewerMessages.DocumentMap => GetLocalizedString(Strings.DocumentMap, (WebConfigReader.Current.ViewerMessages != null) ? WebConfigReader.Current.ViewerMessages.DocumentMap : null);

	string IReportViewerMessages.InvalidPageNumber => GetLocalizedString(Errors.PageNumberInvalid, (WebConfigReader.Current.ViewerMessages != null) ? WebConfigReader.Current.ViewerMessages.InvalidPageNumber : null);

	string IReportViewerMessages.ChangeCredentialsToolTip => GetLocalizedString(Strings.ChangeCredentialsTooltip, (WebConfigReader.Current.ViewerMessages != null) ? WebConfigReader.Current.ViewerMessages.ChangeCredentialsToolTip : null);

	public string ClientNoScript => GetLocalizedString(Strings.NoScript, (ReportViewerMessages2 != null) ? ReportViewerMessages2.ClientNoScript : null);

	public string ClientPrintControlLoadFailed => GetLocalizedString(Strings.CantLoadPrintControl, (ReportViewerMessages2 != null) ? ReportViewerMessages2.ClientPrintControlLoadFailed : null);

	public string ParameterDropDownToolTip => GetLocalizedString(ParameterInputControlStrings.DropDownTooltip, (ReportViewerMessages2 != null) ? ReportViewerMessages2.ParameterDropDownToolTip : null);

	public string CancelLinkText => GetLocalizedString(CommonStrings.CancelLinkText, (ReportViewerMessages3 != null) ? ReportViewerMessages3.CancelLinkText : null);

	public string CalendarLoading => GetLocalizedString(Strings.Loading, (ReportViewerMessages3 != null) ? ReportViewerMessages3.CalendarLoading : null);

	private LocalizationHelper()
	{
	}

	private static string GetLocalizedString(string builtinString, string customString)
	{
		if (customString == null)
		{
			return builtinString;
		}
		return customString;
	}

	string IReportViewerMessages2.GetLocalizedNameForRenderingExtension(string format)
	{
		throw new NotImplementedException();
	}

	public string GetLocalizedNameForRenderingExtension(RenderingExtension ext)
	{
		return GetLocalizedString(ext.LocalizedName, (ReportViewerMessages2 != null) ? ReportViewerMessages2.GetLocalizedNameForRenderingExtension(ext.Name) : null);
	}

	public string ParameterMissingSelectionError(string parameterPrompt)
	{
		string builtinString = Errors.MissingSelectionClientError(parameterPrompt);
		return GetLocalizedString(builtinString, (ReportViewerMessages2 != null) ? ReportViewerMessages2.ParameterMissingSelectionError(parameterPrompt) : null);
	}

	public string ParameterMissingValueError(string parameterPrompt)
	{
		string builtinString = Errors.MissingValueClientError(parameterPrompt);
		return GetLocalizedString(builtinString, (ReportViewerMessages2 != null) ? ReportViewerMessages2.ParameterMissingValueError(parameterPrompt) : null);
	}

	public string CredentialMissingUserNameError(string dataSourcePrompt)
	{
		string missingCredentials = Errors.MissingCredentials;
		return GetLocalizedString(missingCredentials, (ReportViewerMessages2 != null) ? ReportViewerMessages2.CredentialMissingUserNameError(dataSourcePrompt) : null);
	}

	string IReportViewerMessages3.TotalPages(int pageCount, PageCountMode pageCountMode)
	{
		string builtinString = ((pageCountMode != PageCountMode.Actual && pageCount > 0) ? CommonStrings.EstimateTotalPages(pageCount) : pageCount.ToString(CultureInfo.CurrentCulture));
		return GetLocalizedString(builtinString, (ReportViewerMessages3 != null) ? ReportViewerMessages3.TotalPages(pageCount, pageCountMode) : null);
	}
}
