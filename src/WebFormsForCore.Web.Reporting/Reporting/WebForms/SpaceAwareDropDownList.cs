
using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal sealed class SpaceAwareDropDownList : DropDownList, IPostBackEventHandler
  {
    protected override void OnLoad(EventArgs e)
    {
      this.EnsureID();
      base.OnLoad(e);
    }

    protected override void RenderContents(HtmlTextWriter writer)
    {
      ListItemCollection items = this.Items;
      for (int index = 0; index < items.Count; ++index)
      {
        ListItem listItem = items[index];
        writer.WriteBeginTag("option");
        if (listItem.Selected)
          writer.WriteAttribute("selected", "selected");
        writer.WriteAttribute("value", listItem.Value, true);
        if (this.Page != null)
          this.Page.ClientScript.RegisterForEventValidation(this.UniqueID, listItem.Value);
        writer.Write('>');
        string str = HttpUtility.HtmlEncode(listItem.Text).Replace(" ", "&nbsp;");
        writer.Write(str);
        writer.WriteEndTag("option");
        writer.WriteLine();
      }
    }

    public void RaisePostBackEvent(string eventArgument)
    {
      this.RaiseBubbleEvent((object) this, (EventArgs) new AutoPostBackEventArgs());
    }
  }
}
