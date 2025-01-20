
using System;
using System.Web.UI;
using System.Web.UI.WebControls;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal class PostBackTarget : WebControl, IPostBackEventHandler
  {
    public event EventHandler PostBackAsTarget;

    public void RaisePostBackEvent(string eventArgument)
    {
      if (this.PostBackAsTarget == null)
        return;
      this.PostBackAsTarget((object) this, EventArgs.Empty);
    }
  }
}
