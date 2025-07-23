using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.ReportingServices.Rendering.HtmlRenderer;

namespace Microsoft.Reporting.WebForms;

internal sealed class ReportControl : CompositeControl, IPostBackEventHandler, IScriptControl
{
	public const string AutoRefreshParam = "auto";

	private ReportControlSession m_session;

	private Stream m_reportStream;

	private string m_styleBytesString;

	private string m_viewerInstanceIdentifier;

	private int m_pageNumber;

	private SearchState m_searchState;

	private int m_autoRefreshInterval;

	private string m_alertMessage;

	private ScrollTarget m_scrollTarget;

	private InteractivityPostBackMode m_interactivityMode;

	private string m_visibleContainerId;

	private HiddenField m_hiddenActionType;

	private HiddenField m_hiddenActionParam;

	private HiddenField m_hiddenZoomLevel;

	private Timer m_autoRefreshTimer;

	private PostBackTarget m_syncPostBackTarget;

	private string m_scrollScript;

	private string m_scrollContainerId;

	private string m_pageStyles;

	private static DeviceInfoNameBlackList m_blackListDeviceInfoNames;

	protected override HtmlTextWriterTag TagKey => HtmlTextWriterTag.Div;

	public string ScrollContainerId
	{
		get
		{
			return m_scrollContainerId;
		}
		set
		{
			m_scrollContainerId = value;
		}
	}

	public string VisibleContainerId
	{
		get
		{
			return m_visibleContainerId;
		}
		set
		{
			m_visibleContainerId = value;
		}
	}

	private string ActionScriptMethod => string.Format(CultureInfo.InvariantCulture, "var rp=$get('{0}');if(rp&&rp.control)rp.control.InvokeReportAction", JavaScriptHelper.StringEscapeSingleQuote(ClientID));

	private int ViewIteration
	{
		get
		{
			object obj = ViewState["ViewIteration"];
			if (obj == null)
			{
				return 0;
			}
			return (int)obj;
		}
		set
		{
			ViewState["ViewIteration"] = value;
		}
	}

	private string UniqueRenderingId => MakeUniqueRenderingId(m_viewerInstanceIdentifier, ViewIteration);

	public event EventHandler<ReportActionEventArgs> ReportAction;

	public event ZoomChangedEventHandler ZoomChanged;

	public ReportControl()
	{
		base.Style.Add(HtmlTextWriterStyle.Display, "none");
		ID = "ReportControl";
	}

	public override void Dispose()
	{
		ClearReport();
		base.Dispose();
	}

	protected override void CreateChildControls()
	{
		Controls.Clear();
		base.CreateChildControls();
		m_syncPostBackTarget = new PostBackTarget();
		m_syncPostBackTarget.PostBackAsTarget += OnReportAction;
		ScriptManager.GetCurrent(Page).RegisterPostBackControl(m_syncPostBackTarget);
		Controls.Add(m_syncPostBackTarget);
		m_autoRefreshTimer = new Timer();
		m_autoRefreshTimer.Tick += OnAutoRefresh;
		Controls.Add(m_autoRefreshTimer);
		m_hiddenActionType = new HiddenField();
		Controls.Add(m_hiddenActionType);
		m_hiddenActionParam = new HiddenField();
		Controls.Add(m_hiddenActionParam);
		m_hiddenZoomLevel = new HiddenField();
		m_hiddenZoomLevel.ValueChanged += OnZoomChanged;
		Controls.Add(m_hiddenZoomLevel);
	}

	protected override void OnPreRender(EventArgs e)
	{
		EnsureChildControls();
		base.OnPreRender(e);
		ScriptManager.GetCurrent(Page)?.RegisterScriptControl(this);
		m_hiddenActionType.Value = null;
		m_hiddenActionParam.Value = null;
		m_autoRefreshTimer.Enabled = m_reportStream != null && m_autoRefreshInterval > 0;
		if (m_autoRefreshTimer.Enabled)
		{
			m_autoRefreshTimer.Interval = m_autoRefreshInterval * 1000;
		}
	}

	private void RemoveNulls(char[] chars, int count)
	{
		if (chars == null)
		{
			return;
		}
		for (int i = 0; i < chars.Length && i < count; i++)
		{
			if (chars[i] == '\0')
			{
				chars[i] = ' ';
			}
		}
	}

