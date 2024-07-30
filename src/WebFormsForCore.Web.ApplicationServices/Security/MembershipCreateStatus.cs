// Decompiled with JetBrains decompiler
// Type: System.Web.Security.MembershipCreateStatus
// Assembly: System.Web.ApplicationServices, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 49FC561C-A827-422E-A5C7-EDE4066C7817
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System.Web.ApplicationServices\v4.0_4.0.0.0__31bf3856ad364e35\System.Web.ApplicationServices.dll
// XML documentation location: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1\System.Web.ApplicationServices.xml

using System.Runtime.CompilerServices;

#nullable disable
namespace System.Web.Security
{
  /// <summary>Describes the result of a <see cref="M:System.Web.Security.Membership.CreateUser(System.String,System.String)" /> operation.</summary>
  [TypeForwardedFrom("System.Web, Version=2.0.0.0, Culture=Neutral, PublicKeyToken=b03f5f7f11d50a3a")]
  public enum MembershipCreateStatus
  {
    /// <summary>The user was successfully created.</summary>
    Success,
    /// <summary>The user name was not found in the database.</summary>
    InvalidUserName,
    /// <summary>The password is not formatted correctly.</summary>
    InvalidPassword,
    /// <summary>The password question is not formatted correctly.</summary>
    InvalidQuestion,
    /// <summary>The password answer is not formatted correctly.</summary>
    InvalidAnswer,
    /// <summary>The email address is not formatted correctly.</summary>
    InvalidEmail,
    /// <summary>The user name already exists in the database for the application.</summary>
    DuplicateUserName,
    /// <summary>The email address already exists in the database for the application.</summary>
    DuplicateEmail,
    /// <summary>The user was not created, for a reason defined by the provider.</summary>
    UserRejected,
    /// <summary>The provider user key is of an invalid type or format.</summary>
    InvalidProviderUserKey,
    /// <summary>The provider user key already exists in the database for the application.</summary>
    DuplicateProviderUserKey,
    /// <summary>The provider returned an error that is not described by other <see cref="T:System.Web.Security.MembershipCreateStatus" /> enumeration values.</summary>
    ProviderError,
  }
}
