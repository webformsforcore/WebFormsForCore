// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.DataSourceCredentialsCollection
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

using System;
using System.Collections.ObjectModel;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  public sealed class DataSourceCredentialsCollection : Collection<DataSourceCredentials>
  {
    public DataSourceCredentials this[string name]
    {
      get
      {
        foreach (DataSourceCredentials sourceCredentials in (Collection<DataSourceCredentials>) this)
        {
          if (string.Compare(sourceCredentials.Name, name, StringComparison.OrdinalIgnoreCase) == 0)
            return sourceCredentials;
        }
        return (DataSourceCredentials) null;
      }
    }
  }
}
