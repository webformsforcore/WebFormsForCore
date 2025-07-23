using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Microsoft.Reporting.WebForms;

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

	private string CancelScriptUrl => string.Format(CultureInfo.InvariantCulture, "javascript:$get('{0}').control._cancelCurrentPostback();", JavaScriptHelper.StringEscapeSingleQuote(ClientID));

	public int DisplayDelayMillis
	{
		get
		{
			return m_delayMillis;
		}
		set
		{
			m_delayMillis = value;
		}
	}

	public bool CancelLinkVisible
	{
		get
		{
			EnsureChildControls();
			return m_waitControl.CancelLinkVisible;
		}
		set
		{
			EnsureChildControls();
			m_waitControl.CancelLinkVisible = value;
		}
	}

	public AsyncWaitControlTriggerCollection Triggers
	{
		get
		{
			if (m_asyncWaitControlTriggerCollection == null)
			{
				m_asyncWaitControlTriggerCollection = new AsyncWaitControlTriggerCollection();
			}
			return m_asyncWaitControlTriggerCollection;
		}
	}

	public event EventHandler<ClientCanceledStateChangeEventArgs> ClientCanceledStateChanged;

	public AsyncWaitControl(IReportViewerStyles styles)
	{
		m_styles = styles;
		SetUpStyles();
	}

	private void SetUpStyles()
	{
		base.Style.Add(HtmlTextWriterStyle.Position, "absolute");
		base.Style.Add(HtmlTextWriterStyle.Display, "none");
		base.Style.Add(HtmlTextWriterStyle.Filter, string.Format(CultureInfo.InvariantCulture, "alpha(opacity={0})", Convert.ToInt32(70f)));
		base.Style.Add("opacity", 0.7f.ToString(CultureInfo.InvariantCulture));
		BackColor = Color.White;
	}

	protected override void CreateChildControls()
	{
		Controls.Clear();
		m_waitControl = new WaitControl(m_styles, LocalizationHelper.Current.ProgressText);
		m_waitControl.ID = "Wait";
		m_waitControl.Style.Add(HtmlTextWriterStyle.Display, "none");
		m_waitControl.Style.Add(HtmlTextWriterStyle.Position, "absolute");
		Controls.Add(m_waitControl);
		m_clientCanceled = new HiddenField();
		m_clientCanceled.ID = "HiddenCancelField";
		m_clientCanceled.ValueChanged += OnClientCanceledStateChanged;
		Controls.Add(m_clientCanceled);
	}

	private void OnClientCanceledStateChanged(object sender, EventArgs e)
	{
		if (this.ClientCanceledStateChanged != null)
		{
			this.ClientCanceledStateChanged(this, new ClientCanceledStateChangeEventArgs(m_clientCanceled.Value.Equals("true", StringComparison.Ordinal)));
		}
	}

	protected override void OnPreRender(EventArgs e)
	{
		EnsureChildControls();
		base.OnPreRender(e);
		m_waitControl.CancelUrl = CancelScriptUrl;
	}

	protected override void Render(HtmlTextWriter writer)
	{
		if (!base.DesignMode)
		{
			base.ScriptManager.RegisterScriptDescriptors(this);
		}
		EnsureChildControls();
		AddAttributesToRender(writer);
		writer.RenderBeginTag(HtmlTextWriterTag.Div);
		writer.RenderEndTag();
		m_waitControl.RenderControl(writer);
		m_clientCanceled.RenderControl(writer);
	}

	public void SetViewerInfo(string viewerClientId, string viewerFixedTableId, bool clientCanceled, bool skipTimer)
	{
		m_viewerClientId = viewerClientId;
		m_viewerFixedTableId = viewerFixedTableId;
		m_clientCanceled.Value = clientCanceled.ToString(CultureInfo.InvariantCulture);
		m_skipTimer = skipTimer;
	}

	public override IEnumerable<ScriptDescriptor> GetScriptDescriptors()
	{
		ScriptControlDescriptor scriptControlDescriptor = new ScriptControlDescriptor("Microsoft.Reporting.WebFormsClient._AsyncWaitControl", ClientID);
		scriptControlDescriptor.AddProperty("ReportViewerId", m_viewerClientId);
		scriptControlDescriptor.AddProperty("DisplayDelay", m_delayMillis);
		scriptControlDescriptor.AddProperty("SkipTimer", m_skipTimer);
		scriptControlDescriptor.AddProperty("WaitControlId", m_waitControl.ClientID);
		scriptControlDescriptor.AddProperty("FixedTableId", m_viewerFixedTableId);
		scriptControlDescriptor.AddProperty("ClientCanceledId", m_clientCanceled.ClientID);
		scriptControlDescriptor.AddProperty("TriggerIds", Triggers.ToClientIDArray());
		return new ScriptDescriptor[1] { scriptControlDescriptor };
	}
}
