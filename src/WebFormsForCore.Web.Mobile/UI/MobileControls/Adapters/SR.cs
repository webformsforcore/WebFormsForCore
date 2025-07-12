using System.Globalization;
using System.Resources;
using System.Security.Permissions;
using System.Threading;

namespace System.Web.UI.MobileControls.Adapters;

/// <summary>Represents an auto-generated resource class. For information about how to develop ASP.NET mobile applications, see Mobile Apps &amp; Sites with ASP.NET.</summary>
[AspNetHostingPermission(SecurityAction.LinkDemand, Unrestricted = true)]
[AspNetHostingPermission(SecurityAction.InheritanceDemand, Unrestricted = true)]
public class SR
{
	/// <summary>A string that provides the text for the first prompt. This API is obsolete. For information about how to develop ASP.NET mobile applications, see Mobile Apps &amp; Sites with ASP.NET.</summary>
	public const string CalendarAdapterFirstPrompt = "CalendarAdapterFirstPrompt";
	/// <summary>A string that provides the text that prompts the user to select an option. This API is obsolete. For information about how to develop ASP.NET mobile applications, see Mobile Apps &amp; Sites with ASP.NET.</summary>
	public const string CalendarAdapterOptionPrompt = "CalendarAdapterOptionPrompt";
	/// <summary>A string that provides the text that prompts the user to enter a date. This API is obsolete. For information about how to develop ASP.NET mobile applications, see Mobile Apps &amp; Sites with ASP.NET.</summary>
	public const string CalendarAdapterOptionType = "CalendarAdapterOptionType";
	/// <summary>A string that provides the text that prompts the user to select an era. This API is obsolete. For information about how to develop ASP.NET mobile applications, see Mobile Apps &amp; Sites with ASP.NET.</summary>
	public const string CalendarAdapterOptionEra = "CalendarAdapterOptionEra";
	/// <summary>A string that provides the text that prompts the user to select a date. This API is obsolete. For information about how to develop ASP.NET mobile applications, see Mobile Apps &amp; Sites with ASP.NET.</summary>
	public const string CalendarAdapterOptionChooseDate = "CalendarAdapterOptionChooseDate";
	/// <summary>A string that provides the text that prompts the user to select a week. This API is obsolete. For information about how to develop ASP.NET mobile applications, see Mobile Apps &amp; Sites with ASP.NET.</summary>
	public const string CalendarAdapterOptionChooseWeek = "CalendarAdapterOptionChooseWeek";
	/// <summary>A string that provides the text that prompts the user to select a month. This API is obsolete. For information about how to develop ASP.NET mobile applications, see Mobile Apps &amp; Sites with ASP.NET.</summary>
	public const string CalendarAdapterOptionChooseMonth = "CalendarAdapterOptionChooseMonth";
	/// <summary>A string to use as an error message if the date entered was incorrect. This API is obsolete. For information about how to develop ASP.NET mobile applications, see Mobile Apps &amp; Sites with ASP.NET.</summary>
	public const string CalendarAdapterTextBoxErrorMessage = "CalendarAdapterTextBoxErrorMessage";
	/// <summary>A string to use as an error message if the expected decimal code is not present. This API is obsolete. For information about how to develop ASP.NET mobile applications, see Mobile Apps &amp; Sites with ASP.NET.</summary>
	public const string ChtmlImageAdapterDecimalCodeExpectedAfterGroupChar = "ChtmlImageAdapterDecimalCodeExpectedAfterGroupChar";
	/// <summary>A string containing the message to display preceding a link that redirects the browser to another page. This API is obsolete. For information about how to develop ASP.NET mobile applications, see Mobile Apps &amp; Sites with ASP.NET.</summary>
	public const string ChtmlPageAdapterRedirectPageContent = "ChtmlPageAdapterRedirectPageContent";
	/// <summary>A string that provides the text for the redirect link label. This API is obsolete. For information about how to develop ASP.NET mobile applications, see Mobile Apps &amp; Sites with ASP.NET.</summary>
	public const string ChtmlPageAdapterRedirectLinkLabel = "ChtmlPageAdapterRedirectLinkLabel";
	/// <summary>A string to use as an error message if an attempt is made to set the <see cref="P:System.Web.UI.MobileControls.Adapters.ControlAdapter.Page" /> property of a <see cref="T:System.Web.UI.MobileControls.Adapters.ControlAdapter" /> object. This API is obsolete. For information about how to develop ASP.NET mobile applications, see Mobile Apps &amp; Sites with ASP.NET.</summary>
	public const string ControlAdapterBasePagePropertyShouldNotBeSet = "ControlAdapterBasePagePropertyShouldNotBeSet";
	/// <summary>A string to use as an error message if multiple controls are set to appear across a <see cref="T:System.Web.UI.MobileControls.MobilePage" /> object's secondary pages. This API is obsolete. For information about how to develop ASP.NET mobile applications, see Mobile Apps &amp; Sites with ASP.NET.</summary>
	public const string FormAdapterMultiControlsAttemptSecondaryUI = "FormAdapterMultiControlsAttemptSecondaryUI";
	/// <summary>A string to use as an error message if an attempt to write a multipart document is made. This API is obsolete. For information about how to develop ASP.NET mobile applications, see Mobile Apps &amp; Sites with ASP.NET.</summary>
	public const string MobileTextWriterNotMultiPart = "MobileTextWriterNotMultiPart";
	/// <summary>A string to use as an error message if invalid data was posted for the current <see cref="T:System.Web.UI.MobileControls.ObjectList" /> control's <see cref="P:System.Web.UI.MobileControls.ObjectList.ViewMode" /> property. This API is obsolete. For information about how to develop ASP.NET mobile applications, see Mobile Apps &amp; Sites with ASP.NET.</summary>
	public const string ObjectListAdapter_InvalidPostedData = "ObjectListAdapter_InvalidPostedData";
	/// <summary>A string that provides the text for the Back label. This API is obsolete. For information about how to develop ASP.NET mobile applications, see Mobile Apps &amp; Sites with ASP.NET.</summary>
	public const string WmlMobileTextWriterBackLabel = "WmlMobileTextWriterBackLabel";
	/// <summary>A string that provides the text for the OK label. This API is obsolete. For information about how to develop ASP.NET mobile applications, see Mobile Apps &amp; Sites with ASP.NET.</summary>
	public const string WmlMobileTextWriterOKLabel = "WmlMobileTextWriterOKLabel";
	/// <summary>A string that provides the text for the Go label. This API is obsolete. For information about how to develop ASP.NET mobile applications, see Mobile Apps &amp; Sites with ASP.NET.</summary>
	public const string WmlMobileTextWriterGoLabel = "WmlMobileTextWriterGoLabel";
	/// <summary>A string that provides a common phrase for announcing errors. This API is obsolete. For information about how to develop ASP.NET mobile applications, see Mobile Apps &amp; Sites with ASP.NET.</summary>
	public const string WmlPageAdapterServerError = "WmlPageAdapterServerError";
	/// <summary>A string that contains the title of the stack trace. This API is obsolete. For information about how to develop ASP.NET mobile applications, see Mobile Apps &amp; Sites with ASP.NET.</summary>
	public const string WmlPageAdapterStackTrace = "WmlPageAdapterStackTrace";
	/// <summary>A string that provides a title for the partial stack trace. This API is obsolete. For information about how to develop ASP.NET mobile applications, see Mobile Apps &amp; Sites with ASP.NET.</summary>
	public const string WmlPageAdapterPartialStackTrace = "WmlPageAdapterPartialStackTrace";
	/// <summary>A string that provides a title for methods. This API is obsolete. For information about how to develop ASP.NET mobile applications, see Mobile Apps &amp; Sites with ASP.NET.</summary>
	public const string WmlPageAdapterMethod = "WmlPageAdapterMethod";
	/// <summary>A string that provides a title for details. This API is obsolete. For information about how to develop ASP.NET mobile applications, see Mobile Apps &amp; Sites with ASP.NET.</summary>
	public const string WmlObjectListAdapterDetails = "WmlObjectListAdapterDetails";
	/// <summary>A string to use as an error message if there is no style sheet ID present in the query string. This API is obsolete. For information about how to develop ASP.NET mobile applications, see Mobile Apps &amp; Sites with ASP.NET.</summary>
	public const string XhtmlCssHandler_IdNotPresent = "XhtmlCssHandler_IdNotPresent";
	/// <summary>A string to use as an error message if a style sheet was not found. This API is obsolete. For information about how to develop ASP.NET mobile applications, see Mobile Apps &amp; Sites with ASP.NET.</summary>
	public const string XhtmlCssHandler_StylesheetNotFound = "XhtmlCssHandler_StylesheetNotFound";
	/// <summary>A string to use as an error message if invalid data was posted for the current <see cref="T:System.Web.UI.MobileControls.ObjectList" /> control's <see cref="P:System.Web.UI.MobileControls.ObjectList.ViewMode" /> property. This API is obsolete. For information about how to develop ASP.NET mobile applications, see Mobile Apps &amp; Sites with ASP.NET.</summary>
	public const string XhtmlObjectListAdapter_InvalidPostedData = "XhtmlObjectListAdapter_InvalidPostedData";
	/// <summary>A string to use to advise calling the <see cref="M:System.Web.UI.MobileControls.Adapters.XhtmlAdapters.XhtmlMobileTextWriter.SetSessionKey(System.Web.SessionState.HttpSessionState)" /> method before getting the <see cref="P:System.Web.UI.MobileControls.Adapters.XhtmlAdapters.XhtmlMobileTextWriter.SessionKey" /> property. This API is obsolete. For information about how to develop ASP.NET mobile applications, see Mobile Apps &amp; Sites with ASP.NET.</summary>
	public const string XhtmlMobileTextWriter_SessionKeyNotSet = "XhtmlMobileTextWriter_SessionKeyNotSet";
	/// <summary>A string to use to advise calling the <see cref="M:System.Web.UI.MobileControls.Adapters.XhtmlAdapters.XhtmlMobileTextWriter.SetCacheKey(System.Web.Caching.Cache)" /> method before getting the <see cref="P:System.Web.UI.MobileControls.Adapters.XhtmlAdapters.XhtmlMobileTextWriter.CacheKey" /> property. This API is obsolete. For information about how to develop ASP.NET mobile applications, see Mobile Apps &amp; Sites with ASP.NET.</summary>
	public const string XhtmlMobileTextWriter_CacheKeyNotSet = "XhtmlMobileTextWriter_CacheKeyNotSet";
	private static SR loader;
	private ResourceManager resources;

