
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
