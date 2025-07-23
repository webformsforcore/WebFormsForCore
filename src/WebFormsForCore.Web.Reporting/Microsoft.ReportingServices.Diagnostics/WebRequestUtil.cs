using System;
using System.Web;

namespace Microsoft.ReportingServices.Diagnostics;

internal static class WebRequestUtil
{
	internal const string SharepointPreambleLengthHeaderName = "SharepointPreambleLength";

	internal const string SharepointExternalReportServerUrlHeaderName = "SharepointExternalReportServerUrl";

	internal const string SharepointClientHttpMethodHeaderName = "SharepointClientHttpMethod";

	private static readonly string m_clientHostHeaderName = "RSClientHostName";

	public static string ClientHostHeaderName => m_clientHostHeaderName;

	internal static string SharepointExternalReportServerUrl => HttpUtility.UrlDecode(HttpContext.Current.Request.Headers["SharepointExternalReportServerUrl"]);

	public static string GetHostFromRequest(HttpRequest request)
	{
		string text = request.Headers["host"];
		if (text == null || text.Length == 0)
		{
			string leftPart = request.Url.GetLeftPart(UriPartial.Authority);
			string leftPart2 = request.Url.GetLeftPart(UriPartial.Scheme);
			text = leftPart.Substring(leftPart2.Length);
		}
		return text;
	}

	public static bool IsClientLocal()
	{
		HttpContext current = HttpContext.Current;
		return IsClientLocal(current);
	}

	public static bool IsClientLocal(HttpContext context)
	{
		if (context == null)
		{
			return true;
		}
		if (context.Request.IsLocal)
		{
			return context.Request.Headers[LocalClientConstants.ClientNotLocalHeaderName] == null;
		}
		return false;
	}

	public static bool IsClientLocal(ILocalClient clientDetection)
	{
		return clientDetection?.IsClientLocal ?? IsClientLocal();
	}
}
