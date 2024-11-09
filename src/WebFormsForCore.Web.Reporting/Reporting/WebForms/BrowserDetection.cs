// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.BrowserDetection
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

using Microsoft.ReportingServices.Diagnostics.Utilities;
using System.Web;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal class BrowserDetection : IBrowserDetection
  {
    private static BrowserDetection m_browserDetection;

    public static BrowserDetection Current
    {
      get
      {
        if (BrowserDetection.m_browserDetection == null)
          BrowserDetection.m_browserDetection = new BrowserDetection();
        return BrowserDetection.m_browserDetection;
      }
    }

    public bool IsIE => BrowserDetectionUtility.IsIE55OrHigher(HttpContext.Current.Request);

    public bool IsSafari => BrowserDetectionUtility.IsSafari(HttpContext.Current.Request);
  }
}
