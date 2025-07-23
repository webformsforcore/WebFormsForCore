using System.Web.UI;
using System.Web.UI.WebControls;

namespace Microsoft.Reporting.WebForms;

internal sealed class PostBackRadioButton : RadioButton, IPostBackEventHandler
{
	public void RaisePostBackEvent(string eventArgument)
	{
		RaiseBubbleEvent(this, new AutoPostBackEventArgs());
	}
}
