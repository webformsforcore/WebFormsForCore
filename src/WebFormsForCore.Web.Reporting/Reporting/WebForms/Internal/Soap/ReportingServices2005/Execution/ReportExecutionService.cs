
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Web.Services;
using System.Web.Services.Description;
using System.Web.Services.Protocols;
using System.Xml.Serialization;

#nullable disable
namespace Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution
{
  [GeneratedCode("wsdl", "2.0.50727.42")]
  [DebuggerStepThrough]
  [XmlInclude(typeof (ParameterValueOrFieldReference))]
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

    public ReportExecutionService()
    {
      this.Url = "http://localhost/ReportServer/ReportExecution2005.asmx";
    }

    public TrustedUserHeader TrustedUserHeaderValue
    {
      get => this.trustedUserHeaderValueField;
      set => this.trustedUserHeaderValueField = value;
    }

    public PrintControlClsidHeader PrintControlClsidHeaderValue
    {
      get => this.printControlClsidHeaderValueField;
      set => this.printControlClsidHeaderValueField = value;
    }

    public ServerInfoHeader ServerInfoHeaderValue
    {
      get => this.serverInfoHeaderValueField;
      set => this.serverInfoHeaderValueField = value;
    }

    public ExecutionHeader ExecutionHeaderValue
    {
      get => this.executionHeaderValueField;
      set => this.executionHeaderValueField = value;
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

    [SoapHeader("TrustedUserHeaderValue")]
    [SoapHeader("PrintControlClsidHeaderValue", Direction = SoapHeaderDirection.Out)]
    [SoapHeader("ServerInfoHeaderValue", Direction = SoapHeaderDirection.Out)]
    [SoapDocumentMethod("http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices/ListSecureMethods", RequestNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", ResponseNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
    public string[] ListSecureMethods()
    {
      return (string[]) this.Invoke(nameof (ListSecureMethods), new object[0])[0];
    }

    protected new object[] Invoke(string methodName, object[] parameters)
    {
      return base.Invoke(methodName, parameters);
    }

    public IAsyncResult BeginListSecureMethods(AsyncCallback callback, object asyncState)
    {
      return this.BeginInvoke("ListSecureMethods", new object[0], callback, asyncState);
    }

    public string[] EndListSecureMethods(IAsyncResult asyncResult)
    {
      return (string[]) this.EndInvoke(asyncResult)[0];
    }

    public void ListSecureMethodsAsync() => this.ListSecureMethodsAsync((object) null);

    public void ListSecureMethodsAsync(object userState)
    {
      if (this.ListSecureMethodsOperationCompleted == null)
        this.ListSecureMethodsOperationCompleted = new SendOrPostCallback(this.OnListSecureMethodsOperationCompleted);
      this.InvokeAsync("ListSecureMethods", new object[0], this.ListSecureMethodsOperationCompleted, userState);
    }

    private void OnListSecureMethodsOperationCompleted(object arg)
    {
      if (this.ListSecureMethodsCompleted == null)
        return;
      InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs) arg;
      this.ListSecureMethodsCompleted((object) this, new ListSecureMethodsCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
    }

    [SoapDocumentMethod("http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices/LoadReport", RequestNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", ResponseNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
    [SoapHeader("ExecutionHeaderValue", Direction = SoapHeaderDirection.Out)]
    [SoapHeader("TrustedUserHeaderValue")]
    [SoapHeader("ServerInfoHeaderValue", Direction = SoapHeaderDirection.Out)]
    [SoapHeader("PrintControlClsidHeaderValue", Direction = SoapHeaderDirection.Out)]
    [return: XmlElement("executionInfo")]
    public ExecutionInfo LoadReport(string Report, string HistoryID)
    {
      return (ExecutionInfo) this.Invoke(nameof (LoadReport), new object[2]
      {
        (object) Report,
        (object) HistoryID
      })[0];
    }

    public IAsyncResult BeginLoadReport(
      string Report,
      string HistoryID,
      AsyncCallback callback,
      object asyncState)
    {
      return this.BeginInvoke("LoadReport", new object[2]
      {
        (object) Report,
        (object) HistoryID
      }, callback, asyncState);
    }

    public ExecutionInfo EndLoadReport(IAsyncResult asyncResult)
    {
      return (ExecutionInfo) this.EndInvoke(asyncResult)[0];
    }

    public void LoadReportAsync(string Report, string HistoryID)
    {
      this.LoadReportAsync(Report, HistoryID, (object) null);
    }

    public void LoadReportAsync(string Report, string HistoryID, object userState)
    {
      if (this.LoadReportOperationCompleted == null)
        this.LoadReportOperationCompleted = new SendOrPostCallback(this.OnLoadReportOperationCompleted);
      this.InvokeAsync("LoadReport", new object[2]
      {
        (object) Report,
        (object) HistoryID
      }, this.LoadReportOperationCompleted, userState);
    }

    private void OnLoadReportOperationCompleted(object arg)
    {
      if (this.LoadReportCompleted == null)
        return;
      InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs) arg;
      this.LoadReportCompleted((object) this, new LoadReportCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
    }

    [SoapHeader("PrintControlClsidHeaderValue", Direction = SoapHeaderDirection.Out)]
    [SoapHeader("ExecutionHeaderValue", Direction = SoapHeaderDirection.Out)]
    [SoapHeader("TrustedUserHeaderValue")]
    [SoapDocumentMethod("http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices/LoadReport2", RequestNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", ResponseNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
    [SoapHeader("ServerInfoHeaderValue", Direction = SoapHeaderDirection.Out)]
    [return: XmlElement("executionInfo")]
    public ExecutionInfo2 LoadReport2(string Report, string HistoryID)
    {
      return (ExecutionInfo2) this.Invoke(nameof (LoadReport2), new object[2]
      {
        (object) Report,
        (object) HistoryID
      })[0];
    }

    public IAsyncResult BeginLoadReport2(
      string Report,
      string HistoryID,
      AsyncCallback callback,
      object asyncState)
    {
      return this.BeginInvoke("LoadReport2", new object[2]
      {
        (object) Report,
        (object) HistoryID
      }, callback, asyncState);
    }

    public ExecutionInfo2 EndLoadReport2(IAsyncResult asyncResult)
    {
      return (ExecutionInfo2) this.EndInvoke(asyncResult)[0];
    }

    public void LoadReport2Async(string Report, string HistoryID)
    {
      this.LoadReport2Async(Report, HistoryID, (object) null);
    }

    public void LoadReport2Async(string Report, string HistoryID, object userState)
    {
      if (this.LoadReport2OperationCompleted == null)
        this.LoadReport2OperationCompleted = new SendOrPostCallback(this.OnLoadReport2OperationCompleted);
      this.InvokeAsync("LoadReport2", new object[2]
      {
        (object) Report,
        (object) HistoryID
      }, this.LoadReport2OperationCompleted, userState);
    }

    private void OnLoadReport2OperationCompleted(object arg)
    {
      if (this.LoadReport2Completed == null)
        return;
      InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs) arg;
      this.LoadReport2Completed((object) this, new LoadReport2CompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
    }

    [SoapHeader("PrintControlClsidHeaderValue", Direction = SoapHeaderDirection.Out)]
    [SoapHeader("ServerInfoHeaderValue", Direction = SoapHeaderDirection.Out)]
    [SoapHeader("ExecutionHeaderValue", Direction = SoapHeaderDirection.Out)]
    [SoapHeader("TrustedUserHeaderValue")]
    [SoapDocumentMethod("http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices/LoadReportDefinition", RequestNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", ResponseNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
    [return: XmlElement("executionInfo")]
    public ExecutionInfo LoadReportDefinition([XmlElement(DataType = "base64Binary")] byte[] Definition, out Warning[] warnings)
    {
      object[] objArray = this.Invoke(nameof (LoadReportDefinition), new object[1]
      {
        (object) Definition
      });
      warnings = (Warning[]) objArray[1];
      return (ExecutionInfo) objArray[0];
    }

    public IAsyncResult BeginLoadReportDefinition(
      byte[] Definition,
      AsyncCallback callback,
      object asyncState)
    {
      return this.BeginInvoke("LoadReportDefinition", new object[1]
      {
        (object) Definition
      }, callback, asyncState);
    }

    public ExecutionInfo EndLoadReportDefinition(IAsyncResult asyncResult, out Warning[] warnings)
    {
      object[] objArray = this.EndInvoke(asyncResult);
      warnings = (Warning[]) objArray[1];
      return (ExecutionInfo) objArray[0];
    }

    public void LoadReportDefinitionAsync(byte[] Definition)
    {
      this.LoadReportDefinitionAsync(Definition, (object) null);
    }

    public void LoadReportDefinitionAsync(byte[] Definition, object userState)
    {
      if (this.LoadReportDefinitionOperationCompleted == null)
        this.LoadReportDefinitionOperationCompleted = new SendOrPostCallback(this.OnLoadReportDefinitionOperationCompleted);
      this.InvokeAsync("LoadReportDefinition", new object[1]
      {
        (object) Definition
      }, this.LoadReportDefinitionOperationCompleted, userState);
    }

    private void OnLoadReportDefinitionOperationCompleted(object arg)
    {
      if (this.LoadReportDefinitionCompleted == null)
        return;
      InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs) arg;
      this.LoadReportDefinitionCompleted((object) this, new LoadReportDefinitionCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
    }

    [SoapHeader("ServerInfoHeaderValue", Direction = SoapHeaderDirection.Out)]
    [SoapHeader("PrintControlClsidHeaderValue", Direction = SoapHeaderDirection.Out)]
    [SoapHeader("ExecutionHeaderValue", Direction = SoapHeaderDirection.Out)]
    [SoapHeader("TrustedUserHeaderValue")]
    [SoapDocumentMethod("http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices/LoadReportDefinition2", RequestNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", ResponseNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
    [return: XmlElement("executionInfo")]
    public ExecutionInfo2 LoadReportDefinition2([XmlElement(DataType = "base64Binary")] byte[] Definition, out Warning[] warnings)
    {
      object[] objArray = this.Invoke(nameof (LoadReportDefinition2), new object[1]
      {
        (object) Definition
      });
      warnings = (Warning[]) objArray[1];
      return (ExecutionInfo2) objArray[0];
    }

    public IAsyncResult BeginLoadReportDefinition2(
      byte[] Definition,
      AsyncCallback callback,
      object asyncState)
    {
      return this.BeginInvoke("LoadReportDefinition2", new object[1]
      {
        (object) Definition
      }, callback, asyncState);
    }

    public ExecutionInfo2 EndLoadReportDefinition2(IAsyncResult asyncResult, out Warning[] warnings)
    {
      object[] objArray = this.EndInvoke(asyncResult);
      warnings = (Warning[]) objArray[1];
      return (ExecutionInfo2) objArray[0];
    }

    public void LoadReportDefinition2Async(byte[] Definition)
    {
      this.LoadReportDefinition2Async(Definition, (object) null);
    }

    public void LoadReportDefinition2Async(byte[] Definition, object userState)
    {
      if (this.LoadReportDefinition2OperationCompleted == null)
        this.LoadReportDefinition2OperationCompleted = new SendOrPostCallback(this.OnLoadReportDefinition2OperationCompleted);
      this.InvokeAsync("LoadReportDefinition2", new object[1]
      {
        (object) Definition
      }, this.LoadReportDefinition2OperationCompleted, userState);
    }

    private void OnLoadReportDefinition2OperationCompleted(object arg)
    {
      if (this.LoadReportDefinition2Completed == null)
        return;
      InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs) arg;
      this.LoadReportDefinition2Completed((object) this, new LoadReportDefinition2CompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
    }

    [SoapHeader("TrustedUserHeaderValue")]
    [SoapHeader("ExecutionHeaderValue")]
    [SoapDocumentMethod("http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices/SetExecutionCredentials", RequestNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", ResponseNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
    [SoapHeader("ServerInfoHeaderValue", Direction = SoapHeaderDirection.Out)]
    [SoapHeader("PrintControlClsidHeaderValue", Direction = SoapHeaderDirection.Out)]
    [return: XmlElement("executionInfo")]
    public ExecutionInfo SetExecutionCredentials(DataSourceCredentials[] Credentials)
    {
      return (ExecutionInfo) this.Invoke(nameof (SetExecutionCredentials), new object[1]
      {
        (object) Credentials
      })[0];
    }

    public IAsyncResult BeginSetExecutionCredentials(
      DataSourceCredentials[] Credentials,
      AsyncCallback callback,
      object asyncState)
    {
      return this.BeginInvoke("SetExecutionCredentials", new object[1]
      {
        (object) Credentials
      }, callback, asyncState);
    }

    public ExecutionInfo EndSetExecutionCredentials(IAsyncResult asyncResult)
    {
      return (ExecutionInfo) this.EndInvoke(asyncResult)[0];
    }

    public void SetExecutionCredentialsAsync(DataSourceCredentials[] Credentials)
    {
      this.SetExecutionCredentialsAsync(Credentials, (object) null);
    }

    public void SetExecutionCredentialsAsync(DataSourceCredentials[] Credentials, object userState)
    {
      if (this.SetExecutionCredentialsOperationCompleted == null)
        this.SetExecutionCredentialsOperationCompleted = new SendOrPostCallback(this.OnSetExecutionCredentialsOperationCompleted);
      this.InvokeAsync("SetExecutionCredentials", new object[1]
      {
        (object) Credentials
      }, this.SetExecutionCredentialsOperationCompleted, userState);
    }

    private void OnSetExecutionCredentialsOperationCompleted(object arg)
    {
      if (this.SetExecutionCredentialsCompleted == null)
        return;
      InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs) arg;
      this.SetExecutionCredentialsCompleted((object) this, new SetExecutionCredentialsCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
    }

    [SoapDocumentMethod("http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices/SetExecutionCredentials2", RequestNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", ResponseNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
    [SoapHeader("ExecutionHeaderValue")]
    [SoapHeader("TrustedUserHeaderValue")]
    [SoapHeader("ServerInfoHeaderValue", Direction = SoapHeaderDirection.Out)]
    [SoapHeader("PrintControlClsidHeaderValue", Direction = SoapHeaderDirection.Out)]
    [return: XmlElement("executionInfo")]
    public ExecutionInfo2 SetExecutionCredentials2(DataSourceCredentials[] Credentials)
    {
      return (ExecutionInfo2) this.Invoke(nameof (SetExecutionCredentials2), new object[1]
      {
        (object) Credentials
      })[0];
    }

    public IAsyncResult BeginSetExecutionCredentials2(
      DataSourceCredentials[] Credentials,
      AsyncCallback callback,
      object asyncState)
    {
      return this.BeginInvoke("SetExecutionCredentials2", new object[1]
      {
        (object) Credentials
      }, callback, asyncState);
    }

    public ExecutionInfo2 EndSetExecutionCredentials2(IAsyncResult asyncResult)
    {
      return (ExecutionInfo2) this.EndInvoke(asyncResult)[0];
    }

    public void SetExecutionCredentials2Async(DataSourceCredentials[] Credentials)
    {
      this.SetExecutionCredentials2Async(Credentials, (object) null);
    }

    public void SetExecutionCredentials2Async(DataSourceCredentials[] Credentials, object userState)
    {
      if (this.SetExecutionCredentials2OperationCompleted == null)
        this.SetExecutionCredentials2OperationCompleted = new SendOrPostCallback(this.OnSetExecutionCredentials2OperationCompleted);
      this.InvokeAsync("SetExecutionCredentials2", new object[1]
      {
        (object) Credentials
      }, this.SetExecutionCredentials2OperationCompleted, userState);
    }

    private void OnSetExecutionCredentials2OperationCompleted(object arg)
    {
      if (this.SetExecutionCredentials2Completed == null)
        return;
      InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs) arg;
      this.SetExecutionCredentials2Completed((object) this, new SetExecutionCredentials2CompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
    }

    [SoapHeader("TrustedUserHeaderValue")]
    [SoapHeader("ExecutionHeaderValue")]
    [SoapHeader("ServerInfoHeaderValue", Direction = SoapHeaderDirection.Out)]
    [SoapHeader("PrintControlClsidHeaderValue", Direction = SoapHeaderDirection.Out)]
    [SoapDocumentMethod("http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices/SetExecutionParameters", RequestNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", ResponseNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
    [return: XmlElement("executionInfo")]
    public ExecutionInfo SetExecutionParameters(
      ParameterValue[] Parameters,
      string ParameterLanguage)
    {
      return (ExecutionInfo) this.Invoke(nameof (SetExecutionParameters), new object[2]
      {
        (object) Parameters,
        (object) ParameterLanguage
      })[0];
    }

    public IAsyncResult BeginSetExecutionParameters(
      ParameterValue[] Parameters,
      string ParameterLanguage,
      AsyncCallback callback,
      object asyncState)
    {
      return this.BeginInvoke("SetExecutionParameters", new object[2]
      {
        (object) Parameters,
        (object) ParameterLanguage
      }, callback, asyncState);
    }

    public ExecutionInfo EndSetExecutionParameters(IAsyncResult asyncResult)
    {
      return (ExecutionInfo) this.EndInvoke(asyncResult)[0];
    }

    public void SetExecutionParametersAsync(ParameterValue[] Parameters, string ParameterLanguage)
    {
      this.SetExecutionParametersAsync(Parameters, ParameterLanguage, (object) null);
    }

    public void SetExecutionParametersAsync(
      ParameterValue[] Parameters,
      string ParameterLanguage,
      object userState)
    {
      if (this.SetExecutionParametersOperationCompleted == null)
        this.SetExecutionParametersOperationCompleted = new SendOrPostCallback(this.OnSetExecutionParametersOperationCompleted);
      this.InvokeAsync("SetExecutionParameters", new object[2]
      {
        (object) Parameters,
        (object) ParameterLanguage
      }, this.SetExecutionParametersOperationCompleted, userState);
    }

    private void OnSetExecutionParametersOperationCompleted(object arg)
    {
      if (this.SetExecutionParametersCompleted == null)
        return;
      InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs) arg;
      this.SetExecutionParametersCompleted((object) this, new SetExecutionParametersCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
    }

    [SoapHeader("TrustedUserHeaderValue")]
    [SoapHeader("PrintControlClsidHeaderValue", Direction = SoapHeaderDirection.Out)]
    [SoapHeader("ServerInfoHeaderValue", Direction = SoapHeaderDirection.Out)]
    [SoapHeader("ExecutionHeaderValue")]
    [SoapDocumentMethod("http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices/SetExecutionParameters2", RequestNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", ResponseNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
    [return: XmlElement("executionInfo")]
    public ExecutionInfo2 SetExecutionParameters2(
      ParameterValue[] Parameters,
      string ParameterLanguage)
    {
      return (ExecutionInfo2) this.Invoke(nameof (SetExecutionParameters2), new object[2]
      {
        (object) Parameters,
        (object) ParameterLanguage
      })[0];
    }

    public IAsyncResult BeginSetExecutionParameters2(
      ParameterValue[] Parameters,
      string ParameterLanguage,
      AsyncCallback callback,
      object asyncState)
    {
      return this.BeginInvoke("SetExecutionParameters2", new object[2]
      {
        (object) Parameters,
        (object) ParameterLanguage
      }, callback, asyncState);
    }

    public ExecutionInfo2 EndSetExecutionParameters2(IAsyncResult asyncResult)
    {
      return (ExecutionInfo2) this.EndInvoke(asyncResult)[0];
    }

    public void SetExecutionParameters2Async(ParameterValue[] Parameters, string ParameterLanguage)
    {
      this.SetExecutionParameters2Async(Parameters, ParameterLanguage, (object) null);
    }

    public void SetExecutionParameters2Async(
      ParameterValue[] Parameters,
      string ParameterLanguage,
      object userState)
    {
      if (this.SetExecutionParameters2OperationCompleted == null)
        this.SetExecutionParameters2OperationCompleted = new SendOrPostCallback(this.OnSetExecutionParameters2OperationCompleted);
      this.InvokeAsync("SetExecutionParameters2", new object[2]
      {
        (object) Parameters,
        (object) ParameterLanguage
      }, this.SetExecutionParameters2OperationCompleted, userState);
    }

    private void OnSetExecutionParameters2OperationCompleted(object arg)
    {
      if (this.SetExecutionParameters2Completed == null)
        return;
      InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs) arg;
      this.SetExecutionParameters2Completed((object) this, new SetExecutionParameters2CompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
    }

    [SoapHeader("PrintControlClsidHeaderValue", Direction = SoapHeaderDirection.Out)]
    [SoapDocumentMethod("http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices/ResetExecution", RequestNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", ResponseNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
    [SoapHeader("ExecutionHeaderValue")]
    [SoapHeader("ServerInfoHeaderValue", Direction = SoapHeaderDirection.Out)]
    [SoapHeader("TrustedUserHeaderValue")]
    [return: XmlElement("executionInfo")]
    public ExecutionInfo ResetExecution()
    {
      return (ExecutionInfo) this.Invoke(nameof (ResetExecution), new object[0])[0];
    }

    public IAsyncResult BeginResetExecution(AsyncCallback callback, object asyncState)
    {
      return this.BeginInvoke("ResetExecution", new object[0], callback, asyncState);
    }

    public ExecutionInfo EndResetExecution(IAsyncResult asyncResult)
    {
      return (ExecutionInfo) this.EndInvoke(asyncResult)[0];
    }

    public void ResetExecutionAsync() => this.ResetExecutionAsync((object) null);

    public void ResetExecutionAsync(object userState)
    {
      if (this.ResetExecutionOperationCompleted == null)
        this.ResetExecutionOperationCompleted = new SendOrPostCallback(this.OnResetExecutionOperationCompleted);
      this.InvokeAsync("ResetExecution", new object[0], this.ResetExecutionOperationCompleted, userState);
    }

    private void OnResetExecutionOperationCompleted(object arg)
    {
      if (this.ResetExecutionCompleted == null)
        return;
      InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs) arg;
      this.ResetExecutionCompleted((object) this, new ResetExecutionCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
    }

    [SoapHeader("PrintControlClsidHeaderValue", Direction = SoapHeaderDirection.Out)]
    [SoapHeader("ExecutionHeaderValue")]
    [SoapDocumentMethod("http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices/ResetExecution2", RequestNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", ResponseNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
    [SoapHeader("TrustedUserHeaderValue")]
    [SoapHeader("ServerInfoHeaderValue", Direction = SoapHeaderDirection.Out)]
    [return: XmlElement("executionInfo")]
    public ExecutionInfo2 ResetExecution2()
    {
      return (ExecutionInfo2) this.Invoke(nameof (ResetExecution2), new object[0])[0];
    }

    public IAsyncResult BeginResetExecution2(AsyncCallback callback, object asyncState)
    {
      return this.BeginInvoke("ResetExecution2", new object[0], callback, asyncState);
    }

    public ExecutionInfo2 EndResetExecution2(IAsyncResult asyncResult)
    {
      return (ExecutionInfo2) this.EndInvoke(asyncResult)[0];
    }

    public void ResetExecution2Async() => this.ResetExecution2Async((object) null);

    public void ResetExecution2Async(object userState)
    {
      if (this.ResetExecution2OperationCompleted == null)
        this.ResetExecution2OperationCompleted = new SendOrPostCallback(this.OnResetExecution2OperationCompleted);
      this.InvokeAsync("ResetExecution2", new object[0], this.ResetExecution2OperationCompleted, userState);
    }

    private void OnResetExecution2OperationCompleted(object arg)
    {
      if (this.ResetExecution2Completed == null)
        return;
      InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs) arg;
      this.ResetExecution2Completed((object) this, new ResetExecution2CompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
    }

    [SoapHeader("ExecutionHeaderValue")]
    [SoapHeader("ServerInfoHeaderValue", Direction = SoapHeaderDirection.Out)]
    [SoapDocumentMethod("http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices/Render", RequestNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", ResponseNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
    [SoapHeader("TrustedUserHeaderValue")]
    [SoapHeader("PrintControlClsidHeaderValue", Direction = SoapHeaderDirection.Out)]
    [return: XmlElement("Result", DataType = "base64Binary")]
    public byte[] Render(
      string Format,
      string DeviceInfo,
      out string Extension,
      out string MimeType,
      out string Encoding,
      out Warning[] Warnings,
      out string[] StreamIds)
    {
      object[] objArray = this.Invoke(nameof (Render), new object[2]
      {
        (object) Format,
        (object) DeviceInfo
      });
      Extension = (string) objArray[1];
      MimeType = (string) objArray[2];
      Encoding = (string) objArray[3];
      Warnings = (Warning[]) objArray[4];
      StreamIds = (string[]) objArray[5];
      return (byte[]) objArray[0];
    }

    public IAsyncResult BeginRender(
      string Format,
      string DeviceInfo,
      AsyncCallback callback,
      object asyncState)
    {
      return this.BeginInvoke("Render", new object[2]
      {
        (object) Format,
        (object) DeviceInfo
      }, callback, asyncState);
    }

    public byte[] EndRender(
      IAsyncResult asyncResult,
      out string Extension,
      out string MimeType,
      out string Encoding,
      out Warning[] Warnings,
      out string[] StreamIds)
    {
      object[] objArray = this.EndInvoke(asyncResult);
      Extension = (string) objArray[1];
      MimeType = (string) objArray[2];
      Encoding = (string) objArray[3];
      Warnings = (Warning[]) objArray[4];
      StreamIds = (string[]) objArray[5];
      return (byte[]) objArray[0];
    }

    public void RenderAsync(string Format, string DeviceInfo)
    {
      this.RenderAsync(Format, DeviceInfo, (object) null);
    }

    public void RenderAsync(string Format, string DeviceInfo, object userState)
    {
      if (this.RenderOperationCompleted == null)
        this.RenderOperationCompleted = new SendOrPostCallback(this.OnRenderOperationCompleted);
      this.InvokeAsync("Render", new object[2]
      {
        (object) Format,
        (object) DeviceInfo
      }, this.RenderOperationCompleted, userState);
    }

    private void OnRenderOperationCompleted(object arg)
    {
      if (this.RenderCompleted == null)
        return;
      InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs) arg;
      this.RenderCompleted((object) this, new RenderCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
    }

    [SoapHeader("ServerInfoHeaderValue", Direction = SoapHeaderDirection.Out)]
    [SoapDocumentMethod("http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices/Render2", RequestNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", ResponseNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
    [SoapHeader("PrintControlClsidHeaderValue", Direction = SoapHeaderDirection.Out)]
    [SoapHeader("ExecutionHeaderValue")]
    [SoapHeader("TrustedUserHeaderValue")]
    [return: XmlElement("Result", DataType = "base64Binary")]
    public byte[] Render2(
      string Format,
      string DeviceInfo,
      PageCountMode PaginationMode,
      out string Extension,
      out string MimeType,
      out string Encoding,
      out Warning[] Warnings,
      out string[] StreamIds)
    {
      object[] objArray = this.Invoke(nameof (Render2), new object[3]
      {
        (object) Format,
        (object) DeviceInfo,
        (object) PaginationMode
      });
      Extension = (string) objArray[1];
      MimeType = (string) objArray[2];
      Encoding = (string) objArray[3];
      Warnings = (Warning[]) objArray[4];
      StreamIds = (string[]) objArray[5];
      return (byte[]) objArray[0];
    }

    public IAsyncResult BeginRender2(
      string Format,
      string DeviceInfo,
      PageCountMode PaginationMode,
      AsyncCallback callback,
      object asyncState)
    {
      return this.BeginInvoke("Render2", new object[3]
      {
        (object) Format,
        (object) DeviceInfo,
        (object) PaginationMode
      }, callback, asyncState);
    }

    public byte[] EndRender2(
      IAsyncResult asyncResult,
      out string Extension,
      out string MimeType,
      out string Encoding,
      out Warning[] Warnings,
      out string[] StreamIds)
    {
      object[] objArray = this.EndInvoke(asyncResult);
      Extension = (string) objArray[1];
      MimeType = (string) objArray[2];
      Encoding = (string) objArray[3];
      Warnings = (Warning[]) objArray[4];
      StreamIds = (string[]) objArray[5];
      return (byte[]) objArray[0];
    }

    public void Render2Async(string Format, string DeviceInfo, PageCountMode PaginationMode)
    {
      this.Render2Async(Format, DeviceInfo, PaginationMode, (object) null);
    }

    public void Render2Async(
      string Format,
      string DeviceInfo,
      PageCountMode PaginationMode,
      object userState)
    {
      if (this.Render2OperationCompleted == null)
        this.Render2OperationCompleted = new SendOrPostCallback(this.OnRender2OperationCompleted);
      this.InvokeAsync("Render2", new object[3]
      {
        (object) Format,
        (object) DeviceInfo,
        (object) PaginationMode
      }, this.Render2OperationCompleted, userState);
    }

    private void OnRender2OperationCompleted(object arg)
    {
      if (this.Render2Completed == null)
        return;
      InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs) arg;
      this.Render2Completed((object) this, new Render2CompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
    }

    [SoapHeader("TrustedUserHeaderValue")]
    [SoapDocumentMethod("http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices/RenderStream", RequestNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", ResponseNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
    [SoapHeader("ServerInfoHeaderValue", Direction = SoapHeaderDirection.Out)]
    [SoapHeader("PrintControlClsidHeaderValue", Direction = SoapHeaderDirection.Out)]
    [SoapHeader("ExecutionHeaderValue")]
    [return: XmlElement("Result", DataType = "base64Binary")]
    public byte[] RenderStream(
      string Format,
      string StreamID,
      string DeviceInfo,
      out string Encoding,
      out string MimeType)
    {
      object[] objArray = this.Invoke(nameof (RenderStream), new object[3]
      {
        (object) Format,
        (object) StreamID,
        (object) DeviceInfo
      });
      Encoding = (string) objArray[1];
      MimeType = (string) objArray[2];
      return (byte[]) objArray[0];
    }

    public IAsyncResult BeginRenderStream(
      string Format,
      string StreamID,
      string DeviceInfo,
      AsyncCallback callback,
      object asyncState)
    {
      return this.BeginInvoke("RenderStream", new object[3]
      {
        (object) Format,
        (object) StreamID,
        (object) DeviceInfo
      }, callback, asyncState);
    }

    public byte[] EndRenderStream(
      IAsyncResult asyncResult,
      out string Encoding,
      out string MimeType)
    {
      object[] objArray = this.EndInvoke(asyncResult);
      Encoding = (string) objArray[1];
      MimeType = (string) objArray[2];
      return (byte[]) objArray[0];
    }

    public void RenderStreamAsync(string Format, string StreamID, string DeviceInfo)
    {
      this.RenderStreamAsync(Format, StreamID, DeviceInfo, (object) null);
    }

    public void RenderStreamAsync(
      string Format,
      string StreamID,
      string DeviceInfo,
      object userState)
    {
      if (this.RenderStreamOperationCompleted == null)
        this.RenderStreamOperationCompleted = new SendOrPostCallback(this.OnRenderStreamOperationCompleted);
      this.InvokeAsync("RenderStream", new object[3]
      {
        (object) Format,
        (object) StreamID,
        (object) DeviceInfo
      }, this.RenderStreamOperationCompleted, userState);
    }

    private void OnRenderStreamOperationCompleted(object arg)
    {
      if (this.RenderStreamCompleted == null)
        return;
      InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs) arg;
      this.RenderStreamCompleted((object) this, new RenderStreamCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
    }

    [SoapDocumentMethod("http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices/GetExecutionInfo", RequestNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", ResponseNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
    [SoapHeader("TrustedUserHeaderValue")]
    [SoapHeader("ServerInfoHeaderValue", Direction = SoapHeaderDirection.Out)]
    [SoapHeader("PrintControlClsidHeaderValue", Direction = SoapHeaderDirection.Out)]
    [SoapHeader("ExecutionHeaderValue")]
    [return: XmlElement("executionInfo")]
    public ExecutionInfo GetExecutionInfo()
    {
      return (ExecutionInfo) this.Invoke(nameof (GetExecutionInfo), new object[0])[0];
    }

    public IAsyncResult BeginGetExecutionInfo(AsyncCallback callback, object asyncState)
    {
      return this.BeginInvoke("GetExecutionInfo", new object[0], callback, asyncState);
    }

    public ExecutionInfo EndGetExecutionInfo(IAsyncResult asyncResult)
    {
      return (ExecutionInfo) this.EndInvoke(asyncResult)[0];
    }

    public void GetExecutionInfoAsync() => this.GetExecutionInfoAsync((object) null);

    public void GetExecutionInfoAsync(object userState)
    {
      if (this.GetExecutionInfoOperationCompleted == null)
        this.GetExecutionInfoOperationCompleted = new SendOrPostCallback(this.OnGetExecutionInfoOperationCompleted);
      this.InvokeAsync("GetExecutionInfo", new object[0], this.GetExecutionInfoOperationCompleted, userState);
    }

    private void OnGetExecutionInfoOperationCompleted(object arg)
    {
      if (this.GetExecutionInfoCompleted == null)
        return;
      InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs) arg;
      this.GetExecutionInfoCompleted((object) this, new GetExecutionInfoCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
    }

    [SoapHeader("TrustedUserHeaderValue")]
    [SoapDocumentMethod("http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices/GetExecutionInfo2", RequestNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", ResponseNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
    [SoapHeader("ServerInfoHeaderValue", Direction = SoapHeaderDirection.Out)]
    [SoapHeader("PrintControlClsidHeaderValue", Direction = SoapHeaderDirection.Out)]
    [SoapHeader("ExecutionHeaderValue")]
    [return: XmlElement("executionInfo")]
    public ExecutionInfo2 GetExecutionInfo2()
    {
      return (ExecutionInfo2) this.Invoke(nameof (GetExecutionInfo2), new object[0])[0];
    }

    public IAsyncResult BeginGetExecutionInfo2(AsyncCallback callback, object asyncState)
    {
      return this.BeginInvoke("GetExecutionInfo2", new object[0], callback, asyncState);
    }

    public ExecutionInfo2 EndGetExecutionInfo2(IAsyncResult asyncResult)
    {
      return (ExecutionInfo2) this.EndInvoke(asyncResult)[0];
    }

    public void GetExecutionInfo2Async() => this.GetExecutionInfo2Async((object) null);

    public void GetExecutionInfo2Async(object userState)
    {
      if (this.GetExecutionInfo2OperationCompleted == null)
        this.GetExecutionInfo2OperationCompleted = new SendOrPostCallback(this.OnGetExecutionInfo2OperationCompleted);
      this.InvokeAsync("GetExecutionInfo2", new object[0], this.GetExecutionInfo2OperationCompleted, userState);
    }

    private void OnGetExecutionInfo2OperationCompleted(object arg)
    {
      if (this.GetExecutionInfo2Completed == null)
        return;
      InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs) arg;
      this.GetExecutionInfo2Completed((object) this, new GetExecutionInfo2CompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
    }

    [SoapHeader("ExecutionHeaderValue")]
    [SoapDocumentMethod("http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices/GetDocumentMap", RequestNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", ResponseNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
    [SoapHeader("TrustedUserHeaderValue")]
    [SoapHeader("ServerInfoHeaderValue", Direction = SoapHeaderDirection.Out)]
    [SoapHeader("PrintControlClsidHeaderValue", Direction = SoapHeaderDirection.Out)]
    [return: XmlElement("result")]
    public DocumentMapNode GetDocumentMap()
    {
      return (DocumentMapNode) this.Invoke(nameof (GetDocumentMap), new object[0])[0];
    }

    public IAsyncResult BeginGetDocumentMap(AsyncCallback callback, object asyncState)
    {
      return this.BeginInvoke("GetDocumentMap", new object[0], callback, asyncState);
    }

    public DocumentMapNode EndGetDocumentMap(IAsyncResult asyncResult)
    {
      return (DocumentMapNode) this.EndInvoke(asyncResult)[0];
    }

    public void GetDocumentMapAsync() => this.GetDocumentMapAsync((object) null);

    public void GetDocumentMapAsync(object userState)
    {
      if (this.GetDocumentMapOperationCompleted == null)
        this.GetDocumentMapOperationCompleted = new SendOrPostCallback(this.OnGetDocumentMapOperationCompleted);
      this.InvokeAsync("GetDocumentMap", new object[0], this.GetDocumentMapOperationCompleted, userState);
    }

    private void OnGetDocumentMapOperationCompleted(object arg)
    {
      if (this.GetDocumentMapCompleted == null)
        return;
      InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs) arg;
      this.GetDocumentMapCompleted((object) this, new GetDocumentMapCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
    }

    [SoapHeader("PrintControlClsidHeaderValue", Direction = SoapHeaderDirection.Out)]
    [SoapHeader("ExecutionHeaderValue", Direction = SoapHeaderDirection.InOut)]
    [SoapDocumentMethod("http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices/LoadDrillthroughTarget", RequestNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", ResponseNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
    [SoapHeader("ServerInfoHeaderValue", Direction = SoapHeaderDirection.Out)]
    [SoapHeader("TrustedUserHeaderValue")]
    [return: XmlElement("ExecutionInfo")]
    public ExecutionInfo LoadDrillthroughTarget(string DrillthroughID)
    {
      return (ExecutionInfo) this.Invoke(nameof (LoadDrillthroughTarget), new object[1]
      {
        (object) DrillthroughID
      })[0];
    }

    public IAsyncResult BeginLoadDrillthroughTarget(
      string DrillthroughID,
      AsyncCallback callback,
      object asyncState)
    {
      return this.BeginInvoke("LoadDrillthroughTarget", new object[1]
      {
        (object) DrillthroughID
      }, callback, asyncState);
    }

    public ExecutionInfo EndLoadDrillthroughTarget(IAsyncResult asyncResult)
    {
      return (ExecutionInfo) this.EndInvoke(asyncResult)[0];
    }

    public void LoadDrillthroughTargetAsync(string DrillthroughID)
    {
      this.LoadDrillthroughTargetAsync(DrillthroughID, (object) null);
    }

    public void LoadDrillthroughTargetAsync(string DrillthroughID, object userState)
    {
      if (this.LoadDrillthroughTargetOperationCompleted == null)
        this.LoadDrillthroughTargetOperationCompleted = new SendOrPostCallback(this.OnLoadDrillthroughTargetOperationCompleted);
      this.InvokeAsync("LoadDrillthroughTarget", new object[1]
      {
        (object) DrillthroughID
      }, this.LoadDrillthroughTargetOperationCompleted, userState);
    }

    private void OnLoadDrillthroughTargetOperationCompleted(object arg)
    {
      if (this.LoadDrillthroughTargetCompleted == null)
        return;
      InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs) arg;
      this.LoadDrillthroughTargetCompleted((object) this, new LoadDrillthroughTargetCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
    }

    [SoapHeader("ExecutionHeaderValue", Direction = SoapHeaderDirection.InOut)]
    [SoapHeader("PrintControlClsidHeaderValue", Direction = SoapHeaderDirection.Out)]
    [SoapDocumentMethod("http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices/LoadDrillthroughTarget2", RequestNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", ResponseNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
    [SoapHeader("TrustedUserHeaderValue")]
    [SoapHeader("ServerInfoHeaderValue", Direction = SoapHeaderDirection.Out)]
    [return: XmlElement("ExecutionInfo")]
    public ExecutionInfo2 LoadDrillthroughTarget2(string DrillthroughID)
    {
      return (ExecutionInfo2) this.Invoke(nameof (LoadDrillthroughTarget2), new object[1]
      {
        (object) DrillthroughID
      })[0];
    }

    public IAsyncResult BeginLoadDrillthroughTarget2(
      string DrillthroughID,
      AsyncCallback callback,
      object asyncState)
    {
      return this.BeginInvoke("LoadDrillthroughTarget2", new object[1]
      {
        (object) DrillthroughID
      }, callback, asyncState);
    }

    public ExecutionInfo2 EndLoadDrillthroughTarget2(IAsyncResult asyncResult)
    {
      return (ExecutionInfo2) this.EndInvoke(asyncResult)[0];
    }

    public void LoadDrillthroughTarget2Async(string DrillthroughID)
    {
      this.LoadDrillthroughTarget2Async(DrillthroughID, (object) null);
    }

    public void LoadDrillthroughTarget2Async(string DrillthroughID, object userState)
    {
      if (this.LoadDrillthroughTarget2OperationCompleted == null)
        this.LoadDrillthroughTarget2OperationCompleted = new SendOrPostCallback(this.OnLoadDrillthroughTarget2OperationCompleted);
      this.InvokeAsync("LoadDrillthroughTarget2", new object[1]
      {
        (object) DrillthroughID
      }, this.LoadDrillthroughTarget2OperationCompleted, userState);
    }

    private void OnLoadDrillthroughTarget2OperationCompleted(object arg)
    {
      if (this.LoadDrillthroughTarget2Completed == null)
        return;
      InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs) arg;
      this.LoadDrillthroughTarget2Completed((object) this, new LoadDrillthroughTarget2CompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
    }

    [SoapDocumentMethod("http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices/ToggleItem", RequestNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", ResponseNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
    [SoapHeader("ServerInfoHeaderValue", Direction = SoapHeaderDirection.Out)]
    [SoapHeader("PrintControlClsidHeaderValue", Direction = SoapHeaderDirection.Out)]
    [SoapHeader("ExecutionHeaderValue")]
    [SoapHeader("TrustedUserHeaderValue")]
    [return: XmlElement("Found")]
    public bool ToggleItem(string ToggleID)
    {
      return (bool) this.Invoke(nameof (ToggleItem), new object[1]
      {
        (object) ToggleID
      })[0];
    }

    public IAsyncResult BeginToggleItem(string ToggleID, AsyncCallback callback, object asyncState)
    {
      return this.BeginInvoke("ToggleItem", new object[1]
      {
        (object) ToggleID
      }, callback, asyncState);
    }

    public bool EndToggleItem(IAsyncResult asyncResult) => (bool) this.EndInvoke(asyncResult)[0];

    public void ToggleItemAsync(string ToggleID) => this.ToggleItemAsync(ToggleID, (object) null);

    public void ToggleItemAsync(string ToggleID, object userState)
    {
      if (this.ToggleItemOperationCompleted == null)
        this.ToggleItemOperationCompleted = new SendOrPostCallback(this.OnToggleItemOperationCompleted);
      this.InvokeAsync("ToggleItem", new object[1]
      {
        (object) ToggleID
      }, this.ToggleItemOperationCompleted, userState);
    }

    private void OnToggleItemOperationCompleted(object arg)
    {
      if (this.ToggleItemCompleted == null)
        return;
      InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs) arg;
      this.ToggleItemCompleted((object) this, new ToggleItemCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
    }

    [SoapHeader("ExecutionHeaderValue")]
    [SoapDocumentMethod("http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices/NavigateDocumentMap", RequestNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", ResponseNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
    [SoapHeader("ServerInfoHeaderValue", Direction = SoapHeaderDirection.Out)]
    [SoapHeader("PrintControlClsidHeaderValue", Direction = SoapHeaderDirection.Out)]
    [SoapHeader("TrustedUserHeaderValue")]
    [return: XmlElement("PageNumber")]
    public int NavigateDocumentMap(string DocMapID)
    {
      return (int) this.Invoke(nameof (NavigateDocumentMap), new object[1]
      {
        (object) DocMapID
      })[0];
    }

    public IAsyncResult BeginNavigateDocumentMap(
      string DocMapID,
      AsyncCallback callback,
      object asyncState)
    {
      return this.BeginInvoke("NavigateDocumentMap", new object[1]
      {
        (object) DocMapID
      }, callback, asyncState);
    }

    public int EndNavigateDocumentMap(IAsyncResult asyncResult)
    {
      return (int) this.EndInvoke(asyncResult)[0];
    }

    public void NavigateDocumentMapAsync(string DocMapID)
    {
      this.NavigateDocumentMapAsync(DocMapID, (object) null);
    }

    public void NavigateDocumentMapAsync(string DocMapID, object userState)
    {
      if (this.NavigateDocumentMapOperationCompleted == null)
        this.NavigateDocumentMapOperationCompleted = new SendOrPostCallback(this.OnNavigateDocumentMapOperationCompleted);
      this.InvokeAsync("NavigateDocumentMap", new object[1]
      {
        (object) DocMapID
      }, this.NavigateDocumentMapOperationCompleted, userState);
    }

    private void OnNavigateDocumentMapOperationCompleted(object arg)
    {
      if (this.NavigateDocumentMapCompleted == null)
        return;
      InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs) arg;
      this.NavigateDocumentMapCompleted((object) this, new NavigateDocumentMapCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
    }

    [SoapDocumentMethod("http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices/NavigateBookmark", RequestNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", ResponseNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
    [SoapHeader("ExecutionHeaderValue")]
    [SoapHeader("TrustedUserHeaderValue")]
    [SoapHeader("ServerInfoHeaderValue", Direction = SoapHeaderDirection.Out)]
    [SoapHeader("PrintControlClsidHeaderValue", Direction = SoapHeaderDirection.Out)]
    [return: XmlElement("PageNumber")]
    public int NavigateBookmark(string BookmarkID, out string UniqueName)
    {
      object[] objArray = this.Invoke(nameof (NavigateBookmark), new object[1]
      {
        (object) BookmarkID
      });
      UniqueName = (string) objArray[1];
      return (int) objArray[0];
    }

    public IAsyncResult BeginNavigateBookmark(
      string BookmarkID,
      AsyncCallback callback,
      object asyncState)
    {
      return this.BeginInvoke("NavigateBookmark", new object[1]
      {
        (object) BookmarkID
      }, callback, asyncState);
    }

    public int EndNavigateBookmark(IAsyncResult asyncResult, out string UniqueName)
    {
      object[] objArray = this.EndInvoke(asyncResult);
      UniqueName = (string) objArray[1];
      return (int) objArray[0];
    }

    public void NavigateBookmarkAsync(string BookmarkID)
    {
      this.NavigateBookmarkAsync(BookmarkID, (object) null);
    }

    public void NavigateBookmarkAsync(string BookmarkID, object userState)
    {
      if (this.NavigateBookmarkOperationCompleted == null)
        this.NavigateBookmarkOperationCompleted = new SendOrPostCallback(this.OnNavigateBookmarkOperationCompleted);
      this.InvokeAsync("NavigateBookmark", new object[1]
      {
        (object) BookmarkID
      }, this.NavigateBookmarkOperationCompleted, userState);
    }

    private void OnNavigateBookmarkOperationCompleted(object arg)
    {
      if (this.NavigateBookmarkCompleted == null)
        return;
      InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs) arg;
      this.NavigateBookmarkCompleted((object) this, new NavigateBookmarkCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
    }

    [SoapHeader("ExecutionHeaderValue")]
    [SoapDocumentMethod("http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices/FindString", RequestNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", ResponseNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
    [SoapHeader("ServerInfoHeaderValue", Direction = SoapHeaderDirection.Out)]
    [SoapHeader("PrintControlClsidHeaderValue", Direction = SoapHeaderDirection.Out)]
    [SoapHeader("TrustedUserHeaderValue")]
    [return: XmlElement("PageNumber")]
    public int FindString(int StartPage, int EndPage, string FindValue)
    {
      return (int) this.Invoke(nameof (FindString), new object[3]
      {
        (object) StartPage,
        (object) EndPage,
        (object) FindValue
      })[0];
    }

    public IAsyncResult BeginFindString(
      int StartPage,
      int EndPage,
      string FindValue,
      AsyncCallback callback,
      object asyncState)
    {
      return this.BeginInvoke("FindString", new object[3]
      {
        (object) StartPage,
        (object) EndPage,
        (object) FindValue
      }, callback, asyncState);
    }

    public int EndFindString(IAsyncResult asyncResult) => (int) this.EndInvoke(asyncResult)[0];

    public void FindStringAsync(int StartPage, int EndPage, string FindValue)
    {
      this.FindStringAsync(StartPage, EndPage, FindValue, (object) null);
    }

    public void FindStringAsync(int StartPage, int EndPage, string FindValue, object userState)
    {
      if (this.FindStringOperationCompleted == null)
        this.FindStringOperationCompleted = new SendOrPostCallback(this.OnFindStringOperationCompleted);
      this.InvokeAsync("FindString", new object[3]
      {
        (object) StartPage,
        (object) EndPage,
        (object) FindValue
      }, this.FindStringOperationCompleted, userState);
    }

    private void OnFindStringOperationCompleted(object arg)
    {
      if (this.FindStringCompleted == null)
        return;
      InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs) arg;
      this.FindStringCompleted((object) this, new FindStringCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
    }

    [SoapDocumentMethod("http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices/Sort", RequestNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", ResponseNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
    [SoapHeader("TrustedUserHeaderValue")]
    [SoapHeader("ServerInfoHeaderValue", Direction = SoapHeaderDirection.Out)]
    [SoapHeader("PrintControlClsidHeaderValue", Direction = SoapHeaderDirection.Out)]
    [SoapHeader("ExecutionHeaderValue")]
    [return: XmlElement("PageNumber")]
    public int Sort(
      string SortItem,
      SortDirectionEnum Direction,
      bool Clear,
      out string ReportItem,
      out int NumPages)
    {
      object[] objArray = this.Invoke(nameof (Sort), new object[3]
      {
        (object) SortItem,
        (object) Direction,
        (object) Clear
      });
      ReportItem = (string) objArray[1];
      NumPages = (int) objArray[2];
      return (int) objArray[0];
    }

    public IAsyncResult BeginSort(
      string SortItem,
      SortDirectionEnum Direction,
      bool Clear,
      AsyncCallback callback,
      object asyncState)
    {
      return this.BeginInvoke("Sort", new object[3]
      {
        (object) SortItem,
        (object) Direction,
        (object) Clear
      }, callback, asyncState);
    }

    public int EndSort(IAsyncResult asyncResult, out string ReportItem, out int NumPages)
    {
      object[] objArray = this.EndInvoke(asyncResult);
      ReportItem = (string) objArray[1];
      NumPages = (int) objArray[2];
      return (int) objArray[0];
    }

    public void SortAsync(string SortItem, SortDirectionEnum Direction, bool Clear)
    {
      this.SortAsync(SortItem, Direction, Clear, (object) null);
    }

    public void SortAsync(
      string SortItem,
      SortDirectionEnum Direction,
      bool Clear,
      object userState)
    {
      if (this.SortOperationCompleted == null)
        this.SortOperationCompleted = new SendOrPostCallback(this.OnSortOperationCompleted);
      this.InvokeAsync("Sort", new object[3]
      {
        (object) SortItem,
        (object) Direction,
        (object) Clear
      }, this.SortOperationCompleted, userState);
    }

    private void OnSortOperationCompleted(object arg)
    {
      if (this.SortCompleted == null)
        return;
      InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs) arg;
      this.SortCompleted((object) this, new SortCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
    }

    [SoapDocumentMethod("http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices/Sort2", RequestNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", ResponseNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
    [SoapHeader("TrustedUserHeaderValue")]
    [SoapHeader("ExecutionHeaderValue")]
    [SoapHeader("ServerInfoHeaderValue", Direction = SoapHeaderDirection.Out)]
    [SoapHeader("PrintControlClsidHeaderValue", Direction = SoapHeaderDirection.Out)]
    [return: XmlElement("PageNumber")]
    public int Sort2(
      string SortItem,
      SortDirectionEnum Direction,
      bool Clear,
      PageCountMode PaginationMode,
      out string ReportItem,
      out ExecutionInfo2 ExecutionInfo)
    {
      object[] objArray = this.Invoke(nameof (Sort2), new object[4]
      {
        (object) SortItem,
        (object) Direction,
        (object) Clear,
        (object) PaginationMode
      });
      ReportItem = (string) objArray[1];
      ExecutionInfo = (ExecutionInfo2) objArray[2];
      return (int) objArray[0];
    }

    public IAsyncResult BeginSort2(
      string SortItem,
      SortDirectionEnum Direction,
      bool Clear,
      PageCountMode PaginationMode,
      AsyncCallback callback,
      object asyncState)
    {
      return this.BeginInvoke("Sort2", new object[4]
      {
        (object) SortItem,
        (object) Direction,
        (object) Clear,
        (object) PaginationMode
      }, callback, asyncState);
    }

    public int EndSort2(
      IAsyncResult asyncResult,
      out string ReportItem,
      out ExecutionInfo2 ExecutionInfo)
    {
      object[] objArray = this.EndInvoke(asyncResult);
      ReportItem = (string) objArray[1];
      ExecutionInfo = (ExecutionInfo2) objArray[2];
      return (int) objArray[0];
    }

    public void Sort2Async(
      string SortItem,
      SortDirectionEnum Direction,
      bool Clear,
      PageCountMode PaginationMode)
    {
      this.Sort2Async(SortItem, Direction, Clear, PaginationMode, (object) null);
    }

    public void Sort2Async(
      string SortItem,
      SortDirectionEnum Direction,
      bool Clear,
      PageCountMode PaginationMode,
      object userState)
    {
      if (this.Sort2OperationCompleted == null)
        this.Sort2OperationCompleted = new SendOrPostCallback(this.OnSort2OperationCompleted);
      this.InvokeAsync("Sort2", new object[4]
      {
        (object) SortItem,
        (object) Direction,
        (object) Clear,
        (object) PaginationMode
      }, this.Sort2OperationCompleted, userState);
    }

    private void OnSort2OperationCompleted(object arg)
    {
      if (this.Sort2Completed == null)
        return;
      InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs) arg;
      this.Sort2Completed((object) this, new Sort2CompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
    }

    [SoapHeader("TrustedUserHeaderValue")]
    [SoapHeader("ServerInfoHeaderValue", Direction = SoapHeaderDirection.Out)]
    [SoapDocumentMethod("http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices/GetRenderResource", RequestNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", ResponseNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
    [SoapHeader("PrintControlClsidHeaderValue", Direction = SoapHeaderDirection.Out)]
    [return: XmlElement("Result", DataType = "base64Binary")]
    public byte[] GetRenderResource(string Format, string DeviceInfo, out string MimeType)
    {
      object[] objArray = this.Invoke(nameof (GetRenderResource), new object[2]
      {
        (object) Format,
        (object) DeviceInfo
      });
      MimeType = (string) objArray[1];
      return (byte[]) objArray[0];
    }

    public IAsyncResult BeginGetRenderResource(
      string Format,
      string DeviceInfo,
      AsyncCallback callback,
      object asyncState)
    {
      return this.BeginInvoke("GetRenderResource", new object[2]
      {
        (object) Format,
        (object) DeviceInfo
      }, callback, asyncState);
    }

    public byte[] EndGetRenderResource(IAsyncResult asyncResult, out string MimeType)
    {
      object[] objArray = this.EndInvoke(asyncResult);
      MimeType = (string) objArray[1];
      return (byte[]) objArray[0];
    }

    public void GetRenderResourceAsync(string Format, string DeviceInfo)
    {
      this.GetRenderResourceAsync(Format, DeviceInfo, (object) null);
    }

    public void GetRenderResourceAsync(string Format, string DeviceInfo, object userState)
    {
      if (this.GetRenderResourceOperationCompleted == null)
        this.GetRenderResourceOperationCompleted = new SendOrPostCallback(this.OnGetRenderResourceOperationCompleted);
      this.InvokeAsync("GetRenderResource", new object[2]
      {
        (object) Format,
        (object) DeviceInfo
      }, this.GetRenderResourceOperationCompleted, userState);
    }

    private void OnGetRenderResourceOperationCompleted(object arg)
    {
      if (this.GetRenderResourceCompleted == null)
        return;
      InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs) arg;
      this.GetRenderResourceCompleted((object) this, new GetRenderResourceCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
    }

    [SoapHeader("TrustedUserHeaderValue")]
    [SoapHeader("ServerInfoHeaderValue", Direction = SoapHeaderDirection.Out)]
    [SoapDocumentMethod("http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices/ListRenderingExtensions", RequestNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", ResponseNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
    [SoapHeader("PrintControlClsidHeaderValue", Direction = SoapHeaderDirection.Out)]
    [return: XmlArray("Extensions")]
    public Extension[] ListRenderingExtensions()
    {
      return (Extension[]) this.Invoke(nameof (ListRenderingExtensions), new object[0])[0];
    }

    public IAsyncResult BeginListRenderingExtensions(AsyncCallback callback, object asyncState)
    {
      return this.BeginInvoke("ListRenderingExtensions", new object[0], callback, asyncState);
    }

    public Extension[] EndListRenderingExtensions(IAsyncResult asyncResult)
    {
      return (Extension[]) this.EndInvoke(asyncResult)[0];
    }

    public void ListRenderingExtensionsAsync() => this.ListRenderingExtensionsAsync((object) null);

    public void ListRenderingExtensionsAsync(object userState)
    {
      if (this.ListRenderingExtensionsOperationCompleted == null)
        this.ListRenderingExtensionsOperationCompleted = new SendOrPostCallback(this.OnListRenderingExtensionsOperationCompleted);
      this.InvokeAsync("ListRenderingExtensions", new object[0], this.ListRenderingExtensionsOperationCompleted, userState);
    }

    private void OnListRenderingExtensionsOperationCompleted(object arg)
    {
      if (this.ListRenderingExtensionsCompleted == null)
        return;
      InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs) arg;
      this.ListRenderingExtensionsCompleted((object) this, new ListRenderingExtensionsCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
    }

    [SoapHeader("PrintControlClsidHeaderValue", Direction = SoapHeaderDirection.Out)]
    [SoapHeader("ServerInfoHeaderValue", Direction = SoapHeaderDirection.Out)]
    [SoapDocumentMethod("http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices/LogonUser", RequestNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", ResponseNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
    public void LogonUser(string userName, string password, string authority)
    {
      this.Invoke(nameof (LogonUser), new object[3]
      {
        (object) userName,
        (object) password,
        (object) authority
      });
    }

    public IAsyncResult BeginLogonUser(
      string userName,
      string password,
      string authority,
      AsyncCallback callback,
      object asyncState)
    {
      return this.BeginInvoke("LogonUser", new object[3]
      {
        (object) userName,
        (object) password,
        (object) authority
      }, callback, asyncState);
    }

    public void EndLogonUser(IAsyncResult asyncResult) => this.EndInvoke(asyncResult);

    public void LogonUserAsync(string userName, string password, string authority)
    {
      this.LogonUserAsync(userName, password, authority, (object) null);
    }

    public void LogonUserAsync(
      string userName,
      string password,
      string authority,
      object userState)
    {
      if (this.LogonUserOperationCompleted == null)
        this.LogonUserOperationCompleted = new SendOrPostCallback(this.OnLogonUserOperationCompleted);
      this.InvokeAsync("LogonUser", new object[3]
      {
        (object) userName,
        (object) password,
        (object) authority
      }, this.LogonUserOperationCompleted, userState);
    }

    private void OnLogonUserOperationCompleted(object arg)
    {
      if (this.LogonUserCompleted == null)
        return;
      InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs) arg;
      this.LogonUserCompleted((object) this, new AsyncCompletedEventArgs(completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
    }

    [SoapHeader("PrintControlClsidHeaderValue", Direction = SoapHeaderDirection.Out)]
    [SoapDocumentMethod("http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices/Logoff", RequestNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", ResponseNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
    [SoapHeader("ServerInfoHeaderValue", Direction = SoapHeaderDirection.Out)]
    public void Logoff() => this.Invoke(nameof (Logoff), new object[0]);

    public IAsyncResult BeginLogoff(AsyncCallback callback, object asyncState)
    {
      return this.BeginInvoke("Logoff", new object[0], callback, asyncState);
    }

    public void EndLogoff(IAsyncResult asyncResult) => this.EndInvoke(asyncResult);

    public void LogoffAsync() => this.LogoffAsync((object) null);

    public void LogoffAsync(object userState)
    {
      if (this.LogoffOperationCompleted == null)
        this.LogoffOperationCompleted = new SendOrPostCallback(this.OnLogoffOperationCompleted);
      this.InvokeAsync("Logoff", new object[0], this.LogoffOperationCompleted, userState);
    }

    private void OnLogoffOperationCompleted(object arg)
    {
      if (this.LogoffCompleted == null)
        return;
      InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs) arg;
      this.LogoffCompleted((object) this, new AsyncCompletedEventArgs(completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
    }

    public new void CancelAsync(object userState) => base.CancelAsync(userState);
  }
}
