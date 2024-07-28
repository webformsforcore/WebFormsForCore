
using System.Globalization;
using System.Reflection;
using System.Security;
using System.Security.Permissions;

#nullable disable
namespace System
{
	internal static class SecurityUtils
	{
		private static volatile ReflectionPermission memberAccessPermission;
		private static volatile ReflectionPermission restrictedMemberAccessPermission;

		private static ReflectionPermission MemberAccessPermission
		{
			get
			{
				if (SecurityUtils.memberAccessPermission == null)
					SecurityUtils.memberAccessPermission = new ReflectionPermission(ReflectionPermissionFlag.MemberAccess);
				return SecurityUtils.memberAccessPermission;
			}
		}

		private static ReflectionPermission RestrictedMemberAccessPermission
		{
			get
			{
				if (SecurityUtils.restrictedMemberAccessPermission == null)
					SecurityUtils.restrictedMemberAccessPermission = new ReflectionPermission(ReflectionPermissionFlag.RestrictedMemberAccess);
				return SecurityUtils.restrictedMemberAccessPermission;
			}
		}

		private static void DemandReflectionAccess(Type type)
		{
			try
			{
				SecurityUtils.MemberAccessPermission.Demand();
			}
			catch (SecurityException ex)
			{
				SecurityUtils.DemandGrantSet(type.Assembly);
			}
		}

		[SecuritySafeCritical]
		private static void DemandGrantSet(Assembly assembly)
		{
#if NETFRAMEWORK
			PermissionSet permissionSet = assembly.PermissionSet;
			permissionSet.AddPermission((IPermission)SecurityUtils.RestrictedMemberAccessPermission);
			permissionSet.Demand();
#endif
		}

		private static bool HasReflectionPermission(Type type)
		{
			try
			{
				SecurityUtils.DemandReflectionAccess(type);
				return true;
			}
			catch (SecurityException ex)
			{
			}
			return false;
		}

		internal static object SecureCreateInstance(Type type)
		{
			return SecurityUtils.SecureCreateInstance(type, (object[])null, false);
		}

		internal static object SecureCreateInstance(Type type, object[] args, bool allowNonPublic)
		{
			if (type == (Type)null)
				throw new ArgumentNullException(nameof(type));
			BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.CreateInstance;
			if (!type.IsVisible)
				SecurityUtils.DemandReflectionAccess(type);
			else if (allowNonPublic && !SecurityUtils.HasReflectionPermission(type))
				allowNonPublic = false;
			if (allowNonPublic)
				bindingAttr |= BindingFlags.NonPublic;
			return Activator.CreateInstance(type, bindingAttr, (Binder)null, args, (CultureInfo)null);
		}

		internal static object SecureCreateInstance(Type type, object[] args)
		{
			return SecurityUtils.SecureCreateInstance(type, args, false);
		}

		internal static object SecureConstructorInvoke(
		  Type type,
		  Type[] argTypes,
		  object[] args,
		  bool allowNonPublic)
		{
			return SecurityUtils.SecureConstructorInvoke(type, argTypes, args, allowNonPublic, BindingFlags.Default);
		}

		internal static object SecureConstructorInvoke(
		  Type type,
		  Type[] argTypes,
		  object[] args,
		  bool allowNonPublic,
		  BindingFlags extraFlags)
		{
			if (type == (Type)null)
				throw new ArgumentNullException(nameof(type));
			if (!type.IsVisible)
				SecurityUtils.DemandReflectionAccess(type);
			else if (allowNonPublic && !SecurityUtils.HasReflectionPermission(type))
				allowNonPublic = false;
			BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | extraFlags;
			if (!allowNonPublic)
				bindingAttr &= ~BindingFlags.NonPublic;
			ConstructorInfo constructor = type.GetConstructor(bindingAttr, (Binder)null, argTypes, (ParameterModifier[])null);
			return constructor != (ConstructorInfo)null ? constructor.Invoke(args) : (object)null;
		}

		private static bool GenericArgumentsAreVisible(MethodInfo method)
		{
			if (method.IsGenericMethod)
			{
				foreach (Type genericArgument in method.GetGenericArguments())
				{
					if (!genericArgument.IsVisible)
						return false;
				}
			}
			return true;
		}

		internal static object FieldInfoGetValue(FieldInfo field, object target)
		{
			Type declaringType = field.DeclaringType;
			if (declaringType == (Type)null)
			{
				if (!field.IsPublic)
					SecurityUtils.DemandGrantSet(field.Module.Assembly);
			}
			else if (!(declaringType != (Type)null) || !declaringType.IsVisible || !field.IsPublic)
				SecurityUtils.DemandReflectionAccess(declaringType);
			return field.GetValue(target);
		}

		internal static object MethodInfoInvoke(MethodInfo method, object target, object[] args)
		{
			Type declaringType = method.DeclaringType;
			if (declaringType == (Type)null)
			{
				if (!method.IsPublic || !SecurityUtils.GenericArgumentsAreVisible(method))
					SecurityUtils.DemandGrantSet(method.Module.Assembly);
			}
			else if (!declaringType.IsVisible || !method.IsPublic || !SecurityUtils.GenericArgumentsAreVisible(method))
				SecurityUtils.DemandReflectionAccess(declaringType);
			return method.Invoke(target, args);
		}

		internal static object ConstructorInfoInvoke(ConstructorInfo ctor, object[] args)
		{
			Type declaringType = ctor.DeclaringType;
			if (declaringType != (Type)null && (!declaringType.IsVisible || !ctor.IsPublic))
				SecurityUtils.DemandReflectionAccess(declaringType);
			return ctor.Invoke(args);
		}

		internal static object ArrayCreateInstance(Type type, int length)
		{
			if (!type.IsVisible)
				SecurityUtils.DemandReflectionAccess(type);
			return (object)Array.CreateInstance(type, length);
		}
	}
}
