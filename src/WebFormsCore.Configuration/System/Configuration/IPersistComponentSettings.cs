
#nullable disable
namespace System.Configuration
{
  /// <summary>Defines standard functionality for controls or libraries that store and retrieve application settings.</summary>
  public interface IPersistComponentSettings
  {
    /// <summary>Gets or sets a value indicating whether the control should automatically persist its application settings properties.</summary>
    /// <returns>
    /// <see langword="true" /> if the control should automatically persist its state; otherwise, <see langword="false" />.</returns>
    bool SaveSettings { get; set; }

    /// <summary>Gets or sets the value of the application settings key for the current instance of the control.</summary>
    /// <returns>A <see cref="T:System.String" /> containing the settings key for the current instance of the control.</returns>
    string SettingsKey { get; set; }

    /// <summary>Reads the control's application settings into their corresponding properties and updates the control's state.</summary>
    void LoadComponentSettings();

    /// <summary>Persists the control's application settings properties.</summary>
    void SaveComponentSettings();

    /// <summary>Resets the control's application settings properties to their default values.</summary>
    void ResetComponentSettings();
  }
}
