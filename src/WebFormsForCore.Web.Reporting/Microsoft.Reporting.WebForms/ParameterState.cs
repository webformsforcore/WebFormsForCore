namespace Microsoft.Reporting.WebForms;

public enum ParameterState
{
	HasValidValue,
	MissingValidValue,
	HasOutstandingDependencies,
	DynamicValuesUnavailable
}
