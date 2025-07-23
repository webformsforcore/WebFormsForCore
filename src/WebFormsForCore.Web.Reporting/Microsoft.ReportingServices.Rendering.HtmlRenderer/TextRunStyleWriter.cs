using Microsoft.ReportingServices.Rendering.RPLProcessing;

namespace Microsoft.ReportingServices.Rendering.HtmlRenderer;

internal class TextRunStyleWriter : ElementStyleWriter
{
	internal TextRunStyleWriter(HTML4Renderer renderer)
		: base(renderer)
	{
	}

	internal override bool NeedsToWriteNullStyle(StyleWriterMode mode)
	{
		return false;
	}

	internal override void WriteStyles(StyleWriterMode mode, IRPLStyle style)
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		string text = style[(byte)20] as string;
		if (text != null)
		{
			text = HTML4Renderer.HandleSpecialFontCharacters(text);
		}
		WriteStyle(HTML4Renderer.m_fontFamily, text);
		WriteStyle(HTML4Renderer.m_fontSize, style[(byte)21]);
		object obj = style[(byte)22];
		string text2 = null;
		if (obj != null)
		{
			text2 = EnumStrings.GetValue((FontWeights)obj);
			WriteStyle(HTML4Renderer.m_fontWeight, text2);
		}
		obj = style[(byte)19];
		if (obj != null)
		{
			text2 = EnumStrings.GetValue((FontStyles)obj);
			WriteStyle(HTML4Renderer.m_fontStyle, text2);
		}
		obj = style[(byte)24];
		if (obj != null)
		{
			text2 = EnumStrings.GetValue((TextDecorations)obj);
			WriteStyle(HTML4Renderer.m_textDecoration, text2);
		}
		WriteStyle(HTML4Renderer.m_color, style[(byte)27]);
	}
}
