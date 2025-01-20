
using Microsoft.Reporting.WebForms;
using Microsoft.ReportingServices.Interfaces;
using Microsoft.ReportingServices.Rendering.RPLProcessing;
using Microsoft.ReportingServices.Rendering.SPBProcessing;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;

#nullable disable
namespace Microsoft.ReportingServices.Rendering.HtmlRenderer
{
  internal class ViewerRenderer : HTML4Renderer
  {
    internal string PageStyle;
    private string m_fixedHeaderScript;

    public ViewerRenderer(
      ReportControlSession reportControlSession,
      CreateAndRegisterStream streamCallback,
      ViewerRendererDeviceInfo deviceInfo,
      NameValueCollection browserCaps,
      SecondaryStreams secondaryStreams,
      PageCountMode pageCountMode)
      : base((IReportWrapper) new ViewerRenderer.DetachedReportWrapper(deviceInfo.RawDeviceInfo["StreamRoot"] ?? ""), (ISPBProcessing) new ViewerRenderer.SPBProcessingStub(reportControlSession, HttpUtility.HtmlEncode(deviceInfo.RawDeviceInfo["StreamRoot"] ?? ""), pageCountMode), new NameValueCollection(), (DeviceInfo) deviceInfo, deviceInfo.RawDeviceInfo, browserCaps, streamCallback, secondaryStreams)
    {
    }

    public override void Render(HtmlTextWriter outputWriter)
    {
      this.InitializeReport();
      this.m_encoding = outputWriter.Encoding;
      this.m_mainStream = (Stream) Utility.CreateBufferedStream(outputWriter);
      this.m_styleStream = (Stream) Utility.CreateBufferedStream(this.CreateStyleStream(ViewerRenderer.GetStyleStreamName(this.m_pageNum)));
      this.m_styleClassPrefix = this.m_encoding.GetBytes("#" + (this.m_deviceInfo.HtmlPrefixId + "oReportDiv") + " ");
      this.RenderHtmlBody();
      this.RenderSecondaryStreamSpanTagsForJavascriptFunctions();
      this.m_mainStream.Flush();
      this.m_styleStream.Flush();
      this.m_fixedHeaderScript = this.GetFixedHeaderScripts();
      Stream mainStream = this.m_mainStream;
      this.m_mainStream = this.m_styleStream;
      this.PredefinedStyles();
      this.m_styleStream.Flush();
      this.m_mainStream = mainStream;
    }

    private Stream CreateStyleStream(string styleStreamName)
    {
      return this.CreateStream(styleStreamName, "css", Encoding.UTF8, "text/css", false, StreamOper.CreateAndRegister);
    }

    public static string GetStyleStreamName(int pageNumber)
    {
      return HTML4Renderer.GetStyleStreamName("STYLESTREAM", pageNumber);
    }

    protected override void RenderInteractionAction(RPLAction action, ref bool hasHref)
    {
      this.RenderControlActionScript(action);
      this.WriteStream(HTML4Renderer.m_href);
      this.WriteStream(HTML4Renderer.m_quote);
      this.OpenStyle();
      this.WriteStream(HTML4Renderer.m_cursorHand);
      this.WriteStream(HTML4Renderer.m_semiColon);
      hasHref = true;
    }

    protected override void RenderSortAction(
      RPLTextBoxProps textBoxProps,
      RPLFormat.SortOptions sortState)
    {
      this.WriteStream(HTML4Renderer.m_openStyle);
      this.WriteStream(HTML4Renderer.m_cursorHand);
      this.WriteStream(HTML4Renderer.m_semiColon);
      this.WriteStream(HTML4Renderer.m_quote);
      string uniqueName = ((RPLElementProps) textBoxProps).UniqueName;
      this.RenderOnClickActionScript("Sort", sortState == 2 || sortState == null ? uniqueName + "_A" : uniqueName + "_D");
      this.WriteStream(HTML4Renderer.m_closeBracket);
    }

