// Decompiled with JetBrains decompiler
// Type: Microsoft.ReportingServices.Rendering.HtmlRenderer.TextRunStyleWriter
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

using Microsoft.ReportingServices.Rendering.RPLProcessing;

#nullable disable
namespace Microsoft.ReportingServices.Rendering.HtmlRenderer
{
  internal class TextRunStyleWriter : ElementStyleWriter
  {
    internal TextRunStyleWriter(HTML4Renderer renderer)
      : base(renderer)
    {
    }

    internal override bool NeedsToWriteNullStyle(StyleWriterMode mode) => false;

    internal override void WriteStyles(StyleWriterMode mode, IRPLStyle style)
    {
      if (style[(byte) 20] is string fontName)
        fontName = HTML4Renderer.HandleSpecialFontCharacters(fontName);
      this.WriteStyle(HTML4Renderer.m_fontFamily, (object) fontName);
      this.WriteStyle(HTML4Renderer.m_fontSize, style[(byte) 21]);
      object val1 = style[(byte) 22];
      if (val1 != null)
      {
        string str = EnumStrings.GetValue((RPLFormat.FontWeights) val1);
        this.WriteStyle(HTML4Renderer.m_fontWeight, (object) str);
      }
      object val2 = style[(byte) 19];
      if (val2 != null)
      {
        string str = EnumStrings.GetValue((RPLFormat.FontStyles) val2);
        this.WriteStyle(HTML4Renderer.m_fontStyle, (object) str);
      }
      object val3 = style[(byte) 24];
      if (val3 != null)
      {
        string str = EnumStrings.GetValue((RPLFormat.TextDecorations) val3);
        this.WriteStyle(HTML4Renderer.m_textDecoration, (object) str);
      }
      this.WriteStyle(HTML4Renderer.m_color, style[(byte) 27]);
    }
  }
}
