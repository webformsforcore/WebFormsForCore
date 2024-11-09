// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.ReportParameter
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

using System;
using System.Collections.Generic;
using System.Collections.Specialized;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  [Serializable]
  public sealed class ReportParameter
  {
    private string m_name = "";
    private StringCollection m_value = new StringCollection();
    private bool m_visible = true;

    public ReportParameter()
    {
    }

    public ReportParameter(string name)
      : this(name, new string[0])
    {
    }

    public ReportParameter(string name, string value)
      : this(name, new string[1]{ value })
    {
    }

    public ReportParameter(string name, string[] values)
      : this(name, values, true)
    {
    }

    public ReportParameter(string name, string value, bool visible)
      : this(name, new string[1]{ value }, (visible ? 1 : 0) != 0)
    {
    }

    public ReportParameter(string name, string[] values, bool visible)
    {
      this.Name = name;
      if (values == null)
        throw new ArgumentNullException(nameof (values));
      this.Values.AddRange(values);
      this.Visible = visible;
    }

    public string Name
    {
      get => this.m_name;
      set => this.m_name = value;
    }

    public StringCollection Values => this.m_value;

    public bool Visible
    {
      get => this.m_visible;
      set => this.m_visible = value;
    }

    internal static NameValueCollection ToNameValueCollection(
      IEnumerable<ReportParameter> reportParameters)
    {
      if (reportParameters == null)
        throw new ArgumentNullException(nameof (reportParameters));
      NameValueCollection nameValueCollection = new NameValueCollection();
      foreach (ReportParameter reportParameter in reportParameters)
      {
        if (reportParameter == null || reportParameter.Name == null)
          throw new ArgumentNullException(nameof (reportParameters));
        foreach (string str in reportParameter.Values)
          nameValueCollection.Add(reportParameter.Name, str);
      }
      return nameValueCollection;
    }

    internal static ReportParameter[] FromNameValueCollection(NameValueCollection parameterColl)
    {
      if (parameterColl == null)
        return new ReportParameter[0];
      ReportParameter[] reportParameterArray = new ReportParameter[parameterColl.Keys.Count];
      for (int index = 0; index < parameterColl.Keys.Count; ++index)
      {
        ReportParameter reportParameter = new ReportParameter();
        reportParameter.Name = parameterColl.GetKey(index);
        string[] strArray = parameterColl.GetValues(index) ?? new string[1];
        reportParameter.Values.AddRange(strArray);
        reportParameterArray[index] = reportParameter;
      }
      return reportParameterArray;
    }
  }
}
