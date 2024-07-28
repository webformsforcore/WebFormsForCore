
#nullable disable
namespace System.Configuration
{
  /// <summary>Provides data for the <see cref="E:System.Configuration.ApplicationSettingsBase.SettingsLoaded" /> event.</summary>
  public class SettingsLoadedEventArgs : EventArgs
  {
    private SettingsProvider _provider;

    /// <summary>Initializes a new instance of the <see cref="T:System.Configuration.SettingsLoadedEventArgs" /> class.</summary>
    /// <param name="provider">A <see cref="T:System.Configuration.SettingsProvider" /> object from which settings are loaded.</param>
    public SettingsLoadedEventArgs(SettingsProvider provider) => this._provider = provider;

    /// <summary>Gets the settings provider used to store configuration settings.</summary>
    /// <returns>A settings provider.</returns>
    public SettingsProvider Provider => this._provider;
  }
}
