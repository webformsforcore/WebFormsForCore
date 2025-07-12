using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Web;
using System.Web.Compilation;
using System.Web.Configuration;

namespace Microsoft.Web.Infrastructure.DynamicModuleHelper;

[SecurityCritical]
internal sealed class DynamicModuleReflectionUtil
{
	public static readonly Action<Type> Fx45RegisterModuleDelegate = DynamicModuleReflectionUtil.GetFx45RegisterModuleDelegate();
	private MethodInfo _mi_RuntimeConfig_getHttpModules;
	private FieldInfo _fi_ConfigurationElementCollection_bReadOnly;
	private ConstructorInfo _ci_ModuleConfigurationInfo;
	private ConstructorInfo _ci_ListOfModuleConfigurationInfo;
	private FieldInfo _fi_HttpApplication_moduleConfigInfo;
	private FieldInfo _fi_HttpApplication_moduleCollection;
	private FieldInfo _fi_PipelineRuntime_s_ApplicationContext;
	private MethodInfo _mi_HttpApplication_GetModuleCollection;
	private MethodInfo _mi_HttpApplication_BuildIntegratedModuleCollection;
	private Type _type_ListOfModuleConfigurationInfo;
	private MethodInfo _mi_HttpModuleCollection_AddModule;
	public static readonly DynamicModuleReflectionUtil Instance = DynamicModuleReflectionUtil.GetInstance();

	private static Action<Type> GetFx45RegisterModuleDelegate()
	{
		MethodInfo method = typeof(HttpApplication).GetMethod("RegisterModule", BindingFlags.Static | BindingFlags.Public, (Binder)null, new Type[1]
		{
	  typeof (Type)
		}, (ParameterModifier[])null);
		return !(method != (MethodInfo)null) ? (Action<Type>)null : (Action<Type>)Delegate.CreateDelegate(typeof(Action<Type>), method);
	}

	public Action ThrowIfPreAppStartNotRunning { get; private set; }

	public Func<object> GetAppConfig { get; private set; }

	public HttpModulesSection GetHttpModulesFromAppConfig(object target)
	{
		return CommonReflectionUtil.MakeDelegate<Func<HttpModulesSection>>(target, this._mi_RuntimeConfig_getHttpModules)();
	}

	public void SetConfigurationElementCollectionReadOnlyBit(
	  ConfigurationElementCollection target,
	  bool value)
	{
		CommonReflectionUtil.WriteField(this._fi_ConfigurationElementCollection_bReadOnly, (object)target, (object)value);
	}

	[SuppressMessage("Microsoft.Security", "CA2106:SecureAsserts", Justification = "This is contained within a fully critical type, and we carefully control the callers.")]
	[ReflectionPermission(SecurityAction.Assert, MemberAccess = true)]
	public object NewModuleConfigurationInfo(string name, string type, string condition)
	{
		return this._ci_ModuleConfigurationInfo.Invoke(new object[3]
		{
	  (object) name,
	  (object) type,
	  (object) condition
		});
	}

	[SuppressMessage("Microsoft.Security", "CA2106:SecureAsserts", Justification = "This is contained within a fully critical type, and we carefully control the callers.")]
	[ReflectionPermission(SecurityAction.Assert, MemberAccess = true)]
	public IList NewListOfModuleConfigurationInfo()
	{
		return (IList)this._ci_ListOfModuleConfigurationInfo.Invoke((object[])null);
	}

	public IList GetModuleConfigInfo()
	{
		return (IList)CommonReflectionUtil.ReadField(this._fi_HttpApplication_moduleConfigInfo, (object)null);
	}

	public void SetModuleConfigInfo(IList value)
	{
		CommonReflectionUtil.WriteField(this._fi_HttpApplication_moduleConfigInfo, (object)null, (object)value);
	}

	public HttpModuleCollection GetRegisteredModuleCollection(HttpApplication target)
	{
		return (HttpModuleCollection)CommonReflectionUtil.ReadField(this._fi_HttpApplication_moduleCollection, (object)target);
	}

	public IntPtr GetIntegratedModeContext()
	{
		return (IntPtr)CommonReflectionUtil.ReadField(this._fi_PipelineRuntime_s_ApplicationContext, (object)null);
	}

	public HttpModuleCollection GetIntegratedModuleCollection(
	  HttpApplication target,
	  IntPtr appContext)
	{
		return CommonReflectionUtil.MakeDelegate<Func<IntPtr, HttpModuleCollection>>((object)target, this._mi_HttpApplication_GetModuleCollection)(appContext);
	}

