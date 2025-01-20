
using System;
using System.Web.UI.HtmlControls;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal sealed class HtmlInputImageWithClientID : HtmlInputImage
  {
    protected override void OnPreRender(EventArgs e)
    {
      this.EnsureID();
      base.OnPreRender(e);
    }
  }
}
