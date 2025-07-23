using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Microsoft.Reporting.WebForms;

internal sealed class SpaceAwareDropDownList : DropDownList, IPostBackEventHandler
{
	protected override void OnLoad(EventArgs e)
	{
		EnsureID();
		base.OnLoad(e);
	}

	protected override void RenderContents(HtmlTextWriter writer)
	{
		ListItemCollection listItemCollection = Items;
		for (int i = 0; i < listItemCollection.Count; i++)
		{
			ListItem listItem = listItemCollection[i];
			writer.WriteBeginTag("option");
			if (listItem.Selected)
			{
				writer.WriteAttribute("selected", "selected");
			}
			writer.WriteAttribute("value", listItem.Value, fEncode: true);
			if (Page != null)
			{
				Page.ClientScript.RegisterForEventValidation(UniqueID, listItem.Value);
			}
			writer.Write('>');
			string text = HttpUtility.HtmlEncode(listItem.Text);
			text = text.Replace(" ", "&nbsp;");
			writer.Write(text);
			writer.WriteEndTag("option");
			writer.WriteLine();
		}
	}

	public void RaisePostBackEvent(string eventArgument)
	{
		RaiseBubbleEvent(this, new AutoPostBackEventArgs());
	}
}
