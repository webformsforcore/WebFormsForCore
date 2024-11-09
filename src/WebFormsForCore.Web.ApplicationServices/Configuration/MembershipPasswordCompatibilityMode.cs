#nullable disable
namespace System.Web.Configuration
{
  /// <summary>Enumerates the password-compatibility modes for ASP.NET membership.</summary>
  public enum MembershipPasswordCompatibilityMode
  {
    /// <summary>Passwords are in ASP.NET 2.0 mode.</summary>
    Framework20,
    /// <summary>Passwords are in ASP.NET 4 mode.</summary>
    Framework40,
  }
}
