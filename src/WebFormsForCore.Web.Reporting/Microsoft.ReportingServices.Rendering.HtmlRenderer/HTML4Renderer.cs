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
using System.Xml;

namespace Microsoft.ReportingServices.Rendering.HtmlRenderer;

internal abstract class HTML4Renderer : HTMLWriter
{
	internal enum RequestType
	{
		Render,
		Search,
		Bookmark
	}

	internal enum Border
	{
		All,
		Left,
		Top,
		Right,
		Bottom
	}

	internal enum BorderAttribute
	{
		BorderWidth,
		BorderStyle,
		BorderColor
	}

	internal enum Direction
	{
		Row,
		Column
	}

	internal enum PageSection
	{
		Body,
		PageHeader,
		PageFooter
	}

	internal enum FontAttributes
	{
		None,
		Partial,
		All
	}

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

	private const long FitProptionalDefaultSize = 5L;

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

	internal static byte[] m_overflowXHidden;

	internal static byte[] m_percentWidthOverflow;

	internal static byte[] m_layoutFixed;

	internal static byte[] m_layoutBorder;

	internal static byte[] m_ignoreBorder;

	internal static byte[] m_ignoreBorderL;

	internal static byte[] m_ignoreBorderR;

	internal static byte[] m_ignoreBorderT;

	internal static byte[] m_ignoreBorderB;

	internal static byte[] m_percentHeight;

	internal static byte[] m_percentSizesOverflow;

	internal static byte[] m_percentSizes;

	internal static byte[] m_space;

	internal static byte[] m_closeBracket;

	internal static byte[] m_semiColon;

	internal static byte[] m_border;

	internal static byte[] m_borderBottom;

	internal static byte[] m_borderLeft;

	internal static byte[] m_borderRight;

	internal static byte[] m_borderTop;

	internal static byte[] m_marginBottom;

	internal static byte[] m_marginLeft;

	internal static byte[] m_marginRight;

	internal static byte[] m_marginTop;

	internal static byte[] m_textIndent;

	internal static byte[] m_mm;

	internal static byte[] m_styleWidth;

	internal static byte[] m_styleHeight;

	internal static byte[] m_percent;

	internal static byte[] m_ninetyninepercent;

	internal static byte[] m_degree90;

	internal static byte[] m_newLine;

	internal static byte[] m_closeAccol;

	internal static byte[] m_backgroundRepeat;

	internal static byte[] m_closeBrace;

	internal static byte[] m_backgroundColor;

	internal static byte[] m_backgroundImage;

	internal static byte[] m_overflowHidden;

	internal static byte[] m_wordWrap;

	internal static byte[] m_whiteSpacePreWrap;

	internal static byte[] m_leftValue;

	internal static byte[] m_rightValue;

	internal static byte[] m_centerValue;

	internal static byte[] m_textAlign;

	internal static byte[] m_verticalAlign;

	internal static byte[] m_lineHeight;

	internal static byte[] m_color;

	internal static byte[] m_writingMode;

	internal static byte[] m_tbrl;

	internal static byte[] m_btrl;

	internal static byte[] m_lrtb;

	internal static byte[] m_rltb;

	internal static byte[] m_layoutFlow;

	internal static byte[] m_verticalIdeographic;

	internal static byte[] m_horizontal;

	internal static byte[] m_unicodeBiDi;

	internal static byte[] m_direction;

	internal static byte[] m_textDecoration;

	internal static byte[] m_fontWeight;

	internal static byte[] m_fontSize;

	internal static byte[] m_fontFamily;

	internal static byte[] m_fontStyle;

	internal static byte[] m_openAccol;

	internal static byte[] m_borderColor;

	internal static byte[] m_borderStyle;

	internal static byte[] m_borderWidth;

	internal static byte[] m_borderBottomColor;

	internal static byte[] m_borderBottomStyle;

	internal static byte[] m_borderBottomWidth;

	internal static byte[] m_borderLeftColor;

	internal static byte[] m_borderLeftStyle;

	internal static byte[] m_borderLeftWidth;

	internal static byte[] m_borderRightColor;

	internal static byte[] m_borderRightStyle;

	internal static byte[] m_borderRightWidth;

	internal static byte[] m_borderTopColor;

	internal static byte[] m_borderTopStyle;

	internal static byte[] m_borderTopWidth;

	internal static byte[] m_paddingBottom;

	internal static byte[] m_paddingLeft;

	internal static byte[] m_paddingRight;

	internal static byte[] m_paddingTop;

	protected static byte[] m_classAction;

	internal static byte[] m_styleAction;

	internal static byte[] m_emptyTextBox;

	internal static byte[] m_percentSizeInlineTable;

	internal static byte[] m_classPercentSizeInlineTable;

	internal static byte[] m_percentHeightInlineTable;

	internal static byte[] m_classPercentHeightInlineTable;

	internal static byte[] m_dot;

	internal static byte[] m_popupAction;

	internal static byte[] m_tableLayoutFixed;

	internal static byte[] m_borderCollapse;

	internal static byte[] m_none;

	internal static byte[] m_displayNone;

	internal static byte[] m_rtlEmbed;

	internal static byte[] m_classRtlEmbed;

	internal static byte[] m_noVerticalMarginClassName;

	internal static byte[] m_classNoVerticalMargin;

	internal static byte[] m_zeroPoint;

	internal static byte[] m_smallPoint;

	internal static byte[] m_filter;

	internal static byte[] m_basicImageRotation180;

	internal static byte[] m_msoRotation;

	internal static byte[] m_styleMinWidth;

	internal static byte[] m_styleMinHeight;

	private static byte[] m_styleDisplayInlineBlock;

	internal static byte[] m_closeUL;

	internal static byte[] m_closeOL;

	internal static byte[] m_olArabic;

	internal static byte[] m_olRoman;

	internal static byte[] m_olAlpha;

	internal static byte[] m_ulCircle;

	internal static byte[] m_ulDisc;

	internal static byte[] m_ulSquare;

	protected static byte[] m_br;

	protected static byte[] m_tabIndex;

	protected static byte[] m_closeTable;

	protected static byte[] m_openTable;

	protected static byte[] m_closeDiv;

	protected static byte[] m_openDiv;

	protected static byte[] m_zeroBorder;

	protected static byte[] m_cols;

	protected static byte[] m_colSpan;

	protected static byte[] m_rowSpan;

	protected static byte[] m_headers;

	protected static byte[] m_closeTD;

	protected static byte[] m_closeTR;

	protected static byte[] m_firstTD;

	protected static byte[] m_lastTD;

	protected static byte[] m_openTD;

	protected static byte[] m_openTR;

	protected static byte[] m_valign;

	protected static byte[] m_closeQuote;

	internal static string m_closeQuoteString;

	protected static byte[] m_closeSpan;

	protected static byte[] m_openSpan;

	protected static byte[] m_quote;

	internal static string m_quoteString;

	protected static byte[] m_closeTag;

	protected static byte[] m_id;

	protected static byte[] m_px;

	protected static byte[] m_zeroWidth;

	protected static byte[] m_zeroHeight;

	protected static byte[] m_openHtml;

	protected static byte[] m_closeHtml;

	protected static byte[] m_openBody;

	protected static byte[] m_closeBody;

	protected static byte[] m_openHead;

	protected static byte[] m_closeHead;

	protected static byte[] m_openTitle;

	protected static byte[] m_closeTitle;

	protected static byte[] m_openA;

	protected static byte[] m_target;

	protected static byte[] m_closeA;

	protected static string m_hrefString;

	protected static byte[] m_href;

	protected static byte[] m_nohref;

	protected static byte[] m_inlineHeight;

	protected static byte[] m_inlineWidth;

	protected static byte[] m_img;

	protected static byte[] m_imgOnError;

	protected static byte[] m_src;

	protected static byte[] m_topValue;

	protected static byte[] m_alt;

	protected static byte[] m_title;

	protected static byte[] m_classID;

	protected static byte[] m_codeBase;

	protected static byte[] m_valueObject;

	protected static byte[] m_paramObject;

	protected static byte[] m_openObject;

	protected static byte[] m_closeObject;

	protected static byte[] m_equal;

	protected static byte[] m_encodedAmp;

	protected static byte[] m_nbsp;

	protected static byte[] m_questionMark;

	protected static byte[] m_checked;

	protected static byte[] m_checkForEnterKey;

	protected static byte[] m_unchecked;

	protected static byte[] m_showHideOnClick;

	protected static byte[] m_cursorHand;

	protected static byte[] m_rtlDir;

	protected static byte[] m_ltrDir;

	protected static byte[] m_classStyle;

	protected static byte[] m_openStyle;

	protected static byte[] m_underscore;

	protected static byte[] m_lineBreak;

	protected static byte[] m_ssClassID;

	protected static byte[] m_ptClassID;

	protected static byte[] m_xmlData;

	protected static byte[] m_useMap;

	protected static byte[] m_openMap;

	protected static byte[] m_closeMap;

	protected static byte[] m_mapArea;

	protected static byte[] m_mapCoords;

	protected static byte[] m_mapShape;

	protected static byte[] m_name;

	protected static byte[] m_circleShape;

	protected static byte[] m_polyShape;

	protected static byte[] m_rectShape;

	protected static byte[] m_comma;

	private static string m_mapPrefixString;

	protected static byte[] m_mapPrefix;

	protected static byte[] m_classPopupAction;

	protected static byte[] m_closeLi;

	protected static byte[] m_openLi;

	protected static byte[] m_firstNonHeaderPostfix;

	protected static byte[] m_fixedMatrixCornerPostfix;

	protected static byte[] m_fixedRowGroupingHeaderPostfix;

	protected static byte[] m_fixedColumnGroupingHeaderPostfix;

	protected static byte[] m_fixedRowHeaderPostfix;

	protected static byte[] m_fixedColumnHeaderPostfix;

	protected static byte[] m_fixedTableCornerPostfix;

	internal static byte[] m_language;

	private static byte[] m_zeroBorderWidth;

	internal static byte[] m_onLoadFitProportionalPv;

	private static byte[] m_normalWordWrap;

	private static byte[] m_classPercentSizes;

	private static byte[] m_classPercentSizesOverflow;

	private static byte[] m_classPercentWidthOverflow;

	private static byte[] m_classPercentHeight;

	private static byte[] m_classLayoutBorder;

	private static byte[] m_classLayoutFixed;

	private static byte[] m_strokeColor;

	private static byte[] m_strokeWeight;

	private static byte[] m_slineStyle;

	private static byte[] m_dashStyle;

	private static byte[] m_closeVGroup;

	private static byte[] m_openVGroup;

	private static byte[] m_openVLine;

	private static byte[] m_leftSlant;

	private static byte[] m_rightSlant;

	private static byte[] m_pageBreakDelimiter;

	private static byte[] m_nogrowAttribute;

	private static byte[] m_stylePositionAbsolute;

	private static byte[] m_stylePositionRelative;

	private static byte[] m_styleClipRectOpenBrace;

	private static byte[] m_styleTop;

	private static byte[] m_styleLeft;

	private static byte[] m_pxSpace;

	internal static char[] m_cssDelimiters;

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

	protected RequestType m_requestType;

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

	private SecondaryStreams m_createSecondaryStreams = (SecondaryStreams)1;

	protected int m_tabIndexNum;

	protected int m_currentHitCount;

	protected Hashtable m_duplicateItems;

	protected string m_searchText;

	protected bool m_emitImageConsolidationScaling;

	protected bool m_needsCanGrowFalseScript;

	protected bool m_needsGrowRectangleScript;

	protected bool m_needsFitVertTextScript;

	internal static string m_searchHitIdPrefix;

	internal static string m_standardLineBreak;

	protected Stack m_linkToChildStack;

	protected PageSection m_pageSection;

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

	internal string SearchText
	{
		set
		{
			m_searchText = value;
		}
	}

	internal bool NeedResizeImages => m_fitPropImages;

	protected bool IsFragment
	{
		get
		{
			if (m_htmlFragment)
			{
				return !m_deviceInfo.HasActionScript;
			}
			return false;
		}
	}

	internal bool IsBrowserIE => m_deviceInfo.IsBrowserIE;

	protected virtual bool FillPageHeight => m_deviceInfo.IsBrowserIE;

	static HTML4Renderer()
	{
		m_overflowXHidden = null;
		m_percentWidthOverflow = null;
		m_layoutFixed = null;
		m_layoutBorder = null;
		m_ignoreBorder = null;
		m_ignoreBorderL = null;
		m_ignoreBorderR = null;
		m_ignoreBorderT = null;
		m_ignoreBorderB = null;
		m_percentHeight = null;
		m_percentSizesOverflow = null;
		m_percentSizes = null;
		m_space = null;
		m_closeBracket = null;
		m_semiColon = null;
		m_border = null;
		m_borderBottom = null;
		m_borderLeft = null;
		m_borderRight = null;
		m_borderTop = null;
		m_marginBottom = null;
		m_marginLeft = null;
		m_marginRight = null;
		m_marginTop = null;
		m_textIndent = null;
		m_mm = null;
		m_styleWidth = null;
		m_styleHeight = null;
		m_percent = null;
		m_ninetyninepercent = null;
		m_degree90 = null;
		m_newLine = null;
		m_closeAccol = null;
		m_backgroundRepeat = null;
		m_closeBrace = null;
		m_backgroundColor = null;
		m_backgroundImage = null;
		m_overflowHidden = null;
		m_wordWrap = null;
		m_whiteSpacePreWrap = null;
		m_leftValue = null;
		m_rightValue = null;
		m_centerValue = null;
		m_textAlign = null;
		m_verticalAlign = null;
		m_lineHeight = null;
		m_color = null;
		m_writingMode = null;
		m_tbrl = null;
		m_btrl = null;
		m_lrtb = null;
		m_rltb = null;
		m_layoutFlow = null;
		m_verticalIdeographic = null;
		m_horizontal = null;
		m_unicodeBiDi = null;
		m_direction = null;
		m_textDecoration = null;
		m_fontWeight = null;
		m_fontSize = null;
		m_fontFamily = null;
		m_fontStyle = null;
		m_openAccol = null;
		m_borderColor = null;
		m_borderStyle = null;
		m_borderWidth = null;
		m_borderBottomColor = null;
		m_borderBottomStyle = null;
		m_borderBottomWidth = null;
		m_borderLeftColor = null;
		m_borderLeftStyle = null;
		m_borderLeftWidth = null;
		m_borderRightColor = null;
		m_borderRightStyle = null;
		m_borderRightWidth = null;
		m_borderTopColor = null;
		m_borderTopStyle = null;
		m_borderTopWidth = null;
		m_paddingBottom = null;
		m_paddingLeft = null;
		m_paddingRight = null;
		m_paddingTop = null;
		m_classAction = null;
		m_styleAction = null;
		m_emptyTextBox = null;
		m_percentSizeInlineTable = null;
		m_classPercentSizeInlineTable = null;
		m_percentHeightInlineTable = null;
		m_classPercentHeightInlineTable = null;
		m_dot = null;
		m_popupAction = null;
		m_tableLayoutFixed = null;
		m_borderCollapse = null;
		m_none = null;
		m_displayNone = null;
		m_rtlEmbed = null;
		m_classRtlEmbed = null;
		m_noVerticalMarginClassName = null;
		m_classNoVerticalMargin = null;
		m_zeroPoint = null;
		m_smallPoint = null;
		m_filter = null;
		m_basicImageRotation180 = null;
		m_msoRotation = null;
		m_styleMinWidth = null;
		m_styleMinHeight = null;
		m_styleDisplayInlineBlock = null;
		m_br = null;
		m_tabIndex = null;
		m_closeTable = null;
		m_openTable = null;
		m_closeDiv = null;
		m_openDiv = null;
		m_zeroBorder = null;
		m_cols = null;
		m_colSpan = null;
		m_rowSpan = null;
		m_headers = null;
		m_closeTD = null;
		m_closeTR = null;
		m_firstTD = null;
		m_lastTD = null;
		m_openTD = null;
		m_openTR = null;
		m_valign = null;
		m_closeQuote = null;
		m_closeQuoteString = "\">";
		m_closeSpan = null;
		m_openSpan = null;
		m_quote = null;
		m_quoteString = "\"";
		m_closeTag = null;
		m_id = null;
		m_px = null;
		m_zeroWidth = null;
		m_zeroHeight = null;
		m_openHtml = null;
		m_closeHtml = null;
		m_openBody = null;
		m_closeBody = null;
		m_openHead = null;
		m_closeHead = null;
		m_openTitle = null;
		m_closeTitle = null;
		m_openA = null;
		m_target = null;
		m_closeA = null;
		m_hrefString = " href=\"";
		m_href = null;
		m_nohref = null;
		m_inlineHeight = null;
		m_inlineWidth = null;
		m_img = null;
		m_imgOnError = null;
		m_src = null;
		m_topValue = null;
		m_alt = null;
		m_title = null;
		m_classID = null;
		m_codeBase = null;
		m_valueObject = null;
		m_paramObject = null;
		m_openObject = null;
		m_closeObject = null;
		m_equal = null;
		m_encodedAmp = null;
		m_nbsp = null;
		m_questionMark = null;
		m_checked = null;
		m_checkForEnterKey = null;
		m_unchecked = null;
		m_showHideOnClick = null;
		m_cursorHand = null;
		m_rtlDir = null;
		m_ltrDir = null;
		m_classStyle = null;
		m_openStyle = null;
		m_underscore = null;
		m_lineBreak = null;
		m_ssClassID = null;
		m_ptClassID = null;
		m_xmlData = null;
		m_useMap = null;
		m_openMap = null;
		m_closeMap = null;
		m_mapArea = null;
		m_mapCoords = null;
		m_mapShape = null;
		m_name = null;
		m_circleShape = null;
		m_polyShape = null;
		m_rectShape = null;
		m_comma = null;
		m_mapPrefixString = "Map";
		m_mapPrefix = null;
		m_classPopupAction = null;
		m_closeLi = null;
		m_openLi = null;
		m_firstNonHeaderPostfix = null;
		m_fixedMatrixCornerPostfix = null;
		m_fixedRowGroupingHeaderPostfix = null;
		m_fixedColumnGroupingHeaderPostfix = null;
		m_fixedRowHeaderPostfix = null;
		m_fixedColumnHeaderPostfix = null;
		m_fixedTableCornerPostfix = null;
		m_language = null;
		m_zeroBorderWidth = null;
		m_onLoadFitProportionalPv = null;
		m_normalWordWrap = null;
		m_classPercentSizes = null;
		m_classPercentSizesOverflow = null;
		m_classPercentWidthOverflow = null;
		m_classPercentHeight = null;
		m_classLayoutBorder = null;
		m_classLayoutFixed = null;
		m_strokeColor = null;
		m_strokeWeight = null;
		m_slineStyle = null;
		m_dashStyle = null;
		m_closeVGroup = null;
		m_openVGroup = null;
		m_openVLine = null;
		m_leftSlant = null;
		m_rightSlant = null;
		m_pageBreakDelimiter = null;
		m_nogrowAttribute = null;
		m_stylePositionAbsolute = null;
		m_stylePositionRelative = null;
		m_styleClipRectOpenBrace = null;
		m_styleTop = null;
		m_styleLeft = null;
		m_pxSpace = null;
		m_cssDelimiters = new char[13]
		{
			'[', ']', '"', '\'', '<', '>', '{', '}', '(', ')',
			'/', '%', ' '
		};
		m_searchHitIdPrefix = "oHit";
		m_standardLineBreak = "\n";
		UTF8Encoding uTF8Encoding = new UTF8Encoding();
		m_newLine = uTF8Encoding.GetBytes("\r\n");
		m_openTable = uTF8Encoding.GetBytes("<TABLE CELLSPACING=\"0\" CELLPADDING=\"0\"");
		m_zeroBorder = uTF8Encoding.GetBytes(" BORDER=\"0\"");
		m_zeroPoint = uTF8Encoding.GetBytes("0pt");
		m_smallPoint = uTF8Encoding.GetBytes("1px");
		m_cols = uTF8Encoding.GetBytes(" COLS=\"");
		m_colSpan = uTF8Encoding.GetBytes(" COLSPAN=\"");
		m_rowSpan = uTF8Encoding.GetBytes(" ROWSPAN=\"");
		m_headers = uTF8Encoding.GetBytes(" HEADERS=\"");
		m_space = uTF8Encoding.GetBytes(" ");
		m_closeBracket = uTF8Encoding.GetBytes(">");
		m_closeTable = uTF8Encoding.GetBytes("</TABLE>");
		m_openDiv = uTF8Encoding.GetBytes("<DIV");
		m_closeDiv = uTF8Encoding.GetBytes("</DIV>");
		m_openBody = uTF8Encoding.GetBytes("<body");
		m_closeBody = uTF8Encoding.GetBytes("</body>");
		m_openHtml = uTF8Encoding.GetBytes("<html>");
		m_closeHtml = uTF8Encoding.GetBytes("</html>");
		m_openHead = uTF8Encoding.GetBytes("<head>");
		m_closeHead = uTF8Encoding.GetBytes("</head>");
		m_openTitle = uTF8Encoding.GetBytes("<title>");
		m_closeTitle = uTF8Encoding.GetBytes("</title>");
		m_firstTD = uTF8Encoding.GetBytes("<TR><TD");
		m_lastTD = uTF8Encoding.GetBytes("</TD></TR>");
		m_openTD = uTF8Encoding.GetBytes("<TD");
		m_closeTD = uTF8Encoding.GetBytes("</TD>");
		m_closeTR = uTF8Encoding.GetBytes("</TR>");
		m_openTR = uTF8Encoding.GetBytes("<TR");
		m_valign = uTF8Encoding.GetBytes(" VALIGN=\"");
		m_openSpan = uTF8Encoding.GetBytes("<span");
		m_closeSpan = uTF8Encoding.GetBytes("</span>");
		m_quote = uTF8Encoding.GetBytes(m_quoteString);
		m_closeQuote = uTF8Encoding.GetBytes(m_closeQuoteString);
		m_id = uTF8Encoding.GetBytes(" ID=\"");
		m_mm = uTF8Encoding.GetBytes("mm");
		m_px = uTF8Encoding.GetBytes("px");
		m_zeroWidth = uTF8Encoding.GetBytes(" WIDTH=\"0\"");
		m_zeroHeight = uTF8Encoding.GetBytes(" HEIGHT=\"0\"");
		m_closeTag = uTF8Encoding.GetBytes("\"/>");
		m_openA = uTF8Encoding.GetBytes("<a");
		m_target = uTF8Encoding.GetBytes(" TARGET=\"");
		m_closeA = uTF8Encoding.GetBytes("</a>");
		m_href = uTF8Encoding.GetBytes(m_hrefString);
		m_nohref = uTF8Encoding.GetBytes(" nohref=\"true\"");
		m_inlineHeight = uTF8Encoding.GetBytes(" HEIGHT=\"");
		m_inlineWidth = uTF8Encoding.GetBytes(" WIDTH=\"");
		m_img = uTF8Encoding.GetBytes("<IMG");
		m_imgOnError = uTF8Encoding.GetBytes(" onerror=\"this.errored=true;\"");
		m_src = uTF8Encoding.GetBytes(" SRC=\"");
		m_topValue = uTF8Encoding.GetBytes("top");
		m_leftValue = uTF8Encoding.GetBytes("left");
		m_rightValue = uTF8Encoding.GetBytes("right");
		m_centerValue = uTF8Encoding.GetBytes("center");
		m_classID = uTF8Encoding.GetBytes(" CLASSID=\"CLSID:");
		m_codeBase = uTF8Encoding.GetBytes(" CODEBASE=\"");
		m_title = uTF8Encoding.GetBytes(" TITLE=\"");
		m_alt = uTF8Encoding.GetBytes(" ALT=\"");
		m_openObject = uTF8Encoding.GetBytes("<OBJECT");
		m_closeObject = uTF8Encoding.GetBytes("</OBJECT>");
		m_paramObject = uTF8Encoding.GetBytes("<PARAM NAME=\"");
		m_valueObject = uTF8Encoding.GetBytes(" VALUE=\"");
		m_equal = uTF8Encoding.GetBytes("=");
		m_encodedAmp = uTF8Encoding.GetBytes("&amp;");
		m_nbsp = uTF8Encoding.GetBytes("&nbsp;");
		m_questionMark = uTF8Encoding.GetBytes("?");
		m_none = uTF8Encoding.GetBytes("none");
		m_displayNone = uTF8Encoding.GetBytes("display: none;");
		m_checkForEnterKey = uTF8Encoding.GetBytes("if(event.keyCode == 13 || event.which == 13){");
		m_percent = uTF8Encoding.GetBytes("100%");
		m_ninetyninepercent = uTF8Encoding.GetBytes("99%");
		m_degree90 = uTF8Encoding.GetBytes("90");
		m_lineBreak = uTF8Encoding.GetBytes(m_standardLineBreak);
		m_closeBrace = uTF8Encoding.GetBytes(")");
		m_rtlDir = uTF8Encoding.GetBytes(" dir=\"RTL\"");
		m_ltrDir = uTF8Encoding.GetBytes(" dir=\"LTR\"");
		m_br = uTF8Encoding.GetBytes("<br/>");
		m_tabIndex = uTF8Encoding.GetBytes(" tabindex=\"");
		m_useMap = uTF8Encoding.GetBytes(" USEMAP=\"");
		m_openMap = uTF8Encoding.GetBytes("<MAP ");
		m_closeMap = uTF8Encoding.GetBytes("</MAP>");
		m_mapArea = uTF8Encoding.GetBytes("<AREA ");
		m_mapCoords = uTF8Encoding.GetBytes(" COORDS=\"");
		m_mapShape = uTF8Encoding.GetBytes(" SHAPE=\"");
		m_name = uTF8Encoding.GetBytes(" NAME=\"");
		m_circleShape = uTF8Encoding.GetBytes("circle");
		m_polyShape = uTF8Encoding.GetBytes("poly");
		m_rectShape = uTF8Encoding.GetBytes("rect");
		m_comma = uTF8Encoding.GetBytes(",");
		m_mapPrefix = uTF8Encoding.GetBytes(m_mapPrefixString);
		m_openLi = uTF8Encoding.GetBytes("<li");
		m_closeLi = uTF8Encoding.GetBytes("</li>");
		m_firstNonHeaderPostfix = uTF8Encoding.GetBytes("_FNHR");
		m_fixedMatrixCornerPostfix = uTF8Encoding.GetBytes("_MCC");
		m_fixedRowGroupingHeaderPostfix = uTF8Encoding.GetBytes("_FRGH");
		m_fixedColumnGroupingHeaderPostfix = uTF8Encoding.GetBytes("_FCGH");
		m_fixedRowHeaderPostfix = uTF8Encoding.GetBytes("_FRH");
		m_fixedColumnHeaderPostfix = uTF8Encoding.GetBytes("_FCH");
		m_fixedTableCornerPostfix = uTF8Encoding.GetBytes("_FCC");
		m_dot = uTF8Encoding.GetBytes(".");
		m_percentSizes = uTF8Encoding.GetBytes("r1");
		m_percentSizesOverflow = uTF8Encoding.GetBytes("r2");
		m_percentHeight = uTF8Encoding.GetBytes("r3");
		m_ignoreBorder = uTF8Encoding.GetBytes("r4");
		m_ignoreBorderL = uTF8Encoding.GetBytes("r5");
		m_ignoreBorderR = uTF8Encoding.GetBytes("r6");
		m_ignoreBorderT = uTF8Encoding.GetBytes("r7");
		m_ignoreBorderB = uTF8Encoding.GetBytes("r8");
		m_layoutFixed = uTF8Encoding.GetBytes("r9");
		m_layoutBorder = uTF8Encoding.GetBytes("r10");
		m_percentWidthOverflow = uTF8Encoding.GetBytes("r11");
		m_popupAction = uTF8Encoding.GetBytes("r12");
		m_styleAction = uTF8Encoding.GetBytes("r13");
		m_emptyTextBox = uTF8Encoding.GetBytes("r14");
		m_classPercentSizes = uTF8Encoding.GetBytes(" class=\"r1\"");
		m_classPercentSizesOverflow = uTF8Encoding.GetBytes(" class=\"r2\"");
		m_classPercentHeight = uTF8Encoding.GetBytes(" class=\"r3\"");
		m_classLayoutFixed = uTF8Encoding.GetBytes(" class=\"r9");
		m_classLayoutBorder = uTF8Encoding.GetBytes(" class=\"r10");
		m_classPercentWidthOverflow = uTF8Encoding.GetBytes(" class=\"r11\"");
		m_classPopupAction = uTF8Encoding.GetBytes(" class=\"r12\"");
		m_classAction = uTF8Encoding.GetBytes(" class=\"r13\"");
		m_rtlEmbed = uTF8Encoding.GetBytes("r15");
		m_classRtlEmbed = uTF8Encoding.GetBytes(" class=\"r15\"");
		m_noVerticalMarginClassName = uTF8Encoding.GetBytes("r16");
		m_classNoVerticalMargin = uTF8Encoding.GetBytes(" class=\"r16\"");
		m_percentSizeInlineTable = uTF8Encoding.GetBytes("r17");
		m_classPercentSizeInlineTable = uTF8Encoding.GetBytes(" class=\"r17\"");
		m_percentHeightInlineTable = uTF8Encoding.GetBytes("r18");
		m_classPercentHeightInlineTable = uTF8Encoding.GetBytes(" class=\"r18\"");
		m_underscore = uTF8Encoding.GetBytes("_");
		m_openAccol = uTF8Encoding.GetBytes("{");
		m_closeAccol = uTF8Encoding.GetBytes("}");
		m_classStyle = uTF8Encoding.GetBytes(" class=\"");
		m_openStyle = uTF8Encoding.GetBytes(" style=\"");
		m_styleHeight = uTF8Encoding.GetBytes("HEIGHT:");
		m_styleMinHeight = uTF8Encoding.GetBytes("min-height:");
		m_styleWidth = uTF8Encoding.GetBytes("WIDTH:");
		m_styleMinWidth = uTF8Encoding.GetBytes("min-width:");
		m_zeroBorderWidth = uTF8Encoding.GetBytes("border-width:0px");
		m_border = uTF8Encoding.GetBytes("border:");
		m_borderLeft = uTF8Encoding.GetBytes("border-left:");
		m_borderTop = uTF8Encoding.GetBytes("border-top:");
		m_borderBottom = uTF8Encoding.GetBytes("border-bottom:");
		m_borderRight = uTF8Encoding.GetBytes("border-right:");
		m_borderColor = uTF8Encoding.GetBytes("border-color:");
		m_borderStyle = uTF8Encoding.GetBytes("border-style:");
		m_borderWidth = uTF8Encoding.GetBytes("border-width:");
		m_borderBottomColor = uTF8Encoding.GetBytes("border-bottom-color:");
		m_borderBottomStyle = uTF8Encoding.GetBytes("border-bottom-style:");
		m_borderBottomWidth = uTF8Encoding.GetBytes("border-bottom-width:");
		m_borderLeftColor = uTF8Encoding.GetBytes("border-left-color:");
		m_borderLeftStyle = uTF8Encoding.GetBytes("border-left-style:");
		m_borderLeftWidth = uTF8Encoding.GetBytes("border-left-width:");
		m_borderRightColor = uTF8Encoding.GetBytes("border-right-color:");
		m_borderRightStyle = uTF8Encoding.GetBytes("border-right-style:");
		m_borderRightWidth = uTF8Encoding.GetBytes("border-right-width:");
		m_borderTopColor = uTF8Encoding.GetBytes("border-top-color:");
		m_borderTopStyle = uTF8Encoding.GetBytes("border-top-style:");
		m_borderTopWidth = uTF8Encoding.GetBytes("border-top-width:");
		m_semiColon = uTF8Encoding.GetBytes(";");
		m_wordWrap = uTF8Encoding.GetBytes("word-wrap:break-word");
		m_whiteSpacePreWrap = uTF8Encoding.GetBytes("white-space:pre-wrap");
		m_normalWordWrap = uTF8Encoding.GetBytes("word-wrap:normal");
		m_overflowHidden = uTF8Encoding.GetBytes("overflow:hidden");
		m_overflowXHidden = uTF8Encoding.GetBytes("overflow-x:hidden");
		m_borderCollapse = uTF8Encoding.GetBytes("border-collapse:collapse");
		m_tableLayoutFixed = uTF8Encoding.GetBytes("table-layout:fixed");
		m_paddingLeft = uTF8Encoding.GetBytes("padding-left:");
		m_paddingRight = uTF8Encoding.GetBytes("padding-right:");
		m_paddingTop = uTF8Encoding.GetBytes("padding-top:");
		m_paddingBottom = uTF8Encoding.GetBytes("padding-bottom:");
		m_backgroundColor = uTF8Encoding.GetBytes("background-color:");
		m_backgroundImage = uTF8Encoding.GetBytes("background-image:url(");
		m_backgroundRepeat = uTF8Encoding.GetBytes("background-repeat:");
		m_fontStyle = uTF8Encoding.GetBytes("font-style:");
		m_fontFamily = uTF8Encoding.GetBytes("font-family:");
		m_fontSize = uTF8Encoding.GetBytes("font-size:");
		m_fontWeight = uTF8Encoding.GetBytes("font-weight:");
		m_textDecoration = uTF8Encoding.GetBytes("text-decoration:");
		m_textAlign = uTF8Encoding.GetBytes("text-align:");
		m_verticalAlign = uTF8Encoding.GetBytes("vertical-align:");
		m_color = uTF8Encoding.GetBytes("color:");
		m_lineHeight = uTF8Encoding.GetBytes("line-height:");
		m_direction = uTF8Encoding.GetBytes("direction:");
		m_unicodeBiDi = uTF8Encoding.GetBytes("unicode-bidi:");
		m_writingMode = uTF8Encoding.GetBytes("writing-mode:");
		m_msoRotation = uTF8Encoding.GetBytes("mso-rotate:");
		m_tbrl = uTF8Encoding.GetBytes("tb-rl");
		m_btrl = uTF8Encoding.GetBytes("bt-rl");
		m_lrtb = uTF8Encoding.GetBytes("lr-tb");
		m_rltb = uTF8Encoding.GetBytes("rl-tb");
		m_layoutFlow = uTF8Encoding.GetBytes("layout-flow:");
		m_verticalIdeographic = uTF8Encoding.GetBytes("vertical-ideographic");
		m_horizontal = uTF8Encoding.GetBytes("horizontal");
		m_cursorHand = uTF8Encoding.GetBytes("cursor:pointer");
		m_filter = uTF8Encoding.GetBytes("filter:");
		m_language = uTF8Encoding.GetBytes(" LANG=\"");
		m_marginLeft = uTF8Encoding.GetBytes("margin-left:");
		m_marginTop = uTF8Encoding.GetBytes("margin-top:");
		m_marginBottom = uTF8Encoding.GetBytes("margin-bottom:");
		m_marginRight = uTF8Encoding.GetBytes("margin-right:");
		m_textIndent = uTF8Encoding.GetBytes("text-indent:");
		m_onLoadFitProportionalPv = uTF8Encoding.GetBytes(" onload=\"this.fitproportional=true;this.pv=");
		m_basicImageRotation180 = uTF8Encoding.GetBytes("progid:DXImageTransform.Microsoft.BasicImage(rotation=2)");
		m_openVGroup = uTF8Encoding.GetBytes("<v:group coordsize=\"100,100\" coordorigin=\"0,0\"");
		m_openVLine = uTF8Encoding.GetBytes("<v:line from=\"0,");
		m_strokeColor = uTF8Encoding.GetBytes(" strokecolor=\"");
		m_strokeWeight = uTF8Encoding.GetBytes(" strokeWeight=\"");
		m_dashStyle = uTF8Encoding.GetBytes("<v:stroke dashstyle=\"");
		m_slineStyle = uTF8Encoding.GetBytes(" slineStyle=\"");
		m_closeVGroup = uTF8Encoding.GetBytes("</v:line></v:group>");
		m_rightSlant = uTF8Encoding.GetBytes("100\" to=\"100,0\"");
		m_leftSlant = uTF8Encoding.GetBytes("0\" to=\"100,100\"");
		m_pageBreakDelimiter = uTF8Encoding.GetBytes("<div style=\"page-break-after:always\"><hr/></div>");
		m_stylePositionAbsolute = uTF8Encoding.GetBytes("position:absolute;");
		m_stylePositionRelative = uTF8Encoding.GetBytes("position:relative;");
		m_styleClipRectOpenBrace = uTF8Encoding.GetBytes("clip:rect(");
		m_styleTop = uTF8Encoding.GetBytes("top:");
		m_styleLeft = uTF8Encoding.GetBytes("left:");
		m_pxSpace = uTF8Encoding.GetBytes("px ");
		m_closeUL = uTF8Encoding.GetBytes("</ul>");
		m_closeOL = uTF8Encoding.GetBytes("</ol>");
		m_olArabic = uTF8Encoding.GetBytes("<ol");
		m_olRoman = uTF8Encoding.GetBytes("<ol type=\"i\"");
		m_olAlpha = uTF8Encoding.GetBytes("<ol type=\"a\"");
		m_ulDisc = uTF8Encoding.GetBytes("<ul type=\"disc\"");
		m_ulSquare = uTF8Encoding.GetBytes("<ul type=\"square\"");
		m_ulCircle = uTF8Encoding.GetBytes("<ul type=\"circle\"");
		m_nogrowAttribute = uTF8Encoding.GetBytes(" nogrow=\"true\"");
		m_styleMinWidth = uTF8Encoding.GetBytes("min-width: ");
		m_styleMinHeight = uTF8Encoding.GetBytes("min-height: ");
		m_styleDisplayInlineBlock = uTF8Encoding.GetBytes("display: inline-block;");
	}

