
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal sealed class AsyncWaitControl : CompositeScriptControl
  {
    public const int DefaultWaitDelay = 1000;
    private const float m_backOpacity = 0.7f;
    private IReportViewerStyles m_styles;
    private int m_delayMillis = 1000;
    private WaitControl m_waitControl;
    private HiddenField m_clientCanceled;
    private string m_viewerClientId;
    private string m_viewerFixedTableId;
    private AsyncWaitControlTriggerCollection m_asyncWaitControlTriggerCollection;
    private bool m_skipTimer;

    public event EventHandler<ClientCanceledStateChangeEventArgs> ClientCanceledStateChanged;

    public AsyncWaitControl(IReportViewerStyles styles)
    {
      this.m_styles = styles;
      this.SetUpStyles();
    }

    private void SetUpStyles()
    {
      this.Style.Add(HtmlTextWriterStyle.Position, "absolute");
      this.Style.Add(HtmlTextWriterStyle.Display, "none");
      this.Style.Add(HtmlTextWriterStyle.Filter, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "alpha(opacity={0})", (object) Convert.ToInt32(70f)));
      this.Style.Add("opacity", 0.7f.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      this.BackColor = Color.White;
    }

    private string CancelScriptUrl
    {
      get
      {
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "javascript:$get('{0}').control._cancelCurrentPostback();", (object) JavaScriptHelper.StringEscapeSingleQuote(this.ClientID));
      }
    }

    protected override void CreateChildControls()
    {
      this.Controls.Clear();
      this.m_waitControl = new WaitControl(this.m_styles, LocalizationHelper.Current.ProgressText);
      this.m_waitControl.ID = "Wait";
      this.m_waitControl.Style.Add(HtmlTextWriterStyle.Display, "none");
      this.m_waitControl.Style.Add(HtmlTextWriterStyle.Position, "absolute");
      this.Controls.Add((Control) this.m_waitControl);
      this.m_clientCanceled = new HiddenField();
      this.m_clientCanceled.ID = "HiddenCancelField";
      this.m_clientCanceled.ValueChanged += new EventHandler(this.OnClientCanceledStateChanged);
      this.Controls.Add((Control) this.m_clientCanceled);
    }

    private void OnClientCanceledStateChanged(object sender, EventArgs e)
    {
      if (this.ClientCanceledStateChanged == null)
        return;
      this.ClientCanceledStateChanged((object) this, new ClientCanceledStateChangeEventArgs(this.m_clientCanceled.Value.Equals("true", StringComparison.Ordinal)));
    }

    protected override void OnPreRender(EventArgs e)
    {
      this.EnsureChildControls();
      base.OnPreRender(e);
      this.m_waitControl.CancelUrl = this.CancelScriptUrl;
    }

    protected override void Render(HtmlTextWriter writer)
    {
      if (!this.DesignMode)
        this.ScriptManager.RegisterScriptDescriptors((IScriptControl) this);
      this.EnsureChildControls();
      this.AddAttributesToRender(writer);
      writer.RenderBeginTag(HtmlTextWriterTag.Div);
      writer.RenderEndTag();
      this.m_waitControl.RenderControl(writer);
      this.m_clientCanceled.RenderControl(writer);
    }

    public void SetViewerInfo(
      string viewerClientId,
      string viewerFixedTableId,
      bool clientCanceled,
      bool skipTimer)
    {
      this.m_viewerClientId = viewerClientId;
      this.m_viewerFixedTableId = viewerFixedTableId;
      this.m_clientCanceled.Value = clientCanceled.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      this.m_skipTimer = skipTimer;
    }

    public int DisplayDelayMillis
    {
      get => this.m_delayMillis;
      set => this.m_delayMillis = value;
    }

    public bool CancelLinkVisible
    {
      get
      {
        this.EnsureChildControls();
        return this.m_waitControl.CancelLinkVisible;
      }
      set
      {
        this.EnsureChildControls();
        this.m_waitControl.CancelLinkVisible = value;
      }
    }

    public AsyncWaitControlTriggerCollection Triggers
    {
      get
      {
        if (this.m_asyncWaitControlTriggerCollection == null)
          this.m_asyncWaitControlTriggerCollection = new AsyncWaitControlTriggerCollection();
        return this.m_asyncWaitControlTriggerCollection;
      }
    }

    public override IEnumerable<ScriptDescriptor> GetScriptDescriptors()
    {
      ScriptControlDescriptor controlDescriptor = new ScriptControlDescriptor("Microsoft.Reporting.WebFormsClient._AsyncWaitControl", this.ClientID);
      controlDescriptor.AddProperty("ReportViewerId", (object) this.m_viewerClientId);
      controlDescriptor.AddProperty("DisplayDelay", (object) this.m_delayMillis);
      controlDescriptor.AddProperty("SkipTimer", (object) this.m_skipTimer);
      controlDescriptor.AddProperty("WaitControlId", (object) this.m_waitControl.ClientID);
      controlDescriptor.AddProperty("FixedTableId", (object) this.m_viewerFixedTableId);
      controlDescriptor.AddProperty("ClientCanceledId", (object) this.m_clientCanceled.ClientID);
      controlDescriptor.AddProperty("TriggerIds", (object) this.Triggers.ToClientIDArray());
      return (IEnumerable<ScriptDescriptor>) new ScriptDescriptor[1]
      {
        (ScriptDescriptor) controlDescriptor
      };
    }
  }
}
