
using Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution;
using Microsoft.ReportingServices.Common;
using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.Diagnostics.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Net;
using System.Security;
using System.Security.Permissions;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.Services.Protocols;
using System.Xml;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal class SoapReportExecutionService : IReportExecutionService
  {
    private const EndpointVersion EndpointVersion = EndpointVersion.Katmai;
    private const int BufferedReadSize = 81920;
    private WindowsIdentity m_impersonationUser;
    private Uri m_reportServerUrl;
    private IReportServerCredentials m_reportServerCredentials;
    private TrustedUserHeader m_trustedUserHeader;
    private IEnumerable<string> m_headers;
    private IEnumerable<Cookie> m_cookies;
    private int m_timeout;
    private SoapReportExecutionService.ServerReportSoapProxy m_service;

    public SoapReportExecutionService(
      WindowsIdentity impersonationUser,
      Uri reportServerUrl,
      IReportServerCredentials reportServerCredentials,
      TrustedUserHeader trustedUserHeader,
      IEnumerable<string> headers,
      IEnumerable<Cookie> cookies,
      int timeout)
    {
      this.m_impersonationUser = impersonationUser;
      this.m_reportServerUrl = reportServerUrl;
      this.m_reportServerCredentials = reportServerCredentials;
      this.m_trustedUserHeader = trustedUserHeader;
      this.m_headers = headers;
      this.m_cookies = cookies;
      this.m_timeout = timeout;
    }

    private ICredentials ServerNetworkCredentials
    {
      get
      {
        if (this.m_reportServerCredentials != null)
        {
          ICredentials networkCredentials = this.m_reportServerCredentials.NetworkCredentials;
          if (networkCredentials != null)
            return networkCredentials;
        }
        return this.DefaultCredentials;
      }
    }

    private ICredentials DefaultCredentials
    {
      [SecurityCritical, SecurityTreatAsSafe, EnvironmentPermission(SecurityAction.Assert, Read = "USERNAME")] get
      {
        return CredentialCache.DefaultCredentials;
      }
    }

    private SoapReportExecutionService.ServerReportSoapProxy Service
    {
      get
      {
        if (this.m_service == null)
        {
          using (MonitoredScope.New("SoapReportExecutionService.Service - proxy creation"))
          {
            SoapReportExecutionService.ServerReportSoapProxy serverReportSoapProxy = new SoapReportExecutionService.ServerReportSoapProxy(this.m_impersonationUser, this.m_reportServerUrl.ToString(), this.m_headers, this.m_cookies, EndpointVersion.Katmai);
            serverReportSoapProxy.Credentials = this.ServerNetworkCredentials;
            serverReportSoapProxy.Timeout = this.m_timeout;
            if (this.m_trustedUserHeader != null)
              serverReportSoapProxy.TrustedUserHeaderValue = this.m_trustedUserHeader;
            Cookie authCookie;
            string userName;
            string password;
            string authority;
            if (this.m_reportServerCredentials != null && this.m_reportServerCredentials.GetFormsCredentials(out authCookie, out userName, out password, out authority))
            {
              if (authCookie != null)
                serverReportSoapProxy.FormsAuthCookie = authCookie;
              else
                serverReportSoapProxy.LogonUser(userName, password, authority);
            }
            this.m_service = serverReportSoapProxy;
          }
        }
        return this.m_service;
      }
    }

    public ExecutionInfo GetExecutionInfo()
    {
      return SoapReportExecutionService.FromSoapExecutionInfo(this.Service.GetExecutionInfo());
    }

    public ExecutionInfo ResetExecution()
    {
      return SoapReportExecutionService.FromSoapExecutionInfo(this.Service.ResetExecution());
    }

    public ExecutionInfo LoadReport(string report, string historyId)
    {
      return SoapReportExecutionService.FromSoapExecutionInfo(this.Service.LoadReport(report, historyId));
    }

    public ExecutionInfo LoadReportDefinition(byte[] definition)
    {
      return SoapReportExecutionService.FromSoapExecutionInfo(this.Service.LoadReportDefinition(definition, out Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution.Warning[] _));
    }

    public DocumentMapNode GetDocumentMap(string rootLabel)
    {
      return DocumentMapNode.CreateTree(this.Service.GetDocumentMap(), rootLabel);
    }

    public RenderingExtension[] ListRenderingExtensions()
    {
      return RenderingExtension.FromSoapExtensions(this.Service.ListRenderingExtensions());
    }

    public ExecutionInfo SetExecutionCredentials(IEnumerable<DataSourceCredentials> credentials)
    {
      List<Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution.DataSourceCredentials> sourceCredentialsList = new List<Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution.DataSourceCredentials>();
      foreach (DataSourceCredentials credential in credentials)
      {
        if (credential == null)
          throw new ArgumentNullException(nameof (credentials));
        sourceCredentialsList.Add(credential.ToSoapCredentials());
      }
      return SoapReportExecutionService.FromSoapExecutionInfo(this.Service.SetExecutionCredentials(sourceCredentialsList.ToArray()));
    }

    public ExecutionInfo SetExecutionParameters(
      IEnumerable<ReportParameter> parameters,
      string parameterLanguage)
    {
      List<ParameterValue> parameterValueList = new List<ParameterValue>();
      foreach (ReportParameter parameter in parameters)
      {
        foreach (string str in parameter.Values)
          parameterValueList.Add(new ParameterValue()
          {
            Name = parameter.Name,
            Value = str
          });
      }
      return SoapReportExecutionService.FromSoapExecutionInfo(this.Service.SetExecutionParameters(parameterValueList.ToArray(), parameterLanguage));
    }

    public byte[] Render(
      string format,
      string deviceInfo,
      PageCountMode paginationMode,
      out string extension,
      out string mimeType,
      out string encoding,
      out Warning[] warnings,
      out string[] streamIds)
    {
      Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution.PageCountMode PaginationMode = SoapReportExecutionService.SoapPageCountFromViewerAPI(paginationMode);
      Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution.Warning[] Warnings;
      byte[] numArray = this.Service.Render(format, deviceInfo, PaginationMode, out extension, out mimeType, out encoding, out Warnings, out streamIds);
      warnings = Warning.FromSoapWarnings(Warnings);
      return numArray;
    }

    public void Render(
      AbortState abortState,
      string reportPath,
      string executionId,
      string historyId,
      string format,
      XmlNodeList deviceInfo,
      NameValueCollection urlAccessParameters,
      Stream reportStream,
      out string mimeType,
      out string fileNameExtension)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "{0}?{1}&rs:SessionID={2}&rs:command=Render&rs:Format={3}", (object) this.Service.UrlForRender, (object) UrlUtil.UrlEncode(reportPath), (object) UrlUtil.UrlEncode(executionId), (object) UrlUtil.UrlEncode(format));
      if (!string.IsNullOrEmpty(historyId))
      {
        stringBuilder.Append("&rs:snapshot=");
        stringBuilder.Append(UrlUtil.UrlEncode(historyId));
      }
      if (deviceInfo != null)
      {
        foreach (XmlNode xmlNode in deviceInfo)
        {
          stringBuilder.Append("&rc:");
          stringBuilder.Append(UrlUtil.UrlEncode(xmlNode.Name));
          stringBuilder.Append("=");
          stringBuilder.Append(UrlUtil.UrlEncode(xmlNode.InnerText));
        }
      }
      stringBuilder.Append("&rc:Toolbar=false&rs:ErrorResponseAsXml=true&rs:AllowNewSessions=false");
      if (urlAccessParameters != null)
      {
        foreach (string key in urlAccessParameters.Keys)
        {
          stringBuilder.Append("&");
          stringBuilder.Append(UrlUtil.UrlEncode(key));
          stringBuilder.Append("=");
          stringBuilder.Append(UrlUtil.UrlEncode(urlAccessParameters[key]));
        }
      }
      this.ServerUrlRequest(abortState, stringBuilder.ToString(), reportStream, out mimeType, out fileNameExtension);
    }

    public byte[] RenderStream(
      string format,
      string streamId,
      string deviceInfo,
      out string encoding,
      out string mimeType)
    {
      return this.Service.RenderStream(format, streamId, deviceInfo, out encoding, out mimeType);
    }

    public bool IsPrintCabSupported(ClientArchitecture arch)
    {
      if (arch == ClientArchitecture.X86)
        return true;
      return arch == ClientArchitecture.X64 && this.ServerMajorVersion >= 2007;
    }

    public void WritePrintCab(ClientArchitecture arch, Stream stream)
    {
      byte[] userToken = (byte[]) null;
      string userName = string.Empty;
      if (this.m_trustedUserHeader != null)
      {
        userName = this.m_trustedUserHeader.UserName;
        userToken = this.m_trustedUserHeader.UserToken;
      }
      string url;
      if (this.Service.PrintControlClsidHeaderValue == null)
      {
        string printCabFileName = ServerModeSession.GetPrintCabFileName(arch);
        url = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}?rs:command=Get&rc:GetImage={1}{2}&rs:ErrorResponseAsXml=true", (object) this.m_reportServerUrl.ToString(), (object) this.GetServerVersion(), (object) printCabFileName);
      }
      else
        url = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}?rs:command=Get&rs:PrintControlClsid={1}{2}&rs:ErrorResponseAsXml=true", (object) this.m_reportServerUrl.ToString(), (object) this.GetServerVersion(), (object) UrlUtil.UrlEncode(this.GetPrintControlClsid(arch)));
      HttpWebRequest serverUrlAccessObject = WebRequestHelper.GetServerUrlAccessObject(url, this.m_timeout, this.ServerNetworkCredentials, this.Service.FormsAuthCookie, this.m_headers, this.m_cookies, userName, userToken);
      try
      {
        using (new ServerImpersonationContext(this.m_impersonationUser))
        {
          Stream responseStream = serverUrlAccessObject.GetResponse().GetResponseStream();
          byte[] buffer = new byte[16384];
          int count = -1;
          while (count != 0)
          {
            count = responseStream.Read(buffer, 0, buffer.Length);
            stream.Write(buffer, 0, count);
          }
        }
      }
      catch (Exception ex)
      {
        throw WebRequestHelper.ExceptionFromWebResponse(ex);
      }
    }

    public string GetPrintControlClsid(ClientArchitecture arch)
    {
      if (this.Service.ServerInfoHeaderValue == null)
        this.Service.ValidateConnection();
      if (this.Service.PrintControlClsidHeaderValue == null)
      {
        if (arch != ClientArchitecture.X86)
          return "60677965-AB8B-464f-9B04-4BA871A2F17F";
        return this.ServerMajorVersion < 2007 ? "41861299-EAB2-4DCC-986C-802AE12AC499" : "5554DCB0-700B-498D-9B58-4E40E5814405";
      }
      return arch != ClientArchitecture.X86 ? this.Service.PrintControlClsidHeaderValue.Clsid64 : this.Service.PrintControlClsidHeaderValue.Clsid32;
    }

    public int FindString(int startPage, int endPage, string findValue)
    {
      return this.Service.FindString(startPage, endPage, findValue);
    }

    public void ToggleItem(string toggleId) => this.Service.ToggleItem(toggleId);

    public int NavigateBookmark(string bookmarkId, out string uniqueName)
    {
      return this.Service.NavigateBookmark(bookmarkId, out uniqueName);
    }

    public int NavigateDocumentMap(string documentMapId)
    {
      return this.Service.NavigateDocumentMap(documentMapId);
    }

    public ExecutionInfo LoadDrillthroughTarget(string drillthroughId)
    {
      return SoapReportExecutionService.FromSoapExecutionInfo(this.Service.LoadDrillthroughTarget(drillthroughId));
    }

    public int Sort(
      string sortItem,
      SortOrder direction,
      bool clear,
      PageCountMode paginationMode,
      out string reportItem,
      out ExecutionInfo executionInfo,
      out int numPages)
    {
      Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution.PageCountMode PaginationMode = SoapReportExecutionService.SoapPageCountFromViewerAPI(paginationMode);
      SortDirectionEnum Direction = direction == SortOrder.Ascending ? SortDirectionEnum.Ascending : SortDirectionEnum.Descending;
      Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution.ExecutionInfo ExecutionInfo;
      int num = this.Service.Sort(sortItem, Direction, clear, PaginationMode, out reportItem, out ExecutionInfo, out numPages);
      executionInfo = SoapReportExecutionService.FromSoapExecutionInfo(ExecutionInfo);
      return num;
    }

    public byte[] GetStyleSheet(string styleSheetName, bool isImage, out string mimeType)
    {
      using (MemoryStream outputStream = new MemoryStream())
      {
        this.ServerUrlRequest((AbortState) null, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}?rs:command=StyleSheet{1}&Name={2}", (object) this.m_reportServerUrl, isImage ? (object) "Image" : (object) "", (object) System.Web.HttpUtility.UrlEncode(styleSheetName ?? "")), (Stream) outputStream, out mimeType, out string _);
        outputStream.Position = 0L;
        byte[] buffer = new byte[outputStream.Length];
        outputStream.Read(buffer, 0, buffer.Length);
        return buffer;
      }
    }

    public void SetExecutionId(string executionId)
    {
      if (executionId != null)
      {
        this.Service.ExecutionHeaderValue = new ExecutionHeader();
        this.Service.ExecutionHeaderValue.ExecutionID = executionId;
      }
      else
        this.Service.ExecutionHeaderValue = (ExecutionHeader) null;
    }

    public string GetServerVersion()
    {
      if (this.Service.ServerInfoHeaderValue == null)
        this.Service.ValidateConnection();
      return this.Service.ServerInfoHeaderValue.ReportServerVersionNumber;
    }

    public int Timeout
    {
      set
      {
        this.m_timeout = value;
        if (this.m_service == null)
          return;
        this.m_service.Timeout = value;
      }
    }

    private int ServerMajorVersion
    {
      get
      {
        int result = 0;
        string serverVersion = this.GetServerVersion();
        if (!string.IsNullOrEmpty(serverVersion))
        {
          int length = serverVersion.IndexOf(".", StringComparison.Ordinal);
          if (length > 0 && !int.TryParse(serverVersion.Substring(0, length), NumberStyles.None, (IFormatProvider) CultureInfo.InvariantCulture, out result))
            result = 0;
        }
        return result;
      }
    }

    private static ExecutionInfo FromSoapExecutionInfo(Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution.ExecutionInfo soapExecutionInfo)
    {
      if (soapExecutionInfo == null)
        return (ExecutionInfo) null;
      Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution.ReportParameter[] parameters1 = soapExecutionInfo.Parameters;
      ReportParameterInfoCollection parameters2;
      if (parameters1 != null)
      {
        ReportParameterInfo[] parameterInfos = new ReportParameterInfo[parameters1.Length];
        for (int index = 0; index < parameters1.Length; ++index)
          parameterInfos[index] = SoapReportExecutionService.SoapParameterToReportParameterInfo(parameters1[index]);
        parameters2 = new ReportParameterInfoCollection((IList<ReportParameterInfo>) parameterInfos);
      }
      else
        parameters2 = new ReportParameterInfoCollection();
      PageCountMode pageCountMode = PageCountMode.Actual;
      if (soapExecutionInfo is ExecutionInfo2 executionInfo2 && executionInfo2.PageCountMode == Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution.PageCountMode.Estimate)
        pageCountMode = PageCountMode.Estimate;
      ReportPageSettings pageSettings = new ReportPageSettings(soapExecutionInfo.ReportPageSettings.PaperSize.Height, soapExecutionInfo.ReportPageSettings.PaperSize.Width, soapExecutionInfo.ReportPageSettings.Margins.Left, soapExecutionInfo.ReportPageSettings.Margins.Right, soapExecutionInfo.ReportPageSettings.Margins.Top, soapExecutionInfo.ReportPageSettings.Margins.Bottom);
      return new ExecutionInfo(soapExecutionInfo.ExecutionID, soapExecutionInfo.HistoryID, soapExecutionInfo.ReportPath, soapExecutionInfo.NumPages, soapExecutionInfo.HasDocumentMap, soapExecutionInfo.AutoRefreshInterval, soapExecutionInfo.CredentialsRequired, soapExecutionInfo.ParametersRequired, soapExecutionInfo.HasSnapshot, soapExecutionInfo.NeedsProcessing, soapExecutionInfo.ExpirationDateTime, soapExecutionInfo.AllowQueryExecution, pageCountMode, ReportDataSourceInfoCollection.FromSoapDataSourcePrompts(soapExecutionInfo.DataSourcePrompts), parameters2, pageSettings);
    }

    private static ReportParameterInfo SoapParameterToReportParameterInfo(Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution.ReportParameter soapParam)
    {
      string[] currentValues = (string[]) null;
      if (soapParam.DefaultValues != null)
      {
        currentValues = new string[soapParam.DefaultValues.Length];
        for (int index = 0; index < soapParam.DefaultValues.Length; ++index)
          currentValues[index] = soapParam.DefaultValues[index];
      }
      List<ValidValue> validValues = (List<ValidValue>) null;
      if (soapParam.ValidValues != null)
      {
        validValues = new List<ValidValue>(soapParam.ValidValues.Length);
        foreach (Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution.ValidValue validValue in soapParam.ValidValues)
          validValues.Add(new ValidValue(validValue.Label, validValue.Value));
      }
      return new ReportParameterInfo(soapParam.Name, (ParameterDataType) System.Enum.Parse(typeof (ParameterDataType), soapParam.Type.ToString()), soapParam.Nullable, soapParam.AllowBlank, soapParam.MultiValue, soapParam.QueryParameterSpecified && soapParam.QueryParameter, soapParam.Prompt, soapParam.PromptUser, soapParam.DefaultValuesQueryBasedSpecified && soapParam.DefaultValuesQueryBased, soapParam.ValidValuesQueryBasedSpecified && soapParam.ValidValuesQueryBased, (string) null, currentValues, (IList<ValidValue>) validValues, soapParam.Dependencies, (ParameterState) System.Enum.Parse(typeof (ParameterState), soapParam.State.ToString()));
    }

    private static Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution.PageCountMode SoapPageCountFromViewerAPI(
      PageCountMode pageCountMode)
    {
      return pageCountMode == PageCountMode.Actual ? Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution.PageCountMode.Actual : Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution.PageCountMode.Estimate;
    }

    private void ServerUrlRequest(
      AbortState abortState,
      string url,
      Stream outputStream,
      out string mimeType,
      out string fileNameExtension)
    {
      byte[] userToken = (byte[]) null;
      string userName = string.Empty;
      if (this.m_trustedUserHeader != null)
      {
        userName = this.m_trustedUserHeader.UserName;
        userToken = this.m_trustedUserHeader.UserToken;
      }
      HttpWebRequest serverUrlAccessObject = WebRequestHelper.GetServerUrlAccessObject(url, this.m_timeout, this.ServerNetworkCredentials, this.Service.FormsAuthCookie, this.m_headers, this.m_cookies, userName, userToken);
      if (abortState != null)
      {
        if (!abortState.RegisterAbortableRequest(serverUrlAccessObject))
          throw new OperationCanceledException();
      }
      try
      {
        using (new ServerImpersonationContext(this.m_impersonationUser))
        {
          IAsyncResult response1 = serverUrlAccessObject.BeginGetResponse((AsyncCallback) null, (object) null);
          while (!response1.AsyncWaitHandle.WaitOne(15000, false))
          {
            if (HttpContext.Current != null && !HttpContext.Current.Response.IsClientConnected)
              serverUrlAccessObject.Abort();
          }
          WebResponse response2 = serverUrlAccessObject.EndGetResponse(response1);
          mimeType = response2.Headers["Content-Type"];
          fileNameExtension = response2.Headers["FileExtension"];
          Stream responseStream = response2.GetResponseStream();
          if (responseStream == null)
            return;
          using (responseStream)
          {
            byte[] buffer = new byte[81920];
            int count;
            while ((count = responseStream.Read(buffer, 0, buffer.Length)) > 0)
              outputStream.Write(buffer, 0, count);
          }
        }
      }
      catch (Exception ex)
      {
        throw WebRequestHelper.ExceptionFromWebResponse(ex);
      }
    }

    private sealed class ServerReportSoapProxy : RSExecutionConnection
    {
      private WindowsIdentity m_impersonationUser;
      private IEnumerable<string> m_headers;
      private IEnumerable<Cookie> m_cookies;
      public Cookie FormsAuthCookie;

      public ServerReportSoapProxy(
        WindowsIdentity impersonationUser,
        string reportServerLocation,
        IEnumerable<string> headers,
        IEnumerable<Cookie> cookies,
        EndpointVersion version)
        : base(reportServerLocation, version)
      {
        this.m_impersonationUser = impersonationUser;
        this.m_headers = headers;
        this.m_cookies = cookies;
      }

      protected override WebRequest GetWebRequest(Uri uri)
      {
        HttpWebRequest webRequest = (HttpWebRequest) base.GetWebRequest(uri);
        WebRequestHelper.SetRequestHeaders(webRequest, this.FormsAuthCookie, this.m_headers, this.m_cookies);
        return (WebRequest) webRequest;
      }

      protected override WebResponse GetWebResponse(WebRequest request)
      {
        using (new ServerImpersonationContext(this.m_impersonationUser))
        {
          HttpWebResponse webResponse = (HttpWebResponse) base.GetWebResponse(request);
          string header = webResponse.Headers["RSAuthenticationHeader"];
          if (header != null)
          {
            Cookie cookie = webResponse.Cookies[header];
            if (cookie != null)
              this.FormsAuthCookie = cookie;
          }
          return (WebResponse) webResponse;
        }
      }

      protected override void OnSoapException(SoapException e)
      {
        RSExecutionConnection.SoapVersionMismatchException.ThrowIfVersionMismatch(e, "ReportExecution2005.asmx", CommonStrings.UnsupportedReportServerError, false);
        base.OnSoapException(e);
        throw ReportServerException.FromException((Exception) e);
      }
    }
  }
}
