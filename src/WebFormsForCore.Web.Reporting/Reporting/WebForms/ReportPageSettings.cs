
using System;
using System.Drawing.Printing;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  [Serializable]
  public sealed class ReportPageSettings
  {
    private int m_pageWidth;
    private int m_pageHeight;
    private Margins m_margins;

    internal ReportPageSettings(
      double pageHeight,
      double pageWidth,
      double leftMargin,
      double rightMargin,
      double topMargin,
      double bottomMargin)
    {
      this.m_pageWidth = ReportPageSettings.ConvertMmTo100thInch(pageWidth);
      this.m_pageHeight = ReportPageSettings.ConvertMmTo100thInch(pageHeight);
      this.m_margins = new Margins(ReportPageSettings.ConvertMmTo100thInch(leftMargin), ReportPageSettings.ConvertMmTo100thInch(rightMargin), ReportPageSettings.ConvertMmTo100thInch(topMargin), ReportPageSettings.ConvertMmTo100thInch(bottomMargin));
    }

    internal ReportPageSettings()
      : this(1100.0, 850.0, 50.0, 50.0, 50.0, 50.0)
    {
    }

    private static int ConvertMmTo100thInch(double mm) => (int) Math.Round(mm / 25.4 * 100.0);

    public PaperSize PaperSize
    {
      get
      {
        PageSettings customPageSettings = this.CustomPageSettings;
        ReportPageSettings.UpdatePageSettingsForPrinter(customPageSettings, new PrinterSettings());
        return customPageSettings.PaperSize;
      }
    }

    public Margins Margins => (Margins) this.m_margins.Clone();

    public bool IsLandscape => this.m_pageWidth > this.m_pageHeight;

    internal PageSettings CustomPageSettings
    {
      get
      {
        PageSettings customPageSettings = new PageSettings();
        int width = Math.Min(this.m_pageWidth, this.m_pageHeight);
        int height = Math.Max(this.m_pageWidth, this.m_pageHeight);
        customPageSettings.PaperSize = new PaperSize("", width, height);
        customPageSettings.Landscape = this.IsLandscape;
        customPageSettings.Margins = this.Margins;
        return customPageSettings;
      }
    }

    internal static void UpdatePageSettingsForPrinter(
      PageSettings pageSettings,
      PrinterSettings printerSettings)
    {
      if (!printerSettings.IsValid)
        return;
      int num1 = pageSettings.Landscape ? pageSettings.PaperSize.Height : pageSettings.PaperSize.Width;
      int num2 = pageSettings.Landscape ? pageSettings.PaperSize.Width : pageSettings.PaperSize.Height;
      foreach (PaperSize paperSiz in printerSettings.PaperSizes)
      {
        if (num1 == paperSiz.Width && num2 == paperSiz.Height)
        {
          pageSettings.Landscape = false;
          pageSettings.PaperSize = paperSiz;
          break;
        }
        if (num1 == paperSiz.Height && num2 == paperSiz.Width)
        {
          pageSettings.Landscape = true;
          pageSettings.PaperSize = paperSiz;
          break;
        }
      }
      pageSettings.PrinterSettings = printerSettings;
    }

    internal PageSettings ToPageSettings(PrinterSettings currentPrinter)
    {
      PageSettings customPageSettings = this.CustomPageSettings;
      ReportPageSettings.UpdatePageSettingsForPrinter(customPageSettings, currentPrinter);
      customPageSettings.Margins = this.Margins;
      return customPageSettings;
    }
  }
}
