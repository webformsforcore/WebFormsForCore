
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
