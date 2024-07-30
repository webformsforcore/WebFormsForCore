// Decompiled with JetBrains decompiler
// Type: System.Web.Security.MembershipPasswordFormat
// Assembly: System.Web.ApplicationServices, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 49FC561C-A827-422E-A5C7-EDE4066C7817
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System.Web.ApplicationServices\v4.0_4.0.0.0__31bf3856ad364e35\System.Web.ApplicationServices.dll
// XML documentation location: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1\System.Web.ApplicationServices.xml

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
