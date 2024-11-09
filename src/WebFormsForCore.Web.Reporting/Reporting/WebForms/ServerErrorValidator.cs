// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.ServerErrorValidator
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

using System;
using System.Web.UI.WebControls;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal class ServerErrorValidator : CustomValidator
  {
    public ServerErrorValidator() => this.ClientValidationFunction = "MS_ValidateAlwaysTrue";

    protected override void OnPreRender(EventArgs e)
    {
      this.Page.ClientScript.RegisterClientScriptBlock(typeof (ServerErrorValidator), "ValidatorScript", "\r\nfunction MS_ValidateAlwaysTrue(source, args)\r\n{\r\n    args.IsValid = true;\r\n}\r\n", true);
      this.IsValid = false;
      base.OnPreRender(e);
    }

    protected override bool OnServerValidate(string value) => true;
  }
}
