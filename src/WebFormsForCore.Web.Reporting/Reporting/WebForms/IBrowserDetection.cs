
#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal interface IBrowserDetection
  {
    bool IsIE { get; }

    bool IsSafari { get; }
  }
}
