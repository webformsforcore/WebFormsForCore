using System.Web;
using Microsoft.ReportingServices.Diagnostics.Utilities;

namespace Microsoft.Reporting.WebForms;

internal class BrowserDetection : IBrowserDetection
{
	private static BrowserDetection m_browserDetection;

	public static BrowserDetection Current
	{
		get
		{
			if (m_browserDetection == null)
			{
				m_browserDetection = new BrowserDetection();
			}
			return m_browserDetection;
		}
	}

	public bool IsIE => BrowserDetectionUtility.IsIE55OrHigher(HttpContext.Current.Request);

	public bool IsSafari => BrowserDetectionUtility.IsSafari(HttpContext.Current.Request);
}
