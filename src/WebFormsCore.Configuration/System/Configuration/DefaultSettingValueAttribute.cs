
#nullable disable
namespace System.Configuration
{
  /// <summary>Specifies the default value for an application settings property.</summary>
  [AttributeUsage(AttributeTargets.Property)]
  public sealed class DefaultSettingValueAttribute : Attribute
  {
    private readonly string _value;

    /// <summary>Initializes an instance of the <see cref="T:System.Configuration.DefaultSettingValueAttribute" /> class.</summary>
    /// <param name="value">A <see cref="T:System.String" /> that represents the default value for the property.</param>
    public DefaultSettingValueAttribute(string value) => this._value = value;

    /// <summary>Gets the default value for the application settings property.</summary>
    /// <returns>A <see cref="T:System.String" /> that represents the default value for the property.</returns>
    public string Value => this._value;
  }
}
