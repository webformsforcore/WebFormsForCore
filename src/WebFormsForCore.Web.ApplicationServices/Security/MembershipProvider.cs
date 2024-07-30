// Decompiled with JetBrains decompiler
// Type: System.Web.Security.MembershipProvider
// Assembly: System.Web.ApplicationServices, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 49FC561C-A827-422E-A5C7-EDE4066C7817
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System.Web.ApplicationServices\v4.0_4.0.0.0__31bf3856ad364e35\System.Web.ApplicationServices.dll
// XML documentation location: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1\System.Web.ApplicationServices.xml

using System.Configuration.Provider;
using System.Runtime.CompilerServices;
using System.Web.Configuration;
using System.Web.Util;

#nullable disable
namespace System.Web.Security
{
  /// <summary>Defines the contract that ASP.NET implements to provide membership services using custom membership providers.</summary>
  [TypeForwardedFrom("System.Web, Version=2.0.0.0, Culture=Neutral, PublicKeyToken=b03f5f7f11d50a3a")]
  public abstract class MembershipProvider : ProviderBase
  {
    private MembershipValidatePasswordEventHandler _EventHandler;

    /// <summary>Indicates whether the membership provider is configured to allow users to retrieve their passwords.</summary>
    /// <returns>
    /// <see langword="true" /> if the membership provider is configured to support password retrieval; otherwise, <see langword="false" />. The default is <see langword="false" />.</returns>
    public abstract bool EnablePasswordRetrieval { get; }

    /// <summary>Indicates whether the membership provider is configured to allow users to reset their passwords.</summary>
    /// <returns>
    /// <see langword="true" /> if the membership provider supports password reset; otherwise, <see langword="false" />. The default is <see langword="true" />.</returns>
    public abstract bool EnablePasswordReset { get; }

    /// <summary>Gets a value indicating whether the membership provider is configured to require the user to answer a password question for password reset and retrieval.</summary>
    /// <returns>
    /// <see langword="true" /> if a password answer is required for password reset and retrieval; otherwise, <see langword="false" />. The default is <see langword="true" />.</returns>
    public abstract bool RequiresQuestionAndAnswer { get; }

    /// <summary>The name of the application using the custom membership provider.</summary>
    /// <returns>The name of the application using the custom membership provider.</returns>
    public abstract string ApplicationName { get; set; }

    /// <summary>Gets the number of invalid password or password-answer attempts allowed before the membership user is locked out.</summary>
    /// <returns>The number of invalid password or password-answer attempts allowed before the membership user is locked out.</returns>
    public abstract int MaxInvalidPasswordAttempts { get; }

    /// <summary>Gets the number of minutes in which a maximum number of invalid password or password-answer attempts are allowed before the membership user is locked out.</summary>
    /// <returns>The number of minutes in which a maximum number of invalid password or password-answer attempts are allowed before the membership user is locked out.</returns>
    public abstract int PasswordAttemptWindow { get; }

    /// <summary>Gets a value indicating whether the membership provider is configured to require a unique email address for each user name.</summary>
    /// <returns>
    /// <see langword="true" /> if the membership provider requires a unique email address; otherwise, <see langword="false" />. The default is <see langword="true" />.</returns>
    public abstract bool RequiresUniqueEmail { get; }

    /// <summary>Gets a value indicating the format for storing passwords in the membership data store.</summary>
    /// <returns>One of the <see cref="T:System.Web.Security.MembershipPasswordFormat" /> values indicating the format for storing passwords in the data store.</returns>
    public abstract MembershipPasswordFormat PasswordFormat { get; }

    /// <summary>Gets the minimum length required for a password.</summary>
    /// <returns>The minimum length required for a password.</returns>
    public abstract int MinRequiredPasswordLength { get; }

    /// <summary>Gets the minimum number of special characters that must be present in a valid password.</summary>
    /// <returns>The minimum number of special characters that must be present in a valid password.</returns>
    public abstract int MinRequiredNonAlphanumericCharacters { get; }

    /// <summary>Gets the regular expression used to evaluate a password.</summary>
    /// <returns>A regular expression used to evaluate a password.</returns>
    public abstract string PasswordStrengthRegularExpression { get; }

