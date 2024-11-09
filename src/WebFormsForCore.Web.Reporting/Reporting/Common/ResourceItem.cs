// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.Common.ResourceItem
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

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
