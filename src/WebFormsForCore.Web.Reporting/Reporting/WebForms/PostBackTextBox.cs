
using System;
using System.Web.UI;
using System.Web.UI.WebControls;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal sealed class PostBackTextBox : TextBox, IPostBackEventHandler
  {
    public void RaisePostBackEvent(string eventArgument)
    {
      this.RaiseBubbleEvent((object) this, (EventArgs) new AutoPostBackEventArgs());
    }

    protected override void OnLoad(EventArgs e)
    {
      this.EnsureID();
      base.OnLoad(e);
    }
  }
}
