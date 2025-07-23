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
using Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution;
using Microsoft.ReportingServices.Common;
using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.Diagnostics.Utilities;

namespace Microsoft.Reporting.WebForms;

internal class SoapReportExecutionService : IReportExecutionService
{
	private sealed class ServerReportSoapProxy : RSExecutionConnection
	{
		private WindowsIdentity m_impersonationUser;

		private IEnumerable<string> m_headers;

		private IEnumerable<Cookie> m_cookies;

		public Cookie FormsAuthCookie;

		public ServerReportSoapProxy(WindowsIdentity impersonationUser, string reportServerLocation, IEnumerable<string> headers, IEnumerable<Cookie> cookies, EndpointVersion version)
			: base(reportServerLocation, version)
		{
			m_impersonationUser = impersonationUser;
			m_headers = headers;
			m_cookies = cookies;
		}

		protected override WebRequest GetWebRequest(Uri uri)
		{
			HttpWebRequest httpWebRequest = (HttpWebRequest)base.GetWebRequest(uri);
			WebRequestHelper.SetRequestHeaders(httpWebRequest, FormsAuthCookie, m_headers, m_cookies);
			return httpWebRequest;
		}

		protected override WebResponse GetWebResponse(WebRequest request)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Expected O, but got Unknown
			ServerImpersonationContext val = new ServerImpersonationContext(m_impersonationUser);
			try
			{
				HttpWebResponse httpWebResponse = (HttpWebResponse)base.GetWebResponse(request);
				string text = httpWebResponse.Headers["RSAuthenticationHeader"];
				if (text != null)
				{
					Cookie cookie = httpWebResponse.Cookies[text];
					if (cookie != null)
					{
						FormsAuthCookie = cookie;
					}
				}
				return httpWebResponse;
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}

		protected override void OnSoapException(SoapException e)
		{
			SoapVersionMismatchException.ThrowIfVersionMismatch(e, "ReportExecution2005.asmx", CommonStrings.UnsupportedReportServerError, includeInnerException: false);
			base.OnSoapException(e);
			throw ReportServerException.FromException(e);
		}
	}

	private const EndpointVersion EndpointVersion = EndpointVersion.Katmai;

	private const int BufferedReadSize = 81920;

	private WindowsIdentity m_impersonationUser;

	private Uri m_reportServerUrl;

	private IReportServerCredentials m_reportServerCredentials;

	private TrustedUserHeader m_trustedUserHeader;

	private IEnumerable<string> m_headers;

	private IEnumerable<Cookie> m_cookies;

	private int m_timeout;

	private ServerReportSoapProxy m_service;

	private ICredentials ServerNetworkCredentials
	{
		get
		{
			if (m_reportServerCredentials != null)
			{
				ICredentials networkCredentials = m_reportServerCredentials.NetworkCredentials;
				if (networkCredentials != null)
				{
					return networkCredentials;
				}
			}
			return DefaultCredentials;
		}
	}

	private ICredentials DefaultCredentials
	{
		[SecurityCritical]
		[SecurityTreatAsSafe]
		[EnvironmentPermission(SecurityAction.Assert, Read = "USERNAME")]
		get
		{
			return CredentialCache.DefaultCredentials;
		}
	}

	private ServerReportSoapProxy Service
	{
		get
		{
			if (m_service == null)
			{
				MonitoredScope val = MonitoredScope.New("SoapReportExecutionService.Service - proxy creation");
				try
				{
					ServerReportSoapProxy serverReportSoapProxy = new ServerReportSoapProxy(m_impersonationUser, m_reportServerUrl.ToString(), m_headers, m_cookies, EndpointVersion.Katmai);
					serverReportSoapProxy.Credentials = ServerNetworkCredentials;
					serverReportSoapProxy.Timeout = m_timeout;
					if (m_trustedUserHeader != null)
					{
						serverReportSoapProxy.TrustedUserHeaderValue = m_trustedUserHeader;
					}
					if (m_reportServerCredentials != null && m_reportServerCredentials.GetFormsCredentials(out var authCookie, out var userName, out var password, out var authority))
					{
						if (authCookie != null)
						{
							serverReportSoapProxy.FormsAuthCookie = authCookie;
						}
						else
						{
							serverReportSoapProxy.LogonUser(userName, password, authority);
						}
					}
					m_service = serverReportSoapProxy;
				}
				finally
				{
					((IDisposable)val)?.Dispose();
				}
			}
			return m_service;
		}
	}

