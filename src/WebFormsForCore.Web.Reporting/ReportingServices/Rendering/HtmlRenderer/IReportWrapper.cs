﻿// Decompiled with JetBrains decompiler
// Type: Microsoft.ReportingServices.Rendering.HtmlRenderer.IReportWrapper
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

#nullable disable
namespace Microsoft.ReportingServices.Rendering.HtmlRenderer
{
  internal interface IReportWrapper
  {
    string GetStreamUrl(bool useSessionId, string streamName);

    bool HasBookmarks { get; }

    string SortItem { get; }

    string ShowHideToggle { get; }

    string GetReportUrl(bool addParams);

    byte[] GetImageName(string imageID);
  }
}
