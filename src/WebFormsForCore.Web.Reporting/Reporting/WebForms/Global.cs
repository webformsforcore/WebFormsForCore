// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.Global
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

using System;
using System.Globalization;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal class Global
  {
    public static void AddStyle(HtmlControl control, string styleName)
    {
      if (styleName == null)
        return;
      string attribute = control.Attributes["class"];
      if (attribute != null && attribute.Length > 0)
      {
        AttributeCollection attributes;
        (attributes = control.Attributes)["class"] = attributes["class"] + " ";
      }
      control.Attributes.Add("class", control.Attributes["class"] + styleName);
    }

    public static void AddStyle(WebControl control, string styleName)
    {
      if (styleName == null)
        return;
      if (control.CssClass != null && control.CssClass.Length > 0)
        control.CssClass += " ";
      control.CssClass += styleName;
    }

    public static string ZoomString(ZoomMode mode, int zoomPercent)
    {
      return mode == ZoomMode.Percent ? zoomPercent.ToString((IFormatProvider) CultureInfo.InvariantCulture) : mode.ToString();
    }

    internal static bool IsDesignTime => HttpContext.Current == null;
  }
}