	public HTML4Renderer(IReportWrapper report, ISPBProcessing spbProcessing, NameValueCollection reportServerParams, DeviceInfo deviceInfo, NameValueCollection rawDeviceInfo, NameValueCollection browserCaps, CreateAndRegisterStream createAndRegisterStreamCallback, SecondaryStreams secondaryStreams)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Expected O, but got Unknown
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		SearchText = deviceInfo.FindString;
		m_report = report;
		m_spbProcessing = spbProcessing;
		m_createSecondaryStreams = secondaryStreams;
		m_usedStyles = new Hashtable();
		m_images = new Dictionary<string, string>();
		m_browserIE = deviceInfo.IsBrowserIE;
		m_deviceInfo = deviceInfo;
		m_rawDeviceInfo = rawDeviceInfo;
		m_serverParams = reportServerParams;
		m_createAndRegisterStreamCallback = createAndRegisterStreamCallback;
		m_htmlFragment = deviceInfo.HTMLFragment;
		m_onlyVisibleStyles = deviceInfo.OnlyVisibleStyles;
		m_pageNum = deviceInfo.Section;
		rawDeviceInfo.Remove("Section");
		rawDeviceInfo.Remove("FindString");
		rawDeviceInfo.Remove("BookmarkId");
		SPBContext val = new SPBContext();
		val.StartPage = m_pageNum;
		val.EndPage = m_pageNum;
		val.SecondaryStreams = m_createSecondaryStreams;
		val.AddSecondaryStreamNames = true;
		val.UseImageConsolidation = m_deviceInfo.ImageConsolidation;
		m_spbProcessing.SetContext(val);
		m_linkToChildStack = new Stack(1);
		m_stylePrefixIdBytes = Encoding.UTF8.GetBytes(m_deviceInfo.StylePrefixId);
		if (!m_deviceInfo.StyleStream)
		{
			m_useInlineStyle = m_htmlFragment;
		}
	}

	internal void InitializeReport()
	{
		m_rplReport = GetNextPage();
		if (m_rplReport == null)
		{
			throw new InvalidSectionException();
		}
		m_pageContent = m_rplReport.RPLPaginatedPages[0];
		m_rplReportSection = m_pageContent.GetNextReportSection();
		CheckBodyStyle();
		m_contextLanguage = m_rplReport.Language;
		m_expandItem = false;
	}

	protected static string GetStyleStreamName(string aReportName, int aPageNumber)
	{
		return GetStreamName(aReportName, aPageNumber, "style");
	}

	internal static string GetStreamName(string aReportName, int aPageNumber, string suffix)
	{
		if (aPageNumber > 0)
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}{1}{3}", aReportName, '_', suffix, aPageNumber);
		}
		return string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}", aReportName, '_', suffix);
	}

	internal static string HandleSpecialFontCharacters(string fontName)
	{
		if (fontName.IndexOfAny(m_cssDelimiters) != -1)
		{
			fontName = fontName.Trim();
			if (fontName.StartsWith("'", StringComparison.Ordinal))
			{
				fontName = fontName.Substring(1);
			}
			if (fontName.EndsWith("'", StringComparison.Ordinal))
			{
				fontName = fontName.Substring(0, fontName.Length - 1);
			}
			return "'" + fontName.Replace("'", "&quot;") + "'";
		}
		return fontName;
	}

	protected abstract void RenderSortAction(RPLTextBoxProps textBoxProps, SortOptions sortState);

	protected abstract void RenderInternalImageSrc();

	protected abstract void RenderToggleImage(RPLTextBoxProps textBoxProps);

	public abstract void Render(HtmlTextWriter outputWriter);

	internal void RenderStylesOnly(string streamName)
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Expected O, but got Unknown
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Expected O, but got Unknown
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Expected O, but got Unknown
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Expected O, but got Unknown
		m_encoding = Encoding.UTF8;
		Stream stream = CreateStream(streamName, "css", Encoding.UTF8, "text/css", willSeek: false, StreamOper.CreateAndRegister);
		StyleContext styleContext = new StyleContext();
		int num = 0;
		for (m_styleStream = new BufferedStream(stream); m_rplReportSection != null; m_rplReportSection = m_pageContent.GetNextReportSection())
		{
			num = 0;
			RPLItemMeasurement header = m_rplReportSection.Header;
			if (header != null)
			{
				RPLHeaderFooter val = (RPLHeaderFooter)header.Element;
				RPLElementProps elementProps = ((RPLElement)val).ElementProps;
				RPLElementPropsDef definition = elementProps.Definition;
				styleContext.StyleOnCell = true;
				RenderSharedStyle((RPLElement)(object)val, elementProps, definition, definition.SharedStyle, header, definition.ID + "c", styleContext, ref num);
				styleContext.StyleOnCell = false;
				RenderSharedStyle((RPLElement)(object)val, elementProps, definition, definition.SharedStyle, header, definition.ID, styleContext, ref num);
				RPLItemMeasurement[] children = ((RPLContainer)val).Children;
				if (children != null)
				{
					for (int i = 0; i < children.Length; i++)
					{
						RenderStylesOnlyRecursive(children[i], new StyleContext());
					}
				}
				header.Element = null;
			}
			RPLItemMeasurement footer = m_rplReportSection.Footer;
			if (footer != null)
			{
				RPLHeaderFooter val2 = (RPLHeaderFooter)footer.Element;
				RPLElementProps elementProps2 = ((RPLElement)val2).ElementProps;
				RPLElementPropsDef definition2 = elementProps2.Definition;
				styleContext.StyleOnCell = true;
				RenderSharedStyle((RPLElement)(object)val2, elementProps2, definition2, definition2.SharedStyle, footer, definition2.ID + "c", styleContext, ref num);
				styleContext.StyleOnCell = false;
				RenderSharedStyle((RPLElement)(object)val2, elementProps2, definition2, definition2.SharedStyle, footer, definition2.ID, styleContext, ref num);
				RPLItemMeasurement[] children2 = ((RPLContainer)val2).Children;
				if (children2 != null)
				{
					for (int j = 0; j < children2.Length; j++)
					{
						RenderStylesOnlyRecursive(children2[j], new StyleContext());
					}
				}
				footer.Element = null;
			}
			RPLItemMeasurement val3 = new RPLItemMeasurement();
			((RPLSizes)val3).Width = m_pageContent.MaxSectionWidth;
			((RPLSizes)val3).Height = m_rplReportSection.BodyArea.Height;
			RPLItemMeasurement val4 = m_rplReportSection.Columns[0];
			RPLBody val5 = (RPLBody)m_rplReportSection.Columns[0].Element;
			RPLElementProps elementProps3 = ((RPLElement)val5).ElementProps;
			RPLElementPropsDef definition3 = elementProps3.Definition;
			RenderSharedStyle((RPLElement)(object)val5, elementProps3, definition3, definition3.SharedStyle, val3, definition3.ID, styleContext, ref num);
			RPLItemMeasurement[] children3 = ((RPLContainer)val5).Children;
			if (children3 != null && children3.Length > 0)
			{
				for (int k = 0; k < children3.Length; k++)
				{
					RenderStylesOnlyRecursive(children3[k], new StyleContext());
				}
			}
			val4.Element = null;
		}
		m_styleStream.Flush();
	}

	internal void RenderStylesOnlyRecursive(RPLItemMeasurement measurement, StyleContext styleContext)
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Expected O, but got Unknown
		//IL_03aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0450: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04aa: Expected O, but got Unknown
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0675: Unknown result type (might be due to invalid IL or missing references)
		//IL_067c: Expected O, but got Unknown
		//IL_05b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bf: Expected O, but got Unknown
		//IL_070c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0713: Expected O, but got Unknown
		//IL_0719: Unknown result type (might be due to invalid IL or missing references)
		int borderContext = 0;
		RPLElement element = (RPLElement)(object)measurement.Element;
		RPLElementProps elementProps = element.ElementProps;
		RPLElementPropsDef definition = elementProps.Definition;
		RPLStyleProps sharedStyle = definition.SharedStyle;
		string iD = definition.ID;
		object obj = elementProps.Style[(byte)26];
		if (element is RPLTextBox)
		{
			RPLTextBoxPropsDef val = (RPLTextBoxPropsDef)definition;
			bool ignoreVerticalAlign = styleContext.IgnoreVerticalAlign;
			if (val.CanSort && !m_usedStyles.ContainsKey(iD + "p"))
			{
				if (val.CanGrow || val.CanShrink)
				{
					styleContext.StyleOnCell = true;
				}
				if (!val.CanGrow && val.CanShrink)
				{
					styleContext.IgnoreVerticalAlign = true;
				}
				RenderSharedStyle(element, elementProps, definition, sharedStyle, measurement, iD + "p", styleContext, ref borderContext);
				styleContext.StyleOnCell = false;
			}
			if (!m_deviceInfo.IsBrowserIE || m_deviceInfo.BrowserMode == BrowserMode.Standards || m_deviceInfo.OutlookCompat || (obj != null && (int)(VerticalAlignments)obj != 0))
			{
				styleContext.IgnoreVerticalAlign = ignoreVerticalAlign;
			}
			if (val.CanShrink && !m_usedStyles.ContainsKey(iD + "s"))
			{
				styleContext.NoBorders = true;
				RenderSharedStyle(element, elementProps, definition, sharedStyle, measurement, iD + "s", styleContext, ref borderContext);
				if (!val.CanGrow)
				{
					styleContext.IgnoreVerticalAlign = true;
				}
			}
			if (val.CanSort && !val.IsSimple && !IsFragment && val.IsToggleParent)
			{
				styleContext.IgnoreVerticalAlign = ignoreVerticalAlign;
			}
			styleContext.RenderMeasurements = false;
			if (!m_usedStyles.ContainsKey(iD))
			{
				int borderContext2 = borderContext;
				RenderSharedStyle(element, elementProps, definition, sharedStyle, measurement, iD, styleContext, ref borderContext2);
				styleContext.IgnoreVerticalAlign = ignoreVerticalAlign;
				borderContext2 = borderContext;
				RenderSharedStyle(element, elementProps, definition, sharedStyle, measurement, iD + "l", styleContext, ref borderContext2);
				RenderSharedStyle(element, elementProps, definition, sharedStyle, measurement, iD + "r", styleContext, ref borderContext);
			}
			RPLTextBoxProps val2 = (RPLTextBoxProps)(object)((elementProps is RPLTextBoxProps) ? elementProps : null);
			if (!m_usedStyles.ContainsKey(iD + "a") && HasAction(val2.ActionInfo))
			{
				TextRunStyleWriter textRunStyleWriter = new TextRunStyleWriter(this);
				RenderSharedStyle(textRunStyleWriter, definition.SharedStyle, styleContext, iD + "a");
				textRunStyleWriter.WriteStyles(StyleWriterMode.Shared, (IRPLStyle)(object)definition.SharedStyle);
			}
			if (val.IsSimple)
			{
				return;
			}
			RPLTextBox val3 = (RPLTextBox)(object)((element is RPLTextBox) ? element : null);
			ParagraphStyleWriter paragraphStyleWriter = new ParagraphStyleWriter(this, val3);
			TextRunStyleWriter styleWriter = new TextRunStyleWriter(this);
			for (RPLParagraph nextParagraph = val3.GetNextParagraph(); nextParagraph != null; nextParagraph = val3.GetNextParagraph())
			{
				paragraphStyleWriter.Paragraph = nextParagraph;
				string iD2 = ((RPLElement)nextParagraph).ElementProps.Definition.ID;
				paragraphStyleWriter.ParagraphMode = ParagraphStyleWriter.Mode.All;
				RenderSharedStyle(paragraphStyleWriter, ((RPLElement)nextParagraph).ElementProps.Definition.SharedStyle, styleContext, iD2);
				paragraphStyleWriter.ParagraphMode = ParagraphStyleWriter.Mode.ListOnly;
				RenderSharedStyle(paragraphStyleWriter, ((RPLElement)nextParagraph).ElementProps.Definition.SharedStyle, styleContext, iD2 + "l");
				paragraphStyleWriter.ParagraphMode = ParagraphStyleWriter.Mode.ParagraphOnly;
				RenderSharedStyle(paragraphStyleWriter, ((RPLElement)nextParagraph).ElementProps.Definition.SharedStyle, styleContext, iD2 + "p");
				for (RPLTextRun nextTextRun = nextParagraph.GetNextTextRun(); nextTextRun != null; nextTextRun = nextParagraph.GetNextTextRun())
				{
					RenderSharedStyle(styleWriter, ((RPLElement)nextTextRun).ElementProps.Definition.SharedStyle, styleContext, ((RPLElement)nextTextRun).ElementProps.Definition.ID);
				}
			}
			return;
		}
		if (!m_usedStyles.ContainsKey(iD))
		{
			RenderSharedStyle(element, elementProps, definition, sharedStyle, measurement, iD, styleContext, ref borderContext);
		}
		if (element is RPLSubReport)
		{
			RPLItemMeasurement[] children = ((RPLContainer)(RPLSubReport)element).Children;
			if (children != null)
			{
				for (int i = 0; i < children.Length; i++)
				{
					RPLItem element2 = children[i].Element;
					RPLContainer val4 = (RPLContainer)(object)((element2 is RPLContainer) ? element2 : null);
					if (val4 != null && val4.Children != null && val4.Children.Length > 0)
					{
						for (int j = 0; j < val4.Children.Length; j++)
						{
							RenderStylesOnlyRecursive(val4.Children[j], styleContext);
							val4.Children[j] = null;
						}
					}
					children[i] = null;
				}
				measurement.Element = null;
			}
		}
		else if (element is RPLContainer)
		{
			styleContext.InTablix = false;
			RPLItemMeasurement[] children2 = ((RPLContainer)element).Children;
			if (children2 != null && children2.Length > 0)
			{
				for (int k = 0; k < children2.Length; k++)
				{
					RenderStylesOnlyRecursive(children2[k], styleContext);
					children2[k] = null;
				}
			}
		}
		else if (element is RPLTablix)
		{
			RPLTablix val5 = (RPLTablix)element;
			RPLTablixRow nextRow = val5.GetNextRow();
			bool inTablix = styleContext.InTablix;
			while (nextRow != null)
			{
				for (int l = 0; l < nextRow.NumCells; l++)
				{
					RPLTablixCell val6 = nextRow[l];
					RPLElement element3 = (RPLElement)(object)val6.Element;
					RPLElementProps elementProps2 = element3.ElementProps;
					RPLElementPropsDef definition2 = elementProps2.Definition;
					RPLStyleProps sharedStyle2 = definition2.SharedStyle;
					bool zeroWidth = styleContext.ZeroWidth;
					float columnWidth = val5.GetColumnWidth(val6.ColIndex, val6.ColSpan);
					styleContext.ZeroWidth = columnWidth == 0f;
					if (element3 == null)
					{
						continue;
					}
					string iD3 = definition2.ID;
					if (!(element3 is RPLLine) && !m_usedStyles.ContainsKey(iD3 + "c"))
					{
						styleContext.StyleOnCell = true;
						borderContext = GetNewContext(borderContext, val6.ColIndex == 0, val6.ColIndex + val6.ColSpan == val5.ColumnWidths.Length, val6.RowIndex == 0, val6.RowIndex + val6.RowSpan == val5.RowHeights.Length);
						int borderContext3 = borderContext;
						RPLTextBox val7 = (RPLTextBox)element3;
						bool onlyRenderMeasurementsBackgroundBorders = styleContext.OnlyRenderMeasurementsBackgroundBorders;
						if (val7 != null && IsWritingModeVertical((IRPLStyle)(object)sharedStyle2) && m_deviceInfo.IsBrowserIE && m_deviceInfo.BrowserMode == BrowserMode.Standards)
						{
							styleContext.OnlyRenderMeasurementsBackgroundBorders = true;
						}
						RenderSharedStyle(element3, elementProps2, definition2, sharedStyle2, null, iD3 + "c", styleContext, ref borderContext3);
						borderContext3 = borderContext;
						RenderSharedStyle(element3, elementProps2, definition2, sharedStyle2, null, iD3 + "cl", styleContext, ref borderContext3);
						RenderSharedStyle(element3, elementProps2, definition2, sharedStyle2, null, iD3 + "cr", styleContext, ref borderContext);
						styleContext.StyleOnCell = false;
						styleContext.OnlyRenderMeasurementsBackgroundBorders = onlyRenderMeasurementsBackgroundBorders;
					}
					styleContext.InTablix = true;
					if (element3 is RPLContainer)
					{
						RPLItemMeasurement val8 = new RPLItemMeasurement();
						((RPLSizes)val8).Width = val5.GetColumnWidth(val6.ColIndex, val6.ColSpan);
						((RPLSizes)val8).Height = val5.GetRowHeight(val6.RowIndex, val6.RowSpan);
						val8.Element = (RPLItem)(object)((element3 is RPLItem) ? element3 : null);
						RenderStylesOnlyRecursive(val8, styleContext);
					}
					else if (!m_usedStyles.ContainsKey(iD3))
					{
						if (element3 is RPLTextBox)
						{
							object obj2 = element3.ElementProps.Style[(byte)26];
							RPLTextBoxPropsDef val9 = (RPLTextBoxPropsDef)element3.ElementProps.Definition;
							bool flag = obj2 != null && (int)(VerticalAlignments)obj2 != 0 && !val9.CanGrow;
							if (val9.CanSort || flag)
							{
								styleContext.RenderMeasurements = false;
							}
						}
						RenderSharedStyle(element3, elementProps2, definition2, sharedStyle2, null, element3.ElementProps.Definition.ID, styleContext, ref borderContext);
					}
					styleContext.InTablix = inTablix;
					nextRow[l] = null;
					styleContext.ZeroWidth = zeroWidth;
				}
				nextRow = val5.GetNextRow();
			}
		}
		measurement.Element = null;
	}

	internal void RenderEmptyTopTablixRow(RPLTablix tablix, List<RPLTablixOmittedRow> omittedRows, string tablixID, bool emptyCol, TablixFixedHeaderStorage headerStorage)
	{
		bool flag = headerStorage.RowHeaders != null || headerStorage.ColumnHeaders != null;
		WriteStream(m_openTR);
		if (flag)
		{
			string text = tablixID + "r";
			RenderReportItemId(text);
			if (headerStorage.RowHeaders != null)
			{
				headerStorage.RowHeaders.Add(text);
			}
			if (headerStorage.ColumnHeaders != null)
			{
				headerStorage.ColumnHeaders.Add(text);
			}
			if (headerStorage.CornerHeaders != null)
			{
				headerStorage.CornerHeaders.Add(text);
			}
		}
		WriteStream(m_zeroHeight);
		WriteStream(m_closeBracket);
		if (emptyCol)
		{
			headerStorage.HasEmptyCol = true;
			WriteStream(m_openTD);
			if (headerStorage.RowHeaders != null)
			{
				string text2 = tablixID + "e";
				RenderReportItemId(text2);
				headerStorage.RowHeaders.Add(text2);
				if (headerStorage.CornerHeaders != null)
				{
					headerStorage.CornerHeaders.Add(text2);
				}
			}
			WriteStream(m_openStyle);
			WriteStream(m_styleWidth);
			WriteStream("0");
			WriteStream(m_px);
			WriteStream(m_closeQuote);
			WriteStream(m_closeTD);
		}
		int[] array = new int[omittedRows.Count];
		for (int i = 0; i < tablix.ColumnWidths.Length; i++)
		{
			WriteStream(m_openTD);
			if (tablix.FixedColumns[i] && headerStorage.RowHeaders != null)
			{
				string text3 = tablixID + "e" + i;
				RenderReportItemId(text3);
				headerStorage.RowHeaders.Add(text3);
				if (i == tablix.ColumnWidths.Length - 1 || !tablix.FixedColumns[i + 1])
				{
					headerStorage.LastRowGroupCol = text3;
				}
				if (headerStorage.CornerHeaders != null)
				{
					headerStorage.CornerHeaders.Add(text3);
				}
			}
			WriteStream(m_openStyle);
			if (tablix.ColumnWidths[i] == 0f)
			{
				WriteStream(m_displayNone);
			}
			WriteStream(m_styleWidth);
			WriteDStream(tablix.ColumnWidths[i]);
			WriteStream(m_mm);
			WriteStream(m_semiColon);
			WriteStream(m_styleMinWidth);
			WriteDStream(tablix.ColumnWidths[i]);
			WriteStream(m_mm);
			WriteStream(m_closeQuote);
			for (int j = 0; j < omittedRows.Count; j++)
			{
				List<RPLTablixMemberCell> omittedHeaders = ((RPLTablixRow)omittedRows[j]).OmittedHeaders;
				RenderTablixOmittedHeaderCells(omittedHeaders, i, lastCol: false, ref array[j]);
			}
			WriteStream(m_closeTD);
		}
		WriteStream(m_closeTR);
	}

	internal void RenderEmptyHeightCell(float height, string tablixID, bool fixedRow, int row, TablixFixedHeaderStorage headerStorage)
	{
		WriteStream(m_openTD);
		if (headerStorage.RowHeaders != null)
		{
			string text = tablixID + "h" + row;
			RenderReportItemId(text);
			headerStorage.RowHeaders.Add(text);
			if (fixedRow && headerStorage.CornerHeaders != null)
			{
				headerStorage.CornerHeaders.Add(text);
			}
		}
		WriteStream(m_openStyle);
		WriteStream(m_styleHeight);
		WriteDStream(height);
		WriteStream(m_mm);
		WriteStream(m_closeQuote);
		WriteStream(m_closeTD);
	}

	protected static int GetNewContext(int borderContext, bool left, bool right, bool top, bool bottom)
	{
		int num = 0;
		if (borderContext > 0)
		{
			if (top)
			{
				num |= borderContext & 4;
			}
			if (bottom)
			{
				num |= borderContext & 8;
			}
			if (left)
			{
				num |= borderContext & 1;
			}
			if (right)
			{
				num |= borderContext & 2;
			}
		}
		return num;
	}

	protected static int GetNewContext(int borderContext, int x, int y, int xMax, int yMax)
	{
		int num = 0;
		if (borderContext > 0)
		{
			if (x == 1)
			{
				num |= borderContext & 4;
			}
			if (x == xMax)
			{
				num |= borderContext & 8;
			}
			if (y == 1)
			{
				num |= borderContext & 1;
			}
			if (y == yMax)
			{
				num |= borderContext & 2;
			}
		}
		return num;
	}

	protected Rectangle RenderDynamicImage(RPLItemMeasurement measurement, RPLDynamicImageProps dynamicImageProps)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if ((int)m_createSecondaryStreams != 0)
		{
			return dynamicImageProps.ImageConsolidationOffsets;
		}
		Stream stream = null;
		stream = CreateStream(dynamicImageProps.StreamName, "png", null, "image/png", willSeek: false, StreamOper.CreateAndRegister);
		if (dynamicImageProps.DynamicImageContentOffset >= 0)
		{
			m_rplReport.GetImage(dynamicImageProps.DynamicImageContentOffset, stream);
		}
		else if (dynamicImageProps.DynamicImageContent != null)
		{
			byte[] array = new byte[4096];
			dynamicImageProps.DynamicImageContent.Position = 0L;
			int num = (int)dynamicImageProps.DynamicImageContent.Length;
			while (num > 0)
			{
				int num2 = dynamicImageProps.DynamicImageContent.Read(array, 0, Math.Min(array.Length, num));
				stream.Write(array, 0, num2);
				num -= num2;
			}
		}
		return Rectangle.Empty;
	}

	protected bool IsCollectionWithoutContent(RPLContainer container, ref bool empty)
	{
		bool result = false;
		if (container != null)
		{
			result = true;
			if (container.Children == null)
			{
				empty = true;
			}
		}
		return result;
	}

	private void RenderOpenStyle(string id)
	{
		WriteStreamLineBreak();
		if (m_styleClassPrefix != null)
		{
			WriteStream(m_styleClassPrefix);
		}
		WriteStream(m_dot);
		WriteStream(m_stylePrefixIdBytes);
		WriteStream(id);
		WriteStream(m_openAccol);
	}

	protected virtual RPLReport GetNextPage()
	{
		RPLReport result = default(RPLReport);
		m_spbProcessing.GetNextPage(ref result);
		return result;
	}

	protected virtual bool NeedSharedToggleParent(RPLTextBoxProps textBoxProps)
	{
		if (!IsFragment)
		{
			return textBoxProps.IsToggleParent;
		}
		return false;
	}

	protected virtual bool CanSort(RPLTextBoxPropsDef textBoxDef)
	{
		if (!IsFragment)
		{
			return textBoxDef.CanSort;
		}
		return false;
	}

	protected void RenderSortImage(RPLTextBoxProps textBoxProps)
	{
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		if (m_deviceInfo.BrowserMode == BrowserMode.Quirks || m_deviceInfo.IsBrowserIE)
		{
			WriteStream(m_nbsp);
		}
		WriteStream(m_openA);
		WriteStream(m_tabIndex);
		WriteStream(++m_tabIndexNum);
		WriteStream(m_quote);
		SortOptions sortState = textBoxProps.SortState;
		RenderSortAction(textBoxProps, sortState);
		WriteStream(m_img);
		if (m_browserIE)
		{
			WriteStream(m_imgOnError);
		}
		WriteStream(m_zeroBorder);
		WriteStream(m_src);
		RenderSortImageText(sortState);
		WriteStream(m_closeTag);
		WriteStream(m_closeA);
	}

	protected virtual void RenderSortImageText(SortOptions sortState)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Invalid comparison between Unknown and I4
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Invalid comparison between Unknown and I4
		RenderInternalImageSrc();
		if ((int)sortState == 1)
		{
			WriteStream(m_report.GetImageName("sortAsc.gif"));
		}
		else if ((int)sortState == 2)
		{
			WriteStream(m_report.GetImageName("sortDesc.gif"));
		}
		else
		{
			WriteStream(m_report.GetImageName("unsorted.gif"));
		}
	}

	internal void RenderOnClickActionScript(string actionType, string actionArg)
	{
		WriteStream(" onclick=\"");
		WriteStream(m_deviceInfo.ActionScript);
		WriteStream("('");
		WriteStream(actionType);
		WriteStream("','");
		WriteStream(actionArg);
		WriteStream("');return false;\"");
		WriteStream(" onkeypress=\"");
		WriteStream(m_checkForEnterKey);
		WriteStream(m_deviceInfo.ActionScript);
		WriteStream("('");
		WriteStream(actionType);
		WriteStream("','");
		WriteStream(actionArg);
		WriteStream("');}return false;\"");
	}

	protected PaddingSharedInfo GetPaddings(RPLElementStyle style, PaddingSharedInfo paddingInfo)
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Expected O, but got Unknown
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Expected O, but got Unknown
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Expected O, but got Unknown
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Expected O, but got Unknown
		int num = 0;
		RPLReportSize val = null;
		double num2 = 0.0;
		double num3 = 0.0;
		bool flag = false;
		PaddingSharedInfo result = paddingInfo;
		if (paddingInfo != null)
		{
			num = paddingInfo.PaddingContext;
			num2 = paddingInfo.PadH;
			num3 = paddingInfo.PadV;
		}
		if ((num & 4) == 0)
		{
			string text = (string)style[(byte)17];
			if (text != null)
			{
				val = new RPLReportSize(text);
				flag = true;
				num |= 4;
				num3 += val.ToMillimeters();
			}
		}
		if ((num & 8) == 0)
		{
			flag = true;
			string text2 = (string)style[(byte)18];
			if (text2 != null)
			{
				val = new RPLReportSize(text2);
				num |= 8;
				num3 += val.ToMillimeters();
			}
		}
		if ((num & 1) == 0)
		{
			flag = true;
			string text3 = (string)style[(byte)15];
			if (text3 != null)
			{
				val = new RPLReportSize(text3);
				num |= 1;
				num2 += val.ToMillimeters();
			}
		}
		if ((num & 2) == 0)
		{
			flag = true;
			string text4 = (string)style[(byte)16];
			if (text4 != null)
			{
				val = new RPLReportSize(text4);
				num |= 2;
				num2 += val.ToMillimeters();
			}
		}
		if (flag)
		{
			result = new PaddingSharedInfo(num, num2, num3);
		}
		return result;
	}

	protected bool NeedReportItemId(RPLElement repItem, RPLElementProps props)
	{
		if (m_pageSection != PageSection.Body)
		{
			return false;
		}
		bool flag = m_linkToChildStack.Count > 0 && props.Definition.ID.Equals(m_linkToChildStack.Peek());
		if (flag)
		{
			m_linkToChildStack.Pop();
		}
		RPLItemProps val = (RPLItemProps)(object)((props is RPLItemProps) ? props : null);
		RPLElementPropsDef definition = ((RPLElementProps)val).Definition;
		RPLItemPropsDef val2 = (RPLItemPropsDef)(object)((definition is RPLItemPropsDef) ? definition : null);
		string bookmark = val.Bookmark;
		if (bookmark == null)
		{
			bookmark = val2.Bookmark;
		}
		string label = val.Label;
		if (label == null)
		{
			label = val2.Label;
		}
		if (bookmark == null && label == null)
		{
			return flag;
		}
		return true;
	}

	protected void RenderHtmlBody()
	{
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Expected O, but got Unknown
		int num = 0;
		m_isBody = true;
		m_hasOnePage = m_spbProcessing.Done || m_pageNum != 0;
		RenderPageStart(firstPage: true, m_spbProcessing.Done, m_pageContent.PageLayout.Style);
		m_pageSection = PageSection.Body;
		bool flag = m_rplReport != null;
		while (flag)
		{
			bool flag2 = m_pageContent.ReportSectionSizes.Length > 1 || m_rplReportSection.Header != null || m_rplReportSection.Footer != null;
			if (flag2)
			{
				WriteStream(m_openTable);
				WriteStream(m_closeBracket);
			}
			while (m_rplReportSection != null)
			{
				num = 0;
				RPLItemMeasurement header = m_rplReportSection.Header;
				RPLItemMeasurement footer = m_rplReportSection.Footer;
				StyleContext styleContext = new StyleContext();
				RPLItemMeasurement val = m_rplReportSection.Columns[0];
				RPLItem element = val.Element;
				RPLBody val2 = (RPLBody)(object)((element is RPLBody) ? element : null);
				RPLElementProps elementProps = ((RPLElement)val2).ElementProps;
				RPLItemProps val3 = (RPLItemProps)(object)((elementProps is RPLItemProps) ? elementProps : null);
				RPLElementPropsDef definition = ((RPLElementProps)val3).Definition;
				RPLItemPropsDef val4 = (RPLItemPropsDef)(object)((definition is RPLItemPropsDef) ? definition : null);
				if (flag2)
				{
					if (header != null)
					{
						m_pageSection = PageSection.PageHeader;
						m_isBody = false;
						RenderPageHeaderFooter(header);
						m_isBody = true;
					}
					WriteStream(m_firstTD);
					styleContext.StyleOnCell = true;
					RenderReportItemStyle((RPLElement)(object)val2, (RPLElementProps)(object)val3, (RPLElementPropsDef)(object)val4, null, styleContext, ref num, ((RPLElementPropsDef)val4).ID + "c");
					styleContext.StyleOnCell = false;
					WriteStream(m_closeBracket);
				}
				m_pageSection = PageSection.Body;
				m_isBody = true;
				RPLItemMeasurement val5 = new RPLItemMeasurement();
				((RPLSizes)val5).Width = m_pageContent.MaxSectionWidth;
				((RPLSizes)val5).Height = m_rplReportSection.BodyArea.Height;
				RenderRectangle((RPLContainer)(object)val2, (RPLElementProps)(object)val3, (RPLElementPropsDef)(object)val4, val5, ref num, renderId: false, styleContext);
				if (flag2)
				{
					WriteStream(m_closeTD);
					WriteStream(m_closeTR);
					if (footer != null)
					{
						m_pageSection = PageSection.PageFooter;
						m_isBody = false;
						RenderPageHeaderFooter(footer);
						m_isBody = true;
					}
				}
				m_rplReportSection = m_pageContent.GetNextReportSection();
				val.Element = null;
			}
			if (flag2)
			{
				WriteStream(m_closeTable);
			}
			RenderPageEnd();
			if (m_pageNum == 0)
			{
				if (!m_spbProcessing.Done)
				{
					if (m_rplReport != null)
					{
						m_rplReport.Release();
					}
					RPLReport val6 = null;
					val6 = GetNextPage();
					m_pageContent = val6.RPLPaginatedPages[0];
					m_rplReportSection = m_pageContent.GetNextReportSection();
					m_rplReport = val6;
					WriteStream(m_pageBreakDelimiter);
					RenderPageStart(firstPage: false, m_spbProcessing.Done, m_pageContent.PageLayout.Style);
					num = 0;
				}
				else
				{
					flag = false;
				}
			}
			else
			{
				flag = false;
			}
		}
		if (m_rplReport != null)
		{
			m_rplReport.Release();
		}
	}

	protected abstract void WriteScrollbars();

	protected abstract void WriteFixedHeaderOnScrollScript();

	protected abstract void WriteFixedHeaderPropertyChangeScript();

	protected virtual void RenderPageStart(bool firstPage, bool lastPage, RPLElementStyle pageStyle)
	{
		WriteStream(m_openDiv);
		WriteStream(m_ltrDir);
		RenderPageStartDimensionStyles(lastPage);
		if (firstPage)
		{
			RenderReportItemId("oReportDiv");
		}
		bool flag = m_hasOnePage && m_deviceInfo.AllowScript && m_deviceInfo.HTMLFragment;
		if (flag)
		{
			WriteFixedHeaderOnScrollScript();
		}
		if (m_pageHasStyle)
		{
			WriteStream(m_closeBracket);
			WriteStream(m_openDiv);
			OpenStyle();
			if (FillPageHeight)
			{
				WriteStream(m_styleHeight);
				WriteStream(m_percent);
				WriteStream(m_semiColon);
			}
			WriteStream(m_styleWidth);
			WriteStream(m_percent);
			WriteStream(m_semiColon);
			RenderPageStyle(pageStyle);
			CloseStyle(renderQuote: true);
		}
		WriteStream(m_closeBracket);
		WriteStream(m_openTable);
		WriteStream(m_closeBracket);
		WriteStream(m_firstTD);
		if (firstPage)
		{
			RenderReportItemId("oReportCell");
		}
		RenderZoom();
		if (flag)
		{
			WriteFixedHeaderPropertyChangeScript();
		}
		WriteStream(m_closeBracket);
	}

	protected virtual void RenderPageStartDimensionStyles(bool lastPage)
	{
		if (m_pageNum != 0 || lastPage)
		{
			WriteStream(m_openStyle);
			WriteScrollbars();
			if (m_deviceInfo.IsBrowserIE)
			{
				WriteStream(m_styleHeight);
				WriteStream(m_percent);
				WriteStream(m_semiColon);
			}
			WriteStream(m_styleWidth);
			WriteStream(m_percent);
			WriteStream(m_semiColon);
			WriteStream("direction:ltr");
			WriteStream(m_quote);
		}
		else
		{
			OpenStyle();
			WriteStream("direction:ltr");
			CloseStyle(renderQuote: true);
		}
	}

	private void RenderPageStyle(RPLElementStyle style)
	{
		int borderContext = 0;
		if (m_useInlineStyle)
		{
			OpenStyle();
			RenderBackgroundStyleProps((IRPLStyle)(object)style);
			RenderHtmlBorders((IRPLStyle)(object)style, ref borderContext, 0, renderPadding: true, isNonShared: true, null);
			CloseStyle(renderQuote: true);
			return;
		}
		RPLStyleProps sharedProperties = style.SharedProperties;
		RPLStyleProps nonSharedProperties = style.NonSharedProperties;
		if (sharedProperties != null && sharedProperties.Count > 0)
		{
			CloseStyle(renderQuote: true);
			string text = "p";
			byte[] array = (byte[])m_usedStyles[text];
			if (array == null)
			{
				array = m_encoding.GetBytes(text);
				m_usedStyles.Add(text, array);
				if (m_onlyVisibleStyles)
				{
					Stream mainStream = m_mainStream;
					m_mainStream = m_styleStream;
					RenderOpenStyle(text);
					RenderBackgroundStyleProps((IRPLStyle)(object)sharedProperties);
					RenderHtmlBorders((IRPLStyle)(object)sharedProperties, ref borderContext, 0, renderPadding: true, isNonShared: true, null);
					WriteStream(m_closeAccol);
					m_mainStream = mainStream;
				}
			}
			WriteClassStyle(array, close: true);
		}
		if (nonSharedProperties != null && nonSharedProperties.Count > 0)
		{
			OpenStyle();
			borderContext = 0;
			RenderHtmlBorders((IRPLStyle)(object)nonSharedProperties, ref borderContext, 0, renderPadding: true, isNonShared: true, (IRPLStyle)(object)sharedProperties);
			RenderBackgroundStyleProps((IRPLStyle)(object)nonSharedProperties);
			CloseStyle(renderQuote: true);
		}
	}

	protected void OpenStyle()
	{
		if (!m_isStyleOpen)
		{
			m_isStyleOpen = true;
			WriteStream(m_openStyle);
		}
	}

	protected void CloseStyle(bool renderQuote)
	{
		if (m_isStyleOpen && renderQuote)
		{
			WriteStream(m_quote);
		}
		m_isStyleOpen = false;
	}

	internal void WriteClassName(byte[] className, byte[] classNameIfNoPrefix)
	{
		if (m_deviceInfo.HtmlPrefixId.Length > 0 || classNameIfNoPrefix == null)
		{
			WriteStream(m_classStyle);
			WriteAttrEncoded(m_deviceInfo.HtmlPrefixId);
			WriteStream(className);
			WriteStream(m_quote);
		}
		else
		{
			WriteStream(classNameIfNoPrefix);
		}
	}

	protected virtual void WriteClassStyle(byte[] styleBytes, bool close)
	{
		WriteStream(m_classStyle);
		WriteStream(m_stylePrefixIdBytes);
		WriteStream(styleBytes);
		if (close)
		{
			WriteStream(m_quote);
		}
	}

	protected void RenderBackgroundStyleProps(IRPLStyle style)
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Expected O, but got Unknown
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		object obj = style[(byte)34];
		if (obj != null)
		{
			WriteStream(m_backgroundColor);
			WriteStream(obj);
			WriteStream(m_semiColon);
		}
		obj = style[(byte)33];
		if (obj != null)
		{
			WriteStream(m_backgroundImage);
			RenderImageUrl(useSessionId: true, (RPLImageData)obj);
			WriteStream(m_closeBrace);
			WriteStream(m_semiColon);
		}
		obj = style[(byte)35];
		if (obj != null)
		{
			obj = EnumStrings.GetValue((BackgroundRepeatTypes)obj);
			WriteStream(m_backgroundRepeat);
			WriteStream(obj);
			WriteStream(m_semiColon);
		}
	}

	protected virtual void RenderPageEnd()
	{
		if (m_deviceInfo.ExpandContent)
		{
			WriteStream(m_lastTD);
			WriteStream(m_closeTable);
		}
		else
		{
			WriteStream(m_closeTD);
			WriteStream(m_openTD);
			WriteStream(m_inlineWidth);
			WriteStream(m_percent);
			WriteStream(m_quote);
			WriteStream(m_inlineHeight);
			WriteStream("0");
			WriteStream(m_closeQuote);
			WriteStream(m_lastTD);
			WriteStream(m_firstTD);
			WriteStream(m_inlineWidth);
			if (m_deviceInfo.IsBrowserGeckoEngine)
			{
				WriteStream(m_percent);
			}
			else
			{
				WriteStream("0");
			}
			WriteStream(m_quote);
			WriteStream(m_inlineHeight);
			WriteStream(m_percent);
			WriteStream(m_closeQuote);
			WriteStream(m_lastTD);
			WriteStream(m_closeTable);
		}
		if (m_pageHasStyle)
		{
			WriteStream(m_closeDiv);
		}
		WriteStream(m_closeDiv);
	}

	public virtual void WriteStream(string theString)
	{
		if (theString.Length != 0)
		{
			byte[] array = null;
			array = m_encoding.GetBytes(theString);
			m_mainStream.Write(array, 0, array.Length);
		}
	}

	internal void WriteStream(object theString)
	{
		if (theString != null)
		{
			WriteStream(theString.ToString());
		}
	}

	public virtual void WriteStream(byte[] theBytes)
	{
		m_mainStream.Write(theBytes, 0, theBytes.Length);
	}

	protected void WriteStreamCR(string theString)
	{
		WriteStream(theString);
	}

	protected void WriteStreamCR(byte[] theBytes)
	{
		WriteStream(theBytes);
	}

	protected void WriteStreamEncoded(string theString)
	{
		WriteStream(HttpUtility.HtmlEncode(theString));
	}

	protected void WriteAttrEncoded(byte[] attributeName, string theString)
	{
		WriteAttribute(attributeName, m_encoding.GetBytes(HttpUtility.HtmlAttributeEncode(theString)));
	}

	protected virtual void WriteAttribute(byte[] attributeName, byte[] value)
	{
		WriteStream(attributeName);
		WriteStream(value);
		WriteStream(m_quote);
	}

	protected void WriteAttrEncoded(string theString)
	{
		WriteStream(HttpUtility.HtmlAttributeEncode(theString));
	}

	protected void WriteStreamCREncoded(string theString)
	{
		WriteStream(HttpUtility.HtmlEncode(theString));
	}

	protected virtual void WriteStreamLineBreak()
	{
	}

	protected void WriteRSStream(float size)
	{
		WriteStream(size.ToString("f2", CultureInfo.InvariantCulture));
		WriteStream(m_mm);
	}

	protected void WriteRSStreamCR(float size)
	{
		WriteStream(size.ToString("f2", CultureInfo.InvariantCulture));
		WriteStreamCR(m_mm);
	}

	protected void WriteDStream(float size)
	{
		WriteStream(size.ToString("f2", CultureInfo.InvariantCulture));
	}

	private void WriteIdToSecondaryStream(Stream secondaryStream, string tagId)
	{
		Stream mainStream = m_mainStream;
		m_mainStream = secondaryStream;
		WriteReportItemId(tagId);
		WriteStream(',');
		m_mainStream = mainStream;
	}

	internal static void QuoteString(StringBuilder output, string input)
	{
		if (output == null || input == null || input.Length == 0)
		{
			return;
		}
		int i = output.Length;
		output.Append(input);
		for (; i < output.Length; i++)
		{
			if (output[i] == '\\' || output[i] == '"' || output[i] == '\'')
			{
				output.Insert(i, '\\');
				i++;
			}
		}
	}

	protected byte[] RenderSharedStyle(RPLElement reportItem, RPLElementProps props, RPLElementPropsDef definition, RPLStyleProps sharedStyle, RPLItemMeasurement measurement, string id, StyleContext styleContext, ref int borderContext)
	{
		return RenderSharedStyle(reportItem, props, definition, sharedStyle, null, measurement, id, styleContext, ref borderContext);
	}

	protected byte[] RenderSharedStyle(RPLElement reportItem, RPLElementProps props, RPLElementPropsDef definition, RPLStyleProps sharedStyle, RPLStyleProps nonSharedStyle, RPLItemMeasurement measurement, string id, StyleContext styleContext, ref int borderContext)
	{
		Stream mainStream = m_mainStream;
		m_mainStream = m_styleStream;
		RenderOpenStyle(id);
		byte omitBordersState = styleContext.OmitBordersState;
		styleContext.OmitBordersState = 0;
		RenderStyleProps(reportItem, props, definition, measurement, (IRPLStyle)(object)sharedStyle, (IRPLStyle)(object)nonSharedStyle, styleContext, ref borderContext, isNonSharedStyles: false);
		styleContext.OmitBordersState = omitBordersState;
		WriteStream(m_closeAccol);
		m_mainStream = mainStream;
		byte[] bytes = m_encoding.GetBytes(id);
		m_usedStyles.Add(id, bytes);
		return bytes;
	}

	protected byte[] RenderSharedStyle(ElementStyleWriter styleWriter, RPLStyleProps sharedStyle, StyleContext styleContext, string id)
	{
		if (sharedStyle == null || id == null)
		{
			return null;
		}
		Stream mainStream = m_mainStream;
		m_mainStream = m_styleStream;
		RenderOpenStyle(id);
		byte omitBordersState = styleContext.OmitBordersState;
		styleContext.OmitBordersState = 0;
		styleWriter.WriteStyles(StyleWriterMode.Shared, (IRPLStyle)(object)sharedStyle);
		styleContext.OmitBordersState = omitBordersState;
		WriteStream(m_closeAccol);
		m_mainStream = mainStream;
		byte[] bytes = m_encoding.GetBytes(id);
		m_usedStyles.Add(id, bytes);
		return bytes;
	}

	protected void RenderMeasurementStyle(float height, float width)
	{
		RenderMeasurementStyle(height, width, renderMin: false);
	}

	protected void RenderMeasurementStyle(float height, float width, bool renderMin)
	{
		RenderMeasurementHeight(height, renderMin);
		RenderMeasurementWidth(width, renderMinWidth: true);
	}

	protected void RenderMeasurementHeight(float height, bool renderMin)
	{
		if (renderMin)
		{
			WriteStream(m_styleMinHeight);
		}
		else
		{
			WriteStream(m_styleHeight);
		}
		WriteRSStream(height);
		WriteStream(m_semiColon);
	}

	protected void RenderMeasurementMinHeight(float height)
	{
		WriteStream(m_styleMinHeight);
		WriteRSStream(height);
		WriteStream(m_semiColon);
	}

	protected void RenderMeasurementWidth(float width, bool renderMinWidth)
	{
		WriteStream(m_styleWidth);
		WriteRSStream(width);
		WriteStream(m_semiColon);
		if (renderMinWidth)
		{
			RenderMeasurementMinWidth(width);
		}
	}

	protected void RenderMeasurementMinWidth(float minWidth)
	{
		WriteStream(m_styleMinWidth);
		WriteRSStream(minWidth);
		WriteStream(m_semiColon);
	}

	protected void RenderMeasurementHeight(float height)
	{
		RenderMeasurementHeight(height, renderMin: false);
	}

	protected void RenderMeasurementWidth(float width)
	{
		RenderMeasurementWidth(width, renderMinWidth: false);
	}

	private bool ReportPageHasBorder(IRPLStyle style, string backgroundColor)
	{
		bool flag = ReportPageBorder(style, Border.All, backgroundColor);
		if (!flag)
		{
			flag = ReportPageBorder(style, Border.Left, backgroundColor);
			if (!flag)
			{
				flag = ReportPageBorder(style, Border.Right, backgroundColor);
				if (!flag)
				{
					flag = ReportPageBorder(style, Border.Bottom, backgroundColor);
					if (!flag)
					{
						flag = ReportPageBorder(style, Border.Top, backgroundColor);
					}
				}
			}
		}
		return flag;
	}

	protected virtual void RenderDynamicImageSrc(RPLDynamicImageProps dynamicImageProps)
	{
		string text = null;
		string streamName = dynamicImageProps.StreamName;
		if (streamName != null)
		{
			text = m_report.GetStreamUrl(useSessionId: true, streamName);
		}
		if (text != null)
		{
			WriteStream(text);
		}
	}

	protected void RenderHtmlBorders(IRPLStyle styleProps, ref int borderContext, byte omitBordersState, bool renderPadding, bool isNonShared, IRPLStyle sharedStyleProps)
	{
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Expected O, but got Unknown
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		if (renderPadding)
		{
			RenderPaddingStyle(styleProps);
		}
		if (styleProps == null || borderContext == 15)
		{
			return;
		}
		object obj = styleProps[(byte)10];
		object obj2 = styleProps[(byte)5];
		object obj3 = styleProps[(byte)0];
		IRPLStyle val = styleProps;
		if (isNonShared && sharedStyleProps != null && !OnlyGeneralBorder(sharedStyleProps) && !OnlyGeneralBorder(styleProps))
		{
			val = (IRPLStyle)new RPLElementStyle((RPLStyleProps)(object)((styleProps is RPLStyleProps) ? styleProps : null), (RPLStyleProps)(object)((sharedStyleProps is RPLStyleProps) ? sharedStyleProps : null));
		}
		if (borderContext != 0 || omitBordersState != 0 || !OnlyGeneralBorder(val))
		{
			if (obj2 == null || (int)(BorderStyles)obj2 == 0)
			{
				RenderBorderStyle(obj, obj2, obj3, Border.All);
			}
			if ((borderContext & 8) == 0 && (omitBordersState & 2) == 0 && RenderBorderInstance(val, obj, obj2, obj3, Border.Bottom))
			{
				borderContext |= 8;
			}
			if ((borderContext & 1) == 0 && (omitBordersState & 4) == 0 && RenderBorderInstance(val, obj, obj2, obj3, Border.Left))
			{
				borderContext |= 1;
			}
			if ((borderContext & 2) == 0 && (omitBordersState & 8) == 0 && RenderBorderInstance(val, obj, obj2, obj3, Border.Right))
			{
				borderContext |= 2;
			}
			if ((borderContext & 4) == 0 && (omitBordersState & 1) == 0 && RenderBorderInstance(val, obj, obj2, obj3, Border.Top))
			{
				borderContext |= 4;
			}
		}
		else
		{
			if (obj2 != null && (int)(BorderStyles)obj2 != 0)
			{
				borderContext = 15;
			}
			RenderBorderStyle(obj, obj2, obj3, Border.All);
		}
	}

	protected void RenderPaddingStyle(IRPLStyle styleProps)
	{
		if (styleProps != null)
		{
			object obj = styleProps[(byte)15];
			if (obj != null)
			{
				WriteStream(m_paddingLeft);
				WriteStream(obj);
				WriteStream(m_semiColon);
			}
			obj = styleProps[(byte)17];
			if (obj != null)
			{
				WriteStream(m_paddingTop);
				WriteStream(obj);
				WriteStream(m_semiColon);
			}
			obj = styleProps[(byte)16];
			if (obj != null)
			{
				WriteStream(m_paddingRight);
				WriteStream(obj);
				WriteStream(m_semiColon);
			}
			obj = styleProps[(byte)18];
			if (obj != null)
			{
				WriteStream(m_paddingBottom);
				WriteStream(obj);
				WriteStream(m_semiColon);
			}
		}
	}

	protected void RenderMultiLineText(string text)
	{
		if (text == null)
		{
			return;
		}
		int num = 0;
		int num2 = 0;
		int length = text.Length;
		string text2 = null;
		for (int i = 0; i < length; i++)
		{
			switch (text[i])
			{
			case '\r':
				text2 = text.Substring(num2, num - num2);
				WriteStreamEncoded(text2);
				num2 = num + 1;
				break;
			case '\n':
				text2 = text.Substring(num2, num - num2);
				if (!string.IsNullOrEmpty(text2.Trim()))
				{
					WriteStreamEncoded(text2);
				}
				WriteStreamCR(m_br);
				num2 = num + 1;
				break;
			}
			num++;
		}
		if (num2 == 0)
		{
			WriteStreamEncoded(text);
		}
		else
		{
			WriteStreamEncoded(text.Substring(num2, num - num2));
		}
	}

	protected bool IsLineSlanted(RPLItemMeasurement measurement)
	{
		if (measurement == null)
		{
			return false;
		}
		if (((RPLSizes)measurement).Width != 0f && ((RPLSizes)measurement).Height != 0f)
		{
			return true;
		}
		return false;
	}

	protected void RenderCellItem(PageTableCell currCell, int borderContext, bool layoutExpand)
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Expected O, but got Unknown
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Invalid comparison between Unknown and I4
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Expected O, but got Unknown
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Expected O, but got Unknown
		bool flag = false;
		RPLItemMeasurement val = null;
		val = currCell.Measurement;
		RPLItem element = val.Element;
		if (element == null)
		{
			return;
		}
		RPLElementProps elementProps = ((RPLElement)element).ElementProps;
		RPLItemProps val2 = (RPLItemProps)(object)((elementProps is RPLItemProps) ? elementProps : null);
		RPLElementPropsDef definition = ((RPLElementProps)val2).Definition;
		RPLItemPropsDef val3 = (RPLItemPropsDef)(object)((definition is RPLItemPropsDef) ? definition : null);
		flag = NeedReportItemId((RPLElement)(object)val.Element, (RPLElementProps)(object)val2);
		bool flag2 = false;
		if (val2 is RPLImageProps)
		{
			RPLImagePropsDef val4 = (RPLImagePropsDef)val3;
			if ((int)val4.Sizing == 2)
			{
				flag2 = true;
			}
		}
		if (!flag2 && currCell.ConsumedByEmptyWhiteSpace)
		{
			if (val2 is RPLImageProps)
			{
				RPLImageProps val5 = (RPLImageProps)val2;
				_ = (RPLImagePropsDef)((RPLElementProps)val2).Definition;
				if (val5 != null)
				{
					_ = val5.Image.ImageConsolidationOffsets;
					if (!val5.Image.ImageConsolidationOffsets.IsEmpty)
					{
						flag2 = true;
					}
				}
			}
			if (!flag2 && val2 is RPLDynamicImageProps)
			{
				RPLDynamicImageProps val6 = (RPLDynamicImageProps)val2;
				if (val6 != null)
				{
					_ = val6.ImageConsolidationOffsets;
					if (!val6.ImageConsolidationOffsets.IsEmpty)
					{
						flag2 = true;
					}
				}
			}
		}
		if (flag2)
		{
			WriteStream(m_openDiv);
			OpenStyle();
			if (currCell.DXValue > ((RPLSizes)val).Width)
			{
				RenderMeasurementWidth(((RPLSizes)val).Width);
			}
			if (currCell.DYValue > ((RPLSizes)val).Height)
			{
				RenderMeasurementHeight(((RPLSizes)val).Height);
			}
			CloseStyle(renderQuote: true);
			WriteStream(m_closeBracket);
		}
		RenderReportItem((RPLElement)(object)element, (RPLElementProps)(object)val2, (RPLElementPropsDef)(object)val3, val, new StyleContext(), borderContext, flag);
		if (flag2)
		{
			WriteStream(m_closeDiv);
		}
		val.Element = null;
	}

	protected virtual void RenderBlankImage()
	{
		WriteStream(m_img);
		if (m_browserIE)
		{
			WriteStream(m_imgOnError);
		}
		WriteStream(m_src);
		RenderInternalImageSrc();
		WriteStream(m_report.GetImageName("Blank.gif"));
		WriteStream(m_closeTag);
	}

	protected virtual void RenderImageUrl(bool useSessionId, RPLImageData image)
	{
		string text = CreateImageStream(image);
		string text2 = null;
		if (text != null)
		{
			text2 = m_report.GetStreamUrl(useSessionId, text);
		}
		if (text2 != null)
		{
			WriteStream(text2);
		}
	}

	protected virtual void RenderReportItemId(string repItemId)
	{
		WriteStream(m_id);
		WriteReportItemId(repItemId);
		WriteStream(m_quote);
	}

	private void WriteReportItemId(string repItemId)
	{
		WriteAttrEncoded(m_deviceInfo.HtmlPrefixId);
		WriteStream(repItemId);
	}

	protected void RenderTextBox(RPLTextBox textBox, RPLTextBoxProps textBoxProps, RPLTextBoxPropsDef textBoxPropsDef, RPLItemMeasurement measurement, StyleContext styleContext, ref int borderContext, bool renderId)
	{
		//IL_05af: Unknown result type (might be due to invalid IL or missing references)
		//IL_0500: Unknown result type (might be due to invalid IL or missing references)
		//IL_0427: Unknown result type (might be due to invalid IL or missing references)
		string text = null;
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		RPLStyleProps actionStyle = null;
		RPLActionInfo actionInfo = textBoxProps.ActionInfo;
		RPLElementStyle style = ((RPLElementProps)textBoxProps).Style;
		bool flag4 = CanSort(textBoxPropsDef);
		bool flag5 = NeedSharedToggleParent(textBoxProps);
		bool flag6 = false;
		bool isSimple = textBoxPropsDef.IsSimple;
		bool flag7 = !isSimple && flag5;
		bool flag8 = flag4 || flag7;
		bool flag9 = IsDirectionRTL((IRPLStyle)(object)style);
		RPLStyleProps nonSharedStyle = ((RPLElementProps)textBoxProps).NonSharedStyle;
		RPLStyleProps sharedStyle = ((RPLElementPropsDef)textBoxPropsDef).SharedStyle;
		bool flag10 = IsWritingModeVertical((IRPLStyle)(object)style);
		bool flag11 = flag10 && m_deviceInfo.IsBrowserIE;
		bool ignoreVerticalAlign = styleContext.IgnoreVerticalAlign;
		if (isSimple)
		{
			text = textBoxProps.Value;
			if (string.IsNullOrEmpty(text))
			{
				text = textBoxPropsDef.Value;
			}
			if (string.IsNullOrEmpty(text) && !flag4 && !flag5)
			{
				flag = true;
			}
		}
		if (((RPLElementProps)textBoxProps).UniqueName == null)
		{
			flag4 = false;
			flag5 = false;
			renderId = false;
		}
		float adjustedWidth = GetAdjustedWidth(measurement, (IRPLStyle)(object)((RPLElementProps)textBoxProps).Style);
		float adjustedHeight = GetAdjustedHeight(measurement, (IRPLStyle)(object)((RPLElementProps)textBoxProps).Style);
		if (flag)
		{
			styleContext.EmptyTextBox = true;
			WriteStream(m_openTable);
			RenderReportLanguage();
			WriteStream(m_closeBracket);
			WriteStream(m_firstTD);
			if (m_deviceInfo.IsBrowserGeckoEngine)
			{
				WriteStream(m_openDiv);
			}
			OpenStyle();
			float width = ((RPLSizes)measurement).Width;
			float height = ((RPLSizes)measurement).Height;
			if (m_deviceInfo.IsBrowserIE6Or7StandardsMode)
			{
				width = adjustedWidth;
				height = adjustedHeight;
			}
			RenderMeasurementWidth(width, renderMinWidth: false);
			RenderMeasurementMinWidth(adjustedWidth);
			if (!textBoxPropsDef.CanShrink)
			{
				RenderMeasurementHeight(height);
			}
		}
		else
		{
			if (flag11)
			{
				WriteStream(m_openDiv);
				OpenStyle();
				RenderDirectionStyles((RPLElement)(object)textBox, (RPLElementProps)(object)textBoxProps, (RPLElementPropsDef)(object)textBoxPropsDef, null, (IRPLStyle)(object)((RPLElementProps)textBoxProps).Style, (IRPLStyle)(object)nonSharedStyle, isNonSharedStyles: false, styleContext);
				if (m_deviceInfo.IsBrowserIE6Or7StandardsMode && !textBoxPropsDef.CanShrink)
				{
					RenderMeasurementHeight(adjustedHeight);
					RenderHtmlBorders((IRPLStyle)(object)((RPLElementProps)textBoxProps).Style, ref borderContext, styleContext.OmitBordersState, renderPadding: true, isNonShared: true, null);
					styleContext.NoBorders = true;
				}
				WriteStream("display: inline;");
				bool flag12 = false;
				if (m_deviceInfo.BrowserMode == BrowserMode.Standards)
				{
					RenderMeasurementHeight(((RPLSizes)measurement).Height);
					flag12 = true;
				}
				CloseStyle(renderQuote: true);
				if (flag12 && m_deviceInfo.AllowScript)
				{
					if (!m_needsFitVertTextScript)
					{
						CreateFitVertTextIdsStream();
					}
					WriteIdToSecondaryStream(m_fitVertTextIdsStream, ((RPLElementProps)textBoxProps).UniqueName + "_fvt");
					RenderReportItemId(((RPLElementProps)textBoxProps).UniqueName + "_fvt");
				}
				WriteStream(m_closeBracket);
			}
			object obj = style[(byte)26];
			if (textBoxPropsDef.CanGrow)
			{
				WriteStream(m_openTable);
				RenderReportLanguage();
				OpenStyle();
				if (flag11)
				{
					if (m_deviceInfo.IsBrowserIE6Or7StandardsMode)
					{
						RenderMeasurementWidth(adjustedWidth, renderMinWidth: false);
						if (!textBoxPropsDef.CanShrink)
						{
							RenderMeasurementHeight(adjustedHeight);
						}
					}
					else
					{
						RenderMeasurementWidth(((RPLSizes)measurement).Width, renderMinWidth: true);
					}
				}
				if (isSimple && (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(text.Trim())))
				{
					WriteStream(m_borderCollapse);
				}
				CloseStyle(renderQuote: true);
				WriteStream(m_closeBracket);
				WriteStream(m_firstTD);
				OpenStyle();
				if (m_deviceInfo.IsBrowserIE6Or7StandardsMode && !textBoxPropsDef.CanShrink)
				{
					RenderMeasurementWidth(adjustedWidth, renderMinWidth: false);
				}
				else
				{
					RenderMeasurementWidth(((RPLSizes)measurement).Width, renderMinWidth: false);
				}
				RenderMeasurementMinWidth(adjustedWidth);
				if (!textBoxPropsDef.CanShrink)
				{
					if (m_deviceInfo.IsBrowserIE6Or7StandardsMode || (m_deviceInfo.IsBrowserSafari && m_deviceInfo.BrowserMode != BrowserMode.Quirks))
					{
						if (!flag11)
						{
							RenderMeasurementHeight(adjustedHeight);
						}
					}
					else
					{
						RenderMeasurementHeight(((RPLSizes)measurement).Height);
					}
				}
				styleContext.RenderMeasurements = false;
				if (flag8)
				{
					styleContext.StyleOnCell = true;
					RenderReportItemStyle((RPLElement)(object)textBox, (RPLElementProps)(object)textBoxProps, (RPLElementPropsDef)(object)textBoxPropsDef, nonSharedStyle, sharedStyle, measurement, styleContext, ref borderContext, ((RPLElementPropsDef)textBoxPropsDef).ID + "p");
					styleContext.StyleOnCell = false;
					styleContext.NoBorders = true;
				}
				if (textBoxPropsDef.CanShrink)
				{
					if (flag10 || (flag5 && flag9))
					{
						flag2 = true;
					}
					if (!flag2 && obj != null && !styleContext.IgnoreVerticalAlign)
					{
						obj = EnumStrings.GetValue((VerticalAlignments)obj);
						WriteStream(m_verticalAlign);
						WriteStream(obj);
						WriteStream(m_semiColon);
					}
					CloseStyle(renderQuote: true);
					WriteStreamCR(m_closeBracket);
					if (flag2)
					{
						WriteStream(m_openTable);
						WriteStream(m_inlineWidth);
						WriteStream(m_percent);
						WriteStream(m_quote);
						WriteStream(m_closeBracket);
						WriteStream(m_firstTD);
					}
					else
					{
						WriteStream(m_openDiv);
						if (!flag8)
						{
							styleContext.IgnoreVerticalAlign = true;
						}
					}
				}
			}
			else
			{
				WriteStream(m_openDiv);
				styleContext.IgnoreVerticalAlign = true;
				if (!m_deviceInfo.IsBrowserIE || m_deviceInfo.BrowserMode == BrowserMode.Standards || (obj != null && (int)(VerticalAlignments)obj != 0) || m_deviceInfo.OutlookCompat)
				{
					if (!flag8)
					{
						bool onlyRenderMeasurementsBackgroundBorders = styleContext.OnlyRenderMeasurementsBackgroundBorders;
						bool noBorders = styleContext.NoBorders;
						styleContext.OnlyRenderMeasurementsBackgroundBorders = true;
						int borderContext2 = 0;
						if (textBoxPropsDef.CanShrink)
						{
							styleContext.NoBorders = true;
						}
						RenderReportItemStyle((RPLElement)(object)textBox, (RPLElementProps)(object)textBoxProps, (RPLElementPropsDef)(object)textBoxPropsDef, nonSharedStyle, sharedStyle, measurement, styleContext, ref borderContext2, ((RPLElementPropsDef)textBoxPropsDef).ID + "v");
						styleContext.OnlyRenderMeasurementsBackgroundBorders = onlyRenderMeasurementsBackgroundBorders;
						measurement = null;
						if (textBoxPropsDef.CanShrink)
						{
							styleContext.NoBorders = noBorders;
						}
						else
						{
							styleContext.NoBorders = true;
						}
					}
					WriteStreamCR(m_closeBracket);
					styleContext.IgnoreVerticalAlign = ignoreVerticalAlign;
					if (obj != null && (int)(VerticalAlignments)obj != 0)
					{
						WriteStream(m_openTable);
						if (!flag4 || flag10)
						{
							WriteStream(m_inlineWidth);
							WriteStream(m_percent);
							WriteStream(m_quote);
						}
						if (!textBoxPropsDef.CanShrink)
						{
							WriteStream(m_inlineHeight);
							WriteStream(m_percent);
							WriteStream(m_quote);
						}
						WriteStream(m_zeroBorder);
						WriteStream(m_closeBracket);
						WriteStream(m_firstTD);
						flag2 = true;
					}
					else
					{
						WriteStream(m_openDiv);
						flag3 = true;
					}
				}
				if (flag8)
				{
					OpenStyle();
					if (m_deviceInfo.IsBrowserIE6Or7StandardsMode && !textBoxPropsDef.CanShrink)
					{
						RenderMeasurementWidth(adjustedWidth, renderMinWidth: false);
					}
					else
					{
						RenderMeasurementWidth(((RPLSizes)measurement).Width, renderMinWidth: false);
					}
					RenderMeasurementMinWidth(adjustedWidth);
					WriteStream(m_semiColon);
				}
				if (textBoxPropsDef.CanShrink)
				{
					bool noBorders2 = styleContext.NoBorders;
					styleContext.NoBorders = true;
					RenderReportItemStyle((RPLElement)(object)textBox, (RPLElementProps)(object)textBoxProps, (RPLElementPropsDef)(object)textBoxPropsDef, nonSharedStyle, sharedStyle, measurement, styleContext, ref borderContext, ((RPLElementPropsDef)textBoxPropsDef).ID + "s");
					CloseStyle(renderQuote: true);
					WriteStreamCR(m_closeBracket);
					WriteStream(m_openDiv);
					styleContext.IgnoreVerticalAlign = true;
					styleContext.NoBorders = noBorders2;
					styleContext.StyleOnCell = true;
				}
				if (flag8)
				{
					RenderReportItemStyle((RPLElement)(object)textBox, (RPLElementProps)(object)textBoxProps, (RPLElementPropsDef)(object)textBoxPropsDef, nonSharedStyle, sharedStyle, measurement, styleContext, ref borderContext, ((RPLElementPropsDef)textBoxPropsDef).ID + "p");
					styleContext.StyleOnCell = false;
				}
			}
		}
		if (flag8)
		{
			styleContext.IgnoreVerticalAlign = ignoreVerticalAlign;
			CloseStyle(renderQuote: true);
			WriteStreamCR(m_closeBracket);
			WriteStream(m_openTable);
			WriteStream(m_zeroBorder);
			RenderReportLanguage();
			styleContext.RenderMeasurements = false;
			WriteStream(m_closeBracket);
			WriteStream(m_firstTD);
			if (flag10)
			{
				WriteStream(" ROWS='2'");
			}
			RenderAtStart(textBoxProps, (IRPLStyle)(object)style, flag4 && flag9, flag7 && !flag9);
			styleContext.InTablix = true;
		}
		string textBoxClass = GetTextBoxClass(textBoxPropsDef, textBoxProps, nonSharedStyle, ((RPLElementPropsDef)textBoxPropsDef).ID);
		RenderReportItemStyle((RPLElement)(object)textBox, (RPLElementProps)(object)textBoxProps, (RPLElementPropsDef)(object)textBoxPropsDef, nonSharedStyle, sharedStyle, measurement, styleContext, ref borderContext, textBoxClass);
		CloseStyle(renderQuote: true);
		styleContext.IgnoreVerticalAlign = ignoreVerticalAlign;
		if (renderId || flag5 || flag4)
		{
			RenderReportItemId(((RPLElementProps)textBoxProps).UniqueName);
		}
		WriteToolTip((RPLElementProps)(object)textBoxProps);
		if (!flag)
		{
			string language = (string)style[(byte)32];
			RenderLanguage(language);
		}
		WriteStreamCR(m_closeBracket);
		if ((!m_deviceInfo.IsBrowserIE || (m_deviceInfo.BrowserMode == BrowserMode.Standards && !m_deviceInfo.IsBrowserIE6Or7StandardsMode && !flag10)) && isSimple && !string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(text.Trim()))
		{
			WriteStream(m_openDiv);
			if (measurement != null)
			{
				OpenStyle();
				float num = GetInnerContainerWidth((RPLMeasurement)(object)measurement, (IRPLStyle)(object)((RPLElementProps)textBoxProps).Style);
				if (flag4 && !flag9)
				{
					num -= 4.233333f;
				}
				if (num > 0f)
				{
					WriteStream(m_styleWidth);
					WriteRSStream(num);
					WriteStream(m_semiColon);
				}
				CloseStyle(renderQuote: true);
			}
			WriteStream(m_closeBracket);
		}
		if (flag5 && isSimple)
		{
			RenderToggleImage(textBoxProps);
		}
		RPLAction val = null;
		if (HasAction(actionInfo))
		{
			val = actionInfo.Actions[0];
			RenderElementHyperlinkAllTextStyles(((RPLElementProps)textBoxProps).Style, val, ((RPLElementPropsDef)textBoxPropsDef).ID + "a");
			flag6 = true;
			if (flag)
			{
				WriteStream(m_openDiv);
				OpenStyle();
				float num2 = 0f;
				if (measurement != null)
				{
					num2 = ((RPLSizes)measurement).Height;
				}
				if (num2 > 0f)
				{
					num2 = GetInnerContainerHeightSubtractBorders(measurement, (IRPLStyle)(object)((RPLElementProps)textBoxProps).Style);
					if (m_deviceInfo.IsBrowserIE && m_deviceInfo.BrowserMode == BrowserMode.Quirks)
					{
						RenderMeasurementHeight(num2);
					}
					else
					{
						RenderMeasurementMinHeight(num2);
					}
				}
				WriteStream(m_cursorHand);
				WriteStream(m_semiColon);
				CloseStyle(renderQuote: true);
				WriteStream(m_closeBracket);
			}
		}
		RenderTextBoxContent(textBox, textBoxProps, textBoxPropsDef, text, actionStyle, flag5 || flag4, measurement, val);
		if (flag6)
		{
			if (flag)
			{
				WriteStream(m_closeDiv);
			}
			WriteStream(m_closeA);
		}
		if ((!m_deviceInfo.IsBrowserIE || (m_deviceInfo.BrowserMode == BrowserMode.Standards && !m_deviceInfo.IsBrowserIE6Or7StandardsMode && !flag10)) && isSimple && !string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(text.Trim()))
		{
			WriteStream(m_closeDiv);
		}
		if (flag8)
		{
			RenderAtEnd(textBoxProps, (IRPLStyle)(object)style, flag4 && !flag9, flag7 && flag9);
			WriteStream(m_lastTD);
			WriteStream(m_closeTable);
		}
		if (flag)
		{
			if (m_deviceInfo.IsBrowserGeckoEngine)
			{
				WriteStream(m_closeDiv);
			}
			WriteStream(m_lastTD);
			WriteStream(m_closeTable);
			return;
		}
		if (textBoxPropsDef.CanGrow)
		{
			if (textBoxPropsDef.CanShrink)
			{
				if (flag2)
				{
					WriteStream(m_lastTD);
					WriteStream(m_closeTable);
				}
				else
				{
					WriteStream(m_closeDiv);
				}
			}
			WriteStream(m_lastTD);
			WriteStreamCR(m_closeTable);
		}
		else
		{
			if (flag2)
			{
				WriteStream(m_lastTD);
				WriteStream(m_closeTable);
			}
			if (flag3)
			{
				WriteStream(m_closeDiv);
			}
			WriteStreamCR(m_closeDiv);
		}
		if (flag11)
		{
			WriteStream(m_closeDiv);
		}
	}

	private string GetTextBoxClass(RPLTextBoxPropsDef textBoxPropsDef, RPLTextBoxProps textBoxProps, RPLStyleProps nonSharedStyle, string defaultClass)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		if (textBoxPropsDef.SharedTypeCode == TypeCode.Object && (nonSharedStyle == null || nonSharedStyle.Count == 0 || nonSharedStyle[(byte)25] == null))
		{
			object obj = ((RPLElementProps)textBoxProps).Style[(byte)25];
			if (obj != null && (int)(TextAlignments)obj == 0)
			{
				if (GetTextAlignForType(textBoxProps))
				{
					return defaultClass + "r";
				}
				return defaultClass + "l";
			}
		}
		return defaultClass;
	}

	private void WriteToolTip(RPLElementProps props)
	{
		RPLItemProps val = (RPLItemProps)(object)((props is RPLItemProps) ? props : null);
		RPLElementPropsDef definition = ((RPLElementProps)val).Definition;
		RPLItemPropsDef val2 = (RPLItemPropsDef)(object)((definition is RPLItemPropsDef) ? definition : null);
		string toolTip = val.ToolTip;
		if (toolTip == null)
		{
			toolTip = val2.ToolTip;
		}
		if (toolTip != null)
		{
			WriteToolTipAttribute(toolTip);
		}
	}

	private void WriteToolTipAttribute(string tooltip)
	{
		WriteAttrEncoded(m_alt, tooltip);
		WriteAttrEncoded(m_title, tooltip);
	}

	private void WriteOuterConsolidation(Rectangle consolidationOffsets, Sizings sizing, string propsUniqueName)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Expected I4, but got Unknown
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		bool flag = false;
		switch (sizing - 1)
		{
		case 0:
			WriteStream(" imgConDiv=\"true\"");
			m_emitImageConsolidationScaling = true;
			flag = true;
			break;
		case 1:
			WriteStream(" imgConFitProp=\"true\"");
			break;
		}
		if (m_deviceInfo.AllowScript)
		{
			if (m_imgConImageIdsStream == null)
			{
				CreateImgConImageIdsStream();
			}
			WriteIdToSecondaryStream(m_imgConImageIdsStream, propsUniqueName + "_ici");
			RenderReportItemId(propsUniqueName + "_ici");
		}
		WriteStream(" imgConImage=\"" + ((object)sizing).ToString() + "\"");
		if (flag)
		{
			WriteStream(" imgConWidth=\"" + consolidationOffsets.Width + "\"");
			WriteStream(" imgConHeight=\"" + consolidationOffsets.Height + "\"");
		}
		OpenStyle();
		WriteStream(m_styleWidth);
		if (flag)
		{
			WriteStream("1");
		}
		else
		{
			WriteStream(consolidationOffsets.Width);
		}
		WriteStream(m_px);
		WriteStream(m_semiColon);
		WriteStream(m_styleHeight);
		if (flag)
		{
			WriteStream("1");
		}
		else
		{
			WriteStream(consolidationOffsets.Height);
		}
		WriteStream(m_px);
		WriteStream(m_semiColon);
		WriteStream(m_overflowHidden);
		WriteStream(m_semiColon);
		if (m_deviceInfo.BrowserMode == BrowserMode.Standards)
		{
			WriteStream(m_stylePositionAbsolute);
		}
	}

	private void WriteClippedDiv(Rectangle clipCoordinates)
	{
		OpenStyle();
		WriteStream(m_styleTop);
		if (clipCoordinates.Top > 0)
		{
			WriteStream("-");
		}
		WriteStream(clipCoordinates.Top);
		WriteStream(m_px);
		WriteStream(m_semiColon);
		WriteStream(m_styleLeft);
		if (clipCoordinates.Left > 0)
		{
			WriteStream("-");
		}
		WriteStream(clipCoordinates.Left);
		WriteStream(m_px);
		WriteStream(m_semiColon);
		WriteStream(m_stylePositionRelative);
		CloseStyle(renderQuote: true);
	}

	protected void RenderNavigationId(string navigationId)
	{
		if (!IsFragment)
		{
			WriteStream(m_openSpan);
			WriteStream(m_id);
			WriteAttrEncoded(m_deviceInfo.HtmlPrefixId);
			WriteStream(navigationId);
			WriteStream(m_closeTag);
		}
	}

	protected void RenderTablix(RPLTablix tablix, RPLElementProps props, RPLElementPropsDef def, RPLItemMeasurement measurement, StyleContext styleContext, ref int borderContext, bool renderId)
	{
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Expected O, but got Unknown
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Expected O, but got Unknown
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Expected O, but got Unknown
		//IL_042c: Unknown result type (might be due to invalid IL or missing references)
		//IL_045a: Unknown result type (might be due to invalid IL or missing references)
		string uniqueName = props.UniqueName;
		TablixFixedHeaderStorage tablixFixedHeaderStorage = new TablixFixedHeaderStorage();
		if (tablix.ColumnWidths == null)
		{
			tablix.ColumnWidths = new float[0];
		}
		if (tablix.RowHeights == null)
		{
			tablix.RowHeights = new float[0];
		}
		bool flag = InitFixedColumnHeaders(tablix, uniqueName, tablixFixedHeaderStorage);
		bool flag2 = InitFixedRowHeaders(tablix, uniqueName, tablixFixedHeaderStorage);
		bool flag3 = tablix.ColumnHeaderRows == 0 && tablix.RowHeaderColumns == 0 && !m_deviceInfo.AccessibleTablix && m_deviceInfo.BrowserMode != BrowserMode.Standards;
		if (flag && flag2)
		{
			tablixFixedHeaderStorage.CornerHeaders = new List<string>();
		}
		WriteStream(m_openTable);
		int columns = ((tablix.ColumnHeaderRows > 0 || tablix.RowHeaderColumns > 0 || !flag3) ? (tablix.ColumnWidths.Length + 1) : tablix.ColumnWidths.Length);
		WriteStream(m_cols);
		WriteStream(columns.ToString(CultureInfo.InvariantCulture));
		WriteStream(m_quote);
		if (renderId || flag || flag2)
		{
			RenderReportItemId(uniqueName);
		}
		WriteToolTip(((RPLElement)tablix).ElementProps);
		WriteStream(m_zeroBorder);
		OpenStyle();
		WriteStream(m_borderCollapse);
		WriteStream(m_semiColon);
		if (m_deviceInfo.OutlookCompat && measurement != null)
		{
			RenderMeasurementWidth(((RPLSizes)measurement).Width, renderMinWidth: true);
		}
		RenderReportItemStyle((RPLElement)(object)tablix, props, def, measurement, styleContext, ref borderContext, def.ID);
		CloseStyle(renderQuote: true);
		WriteStream(m_closeBracket);
		int colsBeforeRowHeaders = tablix.ColsBeforeRowHeaders;
		RPLTablixRow nextRow = tablix.GetNextRow();
		List<RPLTablixOmittedRow> list = new List<RPLTablixOmittedRow>();
		while (nextRow != null && nextRow is RPLTablixOmittedRow)
		{
			list.Add((RPLTablixOmittedRow)nextRow);
			nextRow = tablix.GetNextRow();
		}
		if (flag3)
		{
			RenderEmptyTopTablixRow(tablix, list, uniqueName, emptyCol: false, tablixFixedHeaderStorage);
			RenderSimpleTablixRows(tablix, uniqueName, nextRow, borderContext, tablixFixedHeaderStorage);
		}
		else
		{
			styleContext = new StyleContext();
			float[] columnWidths = tablix.ColumnWidths;
			float[] rowHeights = tablix.RowHeights;
			int num = columnWidths.Length;
			int numRows = rowHeights.Length;
			RenderEmptyTopTablixRow(tablix, list, uniqueName, emptyCol: true, tablixFixedHeaderStorage);
			bool flag4 = flag;
			int num2 = 0;
			list = new List<RPLTablixOmittedRow>();
			HTMLHeader[] array = null;
			string[] array2 = null;
			OmittedHeaderStack omittedHeaders = null;
			if (m_deviceInfo.AccessibleTablix)
			{
				array = new HTMLHeader[tablix.RowHeaderColumns];
				array2 = new string[num];
				omittedHeaders = new OmittedHeaderStack();
			}
			while (nextRow != null)
			{
				if (nextRow is RPLTablixOmittedRow)
				{
					list.Add((RPLTablixOmittedRow)nextRow);
					nextRow = tablix.GetNextRow();
					continue;
				}
				if (rowHeights[num2] == 0f && num2 > 1 && nextRow.NumCells == 1 && nextRow[0].Element is RPLRectangle)
				{
					RPLRectangle val = (RPLRectangle)nextRow[0].Element;
					if (((RPLContainer)val).Children == null || ((RPLContainer)val).Children.Length == 0)
					{
						nextRow = tablix.GetNextRow();
						num2++;
						continue;
					}
				}
				WriteStream(m_openTR);
				if (tablix.FixedRow(num2) || flag2 || flag4)
				{
					string text = uniqueName + "r" + num2;
					RenderReportItemId(text);
					if (tablix.FixedRow(num2))
					{
						tablixFixedHeaderStorage.ColumnHeaders.Add(text);
						if (tablixFixedHeaderStorage.CornerHeaders != null)
						{
							tablixFixedHeaderStorage.CornerHeaders.Add(text);
						}
					}
					else if (flag4)
					{
						tablixFixedHeaderStorage.BodyID = text;
						flag4 = false;
					}
					if (flag2)
					{
						tablixFixedHeaderStorage.RowHeaders.Add(text);
					}
				}
				WriteStream(m_valign);
				WriteStream(m_topValue);
				WriteStream(m_quote);
				WriteStream(m_closeBracket);
				RenderEmptyHeightCell(rowHeights[num2], uniqueName, tablix.FixedRow(num2), num2, tablixFixedHeaderStorage);
				int num3 = 0;
				int numCells = nextRow.NumCells;
				int num4 = numCells;
				if (nextRow.BodyStart == -1)
				{
					int[] omittedIndices = new int[list.Count];
					for (int i = num3; i < num4; i++)
					{
						RPLTablixCell val2 = nextRow[i];
						RenderColumnHeaderTablixCell(tablix, uniqueName, num, val2.ColIndex, val2.ColSpan, num2, borderContext, val2, styleContext, tablixFixedHeaderStorage, list, omittedIndices);
						if (array2 != null && num2 < tablix.ColumnHeaderRows)
						{
							string text2 = null;
							if (val2 is RPLTablixMemberCell)
							{
								text2 = ((RPLTablixMemberCell)val2).UniqueName;
								if (text2 == null && val2.Element != null)
								{
									text2 = ((RPLElement)val2.Element).ElementProps.UniqueName;
									((RPLTablixMemberCell)val2).UniqueName = text2;
								}
								if (text2 == null)
								{
									continue;
								}
								for (int j = 0; j < val2.ColSpan; j++)
								{
									string text3 = array2[val2.ColIndex + j];
									text3 = ((text3 != null) ? (text3 + " " + HttpUtility.HtmlAttributeEncode(m_deviceInfo.HtmlPrefixId) + text2) : (HttpUtility.HtmlAttributeEncode(m_deviceInfo.HtmlPrefixId) + text2));
									array2[val2.ColIndex + j] = text3;
								}
							}
						}
						nextRow[i] = null;
					}
					list = new List<RPLTablixOmittedRow>();
				}
				else
				{
					if (array != null)
					{
						int headerStart = nextRow.HeaderStart;
						int num5 = 0;
						for (int k = 0; k < array.Length; k++)
						{
							HTMLHeader hTMLHeader = array[k];
							if (array[k] == null)
							{
								hTMLHeader = (array[k] = new HTMLHeader());
							}
							else if (array[k].Span > 1)
							{
								array[k].Span--;
								continue;
							}
							RPLTablixCell val3 = nextRow[num5 + headerStart];
							hTMLHeader.ID = CalculateRowHeaderId(val3, tablix.FixedColumns[val3.ColIndex], uniqueName, num2, k + tablix.ColsBeforeRowHeaders, null, m_deviceInfo.AccessibleTablix, fixedCornerHeader: false);
							hTMLHeader.Span = val3.RowSpan;
							num5++;
						}
					}
					if (list != null && list.Count > 0)
					{
						for (int l = 0; l < list.Count; l++)
						{
							RenderTablixOmittedRow(columns, (RPLTablixRow)(object)list[l]);
						}
						list = null;
					}
					List<RPLTablixMemberCell> omittedHeaders2 = nextRow.OmittedHeaders;
					if (colsBeforeRowHeaders > 0)
					{
						int omittedIndex = 0;
						int headerStart2 = nextRow.HeaderStart;
						int bodyStart = nextRow.BodyStart;
						int m = headerStart2;
						int n = bodyStart;
						int num6 = 0;
						for (; n < num4; n++)
						{
							if (num6 >= colsBeforeRowHeaders)
							{
								break;
							}
							RPLTablixCell val4 = nextRow[n];
							int colSpan = val4.ColSpan;
							RenderTablixCell(tablix, fixedHeader: false, uniqueName, num, numRows, num6, colSpan, num2, borderContext, val4, omittedHeaders2, ref omittedIndex, styleContext, tablixFixedHeaderStorage, array, array2, omittedHeaders);
							num6 += colSpan;
							nextRow[n] = null;
						}
						num4 = ((bodyStart > headerStart2) ? bodyStart : num4);
						if (m >= 0)
						{
							for (; m < num4; m++)
							{
								RPLTablixCell val5 = nextRow[m];
								int colSpan2 = val5.ColSpan;
								RenderTablixCell(tablix, flag2, uniqueName, num, numRows, num6, colSpan2, num2, borderContext, val5, omittedHeaders2, ref omittedIndex, styleContext, tablixFixedHeaderStorage, array, array2, omittedHeaders);
								num6 += colSpan2;
								nextRow[m] = null;
							}
						}
						num3 = n;
						num4 = ((bodyStart < headerStart2) ? headerStart2 : numCells);
						for (int num7 = num3; num7 < num4; num7++)
						{
							RPLTablixCell val6 = nextRow[num7];
							RenderTablixCell(tablix, fixedHeader: false, uniqueName, num, numRows, val6.ColIndex, val6.ColSpan, num2, borderContext, val6, omittedHeaders2, ref omittedIndex, styleContext, tablixFixedHeaderStorage, array, array2, omittedHeaders);
							nextRow[num7] = null;
						}
					}
					else
					{
						int omittedIndex2 = 0;
						for (int num8 = num3; num8 < num4; num8++)
						{
							RPLTablixCell val7 = nextRow[num8];
							int colIndex = val7.ColIndex;
							RenderTablixCell(tablix, tablix.FixedColumns[colIndex], uniqueName, num, numRows, colIndex, val7.ColSpan, num2, borderContext, val7, omittedHeaders2, ref omittedIndex2, styleContext, tablixFixedHeaderStorage, array, array2, omittedHeaders);
							nextRow[num8] = null;
						}
					}
				}
				WriteStream(m_closeTR);
				nextRow = tablix.GetNextRow();
				num2++;
			}
		}
		WriteStream(m_closeTable);
		if (flag || flag2)
		{
			if (m_fixedHeaders == null)
			{
				m_fixedHeaders = new ArrayList();
			}
			m_fixedHeaders.Add(tablixFixedHeaderStorage);
		}
	}

	private void RenderTablixOmittedRow(int columns, RPLTablixRow currentRow)
	{
		int i = 0;
		List<RPLTablixMemberCell> omittedHeaders;
		for (omittedHeaders = currentRow.OmittedHeaders; i < omittedHeaders.Count && omittedHeaders[i].GroupLabel == null; i++)
		{
		}
		if (i >= omittedHeaders.Count)
		{
			return;
		}
		int num = ((RPLTablixCell)omittedHeaders[i]).ColIndex;
		WriteStream(m_openTR);
		WriteStream(m_zeroHeight);
		WriteStream(m_closeBracket);
		WriteStream(m_openTD);
		WriteStream(m_colSpan);
		WriteStream(num.ToString(CultureInfo.InvariantCulture));
		WriteStream(m_quote);
		WriteStream(m_closeBracket);
		WriteStream(m_closeTD);
		for (; i < omittedHeaders.Count; i++)
		{
			if (omittedHeaders[i].GroupLabel != null)
			{
				WriteStream(m_openTD);
				int colIndex = ((RPLTablixCell)omittedHeaders[i]).ColIndex;
				int num2 = colIndex - num;
				if (num2 > 1)
				{
					WriteStream(m_colSpan);
					WriteStream(num2.ToString(CultureInfo.InvariantCulture));
					WriteStream(m_quote);
					WriteStream(m_closeBracket);
					WriteStream(m_closeTD);
					WriteStream(m_openTD);
				}
				int colSpan = ((RPLTablixCell)omittedHeaders[i]).ColSpan;
				if (colSpan > 1)
				{
					WriteStream(m_colSpan);
					WriteStream(colSpan.ToString(CultureInfo.InvariantCulture));
					WriteStream(m_quote);
				}
				RenderReportItemId(omittedHeaders[i].UniqueName);
				WriteStream(m_closeBracket);
				WriteStream(m_closeTD);
				num = colIndex + colSpan;
			}
		}
		if (num < columns)
		{
			WriteStream(m_openTD);
			WriteStream(m_colSpan);
			WriteStream((columns - num).ToString(CultureInfo.InvariantCulture));
			WriteStream(m_quote);
			WriteStream(m_closeBracket);
			WriteStream(m_closeTD);
		}
		WriteStream(m_closeTR);
	}

	protected void RenderSimpleTablixRows(RPLTablix tablix, string tablixID, RPLTablixRow currentRow, int borderContext, TablixFixedHeaderStorage headerStorage)
	{
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Expected O, but got Unknown
		int num = 0;
		StyleContext styleContext = new StyleContext();
		float[] rowHeights = tablix.RowHeights;
		int num2 = tablix.ColumnWidths.Length;
		int num3 = rowHeights.Length;
		bool flag = headerStorage.ColumnHeaders != null;
		SharedListLayoutState sharedListLayoutState = SharedListLayoutState.None;
		while (currentRow != null)
		{
			List<RPLTablixMemberCell> omittedHeaders = currentRow.OmittedHeaders;
			int omittedIndex = 0;
			if (num2 == 1)
			{
				sharedListLayoutState = SharedListLayoutState.None;
				bool flag2 = tablix.SharedLayoutRow(num);
				bool flag3 = tablix.UseSharedLayoutRow(num);
				bool flag4 = tablix.RowsState.Length > num + 1 && tablix.UseSharedLayoutRow(num + 1);
				if (flag2 && flag4)
				{
					sharedListLayoutState = SharedListLayoutState.Start;
				}
				else if (flag3)
				{
					sharedListLayoutState = ((!flag4) ? SharedListLayoutState.End : SharedListLayoutState.Continue);
				}
			}
			if (sharedListLayoutState == SharedListLayoutState.None || sharedListLayoutState == SharedListLayoutState.Start)
			{
				if (rowHeights[num] == 0f && num > 1 && currentRow.NumCells == 1 && currentRow[0].Element is RPLRectangle)
				{
					RPLRectangle val = (RPLRectangle)currentRow[0].Element;
					if (((RPLContainer)val).Children == null || ((RPLContainer)val).Children.Length == 0)
					{
						currentRow = tablix.GetNextRow();
						num++;
						continue;
					}
				}
				WriteStream(m_openTR);
				if (tablix.FixedRow(num) || headerStorage.RowHeaders != null || flag)
				{
					string text = tablixID + "tr" + num;
					RenderReportItemId(text);
					if (tablix.FixedRow(num))
					{
						headerStorage.ColumnHeaders.Add(text);
						if (headerStorage.CornerHeaders != null)
						{
							headerStorage.CornerHeaders.Add(text);
						}
					}
					else if (flag)
					{
						headerStorage.BodyID = text;
						flag = false;
					}
					if (headerStorage.RowHeaders != null)
					{
						headerStorage.RowHeaders.Add(text);
					}
				}
				WriteStream(m_valign);
				WriteStream(m_topValue);
				WriteStream(m_quote);
				WriteStream(m_closeBracket);
			}
			int numCells = currentRow.NumCells;
			bool firstRow = num == 0;
			bool lastRow = num == num3 - 1;
			RPLTablixCell val2 = currentRow[0];
			currentRow[0] = null;
			if (sharedListLayoutState != SharedListLayoutState.None)
			{
				RenderListReportItem(tablix, val2, omittedHeaders, borderContext, styleContext, firstRow, lastRow, sharedListLayoutState, (RPLElement)(object)val2.Element);
			}
			else
			{
				RenderSimpleTablixCellWithHeight(rowHeights[num], tablix, tablixID, num2, num, borderContext, val2, omittedHeaders, ref omittedIndex, styleContext, firstRow, lastRow, headerStorage);
			}
			int i;
			for (i = 1; i < numCells - 1; i++)
			{
				val2 = currentRow[i];
				RenderSimpleTablixCell(tablix, tablixID, val2.ColSpan, num, borderContext, val2, omittedHeaders, ref omittedIndex, lastCol: false, firstRow, lastRow, headerStorage);
				currentRow[i] = null;
			}
			if (numCells > 1)
			{
				val2 = currentRow[i];
				RenderSimpleTablixCell(tablix, tablixID, val2.ColSpan, num, borderContext, val2, omittedHeaders, ref omittedIndex, lastCol: true, firstRow, lastRow, headerStorage);
				currentRow[i] = null;
			}
			if (sharedListLayoutState == SharedListLayoutState.None || sharedListLayoutState == SharedListLayoutState.End)
			{
				WriteStream(m_closeTR);
			}
			currentRow = tablix.GetNextRow();
			num++;
		}
	}

	private void RenderSimpleTablixCellWithHeight(float height, RPLTablix tablix, string tablixID, int numCols, int row, int tablixContext, RPLTablixCell cell, List<RPLTablixMemberCell> omittedCells, ref int omittedIndex, StyleContext styleContext, bool firstRow, bool lastRow, TablixFixedHeaderStorage headerStorage)
	{
		int colIndex = cell.ColIndex;
		int colSpan = cell.ColSpan;
		bool lastCol = colIndex + colSpan == numCols;
		bool zeroWidth = styleContext.ZeroWidth;
		float columnWidth = tablix.GetColumnWidth(colIndex, colSpan);
		styleContext.ZeroWidth = columnWidth == 0f;
		int startIndex = RenderZeroWidthTDsForTablix(colIndex, colSpan, tablix);
		colSpan = GetColSpanMinusZeroWidthColumns(colIndex, colSpan, tablix);
		WriteStream(m_openTD);
		RenderSimpleTablixCellID(tablix, tablixID, row, headerStorage, colIndex);
		if (colSpan > 1)
		{
			WriteStream(m_colSpan);
			WriteStream(colSpan.ToString(CultureInfo.InvariantCulture));
			WriteStream(m_quote);
		}
		OpenStyle();
		WriteStream(m_styleHeight);
		WriteDStream(height);
		WriteStream(m_mm);
		RPLElement element = (RPLElement)(object)cell.Element;
		if (element != null)
		{
			WriteStream(m_semiColon);
			int borderContext = 0;
			RenderTablixReportItemStyle(tablix, tablixContext, cell, styleContext, firstCol: true, lastCol, firstRow, lastRow, element, ref borderContext);
			RenderTablixOmittedHeaderCells(omittedCells, colIndex, lastCol, ref omittedIndex);
			RenderTablixReportItem(tablix, tablixContext, cell, styleContext, firstCol: true, lastCol, firstRow, lastRow, element, ref borderContext);
		}
		else
		{
			if (styleContext.ZeroWidth)
			{
				WriteStream(m_displayNone);
			}
			CloseStyle(renderQuote: true);
			WriteStream(m_closeBracket);
			RenderTablixOmittedHeaderCells(omittedCells, colIndex, lastCol, ref omittedIndex);
			WriteStream(m_nbsp);
		}
		WriteStream(m_closeTD);
		RenderZeroWidthTDsForTablix(startIndex, colSpan, tablix);
		styleContext.ZeroWidth = zeroWidth;
	}

	private void RenderTablixReportItemStyle(RPLTablix tablix, int tablixContext, RPLTablixCell cell, StyleContext styleContext, bool firstCol, bool lastCol, bool firstRow, bool lastRow, RPLElement cellItem, ref int borderContext)
	{
		RPLElementProps elementProps1 = cellItem.ElementProps;
		RPLElementPropsDef definition1 = elementProps1.Definition;
		RPLTextBoxProps elementProps2 = cellItem is RPLTextBox rplTextBox ? ((RPLElement)rplTextBox).ElementProps as RPLTextBoxProps : (RPLTextBoxProps)null;
		RPLTextBoxPropsDef definition2 = rplTextBox != null ? elementProps1.Definition as RPLTextBoxPropsDef : (RPLTextBoxPropsDef)null;
		styleContext.OmitBordersState = cell.ElementState;
		if (!(cellItem is RPLLine))
		{
			styleContext.StyleOnCell = true;
			borderContext = HTML4Renderer.GetNewContext(tablixContext, firstCol, lastCol, firstRow, lastRow);
			if (rplTextBox != null)
			{
				bool ignorePadding = styleContext.IgnorePadding;
				styleContext.IgnorePadding = true;
				RPLItemMeasurement measurement = (RPLItemMeasurement)null;
				if (this.m_deviceInfo.OutlookCompat || !this.m_deviceInfo.IsBrowserIE)
				{
					measurement = new RPLItemMeasurement();
					((RPLSizes)measurement).Width = tablix.GetColumnWidth(cell.ColIndex, cell.ColSpan);
				}
				styleContext.EmptyTextBox = definition2.IsSimple && string.IsNullOrEmpty(elementProps2.Value) && string.IsNullOrEmpty(definition2.Value) && !this.NeedSharedToggleParent(elementProps2) && !this.CanSort(definition2);
				string textBoxClass = this.GetTextBoxClass(definition2, elementProps2, ((RPLElementProps)elementProps2).NonSharedStyle, definition1.ID + "c");
				bool backgroundBorders = styleContext.OnlyRenderMeasurementsBackgroundBorders;
				if (HTML4Renderer.IsWritingModeVertical((IRPLStyle)((RPLElementProps)elementProps2).Style) && this.m_deviceInfo.IsBrowserIE && (definition2.CanGrow || this.m_deviceInfo.BrowserMode == BrowserMode.Standards && !this.m_deviceInfo.IsBrowserIE6Or7StandardsMode))
					styleContext.OnlyRenderMeasurementsBackgroundBorders = true;
				this.RenderReportItemStyle(cellItem, elementProps1, definition1, ((RPLElementProps)elementProps2).NonSharedStyle, ((RPLElementPropsDef)definition2).SharedStyle, measurement, styleContext, ref borderContext, textBoxClass);
				styleContext.OnlyRenderMeasurementsBackgroundBorders = backgroundBorders;
				styleContext.IgnorePadding = ignorePadding;
			}
			else
				this.RenderReportItemStyle(cellItem, elementProps1, definition1, (RPLItemMeasurement)null, styleContext, ref borderContext, definition1.ID + "c");
			styleContext.StyleOnCell = false;
		}
		else if (styleContext.ZeroWidth)
			this.WriteStream(HTML4Renderer.m_displayNone);
		this.CloseStyle(true);
		if (styleContext.EmptyTextBox && rplTextBox != null && elementProps1 != null)
			this.WriteToolTip(elementProps1);
		this.WriteStream(HTML4Renderer.m_closeBracket);
	}

	private void RenderTablixReportItem(RPLTablix tablix, int tablixContext, RPLTablixCell cell, StyleContext styleContext, bool firstCol, bool lastCol, bool firstRow, bool lastRow, RPLElement cellItem, ref int borderContext)
	{
		RPLElementProps elementProps1 = cellItem.ElementProps;
		RPLElementPropsDef definition1 = elementProps1.Definition;
		RPLTextBoxProps elementProps2 = cellItem is RPLTextBox textBox ? ((RPLElement)textBox).ElementProps as RPLTextBoxProps : (RPLTextBoxProps)null;
		RPLTextBoxPropsDef definition2 = textBox != null ? elementProps1.Definition as RPLTextBoxPropsDef : (RPLTextBoxPropsDef)null;
		RPLItemMeasurement measurement = new RPLItemMeasurement();
		styleContext.OmitBordersState = cell.ElementState;
		if (styleContext.EmptyTextBox)
		{
			bool flag = false;
			RPLActionInfo actionInfo = elementProps2.ActionInfo;
			if (this.HasAction(actionInfo))
			{
				this.RenderElementHyperlinkAllTextStyles(((RPLElementProps)elementProps2).Style, actionInfo.Actions[0], ((RPLElementPropsDef)definition2).ID + "a");
				this.WriteStream(HTML4Renderer.m_openDiv);
				this.OpenStyle();
				((RPLSizes)measurement).Height = tablix.GetRowHeight(cell.RowIndex, cell.RowSpan);
				((RPLSizes)measurement).Height = this.GetInnerContainerHeightSubtractBorders(measurement, (IRPLStyle)((RPLElementProps)elementProps2).Style);
				if (this.m_deviceInfo.BrowserMode == BrowserMode.Quirks && this.m_deviceInfo.IsBrowserIE)
					this.RenderMeasurementHeight(((RPLSizes)measurement).Height);
				else
					this.RenderMeasurementMinHeight(((RPLSizes)measurement).Height);
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
				((RPLSizes)measurement).Width = tablix.GetColumnWidth(cell.ColIndex, cell.ColSpan);
				((RPLSizes)measurement).Height = tablix.GetRowHeight(cell.RowIndex, cell.RowSpan);
				this.RenderTextBoxPercent(textBox, elementProps2, definition2, measurement, styleContext, renderId);
			}
			else
			{
				((RPLSizes)measurement).Width = tablix.GetColumnWidth(cell.ColIndex, cell.ColSpan);
				((RPLSizes)measurement).Height = tablix.GetRowHeight(cell.RowIndex, cell.RowSpan);
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

	private void RenderListReportItem(RPLTablix tablix, RPLTablixCell cell, List<RPLTablixMemberCell> omittedHeaders, int tablixContext, StyleContext styleContext, bool firstRow, bool lastRow, SharedListLayoutState layoutState, RPLElement cellItem)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected O, but got Unknown
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Expected O, but got Unknown
		RPLElementProps elementProps = cellItem.ElementProps;
		RPLElementPropsDef definition = elementProps.Definition;
		RPLItemMeasurement val = null;
		val = new RPLItemMeasurement();
		((RPLSizes)val).Width = tablix.ColumnWidths[0];
		((RPLSizes)val).Height = tablix.GetRowHeight(cell.RowIndex, cell.RowSpan);
		((RPLMeasurement)val).State = cell.ElementState;
		bool zeroWidth = styleContext.ZeroWidth;
		styleContext.ZeroWidth = ((RPLSizes)val).Width == 0f;
		if (layoutState == SharedListLayoutState.Start)
		{
			WriteStream(m_openTD);
			if (styleContext.ZeroWidth)
			{
				OpenStyle();
				WriteStream(m_displayNone);
				CloseStyle(renderQuote: true);
			}
			WriteStream(m_closeBracket);
		}
		if (cellItem is RPLRectangle)
		{
			int num = tablix.ColumnWidths.Length;
			int colIndex = cell.ColIndex;
			int colSpan = cell.ColSpan;
			bool right = colIndex + colSpan == num;
			int newContext = GetNewContext(tablixContext, left: true, right, firstRow, lastRow);
			RenderListRectangle((RPLContainer)(RPLRectangle)cellItem, omittedHeaders, val, elementProps, definition, layoutState, newContext);
			if (layoutState == SharedListLayoutState.End)
			{
				WriteStream(m_closeTD);
			}
		}
		else
		{
			int omittedIndex = 0;
			RenderTablixOmittedHeaderCells(omittedHeaders, 0, lastCol: true, ref omittedIndex);
			RenderReportItem(cellItem, elementProps, definition, val, styleContext, 0, NeedReportItemId(cellItem, elementProps));
			styleContext.Reset();
			if (layoutState == SharedListLayoutState.End)
			{
				WriteStream(m_closeTD);
			}
		}
		styleContext.ZeroWidth = zeroWidth;
	}

	protected void RenderListRectangle(RPLContainer rectangle, List<RPLTablixMemberCell> omittedHeaders, RPLItemMeasurement measurement, RPLElementProps props, RPLElementPropsDef def, SharedListLayoutState layoutState, int borderContext)
	{
		RPLItemMeasurement[] children = rectangle.Children;
		GenerateHTMLTable(children, ((RPLSizes)measurement).Top, ((RPLSizes)measurement).Left, ((RPLSizes)measurement).Width, ((RPLSizes)measurement).Height, borderContext, expandLayout: false, layoutState, omittedHeaders, (IRPLStyle)(object)props.Style);
	}

	private void RenderSimpleTablixCell(RPLTablix tablix, string tablixID, int colSpan, int row, int tablixContext, RPLTablixCell cell, List<RPLTablixMemberCell> omittedCells, ref int omittedIndex, bool lastCol, bool firstRow, bool lastRow, TablixFixedHeaderStorage headerStorage)
	{
		StyleContext styleContext = new StyleContext();
		int colIndex = cell.ColIndex;
		bool zeroWidth = styleContext.ZeroWidth;
		float columnWidth = tablix.GetColumnWidth(colIndex, cell.ColSpan);
		styleContext.ZeroWidth = columnWidth == 0f;
		int startIndex = RenderZeroWidthTDsForTablix(colIndex, colSpan, tablix);
		colSpan = GetColSpanMinusZeroWidthColumns(colIndex, colSpan, tablix);
		WriteStream(m_openTD);
		RenderSimpleTablixCellID(tablix, tablixID, row, headerStorage, colIndex);
		if (colSpan > 1)
		{
			WriteStream(m_colSpan);
			WriteStream(colSpan.ToString(CultureInfo.InvariantCulture));
			WriteStream(m_quote);
		}
		RPLElement element = (RPLElement)(object)cell.Element;
		if (element != null)
		{
			int borderContext = 0;
			RenderTablixReportItemStyle(tablix, tablixContext, cell, styleContext, firstCol: false, lastCol, firstRow, lastRow, element, ref borderContext);
			RenderTablixOmittedHeaderCells(omittedCells, colIndex, lastCol, ref omittedIndex);
			RenderTablixReportItem(tablix, tablixContext, cell, styleContext, firstCol: false, lastCol, firstRow, lastRow, element, ref borderContext);
		}
		else
		{
			if (styleContext.ZeroWidth)
			{
				OpenStyle();
				WriteStream(m_displayNone);
				CloseStyle(renderQuote: true);
			}
			WriteStream(m_closeBracket);
			WriteStream(m_nbsp);
			RenderTablixOmittedHeaderCells(omittedCells, colIndex, lastCol, ref omittedIndex);
		}
		WriteStream(m_closeTD);
		RenderZeroWidthTDsForTablix(startIndex, colSpan, tablix);
		styleContext.ZeroWidth = zeroWidth;
	}

	private int GetColSpanMinusZeroWidthColumns(int startColIndex, int colSpan, RPLTablix tablix)
	{
		int num = colSpan;
		for (int i = startColIndex; i < startColIndex + colSpan; i++)
		{
			if (tablix.ColumnWidths[i] == 0f)
			{
				num--;
			}
		}
		return num;
	}

	private int RenderZeroWidthTDsForTablix(int startIndex, int colSpan, RPLTablix tablix)
	{
		int i;
		for (i = startIndex; i < startIndex + colSpan && tablix.ColumnWidths[i] == 0f; i++)
		{
			WriteStream(m_openTD);
			OpenStyle();
			WriteStream(m_displayNone);
			CloseStyle(renderQuote: true);
			WriteStream(m_closeBracket);
			WriteStream(m_closeTD);
		}
		return i;
	}

	private void RenderSimpleTablixCellID(RPLTablix tablix, string tablixID, int row, TablixFixedHeaderStorage headerStorage, int col)
	{
		if (tablix.FixedColumns[col])
		{
			string text = tablixID + "r" + row + "c" + col;
			RenderReportItemId(text);
			headerStorage.RowHeaders.Add(text);
			if (headerStorage.CornerHeaders != null && tablix.FixedRow(row))
			{
				headerStorage.CornerHeaders.Add(text);
			}
		}
	}

	protected void RenderMultiLineTextWithHits(string text, List<int> hits)
	{
		if (text == null)
		{
			return;
		}
		int num = 0;
		int startPos = 0;
		int currentHitIndex = 0;
		int length = text.Length;
		for (int i = 0; i < length; i++)
		{
			switch (text[i])
			{
			case '\r':
				RenderTextWithHits(text, startPos, num, hits, ref currentHitIndex);
				startPos = num + 1;
				break;
			case '\n':
				RenderTextWithHits(text, startPos, num, hits, ref currentHitIndex);
				WriteStreamCR(m_br);
				startPos = num + 1;
				break;
			}
			num++;
		}
		RenderTextWithHits(text, startPos, num, hits, ref currentHitIndex);
	}

	protected void RenderTextWithHits(string text, int startPos, int endPos, List<int> hitIndices, ref int currentHitIndex)
	{
		int length = m_searchText.Length;
		while (currentHitIndex < hitIndices.Count && hitIndices[currentHitIndex] < endPos)
		{
			int num = hitIndices[currentHitIndex];
			string theString = text.Substring(startPos, num - startPos);
			WriteStreamEncoded(theString);
			theString = text.Substring(num, length);
			OutputFindString(theString, 0);
			startPos = num + length;
			currentHitIndex++;
			m_currentHitCount++;
		}
		if (startPos <= endPos)
		{
			string theString = text.Substring(startPos, endPos - startPos);
			WriteStreamEncoded(theString);
		}
	}

	private void OutputFindString(string findString, int offset)
	{
		WriteStream(m_openSpan);
		WriteStream(m_id);
		WriteAttrEncoded(m_deviceInfo.HtmlPrefixId);
		WriteStream(m_searchHitIdPrefix);
		WriteStream(m_currentHitCount.ToString(CultureInfo.InvariantCulture));
		if (offset > 0)
		{
			WriteStream("_");
			WriteStream(offset.ToString(CultureInfo.InvariantCulture));
		}
		WriteStream(m_quote);
		if (m_currentHitCount == 0)
		{
			if (m_deviceInfo.IsBrowserSafari)
			{
				WriteStream(" style=\"COLOR:black;BACKGROUND-COLOR:#B5D4FE;\">");
			}
			else
			{
				WriteStream(" style=\"COLOR:highlighttext;BACKGROUND-COLOR:highlight;\">");
			}
		}
		else
		{
			WriteStream(m_closeBracket);
		}
		WriteStreamEncoded(findString);
		WriteStream(m_closeSpan);
	}

	private bool IsImageNotFitProportional(RPLElement reportItem, RPLElementPropsDef definition)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Invalid comparison between Unknown and I4
		RPLImagePropsDef val = null;
		if (definition is RPLImagePropsDef)
		{
			val = (RPLImagePropsDef)definition;
		}
		if (reportItem is RPLImage && val != null)
		{
			return (int)val.Sizing != 2;
		}
		return false;
	}

	protected void RenderImage(RPLImage image, RPLImageProps imageProps, RPLImagePropsDef imagePropsDef, RPLItemMeasurement measurement, ref int borderContext, bool renderId)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Invalid comparison between Unknown and I4
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Invalid comparison between Unknown and I4
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Invalid comparison between Unknown and I4
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Invalid comparison between Unknown and I4
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Invalid comparison between Unknown and I4
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Invalid comparison between Unknown and I4
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Invalid comparison between Unknown and I4
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Invalid comparison between Unknown and I4
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Invalid comparison between Unknown and I4
		//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Invalid comparison between Unknown and I4
		bool flag = false;
		bool flag2 = false;
		RPLImageData image2 = imageProps.Image;
		RPLActionInfo actionInfo = imageProps.ActionInfo;
		StyleContext styleContext = new StyleContext();
		Sizings sizing = imagePropsDef.Sizing;
		bool flag3 = false;
		if ((int)sizing == 0)
		{
			flag3 = true;
			WriteStream(m_openTable);
			WriteStream(m_closeBracket);
			WriteStream(m_firstTD);
			WriteStream(m_closeBracket);
		}
		WriteStream(m_openDiv);
		int xOffset = 0;
		int yOffset = 0;
		Rectangle imageConsolidationOffsets = imageProps.Image.ImageConsolidationOffsets;
		bool flag4 = !imageConsolidationOffsets.IsEmpty;
		if (flag4)
		{
			if ((int)sizing == 3 || (int)sizing == 2 || (int)sizing == 1)
			{
				styleContext.RenderMeasurements = styleContext.InTablix || (int)sizing != 0;
				RenderReportItemStyle((RPLElement)(object)image, (RPLElementProps)(object)imageProps, (RPLElementPropsDef)(object)imagePropsDef, measurement, styleContext, ref borderContext, ((RPLElementPropsDef)imagePropsDef).ID);
				WriteStream(m_closeBracket);
				WriteStream(m_openDiv);
			}
			WriteOuterConsolidation(imageConsolidationOffsets, sizing, ((RPLElementProps)imageProps).UniqueName);
			RenderReportItemStyle((RPLElement)(object)image, (RPLElementProps)(object)imageProps, (RPLElementPropsDef)(object)imagePropsDef, null, styleContext, ref borderContext, ((RPLElementPropsDef)imagePropsDef).ID);
			xOffset = imageConsolidationOffsets.Left;
			yOffset = imageConsolidationOffsets.Top;
		}
		else
		{
			styleContext.RenderMeasurements = styleContext.InTablix || (int)sizing != 0;
			RenderReportItemStyle((RPLElement)(object)image, (RPLElementProps)(object)imageProps, (RPLElementPropsDef)(object)imagePropsDef, measurement, styleContext, ref borderContext, ((RPLElementPropsDef)imagePropsDef).ID);
		}
		WriteStream(m_closeBracket);
		if (HasAction(actionInfo))
		{
			flag2 = RenderElementHyperlink((IRPLStyle)(object)((RPLElementProps)imageProps).Style, actionInfo.Actions[0]);
		}
		WriteStream(m_img);
		if (m_browserIE)
		{
			WriteStream(m_imgOnError);
		}
		if (renderId || flag)
		{
			RenderReportItemId(((RPLElementProps)imageProps).UniqueName);
		}
		if (imageProps.ActionImageMapAreas != null && imageProps.ActionImageMapAreas.Length > 0)
		{
			WriteAttrEncoded(m_useMap, "#" + m_deviceInfo.HtmlPrefixId + m_mapPrefixString + ((RPLElementProps)imageProps).UniqueName);
			WriteStream(m_zeroBorder);
		}
		else if (flag2)
		{
			WriteStream(m_zeroBorder);
		}
		if ((int)sizing == 2)
		{
			PaddingSharedInfo paddings = GetPaddings(((RPLElement)image).ElementProps.Style, null);
			bool writeSmallSize = !flag4 && m_deviceInfo.BrowserMode == BrowserMode.Standards;
			RenderImageFitProportional(image, measurement, paddings, writeSmallSize);
		}
		else if ((int)sizing == 1 && !flag4)
		{
			if (m_useInlineStyle)
			{
				PercentSizes();
			}
			else
			{
				ClassPercentSizes();
			}
		}
		if (flag4)
		{
			WriteClippedDiv(imageConsolidationOffsets);
		}
		WriteToolTip((RPLElementProps)(object)imageProps);
		WriteStream(m_src);
		RenderImageUrl(useSessionId: true, image2);
		WriteStreamCR(m_closeTag);
		if (flag2)
		{
			WriteStream(m_closeA);
		}
		if (imageProps.ActionImageMapAreas != null && imageProps.ActionImageMapAreas.Length > 0)
		{
			RenderImageMapAreas(imageProps.ActionImageMapAreas, ((RPLSizes)measurement).Width, ((RPLSizes)measurement).Height, ((RPLElementProps)imageProps).UniqueName, xOffset, yOffset);
		}
		if (flag4 && ((int)sizing == 3 || (int)sizing == 2 || (int)sizing == 1))
		{
			WriteStream(m_closeDiv);
		}
		WriteStreamCR(m_closeDiv);
		if (flag3)
		{
			WriteStreamCR(m_lastTD);
			WriteStreamCR(m_closeTable);
		}
	}

	protected int RenderReportItem(RPLElement reportItem, RPLElementProps props, RPLElementPropsDef def, RPLItemMeasurement measurement, StyleContext styleContext, int borderContext, bool renderId)
	{
		int borderContext1 = borderContext;
		if (reportItem == null)
			return borderContext1;
		if (measurement != null)
			styleContext.OmitBordersState = ((RPLMeasurement)measurement).State;
		switch (reportItem)
		{
			case RPLTextBox textBox:
				if (styleContext.InTablix)
				{
					this.RenderTextBoxPercent(textBox, ((RPLElement)textBox).ElementProps as RPLTextBoxProps, ((RPLElement)textBox).ElementProps.Definition as RPLTextBoxPropsDef, measurement, styleContext, renderId);
					break;
				}
				this.RenderTextBox(textBox, ((RPLElement)textBox).ElementProps as RPLTextBoxProps, ((RPLElement)textBox).ElementProps.Definition as RPLTextBoxPropsDef, measurement, styleContext, ref borderContext1, renderId);
				break;
			case RPLTablix _:
				this.RenderTablix((RPLTablix)reportItem, props, def, measurement, styleContext, ref borderContext1, renderId);
				break;
			case RPLRectangle _:
				this.RenderRectangle((RPLContainer)reportItem, props, def, measurement, ref borderContext1, renderId, styleContext);
				break;
			case RPLChart _:
			case RPLGaugePanel _:
			case RPLMap _:
				this.RenderServerDynamicImage(reportItem, (RPLDynamicImageProps)props, def, measurement, borderContext1, renderId, styleContext);
				break;
			case RPLSubReport _:
				this.RenderSubReport((RPLSubReport)reportItem, props, def, measurement, ref borderContext1, renderId, styleContext);
				break;
			case RPLImage _:
				if (styleContext.InTablix)
				{
					this.RenderImagePercent((RPLImage)reportItem, (RPLImageProps)props, (RPLImagePropsDef)def, measurement);
					break;
				}
				this.RenderImage((RPLImage)reportItem, (RPLImageProps)props, (RPLImagePropsDef)def, measurement, ref borderContext1, renderId);
				break;
			case RPLLine _:
				this.RenderLine((RPLLine)reportItem, props, (RPLLinePropsDef)def, measurement, renderId, styleContext);
				break;
		}
		return borderContext1;
	}

	protected void RenderSubReport(RPLSubReport subReport, RPLElementProps subReportProps, RPLElementPropsDef subReportDef, RPLItemMeasurement measurement, ref int borderContext, bool renderId, StyleContext styleContext)
	{
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Expected O, but got Unknown
		if (!styleContext.InTablix || renderId)
		{
			styleContext.RenderMeasurements = false;
			WriteStream(m_openDiv);
			RenderReportItemStyle((RPLElement)(object)subReport, subReportProps, subReportDef, measurement, styleContext, ref borderContext, subReportDef.ID);
			if (renderId)
			{
				RenderReportItemId(subReportProps.UniqueName);
			}
			WriteStreamCR(m_closeBracket);
		}
		RPLItemMeasurement[] children = ((RPLContainer)subReport).Children;
		int num = 0;
		int num2 = borderContext;
		bool usePercentWidth = children.Length > 0;
		int num3 = children.Length;
		for (int i = 0; i < num3; i++)
		{
			if (i == 0 && num3 > 1 && (borderContext & 8) > 0)
			{
				num2 &= -9;
			}
			else if (i == 1 && (borderContext & 4) > 0)
			{
				num2 &= -5;
			}
			if (i > 0 && i == num3 - 1 && (borderContext & 8) > 0)
			{
				num2 |= 8;
			}
			num = num2;
			RPLItemMeasurement val = children[i];
			RPLContainer val2 = (RPLContainer)val.Element;
			RPLElementProps elementProps = ((RPLElement)val2).ElementProps;
			RPLElementPropsDef definition = elementProps.Definition;
			m_isBody = true;
			m_usePercentWidth = usePercentWidth;
			RenderRectangle(val2, elementProps, definition, val, ref num, renderId: false, new StyleContext());
		}
		if (!styleContext.InTablix || renderId)
		{
			WriteStreamCR(m_closeDiv);
		}
	}

	protected void RenderRectangleMeasurements(RPLItemMeasurement measurement, IRPLStyle style)
	{
		float adjustedWidth = GetAdjustedWidth(measurement, style);
		float adjustedHeight = GetAdjustedHeight(measurement, style);
		RenderMeasurementWidth(adjustedWidth, renderMinWidth: true);
		if (m_deviceInfo.IsBrowserIE && m_deviceInfo.BrowserMode == BrowserMode.Standards && !m_deviceInfo.IsBrowserIE6)
		{
			RenderMeasurementMinHeight(adjustedHeight);
		}
		else
		{
			RenderMeasurementHeight(adjustedHeight);
		}
	}

	private void WriteFontSizeSmallPoint()
	{
		if (m_deviceInfo.IsBrowserGeckoEngine)
		{
			WriteStream(m_smallPoint);
		}
		else
		{
			WriteStream(m_zeroPoint);
		}
	}

	protected void RenderRectangle(RPLContainer rectangle, RPLElementProps props, RPLElementPropsDef def, RPLItemMeasurement measurement, ref int borderContext, bool renderId, StyleContext styleContext)
	{
		RPLItemMeasurement[] children = rectangle.Children;
		RPLRectanglePropsDef val = (RPLRectanglePropsDef)(object)((def is RPLRectanglePropsDef) ? def : null);
		if (val != null && val.LinkToChildId != null)
		{
			m_linkToChildStack.Push(val.LinkToChildId);
		}
		bool expandItem = m_expandItem;
		bool flag = renderId;
		string text = props.UniqueName;
		bool flag2 = children == null || children.Length == 0;
		if (flag2 && styleContext.InTablix)
		{
			return;
		}
		bool flag3 = m_deviceInfo.OutlookCompat || !m_browserIE || (flag2 && m_usePercentWidth);
		if (!styleContext.InTablix || renderId)
		{
			if (flag3)
			{
				WriteStream(m_openTable);
				WriteStream(m_zeroBorder);
			}
			else
			{
				WriteStream(m_openDiv);
				if (m_deviceInfo.IsBrowserIE && m_deviceInfo.AllowScript)
				{
					if (!m_needsGrowRectangleScript)
					{
						CreateGrowRectIdsStream();
					}
					flag = true;
					if (!renderId)
					{
						text = props.UniqueName + "_gr";
					}
					WriteIdToSecondaryStream(m_growRectangleIdsStream, text);
				}
			}
			if (flag)
			{
				RenderReportItemId(text);
			}
			if (m_isBody)
			{
				m_isBody = false;
				styleContext.RenderMeasurements = false;
				if (flag2)
				{
					OpenStyle();
					if (m_usePercentWidth)
					{
						RenderMeasurementHeight(((RPLSizes)measurement).Height);
						WriteStream(m_styleWidth);
						WriteStream(m_percent);
						WriteStream(m_semiColon);
					}
					else
					{
						RenderRectangleMeasurements(measurement, (IRPLStyle)(object)props.Style);
					}
				}
				else if (flag3 && m_usePercentWidth)
				{
					OpenStyle();
					WriteStream(m_styleWidth);
					WriteStream(m_percent);
					WriteStream(m_semiColon);
				}
				m_usePercentWidth = false;
			}
			if (!styleContext.InTablix)
			{
				if (styleContext.RenderMeasurements)
				{
					OpenStyle();
					RenderRectangleMeasurements(measurement, (IRPLStyle)(object)props.Style);
				}
				RenderReportItemStyle((RPLElement)(object)rectangle, props, def, measurement, styleContext, ref borderContext, def.ID);
			}
			CloseStyle(renderQuote: true);
			WriteToolTip(props);
			WriteStreamCR(m_closeBracket);
			if (flag3)
			{
				WriteStream(m_firstTD);
				OpenStyle();
				if (flag2)
				{
					RenderMeasurementStyle(((RPLSizes)measurement).Height, ((RPLSizes)measurement).Width);
					WriteStream(m_fontSize);
					WriteStream("1pt");
				}
				else
				{
					WriteStream(m_verticalAlign);
					WriteStream(m_topValue);
				}
				CloseStyle(renderQuote: true);
				WriteStream(m_closeBracket);
			}
		}
		if (flag2)
		{
			WriteStream(m_nbsp);
		}
		else
		{
			bool inTablix = styleContext.InTablix;
			styleContext.InTablix = false;
			flag2 = GenerateHTMLTable(children, ((RPLSizes)measurement).Top, ((RPLSizes)measurement).Left, ((RPLSizes)measurement).Width, ((RPLSizes)measurement).Height, borderContext, expandItem, SharedListLayoutState.None, null, (IRPLStyle)(object)props.Style);
			if (inTablix)
			{
				styleContext.InTablix = true;
			}
		}
		if (!styleContext.InTablix || renderId)
		{
			if (flag3)
			{
				WriteStream(m_lastTD);
				WriteStream(m_closeTable);
			}
			else
			{
				WriteStreamCR(m_closeDiv);
			}
		}
		if (m_linkToChildStack.Count > 0 && val != null && val.LinkToChildId != null && val.LinkToChildId.Equals(m_linkToChildStack.Peek()))
		{
			m_linkToChildStack.Pop();
		}
	}

	private void RenderElementHyperlinkAllTextStyles(RPLElementStyle style, RPLAction action, string id)
	{
		WriteStream(m_openA);
		RenderTabIndex();
		bool hasHref = false;
		if (action.Hyperlink != null)
		{
			WriteStream(m_hrefString + HttpUtility.HtmlAttributeEncode(action.Hyperlink) + m_quoteString);
			hasHref = true;
		}
		else
		{
			RenderInteractionAction(action, ref hasHref);
		}
		TextRunStyleWriter styleWriter = new TextRunStyleWriter(this);
		WriteStyles(id, style.NonSharedProperties, style.SharedProperties, styleWriter);
		if (m_deviceInfo.LinkTarget != null)
		{
			WriteStream(m_target);
			WriteStream(m_deviceInfo.LinkTarget);
			WriteStream(m_quote);
		}
		WriteStream(m_closeBracket);
	}

	private bool RenderElementHyperlink(IRPLStyle style, RPLAction action)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		object obj = style[(byte)24];
		obj = ((obj != null) ? obj : ((object)(TextDecorations)0));
		string color = (string)style[(byte)27];
		return RenderHyperlink(action, (TextDecorations)obj, color);
	}

	protected void RenderTextBoxPercent(RPLTextBox textBox, RPLTextBoxProps textBoxProps, RPLTextBoxPropsDef textBoxPropsDef, RPLItemMeasurement measurement, StyleContext styleContext, bool renderId)
	{
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		RPLStyleProps actionStyle = null;
		RPLActionInfo actionInfo = textBoxProps.ActionInfo;
		RPLStyleProps nonSharedStyle = ((RPLElementProps)textBoxProps).NonSharedStyle;
		RPLStyleProps sharedStyle = ((RPLElementPropsDef)textBoxPropsDef).SharedStyle;
		RPLElementStyle style = ((RPLElementProps)textBoxProps).Style;
		bool flag = CanSort(textBoxPropsDef);
		bool flag2 = NeedSharedToggleParent(textBoxProps);
		bool flag3 = false;
		bool isSimple = textBoxPropsDef.IsSimple;
		bool flag4 = IsDirectionRTL((IRPLStyle)(object)style);
		bool flag5 = IsWritingModeVertical((IRPLStyle)(object)style);
		bool flag6 = flag5 && m_deviceInfo.IsBrowserIE;
		if (flag6)
		{
			if (textBoxPropsDef.CanGrow)
			{
				WriteStream(m_openDiv);
				OpenStyle();
				RenderDirectionStyles((RPLElement)(object)textBox, (RPLElementProps)(object)textBoxProps, (RPLElementPropsDef)(object)textBoxPropsDef, null, (IRPLStyle)(object)((RPLElementProps)textBoxProps).Style, (IRPLStyle)(object)nonSharedStyle, isNonSharedStyles: false, styleContext);
				WriteStream("display: inline;");
				CloseStyle(renderQuote: true);
				ClassPercentHeight();
				if (m_deviceInfo.BrowserMode == BrowserMode.Standards && !m_deviceInfo.IsBrowserIE6Or7StandardsMode && m_deviceInfo.AllowScript)
				{
					if (!m_needsFitVertTextScript)
					{
						CreateFitVertTextIdsStream();
					}
					WriteIdToSecondaryStream(m_fitVertTextIdsStream, ((RPLElementProps)textBoxProps).UniqueName + "_fvt");
					RenderReportItemId(((RPLElementProps)textBoxProps).UniqueName + "_fvt");
				}
				WriteStreamCR(m_closeBracket);
				WriteStream(m_openTable);
				ClassPercentHeight();
				WriteStreamCR(m_closeBracket);
				WriteStream(m_firstTD);
			}
			else
			{
				WriteStream(m_openDiv);
			}
		}
		else
		{
			WriteStream(m_openDiv);
		}
		if (renderId || flag2 || flag)
		{
			RenderReportItemId(((RPLElementProps)textBoxProps).UniqueName);
		}
		bool flag7 = flag2 && !isSimple;
		bool flag8 = flag || flag7;
		if (!textBoxPropsDef.CanGrow)
		{
			if ((!m_browserIE || m_deviceInfo.BrowserMode == BrowserMode.Standards || flag6) && measurement != null)
			{
				styleContext.RenderMeasurements = false;
				float innerContainerHeight = GetInnerContainerHeight(measurement, (IRPLStyle)(object)style);
				OpenStyle();
				RenderMeasurementHeight(innerContainerHeight);
				WriteStream(m_overflowHidden);
				WriteStream(m_semiColon);
			}
			else
			{
				styleContext.RenderMeasurements = true;
			}
			if (!flag8)
			{
				object obj = style[(byte)26];
				bool flag9 = obj != null && (int)(VerticalAlignments)obj != 0 && !textBoxPropsDef.CanGrow;
				flag8 = flag9;
			}
			measurement = null;
		}
		if (flag8)
		{
			CloseStyle(renderQuote: true);
			styleContext.RenderMeasurements = false;
			WriteStreamCR(m_closeBracket);
			WriteStream(m_openTable);
			WriteStream(m_zeroBorder);
			if (isSimple && (flag || flag7))
			{
				WriteClassName(m_percentHeightInlineTable, m_classPercentHeightInlineTable);
			}
			else
			{
				WriteClassName(m_percentSizeInlineTable, m_classPercentSizeInlineTable);
			}
			RenderReportLanguage();
			WriteStream(m_closeBracket);
			WriteStream(m_firstTD);
			if (flag || flag7)
			{
				if (flag5)
				{
					WriteStream(" ROWS='2'");
				}
				RenderAtStart(textBoxProps, (IRPLStyle)(object)style, flag && flag4, flag7 && !flag4);
			}
		}
		int borderContext = 0;
		RenderReportItemStyle((RPLElement)(object)textBox, (RPLElementProps)(object)textBoxProps, (RPLElementPropsDef)(object)textBoxPropsDef, nonSharedStyle, sharedStyle, measurement, styleContext, ref borderContext, ((RPLElementPropsDef)textBoxPropsDef).ID);
		WriteToolTip((RPLElementProps)(object)textBoxProps);
		WriteStreamCR(m_closeBracket);
		if (flag2 && isSimple)
		{
			RenderToggleImage(textBoxProps);
		}
		RPLAction val = null;
		if (HasAction(actionInfo))
		{
			val = actionInfo.Actions[0];
			RenderElementHyperlinkAllTextStyles(style, val, ((RPLElementPropsDef)textBoxPropsDef).ID + "a");
			flag3 = true;
		}
		string text = null;
		if (textBoxPropsDef.IsSimple)
		{
			text = textBoxProps.Value;
			if (string.IsNullOrEmpty(text))
			{
				text = textBoxPropsDef.Value;
			}
		}
		RenderTextBoxContent(textBox, textBoxProps, textBoxPropsDef, text, actionStyle, flag2 || flag, measurement, val);
		if (flag3)
		{
			WriteStream(m_closeA);
		}
		if (flag8)
		{
			RenderAtEnd(textBoxProps, (IRPLStyle)(object)style, flag && !flag4, flag7 && flag4);
			WriteStream(m_lastTD);
			WriteStream(m_closeTable);
		}
		if (flag6)
		{
			if (textBoxPropsDef.CanGrow)
			{
				WriteStreamCR(m_lastTD);
				WriteStreamCR(m_closeTable);
				WriteStreamCR(m_closeDiv);
			}
			else
			{
				WriteStream(m_closeDiv);
			}
		}
		else
		{
			WriteStreamCR(m_closeDiv);
		}
	}

	protected void RenderPageHeaderFooter(RPLItemMeasurement hfMeasurement)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		if (((RPLSizes)hfMeasurement).Height != 0f)
		{
			RPLHeaderFooter val = (RPLHeaderFooter)hfMeasurement.Element;
			int borderContext = 0;
			StyleContext styleContext = new StyleContext();
			WriteStream(m_openTR);
			WriteStream(m_closeBracket);
			WriteStream(m_openTD);
			styleContext.StyleOnCell = true;
			RenderReportItemStyle((RPLElement)(object)val, ((RPLElement)val).ElementProps, ((RPLElement)val).ElementProps.Definition, null, styleContext, ref borderContext, ((RPLElement)val).ElementProps.Definition.ID + "c");
			styleContext.StyleOnCell = false;
			WriteStream(m_closeBracket);
			WriteStream(m_openDiv);
			if (!m_deviceInfo.IsBrowserIE)
			{
				styleContext.RenderMeasurements = false;
				styleContext.RenderMinMeasurements = true;
			}
			RenderReportItemStyle((RPLElement)(object)val, hfMeasurement, ref borderContext, styleContext);
			WriteStreamCR(m_closeBracket);
			RPLItemMeasurement[] children = ((RPLContainer)val).Children;
			if (children != null && children.Length > 0)
			{
				m_renderTableHeight = true;
				GenerateHTMLTable(children, 0f, 0f, m_pageContent.MaxSectionWidth, ((RPLSizes)hfMeasurement).Height, borderContext, expandLayout: false, SharedListLayoutState.None, null, (IRPLStyle)(object)((RPLElement)val).ElementProps.Style);
			}
			else
			{
				WriteStream(m_nbsp);
			}
			m_renderTableHeight = false;
			WriteStreamCR(m_closeDiv);
			WriteStream(m_closeTD);
			WriteStream(m_closeTR);
		}
	}

	protected void RenderStyleProps(RPLElement reportItem, RPLElementProps props, RPLElementPropsDef definition, RPLItemMeasurement measurement, IRPLStyle sharedStyleProps, IRPLStyle nonSharedStyleProps, StyleContext styleContext, ref int borderContext, bool isNonSharedStyles)
	{
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Expected O, but got Unknown
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0565: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ba: Expected O, but got Unknown
		//IL_066f: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_07bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0805: Unknown result type (might be due to invalid IL or missing references)
		if (styleContext.ZeroWidth)
		{
			WriteStream(m_displayNone);
		}
		IRPLStyle val = (isNonSharedStyles ? nonSharedStyleProps : sharedStyleProps);
		if (val == null)
		{
			return;
		}
		object obj = null;
		if (styleContext.StyleOnCell)
		{
			bool renderPadding = true;
			if ((IsWritingModeVertical(sharedStyleProps) || IsWritingModeVertical(nonSharedStyleProps)) && styleContext.IgnorePadding && m_deviceInfo.IsBrowserIE)
			{
				renderPadding = false;
			}
			if (!styleContext.NoBorders)
			{
				RenderHtmlBorders(val, ref borderContext, styleContext.OmitBordersState, renderPadding, isNonSharedStyles, sharedStyleProps);
				RenderBackgroundStyleProps(val);
			}
			if (!styleContext.OnlyRenderMeasurementsBackgroundBorders)
			{
				obj = val[(byte)26];
				if (obj != null && !styleContext.IgnoreVerticalAlign)
				{
					obj = EnumStrings.GetValue((VerticalAlignments)obj);
					WriteStream(m_verticalAlign);
					WriteStream(obj);
					WriteStream(m_semiColon);
				}
				obj = val[(byte)25];
				if (obj != null)
				{
					if ((int)(TextAlignments)obj != 0)
					{
						obj = EnumStrings.GetValue((TextAlignments)obj);
						WriteStream(m_textAlign);
						WriteStream(obj);
						WriteStream(m_semiColon);
					}
					else
					{
						RenderTextAlign((RPLTextBoxProps)(object)((props is RPLTextBoxProps) ? props : null), props.Style);
					}
				}
				RenderDirectionStyles(reportItem, props, definition, measurement, sharedStyleProps, nonSharedStyleProps, isNonSharedStyles, styleContext);
			}
			if (measurement == null || (!m_deviceInfo.OutlookCompat && m_deviceInfo.IsBrowserIE))
			{
				return;
			}
			float num = ((RPLSizes)measurement).Width;
			if ((reportItem is RPLTextBox || IsImageNotFitProportional(reportItem, definition)) && !styleContext.InTablix)
			{
				float adjustedWidth = GetAdjustedWidth(measurement, (IRPLStyle)(object)props.Style);
				if (m_deviceInfo.IsBrowserIE6Or7StandardsMode)
				{
					num = adjustedWidth;
				}
				RenderMeasurementMinWidth(adjustedWidth);
			}
			else
			{
				RenderMeasurementMinWidth(num);
			}
			RenderMeasurementWidth(num, renderMinWidth: false);
			return;
		}
		if (reportItem is RPLTextBox)
		{
			WriteStream(m_wordWrap);
			WriteStream(m_semiColon);
			WriteStream(m_whiteSpacePreWrap);
			WriteStream(m_semiColon);
		}
		if (styleContext.RenderMeasurements || styleContext.RenderMinMeasurements)
		{
			bool empty = false;
			IsCollectionWithoutContent((RPLContainer)(object)((reportItem is RPLContainer) ? reportItem : null), ref empty);
			if (measurement == null || (styleContext.InTablix && !empty && (reportItem is RPLChart || reportItem is RPLGaugePanel || reportItem is RPLMap)))
			{
				if (reportItem is RPLTextBox)
				{
					RPLTextBoxPropsDef val2 = (RPLTextBoxPropsDef)definition;
					if (styleContext.RenderMeasurements)
					{
						WriteStream(m_styleWidth);
					}
					else if (styleContext.RenderMinMeasurements)
					{
						WriteStream(m_styleMinWidth);
					}
					if (styleContext.InTablix && m_deviceInfo.BrowserMode == BrowserMode.Quirks)
					{
						WriteStream(m_ninetyninepercent);
					}
					else
					{
						WriteStream(m_percent);
					}
					WriteStream(m_semiColon);
					if (val2.CanGrow)
					{
						WriteStream(m_overflowXHidden);
					}
					else
					{
						if (styleContext.RenderMeasurements)
						{
							WriteStream(m_styleHeight);
						}
						else if (styleContext.RenderMinMeasurements)
						{
							WriteStream(m_styleMinHeight);
						}
						WriteStream(m_percent);
						WriteStream(m_semiColon);
						WriteStream(m_overflowHidden);
					}
					WriteStream(m_semiColon);
				}
				else if (!(reportItem is RPLTablix))
				{
					RenderPercentSizes();
				}
			}
			else if (reportItem is RPLTextBox)
			{
				float num2 = ((RPLSizes)measurement).Width;
				float height = ((RPLSizes)measurement).Height;
				if (!styleContext.NoBorders && !styleContext.InTablix)
				{
					float adjustedWidth2 = GetAdjustedWidth(measurement, (IRPLStyle)(object)props.Style);
					if (m_deviceInfo.IsBrowserIE6Or7StandardsMode)
					{
						num2 = adjustedWidth2;
						height = GetAdjustedHeight(measurement, (IRPLStyle)(object)props.Style);
					}
					RenderMeasurementMinWidth(adjustedWidth2);
				}
				else
				{
					RenderMeasurementMinWidth(num2);
				}
				RPLTextBoxPropsDef val3 = (RPLTextBoxPropsDef)definition;
				if (val3.CanGrow && val3.CanShrink)
				{
					RenderMeasurementWidth(num2, renderMinWidth: false);
				}
				else
				{
					WriteStream(m_overflowHidden);
					WriteStream(m_semiColon);
					RenderMeasurementWidth(num2, renderMinWidth: false);
					RenderMeasurementHeight(height);
				}
			}
			else if (!(reportItem is RPLTablix))
			{
				if (!(reportItem is RPLRectangle))
				{
					float height2 = ((RPLSizes)measurement).Height;
					float num3 = ((RPLSizes)measurement).Width;
					if (!styleContext.InTablix && IsImageNotFitProportional(reportItem, definition) && !styleContext.NoBorders)
					{
						float adjustedWidth3 = GetAdjustedWidth(measurement, (IRPLStyle)(object)props.Style);
						if (m_deviceInfo.IsBrowserIE6Or7StandardsMode)
						{
							num3 = adjustedWidth3;
							height2 = GetAdjustedHeight(measurement, (IRPLStyle)(object)props.Style);
						}
						RenderMeasurementMinWidth(adjustedWidth3);
					}
					else
					{
						RenderMeasurementMinWidth(num3);
					}
					if (reportItem is RPLHeaderFooter && (!m_deviceInfo.IsBrowserIE || (m_deviceInfo.BrowserMode == BrowserMode.Standards && !m_deviceInfo.IsBrowserIE6)))
					{
						RenderMeasurementMinHeight(height2);
					}
					else
					{
						RenderMeasurementHeight(height2);
					}
					RenderMeasurementWidth(num3, renderMinWidth: false);
				}
				if (empty || reportItem is RPLImage)
				{
					WriteStream(m_overflowHidden);
					WriteStream(m_semiColon);
				}
			}
		}
		if (!styleContext.InTablix && !styleContext.NoBorders)
		{
			RenderHtmlBorders(val, ref borderContext, styleContext.OmitBordersState, !styleContext.EmptyTextBox || m_deviceInfo.IsBrowserIE6Or7StandardsMode, isNonSharedStyles, sharedStyleProps);
			RenderBackgroundStyleProps(val);
		}
		if (styleContext.OnlyRenderMeasurementsBackgroundBorders || (styleContext.EmptyTextBox && isNonSharedStyles))
		{
			return;
		}
		obj = val[(byte)19];
		if (obj != null)
		{
			obj = EnumStrings.GetValue((FontStyles)obj);
			WriteStream(m_fontStyle);
			WriteStream(obj);
			WriteStream(m_semiColon);
		}
		obj = val[(byte)20];
		if (obj != null)
		{
			WriteStream(m_fontFamily);
			WriteStream(HandleSpecialFontCharacters(obj.ToString()));
			WriteStream(m_semiColon);
		}
		obj = val[(byte)21];
		if (obj != null)
		{
			WriteStream(m_fontSize);
			if (string.Compare(obj.ToString(), "0pt", StringComparison.OrdinalIgnoreCase) != 0)
			{
				WriteStream(obj);
			}
			else
			{
				WriteFontSizeSmallPoint();
			}
			WriteStream(m_semiColon);
		}
		else
		{
			RPLTextBoxPropsDef val4 = (RPLTextBoxPropsDef)(object)((definition is RPLTextBoxPropsDef) ? definition : null);
			RPLStyleProps sharedStyle = reportItem.ElementPropsDef.SharedStyle;
			if ((!isNonSharedStyles || sharedStyle == null || sharedStyle.Count == 0) && val4 != null && !val4.IsSimple)
			{
				WriteStream(m_fontSize);
				WriteFontSizeSmallPoint();
				WriteStream(m_semiColon);
			}
		}
		obj = val[(byte)22];
		if (obj != null)
		{
			obj = EnumStrings.GetValue((FontWeights)obj);
			WriteStream(m_fontWeight);
			WriteStream(obj);
			WriteStream(m_semiColon);
		}
		obj = val[(byte)24];
		if (obj != null)
		{
			obj = EnumStrings.GetValue((TextDecorations)obj);
			WriteStream(m_textDecoration);
			WriteStream(obj);
			WriteStream(m_semiColon);
		}
		obj = val[(byte)31];
		if (obj != null)
		{
			obj = EnumStrings.GetValue((UnicodeBiDiTypes)obj);
			WriteStream(m_unicodeBiDi);
			WriteStream(obj);
			WriteStream(m_semiColon);
		}
		obj = val[(byte)27];
		if (obj != null)
		{
			WriteStream(m_color);
			WriteStream(obj);
			WriteStream(m_semiColon);
		}
		obj = val[(byte)28];
		if (obj != null)
		{
			WriteStream(m_lineHeight);
			WriteStream(obj);
			WriteStream(m_semiColon);
		}
		if ((IsWritingModeVertical(sharedStyleProps) || IsWritingModeVertical(nonSharedStyleProps)) && reportItem is RPLTextBox && styleContext.InTablix && m_deviceInfo.IsBrowserIE && !styleContext.IgnorePadding)
		{
			RenderPaddingStyle(val);
		}
		RenderDirectionStyles(reportItem, props, definition, measurement, sharedStyleProps, nonSharedStyleProps, isNonSharedStyles, styleContext);
		obj = val[(byte)26];
		if (obj != null && !styleContext.IgnoreVerticalAlign)
		{
			obj = EnumStrings.GetValue((VerticalAlignments)obj);
			WriteStream(m_verticalAlign);
			WriteStream(obj);
			WriteStream(m_semiColon);
		}
		obj = val[(byte)25];
		if (obj != null)
		{
			if ((int)(TextAlignments)obj != 0)
			{
				WriteStream(m_textAlign);
				WriteStream(EnumStrings.GetValue((TextAlignments)obj));
				WriteStream(m_semiColon);
			}
			else
			{
				RenderTextAlign((RPLTextBoxProps)(object)((props is RPLTextBoxProps) ? props : null), props.Style);
			}
		}
	}

	protected void RenderLine(RPLLine reportItem, RPLElementProps rplProps, RPLLinePropsDef rplPropsDef, RPLItemMeasurement measurement, bool renderId, StyleContext styleContext)
	{
		if (IsLineSlanted(measurement))
		{
			if (renderId)
			{
				RenderNavigationId(rplProps.UniqueName);
			}
			if (m_deviceInfo.BrowserMode == BrowserMode.Quirks)
			{
				RenderVMLLine(reportItem, measurement, styleContext);
			}
			return;
		}
		bool flag = ((RPLSizes)measurement).Height == 0f;
		WriteStream(m_openSpan);
		if (renderId)
		{
			RenderReportItemId(rplProps.UniqueName);
		}
		int borderContext = 0;
		object obj = rplProps.Style[(byte)10];
		if (obj != null)
		{
			OpenStyle();
			if (flag)
			{
				WriteStream(m_styleHeight);
			}
			else
			{
				WriteStream(m_styleWidth);
			}
			WriteStream(obj);
			WriteStream(m_semiColon);
		}
		obj = rplProps.Style[(byte)0];
		if (obj != null)
		{
			OpenStyle();
			WriteStream(m_backgroundColor);
			WriteStream(obj);
		}
		RenderReportItemStyle((RPLElement)(object)reportItem, measurement, ref borderContext);
		CloseStyle(renderQuote: true);
		WriteStream(m_closeBracket);
		WriteStream(m_closeSpan);
	}

	protected bool GenerateHTMLTable(RPLItemMeasurement[] repItemCol, float ownerTop, float ownerLeft, float dxParent, float dyParent, int borderContext, bool expandLayout, SharedListLayoutState layoutState, List<RPLTablixMemberCell> omittedHeaders, IRPLStyle style)
	{
		int num = 0;
		bool result = false;
		object defaultBorderStyle = null;
		object specificBorderStyle = null;
		object specificBorderStyle2 = null;
		object defaultBorderWidth = null;
		object specificBorderWidth = null;
		object specificBorderWidth2 = null;
		if (style != null)
		{
			defaultBorderStyle = style[(byte)5];
			specificBorderStyle = style[(byte)6];
			specificBorderStyle2 = style[(byte)7];
			defaultBorderWidth = style[(byte)10];
			specificBorderWidth = style[(byte)11];
			specificBorderWidth2 = style[(byte)12];
		}
		if (repItemCol == null || repItemCol.Length == 0)
		{
			if (omittedHeaders != null)
			{
				for (int i = 0; i < omittedHeaders.Count; i++)
				{
					if (omittedHeaders[i].GroupLabel != null)
					{
						RenderNavigationId(omittedHeaders[i].UniqueName);
					}
				}
			}
			return result;
		}
		PageTableLayout val = null;
		PageTableLayout.GenerateTableLayout(repItemCol, dxParent, dyParent, 0f, ref val, expandLayout, m_rplReport.ConsumeContainerWhitespace);
		if (val == null)
		{
			return result;
		}
		if (val.BandTable && m_allowBandTable && layoutState == SharedListLayoutState.None && (!m_renderTableHeight || val.NrRows == 1))
		{
			if (omittedHeaders != null)
			{
				for (int j = 0; j < omittedHeaders.Count; j++)
				{
					if (omittedHeaders[j].GroupLabel != null)
					{
						RenderNavigationId(omittedHeaders[j].UniqueName);
					}
				}
			}
			int borderContext2 = 0;
			int k;
			for (k = 0; k < val.NrRows - 1; k++)
			{
				if (borderContext > 0)
				{
					borderContext2 = GetNewContext(borderContext, k + 1, 1, val.NrRows, 1);
				}
				RenderCellItem(val.GetCell(k), borderContext2, layoutExpand: false);
			}
			if (borderContext > 0)
			{
				borderContext2 = GetNewContext(borderContext, k + 1, 1, val.NrRows, 1);
			}
			RenderCellItem(val.GetCell(k), borderContext2, layoutExpand: false);
			return result;
		}
		m_allowBandTable = true;
		bool flag = false;
		bool renderHeight = true;
		bool flag2 = expandLayout;
		int num2 = val.NrCols;
		if (!flag2)
		{
			flag2 = val.AreSpansInColOne();
		}
		if (layoutState == SharedListLayoutState.None || layoutState == SharedListLayoutState.Start)
		{
			WriteStream(m_openTable);
			WriteStream(m_zeroBorder);
			if (flag2)
			{
				num2++;
			}
			if (!m_deviceInfo.IsBrowserGeckoEngine)
			{
				WriteStream(m_cols);
				WriteStream(num2.ToString(CultureInfo.InvariantCulture));
				WriteStream(m_quote);
			}
			RenderReportLanguage();
			if (m_useInlineStyle)
			{
				OpenStyle();
				WriteStream(m_borderCollapse);
				if (expandLayout)
				{
					WriteStream(m_semiColon);
					WriteStream(m_styleHeight);
					WriteStream(m_percent);
				}
			}
			else
			{
				ClassLayoutBorder();
				if (expandLayout)
				{
					WriteStream(m_space);
					WriteAttrEncoded(m_deviceInfo.HtmlPrefixId);
					WriteStream(m_percentHeight);
				}
				WriteStream(m_quote);
			}
			if (m_renderTableHeight)
			{
				if (m_isStyleOpen)
				{
					WriteStream(m_semiColon);
				}
				else
				{
					OpenStyle();
				}
				WriteStream(m_styleHeight);
				WriteDStream(dyParent);
				WriteStream(m_mm);
				m_renderTableHeight = false;
			}
			if (m_deviceInfo.OutlookCompat || m_deviceInfo.IsBrowserSafari)
			{
				if (m_isStyleOpen)
				{
					WriteStream(m_semiColon);
				}
				else
				{
					OpenStyle();
				}
				WriteStream(m_styleWidth);
				float num3 = dxParent;
				if (num3 > 0f)
				{
					num3 = SubtractBorderStyles(num3, defaultBorderStyle, specificBorderStyle, defaultBorderWidth, specificBorderWidth);
					num3 = SubtractBorderStyles(num3, defaultBorderStyle, specificBorderStyle2, defaultBorderWidth, specificBorderWidth2);
					if (num3 < 0f)
					{
						num3 = 1f;
					}
				}
				WriteStream(num3);
				WriteStream(m_mm);
			}
			CloseStyle(renderQuote: true);
			WriteStream(m_closeBracket);
			if (val.NrCols > 1)
			{
				flag = val.NeedExtraRow();
				if (flag)
				{
					WriteStream(m_openTR);
					WriteStream(m_zeroHeight);
					WriteStream(m_closeBracket);
					if (flag2)
					{
						WriteStream(m_openTD);
						WriteStream(m_openStyle);
						WriteStream(m_styleWidth);
						WriteStream("0");
						WriteStream(m_px);
						WriteStream(m_closeQuote);
						WriteStream(m_closeTD);
					}
					for (num = 0; num < val.NrCols; num++)
					{
						WriteStream(m_openTD);
						WriteStream(m_openStyle);
						WriteStream(m_styleWidth);
						float num4 = val.GetCell(num).DXValue.Value;
						if (num4 > 0f)
						{
							if (num == 0)
							{
								num4 = SubtractBorderStyles(num4, defaultBorderStyle, specificBorderStyle, defaultBorderWidth, specificBorderWidth);
							}
							if (num == val.NrCols - 1)
							{
								num4 = SubtractBorderStyles(num4, defaultBorderStyle, specificBorderStyle2, defaultBorderWidth, specificBorderWidth2);
							}
							if (num4 <= 0f)
							{
								num4 = ((m_deviceInfo.BrowserMode != BrowserMode.Standards || !m_deviceInfo.IsBrowserIE) ? 1f : val.GetCell(num).DXValue.Value);
							}
						}
						WriteDStream(num4);
						WriteStream(m_mm);
						WriteStream(m_semiColon);
						WriteStream(m_styleMinWidth);
						WriteDStream(num4);
						WriteStream(m_mm);
						WriteStream(m_closeQuote);
						WriteStream(m_closeTD);
					}
					WriteStream(m_closeTR);
				}
			}
		}
		GenerateTableLayoutContent(val, repItemCol, flag, flag2, renderHeight, borderContext, expandLayout, layoutState, omittedHeaders, style);
		if (layoutState == SharedListLayoutState.None || layoutState == SharedListLayoutState.End)
		{
			if (expandLayout)
			{
				WriteStream(m_firstTD);
				ClassPercentHeight();
				WriteStream(m_cols);
				WriteStream(num2.ToString(CultureInfo.InvariantCulture));
				WriteStream(m_closeQuote);
				WriteStream(m_lastTD);
			}
			WriteStreamCR(m_closeTable);
		}
		return result;
	}

	protected void RenderZoom()
	{
		if (m_deviceInfo.Zoom != 100)
		{
			WriteStream(m_openStyle);
			WriteStream("zoom:");
			WriteStream(m_deviceInfo.Zoom.ToString(CultureInfo.InvariantCulture));
			WriteStream("%\"");
		}
	}

	protected void PredefinedStyles()
	{
		PredefinedStyles(m_deviceInfo, this, m_styleClassPrefix);
	}

	internal static void PredefinedStyles(DeviceInfo m_deviceInfo, HTMLWriter writer)
	{
		PredefinedStyles(m_deviceInfo, writer, null);
	}

	internal static void PredefinedStyles(DeviceInfo deviceInfo, HTMLWriter writer, byte[] classStylePrefix)
	{
		StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, m_percentSizes);
		writer.WriteStream(m_styleHeight);
		writer.WriteStream(m_percent);
		writer.WriteStream(m_semiColon);
		writer.WriteStream(m_styleWidth);
		writer.WriteStream(m_percent);
		writer.WriteStream(m_closeAccol);
		StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, m_percentSizesOverflow);
		writer.WriteStream(m_styleHeight);
		writer.WriteStream(m_percent);
		writer.WriteStream(m_semiColon);
		writer.WriteStream(m_styleWidth);
		writer.WriteStream(m_percent);
		writer.WriteStream(m_semiColon);
		writer.WriteStream(m_overflowHidden);
		writer.WriteStream(m_closeAccol);
		StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, m_percentHeight);
		writer.WriteStream(m_styleHeight);
		writer.WriteStream(m_percent);
		writer.WriteStream(m_closeAccol);
		StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, m_ignoreBorder);
		writer.WriteStream(m_borderStyle);
		writer.WriteStream(m_none);
		writer.WriteStream(m_closeAccol);
		StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, m_ignoreBorderL);
		writer.WriteStream(m_borderLeftStyle);
		writer.WriteStream(m_none);
		writer.WriteStream(m_closeAccol);
		StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, m_ignoreBorderR);
		writer.WriteStream(m_borderRightStyle);
		writer.WriteStream(m_none);
		writer.WriteStream(m_closeAccol);
		StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, m_ignoreBorderT);
		writer.WriteStream(m_borderTopStyle);
		writer.WriteStream(m_none);
		writer.WriteStream(m_closeAccol);
		StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, m_ignoreBorderB);
		writer.WriteStream(m_borderBottomStyle);
		writer.WriteStream(m_none);
		writer.WriteStream(m_closeAccol);
		StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, m_layoutBorder);
		writer.WriteStream(m_borderCollapse);
		writer.WriteStream(m_closeAccol);
		StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, m_layoutFixed);
		writer.WriteStream(m_borderCollapse);
		writer.WriteStream(m_semiColon);
		writer.WriteStream(m_tableLayoutFixed);
		writer.WriteStream(m_closeAccol);
		StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, m_percentWidthOverflow);
		writer.WriteStream(m_styleWidth);
		writer.WriteStream(m_percent);
		writer.WriteStream(m_semiColon);
		writer.WriteStream(m_overflowXHidden);
		writer.WriteStream(m_closeAccol);
		StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, m_popupAction);
		writer.WriteStream("position:absolute;display:none;background-color:white;border:1px solid black;");
		writer.WriteStream(m_closeAccol);
		StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, m_styleAction);
		writer.WriteStream("text-decoration:none;color:black;cursor:pointer;");
		writer.WriteStream(m_closeAccol);
		StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, m_emptyTextBox);
		writer.WriteStream(m_fontSize);
		writer.WriteStream(deviceInfo.IsBrowserGeckoEngine ? m_smallPoint : m_zeroPoint);
		writer.WriteStream(m_closeAccol);
		StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, m_rtlEmbed);
		writer.WriteStream(m_direction);
		writer.WriteStream("RTL;");
		writer.WriteStream(m_unicodeBiDi);
		writer.WriteStream(EnumStrings.GetValue((UnicodeBiDiTypes)1));
		writer.WriteStream(m_closeAccol);
		StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, m_noVerticalMarginClassName);
		writer.WriteStream(m_marginTop);
		writer.WriteStream(m_zeroPoint);
		writer.WriteStream(m_semiColon);
		writer.WriteStream(m_marginBottom);
		writer.WriteStream(m_zeroPoint);
		writer.WriteStream(m_closeAccol);
		StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, m_percentSizeInlineTable);
		writer.WriteStream(m_styleHeight);
		writer.WriteStream(m_percent);
		writer.WriteStream(m_semiColon);
		writer.WriteStream(m_styleWidth);
		writer.WriteStream(m_percent);
		writer.WriteStream(m_semiColon);
		writer.WriteStream("display:inline-table");
		writer.WriteStream(m_closeAccol);
		StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, m_percentHeightInlineTable);
		writer.WriteStream(m_styleHeight);
		writer.WriteStream(m_percent);
		writer.WriteStream(m_semiColon);
		writer.WriteStream("display:inline-table");
		writer.WriteStream(m_closeAccol);
		if (classStylePrefix != null)
		{
			writer.WriteStream(classStylePrefix);
		}
		writer.WriteStream(" * { ");
		string value = null;
		if (deviceInfo.IsBrowserSafari)
		{
			value = "-webkit-";
		}
		else if (deviceInfo.IsBrowserGeckoEngine)
		{
			value = "-moz-";
		}
		if (!string.IsNullOrEmpty(value))
		{
			writer.WriteStream(value);
			writer.WriteStream("box-sizing: border-box; ");
		}
		writer.WriteStream("box-sizing: border-box }");
	}

	private static void StartPredefinedStyleClass(DeviceInfo deviceInfo, HTMLWriter writer, byte[] classStylePrefix, byte[] className)
	{
		if (classStylePrefix != null)
		{
			writer.WriteStream(classStylePrefix);
		}
		writer.WriteStream(m_dot);
		writer.WriteStream(deviceInfo.HtmlPrefixId);
		writer.WriteStream(className);
		writer.WriteStream(m_openAccol);
	}

	private void CheckBodyStyle()
	{
		RPLElementStyle style = m_pageContent.PageLayout.Style;
		string text = (string)style[(byte)34];
		m_pageHasStyle = text != null || style[(byte)33] != null || ReportPageHasBorder((IRPLStyle)(object)style, text);
	}

	private bool ReportPageBorder(IRPLStyle pageStyle, Border border, string backgroundColor)
	{
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		byte b = 0;
		byte b2 = 0;
		byte b3 = 0;
		bool result = false;
		string text = null;
		string text2 = null;
		switch (border)
		{
		case Border.All:
			b = 10;
			b2 = 5;
			b3 = 0;
			break;
		case Border.Bottom:
			b = 14;
			b2 = 9;
			b3 = 4;
			break;
		case Border.Left:
			b = 11;
			b2 = 6;
			b3 = 1;
			break;
		case Border.Right:
			b = 12;
			b2 = 7;
			b3 = 2;
			break;
		default:
			b = 13;
			b2 = 8;
			b3 = 3;
			break;
		}
		object obj = pageStyle[b2];
		if (obj != null && (int)(BorderStyles)obj != 0)
		{
			text = (string)pageStyle[b];
			if (text != null && new RPLReportSize(text).ToMillimeters() > 0.0)
			{
				text2 = (string)pageStyle[b3];
				if (text2 != backgroundColor)
				{
					result = true;
				}
			}
		}
		return result;
	}

	private void BorderBottomAttribute(BorderAttribute attribute)
	{
		if (attribute == BorderAttribute.BorderColor)
		{
			WriteStream(m_borderBottomColor);
		}
		if (attribute == BorderAttribute.BorderStyle)
		{
			WriteStream(m_borderBottomStyle);
		}
		if (attribute == BorderAttribute.BorderWidth)
		{
			WriteStream(m_borderBottomWidth);
		}
	}

	private void BorderLeftAttribute(BorderAttribute attribute)
	{
		if (attribute == BorderAttribute.BorderColor)
		{
			WriteStream(m_borderLeftColor);
		}
		if (attribute == BorderAttribute.BorderStyle)
		{
			WriteStream(m_borderLeftStyle);
		}
		if (attribute == BorderAttribute.BorderWidth)
		{
			WriteStream(m_borderLeftWidth);
		}
	}

	private void BorderRightAttribute(BorderAttribute attribute)
	{
		if (attribute == BorderAttribute.BorderColor)
		{
			WriteStream(m_borderRightColor);
		}
		if (attribute == BorderAttribute.BorderStyle)
		{
			WriteStream(m_borderRightStyle);
		}
		if (attribute == BorderAttribute.BorderWidth)
		{
			WriteStream(m_borderRightWidth);
		}
	}

	private void BorderTopAttribute(BorderAttribute attribute)
	{
		if (attribute == BorderAttribute.BorderColor)
		{
			WriteStream(m_borderTopColor);
		}
		if (attribute == BorderAttribute.BorderStyle)
		{
			WriteStream(m_borderTopStyle);
		}
		if (attribute == BorderAttribute.BorderWidth)
		{
			WriteStream(m_borderTopWidth);
		}
	}

	private void BorderAllAtribute(BorderAttribute attribute)
	{
		if (attribute == BorderAttribute.BorderColor)
		{
			WriteStream(m_borderColor);
		}
		if (attribute == BorderAttribute.BorderStyle)
		{
			WriteStream(m_borderStyle);
		}
		if (attribute == BorderAttribute.BorderWidth)
		{
			WriteStream(m_borderWidth);
		}
	}

	private void RenderBorder(object styleAttribute, Border border, BorderAttribute borderAttribute)
	{
		if (styleAttribute != null)
		{
			switch (border)
			{
			case Border.All:
				BorderAllAtribute(borderAttribute);
				break;
			case Border.Bottom:
				BorderBottomAttribute(borderAttribute);
				break;
			case Border.Right:
				BorderRightAttribute(borderAttribute);
				break;
			case Border.Top:
				BorderTopAttribute(borderAttribute);
				break;
			default:
				BorderLeftAttribute(borderAttribute);
				break;
			}
			WriteStream(styleAttribute);
			WriteStream(m_semiColon);
		}
	}

	private void RenderBorderStyle(object width, object style, object color, Border border)
	{
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		if (width == null && color == null && style == null)
		{
			return;
		}
		if (width != null && color != null && style != null)
		{
			string value = EnumStrings.GetValue((BorderStyles)style);
			switch (border)
			{
			case Border.All:
				WriteStream(m_border);
				break;
			case Border.Bottom:
				WriteStream(m_borderBottom);
				break;
			case Border.Left:
				WriteStream(m_borderLeft);
				break;
			case Border.Right:
				WriteStream(m_borderRight);
				break;
			default:
				WriteStream(m_borderTop);
				break;
			}
			WriteStream(width);
			WriteStream(m_space);
			WriteStream(value);
			WriteStream(m_space);
			WriteStream(color);
			WriteStream(m_semiColon);
		}
		else
		{
			RenderBorder(color, border, BorderAttribute.BorderColor);
			if (style != null)
			{
				string value2 = EnumStrings.GetValue((BorderStyles)style);
				RenderBorder(value2, border, BorderAttribute.BorderStyle);
			}
			RenderBorder(width, border, BorderAttribute.BorderWidth);
		}
	}

	protected bool BorderInstance(IRPLStyle reportItemStyle, object defWidth, object defStyle, object defColor, ref object borderWidth, ref object borderStyle, ref object borderColor, Border border)
	{
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		byte b = 0;
		byte b2 = 0;
		byte b3 = 0;
		switch (border)
		{
		case Border.Bottom:
			b = 14;
			b2 = 9;
			b3 = 4;
			break;
		case Border.Left:
			b = 11;
			b2 = 6;
			b3 = 1;
			break;
		case Border.Right:
			b = 12;
			b2 = 7;
			b3 = 2;
			break;
		case Border.Top:
			b = 13;
			b2 = 8;
			b3 = 3;
			break;
		}
		if (reportItemStyle != null)
		{
			borderStyle = reportItemStyle[b2];
		}
		if (borderStyle == null)
		{
			borderStyle = defStyle;
		}
		if (borderStyle != null && (int)(BorderStyles)borderStyle == 0)
		{
			return false;
		}
		object obj = reportItemStyle[b];
		if (obj == null)
		{
			borderWidth = defWidth;
		}
		else
		{
			borderWidth = obj;
		}
		object obj2 = reportItemStyle[b3];
		if (obj2 == null)
		{
			borderColor = defColor;
		}
		else
		{
			borderColor = obj2;
		}
		if (borderStyle == null && obj == null)
		{
			return obj2 != null;
		}
		return true;
	}

	private bool RenderBorderInstance(IRPLStyle reportItemStyle, object defWidth, object defStyle, object defColor, Border border)
	{
		object borderWidth = null;
		object borderColor = null;
		object borderStyle = null;
		bool flag = BorderInstance(reportItemStyle, defWidth, defStyle, defColor, ref borderWidth, ref borderStyle, ref borderColor, border);
		if (flag)
		{
			RenderBorderStyle(borderWidth, borderStyle, borderColor, border);
		}
		return flag;
	}

	private bool OnlyGeneralBorder(IRPLStyle style)
	{
		bool result = true;
		if (style[(byte)1] != null || style[(byte)11] != null || style[(byte)6] != null || style[(byte)3] != null || style[(byte)13] != null || style[(byte)8] != null || style[(byte)2] != null || style[(byte)12] != null || style[(byte)7] != null || style[(byte)4] != null || style[(byte)14] != null || style[(byte)9] != null)
		{
			result = false;
		}
		return result;
	}

	protected string CreateImageStream(RPLImageData image)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		if (image.ImageName == null)
		{
			return null;
		}
		if (image.IsShared && m_images.ContainsKey(image.ImageName))
		{
			return image.ImageName;
		}
		if ((int)m_createSecondaryStreams == 0)
		{
			Stream stream = CreateStream(image.ImageName, string.Empty, null, image.ImageMimeType, willSeek: false, StreamOper.CreateAndRegister);
			long imageDataOffset = image.ImageDataOffset;
			if (imageDataOffset >= 0)
			{
				m_rplReport.GetImage(imageDataOffset, stream);
			}
			else if (image.ImageData != null)
			{
				stream.Write(image.ImageData, 0, image.ImageData.Length);
			}
		}
		if (image.IsShared)
		{
			m_images.Add(image.ImageName, null);
		}
		return image.ImageName;
	}

	private void RenderAtStart(RPLTextBoxProps textBoxProps, IRPLStyle style, bool renderSort, bool renderToggle)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Expected I4, but got Unknown
		if (!renderSort && !renderToggle)
		{
			return;
		}
		object obj = style[(byte)26];
		VerticalAlignments val = (VerticalAlignments)0;
		if (obj != null)
		{
			val = (VerticalAlignments)obj;
		}
		if (IsWritingModeVertical(style) && m_deviceInfo.IsBrowserIE)
		{
			WriteStream(m_openStyle);
			WriteStream(m_textAlign);
			VerticalAlignments val2 = val;
			switch ((int)val2)
			{
			case 0:
				WriteStream(m_rightValue);
				break;
			case 1:
				WriteStream(m_centerValue);
				break;
			case 2:
				WriteStream(m_leftValue);
				break;
			}
			WriteStream(m_quote);
			WriteStream(m_closeBracket);
			if (renderSort)
			{
				RenderSortImage(textBoxProps);
			}
			if (renderToggle)
			{
				RenderToggleImage(textBoxProps);
			}
			WriteStream(m_closeTD);
			WriteStream(m_closeTR);
			WriteStream(m_firstTD);
		}
		else
		{
			WriteStream(m_openStyle);
			WriteStream(m_verticalAlign);
			WriteStream(EnumStrings.GetValue(val));
			WriteStream(m_quote);
			WriteStream(m_closeBracket);
			if (renderSort)
			{
				RenderSortImage(textBoxProps);
			}
			if (renderToggle)
			{
				RenderToggleImage(textBoxProps);
			}
			WriteStream(m_closeTD);
			WriteStream(m_openTD);
		}
	}

	private void RenderAtEnd(RPLTextBoxProps textBoxProps, IRPLStyle style, bool renderSort, bool renderToggle)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Expected I4, but got Unknown
		if (!renderSort && !renderToggle)
		{
			return;
		}
		object obj = style[(byte)26];
		VerticalAlignments val = (VerticalAlignments)0;
		if (obj != null)
		{
			val = (VerticalAlignments)obj;
		}
		WriteStream(m_closeTD);
		if (IsWritingModeVertical(style) && m_deviceInfo.IsBrowserIE)
		{
			WriteStream(m_closeTR);
			WriteStream(m_firstTD);
			WriteStream(m_openStyle);
			WriteStream(m_textAlign);
			VerticalAlignments val2 = val;
			switch ((int)val2)
			{
			case 0:
				WriteStream(m_rightValue);
				break;
			case 1:
				WriteStream(m_centerValue);
				break;
			case 2:
				WriteStream(m_leftValue);
				break;
			}
			WriteStream(m_quote);
		}
		else
		{
			WriteStream(m_openTD);
			WriteStream(m_openStyle);
			WriteStream(m_verticalAlign);
			WriteStream(EnumStrings.GetValue(val));
			WriteStream(m_quote);
		}
		WriteStream(m_closeBracket);
		if (renderSort)
		{
			RenderSortImage(textBoxProps);
		}
		if (renderToggle)
		{
			RenderToggleImage(textBoxProps);
		}
	}

	private bool RenderHyperlink(RPLAction action, TextDecorations textDec, string color)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		WriteStream(m_openA);
		RenderTabIndex();
		RenderActionHref(action, textDec, color);
		WriteStream(m_closeBracket);
		return true;
	}

	private void RenderTabIndex()
	{
		WriteStream(m_tabIndex);
		WriteStream(++m_tabIndexNum);
		WriteStream(m_quote);
	}

	private bool HasAction(RPLAction action)
	{
		if (action.BookmarkLink == null && action.DrillthroughId == null && action.DrillthroughUrl == null)
		{
			return action.Hyperlink != null;
		}
		return true;
	}

	private bool HasAction(RPLActionInfo actionInfo)
	{
		if (actionInfo != null && actionInfo.Actions != null)
		{
			return HasAction(actionInfo.Actions[0]);
		}
		return false;
	}

	protected abstract void RenderInteractionAction(RPLAction action, ref bool hasHref);

	private bool RenderActionHref(RPLAction action, TextDecorations textDec, string color)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Invalid comparison between Unknown and I4
		bool hasHref = false;
		if (action.Hyperlink != null)
		{
			WriteStream(m_hrefString + HttpUtility.HtmlAttributeEncode(action.Hyperlink) + m_quoteString);
			hasHref = true;
		}
		else
		{
			RenderInteractionAction(action, ref hasHref);
		}
		if ((int)textDec != 1)
		{
			OpenStyle();
			WriteStream(m_textDecoration);
			WriteStream(m_none);
			WriteStream(m_semiColon);
		}
		if (color != null)
		{
			OpenStyle();
			WriteStream(m_color);
			WriteStream(color);
		}
		CloseStyle(renderQuote: true);
		if (m_deviceInfo.LinkTarget != null)
		{
			WriteStream(m_target);
			WriteStream(m_deviceInfo.LinkTarget);
			WriteStream(m_quote);
		}
		return hasHref;
	}

	protected void RenderControlActionScript(RPLAction action)
	{
		StringBuilder stringBuilder = new StringBuilder();
		string text = null;
		if (action.DrillthroughId != null)
		{
			QuoteString(stringBuilder, action.DrillthroughId);
			text = "Drillthrough";
		}
		else
		{
			QuoteString(stringBuilder, action.BookmarkLink);
			text = "Bookmark";
		}
		RenderOnClickActionScript(text, stringBuilder.ToString());
	}

	internal static bool IsDirectionRTL(IRPLStyle style)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Invalid comparison between Unknown and I4
		object obj = style[(byte)29];
		if (obj != null)
		{
			return (int)(Directions)obj == 1;
		}
		return false;
	}

	internal static bool IsWritingModeVertical(IRPLStyle style)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		if (style == null)
		{
			return false;
		}
		object obj = style[(byte)30];
		if (obj != null)
		{
			return HTML4Renderer.IsWritingModeVertical((WritingModes)obj);
		}
		return false;
	}

	internal static bool IsWritingModeVertical(WritingModes writingMode)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Invalid comparison between Unknown and I4
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Invalid comparison between Unknown and I4
		if ((int)writingMode != 1)
		{
			return (int)writingMode == 2;
		}
		return true;
	}

	internal static bool HasHorizontalPaddingStyles(IRPLStyle style)
	{
		if (style != null)
		{
			if (style[(byte)15] == null)
			{
				return style[(byte)16] != null;
			}
			return true;
		}
		return false;
	}

	private void PercentSizes()
	{
		WriteStream(m_openStyle);
		WriteStream(m_styleHeight);
		WriteStream(m_percent);
		WriteStream(m_semiColon);
		WriteStream(m_styleWidth);
		WriteStream(m_percent);
		WriteStream(m_quote);
	}

	private void PercentSizesOverflow()
	{
		WriteStream(m_openStyle);
		WriteStream(m_styleHeight);
		WriteStream(m_percent);
		WriteStream(m_semiColon);
		WriteStream(m_styleWidth);
		WriteStream(m_percent);
		WriteStream(m_semiColon);
		WriteStream(m_overflowHidden);
		WriteStream(m_quote);
	}

	private void ClassLayoutBorder()
	{
		WriteClassName(m_layoutBorder, m_classLayoutBorder);
	}

	private void ClassPercentSizes()
	{
		WriteClassName(m_percentSizes, m_classPercentSizes);
	}

	private void ClassPercentSizesOverflow()
	{
		WriteClassName(m_percentSizesOverflow, m_classPercentSizesOverflow);
	}

	private void ClassPercentHeight()
	{
		WriteClassName(m_percentHeight, m_classPercentHeight);
	}

	private void RenderLanguage(string language)
	{
		if (!string.IsNullOrEmpty(language))
		{
			WriteStream(m_language);
			WriteAttrEncoded(language);
			WriteStream(m_quote);
		}
	}

	private void RenderReportLanguage()
	{
		RenderLanguage(m_contextLanguage);
	}

	private bool InitFixedColumnHeaders(RPLTablix tablix, string tablixID, TablixFixedHeaderStorage storage)
	{
		for (int i = 0; i < tablix.RowHeights.Length; i++)
		{
			if (tablix.FixedRow(i))
			{
				storage.HtmlId = tablixID;
				storage.ColumnHeaders = new List<string>();
				return true;
			}
		}
		return false;
	}

	private bool InitFixedRowHeaders(RPLTablix tablix, string tablixID, TablixFixedHeaderStorage storage)
	{
		for (int i = 0; i < tablix.ColumnWidths.Length; i++)
		{
			if (tablix.FixedColumns[i])
			{
				storage.HtmlId = tablixID;
				storage.RowHeaders = new List<string>();
				return true;
			}
		}
		return false;
	}

	private void RenderVMLLine(RPLLine line, RPLItemMeasurement measurement, StyleContext styleContext)
	{
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		if (!m_hasSlantedLines)
		{
			WriteStream("<?XML:NAMESPACE PREFIX=v /><?IMPORT NAMESPACE=\"v\" IMPLEMENTATION=\"#default#VML\" />");
			m_hasSlantedLines = true;
		}
		WriteStream(m_openVGroup);
		WriteStream(m_openStyle);
		WriteStream(m_styleWidth);
		if (styleContext.InTablix)
		{
			WriteStream(m_percent);
			WriteStream(m_semiColon);
			WriteStream(m_styleHeight);
			WriteStream(m_percent);
		}
		else
		{
			WriteRSStream(((RPLSizes)measurement).Width);
			WriteStream(m_semiColon);
			WriteStream(m_styleHeight);
			WriteRSStream(((RPLSizes)measurement).Height);
		}
		WriteStream(m_closeQuote);
		WriteStream(m_openVLine);
		if (((RPLLinePropsDef)((RPLElement)line).ElementProps.Definition).Slant)
		{
			WriteStream(m_rightSlant);
		}
		else
		{
			WriteStream(m_leftSlant);
		}
		IRPLStyle style = (IRPLStyle)(object)((RPLElement)line).ElementProps.Style;
		string text = (string)style[(byte)0];
		string text2 = (string)style[(byte)10];
		if (text != null && text2 != null)
		{
			int num = new RPLReportColor(text).ToColor().ToArgb() & 0xFFFFFF;
			WriteStream(m_strokeColor);
			WriteStream("#");
			WriteStream(Convert.ToString(num, 16));
			WriteStream(m_quote);
			WriteStream(m_strokeWeight);
			WriteStream(text2);
			WriteStream(m_closeQuote);
		}
		string theString = "solid";
		string text3 = null;
		object obj = style[(byte)5];
		if (obj != null)
		{
			string value = EnumStrings.GetValue((BorderStyles)obj);
			if (string.CompareOrdinal(value, "dashed") == 0)
			{
				theString = "dash";
			}
			else if (string.CompareOrdinal(value, "dotted") == 0)
			{
				theString = "dot";
			}
			if (string.CompareOrdinal(value, "double") == 0)
			{
				text3 = "thinthin";
			}
		}
		WriteStream(m_dashStyle);
		WriteStream(theString);
		if (text3 != null)
		{
			WriteStream(m_quote);
			WriteStream(m_slineStyle);
			WriteStream(text3);
		}
		WriteStream(m_quote);
		WriteStream(m_closeTag);
		WriteStreamCR(m_closeVGroup);
	}

	private List<string> RenderTableCellBorder(PageTableCell currCell, Hashtable renderedLines)
	{
		RPLLine val = null;
		List<string> list = new List<string>(4);
		if (m_isStyleOpen)
		{
			WriteStream(m_semiColon);
		}
		else
		{
			OpenStyle();
		}
		WriteStream(m_zeroBorderWidth);
		val = currCell.BorderLeft;
		if (val != null)
		{
			WriteStream(m_semiColon);
			WriteStream(m_borderLeft);
			RenderBorderLine((RPLElement)(object)val);
			CheckForLineID(val, list, renderedLines);
		}
		val = currCell.BorderRight;
		if (val != null)
		{
			WriteStream(m_semiColon);
			WriteStream(m_borderRight);
			RenderBorderLine((RPLElement)(object)val);
			CheckForLineID(val, list, renderedLines);
		}
		val = currCell.BorderTop;
		if (val != null)
		{
			WriteStream(m_semiColon);
			WriteStream(m_borderTop);
			RenderBorderLine((RPLElement)(object)val);
			CheckForLineID(val, list, renderedLines);
		}
		val = currCell.BorderBottom;
		if (val != null)
		{
			WriteStream(m_semiColon);
			WriteStream(m_borderBottom);
			RenderBorderLine((RPLElement)(object)val);
			CheckForLineID(val, list, renderedLines);
		}
		return list;
	}

	private void CheckForLineID(RPLLine line, List<string> lineIDs, Hashtable renderedLines)
	{
		RPLElementProps elementProps = ((RPLElement)line).ElementProps;
		string uniqueName = elementProps.UniqueName;
		if (!renderedLines.ContainsKey(uniqueName))
		{
			if (NeedReportItemId((RPLElement)(object)line, elementProps))
			{
				lineIDs.Add(elementProps.UniqueName);
			}
			renderedLines.Add(uniqueName, uniqueName);
		}
	}

	private int GenerateTableLayoutContent(PageTableLayout rgTableGrid, RPLItemMeasurement[] repItemCol, bool bfZeroRowReq, bool bfZeroColReq, bool renderHeight, int borderContext, bool layoutExpand, SharedListLayoutState layoutState, List<RPLTablixMemberCell> omittedHeaders, IRPLStyle style)
	{
		int num = 0;
		int i = 0;
		int num2 = 1;
		int num3 = 1;
		int num4 = 0;
		int num5 = 0;
		bool flag = false;
		bool flag2 = true;
		PageTableCell val = null;
		PageTableCell val2 = null;
		Hashtable renderedLines = new Hashtable();
		int nrRows = rgTableGrid.NrRows;
		int nrCols = rgTableGrid.NrCols;
		int num6 = 0;
		int result = 0;
		bool flag3 = true;
		object defaultBorderStyle = null;
		object specificBorderStyle = null;
		object specificBorderStyle2 = null;
		object specificBorderStyle3 = null;
		object specificBorderStyle4 = null;
		object defaultBorderWidth = null;
		object specificBorderWidth = null;
		object specificBorderWidth2 = null;
		object specificBorderWidth3 = null;
		object specificBorderWidth4 = null;
		if (style != null)
		{
			defaultBorderStyle = style[(byte)5];
			specificBorderStyle = style[(byte)6];
			specificBorderStyle2 = style[(byte)7];
			specificBorderStyle3 = style[(byte)8];
			specificBorderStyle4 = style[(byte)9];
			defaultBorderWidth = style[(byte)10];
			specificBorderWidth = style[(byte)11];
			specificBorderWidth2 = style[(byte)12];
			specificBorderWidth3 = style[(byte)13];
			specificBorderWidth4 = style[(byte)14];
		}
		for (; i < nrRows; i++)
		{
			num4 = nrCols * i;
			val = rgTableGrid.GetCell(num4);
			flag = rgTableGrid.EmptyRow((RPLMeasurement[])(object)repItemCol, false, num4, renderHeight, ref num5);
			WriteStream(m_openTR);
			if (!flag)
			{
				WriteStream(m_valign);
				WriteStream(m_topValue);
				WriteStream(m_quote);
			}
			WriteStream(m_closeBracket);
			flag3 = true;
			for (num = 0; num < nrCols; num++)
			{
				int num7 = num + num4;
				bool flag4 = num == 0;
				if (flag4 && bfZeroColReq)
				{
					WriteStream(m_openTD);
					if (renderHeight || num5 <= 0)
					{
						WriteStream(m_openStyle);
						if (m_deviceInfo.OutlookCompat)
						{
							for (int j = 0; j < nrCols; j++)
							{
								val2 = rgTableGrid.GetCell(num4 + j);
								if (!val2.NeedsRowHeight)
								{
									flag3 = false;
									break;
								}
							}
						}
						if (flag3)
						{
							WriteStream(m_styleHeight);
							float num8 = val.DYValue.Value;
							if (num8 > 0f)
							{
								if (i == 0)
								{
									num8 = SubtractBorderStyles(num8, defaultBorderStyle, specificBorderStyle3, defaultBorderWidth, specificBorderWidth3);
								}
								if (i == rgTableGrid.NrRows - num2)
								{
									num8 = SubtractBorderStyles(num8, defaultBorderStyle, specificBorderStyle4, defaultBorderWidth, specificBorderWidth4);
								}
								if (num8 <= 0f)
								{
									num8 = ((m_deviceInfo.BrowserMode != BrowserMode.Standards || !m_deviceInfo.IsBrowserIE) ? 1f : val.DYValue.Value);
								}
							}
							WriteDStream(num8);
							WriteStream(m_mm);
							WriteStream(m_semiColon);
						}
						WriteStream(m_styleWidth);
						WriteDStream(0f);
						WriteStream(m_mm);
						WriteStream(m_quote);
					}
					else
					{
						WriteStream(m_openStyle);
						WriteStream(m_styleWidth);
						WriteDStream(0f);
						WriteStream(m_mm);
						WriteStream(m_quote);
					}
					WriteStream(m_closeBracket);
					if (omittedHeaders != null)
					{
						for (int k = 0; k < omittedHeaders.Count; k++)
						{
							if (omittedHeaders[k].GroupLabel != null)
							{
								RenderNavigationId(omittedHeaders[k].UniqueName);
							}
						}
					}
					WriteStream(m_closeTD);
				}
				val2 = rgTableGrid.GetCell(num7);
				if (val2.Eaten)
				{
					continue;
				}
				if (!val2.InUse)
				{
					MergeEmptyCells(rgTableGrid, num, i, num4, flag2, val2, nrRows, nrCols, num7);
				}
				WriteStream(m_openTD);
				num2 = val2.RowSpan;
				if (num2 != 1)
				{
					WriteStream(m_rowSpan);
					WriteStream(num2.ToString(CultureInfo.InvariantCulture));
					WriteStream(m_quote);
				}
				if (!flag2 || bfZeroRowReq || layoutState == SharedListLayoutState.Continue || layoutState == SharedListLayoutState.End)
				{
					num3 = val2.ColSpan;
					if (num3 != 1)
					{
						WriteStream(m_colSpan);
						WriteStream(num3.ToString(CultureInfo.InvariantCulture));
						WriteStream(m_quote);
					}
				}
				if (flag4 && !bfZeroColReq && (renderHeight || num5 <= 0))
				{
					float num9 = val.DYValue.Value;
					if (num9 >= 0f && flag3 && (i != nrRows - 1 || !flag || layoutState != SharedListLayoutState.None) && (!m_deviceInfo.OutlookCompat || val2.NeedsRowHeight))
					{
						OpenStyle();
						WriteStream(m_styleHeight);
						if (i == 0)
						{
							num9 = SubtractBorderStyles(num9, defaultBorderStyle, specificBorderStyle3, defaultBorderWidth, specificBorderWidth3);
						}
						if (i == rgTableGrid.NrRows - num2)
						{
							num9 = SubtractBorderStyles(num9, defaultBorderStyle, specificBorderStyle4, defaultBorderWidth, specificBorderWidth4);
						}
						if (num9 <= 0f)
						{
							num9 = ((m_deviceInfo.BrowserMode != BrowserMode.Standards || !m_deviceInfo.IsBrowserIE) ? 1f : val.DYValue.Value);
						}
						WriteDStream(num9);
						WriteStream(m_mm);
					}
				}
				if (m_deviceInfo.OutlookCompat || (flag2 && !bfZeroRowReq && (layoutState == SharedListLayoutState.Start || layoutState == SharedListLayoutState.None)))
				{
					float num10 = 0f;
					for (int l = 0; l < num3; l++)
					{
						num10 += rgTableGrid.GetCell(num + l).DXValue.Value;
					}
					float num11 = num10;
					if (m_isStyleOpen)
					{
						WriteStream(m_semiColon);
					}
					else
					{
						OpenStyle();
					}
					WriteStream(m_styleWidth);
					if (num11 > 0f)
					{
						if (num == 0)
						{
							num11 = SubtractBorderStyles(num11, defaultBorderStyle, specificBorderStyle, defaultBorderWidth, specificBorderWidth);
						}
						if (num == rgTableGrid.NrCols - num3)
						{
							num11 = SubtractBorderStyles(num11, defaultBorderStyle, specificBorderStyle2, defaultBorderWidth, specificBorderWidth2);
						}
						if (num11 <= 0f)
						{
							num11 = ((m_deviceInfo.BrowserMode != BrowserMode.Standards || !m_deviceInfo.IsBrowserIE) ? 1f : num10);
						}
					}
					WriteDStream(num11);
					WriteStream(m_mm);
					WriteStream(m_semiColon);
					WriteStream(m_styleMinWidth);
					WriteDStream(num11);
					WriteStream(m_mm);
					WriteStream(m_semiColon);
					if (flag3 && !val2.InUse && m_deviceInfo.OutlookCompat)
					{
						float num12 = val2.DYValue.Value;
						if (num12 < 558.8f)
						{
							WriteStream(m_styleHeight);
							if (num12 > 0f)
							{
								if (i == 0)
								{
									num12 = SubtractBorderStyles(num12, defaultBorderStyle, specificBorderStyle3, defaultBorderWidth, specificBorderWidth3);
								}
								if (i == rgTableGrid.NrRows - num2)
								{
									num12 = SubtractBorderStyles(num12, defaultBorderStyle, specificBorderStyle4, defaultBorderWidth, specificBorderWidth4);
								}
								if (num12 <= 0f)
								{
									num12 = ((m_deviceInfo.BrowserMode != BrowserMode.Standards || !m_deviceInfo.IsBrowserIE) ? 1f : val2.DYValue.Value);
								}
							}
							WriteDStream(num12);
							WriteStream(m_mm);
							WriteStream(m_semiColon);
						}
					}
				}
				List<string> list = null;
				if (val2.HasBorder)
				{
					list = RenderTableCellBorder(val2, renderedLines);
				}
				if (m_isStyleOpen)
				{
					CloseStyle(renderQuote: false);
					WriteStream(m_closeQuote);
				}
				else
				{
					WriteStream(m_closeBracket);
				}
				if (flag4 && !bfZeroColReq && omittedHeaders != null)
				{
					for (int m = 0; m < omittedHeaders.Count; m++)
					{
						if (omittedHeaders[m].GroupLabel != null)
						{
							RenderNavigationId(omittedHeaders[m].UniqueName);
						}
					}
				}
				if (list != null && list.Count > 0)
				{
					for (int n = 0; n < list.Count; n++)
					{
						RenderNavigationId(list[n]);
					}
				}
				if (val2.InUse)
				{
					int num13 = nrRows - val2.RowSpan + 1;
					if (num13 == i + 1 && val2.KeepBottomBorder)
					{
						num13++;
					}
					int num14 = nrCols - val2.ColSpan + 1;
					if (num14 == num + 1 && val2.KeepRightBorder)
					{
						num14++;
					}
					num6 = GetNewContext(borderContext, i + 1, num + 1, num13, num14);
					if ((num6 & 8) > 0 && val2.Measurement != null)
					{
						float height = ((RPLSizes)val2.Measurement).Height;
						float num15 = val2.DYValue.Value;
						for (int num16 = 1; num16 < val2.RowSpan; num16++)
						{
							num15 += rgTableGrid.GetCell(num7 + num16 * rgTableGrid.NrCols).DYValue.Value;
						}
						if (height < num15)
						{
							num6 &= -9;
						}
					}
					if ((num6 & 2) > 0 && val2.Measurement != null)
					{
						float width = ((RPLSizes)val2.Measurement).Width;
						float num17 = val2.DXValue.Value;
						for (int num18 = 1; num18 < val2.ColSpan; num18++)
						{
							num17 += rgTableGrid.GetCell(num7 + num18).DXValue.Value;
						}
						if (width < num17)
						{
							num6 &= -3;
						}
					}
					RenderCellItem(val2, num6, layoutExpand);
				}
				else if (!m_browserIE && val2.HasBorder)
				{
					RenderBlankImage();
				}
				WriteStream(m_closeTD);
			}
			WriteStream(m_closeTR);
			flag2 = false;
			num5--;
		}
		return result;
	}

	private static void MergeEmptyCells(PageTableLayout rgTableGrid, int x, int y, int currRow, bool firstRow, PageTableCell currCell, int numRows, int numCols, int index)
	{
		int num = index + 1;
		int num2 = currRow + numCols;
		if (currCell.BorderLeft == null && !firstRow)
		{
			while (num < num2)
			{
				PageTableCell cell = rgTableGrid.GetCell(num++);
				if (cell.Eaten || cell.InUse || cell.BorderTop != currCell.BorderTop || cell.BorderBottom != currCell.BorderBottom || cell.BorderLeft != null)
				{
					break;
				}
				cell.Eaten = true;
				currCell.ColSpan += 1;
				currCell.BorderRight = cell.BorderRight;
			}
		}
		int num3 = index;
		int num4 = y + 1;
		num = numCols * num4 + x;
		num2 = numCols * numRows;
		while (num < num2)
		{
			PageTableCell cell2 = rgTableGrid.GetCell(num);
			if (cell2.Eaten || cell2.InUse || cell2.BorderLeft != currCell.BorderLeft || cell2.BorderRight != currCell.BorderRight || cell2.BorderTop != null || (currCell.ColSpan == 1 && currCell.BorderLeft == null && currCell.BorderRight == null))
			{
				break;
			}
			int i = 1;
			PageTableCell val = cell2;
			for (; i < currCell.ColSpan; i++)
			{
				PageTableCell cell3 = rgTableGrid.GetCell(num3 + i);
				PageTableCell cell4 = rgTableGrid.GetCell(num + i);
				if (cell4.InUse || cell4.Eaten || cell4.BorderLeft != null || cell4.BorderRight != cell3.BorderRight || cell4.BorderTop != null || cell4.BorderBottom != val.BorderBottom)
				{
					break;
				}
				val = cell4;
			}
			if (i == currCell.ColSpan)
			{
				currCell.RowSpan += 1;
				currCell.BorderBottom = cell2.BorderBottom;
				for (i = 0; i < currCell.ColSpan; i++)
				{
					PageTableCell cell5 = rgTableGrid.GetCell(num + i);
					cell5.Eaten = true;
				}
				num3 = num;
				num4++;
				num = numCols * num4 + x;
				continue;
			}
			break;
		}
	}

	private void RenderIE7WritingMode(WritingModes writingMode, Directions direction, StyleContext styleContext)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Invalid comparison between Unknown and I4
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Invalid comparison between Unknown and I4
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Invalid comparison between Unknown and I4
		WriteStream(m_writingMode);
		if (IsWritingModeVertical(writingMode))
		{
			if ((int)direction == 1)
			{
				WriteStream(m_btrl);
			}
			else
			{
				WriteStream(m_tbrl);
			}
			if ((int)writingMode == 2)
			{
				WriteRotate270(m_deviceInfo, styleContext, WriteStream);
			}
		}
		else if ((int)direction == 1)
		{
			WriteStream(m_rltb);
		}
		else
		{
			WriteStream(m_lrtb);
		}
		WriteStream(m_semiColon);
	}

	internal static void WriteRotate270(DeviceInfo deviceInfo, StyleContext styleContext, Action<byte[]> WriteStream)
	{
		if (deviceInfo.IsBrowserIE && styleContext != null && !styleContext.StyleOnCell)
		{
			if (!styleContext.RotationApplied)
			{
				WriteStream(m_semiColon);
				WriteStream(m_filter);
				WriteStream(m_basicImageRotation180);
				styleContext.RotationApplied = true;
			}
			if (deviceInfo.OutlookCompat)
			{
				WriteStream(m_semiColon);
				WriteStream(m_msoRotation);
				WriteStream(m_degree90);
			}
		}
	}

	private void RenderDirectionStyles(RPLElement reportItem, RPLElementProps props, RPLElementPropsDef definition, RPLItemMeasurement measurement, IRPLStyle sharedStyleProps, IRPLStyle nonSharedStyleProps, bool isNonSharedStyles, StyleContext styleContext)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Expected O, but got Unknown
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Expected O, but got Unknown
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Expected O, but got Unknown
		IRPLStyle val = (isNonSharedStyles ? nonSharedStyleProps : sharedStyleProps);
		bool flag = HasHorizontalPaddingStyles(sharedStyleProps);
		bool flag2 = HasHorizontalPaddingStyles(nonSharedStyleProps);
		object obj = val[(byte)29];
		Directions? val2 = null;
		if (obj != null)
		{
			val2 = (Directions)obj;
			obj = EnumStrings.GetValue(val2.Value);
			WriteStream(m_direction);
			WriteStream(obj);
			WriteStream(m_semiColon);
		}
		obj = val[(byte)30];
		WritingModes? val3 = null;
		if (obj != null)
		{
			val3 = (WritingModes)obj;
			WriteStream(m_layoutFlow);
			if (IsWritingModeVertical(val3.Value))
			{
				WriteStream(m_verticalIdeographic);
			}
			else
			{
				WriteStream(m_horizontal);
			}
			WriteStream(m_semiColon);
			if (m_deviceInfo.IsBrowserIE && IsWritingModeVertical(val3.Value) && measurement != null && reportItem is RPLTextBox)
			{
				RPLTextBoxPropsDef val4 = (RPLTextBoxPropsDef)definition;
				float height = ((RPLSizes)measurement).Height;
				float num = ((RPLSizes)measurement).Width;
				float adjustedWidth = GetAdjustedWidth(measurement, (IRPLStyle)(object)props.Style);
				if (m_deviceInfo.IsBrowserIE6Or7StandardsMode)
				{
					num = adjustedWidth;
					height = GetAdjustedHeight(measurement, (IRPLStyle)(object)props.Style);
				}
				if (val4.CanGrow)
				{
					if (styleContext != null && styleContext.InTablix && !m_deviceInfo.IsBrowserIE6Or7StandardsMode)
					{
						obj = null;
						if (flag2)
						{
							obj = nonSharedStyleProps[(byte)15];
						}
						if (obj == null && flag)
						{
							obj = sharedStyleProps[(byte)15];
						}
						if (obj != null)
						{
							RPLReportSize val5 = new RPLReportSize(obj as string);
							float num2 = (float)val5.ToMillimeters();
							num -= num2;
						}
						obj = null;
						if (flag2)
						{
							obj = nonSharedStyleProps[(byte)16];
						}
						if (obj == null && flag)
						{
							obj = sharedStyleProps[(byte)16];
						}
						if (obj != null)
						{
							RPLReportSize val6 = new RPLReportSize(obj as string);
							float num3 = (float)val6.ToMillimeters();
							num += num3;
						}
					}
					RenderMeasurementWidth((num >= 0f) ? num : 0f);
				}
				else
				{
					WriteStream(m_overflowHidden);
					WriteStream(m_semiColon);
					RenderMeasurementWidth(num, renderMinWidth: false);
					RenderMeasurementHeight(height);
				}
				RenderMeasurementMinWidth(adjustedWidth);
			}
		}
		if (val3.HasValue && val2.HasValue)
		{
			RenderIE7WritingMode(val3.Value, val2.Value, styleContext);
		}
		else if ((val3.HasValue || val2.HasValue) && isNonSharedStyles)
		{
			if (!val3.HasValue)
			{
				obj = definition.SharedStyle[(byte)30];
				val3 = (WritingModes)obj;
			}
			else if (!val2.HasValue)
			{
				obj = definition.SharedStyle[(byte)29];
				val2 = (Directions)obj;
			}
			RenderIE7WritingMode(val3.Value, val2.Value, styleContext);
		}
	}

	private void RenderReportItemStyle(RPLElement reportItem, RPLItemMeasurement measurement, ref int borderContext)
	{
		RPLElementProps elementProps = reportItem.ElementProps;
		RPLElementPropsDef definition = elementProps.Definition;
		RenderReportItemStyle(reportItem, elementProps, definition, measurement, new StyleContext(), ref borderContext, definition.ID);
	}

	private void RenderReportItemStyle(RPLElement reportItem, RPLItemMeasurement measurement, ref int borderContext, StyleContext styleContext)
	{
		RPLElementProps elementProps = reportItem.ElementProps;
		RPLElementPropsDef definition = elementProps.Definition;
		RenderReportItemStyle(reportItem, elementProps, definition, measurement, styleContext, ref borderContext, definition.ID);
	}

	private void RenderReportItemStyle(RPLElement reportItem, RPLElementProps elementProps, RPLElementPropsDef definition, RPLStyleProps nonSharedStyle, RPLStyleProps sharedStyle, RPLItemMeasurement measurement, StyleContext styleContext, ref int borderContext, string styleID)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Expected O, but got Unknown
		if (m_useInlineStyle)
		{
			OpenStyle();
			RPLElementStyle sharedStyleProps = new RPLElementStyle(nonSharedStyle, sharedStyle);
			RenderStyleProps(reportItem, elementProps, definition, measurement, (IRPLStyle)(object)sharedStyleProps, null, styleContext, ref borderContext, isNonSharedStyles: false);
			if (styleContext.EmptyTextBox)
			{
				WriteStream(m_fontSize);
				WriteFontSizeSmallPoint();
			}
			CloseStyle(renderQuote: true);
			return;
		}
		int borderContext2 = borderContext;
		bool flag = sharedStyle != null && sharedStyle.Count > 0;
		if (nonSharedStyle != null && nonSharedStyle.Count > 0)
		{
			bool renderMeasurements = styleContext.RenderMeasurements;
			if (flag)
			{
				styleContext.RenderMeasurements = false;
			}
			OpenStyle();
			RenderStyleProps(reportItem, elementProps, definition, measurement, (IRPLStyle)(object)sharedStyle, (IRPLStyle)(object)nonSharedStyle, styleContext, ref borderContext2, isNonSharedStyles: true);
			CloseStyle(renderQuote: true);
			styleContext.RenderMeasurements = renderMeasurements;
		}
		if (flag)
		{
			byte[] array = (byte[])m_usedStyles[styleID];
			if (array == null)
			{
				if (m_onlyVisibleStyles)
				{
					int borderContext3 = 0;
					array = RenderSharedStyle(reportItem, elementProps, definition, sharedStyle, nonSharedStyle, measurement, styleID, styleContext, ref borderContext3);
				}
				else
				{
					array = m_encoding.GetBytes(styleID);
					m_usedStyles.Add(styleID, array);
				}
			}
			CloseStyle(renderQuote: true);
			WriteClassStyle(array, close: false);
			byte omitBordersState = styleContext.OmitBordersState;
			if (borderContext != 0 || omitBordersState != 0)
			{
				if (borderContext == 15)
				{
					WriteStream(m_space);
					WriteStream(m_deviceInfo.HtmlPrefixId);
					WriteStream(m_ignoreBorder);
				}
				else
				{
					if ((borderContext & 4) != 0 || (omitBordersState & 1) != 0)
					{
						WriteStream(m_space);
						WriteStream(m_deviceInfo.HtmlPrefixId);
						WriteStream(m_ignoreBorderT);
					}
					if ((borderContext & 1) != 0 || (omitBordersState & 4) != 0)
					{
						WriteStream(m_space);
						WriteStream(m_deviceInfo.HtmlPrefixId);
						WriteStream(m_ignoreBorderL);
					}
					if ((borderContext & 8) != 0 || (omitBordersState & 2) != 0)
					{
						WriteStream(m_space);
						WriteStream(m_deviceInfo.HtmlPrefixId);
						WriteStream(m_ignoreBorderB);
					}
					if ((borderContext & 2) != 0 || (omitBordersState & 8) != 0)
					{
						WriteStream(m_space);
						WriteStream(m_deviceInfo.HtmlPrefixId);
						WriteStream(m_ignoreBorderR);
					}
				}
			}
			if (styleContext.EmptyTextBox)
			{
				WriteStream(m_space);
				WriteStream(m_deviceInfo.HtmlPrefixId);
				WriteStream(m_emptyTextBox);
			}
			WriteStream(m_quote);
			if (!styleContext.NoBorders)
			{
				GetBorderContext((IRPLStyle)(object)sharedStyle, ref borderContext, omitBordersState);
			}
		}
		borderContext |= borderContext2;
	}

	private void GetBorderContext(IRPLStyle styleProps, ref int borderContext, byte omitBordersState)
	{
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		object defWidth = styleProps[(byte)10];
		object obj = styleProps[(byte)5];
		object defColor = styleProps[(byte)0];
		object borderWidth = null;
		object borderStyle = null;
		object borderColor = null;
		if (borderContext != 0 || omitBordersState != 0 || !OnlyGeneralBorder(styleProps))
		{
			if ((borderContext & 8) == 0 && (omitBordersState & 2) == 0 && BorderInstance(styleProps, defWidth, obj, defColor, ref borderWidth, ref borderStyle, ref borderColor, Border.Bottom))
			{
				borderContext |= 8;
			}
			if ((borderContext & 1) == 0 && (omitBordersState & 4) == 0 && BorderInstance(styleProps, defWidth, obj, defColor, ref borderWidth, ref borderStyle, ref borderColor, Border.Left))
			{
				borderContext |= 1;
			}
			if ((borderContext & 2) == 0 && (omitBordersState & 8) == 0 && BorderInstance(styleProps, defWidth, obj, defColor, ref borderWidth, ref borderStyle, ref borderColor, Border.Right))
			{
				borderContext |= 2;
			}
			if ((borderContext & 4) == 0 && (omitBordersState & 1) == 0 && BorderInstance(styleProps, defWidth, obj, defColor, ref borderWidth, ref borderStyle, ref borderColor, Border.Top))
			{
				borderContext |= 4;
			}
		}
		else if (obj != null && (int)(BorderStyles)obj != 0)
		{
			borderContext = 15;
		}
	}

	private void RenderReportItemStyle(RPLElement reportItem, RPLElementProps elementProps, RPLElementPropsDef definition, RPLItemMeasurement measurement, StyleContext styleContext, ref int borderContext, string styleID)
	{
		RenderReportItemStyle(reportItem, elementProps, definition, elementProps.NonSharedStyle, definition.SharedStyle, measurement, styleContext, ref borderContext, styleID);
	}

	private void RenderPercentSizes()
	{
		WriteStream(m_styleHeight);
		WriteStream(m_percent);
		WriteStream(m_semiColon);
		WriteStream(m_styleWidth);
		WriteStream(m_percent);
		WriteStream(m_semiColon);
	}

	private void RenderTextAlign(RPLTextBoxProps props, RPLElementStyle style)
	{
		if (props != null)
		{
			WriteStream(m_textAlign);
			bool flag = GetTextAlignForType(props);
			if (IsDirectionRTL((IRPLStyle)(object)style))
			{
				flag = !flag;
			}
			if (flag)
			{
				WriteStream(m_rightValue);
			}
			else
			{
				WriteStream(m_leftValue);
			}
			WriteStream(m_semiColon);
		}
	}

	internal static bool GetTextAlignForType(RPLTextBoxProps textBoxProps)
	{
		TypeCode typeCode = textBoxProps.TypeCode;
		return GetTextAlignForType(typeCode);
	}

	internal static bool GetTextAlignForType(TypeCode typeCode)
	{
		bool result = false;
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
			result = true;
			break;
		}
		return result;
	}

	private bool HasBorderStyle(object borderStyle)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Invalid comparison between Unknown and I4
		if (borderStyle != null)
		{
			return (int)(BorderStyles)borderStyle != 0;
		}
		return false;
	}

	private float SubtractBorderStyles(float width, object defaultBorderStyle, object specificBorderStyle, object defaultBorderWidth, object specificBorderWidth)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Expected O, but got Unknown
		object obj = null;
		obj = specificBorderWidth;
		if (obj == null)
		{
			obj = defaultBorderWidth;
		}
		if (obj != null && (HasBorderStyle(specificBorderStyle) || HasBorderStyle(defaultBorderStyle)))
		{
			RPLReportSize val = new RPLReportSize(obj as string);
			width -= (float)val.ToMillimeters();
		}
		return width;
	}

	private float GetInnerContainerWidth(RPLMeasurement measurement, IRPLStyle containerStyle)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Expected O, but got Unknown
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Expected O, but got Unknown
		if (measurement == null)
		{
			return -1f;
		}
		float width = ((RPLSizes)measurement).Width;
		float num = 0f;
		object obj = containerStyle[(byte)15];
		if (obj != null)
		{
			RPLReportSize val = new RPLReportSize(obj as string);
			num += (float)val.ToMillimeters();
		}
		obj = containerStyle[(byte)16];
		if (obj != null)
		{
			RPLReportSize val2 = new RPLReportSize(obj as string);
			num += (float)val2.ToMillimeters();
		}
		return width - num;
	}

	private float GetInnerContainerWidthSubtractBorders(RPLItemMeasurement measurement, IRPLStyle containerStyle)
	{
		if (measurement == null)
		{
			return -1f;
		}
		float innerContainerWidth = GetInnerContainerWidth((RPLMeasurement)(object)measurement, containerStyle);
		object defaultBorderStyle = containerStyle[(byte)5];
		object defaultBorderWidth = containerStyle[(byte)10];
		object specificBorderWidth = containerStyle[(byte)11];
		object specificBorderStyle = containerStyle[(byte)6];
		innerContainerWidth = SubtractBorderStyles(innerContainerWidth, defaultBorderStyle, specificBorderStyle, defaultBorderWidth, specificBorderWidth);
		specificBorderWidth = containerStyle[(byte)12];
		specificBorderStyle = containerStyle[(byte)7];
		innerContainerWidth = SubtractBorderStyles(innerContainerWidth, defaultBorderStyle, specificBorderStyle, defaultBorderWidth, specificBorderWidth);
		if (innerContainerWidth <= 0f)
		{
			innerContainerWidth = 1f;
		}
		return innerContainerWidth;
	}

	private float GetAdjustedWidth(RPLItemMeasurement measurement, IRPLStyle style)
	{
		float result = ((RPLSizes)measurement).Width;
		if (m_deviceInfo.BrowserMode == BrowserMode.Standards || !m_deviceInfo.IsBrowserIE)
		{
			result = GetInnerContainerWidthSubtractBorders(measurement, style);
		}
		return result;
	}

	private float GetAdjustedHeight(RPLItemMeasurement measurement, IRPLStyle style)
	{
		float result = ((RPLSizes)measurement).Height;
		if (m_deviceInfo.BrowserMode == BrowserMode.Standards || !m_deviceInfo.IsBrowserIE)
		{
			result = GetInnerContainerHeightSubtractBorders(measurement, style);
		}
		return result;
	}

	private float GetInnerContainerHeight(RPLItemMeasurement measurement, IRPLStyle containerStyle)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Expected O, but got Unknown
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Expected O, but got Unknown
		if (measurement == null)
		{
			return -1f;
		}
		float height = ((RPLSizes)measurement).Height;
		float num = 0f;
		object obj = containerStyle[(byte)17];
		if (obj != null)
		{
			RPLReportSize val = new RPLReportSize(obj as string);
			num += (float)val.ToMillimeters();
		}
		obj = containerStyle[(byte)18];
		if (obj != null)
		{
			RPLReportSize val2 = new RPLReportSize(obj as string);
			num += (float)val2.ToMillimeters();
		}
		return height - num;
	}

	private float GetInnerContainerHeightSubtractBorders(RPLItemMeasurement measurement, IRPLStyle containerStyle)
	{
		if (measurement == null)
		{
			return -1f;
		}
		float innerContainerHeight = GetInnerContainerHeight(measurement, containerStyle);
		object defaultBorderStyle = containerStyle[(byte)5];
		object defaultBorderWidth = containerStyle[(byte)10];
		object specificBorderWidth = containerStyle[(byte)13];
		object specificBorderStyle = containerStyle[(byte)8];
		innerContainerHeight = SubtractBorderStyles(innerContainerHeight, defaultBorderStyle, specificBorderStyle, defaultBorderWidth, specificBorderWidth);
		specificBorderWidth = containerStyle[(byte)14];
		specificBorderStyle = containerStyle[(byte)9];
		innerContainerHeight = SubtractBorderStyles(innerContainerHeight, defaultBorderStyle, specificBorderStyle, defaultBorderWidth, specificBorderWidth);
		if (innerContainerHeight <= 0f)
		{
			innerContainerHeight = 1f;
		}
		return innerContainerHeight;
	}

	private void RenderTextBoxContent(RPLTextBox textBox, RPLTextBoxProps tbProps, RPLTextBoxPropsDef tbDef, string textBoxValue, RPLStyleProps actionStyle, bool renderImages, RPLItemMeasurement measurement, RPLAction textBoxAction)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0326: Unknown result type (might be due to invalid IL or missing references)
		//IL_0334: Unknown result type (might be due to invalid IL or missing references)
		//IL_0356: Unknown result type (might be due to invalid IL or missing references)
		//IL_035c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0361: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
		if (tbDef.IsSimple)
		{
			bool flag = false;
			object obj = null;
			bool flag2 = string.IsNullOrEmpty(textBoxValue);
			if (!flag2 && renderImages)
			{
				obj = ((RPLElementProps)tbProps).Style[(byte)24];
				if (obj != null && (int)(TextDecorations)obj != 0)
				{
					obj = EnumStrings.GetValue((TextDecorations)obj);
					flag = true;
					WriteStream(m_openSpan);
					WriteStream(m_openStyle);
					WriteStream(m_textDecoration);
					WriteStream(obj);
					WriteStream(m_closeQuote);
				}
			}
			if (flag2)
			{
				if (!NeedSharedToggleParent(tbProps))
				{
					WriteStream(m_nbsp);
				}
			}
			else
			{
				List<int> list = null;
				if (!string.IsNullOrEmpty(m_searchText))
				{
					int startIndex = 0;
					int length = m_searchText.Length;
					while ((startIndex = textBoxValue.IndexOf(m_searchText, startIndex, StringComparison.OrdinalIgnoreCase)) != -1)
					{
						if (list == null)
						{
							list = new List<int>(2);
						}
						list.Add(startIndex);
						startIndex += length;
					}
					if (list == null)
					{
						RenderMultiLineText(textBoxValue);
					}
					else
					{
						RenderMultiLineTextWithHits(textBoxValue, list);
					}
				}
				else
				{
					RenderMultiLineText(textBoxValue);
				}
			}
			if (flag)
			{
				WriteStream(m_closeSpan);
			}
			return;
		}
		WriteStream(m_openDiv);
		RPLElementStyle style = ((RPLElementProps)tbProps).Style;
		bool flag3 = false;
		bool flag4 = IsWritingModeVertical((IRPLStyle)(object)style);
		if (!m_deviceInfo.IsBrowserIE || !flag4)
		{
			OpenStyle();
			double num = 0.0;
			if (m_deviceInfo.IsBrowserIE)
			{
				WriteStream(m_overflowXHidden);
				WriteStream(m_semiColon);
			}
			num = 0.0;
			if (measurement != null)
			{
				num = GetInnerContainerWidthSubtractBorders(measurement, (IRPLStyle)(object)((RPLElementProps)tbProps).Style);
			}
			if (tbDef.CanSort && !IsFragment && !IsDirectionRTL((IRPLStyle)(object)((RPLElementProps)tbProps).Style))
			{
				num -= 4.233333269755046;
			}
			if (num > 0.0)
			{
				WriteStream(m_styleWidth);
				WriteRSStream((float)num);
				WriteStream(m_semiColon);
			}
		}
		if (IsDirectionRTL((IRPLStyle)(object)style))
		{
			OpenStyle();
			WriteStream(m_direction);
			WriteStream("rtl");
			CloseStyle(renderQuote: true);
			flag3 = true;
			WriteStream(m_classStyle);
			WriteStream(m_rtlEmbed);
		}
		else
		{
			CloseStyle(renderQuote: true);
		}
		if (textBoxAction != null)
		{
			if (!flag3)
			{
				flag3 = true;
				WriteStream(m_classStyle);
			}
			else
			{
				WriteStream(m_space);
			}
			WriteStream(m_styleAction);
		}
		if (flag3)
		{
			WriteStream(m_quote);
		}
		WriteStream(m_closeBracket);
		TextRunStyleWriter trsw = new TextRunStyleWriter(this);
		ParagraphStyleWriter paragraphStyleWriter = new ParagraphStyleWriter(this, textBox);
		RPLStyleProps nonSharedStyle = ((RPLElementProps)tbProps).NonSharedStyle;
		if (nonSharedStyle != null && (nonSharedStyle[(byte)30] != null || nonSharedStyle[(byte)29] != null))
		{
			paragraphStyleWriter.OutputSharedInNonShared = true;
		}
		RPLParagraph nextParagraph = textBox.GetNextParagraph();
		ListLevelStack listLevelStack = null;
		while (nextParagraph != null)
		{
			RPLElementProps elementProps = ((RPLElement)nextParagraph).ElementProps;
			RPLParagraphProps val = (RPLParagraphProps)(object)((elementProps is RPLParagraphProps) ? elementProps : null);
			RPLElementPropsDef definition = ((RPLElementProps)val).Definition;
			RPLParagraphPropsDef val2 = (RPLParagraphPropsDef)(object)((definition is RPLParagraphPropsDef) ? definition : null);
			int num2 = val.ListLevel ?? val2.ListLevel;
			ListStyles val3 = (ListStyles)(((_003F?)val.ListStyle) ?? val2.ListStyle);
			string text = null;
			RPLStyleProps nonSharedStyle2 = ((RPLElementProps)val).NonSharedStyle;
			RPLStyleProps shared = null;
			if (val2 != null)
			{
				if (num2 == 0)
				{
					num2 = val2.ListLevel;
				}
				if ((int)val3 == 0)
				{
					val3 = val2.ListStyle;
				}
				text = ((RPLElementPropsDef)val2).ID;
				if (!paragraphStyleWriter.OutputSharedInNonShared)
				{
					shared = ((RPLElementPropsDef)val2).SharedStyle;
				}
			}
			paragraphStyleWriter.Paragraph = nextParagraph;
			paragraphStyleWriter.ParagraphMode = ParagraphStyleWriter.Mode.All;
			paragraphStyleWriter.CurrentListLevel = num2;
			byte[] array = null;
			if (num2 > 0)
			{
				if (listLevelStack == null)
				{
					listLevelStack = new ListLevelStack();
				}
				bool writeNoVerticalMargin = !m_deviceInfo.IsBrowserIE || !flag4 || (m_deviceInfo.BrowserMode == BrowserMode.Standards && !m_deviceInfo.IsBrowserIE6Or7StandardsMode);
				listLevelStack.PushTo(this, num2, val3, writeNoVerticalMargin);
				if ((int)val3 != 0)
				{
					if (m_deviceInfo.BrowserMode == BrowserMode.Quirks || m_deviceInfo.IsBrowserIE6Or7StandardsMode)
					{
						WriteStream(m_openDiv);
						WriteStream(m_closeBracket);
					}
					WriteStream(m_openLi);
					paragraphStyleWriter.ParagraphMode = ParagraphStyleWriter.Mode.ListOnly;
					WriteStyles(text + "l", nonSharedStyle2, shared, paragraphStyleWriter);
					WriteStream(m_closeBracket);
					array = m_closeLi;
					paragraphStyleWriter.ParagraphMode = ParagraphStyleWriter.Mode.ParagraphOnly;
					text += "p";
				}
			}
			else if (listLevelStack != null)
			{
				listLevelStack.PopAll();
				listLevelStack = null;
			}
			WriteStream(m_openDiv);
			WriteStyles(text, nonSharedStyle2, shared, paragraphStyleWriter);
			WriteStream(m_closeBracket);
			RPLReportSize hangingIndent = val.HangingIndent;
			if (hangingIndent == null)
			{
				hangingIndent = val2.HangingIndent;
			}
			float num3 = 0f;
			if (hangingIndent != null)
			{
				num3 = (float)hangingIndent.ToMillimeters();
			}
			if (num3 > 0f)
			{
				WriteStream(m_openSpan);
				OpenStyle();
				RenderMeasurementWidth(num3, renderMinWidth: true);
				WriteStream(m_styleDisplayInlineBlock);
				CloseStyle(renderQuote: true);
				WriteStream(m_closeBracket);
				if (m_deviceInfo.IsBrowserGeckoEngine)
				{
					WriteStream(m_nbsp);
				}
				WriteStream(m_closeSpan);
			}
			RenderTextRuns(nextParagraph, trsw, textBoxAction);
			WriteStream(m_closeDiv);
			if (array != null)
			{
				WriteStream(array);
				if (m_deviceInfo.BrowserMode == BrowserMode.Quirks || m_deviceInfo.IsBrowserIE6Or7StandardsMode)
				{
					WriteStream(m_closeDiv);
				}
			}
			nextParagraph = textBox.GetNextParagraph();
		}
		listLevelStack?.PopAll();
		WriteStream(m_closeDiv);
	}

	private void RenderTextRuns(RPLParagraph paragraph, TextRunStyleWriter trsw, RPLAction textBoxAction)
	{
		int num = 0;
		RPLTextRun val = null;
		if (!string.IsNullOrEmpty(m_searchText))
		{
			RPLTextRun nextTextRun = paragraph.GetNextTextRun();
			val = nextTextRun;
			List<RPLTextRun> list = new List<RPLTextRun>();
			StringBuilder stringBuilder = new StringBuilder();
			while (nextTextRun != null)
			{
				list.Add(nextTextRun);
				RPLElementProps elementProps = ((RPLElement)nextTextRun).ElementProps;
				string value = ((RPLTextRunProps)((elementProps is RPLTextRunProps) ? elementProps : null)).Value;
				if (string.IsNullOrEmpty(value))
				{
					RPLElementPropsDef elementPropsDef = ((RPLElement)nextTextRun).ElementPropsDef;
					value = ((RPLTextRunPropsDef)((elementPropsDef is RPLTextRunPropsDef) ? elementPropsDef : null)).Value;
				}
				stringBuilder.Append(value);
				nextTextRun = paragraph.GetNextTextRun();
			}
			string text = stringBuilder.ToString();
			int num2 = text.IndexOf(m_searchText, StringComparison.OrdinalIgnoreCase);
			List<int> list2 = new List<int>();
			int num3 = 0;
			int num4 = 0;
			int runOffsetCount = 0;
			int length = m_searchText.Length;
			for (int i = 0; i < list.Count; i++)
			{
				nextTextRun = list[i];
				RPLElementProps elementProps2 = ((RPLElement)nextTextRun).ElementProps;
				string value2 = ((RPLTextRunProps)((elementProps2 is RPLTextRunProps) ? elementProps2 : null)).Value;
				if (string.IsNullOrEmpty(value2))
				{
					RPLElementPropsDef elementPropsDef2 = ((RPLElement)nextTextRun).ElementPropsDef;
					value2 = ((RPLTextRunPropsDef)((elementPropsDef2 is RPLTextRunPropsDef) ? elementPropsDef2 : null)).Value;
				}
				if (string.IsNullOrEmpty(value2))
				{
					continue;
				}
				while (num2 > -1 && num2 < num3 + value2.Length)
				{
					list2.Add(num2 - num3);
					num2 = text.IndexOf(m_searchText, num2 + length, StringComparison.OrdinalIgnoreCase);
				}
				if (list2.Count > 0 || num4 > 0)
				{
					num += RenderTextRunFindString(nextTextRun, list2, num4, ref runOffsetCount, trsw, textBoxAction);
					if (num4 > 0)
					{
						num4 -= value2.Length;
						if (num4 < 0)
						{
							num4 = 0;
						}
					}
					if (list2.Count > 0)
					{
						int num5 = list2[list2.Count - 1];
						list2.Clear();
						if (value2.Length < num5 + length)
						{
							num4 = length - (value2.Length - num5);
						}
					}
				}
				else
				{
					num += RenderTextRun(nextTextRun, trsw, textBoxAction);
				}
				num3 += value2.Length;
			}
		}
		else
		{
			RPLTextRun nextTextRun2 = paragraph.GetNextTextRun();
			val = nextTextRun2;
			while (nextTextRun2 != null)
			{
				num += RenderTextRun(nextTextRun2, trsw, textBoxAction);
				nextTextRun2 = paragraph.GetNextTextRun();
			}
		}
		if (num == 0 && val != null)
		{
			RPLElementProps elementProps3 = ((RPLElement)val).ElementProps;
			RPLTextRunProps val2 = (RPLTextRunProps)(object)((elementProps3 is RPLTextRunProps) ? elementProps3 : null);
			RPLElementPropsDef definition = ((RPLElementProps)val2).Definition;
			WriteStream(m_openSpan);
			WriteStyles(definition.ID, ((RPLElementProps)val2).NonSharedStyle, definition.SharedStyle, trsw);
			WriteStream(m_closeBracket);
			WriteStream(m_nbsp);
			WriteStream(m_closeSpan);
		}
	}

	private int RenderTextRunFindString(RPLTextRun textRun, List<int> hits, int remainingChars, ref int runOffsetCount, TextRunStyleWriter trsw, RPLAction textBoxAction)
	{
		RPLElementProps elementProps = ((RPLElement)textRun).ElementProps;
		RPLTextRunProps val = (RPLTextRunProps)(object)((elementProps is RPLTextRunProps) ? elementProps : null);
		RPLElementPropsDef definition = ((RPLElementProps)val).Definition;
		RPLTextRunPropsDef val2 = (RPLTextRunPropsDef)(object)((definition is RPLTextRunPropsDef) ? definition : null);
		RPLStyleProps shared = null;
		string id = null;
		string value = val.Value;
		string toolTip = val.ToolTip;
		if (val2 != null)
		{
			shared = ((RPLElementPropsDef)val2).SharedStyle;
			id = ((RPLElementPropsDef)val2).ID;
			if (string.IsNullOrEmpty(value))
			{
				value = val2.Value;
			}
			if (string.IsNullOrEmpty(toolTip))
			{
				toolTip = val2.ToolTip;
			}
		}
		if (string.IsNullOrEmpty(value))
		{
			return 0;
		}
		byte[] theBytes = m_closeSpan;
		RPLAction val3 = null;
		if (textBoxAction == null && HasAction(val.ActionInfo))
		{
			val3 = val.ActionInfo.Actions[0];
		}
		if (val3 != null)
		{
			WriteStream(m_openA);
			RenderTabIndex();
			RenderActionHref(val3, (TextDecorations)1, null);
			theBytes = m_closeA;
		}
		else
		{
			WriteStream(m_openSpan);
		}
		if (toolTip != null)
		{
			WriteToolTipAttribute(toolTip);
		}
		WriteStyles(id, ((RPLElementProps)val).NonSharedStyle, shared, trsw);
		WriteStream(m_closeBracket);
		int num = 0;
		int num2 = 0;
		int length = value.Length;
		if (remainingChars > 0)
		{
			int num3 = remainingChars;
			if (num3 > length)
			{
				num3 = length;
			}
			if (num3 > 0)
			{
				OutputFindString(value.Substring(0, num3), runOffsetCount++);
				num += num3;
				if (num3 >= remainingChars)
				{
					m_currentHitCount++;
					runOffsetCount = 0;
				}
			}
		}
		int num4 = hits.Count - 1;
		bool flag = false;
		int length2 = m_searchText.Length;
		if (hits.Count > 0)
		{
			num2 = hits[hits.Count - 1];
			if (num2 + length2 > length)
			{
				flag = true;
			}
			else
			{
				num4 = hits.Count;
			}
		}
		for (int i = 0; i < num4; i++)
		{
			num2 = hits[i];
			if (num < num2)
			{
				RenderMultiLineText(value.Substring(num, num2 - num));
			}
			OutputFindString(value.Substring(num2, length2), 0);
			m_currentHitCount++;
			runOffsetCount = 0;
			num = num2 + length2;
		}
		if (flag)
		{
			num2 = hits[hits.Count - 1];
			if (num < num2)
			{
				RenderMultiLineText(value.Substring(num, num2 - num));
			}
			OutputFindString(value.Substring(num2, length - num2), runOffsetCount++);
		}
		else if (num < length)
		{
			RenderMultiLineText(value.Substring(num));
		}
		WriteStream(theBytes);
		return length;
	}

	private int RenderTextRun(RPLTextRun textRun, TextRunStyleWriter trsw, RPLAction textBoxAction)
	{
		RPLElementProps elementProps = ((RPLElement)textRun).ElementProps;
		RPLTextRunProps val = (RPLTextRunProps)(object)((elementProps is RPLTextRunProps) ? elementProps : null);
		RPLElementPropsDef definition = ((RPLElementProps)val).Definition;
		RPLTextRunPropsDef val2 = (RPLTextRunPropsDef)(object)((definition is RPLTextRunPropsDef) ? definition : null);
		RPLStyleProps shared = null;
		string id = null;
		string value = val.Value;
		string toolTip = val.ToolTip;
		if (val2 != null)
		{
			shared = ((RPLElementPropsDef)val2).SharedStyle;
			id = ((RPLElementPropsDef)val2).ID;
			if (string.IsNullOrEmpty(value))
			{
				value = val2.Value;
			}
			if (string.IsNullOrEmpty(toolTip))
			{
				toolTip = val2.ToolTip;
			}
		}
		if (string.IsNullOrEmpty(value))
		{
			return 0;
		}
		byte[] theBytes = m_closeSpan;
		RPLAction val3 = null;
		if (textBoxAction == null)
		{
			val3 = textBoxAction;
			if (HasAction(val.ActionInfo))
			{
				val3 = val.ActionInfo.Actions[0];
			}
		}
		if (val3 != null)
		{
			WriteStream(m_openA);
			RenderTabIndex();
			RenderActionHref(val3, (TextDecorations)1, null);
			theBytes = m_closeA;
		}
		else
		{
			WriteStream(m_openSpan);
		}
		if (toolTip != null)
		{
			WriteToolTipAttribute(toolTip);
		}
		WriteStyles(id, ((RPLElementProps)val).NonSharedStyle, shared, trsw);
		RenderLanguage(((RPLElementProps)val).Style[(byte)32] as string);
		WriteStream(m_closeBracket);
		RenderMultiLineText(value);
		WriteStream(theBytes);
		return value.Length;
	}

	private void WriteStyles(string id, RPLStyleProps nonShared, RPLStyleProps shared, ElementStyleWriter styleWriter)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Expected O, but got Unknown
		bool flag = (shared != null && shared.Count > 0) || styleWriter.NeedsToWriteNullStyle(StyleWriterMode.Shared);
		if (m_useInlineStyle || (flag && id == null))
		{
			OpenStyle();
			styleWriter.WriteStyles(StyleWriterMode.All, (IRPLStyle)new RPLElementStyle(nonShared, shared));
			CloseStyle(renderQuote: true);
			return;
		}
		if ((nonShared != null && nonShared.Count > 0) || styleWriter.NeedsToWriteNullStyle(StyleWriterMode.NonShared))
		{
			OpenStyle();
			styleWriter.WriteStyles(StyleWriterMode.NonShared, (IRPLStyle)(object)nonShared);
			CloseStyle(renderQuote: true);
		}
		if (!flag || id == null)
		{
			return;
		}
		byte[] array = (byte[])m_usedStyles[id];
		if (array == null)
		{
			if (m_onlyVisibleStyles)
			{
				Stream mainStream = m_mainStream;
				m_mainStream = m_styleStream;
				RenderOpenStyle(id);
				styleWriter.WriteStyles(StyleWriterMode.Shared, (IRPLStyle)(object)shared);
				WriteStream(m_closeAccol);
				m_mainStream = mainStream;
				array = m_encoding.GetBytes(id);
				m_usedStyles.Add(id, array);
			}
			else
			{
				array = m_encoding.GetBytes(id);
				m_usedStyles.Add(id, array);
			}
		}
		CloseStyle(renderQuote: true);
		WriteClassStyle(array, close: true);
	}

	protected abstract void WriteFitProportionalScript(double pv, double ph);

	private void RenderImageFitProportional(RPLImage image, RPLItemMeasurement measurement, PaddingSharedInfo padds, bool writeSmallSize)
	{
		if (!m_deviceInfo.AllowScript)
		{
			return;
		}
		m_fitPropImages = true;
		double pv = 0.0;
		double ph = 0.0;
		if (padds != null)
		{
			pv = padds.PadV;
			ph = padds.PadH;
		}
		WriteFitProportionalScript(pv, ph);
		if (writeSmallSize || !m_browserIE)
		{
			long num = 1L;
			WriteStream(m_inlineHeight);
			if (m_deviceInfo.IsBrowserSafari || m_deviceInfo.IsBrowserGeckoEngine)
			{
				num = 5L;
				if (measurement != null)
				{
					double num2 = ((RPLSizes)measurement).Height;
					if ((double)((RPLSizes)measurement).Width < num2)
					{
						num2 = ((RPLSizes)measurement).Width;
					}
					num = Utility.MMToPx(num2);
					if (num < 5)
					{
						num = 5L;
					}
				}
			}
			WriteStream(num.ToString(CultureInfo.InvariantCulture));
			WriteStream(m_px);
			WriteStream(m_quote);
		}
		if (writeSmallSize)
		{
			WriteStream(m_inlineWidth);
			WriteStream("1");
			WriteStream(m_px);
			WriteStream(m_quote);
		}
	}

	private void RenderImagePercent(RPLImage image, RPLImageProps imageProps, RPLImagePropsDef imagePropsDef, RPLItemMeasurement measurement)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Invalid comparison between Unknown and I4
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Invalid comparison between Unknown and I4
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Invalid comparison between Unknown and I4
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Invalid comparison between Unknown and I4
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Invalid comparison between Unknown and I4
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Invalid comparison between Unknown and I4
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Invalid comparison between Unknown and I4
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Invalid comparison between Unknown and I4
		//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Invalid comparison between Unknown and I4
		//IL_0394: Unknown result type (might be due to invalid IL or missing references)
		//IL_0397: Invalid comparison between Unknown and I4
		//IL_0399: Unknown result type (might be due to invalid IL or missing references)
		//IL_039c: Invalid comparison between Unknown and I4
		//IL_039e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a1: Invalid comparison between Unknown and I4
		bool flag = false;
		bool flag2 = false;
		RPLImageData image2 = imageProps.Image;
		RPLActionInfo actionInfo = imageProps.ActionInfo;
		Sizings sizing = imagePropsDef.Sizing;
		if ((int)sizing == 2 || (int)sizing == 1 || (int)sizing == 3)
		{
			flag = true;
			WriteStream(m_openDiv);
			if (m_useInlineStyle)
			{
				PercentSizesOverflow();
			}
			else
			{
				ClassPercentSizesOverflow();
			}
			if (measurement != null)
			{
				OpenStyle();
				RenderMeasurementMinWidth(GetInnerContainerWidth((RPLMeasurement)(object)measurement, (IRPLStyle)(object)((RPLElementProps)imageProps).Style));
				RenderMeasurementMinHeight(GetInnerContainerHeight(measurement, (IRPLStyle)(object)((RPLElementProps)imageProps).Style));
				CloseStyle(renderQuote: true);
			}
		}
		int xOffset = 0;
		int yOffset = 0;
		Rectangle imageConsolidationOffsets = imageProps.Image.ImageConsolidationOffsets;
		bool flag3 = !imageConsolidationOffsets.IsEmpty;
		if (flag3)
		{
			if (!flag)
			{
				flag = true;
				WriteStream(m_openDiv);
				if ((int)sizing != 0)
				{
					if (m_useInlineStyle)
					{
						PercentSizesOverflow();
					}
					else
					{
						ClassPercentSizesOverflow();
					}
				}
			}
			if ((int)sizing == 3 || (int)sizing == 2 || (int)sizing == 1)
			{
				WriteStream(m_closeBracket);
				WriteStream(m_openDiv);
				if (m_deviceInfo.IsBrowserIE6 && m_deviceInfo.IsBrowserIE6Or7StandardsMode && measurement != null)
				{
					WriteStream(" origWidth=\"");
					WriteRSStream(((RPLSizes)measurement).Width);
					WriteStream("\" origHeight=\"");
					WriteStream("\"");
				}
			}
			WriteOuterConsolidation(imageConsolidationOffsets, sizing, ((RPLElementProps)imageProps).UniqueName);
			CloseStyle(renderQuote: true);
			xOffset = imageConsolidationOffsets.Left;
			yOffset = imageConsolidationOffsets.Top;
		}
		else if (m_deviceInfo.AllowScript && (int)sizing == 1 && m_deviceInfo.BrowserMode == BrowserMode.Standards)
		{
			flag = true;
			WriteStream(m_openDiv);
			if (m_imgFitDivIdsStream == null)
			{
				CreateImgFitDivImageIdsStream();
			}
			WriteIdToSecondaryStream(m_imgFitDivIdsStream, ((RPLElementProps)imageProps).UniqueName + "_ifd");
			RenderReportItemId(((RPLElementProps)imageProps).UniqueName + "_ifd");
		}
		if (flag)
		{
			WriteStream(m_closeBracket);
		}
		if (HasAction(actionInfo))
		{
			flag2 = RenderElementHyperlink((IRPLStyle)(object)((RPLElementProps)imageProps).Style, actionInfo.Actions[0]);
		}
		WriteStream(m_img);
		if (m_browserIE)
		{
			WriteStream(m_imgOnError);
		}
		if (imageProps.ActionImageMapAreas != null && imageProps.ActionImageMapAreas.Length > 0)
		{
			WriteAttrEncoded(m_useMap, "#" + m_deviceInfo.HtmlPrefixId + m_mapPrefixString + ((RPLElementProps)imageProps).UniqueName);
			WriteStream(m_zeroBorder);
		}
		else if (flag2)
		{
			WriteStream(m_zeroBorder);
		}
		if ((int)sizing == 2)
		{
			PaddingSharedInfo padds = null;
			if (m_deviceInfo.IsBrowserSafari)
			{
				padds = GetPaddings(((RPLElement)image).ElementProps.Style, null);
			}
			bool writeSmallSize = !flag3 && m_deviceInfo.BrowserMode == BrowserMode.Standards;
			RenderImageFitProportional(image, null, padds, writeSmallSize);
		}
		else if ((int)sizing == 1 && !flag3)
		{
			if (m_deviceInfo.AllowScript && m_deviceInfo.BrowserMode == BrowserMode.Standards)
			{
				WriteStream(" width=\"1px\" height=\"1px\"");
			}
			else if (m_useInlineStyle)
			{
				PercentSizes();
			}
			else
			{
				ClassPercentSizes();
			}
		}
		if (flag3)
		{
			WriteClippedDiv(imageConsolidationOffsets);
		}
		WriteToolTip((RPLElementProps)(object)imageProps);
		WriteStream(m_src);
		RenderImageUrl(useSessionId: true, image2);
		WriteStream(m_closeTag);
		if (flag2)
		{
			WriteStream(m_closeA);
		}
		if (imageProps.ActionImageMapAreas != null && imageProps.ActionImageMapAreas.Length > 0)
		{
			RenderImageMapAreas(imageProps.ActionImageMapAreas, ((RPLSizes)measurement).Width, ((RPLSizes)measurement).Height, ((RPLElementProps)imageProps).UniqueName, xOffset, yOffset);
		}
		if (flag3 && ((int)sizing == 3 || (int)sizing == 2 || (int)sizing == 1))
		{
			WriteStream(m_closeDiv);
		}
		if (flag)
		{
			WriteStreamCR(m_closeDiv);
		}
	}

	private void RenderImageMapAreas(RPLActionInfoWithImageMap[] actionImageMaps, double width, double height, string uniqueName, int xOffset, int yOffset)
	{
		RPLActionInfoWithImageMap val = null;
		double imageWidth = width * 96.0 * 0.03937007874;
		double imageHeight = height * 96.0 * 0.03937007874;
		WriteStream(m_openMap);
		WriteAttrEncoded(m_name, m_deviceInfo.HtmlPrefixId + m_mapPrefixString + uniqueName);
		WriteStreamCR(m_closeBracket);
		for (int i = 0; i < actionImageMaps.Length; i++)
		{
			val = actionImageMaps[i];
			if (val != null)
			{
				RenderImageMapArea(val, imageWidth, imageHeight, uniqueName, xOffset, yOffset);
			}
		}
		WriteStream(m_closeMap);
	}

	protected void RenderImageMapArea(RPLActionInfoWithImageMap actionImageMap, double imageWidth, double imageHeight, string uniqueName, int xOffset, int yOffset)
	{
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Expected I4, but got Unknown
		RPLAction val = null;
		if (((RPLActionInfo)actionImageMap).Actions != null && ((RPLActionInfo)actionImageMap).Actions.Length > 0)
		{
			val = ((RPLActionInfo)actionImageMap).Actions[0];
			if (!HasAction(val))
			{
				val = null;
			}
		}
		if (actionImageMap.ImageMaps == null || actionImageMap.ImageMaps.Count <= 0)
		{
			return;
		}
		RPLImageMap val2 = null;
		for (int i = 0; i < actionImageMap.ImageMaps.Count; i++)
		{
			val2 = actionImageMap.ImageMaps[i];
			string toolTip = val2.ToolTip;
			if (val == null && toolTip == null)
			{
				continue;
			}
			WriteStream(m_mapArea);
			RenderTabIndex();
			if (toolTip != null)
			{
				WriteToolTipAttribute(toolTip);
			}
			if (val != null)
			{
				RenderActionHref(val, (TextDecorations)0, null);
			}
			else
			{
				WriteStream(m_nohref);
			}
			WriteStream(m_mapShape);
			ShapeType shape = val2.Shape;
			switch (shape - 1)
			{
			case 1:
				WriteStream(m_circleShape);
				break;
			case 0:
				WriteStream(m_polyShape);
				break;
			default:
				WriteStream(m_rectShape);
				break;
			}
			WriteStream(m_quote);
			WriteStream(m_mapCoords);
			float[] coordinates = val2.Coordinates;
			long num = 0L;
			bool flag = true;
			int j = 0;
			if (coordinates != null)
			{
				for (; j < coordinates.Length - 1; j += 2)
				{
					if (!flag)
					{
						WriteStream(m_comma);
					}
					num = (long)((double)(coordinates[j] / 100f) * imageWidth) + xOffset;
					WriteStream(num);
					WriteStream(m_comma);
					num = (long)((double)(coordinates[j + 1] / 100f) * imageHeight) + yOffset;
					WriteStream(num);
					flag = false;
				}
				if (j < coordinates.Length)
				{
					WriteStream(m_comma);
					num = (long)((double)(coordinates[j] / 100f) * imageWidth);
					WriteStream(num);
				}
			}
			WriteStream(m_quote);
			WriteStreamCR(m_closeBracket);
		}
	}

	protected void RenderCreateFixedHeaderFunction(string prefix, string fixedHeaderObject, StringBuilder function, StringBuilder arrayBuilder, bool createHeadersWithArray)
	{
		int num = 0;
		StringBuilder stringBuilder = function;
		if (createHeadersWithArray)
		{
			stringBuilder = arrayBuilder;
		}
		foreach (TablixFixedHeaderStorage fixedHeader in m_fixedHeaders)
		{
			string text = "frgh" + num + '_' + fixedHeader.HtmlId;
			string text2 = "fcgh" + num + '_' + fixedHeader.HtmlId;
			string text3 = "fch" + num + '_' + fixedHeader.HtmlId;
			string value = m_deviceInfo.HtmlPrefixId + text;
			string value2 = m_deviceInfo.HtmlPrefixId + text2;
			string value3 = m_deviceInfo.HtmlPrefixId + text3;
			if (fixedHeader.ColumnHeaders != null)
			{
				string value4 = prefix + "fcghArr" + num;
				arrayBuilder.Append(value4);
				arrayBuilder.Append("=new Array('");
				arrayBuilder.Append(fixedHeader.HtmlId);
				arrayBuilder.Append('\'');
				for (int i = 0; i < fixedHeader.ColumnHeaders.Count; i++)
				{
					arrayBuilder.Append(",'");
					arrayBuilder.Append(fixedHeader.ColumnHeaders[i]);
					arrayBuilder.Append('\'');
				}
				arrayBuilder.Append(");");
				if (!createHeadersWithArray)
				{
					arrayBuilder.Append(value2);
					arrayBuilder.Append("=null;");
					function.Append("if (!");
					function.Append(value2);
					function.Append("){");
					function.Append(value2);
					function.Append("=");
				}
				stringBuilder.Append(fixedHeaderObject);
				stringBuilder.Append(".CreateFixedColumnHeader(");
				stringBuilder.Append(value4);
				stringBuilder.Append(",'");
				stringBuilder.Append(text2);
				stringBuilder.Append("');");
				if (!createHeadersWithArray)
				{
					function.Append("}");
				}
			}
			if (fixedHeader.RowHeaders != null)
			{
				string value5 = prefix + "frhArr" + num;
				arrayBuilder.Append(value5);
				arrayBuilder.Append("=new Array('");
				arrayBuilder.Append(fixedHeader.HtmlId);
				arrayBuilder.Append('\'');
				for (int j = 0; j < fixedHeader.RowHeaders.Count; j++)
				{
					arrayBuilder.Append(",'");
					arrayBuilder.Append(fixedHeader.RowHeaders[j]);
					arrayBuilder.Append('\'');
				}
				arrayBuilder.Append(");");
				if (!createHeadersWithArray)
				{
					arrayBuilder.Append(value);
					arrayBuilder.Append("=null;");
					function.Append("if (!");
					function.Append(value);
					function.Append("){");
					function.Append(value);
					function.Append("=");
				}
				stringBuilder.Append(fixedHeaderObject);
				stringBuilder.Append(".CreateFixedRowHeader(");
				stringBuilder.Append(value5);
				stringBuilder.Append(",'");
				stringBuilder.Append(text);
				stringBuilder.Append("');");
				if (!createHeadersWithArray)
				{
					function.Append("}");
				}
			}
			if (fixedHeader.CornerHeaders != null)
			{
				string value6 = prefix + "fchArr" + num;
				arrayBuilder.Append(value6);
				arrayBuilder.Append("=new Array('");
				arrayBuilder.Append(fixedHeader.HtmlId);
				arrayBuilder.Append('\'');
				for (int k = 0; k < fixedHeader.CornerHeaders.Count; k++)
				{
					arrayBuilder.Append(",'");
					arrayBuilder.Append(fixedHeader.CornerHeaders[k]);
					arrayBuilder.Append('\'');
				}
				arrayBuilder.Append(");");
				if (!createHeadersWithArray)
				{
					arrayBuilder.Append(value3);
					arrayBuilder.Append("=null;");
					function.Append("if (!");
					function.Append(value3);
					function.Append("){");
					function.Append(value3);
					function.Append("=");
				}
				stringBuilder.Append(fixedHeaderObject);
				stringBuilder.Append(".CreateFixedRowHeader(");
				stringBuilder.Append(value6);
				stringBuilder.Append(",'");
				stringBuilder.Append(text3);
				stringBuilder.Append("');");
				if (!createHeadersWithArray)
				{
					function.Append("}");
				}
			}
			function.Append(fixedHeaderObject);
			function.Append(".ShowFixedTablixHeaders('");
			function.Append(fixedHeader.HtmlId);
			function.Append("','");
			function.Append((fixedHeader.BodyID != null) ? fixedHeader.BodyID : fixedHeader.HtmlId);
			function.Append("','");
			function.Append(text);
			function.Append("','");
			function.Append(text2);
			function.Append("','");
			function.Append(text3);
			function.Append("','");
			function.Append(fixedHeader.FirstRowGroupCol);
			function.Append("','");
			function.Append(fixedHeader.LastRowGroupCol);
			function.Append("','");
			function.Append(fixedHeader.LastColGroupRow);
			function.Append("');");
			num++;
		}
	}

	private void RenderServerDynamicImage(RPLElement dynamicImage, RPLDynamicImageProps dynamicImageProps, RPLElementPropsDef def, RPLItemMeasurement measurement, int borderContext, bool renderId, StyleContext styleContext)
	{
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		if (dynamicImage == null)
		{
			return;
		}
		bool flag = dynamicImageProps.ActionImageMapAreas != null && dynamicImageProps.ActionImageMapAreas.Length > 0;
		Rectangle rectangle = RenderDynamicImage(measurement, dynamicImageProps);
		int xOffset = 0;
		int yOffset = 0;
		bool flag2 = !rectangle.IsEmpty;
		bool flag3 = !m_deviceInfo.IsBrowserSafari || m_deviceInfo.AllowScript || !styleContext.InTablix;
		if (flag3)
		{
			WriteStream(m_openDiv);
		}
		bool flag4 = m_deviceInfo.DataVisualizationFitSizing == DataVisualizationFitSizing.Exact && styleContext.InTablix;
		if (flag2)
		{
			Sizings sizing = (Sizings)(flag4 ? 1 : 0);
			WriteOuterConsolidation(rectangle, sizing, ((RPLElementProps)dynamicImageProps).UniqueName);
			RenderReportItemStyle(dynamicImage, null, ref borderContext);
			xOffset = rectangle.Left;
			yOffset = rectangle.Top;
		}
		else if (flag4 && m_deviceInfo.AllowScript)
		{
			if (m_imgFitDivIdsStream == null)
			{
				CreateImgFitDivImageIdsStream();
			}
			WriteIdToSecondaryStream(m_imgFitDivIdsStream, ((RPLElementProps)dynamicImageProps).UniqueName + "_ifd");
			RenderReportItemId(((RPLElementProps)dynamicImageProps).UniqueName + "_ifd");
		}
		if (flag3)
		{
			WriteStream(m_closeBracket);
		}
		WriteStream(m_img);
		if (m_browserIE)
		{
			WriteStream(m_imgOnError);
		}
		if (renderId)
		{
			RenderReportItemId(((RPLElementProps)dynamicImageProps).UniqueName);
		}
		WriteStream(m_zeroBorder);
		bool flag5 = dynamicImage is RPLChart;
		if (flag)
		{
			WriteAttrEncoded(m_useMap, "#" + m_deviceInfo.HtmlPrefixId + m_mapPrefixString + ((RPLElementProps)dynamicImageProps).UniqueName);
			if (flag4)
			{
				OpenStyle();
				if (m_useInlineStyle && !flag2)
				{
					WriteStream(m_styleHeight);
					WriteStream(m_percent);
					WriteStream(m_semiColon);
					WriteStream(m_styleWidth);
					WriteStream(m_percent);
					WriteStream(m_semiColon);
					flag5 = false;
				}
				WriteStream("border-style:none;");
			}
		}
		else if (flag4 && m_useInlineStyle && !flag2)
		{
			PercentSizes();
			flag5 = false;
		}
		StyleContext styleContext2 = new StyleContext();
		if (!flag4 && (m_deviceInfo.IsBrowserIE7 || m_deviceInfo.IsBrowserIE6))
		{
			styleContext2.RenderMeasurements = false;
			styleContext2.RenderMinMeasurements = false;
		}
		if (!flag2)
		{
			if (flag4)
			{
				RenderReportItemStyle(dynamicImage, null, ref borderContext, styleContext2);
			}
			else if (flag5)
			{
				RPLElementProps elementProps = dynamicImage.ElementProps;
				StyleContext styleContext3 = new StyleContext();
				styleContext3.RenderMeasurements = false;
				OpenStyle();
				RenderMeasurementStyle(((RPLSizes)measurement).Height, ((RPLSizes)measurement).Width);
				RenderReportItemStyle(dynamicImage, elementProps, def, measurement, styleContext3, ref borderContext, def.ID);
			}
			else
			{
				RenderReportItemStyle(dynamicImage, measurement, ref borderContext, styleContext2);
			}
		}
		else
		{
			WriteClippedDiv(rectangle);
		}
		WriteToolTip((RPLElementProps)(object)dynamicImageProps);
		WriteStream(m_src);
		RenderDynamicImageSrc(dynamicImageProps);
		WriteStreamCR(m_closeTag);
		if (flag)
		{
			RenderImageMapAreas(dynamicImageProps.ActionImageMapAreas, ((RPLSizes)measurement).Width, ((RPLSizes)measurement).Height, ((RPLElementProps)dynamicImageProps).UniqueName, xOffset, yOffset);
		}
		if (flag3)
		{
			WriteStream(m_closeDiv);
		}
	}

	private void RenderBorderLine(RPLElement reportItem)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		object obj = null;
		IRPLStyle style = (IRPLStyle)(object)reportItem.ElementProps.Style;
		obj = style[(byte)10];
		if (obj != null)
		{
			WriteStream(obj.ToString());
			WriteStream(m_space);
		}
		obj = style[(byte)5];
		if (obj != null)
		{
			obj = EnumStrings.GetValue((BorderStyles)obj);
			WriteStream(obj);
			WriteStream(m_space);
		}
		obj = style[(byte)0];
		if (obj != null)
		{
			WriteStream((string)obj);
		}
	}

	private string CalculateRowHeaderId(RPLTablixCell cell, bool fixedHeader, string tablixID, int row, int col, TablixFixedHeaderStorage headerStorage, bool useElementName, bool fixedCornerHeader)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		string text = null;
		if (cell is RPLTablixMemberCell)
		{
			if (((RPLTablixMemberCell)cell).GroupLabel != null)
			{
				text = ((RPLTablixMemberCell)cell).UniqueName;
			}
			else if (!fixedHeader && useElementName && cell.Element != null && ((RPLElement)cell.Element).ElementProps != null)
			{
				text = ((RPLElement)cell.Element).ElementProps.UniqueName;
			}
		}
		if (fixedHeader)
		{
			if (text == null)
			{
				text = tablixID + "r" + row + "c" + col;
			}
			if (headerStorage != null)
			{
				headerStorage.RowHeaders.Add(text);
				if (headerStorage.CornerHeaders != null && fixedCornerHeader)
				{
					headerStorage.CornerHeaders.Add(text);
				}
			}
		}
		return text;
	}

	private void RenderAccessibleHeaders(RPLTablix tablix, bool fixedHeader, int numCols, int col, int colSpan, int row, RPLTablixCell cell, List<RPLTablixMemberCell> omittedCells, HTMLHeader[] rowHeaderIds, string[] colHeaderIds, OmittedHeaderStack omittedHeaders, ref string id)
	{
		int currentLevel = -1;
		if (tablix.RowHeaderColumns == 0 && omittedCells != null && omittedCells.Count > 0)
		{
			foreach (RPLTablixMemberCell omittedCell in omittedCells)
			{
				RPLTablixMemberDef tablixMemberDef = omittedCell.TablixMemberDef;
				if (tablixMemberDef != null && tablixMemberDef.IsStatic && tablixMemberDef.StaticHeadersTree)
				{
					if (id == null && cell.Element != null && ((RPLElement)cell.Element).ElementProps.UniqueName != null)
					{
						id = ((RPLElement)cell.Element).ElementProps.UniqueName;
					}
					currentLevel = tablixMemberDef.Level;
					omittedHeaders.Push(tablixMemberDef.Level, col, colSpan, id, numCols);
				}
			}
		}
		if (row < tablix.ColumnHeaderRows || fixedHeader || (col >= tablix.ColsBeforeRowHeaders && tablix.RowHeaderColumns > 0 && col < tablix.RowHeaderColumns + tablix.ColsBeforeRowHeaders))
		{
			return;
		}
		bool flag = false;
		string text = colHeaderIds[cell.ColIndex];
		if (!string.IsNullOrEmpty(text))
		{
			WriteStream(m_headers);
			WriteStream(text);
			flag = true;
		}
		foreach (HTMLHeader hTMLHeader in rowHeaderIds)
		{
			string iD = hTMLHeader.ID;
			if (!string.IsNullOrEmpty(iD))
			{
				if (flag)
				{
					WriteStream(m_space);
				}
				else
				{
					WriteStream(m_headers);
				}
				WriteAttrEncoded(m_deviceInfo.HtmlPrefixId);
				WriteStream(iD);
				flag = true;
			}
		}
		string headers = omittedHeaders.GetHeaders(col, currentLevel, HttpUtility.HtmlAttributeEncode(m_deviceInfo.HtmlPrefixId));
		if (!string.IsNullOrEmpty(headers))
		{
			if (flag)
			{
				WriteStream(m_space);
			}
			else
			{
				WriteStream(m_headers);
			}
			WriteStream(headers);
			flag = true;
		}
		if (flag)
		{
			WriteStream(m_quote);
		}
	}

	private void RenderTablixCell(RPLTablix tablix, bool fixedHeader, string tablixID, int numCols, int numRows, int col, int colSpan, int row, int tablixContext, RPLTablixCell cell, List<RPLTablixMemberCell> omittedCells, ref int omittedIndex, StyleContext styleContext, TablixFixedHeaderStorage headerStorage, HTMLHeader[] rowHeaderIds, string[] colHeaderIds, OmittedHeaderStack omittedHeaders)
	{
		bool lastCol = col + colSpan == numCols;
		bool zeroWidth = styleContext.ZeroWidth;
		float columnWidth = tablix.GetColumnWidth(cell.ColIndex, cell.ColSpan);
		styleContext.ZeroWidth = columnWidth == 0f;
		int startIndex = RenderZeroWidthTDsForTablix(col, colSpan, tablix);
		colSpan = GetColSpanMinusZeroWidthColumns(col, colSpan, tablix);
		bool useElementName = m_deviceInfo.AccessibleTablix && tablix.RowHeaderColumns > 0 && col >= tablix.ColsBeforeRowHeaders && col < tablix.RowHeaderColumns + tablix.ColsBeforeRowHeaders;
		bool fixedCornerHeader = fixedHeader && tablix.FixedColumns[col] && tablix.FixedRow(row);
		string id = CalculateRowHeaderId(cell, fixedHeader, tablixID, cell.RowIndex, cell.ColIndex, headerStorage, useElementName, fixedCornerHeader);
		WriteStream(m_openTD);
		if (m_deviceInfo.AccessibleTablix)
		{
			RenderAccessibleHeaders(tablix, fixedHeader, numCols, cell.ColIndex, colSpan, cell.RowIndex, cell, omittedCells, rowHeaderIds, colHeaderIds, omittedHeaders, ref id);
		}
		if (id != null)
		{
			RenderReportItemId(id);
		}
		int rowSpan = cell.RowSpan;
		if (cell.RowSpan > 1)
		{
			WriteStream(m_rowSpan);
			WriteStream(cell.RowSpan.ToString(CultureInfo.InvariantCulture));
			WriteStream(m_quote);
			WriteStream(m_inlineHeight);
			WriteStream(Utility.MmToPxAsString(tablix.GetRowHeight(cell.RowIndex, cell.RowSpan)));
			WriteStream(m_quote);
		}
		if (colSpan > 1)
		{
			WriteStream(m_colSpan);
			WriteStream(cell.ColSpan.ToString(CultureInfo.InvariantCulture));
			WriteStream(m_quote);
		}
		RPLElement element = (RPLElement)(object)cell.Element;
		if (element != null)
		{
			int borderContext = 0;
			RenderTablixReportItemStyle(tablix, tablixContext, cell, styleContext, col == 0, lastCol, row == 0, row + rowSpan == numRows, element, ref borderContext);
			RenderTablixOmittedHeaderCells(omittedCells, col, lastCol, ref omittedIndex);
			RenderTablixReportItem(tablix, tablixContext, cell, styleContext, col == 0, lastCol, row == 0, row + rowSpan == numRows, element, ref borderContext);
		}
		else
		{
			if (styleContext.ZeroWidth)
			{
				OpenStyle();
				WriteStream(m_displayNone);
				CloseStyle(renderQuote: true);
			}
			WriteStream(m_closeBracket);
			RenderTablixOmittedHeaderCells(omittedCells, col, lastCol, ref omittedIndex);
			WriteStream(m_nbsp);
		}
		WriteStream(m_closeTD);
		RenderZeroWidthTDsForTablix(startIndex, colSpan, tablix);
		styleContext.ZeroWidth = zeroWidth;
	}

	private void RenderTablixOmittedHeaderCells(List<RPLTablixMemberCell> omittedHeaders, int colIndex, bool lastCol, ref int omittedIndex)
	{
		if (omittedHeaders == null)
		{
			return;
		}
		while (omittedIndex < omittedHeaders.Count && (((RPLTablixCell)omittedHeaders[omittedIndex]).ColIndex == colIndex || (lastCol && ((RPLTablixCell)omittedHeaders[omittedIndex]).ColIndex > colIndex)))
		{
			RPLTablixMemberCell val = omittedHeaders[omittedIndex];
			if (val.GroupLabel != null)
			{
				RenderNavigationId(val.UniqueName);
			}
			omittedIndex++;
		}
	}

	private void RenderColumnHeaderTablixCell(RPLTablix tablix, string tablixID, int numCols, int col, int colSpan, int row, int tablixContext, RPLTablixCell cell, StyleContext styleContext, TablixFixedHeaderStorage headerStorage, List<RPLTablixOmittedRow> omittedRows, int[] omittedIndices)
	{
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		bool lastCol = col + colSpan == numCols;
		bool zeroWidth = styleContext.ZeroWidth;
		float columnWidth = tablix.GetColumnWidth(col, colSpan);
		styleContext.ZeroWidth = columnWidth == 0f;
		int startIndex = RenderZeroWidthTDsForTablix(col, colSpan, tablix);
		colSpan = GetColSpanMinusZeroWidthColumns(col, colSpan, tablix);
		WriteStream(m_openTD);
		int rowSpan = cell.RowSpan;
		string text = null;
		if (cell is RPLTablixMemberCell && (((RPLTablixMemberCell)cell).GroupLabel != null || m_deviceInfo.AccessibleTablix))
		{
			text = ((RPLTablixMemberCell)cell).UniqueName;
			if (text == null && cell.Element != null && ((RPLElement)cell.Element).ElementProps != null)
			{
				text = ((RPLElement)cell.Element).ElementProps.UniqueName;
				((RPLTablixMemberCell)cell).UniqueName = text;
			}
			if (text != null)
			{
				RenderReportItemId(text);
			}
		}
		if (tablix.FixedColumns[col])
		{
			if (text == null)
			{
				text = tablixID + "r" + row + "c" + col;
				RenderReportItemId(text);
			}
			headerStorage.RowHeaders.Add(text);
			if (headerStorage.CornerHeaders != null)
			{
				headerStorage.CornerHeaders.Add(text);
			}
		}
		if (rowSpan > 1)
		{
			WriteStream(m_rowSpan);
			WriteStream(cell.RowSpan.ToString(CultureInfo.InvariantCulture));
			WriteStream(m_quote);
			WriteStream(m_inlineHeight);
			WriteStream(Utility.MmToPxAsString(tablix.GetRowHeight(cell.RowIndex, cell.RowSpan)));
			WriteStream(m_quote);
		}
		if (colSpan > 1)
		{
			WriteStream(m_colSpan);
			WriteStream(cell.ColSpan.ToString(CultureInfo.InvariantCulture));
			WriteStream(m_quote);
		}
		RPLElement element = (RPLElement)(object)cell.Element;
		if (element != null)
		{
			int borderContext = 0;
			RenderTablixReportItemStyle(tablix, tablixContext, cell, styleContext, col == 0, lastCol, row == 0, lastRow: false, element, ref borderContext);
			for (int i = 0; i < omittedRows.Count; i++)
			{
				RenderTablixOmittedHeaderCells(((RPLTablixRow)omittedRows[i]).OmittedHeaders, col, lastCol, ref omittedIndices[i]);
			}
			RenderTablixReportItem(tablix, tablixContext, cell, styleContext, col == 0, lastCol, row == 0, lastRow: false, element, ref borderContext);
		}
		else
		{
			if (styleContext.ZeroWidth)
			{
				OpenStyle();
				WriteStream(m_displayNone);
				CloseStyle(renderQuote: true);
			}
			WriteStream(m_closeBracket);
			for (int j = 0; j < omittedRows.Count; j++)
			{
				RenderTablixOmittedHeaderCells(((RPLTablixRow)omittedRows[j]).OmittedHeaders, col, lastCol, ref omittedIndices[j]);
			}
			WriteStream(m_nbsp);
		}
		WriteStream(m_closeTD);
		RenderZeroWidthTDsForTablix(startIndex, colSpan, tablix);
		styleContext.ZeroWidth = zeroWidth;
	}

	protected void CreateGrowRectIdsStream()
	{
		string streamName = GetStreamName(m_rplReport.ReportName, m_pageNum, "_gr");
		Stream stream = CreateStream(streamName, "txt", Encoding.UTF8, "text/plain", willSeek: true, StreamOper.CreateOnly);
		m_growRectangleIdsStream = new BufferedStream(stream);
		m_needsGrowRectangleScript = true;
	}

	protected void CreateFitVertTextIdsStream()
	{
		string streamName = GetStreamName(m_rplReport.ReportName, m_pageNum, "_fvt");
		Stream stream = CreateStream(streamName, "txt", Encoding.UTF8, "text/plain", willSeek: true, StreamOper.CreateOnly);
		m_fitVertTextIdsStream = new BufferedStream(stream);
		m_needsFitVertTextScript = true;
	}

	protected void CreateImgConImageIdsStream()
	{
		string streamName = GetStreamName(m_rplReport.ReportName, m_pageNum, "_ici");
		Stream stream = CreateStream(streamName, "txt", Encoding.UTF8, "text/plain", willSeek: true, StreamOper.CreateOnly);
		m_imgConImageIdsStream = new BufferedStream(stream);
	}

	protected void CreateImgFitDivImageIdsStream()
	{
		string streamName = GetStreamName(m_rplReport.ReportName, m_pageNum, "_ifd");
		Stream stream = CreateStream(streamName, "txt", Encoding.UTF8, "text/plain", willSeek: true, StreamOper.CreateOnly);
		m_imgFitDivIdsStream = new BufferedStream(stream);
		m_emitImageConsolidationScaling = true;
	}

	[SecurityTreatAsSafe]
	[SecurityCritical]
	protected Stream CreateStream(string name, string extension, Encoding encoding, string mimeType, bool willSeek, StreamOper operation)
	{
		return m_createAndRegisterStreamCallback(name, extension, encoding, mimeType, willSeek, operation);
	}

	protected void RenderSecondaryStreamIdsSpanTag(Stream secondaryStream, string tagId)
	{
		if (secondaryStream != null && secondaryStream.CanSeek)
		{
			WriteStream(m_openSpan);
			RenderReportItemId(tagId);
			WriteStream(" ids=\"");
			secondaryStream.Seek(0L, SeekOrigin.Begin);
			byte[] array = new byte[4096];
			int count;
			while ((count = secondaryStream.Read(array, 0, array.Length)) > 0)
			{
				m_mainStream.Write(array, 0, count);
			}
			WriteStream("\"");
			WriteStream(m_closeBracket);
			WriteStreamCR(m_closeSpan);
		}
	}

	protected void RenderSecondaryStreamSpanTagsForJavascriptFunctions()
	{
		RenderSecondaryStreamIdsSpanTag(m_growRectangleIdsStream, "growRectangleIdsTag");
		RenderSecondaryStreamIdsSpanTag(m_fitVertTextIdsStream, "fitVertTextIdsTag");
		RenderSecondaryStreamIdsSpanTag(m_imgFitDivIdsStream, "imgFitDivIdsTag");
		RenderSecondaryStreamIdsSpanTag(m_imgConImageIdsStream, "imgConImageIdsTag");
	}
}
