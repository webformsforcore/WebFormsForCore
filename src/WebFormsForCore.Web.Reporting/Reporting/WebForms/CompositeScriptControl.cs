// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.CompositeScriptControl
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal abstract class CompositeScriptControl : CompositeControl, IScriptControl
  {
    protected override HtmlTextWriterTag TagKey => HtmlTextWriterTag.Div;

    protected ScriptManager ScriptManager => ScriptManager.GetCurrent(this.Page);

    protected override void OnPreRender(EventArgs e)
    {
      base.OnPreRender(e);
      this.ScriptManager.RegisterScriptControl<CompositeScriptControl>(this);
    }

    protected override void Render(HtmlTextWriter writer)
    {
      base.Render(writer);
      if (this.DesignMode)
        return;
      this.ScriptManager.RegisterScriptDescriptors((IScriptControl) this);
    }

    public abstract IEnumerable<ScriptDescriptor> GetScriptDescriptors();

    public virtual IEnumerable<ScriptReference> GetScriptReferences()
    {
      return (IEnumerable<ScriptReference>) new ScriptReference[1]
      {
        new ScriptReference(EmbeddedResourceOperation.CreateUrlForScriptFile())
      };
    }
  }
}
