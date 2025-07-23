using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using Microsoft.Reporting.WebForms;
using Microsoft.ReportingServices.Interfaces;
using Microsoft.ReportingServices.Rendering.RPLProcessing;
using Microsoft.ReportingServices.Rendering.SPBProcessing;

namespace Microsoft.ReportingServices.Rendering.HtmlRenderer;

internal class ViewerRenderer : HTML4Renderer
{
	private sealed class DetachedReportWrapper : IReportWrapper
	{
		private string m_StreamRoot;

		private string m_ReportLocation;

		private bool m_HasBookmarks;

		private string m_SortItem;

		private string m_ShowHideToggle;

		private Encoding m_encoding = Encoding.UTF8;

		private Dictionary<string, byte[]> m_imageMap = new Dictionary<string, byte[]>();

		public bool HasBookmarks => m_HasBookmarks;

		public string SortItem => m_SortItem;

		public string ShowHideToggle => m_ShowHideToggle;

		public DetachedReportWrapper(string aStreamRoot)
		{
			m_StreamRoot = aStreamRoot;
		}

		public string GetStreamUrl(bool useSessionId, string streamName)
		{
			if (m_StreamRoot != null && m_StreamRoot != string.Empty)
			{
				StringBuilder stringBuilder = new StringBuilder(m_StreamRoot);
				if (streamName != null)
				{
					stringBuilder.Append(streamName);
				}
				return stringBuilder.ToString();
			}
			return null;
		}

		public string GetReportUrl(bool addParams)
		{
			return m_ReportLocation;
		}

		public byte[] GetImageName(string imageID)
		{
			if (m_imageMap.ContainsKey(imageID))
			{
				return m_imageMap[imageID];
			}
			byte[] bytes = m_encoding.GetBytes(imageID);
			m_imageMap[imageID] = bytes;
			return bytes;
		}
	}

	private sealed class SPBProcessingStub : ISPBProcessing
	{
		private const string DEVICE_INFO_TEMPLATE = "<DeviceInfo><StartPage>{0}</StartPage><EndPage>{1}</EndPage><ToggleItems>{2}</ToggleItems><MeasureItems>{3}</MeasureItems><SecondaryStreams>{4}</SecondaryStreams><StreamNames>{5}</StreamNames><StreamRoot>{6}</StreamRoot><RPLVersion>{7}</RPLVersion><ImageConsolidation>{8}</ImageConsolidation></DeviceInfo>";

		private ReportControlSession m_reportControlSession;

		private PageCountMode m_pageCountMode;

		private string m_streamRoot;

		private SPBContext m_spbContext = new SPBContext();

		public bool Done => true;

		internal SPBProcessingStub(ReportControlSession reportControlSession, string streamRoot, PageCountMode pageCountMode)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Expected O, but got Unknown
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			m_reportControlSession = reportControlSession;
			m_pageCountMode = pageCountMode;
			m_spbContext.StartPage = 1;
			m_spbContext.EndPage = 1;
			m_spbContext.MeasureItems = false;
			m_spbContext.AddToggledItems = false;
			m_spbContext.SecondaryStreams = (SecondaryStreams)1;
			m_spbContext.AddSecondaryStreamNames = true;
			m_spbContext.UseImageConsolidation = true;
			m_streamRoot = streamRoot;
		}

