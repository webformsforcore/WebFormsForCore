
using System;
using System.Collections.Specialized;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal sealed class BackgroundImageOperation : EmbeddedResourceOperation
  {
    private const string UrlParamColor = "Color";

    public static string CreateUrl(string resourceName, Color color)
    {
      return EmbeddedResourceOperation.CreateUrl(resourceName, "BackImage", "Color=" + HttpUtility.UrlEncode(ColorTranslator.ToHtml(color)));
    }

    protected override byte[] GetResource(
      string resourceName,
      out string mimeType,
      NameValueCollection urlQuery)
    {
      byte[] buffer = base.GetResource(resourceName, out mimeType, urlQuery);
      if (buffer != null)
      {
        Color empty = Color.Empty;
        Color color;
        try
        {
          color = ColorTranslator.FromHtml(HandlerOperation.GetAndEnsureParam(urlQuery, "Color"));
        }
        catch (ArgumentException ex)
        {
          throw new HttpHandlerInputException((Exception) ex);
        }
        using (MemoryStream memoryStream1 = new MemoryStream(buffer))
        {
          using (Bitmap bitmap1 = new Bitmap((Stream) memoryStream1))
          {
            using (Bitmap bitmap2 = new Bitmap(bitmap1.Width, bitmap1.Height))
            {
              using (Graphics graphics = Graphics.FromImage((Image) bitmap2))
              {
                float num = color.GetBrightness() / 14f;
                ColorMatrix newColorMatrix = BackgroundImageOperation.MultiplyMatrix(BackgroundImageOperation.ColorScaleMatrix((float) color.R / (float) byte.MaxValue + num, (float) color.G / (float) byte.MaxValue + num, (float) color.B / (float) byte.MaxValue + num), BackgroundImageOperation.GrayScaleMatrix());
                ImageAttributes imageAttr = new ImageAttributes();
                imageAttr.SetColorMatrix(newColorMatrix);
                graphics.Clear(Color.White);
                Rectangle destRect = new Rectangle(0, 0, bitmap1.Width, bitmap1.Height);
                graphics.DrawImage((Image) bitmap1, destRect, 0, 0, destRect.Width, destRect.Height, GraphicsUnit.Pixel, imageAttr);
                using (MemoryStream memoryStream2 = new MemoryStream())
                {
                  bitmap2.Save((Stream) memoryStream2, ImageFormat.Jpeg);
                  buffer = memoryStream2.ToArray();
                  mimeType = "image/jpeg";
                }
              }
            }
          }
        }
      }
      return buffer;
    }

    private static ColorMatrix MultiplyMatrix(ColorMatrix a, ColorMatrix b)
    {
      ColorMatrix colorMatrix = new ColorMatrix();
      for (int row = 0; row <= 4; ++row)
      {
        for (int column = 0; column <= 4; ++column)
          colorMatrix[row, column] = (float) ((double) b[row, 0] * (double) a[0, column] + (double) b[row, 1] * (double) a[1, column] + (double) b[row, 2] * (double) a[2, column] + (double) b[row, 3] * (double) a[3, column] + (double) b[row, 4] * (double) a[4, column]);
      }
      return colorMatrix;
    }

    private static ColorMatrix GrayScaleMatrix()
    {
      float num1 = 0.213f;
      float num2 = 0.715f;
      float num3 = 0.072f;
      return new ColorMatrix(new float[5][]
      {
        new float[5]{ num1, num1, num1, 0.0f, 0.0f },
        new float[5]{ num2, num2, num2, 0.0f, 0.0f },
        new float[5]{ num3, num3, num3, 0.0f, 0.0f },
        new float[5]{ 0.0f, 0.0f, 0.0f, 1f, 0.0f },
        new float[5]{ 0.0f, 0.0f, 0.0f, 0.0f, 1f }
      });
    }

    private static ColorMatrix ColorScaleMatrix(float r, float g, float b)
    {
      return new ColorMatrix(new float[5][]
      {
        new float[5]{ r, 0.0f, 0.0f, 0.0f, 0.0f },
        new float[5]{ 0.0f, g, 0.0f, 0.0f, 0.0f },
        new float[5]{ 0.0f, 0.0f, b, 0.0f, 0.0f },
        new float[5]{ 0.0f, 0.0f, 0.0f, 1f, 0.0f },
        new float[5]{ 0.0f, 0.0f, 0.0f, 0.0f, 1f }
      });
    }
  }
}
