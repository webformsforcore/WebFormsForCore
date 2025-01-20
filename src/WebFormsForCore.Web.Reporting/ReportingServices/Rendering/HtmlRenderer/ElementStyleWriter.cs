
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
