
using System.Runtime.InteropServices;

#nullable disable
namespace System.Configuration
{
  /// <summary>Provides standard configuration methods.</summary>
  [ComVisible(false)]
  public interface IConfigurationSystem
  {
    /// <summary>Gets the specified configuration.</summary>
    /// <param name="configKey">The configuration key.</param>
    /// <returns>The object representing the configuration.</returns>
    object GetConfig(string configKey);

    /// <summary>Used for initialization.</summary>
    void Init();
  }
}
