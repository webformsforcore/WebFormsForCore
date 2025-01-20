
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
