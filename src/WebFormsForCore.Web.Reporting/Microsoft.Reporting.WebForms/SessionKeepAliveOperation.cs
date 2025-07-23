using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;

namespace Microsoft.Reporting.WebForms;

internal sealed class SessionKeepAliveOperation : ViewerDataOperation
{
	public static ScriptComponentDescriptor CreateRequest(ReportViewer viewer)
	{
		bool flag = viewer.ProcessingMode == ProcessingMode.Local;
		bool flag2 = viewer.EnsureSessionOrConfig();
		List<string> list = new List<string>();
		int num = int.MaxValue;
		if (!flag)
		{
			foreach (ServerReport reportsWithSession in GetReportsWithSessions(viewer.ReportHierarchy))
			{
				if (reportsWithSession.HasExecutionId)
				{
					if (!flag2)
					{
						list.Add(reportsWithSession.GetExecutionId());
					}
					num = Math.Min(num, (int)(reportsWithSession.GetExecutionSessionExpiration() - DateTime.Now.ToUniversalTime()).TotalSeconds);
				}
			}
		}
		if (flag2)
		{
			int val = HttpContext.Current.Session.Timeout * 60;
			num = Math.Min(num, val);
		}
		return CreateRequest(viewer.InstanceIdentifier, flag, num, flag2, list.ToArray());
	}

	public static ScriptComponentDescriptor CreateRequest(string viewerInstanceIdentifier, int sessionExpirationSeconds, string[] serverExecutionIds)
	{
		return CreateRequest(viewerInstanceIdentifier, isLocalMode: false, sessionExpirationSeconds, isUsingSession: false, serverExecutionIds);
	}

	private static ScriptComponentDescriptor CreateRequest(string viewerInstanceIdentifier, bool isLocalMode, int sessionExpirationSeconds, bool isUsingSession, string[] serverExecutionIds)
	{
		if (!isUsingSession && (serverExecutionIds == null || serverExecutionIds.Length == 0))
		{
			return null;
		}
		if (sessionExpirationSeconds < 120)
		{
			return null;
		}
		UriBuilder handlerUri = ReportViewerFactory.HttpHandler.HandlerUri;
		handlerUri.Query = string.Format(CultureInfo.InvariantCulture, "{0}={1}{2}", "OpType", "SessionKeepAlive", ViewerDataOperation.ViewerDataOperationQuery(isLocalMode, viewerInstanceIdentifier));
		string pathAndQuery = handlerUri.Uri.PathAndQuery;
		string value = null;
		if (!isUsingSession)
		{
			value = string.Join(Environment.NewLine, serverExecutionIds);
		}
		int num = sessionExpirationSeconds - 60;
		ScriptComponentDescriptor scriptComponentDescriptor = new ScriptComponentDescriptor("Microsoft.Reporting.WebFormsClient._SessionKeepAlive");
		scriptComponentDescriptor.AddProperty("KeepAliveUrl", pathAndQuery);
		scriptComponentDescriptor.AddProperty("KeepAliveBody", value);
		scriptComponentDescriptor.AddProperty("KeepAliveIntervalSeconds", num);
		return scriptComponentDescriptor;
	}

	public override void PerformOperation(NameValueCollection urlQuery, HttpResponse response)
	{
		if (base.ProcessingMode == ProcessingMode.Remote)
		{
			bool flag = false;
			if (base.IsUsingSession)
			{
				foreach (ServerReport reportsWithSession in GetReportsWithSessions(base.ReportHierarchy))
				{
					if (TryTouchServerSession(reportsWithSession))
					{
						flag = true;
					}
				}
			}
			else
			{
				Stream inputStream = HttpContext.Current.Request.InputStream;
				if (inputStream.Length > 0)
				{
					byte[] array = new byte[inputStream.Length];
					inputStream.Read(array, 0, array.Length);
					string text = Encoding.UTF8.GetString(array);
					string[] separator = new string[1] { Environment.NewLine };
					string[] array2 = text.Split(separator, StringSplitOptions.RemoveEmptyEntries);
					if (array2 != null)
					{
						ServerReport serverReport = CreateTempServerReport();
						string[] array3 = array2;
						foreach (string executionId in array3)
						{
							serverReport.SetExecutionId(executionId, fullReportLoad: false);
							if (TryTouchServerSession(serverReport))
							{
								flag = true;
							}
						}
					}
				}
			}
			if (!flag)
			{
				response.StatusCode = 500;
			}
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
		foreach (ReportInfo reportInfo in reportHierarchy)
		{
			ServerReport serverReport = reportInfo.ServerReport;
			if (serverReport.HasExecutionId)
			{
				yield return serverReport;
			}
		}
	}
}
