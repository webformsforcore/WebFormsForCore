using System;
using System.Diagnostics;

namespace Microsoft.Reporting.WebForms;

[Serializable]
internal sealed class ScrollTarget
{
	private string m_navigationId;

	private string m_pixelPosition;

	private ActionScrollStyle m_scrollStyle;

	public string NavigationId
	{
		[DebuggerStepThrough]
		get
		{
			return m_navigationId;
		}
	}

	public string PixelPosition
	{
		[DebuggerStepThrough]
		get
		{
			return m_pixelPosition;
		}
	}

	public ActionScrollStyle ScrollStyle
	{
		[DebuggerStepThrough]
		get
		{
			return m_scrollStyle;
		}
	}

	public ScrollTarget(string navigationId, ActionScrollStyle scrollStyle)
	{
		m_navigationId = navigationId;
		m_scrollStyle = scrollStyle;
	}

	public ScrollTarget(string pixelPosition)
	{
		m_pixelPosition = pixelPosition;
		m_scrollStyle = ActionScrollStyle.SpecificPosition;
	}
}