	/// <summary>Initializes a new instance of the <see cref="T:System.Web.UI.MobileControls.Adapters.SR" /> class. This API is obsolete. For information about how to develop ASP.NET mobile applications, see Mobile Apps &amp; Sites with ASP.NET.</summary>
	public SR()
	{
		this.resources = new ResourceManager("System.Web.UI.MobileControls.Adapters", this.GetType().Assembly);
	}

	private static SR GetLoader()
	{
		if (SR.loader == null)
		{
			SR sr = new SR();
			Interlocked.CompareExchange<SR>(ref SR.loader, sr, (SR)null);
		}
		return SR.loader;
	}

	private static CultureInfo Culture => (CultureInfo)null;

	/// <summary>Returns the name of the specified <see cref="T:System.String" />, formatted with respect to the arguments. This API is obsolete. For information about how to develop ASP.NET mobile applications, see Mobile Apps &amp; Sites with ASP.NET.</summary>
	/// <param name="name">The name of the <see cref="T:System.String" /> to be returned.</param>
	/// <param name="args">An array of arguments to be used in formatting the return value.</param>
	/// <returns>The value of the <see cref="T:System.String" /> referred to by the <paramref name="name" /> parameter, formatted with respect to the arguments listed in the <paramref name="args" /> parameter.</returns>
	public static string GetString(string name, params object[] args)
	{
		return SR.GetString(SR.Culture, name, args);
	}

