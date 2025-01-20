
using System;
using System.Web.UI.WebControls;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal sealed class ViewReportButton : Button
  {
    protected override void OnPreRender(EventArgs e)
    {
      this.EnsureID();
      base.OnPreRender(e);
    }
  }
}
