// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.DeviceInfo
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

using System;
using System.Runtime.InteropServices;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  [ComVisible(false)]
  [Serializable]
  public sealed class DeviceInfo
  {
    private string m_name;
    private string m_value;

    public DeviceInfo(string name, string value)
    {
      if (name == null)
        throw new ArgumentNullException(nameof (name));
      if (value == null)
        throw new ArgumentNullException(nameof (value));
      this.m_name = name;
      this.m_value = value;
    }

    public string Name => this.m_name;

    public string Value => this.m_value;
  }
}
