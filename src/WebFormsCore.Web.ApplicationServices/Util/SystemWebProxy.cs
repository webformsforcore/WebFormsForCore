// Decompiled with JetBrains decompiler
// Type: System.Web.Util.SystemWebProxy
// Assembly: System.Web.ApplicationServices, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 49FC561C-A827-422E-A5C7-EDE4066C7817
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System.Web.ApplicationServices\v4.0_4.0.0.0__31bf3856ad364e35\System.Web.ApplicationServices.dll
// XML documentation location: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1\System.Web.ApplicationServices.xml

using System.Security.Permissions;
using System.Web.Security;

#nullable disable
namespace System.Web.Util
{
  internal static class SystemWebProxy
  {
    public static readonly IMembershipAdapter Membership = SystemWebProxy.GetMembershipAdapter();

    private static IMembershipAdapter GetMembershipAdapter()
    {
      return SystemWebProxy.CreateSystemWebMembershipAdapter() ?? (IMembershipAdapter) new DefaultMembershipAdapter();
    }

    private static IMembershipAdapter CreateSystemWebMembershipAdapter()
    {
      Type type = Type.GetType("System.Web.Security.MembershipAdapter, System.Web, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", false);
      return type != (Type) null ? (IMembershipAdapter) SystemWebProxy.DangerousCreateInstance(type) : (IMembershipAdapter) null;
    }

    [ReflectionPermission(SecurityAction.Assert, Flags = ReflectionPermissionFlag.MemberAccess)]
    private static object DangerousCreateInstance(Type type) => Activator.CreateInstance(type);
  }
}
