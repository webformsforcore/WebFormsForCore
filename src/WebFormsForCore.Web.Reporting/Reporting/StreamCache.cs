// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.StreamCache
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

using Microsoft.ReportingServices.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

#nullable disable
namespace Microsoft.Reporting
{
  [Serializable]
  internal sealed class StreamCache : IDisposable
  {
    private CachedStream m_mainStream;
    private bool m_mainStreamDetached;
    private Dictionary<string, CachedStream> m_secondaryStreams = new Dictionary<string, CachedStream>();
    private CreateStreamDelegate m_createMainStreamDelegate;

    public StreamCache()
      : this((CreateStreamDelegate) null)
    {
    }

    public StreamCache(CreateStreamDelegate createMainStreamDelegate)
    {
      this.m_createMainStreamDelegate = createMainStreamDelegate ?? new CreateStreamDelegate(StreamCache.DefaultCreateStreamDelegate);
    }

    public void Dispose() => this.Clear();

    public void Clear()
    {
      if (!this.m_mainStreamDetached && this.m_mainStream != null)
        this.m_mainStream.Dispose();
      foreach (CachedStream cachedStream in this.m_secondaryStreams.Values)
        cachedStream.Dispose();
      this.m_mainStream = (CachedStream) null;
      this.m_mainStreamDetached = false;
      this.m_secondaryStreams.Clear();
    }

    public Stream StreamCallback(
      string name,
      string extension,
      Encoding encoding,
      string mimeType,
      bool useChunking,
      StreamOper operation)
    {
      int num;
      switch (operation)
      {
        case StreamOper.CreateAndRegister:
          if (this.m_mainStream == null)
          {
            num = !this.m_mainStreamDetached ? 1 : 0;
            break;
          }
          goto default;
        case StreamOper.RegisterOnly:
          return (Stream) null;
        default:
          num = 0;
          break;
      }
      bool flag = num != 0;
      CachedStream cachedStream = new CachedStream((flag ? this.m_createMainStreamDelegate : new CreateStreamDelegate(StreamCache.DefaultCreateStreamDelegate))(), encoding, mimeType, extension);
      if (operation == StreamOper.CreateAndRegister)
      {
        if (flag)
          this.m_mainStream = cachedStream;
        else
          this.m_secondaryStreams.Add(name, cachedStream);
      }
      return cachedStream.Stream;
    }

    public Stream GetMainStream(bool detach)
    {
      return this.GetMainStream(detach, out string _, out string _, out string _);
    }

    public Stream GetMainStream(
      bool detach,
      out string encoding,
      out string mimeType,
      out string fileExtension)
    {
      Stream mainStream = CachedStream.Extract(this.m_mainStream, out encoding, out mimeType, out fileExtension);
      if (detach)
      {
        this.m_mainStreamDetached = detach;
        this.m_mainStream = (CachedStream) null;
      }
      return mainStream;
    }

    public byte[] GetMainStream(out string encoding, out string mimeType, out string fileExtension)
    {
      return this.StreamToBytes(this.GetMainStream(false, out encoding, out mimeType, out fileExtension));
    }

    public byte[] GetSecondaryStream(
      bool remove,
      string name,
      out string encoding,
      out string mimeType,
      out string fileExtension)
    {
      CachedStream cachedStream;
      bool flag = this.m_secondaryStreams.TryGetValue(name, out cachedStream);
      byte[] bytes = this.StreamToBytes(CachedStream.Extract(cachedStream, out encoding, out mimeType, out fileExtension));
      if (flag && remove)
      {
        this.m_secondaryStreams.Remove(name);
        cachedStream.Dispose();
      }
      return bytes;
    }

    public void MoveSecondaryStreamsTo(StreamCache other)
    {
      foreach (KeyValuePair<string, CachedStream> secondaryStream in this.m_secondaryStreams)
        other.m_secondaryStreams.Add(secondaryStream.Key, secondaryStream.Value);
      this.m_secondaryStreams.Clear();
    }

    private byte[] StreamToBytes(Stream stream)
    {
      if (stream == null)
        return (byte[]) null;
      byte[] buffer = new byte[stream.Length];
      int offset = 0;
      while ((long) offset < stream.Length)
        offset += stream.Read(buffer, offset, (int) stream.Length);
      return buffer;
    }

    private static Stream DefaultCreateStreamDelegate() => (Stream) new MemoryStream();
  }
}
