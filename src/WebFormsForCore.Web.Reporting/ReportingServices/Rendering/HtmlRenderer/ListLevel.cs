// Decompiled with JetBrains decompiler
// Type: Microsoft.ReportingServices.Rendering.HtmlRenderer.ListLevel
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

using Microsoft.ReportingServices.Rendering.RPLProcessing;

#nullable disable
namespace Microsoft.ReportingServices.Rendering.HtmlRenderer
{
  internal class ListLevel
  {
    private int m_listLevel;
    private RPLFormat.ListStyles m_style = (RPLFormat.ListStyles) 2;
    private HTML4Renderer m_renderer;

    public int Level
    {
      get => this.m_listLevel;
      set => this.m_listLevel = value;
    }

    public RPLFormat.ListStyles Style
    {
      get => this.m_style;
      set => this.m_style = value;
    }

    internal ListLevel(HTML4Renderer renderer, int listLevel, RPLFormat.ListStyles style)
    {
      this.m_renderer = renderer;
      this.m_listLevel = listLevel;
      this.m_style = style;
    }

    internal void Open(bool writeNoVerticalMarginClass)
    {
      byte[] theBytes = HTML4Renderer.m_olArabic;
      switch ((int) this.m_style)
      {
        case 1:
          switch (this.m_listLevel % 3)
          {
            case 0:
              theBytes = HTML4Renderer.m_olAlpha;
              break;
            case 2:
              theBytes = HTML4Renderer.m_olRoman;
              break;
          }
          break;
        case 2:
          switch (this.m_listLevel % 3)
          {
            case 0:
              theBytes = HTML4Renderer.m_ulSquare;
              break;
            case 1:
              theBytes = HTML4Renderer.m_ulDisc;
              break;
            case 2:
              theBytes = HTML4Renderer.m_ulCircle;
              break;
          }
          break;
      }
      this.m_renderer.WriteStream(theBytes);
      if (this.m_listLevel == 1 && writeNoVerticalMarginClass)
        this.m_renderer.WriteClassName(HTML4Renderer.m_noVerticalMarginClassName, HTML4Renderer.m_classNoVerticalMargin);
      this.m_renderer.WriteStream(HTML4Renderer.m_closeBracket);
    }

    internal void Close()
    {
      byte[] theBytes = HTML4Renderer.m_closeOL;
      if (this.m_style == 2)
        theBytes = HTML4Renderer.m_closeUL;
      this.m_renderer.WriteStream(theBytes);
    }
  }
}
