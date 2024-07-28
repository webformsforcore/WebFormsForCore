
using System.Security.Permissions;

#nullable disable
namespace System.Configuration
{
	internal static class TypeUtil
	{
		[ReflectionPermission(SecurityAction.Assert, Flags = ReflectionPermissionFlag.MemberAccess)]
		internal static object CreateInstanceWithReflectionPermission(string typeString)
		{
			return Activator.CreateInstance(Type.GetType(typeString, true), true);
		}
		[ReflectionPermission(SecurityAction.Assert, Flags = ReflectionPermissionFlag.MemberAccess)]
		internal static object CreateInstanceWithReflectionPermission(Type type)
		{
			return Activator.CreateInstance(type, true);
		}
	}
}
