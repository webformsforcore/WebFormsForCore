// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.ReportViewerUtils
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

using System.Drawing.Printing;

#nullable disable
namespace Microsoft.Reporting
{
  internal static class ReportViewerUtils
  {
    public static PageSettings DeepClonePageSettings(PageSettings pageSettings)
    {
      if (pageSettings == null)
        return (PageSettings) null;
      PageSettings pageSettings1 = (PageSettings) pageSettings.Clone();
      Margins margins = pageSettings.Margins;
      if (margins != (Margins) null)
        pageSettings1.Margins = (Margins) margins.Clone();
      pageSettings1.PaperSize = new PaperSize();
      pageSettings1.PaperSize.Height = pageSettings.PaperSize.Height;
      pageSettings1.PaperSize.PaperName = pageSettings.PaperSize.PaperName;
      pageSettings1.PaperSize.RawKind = pageSettings.PaperSize.RawKind;
      pageSettings1.PaperSize.Width = pageSettings.PaperSize.Width;
      pageSettings1.PaperSource = new PaperSource();
      pageSettings1.PaperSource.RawKind = pageSettings.PaperSource.RawKind;
      pageSettings1.PaperSource.SourceName = pageSettings.PaperSource.SourceName;
      pageSettings1.PrinterResolution = new PrinterResolution();
      pageSettings1.PrinterResolution.Kind = pageSettings.PrinterResolution.Kind;
      pageSettings1.PrinterResolution.X = pageSettings.PrinterResolution.X;
      pageSettings1.PrinterResolution.Y = pageSettings.PrinterResolution.Y;
      if (pageSettings.PrinterSettings != null)
        pageSettings1.PrinterSettings = pageSettings.PrinterSettings;
      return pageSettings1;
    }
  }
}
