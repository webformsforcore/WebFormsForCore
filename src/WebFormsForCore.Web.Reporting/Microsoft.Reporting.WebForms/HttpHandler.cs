using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Security.Permissions;
using System.Web;
using System.Web.SessionState;
using Microsoft.ReportingServices.Diagnostics;

namespace Microsoft.Reporting.WebForms;

public class HttpHandler : IHttpHandler, IRequiresSessionState
{
	internal const string LegacySystemWebSection = "system.web";

	internal const string LegacyHttpHandlerNodeName = "httpHandlers";

	internal const string LegacyHttpHandlerSection = "system.web/httpHandlers";

	internal const string IIS7WebServerSection = "system.webServer";

	internal const string IIS7HandlerNodeName = "handlers";

	internal const string IIS7HttpHandlerSection = "system.webServer/handlers";

	internal const string IIS7HandlerName = "ReportViewerWebControlHandler";

	internal virtual string HttpHandlerPath => "Reserved.ReportViewerWebControl.axd";

	internal string LegacyHttpHandlerEntry => string.Format(CultureInfo.InvariantCulture, "<add verb=\"*\" path=\"{0}\" type = \"{1}\" />", HttpHandlerPath, GetHttpHandlerTypeName());

	internal string IIS7HttpHandlerEntry => string.Format(CultureInfo.InvariantCulture, "<add name=\"{0}\" preCondition=\"integratedMode\" verb=\"*\" path=\"{1}\" type=\"{2}\" />", "ReportViewerWebControlHandler", HttpHandlerPath, GetHttpHandlerTypeName());

	internal UriBuilder HandlerUri
	{
		get
		{
			UriBuilder uriBuilder = new UriBuilder(HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority));
			string text = ApplicationPath;
			if (!text.EndsWith("/", StringComparison.OrdinalIgnoreCase))
			{
				text += "/";
			}
			text += HttpHandlerPath;
			text = HttpContext.Current.Response.ApplyAppPathModifier(text);
			uriBuilder.Path = text;
			return uriBuilder;
		}
	}

	internal virtual string ApplicationPath => HttpContext.Current.Request.ApplicationPath;

	public bool IsReusable => true;

	internal static NameValueCollection RequestParameters => HttpContext.Current.Request.QueryString;

	internal string GetHttpHandlerTypeName()
	{
		return SecurityAssertionHandler.RunWithSecurityAssert(new ReflectionPermission(ReflectionPermissionFlag.NoFlags), () => GetType().AssemblyQualifiedName);
	}

	public void ProcessRequest(HttpContext context)
	{
		string text = RequestParameters["OpType"];
		MonitoredScope val = MonitoredScope.NewFormat("WebForm::HttpHandler.ProcessRequest - operation type = {0} URL = {1}", (object)text, (object)context.Request.RawUrl);
		try
		{
			using HandlerOperation handlerOperation = GetHandler(text);
			if (handlerOperation == null)
			{
				context.Response.StatusCode = 404;
				return;
			}
			context.Response.StatusCode = 200;
			handlerOperation.PerformOperation(RequestParameters, context.Response);
			if (handlerOperation.IsCacheable)
			{
				context.Response.Cache.SetCacheability(HttpCacheability.Public);
			}
			if (!context.Response.BufferOutput)
			{
				string a = context.Request.ServerVariables["SERVER_PROTOCOL"];
				if (string.Equals(a, "HTTP/1.0", StringComparison.OrdinalIgnoreCase))
				{
					context.Response.Close();
				}
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	internal virtual HandlerOperation GetHandler(string operationType)
	{
		if (string.Compare(operationType, "Resource", StringComparison.OrdinalIgnoreCase) == 0)
		{
			return new EmbeddedResourceOperation();
		}
		if (string.Compare(operationType, "ReportImage", StringComparison.OrdinalIgnoreCase) == 0)
		{
			return new ReportImageOperation();
		}
		if (string.Compare(operationType, "Export", StringComparison.OrdinalIgnoreCase) == 0)
		{
			return new ExportOperation();
		}
		if (string.Compare(operationType, "PrintRequest", StringComparison.OrdinalIgnoreCase) == 0)
		{
			return new PrintRequestOperation();
		}
		if (string.Compare(operationType, "PrintCab", StringComparison.OrdinalIgnoreCase) == 0)
		{
			return new PrintCabOperation();
		}
		if (string.Compare(operationType, "Calendar", StringComparison.OrdinalIgnoreCase) == 0)
		{
			return new CalendarIframeOperation();
		}
		if (string.Compare(operationType, "SessionKeepAlive", StringComparison.OrdinalIgnoreCase) == 0)
		{
			return new SessionKeepAliveOperation();
		}
		if (string.Compare(operationType, "StyleSheet", StringComparison.OrdinalIgnoreCase) == 0)
		{
			return new ReportServerStyleSheetOperation();
		}
		if (string.Compare(operationType, "StyleSheetImage", StringComparison.OrdinalIgnoreCase) == 0)
		{
			return new ReportServerStyleSheetOperation();
		}
		if (string.Compare(operationType, "BackImage", StringComparison.OrdinalIgnoreCase) == 0)
		{
			return new BackgroundImageOperation();
		}
		return null;
	}
}
