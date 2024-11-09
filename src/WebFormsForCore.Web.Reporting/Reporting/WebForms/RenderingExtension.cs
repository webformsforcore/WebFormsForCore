// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.RenderingExtension
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

using Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution;
using System;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  [Serializable]
  public sealed class RenderingExtension
  {
    private string m_name;
    private string m_localizedName;
    private bool m_isVisible;

    internal RenderingExtension(string name, string localizedName, bool isVisible)
    {
      this.m_name = name;
      this.m_localizedName = localizedName;
      this.m_isVisible = isVisible;
    }

    internal static RenderingExtension[] FromSoapExtensions(Extension[] soapExtensions)
    {
      if (soapExtensions == null)
        return (RenderingExtension[]) null;
      RenderingExtension[] renderingExtensionArray = new RenderingExtension[soapExtensions.Length];
      for (int index = 0; index < soapExtensions.Length; ++index)
        renderingExtensionArray[index] = new RenderingExtension(soapExtensions[index].Name, soapExtensions[index].LocalizedName, soapExtensions[index].Visible);
      return renderingExtensionArray;
    }

    public string Name => this.m_name;

    public string LocalizedName => this.m_localizedName;

    public bool Visible => this.m_isVisible;
  }
}
