// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.MultiValueSelector
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

using System;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal abstract class MultiValueSelector : CompositeControl
  {
    public string ClientSideObjectName;

    public event EventHandler Change;

    public abstract string[] Value { get; set; }

    public abstract bool HasValue { get; }

    protected void OnChange(object sender, EventArgs e)
    {
      if (this.Change == null)
        return;
      this.Change((object) this, e);
    }

    public virtual void AddScriptDescriptors(ScriptControlDescriptor desc)
    {
    }

    protected void SetSelectorBorder()
    {
      if (!string.IsNullOrEmpty(this.CssClass))
        return;
      this.BorderStyle = BorderStyle.Solid;
      this.BorderColor = Color.DarkGray;
      this.BorderWidth = Unit.Pixel(1);
    }
  }
}
