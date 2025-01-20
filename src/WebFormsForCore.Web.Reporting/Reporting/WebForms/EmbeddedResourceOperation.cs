
using Microsoft.ReportingServices.Rendering.HtmlRenderer;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Web;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal class EmbeddedResourceOperation : HandlerOperation
  {
    private const string UrlParamName = "Name";
    private const string ResourceNameScript = "ViewerScript";
    private static byte[] m_viewerScript;
    private static string m_assemblyVersion;

    public static string CreateUrl(string resourceName)
    {
      return EmbeddedResourceOperation.CreateUrl(resourceName, "Resource");
    }

    public static string CreateUrlForScriptFile()
    {
      return EmbeddedResourceOperation.CreateUrl("ViewerScript");
    }

    protected static string CreateUrl(
      string resourceName,
      string operationType,
      params string[] additionalParams)
    {
      if (Global.IsDesignTime)
        return EmbeddedResourceOperation.CreateReference(resourceName);
      StringBuilder stringBuilder = new StringBuilder();
      foreach (string additionalParam in additionalParams)
        stringBuilder.Append(additionalParam + "&");
      UriBuilder handlerUri = ReportViewerFactory.HttpHandler.HandlerUri;
      handlerUri.Query = "OpType=" + HttpUtility.UrlEncode(operationType) + "&Version=" + EmbeddedResourceOperation.ProductVersion + "&" + stringBuilder.ToString() + "Name=" + HttpUtility.UrlEncode(resourceName);
      return handlerUri.Uri.PathAndQuery;
    }

    public static string CreateReference(string resourceName) => "{" + resourceName + "}";

    public override bool IsCacheable => true;

    protected virtual byte[] GetResource(string resourceName, out string mimeType)
    {
      return this.GetResource(resourceName, out mimeType, (NameValueCollection) null);
    }

    protected virtual byte[] GetResource(
      string resourceName,
      out string mimeType,
      NameValueCollection urlQuery)
    {
      return resourceName.StartsWith("Microsoft.ReportingServices.Rendering.HtmlRenderer.RendererResources.", StringComparison.OrdinalIgnoreCase) ? HTMLRendererResources.GetBytesFullname(resourceName, out mimeType) : ReportViewerEmbeddedResources.Get(resourceName, out mimeType);
    }

    public override void PerformOperation(NameValueCollection urlQuery, HttpResponse response)
    {
      string andEnsureParam = HandlerOperation.GetAndEnsureParam(urlQuery, "Name");
      string mimeType = (string) null;
      byte[] buffer = (byte[]) null;
      if (!string.IsNullOrEmpty(andEnsureParam))
      {
        if (andEnsureParam.Equals("ViewerScript", StringComparison.Ordinal))
        {
          buffer = this.GetViewerScript();
          mimeType = "application/javascript";
        }
        else
          buffer = this.GetResource(andEnsureParam, out mimeType, urlQuery);
      }
      if (buffer != null)
      {
        response.ContentType = mimeType;
        response.BinaryWrite(buffer);
        DateTime date = DateTime.Now.AddMonths(1);
        response.Cache.SetExpires(date);
      }
      else
        response.StatusCode = 404;
    }

    private byte[] GetViewerScript()
    {
      if (EmbeddedResourceOperation.m_viewerScript == null)
      {
        List<byte> viewerScript = new List<byte>(327680);
        this.LoadScriptFiles(viewerScript);
        viewerScript.AddRange((IEnumerable<byte>) Encoding.UTF8.GetBytes("\nif (typeof(Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();"));
        EmbeddedResourceOperation.m_viewerScript = viewerScript.ToArray();
      }
      return EmbeddedResourceOperation.m_viewerScript;
    }

    protected virtual void LoadScriptFiles(List<byte> viewerScript)
    {
      string mimeType;
      viewerScript.AddRange((IEnumerable<byte>) this.GetResource("Microsoft.Reporting.WebForms.Scripts.Common.js", out mimeType));
      viewerScript.AddRange((IEnumerable<byte>) this.GetResource("Microsoft.Reporting.WebForms.Scripts.HoverImage.js", out mimeType));
      viewerScript.AddRange((IEnumerable<byte>) this.GetResource("Microsoft.Reporting.WebForms.Scripts.InternalReportViewer.js", out mimeType));
      viewerScript.AddRange((IEnumerable<byte>) this.GetResource("Microsoft.Reporting.WebForms.Scripts.ParameterInputControls.js", out mimeType));
      viewerScript.AddRange((IEnumerable<byte>) this.GetResource("Microsoft.Reporting.WebForms.Scripts.PromptArea.js", out mimeType));
      viewerScript.AddRange((IEnumerable<byte>) this.GetResource("Microsoft.Reporting.WebForms.Scripts.ReportArea.js", out mimeType));
      viewerScript.AddRange((IEnumerable<byte>) this.GetResource("Microsoft.Reporting.WebForms.Scripts.ReportPage.js", out mimeType));
      viewerScript.AddRange((IEnumerable<byte>) this.GetResource("Microsoft.Reporting.WebForms.Scripts.SessionKeepAlive.js", out mimeType));
      viewerScript.AddRange((IEnumerable<byte>) this.GetResource("Microsoft.Reporting.WebForms.Scripts.ScriptSwitchImage.js", out mimeType));
      viewerScript.AddRange((IEnumerable<byte>) this.GetResource("Microsoft.Reporting.WebForms.Scripts.TextButton.js", out mimeType));
      viewerScript.AddRange((IEnumerable<byte>) this.GetResource("Microsoft.Reporting.WebForms.Scripts.Toolbar.js", out mimeType));
      viewerScript.AddRange((IEnumerable<byte>) this.GetResource("Microsoft.Reporting.WebForms.Scripts.ReportViewer.js", out mimeType));
      viewerScript.AddRange((IEnumerable<byte>) this.GetResource("Microsoft.Reporting.WebForms.Scripts.ToolbarMenu.js", out mimeType));
      viewerScript.AddRange((IEnumerable<byte>) this.GetResource("Microsoft.Reporting.WebForms.Scripts.Splitter.js", out mimeType));
      viewerScript.AddRange((IEnumerable<byte>) this.GetResource("Microsoft.Reporting.WebForms.Scripts.ResizableBehavior.js", out mimeType));
      viewerScript.AddRange((IEnumerable<byte>) this.GetResource("Microsoft.Reporting.WebForms.Scripts.AsyncWaitControl.js", out mimeType));
      viewerScript.AddRange((IEnumerable<byte>) this.GetResource("Microsoft.Reporting.WebForms.Scripts.DocMapArea.js", out mimeType));
      viewerScript.AddRange((IEnumerable<byte>) LocalHtmlRenderer.GetResource("Common.js", out mimeType));
      viewerScript.AddRange((IEnumerable<byte>) LocalHtmlRenderer.GetResource("FitProportional.js", out mimeType));
      viewerScript.AddRange((IEnumerable<byte>) LocalHtmlRenderer.GetResource("FixedHeader.js", out mimeType));
      viewerScript.AddRange((IEnumerable<byte>) LocalHtmlRenderer.GetResource("CanGrowFalse.js", out mimeType));
      viewerScript.AddRange((IEnumerable<byte>) LocalHtmlRenderer.GetResource("ImageConsolidation.js", out mimeType));
    }

    private static string ProductVersion
    {
      get
      {
        if (EmbeddedResourceOperation.m_assemblyVersion == null)
          EmbeddedResourceOperation.m_assemblyVersion = EmbeddedResourceOperation._GetProductVersion();
        return EmbeddedResourceOperation.m_assemblyVersion;
      }
    }

    [SecurityCritical]
    [SecurityTreatAsSafe]
    private static string _GetProductVersion()
    {
      return SecurityAssertionHandler.RunWithSecurityAssert<string>((CodeAccessPermission) new ReflectionPermission(ReflectionPermissionFlag.NoFlags), (Func<string>) (() => AssemblyVersion.InformationalVersion));
    }
  }
}
