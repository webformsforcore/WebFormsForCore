using System.Configuration.Provider;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Web.Util;

#nullable disable
namespace System.Web.Security
{
  /// <summary>Exposes and updates membership user information in the membership data store.</summary>
  [TypeForwardedFrom("System.Web, Version=2.0.0.0, Culture=Neutral, PublicKeyToken=b03f5f7f11d50a3a")]
  [Serializable]
  public class MembershipUser
  {
    private string _UserName;
    private object _ProviderUserKey;
    private string _Email;
    private string _PasswordQuestion;
    private string _Comment;
    private bool _IsApproved;
    private bool _IsLockedOut;
    private DateTime _LastLockoutDate;
    private DateTime _CreationDate;
    private DateTime _LastLoginDate;
    private DateTime _LastActivityDate;
    private DateTime _LastPasswordChangedDate;
    private string _ProviderName;

    /// <summary>Gets the logon name of the membership user.</summary>
    /// <returns>The logon name of the membership user.</returns>
    public virtual string UserName => this._UserName;

    /// <summary>Gets the user identifier from the membership data source for the user.</summary>
    /// <returns>The user identifier from the membership data source for the user.</returns>
    public virtual object ProviderUserKey => this._ProviderUserKey;

    /// <summary>Gets or sets the email address for the membership user.</summary>
    /// <returns>The email address for the membership user.</returns>
    public virtual string Email
    {
      get => this._Email;
      set => this._Email = value;
    }

    /// <summary>Gets the password question for the membership user.</summary>
    /// <returns>The password question for the membership user.</returns>
    public virtual string PasswordQuestion => this._PasswordQuestion;

    /// <summary>Gets or sets application-specific information for the membership user.</summary>
    /// <returns>Application-specific information for the membership user.</returns>
    public virtual string Comment
    {
      get => this._Comment;
      set => this._Comment = value;
    }

    /// <summary>Gets or sets whether the membership user can be authenticated.</summary>
    /// <returns>
    /// <see langword="true" /> if the user can be authenticated; otherwise, <see langword="false" />.</returns>
    public virtual bool IsApproved
    {
      get => this._IsApproved;
      set => this._IsApproved = value;
    }

    /// <summary>Gets a value indicating whether the membership user is locked out and unable to be validated.</summary>
    /// <returns>
    /// <see langword="true" /> if the membership user is locked out and unable to be validated; otherwise, <see langword="false" />.</returns>
    public virtual bool IsLockedOut => this._IsLockedOut;

    /// <summary>Gets the most recent date and time that the membership user was locked out.</summary>
    /// <returns>A <see cref="T:System.DateTime" /> object that represents the most recent date and time that the membership user was locked out.</returns>
    public virtual DateTime LastLockoutDate => this._LastLockoutDate.ToLocalTime();

    /// <summary>Gets the date and time when the user was added to the membership data store.</summary>
    /// <returns>The date and time when the user was added to the membership data store.</returns>
    public virtual DateTime CreationDate => this._CreationDate.ToLocalTime();

    /// <summary>Gets or sets the date and time when the user was last authenticated.</summary>
    /// <returns>The date and time when the user was last authenticated.</returns>
    public virtual DateTime LastLoginDate
    {
      get => this._LastLoginDate.ToLocalTime();
      set => this._LastLoginDate = value.ToUniversalTime();
    }

    /// <summary>Gets or sets the date and time when the membership user was last authenticated or accessed the application.</summary>
    /// <returns>The date and time when the membership user was last authenticated or accessed the application.</returns>
    public virtual DateTime LastActivityDate
    {
      get => this._LastActivityDate.ToLocalTime();
      set => this._LastActivityDate = value.ToUniversalTime();
    }

    /// <summary>Gets the date and time when the membership user's password was last updated.</summary>
    /// <returns>The date and time when the membership user's password was last updated.</returns>
    public virtual DateTime LastPasswordChangedDate => this._LastPasswordChangedDate.ToLocalTime();

