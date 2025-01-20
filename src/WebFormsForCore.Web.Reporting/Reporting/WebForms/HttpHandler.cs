
using Microsoft.ReportingServices.Diagnostics;
using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Security;
using System.Security.Permissions;
using System.Web;
using System.Web.SessionState;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
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

    internal string GetHttpHandlerTypeName()
    {
      return SecurityAssertionHandler.RunWithSecurityAssert<string>((CodeAccessPermission) new ReflectionPermission(ReflectionPermissionFlag.NoFlags), (Func<string>) (() => this.GetType().AssemblyQualifiedName));
    }

    internal string LegacyHttpHandlerEntry
    {
      get
      {
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "<add verb=\"*\" path=\"{0}\" type = \"{1}\" />", (object) this.HttpHandlerPath, (object) this.GetHttpHandlerTypeName());
      }
    }

    internal string IIS7HttpHandlerEntry
    {
      get
      {
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "<add name=\"{0}\" preCondition=\"integratedMode\" verb=\"*\" path=\"{1}\" type=\"{2}\" />", (object) "ReportViewerWebControlHandler", (object) this.HttpHandlerPath, (object) this.GetHttpHandlerTypeName());
      }
    }

    internal UriBuilder HandlerUri
    {
      get
      {
        UriBuilder handlerUri = new UriBuilder(HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority));
        string applicationPath = this.ApplicationPath;
        if (!applicationPath.EndsWith("/", StringComparison.OrdinalIgnoreCase))
          applicationPath += "/";
        string str = HttpContext.Current.Response.ApplyAppPathModifier(applicationPath + this.HttpHandlerPath);
        handlerUri.Path = str;
        return handlerUri;
      }
    }

    internal virtual string ApplicationPath => HttpContext.Current.Request.ApplicationPath;

    public bool IsReusable => true;

    public void ProcessRequest(HttpContext context)
    {
      string requestParameter = HttpHandler.RequestParameters["OpType"];
      using (MonitoredScope.NewFormat("WebForm::HttpHandler.ProcessRequest - operation type = {0} URL = {1}", (object) requestParameter, (object) context.Request.RawUrl))
      {
        using (HandlerOperation handler = this.GetHandler(requestParameter))
        {
          if (handler == null)
          {
            context.Response.StatusCode = 404;
          }
          else
          {
            context.Response.StatusCode = 200;
            handler.PerformOperation(HttpHandler.RequestParameters, context.Response);
            if (handler.IsCacheable)
              context.Response.Cache.SetCacheability(HttpCacheability.Public);
            if (context.Response.BufferOutput || !string.Equals(context.Request.ServerVariables["SERVER_PROTOCOL"], "HTTP/1.0", StringComparison.OrdinalIgnoreCase))
              return;
            context.Response.Close();
          }
        }
      }
    }

    internal static NameValueCollection RequestParameters
    {
      get => HttpContext.Current.Request.QueryString;
    }

    internal virtual HandlerOperation GetHandler(string operationType)
    {
      if (string.Compare(operationType, "Resource", StringComparison.OrdinalIgnoreCase) == 0)
        return (HandlerOperation) new EmbeddedResourceOperation();
      if (string.Compare(operationType, "ReportImage", StringComparison.OrdinalIgnoreCase) == 0)
        return (HandlerOperation) new ReportImageOperation();
      if (string.Compare(operationType, "Export", StringComparison.OrdinalIgnoreCase) == 0)
        return (HandlerOperation) new ExportOperation();
      if (string.Compare(operationType, "PrintRequest", StringComparison.OrdinalIgnoreCase) == 0)
        return (HandlerOperation) new PrintRequestOperation();
      if (string.Compare(operationType, "PrintCab", StringComparison.OrdinalIgnoreCase) == 0)
        return (HandlerOperation) new PrintCabOperation();
      if (string.Compare(operationType, "Calendar", StringComparison.OrdinalIgnoreCase) == 0)
        return (HandlerOperation) new CalendarIframeOperation();
      if (string.Compare(operationType, "SessionKeepAlive", StringComparison.OrdinalIgnoreCase) == 0)
        return (HandlerOperation) new SessionKeepAliveOperation();
      if (string.Compare(operationType, "StyleSheet", StringComparison.OrdinalIgnoreCase) == 0)
        return (HandlerOperation) new ReportServerStyleSheetOperation();
      if (string.Compare(operationType, "StyleSheetImage", StringComparison.OrdinalIgnoreCase) == 0)
        return (HandlerOperation) new ReportServerStyleSheetOperation();
      return string.Compare(operationType, "BackImage", StringComparison.OrdinalIgnoreCase) == 0 ? (HandlerOperation) new BackgroundImageOperation() : (HandlerOperation) null;
    }
  }
}
