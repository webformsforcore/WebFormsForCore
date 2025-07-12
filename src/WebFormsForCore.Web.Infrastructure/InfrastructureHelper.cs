using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Security;
using System.Web;

namespace Microsoft.Web.Infrastructure;

[EditorBrowsable(EditorBrowsableState.Never)]
public static class InfrastructureHelper
{
	[SecuritySafeCritical]
	public static void UnloadAppDomain()
	{
		if (AppDomain.CurrentDomain.IsHomogenous && AppDomain.CurrentDomain.IsFullyTrusted)
		{
			HttpRuntime.UnloadAppDomain();
		}
		else
		{
			InfrastructureHelper.HttpRuntimeReflectionUtil instance = InfrastructureHelper.HttpRuntimeReflectionUtil.Instance;
			if (instance == null)
				return;
			instance.SetUserForcedShutdown();
			int num = instance.ShutdownAppDomain(ApplicationShutdownReason.UnloadAppDomainCalled, "User code called UnloadAppDomain") ? 1 : 0;
		}
	}

	[SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands", Justification = "IsDefinedExtension doesn't do anything dangerous.")]
	[SecuritySafeCritical]
	public static bool IsCodeDomDefinedExtension(string extension)
	{
		return CodeDomProvider.IsDefinedExtension(extension);
	}

	[SecurityCritical]
	private sealed class HttpRuntimeReflectionUtil
	{
		public static readonly InfrastructureHelper.HttpRuntimeReflectionUtil Instance = InfrastructureHelper.HttpRuntimeReflectionUtil.GetInstance();

		public Action SetUserForcedShutdown { get; private set; }

		public Func<ApplicationShutdownReason, string, bool> ShutdownAppDomain { get; private set; }

		private HttpRuntimeReflectionUtil()
		{
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "We catch and ignore all errors because we do not want to bring down the application.")]
		private static InfrastructureHelper.HttpRuntimeReflectionUtil GetInstance()
		{
			try
			{
				InfrastructureHelper.HttpRuntimeReflectionUtil instance = new InfrastructureHelper.HttpRuntimeReflectionUtil();
				MethodInfo method1 = CommonReflectionUtil.FindMethod(typeof(HttpRuntime), "SetUserForcedShutdown", true, Type.EmptyTypes, typeof(void));
				instance.SetUserForcedShutdown = CommonReflectionUtil.MakeDelegate<Action>(method1);
				CommonReflectionUtil.Assert(instance.SetUserForcedShutdown != null);
				MethodInfo method2 = CommonReflectionUtil.FindMethod(typeof(HttpRuntime), "ShutdownAppDomain", true, new Type[2]
				{
		  typeof (ApplicationShutdownReason),
		  typeof (string)
				}, typeof(bool));
				instance.ShutdownAppDomain = CommonReflectionUtil.MakeDelegate<Func<ApplicationShutdownReason, string, bool>>(method2);
				CommonReflectionUtil.Assert(instance.ShutdownAppDomain != null);
				return instance;
			}
			catch
			{
				return (InfrastructureHelper.HttpRuntimeReflectionUtil)null;
			}
		}
	}
}
