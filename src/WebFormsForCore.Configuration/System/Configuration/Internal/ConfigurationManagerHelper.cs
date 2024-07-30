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
