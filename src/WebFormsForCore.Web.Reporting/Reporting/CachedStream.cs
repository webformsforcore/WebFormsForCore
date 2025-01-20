
using System;
using System.IO;
using System.Text;

#nullable disable
namespace Microsoft.Reporting
{
  [Serializable]
  internal sealed class CachedStream : IDisposable
  {
    private Stream m_stream;
    private Encoding m_encoding;
    private string m_mimeType;
    private string m_fileExtension;

    public CachedStream(Stream stream, Encoding encoding, string mimeType, string fileExtension)
    {
      this.m_stream = stream != null ? stream : throw new ArgumentNullException(nameof (stream));
      this.m_encoding = encoding;
      this.m_mimeType = mimeType;
      this.m_fileExtension = fileExtension;
    }

    public static Stream Extract(
      CachedStream cachedStream,
      out string encoding,
      out string mimeType,
      out string fileExtension)
    {
      if (cachedStream != null)
      {
        encoding = cachedStream.Encoding != null ? cachedStream.Encoding.EncodingName : (string) null;
        mimeType = cachedStream.MimeType;
        fileExtension = cachedStream.FileExtension;
        return cachedStream.Stream;
      }
      encoding = (string) null;
      mimeType = (string) null;
      fileExtension = (string) null;
      return (Stream) null;
    }

    public void Dispose() => this.m_stream.Dispose();

    public Stream Stream
    {
      get
      {
        this.m_stream.Seek(0L, SeekOrigin.Begin);
        return this.m_stream;
      }
    }

    public Encoding Encoding => this.m_encoding;

    public string MimeType => this.m_mimeType;

    public string FileExtension => this.m_fileExtension;
  }
}
