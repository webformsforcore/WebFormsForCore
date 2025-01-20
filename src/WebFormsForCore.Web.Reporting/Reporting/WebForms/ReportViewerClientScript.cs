
using Microsoft.ReportingServices.Diagnostics.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security;
using System.Security.Permissions;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal sealed class ReportViewerClientScript : 
    CompositeControl,
    IScriptControl,
    IPostBackEventHandler
  {
    private List<ScriptDescriptor> m_scriptDescriptors;
    private HiddenField m_actionType;
    private HiddenField m_actionParam;

    public event EventHandler<ReportActionEventArgs> ReportAction;

    void IPostBackEventHandler.RaisePostBackEvent(string eventArgument)
    {
      this.EnsureChildControls();
      if (this.ReportAction == null)
        return;
      this.ReportAction((object) this, new ReportActionEventArgs(this.m_actionType.Value, this.m_actionParam.Value));
    }

    protected override void CreateChildControls()
    {
      this.Controls.Clear();
      base.CreateChildControls();
      this.m_actionType = new HiddenField();
      this.Controls.Add((Control) this.m_actionType);
      this.m_actionParam = new HiddenField();
      this.Controls.Add((Control) this.m_actionParam);
    }

    public static bool IsZoomSupported => ReportViewerClientScript.IsIE55OrHigher;

    public static bool IsPrintingSupported
    {
      get
      {
        if (!ReportViewerClientScript.IsIE55OrHigher)
          return false;
        ClientArchitecture clientArchitecture = BrowserDetectionUtility.GetClientArchitecture();
        return clientArchitecture == ClientArchitecture.X86 || clientArchitecture == ClientArchitecture.X64;
      }
    }

    public static bool IsIE55OrHigher
    {
      get
      {
        return HttpContext.Current == null || BrowserDetectionUtility.IsIE55OrHigher(HttpContext.Current.Request);
      }
    }

    public static bool IsGeckoLayoutEngine
    {
      get
      {
        return HttpContext.Current != null && BrowserDetectionUtility.IsGeckoBrowserEngine(HttpContext.Current.Request.UserAgent);
      }
    }

    protected override void OnPreRender(EventArgs e)
    {
      this.EnsureChildControls();
      base.OnPreRender(e);
      ScriptManager.GetCurrent(this.Page).RegisterScriptControl<ReportViewerClientScript>(this);
    }

    protected override void Render(HtmlTextWriter writer)
    {
      this.EnsureChildControls();
      base.Render(writer);
      if (this.DesignMode)
        return;
      ScriptManager.GetCurrent(this.Page).RegisterScriptDescriptors((IScriptControl) this);
    }

    IEnumerable<ScriptReference> IScriptControl.GetScriptReferences()
    {
      ScriptReference scriptReference = new ScriptReference();
      scriptReference.Path = EmbeddedResourceOperation.CreateUrlForScriptFile();
      return (IEnumerable<ScriptReference>) new ScriptReference[1]
      {
        scriptReference
      };
    }

    IEnumerable<ScriptDescriptor> IScriptControl.GetScriptDescriptors()
    {
      return (IEnumerable<ScriptDescriptor>) this.m_scriptDescriptors;
    }

    public void SetViewerInfo(
      ReportViewer viewer,
      string reportAreaId,
      string promptAreaRowId,
      string docMapAreaId,
      string fixedTableId,
      string topLevelUpdatePanelId,
      string docMapUpdatePanelId,
      string promptSplitterId,
      string docMapSplitterId,
      string docMapHeaderOverflowId,
      string directionCacheId,
      string browserModeCacheId,
      ClientPrintInfo clientPrintInfo)
    {
      this.EnsureChildControls();
      this.m_scriptDescriptors = new List<ScriptDescriptor>(2);
      ScriptControlDescriptor desc = new ScriptControlDescriptor("Microsoft.Reporting.WebFormsClient._InternalReportViewer", this.ClientID);
      this.m_scriptDescriptors.Add((ScriptDescriptor) desc);
      desc.AddProperty("ReportViewerId", (object) viewer.ClientID);
      desc.AddProperty("ReportAreaId", (object) reportAreaId);
      desc.AddProperty("DocMapAreaId", (object) docMapAreaId);
      desc.AddProperty("FixedTableId", (object) fixedTableId);
      desc.AddProperty("TopLevelUpdatePanelId", (object) topLevelUpdatePanelId);
      desc.AddProperty("DocMapUpdatePanelId", (object) docMapUpdatePanelId);
      desc.AddProperty("ActionTypeId", (object) this.m_actionType.ClientID);
      desc.AddProperty("ActionParamId", (object) this.m_actionParam.ClientID);
      desc.AddProperty("HasSizingRow", (object) !viewer.SizeToReportContent);
      desc.AddProperty("BaseHeight", (object) viewer.Height.ToString(CultureInfo.InvariantCulture));
      desc.AddProperty("BaseWidth", (object) viewer.Width.ToString(CultureInfo.InvariantCulture));
      desc.AddProperty("DirectionCacheId", (object) directionCacheId);
      desc.AddProperty("BrowserModeId", (object) browserModeCacheId);
      desc.AddProperty("PromptAreaRowId", (object) promptAreaRowId);
      desc.AddProperty("PromptSplitterId", (object) promptSplitterId);
      desc.AddProperty("DocMapSplitterId", (object) docMapSplitterId);
      desc.AddProperty("DocMapHeaderOverflowDivId", (object) docMapHeaderOverflowId);
      desc.AddProperty("UnableToLoadPrintMessage", (object) LocalizationHelper.Current.ClientPrintControlLoadFailed);
      string functionBody = this.Page.ClientScript.GetPostBackEventReference((Control) this, (string) null) + ";";
      desc.AddScriptProperty("PostBackToClientScript", JavaScriptHelper.FormatAsFunction(functionBody));
      ReportControlSession reportControlSession = viewer.ReportControlSession;
      if (reportControlSession != null && reportControlSession.Report.IsReadyForRendering)
      {
        if (clientPrintInfo != null)
          this.RenderPrintScript(desc, clientPrintInfo);
        string url = ExportOperation.CreateUrl(reportControlSession.Report, viewer.InstanceIdentifier, viewer.ExportContentDisposition);
        desc.AddProperty("ExportUrlBase", (object) url);
      }
      if (!viewer.KeepSessionAlive)
        return;
      ScriptComponentDescriptor request = SessionKeepAliveOperation.CreateRequest(viewer);
      if (request == null)
        return;
      this.m_scriptDescriptors.Add((ScriptDescriptor) request);
      request.ID = viewer.ClientID + "_SessionKeepAlive";
    }

    private void RenderPrintScript(ScriptControlDescriptor desc, ClientPrintInfo clientPrintInfo)
    {
      SecurityAssertionHandler.RunWithSecurityAssert((CodeAccessPermission) new ReflectionPermission(ReflectionPermissionFlag.MemberAccess), (Action) (() => desc.AddScriptProperty("PrintInfo", new JavaScriptSerializer().Serialize((object) clientPrintInfo))));
    }
  }
}
