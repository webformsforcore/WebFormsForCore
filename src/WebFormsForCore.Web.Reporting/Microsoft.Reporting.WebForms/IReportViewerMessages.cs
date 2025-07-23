namespace Microsoft.Reporting.WebForms;

public interface IReportViewerMessages
{
	string DocumentMapButtonToolTip { get; }

	string ParameterAreaButtonToolTip { get; }

	string FirstPageButtonToolTip { get; }

	string PreviousPageButtonToolTip { get; }

	string CurrentPageTextBoxToolTip { get; }

	string PageOf { get; }

	string NextPageButtonToolTip { get; }

	string LastPageButtonToolTip { get; }

	string BackButtonToolTip { get; }

	string RefreshButtonToolTip { get; }

	string PrintButtonToolTip { get; }

	string ExportButtonToolTip { get; }

	string ZoomControlToolTip { get; }

	string SearchTextBoxToolTip { get; }

	string FindButtonToolTip { get; }

	string FindNextButtonToolTip { get; }

	string ZoomToPageWidth { get; }

	string ZoomToWholePage { get; }

	string FindButtonText { get; }

	string FindNextButtonText { get; }

	string ViewReportButtonText { get; }

	string ProgressText { get; }

	string TextNotFound { get; }

	string NoMoreMatches { get; }

	string ChangeCredentialsText { get; }

	string NullCheckBoxText { get; }

	string NullValueText { get; }

	string TrueValueText { get; }

	string FalseValueText { get; }

	string SelectAValue { get; }

	string UserNamePrompt { get; }

	string PasswordPrompt { get; }

	string SelectAll { get; }

	string TodayIs { get; }

	string ExportFormatsToolTip { get; }

	string ExportButtonText { get; }

	string SelectFormat { get; }

	string DocumentMap { get; }

	string InvalidPageNumber { get; }

	string ChangeCredentialsToolTip { get; }
}
