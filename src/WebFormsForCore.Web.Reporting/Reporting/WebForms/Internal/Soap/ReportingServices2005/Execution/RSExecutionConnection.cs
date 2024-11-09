// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution.RSExecutionConnection
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

using Microsoft.ReportingServices.Diagnostics;
using Microsoft.SqlServer.ReportingServices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Runtime.Serialization;
using System.Security.Principal;
using System.Web.Services.Protocols;

#nullable disable
namespace Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution
{
  internal class RSExecutionConnection : ReportExecutionService
  {
    internal const string SoapEndpoint = "ReportExecution2005.asmx";
    private string m_secureServerUrl;
    private string m_nonsecureServerUrl;
    private bool m_currentlyUsingSSL;
    private bool m_alwaysUseSSL;
    private bool m_failedUsingKatmai;
    private readonly EndpointVersion m_endpointVersion;
    private RSExecutionConnection.SecureMethodsList m_secureMethods;
    private bool m_unsafeHeaderServerIsIIS5;

    public RSExecutionConnection(string reportServerLocation, EndpointVersion version)
    {
      this.InitializeReportServerUrl(reportServerLocation);
      this.m_endpointVersion = version;
    }

    public void ValidateConnection()
    {
      try
      {
        this.IsSecureMethod("");
        if (this.ServerInfoHeaderValue != null)
          return;
        this.ListSecureMethods();
      }
      catch (SoapException ex)
      {
        this.OnSoapException(ex);
        throw;
      }
      catch (WebException ex)
      {
        RSExecutionConnection.MissingEndpointException.ThrowIfEndpointMissing(ex);
        throw;
      }
      catch (InvalidOperationException ex)
      {
        throw new RSExecutionConnection.MissingEndpointException((Exception) ex);
      }
    }

    private void SetConnectionSSLForMethod(string methodname)
    {
      this.SetConnectionSSL(this.IsSecureMethod(methodname));
    }

    private void SetConnectionSSL(bool useSSL)
    {
      if (this.m_currentlyUsingSSL == useSSL)
        return;
      this.m_currentlyUsingSSL = useSSL;
      this.Url = this.GetSoapURL(this.m_currentlyUsingSSL);
    }

    private void InitializeReportServerUrl(string reportServerLocation)
    {
      if (reportServerLocation == null)
        return;
      this.m_secureMethods = (RSExecutionConnection.SecureMethodsList) null;
      UriBuilder uriBuilder = new UriBuilder(reportServerLocation);
      if (string.Compare(uriBuilder.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase) == 0)
      {
        this.m_alwaysUseSSL = true;
        this.m_nonsecureServerUrl = (string) null;
        this.m_secureServerUrl = uriBuilder.Uri.AbsoluteUri;
        this.m_currentlyUsingSSL = true;
      }
      else
      {
        this.m_alwaysUseSSL = false;
        this.m_nonsecureServerUrl = uriBuilder.Uri.AbsoluteUri;
        uriBuilder.Port = -1;
        uriBuilder.Scheme = Uri.UriSchemeHttps;
        this.m_secureServerUrl = uriBuilder.Uri.AbsoluteUri;
        this.m_currentlyUsingSSL = false;
      }
      this.Url = this.GetSoapURL(this.m_currentlyUsingSSL);
    }

    internal string GetSoapURL(bool useSSL)
    {
      return this.GetServerURL(useSSL) + (object) '/' + "ReportExecution2005.asmx";
    }

    internal string GetServerURL(bool useSSL)
    {
      return useSSL ? this.m_secureServerUrl : this.m_nonsecureServerUrl;
    }

    internal string UrlForRender => this.GetServerURL(this.IsSecureMethod("UrlRender"));

    protected override WebRequest GetWebRequest(Uri uri)
    {
      HttpWebRequest webRequest = (HttpWebRequest) base.GetWebRequest(uri);
      if (5 == Environment.OSVersion.Version.Major && this.m_unsafeHeaderServerIsIIS5 && webRequest.Credentials == CredentialCache.DefaultCredentials)
      {
        webRequest.UnsafeAuthenticatedConnectionSharing = true;
        webRequest.ConnectionGroupName = WindowsIdentity.GetCurrent().Name;
      }
      return (WebRequest) webRequest;
    }

