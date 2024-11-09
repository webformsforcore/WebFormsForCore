// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.ViewerDataOperation
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

using System;
using System.Globalization;
using System.Text;
using System.Web;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal abstract class ViewerDataOperation : HandlerOperation
  {
    private const string ParamIsLocalMode = "Mode";
    private const string ParamControlID = "ControlID";
    private ReportHierarchy m_reportHierarchy;
    private string m_instanceID;
    private bool m_isUsingSession;
    private ProcessingMode m_processingMode = ProcessingMode.Remote;

    public ViewerDataOperation()
    {
      this.m_instanceID = HandlerOperation.GetAndEnsureParam(HttpHandler.RequestParameters, "ControlID");
      if (HttpHandler.RequestParameters["Mode"] != null)
        this.m_processingMode = ProcessingMode.Local;
      this.m_isUsingSession = this.CreateTempReportViewer().EnsureSessionOrConfig();
      if (!this.m_isUsingSession)
        return;
      this.m_reportHierarchy = (ReportHierarchy) HttpContext.Current.Session[this.m_instanceID];
      if (this.m_reportHierarchy == null)
        throw new AspNetSessionExpiredException();
    }

    protected ProcessingMode ProcessingMode => this.m_processingMode;

    protected string InstanceID => this.m_instanceID;

    protected ReportHierarchy ReportHierarchy => this.m_reportHierarchy;

    protected bool IsUsingSession => this.m_isUsingSession;

    private ReportViewer CreateTempReportViewer()
    {
      ReportViewer reportViewer = ReportViewerFactory.CreateReportViewer();
      reportViewer.ProcessingMode = this.ProcessingMode;
      return reportViewer;
    }

    protected ServerReport CreateTempServerReport()
    {
      return this.CreateTempReportViewer().CreateServerReport();
    }

    protected static string ViewerDataOperationQuery(bool isLocalMode, string instanceID)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "&{0}={1}", (object) "ControlID", (object) HttpUtility.UrlEncode(instanceID));
      if (isLocalMode)
        stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "&{0}={1}", (object) "Mode", (object) "true");
      return stringBuilder.ToString();
    }
  }
}
