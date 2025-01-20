
#nullable disable
namespace Microsoft.Reporting.WebForms
{
  public interface IReportViewerMessages3 : IReportViewerMessages2, IReportViewerMessages
  {
    string TotalPages(int pageCount, PageCountMode pageCountMode);

    string CancelLinkText { get; }

    string CalendarLoading { get; }
  }
}
