// Decompiled with JetBrains decompiler
// Type: System.Web.Security.ValidatePasswordEventArgs
// Assembly: System.Web.ApplicationServices, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 49FC561C-A827-422E-A5C7-EDE4066C7817
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System.Web.ApplicationServices\v4.0_4.0.0.0__31bf3856ad364e35\System.Web.ApplicationServices.dll
// XML documentation location: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1\System.Web.ApplicationServices.xml

using System.Runtime.CompilerServices;

#nullable disable
namespace System.Web.Security
{
  /// <summary>Provides event data for the <see cref="E:System.Web.Security.MembershipProvider.ValidatingPassword" /> event of the <see cref="T:System.Web.Security.MembershipProvider" /> class.</summary>
  [TypeForwardedFrom("System.Web, Version=2.0.0.0, Culture=Neutral, PublicKeyToken=b03f5f7f11d50a3a")]
  public sealed class ValidatePasswordEventArgs : EventArgs
  {
    private string _userName;
    private string _password;
    private bool _isNewUser;
    private bool _cancel;
    private Exception _failureInformation;

    /// <summary>Creates a new instance of the <see cref="T:System.Web.Security.ValidatePasswordEventArgs" /> class.</summary>
    /// <param name="userName">The membership user name for the current create-user, change-password, or reset-password action.</param>
    /// <param name="password">The new password for the specified membership user.</param>
    /// <param name="isNewUser">
    /// <see langword="true" /> if the event is occurring while a new user is being created; otherwise, <see langword="false" />.</param>
    public ValidatePasswordEventArgs(string userName, string password, bool isNewUser)
    {
      this._userName = userName;
      this._password = password;
      this._isNewUser = isNewUser;
      this._cancel = false;
    }

    /// <summary>Gets the name of the membership user for the current create-user, change-password, or reset-password action.</summary>
    /// <returns>The name of the membership user for the current create-user, change-password, or reset-password action.</returns>
    public string UserName => this._userName;

    /// <summary>Gets the password for the current create-user, change-password, or reset-password action.</summary>
    /// <returns>The password for the current create-user, change-password, or reset-password action.</returns>
    public string Password => this._password;

    /// <summary>Gets a value that indicates whether the <see cref="E:System.Web.Security.MembershipProvider.ValidatingPassword" /> event is being raised during a call to the <see cref="M:System.Web.Security.MembershipProvider.CreateUser(System.String,System.String,System.String,System.String,System.String,System.Boolean,System.Object,System.Web.Security.MembershipCreateStatus@)" /> method.</summary>
    /// <returns>
    /// <see langword="true" /> if the <see cref="E:System.Web.Security.MembershipProvider.ValidatingPassword" /> event is being raised during a call to the <see cref="M:System.Web.Security.MembershipProvider.CreateUser(System.String,System.String,System.String,System.String,System.String,System.Boolean,System.Object,System.Web.Security.MembershipCreateStatus@)" /> method; otherwise, <see langword="false" />.</returns>
    public bool IsNewUser => this._isNewUser;

    /// <summary>Gets or sets a value that indicates whether the current create-user, change-password, or reset-password action will be canceled.</summary>
    /// <returns>
    /// <see langword="true" /> if the current create-user, change-password, or reset-password action will be canceled; otherwise, <see langword="false" />. The default is <see langword="false" />.</returns>
    public bool Cancel
    {
      get => this._cancel;
      set => this._cancel = value;
    }

    /// <summary>Gets or sets an exception that describes the reason for the password-validation failure.</summary>
    /// <returns>An <see cref="T:System.Exception" /> that describes the reason for the password-validation failure.</returns>
    public Exception FailureInformation
    {
      get => this._failureInformation;
      set => this._failureInformation = value;
    }
  }
}