	/// <summary>Returns the name of the specified <see cref="T:System.String" />, formatted with respect to the specified culture and the specified argument array. This API is obsolete. For information about how to develop ASP.NET mobile applications, see Mobile Apps &amp; Sites with ASP.NET.</summary>
	/// <param name="culture">A <see cref="T:System.Globalization.CultureInfo" /> that contains the relevant cultural information used to format the return value.</param>
	/// <param name="name">The name of the <see cref="T:System.String" /> to be returned.</param>
	/// <param name="args">An array of arguments to be used in formatting the return value.</param>
	/// <returns>The value of the <see cref="T:System.String" /> referred to by the <paramref name="name" /> parameter, formatted with respect to the culture information contained in the <see cref="T:System.Globalization.CultureInfo" /> object referred to by the <paramref name="culture" /> parameter and the arguments referred to by the <paramref name="args" /> parameter.</returns>
	public static string GetString(CultureInfo culture, string name, params object[] args)
	{
		SR loader = SR.GetLoader();
		if (loader == null)
			return (string)null;
		string format = loader.resources.GetString(name, culture);
		if (args == null || args.Length == 0)
			return format;
		for (int index = 0; index < args.Length; ++index)
		{
			if (args[index] is string str && str.Length > 1024)
				args[index] = (object)(str.Substring(0, 1021) + "...");
		}
		return string.Format((IFormatProvider)CultureInfo.CurrentCulture, format, args);
	}

