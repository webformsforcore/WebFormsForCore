// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.IReportExecutionService
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

using Microsoft.ReportingServices.Diagnostics.Utilities;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Xml;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal interface IReportExecutionService
  {
    ExecutionInfo GetExecutionInfo();

    ExecutionInfo ResetExecution();

    ExecutionInfo LoadReport(string report, string historyId);

    ExecutionInfo LoadReportDefinition(byte[] definition);

    DocumentMapNode GetDocumentMap(string rootLabel);

    RenderingExtension[] ListRenderingExtensions();

    ExecutionInfo SetExecutionCredentials(IEnumerable<DataSourceCredentials> credentials);

    ExecutionInfo SetExecutionParameters(
      IEnumerable<ReportParameter> parameters,
      string parameterLanguage);

    byte[] Render(
      string format,
      string deviceInfo,
      PageCountMode paginationMode,
      out string extension,
      out string mimeType,
      out string encoding,
      out Warning[] warnings,
      out string[] streamIds);

    void Render(
      AbortState abortState,
      string reportPath,
      string executionId,
      string historyId,
      string format,
      XmlNodeList deviceInfo,
      NameValueCollection urlAccessParameters,
      Stream reportStream,
      out string mimeType,
      out string fileNameExtension);

    byte[] RenderStream(
      string format,
      string streamId,
      string deviceInfo,
      out string encoding,
      out string mimeType);

    int FindString(int startPage, int endPage, string findValue);

    void ToggleItem(string toggleId);

    int NavigateBookmark(string bookmarkId, out string uniqueName);

    int NavigateDocumentMap(string documentMapId);

    ExecutionInfo LoadDrillthroughTarget(string drillthroughId);

    int Sort(
      string sortItem,
      SortOrder direction,
      bool clear,
      PageCountMode paginationMode,
      out string reportItem,
      out ExecutionInfo executionInfo,
      out int numPages);

    string GetPrintControlClsid(ClientArchitecture arch);

    bool IsPrintCabSupported(ClientArchitecture arch);

    void WritePrintCab(ClientArchitecture arch, Stream stream);

    byte[] GetStyleSheet(string styleSheetName, bool isImage, out string mimeType);

    void SetExecutionId(string executionId);

    string GetServerVersion();

    int Timeout { set; }
  }
}
