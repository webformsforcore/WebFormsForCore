
using System.IO;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  public interface ITemporaryStorage
  {
    Stream CreateTemporaryStream();
  }
}
