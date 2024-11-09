// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.ValidatorPanel
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

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
