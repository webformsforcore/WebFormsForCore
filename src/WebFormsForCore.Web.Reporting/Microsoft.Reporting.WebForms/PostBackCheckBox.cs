using System.Web.UI;
using System.Web.UI.WebControls;

namespace Microsoft.Reporting.WebForms;

internal sealed class PostBackCheckBox : CheckBox, IPostBackEventHandler
{
	public void RaisePostBackEvent(string eventArgument)
	{
		RaiseBubbleEvent(this, new AutoPostBackEventArgs());
	}
}
