using System;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Web;

namespace Microsoft.Reporting.WebForms;

internal abstract class ReportDataOperation : ViewerDataOperation
{
	private const string ParamCulture = "Culture";

	private const string ParamCultureUI = "UICulture";

	private const string ParamCultureUserOverride = "CultureOverrides";

	private const string ParamCultureUIUserOverride = "UICultureOverrides";

	private const string ParamDrillDepth = "ReportStack";

	protected ReportControlSession m_reportControlSession;

	public ReportDataOperation()
		: this(requiresFullReportLoad: true)
	{
	}

	public ReportDataOperation(bool requiresFullReportLoad)
	{
		NameValueCollection requestParameters = HttpHandler.RequestParameters;
		bool flag = base.ProcessingMode == ProcessingMode.Local;
		if (base.IsUsingSession)
		{
			ReportHierarchy reportHierarchy = base.ReportHierarchy;
			int clientStackSize = HandlerOperation.ParseRequiredInt(requestParameters, "ReportStack");
			try
			{
				reportHierarchy.SyncToClientPage(clientStackSize);
			}
			catch (ArgumentOutOfRangeException e)
			{
				throw new HttpHandlerInputException(e);
			}
			ReportInfo reportInfo = reportHierarchy.Peek();
			if (flag)
			{
				m_reportControlSession = reportInfo.LocalSession;
			}
			else
			{
				m_reportControlSession = reportInfo.ServerSession;
			}
		}
		else
		{
			if (flag)
			{
				throw new HttpHandlerInputException(new NotSupportedException());
			}
			ServerReport serverReport = CreateTempServerReport();
			serverReport.LoadFromUrlQuery(requestParameters, requiresFullReportLoad);
			m_reportControlSession = new ServerModeSession(serverReport);
		}
		int culture = HandlerOperation.ParseRequiredInt(requestParameters, "Culture");
		int culture2 = HandlerOperation.ParseRequiredInt(requestParameters, "UICulture");
		bool useUserOverride = HandlerOperation.ParseRequiredBool(requestParameters, "CultureOverrides");
		bool useUserOverride2 = HandlerOperation.ParseRequiredBool(requestParameters, "UICultureOverrides");
		Thread.CurrentThread.CurrentCulture = new CultureInfo(culture, useUserOverride);
		Thread.CurrentThread.CurrentUICulture = new CultureInfo(culture2, useUserOverride2);
	}

	public override void Dispose()
	{
		if (m_reportControlSession != null)
		{
			if (!base.IsUsingSession)
			{
				m_reportControlSession.Dispose();
			}
			else
			{
				m_reportControlSession.DisposeNonSessionResources();
			}
		}
		base.Dispose();
		GC.SuppressFinalize(this);
	}

	protected static string BaseQuery(Report report, string instanceID)
	{
		string value = ((!(report is ServerReport serverReport)) ? "" : BaseServerQuery(serverReport));
		StringBuilder stringBuilder = new StringBuilder(value);
		stringBuilder.AppendFormat("&{0}={1}", "Culture", CultureInfo.CurrentCulture.LCID.ToString(CultureInfo.InvariantCulture));
		stringBuilder.AppendFormat("&{0}={1}", "CultureOverrides", CultureInfo.CurrentCulture.UseUserOverride.ToString(CultureInfo.InvariantCulture));
		stringBuilder.AppendFormat("&{0}={1}", "UICulture", CultureInfo.CurrentUICulture.LCID.ToString(CultureInfo.InvariantCulture));
		stringBuilder.AppendFormat("&{0}={1}", "UICultureOverrides", CultureInfo.CurrentUICulture.UseUserOverride.ToString(CultureInfo.InvariantCulture));
		stringBuilder.AppendFormat("&{0}={1}", "ReportStack", report.DrillthroughDepth.ToString(CultureInfo.InvariantCulture));
		string value2 = ViewerDataOperation.ViewerDataOperationQuery(report is LocalReport, instanceID);
		stringBuilder.Append(value2);
		if (stringBuilder[0] == '&')
		{
			stringBuilder.Remove(0, 1);
		}
		return stringBuilder.ToString();
	}

	internal static void SetStreamingHeaders(string mimeType, HttpResponse response)
	{
		response.BufferOutput = false;
		if (!string.IsNullOrEmpty(mimeType))
		{
			response.ContentType = mimeType;
		}
		response.Expires = -1;
	}

	internal static void StreamToResponse(Stream data, string mimeType, HttpResponse response)
	{
		SetStreamingHeaders(mimeType, response);
		StreamToResponse(data, response);
	}

	internal static void StreamToResponse(Stream data, HttpResponse response)
	{
		int num = 0;
		byte[] buffer = new byte[81920];
		while ((num = data.Read(buffer, 0, 81920)) > 0)
		{
			response.OutputStream.Write(buffer, 0, num);
		}
	}

	private static string BaseServerQuery(ServerReport serverReport)
	{
		return serverReport.SerializeToUrlQuery();
	}
}