	public HttpModuleCollection BuildIntegratedModuleCollection(
	  HttpApplication target,
	  IList moduleList)
	{
		return (HttpModuleCollection)CommonReflectionUtil.MakeDelegate(typeof(Func<,>).MakeGenericType(this._type_ListOfModuleConfigurationInfo, typeof(HttpModuleCollection)), (object)target, this._mi_HttpApplication_BuildIntegratedModuleCollection).DynamicInvoke((object)moduleList);
	}

	public void AddModuleToCollection(HttpModuleCollection target, string name, IHttpModule m)
	{
		CommonReflectionUtil.MakeDelegate<Action<string, IHttpModule>>((object)target, this._mi_HttpModuleCollection_AddModule)(name, m);
	}

	private DynamicModuleReflectionUtil()
	{
	}

	[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "We catch and ignore all errors because we do not want to bring down the application.")]
	private static DynamicModuleReflectionUtil GetInstance()
	{
		try
		{
			if (DynamicModuleReflectionUtil.Fx45RegisterModuleDelegate != null)
				return (DynamicModuleReflectionUtil)null;
			DynamicModuleReflectionUtil instance = new DynamicModuleReflectionUtil();
			MethodInfo method1 = typeof(BuildManager).GetMethod("ThrowIfPreAppStartNotRunning", BindingFlags.Static | BindingFlags.NonPublic, (Binder)null, Type.EmptyTypes, (ParameterModifier[])null);
			instance.ThrowIfPreAppStartNotRunning = CommonReflectionUtil.MakeDelegate<Action>(method1);
			CommonReflectionUtil.Assert(instance.ThrowIfPreAppStartNotRunning != null);
			Type type1 = CommonAssemblies.SystemWeb.GetType("System.Web.Configuration.RuntimeConfig");
			MethodInfo method2 = type1.GetMethod("GetAppConfig", BindingFlags.Static | BindingFlags.NonPublic, (Binder)null, Type.EmptyTypes, (ParameterModifier[])null);
			instance.GetAppConfig = CommonReflectionUtil.MakeDelegate<Func<object>>(method2);
			CommonReflectionUtil.Assert(instance.GetAppConfig != null);
			instance._mi_RuntimeConfig_getHttpModules = CommonReflectionUtil.FindMethod(type1, "get_HttpModules", false, Type.EmptyTypes, typeof(HttpModulesSection));
			instance._fi_ConfigurationElementCollection_bReadOnly = CommonReflectionUtil.FindField(typeof(ConfigurationElementCollection), "bReadOnly", false, typeof(bool));
			Type type2 = CommonAssemblies.SystemWeb.GetType("System.Web.ModuleConfigurationInfo");
			instance._ci_ModuleConfigurationInfo = CommonReflectionUtil.FindConstructor(type2, false, new Type[3]
			{
		typeof (string),
		typeof (string),
		typeof (string)
			});
			Type type3 = typeof(List<>).MakeGenericType(type2);
			instance._type_ListOfModuleConfigurationInfo = type3;
			instance._ci_ListOfModuleConfigurationInfo = CommonReflectionUtil.FindConstructor(type3, false, Type.EmptyTypes);
			instance._fi_HttpApplication_moduleConfigInfo = CommonReflectionUtil.FindField(typeof(HttpApplication), "_moduleConfigInfo", true, type3);
			instance._fi_HttpApplication_moduleCollection = CommonReflectionUtil.FindField(typeof(HttpApplication), "_moduleCollection", false, typeof(HttpModuleCollection));
			Type type4 = CommonAssemblies.SystemWeb.GetType("System.Web.Hosting.PipelineRuntime");
			instance._fi_PipelineRuntime_s_ApplicationContext = CommonReflectionUtil.FindField(type4, "s_ApplicationContext", true, typeof(IntPtr));
			instance._mi_HttpApplication_GetModuleCollection = CommonReflectionUtil.FindMethod(typeof(HttpApplication), "GetModuleCollection", false, new Type[1]
			{
		typeof (IntPtr)
			}, typeof(HttpModuleCollection));
			instance._mi_HttpApplication_BuildIntegratedModuleCollection = CommonReflectionUtil.FindMethod(typeof(HttpApplication), "BuildIntegratedModuleCollection", false, new Type[1]
			{
		type3
			}, typeof(HttpModuleCollection));
			instance._mi_HttpModuleCollection_AddModule = CommonReflectionUtil.FindMethod(typeof(HttpModuleCollection), "AddModule", false, new Type[2]
			{
		typeof (string),
		typeof (IHttpModule)
			}, typeof(void));
			return instance;
		}
		catch
		{
			return (DynamicModuleReflectionUtil)null;
		}
	}
}