    protected override void RenderInternalImageSrc()
    {
      this.WriteAttrEncoded(this.m_deviceInfo.ResourceStreamRoot);
    }

    protected override void RenderToggleImage(RPLTextBoxProps textBoxProps)
    {
      bool toggleState = textBoxProps.ToggleState;
      if (!textBoxProps.IsToggleParent)
        return;
      this.WriteStream(HTML4Renderer.m_openA);
      this.WriteStream(HTML4Renderer.m_tabIndex);
      this.WriteStream((object) ++this.m_tabIndexNum);
      this.WriteStream(HTML4Renderer.m_quote);
      this.WriteStream(HTML4Renderer.m_openStyle);
      this.WriteStream(HTML4Renderer.m_cursorHand);
      this.WriteStream(HTML4Renderer.m_semiColon);
      this.WriteStream(HTML4Renderer.m_quote);
      this.RenderOnClickActionScript("Toggle", ((RPLElementProps) textBoxProps).UniqueName);
      this.WriteStream(HTML4Renderer.m_closeBracket);
      this.WriteStream(HTML4Renderer.m_img);
      if (this.m_browserIE)
        this.WriteStream(HTML4Renderer.m_imgOnError);
      this.WriteStream(HTML4Renderer.m_zeroBorder);
      this.WriteStream(HTML4Renderer.m_src);
      this.WriteToggleImage(toggleState);
      this.WriteStream(HTML4Renderer.m_quote);
      this.WriteStream(HTML4Renderer.m_alt);
      if (toggleState)
        this.WriteStream(RenderRes.ToggleStateCollapse);
      else
        this.WriteStream(RenderRes.ToggleStateExpand);
      this.WriteStream(HTML4Renderer.m_closeTag);
      this.WriteStream(HTML4Renderer.m_closeA);
      this.WriteStream(HTML4Renderer.m_nbsp);
    }

    private void WriteToggleImage(bool toggleState)
    {
      if (toggleState)
        this.WriteStream(EmbeddedResourceOperation.CreateUrl("Microsoft.ReportingServices.Rendering.HtmlRenderer.RendererResources.ToggleMinus.gif"));
      else
        this.WriteStream(EmbeddedResourceOperation.CreateUrl("Microsoft.ReportingServices.Rendering.HtmlRenderer.RendererResources.TogglePlus.gif"));
    }

    protected override void RenderSortImageText(RPLFormat.SortOptions sortState)
    {
      if (sortState == 1)
        this.WriteStream(EmbeddedResourceOperation.CreateUrl("Microsoft.ReportingServices.Rendering.HtmlRenderer.RendererResources.sortAsc.gif"));
      else if (sortState == 2)
        this.WriteStream(EmbeddedResourceOperation.CreateUrl("Microsoft.ReportingServices.Rendering.HtmlRenderer.RendererResources.sortDesc.gif"));
      else
        this.WriteStream(EmbeddedResourceOperation.CreateUrl("Microsoft.ReportingServices.Rendering.HtmlRenderer.RendererResources.unsorted.gif"));
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
      this.WriteStream(HTML4Renderer.m_onLoadFitProportionalPv);
      this.WriteStream(Utility.MmToPxAsString(pv));
      this.WriteStream(";this.ph=");
      this.WriteStream(Utility.MmToPxAsString(ph));
      this.WriteStream(";\"");
    }

