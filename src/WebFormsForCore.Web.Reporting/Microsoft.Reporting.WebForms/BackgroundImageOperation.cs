using System;
using System.Collections.Specialized;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web;

namespace Microsoft.Reporting.WebForms;

internal sealed class BackgroundImageOperation : EmbeddedResourceOperation
{
	private const string UrlParamColor = "Color";

	public static string CreateUrl(string resourceName, Color color)
	{
		return EmbeddedResourceOperation.CreateUrl(resourceName, "BackImage", "Color=" + HttpUtility.UrlEncode(ColorTranslator.ToHtml(color)));
	}

	protected override byte[] GetResource(string resourceName, out string mimeType, NameValueCollection urlQuery)
	{
		byte[] array = base.GetResource(resourceName, out mimeType, urlQuery);
		if (array != null)
		{
			Color empty = Color.Empty;
			try
			{
				empty = ColorTranslator.FromHtml(HandlerOperation.GetAndEnsureParam(urlQuery, "Color"));
			}
			catch (ArgumentException e)
			{
				throw new HttpHandlerInputException(e);
			}
			using MemoryStream stream = new MemoryStream(array);
			using Bitmap bitmap = new Bitmap(stream);
			using Bitmap bitmap2 = new Bitmap(bitmap.Width, bitmap.Height);
			using Graphics graphics = Graphics.FromImage(bitmap2);
			float num = empty.GetBrightness() / 14f;
			ColorMatrix colorMatrix = MultiplyMatrix(ColorScaleMatrix((float)(int)empty.R / 255f + num, (float)(int)empty.G / 255f + num, (float)(int)empty.B / 255f + num), GrayScaleMatrix());
			ImageAttributes imageAttributes = new ImageAttributes();
			imageAttributes.SetColorMatrix(colorMatrix);
			graphics.Clear(Color.White);
			Rectangle destRect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
			graphics.DrawImage(bitmap, destRect, 0, 0, destRect.Width, destRect.Height, GraphicsUnit.Pixel, imageAttributes);
			using MemoryStream memoryStream = new MemoryStream();
			bitmap2.Save(memoryStream, ImageFormat.Jpeg);
			array = memoryStream.ToArray();
			mimeType = "image/jpeg";
		}
		return array;
	}

	private static ColorMatrix MultiplyMatrix(ColorMatrix a, ColorMatrix b)
	{
		ColorMatrix colorMatrix = new ColorMatrix();
		for (int i = 0; i <= 4; i++)
		{
			for (int j = 0; j <= 4; j++)
			{
				colorMatrix[i, j] = b[i, 0] * a[0, j] + b[i, 1] * a[1, j] + b[i, 2] * a[2, j] + b[i, 3] * a[3, j] + b[i, 4] * a[4, j];
			}
		}
		return colorMatrix;
	}

	private static ColorMatrix GrayScaleMatrix()
	{
		float num = 0.213f;
		float num2 = 0.715f;
		float num3 = 0.072f;
		return new ColorMatrix(new float[5][]
		{
			new float[5] { num, num, num, 0f, 0f },
			new float[5] { num2, num2, num2, 0f, 0f },
			new float[5] { num3, num3, num3, 0f, 0f },
			new float[5] { 0f, 0f, 0f, 1f, 0f },
			new float[5] { 0f, 0f, 0f, 0f, 1f }
		});
	}

	private static ColorMatrix ColorScaleMatrix(float r, float g, float b)
	{
		return new ColorMatrix(new float[5][]
		{
			new float[5] { r, 0f, 0f, 0f, 0f },
			new float[5] { 0f, g, 0f, 0f, 0f },
			new float[5] { 0f, 0f, b, 0f, 0f },
			new float[5] { 0f, 0f, 0f, 1f, 0f },
			new float[5] { 0f, 0f, 0f, 0f, 1f }
		});
	}
}
