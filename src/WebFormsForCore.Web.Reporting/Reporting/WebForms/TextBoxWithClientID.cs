// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.TextBoxWithClientID
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

using System;
using System.Web.UI.WebControls;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal sealed class TextBoxWithClientID : TextBox
  {
    protected override void OnLoad(EventArgs e)
    {
      this.EnsureID();
      base.OnLoad(e);
    }
  }
}
