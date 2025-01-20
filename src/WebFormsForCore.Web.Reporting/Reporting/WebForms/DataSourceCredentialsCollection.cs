
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
