
using System;
using System.Globalization;
using System.IO;
using System.Web.UI;

#nullable disable
namespace Microsoft.ReportingServices.Rendering.HtmlRenderer
{
  internal class Utility
  {
    internal const int TextBufferSize = 16384;

    private Utility()
    {
    }

    internal static void CopyStream(Stream source, Stream sink)
    {
      byte[] buffer = new byte[4096];
      for (int count = source.Read(buffer, 0, buffer.Length); count != 0; count = source.Read(buffer, 0, buffer.Length))
        sink.Write(buffer, 0, count);
    }

    internal static string MmToPxAsString(double size)
    {
      return Convert.ToInt64(size * (480.0 / (double) sbyte.MaxValue)).ToString((IFormatProvider) CultureInfo.InvariantCulture);
    }

    internal static long MMToPx(double size)
    {
      return Convert.ToInt64(size * (480.0 / (double) sbyte.MaxValue));
    }

    internal static BufferedStream CreateBufferedStream(HtmlTextWriter sourceWriter)
    {
      return Utility.CreateBufferedStream(((StreamWriter) sourceWriter.InnerWriter).BaseStream);
    }

    internal static BufferedStream CreateBufferedStream(Stream sourceStream)
    {
      return new BufferedStream(sourceStream, 16384);
    }
  }
}
