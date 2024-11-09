// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.AsyncWaitControlTriggerCollection
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

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
