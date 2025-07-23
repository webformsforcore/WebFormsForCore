using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Permissions;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.ReportingServices.Diagnostics.Utilities;

namespace Microsoft.Reporting.WebForms;

internal sealed class ReportViewerClientScript : CompositeControl, IScriptControl, IPostBackEventHandler
{
	private List<ScriptDescriptor> m_scriptDescriptors;

	private HiddenField m_actionType;

	private HiddenField m_actionParam;

	public static bool IsZoomSupported => IsIE55OrHigher;

	public static bool IsPrintingSupported
	{
		get
		{
			if (IsIE55OrHigher)
			{
				ClientArchitecture clientArchitecture = BrowserDetectionUtility.GetClientArchitecture();
				if (clientArchitecture != ClientArchitecture.X86)
				{
					return clientArchitecture == ClientArchitecture.X64;
				}
				return true;
			}
			return false;
		}
	}

	public static bool IsIE55OrHigher
	{
		get
		{
			if (HttpContext.Current == null)
			{
				return true;
			}
			return BrowserDetectionUtility.IsIE55OrHigher(HttpContext.Current.Request);
		}
	}

	public static bool IsGeckoLayoutEngine
	{
		get
		{
			if (HttpContext.Current == null)
			{
				return false;
			}
			return BrowserDetectionUtility.IsGeckoBrowserEngine(HttpContext.Current.Request.UserAgent);
		}
	}

	public event EventHandler<ReportActionEventArgs> ReportAction;

	void IPostBackEventHandler.RaisePostBackEvent(string eventArgument)
	{
		EnsureChildControls();
		if (this.ReportAction != null)
		{
			ReportActionEventArgs e = new ReportActionEventArgs(m_actionType.Value, m_actionParam.Value);
			this.ReportAction(this, e);
		}
	}

	protected override void CreateChildControls()
	{
		Controls.Clear();
		base.CreateChildControls();
		m_actionType = new HiddenField();
		Controls.Add(m_actionType);
		m_actionParam = new HiddenField();
		Controls.Add(m_actionParam);
	}

	protected override void OnPreRender(EventArgs e)
	{
		EnsureChildControls();
		base.OnPreRender(e);
		ScriptManager.GetCurrent(Page).RegisterScriptControl(this);
	}

	protected override void Render(HtmlTextWriter writer)
	{
		EnsureChildControls();
		base.Render(writer);
		if (!base.DesignMode)
		{
			ScriptManager.GetCurrent(Page).RegisterScriptDescriptors(this);
		}
	}

	IEnumerable<ScriptReference> IScriptControl.GetScriptReferences()
	{
		ScriptReference scriptReference = new ScriptReference();
		scriptReference.Path = EmbeddedResourceOperation.CreateUrlForScriptFile();
		return new ScriptReference[1] { scriptReference };
	}

	IEnumerable<ScriptDescriptor> IScriptControl.GetScriptDescriptors()
	{
		return m_scriptDescriptors;
	}

	public void SetViewerInfo(ReportViewer viewer, string reportAreaId, string promptAreaRowId, string docMapAreaId, string fixedTableId, string topLevelUpdatePanelId, string docMapUpdatePanelId, string promptSplitterId, string docMapSplitterId, string docMapHeaderOverflowId, string directionCacheId, string browserModeCacheId, ClientPrintInfo clientPrintInfo)
	{
		EnsureChildControls();
		m_scriptDescriptors = new List<ScriptDescriptor>(2);
		ScriptControlDescriptor scriptControlDescriptor = new ScriptControlDescriptor("Microsoft.Reporting.WebFormsClient._InternalReportViewer", ClientID);
		m_scriptDescriptors.Add(scriptControlDescriptor);
		scriptControlDescriptor.AddProperty("ReportViewerId", viewer.ClientID);
		scriptControlDescriptor.AddProperty("ReportAreaId", reportAreaId);
		scriptControlDescriptor.AddProperty("DocMapAreaId", docMapAreaId);
		scriptControlDescriptor.AddProperty("FixedTableId", fixedTableId);
		scriptControlDescriptor.AddProperty("TopLevelUpdatePanelId", topLevelUpdatePanelId);
		scriptControlDescriptor.AddProperty("DocMapUpdatePanelId", docMapUpdatePanelId);
		scriptControlDescriptor.AddProperty("ActionTypeId", m_actionType.ClientID);
		scriptControlDescriptor.AddProperty("ActionParamId", m_actionParam.ClientID);
		scriptControlDescriptor.AddProperty("HasSizingRow", !viewer.SizeToReportContent);
		scriptControlDescriptor.AddProperty("BaseHeight", viewer.Height.ToString(CultureInfo.InvariantCulture));
		scriptControlDescriptor.AddProperty("BaseWidth", viewer.Width.ToString(CultureInfo.InvariantCulture));
		scriptControlDescriptor.AddProperty("DirectionCacheId", directionCacheId);
		scriptControlDescriptor.AddProperty("BrowserModeId", browserModeCacheId);
		scriptControlDescriptor.AddProperty("PromptAreaRowId", promptAreaRowId);
		scriptControlDescriptor.AddProperty("PromptSplitterId", promptSplitterId);
		scriptControlDescriptor.AddProperty("DocMapSplitterId", docMapSplitterId);
		scriptControlDescriptor.AddProperty("DocMapHeaderOverflowDivId", docMapHeaderOverflowId);
		scriptControlDescriptor.AddProperty("UnableToLoadPrintMessage", LocalizationHelper.Current.ClientPrintControlLoadFailed);
		string functionBody = Page.ClientScript.GetPostBackEventReference(this, null) + ";";
		scriptControlDescriptor.AddScriptProperty("PostBackToClientScript", JavaScriptHelper.FormatAsFunction(functionBody));
		ReportControlSession reportControlSession = viewer.ReportControlSession;
		if (reportControlSession != null && reportControlSession.Report.IsReadyForRendering)
		{
			if (clientPrintInfo != null)
			{
				RenderPrintScript(scriptControlDescriptor, clientPrintInfo);
			}
			string value = ExportOperation.CreateUrl(reportControlSession.Report, viewer.InstanceIdentifier, viewer.ExportContentDisposition);
			scriptControlDescriptor.AddProperty("ExportUrlBase", value);
		}
		if (viewer.KeepSessionAlive)
		{
			ScriptComponentDescriptor scriptComponentDescriptor = SessionKeepAliveOperation.CreateRequest(viewer);
			if (scriptComponentDescriptor != null)
			{
				m_scriptDescriptors.Add(scriptComponentDescriptor);
				scriptComponentDescriptor.ID = viewer.ClientID + "_SessionKeepAlive";
			}
		}
	}

	private void RenderPrintScript(ScriptControlDescriptor desc, ClientPrintInfo clientPrintInfo)
	{
		SecurityAssertionHandler.RunWithSecurityAssert(new ReflectionPermission(ReflectionPermissionFlag.MemberAccess), delegate
		{
			JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
			string script = javaScriptSerializer.Serialize(clientPrintInfo);
			desc.AddScriptProperty("PrintInfo", script);
		});
	}
}
