
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Web.UI;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal sealed class AsyncWaitControlTriggerCollection : Collection<Control>
  {
    protected override void InsertItem(int index, Control control)
    {
      if (this.Contains(control))
        return;
      base.InsertItem(index, control);
    }

    public string[] ToClientIDArray()
    {
      List<string> stringList = new List<string>();
      foreach (Control control in (Collection<Control>) this)
        stringList.Add(control.ClientID);
      return stringList.ToArray();
    }
  }
}
