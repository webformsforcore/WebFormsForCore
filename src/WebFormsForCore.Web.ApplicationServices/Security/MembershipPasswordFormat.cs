using System.Runtime.CompilerServices;

#nullable disable
namespace System.Web.Security
{
  /// <summary>Describes the encryption format for storing passwords for membership users.</summary>
  [TypeForwardedFrom("System.Web, Version=2.0.0.0, Culture=Neutral, PublicKeyToken=b03f5f7f11d50a3a")]
  public enum MembershipPasswordFormat
  {
    /// <summary>Not secure, do not use. Passwords are not encrypted.</summary>
    Clear,
    /// <summary>Passwords are encrypted one-way using the SHA1 hashing algorithm.</summary>
    Hashed,
    /// <summary>Not secure, do not use. Passwords are encrypted using the encryption settings determined by the machineKey Element (ASP.NET Settings Schema) element configuration.</summary>
    Encrypted,
  }
}
