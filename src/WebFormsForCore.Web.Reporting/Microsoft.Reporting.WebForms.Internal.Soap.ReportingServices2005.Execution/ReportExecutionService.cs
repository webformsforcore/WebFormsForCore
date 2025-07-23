using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Web.Services;
using System.Web.Services.Description;
using System.Web.Services.Protocols;
using System.Xml.Serialization;

namespace Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution;

[GeneratedCode("wsdl", "2.0.50727.42")]
[DebuggerStepThrough]
[XmlInclude(typeof(ParameterValueOrFieldReference))]
[EditorBrowsable(EditorBrowsableState.Never)]
[ToolboxItem(false)]
[DesignerCategory("code")]
[WebServiceBinding(Name = "ReportExecutionServiceSoap", Namespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices")]
public class ReportExecutionService : SoapHttpClientProtocol
{
	private TrustedUserHeader trustedUserHeaderValueField;

	private PrintControlClsidHeader printControlClsidHeaderValueField;

	private ServerInfoHeader serverInfoHeaderValueField;

	private SendOrPostCallback ListSecureMethodsOperationCompleted;

	private ExecutionHeader executionHeaderValueField;

	private SendOrPostCallback LoadReportOperationCompleted;

	private SendOrPostCallback LoadReport2OperationCompleted;

	private SendOrPostCallback LoadReportDefinitionOperationCompleted;

	private SendOrPostCallback LoadReportDefinition2OperationCompleted;

	private SendOrPostCallback SetExecutionCredentialsOperationCompleted;

	private SendOrPostCallback SetExecutionCredentials2OperationCompleted;

	private SendOrPostCallback SetExecutionParametersOperationCompleted;

	private SendOrPostCallback SetExecutionParameters2OperationCompleted;

	private SendOrPostCallback ResetExecutionOperationCompleted;

	private SendOrPostCallback ResetExecution2OperationCompleted;

	private SendOrPostCallback RenderOperationCompleted;

	private SendOrPostCallback Render2OperationCompleted;

	private SendOrPostCallback RenderStreamOperationCompleted;

	private SendOrPostCallback GetExecutionInfoOperationCompleted;

	private SendOrPostCallback GetExecutionInfo2OperationCompleted;

	private SendOrPostCallback GetDocumentMapOperationCompleted;

	private SendOrPostCallback LoadDrillthroughTargetOperationCompleted;

	private SendOrPostCallback LoadDrillthroughTarget2OperationCompleted;

	private SendOrPostCallback ToggleItemOperationCompleted;

	private SendOrPostCallback NavigateDocumentMapOperationCompleted;

	private SendOrPostCallback NavigateBookmarkOperationCompleted;

	private SendOrPostCallback FindStringOperationCompleted;

	private SendOrPostCallback SortOperationCompleted;

	private SendOrPostCallback Sort2OperationCompleted;

	private SendOrPostCallback GetRenderResourceOperationCompleted;

	private SendOrPostCallback ListRenderingExtensionsOperationCompleted;

	private SendOrPostCallback LogonUserOperationCompleted;

	private SendOrPostCallback LogoffOperationCompleted;

	public TrustedUserHeader TrustedUserHeaderValue
	{
		get
		{
			return trustedUserHeaderValueField;
		}
		set
		{
			trustedUserHeaderValueField = value;
		}
	}

	public PrintControlClsidHeader PrintControlClsidHeaderValue
	{
		get
		{
			return printControlClsidHeaderValueField;
		}
		set
		{
			printControlClsidHeaderValueField = value;
		}
	}

	public ServerInfoHeader ServerInfoHeaderValue
	{
		get
		{
			return serverInfoHeaderValueField;
		}
		set
		{
			serverInfoHeaderValueField = value;
		}
	}

	public ExecutionHeader ExecutionHeaderValue
	{
		get
		{
			return executionHeaderValueField;
		}
		set
		{
			executionHeaderValueField = value;
		}
	}

	public event ListSecureMethodsCompletedEventHandler ListSecureMethodsCompleted;

	public event LoadReportCompletedEventHandler LoadReportCompleted;

	public event LoadReport2CompletedEventHandler LoadReport2Completed;

	public event LoadReportDefinitionCompletedEventHandler LoadReportDefinitionCompleted;

	public event LoadReportDefinition2CompletedEventHandler LoadReportDefinition2Completed;

	public event SetExecutionCredentialsCompletedEventHandler SetExecutionCredentialsCompleted;

	public event SetExecutionCredentials2CompletedEventHandler SetExecutionCredentials2Completed;

	public event SetExecutionParametersCompletedEventHandler SetExecutionParametersCompleted;

	public event SetExecutionParameters2CompletedEventHandler SetExecutionParameters2Completed;

	public event ResetExecutionCompletedEventHandler ResetExecutionCompleted;

	public event ResetExecution2CompletedEventHandler ResetExecution2Completed;

	public event RenderCompletedEventHandler RenderCompleted;

	public event Render2CompletedEventHandler Render2Completed;

	public event RenderStreamCompletedEventHandler RenderStreamCompleted;

	public event GetExecutionInfoCompletedEventHandler GetExecutionInfoCompleted;

	public event GetExecutionInfo2CompletedEventHandler GetExecutionInfo2Completed;

	public event GetDocumentMapCompletedEventHandler GetDocumentMapCompleted;

	public event LoadDrillthroughTargetCompletedEventHandler LoadDrillthroughTargetCompleted;

	public event LoadDrillthroughTarget2CompletedEventHandler LoadDrillthroughTarget2Completed;

	public event ToggleItemCompletedEventHandler ToggleItemCompleted;

	public event NavigateDocumentMapCompletedEventHandler NavigateDocumentMapCompleted;

	public event NavigateBookmarkCompletedEventHandler NavigateBookmarkCompleted;

	public event FindStringCompletedEventHandler FindStringCompleted;

	public event SortCompletedEventHandler SortCompleted;

	public event Sort2CompletedEventHandler Sort2Completed;

	public event GetRenderResourceCompletedEventHandler GetRenderResourceCompleted;

	public event ListRenderingExtensionsCompletedEventHandler ListRenderingExtensionsCompleted;

	public event LogonUserCompletedEventHandler LogonUserCompleted;

	public event LogoffCompletedEventHandler LogoffCompleted;

	public ReportExecutionService()
	{
		base.Url = "http://localhost/ReportServer/ReportExecution2005.asmx";
	}

	[SoapHeader("TrustedUserHeaderValue")]
	[SoapHeader("PrintControlClsidHeaderValue", Direction = SoapHeaderDirection.Out)]
	[SoapHeader("ServerInfoHeaderValue", Direction = SoapHeaderDirection.Out)]
	[SoapDocumentMethod("http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices/ListSecureMethods", RequestNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", ResponseNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
	public string[] ListSecureMethods()
	{
		object[] array = Invoke("ListSecureMethods", new object[0]);
		return (string[])array[0];
	}

	protected new object[] Invoke(string methodName, object[] parameters)
	{
		return base.Invoke(methodName, parameters);
	}

	public IAsyncResult BeginListSecureMethods(AsyncCallback callback, object asyncState)
	{
		return BeginInvoke("ListSecureMethods", new object[0], callback, asyncState);
	}

	public string[] EndListSecureMethods(IAsyncResult asyncResult)
	{
		object[] array = EndInvoke(asyncResult);
		return (string[])array[0];
	}

	public void ListSecureMethodsAsync()
	{
		ListSecureMethodsAsync(null);
	}

	public void ListSecureMethodsAsync(object userState)
	{
		if (ListSecureMethodsOperationCompleted == null)
		{
			ListSecureMethodsOperationCompleted = OnListSecureMethodsOperationCompleted;
		}
		InvokeAsync("ListSecureMethods", new object[0], ListSecureMethodsOperationCompleted, userState);
	}

	private void OnListSecureMethodsOperationCompleted(object arg)
	{
		if (this.ListSecureMethodsCompleted != null)
		{
			InvokeCompletedEventArgs e = (InvokeCompletedEventArgs)arg;
			this.ListSecureMethodsCompleted(this, new ListSecureMethodsCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
		}
	}

	[SoapDocumentMethod("http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices/LoadReport", RequestNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", ResponseNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
	[SoapHeader("ExecutionHeaderValue", Direction = SoapHeaderDirection.Out)]
	[SoapHeader("TrustedUserHeaderValue")]
	[SoapHeader("ServerInfoHeaderValue", Direction = SoapHeaderDirection.Out)]
	[SoapHeader("PrintControlClsidHeaderValue", Direction = SoapHeaderDirection.Out)]
	[return: XmlElement("executionInfo")]
	public ExecutionInfo LoadReport(string Report, string HistoryID)
	{
		object[] array = Invoke("LoadReport", new object[2] { Report, HistoryID });
		return (ExecutionInfo)array[0];
	}

	public IAsyncResult BeginLoadReport(string Report, string HistoryID, AsyncCallback callback, object asyncState)
	{
		return BeginInvoke("LoadReport", new object[2] { Report, HistoryID }, callback, asyncState);
	}

	public ExecutionInfo EndLoadReport(IAsyncResult asyncResult)
	{
		object[] array = EndInvoke(asyncResult);
		return (ExecutionInfo)array[0];
	}

	public void LoadReportAsync(string Report, string HistoryID)
	{
		LoadReportAsync(Report, HistoryID, null);
	}

	public void LoadReportAsync(string Report, string HistoryID, object userState)
	{
		if (LoadReportOperationCompleted == null)
		{
			LoadReportOperationCompleted = OnLoadReportOperationCompleted;
		}
		InvokeAsync("LoadReport", new object[2] { Report, HistoryID }, LoadReportOperationCompleted, userState);
	}

	private void OnLoadReportOperationCompleted(object arg)
	{
		if (this.LoadReportCompleted != null)
		{
			InvokeCompletedEventArgs e = (InvokeCompletedEventArgs)arg;
			this.LoadReportCompleted(this, new LoadReportCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
		}
	}

	[SoapHeader("PrintControlClsidHeaderValue", Direction = SoapHeaderDirection.Out)]
	[SoapHeader("ExecutionHeaderValue", Direction = SoapHeaderDirection.Out)]
	[SoapHeader("TrustedUserHeaderValue")]
	[SoapDocumentMethod("http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices/LoadReport2", RequestNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", ResponseNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
	[SoapHeader("ServerInfoHeaderValue", Direction = SoapHeaderDirection.Out)]
	[return: XmlElement("executionInfo")]
	public ExecutionInfo2 LoadReport2(string Report, string HistoryID)
	{
		object[] array = Invoke("LoadReport2", new object[2] { Report, HistoryID });
		return (ExecutionInfo2)array[0];
	}

	public IAsyncResult BeginLoadReport2(string Report, string HistoryID, AsyncCallback callback, object asyncState)
	{
		return BeginInvoke("LoadReport2", new object[2] { Report, HistoryID }, callback, asyncState);
	}

	public ExecutionInfo2 EndLoadReport2(IAsyncResult asyncResult)
	{
		object[] array = EndInvoke(asyncResult);
		return (ExecutionInfo2)array[0];
	}

	public void LoadReport2Async(string Report, string HistoryID)
	{
		LoadReport2Async(Report, HistoryID, null);
	}

	public void LoadReport2Async(string Report, string HistoryID, object userState)
	{
		if (LoadReport2OperationCompleted == null)
		{
			LoadReport2OperationCompleted = OnLoadReport2OperationCompleted;
		}
		InvokeAsync("LoadReport2", new object[2] { Report, HistoryID }, LoadReport2OperationCompleted, userState);
	}

	private void OnLoadReport2OperationCompleted(object arg)
	{
		if (this.LoadReport2Completed != null)
		{
			InvokeCompletedEventArgs e = (InvokeCompletedEventArgs)arg;
			this.LoadReport2Completed(this, new LoadReport2CompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
		}
	}

	[SoapHeader("PrintControlClsidHeaderValue", Direction = SoapHeaderDirection.Out)]
	[SoapHeader("ServerInfoHeaderValue", Direction = SoapHeaderDirection.Out)]
	[SoapHeader("ExecutionHeaderValue", Direction = SoapHeaderDirection.Out)]
	[SoapHeader("TrustedUserHeaderValue")]
	[SoapDocumentMethod("http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices/LoadReportDefinition", RequestNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", ResponseNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
	[return: XmlElement("executionInfo")]
	public ExecutionInfo LoadReportDefinition([XmlElement(DataType = "base64Binary")] byte[] Definition, out Warning[] warnings)
	{
		object[] array = Invoke("LoadReportDefinition", new object[1] { Definition });
		warnings = (Warning[])array[1];
		return (ExecutionInfo)array[0];
	}

	public IAsyncResult BeginLoadReportDefinition(byte[] Definition, AsyncCallback callback, object asyncState)
	{
		return BeginInvoke("LoadReportDefinition", new object[1] { Definition }, callback, asyncState);
	}

	public ExecutionInfo EndLoadReportDefinition(IAsyncResult asyncResult, out Warning[] warnings)
	{
		object[] array = EndInvoke(asyncResult);
		warnings = (Warning[])array[1];
		return (ExecutionInfo)array[0];
	}

	public void LoadReportDefinitionAsync(byte[] Definition)
	{
		LoadReportDefinitionAsync(Definition, null);
	}

	public void LoadReportDefinitionAsync(byte[] Definition, object userState)
	{
		if (LoadReportDefinitionOperationCompleted == null)
		{
			LoadReportDefinitionOperationCompleted = OnLoadReportDefinitionOperationCompleted;
		}
		InvokeAsync("LoadReportDefinition", new object[1] { Definition }, LoadReportDefinitionOperationCompleted, userState);
	}

	private void OnLoadReportDefinitionOperationCompleted(object arg)
	{
		if (this.LoadReportDefinitionCompleted != null)
		{
			InvokeCompletedEventArgs e = (InvokeCompletedEventArgs)arg;
			this.LoadReportDefinitionCompleted(this, new LoadReportDefinitionCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
		}
	}

	[SoapHeader("ServerInfoHeaderValue", Direction = SoapHeaderDirection.Out)]
	[SoapHeader("PrintControlClsidHeaderValue", Direction = SoapHeaderDirection.Out)]
	[SoapHeader("ExecutionHeaderValue", Direction = SoapHeaderDirection.Out)]
	[SoapHeader("TrustedUserHeaderValue")]
	[SoapDocumentMethod("http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices/LoadReportDefinition2", RequestNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", ResponseNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
	[return: XmlElement("executionInfo")]
	public ExecutionInfo2 LoadReportDefinition2([XmlElement(DataType = "base64Binary")] byte[] Definition, out Warning[] warnings)
	{
		object[] array = Invoke("LoadReportDefinition2", new object[1] { Definition });
		warnings = (Warning[])array[1];
		return (ExecutionInfo2)array[0];
	}

	public IAsyncResult BeginLoadReportDefinition2(byte[] Definition, AsyncCallback callback, object asyncState)
	{
		return BeginInvoke("LoadReportDefinition2", new object[1] { Definition }, callback, asyncState);
	}

	public ExecutionInfo2 EndLoadReportDefinition2(IAsyncResult asyncResult, out Warning[] warnings)
	{
		object[] array = EndInvoke(asyncResult);
		warnings = (Warning[])array[1];
		return (ExecutionInfo2)array[0];
	}

	public void LoadReportDefinition2Async(byte[] Definition)
	{
		LoadReportDefinition2Async(Definition, null);
	}

	public void LoadReportDefinition2Async(byte[] Definition, object userState)
	{
		if (LoadReportDefinition2OperationCompleted == null)
		{
			LoadReportDefinition2OperationCompleted = OnLoadReportDefinition2OperationCompleted;
		}
		InvokeAsync("LoadReportDefinition2", new object[1] { Definition }, LoadReportDefinition2OperationCompleted, userState);
	}

	private void OnLoadReportDefinition2OperationCompleted(object arg)
	{
		if (this.LoadReportDefinition2Completed != null)
		{
			InvokeCompletedEventArgs e = (InvokeCompletedEventArgs)arg;
			this.LoadReportDefinition2Completed(this, new LoadReportDefinition2CompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
		}
	}

	[SoapHeader("TrustedUserHeaderValue")]
	[SoapHeader("ExecutionHeaderValue")]
	[SoapDocumentMethod("http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices/SetExecutionCredentials", RequestNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", ResponseNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
	[SoapHeader("ServerInfoHeaderValue", Direction = SoapHeaderDirection.Out)]
	[SoapHeader("PrintControlClsidHeaderValue", Direction = SoapHeaderDirection.Out)]
	[return: XmlElement("executionInfo")]
	public ExecutionInfo SetExecutionCredentials(DataSourceCredentials[] Credentials)
	{
		object[] array = Invoke("SetExecutionCredentials", new object[1] { Credentials });
		return (ExecutionInfo)array[0];
	}

	public IAsyncResult BeginSetExecutionCredentials(DataSourceCredentials[] Credentials, AsyncCallback callback, object asyncState)
	{
		return BeginInvoke("SetExecutionCredentials", new object[1] { Credentials }, callback, asyncState);
	}

	public ExecutionInfo EndSetExecutionCredentials(IAsyncResult asyncResult)
	{
		object[] array = EndInvoke(asyncResult);
		return (ExecutionInfo)array[0];
	}

	public void SetExecutionCredentialsAsync(DataSourceCredentials[] Credentials)
	{
		SetExecutionCredentialsAsync(Credentials, null);
	}

	public void SetExecutionCredentialsAsync(DataSourceCredentials[] Credentials, object userState)
	{
		if (SetExecutionCredentialsOperationCompleted == null)
		{
			SetExecutionCredentialsOperationCompleted = OnSetExecutionCredentialsOperationCompleted;
		}
		InvokeAsync("SetExecutionCredentials", new object[1] { Credentials }, SetExecutionCredentialsOperationCompleted, userState);
	}

	private void OnSetExecutionCredentialsOperationCompleted(object arg)
	{
		if (this.SetExecutionCredentialsCompleted != null)
		{
			InvokeCompletedEventArgs e = (InvokeCompletedEventArgs)arg;
			this.SetExecutionCredentialsCompleted(this, new SetExecutionCredentialsCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
		}
	}

	[SoapDocumentMethod("http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices/SetExecutionCredentials2", RequestNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", ResponseNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
	[SoapHeader("ExecutionHeaderValue")]
	[SoapHeader("TrustedUserHeaderValue")]
	[SoapHeader("ServerInfoHeaderValue", Direction = SoapHeaderDirection.Out)]
	[SoapHeader("PrintControlClsidHeaderValue", Direction = SoapHeaderDirection.Out)]
	[return: XmlElement("executionInfo")]
	public ExecutionInfo2 SetExecutionCredentials2(DataSourceCredentials[] Credentials)
	{
		object[] array = Invoke("SetExecutionCredentials2", new object[1] { Credentials });
		return (ExecutionInfo2)array[0];
	}

	public IAsyncResult BeginSetExecutionCredentials2(DataSourceCredentials[] Credentials, AsyncCallback callback, object asyncState)
	{
		return BeginInvoke("SetExecutionCredentials2", new object[1] { Credentials }, callback, asyncState);
	}

	public ExecutionInfo2 EndSetExecutionCredentials2(IAsyncResult asyncResult)
	{
		object[] array = EndInvoke(asyncResult);
		return (ExecutionInfo2)array[0];
	}

	public void SetExecutionCredentials2Async(DataSourceCredentials[] Credentials)
	{
		SetExecutionCredentials2Async(Credentials, null);
	}

	public void SetExecutionCredentials2Async(DataSourceCredentials[] Credentials, object userState)
	{
		if (SetExecutionCredentials2OperationCompleted == null)
		{
			SetExecutionCredentials2OperationCompleted = OnSetExecutionCredentials2OperationCompleted;
		}
		InvokeAsync("SetExecutionCredentials2", new object[1] { Credentials }, SetExecutionCredentials2OperationCompleted, userState);
	}

	private void OnSetExecutionCredentials2OperationCompleted(object arg)
	{
		if (this.SetExecutionCredentials2Completed != null)
		{
			InvokeCompletedEventArgs e = (InvokeCompletedEventArgs)arg;
			this.SetExecutionCredentials2Completed(this, new SetExecutionCredentials2CompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
		}
	}

	[SoapHeader("TrustedUserHeaderValue")]
	[SoapHeader("ExecutionHeaderValue")]
	[SoapHeader("ServerInfoHeaderValue", Direction = SoapHeaderDirection.Out)]
	[SoapHeader("PrintControlClsidHeaderValue", Direction = SoapHeaderDirection.Out)]
	[SoapDocumentMethod("http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices/SetExecutionParameters", RequestNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", ResponseNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
	[return: XmlElement("executionInfo")]
	public ExecutionInfo SetExecutionParameters(ParameterValue[] Parameters, string ParameterLanguage)
	{
		object[] array = Invoke("SetExecutionParameters", new object[2] { Parameters, ParameterLanguage });
		return (ExecutionInfo)array[0];
	}

	public IAsyncResult BeginSetExecutionParameters(ParameterValue[] Parameters, string ParameterLanguage, AsyncCallback callback, object asyncState)
	{
		return BeginInvoke("SetExecutionParameters", new object[2] { Parameters, ParameterLanguage }, callback, asyncState);
	}

	public ExecutionInfo EndSetExecutionParameters(IAsyncResult asyncResult)
	{
		object[] array = EndInvoke(asyncResult);
		return (ExecutionInfo)array[0];
	}

	public void SetExecutionParametersAsync(ParameterValue[] Parameters, string ParameterLanguage)
	{
		SetExecutionParametersAsync(Parameters, ParameterLanguage, null);
	}

	public void SetExecutionParametersAsync(ParameterValue[] Parameters, string ParameterLanguage, object userState)
	{
		if (SetExecutionParametersOperationCompleted == null)
		{
			SetExecutionParametersOperationCompleted = OnSetExecutionParametersOperationCompleted;
		}
		InvokeAsync("SetExecutionParameters", new object[2] { Parameters, ParameterLanguage }, SetExecutionParametersOperationCompleted, userState);
	}

	private void OnSetExecutionParametersOperationCompleted(object arg)
	{
		if (this.SetExecutionParametersCompleted != null)
		{
			InvokeCompletedEventArgs e = (InvokeCompletedEventArgs)arg;
			this.SetExecutionParametersCompleted(this, new SetExecutionParametersCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
		}
	}

	[SoapHeader("TrustedUserHeaderValue")]
	[SoapHeader("PrintControlClsidHeaderValue", Direction = SoapHeaderDirection.Out)]
	[SoapHeader("ServerInfoHeaderValue", Direction = SoapHeaderDirection.Out)]
	[SoapHeader("ExecutionHeaderValue")]
	[SoapDocumentMethod("http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices/SetExecutionParameters2", RequestNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", ResponseNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
	[return: XmlElement("executionInfo")]
	public ExecutionInfo2 SetExecutionParameters2(ParameterValue[] Parameters, string ParameterLanguage)
	{
		object[] array = Invoke("SetExecutionParameters2", new object[2] { Parameters, ParameterLanguage });
		return (ExecutionInfo2)array[0];
	}

	public IAsyncResult BeginSetExecutionParameters2(ParameterValue[] Parameters, string ParameterLanguage, AsyncCallback callback, object asyncState)
	{
		return BeginInvoke("SetExecutionParameters2", new object[2] { Parameters, ParameterLanguage }, callback, asyncState);
	}

	public ExecutionInfo2 EndSetExecutionParameters2(IAsyncResult asyncResult)
	{
		object[] array = EndInvoke(asyncResult);
		return (ExecutionInfo2)array[0];
	}

	public void SetExecutionParameters2Async(ParameterValue[] Parameters, string ParameterLanguage)
	{
		SetExecutionParameters2Async(Parameters, ParameterLanguage, null);
	}

	public void SetExecutionParameters2Async(ParameterValue[] Parameters, string ParameterLanguage, object userState)
	{
		if (SetExecutionParameters2OperationCompleted == null)
		{
			SetExecutionParameters2OperationCompleted = OnSetExecutionParameters2OperationCompleted;
		}
		InvokeAsync("SetExecutionParameters2", new object[2] { Parameters, ParameterLanguage }, SetExecutionParameters2OperationCompleted, userState);
	}

	private void OnSetExecutionParameters2OperationCompleted(object arg)
	{
		if (this.SetExecutionParameters2Completed != null)
		{
			InvokeCompletedEventArgs e = (InvokeCompletedEventArgs)arg;
			this.SetExecutionParameters2Completed(this, new SetExecutionParameters2CompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
		}
	}

	[SoapHeader("PrintControlClsidHeaderValue", Direction = SoapHeaderDirection.Out)]
	[SoapDocumentMethod("http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices/ResetExecution", RequestNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", ResponseNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
	[SoapHeader("ExecutionHeaderValue")]
	[SoapHeader("ServerInfoHeaderValue", Direction = SoapHeaderDirection.Out)]
	[SoapHeader("TrustedUserHeaderValue")]
	[return: XmlElement("executionInfo")]
	public ExecutionInfo ResetExecution()
	{
		object[] array = Invoke("ResetExecution", new object[0]);
		return (ExecutionInfo)array[0];
	}

	public IAsyncResult BeginResetExecution(AsyncCallback callback, object asyncState)
	{
		return BeginInvoke("ResetExecution", new object[0], callback, asyncState);
	}

	public ExecutionInfo EndResetExecution(IAsyncResult asyncResult)
	{
		object[] array = EndInvoke(asyncResult);
		return (ExecutionInfo)array[0];
	}

	public void ResetExecutionAsync()
	{
		ResetExecutionAsync(null);
	}

	public void ResetExecutionAsync(object userState)
	{
		if (ResetExecutionOperationCompleted == null)
		{
			ResetExecutionOperationCompleted = OnResetExecutionOperationCompleted;
		}
		InvokeAsync("ResetExecution", new object[0], ResetExecutionOperationCompleted, userState);
	}

	private void OnResetExecutionOperationCompleted(object arg)
	{
		if (this.ResetExecutionCompleted != null)
		{
			InvokeCompletedEventArgs e = (InvokeCompletedEventArgs)arg;
			this.ResetExecutionCompleted(this, new ResetExecutionCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
		}
	}

	[SoapHeader("PrintControlClsidHeaderValue", Direction = SoapHeaderDirection.Out)]
	[SoapHeader("ExecutionHeaderValue")]
	[SoapDocumentMethod("http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices/ResetExecution2", RequestNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", ResponseNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
	[SoapHeader("TrustedUserHeaderValue")]
	[SoapHeader("ServerInfoHeaderValue", Direction = SoapHeaderDirection.Out)]
	[return: XmlElement("executionInfo")]
	public ExecutionInfo2 ResetExecution2()
	{
		object[] array = Invoke("ResetExecution2", new object[0]);
		return (ExecutionInfo2)array[0];
	}

	public IAsyncResult BeginResetExecution2(AsyncCallback callback, object asyncState)
	{
		return BeginInvoke("ResetExecution2", new object[0], callback, asyncState);
	}

	public ExecutionInfo2 EndResetExecution2(IAsyncResult asyncResult)
	{
		object[] array = EndInvoke(asyncResult);
		return (ExecutionInfo2)array[0];
	}

	public void ResetExecution2Async()
	{
		ResetExecution2Async(null);
	}

	public void ResetExecution2Async(object userState)
	{
		if (ResetExecution2OperationCompleted == null)
		{
			ResetExecution2OperationCompleted = OnResetExecution2OperationCompleted;
		}
		InvokeAsync("ResetExecution2", new object[0], ResetExecution2OperationCompleted, userState);
	}

	private void OnResetExecution2OperationCompleted(object arg)
	{
		if (this.ResetExecution2Completed != null)
		{
			InvokeCompletedEventArgs e = (InvokeCompletedEventArgs)arg;
			this.ResetExecution2Completed(this, new ResetExecution2CompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
		}
	}

	[SoapHeader("ExecutionHeaderValue")]
	[SoapHeader("ServerInfoHeaderValue", Direction = SoapHeaderDirection.Out)]
	[SoapDocumentMethod("http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices/Render", RequestNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", ResponseNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
	[SoapHeader("TrustedUserHeaderValue")]
	[SoapHeader("PrintControlClsidHeaderValue", Direction = SoapHeaderDirection.Out)]
	[return: XmlElement("Result", DataType = "base64Binary")]
	public byte[] Render(string Format, string DeviceInfo, out string Extension, out string MimeType, out string Encoding, out Warning[] Warnings, out string[] StreamIds)
	{
		object[] array = Invoke("Render", new object[2] { Format, DeviceInfo });
		Extension = (string)array[1];
		MimeType = (string)array[2];
		Encoding = (string)array[3];
		Warnings = (Warning[])array[4];
		StreamIds = (string[])array[5];
		return (byte[])array[0];
	}

	public IAsyncResult BeginRender(string Format, string DeviceInfo, AsyncCallback callback, object asyncState)
	{
		return BeginInvoke("Render", new object[2] { Format, DeviceInfo }, callback, asyncState);
	}

	public byte[] EndRender(IAsyncResult asyncResult, out string Extension, out string MimeType, out string Encoding, out Warning[] Warnings, out string[] StreamIds)
	{
		object[] array = EndInvoke(asyncResult);
		Extension = (string)array[1];
		MimeType = (string)array[2];
		Encoding = (string)array[3];
		Warnings = (Warning[])array[4];
		StreamIds = (string[])array[5];
		return (byte[])array[0];
	}

	public void RenderAsync(string Format, string DeviceInfo)
	{
		RenderAsync(Format, DeviceInfo, null);
	}

	public void RenderAsync(string Format, string DeviceInfo, object userState)
	{
		if (RenderOperationCompleted == null)
		{
			RenderOperationCompleted = OnRenderOperationCompleted;
		}
		InvokeAsync("Render", new object[2] { Format, DeviceInfo }, RenderOperationCompleted, userState);
	}

	private void OnRenderOperationCompleted(object arg)
	{
		if (this.RenderCompleted != null)
		{
			InvokeCompletedEventArgs e = (InvokeCompletedEventArgs)arg;
			this.RenderCompleted(this, new RenderCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
		}
	}

	[SoapHeader("ServerInfoHeaderValue", Direction = SoapHeaderDirection.Out)]
	[SoapDocumentMethod("http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices/Render2", RequestNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", ResponseNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
	[SoapHeader("PrintControlClsidHeaderValue", Direction = SoapHeaderDirection.Out)]
	[SoapHeader("ExecutionHeaderValue")]
	[SoapHeader("TrustedUserHeaderValue")]
	[return: XmlElement("Result", DataType = "base64Binary")]
	public byte[] Render2(string Format, string DeviceInfo, PageCountMode PaginationMode, out string Extension, out string MimeType, out string Encoding, out Warning[] Warnings, out string[] StreamIds)
	{
		object[] array = Invoke("Render2", new object[3] { Format, DeviceInfo, PaginationMode });
		Extension = (string)array[1];
		MimeType = (string)array[2];
		Encoding = (string)array[3];
		Warnings = (Warning[])array[4];
		StreamIds = (string[])array[5];
		return (byte[])array[0];
	}

	public IAsyncResult BeginRender2(string Format, string DeviceInfo, PageCountMode PaginationMode, AsyncCallback callback, object asyncState)
	{
		return BeginInvoke("Render2", new object[3] { Format, DeviceInfo, PaginationMode }, callback, asyncState);
	}

	public byte[] EndRender2(IAsyncResult asyncResult, out string Extension, out string MimeType, out string Encoding, out Warning[] Warnings, out string[] StreamIds)
	{
		object[] array = EndInvoke(asyncResult);
		Extension = (string)array[1];
		MimeType = (string)array[2];
		Encoding = (string)array[3];
		Warnings = (Warning[])array[4];
		StreamIds = (string[])array[5];
		return (byte[])array[0];
	}

	public void Render2Async(string Format, string DeviceInfo, PageCountMode PaginationMode)
	{
		Render2Async(Format, DeviceInfo, PaginationMode, null);
	}

	public void Render2Async(string Format, string DeviceInfo, PageCountMode PaginationMode, object userState)
	{
		if (Render2OperationCompleted == null)
		{
			Render2OperationCompleted = OnRender2OperationCompleted;
		}
		InvokeAsync("Render2", new object[3] { Format, DeviceInfo, PaginationMode }, Render2OperationCompleted, userState);
	}

	private void OnRender2OperationCompleted(object arg)
	{
		if (this.Render2Completed != null)
		{
			InvokeCompletedEventArgs e = (InvokeCompletedEventArgs)arg;
			this.Render2Completed(this, new Render2CompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
		}
	}

	[SoapHeader("TrustedUserHeaderValue")]
	[SoapDocumentMethod("http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices/RenderStream", RequestNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", ResponseNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
	[SoapHeader("ServerInfoHeaderValue", Direction = SoapHeaderDirection.Out)]
	[SoapHeader("PrintControlClsidHeaderValue", Direction = SoapHeaderDirection.Out)]
	[SoapHeader("ExecutionHeaderValue")]
	[return: XmlElement("Result", DataType = "base64Binary")]
	public byte[] RenderStream(string Format, string StreamID, string DeviceInfo, out string Encoding, out string MimeType)
	{
		object[] array = Invoke("RenderStream", new object[3] { Format, StreamID, DeviceInfo });
		Encoding = (string)array[1];
		MimeType = (string)array[2];
		return (byte[])array[0];
	}

	public IAsyncResult BeginRenderStream(string Format, string StreamID, string DeviceInfo, AsyncCallback callback, object asyncState)
	{
		return BeginInvoke("RenderStream", new object[3] { Format, StreamID, DeviceInfo }, callback, asyncState);
	}

	public byte[] EndRenderStream(IAsyncResult asyncResult, out string Encoding, out string MimeType)
	{
		object[] array = EndInvoke(asyncResult);
		Encoding = (string)array[1];
		MimeType = (string)array[2];
		return (byte[])array[0];
	}

	public void RenderStreamAsync(string Format, string StreamID, string DeviceInfo)
	{
		RenderStreamAsync(Format, StreamID, DeviceInfo, null);
	}

	public void RenderStreamAsync(string Format, string StreamID, string DeviceInfo, object userState)
	{
		if (RenderStreamOperationCompleted == null)
		{
			RenderStreamOperationCompleted = OnRenderStreamOperationCompleted;
		}
		InvokeAsync("RenderStream", new object[3] { Format, StreamID, DeviceInfo }, RenderStreamOperationCompleted, userState);
	}

	private void OnRenderStreamOperationCompleted(object arg)
	{
		if (this.RenderStreamCompleted != null)
		{
			InvokeCompletedEventArgs e = (InvokeCompletedEventArgs)arg;
			this.RenderStreamCompleted(this, new RenderStreamCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
		}
	}

	[SoapDocumentMethod("http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices/GetExecutionInfo", RequestNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", ResponseNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
	[SoapHeader("TrustedUserHeaderValue")]
	[SoapHeader("ServerInfoHeaderValue", Direction = SoapHeaderDirection.Out)]
	[SoapHeader("PrintControlClsidHeaderValue", Direction = SoapHeaderDirection.Out)]
	[SoapHeader("ExecutionHeaderValue")]
	[return: XmlElement("executionInfo")]
	public ExecutionInfo GetExecutionInfo()
	{
		object[] array = Invoke("GetExecutionInfo", new object[0]);
		return (ExecutionInfo)array[0];
	}

	public IAsyncResult BeginGetExecutionInfo(AsyncCallback callback, object asyncState)
	{
		return BeginInvoke("GetExecutionInfo", new object[0], callback, asyncState);
	}

	public ExecutionInfo EndGetExecutionInfo(IAsyncResult asyncResult)
	{
		object[] array = EndInvoke(asyncResult);
		return (ExecutionInfo)array[0];
	}

	public void GetExecutionInfoAsync()
	{
		GetExecutionInfoAsync(null);
	}

	public void GetExecutionInfoAsync(object userState)
	{
		if (GetExecutionInfoOperationCompleted == null)
		{
			GetExecutionInfoOperationCompleted = OnGetExecutionInfoOperationCompleted;
		}
		InvokeAsync("GetExecutionInfo", new object[0], GetExecutionInfoOperationCompleted, userState);
	}

	private void OnGetExecutionInfoOperationCompleted(object arg)
	{
		if (this.GetExecutionInfoCompleted != null)
		{
			InvokeCompletedEventArgs e = (InvokeCompletedEventArgs)arg;
			this.GetExecutionInfoCompleted(this, new GetExecutionInfoCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
		}
	}

	[SoapHeader("TrustedUserHeaderValue")]
	[SoapDocumentMethod("http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices/GetExecutionInfo2", RequestNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", ResponseNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
	[SoapHeader("ServerInfoHeaderValue", Direction = SoapHeaderDirection.Out)]
	[SoapHeader("PrintControlClsidHeaderValue", Direction = SoapHeaderDirection.Out)]
	[SoapHeader("ExecutionHeaderValue")]
	[return: XmlElement("executionInfo")]
	public ExecutionInfo2 GetExecutionInfo2()
	{
		object[] array = Invoke("GetExecutionInfo2", new object[0]);
		return (ExecutionInfo2)array[0];
	}

	public IAsyncResult BeginGetExecutionInfo2(AsyncCallback callback, object asyncState)
	{
		return BeginInvoke("GetExecutionInfo2", new object[0], callback, asyncState);
	}

	public ExecutionInfo2 EndGetExecutionInfo2(IAsyncResult asyncResult)
	{
		object[] array = EndInvoke(asyncResult);
		return (ExecutionInfo2)array[0];
	}

	public void GetExecutionInfo2Async()
	{
		GetExecutionInfo2Async(null);
	}

	public void GetExecutionInfo2Async(object userState)
	{
		if (GetExecutionInfo2OperationCompleted == null)
		{
			GetExecutionInfo2OperationCompleted = OnGetExecutionInfo2OperationCompleted;
		}
		InvokeAsync("GetExecutionInfo2", new object[0], GetExecutionInfo2OperationCompleted, userState);
	}

	private void OnGetExecutionInfo2OperationCompleted(object arg)
	{
		if (this.GetExecutionInfo2Completed != null)
		{
			InvokeCompletedEventArgs e = (InvokeCompletedEventArgs)arg;
			this.GetExecutionInfo2Completed(this, new GetExecutionInfo2CompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
		}
	}

	[SoapHeader("ExecutionHeaderValue")]
	[SoapDocumentMethod("http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices/GetDocumentMap", RequestNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", ResponseNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
	[SoapHeader("TrustedUserHeaderValue")]
	[SoapHeader("ServerInfoHeaderValue", Direction = SoapHeaderDirection.Out)]
	[SoapHeader("PrintControlClsidHeaderValue", Direction = SoapHeaderDirection.Out)]
	[return: XmlElement("result")]
	public DocumentMapNode GetDocumentMap()
	{
		object[] array = Invoke("GetDocumentMap", new object[0]);
		return (DocumentMapNode)array[0];
	}

	public IAsyncResult BeginGetDocumentMap(AsyncCallback callback, object asyncState)
	{
		return BeginInvoke("GetDocumentMap", new object[0], callback, asyncState);
	}

	public DocumentMapNode EndGetDocumentMap(IAsyncResult asyncResult)
	{
		object[] array = EndInvoke(asyncResult);
		return (DocumentMapNode)array[0];
	}

	public void GetDocumentMapAsync()
	{
		GetDocumentMapAsync(null);
	}

	public void GetDocumentMapAsync(object userState)
	{
		if (GetDocumentMapOperationCompleted == null)
		{
			GetDocumentMapOperationCompleted = OnGetDocumentMapOperationCompleted;
		}
		InvokeAsync("GetDocumentMap", new object[0], GetDocumentMapOperationCompleted, userState);
	}

	private void OnGetDocumentMapOperationCompleted(object arg)
	{
		if (this.GetDocumentMapCompleted != null)
		{
			InvokeCompletedEventArgs e = (InvokeCompletedEventArgs)arg;
			this.GetDocumentMapCompleted(this, new GetDocumentMapCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
		}
	}

	[SoapHeader("PrintControlClsidHeaderValue", Direction = SoapHeaderDirection.Out)]
	[SoapHeader("ExecutionHeaderValue", Direction = SoapHeaderDirection.InOut)]
	[SoapDocumentMethod("http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices/LoadDrillthroughTarget", RequestNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", ResponseNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
	[SoapHeader("ServerInfoHeaderValue", Direction = SoapHeaderDirection.Out)]
	[SoapHeader("TrustedUserHeaderValue")]
	[return: XmlElement("ExecutionInfo")]
	public ExecutionInfo LoadDrillthroughTarget(string DrillthroughID)
	{
		object[] array = Invoke("LoadDrillthroughTarget", new object[1] { DrillthroughID });
		return (ExecutionInfo)array[0];
	}

	public IAsyncResult BeginLoadDrillthroughTarget(string DrillthroughID, AsyncCallback callback, object asyncState)
	{
		return BeginInvoke("LoadDrillthroughTarget", new object[1] { DrillthroughID }, callback, asyncState);
	}

	public ExecutionInfo EndLoadDrillthroughTarget(IAsyncResult asyncResult)
	{
		object[] array = EndInvoke(asyncResult);
		return (ExecutionInfo)array[0];
	}

	public void LoadDrillthroughTargetAsync(string DrillthroughID)
	{
		LoadDrillthroughTargetAsync(DrillthroughID, null);
	}

	public void LoadDrillthroughTargetAsync(string DrillthroughID, object userState)
	{
		if (LoadDrillthroughTargetOperationCompleted == null)
		{
			LoadDrillthroughTargetOperationCompleted = OnLoadDrillthroughTargetOperationCompleted;
		}
		InvokeAsync("LoadDrillthroughTarget", new object[1] { DrillthroughID }, LoadDrillthroughTargetOperationCompleted, userState);
	}

	private void OnLoadDrillthroughTargetOperationCompleted(object arg)
	{
		if (this.LoadDrillthroughTargetCompleted != null)
		{
			InvokeCompletedEventArgs e = (InvokeCompletedEventArgs)arg;
			this.LoadDrillthroughTargetCompleted(this, new LoadDrillthroughTargetCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
		}
	}

	[SoapHeader("ExecutionHeaderValue", Direction = SoapHeaderDirection.InOut)]
	[SoapHeader("PrintControlClsidHeaderValue", Direction = SoapHeaderDirection.Out)]
	[SoapDocumentMethod("http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices/LoadDrillthroughTarget2", RequestNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", ResponseNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
	[SoapHeader("TrustedUserHeaderValue")]
	[SoapHeader("ServerInfoHeaderValue", Direction = SoapHeaderDirection.Out)]
	[return: XmlElement("ExecutionInfo")]
	public ExecutionInfo2 LoadDrillthroughTarget2(string DrillthroughID)
	{
		object[] array = Invoke("LoadDrillthroughTarget2", new object[1] { DrillthroughID });
		return (ExecutionInfo2)array[0];
	}

	public IAsyncResult BeginLoadDrillthroughTarget2(string DrillthroughID, AsyncCallback callback, object asyncState)
	{
		return BeginInvoke("LoadDrillthroughTarget2", new object[1] { DrillthroughID }, callback, asyncState);
	}

	public ExecutionInfo2 EndLoadDrillthroughTarget2(IAsyncResult asyncResult)
	{
		object[] array = EndInvoke(asyncResult);
		return (ExecutionInfo2)array[0];
	}

	public void LoadDrillthroughTarget2Async(string DrillthroughID)
	{
		LoadDrillthroughTarget2Async(DrillthroughID, null);
	}

	public void LoadDrillthroughTarget2Async(string DrillthroughID, object userState)
	{
		if (LoadDrillthroughTarget2OperationCompleted == null)
		{
			LoadDrillthroughTarget2OperationCompleted = OnLoadDrillthroughTarget2OperationCompleted;
		}
		InvokeAsync("LoadDrillthroughTarget2", new object[1] { DrillthroughID }, LoadDrillthroughTarget2OperationCompleted, userState);
	}

	private void OnLoadDrillthroughTarget2OperationCompleted(object arg)
	{
		if (this.LoadDrillthroughTarget2Completed != null)
		{
			InvokeCompletedEventArgs e = (InvokeCompletedEventArgs)arg;
			this.LoadDrillthroughTarget2Completed(this, new LoadDrillthroughTarget2CompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
		}
	}

	[SoapDocumentMethod("http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices/ToggleItem", RequestNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", ResponseNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
	[SoapHeader("ServerInfoHeaderValue", Direction = SoapHeaderDirection.Out)]
	[SoapHeader("PrintControlClsidHeaderValue", Direction = SoapHeaderDirection.Out)]
	[SoapHeader("ExecutionHeaderValue")]
	[SoapHeader("TrustedUserHeaderValue")]
	[return: XmlElement("Found")]
	public bool ToggleItem(string ToggleID)
	{
		object[] array = Invoke("ToggleItem", new object[1] { ToggleID });
		return (bool)array[0];
	}

	public IAsyncResult BeginToggleItem(string ToggleID, AsyncCallback callback, object asyncState)
	{
		return BeginInvoke("ToggleItem", new object[1] { ToggleID }, callback, asyncState);
	}

	public bool EndToggleItem(IAsyncResult asyncResult)
	{
		object[] array = EndInvoke(asyncResult);
		return (bool)array[0];
	}

	public void ToggleItemAsync(string ToggleID)
	{
		ToggleItemAsync(ToggleID, null);
	}

	public void ToggleItemAsync(string ToggleID, object userState)
	{
		if (ToggleItemOperationCompleted == null)
		{
			ToggleItemOperationCompleted = OnToggleItemOperationCompleted;
		}
		InvokeAsync("ToggleItem", new object[1] { ToggleID }, ToggleItemOperationCompleted, userState);
	}

	private void OnToggleItemOperationCompleted(object arg)
	{
		if (this.ToggleItemCompleted != null)
		{
			InvokeCompletedEventArgs e = (InvokeCompletedEventArgs)arg;
			this.ToggleItemCompleted(this, new ToggleItemCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
		}
	}

	[SoapHeader("ExecutionHeaderValue")]
	[SoapDocumentMethod("http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices/NavigateDocumentMap", RequestNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", ResponseNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
	[SoapHeader("ServerInfoHeaderValue", Direction = SoapHeaderDirection.Out)]
	[SoapHeader("PrintControlClsidHeaderValue", Direction = SoapHeaderDirection.Out)]
	[SoapHeader("TrustedUserHeaderValue")]
	[return: XmlElement("PageNumber")]
	public int NavigateDocumentMap(string DocMapID)
	{
		object[] array = Invoke("NavigateDocumentMap", new object[1] { DocMapID });
		return (int)array[0];
	}

	public IAsyncResult BeginNavigateDocumentMap(string DocMapID, AsyncCallback callback, object asyncState)
	{
		return BeginInvoke("NavigateDocumentMap", new object[1] { DocMapID }, callback, asyncState);
	}

	public int EndNavigateDocumentMap(IAsyncResult asyncResult)
	{
		object[] array = EndInvoke(asyncResult);
		return (int)array[0];
	}

	public void NavigateDocumentMapAsync(string DocMapID)
	{
		NavigateDocumentMapAsync(DocMapID, null);
	}

	public void NavigateDocumentMapAsync(string DocMapID, object userState)
	{
		if (NavigateDocumentMapOperationCompleted == null)
		{
			NavigateDocumentMapOperationCompleted = OnNavigateDocumentMapOperationCompleted;
		}
		InvokeAsync("NavigateDocumentMap", new object[1] { DocMapID }, NavigateDocumentMapOperationCompleted, userState);
	}

	private void OnNavigateDocumentMapOperationCompleted(object arg)
	{
		if (this.NavigateDocumentMapCompleted != null)
		{
			InvokeCompletedEventArgs e = (InvokeCompletedEventArgs)arg;
			this.NavigateDocumentMapCompleted(this, new NavigateDocumentMapCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
		}
	}

	[SoapDocumentMethod("http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices/NavigateBookmark", RequestNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", ResponseNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
	[SoapHeader("ExecutionHeaderValue")]
	[SoapHeader("TrustedUserHeaderValue")]
	[SoapHeader("ServerInfoHeaderValue", Direction = SoapHeaderDirection.Out)]
	[SoapHeader("PrintControlClsidHeaderValue", Direction = SoapHeaderDirection.Out)]
	[return: XmlElement("PageNumber")]
	public int NavigateBookmark(string BookmarkID, out string UniqueName)
	{
		object[] array = Invoke("NavigateBookmark", new object[1] { BookmarkID });
		UniqueName = (string)array[1];
		return (int)array[0];
	}

	public IAsyncResult BeginNavigateBookmark(string BookmarkID, AsyncCallback callback, object asyncState)
	{
		return BeginInvoke("NavigateBookmark", new object[1] { BookmarkID }, callback, asyncState);
	}

	public int EndNavigateBookmark(IAsyncResult asyncResult, out string UniqueName)
	{
		object[] array = EndInvoke(asyncResult);
		UniqueName = (string)array[1];
		return (int)array[0];
	}

	public void NavigateBookmarkAsync(string BookmarkID)
	{
		NavigateBookmarkAsync(BookmarkID, null);
	}

	public void NavigateBookmarkAsync(string BookmarkID, object userState)
	{
		if (NavigateBookmarkOperationCompleted == null)
		{
			NavigateBookmarkOperationCompleted = OnNavigateBookmarkOperationCompleted;
		}
		InvokeAsync("NavigateBookmark", new object[1] { BookmarkID }, NavigateBookmarkOperationCompleted, userState);
	}

	private void OnNavigateBookmarkOperationCompleted(object arg)
	{
		if (this.NavigateBookmarkCompleted != null)
		{
			InvokeCompletedEventArgs e = (InvokeCompletedEventArgs)arg;
			this.NavigateBookmarkCompleted(this, new NavigateBookmarkCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
		}
	}

	[SoapHeader("ExecutionHeaderValue")]
	[SoapDocumentMethod("http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices/FindString", RequestNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", ResponseNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
	[SoapHeader("ServerInfoHeaderValue", Direction = SoapHeaderDirection.Out)]
	[SoapHeader("PrintControlClsidHeaderValue", Direction = SoapHeaderDirection.Out)]
	[SoapHeader("TrustedUserHeaderValue")]
	[return: XmlElement("PageNumber")]
	public int FindString(int StartPage, int EndPage, string FindValue)
	{
		object[] array = Invoke("FindString", new object[3] { StartPage, EndPage, FindValue });
		return (int)array[0];
	}

	public IAsyncResult BeginFindString(int StartPage, int EndPage, string FindValue, AsyncCallback callback, object asyncState)
	{
		return BeginInvoke("FindString", new object[3] { StartPage, EndPage, FindValue }, callback, asyncState);
	}

	public int EndFindString(IAsyncResult asyncResult)
	{
		object[] array = EndInvoke(asyncResult);
		return (int)array[0];
	}

	public void FindStringAsync(int StartPage, int EndPage, string FindValue)
	{
		FindStringAsync(StartPage, EndPage, FindValue, null);
	}

	public void FindStringAsync(int StartPage, int EndPage, string FindValue, object userState)
	{
		if (FindStringOperationCompleted == null)
		{
			FindStringOperationCompleted = OnFindStringOperationCompleted;
		}
		InvokeAsync("FindString", new object[3] { StartPage, EndPage, FindValue }, FindStringOperationCompleted, userState);
	}

	private void OnFindStringOperationCompleted(object arg)
	{
		if (this.FindStringCompleted != null)
		{
			InvokeCompletedEventArgs e = (InvokeCompletedEventArgs)arg;
			this.FindStringCompleted(this, new FindStringCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
		}
	}

	[SoapDocumentMethod("http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices/Sort", RequestNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", ResponseNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
	[SoapHeader("TrustedUserHeaderValue")]
	[SoapHeader("ServerInfoHeaderValue", Direction = SoapHeaderDirection.Out)]
	[SoapHeader("PrintControlClsidHeaderValue", Direction = SoapHeaderDirection.Out)]
	[SoapHeader("ExecutionHeaderValue")]
	[return: XmlElement("PageNumber")]
	public int Sort(string SortItem, SortDirectionEnum Direction, bool Clear, out string ReportItem, out int NumPages)
	{
		object[] array = Invoke("Sort", new object[3] { SortItem, Direction, Clear });
		ReportItem = (string)array[1];
		NumPages = (int)array[2];
		return (int)array[0];
	}

	public IAsyncResult BeginSort(string SortItem, SortDirectionEnum Direction, bool Clear, AsyncCallback callback, object asyncState)
	{
		return BeginInvoke("Sort", new object[3] { SortItem, Direction, Clear }, callback, asyncState);
	}

	public int EndSort(IAsyncResult asyncResult, out string ReportItem, out int NumPages)
	{
		object[] array = EndInvoke(asyncResult);
		ReportItem = (string)array[1];
		NumPages = (int)array[2];
		return (int)array[0];
	}

	public void SortAsync(string SortItem, SortDirectionEnum Direction, bool Clear)
	{
		SortAsync(SortItem, Direction, Clear, null);
	}

	public void SortAsync(string SortItem, SortDirectionEnum Direction, bool Clear, object userState)
	{
		if (SortOperationCompleted == null)
		{
			SortOperationCompleted = OnSortOperationCompleted;
		}
		InvokeAsync("Sort", new object[3] { SortItem, Direction, Clear }, SortOperationCompleted, userState);
	}

	private void OnSortOperationCompleted(object arg)
	{
		if (this.SortCompleted != null)
		{
			InvokeCompletedEventArgs e = (InvokeCompletedEventArgs)arg;
			this.SortCompleted(this, new SortCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
		}
	}

	[SoapDocumentMethod("http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices/Sort2", RequestNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", ResponseNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
	[SoapHeader("TrustedUserHeaderValue")]
	[SoapHeader("ExecutionHeaderValue")]
	[SoapHeader("ServerInfoHeaderValue", Direction = SoapHeaderDirection.Out)]
	[SoapHeader("PrintControlClsidHeaderValue", Direction = SoapHeaderDirection.Out)]
	[return: XmlElement("PageNumber")]
	public int Sort2(string SortItem, SortDirectionEnum Direction, bool Clear, PageCountMode PaginationMode, out string ReportItem, out ExecutionInfo2 ExecutionInfo)
	{
		object[] array = Invoke("Sort2", new object[4] { SortItem, Direction, Clear, PaginationMode });
		ReportItem = (string)array[1];
		ExecutionInfo = (ExecutionInfo2)array[2];
		return (int)array[0];
	}

	public IAsyncResult BeginSort2(string SortItem, SortDirectionEnum Direction, bool Clear, PageCountMode PaginationMode, AsyncCallback callback, object asyncState)
	{
		return BeginInvoke("Sort2", new object[4] { SortItem, Direction, Clear, PaginationMode }, callback, asyncState);
	}

	public int EndSort2(IAsyncResult asyncResult, out string ReportItem, out ExecutionInfo2 ExecutionInfo)
	{
		object[] array = EndInvoke(asyncResult);
		ReportItem = (string)array[1];
		ExecutionInfo = (ExecutionInfo2)array[2];
		return (int)array[0];
	}

	public void Sort2Async(string SortItem, SortDirectionEnum Direction, bool Clear, PageCountMode PaginationMode)
	{
		Sort2Async(SortItem, Direction, Clear, PaginationMode, null);
	}

	public void Sort2Async(string SortItem, SortDirectionEnum Direction, bool Clear, PageCountMode PaginationMode, object userState)
	{
		if (Sort2OperationCompleted == null)
		{
			Sort2OperationCompleted = OnSort2OperationCompleted;
		}
		InvokeAsync("Sort2", new object[4] { SortItem, Direction, Clear, PaginationMode }, Sort2OperationCompleted, userState);
	}

	private void OnSort2OperationCompleted(object arg)
	{
		if (this.Sort2Completed != null)
		{
			InvokeCompletedEventArgs e = (InvokeCompletedEventArgs)arg;
			this.Sort2Completed(this, new Sort2CompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
		}
	}

	[SoapHeader("TrustedUserHeaderValue")]
	[SoapHeader("ServerInfoHeaderValue", Direction = SoapHeaderDirection.Out)]
	[SoapDocumentMethod("http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices/GetRenderResource", RequestNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", ResponseNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
	[SoapHeader("PrintControlClsidHeaderValue", Direction = SoapHeaderDirection.Out)]
	[return: XmlElement("Result", DataType = "base64Binary")]
	public byte[] GetRenderResource(string Format, string DeviceInfo, out string MimeType)
	{
		object[] array = Invoke("GetRenderResource", new object[2] { Format, DeviceInfo });
		MimeType = (string)array[1];
		return (byte[])array[0];
	}

	public IAsyncResult BeginGetRenderResource(string Format, string DeviceInfo, AsyncCallback callback, object asyncState)
	{
		return BeginInvoke("GetRenderResource", new object[2] { Format, DeviceInfo }, callback, asyncState);
	}

	public byte[] EndGetRenderResource(IAsyncResult asyncResult, out string MimeType)
	{
		object[] array = EndInvoke(asyncResult);
		MimeType = (string)array[1];
		return (byte[])array[0];
	}

	public void GetRenderResourceAsync(string Format, string DeviceInfo)
	{
		GetRenderResourceAsync(Format, DeviceInfo, null);
	}

	public void GetRenderResourceAsync(string Format, string DeviceInfo, object userState)
	{
		if (GetRenderResourceOperationCompleted == null)
		{
			GetRenderResourceOperationCompleted = OnGetRenderResourceOperationCompleted;
		}
		InvokeAsync("GetRenderResource", new object[2] { Format, DeviceInfo }, GetRenderResourceOperationCompleted, userState);
	}

	private void OnGetRenderResourceOperationCompleted(object arg)
	{
		if (this.GetRenderResourceCompleted != null)
		{
			InvokeCompletedEventArgs e = (InvokeCompletedEventArgs)arg;
			this.GetRenderResourceCompleted(this, new GetRenderResourceCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
		}
	}

	[SoapHeader("TrustedUserHeaderValue")]
	[SoapHeader("ServerInfoHeaderValue", Direction = SoapHeaderDirection.Out)]
	[SoapDocumentMethod("http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices/ListRenderingExtensions", RequestNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", ResponseNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
	[SoapHeader("PrintControlClsidHeaderValue", Direction = SoapHeaderDirection.Out)]
	[return: XmlArray("Extensions")]
	public Extension[] ListRenderingExtensions()
	{
		object[] array = Invoke("ListRenderingExtensions", new object[0]);
		return (Extension[])array[0];
	}

	public IAsyncResult BeginListRenderingExtensions(AsyncCallback callback, object asyncState)
	{
		return BeginInvoke("ListRenderingExtensions", new object[0], callback, asyncState);
	}

	public Extension[] EndListRenderingExtensions(IAsyncResult asyncResult)
	{
		object[] array = EndInvoke(asyncResult);
		return (Extension[])array[0];
	}

	public void ListRenderingExtensionsAsync()
	{
		ListRenderingExtensionsAsync(null);
	}

	public void ListRenderingExtensionsAsync(object userState)
	{
		if (ListRenderingExtensionsOperationCompleted == null)
		{
			ListRenderingExtensionsOperationCompleted = OnListRenderingExtensionsOperationCompleted;
		}
		InvokeAsync("ListRenderingExtensions", new object[0], ListRenderingExtensionsOperationCompleted, userState);
	}

	private void OnListRenderingExtensionsOperationCompleted(object arg)
	{
		if (this.ListRenderingExtensionsCompleted != null)
		{
			InvokeCompletedEventArgs e = (InvokeCompletedEventArgs)arg;
			this.ListRenderingExtensionsCompleted(this, new ListRenderingExtensionsCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
		}
	}

	[SoapHeader("PrintControlClsidHeaderValue", Direction = SoapHeaderDirection.Out)]
	[SoapHeader("ServerInfoHeaderValue", Direction = SoapHeaderDirection.Out)]
	[SoapDocumentMethod("http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices/LogonUser", RequestNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", ResponseNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
	public void LogonUser(string userName, string password, string authority)
	{
		Invoke("LogonUser", new object[3] { userName, password, authority });
	}

	public IAsyncResult BeginLogonUser(string userName, string password, string authority, AsyncCallback callback, object asyncState)
	{
		return BeginInvoke("LogonUser", new object[3] { userName, password, authority }, callback, asyncState);
	}

	public void EndLogonUser(IAsyncResult asyncResult)
	{
		EndInvoke(asyncResult);
	}

	public void LogonUserAsync(string userName, string password, string authority)
	{
		LogonUserAsync(userName, password, authority, null);
	}

	public void LogonUserAsync(string userName, string password, string authority, object userState)
	{
		if (LogonUserOperationCompleted == null)
		{
			LogonUserOperationCompleted = OnLogonUserOperationCompleted;
		}
		InvokeAsync("LogonUser", new object[3] { userName, password, authority }, LogonUserOperationCompleted, userState);
	}

	private void OnLogonUserOperationCompleted(object arg)
	{
		if (this.LogonUserCompleted != null)
		{
			InvokeCompletedEventArgs e = (InvokeCompletedEventArgs)arg;
			this.LogonUserCompleted(this, new AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
		}
	}

	[SoapHeader("PrintControlClsidHeaderValue", Direction = SoapHeaderDirection.Out)]
	[SoapDocumentMethod("http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices/Logoff", RequestNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", ResponseNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
	[SoapHeader("ServerInfoHeaderValue", Direction = SoapHeaderDirection.Out)]
	public void Logoff()
	{
		Invoke("Logoff", new object[0]);
	}

	public IAsyncResult BeginLogoff(AsyncCallback callback, object asyncState)
	{
		return BeginInvoke("Logoff", new object[0], callback, asyncState);
	}

	public void EndLogoff(IAsyncResult asyncResult)
	{
		EndInvoke(asyncResult);
	}

	public void LogoffAsync()
	{
		LogoffAsync(null);
	}

	public void LogoffAsync(object userState)
	{
		if (LogoffOperationCompleted == null)
		{
			LogoffOperationCompleted = OnLogoffOperationCompleted;
		}
		InvokeAsync("Logoff", new object[0], LogoffOperationCompleted, userState);
	}

	private void OnLogoffOperationCompleted(object arg)
	{
		if (this.LogoffCompleted != null)
		{
			InvokeCompletedEventArgs e = (InvokeCompletedEventArgs)arg;
			this.LogoffCompleted(this, new AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
		}
	}

	public new void CancelAsync(object userState)
	{
		base.CancelAsync(userState);
	}
}
