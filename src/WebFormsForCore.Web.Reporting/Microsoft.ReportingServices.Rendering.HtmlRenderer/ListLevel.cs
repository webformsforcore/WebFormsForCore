using Microsoft.ReportingServices.Rendering.RPLProcessing;

namespace Microsoft.ReportingServices.Rendering.HtmlRenderer;

internal class ListLevel
{
	private int m_listLevel;

	private ListStyles m_style = (ListStyles)2;

	private HTML4Renderer m_renderer;

	public int Level
	{
		get
		{
			return m_listLevel;
		}
		set
		{
			m_listLevel = value;
		}
	}

	public ListStyles Style
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return m_style;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			m_style = value;
		}
	}

	internal ListLevel(HTML4Renderer renderer, int listLevel, ListStyles style)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		m_renderer = renderer;
		m_listLevel = listLevel;
		m_style = style;
	}

	internal void Open(bool writeNoVerticalMarginClass)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Expected I4, but got Unknown
		byte[] theBytes = HTML4Renderer.m_olArabic;
		ListStyles style = m_style;
		switch ((int)style)
		{
		case 1:
			switch (m_listLevel % 3)
			{
			case 2:
				theBytes = HTML4Renderer.m_olRoman;
				break;
			case 0:
				theBytes = HTML4Renderer.m_olAlpha;
				break;
			}
			break;
		case 2:
			switch (m_listLevel % 3)
			{
			case 1:
				theBytes = HTML4Renderer.m_ulDisc;
				break;
			case 2:
				theBytes = HTML4Renderer.m_ulCircle;
				break;
			case 0:
				theBytes = HTML4Renderer.m_ulSquare;
				break;
			}
			break;
		}
		m_renderer.WriteStream(theBytes);
		if (m_listLevel == 1 && writeNoVerticalMarginClass)
		{
			m_renderer.WriteClassName(HTML4Renderer.m_noVerticalMarginClassName, HTML4Renderer.m_classNoVerticalMargin);
		}
		m_renderer.WriteStream(HTML4Renderer.m_closeBracket);
	}

	internal void Close()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Invalid comparison between Unknown and I4
		byte[] theBytes = HTML4Renderer.m_closeOL;
		if ((int)m_style == 2)
		{
			theBytes = HTML4Renderer.m_closeUL;
		}
		m_renderer.WriteStream(theBytes);
	}
}
