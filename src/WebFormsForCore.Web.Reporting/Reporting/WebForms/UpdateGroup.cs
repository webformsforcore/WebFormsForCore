// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.UpdateGroup
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

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
