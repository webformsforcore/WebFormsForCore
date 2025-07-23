using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Web;
using Microsoft.ReportingServices.Rendering.HtmlRenderer;

namespace Microsoft.Reporting.WebForms;

internal class EmbeddedResourceOperation : HandlerOperation
{
	private const string UrlParamName = "Name";

	private const string ResourceNameScript = "ViewerScript";

	private static byte[] m_viewerScript;

	private static string m_assemblyVersion;

	public override bool IsCacheable => true;

	private static string ProductVersion
	{
		get
		{
			if (m_assemblyVersion == null)
			{
				m_assemblyVersion = _GetProductVersion();
			}
			return m_assemblyVersion;
		}
	}

	public static string CreateUrl(string resourceName)
	{
		return CreateUrl(resourceName, "Resource");
	}

	public static string CreateUrlForScriptFile()
	{
		return CreateUrl("ViewerScript");
	}

	protected static string CreateUrl(string resourceName, string operationType, params string[] additionalParams)
	{
		if (Global.IsDesignTime)
		{
			return CreateReference(resourceName);
		}
		StringBuilder stringBuilder = new StringBuilder();
		foreach (string text in additionalParams)
		{
			stringBuilder.Append(text + "&");
		}
		UriBuilder handlerUri = ReportViewerFactory.HttpHandler.HandlerUri;
		handlerUri.Query = "OpType=" + HttpUtility.UrlEncode(operationType) + "&Version=" + ProductVersion + "&" + stringBuilder.ToString() + "Name=" + HttpUtility.UrlEncode(resourceName);
		return handlerUri.Uri.PathAndQuery;
	}

	public static string CreateReference(string resourceName)
	{
		return "{" + resourceName + "}";
	}

	protected virtual byte[] GetResource(string resourceName, out string mimeType)
	{
		return GetResource(resourceName, out mimeType, null);
	}

	protected virtual byte[] GetResource(string resourceName, out string mimeType, NameValueCollection urlQuery)
	{
		if (resourceName.StartsWith("Microsoft.ReportingServices.Rendering.HtmlRenderer.RendererResources.", StringComparison.OrdinalIgnoreCase))
		{
			return HTMLRendererResources.GetBytesFullname(resourceName, out mimeType);
		}
		return ReportViewerEmbeddedResources.Get(resourceName, out mimeType);
	}

	public override void PerformOperation(NameValueCollection urlQuery, HttpResponse response)
	{
		string andEnsureParam = HandlerOperation.GetAndEnsureParam(urlQuery, "Name");
		string mimeType = null;
		byte[] array = null;
		if (!string.IsNullOrEmpty(andEnsureParam))
		{
			if (andEnsureParam.Equals("ViewerScript", StringComparison.Ordinal))
			{
				array = GetViewerScript();
				mimeType = "application/javascript";
			}
			else
			{
				array = GetResource(andEnsureParam, out mimeType, urlQuery);
			}
		}
		if (array != null)
		{
			response.ContentType = mimeType;
			response.BinaryWrite(array);
			DateTime expires = DateTime.Now.AddMonths(1);
			response.Cache.SetExpires(expires);
		}
		else
		{
			response.StatusCode = 404;
		}
	}

	private byte[] GetViewerScript()
	{
		if (m_viewerScript == null)
		{
			List<byte> list = new List<byte>(327680);
			LoadScriptFiles(list);
			list.AddRange(Encoding.UTF8.GetBytes("\nif (typeof(Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();"));
			m_viewerScript = list.ToArray();
		}
		return m_viewerScript;
	}

	protected virtual void LoadScriptFiles(List<byte> viewerScript)
	{
		viewerScript.AddRange(GetResource("Microsoft.Reporting.WebForms.Scripts.Common.js", out var mimeType));
		viewerScript.AddRange(GetResource("Microsoft.Reporting.WebForms.Scripts.HoverImage.js", out mimeType));
		viewerScript.AddRange(GetResource("Microsoft.Reporting.WebForms.Scripts.InternalReportViewer.js", out mimeType));
		viewerScript.AddRange(GetResource("Microsoft.Reporting.WebForms.Scripts.ParameterInputControls.js", out mimeType));
		viewerScript.AddRange(GetResource("Microsoft.Reporting.WebForms.Scripts.PromptArea.js", out mimeType));
		viewerScript.AddRange(GetResource("Microsoft.Reporting.WebForms.Scripts.ReportArea.js", out mimeType));
		viewerScript.AddRange(GetResource("Microsoft.Reporting.WebForms.Scripts.ReportPage.js", out mimeType));
		viewerScript.AddRange(GetResource("Microsoft.Reporting.WebForms.Scripts.SessionKeepAlive.js", out mimeType));
		viewerScript.AddRange(GetResource("Microsoft.Reporting.WebForms.Scripts.ScriptSwitchImage.js", out mimeType));
		viewerScript.AddRange(GetResource("Microsoft.Reporting.WebForms.Scripts.TextButton.js", out mimeType));
		viewerScript.AddRange(GetResource("Microsoft.Reporting.WebForms.Scripts.Toolbar.js", out mimeType));
		viewerScript.AddRange(GetResource("Microsoft.Reporting.WebForms.Scripts.ReportViewer.js", out mimeType));
		viewerScript.AddRange(GetResource("Microsoft.Reporting.WebForms.Scripts.ToolbarMenu.js", out mimeType));
		viewerScript.AddRange(GetResource("Microsoft.Reporting.WebForms.Scripts.Splitter.js", out mimeType));
		viewerScript.AddRange(GetResource("Microsoft.Reporting.WebForms.Scripts.ResizableBehavior.js", out mimeType));
		viewerScript.AddRange(GetResource("Microsoft.Reporting.WebForms.Scripts.AsyncWaitControl.js", out mimeType));
		viewerScript.AddRange(GetResource("Microsoft.Reporting.WebForms.Scripts.DocMapArea.js", out mimeType));
		viewerScript.AddRange(LocalHtmlRenderer.GetResource("Common.js", out mimeType));
		viewerScript.AddRange(LocalHtmlRenderer.GetResource("FitProportional.js", out mimeType));
		viewerScript.AddRange(LocalHtmlRenderer.GetResource("FixedHeader.js", out mimeType));
		viewerScript.AddRange(LocalHtmlRenderer.GetResource("CanGrowFalse.js", out mimeType));
		viewerScript.AddRange(LocalHtmlRenderer.GetResource("ImageConsolidation.js", out mimeType));
	}

	[SecurityCritical]
	[SecurityTreatAsSafe]
	private static string _GetProductVersion()
	{
		return SecurityAssertionHandler.RunWithSecurityAssert(new ReflectionPermission(ReflectionPermissionFlag.NoFlags), () => AssemblyVersion.InformationalVersion);
	}
}
