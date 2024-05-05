// Decompiled with JetBrains decompiler
// Type: System.Configuration.Internal.ConfigurationManagerHelper
// Assembly: System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 84F5A18A-F2B1-435C-B86E-09CE162E61E4
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System\v4.0_4.0.0.0__b77a5c561934e089\System.dll
// XML documentation location: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1\System.xml

using System.Net.Configuration;

#nullable disable
namespace System.Configuration.Internal
{
  internal sealed class ConfigurationManagerHelper : IConfigurationManagerHelper
  {
    private ConfigurationManagerHelper()
    {
    }

    void IConfigurationManagerHelper.EnsureNetConfigLoaded()
    {
      SettingsSection.EnsureConfigLoaded();
    }
  }
}