	/// <summary>Returns the name of the specified <see cref="T:System.String" />. This API is obsolete. For information about how to develop ASP.NET mobile applications, see Mobile Apps &amp; Sites with ASP.NET.</summary>
	/// <param name="name">The name of the <see cref="T:System.String" /> to be returned.</param>
	/// <returns>The value of the <see cref="T:System.String" /> referred to by the <paramref name="name" /> parameter.</returns>
	public static string GetString(string name) => SR.GetString(SR.Culture, name);

	/// <summary>Returns the name of the specified <see cref="T:System.String" />, formatted with respect to the information specified by the <paramref name="culture" /> parameter. This API is obsolete. For information about how to develop ASP.NET mobile applications, see Mobile Apps &amp; Sites with ASP.NET.</summary>
	/// <param name="culture">A <see cref="T:System.Globalization.CultureInfo" /> that contains the relevant cultural information used to format the return value.</param>
	/// <param name="name">The name of the <see cref="T:System.String" /> to be returned.</param>
	/// <returns>The value of the <see cref="T:System.String" /> referred to by the <paramref name="name" /> parameter, formatted with respect to the culture information contained in the <see cref="T:System.Globalization.CultureInfo" /> object referred to by the <paramref name="culture" /> parameter.</returns>
	public static string GetString(CultureInfo culture, string name)
	{
		return SR.GetLoader()?.resources.GetString(name, culture);
	}

	/// <summary>Returns the name of the specified <see cref="T:System.String" />. This API is obsolete. For information about how to develop ASP.NET mobile applications, see Mobile Apps &amp; Sites with ASP.NET.</summary>
	/// <param name="name">The name of the <see cref="T:System.String" /> to be returned.</param>
	/// <returns>The value of the <see cref="T:System.String" /> referred to by the <paramref name="name" /> parameter.</returns>
	public static bool GetBoolean(string name) => SR.GetBoolean(SR.Culture, name);

	/// <summary>Returns the name of the specified <see cref="T:System.String" />, formatted with respect to the specified culture. This API is obsolete. For information about how to develop ASP.NET mobile applications, see Mobile Apps &amp; Sites with ASP.NET.</summary>
	/// <param name="culture">A <see cref="T:System.Globalization.CultureInfo" /> that contains the relevant cultural information used to format the return value.</param>
	/// <param name="name">The name of the <see cref="T:System.String" /> to be returned.</param>
	/// <returns>The value of the <see cref="T:System.String" /> referred to by the <paramref name="name" /> parameter, formatted with respect to the culture information contained in the <see cref="T:System.Globalization.CultureInfo" /> referred to by the <paramref name="culture" /> parameter.</returns>
	public static bool GetBoolean(CultureInfo culture, string name)
	{
		SR loader = SR.GetLoader();
		if (loader == null || !(loader.resources.GetObject(name, culture) is bool boolean)) return false;
		return boolean;
	}

