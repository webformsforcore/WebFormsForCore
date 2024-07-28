#nullable disable
namespace System.Configuration
{
  /// <summary>Specifies that an application settings property has a common value for all users of an application. This class cannot be inherited.</summary>
  [AttributeUsage(AttributeTargets.Property)]
  public sealed class ApplicationScopedSettingAttribute : SettingAttribute
  {
  }
}
