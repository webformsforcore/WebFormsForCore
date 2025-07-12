using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Security;
using System.Web;
using System.Web.Configuration;

namespace Microsoft.Web.Infrastructure.DynamicModuleHelper;

[EditorBrowsable(EditorBrowsableState.Never)]
public static class DynamicModuleUtility
{
	[SecuritySafeCritical]
	public static void RegisterModule(Type moduleType)
	{
		if (DynamicModuleReflectionUtil.Fx45RegisterModuleDelegate != null)
			DynamicModuleReflectionUtil.Fx45RegisterModuleDelegate(moduleType);
		else
			DynamicModuleUtility.LegacyModuleRegistrar.RegisterModule(moduleType);
	}

	[SecurityCritical]
	private static class LegacyModuleRegistrar
	{
		private static bool _integratedPipelineInitialized;
		private static readonly object _lockObj = new object();
		private const string _moduleNameFormat = "__DynamicModule_{0}_{1}";
		private static readonly DynamicModuleReflectionUtil _reflectionUtil = DynamicModuleReflectionUtil.Instance;

		public static void RegisterModule(Type moduleType)
		{
			DynamicModuleUtility.LegacyModuleRegistrar.VerifyParameters(moduleType);
			if (DynamicModuleUtility.LegacyModuleRegistrar._reflectionUtil == null)
				return;
			lock (DynamicModuleUtility.LegacyModuleRegistrar._lockObj)
			{
				DynamicModuleUtility.LegacyModuleRegistrar._reflectionUtil.ThrowIfPreAppStartNotRunning();
				DynamicModuleUtility.LegacyModuleRegistrar.AddModuleToClassicPipeline(moduleType);
				DynamicModuleUtility.LegacyModuleRegistrar.AddModuleToIntegratedPipeline(moduleType);
			}
		}

		private static void AddModuleToClassicPipeline(Type moduleType)
		{
			HttpModulesSection httpModulesSection = (HttpModulesSection)null;
			try
			{
				object target = DynamicModuleUtility.LegacyModuleRegistrar._reflectionUtil.GetAppConfig();
				httpModulesSection = DynamicModuleUtility.LegacyModuleRegistrar._reflectionUtil.GetHttpModulesFromAppConfig(target);
				DynamicModuleUtility.LegacyModuleRegistrar._reflectionUtil.SetConfigurationElementCollectionReadOnlyBit((ConfigurationElementCollection)httpModulesSection.Modules, false);
				DynamicModuleRegistryEntry moduleRegistryEntry = DynamicModuleUtility.LegacyModuleRegistrar.CreateDynamicModuleRegistryEntry(moduleType);
				httpModulesSection.Modules.Add(new HttpModuleAction(moduleRegistryEntry.Name, moduleRegistryEntry.Type));
			}
			finally
			{
				if (httpModulesSection != null)
					DynamicModuleUtility.LegacyModuleRegistrar._reflectionUtil.SetConfigurationElementCollectionReadOnlyBit((ConfigurationElementCollection)httpModulesSection.Modules, true);
			}
		}

		private static void AddModuleToIntegratedPipeline(Type moduleType)
		{
			if (!DynamicModuleUtility.LegacyModuleRegistrar._integratedPipelineInitialized)
			{
				DynamicModuleUtility.LegacyModuleRegistrar._integratedPipelineInitialized = true;
				DynamicModuleUtility.LegacyModuleRegistrar.InitializeIntegratedPipeline();
			}
			DynamicModuleRegistryEntry moduleRegistryEntry = DynamicModuleUtility.LegacyModuleRegistrar.CreateDynamicModuleRegistryEntry(moduleType);
			IntegratedDynamicModule.CriticalStatics.DynamicEntries.Add(moduleRegistryEntry);
		}

		private static DynamicModuleRegistryEntry CreateDynamicModuleRegistryEntry(Type moduleType)
		{
			string assemblyQualifiedName = moduleType.AssemblyQualifiedName;
			return new DynamicModuleRegistryEntry(string.Format((IFormatProvider)CultureInfo.InvariantCulture, "__DynamicModule_{0}_{1}", new object[2]
			{
		(object) assemblyQualifiedName,
		(object) Guid.NewGuid()
			}), assemblyQualifiedName);
		}

		private static void InitializeIntegratedPipeline()
		{
			IList list = DynamicModuleUtility.LegacyModuleRegistrar._reflectionUtil.NewListOfModuleConfigurationInfo();
			object obj = DynamicModuleUtility.LegacyModuleRegistrar._reflectionUtil.NewModuleConfigurationInfo("__ASP_IntegratedDynamicModule_Shim", typeof(IntegratedDynamicModule).AssemblyQualifiedName, "managedHandler");
			list.Add(obj);
			DynamicModuleUtility.LegacyModuleRegistrar._reflectionUtil.SetModuleConfigInfo(list);
		}

		[SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly", Justification = "We want to avoid having resource strings in MWI.dll so that we don't have to worry about localization.")]
		private static void VerifyParameters(Type moduleType)
		{
			if (moduleType == (Type)null)
				throw new ArgumentNullException(nameof(moduleType));
			if (!typeof(IHttpModule).IsAssignableFrom(moduleType))
				throw new ArgumentException(nameof(moduleType));
		}
	}
}