	/// <summary>Returns the name of the specified <see cref="T:System.String" />. This API is obsolete. For information about how to develop ASP.NET mobile applications, see Mobile Apps &amp; Sites with ASP.NET.</summary>
	/// <param name="name">The name of the <see cref="T:System.String" /> to be returned.</param>
	/// <returns>The value of the <see cref="T:System.String" /> referred to by the <paramref name="name" /> parameter.</returns>
	public static char GetChar(string name) => SR.GetChar(SR.Culture, name);

	/// <summary>Returns the name of the specified <see cref="T:System.String" />, formatted with respect to the specified culture. This API is obsolete. For information about how to develop ASP.NET mobile applications, see Mobile Apps &amp; Sites with ASP.NET.</summary>
	/// <param name="culture">A <see cref="T:System.Globalization.CultureInfo" /> that contains the relevant cultural information used to format the return value.</param>
	/// <param name="name">The name of the <see cref="T:System.String" /> to be returned.</param>
	/// <returns>The value of the <see cref="T:System.String" /> referred to by the <paramref name="name" /> parameter, formatted with respect to the culture information contained in the <see cref="T:System.Globalization.CultureInfo" /> referred to by the <paramref name="culture" /> parameter.</returns>
	public static char GetChar(CultureInfo culture, string name)
	{
		SR loader = SR.GetLoader();
		if (loader == null || !(loader.resources.GetObject(name, culture) is char minValue)) return char.MinValue;
		return minValue;
	}

	/// <summary>Returns the name of the specified <see cref="T:System.String" />. This API is obsolete. For information about how to develop ASP.NET mobile applications, see Mobile Apps &amp; Sites with ASP.NET.</summary>
	/// <param name="name">The name of the <see cref="T:System.String" /> object to be returned.</param>
	/// <returns>The value of the <see cref="T:System.String" /> referred to by the <paramref name="name" /> parameter.</returns>
	public static byte GetByte(string name) => SR.GetByte(SR.Culture, name);

	/// <summary>Returns the name of the specified <see cref="T:System.String" />, formatted with respect to the specified culture. This API is obsolete. For information about how to develop ASP.NET mobile applications, see Mobile Apps &amp; Sites with ASP.NET.</summary>
	/// <param name="culture">A <see cref="T:System.Globalization.CultureInfo" /> that contains the relevant cultural information used to format the return value.</param>
	/// <param name="name">The name of the <see cref="T:System.String" /> to be returned.</param>
	/// <returns>The value of the <see cref="T:System.String" /> referred to by the <paramref name="name" /> parameter, formatted with respect to the culture information contained in the <see cref="T:System.Globalization.CultureInfo" /> referred to by the <paramref name="culture" /> parameter.</returns>
	public static byte GetByte(CultureInfo culture, string name)
	{
		SR loader = SR.GetLoader();
		if (loader == null || !(loader.resources.GetObject(name, culture) is byte num)) return (byte)0;
		return num;
	}

	/// <summary>Returns the name of the specified <see cref="T:System.String" />. This API is obsolete. For information about how to develop ASP.NET mobile applications, see Mobile Apps &amp; Sites with ASP.NET.</summary>
	/// <param name="name">The name of the <see cref="T:System.String" /> to be returned.</param>
	/// <returns>The value of the <see cref="T:System.String" /> referred to by the <paramref name="name" /> parameter.</returns>
	public static short GetShort(string name) => SR.GetShort(SR.Culture, name);

