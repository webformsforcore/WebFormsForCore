using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Emit;
using System.Security;
using System.Security.Permissions;

namespace Microsoft.Web.Infrastructure;

[SecurityCritical]
internal static class CommonReflectionUtil
{
	public static void Assert(bool b)
	{
		if (!b)
			throw new PlatformNotSupportedException();
	}

	public static ConstructorInfo FindConstructor(Type type, bool isStatic, Type[] argumentTypes)
	{
		ConstructorInfo constructor = type.GetConstructor(CommonReflectionUtil.GetBindingFlags(isStatic), (Binder)null, argumentTypes, (ParameterModifier[])null);
		CommonReflectionUtil.Assert(constructor != (ConstructorInfo)null);
		return constructor;
	}

	public static FieldInfo FindField(
	  Type containingType,
	  string fieldName,
	  bool isStatic,
	  Type fieldType)
	{
		FieldInfo field = containingType.GetField(fieldName, CommonReflectionUtil.GetBindingFlags(isStatic));
		CommonReflectionUtil.Assert(field.FieldType == fieldType);
		return field;
	}

	public static MethodInfo FindMethod(
	  Type containingType,
	  string methodName,
	  bool isStatic,
	  Type[] argumentTypes,
	  Type returnType)
	{
		MethodInfo method = containingType.GetMethod(methodName, CommonReflectionUtil.GetBindingFlags(isStatic), (Binder)null, argumentTypes, (ParameterModifier[])null);
		CommonReflectionUtil.Assert(method.ReturnType == returnType);
		return method;
	}

	private static BindingFlags GetBindingFlags(bool isStatic)
	{
		return (BindingFlags)((isStatic ? 8 : 4) | 32 /*0x20*/ | 16 /*0x10*/);
	}

	public static T MakeDelegate<T>(MethodInfo method) where T : class
	{
		return CommonReflectionUtil.MakeDelegate<T>((object)null, method);
	}

	public static T MakeDelegate<T>(object target, MethodInfo method) where T : class
	{
		return CommonReflectionUtil.MakeDelegate(typeof(T), target, method) as T;
	}

	[SuppressMessage("Microsoft.Security", "CA2106:SecureAsserts", Justification = "This is contained within a fully critical type, and we carefully control the callers.")]
	[ReflectionPermission(SecurityAction.Assert, MemberAccess = true)]
	public static Delegate MakeDelegate(Type delegateType, object target, MethodInfo method)
	{
		return Delegate.CreateDelegate(delegateType, target, method);
	}

	[SuppressMessage("Microsoft.Security", "CA2106:SecureAsserts", Justification = "This is contained within a fully critical type, and we carefully control the callers.")]
	[ReflectionPermission(SecurityAction.Assert, MemberAccess = true)]
	public static object ReadField(FieldInfo fieldInfo, object target) => fieldInfo.GetValue(target);

	[SuppressMessage("Microsoft.Security", "CA2106:SecureAsserts", Justification = "This is contained within a fully critical type, and we carefully control the callers.")]
	[ReflectionPermission(SecurityAction.Assert, MemberAccess = true)]
	public static void WriteField(FieldInfo fieldInfo, object target, object value)
	{
		fieldInfo.SetValue(target, value);
	}

	[ReflectionPermission(SecurityAction.Assert, MemberAccess = true)]
	public static Func<TInstance, TDelegate> MakeFastCreateDelegate<TInstance, TDelegate>(
	  MethodInfo methodInfo)
	  where TInstance : class
	  where TDelegate : class
	{
		DynamicMethod dynamicMethod = new DynamicMethod("FastCreateDelegate_" + methodInfo.Name, typeof(TDelegate), new Type[1]
		{
	  typeof (TInstance)
		}, true);
		ConstructorInfo constructor = typeof(TDelegate).GetConstructor(new Type[2]
		{
	  typeof (object),
	  typeof (IntPtr)
		});
		ILGenerator ilGenerator = dynamicMethod.GetILGenerator();
		ilGenerator.Emit(OpCodes.Ldarg_0);
		ilGenerator.Emit(OpCodes.Dup);
		ilGenerator.Emit(OpCodes.Ldvirtftn, methodInfo);
		ilGenerator.Emit(OpCodes.Newobj, constructor);
		ilGenerator.Emit(OpCodes.Ret);
		return (Func<TInstance, TDelegate>)dynamicMethod.CreateDelegate(typeof(Func<TInstance, TDelegate>));
	}

	[ReflectionPermission(SecurityAction.Assert, MemberAccess = true)]
	public static TDelegate BindMethodToDelegate<TDelegate>(MethodInfo methodInfo) where TDelegate : class
	{
		Type[] argumentTypes;
		Type returnType;
		CommonReflectionUtil.ExtractDelegateSignature(typeof(TDelegate), out argumentTypes, out returnType);
		DynamicMethod dynamicMethod = new DynamicMethod("BindMethodToDelegate_" + methodInfo.Name, returnType, argumentTypes, true);
		ILGenerator ilGenerator = dynamicMethod.GetILGenerator();
		for (int index = 0; index < argumentTypes.Length; ++index)
			ilGenerator.Emit(OpCodes.Ldarg, (short)index);
		ilGenerator.Emit(OpCodes.Callvirt, methodInfo);
		ilGenerator.Emit(OpCodes.Ret);
		return dynamicMethod.CreateDelegate(typeof(TDelegate)) as TDelegate;
	}

	[ReflectionPermission(SecurityAction.Assert, MemberAccess = true)]
	public static TDelegate MakeFastNewObject<TDelegate>(Type type) where TDelegate : class
	{
		Type[] argumentTypes;
		Type returnType;
		CommonReflectionUtil.ExtractDelegateSignature(typeof(TDelegate), out argumentTypes, out returnType);
		ConstructorInfo constructor = CommonReflectionUtil.FindConstructor(type, false, argumentTypes);
		DynamicMethod dynamicMethod = new DynamicMethod("MakeFastNewObject_" + type.Name, returnType, argumentTypes, true);
		ILGenerator ilGenerator = dynamicMethod.GetILGenerator();
		for (int index = 0; index < argumentTypes.Length; ++index)
			ilGenerator.Emit(OpCodes.Ldarg, (short)index);
		ilGenerator.Emit(OpCodes.Newobj, constructor);
		ilGenerator.Emit(OpCodes.Ret);
		return dynamicMethod.CreateDelegate(typeof(TDelegate)) as TDelegate;
	}

	private static void ExtractDelegateSignature(
	  Type delegateType,
	  out Type[] argumentTypes,
	  out Type returnType)
	{
		MethodInfo method = delegateType.GetMethod("Invoke", BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
		argumentTypes = Array.ConvertAll<ParameterInfo, Type>(method.GetParameters(), (Converter<ParameterInfo, Type>)(pInfo => pInfo.ParameterType));
		returnType = method.ReturnType;
	}
}
