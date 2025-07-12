using System;
using System.Collections;
using System.Collections.Generic;
using System.Security;
using System.Threading;
using System.Web;

namespace Microsoft.Web.Infrastructure.DynamicModuleHelper;

internal sealed class IntegratedDynamicModule : IHttpModule
{
	internal const string ModuleName = "__ASP_IntegratedDynamicModule_Shim";

	private IntegratedDynamicModule()
	{
	}

	public void Dispose()
	{
	}

	[SecuritySafeCritical]
	public void Init(HttpApplication context)
	{
		IntegratedDynamicModule.CriticalStatics.Init(context);
	}

	[SecurityCritical]
	internal static class CriticalStatics
	{
		internal static readonly List<DynamicModuleRegistryEntry> DynamicEntries = new List<DynamicModuleRegistryEntry>();
		internal static int _hasBeenInitialized = 0;
		internal static readonly DynamicModuleReflectionUtil _reflectionUtil = DynamicModuleReflectionUtil.Instance;

		public static void Init(HttpApplication context)
		{
			if (Interlocked.Exchange(ref IntegratedDynamicModule.CriticalStatics._hasBeenInitialized, 1) == 1 || IntegratedDynamicModule.CriticalStatics.DynamicEntries.Count == 0 || IntegratedDynamicModule.CriticalStatics._reflectionUtil == null)
				return;
			IntPtr integratedModeContext = IntegratedDynamicModule.CriticalStatics._reflectionUtil.GetIntegratedModeContext();
			if (integratedModeContext == IntPtr.Zero)
				return;
			IntegratedDynamicModule.CriticalStatics._reflectionUtil.SetModuleConfigInfo((IList)null);
			HttpModuleCollection moduleCollection1 = IntegratedDynamicModule.CriticalStatics._reflectionUtil.GetIntegratedModuleCollection(context, integratedModeContext);
			IList moduleConfigInfo = IntegratedDynamicModule.CriticalStatics._reflectionUtil.GetModuleConfigInfo();
			moduleConfigInfo.Insert(0, IntegratedDynamicModule.CriticalStatics._reflectionUtil.NewModuleConfigurationInfo("__ASP_IntegratedDynamicModule_Shim", typeof(IntegratedDynamicModule).AssemblyQualifiedName, "managedHandler"));
			foreach (DynamicModuleRegistryEntry dynamicEntry in IntegratedDynamicModule.CriticalStatics.DynamicEntries)
				moduleConfigInfo.Add(IntegratedDynamicModule.CriticalStatics._reflectionUtil.NewModuleConfigurationInfo(dynamicEntry.Name, dynamicEntry.Type, "managedHandler"));
			HttpModuleCollection moduleCollection2 = IntegratedDynamicModule.CriticalStatics._reflectionUtil.GetRegisteredModuleCollection(context);
			for (int index = 0; index < moduleCollection1.Count; ++index)
				IntegratedDynamicModule.CriticalStatics._reflectionUtil.AddModuleToCollection(moduleCollection2, moduleCollection1.GetKey(index), moduleCollection1.Get(index));
			IList moduleList = IntegratedDynamicModule.CriticalStatics._reflectionUtil.NewListOfModuleConfigurationInfo();
			for (int index = moduleConfigInfo.Count - IntegratedDynamicModule.CriticalStatics.DynamicEntries.Count; index < moduleConfigInfo.Count; ++index)
				moduleList.Add(moduleConfigInfo[index]);
			HttpModuleCollection moduleCollection3 = IntegratedDynamicModule.CriticalStatics._reflectionUtil.BuildIntegratedModuleCollection(context, moduleList);
			for (int index = 0; index < moduleCollection3.Count; ++index)
				IntegratedDynamicModule.CriticalStatics._reflectionUtil.AddModuleToCollection(moduleCollection2, moduleCollection3.GetKey(index), moduleCollection3.Get(index));
		}
	}
}
