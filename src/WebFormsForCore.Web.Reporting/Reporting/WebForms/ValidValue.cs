// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.ValidValue
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  public sealed class ValidValue
  {
    private string m_label;
    private string m_value;

    internal ValidValue(string label, string value)
    {
      this.m_label = label;
      this.m_value = value;
    }

    public string Label => this.m_label;

    public string Value => this.m_value;
  }
}
