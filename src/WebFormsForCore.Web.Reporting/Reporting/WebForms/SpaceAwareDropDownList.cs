﻿// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.SpaceAwareDropDownList
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

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
