// Decompiled with JetBrains decompiler
// Type: Microsoft.ReportingServices.Rendering.HtmlRenderer.ElementStyleWriter
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

using Microsoft.ReportingServices.Rendering.RPLProcessing;

#nullable disable
namespace Microsoft.ReportingServices.Rendering.HtmlRenderer
{
  internal abstract class ElementStyleWriter
  {
    protected HTML4Renderer m_renderer;

    internal ElementStyleWriter(HTML4Renderer renderer) => this.m_renderer = renderer;

    internal abstract bool NeedsToWriteNullStyle(StyleWriterMode mode);

    internal abstract void WriteStyles(StyleWriterMode mode, IRPLStyle style);

    protected void WriteStream(string s) => this.m_renderer.WriteStream(s);

    protected void WriteStream(byte[] value) => this.m_renderer.WriteStream(value);

    protected void WriteStyle(byte[] text, object value)
    {
      if (value == null)
        return;
      this.WriteStream(text);
      this.WriteStream(value.ToString());
      this.WriteStream(HTML4Renderer.m_semiColon);
    }

    protected void WriteStyle(byte[] text, object nonShared, object shared)
    {
      object obj = nonShared ?? shared;
      this.WriteStyle(text, obj);
    }
  }
}
