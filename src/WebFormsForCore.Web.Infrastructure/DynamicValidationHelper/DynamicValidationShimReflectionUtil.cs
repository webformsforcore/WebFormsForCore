using System;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Security;
using System.Web;

namespace Microsoft.Web.Infrastructure.DynamicValidationHelper;

[SecurityCritical]
internal sealed class DynamicValidationShimReflectionUtil
{
	public static readonly DynamicValidationShimReflectionUtil Instance = DynamicValidationShimReflectionUtil.GetInstance();

	public Action<HttpContext> EnableDynamicValidation { get; private set; }

	public GetUnvalidatedCollectionsCallback GetUnvalidatedCollections { get; private set; }

	public Func<HttpContext, bool> IsValidationEnabled { get; private set; }

	private DynamicValidationShimReflectionUtil()
	{
	}

	[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "We catch and ignore all errors because we do not want to bring down the application.")]
	private static DynamicValidationShimReflectionUtil GetInstance()
	{
		try
		{
			DynamicValidationShimReflectionUtil instance = new DynamicValidationShimReflectionUtil();
			Type type = CommonAssemblies.SystemWeb.GetType("Microsoft.Web.Infrastructure.DynamicValidationHelper.DynamicValidationShim", false);
			if (type == (Type)null)
				return (DynamicValidationShimReflectionUtil)null;
			MethodInfo method1 = CommonReflectionUtil.FindMethod(type, "EnableDynamicValidation", true, new Type[1]
			{
		typeof (HttpContext)
			}, typeof(void));
			instance.EnableDynamicValidation = CommonReflectionUtil.MakeDelegate<Action<HttpContext>>(method1);
			CommonReflectionUtil.Assert(instance.EnableDynamicValidation != null);
			MethodInfo method2 = CommonReflectionUtil.FindMethod(type, "IsValidationEnabled", true, new Type[1]
			{
		typeof (HttpContext)
			}, typeof(bool));
			instance.IsValidationEnabled = CommonReflectionUtil.MakeDelegate<Func<HttpContext, bool>>(method2);
			CommonReflectionUtil.Assert(instance.IsValidationEnabled != null);
			MethodInfo method3 = CommonReflectionUtil.FindMethod(type, "GetUnvalidatedCollections", true, new Type[3]
			{
		typeof (HttpContext),
		typeof (Func<NameValueCollection>).MakeByRefType(),
		typeof (Func<NameValueCollection>).MakeByRefType()
			}, typeof(void));
			instance.GetUnvalidatedCollections = CommonReflectionUtil.MakeDelegate<GetUnvalidatedCollectionsCallback>(method3);
			CommonReflectionUtil.Assert(instance.GetUnvalidatedCollections != null);
			return instance;
		}
		catch
		{
			return (DynamicValidationShimReflectionUtil)null;
		}
	}
}
