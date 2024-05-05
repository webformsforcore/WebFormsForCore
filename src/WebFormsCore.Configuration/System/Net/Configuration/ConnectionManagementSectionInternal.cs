// Decompiled with JetBrains decompiler
// Type: System.Net.Configuration.ConnectionManagementSectionInternal
// Assembly: System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 84F5A18A-F2B1-435C-B86E-09CE162E61E4
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System\v4.0_4.0.0.0__b77a5c561934e089\System.dll
// XML documentation location: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1\System.xml

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
