// Decompiled with JetBrains decompiler
// Type: System.Web.Security.RoleProvider
// Assembly: System.Web.ApplicationServices, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 49FC561C-A827-422E-A5C7-EDE4066C7817
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System.Web.ApplicationServices\v4.0_4.0.0.0__31bf3856ad364e35\System.Web.ApplicationServices.dll
// XML documentation location: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1\System.Web.ApplicationServices.xml

using System.Configuration.Provider;
using System.Runtime.CompilerServices;

#nullable disable
namespace System.Web.Security
{
  /// <summary>Defines the contract that ASP.NET implements to provide role-management services using custom role providers.</summary>
  [TypeForwardedFrom("System.Web, Version=2.0.0.0, Culture=Neutral, PublicKeyToken=b03f5f7f11d50a3a")]
  public abstract class RoleProvider : ProviderBase
  {
    /// <summary>Gets or sets the name of the application to store and retrieve role information for.</summary>
    /// <returns>The name of the application to store and retrieve role information for.</returns>
    public abstract string ApplicationName { get; set; }

    /// <summary>Gets a value indicating whether the specified user is in the specified role for the configured <see langword="applicationName" />.</summary>
    /// <param name="username">The user name to search for.</param>
    /// <param name="roleName">The role to search in.</param>
    /// <returns>
    /// <see langword="true" /> if the specified user is in the specified role for the configured <see langword="applicationName" />; otherwise, <see langword="false" />.</returns>
    public abstract bool IsUserInRole(string username, string roleName);

    /// <summary>Gets a list of the roles that a specified user is in for the configured <see langword="applicationName" />.</summary>
    /// <param name="username">The user to return a list of roles for.</param>
    /// <returns>A string array containing the names of all the roles that the specified user is in for the configured <see langword="applicationName" />.</returns>
    public abstract string[] GetRolesForUser(string username);

    /// <summary>Adds a new role to the data source for the configured <see langword="applicationName" />.</summary>
    /// <param name="roleName">The name of the role to create.</param>
    public abstract void CreateRole(string roleName);

    /// <summary>Removes a role from the data source for the configured <see langword="applicationName" />.</summary>
    /// <param name="roleName">The name of the role to delete.</param>
    /// <param name="throwOnPopulatedRole">If <see langword="true" />, throw an exception if <paramref name="roleName" /> has one or more members and do not delete <paramref name="roleName" />.</param>
    /// <returns>
    /// <see langword="true" /> if the role was successfully deleted; otherwise, <see langword="false" />.</returns>
    public abstract bool DeleteRole(string roleName, bool throwOnPopulatedRole);

    /// <summary>Gets a value indicating whether the specified role name already exists in the role data source for the configured <see langword="applicationName" />.</summary>
    /// <param name="roleName">The name of the role to search for in the data source.</param>
    /// <returns>
    /// <see langword="true" /> if the role name already exists in the data source for the configured <see langword="applicationName" />; otherwise, <see langword="false" />.</returns>
    public abstract bool RoleExists(string roleName);

    /// <summary>Adds the specified user names to the specified roles for the configured <see langword="applicationName" />.</summary>
    /// <param name="usernames">A string array of user names to be added to the specified roles.</param>
    /// <param name="roleNames">A string array of the role names to add the specified user names to.</param>
    public abstract void AddUsersToRoles(string[] usernames, string[] roleNames);

    /// <summary>Removes the specified user names from the specified roles for the configured <see langword="applicationName" />.</summary>
    /// <param name="usernames">A string array of user names to be removed from the specified roles.</param>
    /// <param name="roleNames">A string array of role names to remove the specified user names from.</param>
    public abstract void RemoveUsersFromRoles(string[] usernames, string[] roleNames);

    /// <summary>Gets a list of users in the specified role for the configured <see langword="applicationName" />.</summary>
    /// <param name="roleName">The name of the role to get the list of users for.</param>
    /// <returns>A string array containing the names of all the users who are members of the specified role for the configured <see langword="applicationName" />.</returns>
    public abstract string[] GetUsersInRole(string roleName);

    /// <summary>Gets a list of all the roles for the configured <see langword="applicationName" />.</summary>
    /// <returns>A string array containing the names of all the roles stored in the data source for the configured <see langword="applicationName" />.</returns>
    public abstract string[] GetAllRoles();

    /// <summary>Gets an array of user names in a role where the user name contains the specified user name to match.</summary>
    /// <param name="roleName">The role to search in.</param>
    /// <param name="usernameToMatch">The user name to search for.</param>
    /// <returns>A string array containing the names of all the users where the user name matches <paramref name="usernameToMatch" /> and the user is a member of the specified role.</returns>
    public abstract string[] FindUsersInRole(string roleName, string usernameToMatch);
  }
}
