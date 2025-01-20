
using System.Globalization;
using System.Runtime.CompilerServices;

#nullable disable
namespace Microsoft.ReportingServices.Rendering.HtmlRenderer
{
  [CompilerGenerated]
  internal class RenderRes
  {
    protected RenderRes()
    {
    }

    public static CultureInfo Culture
    {
      get => RenderRes.Keys.Culture;
      set => RenderRes.Keys.Culture = value;
    }

    public static string HideDocMapTooltip => RenderRes.Keys.GetString(nameof (HideDocMapTooltip));

    public static string DocumentMap => RenderRes.Keys.GetString(nameof (DocumentMap));

    public static string DefaultDocMapLabel
    {
      get => RenderRes.Keys.GetString(nameof (DefaultDocMapLabel));
    }

    public static string HTML40LocalizedName
    {
      get => RenderRes.Keys.GetString(nameof (HTML40LocalizedName));
    }

    public static string MHTMLLocalizedName
    {
      get => RenderRes.Keys.GetString(nameof (MHTMLLocalizedName));
    }

    public static string rrInvalidSectionError
    {
      get => RenderRes.Keys.GetString(nameof (rrInvalidSectionError));
    }

    public static string rrInvalidDeviceInfo
    {
      get => RenderRes.Keys.GetString(nameof (rrInvalidDeviceInfo));
    }

    public static string ToggleStateCollapse
    {
      get => RenderRes.Keys.GetString(nameof (ToggleStateCollapse));
    }

    public static string ToggleStateExpand => RenderRes.Keys.GetString(nameof (ToggleStateExpand));
  }
}
