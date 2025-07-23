using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.ReportingServices.Diagnostics.Utilities;

namespace Microsoft.Reporting.WebForms;

internal class BrowserNavigationCorrector : CompositeScriptControl
{
	private ReportViewer m_viewer;

	private HiddenField m_scrollPosition;

	private HiddenField m_viewerViewState;

	private UpdatePanel m_updatePanel;

	private HiddenField m_asyncPostBackViewState;

	private HiddenField m_pageState;

	public ReportViewer TargetViewer
	{
		get
		{
			return m_viewer;
		}
		set
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (!(value is IPublicViewState))
			{
				throw new ArgumentOutOfRangeException("value", "Attached ReportViewer must expose ViewState");
			}
			m_viewer = value;
		}
	}

	private bool CorrectionsEnabled
	{
		get
		{
			if (base.ScriptManager.EnablePartialRendering)
			{
				return BrowserDetection.Current.IsIE;
			}
			return false;
		}
	}

	public BrowserNavigationCorrector()
	{
		base.Style.Add(HtmlTextWriterStyle.Display, "none");
	}

	protected override void CreateChildControls()
	{
		Controls.Clear();
		base.CreateChildControls();
		m_scrollPosition = new HiddenField();
		m_scrollPosition.ID = "ScrollPosition";
		Controls.Add(m_scrollPosition);
		m_viewerViewState = new HiddenField();
		m_viewerViewState.ID = "ViewState";
		Controls.Add(m_viewerViewState);
		m_pageState = new HiddenField();
		m_pageState.ID = "PageState";
		Controls.Add(m_pageState);
		m_updatePanel = new UpdatePanel();
		Controls.Add(m_updatePanel);
		m_asyncPostBackViewState = new HiddenField();
		m_asyncPostBackViewState.ID = "NewViewState";
		m_updatePanel.ContentTemplateContainer.Controls.Add(m_asyncPostBackViewState);
	}

	protected override void OnInit(EventArgs e)
	{
		base.OnInit(e);
		base.ScriptManager.RegisterAsyncPostBackControl(this);
		Page.PreRenderComplete += OnPreRenderComplete;
	}

	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);
		EnsureChildControls();
		if (!Page.IsPostBack || !string.Equals(m_pageState.Value, "NeedsCorrection", StringComparison.Ordinal))
		{
			return;
		}
		string value = m_viewerViewState.Value;
		if (!string.IsNullOrEmpty(value))
		{
			LosFormatter losFormatter = new LosFormatter();
			object obj = null;
			try
			{
				obj = losFormatter.Deserialize(value);
			}
			catch (Exception ex)
			{
				RSTrace.UITracer.TraceException(TraceLevel.Warning, "Failed to rebuild the custom ViewState object. \n- Serialized ViewState: \"{0}\". \n- Exception: {1}", new object[2]
				{
					value,
					ex.ToString()
				});
			}
			if (obj != null)
			{
				IPublicViewState publicViewState = (IPublicViewState)m_viewer;
				publicViewState.LoadViewState(obj);
			}
		}
	}

	private void OnPreRenderComplete(object sender, EventArgs e)
	{
		if (base.ScriptManager.IsInAsyncPostBack)
		{
			IPublicViewState publicViewState = (IPublicViewState)m_viewer;
			object value = publicViewState.SaveViewState();
			LosFormatter losFormatter = new LosFormatter();
			using StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);
			losFormatter.Serialize(stringWriter, value);
			m_asyncPostBackViewState.Value = stringWriter.ToString();
			return;
		}
		m_viewerViewState.Value = null;
		m_asyncPostBackViewState.Value = null;
	}

	public override IEnumerable<ScriptDescriptor> GetScriptDescriptors()
	{
		if (CorrectionsEnabled)
		{
			EnsureChildControls();
			ScriptControlDescriptor scriptControlDescriptor = new ScriptControlDescriptor("Microsoft.Reporting.WebFormsClient._BrowserNavigationCorrector", ClientID);
			scriptControlDescriptor.AddProperty("HiddenScrollPositionId", m_scrollPosition.ClientID);
			scriptControlDescriptor.AddProperty("HiddenViewStateId", m_viewerViewState.ClientID);
			scriptControlDescriptor.AddProperty("HiddenNewViewStateId", m_asyncPostBackViewState.ClientID);
			scriptControlDescriptor.AddProperty("ReportViewerId", m_viewer.ClientID);
			scriptControlDescriptor.AddProperty("PageStateId", m_pageState.ClientID);
			string postBackEventReference = Page.ClientScript.GetPostBackEventReference(this, null);
			string script = JavaScriptHelper.FormatAsFunction(postBackEventReference + ";");
			scriptControlDescriptor.AddScriptProperty("TriggerPostBack", script);
			return new ScriptDescriptor[1] { scriptControlDescriptor };
		}
		return null;
	}

	public override IEnumerable<ScriptReference> GetScriptReferences()
	{
		if (CorrectionsEnabled)
		{
			string path = EmbeddedResourceOperation.CreateUrl("Microsoft.Reporting.WebForms.Scripts.BrowserNavigationCorrector.js");
			ScriptReference scriptReference = new ScriptReference(path);
			return new ScriptReference[1] { scriptReference };
		}
		return null;
	}
}
