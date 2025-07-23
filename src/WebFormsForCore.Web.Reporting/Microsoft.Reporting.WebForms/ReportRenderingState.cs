namespace Microsoft.Reporting.WebForms;

internal enum ReportRenderingState
{
	NotReady,
	Preparing,
	Pending,
	AsyncWait,
	Ready,
	Completed
}
