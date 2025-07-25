namespace Microsoft.Reporting.WebForms;

public interface IReportViewerMessages2 : IReportViewerMessages
{
	string ClientNoScript { get; }

	string ClientPrintControlLoadFailed { get; }

	string ParameterDropDownToolTip { get; }

	string GetLocalizedNameForRenderingExtension(string format);

	string ParameterMissingSelectionError(string parameterPrompt);

	string ParameterMissingValueError(string parameterPrompt);

	string CredentialMissingUserNameError(string dataSourcePrompt);
}
