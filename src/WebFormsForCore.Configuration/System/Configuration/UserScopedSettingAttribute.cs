
#nullable disable
namespace System.Configuration
{
  /// <summary>Specifies that an application settings group or property contains distinct values for each user of an application. This class cannot be inherited.</summary>
  [AttributeUsage(AttributeTargets.Property)]
  public sealed class UserScopedSettingAttribute : SettingAttribute
  {
  }
}
