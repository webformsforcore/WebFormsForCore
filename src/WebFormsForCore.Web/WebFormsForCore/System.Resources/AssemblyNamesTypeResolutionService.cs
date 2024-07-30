#if !NETFRAMEWORK

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Reflection;
using System.Web;

#nullable disable
namespace System.Resources
{
  internal class AssemblyNamesTypeResolutionService : ITypeResolutionService
  {
    private AssemblyName[] names;
    private Hashtable cachedAssemblies;
    private Hashtable cachedTypes;
    private static string NetFrameworkPath = Path.Combine(Environment.GetEnvironmentVariable("SystemRoot"), "Microsoft.Net\\Framework");

    internal AssemblyNamesTypeResolutionService(AssemblyName[] names) => this.names = names;

    public Assembly GetAssembly(AssemblyName name) => this.GetAssembly(name, true);

    public Assembly GetAssembly(AssemblyName name, bool throwOnError)
    {
      Assembly assembly = (Assembly) null;
      if (this.cachedAssemblies == null)
        this.cachedAssemblies = Hashtable.Synchronized(new Hashtable());
      if (this.cachedAssemblies.Contains((object) name))
        assembly = this.cachedAssemblies[(object) name] as Assembly;
      if (assembly == (Assembly) null)
      {
        assembly = Assembly.LoadWithPartialName(name.FullName);
        if (assembly != (Assembly) null)
          this.cachedAssemblies[(object) name] = (object) assembly;
        else if (this.names != null)
        {
          for (int index = 0; index < this.names.Length; ++index)
          {
            if (name.Equals((object) this.names[index]))
            {
              try
              {
                assembly = Assembly.LoadFrom(this.GetPathOfAssembly(name));
                if (assembly != (Assembly) null)
                  this.cachedAssemblies[(object) name] = (object) assembly;
              }
              catch
              {
                if (throwOnError)
                  throw;
              }
            }
          }
        }
      }
      return assembly;
    }

    public string GetPathOfAssembly(AssemblyName name) => name.CodeBase;

    public System.Type GetType(string name) => this.GetType(name, true);

    public System.Type GetType(string name, bool throwOnError)
    {
      return this.GetType(name, throwOnError, false);
    }

    public System.Type GetType(string name, bool throwOnError, bool ignoreCase)
    {
      System.Type type = (System.Type) null;
      if (this.cachedTypes == null)
        this.cachedTypes = Hashtable.Synchronized(new Hashtable((IEqualityComparer) StringComparer.Ordinal));
      if (this.cachedTypes.Contains((object) name))
        return this.cachedTypes[(object) name] as System.Type;
      if (name.IndexOf(',') != -1)
        type = System.Type.GetType(name, false, ignoreCase);
      if (type == (System.Type) null && this.names != null)
      {
        int num = name.IndexOf(',');
        if (num > 0 && num < name.Length - 1)
        {
          string assemblyName1 = name.Substring(num + 1).Trim();
          AssemblyName assemblyName2 = (AssemblyName) null;
          try
          {
            assemblyName2 = new AssemblyName(assemblyName1);
          }
          catch
          {
          }
          if (assemblyName2 != null)
          {
            List<AssemblyName> assemblyNameList = new List<AssemblyName>(this.names.Length);
            for (int index = 0; index < this.names.Length; ++index)
            {
              if (string.Compare(assemblyName2.Name, this.names[index].Name, StringComparison.OrdinalIgnoreCase) == 0)
                assemblyNameList.Insert(0, this.names[index]);
              else
                assemblyNameList.Add(this.names[index]);
            }
            this.names = assemblyNameList.ToArray();
          }
        }
        for (int index = 0; index < this.names.Length; ++index)
        {
          Assembly assembly = this.GetAssembly(this.names[index], false);
          if (assembly != (Assembly) null)
          {
            type = assembly.GetType(name, false, ignoreCase);
            if (type == (System.Type) null)
            {
              int length = name.IndexOf(",");
              if (length != -1)
              {
                string name1 = name.Substring(0, length);
                type = assembly.GetType(name1, false, ignoreCase);
              }
            }
          }
          if (type != (System.Type) null)
            break;
        }
      }
      if (type == (System.Type) null & throwOnError)
        throw new ArgumentException(SR.GetString(SR.Invalid_ResX_No_Type));
      if (type != (System.Type) null && (type.Assembly.GlobalAssemblyCache || this.IsNetFrameworkAssembly(type.Assembly.Location)))
        this.cachedTypes[(object) name] = (object) type;
      return type;
    }

    private bool IsNetFrameworkAssembly(string assemblyPath)
    {
      return assemblyPath != null && assemblyPath.StartsWith(AssemblyNamesTypeResolutionService.NetFrameworkPath, StringComparison.OrdinalIgnoreCase);
    }

    public void ReferenceAssembly(AssemblyName name) => throw new NotSupportedException();
  }
}
#endif