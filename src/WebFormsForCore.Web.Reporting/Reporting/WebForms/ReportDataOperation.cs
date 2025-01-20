
using System;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Web;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal abstract class ReportDataOperation : ViewerDataOperation
  {
    private const string ParamCulture = "Culture";
    private const string ParamCultureUI = "UICulture";
    private const string ParamCultureUserOverride = "CultureOverrides";
    private const string ParamCultureUIUserOverride = "UICultureOverrides";
    private const string ParamDrillDepth = "ReportStack";
    protected ReportControlSession m_reportControlSession;

    public ReportDataOperation()
      : this(true)
    {
    }

    public ReportDataOperation(bool requiresFullReportLoad)
    {
      NameValueCollection requestParameters = HttpHandler.RequestParameters;
      bool flag = this.ProcessingMode == ProcessingMode.Local;
      if (this.IsUsingSession)
      {
        ReportHierarchy reportHierarchy = this.ReportHierarchy;
        int requiredInt = HandlerOperation.ParseRequiredInt(requestParameters, "ReportStack");
        try
        {
          reportHierarchy.SyncToClientPage(requiredInt);
        }
        catch (ArgumentOutOfRangeException ex)
        {
          throw new HttpHandlerInputException((Exception) ex);
        }
        ReportInfo reportInfo = reportHierarchy.Peek();
        this.m_reportControlSession = !flag ? (ReportControlSession) reportInfo.ServerSession : (ReportControlSession) reportInfo.LocalSession;
      }
      else
      {
        if (flag)
          throw new HttpHandlerInputException((Exception) new NotSupportedException());
        ServerReport tempServerReport = this.CreateTempServerReport();
        tempServerReport.LoadFromUrlQuery(requestParameters, requiresFullReportLoad);
        this.m_reportControlSession = (ReportControlSession) new ServerModeSession(tempServerReport);
      }
      int requiredInt1 = HandlerOperation.ParseRequiredInt(requestParameters, "Culture");
      int requiredInt2 = HandlerOperation.ParseRequiredInt(requestParameters, "UICulture");
      bool requiredBool1 = HandlerOperation.ParseRequiredBool(requestParameters, "CultureOverrides");
      bool requiredBool2 = HandlerOperation.ParseRequiredBool(requestParameters, "UICultureOverrides");
      Thread.CurrentThread.CurrentCulture = new CultureInfo(requiredInt1, requiredBool1);
      Thread.CurrentThread.CurrentUICulture = new CultureInfo(requiredInt2, requiredBool2);
    }

    public override void Dispose()
    {
      if (this.m_reportControlSession != null)
      {
        if (!this.IsUsingSession)
          this.m_reportControlSession.Dispose();
        else
          this.m_reportControlSession.DisposeNonSessionResources();
      }
      base.Dispose();
      GC.SuppressFinalize((object) this);
    }

    protected static string BaseQuery(Report report, string instanceID)
    {
      StringBuilder stringBuilder = new StringBuilder(!(report is ServerReport serverReport) ? "" : ReportDataOperation.BaseServerQuery(serverReport));
      stringBuilder.AppendFormat("&{0}={1}", (object) "Culture", (object) CultureInfo.CurrentCulture.LCID.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      stringBuilder.AppendFormat("&{0}={1}", (object) "CultureOverrides", (object) CultureInfo.CurrentCulture.UseUserOverride.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      stringBuilder.AppendFormat("&{0}={1}", (object) "UICulture", (object) CultureInfo.CurrentUICulture.LCID.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      stringBuilder.AppendFormat("&{0}={1}", (object) "UICultureOverrides", (object) CultureInfo.CurrentUICulture.UseUserOverride.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      stringBuilder.AppendFormat("&{0}={1}", (object) "ReportStack", (object) report.DrillthroughDepth.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      string str = ViewerDataOperation.ViewerDataOperationQuery(report is LocalReport, instanceID);
      stringBuilder.Append(str);
      if (stringBuilder[0] == '&')
        stringBuilder.Remove(0, 1);
      return stringBuilder.ToString();
    }

    internal static void SetStreamingHeaders(string mimeType, HttpResponse response)
    {
      response.BufferOutput = false;
      if (!string.IsNullOrEmpty(mimeType))
        response.ContentType = mimeType;
      response.Expires = -1;
    }

    internal static void StreamToResponse(Stream data, string mimeType, HttpResponse response)
    {
      ReportDataOperation.SetStreamingHeaders(mimeType, response);
      ReportDataOperation.StreamToResponse(data, response);
    }

    internal static void StreamToResponse(Stream data, HttpResponse response)
    {
      byte[] buffer = new byte[81920];
      int count;
      while ((count = data.Read(buffer, 0, 81920)) > 0)
        response.OutputStream.Write(buffer, 0, count);
    }

    private static string BaseServerQuery(ServerReport serverReport)
    {
      return serverReport.SerializeToUrlQuery();
    }
  }
}
