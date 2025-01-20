
using System.IO;
using System.Text;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  public delegate Stream CreateStreamCallback(
    string name,
    string extension,
    Encoding encoding,
    string mimeType,
    bool willSeek);
}
