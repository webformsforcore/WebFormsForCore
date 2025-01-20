
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal class ValidatorPanel : WebControl
  {
    public bool HasValidatorsToRender
    {
      get
      {
        this.EnsureChildControls();
        foreach (Control control in this.Controls)
        {
          if (control.Visible && control.Controls.Count > 0)
            return true;
        }
        return false;
      }
    }

    public string[] ChildControlIds
    {
      get
      {
        this.EnsureChildControls();
        List<string> stringList = new List<string>(this.Controls.Count);
        if (this.Controls.Count > 0)
        {
          foreach (Control control in this.Controls)
            stringList.Add(control.ClientID);
        }
        return stringList.ToArray();
      }
    }
  }
}