    protected override WebResponse GetWebResponse(WebRequest request)
    {
      WebResponse webResponse = base.GetWebResponse(request);
      if (string.Compare(webResponse.Headers["Server"], "Microsoft-IIS/5.0", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(webResponse.Headers["Server"], "Microsoft-IIS/5.1", StringComparison.OrdinalIgnoreCase) == 0)
        this.m_unsafeHeaderServerIsIIS5 = true;
      return webResponse;
    }

    protected virtual void OnSoapException(SoapException e)
    {
      RSExecutionConnection.SoapVersionMismatchException.ThrowIfVersionMismatch(e, "ReportExecution2005.asmx", SoapExceptionStrings.VersionMismatch, true);
    }

    private string[] GetSecureMethods()
    {
      try
      {
        this.SetConnectionSSL(this.m_alwaysUseSSL);
        return this.ListSecureMethods();
      }
      catch (Exception ex)
      {
        this.m_alwaysUseSSL = !this.m_alwaysUseSSL ? true : throw ex;
        this.SetConnectionSSL(true);
        try
        {
          return this.ListSecureMethods();
        }
        catch
        {
          this.m_alwaysUseSSL = false;
          if (!(ex is WebException webException) || !(webException.Response is HttpWebResponse response) || response.StatusCode != HttpStatusCode.Forbidden)
            throw ex;
          throw;
        }
      }
    }

    private bool IsSecureMethod(string methodname)
    {
      if (this.m_alwaysUseSSL)
        return true;
      if (this.m_secureMethods == null)
      {
        string[] secureMethods = this.GetSecureMethods();
        if (this.m_alwaysUseSSL)
          return true;
        this.m_secureMethods = new RSExecutionConnection.SecureMethodsList();
        foreach (string key in secureMethods)
          this.m_secureMethods.Add(key, (object) null);
      }
      return this.m_secureMethods.ContainsKey(methodname);
    }

    private bool CanUseKatmaiMethods
    {
      get
      {
        switch (this.m_endpointVersion)
        {
          case EndpointVersion.Yukon:
            return false;
          case EndpointVersion.Katmai:
            return true;
          case EndpointVersion.Automatic:
            return !this.m_failedUsingKatmai;
          default:
            return false;
        }
      }
    }

    private bool CheckForDownlevelRetry(SoapException e)
    {
      switch (this.m_endpointVersion)
      {
        case EndpointVersion.Yukon:
        case EndpointVersion.Automatic:
          return RSExecutionConnection.SoapVersionMismatchException.IsVersionMismatch(e, "ReportExecution2005.asmx");
        case EndpointVersion.Katmai:
          return false;
        default:
          return false;
      }
    }

    private void MarkAsFailedUsingKatmai() => this.m_failedUsingKatmai = true;

    public new ExecutionInfo LoadReport(string Report, string HistoryID)
    {
      return RSExecutionConnection.ProxyMethodInvocation.Execute<ExecutionInfo>(this, new RSExecutionConnection.ProxyMethod<ExecutionInfo>("LoadReport2", (RSExecutionConnection.ProxyMethod<ExecutionInfo>.ProxyMethodCallback) (() => (ExecutionInfo) this.LoadReport2(Report, HistoryID))), new RSExecutionConnection.ProxyMethod<ExecutionInfo>(nameof (LoadReport), (RSExecutionConnection.ProxyMethod<ExecutionInfo>.ProxyMethodCallback) (() => base.LoadReport(Report, HistoryID))));
    }

    public new ExecutionInfo LoadReportDefinition(byte[] Definition, out Warning[] warnings)
    {
      Warning[] w = (Warning[]) null;
      ExecutionInfo executionInfo = RSExecutionConnection.ProxyMethodInvocation.Execute<ExecutionInfo>(this, new RSExecutionConnection.ProxyMethod<ExecutionInfo>("LoadReportDefinition2", (RSExecutionConnection.ProxyMethod<ExecutionInfo>.ProxyMethodCallback) (() => (ExecutionInfo) this.LoadReportDefinition2(Definition, out w))), new RSExecutionConnection.ProxyMethod<ExecutionInfo>(nameof (LoadReportDefinition), (RSExecutionConnection.ProxyMethod<ExecutionInfo>.ProxyMethodCallback) (() => base.LoadReportDefinition(Definition, out w))));
      warnings = w;
      return executionInfo;
    }

    public new ExecutionInfo SetExecutionCredentials(DataSourceCredentials[] Credentials)
    {
      return RSExecutionConnection.ProxyMethodInvocation.Execute<ExecutionInfo>(this, new RSExecutionConnection.ProxyMethod<ExecutionInfo>("SetExecutionCredentials2", (RSExecutionConnection.ProxyMethod<ExecutionInfo>.ProxyMethodCallback) (() => (ExecutionInfo) this.SetExecutionCredentials2(Credentials))), new RSExecutionConnection.ProxyMethod<ExecutionInfo>(nameof (SetExecutionCredentials), (RSExecutionConnection.ProxyMethod<ExecutionInfo>.ProxyMethodCallback) (() => base.SetExecutionCredentials(Credentials))));
    }

    public new ExecutionInfo SetExecutionParameters(
      ParameterValue[] Parameters,
      string ParameterLanguage)
    {
      return RSExecutionConnection.ProxyMethodInvocation.Execute<ExecutionInfo>(this, new RSExecutionConnection.ProxyMethod<ExecutionInfo>("SetExecutionParameters2", (RSExecutionConnection.ProxyMethod<ExecutionInfo>.ProxyMethodCallback) (() => (ExecutionInfo) this.SetExecutionParameters2(Parameters, ParameterLanguage))), new RSExecutionConnection.ProxyMethod<ExecutionInfo>(nameof (SetExecutionParameters), (RSExecutionConnection.ProxyMethod<ExecutionInfo>.ProxyMethodCallback) (() => base.SetExecutionParameters(Parameters, ParameterLanguage))));
    }

    public new ExecutionInfo ResetExecution()
    {
      return RSExecutionConnection.ProxyMethodInvocation.Execute<ExecutionInfo>(this, new RSExecutionConnection.ProxyMethod<ExecutionInfo>("ResetExecution2", (RSExecutionConnection.ProxyMethod<ExecutionInfo>.ProxyMethodCallback) (() => (ExecutionInfo) this.ResetExecution2())), new RSExecutionConnection.ProxyMethod<ExecutionInfo>(nameof (ResetExecution), (RSExecutionConnection.ProxyMethod<ExecutionInfo>.ProxyMethodCallback) (() => base.ResetExecution())));
    }

    public new byte[] Render(
      string Format,
      string DeviceInfo,
      out string Extension,
      out string MimeType,
      out string Encoding,
      out Warning[] Warnings,
      out string[] StreamIds)
    {
      return this.Render(Format, DeviceInfo, PageCountMode.Actual, out Extension, out MimeType, out Encoding, out Warnings, out StreamIds);
    }

    public byte[] Render(
      string Format,
      string DeviceInfo,
      PageCountMode PaginationMode,
      out string Extension,
      out string MimeType,
      out string Encoding,
      out Warning[] Warnings,
      out string[] StreamIds)
    {
      string ext = (string) null;
      string mime = (string) null;
      string enc = (string) null;
      Warning[] w = (Warning[]) null;
      string[] sids = (string[]) null;
      byte[] numArray = RSExecutionConnection.ProxyMethodInvocation.Execute<byte[]>(this, new RSExecutionConnection.ProxyMethod<byte[]>("Render2", (RSExecutionConnection.ProxyMethod<byte[]>.ProxyMethodCallback) (() => this.Render2(Format, DeviceInfo, PaginationMode, out ext, out mime, out enc, out w, out sids))), new RSExecutionConnection.ProxyMethod<byte[]>(nameof (Render), (RSExecutionConnection.ProxyMethod<byte[]>.ProxyMethodCallback) (() => base.Render(Format, DeviceInfo, out ext, out mime, out enc, out w, out sids))));
      Extension = ext;
      MimeType = mime;
      Encoding = enc;
      Warnings = w;
      StreamIds = sids;
      return numArray;
    }

    public new byte[] RenderStream(
      string Format,
      string StreamID,
      string DeviceInfo,
      out string Encoding,
      out string MimeType)
    {
      string enc = (string) null;
      string mime = (string) null;
      byte[] numArray = RSExecutionConnection.ProxyMethodInvocation.Execute<byte[]>(this, new RSExecutionConnection.ProxyMethod<byte[]>(nameof (RenderStream), (RSExecutionConnection.ProxyMethod<byte[]>.ProxyMethodCallback) (() => base.RenderStream(Format, StreamID, DeviceInfo, out enc, out mime))));
      Encoding = enc;
      MimeType = mime;
      return numArray;
    }

    public new ExecutionInfo GetExecutionInfo()
    {
      return RSExecutionConnection.ProxyMethodInvocation.Execute<ExecutionInfo>(this, new RSExecutionConnection.ProxyMethod<ExecutionInfo>("GetExecutionInfo2", (RSExecutionConnection.ProxyMethod<ExecutionInfo>.ProxyMethodCallback) (() => (ExecutionInfo) this.GetExecutionInfo2())), new RSExecutionConnection.ProxyMethod<ExecutionInfo>(nameof (GetExecutionInfo), (RSExecutionConnection.ProxyMethod<ExecutionInfo>.ProxyMethodCallback) (() => base.GetExecutionInfo())));
    }

    public new DocumentMapNode GetDocumentMap()
    {
      return RSExecutionConnection.ProxyMethodInvocation.Execute<DocumentMapNode>(this, new RSExecutionConnection.ProxyMethod<DocumentMapNode>(nameof (GetDocumentMap), (RSExecutionConnection.ProxyMethod<DocumentMapNode>.ProxyMethodCallback) (() => base.GetDocumentMap())));
    }

    public new ExecutionInfo LoadDrillthroughTarget(string DrillthroughID)
    {
      return RSExecutionConnection.ProxyMethodInvocation.Execute<ExecutionInfo>(this, new RSExecutionConnection.ProxyMethod<ExecutionInfo>("LoadDrillthroughTarget2", (RSExecutionConnection.ProxyMethod<ExecutionInfo>.ProxyMethodCallback) (() => (ExecutionInfo) this.LoadDrillthroughTarget2(DrillthroughID))), new RSExecutionConnection.ProxyMethod<ExecutionInfo>(nameof (LoadDrillthroughTarget), (RSExecutionConnection.ProxyMethod<ExecutionInfo>.ProxyMethodCallback) (() => base.LoadDrillthroughTarget(DrillthroughID))));
    }

    public new bool ToggleItem(string ToggleID)
    {
      return RSExecutionConnection.ProxyMethodInvocation.Execute<bool>(this, new RSExecutionConnection.ProxyMethod<bool>(nameof (ToggleItem), (RSExecutionConnection.ProxyMethod<bool>.ProxyMethodCallback) (() => base.ToggleItem(ToggleID))));
    }

    public new int NavigateDocumentMap(string DocMapID)
    {
      return RSExecutionConnection.ProxyMethodInvocation.Execute<int>(this, new RSExecutionConnection.ProxyMethod<int>(nameof (NavigateDocumentMap), (RSExecutionConnection.ProxyMethod<int>.ProxyMethodCallback) (() => base.NavigateDocumentMap(DocMapID))));
    }

    public new int NavigateBookmark(string BookmarkID, out string UniqueName)
    {
      string name = (string) null;
      int num = RSExecutionConnection.ProxyMethodInvocation.Execute<int>(this, new RSExecutionConnection.ProxyMethod<int>(nameof (NavigateBookmark), (RSExecutionConnection.ProxyMethod<int>.ProxyMethodCallback) (() => base.NavigateBookmark(BookmarkID, out name))));
      UniqueName = name;
      return num;
    }

    public new int Sort(
      string SortItem,
      SortDirectionEnum Direction,
      bool Clear,
      out string ReportItem,
      out int NumPages)
    {
      string rptItem = (string) null;
      int nPages = 0;
      int num = RSExecutionConnection.ProxyMethodInvocation.Execute<int>(this, new RSExecutionConnection.ProxyMethod<int>(nameof (Sort), (RSExecutionConnection.ProxyMethod<int>.ProxyMethodCallback) (() => base.Sort(SortItem, Direction, Clear, out rptItem, out nPages))));
      ReportItem = rptItem;
      NumPages = nPages;
      return num;
    }

    public int Sort(
      string SortItem,
      SortDirectionEnum Direction,
      bool Clear,
      PageCountMode PaginationMode,
      out string ReportItem,
      out ExecutionInfo ExecutionInfo,
      out int NumPages)
    {
      string rptItem = (string) null;
      int nPages = 0;
      ExecutionInfo2 execInfo = (ExecutionInfo2) null;
      int num1 = RSExecutionConnection.ProxyMethodInvocation.Execute<int>(this, new RSExecutionConnection.ProxyMethod<int>("Sort2", (RSExecutionConnection.ProxyMethod<int>.ProxyMethodCallback) (() =>
      {
        int num2 = this.Sort2(SortItem, Direction, Clear, PaginationMode, out rptItem, out execInfo);
        if (execInfo != null)
          nPages = execInfo.NumPages;
        return num2;
      })), new RSExecutionConnection.ProxyMethod<int>(nameof (Sort), (RSExecutionConnection.ProxyMethod<int>.ProxyMethodCallback) (() => base.Sort(SortItem, Direction, Clear, out rptItem, out nPages))));
      ExecutionInfo = (ExecutionInfo) execInfo;
      NumPages = nPages;
      ReportItem = rptItem;
      return num1;
    }

    public new int FindString(int startPage, int endPage, string findValue)
    {
      return RSExecutionConnection.ProxyMethodInvocation.Execute<int>(this, new RSExecutionConnection.ProxyMethod<int>(nameof (FindString), (RSExecutionConnection.ProxyMethod<int>.ProxyMethodCallback) (() => base.FindString(startPage, endPage, findValue))));
    }

    public new byte[] GetRenderResource(string Format, string DeviceInfo, out string MimeType)
    {
      string mimeType = (string) null;
      byte[] renderResource = RSExecutionConnection.ProxyMethodInvocation.Execute<byte[]>(this, new RSExecutionConnection.ProxyMethod<byte[]>(nameof (GetRenderResource), (RSExecutionConnection.ProxyMethod<byte[]>.ProxyMethodCallback) (() => base.GetRenderResource(Format, DeviceInfo, out mimeType))));
      MimeType = mimeType;
      return renderResource;
    }

    public new Extension[] ListRenderingExtensions()
    {
      return RSExecutionConnection.ProxyMethodInvocation.Execute<Extension[]>(this, new RSExecutionConnection.ProxyMethod<Extension[]>(nameof (ListRenderingExtensions), (RSExecutionConnection.ProxyMethod<Extension[]>.ProxyMethodCallback) (() => base.ListRenderingExtensions())));
    }

    public new void LogonUser(string userName, string password, string authority)
    {
      RSExecutionConnection.ProxyMethodInvocation.Execute<int>(this, new RSExecutionConnection.ProxyMethod<int>((string) null, (RSExecutionConnection.ProxyMethod<int>.ProxyMethodCallback) (() =>
      {
        base.LogonUser(userName, password, authority);
        return 0;
      })));
    }

    public new void Logoff()
    {
      RSExecutionConnection.ProxyMethodInvocation.Execute<int>(this, new RSExecutionConnection.ProxyMethod<int>(nameof (Logoff), (RSExecutionConnection.ProxyMethod<int>.ProxyMethodCallback) (() =>
      {
        base.Logoff();
        return 0;
      })));
    }

    [Serializable]
    internal sealed class MissingEndpointException : Exception
    {
      public MissingEndpointException(Exception inner)
        : base(SoapExceptionStrings.MissingEndpoint, inner)
      {
      }

      private MissingEndpointException(SerializationInfo info, StreamingContext context)
        : base(info, context)
      {
      }

      public static void ThrowIfEndpointMissing(WebException e)
      {
        if (e.Status == WebExceptionStatus.ProtocolError && e.Response != null && e.Response is HttpWebResponse response && response.StatusCode == HttpStatusCode.NotFound)
          throw new RSExecutionConnection.MissingEndpointException((Exception) e);
      }
    }

    [Serializable]
    internal sealed class SoapVersionMismatchException : Exception
    {
      private SoapVersionMismatchException(string message, Exception inner)
        : base(message, inner)
      {
      }

      private SoapVersionMismatchException(SerializationInfo info, StreamingContext context)
        : base(info, context)
      {
      }

      public static void ThrowIfVersionMismatch(
        SoapException e,
        string expectedEndpoint,
        string message,
        bool includeInnerException)
      {
        if (!RSExecutionConnection.SoapVersionMismatchException.IsVersionMismatch(e, expectedEndpoint))
          return;
        if (includeInnerException)
          throw new RSExecutionConnection.SoapVersionMismatchException(message, (Exception) e);
        throw new RSExecutionConnection.SoapVersionMismatchException(message, (Exception) null);
      }

      public static bool IsVersionMismatch(SoapException e, string expectedEndpoint)
      {
        return e.Code == SoapException.ClientFaultCode && !e.Actor.EndsWith(expectedEndpoint, StringComparison.OrdinalIgnoreCase);
      }
    }

    private sealed class SecureMethodsList : Dictionary<string, object>
    {
      public SecureMethodsList()
        : base((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
      {
      }
    }

    private static class ProxyMethodInvocation
    {
      internal static TReturn Execute<TReturn>(
        RSExecutionConnection connection,
        RSExecutionConnection.ProxyMethod<TReturn> method)
      {
        return RSExecutionConnection.ProxyMethodInvocation.Execute<TReturn>(connection, method, (RSExecutionConnection.ProxyMethod<TReturn>) null);
      }

      internal static TReturn Execute<TReturn>(
        RSExecutionConnection connection,
        RSExecutionConnection.ProxyMethod<TReturn> initialMethod,
        RSExecutionConnection.ProxyMethod<TReturn> retryMethod)
      {
        using (MonitoredScope.NewConcat("ProxyMethodInvocation.Execute - Method : ", (object) initialMethod.MethodName))
        {
          if (connection == null)
            throw new ArgumentNullException(nameof (connection));
          if (initialMethod == null)
            throw new ArgumentNullException(nameof (initialMethod));
          RSExecutionConnection.ProxyMethod<TReturn>[] proxyMethodArray;
          if (retryMethod != null && !connection.CanUseKatmaiMethods)
            proxyMethodArray = new RSExecutionConnection.ProxyMethod<TReturn>[1]
            {
              retryMethod
            };
          else if (retryMethod == null)
            proxyMethodArray = new RSExecutionConnection.ProxyMethod<TReturn>[1]
            {
              initialMethod
            };
          else
            proxyMethodArray = new RSExecutionConnection.ProxyMethod<TReturn>[2]
            {
              initialMethod,
              retryMethod
            };
          for (int index = 0; index < proxyMethodArray.Length; ++index)
          {
            RSExecutionConnection.ProxyMethod<TReturn> proxyMethod = proxyMethodArray[index];
            try
            {
              if (!string.IsNullOrEmpty(proxyMethod.MethodName))
                connection.SetConnectionSSLForMethod(proxyMethod.MethodName);
              return proxyMethod.Method();
            }
            catch (SoapException ex)
            {
              if (index < proxyMethodArray.Length + 1 && connection.CheckForDownlevelRetry(ex))
              {
                connection.MarkAsFailedUsingKatmai();
              }
              else
              {
                connection.OnSoapException(ex);
                throw;
              }
            }
            catch (WebException ex)
            {
              RSExecutionConnection.MissingEndpointException.ThrowIfEndpointMissing(ex);
              throw;
            }
            catch (InvalidOperationException ex)
            {
              throw new RSExecutionConnection.MissingEndpointException((Exception) ex);
            }
          }
          throw new InvalidOperationException("Failed to execute method");
        }
      }
    }

    private sealed class ProxyMethod<TReturn>
    {
      private readonly RSExecutionConnection.ProxyMethod<TReturn>.ProxyMethodCallback m_method;
      private readonly string m_methodName;

      internal ProxyMethod(
        string methodName,
        RSExecutionConnection.ProxyMethod<TReturn>.ProxyMethodCallback method)
      {
        if (method == null)
          throw new ArgumentNullException(nameof (method));
        this.m_methodName = methodName;
        this.m_method = method;
      }

      internal RSExecutionConnection.ProxyMethod<TReturn>.ProxyMethodCallback Method
      {
        [DebuggerStepThrough] get => this.m_method;
      }

      internal string MethodName
      {
        [DebuggerStepThrough] get => this.m_methodName;
      }

      internal delegate TReturn ProxyMethodCallback();
    }
  }
}
