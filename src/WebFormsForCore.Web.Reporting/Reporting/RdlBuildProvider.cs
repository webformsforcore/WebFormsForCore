// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.RdlBuildProvider
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.Library;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections;
using System.IO;
using System.Web;
using System.Web.Compilation;
using System.Web.Hosting;

#nullable disable
namespace Microsoft.Reporting
{
  internal sealed class RdlBuildProvider : BuildProvider
  {
    public override CompilerType CodeCompilerType
    {
      get
      {
        this.CompileReport();
        return (CompilerType) null;
      }
    }

    private void CompileReport()
    {
      if (HttpContext.Current != null)
        return;
      byte[] streamBytes = RdlBuildProvider.GetStreamBytes(this.OpenStream());
      HostingEnvironment.MapPath(this.VirtualPath);
      try
      {
        ControlSnapshot controlSnapshot;
        ReportCompiler.CompileReport((ICatalogItemContext) new PreviewItemContext(), streamBytes, false, ref controlSnapshot);
      }
      catch (DefinitionInvalidException ex)
      {
        if (((Exception) ex).InnerException is ReportProcessingException innerException && innerException.ProcessingMessages != null)
        {
          HttpParseException httpParseException = new HttpParseException(((Exception) innerException).Message, (Exception) null, this.VirtualPath, (string) null, 0);
          for (int index = 1; index < ((ArrayList) innerException.ProcessingMessages).Count; ++index)
          {
            ProcessingMessage processingMessage = innerException.ProcessingMessages[index];
            httpParseException.ParserErrors.Add(new ParserError(processingMessage.Message, this.VirtualPath, 0));
          }
          throw httpParseException;
        }
      }
      catch (ReportProcessingException ex)
      {
        throw new HttpParseException(((Exception) ex).Message);
      }
    }

    public static byte[] GetStreamBytes(Stream stream)
    {
      if (stream.CanSeek)
      {
        int length = (int) stream.Length;
        byte[] buffer = new byte[length];
        stream.Read(buffer, 0, length);
        return buffer;
      }
      byte[] streamBytes = new byte[256];
      int offset = 0;
      int num;
      do
      {
        if (streamBytes.Length < offset + 256)
        {
          byte[] destinationArray = new byte[streamBytes.Length * 2];
          Array.Copy((Array) streamBytes, (Array) destinationArray, streamBytes.Length);
          streamBytes = destinationArray;
        }
        num = stream.Read(streamBytes, offset, 256);
        offset += num;
      }
      while (num != 0);
      return streamBytes;
    }
  }
}
