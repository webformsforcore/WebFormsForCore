using System;

namespace Microsoft.Reporting.WebForms;

[Flags]
internal enum UpdateGroup
{
	None = 0,
	Rerendering = 2,
	Reprocessing = 6,
	ExecutionSession = 0xE,
	Appearance = 0xE,
	ToolBarAppearance = 0xE
}
