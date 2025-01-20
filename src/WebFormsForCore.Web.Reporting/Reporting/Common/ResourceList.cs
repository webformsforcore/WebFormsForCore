
using System.Collections.Generic;

#nullable disable
namespace Microsoft.Reporting.Common
{
  internal sealed class ResourceList
  {
    private const string DebugTag = "_Debug";
    public const string MimeTypeImage = "image/gif";
    public const string MimeTypeImagePng = "image/png";
    public const string MimeTypeScript = "application/javascript";
    public const string MimeTypeStyle = "text/css";
    public const string MimeTypeHtml = "text/html";
    private readonly Dictionary<string, ResourceItem> m_items = new Dictionary<string, ResourceItem>();

    internal void Add(string name, string mimeType) => this.Add(name, mimeType, false);

    internal void Add(string name, string mimeType, bool hasDebugMode)
    {
      ResourceItem resourceItem = hasDebugMode ? new ResourceItem(name, name + "_Debug", mimeType) : new ResourceItem(name, mimeType);
      this.m_items.Add(name, resourceItem);
    }

    internal bool TryGetResourceItem(string name, out ResourceItem item)
    {
      return this.m_items.TryGetValue(name, out item);
    }
  }
}