    protected override void RenderPageStart(
      bool firstPage,
      bool lastPage,
      RPLElementStyle pageStyle)
    {
      this.WriteStream(HTML4Renderer.m_openDiv);
      this.WriteStream(HTML4Renderer.m_ltrDir);
      this.WriteStream(HTML4Renderer.m_openStyle);
      if (this.m_deviceInfo.IsBrowserIE)
      {
        this.WriteStream(HTML4Renderer.m_styleHeight);
        this.WriteStream(HTML4Renderer.m_percent);
        this.WriteStream(HTML4Renderer.m_semiColon);
      }
      this.WriteStream(HTML4Renderer.m_styleWidth);
      this.WriteStream(HTML4Renderer.m_percent);
      this.WriteStream(HTML4Renderer.m_semiColon);
      this.WriteStream("direction:ltr");
      this.WriteStream(HTML4Renderer.m_quote);
      this.RenderReportItemId("oReportDiv");
      if (this.m_pageHasStyle)
      {
        Stream mainStream = this.m_mainStream;
        using (MemoryStream memoryStream = new MemoryStream())
        {
          this.m_mainStream = (Stream) memoryStream;
          this.RenderBackgroundStyleProps((IRPLStyle) pageStyle);
          int borderContext = 0;
          this.RenderHtmlBorders((IRPLStyle) pageStyle, ref borderContext, (byte) 0, true, true, (IRPLStyle) null);
          this.PageStyle = new UTF8Encoding(false).GetString(memoryStream.ToArray());
        }
        this.m_mainStream = mainStream;
      }
      else
        this.PageStyle = (string) null;
      this.WriteStream(HTML4Renderer.m_closeBracket);
      this.WriteStream(HTML4Renderer.m_openTable);
      this.WriteStream(HTML4Renderer.m_closeBracket);
      this.WriteStream(HTML4Renderer.m_firstTD);
      this.RenderReportItemId("oReportCell");
      this.RenderZoom();
      this.WriteStream(HTML4Renderer.m_closeBracket);
    }

    protected override void RenderPageEnd()
    {
      this.WriteStream(HTML4Renderer.m_lastTD);
      this.WriteStream(HTML4Renderer.m_closeTable);
      this.WriteStream(HTML4Renderer.m_closeDiv);
    }

    private string GetFixedHeaderScripts()
    {
      if (this.m_fixedHeaders == null || this.m_fixedHeaders.Count == 0 || !this.m_hasOnePage)
        return (string) null;
      StringBuilder arrayBuilder = new StringBuilder();
      StringBuilder function = new StringBuilder();
      this.RenderCreateFixedHeaderFunction("this.", "this.m_fixedHeader", function, arrayBuilder, true);
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("function(firstTime) {");
      stringBuilder.Append("if(firstTime){");
      stringBuilder.Append((object) arrayBuilder);
      stringBuilder.Append("}");
      stringBuilder.Append((object) function);
      stringBuilder.Append("}");
      return stringBuilder.ToString();
    }

    internal string FixedHeaderScript => this.m_fixedHeaderScript;

    private sealed class DetachedReportWrapper : IReportWrapper
    {
      private string m_StreamRoot;
      private string m_ReportLocation;
      private bool m_HasBookmarks;
      private string m_SortItem;
      private string m_ShowHideToggle;
      private Encoding m_encoding = Encoding.UTF8;
      private Dictionary<string, byte[]> m_imageMap = new Dictionary<string, byte[]>();

      public DetachedReportWrapper(string aStreamRoot) => this.m_StreamRoot = aStreamRoot;

      public string GetStreamUrl(bool useSessionId, string streamName)
      {
        if (this.m_StreamRoot == null || !(this.m_StreamRoot != string.Empty))
          return (string) null;
        StringBuilder stringBuilder = new StringBuilder(this.m_StreamRoot);
        if (streamName != null)
          stringBuilder.Append(streamName);
        return stringBuilder.ToString();
      }

      public bool HasBookmarks => this.m_HasBookmarks;

      public string SortItem => this.m_SortItem;

      public string ShowHideToggle => this.m_ShowHideToggle;

      public string GetReportUrl(bool addParams) => this.m_ReportLocation;

