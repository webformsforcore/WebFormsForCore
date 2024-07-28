
#nullable disable
namespace System.Configuration
{
  /// <summary>Represents the method that will handle the <see cref="E:System.Configuration.ApplicationSettingsBase.SettingsLoaded" /> event.</summary>
  /// <param name="sender">The source of the event, typically the settings class.</param>
  /// <param name="e">A <see cref="T:System.Configuration.SettingsLoadedEventArgs" /> object that contains the event data.</param>
  public delegate void SettingsLoadedEventHandler(object sender, SettingsLoadedEventArgs e);
}
