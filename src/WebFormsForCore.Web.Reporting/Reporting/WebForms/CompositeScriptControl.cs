
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
