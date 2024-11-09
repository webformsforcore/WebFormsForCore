// Decompiled with JetBrains decompiler
// Type: Microsoft.ReportingServices.Diagnostics.WebRequestUtil
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

using System;
using System.Web;

#nullable disable
namespace Microsoft.ReportingServices.Diagnostics
{
  internal static class WebRequestUtil
  {
    internal const string SharepointPreambleLengthHeaderName = "SharepointPreambleLength";
    internal const string SharepointExternalReportServerUrlHeaderName = "SharepointExternalReportServerUrl";
    internal const string SharepointClientHttpMethodHeaderName = "SharepointClientHttpMethod";
    private static readonly string m_clientHostHeaderName = "RSClientHostName";

    public static string ClientHostHeaderName => WebRequestUtil.m_clientHostHeaderName;

    public static string GetHostFromRequest(HttpRequest request)
    {
      string hostFromRequest = request.Headers["host"];
      if (hostFromRequest == null || hostFromRequest.Length == 0)
        hostFromRequest = request.Url.GetLeftPart(UriPartial.Authority).Substring(request.Url.GetLeftPart(UriPartial.Scheme).Length);
      return hostFromRequest;
    }

    public static bool IsClientLocal() => WebRequestUtil.IsClientLocal(HttpContext.Current);

    public static bool IsClientLocal(HttpContext context)
    {
      if (context == null)
        return true;
      return context.Request.IsLocal && context.Request.Headers[LocalClientConstants.ClientNotLocalHeaderName] == null;
    }

    public static bool IsClientLocal(ILocalClient clientDetection)
    {
      return clientDetection == null ? WebRequestUtil.IsClientLocal() : clientDetection.IsClientLocal;
    }

    internal static string SharepointExternalReportServerUrl
    {
      get
      {
        return HttpUtility.UrlDecode(HttpContext.Current.Request.Headers[nameof (SharepointExternalReportServerUrl)]);
      }
    }
  }
}
