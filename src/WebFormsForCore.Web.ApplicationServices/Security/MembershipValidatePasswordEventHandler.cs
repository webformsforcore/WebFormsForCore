// Decompiled with JetBrains decompiler
// Type: System.Web.Security.MembershipValidatePasswordEventHandler
// Assembly: System.Web.ApplicationServices, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 49FC561C-A827-422E-A5C7-EDE4066C7817
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System.Web.ApplicationServices\v4.0_4.0.0.0__31bf3856ad364e35\System.Web.ApplicationServices.dll
// XML documentation location: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1\System.Web.ApplicationServices.xml

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
