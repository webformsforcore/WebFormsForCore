
using System;
using System.Web.UI;
using System.Web.UI.WebControls;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal sealed class PostBackRadioButton : RadioButton, IPostBackEventHandler
  {
    public void RaisePostBackEvent(string eventArgument)
    {
      this.RaiseBubbleEvent((object) this, (EventArgs) new AutoPostBackEventArgs());
    }
  }
}
