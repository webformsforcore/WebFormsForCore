
using System.IO;
using System.Reflection;

#nullable disable
namespace Microsoft.Reporting.Common
{
  internal static class EmbeddedResources
  {
    public static byte[] Get(ResourceList list, string name, out string mimeType)
    {
      Stream stream = EmbeddedResources.GetStream(list, name, out mimeType);
      if (stream == null)
        return (byte[]) null;
      using (stream)
      {
        int length = (int) stream.Length;
        byte[] buffer = new byte[length];
        stream.Read(buffer, 0, length);
        return buffer;
      }
    }

    public static Stream GetStream(ResourceList list, string name, out string mimeType)
    {
      ResourceItem resourceItem;
      if (list.TryGetResourceItem(name, out resourceItem))
      {
        mimeType = resourceItem.MimeType;
        return Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceItem.EffectiveName);
      }
      mimeType = (string) null;
      return (Stream) null;
    }
  }
}
