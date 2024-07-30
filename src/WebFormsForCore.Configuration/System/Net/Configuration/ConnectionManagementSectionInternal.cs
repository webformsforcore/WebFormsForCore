
using System.Collections;
using System.Configuration;
using System.Threading;

#nullable disable
namespace System.Net.Configuration
{
  internal sealed class ConnectionManagementSectionInternal
  {
    private Hashtable connectionManagement;
    private static object classSyncObject;

    internal ConnectionManagementSectionInternal(ConnectionManagementSection section)
    {
      if (section.ConnectionManagement.Count <= 0)
        return;
      this.connectionManagement = new Hashtable(section.ConnectionManagement.Count);
      foreach (ConnectionManagementElement managementElement in (ConfigurationElementCollection) section.ConnectionManagement)
        this.connectionManagement[(object) managementElement.Address] = (object) managementElement.MaxConnection;
    }

    internal Hashtable ConnectionManagement => this.connectionManagement ?? new Hashtable();

    internal static object ClassSyncObject
    {
      get
      {
        if (ConnectionManagementSectionInternal.classSyncObject == null)
        {
          object obj = new object();
          Interlocked.CompareExchange(ref ConnectionManagementSectionInternal.classSyncObject, obj, (object) null);
        }
        return ConnectionManagementSectionInternal.classSyncObject;
      }
    }

    internal static ConnectionManagementSectionInternal GetSection()
    {
      lock (ConnectionManagementSectionInternal.ClassSyncObject)
        return !(PrivilegedConfigurationManager.GetSection(ConfigurationStrings.ConnectionManagementSectionPath) is ConnectionManagementSection section) ? (ConnectionManagementSectionInternal) null : new ConnectionManagementSectionInternal(section);
    }
  }
}