    /// <summary>Adds a new membership user to the data source.</summary>
    /// <param name="username">The user name for the new user.</param>
    /// <param name="password">The password for the new user.</param>
    /// <param name="email">The email address for the new user.</param>
    /// <param name="passwordQuestion">The password question for the new user.</param>
    /// <param name="passwordAnswer">The password answer for the new user</param>
    /// <param name="isApproved">Whether or not the new user is approved to be validated.</param>
    /// <param name="providerUserKey">The unique identifier from the membership data source for the user.</param>
    /// <param name="status">A <see cref="T:System.Web.Security.MembershipCreateStatus" /> enumeration value indicating whether the user was created successfully.</param>
    /// <returns>A <see cref="T:System.Web.Security.MembershipUser" /> object populated with the information for the newly created user.</returns>
    public abstract MembershipUser CreateUser(
      string username,
      string password,
      string email,
      string passwordQuestion,
      string passwordAnswer,
      bool isApproved,
      object providerUserKey,
      out MembershipCreateStatus status);

    /// <summary>Processes a request to update the password question and answer for a membership user.</summary>
    /// <param name="username">The user to change the password question and answer for.</param>
    /// <param name="password">The password for the specified user.</param>
    /// <param name="newPasswordQuestion">The new password question for the specified user.</param>
    /// <param name="newPasswordAnswer">The new password answer for the specified user.</param>
    /// <returns>
    /// <see langword="true" /> if the password question and answer are updated successfully; otherwise, <see langword="false" />.</returns>
    public abstract bool ChangePasswordQuestionAndAnswer(
      string username,
      string password,
      string newPasswordQuestion,
      string newPasswordAnswer);

    /// <summary>Gets the password for the specified user name from the data source.</summary>
    /// <param name="username">The user to retrieve the password for.</param>
    /// <param name="answer">The password answer for the user.</param>
    /// <returns>The password for the specified user name.</returns>
    public abstract string GetPassword(string username, string answer);

    /// <summary>Processes a request to update the password for a membership user.</summary>
    /// <param name="username">The user to update the password for.</param>
    /// <param name="oldPassword">The current password for the specified user.</param>
    /// <param name="newPassword">The new password for the specified user.</param>
    /// <returns>
    /// <see langword="true" /> if the password was updated successfully; otherwise, <see langword="false" />.</returns>
    public abstract bool ChangePassword(string username, string oldPassword, string newPassword);

    /// <summary>Resets a user's password to a new, automatically generated password.</summary>
    /// <param name="username">The user to reset the password for.</param>
    /// <param name="answer">The password answer for the specified user.</param>
    /// <returns>The new password for the specified user.</returns>
    public abstract string ResetPassword(string username, string answer);

    /// <summary>Updates information about a user in the data source.</summary>
    /// <param name="user">A <see cref="T:System.Web.Security.MembershipUser" /> object that represents the user to update and the updated information for the user.</param>
    public abstract void UpdateUser(MembershipUser user);

    /// <summary>Verifies that the specified user name and password exist in the data source.</summary>
    /// <param name="username">The name of the user to validate.</param>
    /// <param name="password">The password for the specified user.</param>
    /// <returns>
    /// <see langword="true" /> if the specified username and password are valid; otherwise, <see langword="false" />.</returns>
    public abstract bool ValidateUser(string username, string password);

    /// <summary>Clears a lock so that the membership user can be validated.</summary>
    /// <param name="userName">The membership user whose lock status you want to clear.</param>
    /// <returns>
    /// <see langword="true" /> if the membership user was successfully unlocked; otherwise, <see langword="false" />.</returns>
    public abstract bool UnlockUser(string userName);

    /// <summary>Gets user information from the data source based on the unique identifier for the membership user. Provides an option to update the last-activity date/time stamp for the user.</summary>
    /// <param name="providerUserKey">The unique identifier for the membership user to get information for.</param>
    /// <param name="userIsOnline">
    /// <see langword="true" /> to update the last-activity date/time stamp for the user; <see langword="false" /> to return user information without updating the last-activity date/time stamp for the user.</param>
    /// <returns>A <see cref="T:System.Web.Security.MembershipUser" /> object populated with the specified user's information from the data source.</returns>
    public abstract MembershipUser GetUser(object providerUserKey, bool userIsOnline);

