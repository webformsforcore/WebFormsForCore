// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.MemoryThenTempStorageStream
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

using System.IO;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal sealed class MemoryThenTempStorageStream : Stream
  {
    private const int m_threshold = 65536;
    private ITemporaryStorage m_tempStorage;
    private Stream m_storageStream = (Stream) new MemoryStream();
    private bool m_thresholdReached;
    private bool m_isClosed;

    public MemoryThenTempStorageStream(ITemporaryStorage storage) => this.m_tempStorage = storage;

    public override bool CanRead => true;

    public override bool CanSeek => true;

    public override bool CanWrite => true;

    public override long Length => this.m_storageStream.Length;

    public override long Position
    {
      get => this.m_storageStream.Position;
      set => this.m_storageStream.Position = value;
    }

    public override void Flush() => this.m_storageStream.Flush();

    protected override void Dispose(bool disposing)
    {
      base.Dispose(disposing);
      if (!disposing || this.m_isClosed)
        return;
      this.m_storageStream.Close();
      this.m_isClosed = true;
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      return this.m_storageStream.Read(buffer, offset, count);
    }

    public override int ReadByte() => this.m_storageStream.ReadByte();

    public override long Seek(long offset, SeekOrigin origin)
    {
      return this.m_storageStream.Seek(offset, origin);
    }

    public override void SetLength(long value)
    {
      if (!this.m_thresholdReached && value > 65536L)
        this.ThresholdReached();
      if (this.m_storageStream == null)
        return;
      this.m_storageStream.SetLength(value);
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
      if (!this.m_thresholdReached && this.Position + (long) count > 65536L)
        this.ThresholdReached();
      if (this.m_storageStream == null)
        return;
      this.m_storageStream.Write(buffer, offset, count);
    }

    public override void WriteByte(byte value)
    {
      if (!this.m_thresholdReached && this.Position >= 65536L)
        this.ThresholdReached();
      if (this.m_storageStream == null)
        return;
      this.m_storageStream.WriteByte(value);
    }

    private void ThresholdReached()
    {
      Stream temporaryStream = this.m_tempStorage.CreateTemporaryStream();
      if (temporaryStream == null)
        return;
      if (!temporaryStream.CanSeek || !temporaryStream.CanRead || !temporaryStream.CanWrite)
        throw new InvalidTemporaryStorageStreamException();
      this.m_storageStream.Position = 0L;
      byte[] buffer = new byte[this.m_storageStream.Length];
      this.m_storageStream.Read(buffer, 0, buffer.Length);
      temporaryStream.Write(buffer, 0, buffer.Length);
      this.m_storageStream.Close();
      this.m_storageStream = temporaryStream;
      this.m_thresholdReached = true;
    }
  }
}
