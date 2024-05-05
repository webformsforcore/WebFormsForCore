// Decompiled with JetBrains decompiler
// Type: System.Configuration.ConfigurationManagerInternalFactory
// Assembly: System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 84F5A18A-F2B1-435C-B86E-09CE162E61E4
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System\v4.0_4.0.0.0__b77a5c561934e089\System.dll
// XML documentation location: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1\System.xml

using System.Configuration.Internal;

#nullable disable
namespace System.Configuration
{
  internal static class ConfigurationManagerInternalFactory
  {
    private const string ConfigurationManagerInternalTypeString = "System.Configuration.Internal.ConfigurationManagerInternal, System.Configuration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";
    private static volatile IConfigurationManagerInternal s_instance;

    internal static IConfigurationManagerInternal Instance
    {
      get
      {
        if (ConfigurationManagerInternalFactory.s_instance == null)
          ConfigurationManagerInternalFactory.s_instance = (IConfigurationManagerInternal) TypeUtil.CreateInstanceWithReflectionPermission("System.Configuration.Internal.ConfigurationManagerInternal, System.Configuration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a");
        return ConfigurationManagerInternalFactory.s_instance;
      }
    }
  }
}
