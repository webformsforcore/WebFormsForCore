
using Microsoft.ReportingServices.Interfaces;
using Microsoft.ReportingServices.Rendering.RPLProcessing;
using Microsoft.ReportingServices.Rendering.SPBProcessing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Security;
using System.Text;
using System.Web;
using System.Web.UI;

#nullable disable
namespace Microsoft.ReportingServices.Rendering.HtmlRenderer
{
  internal abstract class HTML4Renderer : HTMLWriter
  {
    private const float MaxWordSize = 558.8f;
    private const string FixedRowMarker = "r";
    private const string FixedColMarker = "c";
    private const string EmptyColMarker = "e";
    private const string EmptyHeightColMarker = "h";
    internal const string FixedRowGroupHeaderPrefix = "frgh";
    internal const string FixedCornerHeaderPrefix = "fch";
    internal const string FixedColGroupHeaderPrefix = "fcgh";
    internal const string FixedRGHArrayPrefix = "frhArr";
    internal const string FixedCGHArrayPrefix = "fcghArr";
    internal const string FixedCHArrayPrefix = "fchArr";
    internal const string ReportDiv = "oReportDiv";
    private const char Space = ' ';
    private const char Comma = ',';
    private const string MSuffix = "_m";
    private const string SSuffix = "_s";
    private const string ASuffix = "_a";
    private const string PSuffix = "_p";
    private const string FitVertTextSuffix = "_fvt";
    private const string GrowRectanglesSuffix = "_gr";
    private const string ImageConImageSuffix = "_ici";
    private const string ImageFitDivSuffix = "_ifd";
    private const long FitProptionalDefaultSize = 5;
    protected const int SecondaryStreamBufferSize = 4096;
    internal const string SortAction = "Sort";
    internal const string ToggleAction = "Toggle";
    internal const string DrillthroughAction = "Drillthrough";
    internal const string BookmarkAction = "Bookmark";
    internal const string GetImageKey = "GetImage";
    internal const string SectionKey = "Section";
    internal const string PrefixIdKey = "PrefixId";
    internal const int IgnoreLeft = 1;
    internal const int IgnoreRight = 2;
    internal const int IgnoreTop = 4;
    internal const int IgnoreBottom = 8;
    internal const int IgnoreAll = 15;
    internal const char StreamNameSeparator = '_';
    internal const string PageStyleName = "p";
    internal const string MHTMLPrefix = "cid:";
    internal const string CSSSuffix = "style";
    protected const string m_resourceNamespace = "Microsoft.ReportingServices.Rendering.HtmlRenderer.RendererResources";
    internal static byte[] m_overflowXHidden = (byte[]) null;
    internal static byte[] m_percentWidthOverflow = (byte[]) null;
    internal static byte[] m_layoutFixed = (byte[]) null;
    internal static byte[] m_layoutBorder = (byte[]) null;
    internal static byte[] m_ignoreBorder = (byte[]) null;
    internal static byte[] m_ignoreBorderL = (byte[]) null;
    internal static byte[] m_ignoreBorderR = (byte[]) null;
    internal static byte[] m_ignoreBorderT = (byte[]) null;
    internal static byte[] m_ignoreBorderB = (byte[]) null;
    internal static byte[] m_percentHeight = (byte[]) null;
    internal static byte[] m_percentSizesOverflow = (byte[]) null;
    internal static byte[] m_percentSizes = (byte[]) null;
    internal static byte[] m_space = (byte[]) null;
    internal static byte[] m_closeBracket = (byte[]) null;
    internal static byte[] m_semiColon = (byte[]) null;
    internal static byte[] m_border = (byte[]) null;
    internal static byte[] m_borderBottom = (byte[]) null;
    internal static byte[] m_borderLeft = (byte[]) null;
    internal static byte[] m_borderRight = (byte[]) null;
    internal static byte[] m_borderTop = (byte[]) null;
    internal static byte[] m_marginBottom = (byte[]) null;
    internal static byte[] m_marginLeft = (byte[]) null;
    internal static byte[] m_marginRight = (byte[]) null;
    internal static byte[] m_marginTop = (byte[]) null;
    internal static byte[] m_textIndent = (byte[]) null;
    internal static byte[] m_mm = (byte[]) null;
    internal static byte[] m_styleWidth = (byte[]) null;
    internal static byte[] m_styleHeight = (byte[]) null;
    internal static byte[] m_percent = (byte[]) null;
    internal static byte[] m_ninetyninepercent = (byte[]) null;
    internal static byte[] m_degree90 = (byte[]) null;
    internal static byte[] m_newLine = (byte[]) null;
    internal static byte[] m_closeAccol = (byte[]) null;
    internal static byte[] m_backgroundRepeat = (byte[]) null;
    internal static byte[] m_closeBrace = (byte[]) null;
    internal static byte[] m_backgroundColor = (byte[]) null;
    internal static byte[] m_backgroundImage = (byte[]) null;
    internal static byte[] m_overflowHidden = (byte[]) null;
    internal static byte[] m_wordWrap = (byte[]) null;
    internal static byte[] m_whiteSpacePreWrap = (byte[]) null;
    internal static byte[] m_leftValue = (byte[]) null;
    internal static byte[] m_rightValue = (byte[]) null;
    internal static byte[] m_centerValue = (byte[]) null;
    internal static byte[] m_textAlign = (byte[]) null;
    internal static byte[] m_verticalAlign = (byte[]) null;
    internal static byte[] m_lineHeight = (byte[]) null;
    internal static byte[] m_color = (byte[]) null;
    internal static byte[] m_writingMode = (byte[]) null;
    internal static byte[] m_tbrl = (byte[]) null;
    internal static byte[] m_btrl = (byte[]) null;
    internal static byte[] m_lrtb = (byte[]) null;
    internal static byte[] m_rltb = (byte[]) null;
    internal static byte[] m_layoutFlow = (byte[]) null;
    internal static byte[] m_verticalIdeographic = (byte[]) null;
    internal static byte[] m_horizontal = (byte[]) null;
    internal static byte[] m_unicodeBiDi = (byte[]) null;
    internal static byte[] m_direction = (byte[]) null;
    internal static byte[] m_textDecoration = (byte[]) null;
    internal static byte[] m_fontWeight = (byte[]) null;
    internal static byte[] m_fontSize = (byte[]) null;
    internal static byte[] m_fontFamily = (byte[]) null;
    internal static byte[] m_fontStyle = (byte[]) null;
    internal static byte[] m_openAccol = (byte[]) null;
    internal static byte[] m_borderColor = (byte[]) null;
    internal static byte[] m_borderStyle = (byte[]) null;
    internal static byte[] m_borderWidth = (byte[]) null;
    internal static byte[] m_borderBottomColor = (byte[]) null;
    internal static byte[] m_borderBottomStyle = (byte[]) null;
    internal static byte[] m_borderBottomWidth = (byte[]) null;
    internal static byte[] m_borderLeftColor = (byte[]) null;
    internal static byte[] m_borderLeftStyle = (byte[]) null;
    internal static byte[] m_borderLeftWidth = (byte[]) null;
    internal static byte[] m_borderRightColor = (byte[]) null;
    internal static byte[] m_borderRightStyle = (byte[]) null;
    internal static byte[] m_borderRightWidth = (byte[]) null;
    internal static byte[] m_borderTopColor = (byte[]) null;
    internal static byte[] m_borderTopStyle = (byte[]) null;
    internal static byte[] m_borderTopWidth = (byte[]) null;
    internal static byte[] m_paddingBottom = (byte[]) null;
    internal static byte[] m_paddingLeft = (byte[]) null;
    internal static byte[] m_paddingRight = (byte[]) null;
    internal static byte[] m_paddingTop = (byte[]) null;
    protected static byte[] m_classAction = (byte[]) null;
    internal static byte[] m_styleAction = (byte[]) null;
    internal static byte[] m_emptyTextBox = (byte[]) null;
    internal static byte[] m_percentSizeInlineTable = (byte[]) null;
    internal static byte[] m_classPercentSizeInlineTable = (byte[]) null;
    internal static byte[] m_percentHeightInlineTable = (byte[]) null;
    internal static byte[] m_classPercentHeightInlineTable = (byte[]) null;
    internal static byte[] m_dot = (byte[]) null;
    internal static byte[] m_popupAction = (byte[]) null;
    internal static byte[] m_tableLayoutFixed = (byte[]) null;
    internal static byte[] m_borderCollapse = (byte[]) null;
    internal static byte[] m_none = (byte[]) null;
    internal static byte[] m_displayNone = (byte[]) null;
    internal static byte[] m_rtlEmbed = (byte[]) null;
    internal static byte[] m_classRtlEmbed = (byte[]) null;
    internal static byte[] m_noVerticalMarginClassName = (byte[]) null;
    internal static byte[] m_classNoVerticalMargin = (byte[]) null;
    internal static byte[] m_zeroPoint = (byte[]) null;
    internal static byte[] m_smallPoint = (byte[]) null;
    internal static byte[] m_filter = (byte[]) null;
    internal static byte[] m_basicImageRotation180 = (byte[]) null;
    internal static byte[] m_msoRotation = (byte[]) null;
    internal static byte[] m_styleMinWidth = (byte[]) null;
    internal static byte[] m_styleMinHeight = (byte[]) null;
    private static byte[] m_styleDisplayInlineBlock = (byte[]) null;
    internal static byte[] m_closeUL;
    internal static byte[] m_closeOL;
    internal static byte[] m_olArabic;
    internal static byte[] m_olRoman;
    internal static byte[] m_olAlpha;
    internal static byte[] m_ulCircle;
    internal static byte[] m_ulDisc;
    internal static byte[] m_ulSquare;
    protected static byte[] m_br = (byte[]) null;
    protected static byte[] m_tabIndex = (byte[]) null;
    protected static byte[] m_closeTable = (byte[]) null;
    protected static byte[] m_openTable = (byte[]) null;
    protected static byte[] m_closeDiv = (byte[]) null;
    protected static byte[] m_openDiv = (byte[]) null;
    protected static byte[] m_zeroBorder = (byte[]) null;
    protected static byte[] m_cols = (byte[]) null;
    protected static byte[] m_colSpan = (byte[]) null;
    protected static byte[] m_rowSpan = (byte[]) null;
    protected static byte[] m_headers = (byte[]) null;
    protected static byte[] m_closeTD = (byte[]) null;
    protected static byte[] m_closeTR = (byte[]) null;
    protected static byte[] m_firstTD = (byte[]) null;
    protected static byte[] m_lastTD = (byte[]) null;
    protected static byte[] m_openTD = (byte[]) null;
    protected static byte[] m_openTR = (byte[]) null;
    protected static byte[] m_valign = (byte[]) null;
    protected static byte[] m_closeQuote = (byte[]) null;
    internal static string m_closeQuoteString = "\">";
    protected static byte[] m_closeSpan = (byte[]) null;
    protected static byte[] m_openSpan = (byte[]) null;
    protected static byte[] m_quote = (byte[]) null;
    internal static string m_quoteString = "\"";
    protected static byte[] m_closeTag = (byte[]) null;
    protected static byte[] m_id = (byte[]) null;
    protected static byte[] m_px = (byte[]) null;
    protected static byte[] m_zeroWidth = (byte[]) null;
    protected static byte[] m_zeroHeight = (byte[]) null;
    protected static byte[] m_openHtml = (byte[]) null;
    protected static byte[] m_closeHtml = (byte[]) null;
    protected static byte[] m_openBody = (byte[]) null;
    protected static byte[] m_closeBody = (byte[]) null;
    protected static byte[] m_openHead = (byte[]) null;
    protected static byte[] m_closeHead = (byte[]) null;
    protected static byte[] m_openTitle = (byte[]) null;
    protected static byte[] m_closeTitle = (byte[]) null;
    protected static byte[] m_openA = (byte[]) null;
    protected static byte[] m_target = (byte[]) null;
    protected static byte[] m_closeA = (byte[]) null;
    protected static string m_hrefString = " href=\"";
    protected static byte[] m_href = (byte[]) null;
    protected static byte[] m_nohref = (byte[]) null;
    protected static byte[] m_inlineHeight = (byte[]) null;
    protected static byte[] m_inlineWidth = (byte[]) null;
    protected static byte[] m_img = (byte[]) null;
    protected static byte[] m_imgOnError = (byte[]) null;
    protected static byte[] m_src = (byte[]) null;
    protected static byte[] m_topValue = (byte[]) null;
    protected static byte[] m_alt = (byte[]) null;
    protected static byte[] m_title = (byte[]) null;
    protected static byte[] m_classID = (byte[]) null;
    protected static byte[] m_codeBase = (byte[]) null;
    protected static byte[] m_valueObject = (byte[]) null;
    protected static byte[] m_paramObject = (byte[]) null;
    protected static byte[] m_openObject = (byte[]) null;
    protected static byte[] m_closeObject = (byte[]) null;
    protected static byte[] m_equal = (byte[]) null;
    protected static byte[] m_encodedAmp = (byte[]) null;
    protected static byte[] m_nbsp = (byte[]) null;
    protected static byte[] m_questionMark = (byte[]) null;
    protected static byte[] m_checked = (byte[]) null;
    protected static byte[] m_checkForEnterKey = (byte[]) null;
    protected static byte[] m_unchecked = (byte[]) null;
    protected static byte[] m_showHideOnClick = (byte[]) null;
    protected static byte[] m_cursorHand = (byte[]) null;
    protected static byte[] m_rtlDir = (byte[]) null;
    protected static byte[] m_ltrDir = (byte[]) null;
    protected static byte[] m_classStyle = (byte[]) null;
    protected static byte[] m_openStyle = (byte[]) null;
    protected static byte[] m_underscore = (byte[]) null;
    protected static byte[] m_lineBreak = (byte[]) null;
    protected static byte[] m_ssClassID = (byte[]) null;
    protected static byte[] m_ptClassID = (byte[]) null;
    protected static byte[] m_xmlData = (byte[]) null;
    protected static byte[] m_useMap = (byte[]) null;
    protected static byte[] m_openMap = (byte[]) null;
    protected static byte[] m_closeMap = (byte[]) null;
    protected static byte[] m_mapArea = (byte[]) null;
    protected static byte[] m_mapCoords = (byte[]) null;
    protected static byte[] m_mapShape = (byte[]) null;
    protected static byte[] m_name = (byte[]) null;
    protected static byte[] m_circleShape = (byte[]) null;
    protected static byte[] m_polyShape = (byte[]) null;
    protected static byte[] m_rectShape = (byte[]) null;
    protected static byte[] m_comma = (byte[]) null;
    private static string m_mapPrefixString = "Map";
    protected static byte[] m_mapPrefix = (byte[]) null;
    protected static byte[] m_classPopupAction = (byte[]) null;
    protected static byte[] m_closeLi = (byte[]) null;
    protected static byte[] m_openLi = (byte[]) null;
    protected static byte[] m_firstNonHeaderPostfix = (byte[]) null;
    protected static byte[] m_fixedMatrixCornerPostfix = (byte[]) null;
    protected static byte[] m_fixedRowGroupingHeaderPostfix = (byte[]) null;
    protected static byte[] m_fixedColumnGroupingHeaderPostfix = (byte[]) null;
    protected static byte[] m_fixedRowHeaderPostfix = (byte[]) null;
    protected static byte[] m_fixedColumnHeaderPostfix = (byte[]) null;
    protected static byte[] m_fixedTableCornerPostfix = (byte[]) null;
    internal static byte[] m_language = (byte[]) null;
    private static byte[] m_zeroBorderWidth = (byte[]) null;
    internal static byte[] m_onLoadFitProportionalPv = (byte[]) null;
    private static byte[] m_normalWordWrap = (byte[]) null;
    private static byte[] m_classPercentSizes = (byte[]) null;
    private static byte[] m_classPercentSizesOverflow = (byte[]) null;
    private static byte[] m_classPercentWidthOverflow = (byte[]) null;
    private static byte[] m_classPercentHeight = (byte[]) null;
    private static byte[] m_classLayoutBorder = (byte[]) null;
    private static byte[] m_classLayoutFixed = (byte[]) null;
    private static byte[] m_strokeColor = (byte[]) null;
    private static byte[] m_strokeWeight = (byte[]) null;
    private static byte[] m_slineStyle = (byte[]) null;
    private static byte[] m_dashStyle = (byte[]) null;
    private static byte[] m_closeVGroup = (byte[]) null;
    private static byte[] m_openVGroup = (byte[]) null;
    private static byte[] m_openVLine = (byte[]) null;
    private static byte[] m_leftSlant = (byte[]) null;
    private static byte[] m_rightSlant = (byte[]) null;
    private static byte[] m_pageBreakDelimiter = (byte[]) null;
    private static byte[] m_nogrowAttribute = (byte[]) null;
    private static byte[] m_stylePositionAbsolute = (byte[]) null;
    private static byte[] m_stylePositionRelative = (byte[]) null;
    private static byte[] m_styleClipRectOpenBrace = (byte[]) null;
    private static byte[] m_styleTop = (byte[]) null;
    private static byte[] m_styleLeft = (byte[]) null;
    private static byte[] m_pxSpace = (byte[]) null;
    internal static char[] m_cssDelimiters = new char[13]
    {
      '[',
      ']',
      '"',
      '\'',
      '<',
      '>',
      '{',
      '}',
      '(',
      ')',
      '/',
      '%',
      ' '
    };
    protected bool m_hasOnePage = true;
    protected Stream m_mainStream;
    internal Encoding m_encoding;
    protected RPLReport m_rplReport;
    protected RPLPageContent m_pageContent;
    protected RPLReportSection m_rplReportSection;
    protected IReportWrapper m_report;
    protected ISPBProcessing m_spbProcessing;
    protected Hashtable m_usedStyles;
    protected NameValueCollection m_serverParams;
    protected DeviceInfo m_deviceInfo;
    protected NameValueCollection m_rawDeviceInfo;
    protected Dictionary<string, string> m_images;
    protected byte[] m_stylePrefixIdBytes;
    protected int m_pageNum;
    protected CreateAndRegisterStream m_createAndRegisterStreamCallback;
    protected bool m_fitPropImages;
    protected bool m_browserIE = true;
    protected HTML4Renderer.RequestType m_requestType;
    protected bool m_htmlFragment;
    protected Stream m_styleStream;
    protected Stream m_growRectangleIdsStream;
    protected Stream m_fitVertTextIdsStream;
    protected Stream m_imgFitDivIdsStream;
    protected Stream m_imgConImageIdsStream;
    protected bool m_useInlineStyle;
    protected bool m_pageWithBookmarkLinks;
    protected bool m_pageWithSortClicks;
    protected bool m_allPages;
    protected int m_outputLineLength;
    protected bool m_onlyVisibleStyles;
    private SecondaryStreams m_createSecondaryStreams = (SecondaryStreams) 1;
    protected int m_tabIndexNum;
    protected int m_currentHitCount;
    protected Hashtable m_duplicateItems;
    protected string m_searchText;
    protected bool m_emitImageConsolidationScaling;
    protected bool m_needsCanGrowFalseScript;
    protected bool m_needsGrowRectangleScript;
    protected bool m_needsFitVertTextScript;
    internal static string m_searchHitIdPrefix = "oHit";
    internal static string m_standardLineBreak = "\n";
    protected Stack m_linkToChildStack;
    protected HTML4Renderer.PageSection m_pageSection;
    protected bool m_pageHasStyle;
    protected bool m_isBody;
    protected bool m_usePercentWidth;
    protected bool m_hasSlantedLines;
    internal bool m_expandItem;
    protected ArrayList m_fixedHeaders;
    private bool m_isStyleOpen;
    private bool m_renderTableHeight;
    private string m_contextLanguage;
    private bool m_allowBandTable = true;
    protected byte[] m_styleClassPrefix;

    static HTML4Renderer()
    {
      UTF8Encoding utF8Encoding = new UTF8Encoding();
      HTML4Renderer.m_newLine = utF8Encoding.GetBytes("\r\n");
      HTML4Renderer.m_openTable = utF8Encoding.GetBytes("<TABLE CELLSPACING=\"0\" CELLPADDING=\"0\"");
      HTML4Renderer.m_zeroBorder = utF8Encoding.GetBytes(" BORDER=\"0\"");
      HTML4Renderer.m_zeroPoint = utF8Encoding.GetBytes("0pt");
      HTML4Renderer.m_smallPoint = utF8Encoding.GetBytes("1px");
      HTML4Renderer.m_cols = utF8Encoding.GetBytes(" COLS=\"");
      HTML4Renderer.m_colSpan = utF8Encoding.GetBytes(" COLSPAN=\"");
      HTML4Renderer.m_rowSpan = utF8Encoding.GetBytes(" ROWSPAN=\"");
      HTML4Renderer.m_headers = utF8Encoding.GetBytes(" HEADERS=\"");
      HTML4Renderer.m_space = utF8Encoding.GetBytes(" ");
      HTML4Renderer.m_closeBracket = utF8Encoding.GetBytes(">");
      HTML4Renderer.m_closeTable = utF8Encoding.GetBytes("</TABLE>");
      HTML4Renderer.m_openDiv = utF8Encoding.GetBytes("<DIV");
      HTML4Renderer.m_closeDiv = utF8Encoding.GetBytes("</DIV>");
      HTML4Renderer.m_openBody = utF8Encoding.GetBytes("<body");
      HTML4Renderer.m_closeBody = utF8Encoding.GetBytes("</body>");
      HTML4Renderer.m_openHtml = utF8Encoding.GetBytes("<html>");
      HTML4Renderer.m_closeHtml = utF8Encoding.GetBytes("</html>");
      HTML4Renderer.m_openHead = utF8Encoding.GetBytes("<head>");
      HTML4Renderer.m_closeHead = utF8Encoding.GetBytes("</head>");
      HTML4Renderer.m_openTitle = utF8Encoding.GetBytes("<title>");
      HTML4Renderer.m_closeTitle = utF8Encoding.GetBytes("</title>");
      HTML4Renderer.m_firstTD = utF8Encoding.GetBytes("<TR><TD");
      HTML4Renderer.m_lastTD = utF8Encoding.GetBytes("</TD></TR>");
      HTML4Renderer.m_openTD = utF8Encoding.GetBytes("<TD");
      HTML4Renderer.m_closeTD = utF8Encoding.GetBytes("</TD>");
      HTML4Renderer.m_closeTR = utF8Encoding.GetBytes("</TR>");
      HTML4Renderer.m_openTR = utF8Encoding.GetBytes("<TR");
      HTML4Renderer.m_valign = utF8Encoding.GetBytes(" VALIGN=\"");
      HTML4Renderer.m_openSpan = utF8Encoding.GetBytes("<span");
      HTML4Renderer.m_closeSpan = utF8Encoding.GetBytes("</span>");
      HTML4Renderer.m_quote = utF8Encoding.GetBytes(HTML4Renderer.m_quoteString);
      HTML4Renderer.m_closeQuote = utF8Encoding.GetBytes(HTML4Renderer.m_closeQuoteString);
      HTML4Renderer.m_id = utF8Encoding.GetBytes(" ID=\"");
      HTML4Renderer.m_mm = utF8Encoding.GetBytes("mm");
      HTML4Renderer.m_px = utF8Encoding.GetBytes("px");
      HTML4Renderer.m_zeroWidth = utF8Encoding.GetBytes(" WIDTH=\"0\"");
      HTML4Renderer.m_zeroHeight = utF8Encoding.GetBytes(" HEIGHT=\"0\"");
      HTML4Renderer.m_closeTag = utF8Encoding.GetBytes("\"/>");
      HTML4Renderer.m_openA = utF8Encoding.GetBytes("<a");
      HTML4Renderer.m_target = utF8Encoding.GetBytes(" TARGET=\"");
      HTML4Renderer.m_closeA = utF8Encoding.GetBytes("</a>");
      HTML4Renderer.m_href = utF8Encoding.GetBytes(HTML4Renderer.m_hrefString);
      HTML4Renderer.m_nohref = utF8Encoding.GetBytes(" nohref=\"true\"");
      HTML4Renderer.m_inlineHeight = utF8Encoding.GetBytes(" HEIGHT=\"");
      HTML4Renderer.m_inlineWidth = utF8Encoding.GetBytes(" WIDTH=\"");
      HTML4Renderer.m_img = utF8Encoding.GetBytes("<IMG");
      HTML4Renderer.m_imgOnError = utF8Encoding.GetBytes(" onerror=\"this.errored=true;\"");
      HTML4Renderer.m_src = utF8Encoding.GetBytes(" SRC=\"");
      HTML4Renderer.m_topValue = utF8Encoding.GetBytes("top");
      HTML4Renderer.m_leftValue = utF8Encoding.GetBytes("left");
      HTML4Renderer.m_rightValue = utF8Encoding.GetBytes("right");
      HTML4Renderer.m_centerValue = utF8Encoding.GetBytes("center");
      HTML4Renderer.m_classID = utF8Encoding.GetBytes(" CLASSID=\"CLSID:");
      HTML4Renderer.m_codeBase = utF8Encoding.GetBytes(" CODEBASE=\"");
      HTML4Renderer.m_title = utF8Encoding.GetBytes(" TITLE=\"");
      HTML4Renderer.m_alt = utF8Encoding.GetBytes(" ALT=\"");
      HTML4Renderer.m_openObject = utF8Encoding.GetBytes("<OBJECT");
      HTML4Renderer.m_closeObject = utF8Encoding.GetBytes("</OBJECT>");
      HTML4Renderer.m_paramObject = utF8Encoding.GetBytes("<PARAM NAME=\"");
      HTML4Renderer.m_valueObject = utF8Encoding.GetBytes(" VALUE=\"");
      HTML4Renderer.m_equal = utF8Encoding.GetBytes("=");
      HTML4Renderer.m_encodedAmp = utF8Encoding.GetBytes("&amp;");
      HTML4Renderer.m_nbsp = utF8Encoding.GetBytes("&nbsp;");
      HTML4Renderer.m_questionMark = utF8Encoding.GetBytes("?");
      HTML4Renderer.m_none = utF8Encoding.GetBytes("none");
      HTML4Renderer.m_displayNone = utF8Encoding.GetBytes("display: none;");
      HTML4Renderer.m_checkForEnterKey = utF8Encoding.GetBytes("if(event.keyCode == 13 || event.which == 13){");
      HTML4Renderer.m_percent = utF8Encoding.GetBytes("100%");
      HTML4Renderer.m_ninetyninepercent = utF8Encoding.GetBytes("99%");
      HTML4Renderer.m_degree90 = utF8Encoding.GetBytes("90");
      HTML4Renderer.m_lineBreak = utF8Encoding.GetBytes(HTML4Renderer.m_standardLineBreak);
      HTML4Renderer.m_closeBrace = utF8Encoding.GetBytes(")");
      HTML4Renderer.m_rtlDir = utF8Encoding.GetBytes(" dir=\"RTL\"");
      HTML4Renderer.m_ltrDir = utF8Encoding.GetBytes(" dir=\"LTR\"");
      HTML4Renderer.m_br = utF8Encoding.GetBytes("<br/>");
      HTML4Renderer.m_tabIndex = utF8Encoding.GetBytes(" tabindex=\"");
      HTML4Renderer.m_useMap = utF8Encoding.GetBytes(" USEMAP=\"");
      HTML4Renderer.m_openMap = utF8Encoding.GetBytes("<MAP ");
      HTML4Renderer.m_closeMap = utF8Encoding.GetBytes("</MAP>");
      HTML4Renderer.m_mapArea = utF8Encoding.GetBytes("<AREA ");
      HTML4Renderer.m_mapCoords = utF8Encoding.GetBytes(" COORDS=\"");
      HTML4Renderer.m_mapShape = utF8Encoding.GetBytes(" SHAPE=\"");
      HTML4Renderer.m_name = utF8Encoding.GetBytes(" NAME=\"");
      HTML4Renderer.m_circleShape = utF8Encoding.GetBytes("circle");
      HTML4Renderer.m_polyShape = utF8Encoding.GetBytes("poly");
      HTML4Renderer.m_rectShape = utF8Encoding.GetBytes("rect");
      HTML4Renderer.m_comma = utF8Encoding.GetBytes(",");
      HTML4Renderer.m_mapPrefix = utF8Encoding.GetBytes(HTML4Renderer.m_mapPrefixString);
      HTML4Renderer.m_openLi = utF8Encoding.GetBytes("<li");
      HTML4Renderer.m_closeLi = utF8Encoding.GetBytes("</li>");
      HTML4Renderer.m_firstNonHeaderPostfix = utF8Encoding.GetBytes("_FNHR");
      HTML4Renderer.m_fixedMatrixCornerPostfix = utF8Encoding.GetBytes("_MCC");
      HTML4Renderer.m_fixedRowGroupingHeaderPostfix = utF8Encoding.GetBytes("_FRGH");
      HTML4Renderer.m_fixedColumnGroupingHeaderPostfix = utF8Encoding.GetBytes("_FCGH");
      HTML4Renderer.m_fixedRowHeaderPostfix = utF8Encoding.GetBytes("_FRH");
      HTML4Renderer.m_fixedColumnHeaderPostfix = utF8Encoding.GetBytes("_FCH");
      HTML4Renderer.m_fixedTableCornerPostfix = utF8Encoding.GetBytes("_FCC");
      HTML4Renderer.m_dot = utF8Encoding.GetBytes(".");
      HTML4Renderer.m_percentSizes = utF8Encoding.GetBytes("r1");
      HTML4Renderer.m_percentSizesOverflow = utF8Encoding.GetBytes("r2");
      HTML4Renderer.m_percentHeight = utF8Encoding.GetBytes("r3");
      HTML4Renderer.m_ignoreBorder = utF8Encoding.GetBytes("r4");
      HTML4Renderer.m_ignoreBorderL = utF8Encoding.GetBytes("r5");
      HTML4Renderer.m_ignoreBorderR = utF8Encoding.GetBytes("r6");
      HTML4Renderer.m_ignoreBorderT = utF8Encoding.GetBytes("r7");
      HTML4Renderer.m_ignoreBorderB = utF8Encoding.GetBytes("r8");
      HTML4Renderer.m_layoutFixed = utF8Encoding.GetBytes("r9");
      HTML4Renderer.m_layoutBorder = utF8Encoding.GetBytes("r10");
      HTML4Renderer.m_percentWidthOverflow = utF8Encoding.GetBytes("r11");
      HTML4Renderer.m_popupAction = utF8Encoding.GetBytes("r12");
      HTML4Renderer.m_styleAction = utF8Encoding.GetBytes("r13");
      HTML4Renderer.m_emptyTextBox = utF8Encoding.GetBytes("r14");
      HTML4Renderer.m_classPercentSizes = utF8Encoding.GetBytes(" class=\"r1\"");
      HTML4Renderer.m_classPercentSizesOverflow = utF8Encoding.GetBytes(" class=\"r2\"");
      HTML4Renderer.m_classPercentHeight = utF8Encoding.GetBytes(" class=\"r3\"");
      HTML4Renderer.m_classLayoutFixed = utF8Encoding.GetBytes(" class=\"r9");
      HTML4Renderer.m_classLayoutBorder = utF8Encoding.GetBytes(" class=\"r10");
      HTML4Renderer.m_classPercentWidthOverflow = utF8Encoding.GetBytes(" class=\"r11\"");
      HTML4Renderer.m_classPopupAction = utF8Encoding.GetBytes(" class=\"r12\"");
      HTML4Renderer.m_classAction = utF8Encoding.GetBytes(" class=\"r13\"");
      HTML4Renderer.m_rtlEmbed = utF8Encoding.GetBytes("r15");
      HTML4Renderer.m_classRtlEmbed = utF8Encoding.GetBytes(" class=\"r15\"");
      HTML4Renderer.m_noVerticalMarginClassName = utF8Encoding.GetBytes("r16");
      HTML4Renderer.m_classNoVerticalMargin = utF8Encoding.GetBytes(" class=\"r16\"");
      HTML4Renderer.m_percentSizeInlineTable = utF8Encoding.GetBytes("r17");
      HTML4Renderer.m_classPercentSizeInlineTable = utF8Encoding.GetBytes(" class=\"r17\"");
      HTML4Renderer.m_percentHeightInlineTable = utF8Encoding.GetBytes("r18");
      HTML4Renderer.m_classPercentHeightInlineTable = utF8Encoding.GetBytes(" class=\"r18\"");
      HTML4Renderer.m_underscore = utF8Encoding.GetBytes("_");
      HTML4Renderer.m_openAccol = utF8Encoding.GetBytes("{");
      HTML4Renderer.m_closeAccol = utF8Encoding.GetBytes("}");
      HTML4Renderer.m_classStyle = utF8Encoding.GetBytes(" class=\"");
      HTML4Renderer.m_openStyle = utF8Encoding.GetBytes(" style=\"");
      HTML4Renderer.m_styleHeight = utF8Encoding.GetBytes("HEIGHT:");
      HTML4Renderer.m_styleMinHeight = utF8Encoding.GetBytes("min-height:");
      HTML4Renderer.m_styleWidth = utF8Encoding.GetBytes("WIDTH:");
      HTML4Renderer.m_styleMinWidth = utF8Encoding.GetBytes("min-width:");
      HTML4Renderer.m_zeroBorderWidth = utF8Encoding.GetBytes("border-width:0px");
      HTML4Renderer.m_border = utF8Encoding.GetBytes("border:");
      HTML4Renderer.m_borderLeft = utF8Encoding.GetBytes("border-left:");
      HTML4Renderer.m_borderTop = utF8Encoding.GetBytes("border-top:");
      HTML4Renderer.m_borderBottom = utF8Encoding.GetBytes("border-bottom:");
      HTML4Renderer.m_borderRight = utF8Encoding.GetBytes("border-right:");
      HTML4Renderer.m_borderColor = utF8Encoding.GetBytes("border-color:");
      HTML4Renderer.m_borderStyle = utF8Encoding.GetBytes("border-style:");
      HTML4Renderer.m_borderWidth = utF8Encoding.GetBytes("border-width:");
      HTML4Renderer.m_borderBottomColor = utF8Encoding.GetBytes("border-bottom-color:");
      HTML4Renderer.m_borderBottomStyle = utF8Encoding.GetBytes("border-bottom-style:");
      HTML4Renderer.m_borderBottomWidth = utF8Encoding.GetBytes("border-bottom-width:");
      HTML4Renderer.m_borderLeftColor = utF8Encoding.GetBytes("border-left-color:");
      HTML4Renderer.m_borderLeftStyle = utF8Encoding.GetBytes("border-left-style:");
      HTML4Renderer.m_borderLeftWidth = utF8Encoding.GetBytes("border-left-width:");
      HTML4Renderer.m_borderRightColor = utF8Encoding.GetBytes("border-right-color:");
      HTML4Renderer.m_borderRightStyle = utF8Encoding.GetBytes("border-right-style:");
      HTML4Renderer.m_borderRightWidth = utF8Encoding.GetBytes("border-right-width:");
      HTML4Renderer.m_borderTopColor = utF8Encoding.GetBytes("border-top-color:");
      HTML4Renderer.m_borderTopStyle = utF8Encoding.GetBytes("border-top-style:");
      HTML4Renderer.m_borderTopWidth = utF8Encoding.GetBytes("border-top-width:");
      HTML4Renderer.m_semiColon = utF8Encoding.GetBytes(";");
      HTML4Renderer.m_wordWrap = utF8Encoding.GetBytes("word-wrap:break-word");
      HTML4Renderer.m_whiteSpacePreWrap = utF8Encoding.GetBytes("white-space:pre-wrap");
      HTML4Renderer.m_normalWordWrap = utF8Encoding.GetBytes("word-wrap:normal");
      HTML4Renderer.m_overflowHidden = utF8Encoding.GetBytes("overflow:hidden");
      HTML4Renderer.m_overflowXHidden = utF8Encoding.GetBytes("overflow-x:hidden");
      HTML4Renderer.m_borderCollapse = utF8Encoding.GetBytes("border-collapse:collapse");
      HTML4Renderer.m_tableLayoutFixed = utF8Encoding.GetBytes("table-layout:fixed");
      HTML4Renderer.m_paddingLeft = utF8Encoding.GetBytes("padding-left:");
      HTML4Renderer.m_paddingRight = utF8Encoding.GetBytes("padding-right:");
      HTML4Renderer.m_paddingTop = utF8Encoding.GetBytes("padding-top:");
      HTML4Renderer.m_paddingBottom = utF8Encoding.GetBytes("padding-bottom:");
      HTML4Renderer.m_backgroundColor = utF8Encoding.GetBytes("background-color:");
      HTML4Renderer.m_backgroundImage = utF8Encoding.GetBytes("background-image:url(");
      HTML4Renderer.m_backgroundRepeat = utF8Encoding.GetBytes("background-repeat:");
      HTML4Renderer.m_fontStyle = utF8Encoding.GetBytes("font-style:");
      HTML4Renderer.m_fontFamily = utF8Encoding.GetBytes("font-family:");
      HTML4Renderer.m_fontSize = utF8Encoding.GetBytes("font-size:");
      HTML4Renderer.m_fontWeight = utF8Encoding.GetBytes("font-weight:");
      HTML4Renderer.m_textDecoration = utF8Encoding.GetBytes("text-decoration:");
      HTML4Renderer.m_textAlign = utF8Encoding.GetBytes("text-align:");
      HTML4Renderer.m_verticalAlign = utF8Encoding.GetBytes("vertical-align:");
      HTML4Renderer.m_color = utF8Encoding.GetBytes("color:");
      HTML4Renderer.m_lineHeight = utF8Encoding.GetBytes("line-height:");
      HTML4Renderer.m_direction = utF8Encoding.GetBytes("direction:");
      HTML4Renderer.m_unicodeBiDi = utF8Encoding.GetBytes("unicode-bidi:");
      HTML4Renderer.m_writingMode = utF8Encoding.GetBytes("writing-mode:");
      HTML4Renderer.m_msoRotation = utF8Encoding.GetBytes("mso-rotate:");
      HTML4Renderer.m_tbrl = utF8Encoding.GetBytes("tb-rl");
      HTML4Renderer.m_btrl = utF8Encoding.GetBytes("bt-rl");
      HTML4Renderer.m_lrtb = utF8Encoding.GetBytes("lr-tb");
      HTML4Renderer.m_rltb = utF8Encoding.GetBytes("rl-tb");
      HTML4Renderer.m_layoutFlow = utF8Encoding.GetBytes("layout-flow:");
      HTML4Renderer.m_verticalIdeographic = utF8Encoding.GetBytes("vertical-ideographic");
      HTML4Renderer.m_horizontal = utF8Encoding.GetBytes("horizontal");
      HTML4Renderer.m_cursorHand = utF8Encoding.GetBytes("cursor:pointer");
      HTML4Renderer.m_filter = utF8Encoding.GetBytes("filter:");
      HTML4Renderer.m_language = utF8Encoding.GetBytes(" LANG=\"");
      HTML4Renderer.m_marginLeft = utF8Encoding.GetBytes("margin-left:");
      HTML4Renderer.m_marginTop = utF8Encoding.GetBytes("margin-top:");
      HTML4Renderer.m_marginBottom = utF8Encoding.GetBytes("margin-bottom:");
      HTML4Renderer.m_marginRight = utF8Encoding.GetBytes("margin-right:");
      HTML4Renderer.m_textIndent = utF8Encoding.GetBytes("text-indent:");
      HTML4Renderer.m_onLoadFitProportionalPv = utF8Encoding.GetBytes(" onload=\"this.fitproportional=true;this.pv=");
      HTML4Renderer.m_basicImageRotation180 = utF8Encoding.GetBytes("progid:DXImageTransform.Microsoft.BasicImage(rotation=2)");
      HTML4Renderer.m_openVGroup = utF8Encoding.GetBytes("<v:group coordsize=\"100,100\" coordorigin=\"0,0\"");
      HTML4Renderer.m_openVLine = utF8Encoding.GetBytes("<v:line from=\"0,");
      HTML4Renderer.m_strokeColor = utF8Encoding.GetBytes(" strokecolor=\"");
      HTML4Renderer.m_strokeWeight = utF8Encoding.GetBytes(" strokeWeight=\"");
      HTML4Renderer.m_dashStyle = utF8Encoding.GetBytes("<v:stroke dashstyle=\"");
      HTML4Renderer.m_slineStyle = utF8Encoding.GetBytes(" slineStyle=\"");
      HTML4Renderer.m_closeVGroup = utF8Encoding.GetBytes("</v:line></v:group>");
      HTML4Renderer.m_rightSlant = utF8Encoding.GetBytes("100\" to=\"100,0\"");
      HTML4Renderer.m_leftSlant = utF8Encoding.GetBytes("0\" to=\"100,100\"");
      HTML4Renderer.m_pageBreakDelimiter = utF8Encoding.GetBytes("<div style=\"page-break-after:always\"><hr/></div>");
      HTML4Renderer.m_stylePositionAbsolute = utF8Encoding.GetBytes("position:absolute;");
      HTML4Renderer.m_stylePositionRelative = utF8Encoding.GetBytes("position:relative;");
      HTML4Renderer.m_styleClipRectOpenBrace = utF8Encoding.GetBytes("clip:rect(");
      HTML4Renderer.m_styleTop = utF8Encoding.GetBytes("top:");
      HTML4Renderer.m_styleLeft = utF8Encoding.GetBytes("left:");
      HTML4Renderer.m_pxSpace = utF8Encoding.GetBytes("px ");
      HTML4Renderer.m_closeUL = utF8Encoding.GetBytes("</ul>");
      HTML4Renderer.m_closeOL = utF8Encoding.GetBytes("</ol>");
      HTML4Renderer.m_olArabic = utF8Encoding.GetBytes("<ol");
      HTML4Renderer.m_olRoman = utF8Encoding.GetBytes("<ol type=\"i\"");
      HTML4Renderer.m_olAlpha = utF8Encoding.GetBytes("<ol type=\"a\"");
      HTML4Renderer.m_ulDisc = utF8Encoding.GetBytes("<ul type=\"disc\"");
      HTML4Renderer.m_ulSquare = utF8Encoding.GetBytes("<ul type=\"square\"");
      HTML4Renderer.m_ulCircle = utF8Encoding.GetBytes("<ul type=\"circle\"");
      HTML4Renderer.m_nogrowAttribute = utF8Encoding.GetBytes(" nogrow=\"true\"");
      HTML4Renderer.m_styleMinWidth = utF8Encoding.GetBytes("min-width: ");
      HTML4Renderer.m_styleMinHeight = utF8Encoding.GetBytes("min-height: ");
      HTML4Renderer.m_styleDisplayInlineBlock = utF8Encoding.GetBytes("display: inline-block;");
    }

    public HTML4Renderer(
      IReportWrapper report,
      ISPBProcessing spbProcessing,
      NameValueCollection reportServerParams,
      DeviceInfo deviceInfo,
      NameValueCollection rawDeviceInfo,
      NameValueCollection browserCaps,
      CreateAndRegisterStream createAndRegisterStreamCallback,
      SecondaryStreams secondaryStreams)
    {
      this.SearchText = deviceInfo.FindString;
      this.m_report = report;
      this.m_spbProcessing = spbProcessing;
      this.m_createSecondaryStreams = secondaryStreams;
      this.m_usedStyles = new Hashtable();
      this.m_images = new Dictionary<string, string>();
      this.m_browserIE = deviceInfo.IsBrowserIE;
      this.m_deviceInfo = deviceInfo;
      this.m_rawDeviceInfo = rawDeviceInfo;
      this.m_serverParams = reportServerParams;
      this.m_createAndRegisterStreamCallback = createAndRegisterStreamCallback;
      this.m_htmlFragment = deviceInfo.HTMLFragment;
      this.m_onlyVisibleStyles = deviceInfo.OnlyVisibleStyles;
      this.m_pageNum = deviceInfo.Section;
      rawDeviceInfo.Remove("Section");
      rawDeviceInfo.Remove("FindString");
      rawDeviceInfo.Remove("BookmarkId");
      this.m_spbProcessing.SetContext(new SPBContext()
      {
        StartPage = this.m_pageNum,
        EndPage = this.m_pageNum,
        SecondaryStreams = this.m_createSecondaryStreams,
        AddSecondaryStreamNames = true,
        UseImageConsolidation = this.m_deviceInfo.ImageConsolidation
      });
      this.m_linkToChildStack = new Stack(1);
      this.m_stylePrefixIdBytes = Encoding.UTF8.GetBytes(this.m_deviceInfo.StylePrefixId);
      if (this.m_deviceInfo.StyleStream)
        return;
      this.m_useInlineStyle = this.m_htmlFragment;
    }

    internal void InitializeReport()
    {
      this.m_rplReport = this.GetNextPage();
      this.m_pageContent = this.m_rplReport != null ? this.m_rplReport.RPLPaginatedPages[0] : throw new InvalidSectionException();
      this.m_rplReportSection = this.m_pageContent.GetNextReportSection();
      this.CheckBodyStyle();
      this.m_contextLanguage = this.m_rplReport.Language;
      this.m_expandItem = false;
    }

    protected static string GetStyleStreamName(string aReportName, int aPageNumber)
    {
      return HTML4Renderer.GetStreamName(aReportName, aPageNumber, "style");
    }

    internal static string GetStreamName(string aReportName, int aPageNumber, string suffix)
    {
      return aPageNumber > 0 ? string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}{2}{1}{3}", (object) aReportName, (object) '_', (object) suffix, (object) aPageNumber) : string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}{2}", (object) aReportName, (object) '_', (object) suffix);
    }

    internal static string HandleSpecialFontCharacters(string fontName)
    {
      if (fontName.IndexOfAny(HTML4Renderer.m_cssDelimiters) == -1)
        return fontName;
      fontName = fontName.Trim();
      if (fontName.StartsWith("'", StringComparison.Ordinal))
        fontName = fontName.Substring(1);
      if (fontName.EndsWith("'", StringComparison.Ordinal))
        fontName = fontName.Substring(0, fontName.Length - 1);
      return "'" + fontName.Replace("'", "&quot;") + "'";
    }

    protected abstract void RenderSortAction(
      RPLTextBoxProps textBoxProps,
      RPLFormat.SortOptions sortState);

    protected abstract void RenderInternalImageSrc();

    protected abstract void RenderToggleImage(RPLTextBoxProps textBoxProps);

    public abstract void Render(HtmlTextWriter outputWriter);

    internal string SearchText
    {
      set => this.m_searchText = value;
    }

    internal void RenderStylesOnly(string streamName)
    {
      this.m_encoding = Encoding.UTF8;
      Stream stream = this.CreateStream(streamName, "css", Encoding.UTF8, "text/css", false, StreamOper.CreateAndRegister);
      StyleContext styleContext = new StyleContext();
      this.m_styleStream = (Stream) new BufferedStream(stream);
      for (; this.m_rplReportSection != null; this.m_rplReportSection = this.m_pageContent.GetNextReportSection())
      {
        int borderContext = 0;
        RPLItemMeasurement header = this.m_rplReportSection.Header;
        if (header != null)
        {
          RPLHeaderFooter element = (RPLHeaderFooter) header.Element;
          RPLElementProps elementProps = ((RPLElement) element).ElementProps;
          RPLElementPropsDef definition = elementProps.Definition;
          styleContext.StyleOnCell = true;
          this.RenderSharedStyle((RPLElement) element, elementProps, definition, definition.SharedStyle, header, definition.ID + "c", styleContext, ref borderContext);
          styleContext.StyleOnCell = false;
          this.RenderSharedStyle((RPLElement) element, elementProps, definition, definition.SharedStyle, header, definition.ID, styleContext, ref borderContext);
          RPLItemMeasurement[] children = ((RPLContainer) element).Children;
          if (children != null)
          {
            for (int index = 0; index < children.Length; ++index)
              this.RenderStylesOnlyRecursive(children[index], new StyleContext());
          }
          header.Element = (RPLItem) null;
        }
        RPLItemMeasurement footer = this.m_rplReportSection.Footer;
        if (footer != null)
        {
          RPLHeaderFooter element = (RPLHeaderFooter) footer.Element;
          RPLElementProps elementProps = ((RPLElement) element).ElementProps;
          RPLElementPropsDef definition = elementProps.Definition;
          styleContext.StyleOnCell = true;
          this.RenderSharedStyle((RPLElement) element, elementProps, definition, definition.SharedStyle, footer, definition.ID + "c", styleContext, ref borderContext);
          styleContext.StyleOnCell = false;
          this.RenderSharedStyle((RPLElement) element, elementProps, definition, definition.SharedStyle, footer, definition.ID, styleContext, ref borderContext);
          RPLItemMeasurement[] children = ((RPLContainer) element).Children;
          if (children != null)
          {
            for (int index = 0; index < children.Length; ++index)
              this.RenderStylesOnlyRecursive(children[index], new StyleContext());
          }
          footer.Element = (RPLItem) null;
        }
        RPLItemMeasurement measurement = new RPLItemMeasurement();
        ((RPLSizes) measurement).Width = this.m_pageContent.MaxSectionWidth;
        ((RPLSizes) measurement).Height = this.m_rplReportSection.BodyArea.Height;
        RPLItemMeasurement column = this.m_rplReportSection.Columns[0];
        RPLBody element1 = (RPLBody) this.m_rplReportSection.Columns[0].Element;
        RPLElementProps elementProps1 = ((RPLElement) element1).ElementProps;
        RPLElementPropsDef definition1 = elementProps1.Definition;
        this.RenderSharedStyle((RPLElement) element1, elementProps1, definition1, definition1.SharedStyle, measurement, definition1.ID, styleContext, ref borderContext);
        RPLItemMeasurement[] children1 = ((RPLContainer) element1).Children;
        if (children1 != null && children1.Length > 0)
        {
          for (int index = 0; index < children1.Length; ++index)
            this.RenderStylesOnlyRecursive(children1[index], new StyleContext());
        }
        column.Element = (RPLItem) null;
      }
      this.m_styleStream.Flush();
    }

    internal void RenderStylesOnlyRecursive(
      RPLItemMeasurement measurement,
      StyleContext styleContext)
    {
      int borderContext1 = 0;
      RPLElement element1 = (RPLElement) measurement.Element;
      RPLElementProps elementProps1 = element1.ElementProps;
      RPLElementPropsDef definition1 = elementProps1.Definition;
      RPLStyleProps sharedStyle1 = definition1.SharedStyle;
      string id1 = definition1.ID;
      object obj1 = elementProps1.Style[(byte) 26];
      if (element1 is RPLTextBox)
      {
        RPLTextBoxPropsDef rplTextBoxPropsDef = (RPLTextBoxPropsDef) definition1;
        bool ignoreVerticalAlign = styleContext.IgnoreVerticalAlign;
        if (rplTextBoxPropsDef.CanSort && !this.m_usedStyles.ContainsKey((object) (id1 + "p")))
        {
          if (rplTextBoxPropsDef.CanGrow || rplTextBoxPropsDef.CanShrink)
            styleContext.StyleOnCell = true;
          if (!rplTextBoxPropsDef.CanGrow && rplTextBoxPropsDef.CanShrink)
            styleContext.IgnoreVerticalAlign = true;
          this.RenderSharedStyle(element1, elementProps1, definition1, sharedStyle1, measurement, id1 + "p", styleContext, ref borderContext1);
          styleContext.StyleOnCell = false;
        }
        if (!this.m_deviceInfo.IsBrowserIE || this.m_deviceInfo.BrowserMode == BrowserMode.Standards || this.m_deviceInfo.OutlookCompat || obj1 != null && (RPLFormat.VerticalAlignments) obj1 != null)
          styleContext.IgnoreVerticalAlign = ignoreVerticalAlign;
        if (rplTextBoxPropsDef.CanShrink && !this.m_usedStyles.ContainsKey((object) (id1 + "s")))
        {
          styleContext.NoBorders = true;
          this.RenderSharedStyle(element1, elementProps1, definition1, sharedStyle1, measurement, id1 + "s", styleContext, ref borderContext1);
          if (!rplTextBoxPropsDef.CanGrow)
            styleContext.IgnoreVerticalAlign = true;
        }
        if (rplTextBoxPropsDef.CanSort && !rplTextBoxPropsDef.IsSimple && !this.IsFragment && rplTextBoxPropsDef.IsToggleParent)
          styleContext.IgnoreVerticalAlign = ignoreVerticalAlign;
        styleContext.RenderMeasurements = false;
        if (!this.m_usedStyles.ContainsKey((object) id1))
        {
          int borderContext2 = borderContext1;
          this.RenderSharedStyle(element1, elementProps1, definition1, sharedStyle1, measurement, id1, styleContext, ref borderContext2);
          styleContext.IgnoreVerticalAlign = ignoreVerticalAlign;
          borderContext2 = borderContext1;
          this.RenderSharedStyle(element1, elementProps1, definition1, sharedStyle1, measurement, id1 + "l", styleContext, ref borderContext2);
          this.RenderSharedStyle(element1, elementProps1, definition1, sharedStyle1, measurement, id1 + "r", styleContext, ref borderContext1);
        }
        RPLTextBoxProps rplTextBoxProps = elementProps1 as RPLTextBoxProps;
        if (!this.m_usedStyles.ContainsKey((object) (id1 + "a")) && this.HasAction(rplTextBoxProps.ActionInfo))
        {
          TextRunStyleWriter styleWriter = new TextRunStyleWriter(this);
          this.RenderSharedStyle((ElementStyleWriter) styleWriter, definition1.SharedStyle, styleContext, id1 + "a");
          styleWriter.WriteStyles(StyleWriterMode.Shared, (IRPLStyle) definition1.SharedStyle);
        }
        if (rplTextBoxPropsDef.IsSimple)
          return;
        RPLTextBox textBox = element1 as RPLTextBox;
        ParagraphStyleWriter styleWriter1 = new ParagraphStyleWriter(this, textBox);
        TextRunStyleWriter styleWriter2 = new TextRunStyleWriter(this);
        for (RPLParagraph nextParagraph = textBox.GetNextParagraph(); nextParagraph != null; nextParagraph = textBox.GetNextParagraph())
        {
          styleWriter1.Paragraph = nextParagraph;
          string id2 = ((RPLElement) nextParagraph).ElementProps.Definition.ID;
          styleWriter1.ParagraphMode = ParagraphStyleWriter.Mode.All;
          this.RenderSharedStyle((ElementStyleWriter) styleWriter1, ((RPLElement) nextParagraph).ElementProps.Definition.SharedStyle, styleContext, id2);
          styleWriter1.ParagraphMode = ParagraphStyleWriter.Mode.ListOnly;
          this.RenderSharedStyle((ElementStyleWriter) styleWriter1, ((RPLElement) nextParagraph).ElementProps.Definition.SharedStyle, styleContext, id2 + "l");
          styleWriter1.ParagraphMode = ParagraphStyleWriter.Mode.ParagraphOnly;
          this.RenderSharedStyle((ElementStyleWriter) styleWriter1, ((RPLElement) nextParagraph).ElementProps.Definition.SharedStyle, styleContext, id2 + "p");
          for (RPLTextRun nextTextRun = nextParagraph.GetNextTextRun(); nextTextRun != null; nextTextRun = nextParagraph.GetNextTextRun())
            this.RenderSharedStyle((ElementStyleWriter) styleWriter2, ((RPLElement) nextTextRun).ElementProps.Definition.SharedStyle, styleContext, ((RPLElement) nextTextRun).ElementProps.Definition.ID);
        }
      }
      else
      {
        if (!this.m_usedStyles.ContainsKey((object) id1))
          this.RenderSharedStyle(element1, elementProps1, definition1, sharedStyle1, measurement, id1, styleContext, ref borderContext1);
        switch (element1)
        {
          case RPLSubReport _:
            RPLItemMeasurement[] children1 = ((RPLContainer) element1).Children;
            if (children1 != null)
            {
              for (int index1 = 0; index1 < children1.Length; ++index1)
              {
                if (children1[index1].Element is RPLContainer element2 && element2.Children != null && element2.Children.Length > 0)
                {
                  for (int index2 = 0; index2 < element2.Children.Length; ++index2)
                  {
                    this.RenderStylesOnlyRecursive(element2.Children[index2], styleContext);
                    element2.Children[index2] = (RPLItemMeasurement) null;
                  }
                }
                children1[index1] = (RPLItemMeasurement) null;
              }
              measurement.Element = (RPLItem) null;
              break;
            }
            break;
          case RPLContainer _:
            styleContext.InTablix = false;
            RPLItemMeasurement[] children2 = ((RPLContainer) element1).Children;
            if (children2 != null && children2.Length > 0)
            {
              for (int index = 0; index < children2.Length; ++index)
              {
                this.RenderStylesOnlyRecursive(children2[index], styleContext);
                children2[index] = (RPLItemMeasurement) null;
              }
              break;
            }
            break;
          case RPLTablix _:
            RPLTablix rplTablix = (RPLTablix) element1;
            RPLTablixRow nextRow = rplTablix.GetNextRow();
            bool inTablix = styleContext.InTablix;
            for (; nextRow != null; nextRow = rplTablix.GetNextRow())
            {
              for (int index = 0; index < nextRow.NumCells; ++index)
              {
                RPLTablixCell rplTablixCell = nextRow[index];
                RPLElement element3 = (RPLElement) rplTablixCell.Element;
                RPLElementProps elementProps2 = element3.ElementProps;
                RPLElementPropsDef definition2 = elementProps2.Definition;
                RPLStyleProps sharedStyle2 = definition2.SharedStyle;
                bool zeroWidth = styleContext.ZeroWidth;
                float columnWidth = rplTablix.GetColumnWidth(rplTablixCell.ColIndex, rplTablixCell.ColSpan);
                styleContext.ZeroWidth = (double) columnWidth == 0.0;
                if (element3 != null)
                {
                  string id3 = definition2.ID;
                  if (!(element3 is RPLLine) && !this.m_usedStyles.ContainsKey((object) (id3 + "c")))
                  {
                    styleContext.StyleOnCell = true;
                    borderContext1 = HTML4Renderer.GetNewContext(borderContext1, rplTablixCell.ColIndex == 0, rplTablixCell.ColIndex + rplTablixCell.ColSpan == rplTablix.ColumnWidths.Length, rplTablixCell.RowIndex == 0, rplTablixCell.RowIndex + rplTablixCell.RowSpan == rplTablix.RowHeights.Length);
                    int borderContext3 = borderContext1;
                    RPLTextBox rplTextBox = (RPLTextBox) element3;
                    bool backgroundBorders = styleContext.OnlyRenderMeasurementsBackgroundBorders;
                    if (rplTextBox != null && HTML4Renderer.IsWritingModeVertical((IRPLStyle) sharedStyle2) && this.m_deviceInfo.IsBrowserIE && this.m_deviceInfo.BrowserMode == BrowserMode.Standards)
                      styleContext.OnlyRenderMeasurementsBackgroundBorders = true;
                    this.RenderSharedStyle(element3, elementProps2, definition2, sharedStyle2, (RPLItemMeasurement) null, id3 + "c", styleContext, ref borderContext3);
                    borderContext3 = borderContext1;
                    this.RenderSharedStyle(element3, elementProps2, definition2, sharedStyle2, (RPLItemMeasurement) null, id3 + "cl", styleContext, ref borderContext3);
                    this.RenderSharedStyle(element3, elementProps2, definition2, sharedStyle2, (RPLItemMeasurement) null, id3 + "cr", styleContext, ref borderContext1);
                    styleContext.StyleOnCell = false;
                    styleContext.OnlyRenderMeasurementsBackgroundBorders = backgroundBorders;
                  }
                  styleContext.InTablix = true;
                  if (element3 is RPLContainer)
                  {
                    RPLItemMeasurement measurement1 = new RPLItemMeasurement();
                    ((RPLSizes) measurement1).Width = rplTablix.GetColumnWidth(rplTablixCell.ColIndex, rplTablixCell.ColSpan);
                    ((RPLSizes) measurement1).Height = rplTablix.GetRowHeight(rplTablixCell.RowIndex, rplTablixCell.RowSpan);
                    measurement1.Element = element3 as RPLItem;
                    this.RenderStylesOnlyRecursive(measurement1, styleContext);
                  }
                  else if (!this.m_usedStyles.ContainsKey((object) id3))
                  {
                    if (element3 is RPLTextBox)
                    {
                      object obj2 = element3.ElementProps.Style[(byte) 26];
                      RPLTextBoxPropsDef definition3 = (RPLTextBoxPropsDef) element3.ElementProps.Definition;
                      bool flag = obj2 != null && (RPLFormat.VerticalAlignments) obj2 != null && !definition3.CanGrow;
                      if (definition3.CanSort || flag)
                        styleContext.RenderMeasurements = false;
                    }
                    this.RenderSharedStyle(element3, elementProps2, definition2, sharedStyle2, (RPLItemMeasurement) null, element3.ElementProps.Definition.ID, styleContext, ref borderContext1);
                  }
                  styleContext.InTablix = inTablix;
                  nextRow[index] = (RPLTablixCell) null;
                  styleContext.ZeroWidth = zeroWidth;
                }
              }
            }
            break;
        }
        measurement.Element = (RPLItem) null;
      }
    }

    internal void RenderEmptyTopTablixRow(
      RPLTablix tablix,
      List<RPLTablixOmittedRow> omittedRows,
      string tablixID,
      bool emptyCol,
      TablixFixedHeaderStorage headerStorage)
    {
      bool flag = headerStorage.RowHeaders != null || headerStorage.ColumnHeaders != null;
      this.WriteStream(HTML4Renderer.m_openTR);
      if (flag)
      {
        string repItemId = tablixID + "r";
        this.RenderReportItemId(repItemId);
        if (headerStorage.RowHeaders != null)
          headerStorage.RowHeaders.Add(repItemId);
        if (headerStorage.ColumnHeaders != null)
          headerStorage.ColumnHeaders.Add(repItemId);
        if (headerStorage.CornerHeaders != null)
          headerStorage.CornerHeaders.Add(repItemId);
      }
      this.WriteStream(HTML4Renderer.m_zeroHeight);
      this.WriteStream(HTML4Renderer.m_closeBracket);
      if (emptyCol)
      {
        headerStorage.HasEmptyCol = true;
        this.WriteStream(HTML4Renderer.m_openTD);
        if (headerStorage.RowHeaders != null)
        {
          string repItemId = tablixID + "e";
          this.RenderReportItemId(repItemId);
          headerStorage.RowHeaders.Add(repItemId);
          if (headerStorage.CornerHeaders != null)
            headerStorage.CornerHeaders.Add(repItemId);
        }
        this.WriteStream(HTML4Renderer.m_openStyle);
        this.WriteStream(HTML4Renderer.m_styleWidth);
        this.WriteStream("0");
        this.WriteStream(HTML4Renderer.m_px);
        this.WriteStream(HTML4Renderer.m_closeQuote);
        this.WriteStream(HTML4Renderer.m_closeTD);
      }
      int[] numArray = new int[omittedRows.Count];
      for (int colIndex = 0; colIndex < tablix.ColumnWidths.Length; ++colIndex)
      {
        this.WriteStream(HTML4Renderer.m_openTD);
        if (tablix.FixedColumns[colIndex] && headerStorage.RowHeaders != null)
        {
          string repItemId = tablixID + "e" + (object) colIndex;
          this.RenderReportItemId(repItemId);
          headerStorage.RowHeaders.Add(repItemId);
          if (colIndex == tablix.ColumnWidths.Length - 1 || !tablix.FixedColumns[colIndex + 1])
            headerStorage.LastRowGroupCol = repItemId;
          if (headerStorage.CornerHeaders != null)
            headerStorage.CornerHeaders.Add(repItemId);
        }
        this.WriteStream(HTML4Renderer.m_openStyle);
        if ((double) tablix.ColumnWidths[colIndex] == 0.0)
          this.WriteStream(HTML4Renderer.m_displayNone);
        this.WriteStream(HTML4Renderer.m_styleWidth);
        this.WriteDStream(tablix.ColumnWidths[colIndex]);
        this.WriteStream(HTML4Renderer.m_mm);
        this.WriteStream(HTML4Renderer.m_semiColon);
        this.WriteStream(HTML4Renderer.m_styleMinWidth);
        this.WriteDStream(tablix.ColumnWidths[colIndex]);
        this.WriteStream(HTML4Renderer.m_mm);
        this.WriteStream(HTML4Renderer.m_closeQuote);
        for (int index = 0; index < omittedRows.Count; ++index)
          this.RenderTablixOmittedHeaderCells(((RPLTablixRow) omittedRows[index]).OmittedHeaders, colIndex, false, ref numArray[index]);
        this.WriteStream(HTML4Renderer.m_closeTD);
      }
      this.WriteStream(HTML4Renderer.m_closeTR);
    }

    internal void RenderEmptyHeightCell(
      float height,
      string tablixID,
      bool fixedRow,
      int row,
      TablixFixedHeaderStorage headerStorage)
    {
      this.WriteStream(HTML4Renderer.m_openTD);
      if (headerStorage.RowHeaders != null)
      {
        string repItemId = tablixID + "h" + (object) row;
        this.RenderReportItemId(repItemId);
        headerStorage.RowHeaders.Add(repItemId);
        if (fixedRow && headerStorage.CornerHeaders != null)
          headerStorage.CornerHeaders.Add(repItemId);
      }
      this.WriteStream(HTML4Renderer.m_openStyle);
      this.WriteStream(HTML4Renderer.m_styleHeight);
      this.WriteDStream(height);
      this.WriteStream(HTML4Renderer.m_mm);
      this.WriteStream(HTML4Renderer.m_closeQuote);
      this.WriteStream(HTML4Renderer.m_closeTD);
    }

    protected static int GetNewContext(
      int borderContext,
      bool left,
      bool right,
      bool top,
      bool bottom)
    {
      int newContext = 0;
      if (borderContext > 0)
      {
        if (top)
          newContext |= borderContext & 4;
        if (bottom)
          newContext |= borderContext & 8;
        if (left)
          newContext |= borderContext & 1;
        if (right)
          newContext |= borderContext & 2;
      }
      return newContext;
    }

    protected static int GetNewContext(int borderContext, int x, int y, int xMax, int yMax)
    {
      int newContext = 0;
      if (borderContext > 0)
      {
        if (x == 1)
          newContext |= borderContext & 4;
        if (x == xMax)
          newContext |= borderContext & 8;
        if (y == 1)
          newContext |= borderContext & 1;
        if (y == yMax)
          newContext |= borderContext & 2;
      }
      return newContext;
    }

    protected Rectangle RenderDynamicImage(
      RPLItemMeasurement measurement,
      RPLDynamicImageProps dynamicImageProps)
    {
      if (this.m_createSecondaryStreams != null)
        return dynamicImageProps.ImageConsolidationOffsets;
      Stream stream = this.CreateStream(dynamicImageProps.StreamName, "png", (Encoding) null, "image/png", false, StreamOper.CreateAndRegister);
      if (dynamicImageProps.DynamicImageContentOffset >= 0L)
        this.m_rplReport.GetImage(dynamicImageProps.DynamicImageContentOffset, stream);
      else if (dynamicImageProps.DynamicImageContent != null)
      {
        byte[] buffer = new byte[4096];
        dynamicImageProps.DynamicImageContent.Position = 0L;
        int count;
        for (int length = (int) dynamicImageProps.DynamicImageContent.Length; length > 0; length -= count)
        {
          count = dynamicImageProps.DynamicImageContent.Read(buffer, 0, Math.Min(buffer.Length, length));
          stream.Write(buffer, 0, count);
        }
      }
      return Rectangle.Empty;
    }

    internal bool NeedResizeImages => this.m_fitPropImages;

    protected bool IsFragment => this.m_htmlFragment && !this.m_deviceInfo.HasActionScript;

    internal bool IsBrowserIE => this.m_deviceInfo.IsBrowserIE;

    protected bool IsCollectionWithoutContent(RPLContainer container, ref bool empty)
    {
      bool flag = false;
      if (container != null)
      {
        flag = true;
        if (container.Children == null)
          empty = true;
      }
      return flag;
    }

    private void RenderOpenStyle(string id)
    {
      this.WriteStreamLineBreak();
      if (this.m_styleClassPrefix != null)
        this.WriteStream(this.m_styleClassPrefix);
      this.WriteStream(HTML4Renderer.m_dot);
      this.WriteStream(this.m_stylePrefixIdBytes);
      this.WriteStream(id);
      this.WriteStream(HTML4Renderer.m_openAccol);
    }

    protected virtual RPLReport GetNextPage()
    {
      RPLReport nextPage;
      this.m_spbProcessing.GetNextPage(ref nextPage);
      return nextPage;
    }

    protected virtual bool NeedSharedToggleParent(RPLTextBoxProps textBoxProps)
    {
      return !this.IsFragment && textBoxProps.IsToggleParent;
    }

    protected virtual bool CanSort(RPLTextBoxPropsDef textBoxDef)
    {
      return !this.IsFragment && textBoxDef.CanSort;
    }

    protected void RenderSortImage(RPLTextBoxProps textBoxProps)
    {
      if (this.m_deviceInfo.BrowserMode == BrowserMode.Quirks || this.m_deviceInfo.IsBrowserIE)
        this.WriteStream(HTML4Renderer.m_nbsp);
      this.WriteStream(HTML4Renderer.m_openA);
      this.WriteStream(HTML4Renderer.m_tabIndex);
      this.WriteStream((object) ++this.m_tabIndexNum);
      this.WriteStream(HTML4Renderer.m_quote);
      RPLFormat.SortOptions sortState = textBoxProps.SortState;
      this.RenderSortAction(textBoxProps, sortState);
      this.WriteStream(HTML4Renderer.m_img);
      if (this.m_browserIE)
        this.WriteStream(HTML4Renderer.m_imgOnError);
      this.WriteStream(HTML4Renderer.m_zeroBorder);
      this.WriteStream(HTML4Renderer.m_src);
      this.RenderSortImageText(sortState);
      this.WriteStream(HTML4Renderer.m_closeTag);
      this.WriteStream(HTML4Renderer.m_closeA);
    }

    protected virtual void RenderSortImageText(RPLFormat.SortOptions sortState)
    {
      this.RenderInternalImageSrc();
      if (sortState == 1)
        this.WriteStream(this.m_report.GetImageName("sortAsc.gif"));
      else if (sortState == 2)
        this.WriteStream(this.m_report.GetImageName("sortDesc.gif"));
      else
        this.WriteStream(this.m_report.GetImageName("unsorted.gif"));
    }

    internal void RenderOnClickActionScript(string actionType, string actionArg)
    {
      this.WriteStream(" onclick=\"");
      this.WriteStream(this.m_deviceInfo.ActionScript);
      this.WriteStream("('");
      this.WriteStream(actionType);
      this.WriteStream("','");
      this.WriteStream(actionArg);
      this.WriteStream("');return false;\"");
      this.WriteStream(" onkeypress=\"");
      this.WriteStream(HTML4Renderer.m_checkForEnterKey);
      this.WriteStream(this.m_deviceInfo.ActionScript);
      this.WriteStream("('");
      this.WriteStream(actionType);
      this.WriteStream("','");
      this.WriteStream(actionArg);
      this.WriteStream("');}return false;\"");
    }

    protected PaddingSharedInfo GetPaddings(RPLElementStyle style, PaddingSharedInfo paddingInfo)
    {
      int paddingContext = 0;
      double padH = 0.0;
      double padV = 0.0;
      bool flag = false;
      PaddingSharedInfo paddings = paddingInfo;
      if (paddingInfo != null)
      {
        paddingContext = paddingInfo.PaddingContext;
        padH = paddingInfo.PadH;
        padV = paddingInfo.PadV;
      }
      if ((paddingContext & 4) == 0)
      {
        string str = (string) style[(byte) 17];
        if (str != null)
        {
          RPLReportSize rplReportSize = new RPLReportSize(str);
          flag = true;
          paddingContext |= 4;
          padV += rplReportSize.ToMillimeters();
        }
      }
      if ((paddingContext & 8) == 0)
      {
        flag = true;
        string str = (string) style[(byte) 18];
        if (str != null)
        {
          RPLReportSize rplReportSize = new RPLReportSize(str);
          paddingContext |= 8;
          padV += rplReportSize.ToMillimeters();
        }
      }
      if ((paddingContext & 1) == 0)
      {
        flag = true;
        string str = (string) style[(byte) 15];
        if (str != null)
        {
          RPLReportSize rplReportSize = new RPLReportSize(str);
          paddingContext |= 1;
          padH += rplReportSize.ToMillimeters();
        }
      }
      if ((paddingContext & 2) == 0)
      {
        flag = true;
        string str = (string) style[(byte) 16];
        if (str != null)
        {
          RPLReportSize rplReportSize = new RPLReportSize(str);
          paddingContext |= 2;
          padH += rplReportSize.ToMillimeters();
        }
      }
      if (flag)
        paddings = new PaddingSharedInfo(paddingContext, padH, padV);
      return paddings;
    }

    protected bool NeedReportItemId(RPLElement repItem, RPLElementProps props)
    {
      if (this.m_pageSection != HTML4Renderer.PageSection.Body)
        return false;
      bool flag = this.m_linkToChildStack.Count > 0 && props.Definition.ID.Equals(this.m_linkToChildStack.Peek());
      if (flag)
        this.m_linkToChildStack.Pop();
      RPLItemProps rplItemProps = props as RPLItemProps;
      RPLItemPropsDef definition = ((RPLElementProps) rplItemProps).Definition as RPLItemPropsDef;
      string str1 = rplItemProps.Bookmark ?? definition.Bookmark;
      string str2 = rplItemProps.Label ?? definition.Label;
      return str1 != null || str2 != null || flag;
    }

    protected void RenderHtmlBody()
    {
      int num = 0;
      this.m_isBody = true;
      this.m_hasOnePage = this.m_spbProcessing.Done || this.m_pageNum != 0;
      this.RenderPageStart(true, this.m_spbProcessing.Done, this.m_pageContent.PageLayout.Style);
      this.m_pageSection = HTML4Renderer.PageSection.Body;
      bool flag1 = this.m_rplReport != null;
      while (flag1)
      {
        bool flag2 = this.m_pageContent.ReportSectionSizes.Length > 1 || this.m_rplReportSection.Header != null || this.m_rplReportSection.Footer != null;
        if (flag2)
        {
          this.WriteStream(HTML4Renderer.m_openTable);
          this.WriteStream(HTML4Renderer.m_closeBracket);
        }
        while (this.m_rplReportSection != null)
        {
          int borderContext = 0;
          RPLItemMeasurement header = this.m_rplReportSection.Header;
          RPLItemMeasurement footer = this.m_rplReportSection.Footer;
          StyleContext styleContext = new StyleContext();
          RPLItemMeasurement column = this.m_rplReportSection.Columns[0];
          RPLBody element = column.Element as RPLBody;
          RPLItemProps elementProps = ((RPLElement) element).ElementProps as RPLItemProps;
          RPLItemPropsDef definition = ((RPLElementProps) elementProps).Definition as RPLItemPropsDef;
          if (flag2)
          {
            if (header != null)
            {
              this.m_pageSection = HTML4Renderer.PageSection.PageHeader;
              this.m_isBody = false;
              this.RenderPageHeaderFooter(header);
              this.m_isBody = true;
            }
            this.WriteStream(HTML4Renderer.m_firstTD);
            styleContext.StyleOnCell = true;
            this.RenderReportItemStyle((RPLElement) element, (RPLElementProps) elementProps, (RPLElementPropsDef) definition, (RPLItemMeasurement) null, styleContext, ref borderContext, ((RPLElementPropsDef) definition).ID + "c");
            styleContext.StyleOnCell = false;
            this.WriteStream(HTML4Renderer.m_closeBracket);
          }
          this.m_pageSection = HTML4Renderer.PageSection.Body;
          this.m_isBody = true;
          RPLItemMeasurement measurement = new RPLItemMeasurement();
          ((RPLSizes) measurement).Width = this.m_pageContent.MaxSectionWidth;
          ((RPLSizes) measurement).Height = this.m_rplReportSection.BodyArea.Height;
          this.RenderRectangle((RPLContainer) element, (RPLElementProps) elementProps, (RPLElementPropsDef) definition, measurement, ref borderContext, false, styleContext);
          if (flag2)
          {
            this.WriteStream(HTML4Renderer.m_closeTD);
            this.WriteStream(HTML4Renderer.m_closeTR);
            if (footer != null)
            {
              this.m_pageSection = HTML4Renderer.PageSection.PageFooter;
              this.m_isBody = false;
              this.RenderPageHeaderFooter(footer);
              this.m_isBody = true;
            }
          }
          this.m_rplReportSection = this.m_pageContent.GetNextReportSection();
          column.Element = (RPLItem) null;
        }
        if (flag2)
          this.WriteStream(HTML4Renderer.m_closeTable);
        this.RenderPageEnd();
        if (this.m_pageNum == 0)
        {
          if (!this.m_spbProcessing.Done)
          {
            if (this.m_rplReport != null)
              this.m_rplReport.Release();
            RPLReport nextPage = this.GetNextPage();
            this.m_pageContent = nextPage.RPLPaginatedPages[0];
            this.m_rplReportSection = this.m_pageContent.GetNextReportSection();
            this.m_rplReport = nextPage;
            this.WriteStream(HTML4Renderer.m_pageBreakDelimiter);
            this.RenderPageStart(false, this.m_spbProcessing.Done, this.m_pageContent.PageLayout.Style);
            num = 0;
          }
          else
            flag1 = false;
        }
        else
          flag1 = false;
      }
      if (this.m_rplReport == null)
        return;
      this.m_rplReport.Release();
    }

    protected abstract void WriteScrollbars();

    protected abstract void WriteFixedHeaderOnScrollScript();

    protected abstract void WriteFixedHeaderPropertyChangeScript();

    protected virtual bool FillPageHeight => this.m_deviceInfo.IsBrowserIE;

    protected virtual void RenderPageStart(
      bool firstPage,
      bool lastPage,
      RPLElementStyle pageStyle)
    {
      this.WriteStream(HTML4Renderer.m_openDiv);
      this.WriteStream(HTML4Renderer.m_ltrDir);
      this.RenderPageStartDimensionStyles(lastPage);
      if (firstPage)
        this.RenderReportItemId("oReportDiv");
      bool flag = this.m_hasOnePage && this.m_deviceInfo.AllowScript && this.m_deviceInfo.HTMLFragment;
      if (flag)
        this.WriteFixedHeaderOnScrollScript();
      if (this.m_pageHasStyle)
      {
        this.WriteStream(HTML4Renderer.m_closeBracket);
        this.WriteStream(HTML4Renderer.m_openDiv);
        this.OpenStyle();
        if (this.FillPageHeight)
        {
          this.WriteStream(HTML4Renderer.m_styleHeight);
          this.WriteStream(HTML4Renderer.m_percent);
          this.WriteStream(HTML4Renderer.m_semiColon);
        }
        this.WriteStream(HTML4Renderer.m_styleWidth);
        this.WriteStream(HTML4Renderer.m_percent);
        this.WriteStream(HTML4Renderer.m_semiColon);
        this.RenderPageStyle(pageStyle);
        this.CloseStyle(true);
      }
      this.WriteStream(HTML4Renderer.m_closeBracket);
      this.WriteStream(HTML4Renderer.m_openTable);
      this.WriteStream(HTML4Renderer.m_closeBracket);
      this.WriteStream(HTML4Renderer.m_firstTD);
      if (firstPage)
        this.RenderReportItemId("oReportCell");
      this.RenderZoom();
      if (flag)
        this.WriteFixedHeaderPropertyChangeScript();
      this.WriteStream(HTML4Renderer.m_closeBracket);
    }

    protected virtual void RenderPageStartDimensionStyles(bool lastPage)
    {
      if (this.m_pageNum != 0 || lastPage)
      {
        this.WriteStream(HTML4Renderer.m_openStyle);
        this.WriteScrollbars();
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
      }
      else
      {
        this.OpenStyle();
        this.WriteStream("direction:ltr");
        this.CloseStyle(true);
      }
    }

    private void RenderPageStyle(RPLElementStyle style)
    {
      int borderContext1 = 0;
      if (this.m_useInlineStyle)
      {
        this.OpenStyle();
        this.RenderBackgroundStyleProps((IRPLStyle) style);
        this.RenderHtmlBorders((IRPLStyle) style, ref borderContext1, (byte) 0, true, true, (IRPLStyle) null);
        this.CloseStyle(true);
      }
      else
      {
        RPLStyleProps sharedProperties1 = style.SharedProperties;
        RPLStyleProps sharedProperties2 = style.NonSharedProperties;
        if (sharedProperties1 != null && sharedProperties1.Count > 0)
        {
          this.CloseStyle(true);
          string str = "p";
          byte[] styleBytes = (byte[]) this.m_usedStyles[(object) str];
          if (styleBytes == null)
          {
            styleBytes = this.m_encoding.GetBytes(str);
            this.m_usedStyles.Add((object) str, (object) styleBytes);
            if (this.m_onlyVisibleStyles)
            {
              Stream mainStream = this.m_mainStream;
              this.m_mainStream = this.m_styleStream;
              this.RenderOpenStyle(str);
              this.RenderBackgroundStyleProps((IRPLStyle) sharedProperties1);
              this.RenderHtmlBorders((IRPLStyle) sharedProperties1, ref borderContext1, (byte) 0, true, true, (IRPLStyle) null);
              this.WriteStream(HTML4Renderer.m_closeAccol);
              this.m_mainStream = mainStream;
            }
          }
          this.WriteClassStyle(styleBytes, true);
        }
        if (sharedProperties2 == null || sharedProperties2.Count <= 0)
          return;
        this.OpenStyle();
        int borderContext2 = 0;
        this.RenderHtmlBorders((IRPLStyle) sharedProperties2, ref borderContext2, (byte) 0, true, true, (IRPLStyle) sharedProperties1);
        this.RenderBackgroundStyleProps((IRPLStyle) sharedProperties2);
        this.CloseStyle(true);
      }
    }

    protected void OpenStyle()
    {
      if (this.m_isStyleOpen)
        return;
      this.m_isStyleOpen = true;
      this.WriteStream(HTML4Renderer.m_openStyle);
    }

    protected void CloseStyle(bool renderQuote)
    {
      if (this.m_isStyleOpen && renderQuote)
        this.WriteStream(HTML4Renderer.m_quote);
      this.m_isStyleOpen = false;
    }

    internal void WriteClassName(byte[] className, byte[] classNameIfNoPrefix)
    {
      if (this.m_deviceInfo.HtmlPrefixId.Length > 0 || classNameIfNoPrefix == null)
      {
        this.WriteStream(HTML4Renderer.m_classStyle);
        this.WriteAttrEncoded(this.m_deviceInfo.HtmlPrefixId);
        this.WriteStream(className);
        this.WriteStream(HTML4Renderer.m_quote);
      }
      else
        this.WriteStream(classNameIfNoPrefix);
    }

    protected virtual void WriteClassStyle(byte[] styleBytes, bool close)
    {
      this.WriteStream(HTML4Renderer.m_classStyle);
      this.WriteStream(this.m_stylePrefixIdBytes);
      this.WriteStream(styleBytes);
      if (!close)
        return;
      this.WriteStream(HTML4Renderer.m_quote);
    }

    protected void RenderBackgroundStyleProps(IRPLStyle style)
    {
      object theString1 = style[(byte) 34];
      if (theString1 != null)
      {
        this.WriteStream(HTML4Renderer.m_backgroundColor);
        this.WriteStream(theString1);
        this.WriteStream(HTML4Renderer.m_semiColon);
      }
      object image = style[(byte) 33];
      if (image != null)
      {
        this.WriteStream(HTML4Renderer.m_backgroundImage);
        this.RenderImageUrl(true, (RPLImageData) image);
        this.WriteStream(HTML4Renderer.m_closeBrace);
        this.WriteStream(HTML4Renderer.m_semiColon);
      }
      object val = style[(byte) 35];
      if (val == null)
        return;
      object theString2 = (object) EnumStrings.GetValue((RPLFormat.BackgroundRepeatTypes) val);
      this.WriteStream(HTML4Renderer.m_backgroundRepeat);
      this.WriteStream(theString2);
      this.WriteStream(HTML4Renderer.m_semiColon);
    }

    protected virtual void RenderPageEnd()
    {
      if (this.m_deviceInfo.ExpandContent)
      {
        this.WriteStream(HTML4Renderer.m_lastTD);
        this.WriteStream(HTML4Renderer.m_closeTable);
      }
      else
      {
        this.WriteStream(HTML4Renderer.m_closeTD);
        this.WriteStream(HTML4Renderer.m_openTD);
        this.WriteStream(HTML4Renderer.m_inlineWidth);
        this.WriteStream(HTML4Renderer.m_percent);
        this.WriteStream(HTML4Renderer.m_quote);
        this.WriteStream(HTML4Renderer.m_inlineHeight);
        this.WriteStream("0");
        this.WriteStream(HTML4Renderer.m_closeQuote);
        this.WriteStream(HTML4Renderer.m_lastTD);
        this.WriteStream(HTML4Renderer.m_firstTD);
        this.WriteStream(HTML4Renderer.m_inlineWidth);
        if (this.m_deviceInfo.IsBrowserGeckoEngine)
          this.WriteStream(HTML4Renderer.m_percent);
        else
          this.WriteStream("0");
        this.WriteStream(HTML4Renderer.m_quote);
        this.WriteStream(HTML4Renderer.m_inlineHeight);
        this.WriteStream(HTML4Renderer.m_percent);
        this.WriteStream(HTML4Renderer.m_closeQuote);
        this.WriteStream(HTML4Renderer.m_lastTD);
        this.WriteStream(HTML4Renderer.m_closeTable);
      }
      if (this.m_pageHasStyle)
        this.WriteStream(HTML4Renderer.m_closeDiv);
      this.WriteStream(HTML4Renderer.m_closeDiv);
    }

    public virtual void WriteStream(string theString)
    {
      if (theString.Length == 0)
        return;
      byte[] bytes = this.m_encoding.GetBytes(theString);
      this.m_mainStream.Write(bytes, 0, bytes.Length);
    }

    internal void WriteStream(object theString)
    {
      if (theString == null)
        return;
      this.WriteStream(theString.ToString());
    }

    public virtual void WriteStream(byte[] theBytes)
    {
      this.m_mainStream.Write(theBytes, 0, theBytes.Length);
    }

    protected void WriteStreamCR(string theString) => this.WriteStream(theString);

    protected void WriteStreamCR(byte[] theBytes) => this.WriteStream(theBytes);

    protected void WriteStreamEncoded(string theString)
    {
      this.WriteStream(HttpUtility.HtmlEncode(theString));
    }

    protected void WriteAttrEncoded(byte[] attributeName, string theString)
    {
      this.WriteAttribute(attributeName, this.m_encoding.GetBytes(HttpUtility.HtmlAttributeEncode(theString)));
    }

    protected virtual void WriteAttribute(byte[] attributeName, byte[] value)
    {
      this.WriteStream(attributeName);
      this.WriteStream(value);
      this.WriteStream(HTML4Renderer.m_quote);
    }

    protected void WriteAttrEncoded(string theString)
    {
      this.WriteStream(HttpUtility.HtmlAttributeEncode(theString));
    }

    protected void WriteStreamCREncoded(string theString)
    {
      this.WriteStream(HttpUtility.HtmlEncode(theString));
    }

    protected virtual void WriteStreamLineBreak()
    {
    }

    protected void WriteRSStream(float size)
    {
      this.WriteStream(size.ToString("f2", (IFormatProvider) CultureInfo.InvariantCulture));
      this.WriteStream(HTML4Renderer.m_mm);
    }

    protected void WriteRSStreamCR(float size)
    {
      this.WriteStream(size.ToString("f2", (IFormatProvider) CultureInfo.InvariantCulture));
      this.WriteStreamCR(HTML4Renderer.m_mm);
    }

    protected void WriteDStream(float size)
    {
      this.WriteStream(size.ToString("f2", (IFormatProvider) CultureInfo.InvariantCulture));
    }

    private void WriteIdToSecondaryStream(Stream secondaryStream, string tagId)
    {
      Stream mainStream = this.m_mainStream;
      this.m_mainStream = secondaryStream;
      this.WriteReportItemId(tagId);
      this.WriteStream((object) ',');
      this.m_mainStream = mainStream;
    }

    internal static void QuoteString(StringBuilder output, string input)
    {
      if (output == null)
        return;
      switch (input)
      {
        case null:
          break;
        case "":
          break;
        default:
          int length = output.Length;
          output.Append(input);
          for (; length < output.Length; ++length)
          {
            if (output[length] == '\\' || output[length] == '"' || output[length] == '\'')
            {
              output.Insert(length, '\\');
              ++length;
            }
          }
          break;
      }
    }

    protected byte[] RenderSharedStyle(
      RPLElement reportItem,
      RPLElementProps props,
      RPLElementPropsDef definition,
      RPLStyleProps sharedStyle,
      RPLItemMeasurement measurement,
      string id,
      StyleContext styleContext,
      ref int borderContext)
    {
      return this.RenderSharedStyle(reportItem, props, definition, sharedStyle, (RPLStyleProps) null, measurement, id, styleContext, ref borderContext);
    }

    protected byte[] RenderSharedStyle(
      RPLElement reportItem,
      RPLElementProps props,
      RPLElementPropsDef definition,
      RPLStyleProps sharedStyle,
      RPLStyleProps nonSharedStyle,
      RPLItemMeasurement measurement,
      string id,
      StyleContext styleContext,
      ref int borderContext)
    {
      Stream mainStream = this.m_mainStream;
      this.m_mainStream = this.m_styleStream;
      this.RenderOpenStyle(id);
      byte omitBordersState = styleContext.OmitBordersState;
      styleContext.OmitBordersState = (byte) 0;
      this.RenderStyleProps(reportItem, props, definition, measurement, (IRPLStyle) sharedStyle, (IRPLStyle) nonSharedStyle, styleContext, ref borderContext, false);
      styleContext.OmitBordersState = omitBordersState;
      this.WriteStream(HTML4Renderer.m_closeAccol);
      this.m_mainStream = mainStream;
      byte[] bytes = this.m_encoding.GetBytes(id);
      this.m_usedStyles.Add((object) id, (object) bytes);
      return bytes;
    }

    protected byte[] RenderSharedStyle(
      ElementStyleWriter styleWriter,
      RPLStyleProps sharedStyle,
      StyleContext styleContext,
      string id)
    {
      if (sharedStyle == null || id == null)
        return (byte[]) null;
      Stream mainStream = this.m_mainStream;
      this.m_mainStream = this.m_styleStream;
      this.RenderOpenStyle(id);
      byte omitBordersState = styleContext.OmitBordersState;
      styleContext.OmitBordersState = (byte) 0;
      styleWriter.WriteStyles(StyleWriterMode.Shared, (IRPLStyle) sharedStyle);
      styleContext.OmitBordersState = omitBordersState;
      this.WriteStream(HTML4Renderer.m_closeAccol);
      this.m_mainStream = mainStream;
      byte[] bytes = this.m_encoding.GetBytes(id);
      this.m_usedStyles.Add((object) id, (object) bytes);
      return bytes;
    }

    protected void RenderMeasurementStyle(float height, float width)
    {
      this.RenderMeasurementStyle(height, width, false);
    }

    protected void RenderMeasurementStyle(float height, float width, bool renderMin)
    {
      this.RenderMeasurementHeight(height, renderMin);
      this.RenderMeasurementWidth(width, true);
    }

    protected void RenderMeasurementHeight(float height, bool renderMin)
    {
      if (renderMin)
        this.WriteStream(HTML4Renderer.m_styleMinHeight);
      else
        this.WriteStream(HTML4Renderer.m_styleHeight);
      this.WriteRSStream(height);
      this.WriteStream(HTML4Renderer.m_semiColon);
    }

    protected void RenderMeasurementMinHeight(float height)
    {
      this.WriteStream(HTML4Renderer.m_styleMinHeight);
      this.WriteRSStream(height);
      this.WriteStream(HTML4Renderer.m_semiColon);
    }

    protected void RenderMeasurementWidth(float width, bool renderMinWidth)
    {
      this.WriteStream(HTML4Renderer.m_styleWidth);
      this.WriteRSStream(width);
      this.WriteStream(HTML4Renderer.m_semiColon);
      if (!renderMinWidth)
        return;
      this.RenderMeasurementMinWidth(width);
    }

    protected void RenderMeasurementMinWidth(float minWidth)
    {
      this.WriteStream(HTML4Renderer.m_styleMinWidth);
      this.WriteRSStream(minWidth);
      this.WriteStream(HTML4Renderer.m_semiColon);
    }

    protected void RenderMeasurementHeight(float height)
    {
      this.RenderMeasurementHeight(height, false);
    }

    protected void RenderMeasurementWidth(float width) => this.RenderMeasurementWidth(width, false);

    private bool ReportPageHasBorder(IRPLStyle style, string backgroundColor)
    {
      bool flag = this.ReportPageBorder(style, HTML4Renderer.Border.All, backgroundColor);
      if (!flag)
      {
        flag = this.ReportPageBorder(style, HTML4Renderer.Border.Left, backgroundColor);
        if (!flag)
        {
          flag = this.ReportPageBorder(style, HTML4Renderer.Border.Right, backgroundColor);
          if (!flag)
          {
            flag = this.ReportPageBorder(style, HTML4Renderer.Border.Bottom, backgroundColor);
            if (!flag)
              flag = this.ReportPageBorder(style, HTML4Renderer.Border.Top, backgroundColor);
          }
        }
      }
      return flag;
    }

    protected virtual void RenderDynamicImageSrc(RPLDynamicImageProps dynamicImageProps)
    {
      string theString = (string) null;
      string streamName = dynamicImageProps.StreamName;
      if (streamName != null)
        theString = this.m_report.GetStreamUrl(true, streamName);
      if (theString == null)
        return;
      this.WriteStream(theString);
    }

    protected void RenderHtmlBorders(
      IRPLStyle styleProps,
      ref int borderContext,
      byte omitBordersState,
      bool renderPadding,
      bool isNonShared,
      IRPLStyle sharedStyleProps)
    {
      if (renderPadding)
        this.RenderPaddingStyle(styleProps);
      if (styleProps == null || borderContext == 15)
        return;
      object styleProp1 = styleProps[(byte) 10];
      object styleProp2 = styleProps[(byte) 5];
      object styleProp3 = styleProps[(byte) 0];
      IRPLStyle irplStyle = styleProps;
      if (isNonShared && sharedStyleProps != null && !this.OnlyGeneralBorder(sharedStyleProps) && !this.OnlyGeneralBorder(styleProps))
        irplStyle = (IRPLStyle) new RPLElementStyle(styleProps as RPLStyleProps, sharedStyleProps as RPLStyleProps);
      if (borderContext != 0 || omitBordersState != (byte) 0 || !this.OnlyGeneralBorder(irplStyle))
      {
        if (styleProp2 == null || (RPLFormat.BorderStyles) styleProp2 == null)
          this.RenderBorderStyle(styleProp1, styleProp2, styleProp3, HTML4Renderer.Border.All);
        if ((borderContext & 8) == 0 && ((int) omitBordersState & 2) == 0 && this.RenderBorderInstance(irplStyle, styleProp1, styleProp2, styleProp3, HTML4Renderer.Border.Bottom))
          borderContext |= 8;
        if ((borderContext & 1) == 0 && ((int) omitBordersState & 4) == 0 && this.RenderBorderInstance(irplStyle, styleProp1, styleProp2, styleProp3, HTML4Renderer.Border.Left))
          borderContext |= 1;
        if ((borderContext & 2) == 0 && ((int) omitBordersState & 8) == 0 && this.RenderBorderInstance(irplStyle, styleProp1, styleProp2, styleProp3, HTML4Renderer.Border.Right))
          borderContext |= 2;
        if ((borderContext & 4) != 0 || ((int) omitBordersState & 1) != 0 || !this.RenderBorderInstance(irplStyle, styleProp1, styleProp2, styleProp3, HTML4Renderer.Border.Top))
          return;
        borderContext |= 4;
      }
      else
      {
        if (styleProp2 != null && (RPLFormat.BorderStyles) styleProp2 != null)
          borderContext = 15;
        this.RenderBorderStyle(styleProp1, styleProp2, styleProp3, HTML4Renderer.Border.All);
      }
    }

    protected void RenderPaddingStyle(IRPLStyle styleProps)
    {
      if (styleProps == null)
        return;
      object styleProp1 = styleProps[(byte) 15];
      if (styleProp1 != null)
      {
        this.WriteStream(HTML4Renderer.m_paddingLeft);
        this.WriteStream(styleProp1);
        this.WriteStream(HTML4Renderer.m_semiColon);
      }
      object styleProp2 = styleProps[(byte) 17];
      if (styleProp2 != null)
      {
        this.WriteStream(HTML4Renderer.m_paddingTop);
        this.WriteStream(styleProp2);
        this.WriteStream(HTML4Renderer.m_semiColon);
      }
      object styleProp3 = styleProps[(byte) 16];
      if (styleProp3 != null)
      {
        this.WriteStream(HTML4Renderer.m_paddingRight);
        this.WriteStream(styleProp3);
        this.WriteStream(HTML4Renderer.m_semiColon);
      }
      object styleProp4 = styleProps[(byte) 18];
      if (styleProp4 == null)
        return;
      this.WriteStream(HTML4Renderer.m_paddingBottom);
      this.WriteStream(styleProp4);
      this.WriteStream(HTML4Renderer.m_semiColon);
    }

    protected void RenderMultiLineText(string text)
    {
      if (text == null)
        return;
      int num = 0;
      int startIndex = 0;
      int length = text.Length;
      for (int index = 0; index < length; ++index)
      {
        switch (text[index])
        {
          case '\n':
            string theString = text.Substring(startIndex, num - startIndex);
            if (!string.IsNullOrEmpty(theString.Trim()))
              this.WriteStreamEncoded(theString);
            this.WriteStreamCR(HTML4Renderer.m_br);
            startIndex = num + 1;
            break;
          case '\r':
            this.WriteStreamEncoded(text.Substring(startIndex, num - startIndex));
            startIndex = num + 1;
            break;
        }
        ++num;
      }
      if (startIndex == 0)
        this.WriteStreamEncoded(text);
      else
        this.WriteStreamEncoded(text.Substring(startIndex, num - startIndex));
    }

    protected bool IsLineSlanted(RPLItemMeasurement measurement)
    {
      return measurement != null && (double) ((RPLSizes) measurement).Width != 0.0 && (double) ((RPLSizes) measurement).Height != 0.0;
    }

    protected void RenderCellItem(PageTableCell currCell, int borderContext, bool layoutExpand)
    {
      int borderContext1 = borderContext;
      RPLItemMeasurement measurement = currCell.Measurement;
      RPLItem element = measurement.Element;
      if (element == null)
        return;
      RPLItemProps elementProps = ((RPLElement) element).ElementProps as RPLItemProps;
      RPLItemPropsDef definition1 = ((RPLElementProps) elementProps).Definition as RPLItemPropsDef;
      bool renderId = this.NeedReportItemId((RPLElement) measurement.Element, (RPLElementProps) elementProps);
      bool flag = false;
      if (elementProps is RPLImageProps && ((RPLImagePropsDef) definition1).Sizing == 2)
        flag = true;
      if (!flag && currCell.ConsumedByEmptyWhiteSpace)
      {
        if (elementProps is RPLImageProps)
        {
          RPLImageProps rplImageProps = (RPLImageProps) elementProps;
          RPLImagePropsDef definition2 = (RPLImagePropsDef) ((RPLElementProps) elementProps).Definition;
          if (rplImageProps != null)
          {
            Rectangle consolidationOffsets = rplImageProps.Image.ImageConsolidationOffsets;
            if (!rplImageProps.Image.ImageConsolidationOffsets.IsEmpty)
              flag = true;
          }
        }
        if (!flag && elementProps is RPLDynamicImageProps)
        {
          RPLDynamicImageProps dynamicImageProps = (RPLDynamicImageProps) elementProps;
          if (dynamicImageProps != null)
          {
            Rectangle consolidationOffsets = dynamicImageProps.ImageConsolidationOffsets;
            if (!dynamicImageProps.ImageConsolidationOffsets.IsEmpty)
              flag = true;
          }
        }
      }
      if (flag)
      {
        this.WriteStream(HTML4Renderer.m_openDiv);
        this.OpenStyle();
        if (RoundedFloat.op_GreaterThan(currCell.DXValue, ((RPLSizes) measurement).Width))
          this.RenderMeasurementWidth(((RPLSizes) measurement).Width);
        if (RoundedFloat.op_GreaterThan(currCell.DYValue, ((RPLSizes) measurement).Height))
          this.RenderMeasurementHeight(((RPLSizes) measurement).Height);
        this.CloseStyle(true);
        this.WriteStream(HTML4Renderer.m_closeBracket);
      }
      this.RenderReportItem((RPLElement) element, (RPLElementProps) elementProps, (RPLElementPropsDef) definition1, measurement, new StyleContext(), borderContext1, renderId);
      if (flag)
        this.WriteStream(HTML4Renderer.m_closeDiv);
      measurement.Element = (RPLItem) null;
    }

    protected virtual void RenderBlankImage()
    {
      this.WriteStream(HTML4Renderer.m_img);
      if (this.m_browserIE)
        this.WriteStream(HTML4Renderer.m_imgOnError);
      this.WriteStream(HTML4Renderer.m_src);
      this.RenderInternalImageSrc();
      this.WriteStream(this.m_report.GetImageName("Blank.gif"));
      this.WriteStream(HTML4Renderer.m_closeTag);
    }

    protected virtual void RenderImageUrl(bool useSessionId, RPLImageData image)
    {
      string imageStream = this.CreateImageStream(image);
      string theString = (string) null;
      if (imageStream != null)
        theString = this.m_report.GetStreamUrl(useSessionId, imageStream);
      if (theString == null)
        return;
      this.WriteStream(theString);
    }

    protected virtual void RenderReportItemId(string repItemId)
    {
      this.WriteStream(HTML4Renderer.m_id);
      this.WriteReportItemId(repItemId);
      this.WriteStream(HTML4Renderer.m_quote);
    }

    private void WriteReportItemId(string repItemId)
    {
      this.WriteAttrEncoded(this.m_deviceInfo.HtmlPrefixId);
      this.WriteStream(repItemId);
    }

    protected void RenderTextBox(
      RPLTextBox textBox,
      RPLTextBoxProps textBoxProps,
      RPLTextBoxPropsDef textBoxPropsDef,
      RPLItemMeasurement measurement,
      StyleContext styleContext,
      ref int borderContext,
      bool renderId)
    {
      string textBoxValue = (string) null;
      bool flag1 = false;
      bool flag2 = false;
      bool flag3 = false;
      RPLStyleProps actionStyle = (RPLStyleProps) null;
      RPLActionInfo actionInfo = textBoxProps.ActionInfo;
      RPLElementStyle style = ((RPLElementProps) textBoxProps).Style;
      bool flag4 = this.CanSort(textBoxPropsDef);
      bool flag5 = this.NeedSharedToggleParent(textBoxProps);
      bool flag6 = false;
      bool isSimple = textBoxPropsDef.IsSimple;
      bool flag7 = !isSimple && flag5;
      bool flag8 = flag4 || flag7;
      bool flag9 = HTML4Renderer.IsDirectionRTL((IRPLStyle) style);
      RPLStyleProps nonSharedStyle = ((RPLElementProps) textBoxProps).NonSharedStyle;
      RPLStyleProps sharedStyle = ((RPLElementPropsDef) textBoxPropsDef).SharedStyle;
      bool flag10 = HTML4Renderer.IsWritingModeVertical((IRPLStyle) style);
      bool flag11 = flag10 && this.m_deviceInfo.IsBrowserIE;
      bool ignoreVerticalAlign = styleContext.IgnoreVerticalAlign;
      if (isSimple)
      {
        textBoxValue = textBoxProps.Value;
        if (string.IsNullOrEmpty(textBoxValue))
          textBoxValue = textBoxPropsDef.Value;
        if (string.IsNullOrEmpty(textBoxValue) && !flag4 && !flag5)
          flag1 = true;
      }
      if (((RPLElementProps) textBoxProps).UniqueName == null)
      {
        flag4 = false;
        flag5 = false;
        renderId = false;
      }
      float adjustedWidth = this.GetAdjustedWidth(measurement, (IRPLStyle) ((RPLElementProps) textBoxProps).Style);
      float adjustedHeight = this.GetAdjustedHeight(measurement, (IRPLStyle) ((RPLElementProps) textBoxProps).Style);
      if (flag1)
      {
        styleContext.EmptyTextBox = true;
        this.WriteStream(HTML4Renderer.m_openTable);
        this.RenderReportLanguage();
        this.WriteStream(HTML4Renderer.m_closeBracket);
        this.WriteStream(HTML4Renderer.m_firstTD);
        if (this.m_deviceInfo.IsBrowserGeckoEngine)
          this.WriteStream(HTML4Renderer.m_openDiv);
        this.OpenStyle();
        float width = ((RPLSizes) measurement).Width;
        float height = ((RPLSizes) measurement).Height;
        if (this.m_deviceInfo.IsBrowserIE6Or7StandardsMode)
        {
          width = adjustedWidth;
          height = adjustedHeight;
        }
        this.RenderMeasurementWidth(width, false);
        this.RenderMeasurementMinWidth(adjustedWidth);
        if (!textBoxPropsDef.CanShrink)
          this.RenderMeasurementHeight(height);
      }
      else
      {
        if (flag11)
        {
          this.WriteStream(HTML4Renderer.m_openDiv);
          this.OpenStyle();
          this.RenderDirectionStyles((RPLElement) textBox, (RPLElementProps) textBoxProps, (RPLElementPropsDef) textBoxPropsDef, (RPLItemMeasurement) null, (IRPLStyle) ((RPLElementProps) textBoxProps).Style, (IRPLStyle) nonSharedStyle, false, styleContext);
          if (this.m_deviceInfo.IsBrowserIE6Or7StandardsMode && !textBoxPropsDef.CanShrink)
          {
            this.RenderMeasurementHeight(adjustedHeight);
            this.RenderHtmlBorders((IRPLStyle) ((RPLElementProps) textBoxProps).Style, ref borderContext, styleContext.OmitBordersState, true, true, (IRPLStyle) null);
            styleContext.NoBorders = true;
          }
          this.WriteStream("display: inline;");
          bool flag12 = false;
          if (this.m_deviceInfo.BrowserMode == BrowserMode.Standards)
          {
            this.RenderMeasurementHeight(((RPLSizes) measurement).Height);
            flag12 = true;
          }
          this.CloseStyle(true);
          if (flag12 && this.m_deviceInfo.AllowScript)
          {
            if (!this.m_needsFitVertTextScript)
              this.CreateFitVertTextIdsStream();
            this.WriteIdToSecondaryStream(this.m_fitVertTextIdsStream, ((RPLElementProps) textBoxProps).UniqueName + "_fvt");
            this.RenderReportItemId(((RPLElementProps) textBoxProps).UniqueName + "_fvt");
          }
          this.WriteStream(HTML4Renderer.m_closeBracket);
        }
        object val = style[(byte) 26];
        if (textBoxPropsDef.CanGrow)
        {
          this.WriteStream(HTML4Renderer.m_openTable);
          this.RenderReportLanguage();
          this.OpenStyle();
          if (flag11)
          {
            if (this.m_deviceInfo.IsBrowserIE6Or7StandardsMode)
            {
              this.RenderMeasurementWidth(adjustedWidth, false);
              if (!textBoxPropsDef.CanShrink)
                this.RenderMeasurementHeight(adjustedHeight);
            }
            else
              this.RenderMeasurementWidth(((RPLSizes) measurement).Width, true);
          }
          if (isSimple && (string.IsNullOrEmpty(textBoxValue) || string.IsNullOrEmpty(textBoxValue.Trim())))
            this.WriteStream(HTML4Renderer.m_borderCollapse);
          this.CloseStyle(true);
          this.WriteStream(HTML4Renderer.m_closeBracket);
          this.WriteStream(HTML4Renderer.m_firstTD);
          this.OpenStyle();
          if (this.m_deviceInfo.IsBrowserIE6Or7StandardsMode && !textBoxPropsDef.CanShrink)
            this.RenderMeasurementWidth(adjustedWidth, false);
          else
            this.RenderMeasurementWidth(((RPLSizes) measurement).Width, false);
          this.RenderMeasurementMinWidth(adjustedWidth);
          if (!textBoxPropsDef.CanShrink)
          {
            if (this.m_deviceInfo.IsBrowserIE6Or7StandardsMode || this.m_deviceInfo.IsBrowserSafari && this.m_deviceInfo.BrowserMode != BrowserMode.Quirks)
            {
              if (!flag11)
                this.RenderMeasurementHeight(adjustedHeight);
            }
            else
              this.RenderMeasurementHeight(((RPLSizes) measurement).Height);
          }
          styleContext.RenderMeasurements = false;
          if (flag8)
          {
            styleContext.StyleOnCell = true;
            this.RenderReportItemStyle((RPLElement) textBox, (RPLElementProps) textBoxProps, (RPLElementPropsDef) textBoxPropsDef, nonSharedStyle, sharedStyle, measurement, styleContext, ref borderContext, ((RPLElementPropsDef) textBoxPropsDef).ID + "p");
            styleContext.StyleOnCell = false;
            styleContext.NoBorders = true;
          }
          if (textBoxPropsDef.CanShrink)
          {
            if (flag10 || flag5 && flag9)
              flag2 = true;
            if (!flag2 && val != null && !styleContext.IgnoreVerticalAlign)
            {
              object theString = (object) EnumStrings.GetValue((RPLFormat.VerticalAlignments) val);
              this.WriteStream(HTML4Renderer.m_verticalAlign);
              this.WriteStream(theString);
              this.WriteStream(HTML4Renderer.m_semiColon);
            }
            this.CloseStyle(true);
            this.WriteStreamCR(HTML4Renderer.m_closeBracket);
            if (flag2)
            {
              this.WriteStream(HTML4Renderer.m_openTable);
              this.WriteStream(HTML4Renderer.m_inlineWidth);
              this.WriteStream(HTML4Renderer.m_percent);
              this.WriteStream(HTML4Renderer.m_quote);
              this.WriteStream(HTML4Renderer.m_closeBracket);
              this.WriteStream(HTML4Renderer.m_firstTD);
            }
            else
            {
              this.WriteStream(HTML4Renderer.m_openDiv);
              if (!flag8)
                styleContext.IgnoreVerticalAlign = true;
            }
          }
        }
        else
        {
          this.WriteStream(HTML4Renderer.m_openDiv);
          styleContext.IgnoreVerticalAlign = true;
          if (!this.m_deviceInfo.IsBrowserIE || this.m_deviceInfo.BrowserMode == BrowserMode.Standards || val != null && (RPLFormat.VerticalAlignments) val != null || this.m_deviceInfo.OutlookCompat)
          {
            if (!flag8)
            {
              bool backgroundBorders = styleContext.OnlyRenderMeasurementsBackgroundBorders;
              bool noBorders = styleContext.NoBorders;
              styleContext.OnlyRenderMeasurementsBackgroundBorders = true;
              int borderContext1 = 0;
              if (textBoxPropsDef.CanShrink)
                styleContext.NoBorders = true;
              this.RenderReportItemStyle((RPLElement) textBox, (RPLElementProps) textBoxProps, (RPLElementPropsDef) textBoxPropsDef, nonSharedStyle, sharedStyle, measurement, styleContext, ref borderContext1, ((RPLElementPropsDef) textBoxPropsDef).ID + "v");
              styleContext.OnlyRenderMeasurementsBackgroundBorders = backgroundBorders;
              measurement = (RPLItemMeasurement) null;
              styleContext.NoBorders = !textBoxPropsDef.CanShrink || noBorders;
            }
            this.WriteStreamCR(HTML4Renderer.m_closeBracket);
            styleContext.IgnoreVerticalAlign = ignoreVerticalAlign;
            if (val != null && (RPLFormat.VerticalAlignments) val != null)
            {
              this.WriteStream(HTML4Renderer.m_openTable);
              if (!flag4 || flag10)
              {
                this.WriteStream(HTML4Renderer.m_inlineWidth);
                this.WriteStream(HTML4Renderer.m_percent);
                this.WriteStream(HTML4Renderer.m_quote);
              }
              if (!textBoxPropsDef.CanShrink)
              {
                this.WriteStream(HTML4Renderer.m_inlineHeight);
                this.WriteStream(HTML4Renderer.m_percent);
                this.WriteStream(HTML4Renderer.m_quote);
              }
              this.WriteStream(HTML4Renderer.m_zeroBorder);
              this.WriteStream(HTML4Renderer.m_closeBracket);
              this.WriteStream(HTML4Renderer.m_firstTD);
              flag2 = true;
            }
            else
            {
              this.WriteStream(HTML4Renderer.m_openDiv);
              flag3 = true;
            }
          }
          if (flag8)
          {
            this.OpenStyle();
            if (this.m_deviceInfo.IsBrowserIE6Or7StandardsMode && !textBoxPropsDef.CanShrink)
              this.RenderMeasurementWidth(adjustedWidth, false);
            else
              this.RenderMeasurementWidth(((RPLSizes) measurement).Width, false);
            this.RenderMeasurementMinWidth(adjustedWidth);
            this.WriteStream(HTML4Renderer.m_semiColon);
          }
          if (textBoxPropsDef.CanShrink)
          {
            bool noBorders = styleContext.NoBorders;
            styleContext.NoBorders = true;
            this.RenderReportItemStyle((RPLElement) textBox, (RPLElementProps) textBoxProps, (RPLElementPropsDef) textBoxPropsDef, nonSharedStyle, sharedStyle, measurement, styleContext, ref borderContext, ((RPLElementPropsDef) textBoxPropsDef).ID + "s");
            this.CloseStyle(true);
            this.WriteStreamCR(HTML4Renderer.m_closeBracket);
            this.WriteStream(HTML4Renderer.m_openDiv);
            styleContext.IgnoreVerticalAlign = true;
            styleContext.NoBorders = noBorders;
            styleContext.StyleOnCell = true;
          }
          if (flag8)
          {
            this.RenderReportItemStyle((RPLElement) textBox, (RPLElementProps) textBoxProps, (RPLElementPropsDef) textBoxPropsDef, nonSharedStyle, sharedStyle, measurement, styleContext, ref borderContext, ((RPLElementPropsDef) textBoxPropsDef).ID + "p");
            styleContext.StyleOnCell = false;
          }
        }
      }
      if (flag8)
      {
        styleContext.IgnoreVerticalAlign = ignoreVerticalAlign;
        this.CloseStyle(true);
        this.WriteStreamCR(HTML4Renderer.m_closeBracket);
        this.WriteStream(HTML4Renderer.m_openTable);
        this.WriteStream(HTML4Renderer.m_zeroBorder);
        this.RenderReportLanguage();
        styleContext.RenderMeasurements = false;
        this.WriteStream(HTML4Renderer.m_closeBracket);
        this.WriteStream(HTML4Renderer.m_firstTD);
        if (flag10)
          this.WriteStream(" ROWS='2'");
        this.RenderAtStart(textBoxProps, (IRPLStyle) style, flag4 && flag9, flag7 && !flag9);
        styleContext.InTablix = true;
      }
      string textBoxClass = this.GetTextBoxClass(textBoxPropsDef, textBoxProps, nonSharedStyle, ((RPLElementPropsDef) textBoxPropsDef).ID);
      this.RenderReportItemStyle((RPLElement) textBox, (RPLElementProps) textBoxProps, (RPLElementPropsDef) textBoxPropsDef, nonSharedStyle, sharedStyle, measurement, styleContext, ref borderContext, textBoxClass);
      this.CloseStyle(true);
      styleContext.IgnoreVerticalAlign = ignoreVerticalAlign;
      if (renderId || flag5 || flag4)
        this.RenderReportItemId(((RPLElementProps) textBoxProps).UniqueName);
      this.WriteToolTip((RPLElementProps) textBoxProps);
      if (!flag1)
        this.RenderLanguage((string) style[(byte) 32]);
      this.WriteStreamCR(HTML4Renderer.m_closeBracket);
      if ((!this.m_deviceInfo.IsBrowserIE || this.m_deviceInfo.BrowserMode == BrowserMode.Standards && !this.m_deviceInfo.IsBrowserIE6Or7StandardsMode && !flag10) && isSimple && !string.IsNullOrEmpty(textBoxValue) && !string.IsNullOrEmpty(textBoxValue.Trim()))
      {
        this.WriteStream(HTML4Renderer.m_openDiv);
        if (measurement != null)
        {
          this.OpenStyle();
          float innerContainerWidth = this.GetInnerContainerWidth((RPLMeasurement) measurement, (IRPLStyle) ((RPLElementProps) textBoxProps).Style);
          if (flag4 && !flag9)
            innerContainerWidth -= 4.233333f;
          if ((double) innerContainerWidth > 0.0)
          {
            this.WriteStream(HTML4Renderer.m_styleWidth);
            this.WriteRSStream(innerContainerWidth);
            this.WriteStream(HTML4Renderer.m_semiColon);
          }
          this.CloseStyle(true);
        }
        this.WriteStream(HTML4Renderer.m_closeBracket);
      }
      if (flag5 && isSimple)
        this.RenderToggleImage(textBoxProps);
      RPLAction rplAction = (RPLAction) null;
      if (this.HasAction(actionInfo))
      {
        rplAction = actionInfo.Actions[0];
        this.RenderElementHyperlinkAllTextStyles(((RPLElementProps) textBoxProps).Style, rplAction, ((RPLElementPropsDef) textBoxPropsDef).ID + "a");
        flag6 = true;
        if (flag1)
        {
          this.WriteStream(HTML4Renderer.m_openDiv);
          this.OpenStyle();
          float num = 0.0f;
          if (measurement != null)
            num = ((RPLSizes) measurement).Height;
          if ((double) num > 0.0)
          {
            float heightSubtractBorders = this.GetInnerContainerHeightSubtractBorders(measurement, (IRPLStyle) ((RPLElementProps) textBoxProps).Style);
            if (this.m_deviceInfo.IsBrowserIE && this.m_deviceInfo.BrowserMode == BrowserMode.Quirks)
              this.RenderMeasurementHeight(heightSubtractBorders);
            else
              this.RenderMeasurementMinHeight(heightSubtractBorders);
          }
          this.WriteStream(HTML4Renderer.m_cursorHand);
          this.WriteStream(HTML4Renderer.m_semiColon);
          this.CloseStyle(true);
          this.WriteStream(HTML4Renderer.m_closeBracket);
        }
      }
      this.RenderTextBoxContent(textBox, textBoxProps, textBoxPropsDef, textBoxValue, actionStyle, flag5 || flag4, measurement, rplAction);
      if (flag6)
      {
        if (flag1)
          this.WriteStream(HTML4Renderer.m_closeDiv);
        this.WriteStream(HTML4Renderer.m_closeA);
      }
      if ((!this.m_deviceInfo.IsBrowserIE || this.m_deviceInfo.BrowserMode == BrowserMode.Standards && !this.m_deviceInfo.IsBrowserIE6Or7StandardsMode && !flag10) && isSimple && !string.IsNullOrEmpty(textBoxValue) && !string.IsNullOrEmpty(textBoxValue.Trim()))
        this.WriteStream(HTML4Renderer.m_closeDiv);
      if (flag8)
      {
        this.RenderAtEnd(textBoxProps, (IRPLStyle) style, flag4 && !flag9, flag7 && flag9);
        this.WriteStream(HTML4Renderer.m_lastTD);
        this.WriteStream(HTML4Renderer.m_closeTable);
      }
      if (flag1)
      {
        if (this.m_deviceInfo.IsBrowserGeckoEngine)
          this.WriteStream(HTML4Renderer.m_closeDiv);
        this.WriteStream(HTML4Renderer.m_lastTD);
        this.WriteStream(HTML4Renderer.m_closeTable);
      }
      else
      {
        if (textBoxPropsDef.CanGrow)
        {
          if (textBoxPropsDef.CanShrink)
          {
            if (flag2)
            {
              this.WriteStream(HTML4Renderer.m_lastTD);
              this.WriteStream(HTML4Renderer.m_closeTable);
            }
            else
              this.WriteStream(HTML4Renderer.m_closeDiv);
          }
          this.WriteStream(HTML4Renderer.m_lastTD);
          this.WriteStreamCR(HTML4Renderer.m_closeTable);
        }
        else
        {
          if (flag2)
          {
            this.WriteStream(HTML4Renderer.m_lastTD);
            this.WriteStream(HTML4Renderer.m_closeTable);
          }
          if (flag3)
            this.WriteStream(HTML4Renderer.m_closeDiv);
          this.WriteStreamCR(HTML4Renderer.m_closeDiv);
        }
        if (!flag11)
          return;
        this.WriteStream(HTML4Renderer.m_closeDiv);
      }
    }

    private string GetTextBoxClass(
      RPLTextBoxPropsDef textBoxPropsDef,
      RPLTextBoxProps textBoxProps,
      RPLStyleProps nonSharedStyle,
      string defaultClass)
    {
      if (textBoxPropsDef.SharedTypeCode == TypeCode.Object && (nonSharedStyle == null || nonSharedStyle.Count == 0 || nonSharedStyle[(byte) 25] == null))
      {
        object obj = ((RPLElementProps) textBoxProps).Style[(byte) 25];
        if (obj != null && (RPLFormat.TextAlignments) obj == null)
          return HTML4Renderer.GetTextAlignForType(textBoxProps) ? defaultClass + "r" : defaultClass + "l";
      }
      return defaultClass;
    }

    private void WriteToolTip(RPLElementProps props)
    {
      RPLItemProps rplItemProps = props as RPLItemProps;
      RPLItemPropsDef definition = ((RPLElementProps) rplItemProps).Definition as RPLItemPropsDef;
      string tooltip = rplItemProps.ToolTip ?? definition.ToolTip;
      if (tooltip == null)
        return;
      this.WriteToolTipAttribute(tooltip);
    }

    private void WriteToolTipAttribute(string tooltip)
    {
      this.WriteAttrEncoded(HTML4Renderer.m_alt, tooltip);
      this.WriteAttrEncoded(HTML4Renderer.m_title, tooltip);
    }

    private void WriteOuterConsolidation(
      Rectangle consolidationOffsets,
      RPLFormat.Sizings sizing,
      string propsUniqueName)
    {
      bool flag = false;
      switch (sizing - 1)
      {
        case 0:
          this.WriteStream(" imgConDiv=\"true\"");
          this.m_emitImageConsolidationScaling = true;
          flag = true;
          break;
        case 1:
          this.WriteStream(" imgConFitProp=\"true\"");
          break;
      }
      if (this.m_deviceInfo.AllowScript)
      {
        if (this.m_imgConImageIdsStream == null)
          this.CreateImgConImageIdsStream();
        this.WriteIdToSecondaryStream(this.m_imgConImageIdsStream, propsUniqueName + "_ici");
        this.RenderReportItemId(propsUniqueName + "_ici");
      }
      this.WriteStream(" imgConImage=\"" + sizing.ToString() + "\"");
      if (flag)
      {
        this.WriteStream(" imgConWidth=\"" + (object) consolidationOffsets.Width + "\"");
        this.WriteStream(" imgConHeight=\"" + (object) consolidationOffsets.Height + "\"");
      }
      this.OpenStyle();
      this.WriteStream(HTML4Renderer.m_styleWidth);
      if (flag)
        this.WriteStream("1");
      else
        this.WriteStream((object) consolidationOffsets.Width);
      this.WriteStream(HTML4Renderer.m_px);
      this.WriteStream(HTML4Renderer.m_semiColon);
      this.WriteStream(HTML4Renderer.m_styleHeight);
      if (flag)
        this.WriteStream("1");
      else
        this.WriteStream((object) consolidationOffsets.Height);
      this.WriteStream(HTML4Renderer.m_px);
      this.WriteStream(HTML4Renderer.m_semiColon);
      this.WriteStream(HTML4Renderer.m_overflowHidden);
      this.WriteStream(HTML4Renderer.m_semiColon);
      if (this.m_deviceInfo.BrowserMode != BrowserMode.Standards)
        return;
      this.WriteStream(HTML4Renderer.m_stylePositionAbsolute);
    }

    private void WriteClippedDiv(Rectangle clipCoordinates)
    {
      this.OpenStyle();
      this.WriteStream(HTML4Renderer.m_styleTop);
      if (clipCoordinates.Top > 0)
        this.WriteStream("-");
      this.WriteStream((object) clipCoordinates.Top);
      this.WriteStream(HTML4Renderer.m_px);
      this.WriteStream(HTML4Renderer.m_semiColon);
      this.WriteStream(HTML4Renderer.m_styleLeft);
      if (clipCoordinates.Left > 0)
        this.WriteStream("-");
      this.WriteStream((object) clipCoordinates.Left);
      this.WriteStream(HTML4Renderer.m_px);
      this.WriteStream(HTML4Renderer.m_semiColon);
      this.WriteStream(HTML4Renderer.m_stylePositionRelative);
      this.CloseStyle(true);
    }

    protected void RenderNavigationId(string navigationId)
    {
      if (this.IsFragment)
        return;
      this.WriteStream(HTML4Renderer.m_openSpan);
      this.WriteStream(HTML4Renderer.m_id);
      this.WriteAttrEncoded(this.m_deviceInfo.HtmlPrefixId);
      this.WriteStream(navigationId);
      this.WriteStream(HTML4Renderer.m_closeTag);
    }

    protected void RenderTablix(
      RPLTablix tablix,
      RPLElementProps props,
      RPLElementPropsDef def,
      RPLItemMeasurement measurement,
      StyleContext styleContext,
      ref int borderContext,
      bool renderId)
    {
      string uniqueName1 = props.UniqueName;
      TablixFixedHeaderStorage fixedHeaderStorage = new TablixFixedHeaderStorage();
      if (tablix.ColumnWidths == null)
        tablix.ColumnWidths = new float[0];
      if (tablix.RowHeights == null)
        tablix.RowHeights = new float[0];
      bool flag1 = this.InitFixedColumnHeaders(tablix, uniqueName1, fixedHeaderStorage);
      bool fixedHeader = this.InitFixedRowHeaders(tablix, uniqueName1, fixedHeaderStorage);
      bool flag2 = tablix.ColumnHeaderRows == 0 && tablix.RowHeaderColumns == 0 && !this.m_deviceInfo.AccessibleTablix && this.m_deviceInfo.BrowserMode != BrowserMode.Standards;
      if (flag1 && fixedHeader)
        fixedHeaderStorage.CornerHeaders = new List<string>();
      this.WriteStream(HTML4Renderer.m_openTable);
      int columns = tablix.ColumnHeaderRows > 0 || tablix.RowHeaderColumns > 0 || !flag2 ? tablix.ColumnWidths.Length + 1 : tablix.ColumnWidths.Length;
      this.WriteStream(HTML4Renderer.m_cols);
      this.WriteStream(columns.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      this.WriteStream(HTML4Renderer.m_quote);
      if (renderId || flag1 || fixedHeader)
        this.RenderReportItemId(uniqueName1);
      this.WriteToolTip(((RPLElement) tablix).ElementProps);
      this.WriteStream(HTML4Renderer.m_zeroBorder);
      this.OpenStyle();
      this.WriteStream(HTML4Renderer.m_borderCollapse);
      this.WriteStream(HTML4Renderer.m_semiColon);
      if (this.m_deviceInfo.OutlookCompat && measurement != null)
        this.RenderMeasurementWidth(((RPLSizes) measurement).Width, true);
      this.RenderReportItemStyle((RPLElement) tablix, props, def, measurement, styleContext, ref borderContext, def.ID);
      this.CloseStyle(true);
      this.WriteStream(HTML4Renderer.m_closeBracket);
      int beforeRowHeaders = tablix.ColsBeforeRowHeaders;
      RPLTablixRow nextRow = tablix.GetNextRow();
      List<RPLTablixOmittedRow> omittedRows1 = new List<RPLTablixOmittedRow>();
      for (; nextRow != null && nextRow is RPLTablixOmittedRow; nextRow = tablix.GetNextRow())
        omittedRows1.Add((RPLTablixOmittedRow) nextRow);
      if (flag2)
      {
        this.RenderEmptyTopTablixRow(tablix, omittedRows1, uniqueName1, false, fixedHeaderStorage);
        this.RenderSimpleTablixRows(tablix, uniqueName1, nextRow, borderContext, fixedHeaderStorage);
      }
      else
      {
        styleContext = new StyleContext();
        float[] columnWidths = tablix.ColumnWidths;
        float[] rowHeights = tablix.RowHeights;
        int length1 = columnWidths.Length;
        int length2 = rowHeights.Length;
        this.RenderEmptyTopTablixRow(tablix, omittedRows1, uniqueName1, true, fixedHeaderStorage);
        bool flag3 = flag1;
        int row = 0;
        List<RPLTablixOmittedRow> omittedRows2 = new List<RPLTablixOmittedRow>();
        HTMLHeader[] rowHeaderIds = (HTMLHeader[]) null;
        string[] colHeaderIds = (string[]) null;
        OmittedHeaderStack omittedHeaders1 = (OmittedHeaderStack) null;
        if (this.m_deviceInfo.AccessibleTablix)
        {
          rowHeaderIds = new HTMLHeader[tablix.RowHeaderColumns];
          colHeaderIds = new string[length1];
          omittedHeaders1 = new OmittedHeaderStack();
        }
        while (nextRow != null)
        {
          if (nextRow is RPLTablixOmittedRow)
          {
            omittedRows2.Add((RPLTablixOmittedRow) nextRow);
            nextRow = tablix.GetNextRow();
          }
          else
          {
            if ((double) rowHeights[row] == 0.0 && row > 1 && nextRow.NumCells == 1 && nextRow[0].Element is RPLRectangle)
            {
              RPLRectangle element = (RPLRectangle) nextRow[0].Element;
              if (((RPLContainer) element).Children == null || ((RPLContainer) element).Children.Length == 0)
              {
                nextRow = tablix.GetNextRow();
                ++row;
                continue;
              }
            }
            this.WriteStream(HTML4Renderer.m_openTR);
            if (tablix.FixedRow(row) || fixedHeader || flag3)
            {
              string repItemId = uniqueName1 + "r" + (object) row;
              this.RenderReportItemId(repItemId);
              if (tablix.FixedRow(row))
              {
                fixedHeaderStorage.ColumnHeaders.Add(repItemId);
                if (fixedHeaderStorage.CornerHeaders != null)
                  fixedHeaderStorage.CornerHeaders.Add(repItemId);
              }
              else if (flag3)
              {
                fixedHeaderStorage.BodyID = repItemId;
                flag3 = false;
              }
              if (fixedHeader)
                fixedHeaderStorage.RowHeaders.Add(repItemId);
            }
            this.WriteStream(HTML4Renderer.m_valign);
            this.WriteStream(HTML4Renderer.m_topValue);
            this.WriteStream(HTML4Renderer.m_quote);
            this.WriteStream(HTML4Renderer.m_closeBracket);
            this.RenderEmptyHeightCell(rowHeights[row], uniqueName1, tablix.FixedRow(row), row, fixedHeaderStorage);
            int num1 = 0;
            int numCells = nextRow.NumCells;
            int num2 = numCells;
            if (nextRow.BodyStart == -1)
            {
              int[] omittedIndices = new int[omittedRows2.Count];
              for (int index1 = num1; index1 < num2; ++index1)
              {
                RPLTablixCell cell = nextRow[index1];
                this.RenderColumnHeaderTablixCell(tablix, uniqueName1, length1, cell.ColIndex, cell.ColSpan, row, borderContext, cell, styleContext, fixedHeaderStorage, omittedRows2, omittedIndices);
                if (colHeaderIds != null && row < tablix.ColumnHeaderRows)
                {
                  if (cell is RPLTablixMemberCell)
                  {
                    string uniqueName2 = ((RPLTablixMemberCell) cell).UniqueName;
                    if (uniqueName2 == null && cell.Element != null)
                    {
                      uniqueName2 = ((RPLElement) cell.Element).ElementProps.UniqueName;
                      ((RPLTablixMemberCell) cell).UniqueName = uniqueName2;
                    }
                    if (uniqueName2 != null)
                    {
                      for (int index2 = 0; index2 < cell.ColSpan; ++index2)
                      {
                        string str1 = colHeaderIds[cell.ColIndex + index2];
                        string str2 = str1 != null ? str1 + " " + HttpUtility.HtmlAttributeEncode(this.m_deviceInfo.HtmlPrefixId) + uniqueName2 : HttpUtility.HtmlAttributeEncode(this.m_deviceInfo.HtmlPrefixId) + uniqueName2;
                        colHeaderIds[cell.ColIndex + index2] = str2;
                      }
                    }
                    else
                      continue;
                  }
                }
                nextRow[index1] = (RPLTablixCell) null;
              }
              omittedRows2 = new List<RPLTablixOmittedRow>();
            }
            else
            {
              if (rowHeaderIds != null)
              {
                int headerStart = nextRow.HeaderStart;
                int num3 = 0;
                for (int index = 0; index < rowHeaderIds.Length; ++index)
                {
                  HTMLHeader htmlHeader = rowHeaderIds[index];
                  if (rowHeaderIds[index] != null)
                  {
                    if (rowHeaderIds[index].Span > 1)
                    {
                      --rowHeaderIds[index].Span;
                      continue;
                    }
                  }
                  else
                  {
                    htmlHeader = new HTMLHeader();
                    rowHeaderIds[index] = htmlHeader;
                  }
                  RPLTablixCell cell = nextRow[num3 + headerStart];
                  htmlHeader.ID = this.CalculateRowHeaderId(cell, tablix.FixedColumns[cell.ColIndex], uniqueName1, row, index + tablix.ColsBeforeRowHeaders, (TablixFixedHeaderStorage) null, this.m_deviceInfo.AccessibleTablix, false);
                  htmlHeader.Span = cell.RowSpan;
                  ++num3;
                }
              }
              if (omittedRows2 != null && omittedRows2.Count > 0)
              {
                for (int index = 0; index < omittedRows2.Count; ++index)
                  this.RenderTablixOmittedRow(columns, (RPLTablixRow) omittedRows2[index]);
                omittedRows2 = (List<RPLTablixOmittedRow>) null;
              }
              List<RPLTablixMemberCell> omittedHeaders2 = nextRow.OmittedHeaders;
              if (beforeRowHeaders > 0)
              {
                int omittedIndex = 0;
                int headerStart = nextRow.HeaderStart;
                int bodyStart = nextRow.BodyStart;
                int num4 = headerStart;
                int num5 = bodyStart;
                int col;
                for (col = 0; num5 < num2 && col < beforeRowHeaders; ++num5)
                {
                  RPLTablixCell cell = nextRow[num5];
                  int colSpan = cell.ColSpan;
                  this.RenderTablixCell(tablix, false, uniqueName1, length1, length2, col, colSpan, row, borderContext, cell, omittedHeaders2, ref omittedIndex, styleContext, fixedHeaderStorage, rowHeaderIds, colHeaderIds, omittedHeaders1);
                  col += colSpan;
                  nextRow[num5] = (RPLTablixCell) null;
                }
                int num6 = bodyStart > headerStart ? bodyStart : num2;
                if (num4 >= 0)
                {
                  for (; num4 < num6; ++num4)
                  {
                    RPLTablixCell cell = nextRow[num4];
                    int colSpan = cell.ColSpan;
                    this.RenderTablixCell(tablix, fixedHeader, uniqueName1, length1, length2, col, colSpan, row, borderContext, cell, omittedHeaders2, ref omittedIndex, styleContext, fixedHeaderStorage, rowHeaderIds, colHeaderIds, omittedHeaders1);
                    col += colSpan;
                    nextRow[num4] = (RPLTablixCell) null;
                  }
                }
                int num7 = num5;
                int num8 = bodyStart < headerStart ? headerStart : numCells;
                for (int index = num7; index < num8; ++index)
                {
                  RPLTablixCell cell = nextRow[index];
                  this.RenderTablixCell(tablix, false, uniqueName1, length1, length2, cell.ColIndex, cell.ColSpan, row, borderContext, cell, omittedHeaders2, ref omittedIndex, styleContext, fixedHeaderStorage, rowHeaderIds, colHeaderIds, omittedHeaders1);
                  nextRow[index] = (RPLTablixCell) null;
                }
              }
              else
              {
                int omittedIndex = 0;
                for (int index = num1; index < num2; ++index)
                {
                  RPLTablixCell cell = nextRow[index];
                  int colIndex = cell.ColIndex;
                  this.RenderTablixCell(tablix, tablix.FixedColumns[colIndex], uniqueName1, length1, length2, colIndex, cell.ColSpan, row, borderContext, cell, omittedHeaders2, ref omittedIndex, styleContext, fixedHeaderStorage, rowHeaderIds, colHeaderIds, omittedHeaders1);
                  nextRow[index] = (RPLTablixCell) null;
                }
              }
            }
            this.WriteStream(HTML4Renderer.m_closeTR);
            nextRow = tablix.GetNextRow();
            ++row;
          }
        }
      }
      this.WriteStream(HTML4Renderer.m_closeTable);
      if (!flag1 && !fixedHeader)
        return;
      if (this.m_fixedHeaders == null)
        this.m_fixedHeaders = new ArrayList();
      this.m_fixedHeaders.Add((object) fixedHeaderStorage);
    }

    private void RenderTablixOmittedRow(int columns, RPLTablixRow currentRow)
    {
      int index = 0;
      List<RPLTablixMemberCell> omittedHeaders = currentRow.OmittedHeaders;
      while (index < omittedHeaders.Count && omittedHeaders[index].GroupLabel == null)
        ++index;
      if (index >= omittedHeaders.Count)
        return;
      int num1 = ((RPLTablixCell) omittedHeaders[index]).ColIndex;
      this.WriteStream(HTML4Renderer.m_openTR);
      this.WriteStream(HTML4Renderer.m_zeroHeight);
      this.WriteStream(HTML4Renderer.m_closeBracket);
      this.WriteStream(HTML4Renderer.m_openTD);
      this.WriteStream(HTML4Renderer.m_colSpan);
      this.WriteStream(num1.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      this.WriteStream(HTML4Renderer.m_quote);
      this.WriteStream(HTML4Renderer.m_closeBracket);
      this.WriteStream(HTML4Renderer.m_closeTD);
      for (; index < omittedHeaders.Count; ++index)
      {
        if (omittedHeaders[index].GroupLabel != null)
        {
          this.WriteStream(HTML4Renderer.m_openTD);
          int colIndex = ((RPLTablixCell) omittedHeaders[index]).ColIndex;
          int num2 = colIndex - num1;
          if (num2 > 1)
          {
            this.WriteStream(HTML4Renderer.m_colSpan);
            this.WriteStream(num2.ToString((IFormatProvider) CultureInfo.InvariantCulture));
            this.WriteStream(HTML4Renderer.m_quote);
            this.WriteStream(HTML4Renderer.m_closeBracket);
            this.WriteStream(HTML4Renderer.m_closeTD);
            this.WriteStream(HTML4Renderer.m_openTD);
          }
          int colSpan = ((RPLTablixCell) omittedHeaders[index]).ColSpan;
          if (colSpan > 1)
          {
            this.WriteStream(HTML4Renderer.m_colSpan);
            this.WriteStream(colSpan.ToString((IFormatProvider) CultureInfo.InvariantCulture));
            this.WriteStream(HTML4Renderer.m_quote);
          }
          this.RenderReportItemId(omittedHeaders[index].UniqueName);
          this.WriteStream(HTML4Renderer.m_closeBracket);
          this.WriteStream(HTML4Renderer.m_closeTD);
          num1 = colIndex + colSpan;
        }
      }
      if (num1 < columns)
      {
        this.WriteStream(HTML4Renderer.m_openTD);
        this.WriteStream(HTML4Renderer.m_colSpan);
        this.WriteStream((columns - num1).ToString((IFormatProvider) CultureInfo.InvariantCulture));
        this.WriteStream(HTML4Renderer.m_quote);
        this.WriteStream(HTML4Renderer.m_closeBracket);
        this.WriteStream(HTML4Renderer.m_closeTD);
      }
      this.WriteStream(HTML4Renderer.m_closeTR);
    }

    protected void RenderSimpleTablixRows(
      RPLTablix tablix,
      string tablixID,
      RPLTablixRow currentRow,
      int borderContext,
      TablixFixedHeaderStorage headerStorage)
    {
      int row = 0;
      StyleContext styleContext = new StyleContext();
      float[] rowHeights = tablix.RowHeights;
      int length1 = tablix.ColumnWidths.Length;
      int length2 = rowHeights.Length;
      bool flag1 = headerStorage.ColumnHeaders != null;
      SharedListLayoutState layoutState = SharedListLayoutState.None;
      while (currentRow != null)
      {
        List<RPLTablixMemberCell> omittedHeaders = currentRow.OmittedHeaders;
        int omittedIndex = 0;
        if (length1 == 1)
        {
          layoutState = SharedListLayoutState.None;
          bool flag2 = tablix.SharedLayoutRow(row);
          bool flag3 = tablix.UseSharedLayoutRow(row);
          bool flag4 = tablix.RowsState.Length > row + 1 && tablix.UseSharedLayoutRow(row + 1);
          if (flag2 && flag4)
            layoutState = SharedListLayoutState.Start;
          else if (flag3)
            layoutState = !flag4 ? SharedListLayoutState.End : SharedListLayoutState.Continue;
        }
        if (layoutState == SharedListLayoutState.None || layoutState == SharedListLayoutState.Start)
        {
          if ((double) rowHeights[row] == 0.0 && row > 1 && currentRow.NumCells == 1 && currentRow[0].Element is RPLRectangle)
          {
            RPLRectangle element = (RPLRectangle) currentRow[0].Element;
            if (((RPLContainer) element).Children == null || ((RPLContainer) element).Children.Length == 0)
            {
              currentRow = tablix.GetNextRow();
              ++row;
              continue;
            }
          }
          this.WriteStream(HTML4Renderer.m_openTR);
          if (tablix.FixedRow(row) || headerStorage.RowHeaders != null || flag1)
          {
            string repItemId = tablixID + "tr" + (object) row;
            this.RenderReportItemId(repItemId);
            if (tablix.FixedRow(row))
            {
              headerStorage.ColumnHeaders.Add(repItemId);
              if (headerStorage.CornerHeaders != null)
                headerStorage.CornerHeaders.Add(repItemId);
            }
            else if (flag1)
            {
              headerStorage.BodyID = repItemId;
              flag1 = false;
            }
            if (headerStorage.RowHeaders != null)
              headerStorage.RowHeaders.Add(repItemId);
          }
          this.WriteStream(HTML4Renderer.m_valign);
          this.WriteStream(HTML4Renderer.m_topValue);
          this.WriteStream(HTML4Renderer.m_quote);
          this.WriteStream(HTML4Renderer.m_closeBracket);
        }
        int numCells = currentRow.NumCells;
        bool firstRow = row == 0;
        bool lastRow = row == length2 - 1;
        RPLTablixCell cell1 = currentRow[0];
        currentRow[0] = (RPLTablixCell) null;
        if (layoutState != SharedListLayoutState.None)
          this.RenderListReportItem(tablix, cell1, omittedHeaders, borderContext, styleContext, firstRow, lastRow, layoutState, (RPLElement) cell1.Element);
        else
          this.RenderSimpleTablixCellWithHeight(rowHeights[row], tablix, tablixID, length1, row, borderContext, cell1, omittedHeaders, ref omittedIndex, styleContext, firstRow, lastRow, headerStorage);
        int num;
        for (num = 1; num < numCells - 1; ++num)
        {
          RPLTablixCell cell2 = currentRow[num];
          this.RenderSimpleTablixCell(tablix, tablixID, cell2.ColSpan, row, borderContext, cell2, omittedHeaders, ref omittedIndex, false, firstRow, lastRow, headerStorage);
          currentRow[num] = (RPLTablixCell) null;
        }
        if (numCells > 1)
        {
          RPLTablixCell cell3 = currentRow[num];
          this.RenderSimpleTablixCell(tablix, tablixID, cell3.ColSpan, row, borderContext, cell3, omittedHeaders, ref omittedIndex, true, firstRow, lastRow, headerStorage);
          currentRow[num] = (RPLTablixCell) null;
        }
        if (layoutState == SharedListLayoutState.None || layoutState == SharedListLayoutState.End)
          this.WriteStream(HTML4Renderer.m_closeTR);
        currentRow = tablix.GetNextRow();
        ++row;
      }
    }

    private void RenderSimpleTablixCellWithHeight(
      float height,
      RPLTablix tablix,
      string tablixID,
      int numCols,
      int row,
      int tablixContext,
      RPLTablixCell cell,
      List<RPLTablixMemberCell> omittedCells,
      ref int omittedIndex,
      StyleContext styleContext,
      bool firstRow,
      bool lastRow,
      TablixFixedHeaderStorage headerStorage)
    {
      int colIndex = cell.ColIndex;
      int colSpan = cell.ColSpan;
      bool lastCol = colIndex + colSpan == numCols;
      bool zeroWidth = styleContext.ZeroWidth;
      float columnWidth = tablix.GetColumnWidth(colIndex, colSpan);
      styleContext.ZeroWidth = (double) columnWidth == 0.0;
      int startIndex = this.RenderZeroWidthTDsForTablix(colIndex, colSpan, tablix);
      int zeroWidthColumns = this.GetColSpanMinusZeroWidthColumns(colIndex, colSpan, tablix);
      this.WriteStream(HTML4Renderer.m_openTD);
      this.RenderSimpleTablixCellID(tablix, tablixID, row, headerStorage, colIndex);
      if (zeroWidthColumns > 1)
      {
        this.WriteStream(HTML4Renderer.m_colSpan);
        this.WriteStream(zeroWidthColumns.ToString((IFormatProvider) CultureInfo.InvariantCulture));
        this.WriteStream(HTML4Renderer.m_quote);
      }
      this.OpenStyle();
      this.WriteStream(HTML4Renderer.m_styleHeight);
      this.WriteDStream(height);
      this.WriteStream(HTML4Renderer.m_mm);
      RPLElement element = (RPLElement) cell.Element;
      if (element != null)
      {
        this.WriteStream(HTML4Renderer.m_semiColon);
        int borderContext = 0;
        this.RenderTablixReportItemStyle(tablix, tablixContext, cell, styleContext, true, lastCol, firstRow, lastRow, element, ref borderContext);
        this.RenderTablixOmittedHeaderCells(omittedCells, colIndex, lastCol, ref omittedIndex);
        this.RenderTablixReportItem(tablix, tablixContext, cell, styleContext, true, lastCol, firstRow, lastRow, element, ref borderContext);
      }
      else
      {
        if (styleContext.ZeroWidth)
          this.WriteStream(HTML4Renderer.m_displayNone);
        this.CloseStyle(true);
        this.WriteStream(HTML4Renderer.m_closeBracket);
        this.RenderTablixOmittedHeaderCells(omittedCells, colIndex, lastCol, ref omittedIndex);
        this.WriteStream(HTML4Renderer.m_nbsp);
      }
      this.WriteStream(HTML4Renderer.m_closeTD);
      this.RenderZeroWidthTDsForTablix(startIndex, zeroWidthColumns, tablix);
      styleContext.ZeroWidth = zeroWidth;
    }

    private void RenderTablixReportItemStyle(
      RPLTablix tablix,
      int tablixContext,
      RPLTablixCell cell,
      StyleContext styleContext,
      bool firstCol,
      bool lastCol,
      bool firstRow,
      bool lastRow,
      RPLElement cellItem,
      ref int borderContext)
    {
      RPLElementProps elementProps1 = cellItem.ElementProps;
      RPLElementPropsDef definition1 = elementProps1.Definition;
      RPLTextBoxProps elementProps2 = cellItem is RPLTextBox rplTextBox ? ((RPLElement) rplTextBox).ElementProps as RPLTextBoxProps : (RPLTextBoxProps) null;
      RPLTextBoxPropsDef definition2 = rplTextBox != null ? elementProps1.Definition as RPLTextBoxPropsDef : (RPLTextBoxPropsDef) null;
      styleContext.OmitBordersState = cell.ElementState;
      if (!(cellItem is RPLLine))
      {
        styleContext.StyleOnCell = true;
        borderContext = HTML4Renderer.GetNewContext(tablixContext, firstCol, lastCol, firstRow, lastRow);
        if (rplTextBox != null)
        {
          bool ignorePadding = styleContext.IgnorePadding;
          styleContext.IgnorePadding = true;
          RPLItemMeasurement measurement = (RPLItemMeasurement) null;
          if (this.m_deviceInfo.OutlookCompat || !this.m_deviceInfo.IsBrowserIE)
          {
            measurement = new RPLItemMeasurement();
            ((RPLSizes) measurement).Width = tablix.GetColumnWidth(cell.ColIndex, cell.ColSpan);
          }
          styleContext.EmptyTextBox = definition2.IsSimple && string.IsNullOrEmpty(elementProps2.Value) && string.IsNullOrEmpty(definition2.Value) && !this.NeedSharedToggleParent(elementProps2) && !this.CanSort(definition2);
          string textBoxClass = this.GetTextBoxClass(definition2, elementProps2, ((RPLElementProps) elementProps2).NonSharedStyle, definition1.ID + "c");
          bool backgroundBorders = styleContext.OnlyRenderMeasurementsBackgroundBorders;
          if (HTML4Renderer.IsWritingModeVertical((IRPLStyle) ((RPLElementProps) elementProps2).Style) && this.m_deviceInfo.IsBrowserIE && (definition2.CanGrow || this.m_deviceInfo.BrowserMode == BrowserMode.Standards && !this.m_deviceInfo.IsBrowserIE6Or7StandardsMode))
            styleContext.OnlyRenderMeasurementsBackgroundBorders = true;
          this.RenderReportItemStyle(cellItem, elementProps1, definition1, ((RPLElementProps) elementProps2).NonSharedStyle, ((RPLElementPropsDef) definition2).SharedStyle, measurement, styleContext, ref borderContext, textBoxClass);
          styleContext.OnlyRenderMeasurementsBackgroundBorders = backgroundBorders;
          styleContext.IgnorePadding = ignorePadding;
        }
        else
          this.RenderReportItemStyle(cellItem, elementProps1, definition1, (RPLItemMeasurement) null, styleContext, ref borderContext, definition1.ID + "c");
        styleContext.StyleOnCell = false;
      }
      else if (styleContext.ZeroWidth)
        this.WriteStream(HTML4Renderer.m_displayNone);
      this.CloseStyle(true);
      if (styleContext.EmptyTextBox && rplTextBox != null && elementProps1 != null)
        this.WriteToolTip(elementProps1);
      this.WriteStream(HTML4Renderer.m_closeBracket);
    }

    private void RenderTablixReportItem(
      RPLTablix tablix,
      int tablixContext,
      RPLTablixCell cell,
      StyleContext styleContext,
      bool firstCol,
      bool lastCol,
      bool firstRow,
      bool lastRow,
      RPLElement cellItem,
      ref int borderContext)
    {
      RPLElementProps elementProps1 = cellItem.ElementProps;
      RPLElementPropsDef definition1 = elementProps1.Definition;
      RPLTextBoxProps elementProps2 = cellItem is RPLTextBox textBox ? ((RPLElement) textBox).ElementProps as RPLTextBoxProps : (RPLTextBoxProps) null;
      RPLTextBoxPropsDef definition2 = textBox != null ? elementProps1.Definition as RPLTextBoxPropsDef : (RPLTextBoxPropsDef) null;
      RPLItemMeasurement measurement = new RPLItemMeasurement();
      styleContext.OmitBordersState = cell.ElementState;
      if (styleContext.EmptyTextBox)
      {
        bool flag = false;
        RPLActionInfo actionInfo = elementProps2.ActionInfo;
        if (this.HasAction(actionInfo))
        {
          this.RenderElementHyperlinkAllTextStyles(((RPLElementProps) elementProps2).Style, actionInfo.Actions[0], ((RPLElementPropsDef) definition2).ID + "a");
          this.WriteStream(HTML4Renderer.m_openDiv);
          this.OpenStyle();
          ((RPLSizes) measurement).Height = tablix.GetRowHeight(cell.RowIndex, cell.RowSpan);
          ((RPLSizes) measurement).Height = this.GetInnerContainerHeightSubtractBorders(measurement, (IRPLStyle) ((RPLElementProps) elementProps2).Style);
          if (this.m_deviceInfo.BrowserMode == BrowserMode.Quirks && this.m_deviceInfo.IsBrowserIE)
            this.RenderMeasurementHeight(((RPLSizes) measurement).Height);
          else
            this.RenderMeasurementMinHeight(((RPLSizes) measurement).Height);
          this.WriteStream(HTML4Renderer.m_semiColon);
          this.WriteStream(HTML4Renderer.m_cursorHand);
          this.WriteStream(HTML4Renderer.m_semiColon);
          this.CloseStyle(true);
          this.WriteStream(HTML4Renderer.m_closeBracket);
          flag = true;
        }
        this.WriteStream(HTML4Renderer.m_nbsp);
        if (flag)
        {
          this.WriteStream(HTML4Renderer.m_closeDiv);
          this.WriteStream(HTML4Renderer.m_closeA);
        }
      }
      else
      {
        styleContext.InTablix = true;
        bool renderId = this.NeedReportItemId(cellItem, elementProps1);
        if (textBox != null)
        {
          styleContext.RenderMeasurements = false;
          ((RPLSizes) measurement).Width = tablix.GetColumnWidth(cell.ColIndex, cell.ColSpan);
          ((RPLSizes) measurement).Height = tablix.GetRowHeight(cell.RowIndex, cell.RowSpan);
          this.RenderTextBoxPercent(textBox, elementProps2, definition2, measurement, styleContext, renderId);
        }
        else
        {
          ((RPLSizes) measurement).Width = tablix.GetColumnWidth(cell.ColIndex, cell.ColSpan);
          ((RPLSizes) measurement).Height = tablix.GetRowHeight(cell.RowIndex, cell.RowSpan);
          switch (cellItem)
          {
            case RPLRectangle _:
            case RPLSubReport _:
            case RPLLine _:
              styleContext.RenderMeasurements = false;
              break;
          }
          this.RenderReportItem(cellItem, elementProps1, definition1, measurement, styleContext, borderContext, renderId);
        }
      }
      styleContext.Reset();
    }

    private void RenderListReportItem(
      RPLTablix tablix,
      RPLTablixCell cell,
      List<RPLTablixMemberCell> omittedHeaders,
      int tablixContext,
      StyleContext styleContext,
      bool firstRow,
      bool lastRow,
      SharedListLayoutState layoutState,
      RPLElement cellItem)
    {
      RPLElementProps elementProps = cellItem.ElementProps;
      RPLElementPropsDef definition = elementProps.Definition;
      RPLItemMeasurement measurement = new RPLItemMeasurement();
      ((RPLSizes) measurement).Width = tablix.ColumnWidths[0];
      ((RPLSizes) measurement).Height = tablix.GetRowHeight(cell.RowIndex, cell.RowSpan);
      ((RPLMeasurement) measurement).State = cell.ElementState;
      bool zeroWidth = styleContext.ZeroWidth;
      styleContext.ZeroWidth = (double) ((RPLSizes) measurement).Width == 0.0;
      if (layoutState == SharedListLayoutState.Start)
      {
        this.WriteStream(HTML4Renderer.m_openTD);
        if (styleContext.ZeroWidth)
        {
          this.OpenStyle();
          this.WriteStream(HTML4Renderer.m_displayNone);
          this.CloseStyle(true);
        }
        this.WriteStream(HTML4Renderer.m_closeBracket);
      }
      if (cellItem is RPLRectangle)
      {
        int length = tablix.ColumnWidths.Length;
        bool right = cell.ColIndex + cell.ColSpan == length;
        int newContext = HTML4Renderer.GetNewContext(tablixContext, true, right, firstRow, lastRow);
        this.RenderListRectangle((RPLContainer) cellItem, omittedHeaders, measurement, elementProps, definition, layoutState, newContext);
        if (layoutState == SharedListLayoutState.End)
          this.WriteStream(HTML4Renderer.m_closeTD);
      }
      else
      {
        int omittedIndex = 0;
        this.RenderTablixOmittedHeaderCells(omittedHeaders, 0, true, ref omittedIndex);
        this.RenderReportItem(cellItem, elementProps, definition, measurement, styleContext, 0, this.NeedReportItemId(cellItem, elementProps));
        styleContext.Reset();
        if (layoutState == SharedListLayoutState.End)
          this.WriteStream(HTML4Renderer.m_closeTD);
      }
      styleContext.ZeroWidth = zeroWidth;
    }

    protected void RenderListRectangle(
      RPLContainer rectangle,
      List<RPLTablixMemberCell> omittedHeaders,
      RPLItemMeasurement measurement,
      RPLElementProps props,
      RPLElementPropsDef def,
      SharedListLayoutState layoutState,
      int borderContext)
    {
      this.GenerateHTMLTable(rectangle.Children, ((RPLSizes) measurement).Top, ((RPLSizes) measurement).Left, ((RPLSizes) measurement).Width, ((RPLSizes) measurement).Height, borderContext, false, layoutState, omittedHeaders, (IRPLStyle) props.Style);
    }

    private void RenderSimpleTablixCell(
      RPLTablix tablix,
      string tablixID,
      int colSpan,
      int row,
      int tablixContext,
      RPLTablixCell cell,
      List<RPLTablixMemberCell> omittedCells,
      ref int omittedIndex,
      bool lastCol,
      bool firstRow,
      bool lastRow,
      TablixFixedHeaderStorage headerStorage)
    {
      StyleContext styleContext = new StyleContext();
      int colIndex = cell.ColIndex;
      bool zeroWidth = styleContext.ZeroWidth;
      float columnWidth = tablix.GetColumnWidth(colIndex, cell.ColSpan);
      styleContext.ZeroWidth = (double) columnWidth == 0.0;
      int startIndex = this.RenderZeroWidthTDsForTablix(colIndex, colSpan, tablix);
      colSpan = this.GetColSpanMinusZeroWidthColumns(colIndex, colSpan, tablix);
      this.WriteStream(HTML4Renderer.m_openTD);
      this.RenderSimpleTablixCellID(tablix, tablixID, row, headerStorage, colIndex);
      if (colSpan > 1)
      {
        this.WriteStream(HTML4Renderer.m_colSpan);
        this.WriteStream(colSpan.ToString((IFormatProvider) CultureInfo.InvariantCulture));
        this.WriteStream(HTML4Renderer.m_quote);
      }
      RPLElement element = (RPLElement) cell.Element;
      if (element != null)
      {
        int borderContext = 0;
        this.RenderTablixReportItemStyle(tablix, tablixContext, cell, styleContext, false, lastCol, firstRow, lastRow, element, ref borderContext);
        this.RenderTablixOmittedHeaderCells(omittedCells, colIndex, lastCol, ref omittedIndex);
        this.RenderTablixReportItem(tablix, tablixContext, cell, styleContext, false, lastCol, firstRow, lastRow, element, ref borderContext);
      }
      else
      {
        if (styleContext.ZeroWidth)
        {
          this.OpenStyle();
          this.WriteStream(HTML4Renderer.m_displayNone);
          this.CloseStyle(true);
        }
        this.WriteStream(HTML4Renderer.m_closeBracket);
        this.WriteStream(HTML4Renderer.m_nbsp);
        this.RenderTablixOmittedHeaderCells(omittedCells, colIndex, lastCol, ref omittedIndex);
      }
      this.WriteStream(HTML4Renderer.m_closeTD);
      this.RenderZeroWidthTDsForTablix(startIndex, colSpan, tablix);
      styleContext.ZeroWidth = zeroWidth;
    }

    private int GetColSpanMinusZeroWidthColumns(int startColIndex, int colSpan, RPLTablix tablix)
    {
      int zeroWidthColumns = colSpan;
      for (int index = startColIndex; index < startColIndex + colSpan; ++index)
      {
        if ((double) tablix.ColumnWidths[index] == 0.0)
          --zeroWidthColumns;
      }
      return zeroWidthColumns;
    }

    private int RenderZeroWidthTDsForTablix(int startIndex, int colSpan, RPLTablix tablix)
    {
      int index;
      for (index = startIndex; index < startIndex + colSpan && (double) tablix.ColumnWidths[index] == 0.0; ++index)
      {
        this.WriteStream(HTML4Renderer.m_openTD);
        this.OpenStyle();
        this.WriteStream(HTML4Renderer.m_displayNone);
        this.CloseStyle(true);
        this.WriteStream(HTML4Renderer.m_closeBracket);
        this.WriteStream(HTML4Renderer.m_closeTD);
      }
      return index;
    }

    private void RenderSimpleTablixCellID(
      RPLTablix tablix,
      string tablixID,
      int row,
      TablixFixedHeaderStorage headerStorage,
      int col)
    {
      if (!tablix.FixedColumns[col])
        return;
      string repItemId = tablixID + "r" + (object) row + "c" + (object) col;
      this.RenderReportItemId(repItemId);
      headerStorage.RowHeaders.Add(repItemId);
      if (headerStorage.CornerHeaders == null || !tablix.FixedRow(row))
        return;
      headerStorage.CornerHeaders.Add(repItemId);
    }

    protected void RenderMultiLineTextWithHits(string text, List<int> hits)
    {
      if (text == null)
        return;
      int endPos = 0;
      int startPos = 0;
      int currentHitIndex = 0;
      int length = text.Length;
      for (int index = 0; index < length; ++index)
      {
        switch (text[index])
        {
          case '\n':
            this.RenderTextWithHits(text, startPos, endPos, hits, ref currentHitIndex);
            this.WriteStreamCR(HTML4Renderer.m_br);
            startPos = endPos + 1;
            break;
          case '\r':
            this.RenderTextWithHits(text, startPos, endPos, hits, ref currentHitIndex);
            startPos = endPos + 1;
            break;
        }
        ++endPos;
      }
      this.RenderTextWithHits(text, startPos, endPos, hits, ref currentHitIndex);
    }

    protected void RenderTextWithHits(
      string text,
      int startPos,
      int endPos,
      List<int> hitIndices,
      ref int currentHitIndex)
    {
      int length = this.m_searchText.Length;
      while (currentHitIndex < hitIndices.Count && hitIndices[currentHitIndex] < endPos)
      {
        int hitIndex = hitIndices[currentHitIndex];
        this.WriteStreamEncoded(text.Substring(startPos, hitIndex - startPos));
        this.OutputFindString(text.Substring(hitIndex, length), 0);
        startPos = hitIndex + length;
        ++currentHitIndex;
        ++this.m_currentHitCount;
      }
      if (startPos > endPos)
        return;
      this.WriteStreamEncoded(text.Substring(startPos, endPos - startPos));
    }

    private void OutputFindString(string findString, int offset)
    {
      this.WriteStream(HTML4Renderer.m_openSpan);
      this.WriteStream(HTML4Renderer.m_id);
      this.WriteAttrEncoded(this.m_deviceInfo.HtmlPrefixId);
      this.WriteStream(HTML4Renderer.m_searchHitIdPrefix);
      this.WriteStream(this.m_currentHitCount.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (offset > 0)
      {
        this.WriteStream("_");
        this.WriteStream(offset.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      }
      this.WriteStream(HTML4Renderer.m_quote);
      if (this.m_currentHitCount == 0)
      {
        if (this.m_deviceInfo.IsBrowserSafari)
          this.WriteStream(" style=\"COLOR:black;BACKGROUND-COLOR:#B5D4FE;\">");
        else
          this.WriteStream(" style=\"COLOR:highlighttext;BACKGROUND-COLOR:highlight;\">");
      }
      else
        this.WriteStream(HTML4Renderer.m_closeBracket);
      this.WriteStreamEncoded(findString);
      this.WriteStream(HTML4Renderer.m_closeSpan);
    }

    private bool IsImageNotFitProportional(RPLElement reportItem, RPLElementPropsDef definition)
    {
      RPLImagePropsDef rplImagePropsDef = (RPLImagePropsDef) null;
      if (definition is RPLImagePropsDef)
        rplImagePropsDef = (RPLImagePropsDef) definition;
      return reportItem is RPLImage && rplImagePropsDef != null && rplImagePropsDef.Sizing != 2;
    }

    protected void RenderImage(
      RPLImage image,
      RPLImageProps imageProps,
      RPLImagePropsDef imagePropsDef,
      RPLItemMeasurement measurement,
      ref int borderContext,
      bool renderId)
    {
      bool flag1 = false;
      bool flag2 = false;
      RPLImageData image1 = imageProps.Image;
      RPLActionInfo actionInfo = imageProps.ActionInfo;
      StyleContext styleContext = new StyleContext();
      RPLFormat.Sizings sizing = imagePropsDef.Sizing;
      bool flag3 = false;
      if (sizing == null)
      {
        flag3 = true;
        this.WriteStream(HTML4Renderer.m_openTable);
        this.WriteStream(HTML4Renderer.m_closeBracket);
        this.WriteStream(HTML4Renderer.m_firstTD);
        this.WriteStream(HTML4Renderer.m_closeBracket);
      }
      this.WriteStream(HTML4Renderer.m_openDiv);
      int xOffset = 0;
      int yOffset = 0;
      Rectangle consolidationOffsets = imageProps.Image.ImageConsolidationOffsets;
      bool flag4 = !consolidationOffsets.IsEmpty;
      if (flag4)
      {
        if (sizing == 3 || sizing == 2 || sizing == 1)
        {
          styleContext.RenderMeasurements = styleContext.InTablix || sizing != 0;
          this.RenderReportItemStyle((RPLElement) image, (RPLElementProps) imageProps, (RPLElementPropsDef) imagePropsDef, measurement, styleContext, ref borderContext, ((RPLElementPropsDef) imagePropsDef).ID);
          this.WriteStream(HTML4Renderer.m_closeBracket);
          this.WriteStream(HTML4Renderer.m_openDiv);
        }
        this.WriteOuterConsolidation(consolidationOffsets, sizing, ((RPLElementProps) imageProps).UniqueName);
        this.RenderReportItemStyle((RPLElement) image, (RPLElementProps) imageProps, (RPLElementPropsDef) imagePropsDef, (RPLItemMeasurement) null, styleContext, ref borderContext, ((RPLElementPropsDef) imagePropsDef).ID);
        xOffset = consolidationOffsets.Left;
        yOffset = consolidationOffsets.Top;
      }
      else
      {
        styleContext.RenderMeasurements = styleContext.InTablix || sizing != 0;
        this.RenderReportItemStyle((RPLElement) image, (RPLElementProps) imageProps, (RPLElementPropsDef) imagePropsDef, measurement, styleContext, ref borderContext, ((RPLElementPropsDef) imagePropsDef).ID);
      }
      this.WriteStream(HTML4Renderer.m_closeBracket);
      if (this.HasAction(actionInfo))
        flag2 = this.RenderElementHyperlink((IRPLStyle) ((RPLElementProps) imageProps).Style, actionInfo.Actions[0]);
      this.WriteStream(HTML4Renderer.m_img);
      if (this.m_browserIE)
        this.WriteStream(HTML4Renderer.m_imgOnError);
      if (renderId || flag1)
        this.RenderReportItemId(((RPLElementProps) imageProps).UniqueName);
      if (imageProps.ActionImageMapAreas != null && imageProps.ActionImageMapAreas.Length > 0)
      {
        this.WriteAttrEncoded(HTML4Renderer.m_useMap, "#" + this.m_deviceInfo.HtmlPrefixId + HTML4Renderer.m_mapPrefixString + ((RPLElementProps) imageProps).UniqueName);
        this.WriteStream(HTML4Renderer.m_zeroBorder);
      }
      else if (flag2)
        this.WriteStream(HTML4Renderer.m_zeroBorder);
      if (sizing == 2)
      {
        PaddingSharedInfo paddings = this.GetPaddings(((RPLElement) image).ElementProps.Style, (PaddingSharedInfo) null);
        bool writeSmallSize = !flag4 && this.m_deviceInfo.BrowserMode == BrowserMode.Standards;
        this.RenderImageFitProportional(image, measurement, paddings, writeSmallSize);
      }
      else if (sizing == 1 && !flag4)
      {
        if (this.m_useInlineStyle)
          this.PercentSizes();
        else
          this.ClassPercentSizes();
      }
      if (flag4)
        this.WriteClippedDiv(consolidationOffsets);
      this.WriteToolTip((RPLElementProps) imageProps);
      this.WriteStream(HTML4Renderer.m_src);
      this.RenderImageUrl(true, image1);
      this.WriteStreamCR(HTML4Renderer.m_closeTag);
      if (flag2)
        this.WriteStream(HTML4Renderer.m_closeA);
      if (imageProps.ActionImageMapAreas != null && imageProps.ActionImageMapAreas.Length > 0)
        this.RenderImageMapAreas(imageProps.ActionImageMapAreas, (double) ((RPLSizes) measurement).Width, (double) ((RPLSizes) measurement).Height, ((RPLElementProps) imageProps).UniqueName, xOffset, yOffset);
      if (flag4 && (sizing == 3 || sizing == 2 || sizing == 1))
        this.WriteStream(HTML4Renderer.m_closeDiv);
      this.WriteStreamCR(HTML4Renderer.m_closeDiv);
      if (!flag3)
        return;
      this.WriteStreamCR(HTML4Renderer.m_lastTD);
      this.WriteStreamCR(HTML4Renderer.m_closeTable);
    }

    protected int RenderReportItem(
      RPLElement reportItem,
      RPLElementProps props,
      RPLElementPropsDef def,
      RPLItemMeasurement measurement,
      StyleContext styleContext,
      int borderContext,
      bool renderId)
    {
      int borderContext1 = borderContext;
      if (reportItem == null)
        return borderContext1;
      if (measurement != null)
        styleContext.OmitBordersState = ((RPLMeasurement) measurement).State;
      switch (reportItem)
      {
        case RPLTextBox textBox:
          if (styleContext.InTablix)
          {
            this.RenderTextBoxPercent(textBox, ((RPLElement) textBox).ElementProps as RPLTextBoxProps, ((RPLElement) textBox).ElementProps.Definition as RPLTextBoxPropsDef, measurement, styleContext, renderId);
            break;
          }
          this.RenderTextBox(textBox, ((RPLElement) textBox).ElementProps as RPLTextBoxProps, ((RPLElement) textBox).ElementProps.Definition as RPLTextBoxPropsDef, measurement, styleContext, ref borderContext1, renderId);
          break;
        case RPLTablix _:
          this.RenderTablix((RPLTablix) reportItem, props, def, measurement, styleContext, ref borderContext1, renderId);
          break;
        case RPLRectangle _:
          this.RenderRectangle((RPLContainer) reportItem, props, def, measurement, ref borderContext1, renderId, styleContext);
          break;
        case RPLChart _:
        case RPLGaugePanel _:
        case RPLMap _:
          this.RenderServerDynamicImage(reportItem, (RPLDynamicImageProps) props, def, measurement, borderContext1, renderId, styleContext);
          break;
        case RPLSubReport _:
          this.RenderSubReport((RPLSubReport) reportItem, props, def, measurement, ref borderContext1, renderId, styleContext);
          break;
        case RPLImage _:
          if (styleContext.InTablix)
          {
            this.RenderImagePercent((RPLImage) reportItem, (RPLImageProps) props, (RPLImagePropsDef) def, measurement);
            break;
          }
          this.RenderImage((RPLImage) reportItem, (RPLImageProps) props, (RPLImagePropsDef) def, measurement, ref borderContext1, renderId);
          break;
        case RPLLine _:
          this.RenderLine((RPLLine) reportItem, props, (RPLLinePropsDef) def, measurement, renderId, styleContext);
          break;
      }
      return borderContext1;
    }

    protected void RenderSubReport(
      RPLSubReport subReport,
      RPLElementProps subReportProps,
      RPLElementPropsDef subReportDef,
      RPLItemMeasurement measurement,
      ref int borderContext,
      bool renderId,
      StyleContext styleContext)
    {
      if (!styleContext.InTablix || renderId)
      {
        styleContext.RenderMeasurements = false;
        this.WriteStream(HTML4Renderer.m_openDiv);
        this.RenderReportItemStyle((RPLElement) subReport, subReportProps, subReportDef, measurement, styleContext, ref borderContext, subReportDef.ID);
        if (renderId)
          this.RenderReportItemId(subReportProps.UniqueName);
        this.WriteStreamCR(HTML4Renderer.m_closeBracket);
      }
      RPLItemMeasurement[] children = ((RPLContainer) subReport).Children;
      int num = borderContext;
      bool flag = children.Length > 0;
      int length = children.Length;
      for (int index = 0; index < length; ++index)
      {
        if (index == 0 && length > 1 && (borderContext & 8) > 0)
          num &= -9;
        else if (index == 1 && (borderContext & 4) > 0)
          num &= -5;
        if (index > 0 && index == length - 1 && (borderContext & 8) > 0)
          num |= 8;
        int borderContext1 = num;
        RPLItemMeasurement measurement1 = children[index];
        RPLContainer element = (RPLContainer) measurement1.Element;
        RPLElementProps elementProps = ((RPLElement) element).ElementProps;
        RPLElementPropsDef definition = elementProps.Definition;
        this.m_isBody = true;
        this.m_usePercentWidth = flag;
        this.RenderRectangle(element, elementProps, definition, measurement1, ref borderContext1, false, new StyleContext());
      }
      if (styleContext.InTablix && !renderId)
        return;
      this.WriteStreamCR(HTML4Renderer.m_closeDiv);
    }

    protected void RenderRectangleMeasurements(RPLItemMeasurement measurement, IRPLStyle style)
    {
      float adjustedWidth = this.GetAdjustedWidth(measurement, style);
      float adjustedHeight = this.GetAdjustedHeight(measurement, style);
      this.RenderMeasurementWidth(adjustedWidth, true);
      if (this.m_deviceInfo.IsBrowserIE && this.m_deviceInfo.BrowserMode == BrowserMode.Standards && !this.m_deviceInfo.IsBrowserIE6)
        this.RenderMeasurementMinHeight(adjustedHeight);
      else
        this.RenderMeasurementHeight(adjustedHeight);
    }

    private void WriteFontSizeSmallPoint()
    {
      if (this.m_deviceInfo.IsBrowserGeckoEngine)
        this.WriteStream(HTML4Renderer.m_smallPoint);
      else
        this.WriteStream(HTML4Renderer.m_zeroPoint);
    }

    protected void RenderRectangle(
      RPLContainer rectangle,
      RPLElementProps props,
      RPLElementPropsDef def,
      RPLItemMeasurement measurement,
      ref int borderContext,
      bool renderId,
      StyleContext styleContext)
    {
      RPLItemMeasurement[] children = rectangle.Children;
      if (def is RPLRectanglePropsDef rectanglePropsDef && rectanglePropsDef.LinkToChildId != null)
        this.m_linkToChildStack.Push((object) rectanglePropsDef.LinkToChildId);
      bool expandItem = this.m_expandItem;
      bool flag1 = renderId;
      string str = props.UniqueName;
      bool flag2 = children == null || children.Length == 0;
      if (flag2 && styleContext.InTablix)
        return;
      bool flag3 = this.m_deviceInfo.OutlookCompat || !this.m_browserIE || flag2 && this.m_usePercentWidth;
      if (!styleContext.InTablix || renderId)
      {
        if (flag3)
        {
          this.WriteStream(HTML4Renderer.m_openTable);
          this.WriteStream(HTML4Renderer.m_zeroBorder);
        }
        else
        {
          this.WriteStream(HTML4Renderer.m_openDiv);
          if (this.m_deviceInfo.IsBrowserIE && this.m_deviceInfo.AllowScript)
          {
            if (!this.m_needsGrowRectangleScript)
              this.CreateGrowRectIdsStream();
            flag1 = true;
            if (!renderId)
              str = props.UniqueName + "_gr";
            this.WriteIdToSecondaryStream(this.m_growRectangleIdsStream, str);
          }
        }
        if (flag1)
          this.RenderReportItemId(str);
        if (this.m_isBody)
        {
          this.m_isBody = false;
          styleContext.RenderMeasurements = false;
          if (flag2)
          {
            this.OpenStyle();
            if (this.m_usePercentWidth)
            {
              this.RenderMeasurementHeight(((RPLSizes) measurement).Height);
              this.WriteStream(HTML4Renderer.m_styleWidth);
              this.WriteStream(HTML4Renderer.m_percent);
              this.WriteStream(HTML4Renderer.m_semiColon);
            }
            else
              this.RenderRectangleMeasurements(measurement, (IRPLStyle) props.Style);
          }
          else if (flag3 && this.m_usePercentWidth)
          {
            this.OpenStyle();
            this.WriteStream(HTML4Renderer.m_styleWidth);
            this.WriteStream(HTML4Renderer.m_percent);
            this.WriteStream(HTML4Renderer.m_semiColon);
          }
          this.m_usePercentWidth = false;
        }
        if (!styleContext.InTablix)
        {
          if (styleContext.RenderMeasurements)
          {
            this.OpenStyle();
            this.RenderRectangleMeasurements(measurement, (IRPLStyle) props.Style);
          }
          this.RenderReportItemStyle((RPLElement) rectangle, props, def, measurement, styleContext, ref borderContext, def.ID);
        }
        this.CloseStyle(true);
        this.WriteToolTip(props);
        this.WriteStreamCR(HTML4Renderer.m_closeBracket);
        if (flag3)
        {
          this.WriteStream(HTML4Renderer.m_firstTD);
          this.OpenStyle();
          if (flag2)
          {
            this.RenderMeasurementStyle(((RPLSizes) measurement).Height, ((RPLSizes) measurement).Width);
            this.WriteStream(HTML4Renderer.m_fontSize);
            this.WriteStream("1pt");
          }
          else
          {
            this.WriteStream(HTML4Renderer.m_verticalAlign);
            this.WriteStream(HTML4Renderer.m_topValue);
          }
          this.CloseStyle(true);
          this.WriteStream(HTML4Renderer.m_closeBracket);
        }
      }
      if (flag2)
      {
        this.WriteStream(HTML4Renderer.m_nbsp);
      }
      else
      {
        bool inTablix = styleContext.InTablix;
        styleContext.InTablix = false;
        this.GenerateHTMLTable(children, ((RPLSizes) measurement).Top, ((RPLSizes) measurement).Left, ((RPLSizes) measurement).Width, ((RPLSizes) measurement).Height, borderContext, expandItem, SharedListLayoutState.None, (List<RPLTablixMemberCell>) null, (IRPLStyle) props.Style);
        if (inTablix)
          styleContext.InTablix = true;
      }
      if (!styleContext.InTablix || renderId)
      {
        if (flag3)
        {
          this.WriteStream(HTML4Renderer.m_lastTD);
          this.WriteStream(HTML4Renderer.m_closeTable);
        }
        else
          this.WriteStreamCR(HTML4Renderer.m_closeDiv);
      }
      if (this.m_linkToChildStack.Count <= 0 || rectanglePropsDef == null || rectanglePropsDef.LinkToChildId == null || !rectanglePropsDef.LinkToChildId.Equals(this.m_linkToChildStack.Peek()))
        return;
      this.m_linkToChildStack.Pop();
    }

    private void RenderElementHyperlinkAllTextStyles(
      RPLElementStyle style,
      RPLAction action,
      string id)
    {
      this.WriteStream(HTML4Renderer.m_openA);
      this.RenderTabIndex();
      bool hasHref = false;
      if (action.Hyperlink != null)
        this.WriteStream(HTML4Renderer.m_hrefString + HttpUtility.HtmlAttributeEncode(action.Hyperlink) + HTML4Renderer.m_quoteString);
      else
        this.RenderInteractionAction(action, ref hasHref);
      TextRunStyleWriter styleWriter = new TextRunStyleWriter(this);
      this.WriteStyles(id, style.NonSharedProperties, style.SharedProperties, (ElementStyleWriter) styleWriter);
      if (this.m_deviceInfo.LinkTarget != null)
      {
        this.WriteStream(HTML4Renderer.m_target);
        this.WriteStream(this.m_deviceInfo.LinkTarget);
        this.WriteStream(HTML4Renderer.m_quote);
      }
      this.WriteStream(HTML4Renderer.m_closeBracket);
    }

    private bool RenderElementHyperlink(IRPLStyle style, RPLAction action)
    {
      object textDec = style[(byte) 24] ?? (object) (RPLFormat.TextDecorations) 0;
      string color = (string) style[(byte) 27];
      return this.RenderHyperlink(action, (RPLFormat.TextDecorations) textDec, color);
    }

    protected void RenderTextBoxPercent(
      RPLTextBox textBox,
      RPLTextBoxProps textBoxProps,
      RPLTextBoxPropsDef textBoxPropsDef,
      RPLItemMeasurement measurement,
      StyleContext styleContext,
      bool renderId)
    {
      RPLStyleProps actionStyle = (RPLStyleProps) null;
      RPLActionInfo actionInfo = textBoxProps.ActionInfo;
      RPLStyleProps nonSharedStyle = ((RPLElementProps) textBoxProps).NonSharedStyle;
      RPLStyleProps sharedStyle = ((RPLElementPropsDef) textBoxPropsDef).SharedStyle;
      RPLElementStyle style = ((RPLElementProps) textBoxProps).Style;
      bool flag1 = this.CanSort(textBoxPropsDef);
      bool flag2 = this.NeedSharedToggleParent(textBoxProps);
      bool flag3 = false;
      bool isSimple = textBoxPropsDef.IsSimple;
      bool flag4 = HTML4Renderer.IsDirectionRTL((IRPLStyle) style);
      bool flag5 = HTML4Renderer.IsWritingModeVertical((IRPLStyle) style);
      bool flag6 = flag5 && this.m_deviceInfo.IsBrowserIE;
      if (flag6)
      {
        if (textBoxPropsDef.CanGrow)
        {
          this.WriteStream(HTML4Renderer.m_openDiv);
          this.OpenStyle();
          this.RenderDirectionStyles((RPLElement) textBox, (RPLElementProps) textBoxProps, (RPLElementPropsDef) textBoxPropsDef, (RPLItemMeasurement) null, (IRPLStyle) ((RPLElementProps) textBoxProps).Style, (IRPLStyle) nonSharedStyle, false, styleContext);
          this.WriteStream("display: inline;");
          this.CloseStyle(true);
          this.ClassPercentHeight();
          if (this.m_deviceInfo.BrowserMode == BrowserMode.Standards && !this.m_deviceInfo.IsBrowserIE6Or7StandardsMode && this.m_deviceInfo.AllowScript)
          {
            if (!this.m_needsFitVertTextScript)
              this.CreateFitVertTextIdsStream();
            this.WriteIdToSecondaryStream(this.m_fitVertTextIdsStream, ((RPLElementProps) textBoxProps).UniqueName + "_fvt");
            this.RenderReportItemId(((RPLElementProps) textBoxProps).UniqueName + "_fvt");
          }
          this.WriteStreamCR(HTML4Renderer.m_closeBracket);
          this.WriteStream(HTML4Renderer.m_openTable);
          this.ClassPercentHeight();
          this.WriteStreamCR(HTML4Renderer.m_closeBracket);
          this.WriteStream(HTML4Renderer.m_firstTD);
        }
        else
          this.WriteStream(HTML4Renderer.m_openDiv);
      }
      else
        this.WriteStream(HTML4Renderer.m_openDiv);
      if (renderId || flag2 || flag1)
        this.RenderReportItemId(((RPLElementProps) textBoxProps).UniqueName);
      bool flag7 = flag2 && !isSimple;
      bool flag8 = flag1 || flag7;
      if (!textBoxPropsDef.CanGrow)
      {
        if ((!this.m_browserIE || this.m_deviceInfo.BrowserMode == BrowserMode.Standards || flag6) && measurement != null)
        {
          styleContext.RenderMeasurements = false;
          float innerContainerHeight = this.GetInnerContainerHeight(measurement, (IRPLStyle) style);
          this.OpenStyle();
          this.RenderMeasurementHeight(innerContainerHeight);
          this.WriteStream(HTML4Renderer.m_overflowHidden);
          this.WriteStream(HTML4Renderer.m_semiColon);
        }
        else
          styleContext.RenderMeasurements = true;
        if (!flag8)
        {
          object obj = style[(byte) 26];
          flag8 = obj != null && (RPLFormat.VerticalAlignments) obj != null && !textBoxPropsDef.CanGrow;
        }
        measurement = (RPLItemMeasurement) null;
      }
      if (flag8)
      {
        this.CloseStyle(true);
        styleContext.RenderMeasurements = false;
        this.WriteStreamCR(HTML4Renderer.m_closeBracket);
        this.WriteStream(HTML4Renderer.m_openTable);
        this.WriteStream(HTML4Renderer.m_zeroBorder);
        if (isSimple && (flag1 || flag7))
          this.WriteClassName(HTML4Renderer.m_percentHeightInlineTable, HTML4Renderer.m_classPercentHeightInlineTable);
        else
          this.WriteClassName(HTML4Renderer.m_percentSizeInlineTable, HTML4Renderer.m_classPercentSizeInlineTable);
        this.RenderReportLanguage();
        this.WriteStream(HTML4Renderer.m_closeBracket);
        this.WriteStream(HTML4Renderer.m_firstTD);
        if (flag1 || flag7)
        {
          if (flag5)
            this.WriteStream(" ROWS='2'");
          this.RenderAtStart(textBoxProps, (IRPLStyle) style, flag1 && flag4, flag7 && !flag4);
        }
      }
      int borderContext = 0;
      this.RenderReportItemStyle((RPLElement) textBox, (RPLElementProps) textBoxProps, (RPLElementPropsDef) textBoxPropsDef, nonSharedStyle, sharedStyle, measurement, styleContext, ref borderContext, ((RPLElementPropsDef) textBoxPropsDef).ID);
      this.WriteToolTip((RPLElementProps) textBoxProps);
      this.WriteStreamCR(HTML4Renderer.m_closeBracket);
      if (flag2 && isSimple)
        this.RenderToggleImage(textBoxProps);
      RPLAction rplAction = (RPLAction) null;
      if (this.HasAction(actionInfo))
      {
        rplAction = actionInfo.Actions[0];
        this.RenderElementHyperlinkAllTextStyles(style, rplAction, ((RPLElementPropsDef) textBoxPropsDef).ID + "a");
        flag3 = true;
      }
      string textBoxValue = (string) null;
      if (textBoxPropsDef.IsSimple)
      {
        textBoxValue = textBoxProps.Value;
        if (string.IsNullOrEmpty(textBoxValue))
          textBoxValue = textBoxPropsDef.Value;
      }
      this.RenderTextBoxContent(textBox, textBoxProps, textBoxPropsDef, textBoxValue, actionStyle, flag2 || flag1, measurement, rplAction);
      if (flag3)
        this.WriteStream(HTML4Renderer.m_closeA);
      if (flag8)
      {
        this.RenderAtEnd(textBoxProps, (IRPLStyle) style, flag1 && !flag4, flag7 && flag4);
        this.WriteStream(HTML4Renderer.m_lastTD);
        this.WriteStream(HTML4Renderer.m_closeTable);
      }
      if (flag6)
      {
        if (textBoxPropsDef.CanGrow)
        {
          this.WriteStreamCR(HTML4Renderer.m_lastTD);
          this.WriteStreamCR(HTML4Renderer.m_closeTable);
          this.WriteStreamCR(HTML4Renderer.m_closeDiv);
        }
        else
          this.WriteStream(HTML4Renderer.m_closeDiv);
      }
      else
        this.WriteStreamCR(HTML4Renderer.m_closeDiv);
    }

    protected void RenderPageHeaderFooter(RPLItemMeasurement hfMeasurement)
    {
      if ((double) ((RPLSizes) hfMeasurement).Height == 0.0)
        return;
      RPLHeaderFooter element = (RPLHeaderFooter) hfMeasurement.Element;
      int borderContext = 0;
      StyleContext styleContext = new StyleContext();
      this.WriteStream(HTML4Renderer.m_openTR);
      this.WriteStream(HTML4Renderer.m_closeBracket);
      this.WriteStream(HTML4Renderer.m_openTD);
      styleContext.StyleOnCell = true;
      this.RenderReportItemStyle((RPLElement) element, ((RPLElement) element).ElementProps, ((RPLElement) element).ElementProps.Definition, (RPLItemMeasurement) null, styleContext, ref borderContext, ((RPLElement) element).ElementProps.Definition.ID + "c");
      styleContext.StyleOnCell = false;
      this.WriteStream(HTML4Renderer.m_closeBracket);
      this.WriteStream(HTML4Renderer.m_openDiv);
      if (!this.m_deviceInfo.IsBrowserIE)
      {
        styleContext.RenderMeasurements = false;
        styleContext.RenderMinMeasurements = true;
      }
      this.RenderReportItemStyle((RPLElement) element, hfMeasurement, ref borderContext, styleContext);
      this.WriteStreamCR(HTML4Renderer.m_closeBracket);
      RPLItemMeasurement[] children = ((RPLContainer) element).Children;
      if (children != null && children.Length > 0)
      {
        this.m_renderTableHeight = true;
        this.GenerateHTMLTable(children, 0.0f, 0.0f, this.m_pageContent.MaxSectionWidth, ((RPLSizes) hfMeasurement).Height, borderContext, false, SharedListLayoutState.None, (List<RPLTablixMemberCell>) null, (IRPLStyle) ((RPLElement) element).ElementProps.Style);
      }
      else
        this.WriteStream(HTML4Renderer.m_nbsp);
      this.m_renderTableHeight = false;
      this.WriteStreamCR(HTML4Renderer.m_closeDiv);
      this.WriteStream(HTML4Renderer.m_closeTD);
      this.WriteStream(HTML4Renderer.m_closeTR);
    }

    protected void RenderStyleProps(
      RPLElement reportItem,
      RPLElementProps props,
      RPLElementPropsDef definition,
      RPLItemMeasurement measurement,
      IRPLStyle sharedStyleProps,
      IRPLStyle nonSharedStyleProps,
      StyleContext styleContext,
      ref int borderContext,
      bool isNonSharedStyles)
    {
      if (styleContext.ZeroWidth)
        this.WriteStream(HTML4Renderer.m_displayNone);
      IRPLStyle irplStyle = isNonSharedStyles ? nonSharedStyleProps : sharedStyleProps;
      if (irplStyle == null)
        return;
      if (styleContext.StyleOnCell)
      {
        bool renderPadding = true;
        if ((HTML4Renderer.IsWritingModeVertical(sharedStyleProps) || HTML4Renderer.IsWritingModeVertical(nonSharedStyleProps)) && styleContext.IgnorePadding && this.m_deviceInfo.IsBrowserIE)
          renderPadding = false;
        if (!styleContext.NoBorders)
        {
          this.RenderHtmlBorders(irplStyle, ref borderContext, styleContext.OmitBordersState, renderPadding, isNonSharedStyles, sharedStyleProps);
          this.RenderBackgroundStyleProps(irplStyle);
        }
        if (!styleContext.OnlyRenderMeasurementsBackgroundBorders)
        {
          object val1 = irplStyle[(byte) 26];
          if (val1 != null && !styleContext.IgnoreVerticalAlign)
          {
            object theString = (object) EnumStrings.GetValue((RPLFormat.VerticalAlignments) val1);
            this.WriteStream(HTML4Renderer.m_verticalAlign);
            this.WriteStream(theString);
            this.WriteStream(HTML4Renderer.m_semiColon);
          }
          object val2 = irplStyle[(byte) 25];
          if (val2 != null)
          {
            if ((RPLFormat.TextAlignments) val2 != null)
            {
              object theString = (object) EnumStrings.GetValue((RPLFormat.TextAlignments) val2);
              this.WriteStream(HTML4Renderer.m_textAlign);
              this.WriteStream(theString);
              this.WriteStream(HTML4Renderer.m_semiColon);
            }
            else
              this.RenderTextAlign(props as RPLTextBoxProps, props.Style);
          }
          this.RenderDirectionStyles(reportItem, props, definition, measurement, sharedStyleProps, nonSharedStyleProps, isNonSharedStyles, styleContext);
        }
        if (measurement == null || !this.m_deviceInfo.OutlookCompat && this.m_deviceInfo.IsBrowserIE)
          return;
        float num = ((RPLSizes) measurement).Width;
        if ((reportItem is RPLTextBox || this.IsImageNotFitProportional(reportItem, definition)) && !styleContext.InTablix)
        {
          float adjustedWidth = this.GetAdjustedWidth(measurement, (IRPLStyle) props.Style);
          if (this.m_deviceInfo.IsBrowserIE6Or7StandardsMode)
            num = adjustedWidth;
          this.RenderMeasurementMinWidth(adjustedWidth);
        }
        else
          this.RenderMeasurementMinWidth(num);
        this.RenderMeasurementWidth(num, false);
      }
      else
      {
        if (reportItem is RPLTextBox)
        {
          this.WriteStream(HTML4Renderer.m_wordWrap);
          this.WriteStream(HTML4Renderer.m_semiColon);
          this.WriteStream(HTML4Renderer.m_whiteSpacePreWrap);
          this.WriteStream(HTML4Renderer.m_semiColon);
        }
        if (styleContext.RenderMeasurements || styleContext.RenderMinMeasurements)
        {
          bool empty = false;
          this.IsCollectionWithoutContent(reportItem as RPLContainer, ref empty);
          int num1;
          if (measurement != null)
          {
            if (styleContext.InTablix && !empty)
            {
              switch (reportItem)
              {
                case RPLChart _:
                case RPLGaugePanel _:
                  num1 = 1;
                  break;
                default:
                  num1 = reportItem is RPLMap ? 1 : 0;
                  break;
              }
            }
            else
              num1 = 0;
          }
          else
            num1 = 1;
          if (num1 != 0)
          {
            switch (reportItem)
            {
              case RPLTextBox _:
                RPLTextBoxPropsDef rplTextBoxPropsDef1 = (RPLTextBoxPropsDef) definition;
                if (styleContext.RenderMeasurements)
                  this.WriteStream(HTML4Renderer.m_styleWidth);
                else if (styleContext.RenderMinMeasurements)
                  this.WriteStream(HTML4Renderer.m_styleMinWidth);
                if (styleContext.InTablix && this.m_deviceInfo.BrowserMode == BrowserMode.Quirks)
                  this.WriteStream(HTML4Renderer.m_ninetyninepercent);
                else
                  this.WriteStream(HTML4Renderer.m_percent);
                this.WriteStream(HTML4Renderer.m_semiColon);
                if (rplTextBoxPropsDef1.CanGrow)
                {
                  this.WriteStream(HTML4Renderer.m_overflowXHidden);
                }
                else
                {
                  if (styleContext.RenderMeasurements)
                    this.WriteStream(HTML4Renderer.m_styleHeight);
                  else if (styleContext.RenderMinMeasurements)
                    this.WriteStream(HTML4Renderer.m_styleMinHeight);
                  this.WriteStream(HTML4Renderer.m_percent);
                  this.WriteStream(HTML4Renderer.m_semiColon);
                  this.WriteStream(HTML4Renderer.m_overflowHidden);
                }
                this.WriteStream(HTML4Renderer.m_semiColon);
                break;
              case RPLTablix _:
                break;
              default:
                this.RenderPercentSizes();
                break;
            }
          }
          else
          {
            switch (reportItem)
            {
              case RPLTextBox _:
                float num2 = ((RPLSizes) measurement).Width;
                float height1 = ((RPLSizes) measurement).Height;
                if (!styleContext.NoBorders && !styleContext.InTablix)
                {
                  float adjustedWidth = this.GetAdjustedWidth(measurement, (IRPLStyle) props.Style);
                  if (this.m_deviceInfo.IsBrowserIE6Or7StandardsMode)
                  {
                    num2 = adjustedWidth;
                    height1 = this.GetAdjustedHeight(measurement, (IRPLStyle) props.Style);
                  }
                  this.RenderMeasurementMinWidth(adjustedWidth);
                }
                else
                  this.RenderMeasurementMinWidth(num2);
                RPLTextBoxPropsDef rplTextBoxPropsDef2 = (RPLTextBoxPropsDef) definition;
                if (rplTextBoxPropsDef2.CanGrow && rplTextBoxPropsDef2.CanShrink)
                {
                  this.RenderMeasurementWidth(num2, false);
                  break;
                }
                this.WriteStream(HTML4Renderer.m_overflowHidden);
                this.WriteStream(HTML4Renderer.m_semiColon);
                this.RenderMeasurementWidth(num2, false);
                this.RenderMeasurementHeight(height1);
                break;
              case RPLTablix _:
                break;
              case RPLRectangle _:
label_71:
                if (empty || reportItem is RPLImage)
                {
                  this.WriteStream(HTML4Renderer.m_overflowHidden);
                  this.WriteStream(HTML4Renderer.m_semiColon);
                  break;
                }
                break;
              default:
                float height2 = ((RPLSizes) measurement).Height;
                float num3 = ((RPLSizes) measurement).Width;
                if (!styleContext.InTablix && this.IsImageNotFitProportional(reportItem, definition) && !styleContext.NoBorders)
                {
                  float adjustedWidth = this.GetAdjustedWidth(measurement, (IRPLStyle) props.Style);
                  if (this.m_deviceInfo.IsBrowserIE6Or7StandardsMode)
                  {
                    num3 = adjustedWidth;
                    height2 = this.GetAdjustedHeight(measurement, (IRPLStyle) props.Style);
                  }
                  this.RenderMeasurementMinWidth(adjustedWidth);
                }
                else
                  this.RenderMeasurementMinWidth(num3);
                if (reportItem is RPLHeaderFooter && (!this.m_deviceInfo.IsBrowserIE || this.m_deviceInfo.BrowserMode == BrowserMode.Standards && !this.m_deviceInfo.IsBrowserIE6))
                  this.RenderMeasurementMinHeight(height2);
                else
                  this.RenderMeasurementHeight(height2);
                this.RenderMeasurementWidth(num3, false);
                goto label_71;
            }
          }
        }
        if (!styleContext.InTablix && !styleContext.NoBorders)
        {
          this.RenderHtmlBorders(irplStyle, ref borderContext, styleContext.OmitBordersState, !styleContext.EmptyTextBox || this.m_deviceInfo.IsBrowserIE6Or7StandardsMode, isNonSharedStyles, sharedStyleProps);
          this.RenderBackgroundStyleProps(irplStyle);
        }
        if (styleContext.OnlyRenderMeasurementsBackgroundBorders || styleContext.EmptyTextBox && isNonSharedStyles)
          return;
        object val3 = irplStyle[(byte) 19];
        if (val3 != null)
        {
          object theString = (object) EnumStrings.GetValue((RPLFormat.FontStyles) val3);
          this.WriteStream(HTML4Renderer.m_fontStyle);
          this.WriteStream(theString);
          this.WriteStream(HTML4Renderer.m_semiColon);
        }
        object obj = irplStyle[(byte) 20];
        if (obj != null)
        {
          this.WriteStream(HTML4Renderer.m_fontFamily);
          this.WriteStream(HTML4Renderer.HandleSpecialFontCharacters(obj.ToString()));
          this.WriteStream(HTML4Renderer.m_semiColon);
        }
        object theString1 = irplStyle[(byte) 21];
        if (theString1 != null)
        {
          this.WriteStream(HTML4Renderer.m_fontSize);
          if (string.Compare(theString1.ToString(), "0pt", StringComparison.OrdinalIgnoreCase) != 0)
            this.WriteStream(theString1);
          else
            this.WriteFontSizeSmallPoint();
          this.WriteStream(HTML4Renderer.m_semiColon);
        }
        else
        {
          RPLTextBoxPropsDef rplTextBoxPropsDef = definition as RPLTextBoxPropsDef;
          RPLStyleProps sharedStyle = reportItem.ElementPropsDef.SharedStyle;
          if ((!isNonSharedStyles || sharedStyle == null || sharedStyle.Count == 0) && rplTextBoxPropsDef != null && !rplTextBoxPropsDef.IsSimple)
          {
            this.WriteStream(HTML4Renderer.m_fontSize);
            this.WriteFontSizeSmallPoint();
            this.WriteStream(HTML4Renderer.m_semiColon);
          }
        }
        object val4 = irplStyle[(byte) 22];
        if (val4 != null)
        {
          object theString2 = (object) EnumStrings.GetValue((RPLFormat.FontWeights) val4);
          this.WriteStream(HTML4Renderer.m_fontWeight);
          this.WriteStream(theString2);
          this.WriteStream(HTML4Renderer.m_semiColon);
        }
        object val5 = irplStyle[(byte) 24];
        if (val5 != null)
        {
          object theString3 = (object) EnumStrings.GetValue((RPLFormat.TextDecorations) val5);
          this.WriteStream(HTML4Renderer.m_textDecoration);
          this.WriteStream(theString3);
          this.WriteStream(HTML4Renderer.m_semiColon);
        }
        object val6 = irplStyle[(byte) 31];
        if (val6 != null)
        {
          object theString4 = (object) EnumStrings.GetValue((RPLFormat.UnicodeBiDiTypes) val6);
          this.WriteStream(HTML4Renderer.m_unicodeBiDi);
          this.WriteStream(theString4);
          this.WriteStream(HTML4Renderer.m_semiColon);
        }
        object theString5 = irplStyle[(byte) 27];
        if (theString5 != null)
        {
          this.WriteStream(HTML4Renderer.m_color);
          this.WriteStream(theString5);
          this.WriteStream(HTML4Renderer.m_semiColon);
        }
        object theString6 = irplStyle[(byte) 28];
        if (theString6 != null)
        {
          this.WriteStream(HTML4Renderer.m_lineHeight);
          this.WriteStream(theString6);
          this.WriteStream(HTML4Renderer.m_semiColon);
        }
        if ((HTML4Renderer.IsWritingModeVertical(sharedStyleProps) || HTML4Renderer.IsWritingModeVertical(nonSharedStyleProps)) && reportItem is RPLTextBox && styleContext.InTablix && this.m_deviceInfo.IsBrowserIE && !styleContext.IgnorePadding)
          this.RenderPaddingStyle(irplStyle);
        this.RenderDirectionStyles(reportItem, props, definition, measurement, sharedStyleProps, nonSharedStyleProps, isNonSharedStyles, styleContext);
        object val7 = irplStyle[(byte) 26];
        if (val7 != null && !styleContext.IgnoreVerticalAlign)
        {
          object theString7 = (object) EnumStrings.GetValue((RPLFormat.VerticalAlignments) val7);
          this.WriteStream(HTML4Renderer.m_verticalAlign);
          this.WriteStream(theString7);
          this.WriteStream(HTML4Renderer.m_semiColon);
        }
        object val8 = irplStyle[(byte) 25];
        if (val8 == null)
          return;
        if ((RPLFormat.TextAlignments) val8 != null)
        {
          this.WriteStream(HTML4Renderer.m_textAlign);
          this.WriteStream(EnumStrings.GetValue((RPLFormat.TextAlignments) val8));
          this.WriteStream(HTML4Renderer.m_semiColon);
        }
        else
          this.RenderTextAlign(props as RPLTextBoxProps, props.Style);
      }
    }

    protected void RenderLine(
      RPLLine reportItem,
      RPLElementProps rplProps,
      RPLLinePropsDef rplPropsDef,
      RPLItemMeasurement measurement,
      bool renderId,
      StyleContext styleContext)
    {
      if (this.IsLineSlanted(measurement))
      {
        if (renderId)
          this.RenderNavigationId(rplProps.UniqueName);
        if (this.m_deviceInfo.BrowserMode != BrowserMode.Quirks)
          return;
        this.RenderVMLLine(reportItem, measurement, styleContext);
      }
      else
      {
        bool flag = (double) ((RPLSizes) measurement).Height == 0.0;
        this.WriteStream(HTML4Renderer.m_openSpan);
        if (renderId)
          this.RenderReportItemId(rplProps.UniqueName);
        int borderContext = 0;
        object theString1 = rplProps.Style[(byte) 10];
        if (theString1 != null)
        {
          this.OpenStyle();
          if (flag)
            this.WriteStream(HTML4Renderer.m_styleHeight);
          else
            this.WriteStream(HTML4Renderer.m_styleWidth);
          this.WriteStream(theString1);
          this.WriteStream(HTML4Renderer.m_semiColon);
        }
        object theString2 = rplProps.Style[(byte) 0];
        if (theString2 != null)
        {
          this.OpenStyle();
          this.WriteStream(HTML4Renderer.m_backgroundColor);
          this.WriteStream(theString2);
        }
        this.RenderReportItemStyle((RPLElement) reportItem, measurement, ref borderContext);
        this.CloseStyle(true);
        this.WriteStream(HTML4Renderer.m_closeBracket);
        this.WriteStream(HTML4Renderer.m_closeSpan);
      }
    }

    protected bool GenerateHTMLTable(
      RPLItemMeasurement[] repItemCol,
      float ownerTop,
      float ownerLeft,
      float dxParent,
      float dyParent,
      int borderContext,
      bool expandLayout,
      SharedListLayoutState layoutState,
      List<RPLTablixMemberCell> omittedHeaders,
      IRPLStyle style)
    {
      bool htmlTable = false;
      object defaultBorderStyle = (object) null;
      object specificBorderStyle1 = (object) null;
      object specificBorderStyle2 = (object) null;
      object defaultBorderWidth = (object) null;
      object specificBorderWidth1 = (object) null;
      object specificBorderWidth2 = (object) null;
      if (style != null)
      {
        defaultBorderStyle = style[(byte) 5];
        specificBorderStyle1 = style[(byte) 6];
        specificBorderStyle2 = style[(byte) 7];
        defaultBorderWidth = style[(byte) 10];
        specificBorderWidth1 = style[(byte) 11];
        specificBorderWidth2 = style[(byte) 12];
      }
      if (repItemCol == null || repItemCol.Length == 0)
      {
        if (omittedHeaders != null)
        {
          for (int index = 0; index < omittedHeaders.Count; ++index)
          {
            if (omittedHeaders[index].GroupLabel != null)
              this.RenderNavigationId(omittedHeaders[index].UniqueName);
          }
        }
        return htmlTable;
      }
      PageTableLayout rgTableGrid = (PageTableLayout) null;
      PageTableLayout.GenerateTableLayout(repItemCol, dxParent, dyParent, 0.0f, ref rgTableGrid, expandLayout, this.m_rplReport.ConsumeContainerWhitespace);
      if (rgTableGrid == null)
        return htmlTable;
      if (rgTableGrid.BandTable && this.m_allowBandTable && layoutState == SharedListLayoutState.None && (!this.m_renderTableHeight || rgTableGrid.NrRows == 1))
      {
        if (omittedHeaders != null)
        {
          for (int index = 0; index < omittedHeaders.Count; ++index)
          {
            if (omittedHeaders[index].GroupLabel != null)
              this.RenderNavigationId(omittedHeaders[index].UniqueName);
          }
        }
        int borderContext1 = 0;
        int num;
        for (num = 0; num < rgTableGrid.NrRows - 1; ++num)
        {
          if (borderContext > 0)
            borderContext1 = HTML4Renderer.GetNewContext(borderContext, num + 1, 1, rgTableGrid.NrRows, 1);
          this.RenderCellItem(rgTableGrid.GetCell(num), borderContext1, false);
        }
        if (borderContext > 0)
          borderContext1 = HTML4Renderer.GetNewContext(borderContext, num + 1, 1, rgTableGrid.NrRows, 1);
        this.RenderCellItem(rgTableGrid.GetCell(num), borderContext1, false);
        return htmlTable;
      }
      this.m_allowBandTable = true;
      bool bfZeroRowReq = false;
      bool renderHeight = true;
      bool bfZeroColReq = expandLayout;
      int nrCols = rgTableGrid.NrCols;
      if (!bfZeroColReq)
        bfZeroColReq = rgTableGrid.AreSpansInColOne();
      if (layoutState == SharedListLayoutState.None || layoutState == SharedListLayoutState.Start)
      {
        this.WriteStream(HTML4Renderer.m_openTable);
        this.WriteStream(HTML4Renderer.m_zeroBorder);
        if (bfZeroColReq)
          ++nrCols;
        if (!this.m_deviceInfo.IsBrowserGeckoEngine)
        {
          this.WriteStream(HTML4Renderer.m_cols);
          this.WriteStream(nrCols.ToString((IFormatProvider) CultureInfo.InvariantCulture));
          this.WriteStream(HTML4Renderer.m_quote);
        }
        this.RenderReportLanguage();
        if (this.m_useInlineStyle)
        {
          this.OpenStyle();
          this.WriteStream(HTML4Renderer.m_borderCollapse);
          if (expandLayout)
          {
            this.WriteStream(HTML4Renderer.m_semiColon);
            this.WriteStream(HTML4Renderer.m_styleHeight);
            this.WriteStream(HTML4Renderer.m_percent);
          }
        }
        else
        {
          this.ClassLayoutBorder();
          if (expandLayout)
          {
            this.WriteStream(HTML4Renderer.m_space);
            this.WriteAttrEncoded(this.m_deviceInfo.HtmlPrefixId);
            this.WriteStream(HTML4Renderer.m_percentHeight);
          }
          this.WriteStream(HTML4Renderer.m_quote);
        }
        if (this.m_renderTableHeight)
        {
          if (this.m_isStyleOpen)
            this.WriteStream(HTML4Renderer.m_semiColon);
          else
            this.OpenStyle();
          this.WriteStream(HTML4Renderer.m_styleHeight);
          this.WriteDStream(dyParent);
          this.WriteStream(HTML4Renderer.m_mm);
          this.m_renderTableHeight = false;
        }
        if (this.m_deviceInfo.OutlookCompat || this.m_deviceInfo.IsBrowserSafari)
        {
          if (this.m_isStyleOpen)
            this.WriteStream(HTML4Renderer.m_semiColon);
          else
            this.OpenStyle();
          this.WriteStream(HTML4Renderer.m_styleWidth);
          float num = dxParent;
          if ((double) num > 0.0)
          {
            num = this.SubtractBorderStyles(this.SubtractBorderStyles(num, defaultBorderStyle, specificBorderStyle1, defaultBorderWidth, specificBorderWidth1), defaultBorderStyle, specificBorderStyle2, defaultBorderWidth, specificBorderWidth2);
            if ((double) num < 0.0)
              num = 1f;
          }
          this.WriteStream((object) num);
          this.WriteStream(HTML4Renderer.m_mm);
        }
        this.CloseStyle(true);
        this.WriteStream(HTML4Renderer.m_closeBracket);
        if (rgTableGrid.NrCols > 1)
        {
          bfZeroRowReq = rgTableGrid.NeedExtraRow();
          if (bfZeroRowReq)
          {
            this.WriteStream(HTML4Renderer.m_openTR);
            this.WriteStream(HTML4Renderer.m_zeroHeight);
            this.WriteStream(HTML4Renderer.m_closeBracket);
            if (bfZeroColReq)
            {
              this.WriteStream(HTML4Renderer.m_openTD);
              this.WriteStream(HTML4Renderer.m_openStyle);
              this.WriteStream(HTML4Renderer.m_styleWidth);
              this.WriteStream("0");
              this.WriteStream(HTML4Renderer.m_px);
              this.WriteStream(HTML4Renderer.m_closeQuote);
              this.WriteStream(HTML4Renderer.m_closeTD);
            }
            for (int index = 0; index < rgTableGrid.NrCols; ++index)
            {
              this.WriteStream(HTML4Renderer.m_openTD);
              this.WriteStream(HTML4Renderer.m_openStyle);
              this.WriteStream(HTML4Renderer.m_styleWidth);
              float num = rgTableGrid.GetCell(index).DXValue.Value;
              if ((double) num > 0.0)
              {
                if (index == 0)
                  num = this.SubtractBorderStyles(num, defaultBorderStyle, specificBorderStyle1, defaultBorderWidth, specificBorderWidth1);
                if (index == rgTableGrid.NrCols - 1)
                  num = this.SubtractBorderStyles(num, defaultBorderStyle, specificBorderStyle2, defaultBorderWidth, specificBorderWidth2);
                if ((double) num <= 0.0)
                  num = this.m_deviceInfo.BrowserMode != BrowserMode.Standards || !this.m_deviceInfo.IsBrowserIE ? 1f : rgTableGrid.GetCell(index).DXValue.Value;
              }
              this.WriteDStream(num);
              this.WriteStream(HTML4Renderer.m_mm);
              this.WriteStream(HTML4Renderer.m_semiColon);
              this.WriteStream(HTML4Renderer.m_styleMinWidth);
              this.WriteDStream(num);
              this.WriteStream(HTML4Renderer.m_mm);
              this.WriteStream(HTML4Renderer.m_closeQuote);
              this.WriteStream(HTML4Renderer.m_closeTD);
            }
            this.WriteStream(HTML4Renderer.m_closeTR);
          }
        }
      }
      this.GenerateTableLayoutContent(rgTableGrid, repItemCol, bfZeroRowReq, bfZeroColReq, renderHeight, borderContext, expandLayout, layoutState, omittedHeaders, style);
      if (layoutState == SharedListLayoutState.None || layoutState == SharedListLayoutState.End)
      {
        if (expandLayout)
        {
          this.WriteStream(HTML4Renderer.m_firstTD);
          this.ClassPercentHeight();
          this.WriteStream(HTML4Renderer.m_cols);
          this.WriteStream(nrCols.ToString((IFormatProvider) CultureInfo.InvariantCulture));
          this.WriteStream(HTML4Renderer.m_closeQuote);
          this.WriteStream(HTML4Renderer.m_lastTD);
        }
        this.WriteStreamCR(HTML4Renderer.m_closeTable);
      }
      return htmlTable;
    }

    protected void RenderZoom()
    {
      if (this.m_deviceInfo.Zoom == 100)
        return;
      this.WriteStream(HTML4Renderer.m_openStyle);
      this.WriteStream("zoom:");
      this.WriteStream(this.m_deviceInfo.Zoom.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      this.WriteStream("%\"");
    }

    protected void PredefinedStyles()
    {
      HTML4Renderer.PredefinedStyles(this.m_deviceInfo, (HTMLWriter) this, this.m_styleClassPrefix);
    }

    internal static void PredefinedStyles(DeviceInfo m_deviceInfo, HTMLWriter writer)
    {
      HTML4Renderer.PredefinedStyles(m_deviceInfo, writer, (byte[]) null);
    }

    internal static void PredefinedStyles(
      DeviceInfo deviceInfo,
      HTMLWriter writer,
      byte[] classStylePrefix)
    {
      HTML4Renderer.StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, HTML4Renderer.m_percentSizes);
      writer.WriteStream(HTML4Renderer.m_styleHeight);
      writer.WriteStream(HTML4Renderer.m_percent);
      writer.WriteStream(HTML4Renderer.m_semiColon);
      writer.WriteStream(HTML4Renderer.m_styleWidth);
      writer.WriteStream(HTML4Renderer.m_percent);
      writer.WriteStream(HTML4Renderer.m_closeAccol);
      HTML4Renderer.StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, HTML4Renderer.m_percentSizesOverflow);
      writer.WriteStream(HTML4Renderer.m_styleHeight);
      writer.WriteStream(HTML4Renderer.m_percent);
      writer.WriteStream(HTML4Renderer.m_semiColon);
      writer.WriteStream(HTML4Renderer.m_styleWidth);
      writer.WriteStream(HTML4Renderer.m_percent);
      writer.WriteStream(HTML4Renderer.m_semiColon);
      writer.WriteStream(HTML4Renderer.m_overflowHidden);
      writer.WriteStream(HTML4Renderer.m_closeAccol);
      HTML4Renderer.StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, HTML4Renderer.m_percentHeight);
      writer.WriteStream(HTML4Renderer.m_styleHeight);
      writer.WriteStream(HTML4Renderer.m_percent);
      writer.WriteStream(HTML4Renderer.m_closeAccol);
      HTML4Renderer.StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, HTML4Renderer.m_ignoreBorder);
      writer.WriteStream(HTML4Renderer.m_borderStyle);
      writer.WriteStream(HTML4Renderer.m_none);
      writer.WriteStream(HTML4Renderer.m_closeAccol);
      HTML4Renderer.StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, HTML4Renderer.m_ignoreBorderL);
      writer.WriteStream(HTML4Renderer.m_borderLeftStyle);
      writer.WriteStream(HTML4Renderer.m_none);
      writer.WriteStream(HTML4Renderer.m_closeAccol);
      HTML4Renderer.StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, HTML4Renderer.m_ignoreBorderR);
      writer.WriteStream(HTML4Renderer.m_borderRightStyle);
      writer.WriteStream(HTML4Renderer.m_none);
      writer.WriteStream(HTML4Renderer.m_closeAccol);
      HTML4Renderer.StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, HTML4Renderer.m_ignoreBorderT);
      writer.WriteStream(HTML4Renderer.m_borderTopStyle);
      writer.WriteStream(HTML4Renderer.m_none);
      writer.WriteStream(HTML4Renderer.m_closeAccol);
      HTML4Renderer.StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, HTML4Renderer.m_ignoreBorderB);
      writer.WriteStream(HTML4Renderer.m_borderBottomStyle);
      writer.WriteStream(HTML4Renderer.m_none);
      writer.WriteStream(HTML4Renderer.m_closeAccol);
      HTML4Renderer.StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, HTML4Renderer.m_layoutBorder);
      writer.WriteStream(HTML4Renderer.m_borderCollapse);
      writer.WriteStream(HTML4Renderer.m_closeAccol);
      HTML4Renderer.StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, HTML4Renderer.m_layoutFixed);
      writer.WriteStream(HTML4Renderer.m_borderCollapse);
      writer.WriteStream(HTML4Renderer.m_semiColon);
      writer.WriteStream(HTML4Renderer.m_tableLayoutFixed);
      writer.WriteStream(HTML4Renderer.m_closeAccol);
      HTML4Renderer.StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, HTML4Renderer.m_percentWidthOverflow);
      writer.WriteStream(HTML4Renderer.m_styleWidth);
      writer.WriteStream(HTML4Renderer.m_percent);
      writer.WriteStream(HTML4Renderer.m_semiColon);
      writer.WriteStream(HTML4Renderer.m_overflowXHidden);
      writer.WriteStream(HTML4Renderer.m_closeAccol);
      HTML4Renderer.StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, HTML4Renderer.m_popupAction);
      writer.WriteStream("position:absolute;display:none;background-color:white;border:1px solid black;");
      writer.WriteStream(HTML4Renderer.m_closeAccol);
      HTML4Renderer.StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, HTML4Renderer.m_styleAction);
      writer.WriteStream("text-decoration:none;color:black;cursor:pointer;");
      writer.WriteStream(HTML4Renderer.m_closeAccol);
      HTML4Renderer.StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, HTML4Renderer.m_emptyTextBox);
      writer.WriteStream(HTML4Renderer.m_fontSize);
      writer.WriteStream(deviceInfo.IsBrowserGeckoEngine ? HTML4Renderer.m_smallPoint : HTML4Renderer.m_zeroPoint);
      writer.WriteStream(HTML4Renderer.m_closeAccol);
      HTML4Renderer.StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, HTML4Renderer.m_rtlEmbed);
      writer.WriteStream(HTML4Renderer.m_direction);
      writer.WriteStream("RTL;");
      writer.WriteStream(HTML4Renderer.m_unicodeBiDi);
      writer.WriteStream(EnumStrings.GetValue((RPLFormat.UnicodeBiDiTypes) 1));
      writer.WriteStream(HTML4Renderer.m_closeAccol);
      HTML4Renderer.StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, HTML4Renderer.m_noVerticalMarginClassName);
      writer.WriteStream(HTML4Renderer.m_marginTop);
      writer.WriteStream(HTML4Renderer.m_zeroPoint);
      writer.WriteStream(HTML4Renderer.m_semiColon);
      writer.WriteStream(HTML4Renderer.m_marginBottom);
      writer.WriteStream(HTML4Renderer.m_zeroPoint);
      writer.WriteStream(HTML4Renderer.m_closeAccol);
      HTML4Renderer.StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, HTML4Renderer.m_percentSizeInlineTable);
      writer.WriteStream(HTML4Renderer.m_styleHeight);
      writer.WriteStream(HTML4Renderer.m_percent);
      writer.WriteStream(HTML4Renderer.m_semiColon);
      writer.WriteStream(HTML4Renderer.m_styleWidth);
      writer.WriteStream(HTML4Renderer.m_percent);
      writer.WriteStream(HTML4Renderer.m_semiColon);
      writer.WriteStream("display:inline-table");
      writer.WriteStream(HTML4Renderer.m_closeAccol);
      HTML4Renderer.StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, HTML4Renderer.m_percentHeightInlineTable);
      writer.WriteStream(HTML4Renderer.m_styleHeight);
      writer.WriteStream(HTML4Renderer.m_percent);
      writer.WriteStream(HTML4Renderer.m_semiColon);
      writer.WriteStream("display:inline-table");
      writer.WriteStream(HTML4Renderer.m_closeAccol);
      if (classStylePrefix != null)
        writer.WriteStream(classStylePrefix);
      writer.WriteStream(" * { ");
      string str = (string) null;
      if (deviceInfo.IsBrowserSafari)
        str = "-webkit-";
      else if (deviceInfo.IsBrowserGeckoEngine)
        str = "-moz-";
      if (!string.IsNullOrEmpty(str))
      {
        writer.WriteStream(str);
        writer.WriteStream("box-sizing: border-box; ");
      }
      writer.WriteStream("box-sizing: border-box }");
    }

    private static void StartPredefinedStyleClass(
      DeviceInfo deviceInfo,
      HTMLWriter writer,
      byte[] classStylePrefix,
      byte[] className)
    {
      if (classStylePrefix != null)
        writer.WriteStream(classStylePrefix);
      writer.WriteStream(HTML4Renderer.m_dot);
      writer.WriteStream(deviceInfo.HtmlPrefixId);
      writer.WriteStream(className);
      writer.WriteStream(HTML4Renderer.m_openAccol);
    }

    private void CheckBodyStyle()
    {
      RPLElementStyle style = this.m_pageContent.PageLayout.Style;
      string backgroundColor = (string) style[(byte) 34];
      this.m_pageHasStyle = backgroundColor != null || style[(byte) 33] != null || this.ReportPageHasBorder((IRPLStyle) style, backgroundColor);
    }

    private bool ReportPageBorder(
      IRPLStyle pageStyle,
      HTML4Renderer.Border border,
      string backgroundColor)
    {
      bool flag = false;
      byte num1;
      byte num2;
      byte num3;
      switch (border)
      {
        case HTML4Renderer.Border.All:
          num1 = (byte) 10;
          num2 = (byte) 5;
          num3 = (byte) 0;
          break;
        case HTML4Renderer.Border.Left:
          num1 = (byte) 11;
          num2 = (byte) 6;
          num3 = (byte) 1;
          break;
        case HTML4Renderer.Border.Right:
          num1 = (byte) 12;
          num2 = (byte) 7;
          num3 = (byte) 2;
          break;
        case HTML4Renderer.Border.Bottom:
          num1 = (byte) 14;
          num2 = (byte) 9;
          num3 = (byte) 4;
          break;
        default:
          num1 = (byte) 13;
          num2 = (byte) 8;
          num3 = (byte) 3;
          break;
      }
      object obj = pageStyle[num2];
      if (obj != null && (RPLFormat.BorderStyles) obj != null)
      {
        string str = (string) pageStyle[num1];
        if (str != null && new RPLReportSize(str).ToMillimeters() > 0.0 && (string) pageStyle[num3] != backgroundColor)
          flag = true;
      }
      return flag;
    }

    private void BorderBottomAttribute(HTML4Renderer.BorderAttribute attribute)
    {
      if (attribute == HTML4Renderer.BorderAttribute.BorderColor)
        this.WriteStream(HTML4Renderer.m_borderBottomColor);
      if (attribute == HTML4Renderer.BorderAttribute.BorderStyle)
        this.WriteStream(HTML4Renderer.m_borderBottomStyle);
      if (attribute != HTML4Renderer.BorderAttribute.BorderWidth)
        return;
      this.WriteStream(HTML4Renderer.m_borderBottomWidth);
    }

    private void BorderLeftAttribute(HTML4Renderer.BorderAttribute attribute)
    {
      if (attribute == HTML4Renderer.BorderAttribute.BorderColor)
        this.WriteStream(HTML4Renderer.m_borderLeftColor);
      if (attribute == HTML4Renderer.BorderAttribute.BorderStyle)
        this.WriteStream(HTML4Renderer.m_borderLeftStyle);
      if (attribute != HTML4Renderer.BorderAttribute.BorderWidth)
        return;
      this.WriteStream(HTML4Renderer.m_borderLeftWidth);
    }

    private void BorderRightAttribute(HTML4Renderer.BorderAttribute attribute)
    {
      if (attribute == HTML4Renderer.BorderAttribute.BorderColor)
        this.WriteStream(HTML4Renderer.m_borderRightColor);
      if (attribute == HTML4Renderer.BorderAttribute.BorderStyle)
        this.WriteStream(HTML4Renderer.m_borderRightStyle);
      if (attribute != HTML4Renderer.BorderAttribute.BorderWidth)
        return;
      this.WriteStream(HTML4Renderer.m_borderRightWidth);
    }

    private void BorderTopAttribute(HTML4Renderer.BorderAttribute attribute)
    {
      if (attribute == HTML4Renderer.BorderAttribute.BorderColor)
        this.WriteStream(HTML4Renderer.m_borderTopColor);
      if (attribute == HTML4Renderer.BorderAttribute.BorderStyle)
        this.WriteStream(HTML4Renderer.m_borderTopStyle);
      if (attribute != HTML4Renderer.BorderAttribute.BorderWidth)
        return;
      this.WriteStream(HTML4Renderer.m_borderTopWidth);
    }

    private void BorderAllAtribute(HTML4Renderer.BorderAttribute attribute)
    {
      if (attribute == HTML4Renderer.BorderAttribute.BorderColor)
        this.WriteStream(HTML4Renderer.m_borderColor);
      if (attribute == HTML4Renderer.BorderAttribute.BorderStyle)
        this.WriteStream(HTML4Renderer.m_borderStyle);
      if (attribute != HTML4Renderer.BorderAttribute.BorderWidth)
        return;
      this.WriteStream(HTML4Renderer.m_borderWidth);
    }

    private void RenderBorder(
      object styleAttribute,
      HTML4Renderer.Border border,
      HTML4Renderer.BorderAttribute borderAttribute)
    {
      if (styleAttribute == null)
        return;
      switch (border)
      {
        case HTML4Renderer.Border.All:
          this.BorderAllAtribute(borderAttribute);
          break;
        case HTML4Renderer.Border.Top:
          this.BorderTopAttribute(borderAttribute);
          break;
        case HTML4Renderer.Border.Right:
          this.BorderRightAttribute(borderAttribute);
          break;
        case HTML4Renderer.Border.Bottom:
          this.BorderBottomAttribute(borderAttribute);
          break;
        default:
          this.BorderLeftAttribute(borderAttribute);
          break;
      }
      this.WriteStream(styleAttribute);
      this.WriteStream(HTML4Renderer.m_semiColon);
    }

    private void RenderBorderStyle(
      object width,
      object style,
      object color,
      HTML4Renderer.Border border)
    {
      if (width == null && color == null && style == null)
        return;
      if (width != null && color != null && style != null)
      {
        string theString = EnumStrings.GetValue((RPLFormat.BorderStyles) style);
        switch (border)
        {
          case HTML4Renderer.Border.All:
            this.WriteStream(HTML4Renderer.m_border);
            break;
          case HTML4Renderer.Border.Left:
            this.WriteStream(HTML4Renderer.m_borderLeft);
            break;
          case HTML4Renderer.Border.Right:
            this.WriteStream(HTML4Renderer.m_borderRight);
            break;
          case HTML4Renderer.Border.Bottom:
            this.WriteStream(HTML4Renderer.m_borderBottom);
            break;
          default:
            this.WriteStream(HTML4Renderer.m_borderTop);
            break;
        }
        this.WriteStream(width);
        this.WriteStream(HTML4Renderer.m_space);
        this.WriteStream(theString);
        this.WriteStream(HTML4Renderer.m_space);
        this.WriteStream(color);
        this.WriteStream(HTML4Renderer.m_semiColon);
      }
      else
      {
        this.RenderBorder(color, border, HTML4Renderer.BorderAttribute.BorderColor);
        if (style != null)
          this.RenderBorder((object) EnumStrings.GetValue((RPLFormat.BorderStyles) style), border, HTML4Renderer.BorderAttribute.BorderStyle);
        this.RenderBorder(width, border, HTML4Renderer.BorderAttribute.BorderWidth);
      }
    }

    protected bool BorderInstance(
      IRPLStyle reportItemStyle,
      object defWidth,
      object defStyle,
      object defColor,
      ref object borderWidth,
      ref object borderStyle,
      ref object borderColor,
      HTML4Renderer.Border border)
    {
      byte num1 = 0;
      byte num2 = 0;
      byte num3 = 0;
      switch (border)
      {
        case HTML4Renderer.Border.Left:
          num1 = (byte) 11;
          num2 = (byte) 6;
          num3 = (byte) 1;
          break;
        case HTML4Renderer.Border.Top:
          num1 = (byte) 13;
          num2 = (byte) 8;
          num3 = (byte) 3;
          break;
        case HTML4Renderer.Border.Right:
          num1 = (byte) 12;
          num2 = (byte) 7;
          num3 = (byte) 2;
          break;
        case HTML4Renderer.Border.Bottom:
          num1 = (byte) 14;
          num2 = (byte) 9;
          num3 = (byte) 4;
          break;
      }
      if (reportItemStyle != null)
        borderStyle = reportItemStyle[num2];
      if (borderStyle == null)
        borderStyle = defStyle;
      if (borderStyle != null && (RPLFormat.BorderStyles) borderStyle == null)
        return false;
      object obj1 = reportItemStyle[num1];
      borderWidth = obj1 ?? defWidth;
      object obj2 = reportItemStyle[num3];
      borderColor = obj2 ?? defColor;
      return borderStyle != null || obj1 != null || obj2 != null;
    }

    private bool RenderBorderInstance(
      IRPLStyle reportItemStyle,
      object defWidth,
      object defStyle,
      object defColor,
      HTML4Renderer.Border border)
    {
      object borderWidth = (object) null;
      object borderColor = (object) null;
      object borderStyle = (object) null;
      bool flag = this.BorderInstance(reportItemStyle, defWidth, defStyle, defColor, ref borderWidth, ref borderStyle, ref borderColor, border);
      if (flag)
        this.RenderBorderStyle(borderWidth, borderStyle, borderColor, border);
      return flag;
    }

    private bool OnlyGeneralBorder(IRPLStyle style)
    {
      bool flag = true;
      if (style[(byte) 1] != null || style[(byte) 11] != null || style[(byte) 6] != null || style[(byte) 3] != null || style[(byte) 13] != null || style[(byte) 8] != null || style[(byte) 2] != null || style[(byte) 12] != null || style[(byte) 7] != null || style[(byte) 4] != null || style[(byte) 14] != null || style[(byte) 9] != null)
        flag = false;
      return flag;
    }

    protected string CreateImageStream(RPLImageData image)
    {
      if (image.ImageName == null)
        return (string) null;
      if (image.IsShared && this.m_images.ContainsKey(image.ImageName))
        return image.ImageName;
      if (this.m_createSecondaryStreams == null)
      {
        Stream stream = this.CreateStream(image.ImageName, string.Empty, (Encoding) null, image.ImageMimeType, false, StreamOper.CreateAndRegister);
        long imageDataOffset = image.ImageDataOffset;
        if (imageDataOffset >= 0L)
          this.m_rplReport.GetImage(imageDataOffset, stream);
        else if (image.ImageData != null)
          stream.Write(image.ImageData, 0, image.ImageData.Length);
      }
      if (image.IsShared)
        this.m_images.Add(image.ImageName, (string) null);
      return image.ImageName;
    }

    private void RenderAtStart(
      RPLTextBoxProps textBoxProps,
      IRPLStyle style,
      bool renderSort,
      bool renderToggle)
    {
      if (!renderSort && !renderToggle)
        return;
      object obj = style[(byte) 26];
      RPLFormat.VerticalAlignments val = (RPLFormat.VerticalAlignments) 0;
      if (obj != null)
        val = (RPLFormat.VerticalAlignments) obj;
      if (HTML4Renderer.IsWritingModeVertical(style) && this.m_deviceInfo.IsBrowserIE)
      {
        this.WriteStream(HTML4Renderer.m_openStyle);
        this.WriteStream(HTML4Renderer.m_textAlign);
        switch ((int) val)
        {
          case 0:
            this.WriteStream(HTML4Renderer.m_rightValue);
            break;
          case 1:
            this.WriteStream(HTML4Renderer.m_centerValue);
            break;
          case 2:
            this.WriteStream(HTML4Renderer.m_leftValue);
            break;
        }
        this.WriteStream(HTML4Renderer.m_quote);
        this.WriteStream(HTML4Renderer.m_closeBracket);
        if (renderSort)
          this.RenderSortImage(textBoxProps);
        if (renderToggle)
          this.RenderToggleImage(textBoxProps);
        this.WriteStream(HTML4Renderer.m_closeTD);
        this.WriteStream(HTML4Renderer.m_closeTR);
        this.WriteStream(HTML4Renderer.m_firstTD);
      }
      else
      {
        this.WriteStream(HTML4Renderer.m_openStyle);
        this.WriteStream(HTML4Renderer.m_verticalAlign);
        this.WriteStream(EnumStrings.GetValue(val));
        this.WriteStream(HTML4Renderer.m_quote);
        this.WriteStream(HTML4Renderer.m_closeBracket);
        if (renderSort)
          this.RenderSortImage(textBoxProps);
        if (renderToggle)
          this.RenderToggleImage(textBoxProps);
        this.WriteStream(HTML4Renderer.m_closeTD);
        this.WriteStream(HTML4Renderer.m_openTD);
      }
    }

    private void RenderAtEnd(
      RPLTextBoxProps textBoxProps,
      IRPLStyle style,
      bool renderSort,
      bool renderToggle)
    {
      if (!renderSort && !renderToggle)
        return;
      object obj = style[(byte) 26];
      RPLFormat.VerticalAlignments val = (RPLFormat.VerticalAlignments) 0;
      if (obj != null)
        val = (RPLFormat.VerticalAlignments) obj;
      this.WriteStream(HTML4Renderer.m_closeTD);
      if (HTML4Renderer.IsWritingModeVertical(style) && this.m_deviceInfo.IsBrowserIE)
      {
        this.WriteStream(HTML4Renderer.m_closeTR);
        this.WriteStream(HTML4Renderer.m_firstTD);
        this.WriteStream(HTML4Renderer.m_openStyle);
        this.WriteStream(HTML4Renderer.m_textAlign);
        switch ((int) val)
        {
          case 0:
            this.WriteStream(HTML4Renderer.m_rightValue);
            break;
          case 1:
            this.WriteStream(HTML4Renderer.m_centerValue);
            break;
          case 2:
            this.WriteStream(HTML4Renderer.m_leftValue);
            break;
        }
        this.WriteStream(HTML4Renderer.m_quote);
      }
      else
      {
        this.WriteStream(HTML4Renderer.m_openTD);
        this.WriteStream(HTML4Renderer.m_openStyle);
        this.WriteStream(HTML4Renderer.m_verticalAlign);
        this.WriteStream(EnumStrings.GetValue(val));
        this.WriteStream(HTML4Renderer.m_quote);
      }
      this.WriteStream(HTML4Renderer.m_closeBracket);
      if (renderSort)
        this.RenderSortImage(textBoxProps);
      if (!renderToggle)
        return;
      this.RenderToggleImage(textBoxProps);
    }

    private bool RenderHyperlink(RPLAction action, RPLFormat.TextDecorations textDec, string color)
    {
      this.WriteStream(HTML4Renderer.m_openA);
      this.RenderTabIndex();
      this.RenderActionHref(action, textDec, color);
      this.WriteStream(HTML4Renderer.m_closeBracket);
      return true;
    }

    private void RenderTabIndex()
    {
      this.WriteStream(HTML4Renderer.m_tabIndex);
      this.WriteStream((object) ++this.m_tabIndexNum);
      this.WriteStream(HTML4Renderer.m_quote);
    }

    private bool HasAction(RPLAction action)
    {
      return action.BookmarkLink != null || action.DrillthroughId != null || action.DrillthroughUrl != null || action.Hyperlink != null;
    }

    private bool HasAction(RPLActionInfo actionInfo)
    {
      return actionInfo != null && actionInfo.Actions != null && this.HasAction(actionInfo.Actions[0]);
    }

    protected abstract void RenderInteractionAction(RPLAction action, ref bool hasHref);

    private bool RenderActionHref(
      RPLAction action,
      RPLFormat.TextDecorations textDec,
      string color)
    {
      bool hasHref = false;
      if (action.Hyperlink != null)
      {
        this.WriteStream(HTML4Renderer.m_hrefString + HttpUtility.HtmlAttributeEncode(action.Hyperlink) + HTML4Renderer.m_quoteString);
        hasHref = true;
      }
      else
        this.RenderInteractionAction(action, ref hasHref);
      if (textDec != 1)
      {
        this.OpenStyle();
        this.WriteStream(HTML4Renderer.m_textDecoration);
        this.WriteStream(HTML4Renderer.m_none);
        this.WriteStream(HTML4Renderer.m_semiColon);
      }
      if (color != null)
      {
        this.OpenStyle();
        this.WriteStream(HTML4Renderer.m_color);
        this.WriteStream(color);
      }
      this.CloseStyle(true);
      if (this.m_deviceInfo.LinkTarget != null)
      {
        this.WriteStream(HTML4Renderer.m_target);
        this.WriteStream(this.m_deviceInfo.LinkTarget);
        this.WriteStream(HTML4Renderer.m_quote);
      }
      return hasHref;
    }

    protected void RenderControlActionScript(RPLAction action)
    {
      StringBuilder output = new StringBuilder();
      string actionType;
      if (action.DrillthroughId != null)
      {
        HTML4Renderer.QuoteString(output, action.DrillthroughId);
        actionType = "Drillthrough";
      }
      else
      {
        HTML4Renderer.QuoteString(output, action.BookmarkLink);
        actionType = "Bookmark";
      }
      this.RenderOnClickActionScript(actionType, output.ToString());
    }

    internal static bool IsDirectionRTL(IRPLStyle style)
    {
      object obj = style[(byte) 29];
      return obj != null && (RPLFormat.Directions) obj == 1;
    }

    internal static bool IsWritingModeVertical(IRPLStyle style)
    {
      if (style == null)
        return false;
      object writingMode = style[(byte) 30];
      return writingMode != null && HTML4Renderer.IsWritingModeVertical((RPLFormat.WritingModes) writingMode);
    }

    internal static bool IsWritingModeVertical(RPLFormat.WritingModes writingMode)
    {
      return writingMode == 1 || writingMode == 2;
    }

    internal static bool HasHorizontalPaddingStyles(IRPLStyle style)
    {
      if (style == null)
        return false;
      return style[(byte) 15] != null || style[(byte) 16] != null;
    }

    private void PercentSizes()
    {
      this.WriteStream(HTML4Renderer.m_openStyle);
      this.WriteStream(HTML4Renderer.m_styleHeight);
      this.WriteStream(HTML4Renderer.m_percent);
      this.WriteStream(HTML4Renderer.m_semiColon);
      this.WriteStream(HTML4Renderer.m_styleWidth);
      this.WriteStream(HTML4Renderer.m_percent);
      this.WriteStream(HTML4Renderer.m_quote);
    }

    private void PercentSizesOverflow()
    {
      this.WriteStream(HTML4Renderer.m_openStyle);
      this.WriteStream(HTML4Renderer.m_styleHeight);
      this.WriteStream(HTML4Renderer.m_percent);
      this.WriteStream(HTML4Renderer.m_semiColon);
      this.WriteStream(HTML4Renderer.m_styleWidth);
      this.WriteStream(HTML4Renderer.m_percent);
      this.WriteStream(HTML4Renderer.m_semiColon);
      this.WriteStream(HTML4Renderer.m_overflowHidden);
      this.WriteStream(HTML4Renderer.m_quote);
    }

    private void ClassLayoutBorder()
    {
      this.WriteClassName(HTML4Renderer.m_layoutBorder, HTML4Renderer.m_classLayoutBorder);
    }

    private void ClassPercentSizes()
    {
      this.WriteClassName(HTML4Renderer.m_percentSizes, HTML4Renderer.m_classPercentSizes);
    }

    private void ClassPercentSizesOverflow()
    {
      this.WriteClassName(HTML4Renderer.m_percentSizesOverflow, HTML4Renderer.m_classPercentSizesOverflow);
    }

    private void ClassPercentHeight()
    {
      this.WriteClassName(HTML4Renderer.m_percentHeight, HTML4Renderer.m_classPercentHeight);
    }

    private void RenderLanguage(string language)
    {
      if (string.IsNullOrEmpty(language))
        return;
      this.WriteStream(HTML4Renderer.m_language);
      this.WriteAttrEncoded(language);
      this.WriteStream(HTML4Renderer.m_quote);
    }

    private void RenderReportLanguage() => this.RenderLanguage(this.m_contextLanguage);

    private bool InitFixedColumnHeaders(
      RPLTablix tablix,
      string tablixID,
      TablixFixedHeaderStorage storage)
    {
      for (int index = 0; index < tablix.RowHeights.Length; ++index)
      {
        if (tablix.FixedRow(index))
        {
          storage.HtmlId = tablixID;
          storage.ColumnHeaders = new List<string>();
          return true;
        }
      }
      return false;
    }

    private bool InitFixedRowHeaders(
      RPLTablix tablix,
      string tablixID,
      TablixFixedHeaderStorage storage)
    {
      for (int index = 0; index < tablix.ColumnWidths.Length; ++index)
      {
        if (tablix.FixedColumns[index])
        {
          storage.HtmlId = tablixID;
          storage.RowHeaders = new List<string>();
          return true;
        }
      }
      return false;
    }

    private void RenderVMLLine(
      RPLLine line,
      RPLItemMeasurement measurement,
      StyleContext styleContext)
    {
      if (!this.m_hasSlantedLines)
      {
        this.WriteStream("<?XML:NAMESPACE PREFIX=v /><?IMPORT NAMESPACE=\"v\" IMPLEMENTATION=\"#default#VML\" />");
        this.m_hasSlantedLines = true;
      }
      this.WriteStream(HTML4Renderer.m_openVGroup);
      this.WriteStream(HTML4Renderer.m_openStyle);
      this.WriteStream(HTML4Renderer.m_styleWidth);
      if (styleContext.InTablix)
      {
        this.WriteStream(HTML4Renderer.m_percent);
        this.WriteStream(HTML4Renderer.m_semiColon);
        this.WriteStream(HTML4Renderer.m_styleHeight);
        this.WriteStream(HTML4Renderer.m_percent);
      }
      else
      {
        this.WriteRSStream(((RPLSizes) measurement).Width);
        this.WriteStream(HTML4Renderer.m_semiColon);
        this.WriteStream(HTML4Renderer.m_styleHeight);
        this.WriteRSStream(((RPLSizes) measurement).Height);
      }
      this.WriteStream(HTML4Renderer.m_closeQuote);
      this.WriteStream(HTML4Renderer.m_openVLine);
      if (((RPLLinePropsDef) ((RPLElement) line).ElementProps.Definition).Slant)
        this.WriteStream(HTML4Renderer.m_rightSlant);
      else
        this.WriteStream(HTML4Renderer.m_leftSlant);
      IRPLStyle style = (IRPLStyle) ((RPLElement) line).ElementProps.Style;
      string str = (string) style[(byte) 0];
      string theString1 = (string) style[(byte) 10];
      if (str != null && theString1 != null)
      {
        int num = new RPLReportColor(str).ToColor().ToArgb() & 16777215;
        this.WriteStream(HTML4Renderer.m_strokeColor);
        this.WriteStream("#");
        this.WriteStream(Convert.ToString(num, 16));
        this.WriteStream(HTML4Renderer.m_quote);
        this.WriteStream(HTML4Renderer.m_strokeWeight);
        this.WriteStream(theString1);
        this.WriteStream(HTML4Renderer.m_closeQuote);
      }
      string theString2 = "solid";
      string theString3 = (string) null;
      object val = style[(byte) 5];
      if (val != null)
      {
        string strA = EnumStrings.GetValue((RPLFormat.BorderStyles) val);
        if (string.CompareOrdinal(strA, "dashed") == 0)
          theString2 = "dash";
        else if (string.CompareOrdinal(strA, "dotted") == 0)
          theString2 = "dot";
        if (string.CompareOrdinal(strA, "double") == 0)
          theString3 = "thinthin";
      }
      this.WriteStream(HTML4Renderer.m_dashStyle);
      this.WriteStream(theString2);
      if (theString3 != null)
      {
        this.WriteStream(HTML4Renderer.m_quote);
        this.WriteStream(HTML4Renderer.m_slineStyle);
        this.WriteStream(theString3);
      }
      this.WriteStream(HTML4Renderer.m_quote);
      this.WriteStream(HTML4Renderer.m_closeTag);
      this.WriteStreamCR(HTML4Renderer.m_closeVGroup);
    }

    private List<string> RenderTableCellBorder(PageTableCell currCell, Hashtable renderedLines)
    {
      List<string> lineIDs = new List<string>(4);
      if (this.m_isStyleOpen)
        this.WriteStream(HTML4Renderer.m_semiColon);
      else
        this.OpenStyle();
      this.WriteStream(HTML4Renderer.m_zeroBorderWidth);
      RPLLine borderLeft = currCell.BorderLeft;
      if (borderLeft != null)
      {
        this.WriteStream(HTML4Renderer.m_semiColon);
        this.WriteStream(HTML4Renderer.m_borderLeft);
        this.RenderBorderLine((RPLElement) borderLeft);
        this.CheckForLineID(borderLeft, lineIDs, renderedLines);
      }
      RPLLine borderRight = currCell.BorderRight;
      if (borderRight != null)
      {
        this.WriteStream(HTML4Renderer.m_semiColon);
        this.WriteStream(HTML4Renderer.m_borderRight);
        this.RenderBorderLine((RPLElement) borderRight);
        this.CheckForLineID(borderRight, lineIDs, renderedLines);
      }
      RPLLine borderTop = currCell.BorderTop;
      if (borderTop != null)
      {
        this.WriteStream(HTML4Renderer.m_semiColon);
        this.WriteStream(HTML4Renderer.m_borderTop);
        this.RenderBorderLine((RPLElement) borderTop);
        this.CheckForLineID(borderTop, lineIDs, renderedLines);
      }
      RPLLine borderBottom = currCell.BorderBottom;
      if (borderBottom != null)
      {
        this.WriteStream(HTML4Renderer.m_semiColon);
        this.WriteStream(HTML4Renderer.m_borderBottom);
        this.RenderBorderLine((RPLElement) borderBottom);
        this.CheckForLineID(borderBottom, lineIDs, renderedLines);
      }
      return lineIDs;
    }

    private void CheckForLineID(RPLLine line, List<string> lineIDs, Hashtable renderedLines)
    {
      RPLElementProps elementProps = ((RPLElement) line).ElementProps;
      string uniqueName = elementProps.UniqueName;
      if (renderedLines.ContainsKey((object) uniqueName))
        return;
      if (this.NeedReportItemId((RPLElement) line, elementProps))
        lineIDs.Add(elementProps.UniqueName);
      renderedLines.Add((object) uniqueName, (object) uniqueName);
    }

    private int GenerateTableLayoutContent(
      PageTableLayout rgTableGrid,
      RPLItemMeasurement[] repItemCol,
      bool bfZeroRowReq,
      bool bfZeroColReq,
      bool renderHeight,
      int borderContext,
      bool layoutExpand,
      SharedListLayoutState layoutState,
      List<RPLTablixMemberCell> omittedHeaders,
      IRPLStyle style)
    {
      int y = 0;
      int num1 = 1;
      int num2 = 1;
      int num3 = 0;
      bool firstRow = true;
      Hashtable renderedLines = new Hashtable();
      int nrRows = rgTableGrid.NrRows;
      int nrCols = rgTableGrid.NrCols;
      int tableLayoutContent = 0;
      object defaultBorderStyle = (object) null;
      object specificBorderStyle1 = (object) null;
      object specificBorderStyle2 = (object) null;
      object specificBorderStyle3 = (object) null;
      object specificBorderStyle4 = (object) null;
      object defaultBorderWidth = (object) null;
      object specificBorderWidth1 = (object) null;
      object specificBorderWidth2 = (object) null;
      object specificBorderWidth3 = (object) null;
      object specificBorderWidth4 = (object) null;
      if (style != null)
      {
        defaultBorderStyle = style[(byte) 5];
        specificBorderStyle1 = style[(byte) 6];
        specificBorderStyle2 = style[(byte) 7];
        specificBorderStyle3 = style[(byte) 8];
        specificBorderStyle4 = style[(byte) 9];
        defaultBorderWidth = style[(byte) 10];
        specificBorderWidth1 = style[(byte) 11];
        specificBorderWidth2 = style[(byte) 12];
        specificBorderWidth3 = style[(byte) 13];
        specificBorderWidth4 = style[(byte) 14];
      }
      for (; y < nrRows; ++y)
      {
        int currRow = nrCols * y;
        PageTableCell cell1 = rgTableGrid.GetCell(currRow);
        bool flag1 = rgTableGrid.EmptyRow((RPLMeasurement[]) repItemCol, false, currRow, renderHeight, ref num3);
        this.WriteStream(HTML4Renderer.m_openTR);
        if (!flag1)
        {
          this.WriteStream(HTML4Renderer.m_valign);
          this.WriteStream(HTML4Renderer.m_topValue);
          this.WriteStream(HTML4Renderer.m_quote);
        }
        this.WriteStream(HTML4Renderer.m_closeBracket);
        bool flag2 = true;
        for (int x = 0; x < nrCols; ++x)
        {
          int index1 = x + currRow;
          bool flag3 = x == 0;
          if (flag3 && bfZeroColReq)
          {
            this.WriteStream(HTML4Renderer.m_openTD);
            if (renderHeight || num3 <= 0)
            {
              this.WriteStream(HTML4Renderer.m_openStyle);
              if (this.m_deviceInfo.OutlookCompat)
              {
                for (int index2 = 0; index2 < nrCols; ++index2)
                {
                  if (!rgTableGrid.GetCell(currRow + index2).NeedsRowHeight)
                  {
                    flag2 = false;
                    break;
                  }
                }
              }
              if (flag2)
              {
                this.WriteStream(HTML4Renderer.m_styleHeight);
                float num4 = cell1.DYValue.Value;
                if ((double) num4 > 0.0)
                {
                  if (y == 0)
                    num4 = this.SubtractBorderStyles(num4, defaultBorderStyle, specificBorderStyle3, defaultBorderWidth, specificBorderWidth3);
                  if (y == rgTableGrid.NrRows - num1)
                    num4 = this.SubtractBorderStyles(num4, defaultBorderStyle, specificBorderStyle4, defaultBorderWidth, specificBorderWidth4);
                  if ((double) num4 <= 0.0)
                    num4 = this.m_deviceInfo.BrowserMode != BrowserMode.Standards || !this.m_deviceInfo.IsBrowserIE ? 1f : cell1.DYValue.Value;
                }
                this.WriteDStream(num4);
                this.WriteStream(HTML4Renderer.m_mm);
                this.WriteStream(HTML4Renderer.m_semiColon);
              }
              this.WriteStream(HTML4Renderer.m_styleWidth);
              this.WriteDStream(0.0f);
              this.WriteStream(HTML4Renderer.m_mm);
              this.WriteStream(HTML4Renderer.m_quote);
            }
            else
            {
              this.WriteStream(HTML4Renderer.m_openStyle);
              this.WriteStream(HTML4Renderer.m_styleWidth);
              this.WriteDStream(0.0f);
              this.WriteStream(HTML4Renderer.m_mm);
              this.WriteStream(HTML4Renderer.m_quote);
            }
            this.WriteStream(HTML4Renderer.m_closeBracket);
            if (omittedHeaders != null)
            {
              for (int index3 = 0; index3 < omittedHeaders.Count; ++index3)
              {
                if (omittedHeaders[index3].GroupLabel != null)
                  this.RenderNavigationId(omittedHeaders[index3].UniqueName);
              }
            }
            this.WriteStream(HTML4Renderer.m_closeTD);
          }
          PageTableCell cell2 = rgTableGrid.GetCell(index1);
          if (!cell2.Eaten)
          {
            if (!cell2.InUse)
              HTML4Renderer.MergeEmptyCells(rgTableGrid, x, y, currRow, firstRow, cell2, nrRows, nrCols, index1);
            this.WriteStream(HTML4Renderer.m_openTD);
            num1 = cell2.RowSpan;
            if (num1 != 1)
            {
              this.WriteStream(HTML4Renderer.m_rowSpan);
              this.WriteStream(num1.ToString((IFormatProvider) CultureInfo.InvariantCulture));
              this.WriteStream(HTML4Renderer.m_quote);
            }
            if (!firstRow || bfZeroRowReq || layoutState == SharedListLayoutState.Continue || layoutState == SharedListLayoutState.End)
            {
              num2 = cell2.ColSpan;
              if (num2 != 1)
              {
                this.WriteStream(HTML4Renderer.m_colSpan);
                this.WriteStream(num2.ToString((IFormatProvider) CultureInfo.InvariantCulture));
                this.WriteStream(HTML4Renderer.m_quote);
              }
            }
            if (flag3 && !bfZeroColReq && (renderHeight || num3 <= 0))
            {
              float num5 = cell1.DYValue.Value;
              if ((double) num5 >= 0.0 && flag2 && (y != nrRows - 1 || !flag1 || layoutState != SharedListLayoutState.None) && (!this.m_deviceInfo.OutlookCompat || cell2.NeedsRowHeight))
              {
                this.OpenStyle();
                this.WriteStream(HTML4Renderer.m_styleHeight);
                if (y == 0)
                  num5 = this.SubtractBorderStyles(num5, defaultBorderStyle, specificBorderStyle3, defaultBorderWidth, specificBorderWidth3);
                if (y == rgTableGrid.NrRows - num1)
                  num5 = this.SubtractBorderStyles(num5, defaultBorderStyle, specificBorderStyle4, defaultBorderWidth, specificBorderWidth4);
                if ((double) num5 <= 0.0)
                  num5 = this.m_deviceInfo.BrowserMode != BrowserMode.Standards || !this.m_deviceInfo.IsBrowserIE ? 1f : cell1.DYValue.Value;
                this.WriteDStream(num5);
                this.WriteStream(HTML4Renderer.m_mm);
              }
            }
            if (this.m_deviceInfo.OutlookCompat || firstRow && !bfZeroRowReq && (layoutState == SharedListLayoutState.Start || layoutState == SharedListLayoutState.None))
            {
              float num6 = 0.0f;
              for (int index4 = 0; index4 < num2; ++index4)
                num6 += rgTableGrid.GetCell(x + index4).DXValue.Value;
              float num7 = num6;
              if (this.m_isStyleOpen)
                this.WriteStream(HTML4Renderer.m_semiColon);
              else
                this.OpenStyle();
              this.WriteStream(HTML4Renderer.m_styleWidth);
              if ((double) num7 > 0.0)
              {
                if (x == 0)
                  num7 = this.SubtractBorderStyles(num7, defaultBorderStyle, specificBorderStyle1, defaultBorderWidth, specificBorderWidth1);
                if (x == rgTableGrid.NrCols - num2)
                  num7 = this.SubtractBorderStyles(num7, defaultBorderStyle, specificBorderStyle2, defaultBorderWidth, specificBorderWidth2);
                if ((double) num7 <= 0.0)
                  num7 = this.m_deviceInfo.BrowserMode != BrowserMode.Standards || !this.m_deviceInfo.IsBrowserIE ? 1f : num6;
              }
              this.WriteDStream(num7);
              this.WriteStream(HTML4Renderer.m_mm);
              this.WriteStream(HTML4Renderer.m_semiColon);
              this.WriteStream(HTML4Renderer.m_styleMinWidth);
              this.WriteDStream(num7);
              this.WriteStream(HTML4Renderer.m_mm);
              this.WriteStream(HTML4Renderer.m_semiColon);
              if (flag2 && !cell2.InUse && this.m_deviceInfo.OutlookCompat)
              {
                float num8 = cell2.DYValue.Value;
                if ((double) num8 < 558.79998779296875)
                {
                  this.WriteStream(HTML4Renderer.m_styleHeight);
                  if ((double) num8 > 0.0)
                  {
                    if (y == 0)
                      num8 = this.SubtractBorderStyles(num8, defaultBorderStyle, specificBorderStyle3, defaultBorderWidth, specificBorderWidth3);
                    if (y == rgTableGrid.NrRows - num1)
                      num8 = this.SubtractBorderStyles(num8, defaultBorderStyle, specificBorderStyle4, defaultBorderWidth, specificBorderWidth4);
                    if ((double) num8 <= 0.0)
                      num8 = this.m_deviceInfo.BrowserMode != BrowserMode.Standards || !this.m_deviceInfo.IsBrowserIE ? 1f : cell2.DYValue.Value;
                  }
                  this.WriteDStream(num8);
                  this.WriteStream(HTML4Renderer.m_mm);
                  this.WriteStream(HTML4Renderer.m_semiColon);
                }
              }
            }
            List<string> stringList = (List<string>) null;
            if (cell2.HasBorder)
              stringList = this.RenderTableCellBorder(cell2, renderedLines);
            if (this.m_isStyleOpen)
            {
              this.CloseStyle(false);
              this.WriteStream(HTML4Renderer.m_closeQuote);
            }
            else
              this.WriteStream(HTML4Renderer.m_closeBracket);
            if (flag3 && !bfZeroColReq && omittedHeaders != null)
            {
              for (int index5 = 0; index5 < omittedHeaders.Count; ++index5)
              {
                if (omittedHeaders[index5].GroupLabel != null)
                  this.RenderNavigationId(omittedHeaders[index5].UniqueName);
              }
            }
            if (stringList != null && stringList.Count > 0)
            {
              for (int index6 = 0; index6 < stringList.Count; ++index6)
                this.RenderNavigationId(stringList[index6]);
            }
            if (cell2.InUse)
            {
              int xMax = nrRows - cell2.RowSpan + 1;
              if (xMax == y + 1 && cell2.KeepBottomBorder)
                ++xMax;
              int yMax = nrCols - cell2.ColSpan + 1;
              if (yMax == x + 1 && cell2.KeepRightBorder)
                ++yMax;
              int newContext = HTML4Renderer.GetNewContext(borderContext, y + 1, x + 1, xMax, yMax);
              if ((newContext & 8) > 0 && cell2.Measurement != null)
              {
                float height = ((RPLSizes) cell2.Measurement).Height;
                float num9 = cell2.DYValue.Value;
                for (int index7 = 1; index7 < cell2.RowSpan; ++index7)
                  num9 += rgTableGrid.GetCell(index1 + index7 * rgTableGrid.NrCols).DYValue.Value;
                if ((double) height < (double) num9)
                  newContext &= -9;
              }
              if ((newContext & 2) > 0 && cell2.Measurement != null)
              {
                float width = ((RPLSizes) cell2.Measurement).Width;
                float num10 = cell2.DXValue.Value;
                for (int index8 = 1; index8 < cell2.ColSpan; ++index8)
                  num10 += rgTableGrid.GetCell(index1 + index8).DXValue.Value;
                if ((double) width < (double) num10)
                  newContext &= -3;
              }
              this.RenderCellItem(cell2, newContext, layoutExpand);
            }
            else if (!this.m_browserIE && cell2.HasBorder)
              this.RenderBlankImage();
            this.WriteStream(HTML4Renderer.m_closeTD);
          }
        }
        this.WriteStream(HTML4Renderer.m_closeTR);
        firstRow = false;
        --num3;
      }
      return tableLayoutContent;
    }

    private static void MergeEmptyCells(
      PageTableLayout rgTableGrid,
      int x,
      int y,
      int currRow,
      bool firstRow,
      PageTableCell currCell,
      int numRows,
      int numCols,
      int index)
    {
      int num1 = index + 1;
      int num2 = currRow + numCols;
      if (currCell.BorderLeft == null && !firstRow)
      {
        while (num1 < num2)
        {
          PageTableCell cell = rgTableGrid.GetCell(num1++);
          if (!cell.Eaten && !cell.InUse && cell.BorderTop == currCell.BorderTop && cell.BorderBottom == currCell.BorderBottom && cell.BorderLeft == null)
          {
            cell.Eaten = true;
            ++currCell.ColSpan;
            currCell.BorderRight = cell.BorderRight;
          }
          else
            break;
        }
      }
      int num3 = index;
      int num4 = y + 1;
      int num5 = numCols * num4 + x;
      for (int index1 = numCols * numRows; num5 < index1; num5 = numCols * num4 + x)
      {
        PageTableCell cell1 = rgTableGrid.GetCell(num5);
        if (cell1.Eaten || cell1.InUse || cell1.BorderLeft != currCell.BorderLeft || cell1.BorderRight != currCell.BorderRight || cell1.BorderTop != null || currCell.ColSpan == 1 && currCell.BorderLeft == null && currCell.BorderRight == null)
          break;
        int num6 = 1;
        PageTableCell pageTableCell = cell1;
        for (; num6 < currCell.ColSpan; ++num6)
        {
          PageTableCell cell2 = rgTableGrid.GetCell(num3 + num6);
          PageTableCell cell3 = rgTableGrid.GetCell(num5 + num6);
          if (!cell3.InUse && !cell3.Eaten && cell3.BorderLeft == null && cell3.BorderRight == cell2.BorderRight && cell3.BorderTop == null && cell3.BorderBottom == pageTableCell.BorderBottom)
            pageTableCell = cell3;
          else
            break;
        }
        if (num6 != currCell.ColSpan)
          break;
        ++currCell.RowSpan;
        currCell.BorderBottom = cell1.BorderBottom;
        for (int index2 = 0; index2 < currCell.ColSpan; ++index2)
          rgTableGrid.GetCell(num5 + index2).Eaten = true;
        num3 = num5;
        ++num4;
      }
    }

    private void RenderIE7WritingMode(
      RPLFormat.WritingModes writingMode,
      RPLFormat.Directions direction,
      StyleContext styleContext)
    {
      this.WriteStream(HTML4Renderer.m_writingMode);
      if (HTML4Renderer.IsWritingModeVertical(writingMode))
      {
        if (direction == 1)
          this.WriteStream(HTML4Renderer.m_btrl);
        else
          this.WriteStream(HTML4Renderer.m_tbrl);
        if (writingMode == 2)
          HTML4Renderer.WriteRotate270(this.m_deviceInfo, styleContext, new Action<byte[]>(this.WriteStream));
      }
      else if (direction == 1)
        this.WriteStream(HTML4Renderer.m_rltb);
      else
        this.WriteStream(HTML4Renderer.m_lrtb);
      this.WriteStream(HTML4Renderer.m_semiColon);
    }

    internal static void WriteRotate270(
      DeviceInfo deviceInfo,
      StyleContext styleContext,
      Action<byte[]> WriteStream)
    {
      if (!deviceInfo.IsBrowserIE || styleContext == null || styleContext.StyleOnCell)
        return;
      if (!styleContext.RotationApplied)
      {
        WriteStream(HTML4Renderer.m_semiColon);
        WriteStream(HTML4Renderer.m_filter);
        WriteStream(HTML4Renderer.m_basicImageRotation180);
        styleContext.RotationApplied = true;
      }
      if (!deviceInfo.OutlookCompat)
        return;
      WriteStream(HTML4Renderer.m_semiColon);
      WriteStream(HTML4Renderer.m_msoRotation);
      WriteStream(HTML4Renderer.m_degree90);
    }

    private void RenderDirectionStyles(
      RPLElement reportItem,
      RPLElementProps props,
      RPLElementPropsDef definition,
      RPLItemMeasurement measurement,
      IRPLStyle sharedStyleProps,
      IRPLStyle nonSharedStyleProps,
      bool isNonSharedStyles,
      StyleContext styleContext)
    {
      IRPLStyle irplStyle = isNonSharedStyles ? nonSharedStyleProps : sharedStyleProps;
      bool flag1 = HTML4Renderer.HasHorizontalPaddingStyles(sharedStyleProps);
      bool flag2 = HTML4Renderer.HasHorizontalPaddingStyles(nonSharedStyleProps);
      object obj1 = irplStyle[(byte) 29];
      RPLFormat.Directions? nullable1 = new RPLFormat.Directions?();
      if (obj1 != null)
      {
        nullable1 = new RPLFormat.Directions?((RPLFormat.Directions) obj1);
        object theString = (object) EnumStrings.GetValue(nullable1.Value);
        this.WriteStream(HTML4Renderer.m_direction);
        this.WriteStream(theString);
        this.WriteStream(HTML4Renderer.m_semiColon);
      }
      object obj2 = irplStyle[(byte) 30];
      RPLFormat.WritingModes? nullable2 = new RPLFormat.WritingModes?();
      if (obj2 != null)
      {
        nullable2 = new RPLFormat.WritingModes?((RPLFormat.WritingModes) obj2);
        this.WriteStream(HTML4Renderer.m_layoutFlow);
        if (HTML4Renderer.IsWritingModeVertical(nullable2.Value))
          this.WriteStream(HTML4Renderer.m_verticalIdeographic);
        else
          this.WriteStream(HTML4Renderer.m_horizontal);
        this.WriteStream(HTML4Renderer.m_semiColon);
        if (this.m_deviceInfo.IsBrowserIE && HTML4Renderer.IsWritingModeVertical(nullable2.Value) && measurement != null && reportItem is RPLTextBox)
        {
          RPLTextBoxPropsDef rplTextBoxPropsDef = (RPLTextBoxPropsDef) definition;
          float height = ((RPLSizes) measurement).Height;
          float width = ((RPLSizes) measurement).Width;
          float adjustedWidth = this.GetAdjustedWidth(measurement, (IRPLStyle) props.Style);
          if (this.m_deviceInfo.IsBrowserIE6Or7StandardsMode)
          {
            width = adjustedWidth;
            height = this.GetAdjustedHeight(measurement, (IRPLStyle) props.Style);
          }
          if (rplTextBoxPropsDef.CanGrow)
          {
            if (styleContext != null && styleContext.InTablix && !this.m_deviceInfo.IsBrowserIE6Or7StandardsMode)
            {
              object obj3 = (object) null;
              if (flag2)
                obj3 = nonSharedStyleProps[(byte) 15];
              if (obj3 == null && flag1)
                obj3 = sharedStyleProps[(byte) 15];
              if (obj3 != null)
              {
                float millimeters = (float) new RPLReportSize(obj3 as string).ToMillimeters();
                width -= millimeters;
              }
              object obj4 = (object) null;
              if (flag2)
                obj4 = nonSharedStyleProps[(byte) 16];
              if (obj4 == null && flag1)
                obj4 = sharedStyleProps[(byte) 16];
              if (obj4 != null)
              {
                float millimeters = (float) new RPLReportSize(obj4 as string).ToMillimeters();
                width += millimeters;
              }
            }
            this.RenderMeasurementWidth((double) width >= 0.0 ? width : 0.0f);
          }
          else
          {
            this.WriteStream(HTML4Renderer.m_overflowHidden);
            this.WriteStream(HTML4Renderer.m_semiColon);
            this.RenderMeasurementWidth(width, false);
            this.RenderMeasurementHeight(height);
          }
          this.RenderMeasurementMinWidth(adjustedWidth);
        }
      }
      if (nullable2.HasValue && nullable1.HasValue)
      {
        this.RenderIE7WritingMode(nullable2.Value, nullable1.Value, styleContext);
      }
      else
      {
        if (!nullable2.HasValue && !nullable1.HasValue || !isNonSharedStyles)
          return;
        if (!nullable2.HasValue)
          nullable2 = new RPLFormat.WritingModes?((RPLFormat.WritingModes) definition.SharedStyle[(byte) 30]);
        else if (!nullable1.HasValue)
          nullable1 = new RPLFormat.Directions?((RPLFormat.Directions) definition.SharedStyle[(byte) 29]);
        this.RenderIE7WritingMode(nullable2.Value, nullable1.Value, styleContext);
      }
    }

    private void RenderReportItemStyle(
      RPLElement reportItem,
      RPLItemMeasurement measurement,
      ref int borderContext)
    {
      RPLElementProps elementProps = reportItem.ElementProps;
      RPLElementPropsDef definition = elementProps.Definition;
      this.RenderReportItemStyle(reportItem, elementProps, definition, measurement, new StyleContext(), ref borderContext, definition.ID);
    }

    private void RenderReportItemStyle(
      RPLElement reportItem,
      RPLItemMeasurement measurement,
      ref int borderContext,
      StyleContext styleContext)
    {
      RPLElementProps elementProps = reportItem.ElementProps;
      RPLElementPropsDef definition = elementProps.Definition;
      this.RenderReportItemStyle(reportItem, elementProps, definition, measurement, styleContext, ref borderContext, definition.ID);
    }

    private void RenderReportItemStyle(
      RPLElement reportItem,
      RPLElementProps elementProps,
      RPLElementPropsDef definition,
      RPLStyleProps nonSharedStyle,
      RPLStyleProps sharedStyle,
      RPLItemMeasurement measurement,
      StyleContext styleContext,
      ref int borderContext,
      string styleID)
    {
      if (this.m_useInlineStyle)
      {
        this.OpenStyle();
        RPLElementStyle sharedStyleProps = new RPLElementStyle(nonSharedStyle, sharedStyle);
        this.RenderStyleProps(reportItem, elementProps, definition, measurement, (IRPLStyle) sharedStyleProps, (IRPLStyle) null, styleContext, ref borderContext, false);
        if (styleContext.EmptyTextBox)
        {
          this.WriteStream(HTML4Renderer.m_fontSize);
          this.WriteFontSizeSmallPoint();
        }
        this.CloseStyle(true);
      }
      else
      {
        int borderContext1 = borderContext;
        bool flag = sharedStyle != null && sharedStyle.Count > 0;
        if (nonSharedStyle != null && nonSharedStyle.Count > 0)
        {
          bool renderMeasurements = styleContext.RenderMeasurements;
          if (flag)
            styleContext.RenderMeasurements = false;
          this.OpenStyle();
          this.RenderStyleProps(reportItem, elementProps, definition, measurement, (IRPLStyle) sharedStyle, (IRPLStyle) nonSharedStyle, styleContext, ref borderContext1, true);
          this.CloseStyle(true);
          styleContext.RenderMeasurements = renderMeasurements;
        }
        if (flag)
        {
          byte[] styleBytes = (byte[]) this.m_usedStyles[(object) styleID];
          if (styleBytes == null)
          {
            if (this.m_onlyVisibleStyles)
            {
              int borderContext2 = 0;
              styleBytes = this.RenderSharedStyle(reportItem, elementProps, definition, sharedStyle, nonSharedStyle, measurement, styleID, styleContext, ref borderContext2);
            }
            else
            {
              styleBytes = this.m_encoding.GetBytes(styleID);
              this.m_usedStyles.Add((object) styleID, (object) styleBytes);
            }
          }
          this.CloseStyle(true);
          this.WriteClassStyle(styleBytes, false);
          byte omitBordersState = styleContext.OmitBordersState;
          if (borderContext != 0 || omitBordersState != (byte) 0)
          {
            if (borderContext == 15)
            {
              this.WriteStream(HTML4Renderer.m_space);
              this.WriteStream(this.m_deviceInfo.HtmlPrefixId);
              this.WriteStream(HTML4Renderer.m_ignoreBorder);
            }
            else
            {
              if ((borderContext & 4) != 0 || ((int) omitBordersState & 1) != 0)
              {
                this.WriteStream(HTML4Renderer.m_space);
                this.WriteStream(this.m_deviceInfo.HtmlPrefixId);
                this.WriteStream(HTML4Renderer.m_ignoreBorderT);
              }
              if ((borderContext & 1) != 0 || ((int) omitBordersState & 4) != 0)
              {
                this.WriteStream(HTML4Renderer.m_space);
                this.WriteStream(this.m_deviceInfo.HtmlPrefixId);
                this.WriteStream(HTML4Renderer.m_ignoreBorderL);
              }
              if ((borderContext & 8) != 0 || ((int) omitBordersState & 2) != 0)
              {
                this.WriteStream(HTML4Renderer.m_space);
                this.WriteStream(this.m_deviceInfo.HtmlPrefixId);
                this.WriteStream(HTML4Renderer.m_ignoreBorderB);
              }
              if ((borderContext & 2) != 0 || ((int) omitBordersState & 8) != 0)
              {
                this.WriteStream(HTML4Renderer.m_space);
                this.WriteStream(this.m_deviceInfo.HtmlPrefixId);
                this.WriteStream(HTML4Renderer.m_ignoreBorderR);
              }
            }
          }
          if (styleContext.EmptyTextBox)
          {
            this.WriteStream(HTML4Renderer.m_space);
            this.WriteStream(this.m_deviceInfo.HtmlPrefixId);
            this.WriteStream(HTML4Renderer.m_emptyTextBox);
          }
          this.WriteStream(HTML4Renderer.m_quote);
          if (!styleContext.NoBorders)
            this.GetBorderContext((IRPLStyle) sharedStyle, ref borderContext, omitBordersState);
        }
        borderContext |= borderContext1;
      }
    }

    private void GetBorderContext(
      IRPLStyle styleProps,
      ref int borderContext,
      byte omitBordersState)
    {
      object styleProp1 = styleProps[(byte) 10];
      object styleProp2 = styleProps[(byte) 5];
      object styleProp3 = styleProps[(byte) 0];
      object borderWidth = (object) null;
      object borderStyle = (object) null;
      object borderColor = (object) null;
      if (borderContext != 0 || omitBordersState != (byte) 0 || !this.OnlyGeneralBorder(styleProps))
      {
        if ((borderContext & 8) == 0 && ((int) omitBordersState & 2) == 0 && this.BorderInstance(styleProps, styleProp1, styleProp2, styleProp3, ref borderWidth, ref borderStyle, ref borderColor, HTML4Renderer.Border.Bottom))
          borderContext |= 8;
        if ((borderContext & 1) == 0 && ((int) omitBordersState & 4) == 0 && this.BorderInstance(styleProps, styleProp1, styleProp2, styleProp3, ref borderWidth, ref borderStyle, ref borderColor, HTML4Renderer.Border.Left))
          borderContext |= 1;
        if ((borderContext & 2) == 0 && ((int) omitBordersState & 8) == 0 && this.BorderInstance(styleProps, styleProp1, styleProp2, styleProp3, ref borderWidth, ref borderStyle, ref borderColor, HTML4Renderer.Border.Right))
          borderContext |= 2;
        if ((borderContext & 4) != 0 || ((int) omitBordersState & 1) != 0 || !this.BorderInstance(styleProps, styleProp1, styleProp2, styleProp3, ref borderWidth, ref borderStyle, ref borderColor, HTML4Renderer.Border.Top))
          return;
        borderContext |= 4;
      }
      else
      {
        if (styleProp2 == null || (RPLFormat.BorderStyles) styleProp2 == null)
          return;
        borderContext = 15;
      }
    }

    private void RenderReportItemStyle(
      RPLElement reportItem,
      RPLElementProps elementProps,
      RPLElementPropsDef definition,
      RPLItemMeasurement measurement,
      StyleContext styleContext,
      ref int borderContext,
      string styleID)
    {
      this.RenderReportItemStyle(reportItem, elementProps, definition, elementProps.NonSharedStyle, definition.SharedStyle, measurement, styleContext, ref borderContext, styleID);
    }

    private void RenderPercentSizes()
    {
      this.WriteStream(HTML4Renderer.m_styleHeight);
      this.WriteStream(HTML4Renderer.m_percent);
      this.WriteStream(HTML4Renderer.m_semiColon);
      this.WriteStream(HTML4Renderer.m_styleWidth);
      this.WriteStream(HTML4Renderer.m_percent);
      this.WriteStream(HTML4Renderer.m_semiColon);
    }

    private void RenderTextAlign(RPLTextBoxProps props, RPLElementStyle style)
    {
      if (props == null)
        return;
      this.WriteStream(HTML4Renderer.m_textAlign);
      bool flag = HTML4Renderer.GetTextAlignForType(props);
      if (HTML4Renderer.IsDirectionRTL((IRPLStyle) style))
        flag = !flag;
      if (flag)
        this.WriteStream(HTML4Renderer.m_rightValue);
      else
        this.WriteStream(HTML4Renderer.m_leftValue);
      this.WriteStream(HTML4Renderer.m_semiColon);
    }

    internal static bool GetTextAlignForType(RPLTextBoxProps textBoxProps)
    {
      return HTML4Renderer.GetTextAlignForType(textBoxProps.TypeCode);
    }

    internal static bool GetTextAlignForType(TypeCode typeCode)
    {
      bool textAlignForType = false;
      switch (typeCode)
      {
        case TypeCode.Char:
        case TypeCode.SByte:
        case TypeCode.Byte:
        case TypeCode.Int16:
        case TypeCode.UInt16:
        case TypeCode.Int32:
        case TypeCode.UInt32:
        case TypeCode.Int64:
        case TypeCode.UInt64:
        case TypeCode.Single:
        case TypeCode.Double:
        case TypeCode.Decimal:
        case TypeCode.DateTime:
          textAlignForType = true;
          break;
      }
      return textAlignForType;
    }

    private bool HasBorderStyle(object borderStyle)
    {
      return borderStyle != null && (RPLFormat.BorderStyles) borderStyle != 0;
    }

    private float SubtractBorderStyles(
      float width,
      object defaultBorderStyle,
      object specificBorderStyle,
      object defaultBorderWidth,
      object specificBorderWidth)
    {
      object obj = specificBorderWidth ?? defaultBorderWidth;
      if (obj != null && (this.HasBorderStyle(specificBorderStyle) || this.HasBorderStyle(defaultBorderStyle)))
      {
        RPLReportSize rplReportSize = new RPLReportSize(obj as string);
        width -= (float) rplReportSize.ToMillimeters();
      }
      return width;
    }

    private float GetInnerContainerWidth(RPLMeasurement measurement, IRPLStyle containerStyle)
    {
      if (measurement == null)
        return -1f;
      float width = ((RPLSizes) measurement).Width;
      float num = 0.0f;
      object obj1 = containerStyle[(byte) 15];
      if (obj1 != null)
      {
        RPLReportSize rplReportSize = new RPLReportSize(obj1 as string);
        num += (float) rplReportSize.ToMillimeters();
      }
      object obj2 = containerStyle[(byte) 16];
      if (obj2 != null)
      {
        RPLReportSize rplReportSize = new RPLReportSize(obj2 as string);
        num += (float) rplReportSize.ToMillimeters();
      }
      return width - num;
    }

    private float GetInnerContainerWidthSubtractBorders(
      RPLItemMeasurement measurement,
      IRPLStyle containerStyle)
    {
      if (measurement == null)
        return -1f;
      float innerContainerWidth = this.GetInnerContainerWidth((RPLMeasurement) measurement, containerStyle);
      object defaultBorderStyle = containerStyle[(byte) 5];
      object defaultBorderWidth = containerStyle[(byte) 10];
      object specificBorderWidth1 = containerStyle[(byte) 11];
      object specificBorderStyle1 = containerStyle[(byte) 6];
      float width = this.SubtractBorderStyles(innerContainerWidth, defaultBorderStyle, specificBorderStyle1, defaultBorderWidth, specificBorderWidth1);
      object specificBorderWidth2 = containerStyle[(byte) 12];
      object specificBorderStyle2 = containerStyle[(byte) 7];
      float widthSubtractBorders = this.SubtractBorderStyles(width, defaultBorderStyle, specificBorderStyle2, defaultBorderWidth, specificBorderWidth2);
      if ((double) widthSubtractBorders <= 0.0)
        widthSubtractBorders = 1f;
      return widthSubtractBorders;
    }

    private float GetAdjustedWidth(RPLItemMeasurement measurement, IRPLStyle style)
    {
      float adjustedWidth = ((RPLSizes) measurement).Width;
      if (this.m_deviceInfo.BrowserMode == BrowserMode.Standards || !this.m_deviceInfo.IsBrowserIE)
        adjustedWidth = this.GetInnerContainerWidthSubtractBorders(measurement, style);
      return adjustedWidth;
    }

    private float GetAdjustedHeight(RPLItemMeasurement measurement, IRPLStyle style)
    {
      float adjustedHeight = ((RPLSizes) measurement).Height;
      if (this.m_deviceInfo.BrowserMode == BrowserMode.Standards || !this.m_deviceInfo.IsBrowserIE)
        adjustedHeight = this.GetInnerContainerHeightSubtractBorders(measurement, style);
      return adjustedHeight;
    }

    private float GetInnerContainerHeight(RPLItemMeasurement measurement, IRPLStyle containerStyle)
    {
      if (measurement == null)
        return -1f;
      float height = ((RPLSizes) measurement).Height;
      float num = 0.0f;
      object obj1 = containerStyle[(byte) 17];
      if (obj1 != null)
      {
        RPLReportSize rplReportSize = new RPLReportSize(obj1 as string);
        num += (float) rplReportSize.ToMillimeters();
      }
      object obj2 = containerStyle[(byte) 18];
      if (obj2 != null)
      {
        RPLReportSize rplReportSize = new RPLReportSize(obj2 as string);
        num += (float) rplReportSize.ToMillimeters();
      }
      return height - num;
    }

    private float GetInnerContainerHeightSubtractBorders(
      RPLItemMeasurement measurement,
      IRPLStyle containerStyle)
    {
      if (measurement == null)
        return -1f;
      float innerContainerHeight = this.GetInnerContainerHeight(measurement, containerStyle);
      object defaultBorderStyle = containerStyle[(byte) 5];
      object defaultBorderWidth = containerStyle[(byte) 10];
      object specificBorderWidth1 = containerStyle[(byte) 13];
      object specificBorderStyle1 = containerStyle[(byte) 8];
      float width = this.SubtractBorderStyles(innerContainerHeight, defaultBorderStyle, specificBorderStyle1, defaultBorderWidth, specificBorderWidth1);
      object specificBorderWidth2 = containerStyle[(byte) 14];
      object specificBorderStyle2 = containerStyle[(byte) 9];
      float heightSubtractBorders = this.SubtractBorderStyles(width, defaultBorderStyle, specificBorderStyle2, defaultBorderWidth, specificBorderWidth2);
      if ((double) heightSubtractBorders <= 0.0)
        heightSubtractBorders = 1f;
      return heightSubtractBorders;
    }

    private void RenderTextBoxContent(
      RPLTextBox textBox,
      RPLTextBoxProps tbProps,
      RPLTextBoxPropsDef tbDef,
      string textBoxValue,
      RPLStyleProps actionStyle,
      bool renderImages,
      RPLItemMeasurement measurement,
      RPLAction textBoxAction)
    {
      if (tbDef.IsSimple)
      {
        bool flag1 = false;
        bool flag2 = string.IsNullOrEmpty(textBoxValue);
        if (!flag2 && renderImages)
        {
          object val = ((RPLElementProps) tbProps).Style[(byte) 24];
          if (val != null && (RPLFormat.TextDecorations) val != null)
          {
            object theString = (object) EnumStrings.GetValue((RPLFormat.TextDecorations) val);
            flag1 = true;
            this.WriteStream(HTML4Renderer.m_openSpan);
            this.WriteStream(HTML4Renderer.m_openStyle);
            this.WriteStream(HTML4Renderer.m_textDecoration);
            this.WriteStream(theString);
            this.WriteStream(HTML4Renderer.m_closeQuote);
          }
        }
        if (flag2)
        {
          if (!this.NeedSharedToggleParent(tbProps))
            this.WriteStream(HTML4Renderer.m_nbsp);
        }
        else
        {
          List<int> hits = (List<int>) null;
          if (!string.IsNullOrEmpty(this.m_searchText))
          {
            int startIndex = 0;
            int length = this.m_searchText.Length;
            int num;
            for (; (num = textBoxValue.IndexOf(this.m_searchText, startIndex, StringComparison.OrdinalIgnoreCase)) != -1; startIndex = num + length)
            {
              if (hits == null)
                hits = new List<int>(2);
              hits.Add(num);
            }
            if (hits == null)
              this.RenderMultiLineText(textBoxValue);
            else
              this.RenderMultiLineTextWithHits(textBoxValue, hits);
          }
          else
            this.RenderMultiLineText(textBoxValue);
        }
        if (!flag1)
          return;
        this.WriteStream(HTML4Renderer.m_closeSpan);
      }
      else
      {
        this.WriteStream(HTML4Renderer.m_openDiv);
        RPLElementStyle style1 = ((RPLElementProps) tbProps).Style;
        bool flag3 = false;
        bool flag4 = HTML4Renderer.IsWritingModeVertical((IRPLStyle) style1);
        if (!this.m_deviceInfo.IsBrowserIE || !flag4)
        {
          this.OpenStyle();
          if (this.m_deviceInfo.IsBrowserIE)
          {
            this.WriteStream(HTML4Renderer.m_overflowXHidden);
            this.WriteStream(HTML4Renderer.m_semiColon);
          }
          double size = 0.0;
          if (measurement != null)
            size = (double) this.GetInnerContainerWidthSubtractBorders(measurement, (IRPLStyle) ((RPLElementProps) tbProps).Style);
          if (tbDef.CanSort && !this.IsFragment && !HTML4Renderer.IsDirectionRTL((IRPLStyle) ((RPLElementProps) tbProps).Style))
            size -= 4.2333332697550459;
          if (size > 0.0)
          {
            this.WriteStream(HTML4Renderer.m_styleWidth);
            this.WriteRSStream((float) size);
            this.WriteStream(HTML4Renderer.m_semiColon);
          }
        }
        if (HTML4Renderer.IsDirectionRTL((IRPLStyle) style1))
        {
          this.OpenStyle();
          this.WriteStream(HTML4Renderer.m_direction);
          this.WriteStream("rtl");
          this.CloseStyle(true);
          flag3 = true;
          this.WriteStream(HTML4Renderer.m_classStyle);
          this.WriteStream(HTML4Renderer.m_rtlEmbed);
        }
        else
          this.CloseStyle(true);
        if (textBoxAction != null)
        {
          if (!flag3)
          {
            flag3 = true;
            this.WriteStream(HTML4Renderer.m_classStyle);
          }
          else
            this.WriteStream(HTML4Renderer.m_space);
          this.WriteStream(HTML4Renderer.m_styleAction);
        }
        if (flag3)
          this.WriteStream(HTML4Renderer.m_quote);
        this.WriteStream(HTML4Renderer.m_closeBracket);
        TextRunStyleWriter trsw = new TextRunStyleWriter(this);
        ParagraphStyleWriter styleWriter = new ParagraphStyleWriter(this, textBox);
        RPLStyleProps nonSharedStyle1 = ((RPLElementProps) tbProps).NonSharedStyle;
        if (nonSharedStyle1 != null && (nonSharedStyle1[(byte) 30] != null || nonSharedStyle1[(byte) 29] != null))
          styleWriter.OutputSharedInNonShared = true;
        RPLParagraph nextParagraph = textBox.GetNextParagraph();
        ListLevelStack listLevelStack = (ListLevelStack) null;
        for (; nextParagraph != null; nextParagraph = textBox.GetNextParagraph())
        {
          RPLParagraphProps elementProps = ((RPLElement) nextParagraph).ElementProps as RPLParagraphProps;
          RPLParagraphPropsDef definition = ((RPLElementProps) elementProps).Definition as RPLParagraphPropsDef;
          int listLevel = elementProps.ListLevel ?? definition.ListLevel;
          RPLFormat.ListStyles style2 = elementProps.ListStyle ?? definition.ListStyle;
          string id = (string) null;
          RPLStyleProps nonSharedStyle2 = ((RPLElementProps) elementProps).NonSharedStyle;
          RPLStyleProps shared = (RPLStyleProps) null;
          if (definition != null)
          {
            if (listLevel == 0)
              listLevel = definition.ListLevel;
            if (style2 == null)
              style2 = definition.ListStyle;
            id = ((RPLElementPropsDef) definition).ID;
            if (!styleWriter.OutputSharedInNonShared)
              shared = ((RPLElementPropsDef) definition).SharedStyle;
          }
          styleWriter.Paragraph = nextParagraph;
          styleWriter.ParagraphMode = ParagraphStyleWriter.Mode.All;
          styleWriter.CurrentListLevel = listLevel;
          byte[] theBytes = (byte[]) null;
          if (listLevel > 0)
          {
            if (listLevelStack == null)
              listLevelStack = new ListLevelStack();
            bool writeNoVerticalMargin = !this.m_deviceInfo.IsBrowserIE || !flag4 || this.m_deviceInfo.BrowserMode == BrowserMode.Standards && !this.m_deviceInfo.IsBrowserIE6Or7StandardsMode;
            listLevelStack.PushTo(this, listLevel, style2, writeNoVerticalMargin);
            if (style2 != null)
            {
              if (this.m_deviceInfo.BrowserMode == BrowserMode.Quirks || this.m_deviceInfo.IsBrowserIE6Or7StandardsMode)
              {
                this.WriteStream(HTML4Renderer.m_openDiv);
                this.WriteStream(HTML4Renderer.m_closeBracket);
              }
              this.WriteStream(HTML4Renderer.m_openLi);
              styleWriter.ParagraphMode = ParagraphStyleWriter.Mode.ListOnly;
              this.WriteStyles(id + "l", nonSharedStyle2, shared, (ElementStyleWriter) styleWriter);
              this.WriteStream(HTML4Renderer.m_closeBracket);
              theBytes = HTML4Renderer.m_closeLi;
              styleWriter.ParagraphMode = ParagraphStyleWriter.Mode.ParagraphOnly;
              id += "p";
            }
          }
          else if (listLevelStack != null)
          {
            listLevelStack.PopAll();
            listLevelStack = (ListLevelStack) null;
          }
          this.WriteStream(HTML4Renderer.m_openDiv);
          this.WriteStyles(id, nonSharedStyle2, shared, (ElementStyleWriter) styleWriter);
          this.WriteStream(HTML4Renderer.m_closeBracket);
          RPLReportSize rplReportSize = elementProps.HangingIndent ?? definition.HangingIndent;
          float width = 0.0f;
          if (rplReportSize != null)
            width = (float) rplReportSize.ToMillimeters();
          if ((double) width > 0.0)
          {
            this.WriteStream(HTML4Renderer.m_openSpan);
            this.OpenStyle();
            this.RenderMeasurementWidth(width, true);
            this.WriteStream(HTML4Renderer.m_styleDisplayInlineBlock);
            this.CloseStyle(true);
            this.WriteStream(HTML4Renderer.m_closeBracket);
            if (this.m_deviceInfo.IsBrowserGeckoEngine)
              this.WriteStream(HTML4Renderer.m_nbsp);
            this.WriteStream(HTML4Renderer.m_closeSpan);
          }
          this.RenderTextRuns(nextParagraph, trsw, textBoxAction);
          this.WriteStream(HTML4Renderer.m_closeDiv);
          if (theBytes != null)
          {
            this.WriteStream(theBytes);
            if (this.m_deviceInfo.BrowserMode == BrowserMode.Quirks || this.m_deviceInfo.IsBrowserIE6Or7StandardsMode)
              this.WriteStream(HTML4Renderer.m_closeDiv);
          }
        }
        listLevelStack?.PopAll();
        this.WriteStream(HTML4Renderer.m_closeDiv);
      }
    }

    private void RenderTextRuns(
      RPLParagraph paragraph,
      TextRunStyleWriter trsw,
      RPLAction textBoxAction)
    {
      int num1 = 0;
      RPLTextRun rplTextRun;
      if (!string.IsNullOrEmpty(this.m_searchText))
      {
        RPLTextRun nextTextRun = paragraph.GetNextTextRun();
        rplTextRun = nextTextRun;
        List<RPLTextRun> rplTextRunList = new List<RPLTextRun>();
        StringBuilder stringBuilder = new StringBuilder();
        for (; nextTextRun != null; nextTextRun = paragraph.GetNextTextRun())
        {
          rplTextRunList.Add(nextTextRun);
          string str = (((RPLElement) nextTextRun).ElementProps as RPLTextRunProps).Value;
          if (string.IsNullOrEmpty(str))
            str = (((RPLElement) nextTextRun).ElementPropsDef as RPLTextRunPropsDef).Value;
          stringBuilder.Append(str);
        }
        string str1 = stringBuilder.ToString();
        int num2 = str1.IndexOf(this.m_searchText, StringComparison.OrdinalIgnoreCase);
        List<int> hits = new List<int>();
        int num3 = 0;
        int remainingChars = 0;
        int runOffsetCount = 0;
        int length = this.m_searchText.Length;
        for (int index = 0; index < rplTextRunList.Count; ++index)
        {
          RPLTextRun textRun = rplTextRunList[index];
          string str2 = (((RPLElement) textRun).ElementProps as RPLTextRunProps).Value;
          if (string.IsNullOrEmpty(str2))
            str2 = (((RPLElement) textRun).ElementPropsDef as RPLTextRunPropsDef).Value;
          if (!string.IsNullOrEmpty(str2))
          {
            for (; num2 > -1 && num2 < num3 + str2.Length; num2 = str1.IndexOf(this.m_searchText, num2 + length, StringComparison.OrdinalIgnoreCase))
              hits.Add(num2 - num3);
            if (hits.Count > 0 || remainingChars > 0)
            {
              num1 += this.RenderTextRunFindString(textRun, hits, remainingChars, ref runOffsetCount, trsw, textBoxAction);
              if (remainingChars > 0)
              {
                remainingChars -= str2.Length;
                if (remainingChars < 0)
                  remainingChars = 0;
              }
              if (hits.Count > 0)
              {
                int num4 = hits[hits.Count - 1];
                hits.Clear();
                if (str2.Length < num4 + length)
                  remainingChars = length - (str2.Length - num4);
              }
            }
            else
              num1 += this.RenderTextRun(textRun, trsw, textBoxAction);
            num3 += str2.Length;
          }
        }
      }
      else
      {
        RPLTextRun nextTextRun = paragraph.GetNextTextRun();
        rplTextRun = nextTextRun;
        for (; nextTextRun != null; nextTextRun = paragraph.GetNextTextRun())
          num1 += this.RenderTextRun(nextTextRun, trsw, textBoxAction);
      }
      if (num1 != 0 || rplTextRun == null)
        return;
      RPLTextRunProps elementProps = ((RPLElement) rplTextRun).ElementProps as RPLTextRunProps;
      RPLElementPropsDef definition = ((RPLElementProps) elementProps).Definition;
      this.WriteStream(HTML4Renderer.m_openSpan);
      this.WriteStyles(definition.ID, ((RPLElementProps) elementProps).NonSharedStyle, definition.SharedStyle, (ElementStyleWriter) trsw);
      this.WriteStream(HTML4Renderer.m_closeBracket);
      this.WriteStream(HTML4Renderer.m_nbsp);
      this.WriteStream(HTML4Renderer.m_closeSpan);
    }

    private int RenderTextRunFindString(
      RPLTextRun textRun,
      List<int> hits,
      int remainingChars,
      ref int runOffsetCount,
      TextRunStyleWriter trsw,
      RPLAction textBoxAction)
    {
      RPLTextRunProps elementProps = ((RPLElement) textRun).ElementProps as RPLTextRunProps;
      RPLTextRunPropsDef definition = ((RPLElementProps) elementProps).Definition as RPLTextRunPropsDef;
      RPLStyleProps shared = (RPLStyleProps) null;
      string id = (string) null;
      string str = elementProps.Value;
      string toolTip = elementProps.ToolTip;
      if (definition != null)
      {
        shared = ((RPLElementPropsDef) definition).SharedStyle;
        id = ((RPLElementPropsDef) definition).ID;
        if (string.IsNullOrEmpty(str))
          str = definition.Value;
        if (string.IsNullOrEmpty(toolTip))
          toolTip = definition.ToolTip;
      }
      if (string.IsNullOrEmpty(str))
        return 0;
      byte[] theBytes = HTML4Renderer.m_closeSpan;
      RPLAction action = (RPLAction) null;
      if (textBoxAction == null && this.HasAction(elementProps.ActionInfo))
        action = elementProps.ActionInfo.Actions[0];
      if (action != null)
      {
        this.WriteStream(HTML4Renderer.m_openA);
        this.RenderTabIndex();
        this.RenderActionHref(action, (RPLFormat.TextDecorations) 1, (string) null);
        theBytes = HTML4Renderer.m_closeA;
      }
      else
        this.WriteStream(HTML4Renderer.m_openSpan);
      if (toolTip != null)
        this.WriteToolTipAttribute(toolTip);
      this.WriteStyles(id, ((RPLElementProps) elementProps).NonSharedStyle, shared, (ElementStyleWriter) trsw);
      this.WriteStream(HTML4Renderer.m_closeBracket);
      int startIndex = 0;
      int length1 = str.Length;
      if (remainingChars > 0)
      {
        int length2 = remainingChars;
        if (length2 > length1)
          length2 = length1;
        if (length2 > 0)
        {
          this.OutputFindString(str.Substring(0, length2), runOffsetCount++);
          startIndex += length2;
          if (length2 >= remainingChars)
          {
            ++this.m_currentHitCount;
            runOffsetCount = 0;
          }
        }
      }
      int num = hits.Count - 1;
      bool flag = false;
      int length3 = this.m_searchText.Length;
      if (hits.Count > 0)
      {
        if (hits[hits.Count - 1] + length3 > length1)
          flag = true;
        else
          num = hits.Count;
      }
      for (int index = 0; index < num; ++index)
      {
        int hit = hits[index];
        if (startIndex < hit)
          this.RenderMultiLineText(str.Substring(startIndex, hit - startIndex));
        this.OutputFindString(str.Substring(hit, length3), 0);
        ++this.m_currentHitCount;
        runOffsetCount = 0;
        startIndex = hit + length3;
      }
      if (flag)
      {
        int hit = hits[hits.Count - 1];
        if (startIndex < hit)
          this.RenderMultiLineText(str.Substring(startIndex, hit - startIndex));
        this.OutputFindString(str.Substring(hit, length1 - hit), runOffsetCount++);
      }
      else if (startIndex < length1)
        this.RenderMultiLineText(str.Substring(startIndex));
      this.WriteStream(theBytes);
      return length1;
    }

    private int RenderTextRun(RPLTextRun textRun, TextRunStyleWriter trsw, RPLAction textBoxAction)
    {
      RPLTextRunProps elementProps = ((RPLElement) textRun).ElementProps as RPLTextRunProps;
      RPLTextRunPropsDef definition = ((RPLElementProps) elementProps).Definition as RPLTextRunPropsDef;
      RPLStyleProps shared = (RPLStyleProps) null;
      string id = (string) null;
      string text = elementProps.Value;
      string toolTip = elementProps.ToolTip;
      if (definition != null)
      {
        shared = ((RPLElementPropsDef) definition).SharedStyle;
        id = ((RPLElementPropsDef) definition).ID;
        if (string.IsNullOrEmpty(text))
          text = definition.Value;
        if (string.IsNullOrEmpty(toolTip))
          toolTip = definition.ToolTip;
      }
      if (string.IsNullOrEmpty(text))
        return 0;
      byte[] theBytes = HTML4Renderer.m_closeSpan;
      RPLAction action = (RPLAction) null;
      if (textBoxAction == null)
      {
        action = textBoxAction;
        if (this.HasAction(elementProps.ActionInfo))
          action = elementProps.ActionInfo.Actions[0];
      }
      if (action != null)
      {
        this.WriteStream(HTML4Renderer.m_openA);
        this.RenderTabIndex();
        this.RenderActionHref(action, (RPLFormat.TextDecorations) 1, (string) null);
        theBytes = HTML4Renderer.m_closeA;
      }
      else
        this.WriteStream(HTML4Renderer.m_openSpan);
      if (toolTip != null)
        this.WriteToolTipAttribute(toolTip);
      this.WriteStyles(id, ((RPLElementProps) elementProps).NonSharedStyle, shared, (ElementStyleWriter) trsw);
      this.RenderLanguage(((RPLElementProps) elementProps).Style[(byte) 32] as string);
      this.WriteStream(HTML4Renderer.m_closeBracket);
      this.RenderMultiLineText(text);
      this.WriteStream(theBytes);
      return text.Length;
    }

    private void WriteStyles(
      string id,
      RPLStyleProps nonShared,
      RPLStyleProps shared,
      ElementStyleWriter styleWriter)
    {
      bool flag = shared != null && shared.Count > 0 || styleWriter.NeedsToWriteNullStyle(StyleWriterMode.Shared);
      if (this.m_useInlineStyle || flag && id == null)
      {
        this.OpenStyle();
        styleWriter.WriteStyles(StyleWriterMode.All, (IRPLStyle) new RPLElementStyle(nonShared, shared));
        this.CloseStyle(true);
      }
      else
      {
        if (nonShared != null && nonShared.Count > 0 || styleWriter.NeedsToWriteNullStyle(StyleWriterMode.NonShared))
        {
          this.OpenStyle();
          styleWriter.WriteStyles(StyleWriterMode.NonShared, (IRPLStyle) nonShared);
          this.CloseStyle(true);
        }
        if (!flag || id == null)
          return;
        byte[] styleBytes = (byte[]) this.m_usedStyles[(object) id];
        if (styleBytes == null)
        {
          if (this.m_onlyVisibleStyles)
          {
            Stream mainStream = this.m_mainStream;
            this.m_mainStream = this.m_styleStream;
            this.RenderOpenStyle(id);
            styleWriter.WriteStyles(StyleWriterMode.Shared, (IRPLStyle) shared);
            this.WriteStream(HTML4Renderer.m_closeAccol);
            this.m_mainStream = mainStream;
            styleBytes = this.m_encoding.GetBytes(id);
            this.m_usedStyles.Add((object) id, (object) styleBytes);
          }
          else
          {
            styleBytes = this.m_encoding.GetBytes(id);
            this.m_usedStyles.Add((object) id, (object) styleBytes);
          }
        }
        this.CloseStyle(true);
        this.WriteClassStyle(styleBytes, true);
      }
    }

    protected abstract void WriteFitProportionalScript(double pv, double ph);

    private void RenderImageFitProportional(
      RPLImage image,
      RPLItemMeasurement measurement,
      PaddingSharedInfo padds,
      bool writeSmallSize)
    {
      if (!this.m_deviceInfo.AllowScript)
        return;
      this.m_fitPropImages = true;
      double pv = 0.0;
      double ph = 0.0;
      if (padds != null)
      {
        pv = padds.PadV;
        ph = padds.PadH;
      }
      this.WriteFitProportionalScript(pv, ph);
      if (writeSmallSize || !this.m_browserIE)
      {
        long num = 1;
        this.WriteStream(HTML4Renderer.m_inlineHeight);
        if (this.m_deviceInfo.IsBrowserSafari || this.m_deviceInfo.IsBrowserGeckoEngine)
        {
          num = 5L;
          if (measurement != null)
          {
            double size = (double) ((RPLSizes) measurement).Height;
            if ((double) ((RPLSizes) measurement).Width < size)
              size = (double) ((RPLSizes) measurement).Width;
            num = Utility.MMToPx(size);
            if (num < 5L)
              num = 5L;
          }
        }
        this.WriteStream(num.ToString((IFormatProvider) CultureInfo.InvariantCulture));
        this.WriteStream(HTML4Renderer.m_px);
        this.WriteStream(HTML4Renderer.m_quote);
      }
      if (!writeSmallSize)
        return;
      this.WriteStream(HTML4Renderer.m_inlineWidth);
      this.WriteStream("1");
      this.WriteStream(HTML4Renderer.m_px);
      this.WriteStream(HTML4Renderer.m_quote);
    }

    private void RenderImagePercent(
      RPLImage image,
      RPLImageProps imageProps,
      RPLImagePropsDef imagePropsDef,
      RPLItemMeasurement measurement)
    {
      bool flag1 = false;
      bool flag2 = false;
      RPLImageData image1 = imageProps.Image;
      RPLActionInfo actionInfo = imageProps.ActionInfo;
      RPLFormat.Sizings sizing = imagePropsDef.Sizing;
      if (sizing == 2 || sizing == 1 || sizing == 3)
      {
        flag1 = true;
        this.WriteStream(HTML4Renderer.m_openDiv);
        if (this.m_useInlineStyle)
          this.PercentSizesOverflow();
        else
          this.ClassPercentSizesOverflow();
        if (measurement != null)
        {
          this.OpenStyle();
          this.RenderMeasurementMinWidth(this.GetInnerContainerWidth((RPLMeasurement) measurement, (IRPLStyle) ((RPLElementProps) imageProps).Style));
          this.RenderMeasurementMinHeight(this.GetInnerContainerHeight(measurement, (IRPLStyle) ((RPLElementProps) imageProps).Style));
          this.CloseStyle(true);
        }
      }
      int xOffset = 0;
      int yOffset = 0;
      Rectangle consolidationOffsets = imageProps.Image.ImageConsolidationOffsets;
      bool flag3 = !consolidationOffsets.IsEmpty;
      if (flag3)
      {
        if (!flag1)
        {
          flag1 = true;
          this.WriteStream(HTML4Renderer.m_openDiv);
          if (sizing != null)
          {
            if (this.m_useInlineStyle)
              this.PercentSizesOverflow();
            else
              this.ClassPercentSizesOverflow();
          }
        }
        if (sizing == 3 || sizing == 2 || sizing == 1)
        {
          this.WriteStream(HTML4Renderer.m_closeBracket);
          this.WriteStream(HTML4Renderer.m_openDiv);
          if (this.m_deviceInfo.IsBrowserIE6 && this.m_deviceInfo.IsBrowserIE6Or7StandardsMode && measurement != null)
          {
            this.WriteStream(" origWidth=\"");
            this.WriteRSStream(((RPLSizes) measurement).Width);
            this.WriteStream("\" origHeight=\"");
            this.WriteStream("\"");
          }
        }
        this.WriteOuterConsolidation(consolidationOffsets, sizing, ((RPLElementProps) imageProps).UniqueName);
        this.CloseStyle(true);
        xOffset = consolidationOffsets.Left;
        yOffset = consolidationOffsets.Top;
      }
      else if (this.m_deviceInfo.AllowScript && sizing == 1 && this.m_deviceInfo.BrowserMode == BrowserMode.Standards)
      {
        flag1 = true;
        this.WriteStream(HTML4Renderer.m_openDiv);
        if (this.m_imgFitDivIdsStream == null)
          this.CreateImgFitDivImageIdsStream();
        this.WriteIdToSecondaryStream(this.m_imgFitDivIdsStream, ((RPLElementProps) imageProps).UniqueName + "_ifd");
        this.RenderReportItemId(((RPLElementProps) imageProps).UniqueName + "_ifd");
      }
      if (flag1)
        this.WriteStream(HTML4Renderer.m_closeBracket);
      if (this.HasAction(actionInfo))
        flag2 = this.RenderElementHyperlink((IRPLStyle) ((RPLElementProps) imageProps).Style, actionInfo.Actions[0]);
      this.WriteStream(HTML4Renderer.m_img);
      if (this.m_browserIE)
        this.WriteStream(HTML4Renderer.m_imgOnError);
      if (imageProps.ActionImageMapAreas != null && imageProps.ActionImageMapAreas.Length > 0)
      {
        this.WriteAttrEncoded(HTML4Renderer.m_useMap, "#" + this.m_deviceInfo.HtmlPrefixId + HTML4Renderer.m_mapPrefixString + ((RPLElementProps) imageProps).UniqueName);
        this.WriteStream(HTML4Renderer.m_zeroBorder);
      }
      else if (flag2)
        this.WriteStream(HTML4Renderer.m_zeroBorder);
      if (sizing == 2)
      {
        PaddingSharedInfo padds = (PaddingSharedInfo) null;
        if (this.m_deviceInfo.IsBrowserSafari)
          padds = this.GetPaddings(((RPLElement) image).ElementProps.Style, (PaddingSharedInfo) null);
        bool writeSmallSize = !flag3 && this.m_deviceInfo.BrowserMode == BrowserMode.Standards;
        this.RenderImageFitProportional(image, (RPLItemMeasurement) null, padds, writeSmallSize);
      }
      else if (sizing == 1 && !flag3)
      {
        if (this.m_deviceInfo.AllowScript && this.m_deviceInfo.BrowserMode == BrowserMode.Standards)
          this.WriteStream(" width=\"1px\" height=\"1px\"");
        else if (this.m_useInlineStyle)
          this.PercentSizes();
        else
          this.ClassPercentSizes();
      }
      if (flag3)
        this.WriteClippedDiv(consolidationOffsets);
      this.WriteToolTip((RPLElementProps) imageProps);
      this.WriteStream(HTML4Renderer.m_src);
      this.RenderImageUrl(true, image1);
      this.WriteStream(HTML4Renderer.m_closeTag);
      if (flag2)
        this.WriteStream(HTML4Renderer.m_closeA);
      if (imageProps.ActionImageMapAreas != null && imageProps.ActionImageMapAreas.Length > 0)
        this.RenderImageMapAreas(imageProps.ActionImageMapAreas, (double) ((RPLSizes) measurement).Width, (double) ((RPLSizes) measurement).Height, ((RPLElementProps) imageProps).UniqueName, xOffset, yOffset);
      if (flag3 && (sizing == 3 || sizing == 2 || sizing == 1))
        this.WriteStream(HTML4Renderer.m_closeDiv);
      if (!flag1)
        return;
      this.WriteStreamCR(HTML4Renderer.m_closeDiv);
    }

    private void RenderImageMapAreas(
      RPLActionInfoWithImageMap[] actionImageMaps,
      double width,
      double height,
      string uniqueName,
      int xOffset,
      int yOffset)
    {
      double imageWidth = width * 96.0 * (5.0 / (double) sbyte.MaxValue);
      double imageHeight = height * 96.0 * (5.0 / (double) sbyte.MaxValue);
      this.WriteStream(HTML4Renderer.m_openMap);
      this.WriteAttrEncoded(HTML4Renderer.m_name, this.m_deviceInfo.HtmlPrefixId + HTML4Renderer.m_mapPrefixString + uniqueName);
      this.WriteStreamCR(HTML4Renderer.m_closeBracket);
      for (int index = 0; index < actionImageMaps.Length; ++index)
      {
        RPLActionInfoWithImageMap actionImageMap = actionImageMaps[index];
        if (actionImageMap != null)
          this.RenderImageMapArea(actionImageMap, imageWidth, imageHeight, uniqueName, xOffset, yOffset);
      }
      this.WriteStream(HTML4Renderer.m_closeMap);
    }

    protected void RenderImageMapArea(
      RPLActionInfoWithImageMap actionImageMap,
      double imageWidth,
      double imageHeight,
      string uniqueName,
      int xOffset,
      int yOffset)
    {
      RPLAction action = (RPLAction) null;
      if (((RPLActionInfo) actionImageMap).Actions != null && ((RPLActionInfo) actionImageMap).Actions.Length > 0)
      {
        action = ((RPLActionInfo) actionImageMap).Actions[0];
        if (!this.HasAction(action))
          action = (RPLAction) null;
      }
      if (actionImageMap.ImageMaps == null || actionImageMap.ImageMaps.Count <= 0)
        return;
      for (int index1 = 0; index1 < actionImageMap.ImageMaps.Count; ++index1)
      {
        RPLImageMap imageMap = actionImageMap.ImageMaps[index1];
        string toolTip = imageMap.ToolTip;
        if (action != null || toolTip != null)
        {
          this.WriteStream(HTML4Renderer.m_mapArea);
          this.RenderTabIndex();
          if (toolTip != null)
            this.WriteToolTipAttribute(toolTip);
          if (action != null)
            this.RenderActionHref(action, (RPLFormat.TextDecorations) 0, (string) null);
          else
            this.WriteStream(HTML4Renderer.m_nohref);
          this.WriteStream(HTML4Renderer.m_mapShape);
          switch (imageMap.Shape - 1)
          {
            case 0:
              this.WriteStream(HTML4Renderer.m_polyShape);
              break;
            case 1:
              this.WriteStream(HTML4Renderer.m_circleShape);
              break;
            default:
              this.WriteStream(HTML4Renderer.m_rectShape);
              break;
          }
          this.WriteStream(HTML4Renderer.m_quote);
          this.WriteStream(HTML4Renderer.m_mapCoords);
          float[] coordinates = imageMap.Coordinates;
          bool flag = true;
          int index2 = 0;
          if (coordinates != null)
          {
            for (; index2 < coordinates.Length - 1; index2 += 2)
            {
              if (!flag)
                this.WriteStream(HTML4Renderer.m_comma);
              this.WriteStream((object) ((long) ((double) coordinates[index2] / 100.0 * imageWidth) + (long) xOffset));
              this.WriteStream(HTML4Renderer.m_comma);
              this.WriteStream((object) ((long) ((double) coordinates[index2 + 1] / 100.0 * imageHeight) + (long) yOffset));
              flag = false;
            }
            if (index2 < coordinates.Length)
            {
              this.WriteStream(HTML4Renderer.m_comma);
              this.WriteStream((object) (long) ((double) coordinates[index2] / 100.0 * imageWidth));
            }
          }
          this.WriteStream(HTML4Renderer.m_quote);
          this.WriteStreamCR(HTML4Renderer.m_closeBracket);
        }
      }
    }

    protected void RenderCreateFixedHeaderFunction(
      string prefix,
      string fixedHeaderObject,
      StringBuilder function,
      StringBuilder arrayBuilder,
      bool createHeadersWithArray)
    {
      int num = 0;
      StringBuilder stringBuilder = function;
      if (createHeadersWithArray)
        stringBuilder = arrayBuilder;
      foreach (TablixFixedHeaderStorage fixedHeader in this.m_fixedHeaders)
      {
        string str1 = "frgh" + (object) num + (object) '_' + fixedHeader.HtmlId;
        string str2 = "fcgh" + (object) num + (object) '_' + fixedHeader.HtmlId;
        string str3 = "fch" + (object) num + (object) '_' + fixedHeader.HtmlId;
        string str4 = this.m_deviceInfo.HtmlPrefixId + str1;
        string str5 = this.m_deviceInfo.HtmlPrefixId + str2;
        string str6 = this.m_deviceInfo.HtmlPrefixId + str3;
        if (fixedHeader.ColumnHeaders != null)
        {
          string str7 = prefix + "fcghArr" + (object) num;
          arrayBuilder.Append(str7);
          arrayBuilder.Append("=new Array('");
          arrayBuilder.Append(fixedHeader.HtmlId);
          arrayBuilder.Append('\'');
          for (int index = 0; index < fixedHeader.ColumnHeaders.Count; ++index)
          {
            arrayBuilder.Append(",'");
            arrayBuilder.Append(fixedHeader.ColumnHeaders[index]);
            arrayBuilder.Append('\'');
          }
          arrayBuilder.Append(");");
          if (!createHeadersWithArray)
          {
            arrayBuilder.Append(str5);
            arrayBuilder.Append("=null;");
            function.Append("if (!");
            function.Append(str5);
            function.Append("){");
            function.Append(str5);
            function.Append("=");
          }
          stringBuilder.Append(fixedHeaderObject);
          stringBuilder.Append(".CreateFixedColumnHeader(");
          stringBuilder.Append(str7);
          stringBuilder.Append(",'");
          stringBuilder.Append(str2);
          stringBuilder.Append("');");
          if (!createHeadersWithArray)
            function.Append("}");
        }
        if (fixedHeader.RowHeaders != null)
        {
          string str8 = prefix + "frhArr" + (object) num;
          arrayBuilder.Append(str8);
          arrayBuilder.Append("=new Array('");
          arrayBuilder.Append(fixedHeader.HtmlId);
          arrayBuilder.Append('\'');
          for (int index = 0; index < fixedHeader.RowHeaders.Count; ++index)
          {
            arrayBuilder.Append(",'");
            arrayBuilder.Append(fixedHeader.RowHeaders[index]);
            arrayBuilder.Append('\'');
          }
          arrayBuilder.Append(");");
          if (!createHeadersWithArray)
          {
            arrayBuilder.Append(str4);
            arrayBuilder.Append("=null;");
            function.Append("if (!");
            function.Append(str4);
            function.Append("){");
            function.Append(str4);
            function.Append("=");
          }
          stringBuilder.Append(fixedHeaderObject);
          stringBuilder.Append(".CreateFixedRowHeader(");
          stringBuilder.Append(str8);
          stringBuilder.Append(",'");
          stringBuilder.Append(str1);
          stringBuilder.Append("');");
          if (!createHeadersWithArray)
            function.Append("}");
        }
        if (fixedHeader.CornerHeaders != null)
        {
          string str9 = prefix + "fchArr" + (object) num;
          arrayBuilder.Append(str9);
          arrayBuilder.Append("=new Array('");
          arrayBuilder.Append(fixedHeader.HtmlId);
          arrayBuilder.Append('\'');
          for (int index = 0; index < fixedHeader.CornerHeaders.Count; ++index)
          {
            arrayBuilder.Append(",'");
            arrayBuilder.Append(fixedHeader.CornerHeaders[index]);
            arrayBuilder.Append('\'');
          }
          arrayBuilder.Append(");");
          if (!createHeadersWithArray)
          {
            arrayBuilder.Append(str6);
            arrayBuilder.Append("=null;");
            function.Append("if (!");
            function.Append(str6);
            function.Append("){");
            function.Append(str6);
            function.Append("=");
          }
          stringBuilder.Append(fixedHeaderObject);
          stringBuilder.Append(".CreateFixedRowHeader(");
          stringBuilder.Append(str9);
          stringBuilder.Append(",'");
          stringBuilder.Append(str3);
          stringBuilder.Append("');");
          if (!createHeadersWithArray)
            function.Append("}");
        }
        function.Append(fixedHeaderObject);
        function.Append(".ShowFixedTablixHeaders('");
        function.Append(fixedHeader.HtmlId);
        function.Append("','");
        function.Append(fixedHeader.BodyID != null ? fixedHeader.BodyID : fixedHeader.HtmlId);
        function.Append("','");
        function.Append(str1);
        function.Append("','");
        function.Append(str2);
        function.Append("','");
        function.Append(str3);
        function.Append("','");
        function.Append(fixedHeader.FirstRowGroupCol);
        function.Append("','");
        function.Append(fixedHeader.LastRowGroupCol);
        function.Append("','");
        function.Append(fixedHeader.LastColGroupRow);
        function.Append("');");
        ++num;
      }
    }

    private void RenderServerDynamicImage(
      RPLElement dynamicImage,
      RPLDynamicImageProps dynamicImageProps,
      RPLElementPropsDef def,
      RPLItemMeasurement measurement,
      int borderContext,
      bool renderId,
      StyleContext styleContext)
    {
      if (dynamicImage == null)
        return;
      bool flag1 = dynamicImageProps.ActionImageMapAreas != null && dynamicImageProps.ActionImageMapAreas.Length > 0;
      Rectangle rectangle = this.RenderDynamicImage(measurement, dynamicImageProps);
      int xOffset = 0;
      int yOffset = 0;
      bool flag2 = !rectangle.IsEmpty;
      bool flag3 = !this.m_deviceInfo.IsBrowserSafari || this.m_deviceInfo.AllowScript || !styleContext.InTablix;
      if (flag3)
        this.WriteStream(HTML4Renderer.m_openDiv);
      bool flag4 = this.m_deviceInfo.DataVisualizationFitSizing == DataVisualizationFitSizing.Exact && styleContext.InTablix;
      if (flag2)
      {
        RPLFormat.Sizings sizing = flag4 ? (RPLFormat.Sizings) 1 : (RPLFormat.Sizings) 0;
        this.WriteOuterConsolidation(rectangle, sizing, ((RPLElementProps) dynamicImageProps).UniqueName);
        this.RenderReportItemStyle(dynamicImage, (RPLItemMeasurement) null, ref borderContext);
        xOffset = rectangle.Left;
        yOffset = rectangle.Top;
      }
      else if (flag4 && this.m_deviceInfo.AllowScript)
      {
        if (this.m_imgFitDivIdsStream == null)
          this.CreateImgFitDivImageIdsStream();
        this.WriteIdToSecondaryStream(this.m_imgFitDivIdsStream, ((RPLElementProps) dynamicImageProps).UniqueName + "_ifd");
        this.RenderReportItemId(((RPLElementProps) dynamicImageProps).UniqueName + "_ifd");
      }
      if (flag3)
        this.WriteStream(HTML4Renderer.m_closeBracket);
      this.WriteStream(HTML4Renderer.m_img);
      if (this.m_browserIE)
        this.WriteStream(HTML4Renderer.m_imgOnError);
      if (renderId)
        this.RenderReportItemId(((RPLElementProps) dynamicImageProps).UniqueName);
      this.WriteStream(HTML4Renderer.m_zeroBorder);
      bool flag5 = dynamicImage is RPLChart;
      if (flag1)
      {
        this.WriteAttrEncoded(HTML4Renderer.m_useMap, "#" + this.m_deviceInfo.HtmlPrefixId + HTML4Renderer.m_mapPrefixString + ((RPLElementProps) dynamicImageProps).UniqueName);
        if (flag4)
        {
          this.OpenStyle();
          if (this.m_useInlineStyle && !flag2)
          {
            this.WriteStream(HTML4Renderer.m_styleHeight);
            this.WriteStream(HTML4Renderer.m_percent);
            this.WriteStream(HTML4Renderer.m_semiColon);
            this.WriteStream(HTML4Renderer.m_styleWidth);
            this.WriteStream(HTML4Renderer.m_percent);
            this.WriteStream(HTML4Renderer.m_semiColon);
            flag5 = false;
          }
          this.WriteStream("border-style:none;");
        }
      }
      else if (flag4 && this.m_useInlineStyle && !flag2)
      {
        this.PercentSizes();
        flag5 = false;
      }
      StyleContext styleContext1 = new StyleContext();
      if (!flag4 && (this.m_deviceInfo.IsBrowserIE7 || this.m_deviceInfo.IsBrowserIE6))
      {
        styleContext1.RenderMeasurements = false;
        styleContext1.RenderMinMeasurements = false;
      }
      if (!flag2)
      {
        if (flag4)
          this.RenderReportItemStyle(dynamicImage, (RPLItemMeasurement) null, ref borderContext, styleContext1);
        else if (flag5)
        {
          RPLElementProps elementProps = dynamicImage.ElementProps;
          StyleContext styleContext2 = new StyleContext();
          styleContext2.RenderMeasurements = false;
          this.OpenStyle();
          this.RenderMeasurementStyle(((RPLSizes) measurement).Height, ((RPLSizes) measurement).Width);
          this.RenderReportItemStyle(dynamicImage, elementProps, def, measurement, styleContext2, ref borderContext, def.ID);
        }
        else
          this.RenderReportItemStyle(dynamicImage, measurement, ref borderContext, styleContext1);
      }
      else
        this.WriteClippedDiv(rectangle);
      this.WriteToolTip((RPLElementProps) dynamicImageProps);
      this.WriteStream(HTML4Renderer.m_src);
      this.RenderDynamicImageSrc(dynamicImageProps);
      this.WriteStreamCR(HTML4Renderer.m_closeTag);
      if (flag1)
        this.RenderImageMapAreas(dynamicImageProps.ActionImageMapAreas, (double) ((RPLSizes) measurement).Width, (double) ((RPLSizes) measurement).Height, ((RPLElementProps) dynamicImageProps).UniqueName, xOffset, yOffset);
      if (!flag3)
        return;
      this.WriteStream(HTML4Renderer.m_closeDiv);
    }

    private void RenderBorderLine(RPLElement reportItem)
    {
      IRPLStyle style = (IRPLStyle) reportItem.ElementProps.Style;
      object obj = style[(byte) 10];
      if (obj != null)
      {
        this.WriteStream(obj.ToString());
        this.WriteStream(HTML4Renderer.m_space);
      }
      object val = style[(byte) 5];
      if (val != null)
      {
        this.WriteStream((object) EnumStrings.GetValue((RPLFormat.BorderStyles) val));
        this.WriteStream(HTML4Renderer.m_space);
      }
      object theString = style[(byte) 0];
      if (theString == null)
        return;
      this.WriteStream((string) theString);
    }

    private string CalculateRowHeaderId(
      RPLTablixCell cell,
      bool fixedHeader,
      string tablixID,
      int row,
      int col,
      TablixFixedHeaderStorage headerStorage,
      bool useElementName,
      bool fixedCornerHeader)
    {
      string rowHeaderId = (string) null;
      if (cell is RPLTablixMemberCell)
      {
        if (((RPLTablixMemberCell) cell).GroupLabel != null)
          rowHeaderId = ((RPLTablixMemberCell) cell).UniqueName;
        else if (!fixedHeader && useElementName && cell.Element != null && ((RPLElement) cell.Element).ElementProps != null)
          rowHeaderId = ((RPLElement) cell.Element).ElementProps.UniqueName;
      }
      if (fixedHeader)
      {
        if (rowHeaderId == null)
          rowHeaderId = tablixID + "r" + (object) row + "c" + (object) col;
        if (headerStorage != null)
        {
          headerStorage.RowHeaders.Add(rowHeaderId);
          if (headerStorage.CornerHeaders != null && fixedCornerHeader)
            headerStorage.CornerHeaders.Add(rowHeaderId);
        }
      }
      return rowHeaderId;
    }

    private void RenderAccessibleHeaders(
      RPLTablix tablix,
      bool fixedHeader,
      int numCols,
      int col,
      int colSpan,
      int row,
      RPLTablixCell cell,
      List<RPLTablixMemberCell> omittedCells,
      HTMLHeader[] rowHeaderIds,
      string[] colHeaderIds,
      OmittedHeaderStack omittedHeaders,
      ref string id)
    {
      int currentLevel = -1;
      if (tablix.RowHeaderColumns == 0 && omittedCells != null && omittedCells.Count > 0)
      {
        foreach (RPLTablixMemberCell omittedCell in omittedCells)
        {
          RPLTablixMemberDef tablixMemberDef = omittedCell.TablixMemberDef;
          if (tablixMemberDef != null && tablixMemberDef.IsStatic && tablixMemberDef.StaticHeadersTree)
          {
            if (id == null && cell.Element != null && ((RPLElement) cell.Element).ElementProps.UniqueName != null)
              id = ((RPLElement) cell.Element).ElementProps.UniqueName;
            currentLevel = tablixMemberDef.Level;
            omittedHeaders.Push(tablixMemberDef.Level, col, colSpan, id, numCols);
          }
        }
      }
      if (row < tablix.ColumnHeaderRows || fixedHeader || col >= tablix.ColsBeforeRowHeaders && tablix.RowHeaderColumns > 0 && col < tablix.RowHeaderColumns + tablix.ColsBeforeRowHeaders)
        return;
      bool flag = false;
      string colHeaderId = colHeaderIds[cell.ColIndex];
      if (!string.IsNullOrEmpty(colHeaderId))
      {
        this.WriteStream(HTML4Renderer.m_headers);
        this.WriteStream(colHeaderId);
        flag = true;
      }
      foreach (HTMLHeader rowHeaderId in rowHeaderIds)
      {
        string id1 = rowHeaderId.ID;
        if (!string.IsNullOrEmpty(id1))
        {
          if (flag)
            this.WriteStream(HTML4Renderer.m_space);
          else
            this.WriteStream(HTML4Renderer.m_headers);
          this.WriteAttrEncoded(this.m_deviceInfo.HtmlPrefixId);
          this.WriteStream(id1);
          flag = true;
        }
      }
      string headers = omittedHeaders.GetHeaders(col, currentLevel, HttpUtility.HtmlAttributeEncode(this.m_deviceInfo.HtmlPrefixId));
      if (!string.IsNullOrEmpty(headers))
      {
        if (flag)
          this.WriteStream(HTML4Renderer.m_space);
        else
          this.WriteStream(HTML4Renderer.m_headers);
        this.WriteStream(headers);
        flag = true;
      }
      if (!flag)
        return;
      this.WriteStream(HTML4Renderer.m_quote);
    }

    private void RenderTablixCell(
      RPLTablix tablix,
      bool fixedHeader,
      string tablixID,
      int numCols,
      int numRows,
      int col,
      int colSpan,
      int row,
      int tablixContext,
      RPLTablixCell cell,
      List<RPLTablixMemberCell> omittedCells,
      ref int omittedIndex,
      StyleContext styleContext,
      TablixFixedHeaderStorage headerStorage,
      HTMLHeader[] rowHeaderIds,
      string[] colHeaderIds,
      OmittedHeaderStack omittedHeaders)
    {
      bool lastCol = col + colSpan == numCols;
      bool zeroWidth = styleContext.ZeroWidth;
      float columnWidth = tablix.GetColumnWidth(cell.ColIndex, cell.ColSpan);
      styleContext.ZeroWidth = (double) columnWidth == 0.0;
      int startIndex = this.RenderZeroWidthTDsForTablix(col, colSpan, tablix);
      colSpan = this.GetColSpanMinusZeroWidthColumns(col, colSpan, tablix);
      bool useElementName = this.m_deviceInfo.AccessibleTablix && tablix.RowHeaderColumns > 0 && col >= tablix.ColsBeforeRowHeaders && col < tablix.RowHeaderColumns + tablix.ColsBeforeRowHeaders;
      bool fixedCornerHeader = fixedHeader && tablix.FixedColumns[col] && tablix.FixedRow(row);
      string rowHeaderId = this.CalculateRowHeaderId(cell, fixedHeader, tablixID, cell.RowIndex, cell.ColIndex, headerStorage, useElementName, fixedCornerHeader);
      this.WriteStream(HTML4Renderer.m_openTD);
      if (this.m_deviceInfo.AccessibleTablix)
        this.RenderAccessibleHeaders(tablix, fixedHeader, numCols, cell.ColIndex, colSpan, cell.RowIndex, cell, omittedCells, rowHeaderIds, colHeaderIds, omittedHeaders, ref rowHeaderId);
      if (rowHeaderId != null)
        this.RenderReportItemId(rowHeaderId);
      int rowSpan = cell.RowSpan;
      if (cell.RowSpan > 1)
      {
        this.WriteStream(HTML4Renderer.m_rowSpan);
        this.WriteStream(cell.RowSpan.ToString((IFormatProvider) CultureInfo.InvariantCulture));
        this.WriteStream(HTML4Renderer.m_quote);
        this.WriteStream(HTML4Renderer.m_inlineHeight);
        this.WriteStream(Utility.MmToPxAsString((double) tablix.GetRowHeight(cell.RowIndex, cell.RowSpan)));
        this.WriteStream(HTML4Renderer.m_quote);
      }
      if (colSpan > 1)
      {
        this.WriteStream(HTML4Renderer.m_colSpan);
        this.WriteStream(cell.ColSpan.ToString((IFormatProvider) CultureInfo.InvariantCulture));
        this.WriteStream(HTML4Renderer.m_quote);
      }
      RPLElement element = (RPLElement) cell.Element;
      if (element != null)
      {
        int borderContext = 0;
        this.RenderTablixReportItemStyle(tablix, tablixContext, cell, styleContext, col == 0, lastCol, row == 0, row + rowSpan == numRows, element, ref borderContext);
        this.RenderTablixOmittedHeaderCells(omittedCells, col, lastCol, ref omittedIndex);
        this.RenderTablixReportItem(tablix, tablixContext, cell, styleContext, col == 0, lastCol, row == 0, row + rowSpan == numRows, element, ref borderContext);
      }
      else
      {
        if (styleContext.ZeroWidth)
        {
          this.OpenStyle();
          this.WriteStream(HTML4Renderer.m_displayNone);
          this.CloseStyle(true);
        }
        this.WriteStream(HTML4Renderer.m_closeBracket);
        this.RenderTablixOmittedHeaderCells(omittedCells, col, lastCol, ref omittedIndex);
        this.WriteStream(HTML4Renderer.m_nbsp);
      }
      this.WriteStream(HTML4Renderer.m_closeTD);
      this.RenderZeroWidthTDsForTablix(startIndex, colSpan, tablix);
      styleContext.ZeroWidth = zeroWidth;
    }

    private void RenderTablixOmittedHeaderCells(
      List<RPLTablixMemberCell> omittedHeaders,
      int colIndex,
      bool lastCol,
      ref int omittedIndex)
    {
      if (omittedHeaders == null)
        return;
      while (omittedIndex < omittedHeaders.Count && (((RPLTablixCell) omittedHeaders[omittedIndex]).ColIndex == colIndex || lastCol && ((RPLTablixCell) omittedHeaders[omittedIndex]).ColIndex > colIndex))
      {
        RPLTablixMemberCell omittedHeader = omittedHeaders[omittedIndex];
        if (omittedHeader.GroupLabel != null)
          this.RenderNavigationId(omittedHeader.UniqueName);
        ++omittedIndex;
      }
    }

    private void RenderColumnHeaderTablixCell(
      RPLTablix tablix,
      string tablixID,
      int numCols,
      int col,
      int colSpan,
      int row,
      int tablixContext,
      RPLTablixCell cell,
      StyleContext styleContext,
      TablixFixedHeaderStorage headerStorage,
      List<RPLTablixOmittedRow> omittedRows,
      int[] omittedIndices)
    {
      bool lastCol = col + colSpan == numCols;
      bool zeroWidth = styleContext.ZeroWidth;
      float columnWidth = tablix.GetColumnWidth(col, colSpan);
      styleContext.ZeroWidth = (double) columnWidth == 0.0;
      int startIndex = this.RenderZeroWidthTDsForTablix(col, colSpan, tablix);
      colSpan = this.GetColSpanMinusZeroWidthColumns(col, colSpan, tablix);
      this.WriteStream(HTML4Renderer.m_openTD);
      int rowSpan = cell.RowSpan;
      string repItemId = (string) null;
      if (cell is RPLTablixMemberCell && (((RPLTablixMemberCell) cell).GroupLabel != null || this.m_deviceInfo.AccessibleTablix))
      {
        repItemId = ((RPLTablixMemberCell) cell).UniqueName;
        if (repItemId == null && cell.Element != null && ((RPLElement) cell.Element).ElementProps != null)
        {
          repItemId = ((RPLElement) cell.Element).ElementProps.UniqueName;
          ((RPLTablixMemberCell) cell).UniqueName = repItemId;
        }
        if (repItemId != null)
          this.RenderReportItemId(repItemId);
      }
      if (tablix.FixedColumns[col])
      {
        if (repItemId == null)
        {
          repItemId = tablixID + "r" + (object) row + "c" + (object) col;
          this.RenderReportItemId(repItemId);
        }
        headerStorage.RowHeaders.Add(repItemId);
        if (headerStorage.CornerHeaders != null)
          headerStorage.CornerHeaders.Add(repItemId);
      }
      if (rowSpan > 1)
      {
        this.WriteStream(HTML4Renderer.m_rowSpan);
        this.WriteStream(cell.RowSpan.ToString((IFormatProvider) CultureInfo.InvariantCulture));
        this.WriteStream(HTML4Renderer.m_quote);
        this.WriteStream(HTML4Renderer.m_inlineHeight);
        this.WriteStream(Utility.MmToPxAsString((double) tablix.GetRowHeight(cell.RowIndex, cell.RowSpan)));
        this.WriteStream(HTML4Renderer.m_quote);
      }
      if (colSpan > 1)
      {
        this.WriteStream(HTML4Renderer.m_colSpan);
        this.WriteStream(cell.ColSpan.ToString((IFormatProvider) CultureInfo.InvariantCulture));
        this.WriteStream(HTML4Renderer.m_quote);
      }
      RPLElement element = (RPLElement) cell.Element;
      if (element != null)
      {
        int borderContext = 0;
        this.RenderTablixReportItemStyle(tablix, tablixContext, cell, styleContext, col == 0, lastCol, row == 0, false, element, ref borderContext);
        for (int index = 0; index < omittedRows.Count; ++index)
          this.RenderTablixOmittedHeaderCells(((RPLTablixRow) omittedRows[index]).OmittedHeaders, col, lastCol, ref omittedIndices[index]);
        this.RenderTablixReportItem(tablix, tablixContext, cell, styleContext, col == 0, lastCol, row == 0, false, element, ref borderContext);
      }
      else
      {
        if (styleContext.ZeroWidth)
        {
          this.OpenStyle();
          this.WriteStream(HTML4Renderer.m_displayNone);
          this.CloseStyle(true);
        }
        this.WriteStream(HTML4Renderer.m_closeBracket);
        for (int index = 0; index < omittedRows.Count; ++index)
          this.RenderTablixOmittedHeaderCells(((RPLTablixRow) omittedRows[index]).OmittedHeaders, col, lastCol, ref omittedIndices[index]);
        this.WriteStream(HTML4Renderer.m_nbsp);
      }
      this.WriteStream(HTML4Renderer.m_closeTD);
      this.RenderZeroWidthTDsForTablix(startIndex, colSpan, tablix);
      styleContext.ZeroWidth = zeroWidth;
    }

    protected void CreateGrowRectIdsStream()
    {
      this.m_growRectangleIdsStream = (Stream) new BufferedStream(this.CreateStream(HTML4Renderer.GetStreamName(this.m_rplReport.ReportName, this.m_pageNum, "_gr"), "txt", Encoding.UTF8, "text/plain", true, StreamOper.CreateOnly));
      this.m_needsGrowRectangleScript = true;
    }

    protected void CreateFitVertTextIdsStream()
    {
      this.m_fitVertTextIdsStream = (Stream) new BufferedStream(this.CreateStream(HTML4Renderer.GetStreamName(this.m_rplReport.ReportName, this.m_pageNum, "_fvt"), "txt", Encoding.UTF8, "text/plain", true, StreamOper.CreateOnly));
      this.m_needsFitVertTextScript = true;
    }

    protected void CreateImgConImageIdsStream()
    {
      this.m_imgConImageIdsStream = (Stream) new BufferedStream(this.CreateStream(HTML4Renderer.GetStreamName(this.m_rplReport.ReportName, this.m_pageNum, "_ici"), "txt", Encoding.UTF8, "text/plain", true, StreamOper.CreateOnly));
    }

    protected void CreateImgFitDivImageIdsStream()
    {
      this.m_imgFitDivIdsStream = (Stream) new BufferedStream(this.CreateStream(HTML4Renderer.GetStreamName(this.m_rplReport.ReportName, this.m_pageNum, "_ifd"), "txt", Encoding.UTF8, "text/plain", true, StreamOper.CreateOnly));
      this.m_emitImageConsolidationScaling = true;
    }

    [SecurityTreatAsSafe]
    [SecurityCritical]
    protected Stream CreateStream(
      string name,
      string extension,
      Encoding encoding,
      string mimeType,
      bool willSeek,
      StreamOper operation)
    {
      return this.m_createAndRegisterStreamCallback(name, extension, encoding, mimeType, willSeek, operation);
    }

    protected void RenderSecondaryStreamIdsSpanTag(Stream secondaryStream, string tagId)
    {
      if (secondaryStream == null || !secondaryStream.CanSeek)
        return;
      this.WriteStream(HTML4Renderer.m_openSpan);
      this.RenderReportItemId(tagId);
      this.WriteStream(" ids=\"");
      secondaryStream.Seek(0L, SeekOrigin.Begin);
      byte[] buffer = new byte[4096];
      int count;
      while ((count = secondaryStream.Read(buffer, 0, buffer.Length)) > 0)
        this.m_mainStream.Write(buffer, 0, count);
      this.WriteStream("\"");
      this.WriteStream(HTML4Renderer.m_closeBracket);
      this.WriteStreamCR(HTML4Renderer.m_closeSpan);
    }

    protected void RenderSecondaryStreamSpanTagsForJavascriptFunctions()
    {
      this.RenderSecondaryStreamIdsSpanTag(this.m_growRectangleIdsStream, "growRectangleIdsTag");
      this.RenderSecondaryStreamIdsSpanTag(this.m_fitVertTextIdsStream, "fitVertTextIdsTag");
      this.RenderSecondaryStreamIdsSpanTag(this.m_imgFitDivIdsStream, "imgFitDivIdsTag");
      this.RenderSecondaryStreamIdsSpanTag(this.m_imgConImageIdsStream, "imgConImageIdsTag");
    }

    internal enum RequestType
    {
      Render,
      Search,
      Bookmark,
    }

    internal enum Border
    {
      All,
      Left,
      Top,
      Right,
      Bottom,
    }

    internal enum BorderAttribute
    {
      BorderWidth,
      BorderStyle,
      BorderColor,
    }

    internal enum Direction
    {
      Row,
      Column,
    }

    internal enum PageSection
    {
      Body,
      PageHeader,
      PageFooter,
    }

    internal enum FontAttributes
    {
      None,
      Partial,
      All,
    }
  }
}
