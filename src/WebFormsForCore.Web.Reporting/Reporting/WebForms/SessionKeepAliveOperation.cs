
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal sealed class SessionKeepAliveOperation : ViewerDataOperation
  {
    public static ScriptComponentDescriptor CreateRequest(ReportViewer viewer)
    {
      bool isLocalMode = viewer.ProcessingMode == ProcessingMode.Local;
      bool isUsingSession = viewer.EnsureSessionOrConfig();
      List<string> stringList = new List<string>();
      int num = int.MaxValue;
      if (!isLocalMode)
      {
        foreach (ServerReport reportsWithSession in SessionKeepAliveOperation.GetReportsWithSessions(viewer.ReportHierarchy))
        {
          if (reportsWithSession.HasExecutionId)
          {
            if (!isUsingSession)
              stringList.Add(reportsWithSession.GetExecutionId());
            TimeSpan timeSpan = reportsWithSession.GetExecutionSessionExpiration() - DateTime.Now.ToUniversalTime();
            num = Math.Min(num, (int) timeSpan.TotalSeconds);
          }
        }
      }
      if (isUsingSession)
      {
        int val2 = HttpContext.Current.Session.Timeout * 60;
        num = Math.Min(num, val2);
      }
      return SessionKeepAliveOperation.CreateRequest(viewer.InstanceIdentifier, isLocalMode, num, isUsingSession, stringList.ToArray());
    }

    public static ScriptComponentDescriptor CreateRequest(
      string viewerInstanceIdentifier,
      int sessionExpirationSeconds,
      string[] serverExecutionIds)
    {
      return SessionKeepAliveOperation.CreateRequest(viewerInstanceIdentifier, false, sessionExpirationSeconds, false, serverExecutionIds);
    }

    private static ScriptComponentDescriptor CreateRequest(
      string viewerInstanceIdentifier,
      bool isLocalMode,
      int sessionExpirationSeconds,
      bool isUsingSession,
      string[] serverExecutionIds)
    {
      if (!isUsingSession && (serverExecutionIds == null || serverExecutionIds.Length == 0))
        return (ScriptComponentDescriptor) null;
      if (sessionExpirationSeconds < 120)
        return (ScriptComponentDescriptor) null;
      UriBuilder handlerUri = ReportViewerFactory.HttpHandler.HandlerUri;
      handlerUri.Query = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}={1}{2}", (object) "OpType", (object) "SessionKeepAlive", (object) ViewerDataOperation.ViewerDataOperationQuery(isLocalMode, viewerInstanceIdentifier));
      string pathAndQuery = handlerUri.Uri.PathAndQuery;
      string str = (string) null;
      if (!isUsingSession)
        str = string.Join(Environment.NewLine, serverExecutionIds);
      int num = sessionExpirationSeconds - 60;
      ScriptComponentDescriptor request = new ScriptComponentDescriptor("Microsoft.Reporting.WebFormsClient._SessionKeepAlive");
      request.AddProperty("KeepAliveUrl", (object) pathAndQuery);
      request.AddProperty("KeepAliveBody", (object) str);
      request.AddProperty("KeepAliveIntervalSeconds", (object) num);
      return request;
    }

    public override void PerformOperation(NameValueCollection urlQuery, HttpResponse response)
    {
      if (this.ProcessingMode == ProcessingMode.Remote)
      {
        bool flag = false;
        if (this.IsUsingSession)
        {
          foreach (ServerReport reportsWithSession in SessionKeepAliveOperation.GetReportsWithSessions(this.ReportHierarchy))
          {
            if (this.TryTouchServerSession(reportsWithSession))
              flag = true;
          }
        }
        else
        {
          Stream inputStream = HttpContext.Current.Request.InputStream;
          if (inputStream.Length > 0L)
          {
            byte[] numArray = new byte[inputStream.Length];
            inputStream.Read(numArray, 0, numArray.Length);
            string[] strArray = Encoding.UTF8.GetString(numArray).Split(new string[1]
            {
              Environment.NewLine
            }, StringSplitOptions.RemoveEmptyEntries);
            if (strArray != null)
            {
              ServerReport tempServerReport = this.CreateTempServerReport();
              foreach (string executionId in strArray)
              {
                tempServerReport.SetExecutionId(executionId, false);
                if (this.TryTouchServerSession(tempServerReport))
                  flag = true;
              }
            }
          }
        }
        if (!flag)
          response.StatusCode = 500;
      }
      response.ContentType = "text/plain";
      response.Write("OK");
    }

    private bool TryTouchServerSession(ServerReport serverReport)
    {
      try
      {
        serverReport.TouchSession();
        return true;
      }
      catch
      {
        return false;
      }
    }

    private static IEnumerable<ServerReport> GetReportsWithSessions(ReportHierarchy reportHierarchy)
    {
      foreach (ReportInfo reportInfo in (Stack<ReportInfo>) reportHierarchy)
      {
        ServerReport serverReport = reportInfo.ServerReport;
        if (serverReport.HasExecutionId)
          yield return serverReport;
      }
    }
  }
}
