// Decompiled with JetBrains decompiler
// Type: System.Web.Security.MembershipCreateUserException
// Assembly: System.Web.ApplicationServices, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 49FC561C-A827-422E-A5C7-EDE4066C7817
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System.Web.ApplicationServices\v4.0_4.0.0.0__31bf3856ad364e35\System.Web.ApplicationServices.dll
// XML documentation location: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1\System.Web.ApplicationServices.xml

using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Security.Permissions;

#nullable disable
namespace System.Web.Security
{
  /// <summary>The exception that is thrown when a user is not successfully created by a membership provider.</summary>
  [TypeForwardedFrom("System.Web, Version=2.0.0.0, Culture=Neutral, PublicKeyToken=b03f5f7f11d50a3a")]
  [Serializable]
  public class MembershipCreateUserException : Exception
  {
    private MembershipCreateStatus _StatusCode = MembershipCreateStatus.ProviderError;

    /// <summary>Initializes a new instance of the <see cref="T:System.Web.Security.MembershipCreateUserException" /> class with the specified <see cref="P:System.Web.Security.MembershipCreateUserException.StatusCode" /> value.</summary>
    /// <param name="statusCode">A <see cref="T:System.Web.Security.MembershipCreateStatus" /> enumeration value that describes the reason for the exception.</param>
    public MembershipCreateUserException(MembershipCreateStatus statusCode)
      : base(MembershipCreateUserException.GetMessageFromStatusCode(statusCode))
    {
      this._StatusCode = statusCode;
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.Web.Security.MembershipCreateUserException" /> class and sets the <see cref="P:System.Exception.Message" /> property to the supplied <paramref name="message" /> parameter value</summary>
    /// <param name="message">A description of the reason for the exception.</param>
    public MembershipCreateUserException(string message)
      : base(message)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.Web.Security.MembershipCreateUserException" /> class with the supplied serialization information and context.</summary>
    /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
    /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
    protected MembershipCreateUserException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
      this._StatusCode = (MembershipCreateStatus) info.GetInt32(nameof (_StatusCode));
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.Web.Security.MembershipCreateUserException" /> class.</summary>
    public MembershipCreateUserException()
    {
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.Web.Security.MembershipCreateUserException" /> class and sets the <see cref="P:System.Exception.Message" /> property to the supplied <paramref name="message" /> and the <see cref="P:System.Exception.InnerException" /> property to the supplied <paramref name="innerException" />.</summary>
    /// <param name="message">A description of the reason for the exception.</param>
    /// <param name="innerException">The exception that caused the <see cref="T:System.Web.Security.MembershipCreateUserException" />.</param>
    public MembershipCreateUserException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    /// <summary>Gets a description of the reason for the exception.</summary>
    /// <returns>A <see cref="T:System.Web.Security.MembershipCreateStatus" /> enumeration value that describes the reason for the exception.</returns>
    public MembershipCreateStatus StatusCode => this._StatusCode;

    /// <summary>Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo" /> with the data needed to serialize the target object.</summary>
    /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> to populate with data.</param>
    /// <param name="context">A <see cref="T:System.Runtime.Serialization.StreamingContext" /> that specifies the destination for this serialization.</param>
    [PermissionSet(SecurityAction.LinkDemand, Unrestricted = true)]
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      base.GetObjectData(info, context);
      info.AddValue("_StatusCode", (object) this._StatusCode);
    }

    internal static string GetMessageFromStatusCode(MembershipCreateStatus statusCode)
    {
      switch (statusCode)
      {
        case MembershipCreateStatus.Success:
          return ApplicationServicesStrings.Membership_no_error;
        case MembershipCreateStatus.InvalidUserName:
          return ApplicationServicesStrings.Membership_InvalidUserName;
        case MembershipCreateStatus.InvalidPassword:
          return ApplicationServicesStrings.Membership_InvalidPassword;
        case MembershipCreateStatus.InvalidQuestion:
          return ApplicationServicesStrings.Membership_InvalidQuestion;
        case MembershipCreateStatus.InvalidAnswer:
          return ApplicationServicesStrings.Membership_InvalidAnswer;
        case MembershipCreateStatus.InvalidEmail:
          return ApplicationServicesStrings.Membership_InvalidEmail;
        case MembershipCreateStatus.DuplicateUserName:
          return ApplicationServicesStrings.Membership_DuplicateUserName;
        case MembershipCreateStatus.DuplicateEmail:
          return ApplicationServicesStrings.Membership_DuplicateEmail;
        case MembershipCreateStatus.UserRejected:
          return ApplicationServicesStrings.Membership_UserRejected;
        case MembershipCreateStatus.InvalidProviderUserKey:
          return ApplicationServicesStrings.Membership_InvalidProviderUserKey;
        case MembershipCreateStatus.DuplicateProviderUserKey:
          return ApplicationServicesStrings.Membership_DuplicateProviderUserKey;
        default:
          return ApplicationServicesStrings.Provider_Error;
      }
    }
  }
}
