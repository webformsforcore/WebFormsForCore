
using System;
using System.Diagnostics;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  [Serializable]
  internal sealed class ScrollTarget
  {
    private string m_navigationId;
    private string m_pixelPosition;
    private ActionScrollStyle m_scrollStyle;

    public ScrollTarget(string navigationId, ActionScrollStyle scrollStyle)
    {
      this.m_navigationId = navigationId;
      this.m_scrollStyle = scrollStyle;
    }

    public ScrollTarget(string pixelPosition)
    {
      this.m_pixelPosition = pixelPosition;
      this.m_scrollStyle = ActionScrollStyle.SpecificPosition;
    }

    public string NavigationId
    {
      [DebuggerStepThrough] get => this.m_navigationId;
    }

    public string PixelPosition
    {
      [DebuggerStepThrough] get => this.m_pixelPosition;
    }

    public ActionScrollStyle ScrollStyle
    {
      [DebuggerStepThrough] get => this.m_scrollStyle;
    }
  }
}
