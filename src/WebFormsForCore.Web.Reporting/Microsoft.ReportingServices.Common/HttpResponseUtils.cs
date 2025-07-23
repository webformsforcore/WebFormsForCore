using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Text;
using System.Web;

namespace Microsoft.ReportingServices.Common;

internal static class HttpResponseUtils
{
	public static void ApplyRSStreamOperationHeaders(HttpResponse response, string mimeType, Encoding encoding, string name, string extension)
	{
		response.ContentType = mimeType;
		ApplyContentEncodingHeaders(response, encoding);
		if (extension != null)
		{
			response.AddHeader("FileExtension", extension);
		}
		string fileName = name + "." + extension;
		if ("text/html".Equals(mimeType, StringComparison.Ordinal))
		{
			AddContentDisposition(fileName, response, ContentDisposition.Inline);
		}
		else
		{
			AddContentDisposition(fileName, response, ContentDisposition.Attachment);
		}
		response.Cache.SetCacheability(HttpCacheability.Private);
		if (mimeType != "multipart/related" && extension != "mhtml")
		{
			response.Cache.SetLastModified(DateTime.Now);
		}
		response.Expires = -1;
	}

	internal static void ApplyContentEncodingHeaders(HttpResponse response, Encoding encoding)
	{
		if (encoding != null)
		{
			response.ContentEncoding = encoding;
			response.Charset = encoding.WebName;
		}
	}

	public static void ApplyCacheControlHeaders(HttpRequest request, HttpResponse response)
	{
		string text = request.QueryString["rs:Command"];
		if (string.IsNullOrEmpty(text))
		{
			return;
		}
		switch (text.ToUpperInvariant())
		{
		case "BLANK":
			response.Cache.SetCacheability(HttpCacheability.Public);
			response.Cache.SetExpires(DateTime.Today.AddMonths(1));
			break;
		case "GET":
		{
			response.Cache.SetCacheability(HttpCacheability.Private);
			string text2 = request.QueryString["rc:GetImage"];
			if (text2 == null)
			{
				DateTime expires = DateTime.Now.AddMonths(1);
				response.Cache.SetExpires(expires);
			}
			break;
		}
		case "STYLESHEET":
		case "STYLESHEETIMAGE":
			response.Cache.SetCacheability(HttpCacheability.Private);
			break;
		}
	}

	public static void ApplyReportBuilderFileResponseHeaders(string fileName, HttpContext context)
	{
		if (fileName.EndsWith(".manifest", StringComparison.OrdinalIgnoreCase))
		{
			context.Response.ContentType = "application/x-ms-manifest";
			context.Response.Cache.SetExpires(context.Timestamp.AddMinutes(-1.0));
		}
		else if (fileName.EndsWith(".application", StringComparison.OrdinalIgnoreCase))
		{
			context.Response.ContentType = "application/x-ms-application";
			context.Response.Cache.SetExpires(context.Timestamp.AddMinutes(-1.0));
		}
		else if (fileName.EndsWith(".deploy", StringComparison.OrdinalIgnoreCase))
		{
			context.Response.ContentType = "application/octet-stream";
		}
	}

	internal static void ApplySecurityHeaders(HttpResponse response)
	{
		response.AddHeader("X-Content-Type-Options", "nosniff");
	}

	public static void AddContentDisposition(string fileName, HttpResponse response, ContentDisposition contentDisposition)
	{
		string text = EncodeFileNameForMimeHeader(fileName);
		string text2 = null;
		text2 = ((contentDisposition != ContentDisposition.Inline) ? "attachment" : "inline");
		text2 = text2 + "; filename=\"" + text + "\"";
		response.AddHeader("Content-Disposition", text2);
	}

	public static string EncodeFileNameForMimeHeader(string fileName)
	{
		StringBuilder stringBuilder = new StringBuilder(fileName.Length);
		char[] array = fileName.ToCharArray();
		for (int i = 0; i < fileName.Length; i++)
		{
			int num = Convert.ToInt32(array[i]);
			if ((num >= 65 && num <= 90) || (num >= 97 && num <= 122) || (num >= 48 && num <= 57) || num == 32 || num == 46)
			{
				stringBuilder.Append(array[i]);
				continue;
			}
			bool flag = num >= 55296 && num <= 57343;
			byte[] bytes = Encoding.UTF8.GetBytes(array, i, (!flag) ? 1 : 2);
			byte[] array2 = bytes;
			foreach (byte b in array2)
			{
				stringBuilder.Append("%");
				stringBuilder.Append(b.ToString("X", CultureInfo.InvariantCulture));
			}
			if (flag)
			{
				i++;
			}
		}
		return stringBuilder.ToString();
	}

	public static bool IsViewerRequested(NameValueCollection requestParameters)
	{
		string text = requestParameters["rc:Toolbar"];
		if (text == null)
		{
			string text2 = requestParameters["rs:Command"];
			if (text2 != null)
			{
				return string.Compare(text2, "Render", StringComparison.OrdinalIgnoreCase) == 0;
			}
			return true;
		}
		return string.Compare(text, bool.FalseString, StringComparison.OrdinalIgnoreCase) != 0;
	}

	public static bool IsRequestedFormatHtml(NameValueCollection requestParameters)
	{
		string text = requestParameters["rs:Format"];
		if (text != null)
		{
			return string.Compare(text, "HTML4.0", StringComparison.OrdinalIgnoreCase) == 0;
		}
		return true;
	}
}
