
#nullable disable
namespace System.Configuration
{
  /// <summary>Provides a string that describes an individual configuration property. This class cannot be inherited.</summary>
  [AttributeUsage(AttributeTargets.Property)]
  public sealed class SettingsDescriptionAttribute : Attribute
  {
    private readonly string _desc;

    /// <summary>Initializes an instance of the <see cref="T:System.Configuration.SettingsDescriptionAttribute" /> class.</summary>
    /// <param name="description">The <see cref="T:System.String" /> used as descriptive text.</param>
    public SettingsDescriptionAttribute(string description) => this._desc = description;

    /// <summary>Gets the descriptive text for the associated configuration property.</summary>
    /// <returns>A <see cref="T:System.String" /> containing the descriptive text for the associated configuration property.</returns>
    public string Description => this._desc;
  }
}