	protected override void RenderChildren(HtmlTextWriter writer)
	{
		ScriptManager current = ScriptManager.GetCurrent(Page);
		current?.RegisterScriptDescriptors(this);
		m_syncPostBackTarget.RenderControl(writer);
		m_hiddenActionType.RenderControl(writer);
		m_hiddenActionParam.RenderControl(writer);
		m_hiddenZoomLevel.RenderControl(writer);
		if (m_autoRefreshTimer.Enabled)
		{
			m_autoRefreshTimer.RenderControl(writer);
		}
		if (m_reportStream == null)
		{
			return;
		}
		writer.AddStyleAttribute(HtmlTextWriterStyle.Display, "none");
		writer.RenderBeginTag(HtmlTextWriterTag.Div);
		int num = 40960;
		byte[] array = new byte[num];
		char[] array2 = new char[num];
		int num2 = 0;
		Decoder decoder = Encoding.UTF8.GetDecoder();
		int num3 = 0;
		bool flag = current?.IsInAsyncPostBack ?? false;
		while ((num2 = m_reportStream.Read(array, 0, num)) > 0)
		{
			int chars = decoder.GetChars(array, 0, num2, array2, 0, flush: false);
			if (flag)
			{
				RemoveNulls(array2, chars);
			}
			writer.Write(array2, 0, chars);
			num3 += num2;
			if (num3 >= num)
			{
				writer.Flush();
				num3 = 0;
			}
		}
		writer.RenderEndTag();
	}

	public void ClearReport()
	{
		if (m_reportStream != null)
		{
			m_scrollScript = null;
			m_reportStream.Dispose();
			m_reportStream = null;
			m_pageStyles = null;
		}
	}

	public void SetZoom(ZoomMode zoomMode, int zoomPercent)
	{
		EnsureChildControls();
		m_hiddenZoomLevel.Value = Global.ZoomString(zoomMode, zoomPercent);
	}

	private void OnZoomChanged(object sender, EventArgs e)
	{
		if (this.ZoomChanged != null)
		{
			ZoomChangeEventArgs e2 = null;
			string value = m_hiddenZoomLevel.Value;
			int result;
			if (string.Equals(value, ZoomMode.FullPage.ToString(), StringComparison.Ordinal))
			{
				e2 = new ZoomChangeEventArgs(ZoomMode.FullPage, 100);
			}
			else if (string.Equals(value, ZoomMode.PageWidth.ToString(), StringComparison.Ordinal))
			{
				e2 = new ZoomChangeEventArgs(ZoomMode.PageWidth, 100);
			}
			else if (int.TryParse(value, out result) && result > 0)
			{
				e2 = new ZoomChangeEventArgs(ZoomMode.Percent, result);
			}
			if (e2 != null)
			{
				this.ZoomChanged(this, e2);
			}
		}
	}

	public int RenderReport(ReportControlSession session, string viewerInstanceIdentifier, PageCountMode pageCountMode, int pageNumber, InteractivityPostBackMode interactivityMode, SearchState searchState, string replacementRoot, string hyperlinkTarget, ScrollTarget scrollTarget, string alertMessage, DeviceInfoCollection initialDeviceInfos, string browserMode, bool sizeToContent)
	{
		if (m_reportStream != null)
		{
			throw new InvalidOperationException();
		}
		m_session = session;
		ViewIteration++;
		m_pageNumber = pageNumber;
		m_searchState = searchState;
		m_viewerInstanceIdentifier = viewerInstanceIdentifier;
		m_alertMessage = alertMessage;
		m_scrollTarget = scrollTarget;
		m_interactivityMode = interactivityMode;
		bool useImageConsolidation = !sizeToContent;
		NameValueCollection deviceInfo = CreateDeviceInfo(initialDeviceInfos, session.Report, pageNumber, searchState, replacementRoot, hyperlinkTarget, browserMode, useImageConsolidation);
		m_reportStream = session.RenderReportHTML4(deviceInfo, pageCountMode, out m_scrollScript, out m_pageStyles);
		m_autoRefreshInterval = session.Report.AutoRefreshInterval;
		int totalPages = session.Report.GetTotalPages();
		if (m_pageNumber > totalPages)
		{
			m_pageNumber = totalPages;
		}
		m_autoRefreshInterval = session.Report.AutoRefreshInterval;
		if (sizeToContent)
		{
			m_scrollScript = null;
		}
		string styleStreamName = LocalHtmlRenderer.GetStyleStreamName(pageNumber);
		string mimeType;
		byte[] streamImage = session.GetStreamImage(styleStreamName, null, out mimeType);
		m_styleBytesString = null;
		if (streamImage != null && streamImage.Length > 0)
		{
			Encoding encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
			m_styleBytesString = encoding.GetString(streamImage);
		}
		return m_pageNumber;
	}