    /// <summary>Gets information from the data source for a user. Provides an option to update the last-activity date/time stamp for the user.</summary>
    /// <param name="username">The name of the user to get information for.</param>
    /// <param name="userIsOnline">
    /// <see langword="true" /> to update the last-activity date/time stamp for the user; <see langword="false" /> to return user information without updating the last-activity date/time stamp for the user.</param>
    /// <returns>A <see cref="T:System.Web.Security.MembershipUser" /> object populated with the specified user's information from the data source.</returns>
    public abstract MembershipUser GetUser(string username, bool userIsOnline);

    internal MembershipUser GetUser(string username, bool userIsOnline, bool throwOnError)
    {
      MembershipUser user = (MembershipUser) null;
      try
      {
        user = this.GetUser(username, userIsOnline);
      }
      catch (ArgumentException ex)
      {
        if (throwOnError)
          throw;
      }
      return user;
    }

    /// <summary>Gets the user name associated with the specified email address.</summary>
    /// <param name="email">The email address to search for.</param>
    /// <returns>The user name associated with the specified email address. If no match is found, return <see langword="null" />.</returns>
    public abstract string GetUserNameByEmail(string email);

    /// <summary>Removes a user from the membership data source.</summary>
    /// <param name="username">The name of the user to delete.</param>
    /// <param name="deleteAllRelatedData">
    /// <see langword="true" /> to delete data related to the user from the database; <see langword="false" /> to leave data related to the user in the database.</param>
    /// <returns>
    /// <see langword="true" /> if the user was successfully deleted; otherwise, <see langword="false" />.</returns>
    public abstract bool DeleteUser(string username, bool deleteAllRelatedData);

    /// <summary>Gets a collection of all the users in the data source in pages of data.</summary>
    /// <param name="pageIndex">The index of the page of results to return. <paramref name="pageIndex" /> is zero-based.</param>
    /// <param name="pageSize">The size of the page of results to return.</param>
    /// <param name="totalRecords">The total number of matched users.</param>
    /// <returns>A <see cref="T:System.Web.Security.MembershipUserCollection" /> collection that contains a page of <paramref name="pageSize" /><see cref="T:System.Web.Security.MembershipUser" /> objects beginning at the page specified by <paramref name="pageIndex" />.</returns>
    public abstract MembershipUserCollection GetAllUsers(
      int pageIndex,
      int pageSize,
      out int totalRecords);

    /// <summary>Gets the number of users currently accessing the application.</summary>
    /// <returns>The number of users currently accessing the application.</returns>
    public abstract int GetNumberOfUsersOnline();

    /// <summary>Gets a collection of membership users where the user name contains the specified user name to match.</summary>
    /// <param name="usernameToMatch">The user name to search for.</param>
    /// <param name="pageIndex">The index of the page of results to return. <paramref name="pageIndex" /> is zero-based.</param>
    /// <param name="pageSize">The size of the page of results to return.</param>
    /// <param name="totalRecords">The total number of matched users.</param>
    /// <returns>A <see cref="T:System.Web.Security.MembershipUserCollection" /> collection that contains a page of <paramref name="pageSize" /><see cref="T:System.Web.Security.MembershipUser" /> objects beginning at the page specified by <paramref name="pageIndex" />.</returns>
    public abstract MembershipUserCollection FindUsersByName(
      string usernameToMatch,
      int pageIndex,
      int pageSize,
      out int totalRecords);

    /// <summary>Gets a collection of membership users where the email address contains the specified email address to match.</summary>
    /// <param name="emailToMatch">The email address to search for.</param>
    /// <param name="pageIndex">The index of the page of results to return. <paramref name="pageIndex" /> is zero-based.</param>
    /// <param name="pageSize">The size of the page of results to return.</param>
    /// <param name="totalRecords">The total number of matched users.</param>
    /// <returns>A <see cref="T:System.Web.Security.MembershipUserCollection" /> collection that contains a page of <paramref name="pageSize" /><see cref="T:System.Web.Security.MembershipUser" /> objects beginning at the page specified by <paramref name="pageIndex" />.</returns>
    public abstract MembershipUserCollection FindUsersByEmail(
      string emailToMatch,
      int pageIndex,
      int pageSize,
      out int totalRecords);

