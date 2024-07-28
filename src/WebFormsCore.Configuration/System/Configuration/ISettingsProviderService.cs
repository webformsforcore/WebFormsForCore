
#nullable disable
namespace System.Configuration
{
  /// <summary>Provides an interface for defining an alternate application settings provider.</summary>
  public interface ISettingsProviderService
  {
    /// <summary>Returns the settings provider compatible with the specified settings property.</summary>
    /// <param name="property">The <see cref="T:System.Configuration.SettingsProperty" /> that requires serialization.</param>
    /// <returns>If found, the <see cref="T:System.Configuration.SettingsProvider" /> that can persist the specified settings property; otherwise, <see langword="null" />.</returns>
    SettingsProvider GetSettingsProvider(SettingsProperty property);
  }
}
