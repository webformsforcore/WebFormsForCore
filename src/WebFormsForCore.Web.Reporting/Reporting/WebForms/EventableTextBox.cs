// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.EventableTextBox
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

using System;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal sealed class EventableTextBox : TextBox, IPostBackEventHandler
  {
    private const string CurrentPagePostBackScript = "if (event.keyCode == 10 || event.keyCode == 13) {{{0}; return false;}}";
    public string CustomOnEnterScript;
    public bool AddKeyPressHandler = true;

    public event EventHandler EnterPressed;

    protected override void OnPreRender(EventArgs e)
    {
      base.OnPreRender(e);
      if (this.DesignMode)
        return;
      this.EnsureID();
      if (!this.AddKeyPressHandler)
        return;
      this.Attributes.Add("onkeypress", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "if (event.keyCode == 10 || event.keyCode == 13) {{{0}; return false;}}", (object) (this.CustomOnEnterScript ?? this.Page.ClientScript.GetPostBackEventReference((Control) this, (string) null))));
    }

    public void RaisePostBackEvent(string eventArgument)
    {
      if (this.EnterPressed == null)
        return;
      this.EnterPressed((object) this, (EventArgs) null);
    }
  }
}
