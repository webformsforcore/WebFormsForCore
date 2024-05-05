// Decompiled with JetBrains decompiler
// Type: System.Net.Configuration.WebRequestModulesSectionInternal
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
  internal sealed class WebRequestModulesSectionInternal
  {
    private static object classSyncObject;
    private ArrayList webRequestModules;

    internal WebRequestModulesSectionInternal(WebRequestModulesSection section)
    {
      if (section.WebRequestModules.Count <= 0)
        return;
      this.webRequestModules = new ArrayList(section.WebRequestModules.Count);
      foreach (WebRequestModuleElement webRequestModule in (ConfigurationElementCollection) section.WebRequestModules)
      {
        try
        {
          this.webRequestModules.Add((object) new WebRequestPrefixElement(webRequestModule.Prefix, webRequestModule.Type));
        }
        catch (Exception ex)
        {
          if (!NclUtilities.IsFatal(ex))
            throw new ConfigurationErrorsException(SR.GetString("net_config_webrequestmodules"), ex);
          throw;
        }
      }
    }

    internal static object ClassSyncObject
    {
      get
      {
        if (WebRequestModulesSectionInternal.classSyncObject == null)
        {
          object obj = new object();
          Interlocked.CompareExchange(ref WebRequestModulesSectionInternal.classSyncObject, obj, (object) null);
        }
        return WebRequestModulesSectionInternal.classSyncObject;
      }
    }

    internal static WebRequestModulesSectionInternal GetSection()
    {
      lock (WebRequestModulesSectionInternal.ClassSyncObject)
        return !(PrivilegedConfigurationManager.GetSection(ConfigurationStrings.WebRequestModulesSectionPath) is WebRequestModulesSection section) ? (WebRequestModulesSectionInternal) null : new WebRequestModulesSectionInternal(section);
    }

    internal ArrayList WebRequestModules => this.webRequestModules ?? new ArrayList(0);
  }
}