	internal static DeviceInfoNameBlackList GetDeviceInfoBlackList()
	{
		if (m_blackListDeviceInfoNames == null)
		{
			m_blackListDeviceInfoNames = new DeviceInfoNameBlackList();
			m_blackListDeviceInfoNames.Add("HTMLFragment");
			m_blackListDeviceInfoNames.Add("Section", Errors.InvalidDeviceInfoSection);
			m_blackListDeviceInfoNames.Add("StreamRoot");
			m_blackListDeviceInfoNames.Add("ResourceStreamRoot");
			m_blackListDeviceInfoNames.Add("ActionScript");
			m_blackListDeviceInfoNames.Add("JavaScript");
			m_blackListDeviceInfoNames.Add("FindString", Errors.InvalidDeviceInfoFind);
			m_blackListDeviceInfoNames.Add("ReplacementRoot");
			m_blackListDeviceInfoNames.Add("PrefixId");
			m_blackListDeviceInfoNames.Add("StyleStream");
			m_blackListDeviceInfoNames.Add("LinkTarget", Errors.InvalidDeviceInfoLinkTarget);
			m_blackListDeviceInfoNames.Add("ExpandContent");
		}
		return m_blackListDeviceInfoNames;
	}

	private NameValueCollection CreateDeviceInfo(DeviceInfoCollection initialDeviceInfos, Report report, int pageNumber, SearchState searchState, string replacementRoot, string linkTarget, string browserMode, bool useImageConsolidation)
	{
		NameValueCollection nameValueCollection = new NameValueCollection();
		foreach (DeviceInfo initialDeviceInfo in initialDeviceInfos)
		{
			nameValueCollection.Add(initialDeviceInfo.Name, initialDeviceInfo.Value);
		}
		nameValueCollection.Set("HTMLFragment", "true");
		nameValueCollection.Set("Section", pageNumber.ToString(CultureInfo.InvariantCulture));
		string value = ReportImageOperation.CreateUrl(report, m_viewerInstanceIdentifier, isResourceStreamRoot: false);
		nameValueCollection.Set("StreamRoot", value);
		string value2 = ReportImageOperation.CreateUrl(report, m_viewerInstanceIdentifier, isResourceStreamRoot: true);
		nameValueCollection.Set("ResourceStreamRoot", value2);
		nameValueCollection.Set("ActionScript", ActionScriptMethod);
		if (searchState != null)
		{
			nameValueCollection.Set("FindString", searchState.Text);
		}
		if (!string.IsNullOrEmpty(replacementRoot))
		{
			nameValueCollection.Set("ReplacementRoot", replacementRoot);
		}
		nameValueCollection.Set("PrefixId", UniqueRenderingId);
		nameValueCollection.Set("StyleStream", "true");
		if (!string.IsNullOrEmpty(linkTarget))
		{
			nameValueCollection.Set("LinkTarget", linkTarget);
		}
		if (HttpContext.Current != null && HttpContext.Current.Request != null)
		{
			string userAgent = HttpContext.Current.Request.UserAgent;
			if (userAgent != null)
			{
				nameValueCollection.Set("UserAgent", userAgent);
			}
		}
		if (!string.IsNullOrEmpty(browserMode) && nameValueCollection["BrowserMode"] == null)
		{
			nameValueCollection.Set("BrowserMode", browserMode);
		}
		if (!useImageConsolidation)
		{
			nameValueCollection.Set("ImageConsolidation", "false");
		}
		return nameValueCollection;
	}

	public void RaisePostBackEvent(string eventArgument)
	{
		OnReportAction(this, EventArgs.Empty);
	}

	private void OnReportAction(object sender, EventArgs e)
	{
		EnsureChildControls();
		string value = m_hiddenActionType.Value;
		string value2 = m_hiddenActionParam.Value;
		OnReportAction(value, value2);
	}

	private void OnAutoRefresh(object sender, EventArgs e)
	{
		OnReportAction("Refresh", "auto");
	}

	private void OnReportAction(string actionType, string actionParam)
	{
		if (this.ReportAction != null)
		{
			this.ReportAction(this, new ReportActionEventArgs(actionType, actionParam));
		}
	}

