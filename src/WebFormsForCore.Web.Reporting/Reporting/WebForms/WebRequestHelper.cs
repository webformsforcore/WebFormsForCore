
using Microsoft.ReportingServices.Common;
using Microsoft.ReportingServices.Diagnostics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Web;
using System.Xml;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal static class WebRequestHelper
  {
    private const string InfoQuery = "rs:MoreInformation";
    private const string SPUserTokenParam = "rs:TrustedUserToken";
    private const string SPUserNameParam = "rs:TrustedUserName";

    public static HttpWebRequest GetServerUrlAccessObject(
      string url,
      int timeout,
      ICredentials credentials,
      Cookie formsAuthCookie,
      IEnumerable<string> headers,
      IEnumerable<Cookie> cookies,
      string userName,
      byte[] userToken)
    {
      HttpWebRequest request = (HttpWebRequest) WebRequest.Create(url);
      request.Credentials = credentials;
      request.Timeout = timeout;
      WebRequestHelper.SetRequestHeaders(request, formsAuthCookie, headers, cookies);
      if (userToken != null && !string.IsNullOrEmpty(userName))
      {
        string base64String = Convert.ToBase64String(userToken);
        string s = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}={1}&{2}={3}", (object) "rs:TrustedUserName", (object) UrlUtil.UrlEncode(userName), (object) "rs:TrustedUserToken", (object) UrlUtil.UrlEncode(base64String));
        request.Method = "POST";
        request.ContentType = "application/x-www-form-urlencoded";
        byte[] bytes = Encoding.UTF8.GetBytes(s);
        using (Stream requestStream = request.GetRequestStream())
          requestStream.Write(bytes, 0, bytes.Length);
      }
      return request;
    }

    public static void SetRequestHeaders(
      HttpWebRequest request,
      Cookie formsAuthCookie,
      IEnumerable<string> headers,
      IEnumerable<Cookie> cookies)
    {
      request.Headers.Add("Accept-Language", Thread.CurrentThread.CurrentCulture.Name);
      if (!WebRequestUtil.IsClientLocal())
        request.Headers.Add(LocalClientConstants.ClientNotLocalHeaderName, "true");
      request.CookieContainer = new CookieContainer();
      if (formsAuthCookie != null)
        request.CookieContainer.Add(formsAuthCookie);
      if (cookies != null)
      {
        foreach (Cookie cookie in cookies)
          request.CookieContainer.Add(cookie);
      }
      if (headers != null)
      {
        foreach (string header in headers)
          request.Headers.Add(header);
      }
      if (HttpContext.Current == null)
        return;
      request.UserAgent = HttpContext.Current.Request.UserAgent;
      if (string.Compare(request.Address.GetComponents(UriComponents.Host, UriFormat.Unescaped), "localhost", StringComparison.OrdinalIgnoreCase) != 0)
        return;
      request.Headers.Add(WebRequestUtil.ClientHostHeaderName, WebRequestUtil.GetHostFromRequest(HttpContext.Current.Request));
    }

    public static ReportServerException ExceptionFromWebResponse(Exception e)
    {
      return ReportServerException.FromException(WebRequestHelper.ExceptionFromWebResponseUnwrapped(e));
    }

    private static Exception ExceptionFromWebResponseUnwrapped(Exception e)
    {
      IOException ioException = e as IOException;
      WebException webException = e as WebException;
      if (ioException != null)
      {
        if (ioException.InnerException is SocketException innerException && innerException.SocketErrorCode == SocketError.Interrupted)
          return (Exception) new OperationCanceledException();
      }
      else if (webException != null)
      {
        if (webException.Status == WebExceptionStatus.RequestCanceled)
          return (Exception) new OperationCanceledException();
        if (webException.Response != null)
        {
          Stream responseStream = webException.Response.GetResponseStream();
          try
          {
            XmlDocument xmlDocument = new XmlDocument();
            XmlReader reader = XmlReader.Create(responseStream, new XmlReaderSettings()
            {
              CheckCharacters = false
            });
            xmlDocument.Load(reader);
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDocument.NameTable);
            nsmgr.AddNamespace("rs", "http://www.microsoft.com/sql/reportingservices");
            if (xmlDocument.DocumentElement != null)
            {
              Exception exception = (Exception) ReportServerException.FromMoreInformationNode(xmlDocument.DocumentElement.SelectSingleNode("rs:MoreInformation", nsmgr));
              if (exception != null)
                return exception;
            }
          }
          catch (Exception ex)
          {
          }
          finally
          {
            responseStream.Close();
          }
        }
      }
      return e;
    }
  }
}
