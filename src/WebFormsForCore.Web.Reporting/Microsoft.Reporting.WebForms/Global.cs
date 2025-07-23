using System.Globalization;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Microsoft.Reporting.WebForms;

internal class Global
{
	internal static bool IsDesignTime => HttpContext.Current == null;

	public static void AddStyle(HtmlControl control, string styleName)
	{
		if (styleName != null)
		{
			string text = control.Attributes["class"];
			if (text != null && text.Length > 0)
			{
				control.Attributes["class"] += " ";
			}
			control.Attributes.Add("class", control.Attributes["class"] + styleName);
		}
	}

	public static void AddStyle(WebControl control, string styleName)
	{
		if (styleName != null)
		{
			if (control.CssClass != null && control.CssClass.Length > 0)
			{
				control.CssClass += " ";
			}
			control.CssClass += styleName;
		}
	}

	public static string ZoomString(ZoomMode mode, int zoomPercent)
	{
		if (mode == ZoomMode.Percent)
		{
			return zoomPercent.ToString(CultureInfo.InvariantCulture);
		}
		return mode.ToString();
	}
}