	public int Timeout
	{
		set
		{
			m_timeout = value;
			if (m_service != null)
			{
				m_service.Timeout = value;
			}
		}
	}

	private int ServerMajorVersion
	{
		get
		{
			int result = 0;
			string serverVersion = GetServerVersion();
			if (!string.IsNullOrEmpty(serverVersion))
			{
				int num = serverVersion.IndexOf(".", StringComparison.Ordinal);
				if (num > 0)
				{
					string s = serverVersion.Substring(0, num);
					if (!int.TryParse(s, NumberStyles.None, CultureInfo.InvariantCulture, out result))
					{
						result = 0;
					}
				}
			}
			return result;
		}
	}

	public SoapReportExecutionService(WindowsIdentity impersonationUser, Uri reportServerUrl, IReportServerCredentials reportServerCredentials, TrustedUserHeader trustedUserHeader, IEnumerable<string> headers, IEnumerable<Cookie> cookies, int timeout)
	{
		m_impersonationUser = impersonationUser;
		m_reportServerUrl = reportServerUrl;
		m_reportServerCredentials = reportServerCredentials;
		m_trustedUserHeader = trustedUserHeader;
		m_headers = headers;
		m_cookies = cookies;
		m_timeout = timeout;
	}

	public ExecutionInfo GetExecutionInfo()
	{
		Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution.ExecutionInfo executionInfo = Service.GetExecutionInfo();
		return FromSoapExecutionInfo(executionInfo);
	}

	public ExecutionInfo ResetExecution()
	{
		Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution.ExecutionInfo soapExecutionInfo = Service.ResetExecution();
		return FromSoapExecutionInfo(soapExecutionInfo);
	}

	public ExecutionInfo LoadReport(string report, string historyId)
	{
		Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution.ExecutionInfo soapExecutionInfo = Service.LoadReport(report, historyId);
		return FromSoapExecutionInfo(soapExecutionInfo);
	}

	public ExecutionInfo LoadReportDefinition(byte[] definition)
	{
		Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution.Warning[] warnings;
		Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution.ExecutionInfo soapExecutionInfo = Service.LoadReportDefinition(definition, out warnings);
		return FromSoapExecutionInfo(soapExecutionInfo);
	}

	public DocumentMapNode GetDocumentMap(string rootLabel)
	{
		Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution.DocumentMapNode documentMap = Service.GetDocumentMap();
		return DocumentMapNode.CreateTree(documentMap, rootLabel);
	}

	public RenderingExtension[] ListRenderingExtensions()
	{
		Extension[] soapExtensions = Service.ListRenderingExtensions();
		return RenderingExtension.FromSoapExtensions(soapExtensions);
	}

	public ExecutionInfo SetExecutionCredentials(IEnumerable<DataSourceCredentials> credentials)
	{
		List<Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution.DataSourceCredentials> list = new List<Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution.DataSourceCredentials>();
		foreach (DataSourceCredentials credential in credentials)
		{
			if (credential == null)
			{
				throw new ArgumentNullException("credentials");
			}
			list.Add(credential.ToSoapCredentials());
		}
		Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution.ExecutionInfo soapExecutionInfo = Service.SetExecutionCredentials(list.ToArray());
		return FromSoapExecutionInfo(soapExecutionInfo);
	}

	public ExecutionInfo SetExecutionParameters(IEnumerable<ReportParameter> parameters, string parameterLanguage)
	{
		List<ParameterValue> list = new List<ParameterValue>();
		foreach (ReportParameter parameter in parameters)
		{
			StringEnumerator enumerator2 = parameter.Values.GetEnumerator();
			try
			{
				while (enumerator2.MoveNext())
				{
					string current2 = enumerator2.Current;
					ParameterValue parameterValue = new ParameterValue();
					parameterValue.Name = parameter.Name;
					parameterValue.Value = current2;
					list.Add(parameterValue);
				}
			}
			finally
			{
				if (enumerator2 is IDisposable disposable)
				{
					disposable.Dispose();
				}
			}
		}
		Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution.ExecutionInfo soapExecutionInfo = Service.SetExecutionParameters(list.ToArray(), parameterLanguage);
		return FromSoapExecutionInfo(soapExecutionInfo);
	}