    /// <summary>Encrypts a password.</summary>
    /// <param name="password">A byte array that contains the password to encrypt.</param>
    /// <returns>A byte array that contains the encrypted password.</returns>
    /// <exception cref="T:System.Configuration.Provider.ProviderException">The <see cref="P:System.Web.Configuration.MachineKeySection.ValidationKey" /> property or <see cref="P:System.Web.Configuration.MachineKeySection.DecryptionKey" /> property is set to <see langword="AutoGenerate" />.</exception>
    /// <exception cref="T:System.PlatformNotSupportedException">This method is not available. This can occur if the application targets the .NET Framework 4 Client Profile. To prevent this exception, override the method, or change the application to target the full version of the .NET Framework.</exception>
    protected virtual byte[] EncryptPassword(byte[] password)
    {
      return this.EncryptPassword(password, MembershipPasswordCompatibilityMode.Framework20);
    }

    /// <summary>Encrypts the specified password using the specified password-compatibility mode.</summary>
    /// <param name="password">A byte array that contains the password to encrypt.</param>
    /// <param name="legacyPasswordCompatibilityMode">The membership password-compatibility mode.</param>
    /// <returns>A byte array that contains the encrypted password.</returns>
    /// <exception cref="T:System.Configuration.Provider.ProviderException">The <see cref="P:System.Web.Configuration.MachineKeySection.ValidationKey" /> property or <see cref="P:System.Web.Configuration.MachineKeySection.DecryptionKey" /> property is set to <see langword="AutoGenerate" />.</exception>
    /// <exception cref="T:System.PlatformNotSupportedException">This method is not available. This can occur if the application targets the .NET Framework 4 Client Profile. To prevent this exception, override the method, or change the application to target the full version of the .NET Framework.</exception>
    protected virtual byte[] EncryptPassword(
      byte[] password,
      MembershipPasswordCompatibilityMode legacyPasswordCompatibilityMode)
    {
      if (SystemWebProxy.Membership.IsDecryptionKeyAutogenerated)
        throw new ProviderException(ApplicationServicesStrings.Can_not_use_encrypted_passwords_with_autogen_keys);
      return SystemWebProxy.Membership.EncryptOrDecryptData(true, password, legacyPasswordCompatibilityMode == MembershipPasswordCompatibilityMode.Framework20);
    }

    /// <summary>Decrypts an encrypted password.</summary>
    /// <param name="encodedPassword">A byte array that contains the encrypted password to decrypt.</param>
    /// <returns>A byte array that contains the decrypted password.</returns>
    /// <exception cref="T:System.Configuration.Provider.ProviderException">The <see cref="P:System.Web.Configuration.MachineKeySection.ValidationKey" /> property or <see cref="P:System.Web.Configuration.MachineKeySection.DecryptionKey" /> property is set to <see langword="AutoGenerate" />.</exception>
    /// <exception cref="T:System.PlatformNotSupportedException">This method is not available. This can occur if the application targets the .NET Framework 4 Client Profile. To prevent this exception, override the method, or change the application to target the full version of the .NET Framework.</exception>
    protected virtual byte[] DecryptPassword(byte[] encodedPassword)
    {
      if (SystemWebProxy.Membership.IsDecryptionKeyAutogenerated)
        throw new ProviderException(ApplicationServicesStrings.Can_not_use_encrypted_passwords_with_autogen_keys);
      try
      {
        return SystemWebProxy.Membership.EncryptOrDecryptData(false, encodedPassword, false);
      }
      catch
      {
        if (!SystemWebProxy.Membership.UsingCustomEncryption)
          throw;
      }
      return SystemWebProxy.Membership.EncryptOrDecryptData(false, encodedPassword, true);
    }

    /// <summary>Occurs when a user is created, a password is changed, or a password is reset.</summary>
    public event MembershipValidatePasswordEventHandler ValidatingPassword
    {
      add => this._EventHandler += value;
      remove => this._EventHandler -= value;
    }

    /// <summary>Raises the <see cref="E:System.Web.Security.MembershipProvider.ValidatingPassword" /> event if an event handler has been defined.</summary>
    /// <param name="e">The <see cref="T:System.Web.Security.ValidatePasswordEventArgs" /> to pass to the <see cref="E:System.Web.Security.MembershipProvider.ValidatingPassword" /> event handler.</param>
    protected virtual void OnValidatingPassword(ValidatePasswordEventArgs e)
    {
      if (this._EventHandler == null)
        return;
      this._EventHandler((object) this, e);
    }
  }
}
