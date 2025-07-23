namespace Microsoft.Reporting.WebForms;

internal interface IPublicViewState
{
	void LoadViewState(object viewState);

	object SaveViewState();
}
