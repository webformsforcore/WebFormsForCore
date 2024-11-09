// Decompiled with JetBrains decompiler
// Type: Microsoft.ReportingServices.Rendering.HtmlRenderer.TablixFixedHeaderStorage
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.ReportingServices.Rendering.HtmlRenderer
{
  internal class TablixFixedHeaderStorage
  {
    private string m_bodyId;
    private string m_htmlId;
    private string m_lastRowGroupCol = "";
    private int m_firstRowGroupColIndex = 1;
    private List<string> m_rowHeaders;
    private List<string> m_columnHeaders;
    private List<string> m_cornerHeaders;

    internal string BodyID
    {
      get => this.m_bodyId;
      set => this.m_bodyId = value;
    }

    internal string HtmlId
    {
      get => this.m_htmlId;
      set => this.m_htmlId = value;
    }

    public List<string> RowHeaders
    {
      get => this.m_rowHeaders;
      set => this.m_rowHeaders = value;
    }

    public bool HasEmptyCol
    {
      get => this.m_firstRowGroupColIndex == 2;
      set
      {
        if (value)
          this.m_firstRowGroupColIndex = 2;
        else
          this.m_firstRowGroupColIndex = 1;
      }
    }

    public string FirstRowGroupCol
    {
      get => this.m_rowHeaders == null ? "" : this.m_rowHeaders[this.m_firstRowGroupColIndex];
    }

    public string LastRowGroupCol
    {
      get => this.m_lastRowGroupCol;
      set => this.m_lastRowGroupCol = value;
    }

    public string LastColGroupRow
    {
      get
      {
        return this.m_columnHeaders == null ? "" : this.m_columnHeaders[this.m_columnHeaders.Count - 1];
      }
    }

    public List<string> ColumnHeaders
    {
      get => this.m_columnHeaders;
      set => this.m_columnHeaders = value;
    }

    public List<string> CornerHeaders
    {
      get => this.m_cornerHeaders;
      set => this.m_cornerHeaders = value;
    }
  }
}