	public byte[] Render(string format, string deviceInfo, PageCountMode paginationMode, out string extension, out string mimeType, out string encoding, out Warning[] warnings, out string[] streamIds)
	{
		Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution.PageCountMode paginationMode2 = SoapPageCountFromViewerAPI(paginationMode);
		Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution.Warning[] Warnings;
		byte[] result = Service.Render(format, deviceInfo, paginationMode2, out extension, out mimeType, out encoding, out Warnings, out streamIds);
		warnings = Warning.FromSoapWarnings(Warnings);
		return result;
	}

	public void Render(AbortState abortState, string reportPath, string executionId, string historyId, string format, XmlNodeList deviceInfo, NameValueCollection urlAccessParameters, Stream reportStream, out string mimeType, out string fileNameExtension)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "{0}?{1}&rs:SessionID={2}&rs:command=Render&rs:Format={3}", Service.UrlForRender, UrlUtil.UrlEncode(reportPath), UrlUtil.UrlEncode(executionId), UrlUtil.UrlEncode(format));
		if (!string.IsNullOrEmpty(historyId))
		{
			stringBuilder.Append("&rs:snapshot=");
			stringBuilder.Append(UrlUtil.UrlEncode(historyId));
		}
		if (deviceInfo != null)
		{
			foreach (XmlNode item in deviceInfo)
			{
				stringBuilder.Append("&rc:");
				stringBuilder.Append(UrlUtil.UrlEncode(item.Name));
				stringBuilder.Append("=");
				stringBuilder.Append(UrlUtil.UrlEncode(item.InnerText));
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
		ServerUrlRequest(abortState, stringBuilder.ToString(), reportStream, out mimeType, out fileNameExtension);
	}

	public byte[] RenderStream(string format, string streamId, string deviceInfo, out string encoding, out string mimeType)
	{
		return Service.RenderStream(format, streamId, deviceInfo, out encoding, out mimeType);
	}

	public bool IsPrintCabSupported(ClientArchitecture arch)
	{
		return arch switch
		{
			ClientArchitecture.X64 => ServerMajorVersion >= 2007, 
			ClientArchitecture.X86 => true, 
			_ => false, 
		};
	}

	public void WritePrintCab(ClientArchitecture arch, Stream stream)
	{
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Expected O, but got Unknown
		byte[] userToken = null;
		string userName = string.Empty;
		if (m_trustedUserHeader != null)
		{
			userName = m_trustedUserHeader.UserName;
			userToken = m_trustedUserHeader.UserToken;
		}
		string url;
		if (Service.PrintControlClsidHeaderValue == null)
		{
			string printCabFileName = ServerModeSession.GetPrintCabFileName(arch);
			url = string.Format(CultureInfo.InvariantCulture, "{0}?rs:command=Get&rc:GetImage={1}{2}&rs:ErrorResponseAsXml=true", m_reportServerUrl.ToString(), GetServerVersion(), printCabFileName);
		}
		else
		{
			url = string.Format(CultureInfo.InvariantCulture, "{0}?rs:command=Get&rs:PrintControlClsid={1}{2}&rs:ErrorResponseAsXml=true", m_reportServerUrl.ToString(), GetServerVersion(), UrlUtil.UrlEncode(GetPrintControlClsid(arch)));
		}
		HttpWebRequest serverUrlAccessObject = WebRequestHelper.GetServerUrlAccessObject(url, m_timeout, ServerNetworkCredentials, Service.FormsAuthCookie, m_headers, m_cookies, userName, userToken);
		try
		{
			ServerImpersonationContext val = new ServerImpersonationContext(m_impersonationUser);
			try
			{
				WebResponse response = serverUrlAccessObject.GetResponse();
				Stream responseStream = response.GetResponseStream();
				byte[] array = new byte[16384];
				int num = -1;
				while (num != 0)
				{
					num = responseStream.Read(array, 0, array.Length);
					stream.Write(array, 0, num);
				}
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
		catch (Exception e)
		{
			throw WebRequestHelper.ExceptionFromWebResponse(e);
		}
	}

	public string GetPrintControlClsid(ClientArchitecture arch)
	{
		if (Service.ServerInfoHeaderValue == null)
		{
			Service.ValidateConnection();
		}
		if (Service.PrintControlClsidHeaderValue == null)
		{
			if (arch != ClientArchitecture.X86)
			{
				return "60677965-AB8B-464f-9B04-4BA871A2F17F";
			}
			if (ServerMajorVersion < 2007)
			{
				return "41861299-EAB2-4DCC-986C-802AE12AC499";
			}
			return "5554DCB0-700B-498D-9B58-4E40E5814405";
		}
		if (arch != ClientArchitecture.X86)
		{
			return Service.PrintControlClsidHeaderValue.Clsid64;
		}
		return Service.PrintControlClsidHeaderValue.Clsid32;
	}

	public int FindString(int startPage, int endPage, string findValue)
	{
		return Service.FindString(startPage, endPage, findValue);
	}

	public void ToggleItem(string toggleId)
	{
		Service.ToggleItem(toggleId);
	}

	public int NavigateBookmark(string bookmarkId, out string uniqueName)
	{
		return Service.NavigateBookmark(bookmarkId, out uniqueName);
	}

	public int NavigateDocumentMap(string documentMapId)
	{
		return Service.NavigateDocumentMap(documentMapId);
	}

	public ExecutionInfo LoadDrillthroughTarget(string drillthroughId)
	{
		Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution.ExecutionInfo soapExecutionInfo = Service.LoadDrillthroughTarget(drillthroughId);
		return FromSoapExecutionInfo(soapExecutionInfo);
	}

	public int Sort(string sortItem, SortOrder direction, bool clear, PageCountMode paginationMode, out string reportItem, out ExecutionInfo executionInfo, out int numPages)
	{
		Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution.PageCountMode paginationMode2 = SoapPageCountFromViewerAPI(paginationMode);
		SortDirectionEnum direction2 = ((direction == SortOrder.Ascending) ? SortDirectionEnum.Ascending : SortDirectionEnum.Descending);
		Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution.ExecutionInfo ExecutionInfo;
		int result = Service.Sort(sortItem, direction2, clear, paginationMode2, out reportItem, out ExecutionInfo, out numPages);
		executionInfo = FromSoapExecutionInfo(ExecutionInfo);
		return result;
	}

	public byte[] GetStyleSheet(string styleSheetName, bool isImage, out string mimeType)
	{
		string url = string.Format(CultureInfo.InvariantCulture, "{0}?rs:command=StyleSheet{1}&Name={2}", m_reportServerUrl, isImage ? "Image" : "", HttpUtility.UrlEncode(styleSheetName ?? ""));
		using MemoryStream memoryStream = new MemoryStream();
		ServerUrlRequest(null, url, memoryStream, out mimeType, out var _);
		memoryStream.Position = 0L;
		byte[] array = new byte[memoryStream.Length];
		memoryStream.Read(array, 0, array.Length);
		return array;
	}

	public void SetExecutionId(string executionId)
	{
		if (executionId != null)
		{
			Service.ExecutionHeaderValue = new ExecutionHeader();
			Service.ExecutionHeaderValue.ExecutionID = executionId;
		}
		else
		{
			Service.ExecutionHeaderValue = null;
		}
	}

	public string GetServerVersion()
	{
		if (Service.ServerInfoHeaderValue == null)
		{
			Service.ValidateConnection();
		}
		return Service.ServerInfoHeaderValue.ReportServerVersionNumber;
	}

	private static ExecutionInfo FromSoapExecutionInfo(Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution.ExecutionInfo soapExecutionInfo)
	{
		if (soapExecutionInfo == null)
		{
			return null;
		}
		ReportParameterInfoCollection reportParameterInfoCollection = null;
		Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution.ReportParameter[] parameters = soapExecutionInfo.Parameters;
		if (parameters != null)
		{
			ReportParameterInfo[] array = new ReportParameterInfo[parameters.Length];
			for (int i = 0; i < parameters.Length; i++)
			{
				array[i] = SoapParameterToReportParameterInfo(parameters[i]);
			}
			reportParameterInfoCollection = new ReportParameterInfoCollection(array);
		}
		else
		{
			reportParameterInfoCollection = new ReportParameterInfoCollection();
		}
		PageCountMode pageCountMode = PageCountMode.Actual;
		if (soapExecutionInfo is ExecutionInfo2 { PageCountMode: Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution.PageCountMode.Estimate })
		{
			pageCountMode = PageCountMode.Estimate;
		}
		ReportPageSettings pageSettings = new ReportPageSettings(soapExecutionInfo.ReportPageSettings.PaperSize.Height, soapExecutionInfo.ReportPageSettings.PaperSize.Width, soapExecutionInfo.ReportPageSettings.Margins.Left, soapExecutionInfo.ReportPageSettings.Margins.Right, soapExecutionInfo.ReportPageSettings.Margins.Top, soapExecutionInfo.ReportPageSettings.Margins.Bottom);
		return new ExecutionInfo(soapExecutionInfo.ExecutionID, soapExecutionInfo.HistoryID, soapExecutionInfo.ReportPath, soapExecutionInfo.NumPages, soapExecutionInfo.HasDocumentMap, soapExecutionInfo.AutoRefreshInterval, soapExecutionInfo.CredentialsRequired, soapExecutionInfo.ParametersRequired, soapExecutionInfo.HasSnapshot, soapExecutionInfo.NeedsProcessing, soapExecutionInfo.ExpirationDateTime, soapExecutionInfo.AllowQueryExecution, pageCountMode, ReportDataSourceInfoCollection.FromSoapDataSourcePrompts(soapExecutionInfo.DataSourcePrompts), reportParameterInfoCollection, pageSettings);
	}

	private static ReportParameterInfo SoapParameterToReportParameterInfo(Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution.ReportParameter soapParam)
	{
		string[] array = null;
		if (soapParam.DefaultValues != null)
		{
			array = new string[soapParam.DefaultValues.Length];
			for (int i = 0; i < soapParam.DefaultValues.Length; i++)
			{
				array[i] = soapParam.DefaultValues[i];
			}
		}
		List<ValidValue> list = null;
		if (soapParam.ValidValues != null)
		{
			list = new List<ValidValue>(soapParam.ValidValues.Length);
			Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution.ValidValue[] validValues = soapParam.ValidValues;
			foreach (Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution.ValidValue validValue in validValues)
			{
				list.Add(new ValidValue(validValue.Label, validValue.Value));
			}
		}
		return new ReportParameterInfo(soapParam.Name, (ParameterDataType)Enum.Parse(typeof(ParameterDataType), soapParam.Type.ToString()), soapParam.Nullable, soapParam.AllowBlank, soapParam.MultiValue, soapParam.QueryParameterSpecified && soapParam.QueryParameter, soapParam.Prompt, soapParam.PromptUser, soapParam.DefaultValuesQueryBasedSpecified && soapParam.DefaultValuesQueryBased, soapParam.ValidValuesQueryBasedSpecified && soapParam.ValidValuesQueryBased, null, array, list, soapParam.Dependencies, (ParameterState)Enum.Parse(typeof(ParameterState), soapParam.State.ToString()));
	}

	private static Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution.PageCountMode SoapPageCountFromViewerAPI(PageCountMode pageCountMode)
	{
		if (pageCountMode == PageCountMode.Actual)
		{
			return Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution.PageCountMode.Actual;
		}
		return Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution.PageCountMode.Estimate;
	}

	private void ServerUrlRequest(AbortState abortState, string url, Stream outputStream, out string mimeType, out string fileNameExtension)
	{
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Expected O, but got Unknown
		byte[] userToken = null;
		string userName = string.Empty;
		if (m_trustedUserHeader != null)
		{
			userName = m_trustedUserHeader.UserName;
			userToken = m_trustedUserHeader.UserToken;
		}
		HttpWebRequest serverUrlAccessObject = WebRequestHelper.GetServerUrlAccessObject(url, m_timeout, ServerNetworkCredentials, Service.FormsAuthCookie, m_headers, m_cookies, userName, userToken);
		if (abortState != null && !abortState.RegisterAbortableRequest(serverUrlAccessObject))
		{
			throw new OperationCanceledException();
		}
		try
		{
			ServerImpersonationContext val = new ServerImpersonationContext(m_impersonationUser);
			try
			{
				IAsyncResult asyncResult = serverUrlAccessObject.BeginGetResponse(null, null);
				while (!asyncResult.AsyncWaitHandle.WaitOne(15000, exitContext: false))
				{
					if (HttpContext.Current != null && !HttpContext.Current.Response.IsClientConnected)
					{
						serverUrlAccessObject.Abort();
					}
				}
				WebResponse webResponse = serverUrlAccessObject.EndGetResponse(asyncResult);
				mimeType = webResponse.Headers["Content-Type"];
				fileNameExtension = webResponse.Headers["FileExtension"];
				Stream responseStream = webResponse.GetResponseStream();
				if (responseStream == null)
				{
					return;
				}
				using (responseStream)
				{
					byte[] array = new byte[81920];
					int count;
					while ((count = responseStream.Read(array, 0, array.Length)) > 0)
					{
						outputStream.Write(array, 0, count);
					}
				}
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
		catch (Exception e)
		{
			throw WebRequestHelper.ExceptionFromWebResponse(e);
		}
	}
}
