// Decompiled with JetBrains decompiler
// Type: Microsoft.ReportingServices.Interfaces.CreateAndRegisterStream
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

using System.IO;
using System.Text;

#nullable disable
namespace Microsoft.ReportingServices.Interfaces
{
  internal delegate Stream CreateAndRegisterStream(
    string name,
    string extension,
    Encoding encoding,
    string mimeType,
    bool willSeek,
    StreamOper operation);
}
