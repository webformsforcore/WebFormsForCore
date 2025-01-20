
using Microsoft.ReportingServices.Diagnostics.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal class BrowserNavigationCorrector : CompositeScriptControl
  {
    private ReportViewer m_viewer;
    private HiddenField m_scrollPosition;
    private HiddenField m_viewerViewState;
    private UpdatePanel m_updatePanel;
    private HiddenField m_asyncPostBackViewState;
    private HiddenField m_pageState;

    public BrowserNavigationCorrector() => this.Style.Add(HtmlTextWriterStyle.Display, "none");

    public ReportViewer TargetViewer
    {
      get => this.m_viewer;
      set
      {
        if (value == null)
          throw new ArgumentNullException(nameof (value));
        this.m_viewer = value is IPublicViewState ? value : throw new ArgumentOutOfRangeException(nameof (value), "Attached ReportViewer must expose ViewState");
      }
    }

    protected override void CreateChildControls()
    {
      this.Controls.Clear();
      base.CreateChildControls();
      this.m_scrollPosition = new HiddenField();
      this.m_scrollPosition.ID = "ScrollPosition";
      this.Controls.Add((Control) this.m_scrollPosition);
      this.m_viewerViewState = new HiddenField();
      this.m_viewerViewState.ID = "ViewState";
      this.Controls.Add((Control) this.m_viewerViewState);
      this.m_pageState = new HiddenField();
      this.m_pageState.ID = "PageState";
      this.Controls.Add((Control) this.m_pageState);
      this.m_updatePanel = new UpdatePanel();
      this.Controls.Add((Control) this.m_updatePanel);
      this.m_asyncPostBackViewState = new HiddenField();
      this.m_asyncPostBackViewState.ID = "NewViewState";
      this.m_updatePanel.ContentTemplateContainer.Controls.Add((Control) this.m_asyncPostBackViewState);
    }

    protected override void OnInit(EventArgs e)
    {
      base.OnInit(e);
      this.ScriptManager.RegisterAsyncPostBackControl((Control) this);
      this.Page.PreRenderComplete += new EventHandler(this.OnPreRenderComplete);
    }

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);
      this.EnsureChildControls();
      if (!this.Page.IsPostBack || !string.Equals(this.m_pageState.Value, "NeedsCorrection", StringComparison.Ordinal))
        return;
      string input = this.m_viewerViewState.Value;
      if (string.IsNullOrEmpty(input))
        return;
      LosFormatter losFormatter = new LosFormatter();
      object viewState = (object) null;
      try
      {
        viewState = losFormatter.Deserialize(input);
      }
      catch (Exception ex)
      {
        RSTrace.UITracer.TraceException(TraceLevel.Warning, "Failed to rebuild the custom ViewState object. \n- Serialized ViewState: \"{0}\". \n- Exception: {1}", new object[2]
        {
          (object) input,
          (object) ex.ToString()
        });
      }
      if (viewState == null)
        return;
      ((IPublicViewState) this.m_viewer).LoadViewState(viewState);
    }

    private void OnPreRenderComplete(object sender, EventArgs e)
    {
      if (this.ScriptManager.IsInAsyncPostBack)
      {
        object obj = ((IPublicViewState) this.m_viewer).SaveViewState();
        using (StringWriter output = new StringWriter((IFormatProvider) CultureInfo.InvariantCulture))
        {
          new LosFormatter().Serialize((TextWriter) output, obj);
          this.m_asyncPostBackViewState.Value = output.ToString();
        }
      }
      else
      {
        this.m_viewerViewState.Value = (string) null;
        this.m_asyncPostBackViewState.Value = (string) null;
      }
    }

    private bool CorrectionsEnabled
    {
      get => this.ScriptManager.EnablePartialRendering && BrowserDetection.Current.IsIE;
    }

    public override IEnumerable<ScriptDescriptor> GetScriptDescriptors()
    {
      if (!this.CorrectionsEnabled)
        return (IEnumerable<ScriptDescriptor>) null;
      this.EnsureChildControls();
      ScriptControlDescriptor controlDescriptor = new ScriptControlDescriptor("Microsoft.Reporting.WebFormsClient._BrowserNavigationCorrector", this.ClientID);
      controlDescriptor.AddProperty("HiddenScrollPositionId", (object) this.m_scrollPosition.ClientID);
      controlDescriptor.AddProperty("HiddenViewStateId", (object) this.m_viewerViewState.ClientID);
      controlDescriptor.AddProperty("HiddenNewViewStateId", (object) this.m_asyncPostBackViewState.ClientID);
      controlDescriptor.AddProperty("ReportViewerId", (object) this.m_viewer.ClientID);
      controlDescriptor.AddProperty("PageStateId", (object) this.m_pageState.ClientID);
      string script = JavaScriptHelper.FormatAsFunction(this.Page.ClientScript.GetPostBackEventReference((Control) this, (string) null) + ";");
      controlDescriptor.AddScriptProperty("TriggerPostBack", script);
      return (IEnumerable<ScriptDescriptor>) new ScriptDescriptor[1]
      {
        (ScriptDescriptor) controlDescriptor
      };
    }

    public override IEnumerable<ScriptReference> GetScriptReferences()
    {
      if (!this.CorrectionsEnabled)
        return (IEnumerable<ScriptReference>) null;
      return (IEnumerable<ScriptReference>) new ScriptReference[1]
      {
        new ScriptReference(EmbeddedResourceOperation.CreateUrl("Microsoft.Reporting.WebForms.Scripts.BrowserNavigationCorrector.js"))
      };
    }
  }
}