		public Stream GetNextPage(out RPLReport rplReport)
		{
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_0273: Unknown result type (might be due to invalid IL or missing references)
			//IL_0279: Expected O, but got Unknown
			string text = "10.6";
			string deviceInfo = string.Format(CultureInfo.InvariantCulture, "<DeviceInfo><StartPage>{0}</StartPage><EndPage>{1}</EndPage><ToggleItems>{2}</ToggleItems><MeasureItems>{3}</MeasureItems><SecondaryStreams>{4}</SecondaryStreams><StreamNames>{5}</StreamNames><StreamRoot>{6}</StreamRoot><RPLVersion>{7}</RPLVersion><ImageConsolidation>{8}</ImageConsolidation></DeviceInfo>", m_spbContext.StartPage, m_spbContext.EndPage, m_spbContext.AddToggledItems, m_spbContext.MeasureItems, ((object)m_spbContext.SecondaryStreams).ToString(), m_spbContext.AddSecondaryStreamNames, m_streamRoot, text, m_spbContext.UseImageConsolidation);
			NameValueCollection nameValueCollection = new NameValueCollection();
			nameValueCollection.Add("rs:PageCountMode", m_pageCountMode.ToString());
			string mimeType;
			string fileExtension;
			Stream stream = m_reportControlSession.RenderReport("RPL", allowInternalRenderers: true, deviceInfo, nameValueCollection, cacheSecondaryStreamsForHtml: true, out mimeType, out fileExtension);
			rplReport = null;
			if (stream == null || stream.Length <= 0)
			{
				stream?.Dispose();
				stream = null;
				int totalPages = m_reportControlSession.Report.GetTotalPages();
				if (totalPages < m_spbContext.EndPage)
				{
					m_spbContext.EndPage = totalPages;
					if (m_spbContext.StartPage > m_spbContext.EndPage)
					{
						m_spbContext.StartPage = m_spbContext.EndPage;
					}
					deviceInfo = string.Format(CultureInfo.InvariantCulture, "<DeviceInfo><StartPage>{0}</StartPage><EndPage>{1}</EndPage><ToggleItems>{2}</ToggleItems><MeasureItems>{3}</MeasureItems><SecondaryStreams>{4}</SecondaryStreams><StreamNames>{5}</StreamNames><StreamRoot>{6}</StreamRoot><RPLVersion>{7}</RPLVersion><ImageConsolidation>{8}</ImageConsolidation></DeviceInfo>", m_spbContext.StartPage, m_spbContext.EndPage, m_spbContext.AddToggledItems, m_spbContext.MeasureItems, ((object)m_spbContext.SecondaryStreams).ToString(), m_spbContext.AddSecondaryStreamNames, m_streamRoot, text, m_spbContext.UseImageConsolidation);
					stream = m_reportControlSession.RenderReport("RPL", allowInternalRenderers: true, deviceInfo, nameValueCollection, cacheSecondaryStreamsForHtml: true, out mimeType, out fileExtension);
				}
			}
			if (stream != null && stream.Length > 0)
			{
				BinaryReader binaryReader = new BinaryReader(stream, Encoding.Unicode);
				rplReport = new RPLReport(binaryReader);
			}
			return stream;
		}

