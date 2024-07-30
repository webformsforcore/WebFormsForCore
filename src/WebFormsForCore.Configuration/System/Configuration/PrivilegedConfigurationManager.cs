#if !WebFormsForCore

using System.Security.Permissions;

#nullable disable
namespace System.Configuration
{
  [ConfigurationPermission(SecurityAction.Assert, Unrestricted = true)]
  internal static class PrivilegedConfigurationManager
  {
    internal static ConnectionStringSettingsCollection ConnectionStrings
    {
      get => ConfigurationManager.ConnectionStrings;
    }

    internal static object GetSection(string sectionName)
    {
      return ConfigurationManager.GetSection(sectionName);
    }
  }
}

#endif