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
			return SystemWebProxy.CreateSystemWebMembershipAdapter() ?? (IMembershipAdapter)new DefaultMembershipAdapter();
		}

		private static IMembershipAdapter CreateSystemWebMembershipAdapter()
		{
#if NETFRAMEWORK
            Type type = Type.GetType("System.Web.Security.MembershipAdapter, System.Web, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", false);
#else
			Type type = Type.GetType("System.Web.Security.MembershipAdapter, EstrellasDeEsperanza.WebFormsForCore.Web", false);
#endif
			return type != (Type)null ? (IMembershipAdapter)SystemWebProxy.DangerousCreateInstance(type) : (IMembershipAdapter)null;
		}

		[ReflectionPermission(SecurityAction.Assert, Flags = ReflectionPermissionFlag.MemberAccess)]
		private static object DangerousCreateInstance(Type type) => Activator.CreateInstance(type);
	}
}
