
#nullable disable
namespace Microsoft.ReportingServices.Rendering.HtmlRenderer
{
  internal interface HTMLWriter
  {
    void WriteStream(byte[] bytes);

    void WriteStream(string value);
  }
}
