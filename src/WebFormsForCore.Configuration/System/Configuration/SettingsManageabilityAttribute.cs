
#nullable disable
namespace System.Configuration
{
  /// <summary>Specifies special services for application settings properties. This class cannot be inherited.</summary>
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
  public sealed class SettingsManageabilityAttribute : Attribute
  {
    private readonly SettingsManageability _manageability;

    /// <summary>Initializes a new instance of the <see cref="T:System.Configuration.SettingsManageabilityAttribute" /> class.</summary>
    /// <param name="manageability">A <see cref="T:System.Configuration.SettingsManageability" /> value that enumerates the services being requested.</param>
    public SettingsManageabilityAttribute(SettingsManageability manageability)
    {
      this._manageability = manageability;
    }

    /// <summary>Gets the set of special services that have been requested.</summary>
    /// <returns>A value that results from using the logical <see langword="OR" /> operator to combine all the <see cref="T:System.Configuration.SettingsManageability" /> enumeration values corresponding to the requested services.</returns>
    public SettingsManageability Manageability => this._manageability;
  }
}
