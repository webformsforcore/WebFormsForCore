using System.Runtime.CompilerServices;

#nullable disable
namespace System.Web.Security
{
  /// <summary>Represents the method that will handle the <see cref="E:System.Web.Security.MembershipProvider.ValidatingPassword" /> event of the <see cref="T:System.Web.Security.MembershipProvider" /> class.</summary>
  /// <param name="sender">The <see cref="T:System.Web.Security.MembershipProvider" /> that raised the <see cref="E:System.Web.Security.MembershipProvider.ValidatingPassword" /> event.</param>
  /// <param name="e">A <see cref="T:System.Web.Security.ValidatePasswordEventArgs" /> object that contains the event data.</param>
  [TypeForwardedFrom("System.Web, Version=2.0.0.0, Culture=Neutral, PublicKeyToken=b03f5f7f11d50a3a")]
  public delegate void MembershipValidatePasswordEventHandler(
    object sender,
    ValidatePasswordEventArgs e);
}
