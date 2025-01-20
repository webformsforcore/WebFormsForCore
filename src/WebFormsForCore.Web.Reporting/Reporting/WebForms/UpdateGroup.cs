
using System;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  [Flags]
  internal enum UpdateGroup
  {
    None = 0,
    Rerendering = 2,
    Reprocessing = 6,
    ExecutionSession = 14, // 0x0000000E
    Appearance = ExecutionSession, // 0x0000000E
    ToolBarAppearance = Appearance, // 0x0000000E
  }
}