	/// <summary>Returns the name of the specified <see cref="T:System.String" />, formatted with respect to the information specified by the <paramref name="culture" /> parameter. This API is obsolete. For information about how to develop ASP.NET mobile applications, see Mobile Apps &amp; Sites with ASP.NET.</summary>
	/// <param name="culture">A <see cref="T:System.Globalization.CultureInfo" /> that contains the relevant cultural information used to format the return value.</param>
	/// <param name="name">The name of the <see cref="T:System.String" /> to be returned.</param>
	/// <returns>The value of the <see cref="T:System.String" /> referred to by the <paramref name="name" /> parameter, formatted with respect to the culture information contained in the <see cref="T:System.Globalization.CultureInfo" /> object referred to by the <paramref name="culture" /> parameter.</returns>
	public static short GetShort(CultureInfo culture, string name)
	{
		SR loader = SR.GetLoader();
		if (loader == null || !(loader.resources.GetObject(name, culture) is short num)) return (short)0;
		return num;
	}

	/// <summary>Returns the name of the specified <see cref="T:System.String" />. This API is obsolete. For information about how to develop ASP.NET mobile applications, see Mobile Apps &amp; Sites with ASP.NET.</summary>
	/// <param name="name">The name of the <see cref="T:System.String" /> to be returned.</param>
	/// <returns>The value of the <see cref="T:System.String" /> referred to by the <paramref name="name" /> parameter.</returns>
	public static int GetInt(string name) => SR.GetInt(SR.Culture, name);

	/// <summary>Returns the name of the specified <see cref="T:System.String" />, formatted with respect to the information specified by the <paramref name="culture" /> parameter. This API is obsolete. For information about how to develop ASP.NET mobile applications, see Mobile Apps &amp; Sites with ASP.NET.</summary>
	/// <param name="culture">A <see cref="T:System.Globalization.CultureInfo" /> that contains the relevant cultural information used to format the return value.</param>
	/// <param name="name">The name of the <see cref="T:System.String" /> to be returned.</param>
	/// <returns>The value of the <see cref="T:System.String" /> referred to by the <paramref name="name" /> parameter, formatted with respect to the culture information contained in the <see cref="T:System.Globalization.CultureInfo" /> object referred to by the <paramref name="culture" /> parameter.</returns>
	public static int GetInt(CultureInfo culture, string name)
	{
		SR loader = SR.GetLoader();
		if (loader == null || !(loader.resources.GetObject(name, culture) is int num))
			return 0;
		else return num;
	}

	/// <summary>Returns the name of the specified <see cref="T:System.String" />. This API is obsolete. For information about how to develop ASP.NET mobile applications, see Mobile Apps &amp; Sites with ASP.NET.</summary>
	/// <param name="name">The name of the <see cref="T:System.String" /> to be returned.</param>
	/// <returns>The value of the <see cref="T:System.String" /> referred to by the <paramref name="name" /> parameter.</returns>
	public static long GetLong(string name) => SR.GetLong(SR.Culture, name);

	/// <summary>Returns the name of the specified <see cref="T:System.String" />, formatted with respect to the information specified by the <paramref name="culture" /> parameter. This API is obsolete. For information about how to develop ASP.NET mobile applications, see Mobile Apps &amp; Sites with ASP.NET.</summary>
	/// <param name="culture">A <see cref="T:System.Globalization.CultureInfo" /> that contains the relevant cultural information used to format the return value.</param>
	/// <param name="name">The name of the <see cref="T:System.String" /> to be returned.</param>
	/// <returns>The value of the <see cref="T:System.String" /> referred to by the <paramref name="name" /> parameter, formatted with respect to the culture information contained in the <see cref="T:System.Globalization.CultureInfo" /> object referred to by the <paramref name="culture" /> parameter.</returns>
	public static long GetLong(CultureInfo culture, string name)
	{
		SR loader = SR.GetLoader();
		if (loader == null || !(loader.resources.GetObject(name, culture) is long num))
			return 0L;
		return num;
	}

	/// <summary>Returns the name of the specified <see cref="T:System.String" />. This API is obsolete. For information about how to develop ASP.NET mobile applications, see Mobile Apps &amp; Sites with ASP.NET.</summary>
	/// <param name="name">The name of the <see cref="T:System.String" /> to be returned.</param>
	/// <returns>The value of the <see cref="T:System.String" /> referred to by the <paramref name="name" /> parameter.</returns>
	public static float GetFloat(string name) => SR.GetFloat(SR.Culture, name);