		public void SetContext(SPBContext spbContext)
		{
			m_spbContext = spbContext;
		}
	}

	internal string PageStyle;

	private string m_fixedHeaderScript;

	internal string FixedHeaderScript => m_fixedHeaderScript;

	public ViewerRenderer(ReportControlSession reportControlSession, CreateAndRegisterStream streamCallback, ViewerRendererDeviceInfo deviceInfo, NameValueCollection browserCaps, SecondaryStreams secondaryStreams, PageCountMode pageCountMode)
		: base(new DetachedReportWrapper(deviceInfo.RawDeviceInfo["StreamRoot"] ?? ""), (ISPBProcessing)(object)new SPBProcessingStub(reportControlSession, HttpUtility.HtmlEncode(deviceInfo.RawDeviceInfo["StreamRoot"] ?? ""), pageCountMode), new NameValueCollection(), deviceInfo, deviceInfo.RawDeviceInfo, browserCaps, streamCallback, secondaryStreams)
	{
	}//IL_0054: Unknown result type (might be due to invalid IL or missing references)


	public override void Render(HtmlTextWriter outputWriter)
	{
		InitializeReport();
		m_encoding = outputWriter.Encoding;
		m_mainStream = Utility.CreateBufferedStream(outputWriter);
		string styleStreamName = GetStyleStreamName(m_pageNum);
		Stream sourceStream = CreateStyleStream(styleStreamName);
		m_styleStream = Utility.CreateBufferedStream(sourceStream);
		string text = m_deviceInfo.HtmlPrefixId + "oReportDiv";
		m_styleClassPrefix = m_encoding.GetBytes("#" + text + " ");
		RenderHtmlBody();
		RenderSecondaryStreamSpanTagsForJavascriptFunctions();
		m_mainStream.Flush();
		m_styleStream.Flush();
		m_fixedHeaderScript = GetFixedHeaderScripts();
		Stream mainStream = m_mainStream;
		m_mainStream = m_styleStream;
		PredefinedStyles();
		m_styleStream.Flush();
		m_mainStream = mainStream;
	}

	private Stream CreateStyleStream(string styleStreamName)
	{
		return CreateStream(styleStreamName, "css", Encoding.UTF8, "text/css", willSeek: false, StreamOper.CreateAndRegister);
	}

	public static string GetStyleStreamName(int pageNumber)
	{
		return HTML4Renderer.GetStyleStreamName("STYLESTREAM", pageNumber);
	}

	protected override void RenderInteractionAction(RPLAction action, ref bool hasHref)
	{
		RenderControlActionScript(action);
		WriteStream(HTML4Renderer.m_href);
		WriteStream(HTML4Renderer.m_quote);
		OpenStyle();
		WriteStream(HTML4Renderer.m_cursorHand);
		WriteStream(HTML4Renderer.m_semiColon);
		hasHref = true;
	}

	protected override void RenderSortAction(RPLTextBoxProps textBoxProps, SortOptions sortState)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Invalid comparison between Unknown and I4
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		WriteStream(HTML4Renderer.m_openStyle);
		WriteStream(HTML4Renderer.m_cursorHand);
		WriteStream(HTML4Renderer.m_semiColon);
		WriteStream(HTML4Renderer.m_quote);
		string uniqueName = ((RPLElementProps)textBoxProps).UniqueName;
		uniqueName = (((int)sortState != 2 && (int)sortState != 0) ? (uniqueName + "_D") : (uniqueName + "_A"));
		RenderOnClickActionScript("Sort", uniqueName);
		WriteStream(HTML4Renderer.m_closeBracket);
	}

	protected override void RenderInternalImageSrc()
	{
		WriteAttrEncoded(m_deviceInfo.ResourceStreamRoot);
	}

	protected override void RenderToggleImage(RPLTextBoxProps textBoxProps)
	{
		bool toggleState = textBoxProps.ToggleState;
		if (textBoxProps.IsToggleParent)
		{
			WriteStream(HTML4Renderer.m_openA);
			WriteStream(HTML4Renderer.m_tabIndex);
			WriteStream(++m_tabIndexNum);
			WriteStream(HTML4Renderer.m_quote);
			WriteStream(HTML4Renderer.m_openStyle);
			WriteStream(HTML4Renderer.m_cursorHand);
			WriteStream(HTML4Renderer.m_semiColon);
			WriteStream(HTML4Renderer.m_quote);
			RenderOnClickActionScript("Toggle", ((RPLElementProps)textBoxProps).UniqueName);
			WriteStream(HTML4Renderer.m_closeBracket);
			WriteStream(HTML4Renderer.m_img);
			if (m_browserIE)
			{
				WriteStream(HTML4Renderer.m_imgOnError);
			}
			WriteStream(HTML4Renderer.m_zeroBorder);
			WriteStream(HTML4Renderer.m_src);
			WriteToggleImage(toggleState);
			WriteStream(HTML4Renderer.m_quote);
			WriteStream(HTML4Renderer.m_alt);
			if (toggleState)
			{
				WriteStream(RenderRes.ToggleStateCollapse);
			}
			else
			{
				WriteStream(RenderRes.ToggleStateExpand);
			}
			WriteStream(HTML4Renderer.m_closeTag);
			WriteStream(HTML4Renderer.m_closeA);
			WriteStream(HTML4Renderer.m_nbsp);
		}
	}

	private void WriteToggleImage(bool toggleState)
	{
		if (toggleState)
		{
			WriteStream(EmbeddedResourceOperation.CreateUrl("Microsoft.ReportingServices.Rendering.HtmlRenderer.RendererResources.ToggleMinus.gif"));
		}
		else
		{
			WriteStream(EmbeddedResourceOperation.CreateUrl("Microsoft.ReportingServices.Rendering.HtmlRenderer.RendererResources.TogglePlus.gif"));
		}
	}

	protected override void RenderSortImageText(SortOptions sortState)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Invalid comparison between Unknown and I4
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Invalid comparison between Unknown and I4
		if ((int)sortState == 1)
		{
			WriteStream(EmbeddedResourceOperation.CreateUrl("Microsoft.ReportingServices.Rendering.HtmlRenderer.RendererResources.sortAsc.gif"));
		}
		else if ((int)sortState == 2)
		{
			WriteStream(EmbeddedResourceOperation.CreateUrl("Microsoft.ReportingServices.Rendering.HtmlRenderer.RendererResources.sortDesc.gif"));
		}
		else
		{
			WriteStream(EmbeddedResourceOperation.CreateUrl("Microsoft.ReportingServices.Rendering.HtmlRenderer.RendererResources.unsorted.gif"));
		}
	}

	protected override void WriteScrollbars()
	{
	}

	protected override void WriteFixedHeaderOnScrollScript()
	{
	}

	protected override void WriteFixedHeaderPropertyChangeScript()
	{
	}

	protected override void WriteFitProportionalScript(double pv, double ph)
	{
		WriteStream(HTML4Renderer.m_onLoadFitProportionalPv);
		WriteStream(Utility.MmToPxAsString(pv));
		WriteStream(";this.ph=");
		WriteStream(Utility.MmToPxAsString(ph));
		WriteStream(";\"");
	}

	protected override void RenderPageStart(bool firstPage, bool lastPage, RPLElementStyle pageStyle)
	{
		WriteStream(HTML4Renderer.m_openDiv);
		WriteStream(HTML4Renderer.m_ltrDir);
		WriteStream(HTML4Renderer.m_openStyle);
		if (m_deviceInfo.IsBrowserIE)
		{
			WriteStream(HTML4Renderer.m_styleHeight);
			WriteStream(HTML4Renderer.m_percent);
			WriteStream(HTML4Renderer.m_semiColon);
		}
		WriteStream(HTML4Renderer.m_styleWidth);
		WriteStream(HTML4Renderer.m_percent);
		WriteStream(HTML4Renderer.m_semiColon);
		WriteStream("direction:ltr");
		WriteStream(HTML4Renderer.m_quote);
		RenderReportItemId("oReportDiv");
		if (m_pageHasStyle)
		{
			Stream mainStream = m_mainStream;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				m_mainStream = memoryStream;
				RenderBackgroundStyleProps((IRPLStyle)(object)pageStyle);
				int borderContext = 0;
				RenderHtmlBorders((IRPLStyle)(object)pageStyle, ref borderContext, 0, renderPadding: true, isNonShared: true, null);
				Encoding encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
				PageStyle = encoding.GetString(memoryStream.ToArray());
			}
			m_mainStream = mainStream;
			mainStream = null;
		}
		else
		{
			PageStyle = null;
		}
		WriteStream(HTML4Renderer.m_closeBracket);
		WriteStream(HTML4Renderer.m_openTable);
		WriteStream(HTML4Renderer.m_closeBracket);
		WriteStream(HTML4Renderer.m_firstTD);
		RenderReportItemId("oReportCell");
		RenderZoom();
		WriteStream(HTML4Renderer.m_closeBracket);
	}

	protected override void RenderPageEnd()
	{
		WriteStream(HTML4Renderer.m_lastTD);
		WriteStream(HTML4Renderer.m_closeTable);
		WriteStream(HTML4Renderer.m_closeDiv);
	}

	private string GetFixedHeaderScripts()
	{
		if (m_fixedHeaders == null || m_fixedHeaders.Count == 0 || !m_hasOnePage)
		{
			return null;
		}
		StringBuilder stringBuilder = new StringBuilder();
		StringBuilder stringBuilder2 = new StringBuilder();
		RenderCreateFixedHeaderFunction("this.", "this.m_fixedHeader", stringBuilder2, stringBuilder, createHeadersWithArray: true);
		StringBuilder stringBuilder3 = new StringBuilder();
		stringBuilder3.Append("function(firstTime) {");
		stringBuilder3.Append("if(firstTime){");
		stringBuilder3.Append(stringBuilder);
		stringBuilder3.Append("}");
		stringBuilder3.Append(stringBuilder2);
		stringBuilder3.Append("}");
		return stringBuilder3.ToString();
	}
}
