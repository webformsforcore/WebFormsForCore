
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