	private static string MakeUniqueRenderingId(string instanceId, int viewiteration)
	{
		return string.Format(CultureInfo.InvariantCulture, "P{0}_{1}_", instanceId, viewiteration);
	}

	public IEnumerable<ScriptDescriptor> GetScriptDescriptors()
	{
		ScriptControlDescriptor scriptControlDescriptor = new ScriptControlDescriptor("Microsoft.Reporting.WebFormsClient._ReportPage", ClientID);
		if (m_reportStream != null)
		{
			string uniqueRenderingId = UniqueRenderingId;
			scriptControlDescriptor.AddProperty("SearchHitPrefix", uniqueRenderingId + HTML4Renderer.m_searchHitIdPrefix);
			scriptControlDescriptor.AddProperty("ReportCellId", uniqueRenderingId + "oReportCell");
			scriptControlDescriptor.AddProperty("ReportDivId", uniqueRenderingId + "oReportDiv");
			scriptControlDescriptor.AddProperty("ScrollableContainerId", ScrollContainerId);
			if (m_alertMessage != null)
			{
				scriptControlDescriptor.AddProperty("LoadMessage", m_alertMessage);
			}
			if (m_scrollTarget != null)
			{
				string navigationId = m_scrollTarget.NavigationId;
				if (!string.IsNullOrEmpty(navigationId))
				{
					scriptControlDescriptor.AddProperty("NavigationId", uniqueRenderingId + navigationId);
				}
				if (m_scrollTarget.ScrollStyle == ActionScrollStyle.MaintainPosition)
				{
					string text = MakeUniqueRenderingId(m_viewerInstanceIdentifier, ViewIteration - 1);
					scriptControlDescriptor.AddProperty("PreviousViewNavigationAlignmentId", text + navigationId);
				}
				scriptControlDescriptor.AddProperty("AvoidScrollChange", m_scrollTarget.ScrollStyle == ActionScrollStyle.AvoidScrolling);
				scriptControlDescriptor.AddProperty("AvoidScrollFromOrigin", m_scrollTarget.ScrollStyle == ActionScrollStyle.AvoidScrollingFromOrigin);
				string pixelPosition = m_scrollTarget.PixelPosition;
				if (!string.IsNullOrEmpty(pixelPosition))
				{
					scriptControlDescriptor.AddProperty("SpecificScrollPosition", pixelPosition);
				}
			}
			scriptControlDescriptor.AddProperty("ReportStyles", m_styleBytesString);
			scriptControlDescriptor.AddProperty("PrefixId", uniqueRenderingId);
			if (m_scrollScript != null)
			{
				scriptControlDescriptor.AddScriptProperty("ScrollScript", m_scrollScript);
			}
			if (m_pageStyles != null)
			{
				scriptControlDescriptor.AddProperty("ReportPageStyles", m_pageStyles);
			}
			scriptControlDescriptor.AddProperty("InteractivityMode", m_interactivityMode.ToString());
			scriptControlDescriptor.AddProperty("ActionTypeId", m_hiddenActionType.ClientID);
			scriptControlDescriptor.AddProperty("ActionParamId", m_hiddenActionParam.ClientID);
			string postBackEventReference = Page.ClientScript.GetPostBackEventReference(m_syncPostBackTarget, null);
			scriptControlDescriptor.AddScriptProperty("TriggerSyncInteractivity", JavaScriptHelper.FormatAsFunction(postBackEventReference + ";"));
			string postBackEventReference2 = Page.ClientScript.GetPostBackEventReference(this, null);
			scriptControlDescriptor.AddScriptProperty("TriggerAsyncInteractivity", JavaScriptHelper.FormatAsFunction(postBackEventReference2 + ";"));
			string script = ToolbarControl.GenerateUpdateProperties(m_session, m_pageNumber, m_searchState);
			scriptControlDescriptor.AddScriptProperty("ToolBarUpdate", script);
		}
		scriptControlDescriptor.AddProperty("HiddenZoomLevelId", m_hiddenZoomLevel.ClientID);
		scriptControlDescriptor.AddProperty("StyleElementId", ClientID + "_styles");
		return new ScriptDescriptor[1] { scriptControlDescriptor };
	}

	public IEnumerable<ScriptReference> GetScriptReferences()
	{
		ScriptReference scriptReference = new ScriptReference();
		scriptReference.Path = EmbeddedResourceOperation.CreateUrlForScriptFile();
		return new ScriptReference[1] { scriptReference };
	}
}
