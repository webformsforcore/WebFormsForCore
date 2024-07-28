
using System.Configuration.Provider;

#nullable disable
namespace System.Configuration
{
  /// <summary>Represents a collection of application settings providers.</summary>
  public class SettingsProviderCollection : ProviderCollection
  {
    /// <summary>Adds a new settings provider to the collection.</summary>
    /// <param name="provider">A <see cref="T:System.Configuration.Provider.ProviderBase" /> to add to the collection.</param>
    /// <exception cref="T:System.ArgumentException">The <paramref name="provider" /> parameter is not of type <see cref="T:System.Configuration.SettingsProvider" />.
    /// -or-
    /// The <see cref="P:System.Configuration.Provider.ProviderBase.Name" /> property of the provider parameter is null or an empty string.
    /// -or-
    /// A settings provider with the same <see cref="P:System.Configuration.Provider.ProviderBase.Name" /> already exists in the collection.</exception>
    /// <exception cref="T:System.NotSupportedException">The collection is read-only.</exception>
    /// <exception cref="T:System.ArgumentNullException">The <paramref name="provider" /> parameter is <see langword="null" />.</exception>
    public override void Add(ProviderBase provider)
    {
      if (provider == null)
        throw new ArgumentNullException(nameof (provider));
      if (!(provider is SettingsProvider))
        throw new ArgumentException(SR.GetString("Config_provider_must_implement_type", (object) typeof (SettingsProvider).ToString()), nameof (provider));
      base.Add(provider);
    }

    /// <summary>Gets the settings provider in the collection that matches the specified name.</summary>
    /// <param name="name">A <see cref="T:System.String" /> containing the friendly name of the settings provider.</param>
    /// <returns>If found, the <see cref="T:System.Configuration.SettingsProvider" /> whose name matches that specified by the name parameter; otherwise, <see langword="null" />.</returns>
    /// <exception cref="T:System.ArgumentNullException">The <paramref name="name" /> parameter is <see langword="null" />.</exception>
    /// <exception cref="T:System.NotSupportedException">The collection is read-only when setting this value.</exception>
    public SettingsProvider this[string name] => (SettingsProvider) base[name];
  }
}
