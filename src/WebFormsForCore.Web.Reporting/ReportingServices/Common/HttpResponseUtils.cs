
using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Text;
using System.Web;

#nullable disable
namespace Microsoft.ReportingServices.Common
{
  internal static class HttpResponseUtils
  {
    public static void ApplyRSStreamOperationHeaders(
      HttpResponse response,
      string mimeType,
      Encoding encoding,
      string name,
      string extension)
    {
      response.ContentType = mimeType;
      HttpResponseUtils.ApplyContentEncodingHeaders(response, encoding);
      if (extension != null)
        response.AddHeader("FileExtension", extension);
      string fileName = name + "." + extension;
      if ("text/html".Equals(mimeType, StringComparison.Ordinal))
        HttpResponseUtils.AddContentDisposition(fileName, response, ContentDisposition.Inline);
      else
        HttpResponseUtils.AddContentDisposition(fileName, response, ContentDisposition.Attachment);
      response.Cache.SetCacheability(HttpCacheability.Private);
      if (mimeType != "multipart/related" && extension != "mhtml")
        response.Cache.SetLastModified(DateTime.Now);
      response.Expires = -1;
    }

    internal static void ApplyContentEncodingHeaders(HttpResponse response, Encoding encoding)
    {
      if (encoding == null)
        return;
      response.ContentEncoding = encoding;
      response.Charset = encoding.WebName;
    }

    public static void ApplyCacheControlHeaders(HttpRequest request, HttpResponse response)
    {
      string str = request.QueryString["rs:Command"];
      if (string.IsNullOrEmpty(str))
        return;
      switch (str.ToUpperInvariant())
      {
        case "BLANK":
          response.Cache.SetCacheability(HttpCacheability.Public);
          response.Cache.SetExpires(DateTime.Today.AddMonths(1));
          break;
        case "GET":
          response.Cache.SetCacheability(HttpCacheability.Private);
          if (request.QueryString["rc:GetImage"] != null)
            break;
          DateTime date = DateTime.Now.AddMonths(1);
          response.Cache.SetExpires(date);
          break;
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
      else
      {
        if (!fileName.EndsWith(".deploy", StringComparison.OrdinalIgnoreCase))
          return;
        context.Response.ContentType = "application/octet-stream";
      }
    }

    internal static void ApplySecurityHeaders(HttpResponse response)
    {
      response.AddHeader("X-Content-Type-Options", "nosniff");
    }

    public static void AddContentDisposition(
      string fileName,
      HttpResponse response,
      ContentDisposition contentDisposition)
    {
      string str1 = HttpResponseUtils.EncodeFileNameForMimeHeader(fileName);
      string str2 = (contentDisposition != ContentDisposition.Inline ? "attachment" : "inline") + "; filename=\"" + str1 + "\"";
      response.AddHeader("Content-Disposition", str2);
    }

    public static string EncodeFileNameForMimeHeader(string fileName)
    {
      StringBuilder stringBuilder = new StringBuilder(fileName.Length);
      char[] charArray = fileName.ToCharArray();
      for (int index = 0; index < fileName.Length; ++index)
      {
        int int32 = Convert.ToInt32(charArray[index]);
        if (int32 >= 65 && int32 <= 90 || int32 >= 97 && int32 <= 122 || int32 >= 48 && int32 <= 57 || int32 == 32 || int32 == 46)
        {
          stringBuilder.Append(charArray[index]);
        }
        else
        {
          bool flag = int32 >= 55296 && int32 <= 57343;
          foreach (byte num in Encoding.UTF8.GetBytes(charArray, index, flag ? 2 : 1))
          {
            stringBuilder.Append("%");
            stringBuilder.Append(num.ToString("X", (IFormatProvider) CultureInfo.InvariantCulture));
          }
          if (flag)
            ++index;
        }
      }
      return stringBuilder.ToString();
    }

    public static bool IsViewerRequested(NameValueCollection requestParameters)
    {
      string requestParameter1 = requestParameters["rc:Toolbar"];
      if (requestParameter1 != null)
        return string.Compare(requestParameter1, bool.FalseString, StringComparison.OrdinalIgnoreCase) != 0;
      string requestParameter2 = requestParameters["rs:Command"];
      return requestParameter2 == null || string.Compare(requestParameter2, "Render", StringComparison.OrdinalIgnoreCase) == 0;
    }

    public static bool IsRequestedFormatHtml(NameValueCollection requestParameters)
    {
      string requestParameter = requestParameters["rs:Format"];
      return requestParameter == null || string.Compare(requestParameter, "HTML4.0", StringComparison.OrdinalIgnoreCase) == 0;
    }
  }
}
