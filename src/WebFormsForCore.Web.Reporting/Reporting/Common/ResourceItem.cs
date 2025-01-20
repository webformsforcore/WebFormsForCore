
using System.Web;

#nullable disable
namespace Microsoft.Reporting.Common
{
  internal sealed class ResourceItem
  {
    private readonly string m_name;
    private readonly string m_debugName;
    private readonly string m_mimeType;

    internal ResourceItem(string name, string debugName, string mimeType)
    {
      this.m_name = name;
      this.m_debugName = debugName;
      this.m_mimeType = mimeType;
    }

    internal ResourceItem(string name, string mimeType)
      : this(name, name, mimeType)
    {
    }

    internal string EffectiveName
    {
      get => !ResourceItem.IsDebuggingEnabled ? this.m_name : this.m_debugName;
    }

    private static bool IsDebuggingEnabled
    {
      get
      {
        HttpContext current = HttpContext.Current;
        return current != null && current.IsDebuggingEnabled;
      }
    }

    internal string MimeType => this.m_mimeType;
  }
}