      public byte[] GetImageName(string imageID)
      {
        if (this.m_imageMap.ContainsKey(imageID))
          return this.m_imageMap[imageID];
        byte[] bytes = this.m_encoding.GetBytes(imageID);
        this.m_imageMap[imageID] = bytes;
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

      internal SPBProcessingStub(
        ReportControlSession reportControlSession,
        string streamRoot,
        PageCountMode pageCountMode)
      {
        this.m_reportControlSession = reportControlSession;
        this.m_pageCountMode = pageCountMode;
        this.m_spbContext.StartPage = 1;
        this.m_spbContext.EndPage = 1;
        this.m_spbContext.MeasureItems = false;
        this.m_spbContext.AddToggledItems = false;
        this.m_spbContext.SecondaryStreams = (SecondaryStreams) 1;
        this.m_spbContext.AddSecondaryStreamNames = true;
        this.m_spbContext.UseImageConsolidation = true;
        this.m_streamRoot = streamRoot;
      }

      public Stream GetNextPage(out RPLReport rplReport)
      {
        string str = "10.6";
        string deviceInfo = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "<DeviceInfo><StartPage>{0}</StartPage><EndPage>{1}</EndPage><ToggleItems>{2}</ToggleItems><MeasureItems>{3}</MeasureItems><SecondaryStreams>{4}</SecondaryStreams><StreamNames>{5}</StreamNames><StreamRoot>{6}</StreamRoot><RPLVersion>{7}</RPLVersion><ImageConsolidation>{8}</ImageConsolidation></DeviceInfo>", (object) this.m_spbContext.StartPage, (object) this.m_spbContext.EndPage, (object) this.m_spbContext.AddToggledItems, (object) this.m_spbContext.MeasureItems, (object) this.m_spbContext.SecondaryStreams.ToString(), (object) this.m_spbContext.AddSecondaryStreamNames, (object) this.m_streamRoot, (object) str, (object) this.m_spbContext.UseImageConsolidation);
        NameValueCollection additionalParams = new NameValueCollection();
        additionalParams.Add("rs:PageCountMode", this.m_pageCountMode.ToString());
        string mimeType;
        string fileExtension;
        Stream input = this.m_reportControlSession.RenderReport("RPL", true, deviceInfo, additionalParams, true, out mimeType, out fileExtension);
        rplReport = (RPLReport) null;
        if (input == null || input.Length <= 0L)
        {
          input?.Dispose();
          input = (Stream) null;
          int totalPages = this.m_reportControlSession.Report.GetTotalPages();
          if (totalPages < this.m_spbContext.EndPage)
          {
            this.m_spbContext.EndPage = totalPages;
            if (this.m_spbContext.StartPage > this.m_spbContext.EndPage)
              this.m_spbContext.StartPage = this.m_spbContext.EndPage;
            input = this.m_reportControlSession.RenderReport("RPL", true, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "<DeviceInfo><StartPage>{0}</StartPage><EndPage>{1}</EndPage><ToggleItems>{2}</ToggleItems><MeasureItems>{3}</MeasureItems><SecondaryStreams>{4}</SecondaryStreams><StreamNames>{5}</StreamNames><StreamRoot>{6}</StreamRoot><RPLVersion>{7}</RPLVersion><ImageConsolidation>{8}</ImageConsolidation></DeviceInfo>", (object) this.m_spbContext.StartPage, (object) this.m_spbContext.EndPage, (object) this.m_spbContext.AddToggledItems, (object) this.m_spbContext.MeasureItems, (object) this.m_spbContext.SecondaryStreams.ToString(), (object) this.m_spbContext.AddSecondaryStreamNames, (object) this.m_streamRoot, (object) str, (object) this.m_spbContext.UseImageConsolidation), additionalParams, true, out mimeType, out fileExtension);
          }
        }
        if (input != null && input.Length > 0L)
        {
          BinaryReader binaryReader = new BinaryReader(input, Encoding.Unicode);
          rplReport = new RPLReport(binaryReader);
        }
        return input;
      }

      public void SetContext(SPBContext spbContext) => this.m_spbContext = spbContext;

      public bool Done => true;
    }
  }
}
