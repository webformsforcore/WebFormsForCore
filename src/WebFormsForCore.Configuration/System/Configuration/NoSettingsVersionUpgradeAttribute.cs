
#nullable disable
namespace System.Configuration
{
  /// <summary>Specifies that a settings provider should disable any logic that gets invoked when an application upgrade is detected. This class cannot be inherited.</summary>
  [AttributeUsage(AttributeTargets.Property)]
  public sealed class NoSettingsVersionUpgradeAttribute : Attribute
  {
  }
}
