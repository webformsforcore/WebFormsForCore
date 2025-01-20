
using Microsoft.Reporting.Common;
using System.Collections.Generic;
using System.IO;
using System.Text;

#nullable disable
namespace Microsoft.ReportingServices.Rendering.HtmlRenderer
{
  internal static class HTMLRendererResources
  {
    public const string ResourceNamespace = "Microsoft.ReportingServices.Rendering.HtmlRenderer.RendererResources.";
    public const string TogglePlus = "TogglePlus.gif";
    public const string ToggleMinus = "ToggleMinus.gif";
    public const string SortAsc = "sortAsc.gif";
    public const string SortDesc = "sortDesc.gif";
    public const string SortNone = "unsorted.gif";
    public const string Blank = "Blank.gif";
    public const string CommonScript = "Common.js";
    public const string FitProportionalScript = "FitProportional.js";
    public const string FixedHeaderScript = "FixedHeader.js";
    public const string CanGrowFalseScript = "CanGrowFalse.js";
    public const string ImageConsolidationScript = "ImageConsolidation.js";
    private static ResourceList m_resourceList = new ResourceList();

    static HTMLRendererResources()
    {
      HTMLRendererResources.m_resourceList.Add("Microsoft.ReportingServices.Rendering.HtmlRenderer.RendererResources.TogglePlus.gif", "image/gif");
      HTMLRendererResources.m_resourceList.Add("Microsoft.ReportingServices.Rendering.HtmlRenderer.RendererResources.ToggleMinus.gif", "image/gif");
      HTMLRendererResources.m_resourceList.Add("Microsoft.ReportingServices.Rendering.HtmlRenderer.RendererResources.sortAsc.gif", "image/gif");
      HTMLRendererResources.m_resourceList.Add("Microsoft.ReportingServices.Rendering.HtmlRenderer.RendererResources.sortDesc.gif", "image/gif");
      HTMLRendererResources.m_resourceList.Add("Microsoft.ReportingServices.Rendering.HtmlRenderer.RendererResources.unsorted.gif", "image/gif");
      HTMLRendererResources.m_resourceList.Add("Microsoft.ReportingServices.Rendering.HtmlRenderer.RendererResources.Blank.gif", "image/gif");
      HTMLRendererResources.m_resourceList.Add("Microsoft.ReportingServices.Rendering.HtmlRenderer.RendererResources.Common.js", "application/javascript", true);
      HTMLRendererResources.m_resourceList.Add("Microsoft.ReportingServices.Rendering.HtmlRenderer.RendererResources.FitProportional.js", "application/javascript", true);
      HTMLRendererResources.m_resourceList.Add("Microsoft.ReportingServices.Rendering.HtmlRenderer.RendererResources.FixedHeader.js", "application/javascript", true);
      HTMLRendererResources.m_resourceList.Add("Microsoft.ReportingServices.Rendering.HtmlRenderer.RendererResources.CanGrowFalse.js", "application/javascript", true);
      HTMLRendererResources.m_resourceList.Add("Microsoft.ReportingServices.Rendering.HtmlRenderer.RendererResources.ImageConsolidation.js", "application/javascript", true);
    }

    public static void PopulateResources(
      Dictionary<string, byte[]> nameToResourceMap,
      string prefix)
    {
      Encoding utF8 = Encoding.UTF8;
      nameToResourceMap["TogglePlus.gif"] = HTMLRendererResources.CreateFullName(utF8, prefix, "TogglePlus.gif");
      nameToResourceMap["ToggleMinus.gif"] = HTMLRendererResources.CreateFullName(utF8, prefix, "ToggleMinus.gif");
      nameToResourceMap["sortAsc.gif"] = HTMLRendererResources.CreateFullName(utF8, prefix, "sortAsc.gif");
      nameToResourceMap["sortDesc.gif"] = HTMLRendererResources.CreateFullName(utF8, prefix, "sortDesc.gif");
      nameToResourceMap["unsorted.gif"] = HTMLRendererResources.CreateFullName(utF8, prefix, "unsorted.gif");
      nameToResourceMap["Blank.gif"] = HTMLRendererResources.CreateFullName(utF8, prefix, "Blank.gif");
    }

    public static Stream GetStream(string name, out string mimeType)
    {
      return EmbeddedResources.GetStream(HTMLRendererResources.m_resourceList, "Microsoft.ReportingServices.Rendering.HtmlRenderer.RendererResources." + name, out mimeType);
    }

    private static byte[] CreateFullName(Encoding encoding, string prefix, string name)
    {
      return encoding.GetBytes(prefix + name);
    }

    public static byte[] GetBytes(string name)
    {
      string mimeType = (string) null;
      return HTMLRendererResources.GetBytes(name, out mimeType);
    }

    public static byte[] GetBytes(string name, out string mimeType)
    {
      return EmbeddedResources.Get(HTMLRendererResources.m_resourceList, "Microsoft.ReportingServices.Rendering.HtmlRenderer.RendererResources." + name, out mimeType);
    }

    public static byte[] GetBytesFullname(string nameWithNamespace, out string mimeType)
    {
      return EmbeddedResources.Get(HTMLRendererResources.m_resourceList, nameWithNamespace, out mimeType);
    }
  }
}