    /// <summary>Gets whether the user is currently online.</summary>
    /// <returns>
    /// <see langword="true" /> if the user is online; otherwise, <see langword="false" />.</returns>
    /// <exception cref="T:System.PlatformNotSupportedException">This method is not available. This can occur if the application targets the .NET Framework 4 Client Profile. To prevent this exception, override the method, or change the application to target the full version of the .NET Framework.</exception>
    public virtual bool IsOnline
    {
      get
      {
        DateTime dateTime = DateTime.UtcNow.Subtract(new TimeSpan(0, SystemWebProxy.Membership.UserIsOnlineTimeWindow, 0));
        return this.LastActivityDate.ToUniversalTime() > dateTime;
      }
    }

    /// <summary>Returns the user name for the membership user.</summary>
    /// <returns>The <see cref="P:System.Web.Security.MembershipUser.UserName" /> for the membership user.</returns>
    public override string ToString() => this.UserName;

    /// <summary>Gets the name of the membership provider that stores and retrieves user information for the membership user.</summary>
    /// <returns>The name of the membership provider that stores and retrieves user information for the membership user.</returns>
    public virtual string ProviderName => this._ProviderName;

    /// <summary>Creates a new membership user object with the specified property values.</summary>
    /// <param name="providerName">The <see cref="P:System.Web.Security.MembershipUser.ProviderName" /> string for the membership user.</param>
    /// <param name="name">The <see cref="P:System.Web.Security.MembershipUser.UserName" /> string for the membership user.</param>
    /// <param name="providerUserKey">The <see cref="P:System.Web.Security.MembershipUser.ProviderUserKey" /> identifier for the membership user.</param>
    /// <param name="email">The <see cref="P:System.Web.Security.MembershipUser.Email" /> string for the membership user.</param>
    /// <param name="passwordQuestion">The <see cref="P:System.Web.Security.MembershipUser.PasswordQuestion" /> string for the membership user.</param>
    /// <param name="comment">The <see cref="P:System.Web.Security.MembershipUser.Comment" /> string for the membership user.</param>
    /// <param name="isApproved">The <see cref="P:System.Web.Security.MembershipUser.IsApproved" /> value for the membership user.</param>
    /// <param name="isLockedOut">
    /// <see langword="true" /> to lock out the membership user; otherwise, <see langword="false" />.</param>
    /// <param name="creationDate">The <see cref="P:System.Web.Security.MembershipUser.CreationDate" /><see cref="T:System.DateTime" /> object for the membership user.</param>
    /// <param name="lastLoginDate">The <see cref="P:System.Web.Security.MembershipUser.LastLoginDate" /><see cref="T:System.DateTime" /> object for the membership user.</param>
    /// <param name="lastActivityDate">The <see cref="P:System.Web.Security.MembershipUser.LastActivityDate" /><see cref="T:System.DateTime" /> object for the membership user.</param>
    /// <param name="lastPasswordChangedDate">The <see cref="P:System.Web.Security.MembershipUser.LastPasswordChangedDate" /><see cref="T:System.DateTime" /> object for the membership user.</param>
    /// <param name="lastLockoutDate">The <see cref="P:System.Web.Security.MembershipUser.LastLockoutDate" /><see cref="T:System.DateTime" /> object for the membership user.</param>
    /// <exception cref="T:System.ArgumentException">
    ///         <paramref name="providerName" /> is <see langword="null" />.
    /// -or-
    /// <paramref name="providerName" /> is not found in the <see cref="P:System.Web.Security.Membership.Providers" /> collection.</exception>
    /// <exception cref="T:System.PlatformNotSupportedException">The constructor is not available. This can occur if the application targets the .NET Framework 4 Client Profile. To prevent this exception, derive your class from the type and then call the default protected constructor, or change the application to target the full version of the .NET Framework.</exception>
    public MembershipUser(
      string providerName,
      string name,
      object providerUserKey,
      string email,
      string passwordQuestion,
      string comment,
      bool isApproved,
      bool isLockedOut,
      DateTime creationDate,
      DateTime lastLoginDate,
      DateTime lastActivityDate,
      DateTime lastPasswordChangedDate,
      DateTime lastLockoutDate)
    {
      if (providerName == null || SystemWebProxy.Membership.Providers[providerName] == null)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ApplicationServicesStrings.Membership_provider_name_invalid), nameof (providerName));
      if (name != null)
        name = name.Trim();
      if (email != null)
        email = email.Trim();
      if (passwordQuestion != null)
        passwordQuestion = passwordQuestion.Trim();
      this._ProviderName = providerName;
      this._UserName = name;
      this._ProviderUserKey = providerUserKey;
      this._Email = email;
      this._PasswordQuestion = passwordQuestion;
      this._Comment = comment;
      this._IsApproved = isApproved;
      this._IsLockedOut = isLockedOut;
      this._CreationDate = creationDate.ToUniversalTime();
      this._LastLoginDate = lastLoginDate.ToUniversalTime();
      this._LastActivityDate = lastActivityDate.ToUniversalTime();
      this._LastPasswordChangedDate = lastPasswordChangedDate.ToUniversalTime();
      this._LastLockoutDate = lastLockoutDate.ToUniversalTime();
    }

    /// <summary>Creates a new instance of a <see cref="T:System.Web.Security.MembershipUser" /> object for a class that inherits the <see cref="T:System.Web.Security.MembershipUser" /> class.</summary>
    protected MembershipUser()
    {
    }

    internal virtual void Update()
    {
      SystemWebProxy.Membership.Providers[this.ProviderName].UpdateUser(this);
      this.UpdateSelf();
    }

    /// <summary>Gets the password for the membership user from the membership data store.</summary>
    /// <returns>The password for the membership user.</returns>
    /// <exception cref="T:System.PlatformNotSupportedException">This method is not available. This can occur if the application targets the .NET Framework 4 Client Profile. To prevent this exception, override the method, or change the application to target the full version of the .NET Framework.</exception>
    public virtual string GetPassword()
    {
      return SystemWebProxy.Membership.Providers[this.ProviderName].GetPassword(this.UserName, (string) null);
    }

    /// <summary>Gets the password for the membership user from the membership data store.</summary>
    /// <param name="passwordAnswer">The password answer for the membership user.</param>
    /// <returns>The password for the membership user.</returns>
    /// <exception cref="T:System.PlatformNotSupportedException">This method is not available. This can occur if the application targets the .NET Framework 4 Client Profile. To prevent this exception, override the method, or change the application to target the full version of the .NET Framework.</exception>
    public virtual string GetPassword(string passwordAnswer)
    {
      return SystemWebProxy.Membership.Providers[this.ProviderName].GetPassword(this.UserName, passwordAnswer);
    }

    internal string GetPassword(bool throwOnError)
    {
      return this.GetPassword((string) null, false, throwOnError);
    }

    internal string GetPassword(string answer, bool throwOnError)
    {
      return this.GetPassword(answer, true, throwOnError);
    }

    private string GetPassword(string answer, bool useAnswer, bool throwOnError)
    {
      string password = (string) null;
      try
      {
        password = !useAnswer ? this.GetPassword() : this.GetPassword(answer);
      }
      catch (ArgumentException ex)
      {
        if (throwOnError)
          throw;
      }
      catch (MembershipPasswordException ex)
      {
        if (throwOnError)
          throw;
      }
      catch (ProviderException ex)
      {
        if (throwOnError)
          throw;
      }
      return password;
    }

    /// <summary>Updates the password for the membership user in the membership data store.</summary>
    /// <param name="oldPassword">The current password for the membership user.</param>
    /// <param name="newPassword">The new password for the membership user.</param>
    /// <returns>
    /// <see langword="true" /> if the update was successful; otherwise, <see langword="false" />.</returns>
    /// <exception cref="T:System.ArgumentException">
    ///         <paramref name="oldPassword" /> is an empty string.
    /// -or-
    /// <paramref name="newPassword" /> is an empty string.</exception>
    /// <exception cref="T:System.ArgumentNullException">
    ///         <paramref name="oldPassword" /> is <see langword="null" />.
    /// -or-
    /// <paramref name="newPassword" /> is <see langword="null" />.</exception>
    /// <exception cref="T:System.PlatformNotSupportedException">This method is not available. This can occur if the application targets the .NET Framework 4 Client Profile. To prevent this exception, override the method, or change the application to target the full version of the .NET Framework.</exception>
    public virtual bool ChangePassword(string oldPassword, string newPassword)
    {
      SecurityServices.CheckPasswordParameter(oldPassword, nameof (oldPassword));
      SecurityServices.CheckPasswordParameter(newPassword, nameof (newPassword));
      if (!SystemWebProxy.Membership.Providers[this.ProviderName].ChangePassword(this.UserName, oldPassword, newPassword))
        return false;
      this.UpdateSelf();
      return true;
    }

    internal bool ChangePassword(string oldPassword, string newPassword, bool throwOnError)
    {
      bool flag = false;
      try
      {
        flag = this.ChangePassword(oldPassword, newPassword);
      }
      catch (ArgumentException ex)
      {
        if (throwOnError)
          throw;
      }
      catch (MembershipPasswordException ex)
      {
        if (throwOnError)
          throw;
      }
      catch (ProviderException ex)
      {
        if (throwOnError)
          throw;
      }
      return flag;
    }

    /// <summary>Updates the password question and answer for the membership user in the membership data store.</summary>
    /// <param name="password">The current password for the membership user.</param>
    /// <param name="newPasswordQuestion">The new password question value for the membership user.</param>
    /// <param name="newPasswordAnswer">The new password answer value for the membership user.</param>
    /// <returns>
    /// <see langword="true" /> if the update was successful; otherwise, <see langword="false" />.</returns>
    /// <exception cref="T:System.ArgumentException">
    ///         <paramref name="password" /> is an empty string.
    /// -or-
    /// <paramref name="newPasswordQuestion" /> is an empty string.
    /// -or-
    /// <paramref name="newPasswordAnswer" /> is an empty string.</exception>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="password" /> is <see langword="null" />.</exception>
    /// <exception cref="T:System.PlatformNotSupportedException">This method is not available. This can occur if the application targets the .NET Framework 4 Client Profile. To prevent this exception, override the method, or change the application to target the full version of the .NET Framework.</exception>
    public virtual bool ChangePasswordQuestionAndAnswer(
      string password,
      string newPasswordQuestion,
      string newPasswordAnswer)
    {
      SecurityServices.CheckPasswordParameter(password, nameof (password));
      SecurityServices.CheckForEmptyOrWhiteSpaceParameter(ref newPasswordQuestion, nameof (newPasswordQuestion));
      SecurityServices.CheckForEmptyOrWhiteSpaceParameter(ref newPasswordAnswer, nameof (newPasswordAnswer));
      if (!SystemWebProxy.Membership.Providers[this.ProviderName].ChangePasswordQuestionAndAnswer(this.UserName, password, newPasswordQuestion, newPasswordAnswer))
        return false;
      this.UpdateSelf();
      return true;
    }

    /// <summary>Resets a user's password to a new, automatically generated password.</summary>
    /// <param name="passwordAnswer">The password answer for the membership user.</param>
    /// <returns>The new password for the membership user.</returns>
    /// <exception cref="T:System.PlatformNotSupportedException">This method is not available. This can occur if the application targets the .NET Framework 4 Client Profile. To prevent this exception, override the method, or change the application to target the full version of the .NET Framework.</exception>
    public virtual string ResetPassword(string passwordAnswer)
    {
      string str = SystemWebProxy.Membership.Providers[this.ProviderName].ResetPassword(this.UserName, passwordAnswer);
      if (!string.IsNullOrEmpty(str))
        this.UpdateSelf();
      return str;
    }

    /// <summary>Resets a user's password to a new, automatically generated password.</summary>
    /// <returns>The new password for the membership user.</returns>
    /// <exception cref="T:System.PlatformNotSupportedException">This method is not available. This can occur if the application targets the .NET Framework 4 Client Profile. To prevent this exception, override the method, or change the application to target the full version of the .NET Framework.</exception>
    public virtual string ResetPassword() => this.ResetPassword((string) null);

    internal string ResetPassword(bool throwOnError)
    {
      return this.ResetPassword((string) null, false, throwOnError);
    }

    internal string ResetPassword(string passwordAnswer, bool throwOnError)
    {
      return this.ResetPassword(passwordAnswer, true, throwOnError);
    }

    private string ResetPassword(string passwordAnswer, bool useAnswer, bool throwOnError)
    {
      string str = (string) null;
      try
      {
        str = !useAnswer ? this.ResetPassword() : this.ResetPassword(passwordAnswer);
      }
      catch (ArgumentException ex)
      {
        if (throwOnError)
          throw;
      }
      catch (MembershipPasswordException ex)
      {
        if (throwOnError)
          throw;
      }
      catch (ProviderException ex)
      {
        if (throwOnError)
          throw;
      }
      return str;
    }

    /// <summary>Clears the locked-out state of the user so that the membership user can be validated.</summary>
    /// <returns>
    /// <see langword="true" /> if the membership user was successfully unlocked; otherwise, <see langword="false" />.</returns>
    /// <exception cref="T:System.PlatformNotSupportedException">This method is not available. This can occur if the application targets the .NET Framework 4 Client Profile. To prevent this exception, override the method, or change the application to target the full version of the .NET Framework.</exception>
    public virtual bool UnlockUser()
    {
      if (!SystemWebProxy.Membership.Providers[this.ProviderName].UnlockUser(this.UserName))
        return false;
      this.UpdateSelf();
      return !this.IsLockedOut;
    }

    private void UpdateSelf()
    {
      MembershipUser user = SystemWebProxy.Membership.Providers[this.ProviderName].GetUser(this.UserName, false);
      if (user == null)
        return;
      try
      {
        this._LastPasswordChangedDate = user.LastPasswordChangedDate.ToUniversalTime();
      }
      catch (NotSupportedException ex)
      {
      }
      try
      {
        this.LastActivityDate = user.LastActivityDate;
      }
      catch (NotSupportedException ex)
      {
      }
      try
      {
        this.LastLoginDate = user.LastLoginDate;
      }
      catch (NotSupportedException ex)
      {
      }
      try
      {
        this._CreationDate = user.CreationDate.ToUniversalTime();
      }
      catch (NotSupportedException ex)
      {
      }
      try
      {
        this._LastLockoutDate = user.LastLockoutDate.ToUniversalTime();
      }
      catch (NotSupportedException ex)
      {
      }
      try
      {
        this._IsLockedOut = user.IsLockedOut;
      }
      catch (NotSupportedException ex)
      {
      }
      try
      {
        this.IsApproved = user.IsApproved;
      }
      catch (NotSupportedException ex)
      {
      }
      try
      {
        this.Comment = user.Comment;
      }
      catch (NotSupportedException ex)
      {
      }
      try
      {
        this._PasswordQuestion = user.PasswordQuestion;
      }
      catch (NotSupportedException ex)
      {
      }
      try
      {
        this.Email = user.Email;
      }
      catch (NotSupportedException ex)
      {
      }
      try
      {
        this._ProviderUserKey = user.ProviderUserKey;
      }
      catch (NotSupportedException ex)
      {
      }
    }
  }
}