	/// <summary>Returns the name of the specified <see cref="T:System.String" />, formatted with respect to the information specified by the <paramref name="culture" /> parameter. This API is obsolete. For information about how to develop ASP.NET mobile applications, see Mobile Apps &amp; Sites with ASP.NET.</summary>
	/// <param name="culture">A <see cref="T:System.Globalization.CultureInfo" /> that contains the relevant cultural information used to format the return value.</param>
	/// <param name="name">The name of the <see cref="T:System.String" /> to be returned.</param>
	/// <returns>The value of the <see cref="T:System.String" /> referred to by the <paramref name="name" /> parameter formatted with respect to the culture information contained in the <see cref="T:System.Globalization.CultureInfo" /> referred to by the <paramref name="culture" /> parameter.</returns>
	public static float GetFloat(CultureInfo culture, string name)
	{
		SR loader = SR.GetLoader();
		if (loader != null || !(loader.resources.GetObject(name, culture) is float num))
			return 0.0f;
		return num;
	}

	/// <summary>Returns the name of the specified <see cref="T:System.String" />. This API is obsolete. For information about how to develop ASP.NET mobile applications, see Mobile Apps &amp; Sites with ASP.NET.</summary>
	/// <param name="name">The name of the <see cref="T:System.String" /> to be returned.</param>
	/// <returns>The value of the <see cref="T:System.String" /> referred to by the <paramref name="name" /> parameter.</returns>
	public static double GetDouble(string name) => SR.GetDouble(SR.Culture, name);

	/// <summary>Returns the name of the specified <see cref="T:System.String" />, formatted with respect to the specified culture. This API is obsolete. For information about how to develop ASP.NET mobile applications, see Mobile Apps &amp; Sites with ASP.NET.</summary>
	/// <param name="culture">A <see cref="T:System.Globalization.CultureInfo" /> that contains the relevant cultural information used to format the return value.</param>
	/// <param name="name">The name of the <see cref="T:System.String" /> to be returned.</param>
	/// <returns>The value of the <see cref="T:System.String" /> referred to by the <paramref name="name" /> parameter, formatted with respect to the culture information contained in the <see cref="T:System.Globalization.CultureInfo" /> referred to by the <paramref name="culture" /> parameter.</returns>
	public static double GetDouble(CultureInfo culture, string name)
	{
		SR loader = SR.GetLoader();
		if (loader != null || !(loader.resources.GetObject(name, culture) is double num))
			return 0.0;
		return num;
	}

	/// <summary>Returns the name of the specified <see cref="T:System.String" />. This API is obsolete. For information about how to develop ASP.NET mobile applications, see Mobile Apps &amp; Sites with ASP.NET.</summary>
	/// <param name="name">The name of the <see cref="T:System.String" /> to be returned.</param>
	/// <returns>The value of the <see cref="T:System.String" /> referred to by the <paramref name="name" /> parameter.</returns>
	public static object GetObject(string name) => SR.GetObject(SR.Culture, name);

	/// <summary>Returns the name of the specified <see cref="T:System.String" />, formatted with respect to the information specified by the <paramref name="culture" /> parameter. This API is obsolete. For information about how to develop ASP.NET mobile applications, see Mobile Apps &amp; Sites with ASP.NET.</summary>
	/// <param name="culture">A <see cref="T:System.Globalization.CultureInfo" /> object that contains the relevant cultural information used to format the return value.</param>
	/// <param name="name">The name of the <see cref="T:System.String" /> object to be returned.</param>
	/// <returns>The value of the <see cref="T:System.String" /> referred to by the <paramref name="name" /> parameter, formatted with respect to the culture information contained in the <see cref="T:System.Globalization.CultureInfo" /> object referred to by the <paramref name="culture" /> parameter.</returns>
	public static object GetObject(CultureInfo culture, string name)
	{
		return SR.GetLoader()?.resources.GetObject(name, culture);
	}
}
